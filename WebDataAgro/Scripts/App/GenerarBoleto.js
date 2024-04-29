var viewModel;
var datosIniCrearContrato;
var externo;
var enviarEmail;

$(document).ready(function () {

    $('#menuproveedor').hide();
    inicializarPopUpConratoSap();
});


function Filtrar() {


    if (ValidarGenerarBoletos()) {
        BlockUi('Cargando...');
        setTimeout(function () {
            var boleto = {
                ContratoSAP: $("#GeneraBoletoContratoSAPId").val().padStart(10, '0'),
                TipoNegocioId: $("#tipoId").val(),
                Mail: $("#mailId").is(":checked")
            };
            //$("#fleteProcedenciaModal").modal("hide");
            var url = '/Boleto/GenerarBoletos';
            var data = boleto;
            var result = MSExecuteOnServer(url, data);
            $.unblockUI();
            var viewmodel = {
                ObtenerBoletos: result
            }
            kendo.bind($("#ModalBoleto"), viewmodel);
            if (result.length == 0) {
                MensErr("No se generó ningún boleto.");
            } else {
                $("#ModalBoleto").modal("show");
            }
        }, 250);
    }
}

function AsignarDatos() {
    viewModel.set("TipoCombo", datosIniCrearContrato.Datos.tiponegocio);
    var contrato = {
        
    };
    var url = '/Compranet/TraerContratoCompleto';
    var data = servicio;
    var result = MSExecuteOnServer(url, data);
}


function ValidarGenerarBoletos() {
    var contratoDesde = $("#GeneraBoletoContratoSAPId").val();
    if (contratoDesde == "") {
        MensErr("El campo Negocio no puede estar vacío");
        $.unblockUI();
        return false;
    }
    return true;
}


function modalcontratoGenerarBoleto() {
    $("#abrirGeneraBoletoPopUpCargarValores").click(function () {
        $("#popupcargarvalores").modal('toggle');
    });
}

$("#GeneraBoletoContratoSAPId").bind("paste", function (e) {
    e.preventDefault();
    if (e.originalEvent.clipboardData !== undefined) {
        clipText = e.originalEvent.clipboardData.getData('text/plain');
    } else {
        clipText = window.clipboardData.getData('text');
    }
    $("#GeneraBoletoContratoSAPId").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));

    GeneraBoletoCambioVariosContratos();
});



$("#GeneraBoletoContratoSAPId").change(GeneraBoletoCambioVariosContratos);

$("body").on("click", "#abrirGeneraBoletoPopUpCargarValores", function () {
    ListarNegocios();
});

$("body").on("click", "#filtrarBoletos", function () {
    FiltrarBoletos();
});

$("body").on("change", "#desde", function () {
    if ($("#desde").val()=='') {
        $("#hasta").prop("disabled", true);
        $("#filtrarBoletos").prop("disabled", true);
    } else {
        $("#hasta").removeAttr('disabled');
        $("#filtrarBoletos").removeAttr('disabled');
    }
});

function FiltrarBoletos() {
    let fechaDesde = $("#desde").val();
    let fechaHasta = $("#hasta").val();
    let tipoNegocio = $("#tipoId").val();
    BlockUi("Consultando...");
    var data = { desde: fechaDesde, hasta: fechaHasta, negocio: tipoNegocio };
    result = MSExecuteOnServer('/Boleto/FiltrarBoletos', data);
    $.unblockUI();
    if (result.length == 0) {
        MensAlerta("Sin Resultados");
    } else {
        var concatenada = result.join(';');
        $("#GeneraBoletoContratoSAPId").val(concatenada);
        GeneraBoletoCambioVariosContratos();
    }
}

function ListarNegocios() {
    let tipoNegocio = $("#tipoId").val();

    lista = $("#GeneraBoletoContratoSAPId").val().split(';');

    BlockUi("Consultando...");
    if (lista.length > 1) {
        var data = { listaContratoSap: lista, negocio: tipoNegocio };
    }
    else {
        var lista = new Array($("#GeneraBoletoContratoSAPId").val(), $("#GeneraBoletoContratoSAPHastaId").val());
        var data = { listaContratoSap: lista, negocio: tipoNegocio };
    }
    if (lista.length > 0 || ($("#GeneraBoletoContratoSAPId").val() != null && $("#GeneraBoletoContratoSAPHastaId").val() != null)) {
        result = MSExecuteOnServer('/Boleto/ListarContratos', data);
        $.unblockUI();
        if (result.length == 0) {
            MensAlerta("Sin Resultados.");
        } else {
            var concatenada = result.join(';');
            $("#GeneraBoletoContratoSAPId").val(concatenada);
            GeneraBoletoCambioVariosContratos();

            $('#popUpCargarValores').modal('show')
        }
    }
    else {
        MensAlerta("Problema en el rango de valores Contrato SAP.");
    }
}


function GeneraBoletoCambioVariosContratos() {

    if
        (parseInt($("#GeneraBoletoContratoSAPHastaId").val()) - parseInt($("#GeneraBoletoContratoSAPId").val()) > 1000) {

        PopUpError("Seleccione un rango de valores menor a 1000");

        return;
    }

    var lista = [];
    lista = $("#GeneraBoletoContratoSAPId").val().split(';');
    if (lista.length > 1) {

        $("#GeneraBoletoContratoSAPHastaId").attr('disabled', 'disabled');
        $("#GeneraBoletoContratoSAPHastaId").val("")
        ArmarTabla(lista);
    } else if ($.isNumeric($("#GeneraBoletoContratoSAPId").val()) && $.isNumeric($("#GeneraBoletoContratoSAPHastaId").val())) {

        $("#GeneraBoletoContratoSAPHastaId").removeAttr('disabled');

        lista = [];
        for (var i = parseInt($("#GeneraBoletoContratoSAPId").val()); i <= parseInt($("#GeneraBoletoContratoSAPHastaId").val()); i++) {
            lista.push(i);
        }
        ArmarTabla(lista);
    }
    else {
        $("#GeneraBoletoContratoSAPHastaId").removeAttr('disabled');
        ArmarTabla(lista);
    }
}

function inicializarPopUpConratoSap() {

    crearPopUp("Contratos");

    modalcontratoGenerarBoleto();


    $("#GeneraBoletoContratoSAPId").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#GeneraBoletoContratoSAPId").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));

        GeneraBoletoCambioVariosContratos();
    });

    $("#GeneraBoletoContratoSAPId").change(GeneraBoletoCambioVariosContratos);
    $("#ContratoSAPHastaId").change(GeneraBoletoCambioVariosContratos);

    $(document).on("click", ".agregarContrato", function () {
        var num = $("#nuevoNumContrato").val();
        if (num.trim() != "" && num.trim() != null) {
            $("#contratos-table").append('<tr><td>' + num + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" style="height: 34px;float: right" type="button"></button></td></tr></td></tr>');
            $(".nuevoNumContrato").val('');
            $(".nuevoNumContrato").focus();
            GenerarContratoSAPDesde();
        }
    });

    $(document).on("click", ".borrarContrato", function () {
        $(this).parent().parent().remove();
        GenerarContratoSAPDesde();
    });
}