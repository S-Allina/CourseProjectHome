class InventoryChat {
    constructor() {
        this.connection = null;
        this.inventoryId = null;
        this.isConnected = false;
    }

    initialize() {
        const inventoryIdElement = document.getElementById('inventoryId');
        if (!inventoryIdElement) {
            console.error('Inventory ID element not found');
            return;
        }

        this.inventoryId = parseInt(inventoryIdElement.value);
        console.log('Initializing chat for inventory:', this.inventoryId);

        this.initializeSignalR();
        this.setupEventListeners();
    }

    setupEventListeners() {
        const messageForm = document.getElementById('messageForm');
        const messageInput = document.getElementById('messageInput');
        const sendButton = document.getElementById('sendButton');

        if (messageForm) {
            messageForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.sendMessage();
            });
        }

        if (sendButton) {
            sendButton.addEventListener('click', (e) => {
                e.preventDefault();
                this.sendMessage();
            });
        }

        if (messageInput) {
            messageInput.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    this.sendMessage();
                }
            });

            messageInput.addEventListener('input', function () {
                this.style.height = 'auto';
                this.style.height = Math.min(this.scrollHeight, 120) + 'px';
            });
        }
    }

    initializeSignalR() {
        console.log('Initializing SignalR connection...');

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.on("ReceiveMessage", (message) => {
            console.log('Received message object:', message);

            if (typeof message === 'string') {
            } else if (typeof message === 'object') {
                this.addMessageObject(message);
            } else {
                console.warn('Unknown message format:', message);
            }
        });

        this.connection.on("LoadMessageHistory", (messages) => {
            console.log('Loading message history:', messages);
            this.displayMessageHistory(messages);
        });

        this.connection.onreconnecting((error) => {
            console.log('Connection reconnecting...', error);
            this.updateStatus('Reconnecting...');
        });

        this.connection.onreconnected((connectionId) => {
            console.log('Connection reconnected:', connectionId);
            this.updateStatus('Connected');
            this.joinGroup();
        });

        this.connection.onclose((error) => {
            console.log('Connection closed:', error);
            this.updateStatus('Disconnected');
        });

        this.startConnection();
    }

    async startConnection() {
        try {
            console.log('Starting SignalR connection...');
            await this.connection.start();
            console.log('SignalR connected successfully');
            this.updateStatus('Connected');

            await this.joinGroup();
        } catch (err) {
            console.error('SignalR connection failed:', err);
            this.updateStatus('Connection failed');
            this.addMessage('System', 'Connection failed: ' + err.message);

            setTimeout(() => this.startConnection(), 5000);
        }
    }

    async joinGroup() {
        if (!this.connection || this.connection.state !== 'Connected') {
            console.log('Cannot join group - not connected');
            return;
        }

        try {
            console.log('Joining inventory group:', this.inventoryId);
            await this.connection.invoke("JoinInventoryGroup", this.inventoryId);
        } catch (err) {
            console.error('Error joining group:', err);
            this.addMessage('System', 'Error joining group: ' + err.message);
        }
    }

    async sendMessage() {
        const messageInput = document.getElementById('messageInput');
        if (!messageInput) {
            console.error('Message input not found');
            return;
        }

        const message = messageInput.value.trim();
        if (!message) {
            return;
        }

        if (!this.connection || this.connection.state !== 'Connected') {
            this.addMessage('System', 'Not connected to chat. Please wait...');
            return;
        }

        console.log('Sending message:', { inventoryId: this.inventoryId, message });

        try {
            messageInput.value = '';
            messageInput.style.height = 'auto';

            await this.connection.invoke("SendMessage", this.inventoryId, message);
            console.log('Message sent successfully');

        } catch (err) {
            console.error('Error sending message:', err);
            messageInput.value = message;
            this.addMessage('System', 'Failed to send message: ' + err.message);
        }
    }

    addMessageObject(message) {
        const messagesDiv = document.getElementById('chatMessages');
        if (!messagesDiv) {
            console.error('Chat messages container not found');
            return;
        }

        const loadingIndicator = document.getElementById('loadingMessages');
        if (loadingIndicator) {
            loadingIndicator.style.display = 'none';
        }

        const messageElement = document.createElement('div');
        messageElement.className = 'chat-message';

        const currentUserId = document.getElementById('currentUserId')?.value;
        const isCurrentUser = message.userId === currentUserId;

        if (isCurrentUser) {
            messageElement.classList.add('current-user');
        }

        messageElement.innerHTML = `
            <div class="message-header">
                <strong class="user-name">${this.escapeHtml(message.userName)}</strong>
                <span class="message-time">${message.formattedTime}</span>
            </div>
            <div class="message-text">${message.messageHtml}</div>
        `;

        messagesDiv.appendChild(messageElement);
        this.scrollToBottom();
    }

    addMessage(user, message) {
        const messagesDiv = document.getElementById('chatMessages');
        if (!messagesDiv) {
            console.error('Chat messages container not found');
            return;
        }

        const loadingIndicator = document.getElementById('loadingMessages');
        if (loadingIndicator) {
            loadingIndicator.style.display = 'none';
        }

        const messageElement = document.createElement('div');
        messageElement.className = 'chat-message';

        if (user === 'System') {
        } else {
            messageElement.innerHTML = `<strong>${this.escapeHtml(user)}:</strong> ${this.escapeHtml(message)}`;
        }

        messagesDiv.appendChild(messageElement);
        this.scrollToBottom();
    }

    displayMessageHistory(messages) {
        const messagesDiv = document.getElementById('chatMessages');
        if (!messagesDiv) {
            return;
        }

        const loadingIndicator = document.getElementById('loadingMessages');
        if (loadingIndicator) {
            loadingIndicator.style.display = 'none';
        }

        messagesDiv.innerHTML = '';

        if (messages && messages.length > 0) {
            messages.forEach(message => {
                this.addMessageObject(message);
            });
        } else {
            messagesDiv.innerHTML = '<div class="text-center text-muted py-4">No messages yet</div>';
        }

        this.scrollToBottom();
    }

    scrollToBottom() {
        const messagesDiv = document.getElementById('chatMessages');
        if (messagesDiv) {
            messagesDiv.scrollTop = messagesDiv.scrollHeight;
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    updateStatus(status) {
        const statusElement = document.getElementById('connectionStatus');
        if (!statusElement) {
            return;
        }

        const statusText = statusElement.querySelector('.status-text');
        if (statusText) {
            statusText.textContent = status;
        } else {
            if (status === 'Connected') {
                statusElement.innerHTML = '<i class="fas fa-circle me-1"></i><span class="status-text">Connected</span>';
                statusElement.className = 'badge bg-success';
            } else if (status === 'Reconnecting...') {
                statusElement.innerHTML = '<i class="fas fa-circle me-1"></i><span class="status-text">Reconnecting...</span>';
                statusElement.className = 'badge bg-warning';
            } else {
                statusElement.innerHTML = '<i class="fas fa-circle me-1"></i><span class="status-text">Disconnected</span>';
                statusElement.className = 'badge bg-danger';
            }
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
    console.log('DOM loaded, initializing chat...');
    const chatContainer = document.getElementById('chat');
    if (!chatContainer) {
        console.log('Chat container not found on this page');
        return;
    }

    chat = new InventoryChat();
    chat.initialize();
});