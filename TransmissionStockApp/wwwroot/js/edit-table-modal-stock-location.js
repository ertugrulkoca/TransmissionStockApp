const selectStockLocation = document.getElementById('selectStockLocation');
const formStockLocation = document.getElementById('formStockLocation');
const stockLocationId = document.getElementById('stockLocationId');
const stockLocationShelfCode = document.getElementById('stockLocationShelfCode');

document.getElementById('btnNewStockLocation').addEventListener('click', clearStockLocationForm);
document.getElementById('btnDeleteStockLocation').addEventListener('click', deleteStockLocation);
formStockLocation.addEventListener('submit', saveStockLocation);

function clearStockLocationForm() {
    stockLocationId.value = '';
    stockLocationShelfCode.value = '';
    $(selectStockLocation).val(null).trigger('change');
}

async function deleteStockLocation() {
    const id = stockLocationId.value;
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
        text: 'Bu stok konumunu silmek istediğinize emin misiniz?',
        showCancelButton: true,
        confirmButtonText: 'Evet',
        cancelButtonText: 'Hayır'
    });

    if (!confirmResult.isConfirmed) return;

    try {
        const res = await fetch(`/api/StockLocation/${id}`, { method: 'DELETE' });
        if (res.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Silindi.'
            });
            clearStockLocationForm();
            updateStockLocationList();
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

async function saveStockLocation(e) {
    e.preventDefault();

    const id = stockLocationId.value;
    const shelfCode = stockLocationShelfCode.value.trim();

    if (!shelfCode) {
        await Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Raf kodu boş olamaz.'
        });
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/StockLocation/${id}` : '/api/StockLocation';

    try {
        const res = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, shelfCode })
        });

        if (res.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Başarılı',
                text: 'Kaydedildi.'
            });
            clearStockLocationForm();
            updateStockLocationList();
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

async function updateStockLocationList() {
    try {
        const res = await fetch('/api/StockLocation');
        if (!res.ok) throw new Error('Veri alınamadı: ' + res.statusText);
        const result = await res.json();
        const data = result.data || [];
        const options = data.map(d => ({ id: d.id, text: d.shelfCode }));

        if ($(selectStockLocation).hasClass('select2-hidden-accessible')) {
            $(selectStockLocation).select2('destroy');
        }

        $(selectStockLocation).empty().select2({
            data: options,
            placeholder: 'Raf seçin...',
            allowClear: true,
            width: '100%',
            dropdownParent: $('#editTableModal')
        });

        if (data.length > 0) {
            $(selectStockLocation).val(data[0].id).trigger('change');
            stockLocationId.value = data[0].id;
            stockLocationShelfCode.value = data[0].shelfCode;
        } else {
            stockLocationId.value = '';
            stockLocationShelfCode.value = '';
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Hata',
            text: 'Stok lokasyonları yüklenirken hata oluştu: ' + error.message
        });
    }
}

$(selectStockLocation).off('select2:select').on('select2:select', function (e) {
    const data = e.params.data;
    stockLocationId.value = data.id;
    stockLocationShelfCode.value = data.text;
});

$('button[data-bs-toggle="tab"]').on('shown.bs.tab', async function (e) {
    const target = $(e.target).attr('data-bs-target');
    if (target === '#tabStockLocation') {
        await updateStockLocationList();
    }
});
