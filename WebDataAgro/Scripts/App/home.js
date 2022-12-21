var conts = [];
var viewModel;
var htmlaux = "";

var filtro = {};
var pagina = 1;
var visualiza;

var datosCompra;

var checkear = function (el, nam) {
    var str = "." + $(el).attr('class');
    var elem = $(str + " input[name='" + nam + "']");
    elem.prop("checked", !elem.is(":checked"));
};

$(document).ready(function () {

    InicializarDatos();
    CrearObjetivo();
    armarFunciones();
    setChangeChecks();
    armarFuncionalidadesHome();
    $(".atras-nav").hide();
    $(".navbarsegundo-bread").html("Inicio");
    $("#GuardarCambios").hide();
    armarCarouselHome();
    $('[data-toggle="tooltip"]').tooltip();
    $("#filtro-comercialselect").css("width", "200px").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        filter: "contains"
    });

});
function MostrarTooltip(e) {
    $(e + '[data-toggle="tooltip"]').click(function () {
        $(e + "[data-toggle='tooltip']").on('shown.bs.tooltip', function () {
            $(e + '[data-toggle="tooltip"]').tooltip("hide");
        });
        $(e + "[data-toggle='tooltip']").on('hidden.bs.tooltip', function () {
            $(e + '[data-toggle="tooltip"]').tooltip("show");
        });
    });
}
function InicializarDatos() {
    var result = MSExecuteOnServer('/Home/Inicializar');
    kendo.culture("es-AR");
    datosCompra = result.Detalle;
    var param = {
        "materialId": null,
        "campaniaId": null,
        "toneladas": null
    };

    viewModel = kendo.observable({
        Parametros: param,
        campaniaCombo: [],
        materialCombo: [],

        Soja: [],
        Trigo: [],
        Maiz: [],
        Girasol: [],
    });
    for (var i = 0; i < datosCompra.length; i++) {

        datosCompra[i].ConCorredor.ComprasConPrecio = kendo.toString(datosCompra[i].ConCorredor.ComprasConPrecio, "n2")
        datosCompra[i].DirectoAcopiador.ComprasConPrecio = kendo.toString(datosCompra[i].DirectoAcopiador.ComprasConPrecio, "n2")
        datosCompra[i].DirectoProductor.ComprasConPrecio = kendo.toString(datosCompra[i].DirectoProductor.ComprasConPrecio, "n2")
        datosCompra[i].ConCorredor.RecibidoSinPrecio = kendo.toString(datosCompra[i].ConCorredor.RecibidoSinPrecio, "n2")
        datosCompra[i].DirectoAcopiador.RecibidoSinPrecio = kendo.toString(datosCompra[i].DirectoAcopiador.RecibidoSinPrecio, "n2")
        datosCompra[i].DirectoProductor.RecibidoSinPrecio = kendo.toString(datosCompra[i].DirectoProductor.RecibidoSinPrecio, "n2")
        datosCompra[i].ConCorredor.ARecibirAFijar = kendo.toString(datosCompra[i].ConCorredor.ARecibirAFijar, "n2")
        datosCompra[i].DirectoAcopiador.ARecibirAFijar = kendo.toString(datosCompra[i].DirectoAcopiador.ARecibirAFijar, "n2")
        datosCompra[i].DirectoProductor.ARecibirAFijar = kendo.toString(datosCompra[i].DirectoProductor.ARecibirAFijar, "n2")
        datosCompra[i].ConCorredor.FasonFas = kendo.toString(datosCompra[i].ConCorredor.FasonFas, "n2")
        datosCompra[i].DirectoAcopiador.FasonFas = kendo.toString(datosCompra[i].DirectoAcopiador.FasonFas, "n2")
        datosCompra[i].DirectoProductor.FasonFas = kendo.toString(datosCompra[i].DirectoProductor.FasonFas, "n2")
    }

    kendo.bind($("#tabla-soja"), viewModel);
    kendo.bind($("#tabla-tri"), viewModel);
    kendo.bind($("#tabla-maiz"), viewModel);
    kendo.bind($("#tabla-gi"), viewModel);
    var soja = datosCompra.filter(function (x) { return (x.Material == "Soja") });
    var maiz = datosCompra.filter(function (x) { return (x.Material == "Maiz") })
    var trigo = datosCompra.filter(function (x) { return (x.Material == "Trigo") })
    var girasol = datosCompra.filter(function (x) { return (x.Material == "Girasol") });

    if (soja.length > 0) {
        $("#mostrarSoja").show();
        $("#sojaCampania").text(soja[0].Campana);
    }
    if (maiz.length > 0) {
        $("#mostrarMaiz").show();
        $("#maizCampania").text(maiz[0].Campana);
    }
    if (trigo.length > 0) {
        $("#mostrarTrigo").show();
        $("#trigoCampania").text(trigo[0].Campana);
    }
    if (girasol.length > 0) {
        $("#mostrarGir").show();
        $("#girCampania").text(girasol[0].Campana);
    }
    viewModel.set("Soja", soja);
    viewModel.set("Trigo", trigo);
    viewModel.set("Maiz", maiz);
    viewModel.set("Girasol", girasol);
    BorrarFilasVacias();
    pagina = 1;
    $(".lista-contactos-general").empty();

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            conts = result.Contactos.Contactos;
            armarSelects(result.Datos);
            actualizarContactos(result.Contactos);
            ArmarCabeceraContactos();
            ArmarContactos(conts);
            ArmarCamapaña(result.Campaña);
            ArmarObjetivo(result.Objetivo.Objetivos);
            CargarModelObjetivosComerciales(result.Objetivo.Comerciales);
            CargarViewModel(result.Datos);
        }
    }
}

