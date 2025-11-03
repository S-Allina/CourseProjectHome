// ===== ACCESS MANAGEMENT =====
class AccessManager {
    constructor(autoSaveManager) {
        this.autoSaveManager = autoSaveManager;
        this.users = [];
        this.currentAccessList = window.accessListData || [];
        this.dataTable = null;
    }

    init() {
        this.setupUserSearch();
        this.initializeDataTable();
        this.setupRemoveAccess();
        this.setupAccessControlToggle();
    }

    initializeDataTable() {
        const table = document.getElementById('accessListTable');
        if (!table) return;

        this.dataTable = $('#accessListTable').DataTable({
            pageLength: 10,
            responsive: true,
            order: [[2, 'desc']], 
            language: {
                lengthMenu: "Show _MENU_ entries",
                info: "Showing _START_ to _END_ of _TOTAL_ entries",
                search: "Search:",
                paginate: {
                    first: "First",
                    last: "Last",
                    next: "Next",
                    previous: "Previous"
                },
                emptyTable: "No users have access to this inventory yet"
            },
            columns: [
                { data: 'user' }, 
                { data: 'accessLevel' }, 
                { data: 'grantedAt' }, 
                {
                    data: 'actions',
                    orderable: false,
                    searchable: false
            ],
            data: this.prepareTableData()
        });

        this.updateDataTable();
    }

    prepareTableData() {
        return this.currentAccessList.map(access => {
            const userInitial = access.userEmail?.charAt(0)?.toUpperCase() || 'U';

            return {
                user: `
                    <div class="d-flex align-items-center">
                        <div class="avatar-sm bg-primary rounded-circle d-flex align-items-center justify-content-center me-2">
                            <span class="text-white">${userInitial}</span>
                        </div>
                        <div>
                            <div class="fw-semibold user-name">${access.userName || 'Loading...'}</div>
                            <small class="text-muted user-email">${access.userEmail || 'Loading...'}</small>
                        </div>
                    </div>
                `,
                accessLevel: `
                    <span class="badge ${this.getAccessLevelBadgeClass(access.accessLevel)}">
                        ${this.getAccessLevelText(access.accessLevel)}
                    </span>
                `,
                grantedAt: new Date(access.grantedAt).toLocaleString(),
                actions: `
                    <button type="button"
                            class="btn btn-sm btn-outline-danger remove-access"
                            data-user-id="${access.userId}"
                            title="Remove Access">
                        <i class="fas fa-times"></i>
                    </button>
                `,
                userId: access.userId // Скрытые данные для поиска
            };
        });
    }

    updateDataTable() {
        if (this.dataTable) {
            this.dataTable.clear();
            this.dataTable.rows.add(this.prepareTableData());
            this.dataTable.draw();

            // Загружаем детали пользователей после обновления таблицы
            this.loadUserDetails();
        }
    }

    setupUserSearch() {
        const searchInput = document.getElementById('userSearchInput');
        const searchResults = document.getElementById('userSearchResults');
        const searchBtn = document.getElementById('searchUserBtn');

        if (!searchInput) return;

        searchInput.addEventListener('input', this.debounce((e) => {
            const query = e.target.value.trim();
            if (query.length >= 2) {
                this.searchUsers(query);
            } else {
                this.hideSearchResults();
            }
        }, 300));

        searchBtn.addEventListener('click', () => {
            const query = searchInput.value.trim();
            if (query.length >= 2) {
                this.searchUsers(query);
            }
        });

        document.addEventListener('click', (e) => {
            if (searchResults && !searchResults.contains(e.target) && e.target !== searchInput) {
                this.hideSearchResults();
            }
        });
    }

    async searchUsers(query) {
        try {
            const response = await fetch(`/Search/search-users?query=${encodeURIComponent(query)}&limit=10`);
            if (response.ok) {
                const users = await response.json();
                this.displaySearchResults(users);
            } else {
                console.error('Search failed with status:', response.status);
                this.hideSearchResults();
            }
        } catch (error) {
            console.error('Search failed:', error);
            this.hideSearchResults();
        }
    }

    displaySearchResults(users) {
        const resultsContainer = document.getElementById('userSearchResults');
        if (!resultsContainer) return;

        if (users.length === 0) {
            resultsContainer.innerHTML = '<div class="dropdown-item text-muted">No users found</div>';
            resultsContainer.style.display = 'block';
            return;
        }

        let html = '';
        users.forEach(user => {
            const isAlreadyAdded = this.currentAccessList.some(access => access.userId === user.id);

            html += `
                <div class="dropdown-item d-flex justify-content-between align-items-center ${isAlreadyAdded ? 'text-muted' : ''}"
                     data-user-id="${user.id}"
                     data-user-email="${user.email}"
                     data-user-name="${user.firstName} ${user.lastName}">
                    <div>
                        <div class="fw-semibold">${user.firstName} ${user.lastName}</div>
                        <small class="text-muted">${user.email}</small>
                    </div>
                    ${isAlreadyAdded ?
                    '<span class="badge bg-secondary">Already added</span>' :
                    `<button type="button" class="btn btn-sm btn-outline-primary add-user-btn">Add</button>`
                }
                </div>
            `;
        });

        resultsContainer.innerHTML = html;
        resultsContainer.style.display = 'block';

        resultsContainer.querySelectorAll('.add-user-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                const item = btn.closest('.dropdown-item');
                this.addUserToAccessList(
                    item.dataset.userId,
                    item.dataset.userName,
                    item.dataset.userEmail
                );
                this.hideSearchResults();
                document.getElementById('userSearchInput').value = '';
            });
        });
    }

    hideSearchResults() {
        const resultsContainer = document.getElementById('userSearchResults');
        if (resultsContainer) {
            resultsContainer.style.display = 'none';
        }
    }

    addUserToAccessList(userId, userName, userEmail) {
        if (this.currentAccessList.some(access => access.userId === userId)) {
            this.showToast('User already has access', 'warning');
            return;
        }

        const accessLevel = parseInt(document.getElementById('accessLevelSelect').value);
        const grantedAt = new Date().toISOString();

        this.currentAccessList.push({
            userId: userId,
            accessLevel: accessLevel,
            grantedAt: grantedAt,
            userName: userName,
            userEmail: userEmail
        });

        this.updateDataTable();
        this.updateHiddenAccessFields();

        this.showToast('User added successfully', 'success');

        if (this.autoSaveManager.isEditMode) {
            this.autoSaveManager.handleChange();
        }
    }

    removeUserFromAccessList(userId) {
        if (confirm('Are you sure you want to remove access for this user?')) {
            this.currentAccessList = this.currentAccessList.filter(access => access.userId !== userId);
            this.updateDataTable();
            this.updateHiddenAccessFields();

            if (this.autoSaveManager.isEditMode) {
                this.autoSaveManager.handleChange();
            }
        }
    }

    updateHiddenAccessFields() {
        const container = document.getElementById('accessListData');
        if (!container) return;

        let html = '';
        this.currentAccessList.forEach((access, index) => {
            html += `
                <input type="hidden" name="AccessList[${index}].UserId" value="${access.userId}" />
                <input type="hidden" name="AccessList[${index}].AccessLevel" value="${access.accessLevel}" />
                <input type="hidden" name="AccessList[${index}].GrantedAt" value="${access.grantedAt}" />
            `;
        });

        container.innerHTML = html;
    }

    async loadUserDetails() {
        const userIds = this.currentAccessList.map(access => access.userId);

        if (userIds.length === 0) return;

        try {
            const response = await fetch('/Search/users-details', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({ userIds: userIds })
            });

            if (response.ok) {
                const usersDetails = await response.json();
                this.updateUserDetailsInTable(usersDetails);
            }
        } catch (error) {
            console.error('Failed to load user details:', error);
        }
    }

    updateUserDetailsInTable(usersDetails) {
        usersDetails.forEach(user => {
            // Обновляем данные в currentAccessList
            const accessItem = this.currentAccessList.find(access => access.userId === user.id);
            if (accessItem) {
                accessItem.userName = `${user.firstName} ${user.lastName}`;
                accessItem.userEmail = user.email;
            }

            // Обновляем DataTable
            if (this.dataTable) {
                this.updateDataTable();
            }
        });
    }

    setupRemoveAccess() {
        // Используем делегирование событий для динамически создаваемых кнопок
        $(document).on('click', '.remove-access', (e) => {
            const userId = e.currentTarget.dataset.userId;
            this.removeUserFromAccessList(userId);
        });
    }

    setupAccessControlToggle() {
        const accessControlToggle = document.getElementById('accessControlToggle');
        const accessControls = document.querySelectorAll('.access-control');
        const accessManagementSection = document.querySelector('.access-management-section');
        const accessListTable = document.getElementById('accessListTable');

        if (!accessControlToggle) return;

        function toggleAccessControls() {
            if (!accessControlToggle.checked) {
                accessControls.forEach(control => {
                    control.disabled = false;
                    control.classList.remove('disabled');
                });
                if (accessManagementSection) {
                    accessManagementSection.classList.remove('disabled');
                }
                if (accessListTable) {
                    accessListTable.classList.remove('disabled-table');
                }
            } else {
                accessControls.forEach(control => {
                    control.disabled = true;
                    control.classList.add('disabled');
                });
                if (accessManagementSection) {
                    accessManagementSection.classList.add('disabled');
                }
                if (accessListTable) {
                    accessListTable.classList.add('disabled-table');
                }
            }
        }

        toggleAccessControls();
        accessControlToggle.addEventListener('change', toggleAccessControls);

        // Блокируем клики по disabled элементам
        document.addEventListener('click', (e) => {
            if (e.target.closest('.remove-access')) {
                const removeButton = e.target.closest('.remove-access');
                if (removeButton.disabled) {
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            }
        });

        accessControls.forEach(control => {
            control.addEventListener('click', function (e) {
                if (this.disabled) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });

            control.addEventListener('keydown', function (e) {
                if (this.disabled) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        });
    }

    getAccessLevelBadgeClass(accessLevel) {
        switch (accessLevel) {
            case 1: return 'bg-secondary';
            case 2: return 'bg-primary';
            case 3: return 'bg-success';
            default: return 'bg-secondary';
        }
    }

    getAccessLevelText(accessLevel) {
        switch (accessLevel) {
            case 1: return 'Read Only';
            case 2: return 'Read & Write';
            case 3: return 'Admin';
            default: return 'Unknown';
        }
    }

    showToast(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        toast.style.cssText = 'top: 20px; right: 20px; z-index: 1050; min-width: 300px;';
        toast.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.remove();
        }, 3000);
    }

    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
}

// Делаем класс глобально доступным
window.AccessManager = AccessManager;