const editTableButton = document.getElementById("edit-table-btn");
const selectTransmissionBrand = document.getElementById('selectTransmissionBrand');
const formTransmissionBrand = document.getElementById('formTransmissionBrand');
const transmissionBrandId = document.getElementById('transmissionBrandId');
const transmissionBrandName = document.getElementById('transmissionBrandName');

document.getElementById("btnNewTransmissionBrand").addEventListener("click", clearForm);
document.getElementById("btnDeleteTransmissionBrand").addEventListener("click", deleteBrand);
formTransmissionBrand.addEventListener("submit", saveBrand);

function clearForm() {
    transmissionBrandId.value = '';
    transmissionBrandName.value = '';
    $(selectTransmissionBrand).val(null).trigger('change');
}

async function deleteBrand() {
    const id = transmissionBrandId.value;
    if (!id) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Önce bir kayıt seçin.'
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
        const res = await fetch(`/api/TransmissionBrand/${id}`, { method: 'DELETE' });
        if (res.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Silindi.'
            });
            clearForm();
            updateSelect2List();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Silme işlemi başarısız.'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Silme sırasında hata oluştu: ' + error.message
        });
    }
}

async function saveBrand(e) {
    e.preventDefault();

    const id = transmissionBrandId.value;
    const name = transmissionBrandName.value.trim();

    if (!name) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Marka adı boş olamaz.'
        });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/TransmissionBrand/${id}` : '/api/TransmissionBrand';

    try {
        const res = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, name })
        });

        if (res.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Kaydedildi.'
            });
            clearForm();
            updateSelect2List();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Hata',
                text: 'Kaydetme işlemi başarısız.'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Kaydetme sırasında hata oluştu: ' + error.message
        });
    }
}

async function updateSelect2List() {
    try {
        const res = await fetch('/api/TransmissionBrand');
        if (!res.ok) throw new Error('Veri alınamadı: ' + res.statusText);

        const result = await res.json();
        if (!result.success) {
            throw new Error(result.errorMessage || 'Veri alınamadı');
        }

        const options = result.data.map(b => ({ id: b.id, text: b.name }));
        if ($(selectTransmissionBrand).hasClass('select2-hidden-accessible')) {
            $(selectTransmissionBrand).select2('destroy');
        }
        $(selectTransmissionBrand).empty().select2({
            data: options,
            placeholder: "Şanzıman markası seçin...",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#editTableModal')
        });

        if (options.length > 0) {
            $(selectTransmissionBrand).val(options[0].id).trigger('change');
            transmissionBrandId.value = options[0].id;
            transmissionBrandName.value = options[0].text;
        } else {
            transmissionBrandId.value = '';
            transmissionBrandName.value = '';
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Markalar yüklenirken hata oluştu: ' + error.message
        });
    }
}

$(selectTransmissionBrand).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    transmissionBrandId.value = data.id;
    transmissionBrandName.value = data.text;
});

editTableButton.addEventListener("click", async function () {
    new bootstrap.Modal(document.getElementById("editTableModal")).show();
    await updateSelect2List();
});
