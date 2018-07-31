var viewModel;
var datosIniCrearContrato;

$(document).ready(function () {

    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    CrearViewModel();

    InicializarElementos();
    InicializarDatos();
});


function InicializarElementos() {

    kendo.culture("es-AR");

    $(".datos-adicionales").hide();
    $(".datos-boleto").hide();
    $(".datos-establecimiento").hide();
    $(".datos-topesplazos").hide();
    $(".datos-pago").hide();
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

    $("#buscadorProveedorModalPendiente").click(function () {
        $("#buscadorProveedorModalPendiente").val("");
    });

    $("#buscadorProveedorModalPendiente").keyup(function (e) {
        armarBusquedaResultProveedorModalPendiente(e);
    });

    $("#buscadorProveedorModalPendiente").focus(function (e) {
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

    $("#tipoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId"
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

    $("#provinciaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA PROVINCIA...",
        dataTextField: "Nombre",
        dataValueField: "Provinciaid",
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

    $("#clasificacion").kendoDropDownList({
        optionLabel: "SELECCIONE LA CLASIFICACIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#clasificacion").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacion").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#clasificacionModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CLASIFICACIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#clasificacionModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacionModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#destinoid").kendoDropDownList({
        optionLabel: "SELECCIONE EL DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#destinoid").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinoId").data("kendoDropDownList");
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
    $("#bolsaConfirmaModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#bolsaConfirmaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaConfirmaModalPendienteId").data("kendoDropDownList");
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
    $("#bolsaFisicoModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#bolsaFisicoModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaFisicoModalPendienteId").data("kendoDropDownList");
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
    $("#condicionFijacionModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE CONDICIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#condicionFijacionModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#condicionFijacionModalPendienteId").data("kendoDropDownList");
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
    $("#standardCalidadModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE STANDARD DE CALIDAD...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#standardCalidadModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#standardCalidadModalPendienteId").data("kendoDropDownList");
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
    $("#calidadesEspecialesModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE CALIDAD ESPECIAL...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#calidadesEspecialesModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#calidadesEspecialesModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#cantidadId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
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

    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }

    var mesPost = hoy.getMonth() + 2;
    if (mesPost < 10) {
        mesPost = "0" + mesPost.toString();
    }

    if (dia < 10) {
        dia = "0" + dia.toString();
    }

    var date = dia + '-' + mes + '-' + anio; //new Date(anio, mes, dia);
    var datehasta = dia + '-' + mesPost + '-' + anio; //new Date(anio, mesPost, dia);

    $("#fechaDesdeId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHastaId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
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

    $(".formulario-footer-guardar-contrato").click(function () {
        ObtenerDatos();
    });

    $(".formulario-footer-cancelar").click(function () {
        window.location.href = window.location.origin + "/CompraNet";
    });

    $("#DatosAdicionales").click(function () {
        if (document.querySelector(".datos-adicionales").style.display == "none"){
            document.querySelector(".datos-adicionales").style.display = "block";
        } else {
            document.querySelector(".datos-adicionales").style.display = "none";           
        }
    });

    $("#DatosTopesPlazos").click(function () {
        if (document.querySelector(".datos-topesplazos").style.display == "none") {
            document.querySelector(".datos-topesplazos").style.display = "block";            
        } else {
            document.querySelector(".datos-topesplazos").style.display = "none";
        }
    });

    $("#DatosBoleto").click(function () {
        if (document.querySelector(".datos-boleto").style.display == "none") {
            document.querySelector(".datos-boleto").style.display = "block";
        } else {
            document.querySelector(".datos-boleto").style.display = "none";
        }
    });
    
    $("#DatosPago").click(function () {
        if (document.querySelector(".datos-pago").style.display == "none") {
            document.querySelector(".datos-pago").style.display = "block";
        } else {
            document.querySelector(".datos-pago").style.display = "none";
        }
    });

    $("#DatosDescuentos").click(function () {
        if (document.querySelector(".datos-descuentos").style.display == "none") {
            document.querySelector(".datos-descuentos").style.display = "block";
        } else {
            document.querySelector(".datos-descuentos").style.display = "none";
        }
    });

    $("#DatosCalidades").click(function () {
        if (document.querySelector(".datos-calidades").style.display == "none") {
            document.querySelector(".datos-calidades").style.display = "block";
        } else {
            document.querySelector(".datos-calidades").style.display = "none";
        }
    });

    $("#DatosEstablecimiento").click(function () {
        if (document.querySelector(".datos-establecimiento").style.display == "none") {
            document.querySelector(".datos-establecimiento").style.display = "block";
        } else {
            document.querySelector(".datos-establecimiento").style.display = "none";
        }
    });

    $("#sustentableId").click(function () {
        if ($(this).is(':checked')) {
            $("#sustentableDiv").show();
        }
        else {
            $("#sustentableDiv").hide();
            $("#sustentablePrecioId").val("");
        }
    });

    $("#tipoId").on("change", function () {

        if ($("#tipoId").val() == 3) {
            $("#fechasDiv").hide();
            $("#fechaDesdeDiv").hide();
            $("#fechaHastaDiv").hide();
            $("#campanaDiv").hide();
            $("#procedenciaDiv").hide();
            $("#clasificacionDiv").hide();
            $("#destinoDiv").hide();
            $("#CantidadCamionesDiv").hide();
            $("#planCanjeConsignatarioIdDiv").hide();
            $("#DatosBoleto").hide();
            $("#DatosTopesPlazos").hide();
            $("#DatosPago").hide();
            $("#DatosEstablecimiento").hide();
            $("#baseDiv").hide();
            $("#DatosAdicionales").hide();
            $(".datos-adicionales").hide();
            $("#DatosCalidades").hide();
            $(".datos-calidades").hide();
            $(".datos-boleto").hide();
            $(".datos-topesplazos").hide();
            $(".datos-pago").hide();
            $(".datos-establecimiento").hide();
            $("#ContratoDiv").show();
            $("#guardarBtn").empty();
            $("#DatosBoleto").hide();
            $("#DatosPago").hide();
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
            $("#DatosTopesPlazos").show();
            $("#DatosPago").show();
            $("#DatosEstablecimiento").show();
            $("#DatosCalidades").show();
            $("#baseDiv").show();
            $("#DatosAdicionales").show();
            $("#ContratoDiv").hide();
            $("#guardarBtn").empty();
            $("#DatosBoleto").show();
            $("#guardarBtn").append("Generar Negocio");
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
            $("#pesificadoDiasId").val("");
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

    $('select[id="material"]').change(function () {
        if ($(this).val() != "") {
            CargarCampaniaPorMaterial($(this).val());
            CargarCalidadPorMaterial($(this).val())
        }
    });
    
    $('select[id="provinciaId"]').change(function () {
        if ($(this).val() != "") CargarLocalidadPorProvincia($(this).val());
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
        optionLabel: "DESCUENTOS",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#tipoPeriodoDBId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoPeriodoDBId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#TipoDBId").kendoDropDownList({
        optionLabel: "TIPO",
        dataTextField: "Descripcion",
        dataValueField: "Id"
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
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHastaDescuentoId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#ImporteDescuentoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: null,
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
    $("#PorcentajeDescuentoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

  $('select[id="tipoPeriodoDBId"]').change(function () {
        if ($(this).val() == 1) {
            $(".fecha-descuento").hide();
            $("#fechaHastaDescuentoId").val("");
            $("#fechaDesdeDescuentoId").val("");
        }
        else {
            $(".fecha-descuento").show();
            $("#fechaDesdeDescuentoId").val(date)
            $("#fechaHastaDescuentoId").val(datehasta)
        }
    });
    $("#IngresarDescuento").click(AgregarDescuentos);
}

function CargarCampaniaPorMaterial(value) {

    var resultGrano = MSExecuteOnServer('/CompraNet/TraerCampanaPorMaterial', { MaterialId: value });

    var campanaActualId = MSExecuteOnServer('/CompraNet/TraerCampanaActualMaterial', { MaterialId: value });

    viewModel.set("CampanaCombo", resultGrano);


    $("#campanaId").data("kendoDropDownList").value(campanaActualId);
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
            "pesificadoDiasId":null,
            "noInformaSioId": null,
            "trigoEspecialId": null,
            "statusId": null,
            "observacionId": null,
            "clasificacionId": null,
            "cantidadCamionesId": null,
            "condicionFijacionId":null,
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
            "condicionFijacionIdModal":null,
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

            ProveedorCombo: [],
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
            TipoPeriodoDBCombo:[],
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
            CondicionFijacionComboModalPendiente:[],
            Destino: [],
            StandardComboModalPendiente: [],
            EspecialesComboModalPendiente:[],
            isControlDisabled: true,
            Descuentos: []
        });

        kendo.bind($("#CrearContrato"), viewModel);
        kendo.bind($("#CompraNet"), viewModel);
        kendo.bind($("#modalPendienteDiv"), viewModel);
        kendo.bind("#tabla-descuentos", viewModel);
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
    }

    MSExecuteURLOnServerAsync('/CompraNet/InicializarContrato', funcReturn, '');
}

function AsignarDatos()
{
    viewModel.set("ProveedorCombo", datosIniCrearContrato.Datos.proveedor);
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
    viewModel.set("TipoDBCombo", datosIniCrearContrato.Datos.TipoDB );
    viewModel.set("DescuentoMonedaCombo", datosIniCrearContrato.Datos.MonedaDescuento );

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
    viewModel.set("BolsaComboComboModalPendiente", datosIniCrearContrato.Datos.Bolsa);
    viewModel.set("CondicionFijacionComboModalPendiente", datosIniCrearContrato.Datos.Condicion);
    viewModel.set("StandardComboModalPendiente", datosIniCrearContrato.Datos.Standard);

    viewModel.set("isControlDisabled", false);

    var comercialId = MSExecuteOnServer('/CompraNet/ObtenerComercialId');
    if ($("#tipoId").data("kendoDropDownList")) $("#tipoId").data("kendoDropDownList").value("2");
    if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
    if ($("#comercialId").data("kendoDropDownList")) $("#comercialId").data("kendoDropDownList").value(comercialId);
    if ($("#material").data("kendoDropDownList")) $("#material").data("kendoDropDownList").value("3");
    if ($("#campanaId").data("kendoDropDownList")) CargarCampaniaPorMaterial("3");
    if ($("#destinoid").data("kendoDropDownList")) $("#destinoid").data("kendoDropDownList").value("1");
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

    var fecha = new Date();
    var fechaHoy = new Date(
        fecha.getFullYear(),
        fecha.getMonth(),
        fecha.getDate(),
        fecha.getHours(),
        fecha.getMinutes(),
        fecha.getSeconds()
        );
    var proveedorId;

    obj.MaterialId = $("#material").val();
    obj.TipoNegocioId = $("#tipoId").val();
    obj.Cantidad =$("#cantidadId").val();
    obj.Precio = $("#precioId").val();
    obj.FechaEntrega = $("#fechaHastaId").val();
    obj.CampanaId = $("#campanaId").val();
    obj.FechaDesde = $("#fechaDesdeId").val();
    obj.FechaHasta = $("#fechaHastaId").val();
    obj.MonedaId = $("#precioMonedaId").val();
    obj.Fecha = fechaHoy;
    obj.ComercialId = $("#comercialId").val();
    obj.ProvinciaId = $("#provinciaId").val();
    obj.LocalidadId = $("#LocalidadId").val();
    obj.Base = $("#baseId").is(":checked") ? true : false;
    obj.ImporteSustentable = $("#sustentablePrecioId").val();
    obj.MonedaIdSustentable = $("#sustentableMonedaId").val();
    obj.FechaDolarizado = $("#dolarizadoFechaId").val();
    obj.DiasPesificado = $("#pesificadoDiasId").val();
    obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.TrigoEspecial = $("#trigoEspecialId").is(":checked") ? true : false;
    obj.Estado = $("#baseId").is(":checked") ? "3" : "1";
    obj.ContratoId = $("#contratoId").val();
    obj.Observacion = $("#observacionId").val();
    obj.ClasificacionId = $("#clasificacion").val();
    obj.CantidadCamiones = $("#cantidadCamionesId").val();
    obj.EstablecimientoPropio = ($("input[name='establecimiento']:checked").val() == "Propio") ? true : ($("input[name='establecimiento']:checked").val() == "Arrendado") ? false : null;
    obj.DesdeFijacion = $("#fechaDesdeTopeId").val();
    obj.HastaFijacion = $("#fechaHastaTopeId").val();
    obj.CondicionFijacionId = $("#condicionFijacionId").val();
    obj.DestinoId = $("#destinoId").val();
    obj.planCanje = $("#planCanjeId").is(":checked") ? true : false;
    obj.consignatario = $("#consignatarioId").is(":checked") ? true : false;
    obj.CD = $("#CDId").is(":checked") ? true : false;
    obj.Warrant = $("#WarrantId").is(":checked") ? true : false;
    obj.PagoDirectoVendedor = $("#pagoDirectoId").is(":checked") ? true : false;
    obj.StandardDeCalidadId = $("#standardCalidadId").val();
    obj.CalidadEspecialId = $("#calidadesEspecialesId").val();
    obj.ValorCalidadEspecial = $("#valorEspecialesId").val();

    if ($("#boletoConfirmaId").is(':checked'))
    {
        obj.BoletoId = 1;
        obj.BolsaId = $("#bolsaConfirmaId").val();
    }
    else if ($("#boletoFisicoId").is(':checked'))
    {
        obj.BoletoId = 2;
        obj.BolsaId = $("#bolsaFisicoId").val();
    }
    else
    {
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
            MensInfo("Se ha realizado la operacion con exito");
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

function armarBusquedaResultProveedorModalPendiente() {
if ($("#buscadorProveedorModalPendiente").val().length >= 3) {
    $("#buscadorResultModalPendiente").empty();

            //aca tiene que ir a buscar
             var txt = $("#buscadorProveedorModalPendiente").val().toUpperCase();


            var result = MSExecuteOnServer('/Proveedor/BuscarProveedores', { filtro: txt });


            var html = "";
            for (var i = 0; i < result.length; i++) {

                var valor = "";


                valor = result[i].RazonSocial + ' (' + result[i].Cuit + ')';
        
                valor = valor.toUpperCase().split(txt).join("<strong>" + txt + "</strong>");

                var url = MSGetUrl("/Content/Images/usuario-busqueda.png");
        
                html += '<div class="buscar-result-linea" onclick="seleccionarProveedorModalPendiente(this)" >'
                      + '<img class="buscar-cont" src="..' + url + '" /> '
                      + '<p class="buscar-nomb">' + valor + '</p>'
                      + '</div>';
            }

            if (!result.length) {
                html += '<div class="buscar-result-linea">'
                      + '<p class="buscar-nomb">No se encontraron resultados</p>'
                      + '</div>';
            }

            $("#buscadorResultModalPendiente").append(html);
            console.log($("#buscadorProveedorModalPendiente").is(":focus"));
            $("#buscadorResultModalPendiente").show();
        } else {
            $("#buscadorResultModalPendiente").empty();
            $("#buscadorResultModalPendiente").hide();
        }
}

function seleccionarProveedor(opciones) {
    $("#buscadorProveedor").val(opciones.innerText);
    $("#buscadorResult").hide();

    obtenerLocalidadProvincia();        
}

function seleccionarProveedorModalPendiente(opciones) {
    $("#buscadorProveedorModalPendiente").val(opciones.innerText);
    $("#buscadorResultModalPendiente").hide();

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
            if (localidadProvincia.LocalidadId != "") {
                if (localidadProvincia.CUIT != "") {
                    $("#provinciaId").data("kendoDropDownList").value(localidadProvincia.ProvinciaId);
                    CargarLocalidadPorProvincia(localidadProvincia.ProvinciaId);
                    $("#LocalidadId").data("kendoDropDownList").value(localidadProvincia.LocalidadId);
                } else {
                    $("#provinciaId").data("kendoDropDownList").value("");
                    $("#LocalidadId").data("kendoDropDownList").value("");
                }                
            }
            $("#clasificacion").data("kendoDropDownList").value(localidadProvincia.ClasificacionId);
            if (localidadProvincia.BoletoId == 1) {
                $("#boletoConfirmaId").prop("checked", true);
                $("#BolsaConfirmaDiv").show();
                $("#bolsaConfirmaId").data("kendoDropDownList").value(localidadProvincia.BolsaId);
            } else if (localidadProvincia.BoletoId == 2) {
                $("#boletoFisicoId").prop("checked", true);
                $("#BolsaFisicoDiv").show();
                $("#bolsaFisicoId").data("kendoDropDownList").value(localidadProvincia.BolsaId);
            } else {
                $("#boletoNingunoId").prop("checked", true);
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
    }
}

function validarDescuento(descuento) {
    var errores = [];

    if (descuento.TipoPeriodoDBId === 0 || descuento.TipoPeriodoDBId === "" || descuento.TipoPeriodoDBId === null) {
        errores.push("El campo Descuento no puede estar vacio")
    }
    if (descuento.TipoDBId === 0 || descuento.TipoDBId === null || descuento.TipoDBId === "") {
        errores.push("El campo Tipo no puede estar vacio")
    }
    if (descuento.TipoPeriodoDBId != 1 && (descuento.FechaDesde == "" || descuento.FechaDesde == "Undefined" || descuento.FechaHasta == "" || descuento.FechaHasta == "Undefined"  )) {
        errores.push("La fecha no puede estar vacia")        
    }    
    var fechaD = kendo.parseDate(descuento.FechaDesde, "dd-MM-yyyy");
    var fechaH = kendo.parseDate(descuento.FechaHasta, "dd-MM-yyyy");
    if (descuento.TipoPeriodoDBId != 1 && (!fechaD || !fechaH)) {
        errores.push("La fecha no es válida");
    }
    if (descuento.Importe === "" || descuento.Importe === null) {
        errores.push("El campo Importe no puede estar vacio")
    }
    if (descuento.MonedaId === "Moneda" || descuento.MonedaId === null || descuento.MonedaId ==="Undefined") {
        errores.push("El campo Moneda no puede estar vacio")
    }
    if (descuento.Porcentaje === "" || descuento.Porcentaje === null || descuento.Porcentaje === "undefined") {
        errores.push("El campo Porcentaje no puede estar vacio")
    }

    return errores
}
