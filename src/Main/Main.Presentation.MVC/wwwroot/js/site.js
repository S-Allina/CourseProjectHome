// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function initializeDataTables() {
    // Инициализация для обеих таблиц
    $('#myInventoriesTable, #sharedInventoriesTable').DataTable({
        pageLength: 10,
        responsive: true,
        order: [[3, 'desc']], // Сортировка по дате создания
        language: {
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        }
    });
}