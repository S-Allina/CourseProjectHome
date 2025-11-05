document.getElementById('deleteSelected').addEventListener('click', function () {
    const selectedCheckboxes = document.querySelectorAll('.rowCheckbox:checked');

    if (selectedCheckboxes.length === 0) {
        alert('@Localizer["Please select at least one inventory to delete."]');
        return;
    }

        const form = document.createElement('form');
        form.method = 'post';
        form.action = 'Inventories/Delete';

        selectedCheckboxes.forEach(checkbox => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'selectedIds';
            input.value = checkbox.value;
            form.appendChild(input);
        });

        document.body.appendChild(form);
        form.submit();
});