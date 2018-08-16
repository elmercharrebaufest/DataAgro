var viewModel;
var datosIniCrearContrato;
var contratoEdit;
var contratoId;
var fijacionId;
$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });
    CrearViewModel();
    InicializarElementos();
    InicializarDatos();
});

function ObtenerFechaDesde(fechaBase) {
    var hoy = fechaBase != undefined ? fechaBase : new Date();
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

function ObtenerFechaHasta(fechaBase) {
    var hoy = fechaBase != undefined ? fechaBase : new Date();
    var anio = hoy.getFullYear();
    var mesPost = hoy.getMonth() + 2;
    var dia = hoy.getDate();
    
    if (dia == 1) {
        dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
        mesPost = hoy.getMonth() + 1;
    }
    if (mesPost < 10) {
        mesPost = "0" + mesPost.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return  dia + '-' + mesPost + '-' + anio;
}

function InicializarElementos() {
    kendo.culture("es-AR");

    $(".datos-adicionales").hide();
    $(".datos-boleto").hide();
    $(".datos-establecimiento").hide();
    $(".datos-topesplazos").hide();
    $(".datos-calidades").hide();
    $(".datos-descuentos").hide();

    $("#proveedorId").kendoDropDownList({
        optionLabel: "SELECCIONE UN PROVEEDOR...",
        dataTextField: "Descripcion",
        dataValueField: "ProveedorId"
    });

    $("#proveedorId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#proveedorId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").val("");
    });

    $("#buscadorProveedor").keyup(function (e) {
        armarBusquedaResultProveedor(e);
    });

    $("#buscadorProveedor").focus(function (e) {
        e.stopPropagation();
        e.preventDefault();
    });

    $("#comercialId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId",
    });

    $("#comercialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#comercialModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId",
    });

    $("#comercialModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#plazosYTopesFijacion").hide();
    $("#tipoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId",
        change: function () {
            $("#plazosYTopesFijacion").hide();
            $("#pagosDiv").hide();
            if (this.value() == 3) {
                $("#fechasDiv").hide();
                $("#fechaDesdeDiv").hide();
                $("#fechaHastaDiv").hide();
                $("#operacionDiv").hide();
                $("#campanaDiv").hide();
                $("#procedenciaDiv").hide();
                $("#clasificacionDiv").hide();
                $("#destinoDiv").hide();
                $("#CantidadCamionesDiv").hide();
                $("#planCanjeConsignatarioIdDiv").hide();
                $("#DatosBoleto").hide();
                $("#DatosPago").hide();
                $("#DatosEstablecimiento").hide();
                $("#DatosDescuentos").hide();
                $("#baseDiv").hide();
                $("#DatosAdicionales").hide();
                $(".datos-adicionales").hide();
                $("#DatosCalidades").hide();
                $(".datos-calidades").hide();
                $(".datos-boleto").hide();
                $(".datos-topesplazos").hide();
                $(".datos-establecimiento").hide();
                $("#ContratoDiv").show();
                $("#guardarBtn").empty();
                $("#DatosBoleto").hide();
                $("#DatosPago").hide();
                $("#observacionDiv").removeClass("col-md-3");
                $("#observacionDiv").addClass("col-md-6");
                $("#guardarBtn").append("Guardar Fijacion");
            }
            else {
                $("#fechasDiv").show();
                $("#fechaDesdeDiv").show();
                $("#fechaHastaDiv").show();
                $("#fechaHastaContratoDiv").show();
                $("#campanaDiv").show();
                $("#procedenciaDiv").show();
                $("#clasificacionDiv").show();
                $("#destinoDiv").show();
                $("#CantidadCamionesDiv").show();
                $("#planCanjeConsignatarioIdDiv").show();
                $("#DatosBoleto").show();
                $("#DatosPago").show();
                $("#DatosEstablecimiento").show();
                $("#DatosCalidades").show();
                $("#DatosDescuentos").show();
                $("#baseDiv").show();
                $("#DatosAdicionales").show();
                $("#ContratoDiv").hide();
                $("#guardarBtn").empty();
                $("#DatosBoleto").show();
                $("#observacionDiv").removeClass("col-md-6");
                $("#observacionDiv").addClass("col-md-3");
                $("#guardarBtn").append("Generar Negocio");
                if (this.value() == 1) {
                    $("#plazosYTopesFijacion").show();
                    $("#CDId").prop("checked", false);
                    $("#WarrantId").prop("checked", false);
                    $("#pagoDirectoId").prop("checked", false);
                }
                if (this.value() == 2) {
                    $("#pagosDiv").show();
                    $("#fechaDesdeTopeId").val("");
                    $("#fechaHastaTopeId").val("");
                    $("#condicionFijacionId").data("kendoDropDownList").value("");
                }
                if (contratoId !== undefined && contratoId) {
                    $("#operacionDiv").show();
                }
            }
        }
    });

    $("#tipoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#material").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#precioMonedaId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#precioMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#precioMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#campanaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });

    $("#campanaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#establecimientoDiv").hide();
    $("#provinciaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA PROVINCIA...",
        dataTextField: "Nombre",
        dataValueField: "Provinciaid",
        change: function () {

            if (this.value() != "") {
                CargarLocalidadPorProvincia(this.value());
            }

            if (this.value() == 1) {
                $("#establecimientoDiv").show();
            } else {
                $("#establecimientoDiv").hide();
                $("#establecimientoPropioId").prop("checked", false);
                $("#establecimientoArrendadoId").prop("checked", false);
            }
        }
    });

    $("#provinciaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#provinciaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#LocalidadId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA LOCALIDAD...",
        dataTextField: "Nombre",
        dataValueField: "LocalidadId"
    });

    $("#LocalidadId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#LocalidadId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#sustentableMonedaId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#sustentableMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#sustentableMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#statusId").kendoDropDownList({
        optionLabel: "Estado de Contrato...",
        dataTextField: "Descripcion",
        dataValueField: "EstadosContratosId"
    });

    $("#statusId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#statusId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#materialFiltroIndex").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#materialFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#materialFiltroIndex").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#campanaFiltroIndex").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });

    $("#campanaFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaFiltroIndex").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#campanaModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });

    $("#campanaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#statusFiltroIndex").kendoDropDownList({
        optionLabel: "Estado de Contrato...",
        dataTextField: "Descripcion",
        dataValueField: "EstadosContratosId"
    });

    $("#statusFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#statusFiltroIndex").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#consignatarioDiv").hide();
    $("#clasificacion").kendoDropDownList({
        optionLabel: "SELECCIONE LA CLASIFICACIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            if (this.value() == 2) {
                $("#consignatarioDiv").show()
            } else {
                $("#consignatarioDiv").hide();
                $("#consignatarioId").prop("checked", false); 
            }
        }
    });

    $("#clasificacion").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacion").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#clasificacionModalPendienteId").kendoDropDownList({
        optionLabel: "CLASIFICACIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#clasificacionModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacionModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#destinoId").kendoDropDownList({
        optionLabel: "SELECCIONE EL DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#destinoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#destinoModalPendienteId").kendoDropDownList({
        optionLabel: "DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#destinoModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinoModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#bolsaConfirmaId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    var bolsaConfirma = $("#bolsaConfirmaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaConfirmaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#bolsaConfirmaIdModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#bolsaConfirmaIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaConfirmaIdModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#bolsaFisicoId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#bolsaFisicoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaFisicoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#bolsaFisicoIdModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#bolsaFisicoIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaFisicoIdModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#condicionFijacionId").kendoDropDownList({
        optionLabel: "SELECCIONE CONDICIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#condicionFijacionId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#condicionFijacionId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#condicionFijacionIdModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE CONDICIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
    });

    $("#condicionFijacionIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#condicionFijacionIdModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#standardCalidadId").kendoDropDownList({
        optionLabel: "SELECCIONE STANDARD DE CALIDAD...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#standardCalidadId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#standardCalidadId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#calidadesEspecialesId").kendoDropDownList({
        optionLabel: "SELECCIONE CALIDAD",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#calidadesEspecialesId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#calidadesEspecialesId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#cantidadId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        //change: function () {
        //    $("#cantidadCamionesId").data("kendoNumericTextBox").value(Math.ceil(this.value() / 30000));
        //}
    });

    $("#cantidadCamionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });
    $("#precioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#sustentablePrecioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#pesificadoDiasId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#valorEspecialesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    var date = ObtenerFechaDesde();
    var datehasta = ObtenerFechaHasta();

    $("#fechaDesdeId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () { $("#fechaHastaId").val(ObtenerFechaHasta(this.value()))},
    });
    $("#fechaHastaId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaOperacionId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () { $("#fechaHastaTopeId").val(ObtenerFechaHasta(this.value())) },
    });
    $("#fechaHastaTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#dolarizadoFechaId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeId").val(date);
    $("#fechaHastaId").val(datehasta);
    $("#fechaOperacionId").val(date);

    $(".formulario-footer-guardar-contrato").click(function () {
        ObtenerDatos();
    });

    $(".formulario-footer-cancelar").click(function () {
        window.location.href = window.location.origin + "/CompraNet";
    });

    $("#DatosAdicionales").click(function () {
        if (document.querySelector(".datos-adicionales").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-adicionales").style.display = "block";
        } else {
            document.querySelector(".datos-adicionales").style.display = "none";
        }
    });

    $("#DatosBoleto").click(function () {
        if (document.querySelector(".datos-boleto").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-boleto").style.display = "block";
        } else {
            document.querySelector(".datos-boleto").style.display = "none";
        }
    });
    
    $("#DatosDescuentos").click(function () {
        if (document.querySelector(".datos-descuentos").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-descuentos").style.display = "block";
        } else {
            document.querySelector(".datos-descuentos").style.display = "none";
        }
    });

    $("#DatosCalidades").click(function () {
        if (document.querySelector(".datos-calidades").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-calidades").style.display = "block";
        } else {
            document.querySelector(".datos-calidades").style.display = "none";
        }
    });

    $("#sustentableId").click(function () {
        if ($(this).is(':checked')) {
            $("#sustentableDiv").show();
        }
        else {
            $("#sustentableDiv").hide();
            $("#sustentablePrecioId").data("kendoNumericTextBox").value("");
        }
    });
    
    $("#dolarizadoId").click(function () {
        if ($(this).is(':checked')) {
            $("#dolarizadoDiv").show();
        }
        else {
            $("#dolarizadoDiv").hide();
            $("#dolarizadoFechaId").val("");
        }
    });

    $("#pesificadoId").click(function () {
        if ($(this).is(':checked')) {
            $("#pesificadoDiv").show();
        }
        else {
            $("#pesificadoDiv").hide();
            $("#pesificadoDiasId").data("kendoNumericTextBox").value("");;
        }
    });

    $("#boletoConfirmaId").click(function () {
        if ($(this).is(':checked')) {
            $("#BolsaConfirmaDiv").show();
            $("#BolsaFisicoDiv").hide();
            $("#boletoFisicoId").prop("checked", false);
            $("#boletoNingunoId").prop("checked", false);
            $("#bolsaFisicoId").data("kendoDropDownList").value("");
        }
        else {
            $("#BolsaConfirmaDiv").hide();
            $("#bolsaConfirmaId").data("kendoDropDownList").value("");
        }
    });

    $("#boletoFisicoId").click(function () {
        if ($(this).is(':checked')) {
            $("#BolsaFisicoDiv").show();
            $("#BolsaConfirmaDiv").hide();
            $("#boletoConfirmaId").prop("checked", false);
            $("#boletoNingunoId").prop("checked", false);
            $("#bolsaConfirmaId").data("kendoDropDownList").value("");
        }
        else {
            $("#BolsaFisicoDiv").hide();
            $("#bolsaFisicoId").data("kendoDropDownList").value("");
        }
    });

    $("#boletoNingunoId").click(function () {
        if ($(this).is(':checked')) {
            $("#BolsaConfirmaDiv").hide();
            $("#BolsaFisicoDiv").hide();
            $("#boletoFisicoId").prop("checked", false);
            $("#boletoConfirmaId").prop("checked", false);
            $("#bolsaFisicoId").data("kendoDropDownList").value("");
            $("#bolsaConfirmaId").data("kendoDropDownList").value("");
        }
    });

    $("#CDId").click(function () {
        $("#WarrantId").prop("checked", false);
        $("#pagoDirectoId").prop("checked", false);
    });

    $("#WarrantId").click(function () {
        $("#CDId").prop("checked", false);
        $("#pagoDirectoId").prop("checked", false);
    });

    $("#pagoDirectoId").click(function () {
        $("#CDId").prop("checked", false);
        $("#WarrantId").prop("checked", false);
    });

    $("#establecimientoPropioId").click(function () {
        $("#establecimientoArrendadoId").prop("checked", false);        
    });
    $("#establecimientoArrendadoId").click(function () {
        $("#establecimientoPropioId").prop("checked", false);       
    });

    $('select[id="material"]').change(function () {
        if ($(this).val() != "") {
            CargarCampaniaPorMaterial($(this).val());
            CargarCalidadPorMaterial($(this).val());
            var iteraciones = viewModel.Calidades.length;
            for (var i = 0; i < iteraciones; i++) {
                viewModel.Calidades.pop();
            }
        }
    });

    $('#material').change(function () {
        obtenerLocalidadProvincia();
    });

    $('#campanaId').change(function () {
        obtenerLocalidadProvincia();
    });

    $('#tipoId').change(function () {
        if ($('#tipoId').val() == 1) {
            $("#condicionFijacionId").data("kendoDropDownList").value("7");
            $("#fechaDesdeTopeId").val(date);
            $("#fechaHastaTopeId").val(datehasta);
        } else {
            $("#condicionFijacionId").data("kendoDropDownList").value("");
            $("#fechaDesdeTopeId").val("");
            $("#fechaHastaTopeId").val("");
        }
    });

    $('#standardCalidadId').change(function () {
        if ($(this).val() == 2) {
            $("#especialesId").show();
        }
        else {
            $("#especialesId").hide();
            $("#calidadesEspecialesId").data("kendoDropDownList").value("");
            $("#valorEspecialesId").data("kendoNumericTextBox").value("");
        }
    });
    $("#tipoPeriodoDBId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        dataBound: function () {
            this.select(0);
        }
    });

    $("#tipoPeriodoDBId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoPeriodoDBId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#TipoDBId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        dataBound: function () {
            this.select(0);
        }
    });

    $("#TipoDBId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#TipoDBId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#fechaDesdeDescuentoId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () { $("#fechaHastaDescuentoId").val(ObtenerFechaHasta(this.value())) }
    });
    $("#fechaHastaDescuentoId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#descuentoMonedaId").kendoDropDownList({
        optionLabel: "Moneda",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#descuentoMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#descuentoMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    //$("#PorcentajeDescuentoId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n2",
    //    spinners: false,
    //    min: 0
    //});
    $("#PorcentajeDescuentoId").val(1);

    $('select[id="tipoPeriodoDBId"]').change(function () {
        if ($(this).val() == 1) {
            $(".fecha-descuento").hide();
            $(".fecha-descuento-pendiente").hide();
            $("#fechaHastaDescuentoId").val("");
            $("#fechaDesdeDescuentoId").val("");
        }
        else {
            $(".fecha-descuento").show();
            $(".fecha-descuento-pendiente").show();
            $("#fechaDesdeDescuentoId").val(date)
            $("#fechaHastaDescuentoId").val(datehasta)
        }
    });
    $("#IngresarDescuento").click(AgregarDescuentos);
    $("#IngresarCalidad").click(AgregarCalidades);
}

function CargarCampaniaPorMaterial(value) {
    var resultGrano = MSExecuteOnServer('/CompraNet/TraerCampanaPorMaterial', { MaterialId: value });

    var campanaActualId = MSExecuteOnServer('/CompraNet/TraerCampanaActualMaterial', { MaterialId: value });

    viewModel.set("CampanaCombo", resultGrano);

    $("#campanaId").data("kendoDropDownList").value(campanaActualId);
}

function CerrarDatosPendientes() {
    document.querySelector(".datos-adicionales").style.display = "none";
    document.querySelector(".datos-boleto").style.display = "none";
    document.querySelector(".datos-descuentos").style.display = "none";
    document.querySelector(".datos-calidades").style.display = "none";
}

function CargarCalidadPorMaterial(value) {
    var calidadGrano = MSExecuteOnServer('/CompraNet/TraerCalidadesPorMaterial', { MaterialId: value });
    viewModel.set("EspecialesCombo", calidadGrano);
}

function CargarLocalidadPorProvincia(value) {
    var resultLocalidad = MSExecuteOnServer('/CompraNet/TraerLocalidadPorProvincia', { ProvinciaId: value });

    viewModel.set("LocalidadCombo", resultLocalidad.Localidad);
    viewModel.set("LocalidadComboModalPendiente", resultLocalidad.Localidad);
}

function CrearViewModel() {
    var param = {
        "proveedorId": null,
        "material": null,
        "materialDesc": null,
        "cantidadId": null,
        "precioId": null,
        "campanaId": null,
        "campanaDesc": null,
        "provinciaId": null,
        "provinciaIdDesc": null,
        "LocalidadId": null,
        "LocalidadIdDesc": null,
        "baseId": null,
        "sustentableId": null,
        "sustentablePrecioId": null,
        "dolarizadoId": null,
        "dolarizadoFechaId": null,
        "pesificadoId": null,
        "pesificadoDiasId": null,
        "noInformaSioId": null,
        "trigoEspecialId": null,
        "statusId": null,
        "observacionId": null,
        "clasificacionId": null,
        "cantidadCamionesId": null,
        "condicionFijacionId": null,
        "destinoId": null,
        "planCanjeId": null,
        "consignatarioId": null,
        "cdId": null,
        "warrantId": null,
        "pagoDirectoId": null,
        "standardCalidadId": null,
        "calidadesEspecialesId": null,
        "tipoPeriodoDBId": null,

        "proveedorIdModal": null,
        "comercialIdModal": null,
        "comercialDescModal": null,
        "materialModal": null,
        "materialDescModal": null,
        "cantidadIdModal": null,
        "tipoIdModal": null,
        "tipoDescModal": null,
        "precioIdModal": null,
        "precioMonedaIdModal": null,
        "precioMonedaDescModal": null,
        "campanaIdModal": null,
        "campanaDescModal": null,
        "fechaDesdeIdModal": null,
        "fechaHastaIdModal": null,
        "fechaEntregaIdModal": null,
        "provinciaModalPendienteId": null,
        "provinciaIdDescModal": null,
        "LocalidadIdModal": null,
        "LocalidadIdDescModal": null,
        "baseIdModal": null,
        "sustentableIdModal": null,
        "sustentablePrecioIdModal": null,
        "sustentableMonedaIdModal": null,
        "sustentableMonedaDescModal": null,
        "dolarizadoIdModal": null,
        "dolarizadoFechaIdModal": null,
        "pesificadoIdModal": null,
        "pesificadoDiasIdModal": null,
        "noInformaSioIdModal": null,
        "trigoEspecialIdModal": null,
        "observacionIdModal": null,
        "clasificacionIdModal": null,
        "cantidadCamionesIdModal": null,
        "condicionFijacionIdModal": null,
        "destinoIdModal": null,
        "planCanjeIdModal": null,
        "consignatarioIdModal": null,
        "cDIdModal": null,
        "warrantIdModal": null,
        "pagoDirectoIdModal": null,
        "standardCalidadIdModal": null,
        "calidadesEspecialesIdModal": null,
        "tipoPeriodoDBIdModal": null,
        "Descuentos": null
    };
    viewModel = kendo.observable({
        Parametros: param,
        
        ComercialCombo: [],
        MaterialCombo: [],
        TipoCombo: [],
        PrecioMonedaCombo: [],
        CampanaCombo: [],
        ProvinciaCombo: [],
        LocalidadCombo: [],
        SustentableMonedaCombo: [],
        EstadoCombo: [],
        CondicionFijacionCombo: [],
        StandardCombo: [],
        EspecialesCombo: [],
        TipoPeriodoDBCombo: [],
        TipoDBCombo: [],

        ComercialComboModalPendiente: [],
        MaterialComboModalPendiente: [],
        TipoComboModalPendiente: [],
        PrecioMonedaComboModalPendiente: [],
        CampanaComboModalPendiente: [],
        ProvinciaComboModalPendiente: [],
        LocalidadComboModalPendiente: [],
        SustentableMonedaComboModalPendiente: [],
        Clasificacion: [],
        CondicionFijacionComboModalPendiente: [],
        Destino: [],
        StandardComboModalPendiente: [],
        EspecialesComboModalPendiente: [],
        isControlDisabled: true,
        Descuentos: [],
        Calidades:[],
        DescuentosVisualizar: [],
        CalidadesVisualizar: []
    });

    kendo.bind($("#CrearContrato"), viewModel);
    kendo.bind($("#CompraNet"), viewModel);
    kendo.bind($("#modalPendienteDiv"), viewModel);
    kendo.bind($("#tabla-descuentos"), viewModel);
    kendo.bind($("#tabla-descuentos-visualizar"), viewModel);
    kendo.bind($("#tabla-calidades"), viewModel);
    kendo.bind($("#tabla-calidades-visualizar"), viewModel);
}

function InicializarDatos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniCrearContrato = data;
            AsignarDatos();
        }

        if (contratoId !== undefined && contratoId) {
            setTimeout(InicializarContratoEdit, 300);
        } else if (fijacionId !== undefined && fijacionId) {
            setTimeout(InicializarFijacionEdit, 300);
        } else {
            $.unblockUI();
        }
    }

    MSExecuteURLOnServerAsync('/CompraNet/InicializarContrato', funcReturn, '');
}

