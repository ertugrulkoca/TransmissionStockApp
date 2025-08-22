const selectTransmissionStatus = document.getElementById('selectTransmissionStatus');
const formTransmissionStatus = document.getElementById('formTransmissionStatus');
const transmissionStatusId = document.getElementById('transmissionStatusId');
const transmissionStatusName = document.getElementById('transmissionStatusName');

document.getElementById("btnNewTransmissionStatus").addEventListener("click", clearTransmissionStatusForm);
document.getElementById("btnDeleteTransmissionStatus").addEventListener("click", deleteTransmissionStatus);
formTransmissionStatus.addEventListener("submit", saveTransmissionStatus);

function clearTransmissionStatusForm() {
    transmissionStatusId.value = '';
    transmissionStatusName.value = '';
    $(selectTransmissionStatus).val(null).trigger('change');
}

async function deleteTransmissionStatus() {
    const id = transmissionStatusId.value;
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
        text: 'Bu durumu silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet',
        cancelButtonText: 'Hayır'
    });

    if (!confirmResult.isConfirmed) return;

    try {
        const res = await fetch(`/api/TransmissionStatus/${id}`, { method: 'DELETE' });
        if (res.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Silindi.'
            });
            clearTransmissionStatusForm();
            updateTransmissionStatusList();
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

async function saveTransmissionStatus(e) {
    e.preventDefault();

    const id = transmissionStatusId.value;
    const name = transmissionStatusName.value.trim();

    if (!name) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Durum adı boş olamaz.'
        });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/TransmissionStatus/${id}` : '/api/TransmissionStatus';

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
            clearTransmissionStatusForm();
            updateTransmissionStatusList();
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

async function updateTransmissionStatusList() {
    try {
        const res = await fetch('/api/TransmissionStatus');
        if (!res.ok) throw new Error('Veri alınamadı: ' + res.statusText);
        const data = await res.json();
        const options = data.map(s => ({ id: s.id, text: s.name }));

        if ($(selectTransmissionStatus).hasClass("select2-hidden-accessible")) {
            $(selectTransmissionStatus).select2('destroy');
        }

        $(selectTransmissionStatus).empty().select2({
            data: options,
            placeholder: "Şanzıman durumu seçin...",
            allowClear: true,
            width: '100%',
            dropdownParent: $('#editTableModal')
        });

        if (data.length > 0) {
            $(selectTransmissionStatus).val(data[0].id).trigger('change');
            transmissionStatusId.value = data[0].id;
            transmissionStatusName.value = data[0].name;
        } else {
            transmissionStatusId.value = '';
            transmissionStatusName.value = '';
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Durumlar yüklenirken hata oluştu: ' + error.message
        });
    }
}

$(selectTransmissionStatus).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    transmissionStatusId.value = data.id;
    transmissionStatusName.value = data.text;
});

$('button[data-bs-toggle="tab"]').on('shown.bs.tab', async function (e) {
    const target = $(e.target).attr("data-bs-target");
    if (target === '#tabTransmissionStatus') {
        await updateTransmissionStatusList();
    }
});
