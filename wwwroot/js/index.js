const generalSearchInput = document.getElementById("generalSearch");
const table = document.querySelector("table.table");
const totalAdetCell = document.getElementById("total-adet");
const durumAdetCell = document.getElementById("durum-adet");
const thElements = Array.from(table.querySelectorAll("thead th"));
const filters = new Map();
var recordModal = document.getElementById('recordModal');
const addNewBtn = document.getElementById("add-new-btn");
const printTableBtn = document.getElementById("printTableBtn");
const shelfRowsContainer = document.getElementById('shelfRowsContainer');
const btnAddShelfRow = document.getElementById('btnAddShelfRow');

const ACTION_COL_INDEX = 12;

//create
const saveButton = document.getElementById("saveButton");
const sparePartNoInput = document.getElementById("sparePartNo");
const transmissionBrandSelect = document.getElementById("transmissionBrand");
const transmissionCodeInput = document.getElementById("transmissionCode");
const vehicleBrandSelect = document.getElementById("vehicleBrand");
const vehicleModelSelect = document.getElementById("vehicleModel");
const vehicleYearInput = document.getElementById("vehicleYear");
const transmissionNoInput = document.getElementById("transmissionNo");
const driveTypeInput = document.getElementById("driveType");
const transmissionStatusSelect = document.getElementById("transmissionStatus");
const descriptionInput = document.getElementById("description");
window.isEditMode = false;

//delete
let deleteId = null;
const confirmDeleteButton = document.getElementById("confirmDeleteBtn");


function openDeleteModal() {
    new bootstrap.Modal(document.getElementById("deleteModal")).show();
}

let selectsPopulated = false;
async function ensureSelectsReady() {
    if (!selectsPopulated) {
        await populateSelectFields();
        selectsPopulated = true;
    }
}

function fillStockLocationRows(shelves = []) {
    // Yeni sistem: #shelfRowsContainer içinde .stock-shelf-row (select2 ile)
    shelfRowsContainer.innerHTML = "";

    (shelves || []).forEach(sh => {
        addShelfRow(sh.id, sh.quantity);
    });

    if (!shelves || shelves.length === 0) {
        addShelfRow(null, 1);
    }
}

function getRecordModalInstance() {
    const el = document.getElementById("recordModal");
    let instance = bootstrap.Modal.getInstance(el);
    if (!instance) {
        instance = new bootstrap.Modal(el);
    }
    return instance;
}


async function openEditModal(row) {
    // Bu fonksiyon artık sadece geri uyumluluk için var.
    // Asıl edit akışı: editStock(id) -> API'dan çek -> openEditModalFromData(stock)
    const id = row?.getAttribute?.("data-id") || row?.dataset?.id;
    if (id) {
        await editStock(id);
    }
}

function openEditModalFromData(stock) {
    const form = document.getElementById("recordForm");

    // Lokasyon satırlarını doldur
    if (typeof fillStockLocationRows === 'function') {
        fillStockLocationRows(stock.shelves || []);
    }

    // Inputlar
    form.querySelector("#sparePartNo").value = stock.sparePartNo || "";
    form.querySelector("#transmissionCode").value = stock.transmissionCode || "";
    form.querySelector("#vehicleYear").value = stock.year || "";
    form.querySelector("#transmissionNo").value = stock.transmissionNumber || "";
    form.querySelector("#description").value = stock.description || "";

    // Select2’ler
    $("#transmissionBrand").val(stock.transmissionBrandId).trigger("change");
    $("#vehicleBrand").val(stock.vehicleBrandId).trigger("change");
    $("#vehicleModel").val(stock.vehicleModelId).trigger("change");
    $("#driveType").val(stock.driveTypeId).trigger("change");
    $("#transmissionStatus").val(stock.transmissionStatusId).trigger("change");

    // Gizli alanlar ve başlık
    form.querySelector("#rowIndex").value = ""; // tabloya bağlı değiliz artık
    form.querySelector("#stockId").value = stock.id;
    document.getElementById("recordModalLabel").textContent = "Kayıt Düzenle";

    window.isEditMode = true;

    // Modalı aç
    const modal = getRecordModalInstance();
    modal.show();
}


//function editStock(id) {
//    const row = document.querySelector(`#stockTable tbody tr[data-id="${id}"]`);
//    if (row) {
//        openEditModal(row);
//    }
//}