function AsignarDatos() {
    viewModel.set("ComercialCombo", datosIniCrearContrato.Datos.comercial);
    viewModel.set("MaterialCombo", datosIniCrearContrato.Datos.material);
    viewModel.set("TipoCombo", datosIniCrearContrato.Datos.tiponegocio);
    viewModel.set("PrecioMonedaCombo", datosIniCrearContrato.Datos.moneda);
    viewModel.set("ProvinciaCombo", datosIniCrearContrato.Datos.prov);
    viewModel.set("LocalidadCombo", datosIniCrearContrato.Datos.loc);
    viewModel.set("SustentableMonedaCombo", datosIniCrearContrato.Datos.monedaSustentable);
    viewModel.set("EstadoCombo", datosIniCrearContrato.Datos.estadoContrato);
    viewModel.set("CampanaCombo", datosIniCrearContrato.Datos.campaña);
    viewModel.set("ClasificacionCombo", datosIniCrearContrato.Datos.Clasificacion);
    viewModel.set("DestinoCombo", datosIniCrearContrato.Datos.Destino);
    viewModel.set("BolsaCombo", datosIniCrearContrato.Datos.Bolsa);
    viewModel.set("CondicionFijacionCombo", datosIniCrearContrato.Datos.Condicion);
    viewModel.set("StandardCombo", datosIniCrearContrato.Datos.Standard);

    viewModel.set("TipoPeriodoDBCombo", datosIniCrearContrato.Datos.TipoPeriodoDB);
    viewModel.set("TipoDBCombo", datosIniCrearContrato.Datos.TipoDB);
    viewModel.set("DescuentoMonedaCombo", datosIniCrearContrato.Datos.MonedaDescuento);

    viewModel.set("ComercialComboModalPendiente", datosIniCrearContrato.Datos.comercial);
    viewModel.set("MaterialComboModalPendiente", datosIniCrearContrato.Datos.material);
    viewModel.set("TipoComboModalPendiente", datosIniCrearContrato.Datos.tiponegocio);
    viewModel.set("PrecioMonedaComboModalPendiente", datosIniCrearContrato.Datos.moneda);
    viewModel.set("ProvinciaComboModalPendiente", datosIniCrearContrato.Datos.prov);
    viewModel.set("LocalidadComboModalPendiente", datosIniCrearContrato.Datos.loc);
    viewModel.set("SustentableMonedaComboModalPendiente", datosIniCrearContrato.Datos.monedaSustentable);
    viewModel.set("CampanaComboModalPendiente", datosIniCrearContrato.Datos.campaña);
    viewModel.set("ClasificacionComboModalPendiente", datosIniCrearContrato.Datos.Clasificacion);
    viewModel.set("DestinoComboModalPendiente", datosIniCrearContrato.Datos.Destino);
    viewModel.set("BolsaComboModalPendiente", datosIniCrearContrato.Datos.Bolsa);
    viewModel.set("CondicionFijacionComboModalPendiente", datosIniCrearContrato.Datos.Condicion);
    viewModel.set("StandardComboModalPendiente", datosIniCrearContrato.Datos.Standard);

    viewModel.set("isControlDisabled", false);
    
    if ($("#tipoId").data("kendoDropDownList")) $("#tipoId").data("kendoDropDownList").value("2");
    if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
    if ($("#comercialId").data("kendoDropDownList")) $("#comercialId").data("kendoDropDownList").value(comercialId);
    if ($("#material").data("kendoDropDownList")) $("#material").data("kendoDropDownList").value("3");
    if ($("#campanaId").data("kendoDropDownList")) CargarCampaniaPorMaterial("3");
    if ($("#destinoId").data("kendoDropDownList")) $("#destinoId").data("kendoDropDownList").value("1");
    if ($("#descuentoMonedaId").data("kendoDropDownList")) $("#descuentoMonedaId").data("kendoDropDownList").value("1")
    var materialId = $('select[id="material"]').val();
    var calidadGrano = MSExecuteOnServer('/CompraNet/TraerCalidadesPorMaterial', { MaterialId: materialId });
    viewModel.set("EspecialesCombo", calidadGrano);
}

