// DOM
const selectWarehouse = document.getElementById('selectWarehouse');
const formWarehouse = document.getElementById('formWarehouse');
const warehouseIdInput = document.getElementById('WarehouseId');
const warehouseNameInput = document.getElementById('WarehouseCode'); // depo adı

const selectShelf = document.getElementById('selectShelf');
const formShelf = document.getElementById('formStockLocation');
const shelfIdInput = document.getElementById('stockLocationId');
const shelfCodeInput = document.getElementById('stockLocationShelfCode');

let warehousesCache = []; // [{id,name}]

// init select2
function initSelect2(el, placeholder) {
    $(el).select2({
        placeholder,
        allowClear: true,
        width: '100%',
        dropdownParent: $('#editTableModal')
    });
}

function unwrapList(json) {
    if (json && Array.isArray(json.data)) return json.data; // OperationResult
    if (Array.isArray(json)) return json;
    return [];
}


async function refreshMainLookupShelves() {
    // Ana ekrandaki "Yeni Kayıt" modalında raf listesi güncel kalsın diye
    try {
        const res = await fetch('/api/lookup');
        if (!res.ok) return;
        const data = await res.json();
        window.lookupShelves = data.shelves || [];
        window.lookupWarehouses = data.warehouses || [];
    } catch { /* ignore */ }
}

async function readErrorMessage(res) {
    try {
        const j = await res.json();
        return j?.message || j?.error || (j?.errors ? JSON.stringify(j.errors) : null);
    } catch {
        try { return await res.text(); } catch { return null; }
    }
}

function clearShelfForm() {
    shelfIdInput.value = '';
    shelfCodeInput.value = '';
    $(selectShelf).val(null).trigger('change');
}

function clearShelfSelect() {
    $(selectShelf).empty().select2({
        placeholder: "Raf seçin...",
        allowClear: true,
        width: '100%',
        dropdownParent: $('#editTableModal')
    });
}

function clearWarehouseForm() {
    warehouseIdInput.value = '';
    warehouseNameInput.value = '';
    $(selectWarehouse).val(null).trigger('change');

    clearShelfForm();
    clearShelfSelect();
}

// --------------------------
// LOAD WAREHOUSES
// --------------------------
async function loadWarehouses() {
    const res = await fetch('/api/Warehouse');
    if (!res.ok) throw new Error(await readErrorMessage(res) || "Depolar alınamadı.");

    const json = await res.json();
    const list = unwrapList(json);

    warehousesCache = list.map(w => ({
        id: w.id ?? w.Id,
        name: w.name ?? w.Name
    }));

    const options = warehousesCache.map(w => ({ id: w.id, text: w.name }));

    $(selectWarehouse).empty().select2({
        data: options,
        placeholder: "Depo seçin...",
        allowClear: true,
        width: '100%',
        dropdownParent: $('#editTableModal')
    });
}

// --------------------------
// LOAD SHELVES BY WAREHOUSE (SERVER SIDE)
// --------------------------
async function loadShelvesByWarehouse(warehouseId) {
    if (!warehouseId) {
        clearShelfSelect();
        return;
    }

    const res = await fetch(`/api/Shelf/by-warehouse/${warehouseId}`);
    if (!res.ok) throw new Error(await readErrorMessage(res) || "Raflar alınamadı.");

    const json = await res.json();
    const list = unwrapList(json);

    const options = list.map(s => ({
        id: s.id ?? s.Id,
        text: s.shelfCode ?? s.ShelfCode
    }));

    $(selectShelf).empty().select2({
        data: options,
        placeholder: "Raf seçin...",
        allowClear: true,
        width: '100%',
        dropdownParent: $('#editTableModal')
    });

    clearShelfForm();
}

// --------------------------
// INIT TAB
// --------------------------
async function initWarehouseShelfTab() {
    initSelect2(selectWarehouse, "Depo seçin...");
    initSelect2(selectShelf, "Raf seçin...");

    await loadWarehouses();

    if (warehousesCache.length === 0) {
        clearWarehouseForm();
        return;
    }

    const first = warehousesCache[0];
    warehouseIdInput.value = first.id;
    warehouseNameInput.value = first.name;
    $(selectWarehouse).val(first.id).trigger('change.select2');

    await loadShelvesByWarehouse(first.id);
}

// --------------------------
// EVENTS
// --------------------------
$(selectWarehouse).off('select2:select').on('select2:select', async function (e) {
    const data = e.params.data; // {id,text}
    warehouseIdInput.value = data.id;
    warehouseNameInput.value = data.text;

    clearShelfForm();
    await loadShelvesByWarehouse(data.id);
});

$(selectShelf).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    shelfIdInput.value = data.id;
    shelfCodeInput.value = data.text;
});

$('button[data-bs-toggle="tab"]').on('shown.bs.tab', async function (e) {
    const target = $(e.target).attr('data-bs-target');
    if (target === '#tabStockLocation') {
        try {
            await initWarehouseShelfTab();
        } catch (err) {
            await Swal.fire({ icon: 'error', title: 'Hata', text: err.message });
        }
    }
});

