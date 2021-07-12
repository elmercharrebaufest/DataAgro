var pausado;
var material = [];
$(document).ready(function () {
    kendo.culture("es-AR");

    kendo.culture();
    InicializarElementos();

});


function InicializarElementos() {
    var hoy = new Date();
    pausado = ConvertirStringABool(pausado);

    $("#DesdeVigencia").kendoDateTimePicker();
    $("#HastaVigencia").kendoDateTimePicker();
    $("#DesdeVigenciaPago").kendoDateTimePicker();
    $("#HastaVigenciaPago").kendoDateTimePicker();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigenciaPago").val(stringDia + " " + "00:00");
    $("#HastaVigenciaPago").val(stringDia + " " + "23:59");
    $("#Precio").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    $("#CantidadDia").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false
    });
    $("#Tasa").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#PrecioSustentable").kendoNumericTextBox({ culture: "es-AR", format: "n2", spinners: false, min: 0 });
    $("#DesdeVigenciaSustentable").kendoDateTimePicker();
    $("#HastaVigenciaSustentable").kendoDateTimePicker();
    $("#DesdeEntregaSustentable").kendoDatePicker();
    $("#HastaEntregaSustentable").kendoDatePicker();

    $("#DiaPizarra").kendoDatePicker();
    $("#DiaPizarraHasta").kendoDatePicker();
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
    $("#PagoTab").click(function () {
        DeseleccionarForms();
        $("#PagoTab").children().addClass("whc-selected");
        $("#HabilitacionPagoDiferido").show();
    });
    $("#SustentableTab").click(function () {
        DeseleccionarForms();
        $("#SustentableTab").children().addClass("whc-selected");
        $("#HabilitacionSustentable").show();
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

    $("#selectall").on("click", function () {
        $(".case").prop("checked", this.checked);
    });

    // if all checkbox are selected, check the selectall checkbox and viceversa  
    $(".case").on("click", function () {
        if ($(".case").length == $(".case:checked").length) {
            $("#selectall").prop("checked", true);
        } else {
            $("#selectall").prop("checked", false);
        }
    });

    if (pausado == true) {
        $(".pausado").prop("checked", this.checked);
    }


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
function LimpiarPagoForm() {
    $("#CantidadDia").data("kendoNumericTextBox").value("0");
    $("#Importe").data("kendoNumericTextBox").value("0");
    $("#TipoNegocioIdPago").val("");
    $("#MaterialIdPago").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigenciaPago").val(stringDia + " " + "00:00");
    $("#HastaVigenciaPago").val(stringDia + " " + "23:59");
}
function LimpiarPizarraForm() {
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DiaPizarra").val(stringDia);
    $("#DiaPizarraHasta").val(stringDia);
    $("#PizarraDesde").val("00:00");
    $("#PizarraHasta").val("23:59");
}
function LimpiarFijacionForm() {
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#FijacionDia").val(stringDia);
    $("#MaterialFijacionId").val("");
}
function LimpiarSustentableForm() {
    $("#MonedaId").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#PrecioSustentable").data("kendoNumericTextBox").value("0");
    $("#DesdeVigenciaSustentable").val(stringDia + " " + "00:00");
    $("#HastaVigenciaSustentable").val(stringDia + " " + "23:59");
    $("#DesdeEntregaSustentable").data("kendoDatePicker").value("");
    $("#HastaEntregaSustentable").data("kendoDatePicker").value("");
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
    $("#PagoTab").children().removeClass("whc-selected");
    $("#HabilitacionPagoDiferido").hide();
    $("#SustentableTab").children().removeClass("whc-selected");
    $("#HabilitacionSustentable").hide();
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

function copiarPrecioMOA(configuracion) {
    configuracion = JSON.parse(configuracion);
    $("#TipoNegocioId").val(configuracion.TipoNegocioId);
    $("#TipoNegocioId").change();
    $("#MonedaId").val(configuracion.MonedaId);
    $("#MaterialId").val(configuracion.MaterialId);
    $("#DestinoId").val(configuracion.DestinoId);
    if (configuracion.Precio > 0) {
        $("#Precio").data("kendoNumericTextBox").value(configuracion.Precio);
    }
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigencia").val(stringDia + " " + "00:00");
    $("#HastaVigencia").val(stringDia + " " + "23:59");
    $("#DesdeEntrega").data("kendoDatePicker").value(new Date(configuracion.DesdeEntrega));
    $("#HastaEntrega").data("kendoDatePicker").value(new Date(configuracion.HastaEntrega));
    if (configuracion.DesdeFijacion != null) {
        $("#DesdeFijacion").data("kendoDatePicker").value(new Date(configuracion.DesdeFijacion));
    }
    if (configuracion.HastaFijacion != null) {
        $("#HastaFijacion").data("kendoDatePicker").value(new Date(configuracion.HastaFijacion));
    }
}

function copiarPago(configuracion) {
    configuracion = JSON.parse(configuracion);
    $("#TipoNegocioId").val(configuracion.TipoNegocioId);
    $("#TipoNegocioId").change();
    $("#MaterialId").val(configuracion.MaterialId);
    if (configuracion.Importe > 0) {
        $("#Importe").data("kendoNumericTextBox").value(configuracion.Importe);
    }
    if (configuracion.CantidadDia > 0) {
        $("#CantidadDia").data("kendoNumericTextBox").value(configuracion.CantidadDia);
    }
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigenciaPago").val(stringDia + " " + "00:00");
    $("#HastaVigenciaPago").val(stringDia + " " + "23:59");

}

//function Pausar() {  
//    MSExecuteOnServer("ConfiguracionInterna/PausarPrecios", { pausa: $("#pausar").is(":checked") ? true : false});
//    MensInfo("El cambio se guardó correctamente");
//}

function ObtenerDatosMaterialHabilitado() {

    var obj = {};
    var lista = [];
    obj.MaterialId = 0;
    obj.Habilitado = $("#selectall").is(":checked");
    lista.push(obj);
    for (var i = 0; i < material.length; i++) {
        obj = {}
        obj.MaterialId = material[i].MaterialId;
        obj.Habilitado = $("#" + material[i].MaterialId).is(":checked");
        lista.push(obj);
    }

    MSExecuteOnServer("ConfiguracionInterna/PausarPrecios", { lista: lista });
    MensInfo("El cambio se guardó correctamente");
}

function AbrirModal() {
    $("#modalHabilitarMaterial").modal("show");
}

