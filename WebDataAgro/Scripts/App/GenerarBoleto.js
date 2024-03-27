var viewModel;
var datosIniCrearContrato;
var externo;
var enviarEmail;

$(document).ready(function () {

    $('#menuproveedor').hide();
    InicializarDatos();
    inicializarPopUpSap("Contratos");
});


function Filtrar() {


    if (ValidarGenerarBoletos()) {
        BlockUi('Cargando...');
        setTimeout(function () {
            var boleto = {
                ContratoSAP: $("#ContratoSAPId").val().padStart(10, '0'),
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
            $("#ModalBoleto").modal("show");
            if (result.count == 0) {
                MensInfo("Prueba")
            }
        }, 250);
    }
}

function InicializarDatos() {
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

    var contratoDesde = $("#ContratoSAPId").val();
    if (contratoDesde == "") {
        MensErr("El campo Negocio no puede estar vacío");
        $.unblockUI();
        return false;
    }


    return true;
}

$("body").on("click", "#filtrarBoletos", function () {
    FiltrarBoletos();
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
        $("#ContratoSAPId").val(concatenada);
        CambioVariosContratos();
    }
}