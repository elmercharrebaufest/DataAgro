$(document).ready(function () {
  
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
}