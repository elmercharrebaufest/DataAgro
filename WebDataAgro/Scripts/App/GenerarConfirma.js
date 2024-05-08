//Variables Globales
var viewModel;

$(document).ready(function () {
    inicializarPopUpContratoSap();
});

function Generar() {
    var confirma = {
        ContratoSAP: $("#Negocio").val(),
        TipoNegocioId: $("#TipoNegocioId").val()
    };
    var url = '/Confirma/GenerarConfirma';
    var data = confirma;
    var result = MSExecuteOnServer(url, data);
    if (result.length == 0) {
        MensErr("No se generó ningún boleto.");
    } else {
        $("#ModalBoleto").modal("show");
    }
}

//Eventos
$("body").on("click", "#abrirPopUpCargarConfirmas", function () {
    ListarNegocios();
});

$("body").on("change", "#Negocio", function () {
    let lista = trimEnd($("#Negocio").val()).split(';');
    if (lista.length > 1) {
        $("#NegocioHasta").attr('disabled', 'disabled');
        $("#NegocioHasta").val("");
    } else {
        $("#NegocioHasta").removeAttr('disabled');
    }
});

$("body").on("click", "#filtrarConfirmas", function () {
    FiltrarNegocios();
});

$("body").on("change", "#desde", function () {
    if ($("#desde").val() == '') {
        $("#hasta").prop("disabled", true);
        $("#filtrarConfirmas").prop("disabled", true);
    } else {
        $("#hasta").removeAttr('disabled');
        $("#filtrarConfirmas").removeAttr('disabled');
    }
});


function modalcontratoGenerarConfirma() {
    $("#abrirPopUpCargarConfirmas").click(function () {
        $("#popupcargarvalores").modal('toggle');
    });
}

function ListarNegocios() {
    debugger
    //Obtenemos parametros
    var tipoNegocio = $("#TipoNegocioId").val();
    var hasta = $("#NegocioHasta").val();
    var lista = trimEnd($("#Negocio").val()).split(';');
    //Realizamos la consulta correspondiente
    if (isNullOrWhitespace(hasta)) {
        let data = { listaCodigosSAP: lista, tipoNegocio: tipoNegocio };
        rechazados = MSExecuteOnServer('/Confirma/ValidarNegocios', data);
        if (rechazados.length > 0) {
            MensAlerta(rechazados);
        }
        ActualizarTabla();
    } else {
        let data = { desdeSAP: $("#Negocio").val(), hastaSAP: $("#NegocioHasta").val(), tipoNegocio: tipoNegocio };
        result = MSExecuteOnServer('/Confirma/ListarNegocios', data);

        if (result.length == 0) {
            MensAlerta("No se ha recuperado negocios validos para los parametros indicados.");
        } else {
            var concatenada = result.join(';');
            $("#Negocio").val(concatenada);
            if (result.length > 1) {
                $("#NegocioHasta").attr('disabled', 'disabled');
                $("#NegocioHasta").val("");
            }
            ActualizarTabla();
        }
    }
    $("#popUpCargarValores").modal('toggle');
    $('#popUpCargarValores').modal('show');
}


//Funciones Utiles
function isNullOrWhitespace(input) {
    return !input || !input.trim();
}

function trimEnd(cadena) {
    return cadena.at(cadena.length - 1) == ';' ? cadena.slice(0, cadena.length - 1) : cadena;
}

