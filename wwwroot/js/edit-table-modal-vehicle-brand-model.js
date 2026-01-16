// HTML elemanları
const selectVehicleBrand = document.getElementById('selectVehicleBrand');
const selectVehicleModel = document.getElementById('selectVehicleModel');

const formVehicleBrand = document.getElementById('formVehicleBrand');
const vehicleBrandId = document.getElementById('vehicleBrandId');
const vehicleBrandName = document.getElementById('vehicleBrandName');

const formVehicleModel = document.getElementById('formVehicleModel');
const vehicleModelId = document.getElementById('vehicleModelId');
const vehicleModelName = document.getElementById('vehicleModelName');


async function readOpResult(res) {
    let json = null;
    try { json = await res.json(); } catch { /* ignore */ }
    return { ok: res.ok, json };
}

function getOpError(json) {
    return json?.errorMessage || json?.message || json?.title || "İşlem başarısız.";
}

document.getElementById("btnNewVehicleBrand").addEventListener("click", clearBrandForm);
document.getElementById("btnDeleteVehicleBrand").addEventListener("click", deleteBrand);
document.getElementById("btnDeleteVehicleModel").addEventListener("click", deleteModel);

formVehicleBrand.addEventListener("submit", saveBrand);
formVehicleModel.addEventListener("submit", saveModel);

function clearBrandForm() {
    vehicleBrandId.value = '';
    vehicleBrandName.value = '';
    $(selectVehicleBrand).val(null).trigger('change');
    clearModelForm();
    clearModelSelect();
}

function clearModelForm() {
    vehicleModelId.value = '';
    vehicleModelName.value = '';
    $(selectVehicleModel).val(null).trigger('change');
}

function clearModelSelect() {
    $(selectVehicleModel).empty().select2({
        placeholder: "Model seçin...",
        allowClear: true,
        width: '100%',
        dropdownParent: $('#editTableModal')
    });
}

async function deleteBrand() {
    const id = vehicleBrandId.value;
    if (!id) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: "Önce bir marka seçin."
        });
        return;
    }

    const confirmResult = await Swal.fire({
        icon: 'question',
        title: 'Onay',
        text: 'Bu markayı silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet',
        cancelButtonText: 'Hayır'
    });

    if (!confirmResult.isConfirmed) return;

    try {
        const res = await fetch(`/api/VehicleBrand/${id}`, { method: 'DELETE' });
        const { json } = await readOpResult(res);
        if (json?.success) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: "Marka silindi."
            });
            clearBrandForm();
            updateVehicleBrandSelect();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: getOpError(json)
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: "Silme sırasında hata oluştu: " + error.message
        });
    }
}

