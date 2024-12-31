// service-worker.js

self.addEventListener('push', function (event) {
    if (event.data) {
        const data = event.data.json();
        const options = {
            body: data.message,
            icon: '/assets/images/messenger-window-icon-3d-render-illustration-isolated-white-background.jpg', // Update the path as needed
            badge: '/assets/images/messenger-window-icon-3d-render-illustration-isolated-white-background.jpg', // Optional
            data: {
                url: data.url || '/BossMessaging' // URL to open when notification is clicked
            }
        };
        event.waitUntil(
            self.registration.showNotification(data.title, options)
        );
    }
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();
    const url = event.notification.data.url;
    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then(function (clientList) {
            for (const client of clientList) {
                if (client.url === url && 'focus' in client) {
                    return client.focus();
                }
            }
            if (clients.openWindow) {
                return clients.openWindow(url);
            }
        })
    );
});
