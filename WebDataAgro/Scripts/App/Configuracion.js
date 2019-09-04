$(document).ready(function () {
  
    InicializarElementos();
});


function InicializarElementos() {
    $("#CantidadDias").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
}