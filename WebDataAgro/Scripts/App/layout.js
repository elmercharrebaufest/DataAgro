var notif = 0;
var htmlnotifaux = "";
var options = {};
var strFiltro = "";
var notificaciones = [];
var filtro;

function ArmarNotificaciones() {
    var result = MSExecuteOnServer('/Home/TraerActividadesPorComercialId');

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            $(".notificaciones-contenedor").empty();

            notificaciones = result.Actividades;

            notif = notificaciones.length;

            for (var ii in notificaciones) {
                (function (i) {
                    var url = MSGetUrl("/Content/Images/notificaciones-reloj.png");
                    if (notificaciones[i].ProveedorId != 0) {
                        htmlnotifaux += '<div class="notificacion-detalle">'
                            + '<div class="notificacion-detalle-horario">'
                            + '<img class="notificacion-reloj" src="..' + url + '" />'
                            + '<span class="notificacion-hora"> ' + notificaciones[i].Dia + " " + notificaciones[i].Hora + '</span>'
                            + '</div>'
                            + '<div class="notificacion-detalle-contacto">'
                            + notificaciones[i].Contacto
                            + '</div>'
                            + '<div class="notificacion-detalle-titulo">'
                            + notificaciones[i].Tema
                            + '</div>'
                            + '<div class="notificacion-detalle-descripcion">'
                            + notificaciones[i].Comentarios
                            + '</div>'
                            + '</div>';
                    } else {
                        htmlnotifaux += '<div class="notificacion-detalle">'
                            + '<div class="notificacion-detalle-horario class="notificacion-detalle-titulo">'
                            + '<img class="notificacion-reloj" src="..' + url + ' " />'
                            + notificaciones[i].Tema
                            + '<div class="notificacion-detalle-descripcion">'
                            + notificaciones[i].Comentarios
                            + '</div>'
                            + '<span class="notificacion-hora"> Hasta el: ' + notificaciones[i].Dia + '</span>'
                            + '</div>'
                            + '</div>';
                    }
                })(ii);
            }

            if (notif > 0) {
                setTimeout(function () {
                    $(".notificaciones-contenedor").append(htmlnotifaux);
                }, 300);
            } else {
                $(".notificaciones-contenedor").remove();
            }
        }
    }
}

