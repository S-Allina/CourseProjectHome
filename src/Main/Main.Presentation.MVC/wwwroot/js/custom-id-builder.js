export class CustomIdBuilder {
    constructor() {
        this.components = [];
        this.componentCount = 0;
        this.init();
    }

    init() {
        console.log('CustomIdBuilder initialized');
        this.initDragAndDrop();
        this.bindEvents();
        this.addSampleComponents();
    }

    initDragAndDrop() {
        const formatComponents = document.getElementById('formatComponents');
        console.log('formatComponents found:', formatComponents);

        if (formatComponents) {
            Sortable.create(formatComponents, {
                animation: 150,
                ghostClass: 'bg-warning',
                onEnd: (evt) => {
                    this.updateComponentsOrder();
                    this.generatePreview();
                }
            });
        }

        this.initializeDragToRemove();
    }

    initializeDragToRemove() {
        const formatBuilder = document.getElementById('formatBuilder');
        console.log('formatBuilder found:', formatBuilder);

        if (!formatBuilder) return;

        formatBuilder.addEventListener('dragover', (e) => {
            e.preventDefault();
            formatBuilder.classList.add('border-danger');
        });

        formatBuilder.addEventListener('dragleave', (e) => {
            formatBuilder.classList.remove('border-danger');
        });

        formatBuilder.addEventListener('drop', (e) => {
            e.preventDefault();
            formatBuilder.classList.remove('border-danger');

            const componentId = e.dataTransfer.getData('text/plain');
            this.removeComponent(componentId);
        });
    }

    bindEvents() {
        // Кнопки добавления компонентов
        const componentButtons = document.getElementById('componentButtons');
        console.log('componentButtons found:', componentButtons);

        if (componentButtons) {
            componentButtons.addEventListener('click', (e) => {
                console.log('Button clicked:', e.target);
                const button = e.target.closest('button');
                if (button) {
                    const type = button.dataset.componentType;
                    console.log('Adding component:', type);
                    this.addComponent(type);
                }
            });
        } else {
            console.error('componentButtons element not found!');
        }

        // Кнопка обновления preview
        const refreshPreview = document.getElementById('refreshPreview');
        console.log('refreshPreview found:', refreshPreview);

        if (refreshPreview) {
            refreshPreview.addEventListener('click', () => {
                this.generatePreview();
            });
        }
    }

    addComponent(type) {
        console.log('Adding component of type:', type);
        this.componentCount++;
        const componentId = `component-${this.componentCount}`;

        const componentConfig = this.getComponentConfig(type, componentId);

        // Добавляем компонент в UI
        const noComponentsMessage = document.getElementById('noComponentsMessage');
        const formatComponents = document.getElementById('formatComponents');

        console.log('noComponentsMessage:', noComponentsMessage);
        console.log('formatComponents:', formatComponents);

        if (noComponentsMessage) noComponentsMessage.style.display = 'none';
        if (formatComponents) {
            formatComponents.insertAdjacentHTML('beforeend', componentConfig.html);
            console.log('Component added to DOM');
        } else {
            console.error('formatComponents not found!');
        }

        // Добавляем в массив компонентов
        this.components.push({
            id: componentId,
            type: type,
            value: componentConfig.defaultValue
        });

        this.initializeComponentEvents(componentId);
        this.generatePreview();
    }

    getComponentConfig(type, componentId) {
        const configs = {
            'fixed-text': {
                html: `
                    <div class="component-item badge bg-primary p-2" draggable="true" data-type="fixed-text" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>Fixed Text</span>
                        <input type="text" class="form-control form-control-sm d-inline-block w-auto ms-1"
                               placeholder="Enter text" value="ITEM" />
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: 'ITEM'
            },
            'random-20bit': {
                html: `
                    <div class="component-item badge bg-success p-2" draggable="true" data-type="random-20bit" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>20-bit Random</span>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{R20}'
            },
            'random-32bit': {
                html: `
                    <div class="component-item badge bg-success p-2" draggable="true" data-type="random-32bit" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>32-bit Random</span>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{R32}'
            },
            'random-6digit': {
                html: `
                    <div class="component-item badge bg-success p-2" draggable="true" data-type="random-6digit" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>6-digit Random</span>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{R6D}'
            },
            'random-9digit': {
                html: `
                    <div class="component-item badge bg-success p-2" draggable="true" data-type="random-9digit" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>9-digit Random</span>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{R9D}'
            },
            'guid': {
                html: `
                    <div class="component-item badge bg-info p-2" draggable="true" data-type="guid" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>GUID</span>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{GUID}'
            },
            'datetime': {
                html: `
                    <div class="component-item badge bg-warning text-dark p-2" draggable="true" data-type="datetime" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>Date/Time</span>
                        <select class="form-select form-select-sm d-inline-block w-auto ms-1">
                            <option value="{YYYYMMDD}">YYYYMMDD</option>
                            <option value="{YYMMDD}">YYMMDD</option>
                            <option value="{YYYY-MM-DD}">YYYY-MM-DD</option>
                            <option value="{DDMMYYYY}">DDMMYYYY</option>
                            <option value="{UNIX}">Unix Timestamp</option>
                        </select>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{YYYYMMDD}'
            },
            'sequence': {
                html: `
                    <div class="component-item badge bg-danger p-2" draggable="true" data-type="sequence" id="${componentId}">
                        <i class="fas fa-grip-vertical me-1"></i>
                        <span>Sequence</span>
                        <input type="number" class="form-control form-control-sm d-inline-block w-auto ms-1"
                               value="6" min="1" max="10" />
                        <span class="ms-1">digits</span>
                        <button type="button" class="btn-close btn-close-white ms-1"></button>
                    </div>
                `,
                defaultValue: '{SEQ:6}'
            }
        };

        return configs[type] || configs['fixed-text'];
    }

    initializeComponentEvents(componentId) {
        const componentElement = document.getElementById(componentId);
        if (!componentElement) return;

        // Drag events
        componentElement.addEventListener('dragstart', (e) => {
            e.dataTransfer.setData('text/plain', componentId);
        });

        // Remove button
        const removeButton = componentElement.querySelector('.btn-close');
        if (removeButton) {
            removeButton.addEventListener('click', () => {
                this.removeComponent(componentId);
            });
        }

        // Input/select change events
        const input = componentElement.querySelector('input[type="text"]');
        const select = componentElement.querySelector('select');
        const numberInput = componentElement.querySelector('input[type="number"]');

        if (input) {
            input.addEventListener('change', (e) => {
                this.updateComponent(componentId, e.target.value);
            });
        }

        if (select) {
            select.addEventListener('change', (e) => {
                this.updateComponent(componentId, e.target.value);
            });
        }

        if (numberInput) {
            numberInput.addEventListener('change', (e) => {
                this.updateComponent(componentId, `{SEQ:${e.target.value}}`);
            });
        }
    }

    removeComponent(componentId) {
        // Remove from UI
        const componentElement = document.getElementById(componentId);
        if (componentElement) {
            componentElement.remove();
        }

        // Remove from components array
        this.components = this.components.filter(c => c.id !== componentId);

        // Show message if no components left
        const noComponentsMessage = document.getElementById('noComponentsMessage');
        if (this.components.length === 0 && noComponentsMessage) {
            noComponentsMessage.style.display = 'block';
        }

        this.generatePreview();
    }

    updateComponent(componentId, value) {
        const component = this.components.find(c => c.id === componentId);
        if (component) {
            component.value = value;
            this.generatePreview();
        }
    }

    updateComponentsOrder() {
        const componentElements = document.querySelectorAll('.component-item');
        const newComponents = [];

        componentElements.forEach(element => {
            const componentId = element.id;
            const existingComponent = this.components.find(c => c.id === componentId);
            if (existingComponent) {
                newComponents.push(existingComponent);
            }
        });

        this.components = newComponents;
    }

    generatePreview() {
        let preview = '';

        this.components.forEach(component => {
            switch (component.type) {
                case 'fixed-text':
                    preview += component.value || 'TEXT';
                    break;
                case 'random-20bit':
                    preview += Math.floor(Math.random() * 1048576).toString().padStart(6, '0');
                    break;
                case 'random-32bit':
                    preview += Math.floor(Math.random() * 4294967296).toString().padStart(10, '0');
                    break;
                case 'random-6digit':
                    preview += Math.floor(Math.random() * 1000000).toString().padStart(6, '0');
                    break;
                case 'random-9digit':
                    preview += Math.floor(Math.random() * 1000000000).toString().padStart(9, '0');
                    break;
                case 'guid':
                    preview += 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
                        const r = Math.random() * 16 | 0;
                        const v = c == 'x' ? r : (r & 0x3 | 0x8);
                        return v.toString(16);
                    });
                    break;
                case 'datetime':
                    const now = new Date();
                    switch (component.value) {
                        case '{YYYYMMDD}':
                            preview += now.toISOString().slice(0, 10).replace(/-/g, '');
                            break;
                        case '{YYMMDD}':
                            preview += now.toISOString().slice(2, 10).replace(/-/g, '').slice(0, 6);
                            break;
                        case '{YYYY-MM-DD}':
                            preview += now.toISOString().slice(0, 10);
                            break;
                        case '{DDMMYYYY}':
                            preview += now.toISOString().slice(8, 10) + now.toISOString().slice(5, 7) + now.toISOString().slice(0, 4);
                            break;
                        case '{UNIX}':
                            preview += Math.floor(now.getTime() / 1000);
                            break;
                        default:
                            preview += now.toISOString().slice(0, 10).replace(/-/g, '');
                    }
                    break;
                case 'sequence':
                    const digits = parseInt(component.value.replace('{SEQ:', '').replace('}', '')) || 6;
                    preview += '1'.padStart(digits, '0');
                    break;
            }
        });

        const previewElement = document.getElementById('customIdPreview');
        if (previewElement) {
            previewElement.value = preview;
        }

        this.updateHiddenField();
    }

    updateHiddenField() {
        const formatString = this.components.map(c => c.value).join('');
        const hiddenField = document.getElementById('hiddenCustomIdFormat');
        if (hiddenField) {
            hiddenField.value = formatString;
        }
    }

    addSampleComponents() {
        this.addComponent('fixed-text');
        this.addComponent('sequence');
        this.generatePreview();
    }

    // Метод для загрузки существующего формата (для редактирования)
    loadExistingFormat(format) {
        if (!format) return;

        // Очищаем текущие компоненты
        this.components = [];
        this.componentCount = 0;

        const formatComponents = document.getElementById('formatComponents');
        if (formatComponents) {
            formatComponents.innerHTML = '<div class="text-muted text-center w-100 py-3" id="noComponentsMessage">No components added. Start by adding components from the panel on the right.</div>';
        }

        // Простой парсинг формата - можно улучшить при необходимости
        const regex = /(\{[^}]+\}|[^{]+)/g;
        const matches = format.match(regex);

        if (matches) {
            matches.forEach(match => {
                if (match.startsWith('{') && match.endsWith('}')) {
                    const cleanMatch = match.slice(1, -1).toLowerCase();
                    if (cleanMatch.startsWith('seq:')) {
                        this.addComponent('sequence');
                    } else if (cleanMatch === 'r20') {
                        this.addComponent('random-20bit');
                    } else if (cleanMatch === 'r32') {
                        this.addComponent('random-32bit');
                    } else if (cleanMatch === 'r6d') {
                        this.addComponent('random-6digit');
                    } else if (cleanMatch === 'r9d') {
                        this.addComponent('random-9digit');
                    } else if (cleanMatch === 'guid') {
                        this.addComponent('guid');
                    } else if (['yyyymmdd', 'yymmdd', 'yyyy-mm-dd', 'ddmmyyyy', 'unix'].includes(cleanMatch)) {
                        this.addComponent('datetime');
                    } else {
                        this.addComponent('fixed-text');
                    }
                } else {
                    // Текст вне скобок
                    const component = this.components.find(c => c.type === 'fixed-text');
                    if (component) {
                        component.value = match;
                    } else {
                        this.addComponent('fixed-text');
                        const newComponent = this.components[this.components.length - 1];
                        newComponent.value = match;
                    }
                }
            });
        }

        this.generatePreview();
    }

    // Получить текущий формат
    getCurrentFormat() {
        return this.components.map(c => c.value).join('');
    }
}