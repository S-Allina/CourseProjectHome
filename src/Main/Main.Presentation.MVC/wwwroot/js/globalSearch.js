class GlobalSearch {
    constructor() {
        this.searchInput = document.getElementById('globalSearchInput');
        this.quickResults = document.getElementById('quickSearchResults');
        this.quickContent = document.getElementById('quickSearchContent');
        this.searchTimeout = null;
        this.isLoading = false;

        this.init();
    }

    init() {
        // Обработчики событий для поля поиска
        this.searchInput.addEventListener('input', this.handleInput.bind(this));
        this.searchInput.addEventListener('focus', this.handleFocus.bind(this));

        // Закрытие результатов при клике вне области
        document.addEventListener('click', this.handleDocumentClick.bind(this));

        // Обработка клавиш
        this.searchInput.addEventListener('keydown', this.handleKeydown.bind(this));

        console.log('Global Search initialized');
    }

    handleInput(e) {
        const term = e.target.value.trim();

        clearTimeout(this.searchTimeout);

        if (term.length < 2) {
            this.hideResults();
            return;
        }

        this.searchTimeout = setTimeout(() => {
            this.performQuickSearch(term);
        }, 300);
    }

    handleFocus() {
        const term = this.searchInput.value.trim();
        if (term.length >= 2 && this.quickContent.innerHTML.trim() !== '') {
            this.showResults();
        }
    }

    handleDocumentClick(e) {
        if (!this.quickResults.contains(e.target) && e.target !== this.searchInput) {
            this.hideResults();
        }
    }

    handleKeydown(e) {
        switch (e.key) {
            case 'Escape':
                this.hideResults();
                break;
            case 'ArrowDown':
                e.preventDefault();
                this.focusFirstResult();
                break;
        }
    }

    async performQuickSearch(term) {
        if (this.isLoading) return;

        this.isLoading = true;
        this.showLoading();

        try {
            const response = await fetch(`/Search/QuickSearch?term=${encodeURIComponent(term)}`);
            const data = await response.json();

            if (data.success) {
                this.displayQuickResults(data.data);
            } else {
                this.displayError(data.error || 'Ошибка поиска');
            }
        } catch (error) {
            console.error('Search error:', error);
            this.displayError('Ошибка соединения');
        } finally {
            this.isLoading = false;
        }
    }

    displayQuickResults(data) {
        if (!data.results || data.results.length === 0) {
            this.quickContent.innerHTML = `
                <div class="text-center text-muted p-3">
                    <i class="fas fa-search me-2"></i>
                    Ничего не найдено
                </div>
            `;
            this.showResults();
            return;
        }

        let html = '';

        // Группируем результаты по типам
        const byType = {
            Inventory: data.results.filter(r => r.type === 'Inventory'),
            Item: data.results.filter(r => r.type === 'Item'),
            User: data.results.filter(r => r.type === 'User')
        };

        // Инвентари
        if (byType.Inventory.length > 0) {
            html += this.renderResultGroup('Инвентари', byType.Inventory, 'fa-archive');
        }

        // Предметы
        if (byType.Item.length > 0) {
            html += this.renderResultGroup('Предметы', byType.Item, 'fa-box');
        }

        // Пользователи
        if (byType.User && byType.User.length > 0) {
            html += this.renderResultGroup('Пользователи', byType.User, 'fa-user');
        }

        // Ссылка на полный поиск
        html += `
            <div class="border-top pt-2">
                <a href="/Search/Index?q=${encodeURIComponent(data.searchTerm)}" 
                   class="btn btn-outline-primary btn-sm w-100">
                    <i class="fas fa-search me-1"></i>
                    Все результаты поиска
                </a>
            </div>
        `;

        this.quickContent.innerHTML = html;
        this.showResults();
    }

    renderResultGroup(title, items, icon) {
        return `
            <div class="mb-2">
                <h6 class="text-muted mb-2 small">
                    <i class="fas ${icon} me-1"></i>${title}
                </h6>
                ${items.map(item => `
                    <a href="${item.url}" class="search-result-item d-block p-2 text-decoration-none text-dark rounded">
                        <div class="fw-semibold">${this.escapeHtml(item.name)}</div>
                        <small class="text-muted">${this.escapeHtml(item.additionalInfo || '')}</small>
                    </a>
                `).join('')}
            </div>
        `;
    }

    showLoading() {
        this.quickContent.innerHTML = `
            <div class="text-center p-3">
                <div class="spinner-border spinner-border-sm text-primary me-2"></div>
                <span class="text-muted">Поиск...</span>
            </div>
        `;
        this.showResults();
    }

    displayError(message) {
        this.quickContent.innerHTML = `
            <div class="text-center text-danger p-2">
                <i class="fas fa-exclamation-triangle me-2"></i>
                ${message}
            </div>
        `;
        this.showResults();
    }

    showResults() {
        this.quickResults.style.display = 'block';
    }

    hideResults() {
        this.quickResults.style.display = 'none';
    }

    focusFirstResult() {
        const firstResult = this.quickContent.querySelector('.search-result-item');
        if (firstResult) {
            firstResult.focus();
        }
    }

    escapeHtml(unsafe) {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    new GlobalSearch();
});