async function editStock(id) {
    try {
        const res = await fetch(`/api/TransmissionStock/${id}`);
        const result = await res.json();

        if (!res.ok || !result?.success || !result.data) {
            const msg = (result && (result.errorMessage || result.title)) || 'Kayıt alınamadı.';
            await Swal.fire({ icon: 'error', title: 'Hata', text: msg });
            return;
        }

        const stock = result.data; // API’den gelen ViewModel
        await ensureSelectsReady();
        openEditModalFromData(stock); // aşağıda yazıyoruz
    } catch (err) {
        console.error(err);
        await Swal.fire({ icon: 'error', title: 'Sunucu Hatası', text: err.message });
    }
}

function deleteStock(id) {
    deleteId = id;
    openDeleteModal();
}

// --- Yardımcı fonksiyon: ilk harfi büyük yap ---
function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function updateSummary(data) {
    let toplamAdet = 0;
    const durumSayac = {};

    data.forEach(stock => {
        const adet = stock.totalQuantity || 0;
        toplamAdet += adet;

        const durum = (stock.transmissionStatusName || "").trim().toLowerCase();
        durumSayac[durum] = (durumSayac[durum] || 0) + adet;
    });

    if (totalAdetCell) totalAdetCell.textContent = `Toplam Şanzıman Adeti: ${toplamAdet}`;

    if (durumAdetCell) {
        const text = Object.entries(durumSayac)
            .map(([key, val]) => capitalize(key) + ": " + val)
            .join(" | ");
        durumAdetCell.textContent = text;
    }
}


function parseShelfSummary(summary) {
    const s = (summary || "").trim();
    if (!s) return "";

    // Beklenen format örn: "Depo1: A1(2), A2(1) | Depo2: B1(3)"
    const parts = s.split("|").map(x => x.trim()).filter(Boolean);
    const shelfParts = [];

    for (const p of parts) {
        const idx = p.indexOf(":");
        if (idx > -1) {
            const shelves = p.substring(idx + 1).trim();
            if (shelves) shelfParts.push(shelves);
        } else {
            // Format farklıysa: komple raf olarak göster
            shelfParts.push(p);
        }
    }

    return shelfParts.join(" | ");
}

function applyFilters() {
    const genelArama = generalSearchInput.value.trim().toUpperCase();

    filteredStocks = allStocks.filter(stock => {
        let matchesFilters = true;

        filters.forEach((filter, colIdx) => {
            let fieldValue = "";
            switch (colIdx) {
                case 0: fieldValue = stock.sparePartNo; break;
                case 1: fieldValue = stock.transmissionBrandName; break;
                case 2: fieldValue = stock.transmissionCode; break;
                case 3: fieldValue = stock.transmissionNumber; break;
                case 4: fieldValue = stock.transmissionStatusName; break;
                case 5: fieldValue = stock.driveTypeName || ""; break;
                case 6: fieldValue = stock.vehicleBrandName; break;
                case 7: fieldValue = stock.vehicleModelName; break;
                case 8: fieldValue = stock.year ? stock.year.toString() : ""; break;
                case 9: fieldValue = parseShelfSummary(stock.shelfSummary); break;
                case 10: fieldValue = stock.totalQuantity ? stock.totalQuantity.toString() : ""; break;
                case 11: fieldValue = stock.description; break;
                default: fieldValue = "";
            }

            if (!fieldValue || !fieldValue.toUpperCase().includes(filter.value)) {
                matchesFilters = false;
            }
        });

        if (genelArama) {
            const haystack = [
                stock.sparePartNo,
                stock.transmissionBrandName,
                stock.transmissionCode,
                stock.transmissionNumber,
                stock.transmissionStatusName,
                stock.driveTypeName || "",
                stock.vehicleBrandName,
                stock.vehicleModelName,
                stock.year ? stock.year.toString() : "",
                parseShelfSummary(stock.shelfSummary),
                stock.totalQuantity ? stock.totalQuantity.toString() : "",
                stock.description
            ].join(" ").toUpperCase();

            if (!haystack.includes(genelArama)) {
                matchesFilters = false;
            }
        }

        return matchesFilters;
    });

    currentPage = 1;
    renderPage(currentPage);
    updatePaginationControls();
    updateSummary(filteredStocks);
}

