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
    $("#DesdeEntregaPizarra").kendoDatePicker();
    $("#HastaEntregaPizarra").kendoDatePicker();
    $("#DesdeEntrega").kendoDatePicker();
    $("#HastaEntrega").kendoDatePicker();
    $("#DesdeFijacion").kendoDatePicker();
    $("#HastaFijacion").kendoDatePicker();
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
    $("#CampañaTab").click(function () {
        DeseleccionarForms();
        $("#CampañaTab").children().addClass("whc-selected");
        $("#HabilitacionCampaña").show();
    });
    $("#FijacionTab").click(function () {
        DeseleccionarForms();
        $("#FijacionTab").children().addClass("whc-selected");
        $("#HabilitacionFijacion").show();
    });

    $("#TipoNegocioId").change(function () {       
        LimpiarPrecioForm();
        var value = $("#TipoNegocioId").val();
        if (value == 1) {
            $("#divPrecioFijacion").show();
            $("#divPrecioEntrega").show();
            $("#Precio").data("kendoNumericTextBox").enable(false);
            $("#MonedaId").prop('disabled', 'disabled');
        }
        if (value == 2) {
            $("#divPrecioFijacion").hide();
            $("#divPrecioEntrega").show();
            $("#Precio").data("kendoNumericTextBox").enable(true);
            $("#MonedaId").prop('disabled', false);
        }
        if (value == 3) {
            $("#divPrecioFijacion").hide();
            $("#divPrecioEntrega").hide();
            $("#Precio").data("kendoNumericTextBox").enable(true);
            $("#MonedaId").prop('disabled', false);

        }
        if (value == undefined || value == null || value == "") {
            $("#divPrecioFijacion").hide();
            $("#divPrecioEntrega").hide();
        }
    });

    $("#TipoNegocioIdPizarra").change(function () {
        LimpiarPizarraForm();
        var value = $("#TipoNegocioIdPizarra").val();
        if (value == 2) {
            $("#divEntregaPizarra").show();
        }
        if (value == 3) {
            $("#divEntregaPizarra").hide();
        }
        if (value == undefined || value == null || value == "") {
            $("#divEntregaPizarra").hide();
        }
    });
    $("#TipoNegocioId").change();
    $("#TipoNegocioIdPizarra").change();
}

function LimpiarPrecioForm() {
    $("#Precio").data("kendoNumericTextBox").value("0");
    $("#MonedaId").val("");
    $("#MaterialId").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigencia").val(stringDia + " " + "00:00");
    $("#HastaVigencia").val(stringDia + " " + "23:59");
    $("#DesdeEntrega").data("kendoDatePicker").value("");
    $("#HastaEntrega").data("kendoDatePicker").value("");
    $("#DesdeFijacion").data("kendoDatePicker").value("");
    $("#HastaFijacion").data("kendoDatePicker").value("");
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
    $("#CampañaTab").children().removeClass("whc-selected");
    $("#HabilitacionCampaña").hide();
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}