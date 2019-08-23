$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarDate();
});

function copia_portapapeles(data) {
    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', data);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', data);
        }
    }
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
}

function Refrescar() {
    $("#buscar").click();
}

function InicializarDate() {
    kendo.culture("es-AR");
    var date = ObtenerFecha();
    $("#fecha").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]        
    });
    fechaString = $("#fecha").val();
}

function ObtenerFecha() {
    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

