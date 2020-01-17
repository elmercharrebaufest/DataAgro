$(document).ready(function () {
    kendo.culture("es-AR");

    kendo.culture();
    InicializarElementos();
});


function InicializarElementos() {
    var hoy = new Date();
    $("#DesdeVigencia").kendoDateTimePicker();
    $("#HastaVigencia").kendoDateTimePicker();
    $("#Precio").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    $("#DiaPizarra").kendoDatePicker();
    $("#PizarraDesde").kendoTimePicker();
    $("#PizarraHasta").kendoTimePicker();
    $("#FijacionDia").kendoDatePicker();
    $("#PrecioTab").click(function () {
        DeseleccionarForms();
        $("#PrecioTab").children().addClass("whc-selected");
        $("#PrecioMoa").show();
    });
    $("#PizarraTab").click(function () {
        DeseleccionarForms();
        $("#PizarraTab").children().addClass("whc-selected");
        $("#HabilitacionPizarra").show();
    });
    $("#FijacionTab").click(function () {
        DeseleccionarForms();
        $("#FijacionTab").children().addClass("whc-selected");
        $("#HabilitacionFijacion").show();
    });
}

function LimpiarPrecioForm() {
    $("#Precio").data("kendoNumericTextBox").value("0");
    $("#MonedaId").val("");
    $("#MaterialId").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth()+1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigencia").val(stringDia + " " + "00:00");
    $("#HastaVigencia").val(stringDia + " " + "23:59");
}
function LimpiarPizarraForm() {
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DiaPizarra").val(stringDia);
    $("#PizarraDesde").val("00:00");
    $("#PizarraHasta").val("23:59");
}
function LimpiarFijacionForm() {
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#FijacionDia").val(stringDia);
    $("#MaterialFijacionId").val("");
}

function DeseleccionarForms() {
    $("#PrecioTab").children().removeClass("whc-selected");
    $("#PrecioMoa").hide();
    $("#PizarraTab").children().removeClass("whc-selected");
    $("#HabilitacionPizarra").hide();
    $("#FijacionTab").children().removeClass("whc-selected");
    $("#HabilitacionFijacion").hide();
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}