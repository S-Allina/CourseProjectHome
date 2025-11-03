function initializeDataTables() {
    // Инициализация для моих инвентарей
    $('#myInventoriesTable').DataTable({
        pageLength: 10,
        responsive: true,
        order: [[6, 'desc']], // Сортировка по дате создания (столбец 6)
        columnDefs: [
            { orderable: false, targets: [0, 4, 9] }, // Отключаем сортировку для чекбоксов, полей и статуса
            { searchable: false, targets: [0, 4, 9] }, // Отключаем поиск для тех же столбцов
            { width: "30px", targets: 0 },
            { width: "80px", targets: 4 },
            { width: "100px", targets: 9 }
        ],
        language: {
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            search: "Search:",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },
        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rt<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>'
    });

    // Инициализация для общих инвентарей
    $('#sharedInventoriesTable').DataTable({
        pageLength: 10,
        responsive: true,
        order: [[6, 'desc']],
        columnDefs: [
            { orderable: false, targets: [0, 4, 9] },
            { searchable: false, targets: [0, 4, 9] },
            { width: "30px", targets: 0 },
            { width: "80px", targets: 4 },
            { width: "100px", targets: 9 }
        ],
        language: {
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            search: "Search:",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },
        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rt<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>'
    });

    // Обработка клика по строкам
    $('.clickable-row').on('click', function () {
        const inventoryId = $(this).data('inventory-id');
        window.location.href = `/Items/Index?inventoryId=${inventoryId}`;
    });

    // Обработка "Select All"
    $('#selectAllMy, #selectAllShared').on('change', function () {
        const tableType = this.id.replace('selectAll', '');
        const tableId = tableType + 'InventoriesTable';
        const isChecked = $(this).is(':checked');
        $(`#${tableId} .rowCheckbox`).prop('checked', isChecked);
    });
}

// Функция для массового удаления
function deleteSelectedInventories(tableType) {
    const tableId = tableType + 'InventoriesTable';
    const selectedIds = [];

    $(`#${tableId} .rowCheckbox:checked`).each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        alert('Please select at least one inventory to delete.');
        return;
    }

    if (confirm(`Are you sure you want to delete ${selectedIds.length} selected inventories?`)) {
        // Ваш код для удаления
        $.post('@Url.Action("Delete", "Inventories")', { selectedIds: selectedIds })
            .done(function () {
                location.reload();
            })
            .fail(function () {
                alert('Error deleting inventories.');
            });
    }
}

// Инициализация при загрузке документа
$(document).ready(function () {
    initializeDataTables();
});