function mobile() {
    var ww = document.body.clientWidth;

    if (ww < 578) {
        $(".li-contacto").attr("display", "inline-block");
        $(".li-contacto").removeClass("float-right");
    }
    else {
        $(".li-contacto").addClass("float-right");
    }

    if (ww < 750) {
        $(".padding").css({ paddingTop: "0px" });

        $("#segundo").addClass("left");
        $("#segundo").appendTo(".navbar-header");
        $("#segundo").addClass("floatito");
        $(".icons").addClass("float-left");
        $(".imgs").removeClass("padding");

        $(".nav-text1").removeClass("dropdown-toggle");
        $(".nav-text1").removeAttr("data-toggle").attr("data-toggle", "collapse").attr("data-target", "#reportes");
        $("#informes").removeClass("dropdown-menu").addClass("collapse navbar-collapse");

        $(".nav-text2").removeClass("dropdown-toggle");
        $(".nav-text2").removeAttr("data-toggle").attr("data-toggle", "collapse").attr("data-target", "#informes");
        $("#reportes").removeClass("dropdown-menu").addClass("collapse navbar-collapse");

        $(".nav-text3").removeClass("dropdown-toggle");
        $(".nav-text3").removeAttr("data-toggle").attr("data-toggle", "collapse").attr("data-target", "#tablas");
        $("#tablas").removeClass("dropdown-menu").addClass("collapse navbar-collapse");

        $(".nav-text4").removeClass("dropdown-toggle");
        $(".nav-text4").removeAttr("data-toggle").attr("data-toggle", "collapse").attr("data-target", "#negocios");
        $("#negocios").removeClass("dropdown-menu").addClass("collapse navbar-collapse");


        $(".nav-text6").removeClass("dropdown-toggle");
        $(".nav-text6").removeAttr("data-toggle").attr("data-toggle", "collapse").attr("data-target", "#cupos");
        $("#cupos").removeClass("dropdown-menu").addClass("collapse navbar-collapse");

        $(".nav-text5").removeClass("dropdown-toggle");
        $(".nav-text5").removeAttr("data-toggle").attr("data-toggle", "collapse").attr("data-target", "#research");
        $("#research").removeClass("dropdown-menu").addClass("collapse navbar-collapse");
    } else {

        $("#segundo").removeClass("left");
        $("#segundo").removeClass("floatito");
        $("#segundo").appendTo("#myNavbar");
        $(".icons").removeClass("float-left");
        $(".imgs").addClass("padding");

        $(".nav-text1").addClass("dropdown-toggle");
        $(".nav-text1").removeAttr("data-toggle").attr("data-toggle", "dropdown").removeAttr("data-target");
        $("#informes").addClass("dropdown-menu").removeClass("collapse navbar-collapse");

        $(".nav-text2").addClass("dropdown-toggle");
        $(".nav-text2").removeAttr("data-toggle").attr("data-toggle", "dropdown").removeAttr("data-target");
        $("#reportes").addClass("dropdown-menu").removeClass("collapse navbar-collapse");

        $(".nav-text3").addClass("dropdown-toggle");
        $(".nav-text3").removeAttr("data-toggle").attr("data-toggle", "dropdown").removeAttr("data-target");
        $("#tablas").addClass("dropdown-menu").removeClass("collapse navbar-collapse");

        $(".nav-text4").addClass("dropdown-toggle");
        $(".nav-text4").removeAttr("data-toggle").attr("data-toggle", "dropdown").removeAttr("data-target");
        $("#negocios").addClass("dropdown-menu").removeClass("collapse navbar-collapse");

        $(".nav-text5").addClass("dropdown-toggle");
        $(".nav-text5").removeAttr("data-toggle").attr("data-toggle", "dropdown").removeAttr("data-target");
        $("#research").addClass("dropdown-menu").removeClass("collapse navbar-collapse");

        $(".nav-text6").addClass("dropdown-toggle");
        $(".nav-text6").removeAttr("data-toggle").attr("data-toggle", "dropdown").removeAttr("data-target");
        $("#cupos").addClass("dropdown-menu").removeClass("collapse navbar-collapse");

    }
    if (window.innerWidth < 768) {
        $("#AgregarContacto2").attr("style", "display:none;");
        $("#menuproveedor").removeAttr("hidden");
        $("#menuproveedor").removeAttr("style");
        $("#buscar-proveedor").removeAttr("style");
        /*$("#findproveedor").attr("hidden", "hidden");*/
        $("#findproveedor").attr("style", "display:none;");
        $("#segundo").addClass("left");
        $("#segundo").appendTo(".navbar-header");
        $("#segundo").addClass("floatito");
        $(".icons").addClass("float-left");
        $(".imgs").removeClass("padding");
    } else {
        $("#AgregarContacto2").removeAttr("style");
        /*$("#menuproveedor").attr("hidden", "hidden");*/
        $("#menuproveedor").attr("style", "display: none;");
        $("#buscar-proveedor").attr("style", "display:none;");
        $("#findproveedor").removeAttr("style");
        $("#segundo").removeClass("left");
        $("#segundo").removeClass("floatito");
        $("#segundo").appendTo("#myNavbar");
        $(".icons").removeClass("float-left");
        $(".imgs").addClass("padding");
    }
}

