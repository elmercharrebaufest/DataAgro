$(document).ready(function () {
  kendo
    InicializarElementos();
});


function InicializarElementos() {
    $(".number-input").kendoNumericTextBox({
        format:"####################",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: 'n2',
        decimals: 2,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
}