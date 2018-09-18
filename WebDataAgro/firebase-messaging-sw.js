importScripts('https://www.gstatic.com/firebasejs/5.5.0/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/5.5.0/firebase-messaging.js');


var config = {
    apiKey: "AIzaSyAoYLUaU77nDrO17zgt1pR_eeAitk_4md0",
    authDomain: "dataagro-786eb.firebaseapp.com",
    databaseURL: "https://dataagro-786eb.firebaseio.com",
    projectId: "dataagro-786eb",
    storageBucket: "dataagro-786eb.appspot.com",
    messagingSenderId: "93653202795"
};
firebase.initializeApp(config);
 
var messaging = firebase.messaging();
messaging.setBackgroundMessageHandler(function (payload) {
    var dataFromServer = JSON.parse(payload.data.notification);
    var notificationTitle = dataFromServer.title;
    var notificationOptions = {
        body: dataFromServer.body,
        icon: dataFromServer.icon,
        data: {
            url:dataFromServer.url
        }
    };
    return self.registration.showNotification(notificationTitle,
        notificationOptions);
});
 
self.addEventListener("notificationclick", function (event)
{
    var urlToRedirect = event.notification.data.url;
    event.notification.close();
    event.waitUntil(self.clients.openWindow(urlToRedirect));
});