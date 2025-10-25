class ThemeManager {
    constructor() {
        this.currentTheme = this.getSavedTheme() || 'light';
        this.init();
    }

    init() {
        this.applyTheme(this.currentTheme);
        this.bindEvents();
        this.updateLogoutHandler();
    }

    getSavedTheme() {
        return localStorage.getItem('app-theme') || this.getThemeFromClaim();
    }

    getThemeFromClaim() {
        // Если нужно получить тему из claims при загрузке
        const html = document.documentElement;
        return html.getAttribute('data-bs-theme') || 'light';
    }

    applyTheme(theme) {
        const html = document.documentElement;

        // Устанавливаем data-bs-theme для html элемента
        html.setAttribute('data-bs-theme', theme);

        // Сохраняем в localStorage
        localStorage.setItem('app-theme', theme);

        this.currentTheme = theme;
        this.updateActiveThemeIndicator();
    }

    bindEvents() {
        // Обработчики для переключателя темы
        document.querySelectorAll('.theme-selector').forEach(button => {
            button.addEventListener('click', (e) => {
                e.preventDefault();
                const theme = e.target.closest('.theme-selector').dataset.theme;
                this.switchTheme(theme);
            });
        });

        // Обработчик для сохранения темы при выходе
        document.addEventListener('click', (e) => {
            if (e.target.matches('#logoutLink') || e.target.closest('#logoutLink')) {
                e.preventDefault();
                this.saveThemeBeforeLogout().then(() => {
                    window.location.href = e.target.href || e.target.closest('a').href;
                });
            }
        });
    }

    async switchTheme(theme) {
        if (theme === this.currentTheme) return;

        this.applyTheme(theme);

        try {
            // Сохраняем тему на сервере
            await this.saveThemeToServer(theme);
        } catch (error) {
            console.error('Ошибка сохранения темы на сервере:', error);
        }
    }

    async saveThemeToServer(theme) {
        const response = await fetch('/api/users/UpdateTheme', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ theme: theme })
        });

        if (!response.ok) {
            throw new Error('Ошибка сохранения темы');
        }
    }

    async saveThemeBeforeLogout() {
        try {
            await this.saveThemeToServer(this.currentTheme);
        } catch (error) {
            console.error('Ошибка при сохранении темы перед выходом:', error);
        }
    }

    updateActiveThemeIndicator() {
        document.querySelectorAll('.theme-selector').forEach(item => {
            if (item.dataset.theme === this.currentTheme) {
                item.classList.add('active', 'fw-bold');
                const icon = item.dataset.theme === 'light' ? 'fa-sun' : 'fa-moon';
                item.innerHTML = `<i class="fas fa-check me-2"></i><i class="fas ${icon} me-2"></i>${item.textContent}`;
            } else {
                item.classList.remove('active', 'fw-bold');
                const theme = item.dataset.theme;
                const icon = theme === 'light' ? 'fa-sun' : 'fa-moon';
                item.innerHTML = `<i class="fas ${icon} me-2"></i>${theme === 'light' ? 'Светлая' : 'Темная'}`;
            }
        });
    }

    updateLogoutHandler() {
        const logoutLink = document.getElementById('logoutLink');
        if (logoutLink) {
            logoutLink.addEventListener('click', async (e) => {
                e.preventDefault();
                await this.saveThemeBeforeLogout();
                window.location.href = logoutLink.href;
            });
        }
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    window.themeManager = new ThemeManager();
});