$(window).resize(OcultarActividadesMobile);

function OcultarActividadesMobile() {
    var ww = document.body.clientWidth;

    if (ww < 1200) {
        $(".contenedor-principal-widget").hide();
    } else {
        $(".contenedor-principal-widget").show();
    }
}

function armarCarouselHome() {
    $(".contenedor-principal-widget").empty();
    var htmlCarouselHome = "";
    htmlCarouselHome += '<div class="contenedor-principal-miscontactos-titulo">Próximas Actividades</div>' +
        '<div id="carouselHome" class="carousel slide" data-ride="carousel">' +
        '<div class="carousel-inner">';
    var activo = false;

    var notificacionesAux = {};

    for (var ii in notificaciones) {
        (function (i) {
            notificacionesAux[notificaciones[i].Dia] = notificacionesAux[notificaciones[i].Dia] || [];
            notificacionesAux[notificaciones[i].Dia].push(notificaciones[i]);
        })(ii);
    }

    console.log("notifaux", notificacionesAux);
    if (notificacionesAux && Object.keys(notificacionesAux).length > 0) {
        for (var ii in notificacionesAux) {
            (function (i) {
                if (!activo) {
                    htmlCarouselHome += '<div class="item active">' +
                        '<div class="carouselHome-contenedor">' +
                        '<div class="carouselHome-fecha">' +
                        '<span>' + i + '</span>' +
                        '</div>' +
                        '<div class="carouselHome-contacto-contenedor">';
                    for (var jj in notificacionesAux[i]) {
                        (function (j) {
                            if (notificacionesAux[i][j].ProveedorId != 0) {
                                htmlCarouselHome +=
                                    '<a target="_blank" href="' + MSGetUrl("/proveedor/Detalle?ProveedorId=" + notificacionesAux[i][j].ProveedorId) + '&Agenda=true">' +
                                    '<div class="linea-carouselHome">' +
                                    '<div class="carouselHome-hora">' +
                                    '<span>' + notificacionesAux[i][j].Hora + '</span>' +
                                    '</div>' +
                                    '<div class="carouselHome-contacto">' +
                                    '<span>' + notificacionesAux[i][j].Contacto + '</span>' +
                                    '</div>' +
                                    '<div class="carouselHome-descripcion">' +
                                    '<span>' + notificacionesAux[i][j].Comentarios + '</span>' +
                                    '</div>' +
                                    '</div>' +
                                    '</a>';
                            } else {
                                htmlCarouselHome +=
                                    '<a>' +
                                    '<div class="linea-carouselHome">' +
                                    '<div>' +
                                    '<span>' + notificacionesAux[i][j].Tema + '</span>' +
                                    '</div>' +
                                    '<div class="carouselHome-descripcion">' +
                                    '<span class="span-display">' + notificacionesAux[i][j].Comentarios + '</span>' +

                                    '<div class="carouselHome-hora ">' +
                                    '<span class="notificacion-hora sinMargen"> Hasta el: ' + notificacionesAux[i][j].Dia + '</span>' +
                                    '</div>' +
                                    '</div>' +
                                    '</div>' +
                                    '</a>';
                            }
                        })(jj);
                    }
                    htmlCarouselHome += '</div>' +
                        '</div>' +
                        '</div>';
                    activo = true;
                } else {
                    htmlCarouselHome += '<div class="item">' +
                        '<div class="carouselHome-contenedor">' +
                        '<div class="carouselHome-fecha">' +
                        '<span>' + i + '</span>' +
                        '</div>' +
                        '<div class="carouselHome-contacto-contenedor">';
                    for (var jj in notificacionesAux[i]) {
                        (function (j) {
                            if (notificacionesAux[i][j].ProveedorId != 0) {
                                htmlCarouselHome +=
                                    '<a target="_blank" href="' + MSGetUrl("/proveedor/Detalle?ProveedorId=" + notificacionesAux[i][j].ProveedorId) + '&Agenda=true">' +
                                    '<div class="linea-carouselHome">' +
                                    '<div class="carouselHome-hora">' +
                                    '<span>' + notificacionesAux[i][j].Hora + '</span>' +
                                    '</div>' +
                                    '<div class="carouselHome-contacto">' +
                                    '<span>' + notificacionesAux[i][j].Contacto + '</span>' +
                                    '</div>' +
                                    '<div class="carouselHome-descripcion">' +
                                    '<span>' + notificacionesAux[i][j].Comentarios + '</span>' +
                                    '</div>' +
                                    '</div>' +
                                    '</a>';
                            } else {
                                htmlCarouselHome +=
                                    '<a>' +
                                    '<div class="linea-carouselHome">' +
                                    '<div>' +
                                    '<span>' + notificacionesAux[i][j].Tema + '</span>' +
                                    '</div>' +
                                    '<div class="carouselHome-descripcion">' +
                                    '<span class="span-display">' + notificacionesAux[i][j].Comentarios + '</span>' +

                                    '<div class="carouselHome-hora ">' +
                                    '<span class="notificacion-hora sinMargen"> Hasta el: ' + notificacionesAux[i][j].Dia + '</span>' +
                                    '</div>' +
                                    '</div>' +
                                    '</div>' +
                                    '</a>';
                            }
                        })(jj);
                    }
                    htmlCarouselHome += '</div>' +
                        '</div>' +
                        '</div>';
                }
            })(ii);
        }
    } else {
        htmlCarouselHome += '<div class="item active">' +
            '<div class="carouselHome-contenedor">' +
            '<div class="carouselHome-fecha">' +
            '<span>' + kendo.toString(new Date(), "dd/MM/yyyy hh:mm") + '</span>' +
            '</div>' +
            '<div class="carouselHome-contacto-contenedor">' +
            '<div class="carouselHome-sin-act">' +
            'No hay próximas actividades' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';
    }

    htmlCarouselHome += '<a class="left carousel-control" href="#carouselHome" data-slide="prev">' +
        '<span class="glyphicon glyphicon-chevron-left"></span>' +
        '<span class="sr-only">Previous</span>' +
        '</a>' +
        '<a class="right carousel-control" href="#carouselHome" data-slide="next">' +
        '<span class="glyphicon glyphicon-chevron-right"></span>' +
        '<span class="sr-only">Next</span>' +
        '</a>' +
        '</div>';

    $(".contenedor-principal-widget").append(htmlCarouselHome);
    $("#carouselHome").carousel({ interval: false });
}