function LimpiarValidaciones() {
    $("#errproveedorId").css("display", "none");
    $("#errcomercialId").css("display", "none");
    $("#errmaterial").css("display", "none");
    $("#errcantidadId").css("display", "none");
    $("#errtipoId").css("display", "none");
    $("#errprecioId").css("display", "none");
    $("#errprecioMonedaId").css("display", "none");
    $("#errcampanaId").css("display", "none");
    $("#errfechaDesdeId").css("display", "none");
    $("#errfechaHastaId").css("display", "none");
    $("#errfechaEntregaId").css("display", "none");
    $("#errprovinciaId").css("display", "none");
    $("#errLocalidadId").css("display", "none");
    $("#errBaseId").css("display", "none");
    $("#errsustentableId").css("display", "none");
    $("#errsustentablePrecioId").css("display", "none");
    $("#errsustentableMonedaId").css("display", "none");
    $("#errdolarizadoId").css("display", "none");
    $("#errdolarizadoFechaId").css("display", "none");
    $("#errpesificadoId").css("display", "none");
    $("#errpesificadoDiasId").css("display", "none");
    $("#errnoInformaSioId").css("display", "none");
    $("#errtrigoEspecialId").css("display", "none");
    $("#errobservacionId").css("display", "none");
    $("#errclasificacion").css("display", "none");
    $("#errdestinoId").css("display", "none");
    $("#errBoletoConfirmaId").css("display", "none");
    $("#errBolsaConfirmaId").css("display", "none");
    $("#errBoletoFisicoId").css("display", "none");
    $("#errbolsaFisicoId").css("display", "none");
    $("#errBoletoNingunoId").css("display", "none");
    $("#errEstablecimientoPropioId").css("display", "none");
    $("#errcantidadCamionesId").css("display", "none");
    $("#errfechaDesdeTopeId").css("display", "none");
    $("#errfechaHastaTopeId").css("display", "none");
    $("#errcondicionFijacionId").css("display", "none");
    $("#errplanCanjeId").css("display", "none");
    $("#errConsignatarioId").css("display", "none");
    $("#errCDId").css("display", "none");
    $("#errWarrantId").css("display", "none");
    $("#errpagoDirectoId").css("display", "none");
    $("#errstandardCalidadId").css("display", "none");
    $("#errcalidadesEspecialesId").css("display", "none");
    $("#errvalorEspecialesId").css("display", "none");

    $("#errtipoPeriodoDBId").css("display", "none");
    $("#errTipoDBId").css("display", "none");
    $("#errfechaDesdeDescuentoId").css("display", "none");
    $("#errfechaHastaDescuentoId").css("display", "none");
    $("#errImporteDescuentoId").css("display", "none");
    $("#errdescuentoMonedaId").css("display", "none");
    $("#errPorcentajeDescuentoId").css("display", "none");
}

