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