function inicializarPopUpContratoSap() {

    crearPopUp("Confirmas");

    modalcontratoGenerarConfirma();


    $("#Negocio").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#Negocio").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));

        ActualizarTabla();
    });

    $(document).on("click", ".agregarContrato", function () {
        //PENDIENTE: Se requiere agregar validacion contra el servicio del Codigo SAP aqui ingresado
        let num = $("#nuevoNumContrato").val();
        if (num.trim() != "" && num.trim() != null) {
            let lista = trimEnd($("#Negocio").val()).split(';');
            lista.push(num);
            $("#Negocio").val(lista.join(';'));
            $("#contratos-table").append('<tr><td>' + num + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" data-id="' + num + '" style="height: 34px;float: right" type="button"></button></td></tr></td></tr>');
            $(".nuevoNumContrato").val('');
            $(".nuevoNumContrato").focus();
        }
    });

    $(document).on("click", ".borrarContrato", function (e) {
        let objetivo = e.currentTarget.dataset.id;
        let lista = trimEnd($("#Negocio").val()).split(';');
        let indice = lista.indexOf(objetivo); // obtenemos el indice
        lista.splice(indice, 1); // 1 es la cantidad de elemento a eliminar
        $("#Negocio").val(lista.join(';'));
        ActualizarTabla();
    });
}

function ActualizarTabla() {
    var lista = [];
    lista = trimEnd($("#Negocio").val()).split(';');
    ArmarTabla(lista);
}


//Otras Funciones
function crearPopUp(nombrePopUp) {

    if ($("#popUpCargarValores").children().length == 0) {

        $("#popUpCargarValores").prepend('<div class="modal-dialog" id="1" role="document"></div>');
        $("#1").prepend('<div id="2" class="modal-content">');

        $("#2").prepend('<div id="4" class="modal-body"></div>');
        $("#2").prepend('<div id="3" class="modal-header"></div>');

        //tabla
        $("#4").prepend('<div id="7" class="table-responsive">');
        $("#7").prepend('<table id="contratos-table" class="table table-striped"></table>');

        //body
        $("#7").prepend('<div id="8" class="row">');
        $("#8").prepend('<div id="10" class="col-xs-2">');
        $("#8").prepend('<div id="9" class="col-xs-4"></div>');

        $("#10").prepend('<button class="k-button k-button-icontext fa fa-plus agregarContrato" style="height: 34px;" type="button"></button>');
        $("#9").prepend('<input type="text" id="nuevoNumContrato" class="nuevoNumContrato">');

        //header
        $("#5").prepend('<span aria-hidden="true">&times;</span>');
        $("#3").prepend('<div id="6" class="status confirmado-modal"></div>');
        $("#6").prepend('<div>Seleccione ' + nombrePopUp + '</div>');
        $("#3").prepend('<button id="5" type="button" class="cerrar close" data-dismiss="modal" aria-label="Close"></button>');
    }
}

function ArmarTabla(contratos) {
    $("#contratos-table").empty();
    var tabla = '<tr class="seleccionado" ><th >Seleccionados:</th></tr>';
    if (contratos) {
        tabla += "<div id='div1'>"
        for (var i = 0; i < contratos.length; i++) {
            if (contratos[i] != "") {
                tabla += '<tr><td class="hide contador" style="width: 40px; text-align: center">' + (i + 1) + '</td><td style="width: 100%">' + contratos[i] + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" data-id="' + contratos[i] + '" style="height: 34px;float: right" type="button"></button></td></tr>';
            }
        }
        tabla += "</div>";
    }
    $("#contratos-table").append(tabla);
}


function FiltrarNegocios() {
    let fechaDesde = $("#desde").val();
    let fechaHasta = $("#hasta").val();
    let tipoNegocio = $("#TipoNegocioId").val();
    BlockUi("Consultando...");
    var data = { desde: fechaDesde, hasta: fechaHasta, negocio: tipoNegocio };
    var result = MSExecuteOnServer('/Confirma/FiltrarNegociosPorFecha', data);
    $.unblockUI();
    if (result.length == 0) {
        MensAlerta("Sin Resultados");
    } else {
        var concatenada = result.join(';');
        $("#Negocio").val(concatenada);
        if (result.length > 1) {
            $("#NegocioHasta").attr('disabled', 'disabled');
            $("#NegocioHasta").val("");
        }
        ActualizarTabla();
    }
}