function ObtenerDatos() {
    var obj = {};

    var proveedorId;  
    obj.ContratoId = contratoId == undefined ? 0 : contratoId;
    obj.FijacionDePrecioContratoId = fijacionId == undefined ? 0 : fijacionId;
    obj.MaterialId = $("#material").val();
    obj.TipoNegocioId = $("#tipoId").val();
    obj.Cantidad = $("#cantidadId").val();
    obj.Precio = $("#precioId").val();
    obj.FechaEntrega = $("#fechaHastaId").val();
    obj.CampanaId = $("#campanaId").val();
    obj.FechaDesde = $("#fechaDesdeId").val();
    obj.FechaHasta = $("#fechaHastaId").val();
    obj.MonedaId = $("#precioMonedaId").val();
    obj.Fecha = $("#fechaOperacionId").val();
    obj.ComercialId = $("#comercialId").val();
    obj.ProvinciaId = $("#provinciaId").val();
    obj.LocalidadId = $("#LocalidadId").val();
    obj.Base = $("#baseId").is(":checked") ? true : false;
    obj.ImporteSustentable = $("#sustentablePrecioId").val();
    obj.MonedaSustentableId = $("#sustentableMonedaId").val();
    obj.FechaDolarizado = $("#dolarizadoFechaId").val();
    obj.DiasPesificado = $("#pesificadoDiasId").val();
    obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.TrigoEspecial = $("#trigoEspecialId").is(":checked") ? true : false;
    obj.EstadoId = $("#baseId").is(":checked") ? "3" : "1";
    obj.Observacion = $("#observacionId").val();
    obj.ClasificacionId = $("#clasificacion").val();
    obj.CantidadCamiones = $("#cantidadCamionesId").val();
    obj.EstablecimientoPropio = ($("#establecimientoPropioId").is(":checked")) ? true : ($("#establecimientoArrendadoId").is(":checked")) ? false : null; 
    obj.DesdeFijacion = $("#fechaDesdeTopeId").val();
    obj.HastaFijacion = $("#fechaHastaTopeId").val();
    obj.CondicionFijacionId = $("#condicionFijacionId").val();
    obj.DestinoId = $("#destinoId").val();
    obj.planCanje = $("#planCanjeId").is(":checked") ? true : false;
    obj.consignatario = $("#consignatarioId").is(":checked") ? true : false;
    obj.CD = $("#CDId").is(":checked") ? true : false;
    obj.Warrant = $("#WarrantId").is(":checked") ? true : false;
    obj.PagoDirectoVendedor = $("#pagoDirectoId").is(":checked") ? true : false;
    
    if ($("#boletoConfirmaId").is(':checked')) {
        obj.BoletoId = 1;
        obj.BolsaId = $("#bolsaConfirmaId").val();
    }
    else if ($("#boletoFisicoId").is(':checked')) {
        obj.BoletoId = 2;
        obj.BolsaId = $("#bolsaFisicoId").val();
    }
    else if ($("#boletoNingunoId").is(':checked')) {
        obj.BoletoId = 3;
        obj.BolsaId = 0;
    }

    if ($("#buscadorProveedor").val() != "") {
        cuitAux = $("#buscadorProveedor").val().split('(');
        cuit = cuitAux[1].split(')');

        proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0] });
    }

    obj.ProveedorId = proveedorId;

    obj.Descuentos = viewModel.Descuentos;
    obj.Calidad = viewModel.Calidades;
    GrabarContrato(obj);
}

