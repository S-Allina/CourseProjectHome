class InventoryChat {
    constructor() {
        this.connection = null;
        this.inventoryId = null;
        this.isConnected = false;
        this.messages = [];
        this.currentFile = null;
        this.isLoadingMore = false; 
    }

    initialize() {
        this.inventoryId = document.getElementById('inventoryId')?.value;
        if (!this.inventoryId) {
            console.error('Inventory ID not found');
            return;
        }

        this.initializeSignalR();
        this.initializeEventListeners();

        const chatTab = document.getElementById('chat-tab');
        if (chatTab && chatTab.classList.contains('active')) {
            this.loadMessageHistory();
        }
    }


    initializeSignalR() {
        console.log('Initializing SignalR connection...');

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Добавьте обработчики для отладки
        this.connection.onreconnecting(() => {
            console.log('SignalR reconnecting...');
            this.updateConnectionStatus(false);
        });

        this.connection.onreconnected(() => {
            console.log('SignalR reconnected');
            this.updateConnectionStatus(true);
        });

        this.connection.onclose(() => {
            console.log('SignalR connection closed');
            this.updateConnectionStatus(false);
        });

        // Остальные обработчики...
        this.connection.on("ReceiveMessage", (message) => {
            console.log('Received message:', message);
            this.addMessageToChat(message);
        });

        this.startConnection();
    }

    async startConnection() {
        // Если соединение уже запускается или активно, не пытаемся переподключиться
        if (this.connection.state === signalR.HubConnectionState.Connecting ||
            this.connection.state === signalR.HubConnectionState.Connected) {
            console.log('Connection already in progress or established');
            return;
        }

        try {
            await this.connection.start();
            console.log('SignalR Connected');
            this.isConnected = true;

            // Присоединяемся к группе инвентаря
            try {
                await this.connection.invoke("JoinInventoryGroup", this.inventoryId);
                console.log('Successfully joined inventory group:', this.inventoryId);
            } catch (joinError) {
                console.error('Error joining inventory group:', joinError);
                // Не прерываем соединение из-за ошибки присоединения к группе
            }

            this.updateConnectionStatus(true);
        } catch (err) {
            console.error('SignalR Connection Error: ', err);
            this.updateConnectionStatus(false);

            // Останавливаем соединение перед повторной попыткой
            try {
                await this.connection.stop();
            } catch (stopError) {
                console.log('Error stopping connection:', stopError);
            }

            // Пытаемся переподключиться через 5 секунд
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    initializeEventListeners() {
        const messageForm = document.getElementById('messageForm');
        const messageInput = document.getElementById('messageInput');
        const attachButton = document.getElementById('attachButton');
        const fileInput = document.getElementById('fileInput');
        const chatMessages = document.getElementById('chatMessages');
        const chatTab = document.getElementById('chat-tab');

        // Проверяем существование элементов перед добавлением обработчиков
        if (!messageForm || !messageInput || !attachButton || !fileInput || !chatMessages) {
            console.error('One or more chat elements not found in DOM');
            return;
        }

        // Обработчик переключения на вкладку чата
        if (chatTab) {
            chatTab.addEventListener('shown.bs.tab', () => {
                console.log('Chat tab activated');
                this.loadMessageHistory();
                this.scrollToBottom();
            });
        }

        // Отправка сообщения
        messageForm.addEventListener('submit', (e) => {
            e.preventDefault();
            this.sendMessage();
        });

        // Enter для отправки, Shift+Enter для новой строки
        messageInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });

        // Прикрепление файла
        attachButton.addEventListener('click', () => {
            fileInput.click();
        });

        fileInput.addEventListener('change', (e) => {
            this.handleFileSelect(e);
        });

        // Emoji button (если нужно)
        const emojiButton = document.getElementById('emojiButton');
        if (emojiButton) {
            emojiButton.addEventListener('click', () => {
                this.showEmojiPicker();
            });
        }

        // Auto-scroll to bottom when new messages arrive
        const observer = new MutationObserver(() => {
            this.scrollToBottom();
        });

        observer.observe(chatMessages, { childList: true, subtree: true });

        // Load more messages when scrolling to top
        chatMessages.addEventListener('scroll', () => {
            if (chatMessages.scrollTop === 0) {
                this.loadMoreMessages();
            }
        });

        // Handle connection state changes
        this.connection.onreconnecting(() => {
            this.updateConnectionStatus(false);
        });

        this.connection.onreconnected(() => {
            this.updateConnectionStatus(true);
        });
    }

    async sendMessage() {
        const messageInput = document.getElementById('messageInput');
        const messageText = messageInput.value.trim();

        if (!messageText && !this.currentFile) {
            return;
        }

        if (!this.isConnected) {
            alert('Connection lost. Please try again.');
            return;
        }

        try {
            const messageDto = {
                inventoryId: parseInt(this.inventoryId),
                message: messageText,
                messageType: this.currentFile ? 1 : 0, // 1 = File, 0 = Text
                file: this.currentFile
            };

            await this.connection.invoke("SendMessage", messageDto);

            // Очищаем форму
            messageInput.value = '';
            this.clearFilePreview();

        } catch (err) {
            console.error('Error sending message: ', err);
            alert('Error sending message. Please try again.');
        }
    }

    handleFileSelect(event) {
        const file = event.target.files[0];
        if (!file) return;

        // Проверяем размер файла (максимум 10MB)
        if (file.size > 10 * 1024 * 1024) {
            alert('File size must be less than 10MB');
            return;
        }

        this.currentFile = file;
        this.showFilePreview(file);
    }

    showFilePreview(file) {
        const filePreview = document.getElementById('filePreview');
        const messageInput = document.getElementById('messageInput');

        filePreview.innerHTML = `
            <div class="d-flex align-items-center justify-content-between p-2 border rounded bg-light">
                <div class="d-flex align-items-center">
                    <i class="fas fa-file me-2 text-primary"></i>
                    <span class="small">${file.name} (${this.formatFileSize(file.size)})</span>
                </div>
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="chat.removeFile()">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;
        filePreview.style.display = 'block';

        // Добавляем имя файла в сообщение
        if (!messageInput.value.includes(file.name)) {
            messageInput.value = messageInput.value ?
                `${messageInput.value} [File: ${file.name}]` :
                `[File: ${file.name}]`;
        }
    }

    removeFile() {
        this.currentFile = null;
        this.clearFilePreview();

        // Убираем упоминание файла из текста
        const messageInput = document.getElementById('messageInput');
        messageInput.value = messageInput.value.replace(/\[File: [^\]]+\]/, '').trim();
    }

    clearFilePreview() {
        const filePreview = document.getElementById('filePreview');
        const fileInput = document.getElementById('fileInput');

        filePreview.style.display = 'none';
        filePreview.innerHTML = '';
        fileInput.value = '';
    }

    async loadMessageHistory() {
        try {
            const response = await fetch(`/api/chat/messages/${this.inventoryId}?take=50`);
            if (response.ok) {
                const messages = await response.json();
                this.displayMessageHistory(messages);
            }
        } catch (err) {
            console.error('Error loading message history: ', err);
        } finally {
            //document.getElementById('loadingMessages').style.display = 'none';
        }
    }

    async loadMoreMessages() {
        const chatMessages = document.getElementById('chatMessages');
        const firstMessage = chatMessages.querySelector('.chat-message');

        if (!firstMessage || this.isLoadingMore) return;

        this.isLoadingMore = true;

        try {
            const skip = this.messages.length;
            const response = await fetch(`/api/chat/messages/${this.inventoryId}?skip=${skip}&take=20`);

            if (response.ok) {
                const newMessages = await response.json();
                if (newMessages.length > 0) {
                    this.prependMessages(newMessages);
                }
            }
        } catch (err) {
            console.error('Error loading more messages: ', err);
        } finally {
            this.isLoadingMore = false;
        }
    }

    displayMessageHistory(messages) {
        this.messages = messages;
        const chatMessages = document.getElementById('chatMessages');

        chatMessages.innerHTML = messages.map(message =>
            this.createMessageElement(message)
        ).join('');

        this.scrollToBottom();
    }

    prependMessages(messages) {
        this.messages = [...messages, ...this.messages];
        const chatMessages = document.getElementById('chatMessages');
        const scrollHeightBefore = chatMessages.scrollHeight;
        const scrollTop = chatMessages.scrollTop;

        const newMessagesHtml = messages.map(message =>
            this.createMessageElement(message)
        ).join('');

        chatMessages.insertAdjacentHTML('afterbegin', newMessagesHtml);

        // Сохраняем позицию скролла
        const newScrollHeight = chatMessages.scrollHeight;
        chatMessages.scrollTop = scrollTop + (newScrollHeight - scrollHeightBefore);
    }

    addMessageToChat(message) {
        this.messages.push(message);
        const chatMessages = document.getElementById('chatMessages');

        chatMessages.insertAdjacentHTML('beforeend', this.createMessageElement(message));

        // Показываем уведомление если чат не в фокусе
        if (!this.isChatVisible() && message.userId !== this.getCurrentUserId()) {
            this.showNotification(message);
        }
    }

    createMessageElement(message) {
        const isCurrentUser = message.userId === this.getCurrentUserId();
        const messageClass = isCurrentUser ? 'chat-message current-user' : 'chat-message';
        const time = new Date(message.createdAt).toLocaleTimeString([], {
            hour: '2-digit', minute: '2-digit'
        });

        let messageContent = '';

        if (message.messageType === 1) { // File message
            messageContent = `
                <div class="file-attachment">
                    <a href="${message.fileUrl}" target="_blank" class="file-link">
                        <i class="fas fa-file-download me-1"></i>
                        ${message.message || 'Download file'}
                    </a>
                </div>
            `;
        } else if (message.messageType === 2) { // System message
            return `
                <div class="system-message text-center text-muted small my-2">
                    <em>${message.message}</em>
                </div>
            `;
        } else { // Text message
            // Basic markdown support
            const formattedMessage = this.formatMessage(message.message);
            messageContent = `<div class="message-text">${formattedMessage}</div>`;
        }

        return `
            <div class="${messageClass}" data-message-id="${message.id}">
                <div class="message-header">
                    <strong class="user-name">${message.userName}</strong>
                    <span class="message-time">${time}</span>
                    ${message.isEdited ? '<span class="edited-badge">edited</span>' : ''}
                </div>
                <div class="message-content">
                    ${messageContent}
                </div>
                ${isCurrentUser ? this.createMessageActions(message.id) : ''}
            </div>
        `;
    }

    createMessageActions(messageId) {
        return `
            <div class="message-actions">
                <button class="btn btn-sm btn-outline-secondary" onclick="chat.editMessage(${messageId})">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" onclick="chat.deleteMessage(${messageId})">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;
    }

    formatMessage(text) {
        if (!text) return ''; // Защита от undefined/null

        // Простая поддержка markdown
        return text
            .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>') // **bold**
            .replace(/\*(.*?)\*/g, '<em>$1</em>') // *italic*
            .replace(/`(.*?)`/g, '<code>$1</code>') // `code`
            .replace(/\n/g, '<br>'); // Переносы строк
    }

    addSystemMessage(text) {
        const systemMessage = {
            id: Date.now(),
            message: text,
            messageType: 2,
            createdAt: new Date().toISOString(),
            userName: 'System'
        };

        this.addMessageToChat(systemMessage);
    }

    updateMessage(messageId, newMessage) {
        const messageElement = document.querySelector(`[data-message-id="${messageId}"]`);
        if (messageElement) {
            const messageText = messageElement.querySelector('.message-text');
            if (messageText) {
                messageText.innerHTML = this.formatMessage(newMessage);
            }

            // Добавляем badge "edited"
            const editedBadge = messageElement.querySelector('.edited-badge') ||
                document.createElement('span');
            editedBadge.className = 'edited-badge';
            editedBadge.textContent = 'edited';

            if (!messageElement.querySelector('.edited-badge')) {
                messageElement.querySelector('.message-header').appendChild(editedBadge);
            }
        }
    }

    removeMessage(messageId) {
        const messageElement = document.querySelector(`[data-message-id="${messageId}"]`);
        if (messageElement) {
            messageElement.remove();
        }
    }

    async editMessage(messageId) {
        const message = this.messages.find(m => m.id === messageId);
        if (!message) return;

        const newMessage = prompt('Edit your message:', message.message);
        if (newMessage && newMessage !== message.message) {
            try {
                await this.connection.invoke("EditMessage", messageId, newMessage);
            } catch (err) {
                console.error('Error editing message: ', err);
                alert('Error editing message');
            }
        }
    }

    async deleteMessage(messageId) {
        if (confirm('Are you sure you want to delete this message?')) {
            try {
                await this.connection.invoke("DeleteMessage", messageId);
            } catch (err) {
                console.error('Error deleting message: ', err);
                alert('Error deleting message');
            }
        }
    }

    updateOnlineCount(count) {
        const onlineCount = document.getElementById('onlineCount');
        if (onlineCount) {
            onlineCount.textContent = `${count} online`;
        }
    }

        updateConnectionStatus(connected) {
            const statusIndicator = document.getElementById('connectionStatus');
            if (statusIndicator) {
                if (connected) {
                    statusIndicator.className = 'badge bg-success';
                    statusIndicator.innerHTML = '<i class="fas fa-circle me-1"></i>Connected';
                } else {
                    statusIndicator.className = 'badge bg-danger';
                    statusIndicator.innerHTML = '<i class="fas fa-circle me-1"></i>Connecting...';
                }
            }
        }

    scrollToBottom() {
        const chatMessages = document.getElementById('chatMessages');
        if (chatMessages) {
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }
    }

    isChatVisible() {
        const chatOffcanvas = document.getElementById('chatOffcanvas');
        return chatOffcanvas && chatOffcanvas.classList.contains('show');
    }

    showNotification(message) {
        // Проверяем поддержку Notification API
        if ("Notification" in window && Notification.permission === "granted") {
            new Notification(`New message from ${message.userName}`, {
                body: message.messageType === 1 ? 'Sent a file' : message.message,
                icon: '/favicon.ico'
            });
        }
    }

    getCurrentUserId() {
        return document.getElementById('currentUserId')?.value || '';
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    requestNotificationPermission() {
        if ("Notification" in window && Notification.permission === "default") {
            Notification.requestPermission();
        }
    }

    destroy() {
        if (this.connection) {
            this.connection.stop();
        }
    }
}

let chat;

document.addEventListener('DOMContentLoaded', function () {
    const chatContainer = document.getElementById('chat');
    if (!chatContainer) {
        console.log('Chat container not found on this page');
        return;
    }

    chat = new InventoryChat();
    chat.initialize();
    chat.requestNotificationPermission();
});
window.chat = {
    removeFile: function () {
        chat?.removeFile();
    },
    editMessage: function (messageId) {
        chat?.editMessage(messageId);
    },
    deleteMessage: function (messageId) {
        chat?.deleteMessage(messageId);
    }
};