async function saveBrand(e) {
    e.preventDefault();
    const id = vehicleBrandId.value;
    const name = vehicleBrandName.value.trim();

    if (!name) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: "Marka adı boş olamaz."
        });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/VehicleBrand/${id}` : `/api/VehicleBrand`;

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, name, vehicleBrandId: id })
        });

        const { json } = await readOpResult(res);
        if (json?.success) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: "Model kaydedildi."
            });
            clearModelForm();
            updateVehicleModelSelect(id);
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: getOpError(json)
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: "Model kaydetme sırasında hata oluştu: " + error.message
        });
    }
}



async function deleteModel() {
    const id = (vehicleModelId.value || "").trim();
    const brandId = $(selectVehicleBrand).val();

    if (!id) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: 'Silmek için bir model seçin.' });
        return;
    }

    const confirmRes = await Swal.fire({
        icon: 'warning',
        title: 'Silme Onayı',
        text: 'Bu araç modelini silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet, sil',
        cancelButtonText: 'Vazgeç'
    });
    if (!confirmRes.isConfirmed) return;

    try {
        const res = await fetch(`/api/VehicleModel/${id}`, { method: 'DELETE' });
        const { json } = await readOpResult(res);

        if (json?.success) {
            await Swal.fire({ icon: 'success', title: 'Başarılı', text: 'Model silindi.' });
            clearModelForm();
            await updateVehicleModelSelect(brandId);
        } else {
            await Swal.fire({ icon: 'error', title: 'Hata', text: getOpError(json) || 'Silme işlemi başarısız.' });
        }
    } catch (error) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: 'Silme sırasında hata oluştu: ' + error.message });
    }
}

async function saveModel(e) {
    e.preventDefault();

    const id = (vehicleModelId.value || "").trim();
    const name = (vehicleModelName.value || "").trim();
    const brandId = $(selectVehicleBrand).val();

    if (!brandId) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: 'Önce bir araç markası seçin.' });
        return;
    }
    if (!name) {
        await Swal.fire({ icon: 'warning', title: 'Uyarı', text: 'Model adı boş olamaz.' });
        return;
    }

    const isUpdate = !!id;
    const url = isUpdate ? `/api/VehicleModel/${id}` : `/api/VehicleModel`;
    const method = isUpdate ? 'PUT' : 'POST';

    // Backend DTO: Create { name, vehicleBrandId } / Update { id, name, vehicleBrandId }
    const body = isUpdate
        ? { id: Number(id), name, vehicleBrandId: Number(brandId) }
        : { name, vehicleBrandId: Number(brandId) };

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        const { json } = await readOpResult(res);

        if (json?.success) {
            await Swal.fire({ icon: 'success', title: 'Başarılı', text: isUpdate ? 'Model güncellendi.' : 'Model eklendi.' });
            clearModelForm();
            await updateVehicleModelSelect(brandId);
        } else {
            await Swal.fire({ icon: 'error', title: 'Hata', text: getOpError(json) || 'Model kaydedilemedi.' });
        }
    } catch (error) {
        await Swal.fire({ icon: 'error', title: 'Hata', text: 'Kayıt sırasında hata oluştu: ' + error.message });
    }
}

async function updateVehicleBrandSelect() {
    try {
        const res = await fetch('/api/VehicleBrand');
        if (!res.ok) throw new Error('Veri alınamadı: ' + res.statusText);
        const data = await res.json();
        if (!data?.success) throw new Error(getOpError(data));
        const options = (data.data || []).map(b => ({ id: b.id, text: b.name }));

        $(selectVehicleBrand).empty().select2({
            data: options,
            placeholder: "Araç markası seçin...",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#editTableModal')
        });

        if ((data.data || []).length > 0) {
            const first = (data.data || [])[0];
            $(selectVehicleBrand).val(first.id).trigger('change');
            vehicleBrandId.value = first.id;
            vehicleBrandName.value = first.name;
            updateVehicleModelSelect(first.id);

        } else {
            clearBrandForm();
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Araç markaları yüklenirken hata oluştu: ' + error.message
        });
    }
}

async function updateVehicleModelSelect(brandId) {
    if (!brandId) {
        clearModelSelect();
        return;
    }

    try {
        const res = await fetch(`/api/VehicleModel/by-brand/${brandId}`);
        if (!res.ok) throw new Error('Veri alınamadı: ' + res.statusText);
        const data = await res.json();
        if (!data?.success) throw new Error(getOpError(data));
        const filtered = (data.data || []).filter(m => m.vehicleBrandId == brandId);
        const options = filtered.map(m => ({ id: m.id, text: m.name }));

        $(selectVehicleModel).empty().select2({
            data: options,
            placeholder: "Model seçin...",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#editTableModal')
        });

        clearModelForm();
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Araç modelleri yüklenirken hata oluştu: ' + error.message
        });
    }
}

// Marka seçilince form ve model select güncellenir
$(selectVehicleBrand).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    vehicleBrandId.value = data.id;
    vehicleBrandName.value = data.text;
    clearModelForm();
    updateVehicleModelSelect(data.id);
});

// Model seçilince form doldurulur
$(selectVehicleModel).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    vehicleModelId.value = data.id;
    vehicleModelName.value = data.text;
});

$('button[data-bs-toggle="tab"]').on('shown.bs.tab', async function (e) {
    const target = $(e.target).attr('data-bs-target');
    if (target === '#tabVehicleBrandModel') {
        await updateVehicleBrandSelect();
    }
});