function GrabarContrato(nuevoContrato) {
    var result;

    if (nuevoContrato.TipoNegocioId != 3) {
        result = MSExecuteOnServer('/CompraNet/GrabarContrato', nuevoContrato);
    }
    else {
        result = MSExecuteOnServer('/CompraNet/GrabarFijacion', nuevoContrato);
    }

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            window.location.href = window.location.origin + "/CompraNet";
        }
    }
}

function armarBusquedaResultProveedor() {
    if ($("#buscadorProveedor").val().length >= 3) {
        $("#buscadorResult").empty();

        //aca tiene que ir a buscar
        var txt = $("#buscadorProveedor").val().toUpperCase();

        var result = MSExecuteOnServer('/Proveedor/BuscarProveedores', { filtro: txt });

        var html = "";
        for (var i = 0; i < result.length; i++) {
            var valor = "";

            valor = result[i].RazonSocial + ' (' + result[i].Cuit + ')';

            valor = valor.toUpperCase().split(txt).join("<strong>" + txt + "</strong>");

            var url = MSGetUrl("/Content/Images/usuario-busqueda.png");

            html += '<div class="buscar-result-linea" onclick="seleccionarProveedor(this)" >'
                + '<img class="buscar-cont" src="..' + url + '" /> '
                + '<p class="buscar-nomb">' + valor + '</p>'
                + '</div>';
        }

        if (!result.length) {
            html += '<div class="buscar-result-linea">'
                + '<p class="buscar-nomb">No se encontraron resultados</p>'
                + '</div>';
        }

        $("#buscadorResult").append(html);
        console.log($("#buscadorProveedor").is(":focus"));
        $("#buscadorResult").show();
    } else {
        $("#buscadorResult").empty();
        $("#buscadorResult").hide();
    }
}

