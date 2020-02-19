var diaCerrado;
var matLen;
var objLen;
var diferencial;

$(document).ready(function () {
  
    InicializarElementos();  
});

function TimeOut() {
    window.setTimeout(function () {
        $(".alert").fadeTo(1000, 0, function () {
            $(this).remove();
        });
    }, 3000);
}


function InicializarElementos() {
    $(".number-input-diferencial").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
}

function diferencialInput() {
    $(".number-input-diferencial").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
   
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

function desplegarHistorial() {
    mostrarocultar('#mostrar-historial');
    $("#historialMaterial").show();
    InicializarElementos();
}