function generarFiltro(estado) {
    filtro = {};

    if ($("#filtro-zonaselect").val() && $("#filtro-zonaselect").val() != "null")
        filtro.Zona = $("#filtro-zonaselect").val();

    if ($("#filtro-comercialselect").val() && $("#filtro-comercialselect").val() != "null")
        filtro.Comercial = $("#filtro-comercialselect").val();

    if ($("#per-tie-sel").val() && $("#per-tie-sel").val() != "null")
        filtro.Campaña = $("#per-tie-sel").val();

    if ($("#segmentacion-sel").val() && $("#segmentacion-sel").val() != "null")
        filtro.Segmentacion = $("#segmentacion-sel").val();

    if ($("#condpreferentes") && $("#condpreferentes").val() && $("#condpreferentes").val().length > 0) {
        if ($.inArray("null", $("#condpreferentes").val()) === -1) {
            filtro.Condicion = $("#condpreferentes").val().join("|");
        } else {
            filtro.Condicion = null;
        }
    }

    if ($("#tipo-actividad") && $("#tipo-actividad").val() && $("#tipo-actividad").val().length > 0) {
        if ($.inArray("null", $("#tipo-actividad").val()) === -1) {
            filtro.Actividad = $("#tipo-actividad").val().join("|");
        } else {
            filtro.Actividad = null;
        }
    }

    if ($("#material-granos").val() && $("#material-granos").val() != "null") {
        filtro.Material = $("#material-granos").val();
    }

    if ($("#has-Indistinto").is(":checked") ||
        $("#has-unoados").is(":checked") ||
        $("#has-dosacinco").is(":checked") ||
        $("#has-masdecinco").is(":checked")
    ) {
        if ($("#has-Indistinto").is(":checked")) {
            $("#has-unoados").prop("checked", false);
            $("#has-dosacinco").prop("checked", false);
            $("#has-masdecinco").prop("checked", false);
        }

        filtro.Hectareas = [];
        $("#has-Indistinto").is(":checked") && filtro.Hectareas.push($("#has-Indistinto").prop("value"));
        $("#has-unoados").is(":checked") && filtro.Hectareas.push($("#has-unoados").prop("value"));
        $("#has-dosacinco").is(":checked") && filtro.Hectareas.push($("#has-dosacinco").prop("value"));
        $("#has-masdecinco").is(":checked") && filtro.Hectareas.push($("#has-masdecinco").prop("value"));
        filtro.Hectareas = filtro.Hectareas.join("|");
        if (filtro.Hectareas === "null")
            filtro.Hectareas = null;
    }

    if ($("#tns-Indistinto").is(":checked") ||
        $("#tns-unoados").is(":checked") ||
        $("#tns-dosacinco").is(":checked") ||
        $("#tns-masdecinco").is(":checked")
    ) {
        if ($("#tns-Indistinto").is(":checked")) {
            $("#tns-unoados").prop("checked", false);
            $("#tns-dosacinco").prop("checked", false);
            $("#tns-masdecinco").prop("checked", false);
        }

        filtro.Toneladas = [];
        $("#tns-Indistinto").is(":checked") && filtro.Toneladas.push($("#tns-Indistinto").prop("value"));
        $("#tns-unoados").is(":checked") && filtro.Toneladas.push($("#tns-unoados").prop("value"));
        $("#tns-dosacinco").is(":checked") && filtro.Toneladas.push($("#tns-dosacinco").prop("value"));
        $("#tns-masdecinco").is(":checked") && filtro.Toneladas.push($("#tns-masdecinco").prop("value"));
        filtro.Toneladas = filtro.Toneladas.join("|");
        if (filtro.Toneladas === "null")
            filtro.Toneladas = null;
    }

    if ($("#cal-Indistinto").is(":checked") ||
        $("#cal-cinco").is(":checked") ||
        $("#cal-cuatro").is(":checked") ||
        $("#cal-tres").is(":checked") ||
        $("#cal-dos").is(":checked") ||
        $("#cal-uno").is(":checked")
    ) {
        if ($("#cal-Indistinto").is(":checked")) {
            $("#cal-cinco").prop("checked", false);
            $("#cal-cuatro").prop("checked", false);
            $("#cal-tres").prop("checked", false);
            $("#cal-dos").prop("checked", false);
            $("#cal-uno").prop("checked", false);
        }

        filtro.Calificacion = [];
        $("#cal-Indistinto").is(":checked") && filtro.Calificacion.push($("#cal-Indistinto").prop("value"));
        $("#cal-cinco").is(":checked") && filtro.Calificacion.push($("#cal-cinco").prop("value"));
        $("#cal-cuatro").is(":checked") && filtro.Calificacion.push($("#cal-cuatro").prop("value"));
        $("#cal-tres").is(":checked") && filtro.Calificacion.push($("#cal-tres").prop("value"));
        $("#cal-dos").is(":checked") && filtro.Calificacion.push($("#cal-dos").prop("value"));
        $("#cal-uno").is(":checked") && filtro.Calificacion.push($("#cal-uno").prop("value"));
        filtro.Calificacion = filtro.Calificacion.join("|");
        if (filtro.Calificacion === "null")
            filtro.Calificacion = null;
    }
    filtro.pagina = 1;
    filtro.Estado = estado;
    return filtro;
}