function seleccionarProveedor(opciones) {
    $("#buscadorProveedor").val(opciones.innerText);
    $("#buscadorResult").hide();

    obtenerLocalidadProvincia();
}

function obtenerLocalidadProvincia() {
    if ($("#buscadorProveedor").val() != "") {
        let cuitProvAux = $("#buscadorProveedor").val().split('(');
        let cuitProv = cuitProvAux[1].split(')');

        if (cuitProv[0] != null && $("#material").val() != "" && $("#campanaId").val() != "") {
            let localidadProvincia = MSExecuteOnServer('/CompraNet/ObtenerProvinciaLocalidadProv', {
                CUIT: cuitProv[0],
                MaterialId: $("#material").val(),
                CampanaId: $("#campanaId").val()
            });
            limpiarBoleto();
            if (localidadProvincia != null) {
                if (localidadProvincia.LocalidadId != "") {
                    if (localidadProvincia.CUIT != "") {
                        $("#provinciaId").data("kendoDropDownList").value(localidadProvincia.ProvinciaId);
                        $("#provinciaId").data("kendoDropDownList").trigger("change");
                        $("#LocalidadId").data("kendoDropDownList").value(localidadProvincia.LocalidadId);
                    } else {
                        $("#provinciaId").data("kendoDropDownList").value("");
                        $("#provinciaId").data("kendoDropDownList").trigger("change");
                        $("#LocalidadId").data("kendoDropDownList").value("");
                    }
                }
                $("#clasificacion").data("kendoDropDownList").value(localidadProvincia.ClasificacionId);
                $("#clasificacion").data("kendoDropDownList").trigger("change");
                if (localidadProvincia.BoletoId == 1) {
                    $("#boletoConfirmaId").prop("checked", true);
                    $("#BolsaConfirmaDiv").show();
                    $("#bolsaConfirmaId").data("kendoDropDownList").value(localidadProvincia.BolsaId);
                    $("#bolsaConfirmaId").data("kendoDropDownList").trigger("change");
                } else if (localidadProvincia.BoletoId == 2) {
                    $("#boletoFisicoId").prop("checked", true);
                    $("#BolsaFisicoDiv").show();
                    $("#bolsaFisicoId").data("kendoDropDownList").value(localidadProvincia.BolsaId);
                    $("#bolsaFisicoId").data("kendoDropDownList").trigger("change");
                } else {
                    $("#boletoNingunoId").prop("checked", true);
                }
            }
        }
    }
}

