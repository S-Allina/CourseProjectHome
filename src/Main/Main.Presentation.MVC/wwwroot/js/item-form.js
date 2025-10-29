document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('itemForm');

    // Валидация формы
    form.addEventListener('submit', function (e) {
        if (!validateForm()) {
            e.preventDefault();
            showValidationErrors();
        }
    });

    // Обработка чекбоксов удаления файлов
    const removeFileCheckboxes = document.querySelectorAll('.remove-file-checkbox');
    removeFileCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const fileInput = this.closest('.file-field-container').querySelector('.file-input');
            fileInput.disabled = this.checked;
            if (this.checked) {
                fileInput.removeAttribute('data-val');
                fileInput.removeAttribute('data-val-required');
            } else {
                const container = this.closest('.field-container');
                const isRequired = container.dataset.required === 'true';
                if (isRequired) {
                    fileInput.setAttribute('data-val', 'true');
                    fileInput.setAttribute('data-val-required', 'This field is required');
                }
            }
        });
    });

    // Динамическая валидация
    function validateForm() {
        let isValid = true;
        const requiredInputs = document.querySelectorAll('.field-input[data-val="true"]');

        requiredInputs.forEach(input => {
            const value = getInputValue(input);
            if (!value || value.toString().trim() === '') {
                isValid = false;
                markFieldAsInvalid(input);
            } else {
                markFieldAsValid(input);
            }
        });

        return isValid;
    }

    function getInputValue(input) {
        if (input.type === 'checkbox') {
            return input.checked;
        }
        if (input.type === 'file') {
            return input.files.length > 0 ? input.value : null;
        }
        return input.value;
    }

    function markFieldAsInvalid(input) {
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');
    }

    function markFieldAsValid(input) {
        input.classList.add('is-valid');
        input.classList.remove('is-invalid');
    }

    function showValidationErrors() {
        const firstInvalidField = document.querySelector('.is-invalid');
        if (firstInvalidField) {
            firstInvalidField.scrollIntoView({ behavior: 'smooth', block: 'center' });
            firstInvalidField.focus();
        }
        alert('Please fill in all required fields marked with *.');
    }
});