function updateFiltro(estado) {

    $(".lista-contactos-general").empty();
    var filtro = generarFiltro(estado);
    pagina = 1;
    var result = MSExecuteOnServer('/Home/TraerBusquedaContacto', filtro);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            conts = result.Contactos.Contactos;
            actualizarContactos(result.Contactos);
            ArmarContactos(conts);
        }
    }
}

function actualizarContactos(contactos) {
    $(".cont-agend").html(contactos.TotalContactos);
    $(".cont-alta").html(contactos.TotalPotencialContactos);
    $(".cont-no-clie").html(contactos.TotalSinInteresContactos);
    $(".cont-op").html(contactos.TotalOperandoContactos);
    $(".cont-no-op").html(contactos.TotalNoOperandoContactos);
    $(".cont-baj").html(contactos.TotalBajaContactos);

    $(".cont-habilitado").html(contactos.TotalHabilitadoContactos);
    $(".cont-legajo-irregular").html(contactos.TotalLegajoIrregularContactos);
    $(".cont-no-habilitado").html(contactos.TotalNoHabilitadoContactos);
}

//function ObtenerEstadoActual() {
//    return $(".cont-agend-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? null :
//        $(".cont-alta-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 1 :
//            $(".cont-op-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 2 :
//                $(".cont-no-op-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 3 :
//                    $(".cont-baj-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 4 :
//                        $(".cont-alta-no-clie .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 5 : null
//}

