var viewModel;

var datosIniAbmCentro;

$(document).ready(function() {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CreateGridRango();

    CrearViewModel();

    AsignarBotones();
    InicializarCombos();
    InicializarBusquedaInicial();
  
});

function InicializarElementos() {
    kendo.culture("es-AR");
    var hoy = new Date();

    $("#precioMinimo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });


    $("#precioMaximo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#material").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });
    
    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#material").data("kendoDropDownList").text("");
        }
    });
    $("#tipoNegocio").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId"
    });
    $("#tipoNegocio").change( function(){
        if ($("#tipoNegocio").val() == "3") {
            $("#divEntrega").hide();
            $("#desdeMes").data("kendoNumericTextBox").value("");
            $("#hastaMes").data("kendoNumericTextBox").value("");
            $("#desdeAnio").data("kendoNumericTextBox").value("");
            $("#hastaAnio").data("kendoNumericTextBox").value("");
        } else {
            $("#divEntrega").show();
        }
    });
    $("#tipoNegocio").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#tipoNegocio").data("kendoDropDownList").text("");
        }
    });

    $("#zona").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#zona").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#zona").data("kendoDropDownList").text("");
        }
    });
    $("#moneda").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#moneda").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#moneda").data("kendoDropDownList").text("");
        }
    });

    $("#FechaDesde").kendoDateTimePicker({
        value: new Date(),
        dateInput: true
    });
    $("#FechaHasta").kendoDateTimePicker({
        value: new Date(),
        dateInput: true
    });

    $("#cantidad").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });


    $("#desdeMes").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 1,
        max:12
    });
    $("#hastaMes").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 1,
        max:12
    });
    $("#desdeAnio").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 0
    });
    $("#hastaAnio").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 0
    });

    $("#butAgregar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Agregar.png")
    });

    $("#butModificar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Modificar.png")
    });

    $("#butEliminar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Eliminar.png")
    });

    $("#butAceptar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Aceptar.png")
    });

    $("#butCancelar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Cancelar.png")
    });

    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    var stringHora = hoy.getHours() + ":" + hoy.getMinutes();
    $("#FechaDesde").data("kendoDateTimePicker").value(stringDia + " " + stringHora);
    $("#FechaHasta").data("kendoDateTimePicker").value(stringDia + " " + "23:59");
    //$("#cantidad").data("kendoNumericTextBox").value("20000");
    $("#hastaAnio").data("kendoNumericTextBox").value(hoy.getFullYear().toString());
    $("#desdeAnio").data("kendoNumericTextBox").value(hoy.getFullYear().toString());
}

function CrearResultadosDataSource(datos) {
    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    Id: { type: "number", editable: false },
                    PrecioMinimo: { type: "number", editable: false },
                    PrecioMaximo: { type: "number", editable: false },
                    Material: { type: "string", editable: false },
                    TipoNegocio: { type: "string", editable: false },
                    Moneda: { type: "string", editable: false },
                    FechaDesde: { type: "date", format: 'DD/MM/YYYY HH:mm:ss', editable: false },
                    FechaHasta: { type: "date", format: 'DD/MM/YYYY HH:mm:ss', editable: false },
                    Cantidad: { type: "number", editable: false },
                    EntregaDesde: { type: "string", editable: false },
                    EntregaHasta: { type: "string", editable: false },
                    Zona: { type: "string", editable: false }
                }
            }
        },
        sort: { field: "FechaDesde", dir: "desc" }
    });

    return ds;
}

function CreateGridRango() {
    $("#gridIniRango").kendoGrid({

        columns: [
            { field: "TipoNegocio", title: "Negocio", filterable: false },
            { field: "PrecioMinimo", title: "Mínimo", filterable: false },
            { field: "PrecioMaximo", title: "Máximo", filterable: false },
            { field: "Material", title: "Material", filterable: false },
            { field: "Moneda", title: "Moneda", filterable: false },
            {
                field: "FechaDesde", title: "Fecha Desde", filterable: false,
                template: "#= kendo.toString(kendo.parseDate(FechaDesde, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm') #"
            },
            {
                field: "FechaHasta", title: "Fecha Hasta", filterable: false,
                template: "#= kendo.toString(kendo.parseDate(FechaHasta, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm') #"
            },
            { field: "Cantidad", title: "Cantidad", filterable: false },
            { field: "EntregaDesde", title: "Entrega Desde", filterable: false },
            { field: "EntregaHasta", title: "Entrega Hasta", filterable: false },
            { field: "Zona", title: "Zona", filterable: false }
        ],
        sortable: true,
        scrollable: false,
        selectable: "row",
        change: onChangeGridInicial
    });
}

