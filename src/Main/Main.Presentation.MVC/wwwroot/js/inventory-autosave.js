// ~/js/inventory-autosave.js
class InventoryAutoSave {
    constructor() {
        this.isEditMode = document.getElementById('inventoryForm').dataset.isEdit === 'true';
        this.isDirty = false;
        this.saveTimer = null;
        this.isSaving = false;
        this.saveDelay = 8000; // 8 секунд
        this.lastSavedData = null;

        this.initializeAutoSave();
    }

    initializeAutoSave() {
        if (!this.isEditMode) return;

        // Отслеживание изменений в форме
        this.setupChangeTracking();

        // Периодическая проверка изменений
        this.setupPeriodicCheck();

        // Обработка перед закрытием страницы
        this.setupBeforeUnload();
    }

    setupChangeTracking() {
        const form = document.getElementById('inventoryForm');

        // Отслеживание изменений в input, textarea, select
        form.addEventListener('input', this.handleChange.bind(this));
        form.addEventListener('change', this.handleChange.bind(this));

        // Отслеживание изменений в кастомных компонентах (Custom ID Format)
        this.setupCustomComponentsTracking();
    }

    setupCustomComponentsTracking() {
        // Отслеживание изменений в компонентах Custom ID Format
        const customIdComponents = document.getElementById('formatComponents');
        if (customIdComponents) {
            customIdComponents.addEventListener('input', this.handleChange.bind(this));
            customIdComponents.addEventListener('change', this.handleChange.bind(this));
        }

        // Отслеживание изменений в полях (Fields)
        const fieldsContainer = document.getElementById('fieldsContainer');
        if (fieldsContainer) {
            fieldsContainer.addEventListener('input', this.handleChange.bind(this));
            fieldsContainer.addEventListener('change', this.handleChange.bind(this));
        }

        // Отслеживание изменений в тегах
        const tagInput = document.getElementById('tagInput');
        if (tagInput) {
            tagInput.addEventListener('input', this.handleChange.bind(this));
        }
    }

    handleChange(event) {
        // Игнорируем некоторые системные события
        if (event.target.type === 'hidden') return;

        this.markAsDirty();
        this.scheduleAutoSave();
    }

    markAsDirty() {
        if (!this.isDirty) {
            this.isDirty = true;
            document.getElementById('isDirty').value = 'true';
            this.updateSaveIndicator('Unsaved changes');
        }
    }

    scheduleAutoSave() {
        // Очищаем предыдущий таймер
        if (this.saveTimer) {
            clearTimeout(this.saveTimer);
        }

        // Устанавливаем новый таймер
        this.saveTimer = setTimeout(() => {
            this.performAutoSave();
        }, this.saveDelay);
    }

    async performAutoSave() {
        if (!this.isDirty || this.isSaving || !this.isEditMode) return;

        this.isSaving = true;
        this.updateSaveIndicator('Auto-saving...', true);

        try {
            const formData = this.prepareFormData();

            const response = await fetch('/Inventory/AutoSave', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                const result = await response.json();

                if (result.success) {
                    this.handleSaveSuccess(result);
                } else {
                    this.handleSaveError(result);
                }
            } else {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
        } catch (error) {
            console.error('Auto-save failed:', error);
            this.handleSaveError({ message: 'Auto-save failed. Please save manually.' });
        } finally {
            this.isSaving = false;
        }
    }

    prepareFormData() {
        const form = document.getElementById('inventoryForm');
        const formData = new FormData(form);

        const data = {
            Id: document.querySelector('input[name="Id"]')?.value,
            Name: document.querySelector('input[name="Name"]')?.value,
            Description: document.querySelector('textarea[name="Description"]')?.value,
            CategoryId: document.querySelector('input[name="CategoryId"]')?.value,
            ImageUrl: document.querySelector('input[name="ImageUrl"]')?.value,
            IsPublic: document.querySelector('input[name="IsPublic"]')?.checked,
            Version: document.getElementById('Version').value,
            CustomIdFormat: document.getElementById('hiddenCustomIdFormat')?.value,
            Tags: document.getElementById('hiddenTags')?.value
        };

        // Собираем данные полей (Fields)
        data.Fields = this.collectFieldsData();

        return data;
    }

    collectFieldsData() {
        const fields = [];
        const fieldElements = document.querySelectorAll('.field-item');

        fieldElements.forEach((fieldElement, index) => {
            const field = {
                Id: fieldElement.querySelector('input[name$=".Id"]')?.value || 0,
                Name: fieldElement.querySelector('input[name$=".Name"]')?.value,
                FieldType: fieldElement.querySelector('select[name$=".FieldType"]')?.value,
                OrderIndex: fieldElement.querySelector('input[name$=".OrderIndex"]')?.value || index,
                Description: fieldElement.querySelector('textarea[name$=".Description"]')?.value,
                IsVisibleInTable: fieldElement.querySelector('input[name$=".IsVisibleInTable"]')?.checked || false,
                IsRequired: fieldElement.querySelector('input[name$=".IsRequired"]')?.checked || false
            };

            fields.push(field);
        });

        return fields;
    }

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]').value;
    }

    handleSaveSuccess(result) {
        this.isDirty = false;
        document.getElementById('isDirty').value = 'false';
        document.getElementById('lastSavedVersion').value = result.Version;
        document.getElementById('Version').value = result.Version;

        this.updateSaveIndicator('All changes saved', false, 'success');

        // Скрываем индикатор через 3 секунды
        setTimeout(() => {
            this.hideSaveIndicator();
        }, 3000);
    }

    handleSaveError(result) {
        this.updateSaveIndicator(result.message || 'Auto-save failed', false, 'error');

        if (result.isConcurrencyError) {
            this.handleConcurrencyError(result);
        }
    }

    handleConcurrencyError(result) {
        // Показываем диалог разрешения конфликта
        this.showConcurrencyDialog(result);
    }

    showConcurrencyDialog(result) {
        const userChoice = confirm(
            'This inventory was modified by another user. ' +
            'Do you want to reload the page to see the latest changes? ' +
            'Your unsaved changes will be lost.'
        );

        if (userChoice) {
            window.location.reload();
        }
    }

    updateSaveIndicator(message, showSpinner = false, type = 'info') {
        const indicator = document.getElementById('autoSaveIndicator');
        const messageElement = document.getElementById('autoSaveMessage');

        if (!indicator) return;

        // Обновляем классы в зависимости от типа
        indicator.className = `alert alert-${type} ${showSpinner ? '' : 'd-none'}`;
        messageElement.textContent = message;

        if (message && !showSpinner) {
            indicator.classList.remove('d-none');
        }
    }

    hideSaveIndicator() {
        const indicator = document.getElementById('autoSaveIndicator');
        if (indicator) {
            indicator.classList.add('d-none');
        }
    }

    setupPeriodicCheck() {
        // Периодическая проверка каждые 30 секунд
        setInterval(() => {
            if (this.isDirty && !this.isSaving) {
                this.performAutoSave();
            }
        }, 30000);
    }

    setupBeforeUnload() {
        window.addEventListener('beforeunload', (event) => {
            if (this.isDirty) {
                event.preventDefault();
                event.returnValue = 'You have unsaved changes. Are you sure you want to leave?';
                return event.returnValue;
            }
        });
    }

    // Метод для принудительного сохранения
    async forceSave() {
        if (this.isDirty && !this.isSaving) {
            await this.performAutoSave();
        }
    }
}

// Экспорт для использования в других модулях
window.InventoryAutoSave = InventoryAutoSave;