function ObtenerEstadoActual() {
    return $(".cont-agend-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? null :
        $(".cont-habilitado-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 1 :
            $(".cont-legajo-irregular-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 2 :
                $(".cont-no-habilitado-det .contenedor-principal-miscontactos-detalle").hasClass("miscontactos-selected") ? 3 : null
}

function ArmarCabeceraContactos() {
    $(".cont-agend-det").click(function () {
        LimpiarClase();
        $(".cont-agend-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(null);
    });

    $(".cont-alta-det").click(function () {
        LimpiarClase();
        $(".cont-alta-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(1);
    });

    $(".cont-op-det").click(function (e) {
        LimpiarClase();
        $(".cont-op-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(2);
    });

    $(".cont-no-op-det").click(function () {
        LimpiarClase();
        $(".cont-no-op-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(3);
    });

    $(".cont-baj-det").click(function () {
        LimpiarClase();
        $(".cont-baj-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(4);
    });

    $(".cont-alta-no-clie").click(function () {
        LimpiarClase();
        $(".cont-alta-no-clie .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(5);
    });

    $(".cont-habilitado-det").click(function () {
        LimpiarClase();
        $(".cont-habilitado-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(1);
    });

    $(".cont-legajo-irregular-det").click(function (e) {
        LimpiarClase();
        $(".cont-legajo-irregular-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(2);
    });

    $(".cont-no-habilitado-det").click(function () {
        LimpiarClase();
        $(".cont-no-habilitado-det .contenedor-principal-miscontactos-detalle").addClass("miscontactos-selected");
        updateFiltro(3);
    });

    function LimpiarClase() {
        $(".cont-agend-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-alta-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-op-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-no-op-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-baj-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-alta-no-clie .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-agend-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");

        $(".cont-habilitado-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-legajo-irregular-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
        $(".cont-no-habilitado-det .contenedor-principal-miscontactos-detalle").removeClass("miscontactos-selected");
    }
}

function ArmarContactos(contactos) {
    htmlaux = "";
    for (var ii in contactos) {
        $("#verMasContactos").show();
        (function (i) {
            var colorHome = contactos[i].EstadoHomeId == 1 ? 'green' : contactos[i].EstadoHomeId == 2 ? 'yellow' : 'red';
            var colorTextoEstadoHome = contactos[i].EstadoHomeId == 2 ? 'grey' : 'white';
            var estadoHomeMensaje = contactos[i].EstadoHomeMensaje == null ? "" : contactos[i].EstadoHomeMensaje;
            var clase = '';
            var onClick = '';
            var htmlurl = MSGetUrl('/Proveedor/Detalle?ProveedorId=' + contactos[i].ProveedorId);
            if (visualiza === "False") {
                htmlurl = '#';
                onClick = ' onClick= "return false"';
                clase = ' deshabilitado';
            }
            htmlaux += '<a href=' + htmlurl + onClick + ' class="' + clase + '"><div class="col-lg-12 lista-contactos-contenedor' + clase;
            contactos[i].Corredor ? htmlaux += ' detalleCorredor' : '';
            htmlaux += '">'
                + '<div class="lista-contactos-estado">'
                + '<br>'
                //+ '<span class="lista-contactos-estadoHome-titulo" style="background:' + colorHome + '; color:' + colorTextoEstadoHome + ';">Estado Home: <b>' + contactos[i].EstadoHomeMensaje + '</b></span>'
                + '<span class="lista-contactos-estadoHome-titulo" style="background:' + colorHome + '; color:' + colorTextoEstadoHome + ';"> ' + estadoHomeMensaje + '  </span>'
                + '<br>'
                + '<span class="lista-contactos-estado-titulo">Estado:</span>'
                + '<span class="lista-contactos-estado-ab"> ' + contactos[i].Estado + '</span>'
                + '<span class="lista-contactos-estado-estrellas">';
            for (var j = 0; j < contactos[i].Calificacion; j++) {
                var url = MSGetUrl("/Content/Images/estrellacalificacion.png");
                htmlaux += '<img src="..' + url + '" />';
            }
            var url2 = MSGetUrl("/Content/Images/listcont.png");

            var telefonos = (contactos[i].Telefono ? contactos[i].Telefono : "Ninguno");
            var telaux = [];
            if (telefonos !== "Ninguno") {
                telefonos = telefonos.split(";");
                for (var x in telefonos) {
                    if (telefonos[x] != "")
                        telaux.push('<a href="tel: ' + telefonos[x] + '" target: "_blank"><i class="fa fa-skype" aria-hidden="true"> ' + telefonos[x] + '</i></a>');
                }
                telaux = telaux.join(" ");
            } else {
                telaux = "Ninguno";
            }

            var Mail = (contactos[i].Mail ? contactos[i].Mail : "Ninguno");
            var mailaux = [];
            if (Mail !== "Ninguno") {
                Mail = Mail.split(";");
                for (var x in Mail) {
                    if (Mail[x] != "")
                        mailaux.push('<a href="mailto: ' + Mail[x] + '" target: "_blank"><i class="fa fa-envelope-o" aria-hidden="true"> ' + Mail[x] + '</i></a>');
                }
                mailaux = mailaux.join(" ");
            } else {
                mailaux = "Ninguno";
            }

            htmlaux += "</span>";
            if (contactos[i].NoOperable) {
                var url3 = MSGetUrl("/Content/Images/no-operable.png");
                htmlaux += '<div class="lista-contacto-no-operable"' +
                    'data-toggle="tooltip" title="' + contactos[i].TooltipNoOperable + '" click="MostrarTooltip(this)">'
                    + '<img class="img-contacto-no-operable" src="..' + url3 + '" />'
                    + '<span class="span-contacto-no-operable">No operable</span>'
                    + '</div>';
            }
            htmlaux += '</div>'
                + '<div class="lista-contactos-datos">'
                + '<div>'
                + '<img class="img-contactos-datos" src="..' + url2 + '" />'
                + '<span class="lista-contactos-datos-razonsocial"><b>' + contactos[i].RazonSocial + '</b> (CUIT ' + contactos[i].Cuit + ')</span>'
                + '</div>'
                + '<div class="lista-contactos-datos-emailtelefono">'
                + mailaux
                + '</div>'
                + '<div class="lista-contactos-datos-emailtelefono">'
                + telaux
                + '</div>'
                + '</div>'
                + '<div class="lista-contacto-footer">'
                + '<div class="lista-contacto-footer-comacargo">'
                + "Comercial a cargo: " + contactos[i].ComercialCargo
                + '</div>'
                + '<div class="lista-contacto-footer-ulcontacto">'
                + '<span><em><b>Último Contacto</b> ' + contactos[i].UltimoContacto + '</em></span>'
                + '</div>'
                + '</div>';

            htmlaux += '</div></a>';
        })(ii);
    }
    $(".lista-contactos-general").append(htmlaux);
}

function setChangeChecks() {
    $('#form-filtro > div :input').change(function () {
        updateFiltro(ObtenerEstadoActual());
    });
}

function TraerSiguiente() {
    pagina += 1;
    filtro.pagina = pagina
    $("#verMasContactos").hide();
    $("#cargandoContactos").show();
    function callback(result) {
        if (result != null) {
            if (ExistsErrorMessages(result.Errores)) {
                ShowTooltipMessages("err", result.Errores);
            }
            else {
                conts = result.Contactos.Contactos;
                ArmarContactos(conts);
            }
        }
        $("#cargandoContactos").hide();
        $("#verMasContactos").show();
    }
    var result = MSExecuteOnServerAsync('/Home/TraerBusquedaContacto', filtro, callback);


}

function ArmarCamapaña(campañas) {
    //$(".contenedor-principal-campanas-titulo").html(campañas.Nombre);
    var html = "";
    for (var ii in campañas.Materiales) {
        (function (i) {
            var ToneladasAux = campañas.Materiales[i].Toneladas.toString().split(".");
            if (ToneladasAux.length > 1) {
                ToneladasAux[0] = ToneladasAux[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                ToneladasAux[1] = ToneladasAux[1].length > 0 ? ToneladasAux[1].substr(0, 2) : "";
                ToneladasAux = ToneladasAux.join(",");
            }
            else {
                ToneladasAux[0] = ToneladasAux[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                ToneladasAux = ToneladasAux.join("");
            }
            //ToneladasAux.join(",");

            html += '<div class="contenedor-principal-campanas-detalle">'
                + '<div class="contenedor-principal-campanas-grano">'
                + campañas.Materiales[i].Nombre
                + '</div>'
                + '<div class="contenedor-principal-campanas-cantidad">'
                + ToneladasAux
                + '</div>'
                + '<div class="contenedor-principal-campanas-grano">'
                + campañas.Materiales[i].Campaña
                + '</div>'
                + '</div>';
        })(ii);
    }
    $(".contenedor-principal-campanas-detalle").append(html);
}
function ArmarObjetivo(objetivos) {
    var html = "";
    for (var ii in objetivos) {
        (function (i) {
            var ToneladasAux = FormatearNumeros(objetivos[i].Toneladas);
            //ToneladasAux.join(",");

            html += '<div class="contenedor-principal-objetivo-detalle">'
                + '<div class="contenedor-principal-objetivo-grano">'
                + objetivos[i].Material
                + '</div>'
                + '<div class="contenedor-principal-objetivo-cantidad">'
                + ToneladasAux
                + '</div>'
                + '<div class="contenedor-principal-objetivo-grano">'
                + objetivos[i].Campana
                + '</div>'
                + '</div>';
        })(ii);
    }
    $(".contenedor-principal-objetivo-detalle").append(html);
}

function CargarModelObjetivosComerciales(comercial) {
    var html = "";
    for (var i in comercial) {
        html += '<div class="row listado"><div class="comercial-listado" data-toggle="collapse"  href="#comercial' + comercial[i].ComercialId + '"> <span class="comercial-objetivo">' + comercial[i].Comercial + '</span></div>' +
            '<div id="comercial' + comercial[i].ComercialId + '" class="panel-collapse collapse in">' +
            '<div class="col-xs-12"><table class="tabla-home">';
        for (var j in comercial[i].Objetivos) {
            var ToneladasAux = FormatearNumeros(comercial[i].Objetivos[j].Toneladas);
            html += '<tr><th class="col-xs-4">' + comercial[i].Objetivos[j].Material + '</th>' +
                '<td class="col-xs-5">' + ToneladasAux + '</td>' +
                '<td class="col-xs-2">' + comercial[i].Objetivos[j].Campana + '</td>' +
                '<td class="col-xs-1"><a class="fa fa-minus-circle danger" onclick="AlertaObjetivoBorrar(' + comercial[i].Objetivos[j].Id + ')"></td>' + '</tr>';
        }
        html += '</table></div></div></div>';
    }
    $(".contenedor-principal-detalle").append(html);
}
function FormatearNumeros(ton) {
    var ToneladasAux = ton.toString().split(".");
    if (ToneladasAux.length > 1) {
        ToneladasAux[0] = ToneladasAux[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        ToneladasAux[1] = ToneladasAux[1].lenght > 0 ? ToneladasAux[1].substr(0, 2) : "";
        ToneladasAux = ToneladasAux.join(",");
    }
    else {
        ToneladasAux[0] = ToneladasAux[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        ToneladasAux = ToneladasAux.join("");
    }
    return ToneladasAux;
}
function armarSelectCampañas() {
    $("#per-tie-sel").multiselect({
        header: false,
        multiple: false,
        selectedList: 1,
        noneSelectedText: "Elegir"
    });//.multiselectfilter();

    //para que funcione el filtro, hay que sacar el header: false
}

function armarSelectTipoActividad() {
    $("#tipo-actividad").multiselect({
        selectedList: 1,
        noneSelectedText: "Elegir",
        header: false
    });
}

function armarSelectEstadoContacto() {
    $("#estado-contacto").multiselect({
        selectedList: 1,
        noneSelectedText: "Elegir",
        header: false
    });
}

function armarSelectCondicionPreferente() {
    $("#condpreferentes").multiselect({
        selectedList: 1,
        noneSelectedText: "Elegir",
        header: false
    });
}

function armarSelectSegmentacion() {
    $("#segmentacion-sel").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Elegir",
    });
}

function armarSelectGranos() {
    $("#material-granos").multiselect({
        header: false,
        multiple: false,
        selectedList: 1,
        noneSelectedText: "Elegir",
    });
}

function armarSelects(result) {
    var htmlCampaña = "";
    htmlCampaña += '<select id="per-tie-sel">';
    htmlCampaña += '<option value="null">Todas las campañas</option>';
    for (var ii in result.camp) {
        (function (i) {
            htmlCampaña += '<option value="' + result.camp[i].CampañaId + '">' + result.camp[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCampaña += '</select>';
    $(".periodo-tiempo").append(htmlCampaña);

    if (result.come.length > 1) {
        var htmlComercial = "";
        htmlComercial += '<select id="filtro-comercialselect">';
        htmlComercial += '<option value="null">Todos los comerciales</option>';
        for (var ii in result.come) {
            (function (i) {
                htmlComercial += '<option value="' + result.come[i].ComercialId + '">' + result.come[i].Comercial + '</option>';
            })(ii);
        }
        htmlComercial += '</select>';
        $(".filtro-comercial").append(htmlComercial);

    } else {
        $(".filtro-comercial").parent().hide();
    }

    if (result.zona.length > 1) {
        var htmlZona = "";
        htmlZona += '<select id="filtro-zonaselect">';
        htmlZona += '<option value="null">Todas las zonas</option>';
        for (var ii in result.zona) {
            (function (i) {
                htmlZona += '<option value="' + result.zona[i].ZonaId + '">' + result.zona[i].Descripcion + '</option>';
            })(ii);
        }
        htmlZona += '</select>';
        $(".filtro-zona").append(htmlZona);
    } else {
        $(".filtro-zona").parent().hide();
    }

    var htmlGranos = "";
    htmlGranos += '<select id="material-granos">';
    htmlGranos += '<option value="null">Todos</option>';
    for (var ii in result.mat) {
        (function (i) {
            htmlGranos += '<option value="' + result.mat[i].MaterialId + '">' + result.mat[i].Descripcion + '</option>';
        })(ii);
    }
    htmlGranos += '</select>';
    $(".material-granos").append(htmlGranos);

    var grupos = {};
    for (var jj in result.segm) {
        (function (j) {
            grupos[result.segm[j].Grupo] = grupos[result.segm[j].Grupo] || [];
            grupos[result.segm[j].Grupo].push({
                SegmentacionId: result.segm[j].SegmentacionId,
                Descripcion: result.segm[j].Descripcion
            });
        })(jj);
    }
    console.log("asdas", grupos)

    var htmlSegmentacion = "";
    htmlSegmentacion += '<select id="segmentacion-sel">';
    htmlSegmentacion += '<option value="null">Todos</option>';
    if (grupos[null]) {
        var nulo = grupos[null];
        for (var ii in nulo) {
            (function (i) {
                htmlSegmentacion += '<option value="' + nulo[i].SegmentacionId + '">' + nulo[i].Descripcion + '</option>';
            })(ii);
        }
    }
    for (var ii in grupos) {
        (function (i) {
            if (i != "null") {
                htmlSegmentacion += '<optgroup label="' + i + '">';
                for (var jj in grupos[i]) {
                    (function (j) {
                        htmlSegmentacion += '<option value="' + grupos[i][j].SegmentacionId + '">' + grupos[i][j].Descripcion + '</option>';
                    })(jj);
                }
                htmlSegmentacion += '</optgroup>';
            }
        })(ii);
    }
    htmlSegmentacion += '</select>';

    $(".campo-segmentacion").append(htmlSegmentacion);

    /*var htmlTipoActividad = "";
    htmlTipoActividad += '<select multiple class"campo-tipoactividad" id="tipo-actividad">';
    htmlTipoActividad += '<option value="null">Todos</option>';
    for (var ii in result.tipoact) {
        (function (i) {
            htmlTipoActividad += '<option value="' + result.tipoact[i].TipoActividadId + '">' + result.tipoact[i].Descripcion + '</option>';
        })(ii);
    }
    htmlTipoActividad += '</select>';
    $(".campo-tipoactividad").append(htmlTipoActividad);*/

    //var htmlEstadoContacto = "";
    //htmlEstadoContacto += '<select multiple class"campo-estado" id="estado-contacto">';
    //htmlEstadoContacto += '<option value="null">Todos</option>';
    //for (var ii in result.est) {
    //    (function (i) {
    //        htmlEstadoContacto += '<option value="' + result.est[i].EstadoId + '">' + result.est[i].Descripcion + '</option>';
    //    })(ii);
    //}
    //htmlEstadoContacto += '</select>';
    //$(".campo-estado").append(htmlEstadoContacto);

    var htmlCondicion = "";
    htmlCondicion += '<select multiple class"campo-condicion" id="condpreferentes">';
    htmlCondicion += '<option value="null">Todos</option>';
    for (var ii in result.cond) {
        (function (i) {
            htmlCondicion += '<option value="' + result.cond[i].CondicionId + '">' + result.cond[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCondicion += '</select>';
    $(".campo-condicion").append(htmlCondicion);

    armarSelectCampañas();
    armarSelectSegmentacion();
    armarSelectGranos();
    armarSelectTipoActividad();
    armarSelectEstadoContacto();
    armarSelectCondicionPreferente();
}

function armarFunciones() {
    $("#exportarPDF").click(function () {
        exportar(0);
    });
    $("#exportarExcel").click(function () {
        exportar(1);
    });
    $("#exportarAll").click(function () {
        exportar(2);
    });

    $("body").click(function () {
        if ($(".lista-contacto-no-operable-tooltip").is(":visible")) {
            $(".lista-contacto-no-operable-tooltip").hide();
            $(".lista-contacto-no-operable-tooltip-arrow").hide();
        }
    });
}

function exportar(value) {

    var filtro = generarFiltro(ObtenerEstadoActual());

    if (value == 0) {
        DescargarPDF(filtro);
    }
    else if (value == 1) {
        DescargarExcel(filtro);
    }
    else {
        DescargarExportAll(filtro);
    }
}

function DescargarPDF(param) {
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Reporte?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    }
    MSExecuteOnServerAsync('/Home/ExportarContactosPDF', param, funcReturn, true);
}

function DescargarExcel(param) {
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    }

    MSExecuteOnServerAsync('/Home/ExportarContactosExcel', param, funcReturn, true);
}

function DescargarExportAll(param) {
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    };
    MSExecuteOnServerAsync('/Home/ExportarAll', param, funcReturn, true);
}

function armarFuncionalidadesHome() {
    $(".sap").hover(function () {
        console.log("hover sap");
        $(".sap .link-externos img").css({
            opacity: 1
        });
    }, function () {
        $(".sap .link-externos img").css({
            opacity: 0.8
        });
    });

    $(".scato").hover(function () {
        console.log("hover sap");
        $(".scato .link-externos img").css({
            opacity: 1
        });
    }, function () {
        $(".scato .link-externos img").css({
            opacity: 0.8
        });
    });

    $(".field").hover(function () {
        console.log("hover sap");
        $(".field .link-externos img").css({
            opacity: 1
        });
    }, function () {
        $(".field .link-externos img").css({
            opacity: 0.8
        });
    });

    $(".lista-contacto-no-operable").click(function (e) {
        e.stopPropagation();
        e.preventDefault();
    });

    $(".mostrar-filtro span").click(function () {
        if ($("#form-filtro").is(":visible")) {
            $("#form-filtro").slideUp("slow");
            $(".mostrar-filtro span").html("Mostrar filtro +");
        } else {
            $("#form-filtro").slideDown("slow");
            $(".mostrar-filtro span").html("Ocultar filtro -");
        }
    });
}
function CrearObjetivo() {
    $("#MaterialId").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#CampaniaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });
    $("#Toneladas").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false
    });
    var param = {
        "materialId": null,
        "campaniaId": null,
        "toneladas": null
    };
    //viewModel = kendo.observable({
    //    Parametros: param,

    //    campaniaCombo: [],
    //    materialCombo: []
    //});

    kendo.bind($("#objetivo-modal"), viewModel);
    $("#cancelar-borrar").click(function () {
        $("#borrar-objetivo").hide();
    });
    $("#objetivo-modal").on("hidden.bs.modal", function () {
        viewModel.set("Parametros", param);
    });
}
function CargarViewModel(datos) {
    var campania = datos.camp.filter(function (camp) { return camp.CampañaId > 6; });
    viewModel.set("materialCombo", datos.mat);
    viewModel.set("campaniaCombo", campania);
}
function AgregarObjetivo() {
    var objetivo = {};
    objetivo.MaterialId = $("#MaterialId").val();
    objetivo.CampanaId = $("#CampaniaId").val();
    objetivo.ToneladasObjetivos = $("#Toneladas").val();

    var res = MSExecuteOnServer('/Home/GuardarObjetivoComercial', objetivo);
    if (res != null) {
        if (ExistsErrorMessages(res.Errores)) {
            MensErr(res.Errores[0].Message);
        }
        else {
            $('#objetivo-modal').modal('toggle');
            MensInfo("Grabación Exitosa");
            Actualizar();
        }
    }
}
function Actualizar() {
    $(".contenedor-principal-objetivo-detalle").empty();
    $(".contenedor-principal-detalle").empty();
    var obj = MSExecuteOnServer('/Home/TraerObjetivos');

    ArmarObjetivo(obj.Objetivo.Objetivos);
    CargarModelObjetivosComerciales(obj.Objetivo.Comerciales);
}
function AlertaObjetivoBorrar(e) {
    $("#borrar-objetivo").show();
    $("#aceptar-borrar").click(function () {
        EliminarObjetivo(e);
        $("#borrar-objetivo").hide();
        $("#aceptar-borrar").unbind('click');
    });
}

function EliminarObjetivo(id) {
    var res = MSExecuteOnServer('/Home/EliminarObjetivo', { id: id });

    if (res != null) {
        if (ExistsErrorMessages(res.Errores)) {
            MensErr(res.Errores[0].Message);
        }
        else {
            Actualizar();
        }
    }
}


function BorrarFilasVacias() {
    var $filasEncabezado = $("#tablaTrigo tr:not('.encabezado')");
    Remover($filasEncabezado);
    $filasEncabezado = $("#tablaSoja tr:not('.encabezado')");
    Remover($filasEncabezado);
    $filasEncabezado = $("#tablaGi tr:not('.encabezado')");
    Remover($filasEncabezado);
    $filasEncabezado = $("#tablaMaiz tr:not('.encabezado')");
    Remover($filasEncabezado);
}

function Remover($filasEncabezado) {

    $filasEncabezado.each(function () {
        var valorFila = 0
        $(this).find('td').each(function (i) {
            if (i != 0) {
                valorFila += parseInt($(this).context.innerText);
            }
        });
        if (valorFila == 0) {
            $(this).remove();
        }
    });
}