$(document).ready(function () {
    var ww = document.body.clientWidth;
    mobile();
    OcultarActividadesMobile();

    $(".miscontactos-nav").parent().attr("href", window.location.origin);

    ArmarNotificaciones();
    $("#notificaciones-a").hover(function () {
        var alt = 70 + $(document).scrollTop();
        $(".notificaciones-contenedor").show().css({
            left: $("#notificaciones-a").offset().left - 117,
            top: alt,
            overflow: 'auto',
            'max-height': '280px'
        });
    }, function () {
        $(".notificaciones-contenedor").hide();
    });

    $(".notificaciones-contenedor").hover(function () {
        var alt = 60 + $(document).scrollTop();
        $(".notificaciones-contenedor").show().css({
            left: $(this).offset().left,
            top: alt,
            overflow: 'auto',
            'max-height': '280px'
        });
    }, function () {
        $(".notificaciones-contenedor").hide();
    });

    //if (notif > 0) {
    $(".cant-notif-span").html(notif);
    /*} else {
        $(".cant-notif-span").parent().remove();
    }*/

    /*Reportes */

    $("#reportes-a").hover(function () {
        var alt = 60 + $(document).scrollTop();
        $(".reporte-detalle").show().css({
            left: $(this).offset().left,
            top: alt,
            overflow: 'auto',
            'max-height': '280px'
        });
    }, function () {
        $(".reporte-detalle").hide();
    });

    $(".reporte-detalle").hover(function () {
        var alt = 60 + $(document).scrollTop();
        $(".reporte-detalle").show().css({
            left: $(this).offset().left,
            top: alt,
            overflow: 'auto',
            'max-height': '280px'
        });
    }, function () {
        $(".reporte-detalle").hide();
    });

    if ($("#tablas-a").length > 0) {
        $("#tablas-a").hover(function () {
            var alt = 60 + $(document).scrollTop();
            $(".tablas-detalle").show().css({
                left: $(this).offset().left,
                top: alt,
                overflow: 'auto',
                'max-height': '280px'
            });

            if (($(this).offset().left + $(".tablas-detalle").width()) > $(window).width()) {
                $(".tablas-detalle").show().css({
                    left: 'auto',
                    right: $(window).width() - ($(".mis-tablas-li").offset().left + $(".mis-tablas-li").width()),
                    top: alt,
                    overflow: 'auto',
                    'max-height': '280px'
                });
            }
        }, function () {
            $(".tablas-detalle").hide();
        });

        $(".tablas-detalle").hover(function () {
            var alt = 60 + $(document).scrollTop();
            $(".tablas-detalle").show().css({
                left: $(this).offset().left,
                top: alt,
                overflow: 'auto',
                'max-height': '280px'
            });

            if (($(this).offset().left + $(".tablas-detalle").width()) > $(window).width()) {
                $(".tablas-detalle").show().css({
                    left: 'auto',
                    right: $(window).width() - ($(".mis-tablas-li").offset().left + $(".mis-tablas-li").width()),
                    top: alt,
                    overflow: 'auto',
                    'max-height': '280px'
                });
            }
        }, function () {
            $(".tablas-detalle").hide();
        });
    }

    if ($("#compraNet-a").length > 0) {
        $("#compraNet-a").hover(function () {
            var alt = 60 + $(document).scrollTop();
            $(".compraNet-detalle").show().css({
                left: $(this).offset().left,
                top: alt,
                overflow: 'auto',
                'max-height': '280px'
            });

            if (($(this).offset().left + $(".compraNet-detalle").width()) > $(window).width()) {
                $(".compraNet-detalle").show().css({
                    left: 'auto',
                    right: $(window).width() - ($(".mis-tablas-li").offset().left + $(".mis-tablas-li").width()),
                    top: alt,
                    overflow: 'auto',
                    'max-height': '280px'
                });
            }
        }, function () {
            $(".compraNet-detalle").hide();
        });

        $(".compraNet-detalle").hover(function () {
            var alt = 60 + $(document).scrollTop();
            $(".compraNet-detalle").show().css({
                left: $(this).offset().left,
                top: alt,
                overflow: 'auto',
                'max-height': '280px'
            });

            if (($(this).offset().left + $(".compraNet-detalle").width()) > $(window).width()) {
                $(".compraNet-detalle").show().css({
                    left: 'auto',
                    right: $(window).width() - ($(".mis-tablas-li").offset().left + $(".mis-tablas-li").width()),
                    top: alt,
                    overflow: 'auto',
                    'max-height': '280px'
                });
            }
        }, function () {
            $(".compraNet-detalle").hide();
        });
    }

    $("#informes-a").hover(function () {
        var alt = 60 + $(document).scrollTop();
        $(".informe-detalle").show().css({
            left: $(this).offset().left,
            top: alt,
            overflow: 'auto',
            'max-height': '280px'
        });
    }, function () {
        $(".informe-detalle").hide();
    });

    $(".informe-detalle").hover(function () {
        var alt = 60 + $(document).scrollTop();
        $(".informe-detalle").show().css({
            left: $(this).offset().left,
            top: alt,
            overflow: 'auto',
            'max-height': '280px'
        });
    }, function () {
        $(".informe-detalle").hide();
    });

    $("#buscar-proveedor").click(function () {
        var menu = $('#menuproveedor');
        if (menu.is(':visible')) {
            menu.hide();
        } else {
            menu.show();
        }
    });

    $(".buscador-nav-input").keyup(function (e) {
        armarBusquedaResult(e, $(this).attr('id'));
    });

    $(".buscador-nav-input").focus(function (e) {
        e.stopPropagation();
        e.preventDefault();
        armarBusquedaResult(e, $(this).attr('id'));
    })

    $(".mis-postit").click(function (e) {
        e.stopPropagation();
        e.preventDefault();

        InicializarPost();
    });

    $("#guardar-postit").click(function () {
        guardarPost();
    });

    $(".salir-postit").click(function () {
        $("#modalPostit").modal('hide');
    });
});

function InicializarPost() {
    result = MSExecuteOnServer('/Home/TraerPostIt');
    if (result) {
        $("#texto-postit").val(result.Texto);
        $("#modalPostit").modal();
    }
}