function onChangeGridInicial() {
    var row = this.select();

    var data = this.dataItem(row);

    if (data == null) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }
}

function CrearViewModel() {
    var ResultadosDataSource = CrearResultadosDataSource([]);

    viewModel = kendo.observable({
        Resultados: ResultadosDataSource,

        isReadOnly: true,
        isFilterDisabled: true,
        isControlDisabled: false,
        isModifyDisabled: false,
        isAddNewDisabled: true,
        isDeleteDisabled: true,

        Rango: null,
    });

    kendo.bind($("#Abm"), viewModel);
}

function InicializarCombos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniAbmRango = data;
            AsignarCombos();
            InicializarBusquedaInicial();
        }
    };

    MSExecuteURLOnServerAsync('/RangoConfirmacionAutomatica/Inicializar', funcReturn, '');
}

function AsignarCombos() {
    viewModel.set("MaterialCombo", datosIniAbmRango.Datos.Material);
    viewModel.set("MonedaCombo", datosIniAbmRango.Datos.Moneda);
    viewModel.set("ZonaCombo", datosIniAbmRango.Datos.Zona);
    viewModel.set("TipoNegocioCombo", datosIniAbmRango.Datos.TipoNegocio);
    
}

function InicializarBusquedaInicial() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            viewModel.set("Resultados", CrearResultadosDataSource(data.Datos));
            HabilitarCancelar();
        }
    }

    MSExecuteURLOnServerAsync('/RangoConfirmacionAutomatica/Buscar', funcReturn, '');
}

function AsignarBotones() {
    $("#butAgregar").click(function () {
        Agregar();
        var hoy = new Date();
        var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
        var stringHora = hoy.getHours() + ":" + hoy.getMinutes();
        $("#FechaDesde").data("kendoDateTimePicker").value(stringDia + " " + stringHora);
        $("#FechaHasta").data("kendoDateTimePicker").value(stringDia + " " + "23:59");
        $("#cantidad").data("kendoNumericTextBox").value("20000");
        $("#hastaAnio").data("kendoNumericTextBox").value(hoy.getFullYear().toString());
        $("#desdeAnio").data("kendoNumericTextBox").value(hoy.getFullYear().toString());
      
    });

    $("#butModificar").click(function () {
        Modificar();
    });

    $("#gridIniRango").on("dblclick", "tr.k-state-selected", function () {
        LimpiarAgregarModificar();
        Modificar();
    });

    $("#butEliminar").click(function () {
        Eliminar();
    });

    $("#butAceptar").click(function () {
        Grabar();
    });

    $("#butCancelar").click(function () {
        Cancelar();
    });
}

function UpdateViewModel(model) {


    var rango = {
        "Id": model.Rango.Id,
        "PrecioMinimo": model.Rango.PrecioMinimo,
        "PrecioMaximo": model.Rango.PrecioMaximo,
        "MaterialId": model.Rango.MaterialId,
        "MonedaId": model.Rango.MonedaId,
        "FechaDesde": model.Rango.FechaDesde,
        "FechaHasta": model.Rango.FechaHasta,
        "ZonaId": model.Rango.ZonaId,
        "TipoNegocioId": model.Rango.TipoNegocioId,
        "Cantidad": model.Rango.Cantidad,
        "DesdeMes": model.Rango.DesdeMes,
        "DesdeAnio": model.Rango.DesdeAnio,
        "HastaMes": model.Rango.HastaMes,
        "HastaAnio": model.Rango.HastaAnio
    };

    viewModel.set("RangoConfirmacionAutomatica", rango);
}

function LimpiarValidaciones() {
    //completar
    $("#errDescripcion").css("display", "none");
    $("#errCodigoSap").css("display", "none");
}

function HabilitarInicio() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function HabilitarAgregar() {
    var grid = $("#gridIniRango").data("kendoGrid");

    grid.clearSelection();

    $('#rootwizard').bootstrapWizard('show', 'tab2');
}

function HabilitarEdicion() {
    $('#rootwizard').bootstrapWizard('show', 'tab2');
}

