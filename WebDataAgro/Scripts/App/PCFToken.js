
    
$(document).ajaxSend(function (event, jqXHR, ajaxSettings) {
    var type = ajaxSettings.type.toUpperCase();
    if (["POST"].indexOf(type) != -1)   {
        var $token = $("[name='__RequestVerificationToken']");
        if ($token.length > 0) {
            var token = $token.first().val();
            jqXHR.setRequestHeader("__RequestVerificationToken", token);
        }
    }

    // 
});
