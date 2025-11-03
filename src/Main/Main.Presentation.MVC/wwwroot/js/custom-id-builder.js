document.addEventListener('DOMContentLoaded', function () {
    accessManager.init();
    const formatComponents = document.getElementById('formatComponents');

    if (formatComponents) {
        Sortable.create(formatComponents, {
            animation: 150,
            ghostClass: 'bg-warning',
            onEnd: function (evt) {
                updateComponentsOrder();
                generatePreview();

                if (autoSaveManager.isEditMode) {
                    autoSaveManager.handleChange();
                }
            }
        });

        initializeDragToRemove();
    }

    autoSaveManager.init();

    addComponent('fixed-text');
    addComponent('sequence');
    generatePreview();

    initializeFieldsFromModel();

    function initializeDragToRemove() {
        const formatBuilder = document.getElementById('formatBuilder');
        if (!formatBuilder) return;

        formatBuilder.addEventListener('dragover', function (e) {
            e.preventDefault();
            this.classList.add('border-danger');
        });

        formatBuilder.addEventListener('dragleave', function (e) {
            this.classList.remove('border-danger');
        });

        formatBuilder.addEventListener('drop', function (e) {
            e.preventDefault();
            this.classList.remove('border-danger');

            const componentId = e.dataTransfer.getData('text/plain');
            removeComponent(componentId);
        });
    }

    function addComponent(type) {
        componentCount++;
        const componentId = `component-${componentCount}`;

        let componentHtml = '';
        let defaultValue = '';

        switch (type) {
            case 'fixed-text':
                componentHtml = `
                        <div class="component-item badge bg-primary p-2" draggable="true" data-type="fixed-text" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>@Localizer["Text"]</span>
                            <input type="text" class="form-control form-control-sm d-inline-block w-auto ms-1"
                                   placeholder="Enter text" value='@Localizer["ITEM"]'
                                   onchange="updateComponent('${componentId}', this.value)" />
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = 'ITEM';
                break;

            case 'random-20bit':
                componentHtml = `
                        <div class="component-item badge bg-success p-2" draggable="true" data-type="random-20bit" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>20-bit @Localizer["Random"]</span>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{R20}';
                break;

            case 'random-32bit':
                componentHtml = `
                        <div class="component-item badge bg-success p-2" draggable="true" data-type="random-32bit" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>32-bit @Localizer["Random"]</span>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{R32}';
                break;

            case 'random-6digit':
                componentHtml = `
                        <div class="component-item badge bg-success p-2" draggable="true" data-type="random-6digit" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>6-digit @Localizer["Random"]</span>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{R6D}';
                break;

            case 'random-9digit':
                componentHtml = `
                        <div class="component-item badge bg-success p-2" draggable="true" data-type="random-9digit" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>9-digit @Localizer["Random"]</span>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{R9D}';
                break;

            case 'guid':
                componentHtml = `
                        <div class="component-item badge bg-info p-2" draggable="true" data-type="guid" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>@Localizer["GUID"]</span>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{GUID}';
                break;

            case 'datetime':
                componentHtml = `
                        <div class="component-item badge bg-warning text-dark p-2" draggable="true" data-type="datetime" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>@Localizer["Date/Time"]</span>
                            <select class="form-select form-select-sm d-inline-block w-auto ms-1" onchange="updateComponent('${componentId}', this.value)">
                                <option value="{YYYYMMDD}">YYYYMMDD</option>
                                <option value="{YYMMDD}">YYMMDD</option>
                                <option value="{YYYY-MM-DD}">YYYY-MM-DD</option>
                                <option value="{DDMMYYYY}">DDMMYYYY</option>
                            </select>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{YYYYMMDD}';
                break;

            case 'sequence':
                componentHtml = `
                        <div class="component-item badge bg-danger p-2" draggable="true" data-type="sequence" id="${componentId}">
                            <i class="fas fa-grip-vertical me-1"></i>
                            <span>@Localizer["sequence"]</span>
                            <input type="number" class="form-control form-control-sm d-inline-block w-auto ms-1"
                                   value="6" min="1" max="10"
                                   onchange="updateComponent('${componentId}', '{SEQ:' + this.value + '}')" />
                            <span class="ms-1">digits</span>
                            <button type="button" class="btn-close btn-close-white ms-1" onclick="removeComponent('${componentId}')"></button>
                        </div>
                    `;
                defaultValue = '{SEQ:6}';
                break;
        }

        const noComponentsMessage = document.getElementById('noComponentsMessage');
        const formatComponents = document.getElementById('formatComponents');

        if (noComponentsMessage) {
            noComponentsMessage.style.display = 'none';
        }
        if (formatComponents) {
            formatComponents.insertAdjacentHTML('beforeend', componentHtml);
        }

        components.push({
            id: componentId,
            type: type,
            value: defaultValue
        });

        const componentElement = document.getElementById(componentId);
        if (componentElement) {
            componentElement.addEventListener('dragstart', function (e) {
                e.dataTransfer.setData('text/plain', componentId);
            });
        }

        generatePreview();

        if (autoSaveManager.isEditMode) {
            autoSaveManager.handleChange();
        }
    }

    function removeComponent(componentId) {
        const componentElement = document.getElementById(componentId);
        if (componentElement) {
            componentElement.remove();
        }

        components = components.filter(c => c.id !== componentId);

        if (components.length === 0) {
            const noComponentsMessage = document.getElementById('noComponentsMessage');
            if (noComponentsMessage) {
                noComponentsMessage.style.display = 'block';
            }
        }

        generatePreview();

        if (autoSaveManager.isEditMode) {
            autoSaveManager.handleChange();
        }
    }

    function updateComponent(componentId, value) {
        const component = components.find(c => c.id === componentId);
        if (component) {
            component.value = value;
            generatePreview();

            if (autoSaveManager.isEditMode) {
                autoSaveManager.handleChange();
            }
        }
    }

    function updateComponentsOrder() {
        const componentElements = document.querySelectorAll('.component-item');
        const newComponents = [];

        componentElements.forEach(element => {
            const componentId = element.id;
            const existingComponent = components.find(c => c.id === componentId);
            if (existingComponent) {
                newComponents.push(existingComponent);
            }
        });

        components = newComponents;
    }

    function generatePreview() {
        let preview = '';

        components.forEach(component => {
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
                    preview += 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
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

        const formatString = components.map(c => c.value).join('');
        const hiddenFormatElement = document.getElementById('hiddenCustomIdFormat');
        if (hiddenFormatElement) {
            hiddenFormatElement.value = formatString;
        }
    }
})
