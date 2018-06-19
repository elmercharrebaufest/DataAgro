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
        //e.stopPropagation();
        //e.preventDefault();
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

    $("#cantidadId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#precioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
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

    $(".btnagregar").click(function () {
        if (document.querySelector(".datos-adicionales").style.display == "none"){
            document.querySelector(".datos-adicionales").style.display = "block";
        } else {
            document.querySelector(".datos-adicionales").style.display = "none";
            $("#sustentableId").prop('checked', false);
            $("#dolarizadoId").prop('checked', false);
            $("#pesificadoId").prop('checked', false);
            $("#noInformaSioId").prop('checked', false);
            $("#trigoEspecialId").prop('checked', false);

            $("#sustentableDiv").hide();
            $("#sustentablePrecioId").val("");

            $("#dolarizadoDiv").hide();
            $("#dolarizadoFechaId").val("");

            $("#pesificadoDiv").hide();
            $("#pesificadoDiasId").val("");
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
            $("#baseDiv").hide();
            $("#DatosAdicionales").hide();
            $(".datos-adicionales").hide();
            $("#ContratoDiv").show();
            $("#guardarBtn").empty();
            $("#guardarBtn").append("Guardar Fijacion");
        }
        else {
            $("#fechasDiv").show();
            $("#fechaDesdeDiv").show();
            $("#fechaHastaDiv").show();
            $("#fechaHastaContratoDiv").show();
            $("#campanaDiv").show();
            $("#procedenciaDiv").show();
            $("#baseDiv").show();
            $("#DatosAdicionales").show();
            $("#ContratoDiv").hide();
            $("#guardarBtn").empty();
            $("#guardarBtn").append("Generar Negocio");
        }
    });

    $(".datos-adicionales").hide()

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

    $('select[id="material"]').change(function () {
        if ($(this).val() != "") CargarCampaniaPorMaterial($(this).val());
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
}

function CargarCampaniaPorMaterial(value) {

    var resultGrano = MSExecuteOnServer('/CompraNet/TraerCampanaPorMaterial', { MaterialId: value });

    var campanaActualId = MSExecuteOnServer('/CompraNet/TraerCampanaActualMaterial', { MaterialId: value });

    viewModel.set("CampanaCombo", resultGrano);


    $("#campanaId").data("kendoDropDownList").value(campanaActualId);
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

            ComercialComboModalPendiente: [],
            MaterialComboModalPendiente: [],
            TipoComboModalPendiente: [],
            PrecioMonedaComboModalPendiente: [],
            CampanaComboModalPendiente: [],
            ProvinciaComboModalPendiente: [],
            LocalidadComboModalPendiente: [],
            SustentableMonedaComboModalPendiente: [],
        
            isControlDisabled: true,

        });

        kendo.bind($("#CrearContrato"), viewModel);
        kendo.bind($("#CompraNet"), viewModel);
        kendo.bind($("#modalPendienteDiv"), viewModel);

    }

    function InicializarDatos() {

        var funcReturn = function (data) {

            if (ExistsErrorMessages(data.Errores)) {
                ShowErrorMessages(data.Errores);
            }
            else {
                datosIniCrearContrato = data;
                AsignarDatos();
                RefrescarWidgets();
            }
        }

        MSExecuteURLOnServerAsync('/CompraNet/InicializarContrato', funcReturn, '');
    }

    function AsignarDatos() {

        /*
        viewModel.set("Parametros", datosIniCrearContrato.Param);
        viewModel.set("Parametros", datosIniCrearContrato.Param);
        */
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

        viewModel.set("ComercialComboModalPendiente", datosIniCrearContrato.Datos.comercial);
        viewModel.set("MaterialComboModalPendiente", datosIniCrearContrato.Datos.material);
        viewModel.set("TipoComboModalPendiente", datosIniCrearContrato.Datos.tiponegocio);
        viewModel.set("PrecioMonedaComboModalPendiente", datosIniCrearContrato.Datos.moneda);
        viewModel.set("ProvinciaComboModalPendiente", datosIniCrearContrato.Datos.prov);
        viewModel.set("LocalidadComboModalPendiente", datosIniCrearContrato.Datos.loc);
        viewModel.set("SustentableMonedaComboModalPendiente", datosIniCrearContrato.Datos.monedaSustentable);
        viewModel.set("CampanaComboModalPendiente", datosIniCrearContrato.Datos.campaña);


        viewModel.set("isControlDisabled", false);

        var comercialId = MSExecuteOnServer('/CompraNet/ObtenerComercialId');
        if ( $("#tipoId").data("kendoDropDownList") ) $("#tipoId").data("kendoDropDownList").value("2");
        if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        if ($("#comercialId").data("kendoDropDownList")) $("#comercialId").data("kendoDropDownList").value(comercialId.Result);
        if ($("#sustentableMonedaId").data("kendoDropDownList")) $("#sustentableMonedaId").data("kendoDropDownList").value("USDM ");
        if ($("#material").data("kendoDropDownList")) $("#material").data("kendoDropDownList").value("3");
        if ($("#campanaId").data("kendoDropDownList")) CargarCampaniaPorMaterial("3");

    }

    function RefrescarWidgets() {
        /*
        viewModel.Parametros.proveedorId = $("#proveedorId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.comercialId = $("#comercialId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.material = $("#material").data("kendoDropDownList").dataItem();
        viewModel.Parametros.tipoId = $("#tipoId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.precioMonedaId = $("#precioMonedaId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.campanaId = $("#campanaId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.provinciaId = $("#provinciaId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.LocalidadId = $("#LocalidadId").data("kendoDropDownList").dataItem();
        viewModel.Parametros.sustentableMonedaId = $("#sustentableMonedaId").data("kendoDropDownList").dataItem();
        */
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
        //obj.ProveedorId = $("#proveedorId").val();
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

        if ($("#buscadorProveedor").val() != "") {

            cuitAux = $("#buscadorProveedor").val().split('(');
            cuit = cuitAux[1].split(')');

            proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0] });

        }

        obj.ProveedorId = proveedorId;

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

            if (ExistsErrorMessages(result.errores)) {
                MensErr(result.errores[0].Message);
            }
            else {
                window.location.href = window.location.origin + "/CompraNet";
                MensInfo("Se ha realizado la operacion con exito");
                //window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + result.ProveedorId;
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

                if (localidadProvincia.CUIT != "") {
                    $("#provinciaId").data("kendoDropDownList").value(localidadProvincia.ProvinciaId);
                    CargarLocalidadPorProvincia(localidadProvincia.ProvinciaId);
                    $("#LocalidadId").data("kendoDropDownList").value(localidadProvincia.LocalidadId);
                } else {
                    $("#provinciaId").data("kendoDropDownList").value("");
                    $("#LocalidadId").data("kendoDropDownList").value("");
                }
                
            }
        }
    }
