$("body").on("click", "#filtrarBoletos", function () {
    FiltrarBoletos();
});

function FiltrarBoletos() {
    let fechaDesde = $("#desde").val();
    let fechaHasta = $("#hasta").val();
    let tipoNegocio = $("#tipoId").val();
    var data = { desde: fechaDesde, hasta: fechaHasta, negocio: tipoNegocio };
    var result = MSExecuteOnServer('/Boleto/FiltrarBoletos', data);
    var concatenada = result.join(';');
    $("#ContratoSAPId").val(concatenada);
    CambioVariosContratos();
}