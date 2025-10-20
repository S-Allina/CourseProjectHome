import { CustomIdBuilder } from './custom-id-builder.js';
//import { FieldsManager } from './fields-manager.js';
import { TagsManager } from './tags-manager.js';
import './inventory-autosave.js';

export class InventoryForm {
    constructor(options = {}) {
        this.isEdit = options.isEdit || false;
        this.inventoryId = options.inventoryId || null;
        this.initialData = options.initialData || null;

        this.customIdBuilder = null;
        //this.fieldsManager = null;
        this.tagsManager = null;

        this.init();
    }

    init() {
        console.log('InventoryForm initialized');
        this.initializeManagers();
        this.bindEvents();
        this.loadInitialData();
    }

    initializeManagers() {
        console.log('Initializing managers...');
        this.customIdBuilder = new CustomIdBuilder();
        //this.fieldsManager = new FieldsManager();
        this.tagsManager = new TagsManager();
        console.log('All managers initialized');
    }

    bindEvents() {
        // Обработка отправки формы
        const form = document.getElementById('inventoryForm');
        if (form) {
            form.addEventListener('submit', (e) => {
                this.onFormSubmit(e);
            });
        }
    }

    onFormSubmit(e) {
        // Убеждаемся, что все данные сохранены в скрытые поля
        this.customIdBuilder.updateHiddenField();
        this.tagsManager.updateHiddenField();

        // Дополнительная валидация может быть добавлена здесь
    }

    loadInitialData() {
        if (!this.initialData) return;

        // Загрузка данных для редактирования
        if (this.isEdit) {
            this.loadEditData(this.initialData);
        }
    }

    loadEditData(data) {
        // Загрузка CustomIdFormat
        if (data.customIdFormat) {
            this.customIdBuilder.loadExistingFormat(data.customIdFormat);
        }
    }
}

let autoSaveManager = null;

document.addEventListener('DOMContentLoaded',  ()=> {
    // Инициализация автосохранения только в режиме редактирования
    const isEditMode = document.getElementById('inventoryForm').dataset.isEdit === 'true';
    if (isEditMode) {
        autoSaveManager = new InventoryAutoSave();
    }
    console.log('DOM loaded, initializing InventoryForm...');
    const formElement = document.getElementById('inventoryForm');
    console.log('Form element found:', formElement);

    if (!formElement) {
        console.error('Inventory form element not found!');
        return;
    }

    const options = {
        isEdit: formElement.dataset.isEdit === 'true',
        inventoryId: formElement.dataset.inventoryId || null,
        initialData: window.initialFormData || null
    };

    window.inventoryForm = new InventoryForm(options);
    console.log('InventoryForm created successfully');
});
export { autoSaveManager };