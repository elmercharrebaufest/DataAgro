$(document).ready(function () {
    try {
        initialiseUI();
    }
    catch (error) {
        console.error(error);
    }
    
});
function initialiseUI() {
    MSExecuteOnServerAsync('/CompraNet/UsuarioSuscripto', null,
        function (result) {
            if (result == true) {
                $("#suscribirNotificacion").prop("checked", true);
                subscribeUser();
            }
        }, true
    );
    

    $("#suscribirNotificacion").on("click",
        function requestPushNotification() {
            var $ctrl = $(this);
            if ($ctrl.is(":checked")) {
                console.log("checked");
                subscribeUser();
            } else {
                console.log("unchecked");
                unsubscribeUser();
            }
        });
}

function subscribeUser() {
    var isSubscribed = false;
    var messaging = firebase.messaging();
    messaging.requestPermission()
        .then(function () {
            messaging.getToken()
                .then(function (currentToken) {
                    if (currentToken) {
                        console.log(currentToken);
                        updateSubscriptionOnServer(currentToken);
                        isSubscribed = true;
                    } else {
                        updateSubscriptionOnServer(null);
                    }
                    $("#suscribirNotificacion").prop('checked', isSubscribed);
                })
                .catch(function (err) {
                    isSubscribed = false;
                    updateSubscriptionOnServer(null);
                });
        })
        .catch(function (err) {
            console.log('Unable to get permission to notify.', err);
        });
}

function unsubscribeUser() {
    var messaging = firebase.messaging();
    messaging.getToken()
        .then(function (currentToken) {
            messaging.deleteToken(currentToken)
                .then(function () {
                    updateSubscriptionOnServer(null);
                })
                .catch(function (err) {
                    console.log('Unable to delete token. ', err);
                });
        })
        .catch(function (err) {
            console.log('Error retrieving Instance ID token. ', err);
        });
}

function updateSubscriptionOnServer(subscription) {
    var subscriptionDetail = { key: "" };
    if (subscription) {
        subscriptionDetail = { key: subscription };
    } else {
        console.log("delete on the server the token");
    }
    var dateToSent = subscriptionDetail;
    var result = MSExecuteOnServer('/CompraNet/SuscripcionNotificaciones', dateToSent)
    //$.ajax({
    //    url: apiUrl,
    //    type: 'POST',
    //    data: dateToSent,
    //    cache: true,
    //    dataType: 'json',
    //    success: function (json) {
    //        if (json.IsValid) {
    //        } else {
    //        }
    //    },
    //    error: function (xmlHttpRequest, textStatus, errorThrown) {
    //        console.log('some error occured', textStatus, errorThrown);
    //    },
    //    always: function () {
    //    }
    //});

}