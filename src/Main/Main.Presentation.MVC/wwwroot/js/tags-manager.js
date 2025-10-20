export class TagsManager {
    constructor() {
        this.tags = [];
        this.init();
    }

    init() {
        this.bindEvents();
    }

    bindEvents() {
        document.getElementById('tagInput').addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ',') {
                e.preventDefault();
                this.addTag(e.target.value.trim().replace(',', ''));
                e.target.value = '';
            }
        });
    }

    addTag(tagText) {
        if (tagText && !this.tags.includes(tagText)) {
            this.tags.push(tagText);
            this.updateTagsDisplay();
        }
    }

    removeTag(tagText) {
        this.tags = this.tags.filter(tag => tag !== tagText);
        this.updateTagsDisplay();
    }

    updateTagsDisplay() {
        const container = document.getElementById('tagsContainer');
        const hiddenInput = document.getElementById('hiddenTags');

        container.innerHTML = '';
        hiddenInput.value = JSON.stringify(this.tags);

        this.tags.forEach(tag => {
            const tagElement = document.createElement('span');
            tagElement.className = 'badge bg-primary me-2 mb-2';
            tagElement.innerHTML = `
                ${tag}
                <button type="button" class="btn-close btn-close-white ms-1" data-tag="${tag}"></button>
            `;
            container.appendChild(tagElement);
        });

        // Добавляем обработчики для кнопок удаления тегов
        container.querySelectorAll('button').forEach(button => {
            button.addEventListener('click', (e) => {
                const tag = e.target.closest('button').dataset.tag;
                this.removeTag(tag);
            });
        });
    }

    updateHiddenField() {
        document.getElementById('hiddenTags').value = JSON.stringify(this.tags);
    }
}