function limpiarBoleto() {
    $("#boletoConfirmaId").prop("checked", false);
    $("#BolsaConfirmaDiv").hide();
    $("#bolsaConfirmaId").data("kendoDropDownList").value("");
    $("#boletoFisicoId").prop("checked", false);
    $("#BolsaFisicoDiv").hide();
    $("#bolsaFisicoId").data("kendoDropDownList").value("");
    $("#boletoNingunoId").prop("checked", false);
}

function AgregarDescuentos() {
    var descuento = {
        Id: 0,
        TipoPeriodoDBDesc: $("#tipoPeriodoDBId").data("kendoDropDownList").text(),
        TipoPeriodoDBId: $("#tipoPeriodoDBId").data("kendoDropDownList").value(),
        TipoDBDesc: $("#TipoDBId").data("kendoDropDownList").text(),
        TipoDBId: $("#TipoDBId").data("kendoDropDownList").value(),
        FechaDesde: $("#fechaDesdeDescuentoId").val(),
        FechaHasta: $("#fechaHastaDescuentoId").val(),
        Importe: $("#ImporteDescuentoId").val(),
        MonedaId: $("#descuentoMonedaId").data("kendoDropDownList").text(),
        Porcentaje: $("#PorcentajeDescuentoId").val(),
        Borrar: function () {
            viewModel.Descuentos.remove(this);
        }
    };
    
    var err = validarDescuento(descuento)
    if (ExistsErrorMessages(err)) {
        MensErr(err[0]);
    }
    else {
        viewModel.Descuentos.push(descuento);
        $("#ImporteDescuentoId").val("");
        $("#PorcentajeDescuentoId").val(1);
    }
}

function validarDescuento(descuento) {
    var errores = [];

    if (descuento.TipoPeriodoDBId === 0 || descuento.TipoPeriodoDBId === "" || descuento.TipoPeriodoDBId === null) {
        errores.push("El campo Descuento no puede estar vacíos");
    }
    if (descuento.TipoDBId === 0 || descuento.TipoDBId === null || descuento.TipoDBId === "") {
        errores.push("El campo Tipo no puede estar vacíos");
    }
    if (descuento.TipoPeriodoDBId != 1 && (descuento.FechaDesde == "" || descuento.FechaDesde == "Undefined" || descuento.FechaHasta == "" || descuento.FechaHasta == "Undefined")) {
        errores.push("La fecha no puede estar vacía");
    }
    var fechaD = kendo.parseDate(descuento.FechaDesde, "dd-MM-yyyy");
    var fechaH = kendo.parseDate(descuento.FechaHasta, "dd-MM-yyyy");
    if (descuento.TipoPeriodoDBId != 1 && (!fechaD || !fechaH)) {
        errores.push("La fecha no es válida");
    }
    if ((descuento.Importe === "" || descuento.Importe === null)
        && (descuento.Porcentaje === "" || descuento.Porcentaje === null || descuento.Porcentaje === "undefined")) {
        errores.push("El campo Importe y Porcentaje no pueden estar vacíos");
    }
    if (descuento.MonedaId === "Moneda" || descuento.MonedaId === null || descuento.MonedaId === "Undefined") {
        errores.push("El campo Moneda no puede estar vacío");
    }
    return errores
}

function AgregarCalidades() {
    var calidades = {
        Id: 0,
        CalidadEspecialDesc: $("#calidadesEspecialesId").data("kendoDropDownList").text(),
        CalidadEspecialId: $("#calidadesEspecialesId").data("kendoDropDownList").value(),        
        Valor: $("#valorEspecialesId").val(), 
        StandardDeCalidadId: 2,
        Borrar: function () {
            viewModel.Calidades.remove(this);
        }
    };

    var err = validarCalidad(calidades)
    if (ExistsErrorMessages(err)) {
        MensErr(err[0]);
    }
    else {
        viewModel.Calidades.push(calidades);
        $("#calidadesEspecialesId").val("");
        $("#valorEspecialesId").data("kendoNumericTextBox").value("");
    }
}

function validarCalidad(calidad) {
    var errores = [];

    if (calidad.CalidadEspecialId === 0 || calidad.CalidadEspecialId === "" || calidad.CalidadEspecialId === null) {
        errores.push("El campo Calidades Especiales no puede estar vacio")
    }
    if (calidad.Valor === "" || calidad.Valor === null || calidad.Valor === "undefined") {
        errores.push("El campo Valor no puede estar vacio")
    }
    return errores
}

function InicializarContratoEdit() {
    var datos = { id: contratoId }
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerContratoCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
    $("#operacionDiv").show();
}

function InicializarFijacionEdit() {
    var datos = { id: fijacionId }
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerFijacionCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
}

function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
};

