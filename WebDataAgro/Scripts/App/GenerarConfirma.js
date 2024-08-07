//Inicializar
$(document).ready(function () {
    inicializarPopUpContratoSap();
});

//Funcion Generar Confirma
function Generar() {
    var confirma = {
        ContratoSAP: $("#Negocio").val(),
        ClaseNegocioId: $("#ClaseNegocioId").val()
    };
    var url = '/Confirma/GenerarConfirma';
    var data = confirma;
    var result = MSExecuteOnServer(url, data);
    if (result.confirmasGenerados == undefined || result.confirmasGenerados.length == 0) {
        if (result.HayError) {
            result.ListaErrores.forEach(err => MensErr(err.Message));
        } else {
            MensErr("No se generó ningún Confirma.");
        }
    } else {
        CargarTablaModal(result.confirmasGenerados);
        $("#ModalConfirma").modal("show");
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

$("body").on("click", "#abrirPopUpCargarConfirmas", function () {
    $("#popupcargarvalores").modal('toggle');
});

$("#Negocio").bind("paste", function (e) {//En caso de Pegar Codigos
    e.preventDefault();
    if (e.originalEvent.clipboardData !== undefined) {
        clipText = e.originalEvent.clipboardData.getData('text/plain');
    } else {
        clipText = window.clipboardData.getData('text');
    }
    $("#Negocio").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));
    ActualizarTabla();
});

function ListarNegocios() {
    //Obtenemos parametros
    var claseNegocio = $("#ClaseNegocioId").val();
    var hasta = $("#NegocioHasta").val();
    var lista = trimEnd($("#Negocio").val()).split(';');
    //Realizamos la consulta correspondiente
    BlockUi("Consultando...");
    if (isNullOrWhitespace(hasta)) {
        let data = { listaCodigosSAP: lista, claseNegocio: claseNegocio };
        rechazados = MSExecuteOnServer('/Confirma/ValidarNegocios', data);
        if (rechazados.length > 0) {
            MensAlerta(rechazados);
        } else {
            ActualizarTabla();
            $("#popUpCargarValores").modal('toggle');
            $('#popUpCargarValores').modal('show');
        }
        
    } else {
        let data = { desdeSAP: $("#Negocio").val(), hastaSAP: $("#NegocioHasta").val(), claseNegocio: claseNegocio };
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
            $("#popUpCargarValores").modal('toggle');
            $('#popUpCargarValores').modal('show');
        }
    }
    $.unblockUI();
}

function inicializarPopUpContratoSap() {
    crearPopUp("Confirmas");
    $("#popupcargarvalores").modal('toggle'); //Mostrar/Ocultar Modal/PopUp Cargar Valores

    $(document).on("click", ".agregarContrato", function () {
        let num = $("#nuevoNumContrato").val(); //recupera el numero ingresado
        if (num.trim() != "" && num.trim() != null) { //validamos que no este vacio
            let mensaje = ValidarNegocio(num); //validamos el codigo
            if (mensaje == "") { //Válido 
                let lista = trimEnd($("#Negocio").val()).split(';');
                lista.push(num);
                $("#Negocio").val(lista.join(';'));
                $("#contratos-table").append('<tr><td>' + num + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" data-id="' + num + '" style="height: 34px;float: right" type="button"></button></td></tr></td></tr>');
                $(".nuevoNumContrato").val('');
                $(".nuevoNumContrato").focus();
            } else { //Inválido
                MensErr(mensaje);
            }
            
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

function CargarTablaModal(contratos) {
    $("#tabla-cap-pendientes").empty();
    var tabla = '';
    for (var i = 0; i < contratos.length; i++) {
        tabla += '<tr><td>'
            + contratos[i].NegocioSAP
            + '</td><td>'
            + contratos[i].FechaGeneracionFormateada
            + '</td><td>'
            + (contratos[i].Generado ? '<i class="fa fa-check generado" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
            + '</td><td>'
            + (contratos[i].IsWebService ? '<i class="fa fa-check web" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
            + (contratos[i].Generado ? ('<a href="/Confirma/DescargarArchivoConfirma?codigoSAP=' + contratos[i].NegocioSAP + '" class="k-button k-button-icontext" style="height: 34px;text-align: center;margin-left: 1rem;"><i class="fa fa-download generado" style="text-align: center"></i></a>') : "")
            + '</td><td>'
            + contratos[i].Mensaje
            + '</td></tr>';
    }
    $("#tabla-cap-pendientes").append(tabla);
}

function FiltrarNegocios() {
    let fechaDesde = $("#desde").val();
    let fechaHasta = $("#hasta").val();
    let claseNegocio = $("#ClaseNegocioId").val();
    BlockUi("Consultando...");
    var data = { desde: fechaDesde, hasta: fechaHasta, claseNegocio: claseNegocio };
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

function ValidarNegocio(codigoSAP) {
    let claseNegocio = $("#ClaseNegocioId").val();
    BlockUi("Consultando...");
    var data = { codigoSAP: codigoSAP, claseNegocio: claseNegocio };
    var result = MSExecuteOnServer('/Confirma/ValidarNegocio', data);
    $.unblockUI();
    return result;
}

//Funciones Utiles
function isNullOrWhitespace(input) {
    return !input || !input.trim();
}

function trimEnd(cadena) {
    return cadena.at(cadena.length - 1) == ';' ? cadena.slice(0, cadena.length - 1) : cadena;
}