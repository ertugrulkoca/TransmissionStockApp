const selectTransmissionDriveType = document.getElementById('selectTransmissionDriveType');
const formTransmissionDriveType = document.getElementById('formTransmissionDriveType');
const transmissionDriveTypeId = document.getElementById('transmissionDriveTypeId');
const transmissionDriveTypeName = document.getElementById('transmissionDriveTypeName');
//const transmissionDriveTypeValue = document.getElementById('transmissionDriveTypeValue');

document.getElementById('btnNewTransmissionDriveType').addEventListener('click', clearTransmissionDriveTypeForm);
document.getElementById('btnDeleteTransmissionDriveType').addEventListener('click', deleteTransmissionDriveType);
formTransmissionDriveType.addEventListener('submit', saveTransmissionDriveType);

function clearTransmissionDriveTypeForm() {
    transmissionDriveTypeId.value = '';
    transmissionDriveTypeName.value = '';
    //transmissionDriveTypeValue.value = '';
    $(selectTransmissionDriveType).val(null).trigger('change');
}

async function deleteTransmissionDriveType() {
    const id = transmissionDriveTypeId.value;
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
        text: 'Bu çekiş tipini silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet',
        cancelButtonText: 'Hayır'
    });

    if (!confirmResult.isConfirmed) return;

    try {
        const res = await fetch(`/api/TransmissionDriveType/${id}`, { method: 'DELETE' });
        if (res.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Silindi.'
            });
            clearTransmissionDriveTypeForm();
            updateTransmissionDriveTypeList();
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

async function saveTransmissionDriveType(e) {
    e.preventDefault();

    const id = transmissionDriveTypeId.value;
    const name = transmissionDriveTypeName.value.trim();

    if (!name) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Çekiş tipi adı boş olamaz.'
        });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/TransmissionDriveType/${id}` : '/api/TransmissionDriveType';

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
            clearTransmissionDriveTypeForm();
            updateTransmissionDriveTypeList();
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

async function updateTransmissionDriveTypeList() {
    try {
        const res = await fetch('/api/TransmissionDriveType');
        if (!res.ok) throw new Error('Veri alınamadı: ' + res.statusText);
        const data = await res.json();

        const options = data.map(d => ({ id: d.id, text: d.name, value: d.value }));

        if ($(selectTransmissionDriveType).hasClass('select2-hidden-accessible')) {
            $(selectTransmissionDriveType).select2('destroy');
        }

        $(selectTransmissionDriveType).empty().select2({
            data: options,
            placeholder: 'Çekiş tipi seçin...',
            allowClear: true,
            width: '100%',
            dropdownParent: $('#editTableModal')
        });

        if (data.length > 0) {
            $(selectTransmissionDriveType).val(data[0].id).trigger('change');
            transmissionDriveTypeId.value = data[0].id;
            //transmissionDriveTypeValue.value = data[0].value;
            transmissionDriveTypeName.value = data[0].name;
        } else {
            transmissionDriveTypeId.value = '';
            //transmissionDriveTypeValue.value = '';
            transmissionDriveTypeName.value = '';
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Çekiş tipleri yüklenirken hata oluştu: ' + error.message
        });
    }
}

$(selectTransmissionDriveType).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    transmissionDriveTypeId.value = data.id;
    //transmissionDriveTypeValue.value = data.value;
    transmissionDriveTypeName.value = data.text;
});

$('button[data-bs-toggle="tab"]').on('shown.bs.tab', async function (e) {
    const target = $(e.target).attr('data-bs-target');
    if (target === '#tabTransmissionDriveType') {
        await updateTransmissionDriveTypeList();
    }
});