function CargarDatosEditar(contrato) {
    $("#buscadorProveedor").val(contrato.Proveedor);
    $("#fechaDesdeId").val(formatearFecha(contrato.FechaDesdeFormateado));
    $("#fechaHastaId").val(formatearFecha(contrato.FechaHastaFormateado));
    $("#fechaOperacionId").val(formatearFecha(contrato.FechaFormateado));

    $("#tipoId").data("kendoDropDownList").value(contrato.TipoNegocioId);
    $("#tipoId").data("kendoDropDownList").trigger("change");

    $("#material").data("kendoDropDownList").value(contrato.MaterialId);
    $("#material").data("kendoDropDownList").trigger("change");

    $("#observacionId").val(contrato.Observacion);
    $("#cantidadId").data("kendoNumericTextBox").value(contrato.Cantidad);
    $("#precioId").data("kendoNumericTextBox").value(contrato.Precio);

    $("#precioMonedaId").data("kendoDropDownList").value(contrato.MonedaId);
    $("#precioMonedaId").data("kendoDropDownList").trigger("change");

    $("#provinciaId").data("kendoDropDownList").value(contrato.ProvinciaId);
    $("#provinciaId").data("kendoDropDownList").trigger("change");

    $("#clasificacion").data("kendoDropDownList").value(contrato.ClasificacionId);
    $("#clasificacion").data("kendoDropDownList").trigger("change");

    $("#destinoId").data("kendoDropDownList").value(contrato.DestinoId);
    $("#destinoId").data("kendoDropDownList").trigger("change");
    if (contrato.CantidadCamiones !== "null" && contrato.CantidadCamiones !== undefined && contrato.CantidadCamiones !== 0) {
        $("#cantidadCamionesId").data("kendoNumericTextBox").value(contrato.CantidadCamiones);
    }

    (contrato.PlanCanje == true) ? $("#planCanjeId").prop("checked", true) : $("#planCanjeId").prop("checked", false);
    (contrato.Consignatario == true) ? $("#consignatarioId").prop("checked", true) : $("#consignatarioId").prop("checked", false);
    if (contrato.LocalidadId !== "null" && contrato.LocalidadId !== "undefined") $("#LocalidadId").data("kendoDropDownList").value(contrato.LocalidadId);
    if (contrato.ComercialId !== "null" && contrato.ComercialId !== "undefined") $("#comercialId").data("kendoDropDownList").value(contrato.ComercialId);

    $("#campanaId").data("kendoDropDownList").value(contrato.CampanaId);

    if (contrato.Base == true) {
        $("#baseId").prop("checked", true);
    } else {
        $("#baseId").prop("checked", false);
    }

    if (contrato.Importe_Sustentable !== null && contrato.Importe_Sustentable !== undefined && contrato.Importe_Sustentable !== 0) {
        $("#sustentablePrecioId").data("kendoNumericTextBox").value(contrato.Importe_Sustentable);
        $("#sustentableMonedaId").data("kendoDropDownList").value(contrato.Moneda_Sustentable);
        $("#sustentableId").prop("checked", true);
        $("#sustentableDiv").show();
    }

    if (contrato.Fecha_DolarizadoFormateado !== null && contrato.Fecha_DolarizadoFormateado !== undefined && contrato.Fecha_DolarizadoFormateado !== "") {
        $("#dolarizadoId").prop("checked", true);
        $("#dolarizadoDiv").show();
        $("#dolarizadoFechaId").val(contrato.Fecha_DolarizadoFormateado);
    }

    if (contrato.Dias_Pesificado !== null && contrato.Dias_Pesificado !== undefined && contrato.Dias_Pesificado !== "") {
        $("#pesificadoId").prop("checked", true);
        $("#pesificadoDiv").show();
        $("#pesificadoDiasId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    }

    if (contrato.NoInformaSIO == true) {
        $("#noInformaSioId").prop("checked", true);
    } else {
        $("#noInformaSioId").prop("checked", false);
    }

    if (contrato.TrigoEspecial == true) {
        $("#trigoEspecialId").prop("checked", true);
    } else {
        $("#trigoEspecialId").prop("checked", false);
    }
    if (contrato.BoletoId == 1) {
        $("#boletoConfirmaId").prop("checked", true);
        $("#BolsaConfirmaDiv").show();
        $("#bolsaConfirmaId").data("kendoDropDownList").value(contrato.BolsaId);
    } else if (contrato.BoletoId == 2) {
        $("#boletoFisicoId").prop("checked", true);
        $("#BolsaFisicoDiv").show();
        $("#bolsaFisicoId").data("kendoDropDownList").value(contrato.BolsaId);
    } else if (contrato.BoletoId == 3) {
        $("#boletoNingunoId").prop("checked", true);
    }

    if (!(contrato.DesdeFijacionFormateado == null && contrato.DesdeFijacionFormateado == undefined && contrato.DesdeFijacionFormateado == "")) {
        $("#fechaDesdeTopeId").val(contrato.DesdeFijacionFormateado);
    } else {
        $("#fechaDesdeTopeId").val("");
    }
    if (!(contrato.HastaFijacionFormateado == "null" && contrato.HastaFijacionFormateado == undefined && contrato.HastaFijacionFormateado == "")) {
        $("#fechaHastaTopeId").val(contrato.HastaFijacionFormateado);
    } else {
        $("#fechaDesdeTopeId").val("");
    }
    $("#condicionFijacionId").data("kendoDropDownList").value(contrato.CondicionFijacion);
    contrato.CD == true ? $("#CDId").prop("checked", true) : $("#CDId").prop("checked", false);
    contrato.Warrant == true ? $("#WarrantId").prop("checked", true) : $("#WarrantId").prop("checked", false);
    contrato.PagoDirectoVendedor == true ? $("#pagoDirectoId").prop("checked", true) : $("#pagoDirectoId").prop("checked", false);

    contrato.EstablecimientoPropio == true ? $("#establecimientoPropioId").prop("checked", true) : contrato.EstablecimientoPropio == false ? $("#establecimientoArrendadoId").prop("checked", true) : false;

    var iteracionesDescuentos = viewModel.Descuentos.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.Descuentos.pop();
    }
    var iteracionesCalidades = viewModel.Calidades.length;
    for (var i = 0; i < iteracionesCalidades; i++) {
        viewModel.Calidades.pop();
    }

    var descuentosDto = contrato.Descuentos;
    
    $.each(descuentosDto, function (key, descuento) {
        var descuentoKendo = {
            Id: descuento.Id,
            TipoPeriodoDBDesc: descuento.TipoPeriodoDBDesc,
            TipoPeriodoDBId: descuento.TipoPeriodoDBId,
            TipoDBDesc: descuento.TipoDBDesc,
            TipoDBId: descuento.TipoDBId,
            FechaDesde: descuento.FechaDesde,
            FechaHasta: descuento.FechaHasta,
            Importe: descuento.Importe,
            MonedaId: descuento.MonedaId,
            Porcentaje: descuento.Porcentaje,
            ContratoId: descuento.ContratoId,
            Borrar: function () {
                viewModel.Descuentos.remove(this);
            }
        };
        viewModel.Descuentos.push(descuentoKendo);
    });
    var calidadesDto = contrato.Calidades;
    $.each(calidadesDto, function (key, calidad) {
        var calidadKendo = {
            Id: calidad.Id,
            CalidadEspecialId: calidad.CalidadEspecialId,
            CalidadEspecialDesc: calidad.CalidadEspecialDesc,
            Valor: calidad.Valor,
            ContratoId: calidad.ContratoId,
            StandardDeCalidadId: 2,
            Borrar: function () {
                viewModel.Calidades.remove(this);
            }
        };
        viewModel.Calidades.push(calidadKendo);
    });
}