generalSearchInput.addEventListener("input", () => applyFilters());

let allStocks = [];
let filteredStocks = [];
let currentPage = 1;
const rowsPerPage = 10;

function renderPage(page) {
    const tbody = document.querySelector('#stockTable tbody');
    tbody.innerHTML = '';

    const start = (page - 1) * rowsPerPage;
    const end = start + rowsPerPage;
    const pageData = filteredStocks.slice(start, end);

    pageData.forEach(stock => {
        const tr = document.createElement('tr');
        tr.setAttribute("data-id", stock.id);
        tr.setAttribute("data-stock", JSON.stringify(stock));
        tr.innerHTML = `
            <td>${stock.sparePartNo ?? ''}</td>
            <td>${stock.transmissionBrandName ?? ''}</td>
            <td>${stock.transmissionCode ?? ''}</td>
            <td>${stock.transmissionNumber ?? ''}</td>
            <td>${stock.transmissionStatusName ?? ''}</td>
            <td>${stock.driveTypeName ?? ''}</td>
            <td>${stock.vehicleBrandName ?? ''}</td>
            <td>${stock.vehicleModelName ?? ''}</td>
            <td>${stock.year ?? ''}</td>
            <td>${parseShelfSummary(stock.shelfSummary)}</td>
            <td>${stock.totalQuantity ?? 0}</td>
            <td>${stock.description ?? ''}</td>
            <td class="text-center">
              <button class="btn btn-sm btn-primary" onclick="editStock(${stock.id})"><i class="bi bi-pencil"></i></button>
              <button class="btn btn-sm btn-danger" onclick="deleteStock(${stock.id})"><i class="bi bi-trash"></i></button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function updatePaginationControls() {
    const container = document.getElementById("paginationControls");
    container.innerHTML = "";

    const totalPages = Math.ceil(filteredStocks.length / rowsPerPage); 
    if (totalPages <= 1) return;

    if (currentPage > 1) {
        const prevBtn = document.createElement("button");
        prevBtn.className = "btn btn-sm btn-secondary me-1";
        prevBtn.textContent = "«";
        prevBtn.addEventListener("click", () => {
            currentPage--;
            renderPage(currentPage);
            updatePaginationControls();
        });
        container.appendChild(prevBtn);
    }

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement("button");
        btn.className = "btn btn-sm me-1 " + (i === currentPage ? "btn-primary" : "btn-outline-primary");
        btn.textContent = i;
        btn.addEventListener("click", () => {
            currentPage = i;
            renderPage(currentPage);
            updatePaginationControls();
        });
        container.appendChild(btn);
    }

    if (currentPage < totalPages) {
        const nextBtn = document.createElement("button");
        nextBtn.className = "btn btn-sm btn-secondary";
        nextBtn.textContent = "»";
        nextBtn.addEventListener("click", () => {
            currentPage++;
            renderPage(currentPage);
            updatePaginationControls();
        });
        container.appendChild(nextBtn);
    }
}

async function loadStockTable() {
    try {
        const response = await fetch('/api/TransmissionStock');
        if (!response.ok) {
            Swal.fire({
                icon: 'error',
                title: 'Hata!',
                text: 'Veri alınamadı: ' + response.statusText
            });
            return;
        }

        const result = await response.json();
        if (!result.success) {
            Swal.fire({
                icon: 'error',
                title: 'Hata!',
                text: 'API hata: ' + (result.errorMessage)
            });
            return;
        }

        allStocks = result.data || [];
        applyFilters();

    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: 'İstek hatası:' + error
        });
    }
}




// Modal açıldığında select2'yi init et
recordModal.addEventListener('shown.bs.modal', function () {

    // select2 init (doğru)
    $('#recordModal select.select2').each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            $(this).select2({
                dropdownParent: $('#recordModal'),
                width: '100%',
                theme: 'bootstrap-5'
            });
        }
    });

    // ❗ SADECE CREATE MOD
    if (!window.isEditMode) {
        resetShelfRows();
    }
});



// --- Satır çift tıklama ile düzenleme ---
document.addEventListener("dblclick", e => {
    const row = e.target.closest("tr");
    if (row && row.parentElement.tagName === "TBODY") {
        const id = row.getAttribute("data-id") || row.dataset.id;
        if (id) editStock(id);
    }
});

// --- Yeni kayıt butonu ---
addNewBtn.addEventListener("click", () => {
    const form = document.getElementById("recordForm");

    // Önce uyarıları ve select2 işaretlerini temizle
    resetValidationState();

    // Form alanlarını sıfırla
    form.reset();

    // gizli alan vs.
    const rowIndexInput = form.querySelector("#rowIndex");
    if (rowIndexInput) rowIndexInput.value = "";
    const stockIdInput = form.querySelector("#stockId");
    if (stockIdInput) stockIdInput.value = "";

    // select2’leri sıfırla
    $('#recordModal select.select2').val(null).trigger('change');

    // lokasyon satırlarını temiz başlat
    fillStockLocationRows([]);

    window.isEditMode = false;

    // başlığı ayarla
    document.getElementById("recordModalLabel").textContent = "Yeni Kayıt";

    // HEP AYNI modal instance kullan
    const modal = getRecordModalInstance();
    modal.show();
});




// --- Yazdırma işlemi ---
printTableBtn.addEventListener("click", () => {
    const clone = table.cloneNode(true);

    // Aksiyon sütunlarını kaldır
    clone.querySelectorAll("thead th:last-child, tbody td:last-child").forEach(el => el.remove());

    // Filtre ikonlarını ve inputları kaldır
    clone.querySelectorAll("thead th").forEach(th => {
        const spanText = th.querySelector("span")?.textContent || th.textContent;
        th.textContent = spanText.trim();
    });

    const printWindow = window.open("", "", "width=900,height=600");
    printWindow.document.write(`
      <html><head>
      <title>Yazdır</title>
      <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
      <style>table, th, td { border:1px solid #ccc; border-collapse:collapse; }</style>
      </head><body>
      <h4>Şanzıman Stok Listesi</h4>
      ${clone.outerHTML}
      <script>
        window.onload = function() {
          window.print();
          window.onafterprint = function() { window.close(); };
        };
      <\/script>
      </body></html>
    `);
    printWindow.document.close();
});

// --- Filtre ikonları ve inputları ekle ---
thElements.forEach((th, index) => {
    if (index === ACTION_COL_INDEX) return;

    const colName = th.textContent.trim();

    const container = document.createElement("div");
    container.className = "d-flex justify-content-between align-items-center";

    const span = document.createElement("span");
    span.textContent = colName;

    const icon = document.createElement("span");
    icon.className = "filter-icon";
    icon.title = "Filtrele";
    icon.style.cursor = "pointer";
    icon.innerHTML = "&#128269;";

    container.appendChild(span);

    const input = document.createElement("input");
    input.type = "text";
    input.classList.add("form-control", "form-control-sm", "mt-1", "filter-input");
    input.style.display = "none";
    input.placeholder = `Filtrele: ${colName}`;

    th.textContent = "";
    th.appendChild(container);
    th.appendChild(input);

    span.addEventListener("click", () => {
        input.style.display = input.style.display === "none" ? "block" : "none";
        if (input.style.display === "none") {
            input.value = "";
            filters.delete(index);
            applyFilters();
        }
        input.focus();
    });

    input.addEventListener("input", () => {
        const val = input.value.trim().toUpperCase();
        if (val) filters.set(index, { name: colName, value: val });
        else filters.delete(index);
        applyFilters();
    });

    input.addEventListener("focusout", () => {
        setTimeout(() => {
            if (!input.value.trim()) {
                input.style.display = "none";
            }
        }, 100);
    });

});


document.addEventListener("focusout", function (e) {
    const input = e.target;

    if (input.classList.contains("filter-input")) {
        setTimeout(() => {
            if (!input.value.trim()) {
                input.style.display = "none";
            }
        }, 100);
    }
});

let globalShelfCodes = [];
async function populateSelectFields() {
    try {
        const res = await fetch('/api/lookup');
        if (!res.ok) {
            Swal.fire({
                icon: 'error',
                title: 'Hata!',
                text: 'Veriler alınamadı'
            });
            return;
        }

        const data = await res.json();

        fillSelect('transmissionBrand', data.transmissionBrands, false, false, true);
        fillSelect('vehicleBrand', data.vehicleBrands, false, false, false);
        fillSelect('vehicleModel', data.vehicleModels, false, false, false);
        fillSelect('driveType', data.driveTypes, true, false, false);
        fillSelect('transmissionStatus', data.transmissionStatuses, false, false, true);

        window.lookupShelves = data.shelves;
        window.lookupWarehouses = data.warehouses;

        sessionStorage.setItem(
            "allVehicleModels",
            JSON.stringify(data.vehicleModels)
        );

    } catch (err) {
        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: 'Lookup verileri yüklenemedi: ' + err.message
        });
    }
}




function fillSelect(id, items, isValueName = false, isSimpleList = false, makeRequired = false) {
    const select = document.getElementById(id);
    select.innerHTML = '<option value="">Seçiniz</option>';

    if (makeRequired) {
        select.setAttribute('required', 'required');
    }

    items.forEach(item => {
        const opt = document.createElement('option');
        if (isSimpleList) {
            opt.value = item;
            opt.textContent = item;
        } else if (isValueName) {
            opt.value = item.id;
            opt.textContent = item.name;
        } else {
            opt.value = item.id;
            opt.textContent = item.name;
        }
        select.appendChild(opt);
    });
}


$('#vehicleBrand').on('change', function () {
    const markaId = parseInt(this.value);
    const allModels = JSON.parse(sessionStorage.getItem("allVehicleModels")) || [];

    const $modelSelect = $('#vehicleModel');
    $modelSelect.empty().append('<option value="">Seçiniz</option>');

    let filtered;
    if (isNaN(markaId)) {
        filtered = allModels;
    } else {
        filtered = allModels.filter(m => m.vehicleBrandId === markaId);
    }

    filtered.forEach(m => {
        $modelSelect.append(new Option(m.name, m.id));
    });

    $modelSelect.trigger('change.select2');
});

//function createStockLocationRow(shelf = "", quantity = "") {
//    const div = document.createElement("div");
//    div.className = "row align-items-center stock-location-row";

//    div.innerHTML = `
//        <div class="col-5">
//            <select class="form-select form-select-sm shelf-select" required>
//                <option value="">Seçiniz</option>
//            </select>
//        </div>
//        <div class="col-5">
//            <input type="number" min="0" step="1" class="form-control form-control-sm quantity-input" value="${quantity}" required />
//        </div>
//        <div class="col-2 text-end">
//            <button type="button" class="btn btn-sm btn-danger remove-row">–</button>
//        </div>
//    `;

//    const shelfSelect = div.querySelector(".shelf-select");
//    globalShelfCodes.forEach(code => {
//        const opt = document.createElement("option");
//        opt.value = code;
//        opt.textContent = code;
//        shelfSelect.appendChild(opt);
//    });

//    shelfSelect.value = shelf;

//    const qty = div.querySelector(".quantity-input");

//    // 1) Klavyeden “-”, “e”, “+” engelle
//    qty.addEventListener("keydown", (e) => {
//        if (["-", "e", "E", "+"].includes(e.key)) e.preventDefault();
//    });

//    // 2) Yapıştırmada/elle yazmada negatifi sıfıra çek
//    const clamp = () => {
//        let v = qty.value.trim();
//        if (v === "") return;                  // required zaten yakalar
//        let n = Number(v);
//        if (!Number.isFinite(n) || n < 0) n = 0;
//        // tam sayı zorunluluğu
//        n = Math.floor(n);
//        qty.value = String(n);
//    };
//    qty.addEventListener("input", clamp);
//    qty.addEventListener("blur", clamp);

//    // 3) Mouse tekerleğiyle negatif gitmesin (istenirse)
//    qty.addEventListener("wheel", (e) => {
//        // input focus’tayken wheel ile değişimi engelle
//        e.preventDefault();
//    }, { passive: false });

//    div.querySelector(".remove-row").addEventListener("click", () => div.remove());

//    return div;
//}




saveButton.addEventListener("click", async function (event) {
    event.preventDefault();

    // 1) Raf satırlarını oku (YENİ SİSTEM)
    const shelfRows = document.querySelectorAll("#shelfRowsContainer .stock-shelf-row");

    let shelvesRaw = Array.from(shelfRows).map(r => {
        const shelfIdStr = $(r).find(".shelfSelect").val(); // select2 -> jQuery val
        const qtyStr = r.querySelector(".shelfQty")?.value;

        return {
            shelfId: shelfIdStr ? Number(shelfIdStr) : null,
            quantity: qtyStr ? Number(qtyStr) : 0
        };
    });

    // 2) Normalize: aynı raf birden fazla seçildiyse adetleri topla
    const shelvesMap = new Map(); // shelfId -> quantity
    for (const s of shelvesRaw) {
        if (!s.shelfId) continue;
        const prev = shelvesMap.get(s.shelfId) || 0;
        shelvesMap.set(s.shelfId, prev + (Number.isFinite(s.quantity) ? s.quantity : 0));
    }

    const shelves = Array.from(shelvesMap.entries()).map(([shelfId, quantity]) => ({
        shelfId,
        quantity
    }));

    // 3) Raf zorunluluğu + validasyon (backend de istiyor)
    const invalidShelf = shelves.some(x => !x.shelfId || !Number.isFinite(x.quantity) || x.quantity <= 0);
    if (shelves.length === 0 || invalidShelf) {
        await Swal.fire({
            icon: "error",
            title: "Eksik Bilgi!",
            text: "Depo/Raf bilgisi zorunludur. Lütfen en az 1 raf seçin ve adeti 1'den büyük girin."
        });
        return;
    }

    // 4) Toplam miktar (normalize edilmiş raflar üzerinden)
    const totalQuantity = shelves.reduce((sum, x) => sum + (x.quantity || 0), 0);

    // 5) Zorunlu alan kontrolü (formu hedefle)
    const form = document.getElementById("recordForm"); // <-- recordModal değil
    const requiredFields = form.querySelectorAll("[required]");
    const missingFields = Array.from(requiredFields).filter(field => !field.value);

    requiredFields.forEach(field => {
        const isEmpty = !field.value.trim();
        field.classList.toggle("is-invalid", isEmpty);

        // Select2 için container'a da uygula
        if (field.classList.contains("select2-hidden-accessible")) {
            const select2Container = $(field).next(".select2-container");
            if (isEmpty) select2Container.addClass("is-invalid");
            else select2Container.removeClass("is-invalid");
        }
    });

    if (missingFields.length > 0) {
        await Swal.fire({
            icon: 'error',
            title: 'Eksik Bilgi!',
            text: 'Lütfen tüm zorunlu alanları doldurun.'
        });
        return;
    }

    // 4) Toplam 0 ise onay iste
    if (totalQuantity === 0) {
        const confirmRes = await Swal.fire({
            icon: 'warning',
            title: 'Stoğu sıfırlamak üzeresiniz',
            text: 'Bu ürünün toplam stok miktarı 0 olacak. Emin misiniz?',
            showCancelButton: true,
            confirmButtonText: 'Evet, sıfırla',
            cancelButtonText: 'Vazgeç'
        });
        if (!confirmRes.isConfirmed) return;
    }

    // 5) Payload (driveTypeValue gönderiyoruz; sayısalları number yapıyoruz)
    const sparePartNoInputVal = sparePartNoInput.value.trim();
    const data = {
        sparePartNo: sparePartNoInputVal === "" ? null : sparePartNoInputVal,
        transmissionBrandId: transmissionBrandSelect.value === "" ? null : Number(transmissionBrandSelect.value),
        transmissionCode: transmissionCodeInput.value === "" ? null : transmissionCodeInput.value,
        transmissionNumber: transmissionNoInput.value === "" ? null : transmissionNoInput.value,
        year: vehicleYearInput.value === "" ? null : Number(vehicleYearInput.value),
        vehicleBrandId: vehicleBrandSelect.value === "" ? null : Number(vehicleBrandSelect.value),
        vehicleModelId: vehicleModelSelect.value === "" ? null : Number(vehicleModelSelect.value),

        // ÖNEMLİ: backend DTO DriveTypeValue bekliyor
        driveTypeId: driveTypeInput.value === "" ? null : Number(driveTypeInput.value),

        transmissionStatusId: transmissionStatusSelect.value === "" ? null : Number(transmissionStatusSelect.value),
        description: descriptionInput.value,

        // 0 ise null DEĞİL, boş dizi gönderiyoruz
        shelves: shelves.map(x => ({ shelfId: x.shelfId, quantity: x.quantity })),

        // backend bunu kullanmıyor ama debug için kalabilir
        totalQuantity
    };

    const stockId = document.querySelector("#stockId").value;

    // ... payload hazırlamadan hemen önce ya da sonra:
    if (!stockId) { // YENİ KAYIT modundasın
        const preBody = {
            transmissionBrandId: Number(transmissionBrandSelect.value),
            sparePartNo: (sparePartNoInput.value || "").trim(),
            transmissionStatusId: Number(transmissionStatusSelect.value)
        };

        const preRes = await fetch('/api/TransmissionStock/check-duplicate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(preBody)
        });
        const preJson = await preRes.json();

        if (preRes.ok && preJson?.success && preJson.data?.exists) {
            const d = preJson.data;

            const choice = await Swal.fire({
                icon: 'warning',
                title: 'Bu kayıt zaten var!',
                html: `
                        <div style="text-align:left">
                          <div><b>Parça No:</b> ${d.sparePartNo}</div>
                          <div><b>Marka:</b> ${d.transmissionBrandName}</div>
                          <div><b>Durum:</b> ${d.transmissionStatusName}</div>
                          <div><b>Mevcut Adet:</b> ${d.totalQuantity}</div>
                          <div><b>Raflar:</b> ${d.shelfSummary || '-'}</div>
                          <hr/>
                          <div>Mevcut kaydı açıp düzenlemek ister misiniz?</div>
                        </div>
                      `,
                showCancelButton: true,
                confirmButtonText: 'Mevcut kaydı aç ve düzenle',
                cancelButtonText: 'İptal'
            });

            if (choice.isConfirmed) {
                await editStock(d.existingId); // Aşağıdaki fonksiyon
            }
            return; // YENİ kayıt gönderimini iptal et
        }
    }


    const method = stockId ? "PUT" : "POST";
    const url = stockId ? `/api/TransmissionStock/${stockId}` : `/api/TransmissionStock`;

    try {
        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (!response.ok) {
            const errorText = result.errorMessage || result.title || 'Beklenmeyen bir hata oluştu.';
            await Swal.fire({ icon: 'error', title: 'İşlem başarısız!', text: errorText });
            return;
        }

        if (!result.success) {
            await Swal.fire({ icon: 'warning', title: 'Uyarı!', text: result.errorMessage || 'İşlem gerçekleştirilemedi.' });
            return;
        }

        await Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: stockId ? 'Kayıt güncellendi.' : 'Yeni kayıt eklendi.',
            timer: 1500,
            showConfirmButton: false
        });

        const modal = bootstrap.Modal.getInstance(document.getElementById("recordModal"));
        modal.hide();

        await loadStockTable();

    } catch (err) {
        console.error("Hata:", err);
        await Swal.fire({
            icon: 'error',
            title: 'Sunucu Hatası!',
            text: 'Bir hata oluştu. Lütfen daha sonra tekrar deneyin.\n' + err.message
        });
    }
});



confirmDeleteButton.addEventListener("click", async function (event) {
    if (deleteId === null) return;

    try {
        const response = await fetch(`/api/TransmissionStock/${deleteId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error("Silme başarısız: " + response.statusText);
        }
        deleteId = null;
        await loadStockTable();
        const modal = bootstrap.Modal.getInstance(document.getElementById("deleteModal"));
        modal.hide();
    } catch (error) {
        console.error("Silme hatası:", error);
    }

});




// yardımcı: validasyon ve select2 uyarılarını temizle
function resetValidationState() {
    const form = document.getElementById("recordForm");
    if (!form) return;

    // elle eklediğin .is-invalid/.is-valid sınıflarını temizle
    form.querySelectorAll(".is-invalid, .is-valid").forEach(el => {
        el.classList.remove("is-invalid", "is-valid");
    });

    // select2 kırmızı çerçevesini temizle
    $('#recordModal .select2-container').removeClass('is-invalid');

    // HTML5/Bootstrap doğrulama kullanıyorsan
    form.classList.remove("was-validated");
}



recordModal.addEventListener('hidden.bs.modal', function () {
    const form = document.getElementById("recordForm");
    if (!form) return;

    // 1) Validation state temizle
    resetValidationState();

    // 2) Form resetle
    form.reset();

    // 3) Select2 resetle
    $('#recordModal select.select2').val(null).trigger('change');

    // 4) Lokasyon satırlarını temizle
    if (typeof fillStockLocationRows === 'function') {
        fillStockLocationRows([]);
    } else {
        const wrapper = document.getElementById("stockLocationsWrapper");
        if (wrapper) { wrapper.innerHTML = ""; }
    }
    window.isEditMode = false;

    // 4.1) Hidden stok id temizle
    const stockIdInput = form.querySelector("#stockId");
    if (stockIdInput) stockIdInput.value = "";

    // 5) Backdrop ve body cleanup (çift modal/backdrop bug’ını engellemek için)
    document.querySelectorAll(".modal-backdrop").forEach(el => el.remove());
    document.body.classList.remove("modal-open");
    document.body.style = "";
});


$('#driveType').on('change', function () {
    console.log(driveTypeInput.value);
});

btnAddShelfRow.addEventListener('click', () => addShelfRow());

function addShelfRow(selectedShelfId = null, quantity = 1) {
    const row = document.createElement('div');

    const isFirstRow = shelfRowsContainer.children.length === 0;

    row.className = 'row g-2 align-items-end stock-shelf-row mb-2';

    row.innerHTML = `
      <div class="col-md-4">
        ${isFirstRow ? `<label class="form-label mb-1">Depo</label>` : ``}
        <select class="form-select warehouseSelect">
          <option></option>
        </select>
      </div>

      <div class="col-md-4">
        ${isFirstRow ? `<label class="form-label mb-1">Raf</label>` : ``}
        <select class="form-select shelfSelect">
          <option></option>
        </select>
      </div>

      <div class="col-md-3">
        ${isFirstRow ? `<label class="form-label mb-1">Adet</label>` : ``}
        <input type="number" class="form-control shelfQty" min="1" value="${quantity}">
      </div>

      <div class="col-md-1 d-flex justify-content-end">
        ${isFirstRow ? `<label class="form-label mb-1 d-block">&nbsp;</label>` : ``}
        <button type="button" class="btn btn-danger btn-sm btnRemoveShelf">
          <i class="bi bi-x-lg"></i>
        </button>
      </div>
    `;

    shelfRowsContainer.appendChild(row);

    const warehouseEl = row.querySelector('.warehouseSelect');
    const shelfEl = row.querySelector('.shelfSelect');

    const warehouses = window.lookupWarehouses || [];
    const shelves = window.lookupShelves || [];

    const whOptions = warehouses.map(w => ({ id: w.id, text: w.name }));
    $(warehouseEl).select2({
        data: whOptions,
        placeholder: "Depo seçin...",
        allowClear: true,
        width: '100%',
        dropdownParent: $('#recordModal')
    });

    $(shelfEl).select2({
        data: [],
        placeholder: "Raf seçin...",
        allowClear: true,
        width: '100%',
        dropdownParent: $('#recordModal')
    });

    $(warehouseEl).on('change', function () {
        const whId = $(this).val() ? Number($(this).val()) : null;

        $(shelfEl).empty().trigger('change');
        if (!whId) return;

        const filtered = shelves
            .filter(s => s.warehouseId === whId)
            .map(s => ({ id: s.id, text: s.shelfCode }));

        // yeniden init yerine: option basıp trigger daha sağlıklı
        $(shelfEl).select2({
            data: filtered,
            placeholder: "Raf seçin...",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#recordModal')
        });
    });

    if (selectedShelfId) {
        const shelf = shelves.find(x => x.id === Number(selectedShelfId));
        if (shelf) {
            $(warehouseEl).val(String(shelf.warehouseId)).trigger('change');
            setTimeout(() => {
                $(shelfEl).val(String(shelf.id)).trigger('change');
            }, 0);
        }
    }

    row.querySelector('.btnRemoveShelf').addEventListener('click', () => row.remove());
}



function resetShelfRows() {
    shelfRowsContainer.innerHTML = '';
    addShelfRow(null, 1);
}

$.fn.select2.defaults.set('language', {
    noResults: function () {
        return "Sonuç bulunamadı";
    }
});

document.addEventListener("DOMContentLoaded", async () => {  
    await populateSelectFields();
    await loadStockTable();
    $(document).on('select2:open', () => {
        document.querySelector('.select2-container--open .select2-search__field')?.focus();
    });
});