function guardarPost() {
    var obj = {};

    obj.Texto = $("#texto-postit").val();

    var result = MSExecuteOnServer('/Home/GuardarPostIt', obj);

    if (result) {
        $("#modalPostit").modal('hide');
    }
}

function armarBusquedaResult(value, inputId) {
    if ($("#" + inputId).val().length >= 3) {
        $(".buscar-result").empty();

        //aca tiene que ir a buscar
        var txt = $("#" + inputId).val().toUpperCase();

        var result = MSExecuteOnServer('/Home/BusquedaHome', { filtro: txt });

        var html = "";
        var url = "";
        for (var i = 0; i < result.length; i++) {
            var valor = "";
            var colorEstado = result[i].Color;
            valor = result[i].RazonSocial + ' (' + result[i].Cuit + ') ' + (result[i].Estado != null ? result[i].Estado : "");

            valor = valor.toUpperCase().split(txt).join("<strong>" + txt + "</strong>");

            var url = MSGetUrl("/Content/Images/usuario-busqueda.png");
            if (result[i].EstaAsignado) {
                url = MSGetUrl("/proveedor/Detalle?ProveedorId=" + result[i].Id)
            } else {
                url = MSGetUrl("/proveedor/ReporteProveedor?valor=" + result[i].Cuit)
            }
            html += '<a href=' + url + '>'
                + '<div class="buscar-result-linea" >'
                + '<img class="buscar-cont" src="..' + url + '" /> '
                + '<p class="buscar-nomb" style="color:' + colorEstado + '">' + valor + '</p>'
                + '</div>'
                + '</a>';
        }

        if (result.length == 1) {
            if (value && (value.keyCode || value.which) == 13) {

                var htmlurl = MSGetUrl("/proveedor/Detalle?ProveedorId=" + result[0].Id);
                var htmlurl = MSGetUrl("/proveedor/ObtenerReporteProveedor?Valor=" + result[0].Cuit);
                window.location.href = window.location.origin + htmlurl;
            }
        }

        if (!result.length) {
            html += '<div class="buscar-result-linea">'
                + '<p class="buscar-nomb">No se encontraron resultados</p>'
                + '</div>';
        }

        $(".buscar-result").append(html);
        console.log($(".buscador-nav-input").is(":focus"));
        $(".buscar-result").show();
    } else {
        $(".buscar-result").empty();
        $(".buscar-result").hide();
    }
}

$(window).click(function (e) {
    if ($(".buscar-result").is(":visible")) {
        if (!$(".buscador-nav-input").is(":focus")) {
            $(".buscar-result").empty();
            $(".buscar-result").hide();
        }
    }
})

$(window).scroll(function (event) {
    var alt = 70 + $(document).scrollTop();
    $(".notificaciones-contenedor").css({
        right: $("#notificaciones-a").offset().right,
        top: alt
    });
    $(".reporte-detalle").css({
        left: $("#reportes-a").offset().left,
        top: alt
    });

    if ($("#tablas-a").length) {
        $(".tablas-detalle").css({
            left: $("#tablas-a").offset().left,
            top: alt
        });
    }
});

$(window).resize(mobile);

$(document).ready(function () {
    try {
        // Initialize Firebase
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
        messaging.onMessage(function (payload) {
            var dataFromServer = JSON.parse(payload.data.notification);
            //var myMessageBar = new MessageBar();
            //myMessageBar.setMessage(dataFromServer.title + " : " + dataFromServer.body);
            notifyMe(dataFromServer);
        });
    }
    catch (error) {
        console.error(error);
    }
});

function notifyMe(dataFromServer) {
    if (!("Notification" in window)) {
        alert("Este navegador no soporta notificaciones");
    }
    else if (Notification.permission === "granted") {
        notify();
    }
    else if (Notification.permission !== 'denied') {
        Notification.requestPermission(function (permission) {
            if (permission === "granted") {
                notify();
            }
        });
    }

    function notify() {
        var notification = new Notification(dataFromServer.title, {
            icon: dataFromServer.icon,
            body: dataFromServer.body,
        });

        notification.onclick = function () {
            window.open(dataFromServer.url);
        };
        setTimeout(notification.close.bind(notification), 100);
    }
}

function OcultarActividadesMobile() {
    var ww = document.body.clientWidth;

    if (ww < 1200) {
        $(".contenedor-principal-widget").hide();
    } else {
        $(".contenedor-principal-widget").show();
    }
}

function toggleChatbot() {
    var modal = document.getElementById('chatbot-modal');
    if (modal.style.display === 'flex') {
        modal.style.display = 'none';
    } else {
        modal.style.display = 'flex';
    }
}