// --------------------------
// CRUD: SHELF (save/delete) -> refresh current warehouse shelves
// --------------------------
async function saveShelf(e) {
    e.preventDefault();

    const warehouseId = parseInt(warehouseIdInput.value || "0", 10);
    if (!warehouseId) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: "Önce bir depo seçin." });
        return;
    }

    const id = parseInt(shelfIdInput.value || "0", 10);
    const shelfCode = shelfCodeInput.value.trim();

    if (!shelfCode) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: "Raf kodu boş olamaz." });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/Shelf/${id}` : `/api/Shelf`;
    const payload = id ? { id, warehouseId, shelfCode } : { warehouseId, shelfCode };

    const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (!res.ok) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: await readErrorMessage(res) || "Raf kaydedilemedi." });
        return;
    }

    await Swal.fire({ icon: 'success', title: 'Başarılı', text: "Raf kaydedildi." });
    clearShelfForm();
    await loadShelvesByWarehouse(warehouseId);
    await refreshMainLookupShelves();
}

async function deleteShelf() {
    const id = parseInt(shelfIdInput.value || "0", 10);
    if (!id) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: "Önce bir raf seçin." });
        return;
    }

    const confirmResult = await Swal.fire({
        icon: 'question',
        title: 'Onay',
        text: 'Bu rafı silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet',
        cancelButtonText: 'Hayır'
    });

    if (!confirmResult.isConfirmed) return;

    const res = await fetch(`/api/Shelf/${id}`, { method: 'DELETE' });

    if (!res.ok) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: await readErrorMessage(res) || "Raf silinemedi." });
        return;
    }

    await Swal.fire({ icon: 'success', title: 'Başarılı', text: "Raf silindi." });

    const warehouseId = parseInt(warehouseIdInput.value || "0", 10);
    clearShelfForm();
    await loadShelvesByWarehouse(warehouseId);
}


async function saveWarehouse(e) {
    e.preventDefault();
    e.stopPropagation();

    const id = parseInt(warehouseIdInput.value || "0", 10);
    const name = warehouseNameInput.value.trim();

    if (!name) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: "Depo kodu/adı boş olamaz." });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/Warehouse/${id}` : `/api/Warehouse`;
    const payload = id ? { id, name } : { name };

    const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (!res.ok) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: await readErrorMessage(res) || "Depo kaydedilemedi." });
        return;
    }

    // Eğer backend OperationResult dönüyorsa success kontrolü (sağlam olsun)
    let json = null;
    try { json = await res.json(); } catch { /* bazı delete/put 204 dönebilir */ }

    if (json && json.success === false) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: json.errorMessage || "Depo kaydedilemedi." });
        return;
    }

    await Swal.fire({ icon: 'success', title: 'Başarılı', text: "Depo kaydedildi." });

    // yeniden yükle + seçimi güncelle
    await loadWarehouses();

    // Kaydettiğin depoyu select’te seç (id varsa id, yoksa isimle bul)
    if (id) {
        $(selectWarehouse).val(String(id)).trigger('change.select2');
    } else {
        // yeni eklenende id’yi listeden bul
        const found = warehousesCache.find(w => (w.name || "").toLowerCase() === name.toLowerCase());
        if (found) {
            warehouseIdInput.value = found.id;
            $(selectWarehouse).val(String(found.id)).trigger('change.select2');
            await loadShelvesByWarehouse(found.id);
        }
    }

    await refreshMainLookupShelves();
}
formWarehouse.addEventListener("submit", saveWarehouse);

document.getElementById("btnNewWarehouse").addEventListener("click", () => {
    warehouseIdInput.value = "";          // id boş -> POST
    warehouseNameInput.value = "";
    $(selectWarehouse).val(null).trigger("change");
    clearShelfForm();
    clearShelfSelect();
});

async function deleteWarehouse() {
    const id = parseInt(warehouseIdInput.value || "0", 10);
    if (!id) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: "Önce bir depo seçin." });
        return;
    }

    const confirmResult = await Swal.fire({
        icon: 'question',
        title: 'Onay',
        text: 'Bu depoyu silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet',
        cancelButtonText: 'Hayır'
    });
    if (!confirmResult.isConfirmed) return;

    const res = await fetch(`/api/Warehouse/${id}`, { method: 'DELETE' });

    if (!res.ok) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: await readErrorMessage(res) || "Depo silinemedi." });
        return;
    }

    // OperationResult döndürüyorsa success kontrolü
    let json = null;
    try { json = await res.json(); } catch { }
    if (json && json.success === false) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: json.errorMessage || "Depo silinemedi." });
        return;
    }

    await Swal.fire({ icon: 'success', title: 'Başarılı', text: "Depo silindi." });

    await loadWarehouses();
    clearWarehouseForm();
    await refreshMainLookupShelves();
}

document.getElementById("btnDeleteWarehouse").addEventListener("click", deleteWarehouse);


// form submit bind
formShelf.addEventListener("submit", saveShelf);
document.getElementById("btnDeleteStockLocation").addEventListener("click", deleteShelf);
document.getElementById("btnNewStockLocation").addEventListener("click", clearShelfForm);

// Warehouse CRUD'larını senin mevcut JS'inden aynı şekilde tutabilirsin;
// sadece raf tarafında artık loadShelvesByWarehouse(...) çağır.
