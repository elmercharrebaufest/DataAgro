var fechaString;
var url;
$(document).ready(function () {
    fechaString = ObtenerFechaDesde();
    url = $('#descargaReporte').attr('href');
    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString);
    InicializarDate();
    setInterval(Refrescar, 300000);
});

function InicializarDate() {
    kendo.culture("es-AR");
    var date = ObtenerFechaDesde();
    $("#fecha").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]       
    });
    $("#fecha").change(function () {
        fechaString = $("#fecha").val();
        $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString);
    });
}
      
function ObtenerFechaDesde() {
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

function Refrescar() {
    $("#buscar-reporte").click();
}