function HabilitarCancelar() {
    viewModel.set("isFilterDisabled", false);
    viewModel.set("isAddNewDisabled", false);
    viewModel.set("isDeleteDisabled", true);
}

function Agregar() {

    var result = MSExecuteURLOnServer('/RangoConfirmacionAutomatica/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}
function LimpiarAgregarModificar() {
    var rango = {};
    viewModel.set("RangoConfirmacionAutomatica", rango);
    $("#precioMinimo").data("kendoNumericTextBox").value("");
    $("#precioMaximo").data("kendoNumericTextBox").value("");
    $("#material").data("kendoDropDownList").value("");
    $("#tipoNegocio").data("kendoDropDownList").value("");
    $("#moneda").data("kendoDropDownList").value("");
    $("#FechaDesde").data("kendoDateTimePicker").value("");
    $("#FechaHasta").data("kendoDateTimePicker").value("");
    $("#zona").data("kendoDropDownList").value("");
    $("#cantidad").data("kendoNumericTextBox").value("");
    $("#desdeMes").data("kendoNumericTextBox").value("");
    $("#desdeAnio").data("kendoNumericTextBox").value("");
    $("#hastaMes").data("kendoNumericTextBox").value("");
    $("#hastaAnio").data("kendoNumericTextBox").value("");
}
function Modificar() {

    var grid = $("#gridIniRango").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    if (data == null) {
        return;
    }

    var param = {
        "Id": data.Id
    };

    if (data.Id > 0) {
        var datosRango = MSExecuteOnServer('/RangoConfirmacionAutomatica/RangoCombo', param);

        if (datosRango != null) {
            if (ExistsErrorMessages(datosRango.Errores)) {
                ShowTooltipMessages("err", datosRango.Errores);
            }
            else {
                viewModel.set("RangoConfirmacionAutomatica", datosRango.Rango);
                $("#FechaDesde").data("kendoDateTimePicker").value(kendo.parseDate(datosRango.Rango.FechaDesde, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                $("#FechaHasta").data("kendoDateTimePicker").value(kendo.parseDate(datosRango.Rango.FechaHasta, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                viewModel.set("isModifyDisabled", true);
                HabilitarEdicion();
                LimpiarValidaciones();
                UpdateViewModel(datosRango);
            }
        }
    }
    $("#tipoNegocio").change();
}

function Eliminar() {
    Confirma('¿ Confirma la eliminación de este registro ?',
        function (dialogItself) {
            EjecutarEliminar();
            dialogItself.close();
        });
}

function EjecutarEliminar() {
    var grid = $("#gridIniRango").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "Id": data.Id
    };

    var result = MSExecuteOnServer('/RangoConfirmacionAutomatica/Eliminar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            LimpiarValidaciones();
            HabilitarInicio();
            InicializarBusquedaInicial();
        }
    }
}

function Grabar() {
    var grid = $("#gridIniRango").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    LimpiarValidaciones();

    var datos = {
        "Id": viewModel.get("RangoConfirmacionAutomatica.Id"),
        "PrecioMinimo": $("#precioMinimo").data("kendoNumericTextBox").value(),
        "PrecioMaximo": $("#precioMaximo").data("kendoNumericTextBox").value(),
        "MaterialId": $("#material").data("kendoDropDownList").value(),
        "MonedaId": $("#moneda").data("kendoDropDownList").value(),
        "FechaDesde": $("#FechaDesde").data("kendoDateTimePicker").value(),
        "Fechahasta": $("#FechaHasta").data("kendoDateTimePicker").value(),
        "ZonaId": $("#zona").data("kendoDropDownList").value(),
        "TipoNegocioId": $("#tipoNegocio").data("kendoDropDownList").value(),
        "Cantidad": $("#cantidad").data("kendoNumericTextBox").value(),
        "DesdeMes": $("#desdeMes").data("kendoNumericTextBox").value(),
        "DesdeAnio": $("#desdeAnio").data("kendoNumericTextBox").value(),
        "HastaMes": $("#hastaMes").data("kendoNumericTextBox").value(),
        "HastaAnio": $("#hastaAnio").data("kendoNumericTextBox").value()
    };
    var result = MSExecuteOnServer('/RangoConfirmacionAutomatica/Grabar', datos);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            UpdateViewModel(result);
            InicializarBusquedaInicial();
            MensInfo("Grabación Realizada Correctamente");
            HabilitarInicio();
            LimpiarAgregarModificar();
        }
    }
}

function Cancelar() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}