//export class TagsManager {
//    constructor() {
//        this.tags = [];
//        this.init();
//    }

//    init() {
//        this.bindEvents();
//    }

//    bindEvents() {
//        const tagInput = document.getElementById('tagInput');
//        if (tagInput) {
//            tagInput.addEventListener('keydown', (e) => {
//                if (e.key === 'Enter' || e.key === ',') {
//                    e.preventDefault();
//                    this.addTag(e.target.value.trim().replace(',', ''));
//                    e.target.value = '';
//                }
//            });

//            // Также обрабатываем ввод запятой
//            tagInput.addEventListener('input', (e) => {
//                if (e.target.value.includes(',')) {
//                    const tags = e.target.value.split(',').map(tag => tag.trim()).filter(tag => tag);
//                    tags.forEach(tag => this.addTag(tag));
//                    e.target.value = '';
//                }
//            });
//        }
//    }

//    addTag(tagText) {
//        if (tagText && !this.tags.includes(tagText)) {
//            this.tags.push(tagText);
//            this.updateTagsDisplay();
//        }
//    }

//    removeTag(tagText) {
//        this.tags = this.tags.filter(tag => tag !== tagText);
//        this.updateTagsDisplay();
//    }

//    updateTagsDisplay() {
//        const container = document.getElementById('tagsContainer');
//        const hiddenInput = document.getElementById('hiddenTags');

//        if (!container) return;

//        container.innerHTML = '';

//        if (hiddenInput) {
//            hiddenInput.value = JSON.stringify(this.tags);
//        }

//        this.tags.forEach(tag => {
//            const tagElement = document.createElement('span');
//            tagElement.className = 'badge bg-primary me-2 mb-2';
//            tagElement.innerHTML = `
//                ${tag}
//                <button type="button" class="btn-close btn-close-white ms-1" data-tag="${this.escapeHtml(tag)}"></button>
//            `;
//            container.appendChild(tagElement);
//        });

//        // Добавляем обработчики для кнопок удаления тегов
//        container.querySelectorAll('button[data-tag]').forEach(button => {
//            button.addEventListener('click', (e) => {
//                const tag = e.target.closest('button').dataset.tag;
//                this.removeTag(tag);
//            });
//        });
//    }

//    updateHiddenField() {
//        const hiddenInput = document.getElementById('hiddenTags');
//        if (hiddenInput) {
//            hiddenInput.value = JSON.stringify(this.tags);
//        }
//    }

//    // Загрузка существующих тегов (для редактирования)
//    loadExistingTags(tags) {
//        if (!tags || !Array.isArray(tags)) return;

//        this.tags = [...tags];
//        this.updateTagsDisplay();
//    }

//    // Получить текущие теги
//    getCurrentTags() {
//        return [...this.tags];
//    }

//    // Экранирование HTML для безопасности
//    escapeHtml(unsafe) {
//        return unsafe
//            .replace(/&/g, "&amp;")
//            .replace(/</g, "&lt;")
//            .replace(/>/g, "&gt;")
//            .replace(/"/g, "&quot;")
//            .replace(/'/g, "&#039;");
//    }
//}