var viewModel;

var datosIniAbmPreslip;

$(document).ready(function () {

    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InitMaskedDatePicker();

    InicializarElementos();

    CreateGridPreslip();

    CrearViewModel();

    AsignarBotones();

    InicializarCombos();
	
	
	
});

function InicializarElementos() {

    kendo.culture("es-AR");

    $("#TipoDeNegocioId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "TipoDeNegocioId"
    });

    $("#TipoDeNegocioId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#TipoDeNegocioId").data("kendoDropDownList").text("");
        }
    });

    $("#MaterialId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#MaterialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#MaterialId").data("kendoDropDownList").text("");
        }
    });

    $("#CampañaId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });

    $("#CampañaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#CampañaId").data("kendoDropDownList").text("");
        }
    });

    $("#Cantidad").kendoNumericTextBox({
        format: "#.00"
    });

    $("#Precio").kendoNumericTextBox({
        format: "#.00"
    });

    $("#FechaDesde").kendoMaskedDatePicker();

    $("#FechaHasta").kendoMaskedDatePicker();

    $("#FechaDeEntrega").kendoMaskedDatePicker();

    $("#ProveedorId").kendoNumericTextBox({
        format: "0"
    });

    $("#EstadoPreslipId").kendoNumericTextBox({
        format: "0"
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

}

function CrearResultadosDataSource(datos) {

    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    PreslipId: { type: "number", editable: false },
                    TdnDescripcion: { type: "string", editable: false },
                    MatDescripcion: { type: "string", editable: false },
                    CamDescripcion: { type: "string", editable: false },
                    Cantidad: { type: "number", editable: false },
                    Precio: { type: "number", editable: false },
                    FechaDesde: { type: "date", editable: false },
                    FechaHasta: { type: "date", editable: false },
                    FechaDeEntrega: { type: "date", editable: false },
                    ProveedorId: { type: "number", editable: false },
                    EstadoPreslipId: { type: "number", editable: false },
                }
            }
        },
    });

    return ds;
}

function enviar(e) {
	e.preventDefault();

	var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
	console.log("data",dataItem);
}

function CreateGridPreslip() {

    $("#gridIniPreslip").kendoGrid({
        columns: [
            { field: "TdnDescripcion", title: "Tipo de Negocio" },
            { field: "MatDescripcion", title: "Material" },
            { field: "CamDescripcion", title: "Campaña" },
            { field: "Cantidad", title: "Cantidad", width: "120px", format: "{0:n2}", attributes: { style: "text-align: right;" } },
            { field: "Precio", title: "Precio", width: "120px", format: "{0:n2}", attributes: { style: "text-align: right;" } },
            { field: "FechaDesde", title: "Fecha Desde", width: "130px", format: "{0:dd/MM/yyyy HH:mm}" },
            { field: "FechaHasta", title: "Fecha Hasta", width: "130px", format: "{0:dd/MM/yyyy HH:mm}" },
            { field: "FechaDeEntrega", title: "Fecha de Entrega", width: "130px", format: "{0:dd/MM/yyyy HH:mm}" },
            { field: "ProveedorId", title: "Proveedor", width: "90px", format: "{0:n0}", attributes: { style: "text-align: right;" } },
            { field: "EstadoPreslipId", title: "Estado ", width: "90px", format: "{0:n0}", attributes: { style: "text-align: right;" } },
			{ command: { text: "Enviar", click: enviar }, title: " ", width: "180px" }
        ],

        sortable: true,
        selectable: "row",
        change: onChangeGridInicial,

        filterable: {
            extra: false,
            messages: {
                info: "Filtros:",
                filter: "Filtrar",
                clear: "Limpiar",
                isTrue: "SI",
                isFalse: "NO",
                and: "Y",
                or: "O"
            },
            operators: {
               string: {
                    eq: "Igual",
                    neq: "Distinto",
                    startswith: "Comienza con",
                    contains: "Contiene",
                    endswith: "Finaliza con"
               },
               date: {
                    eq: "Igual",
                    neq: "Distinto",
                    gte: "Después o igual a",
                    gt: "Después",
                    lte: "Antes o igual a",
                    lt: "Antes",
              },
               number: {
                    eq: "Igual a",
                    neq: "Distinto a",
                    gte: "Mayor que o igual a",
                    gt: "Mayor que",
                    lte: "Menor que o igual a",
                    lt: "Menor que"
              }
           }
        }
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

        TipoDeNegocioCombo: [],
        MaterialCombo: [],
        CampañaCombo: [],

        Preslip: null,

    });

    kendo.bind($("#Abm"), viewModel);
}

function InicializarCombos() {

    var funcReturn = function (data) {

        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniAbmPreslip = data;
            AsignarCombos();
            InicializarBusquedaInicial();
        }
    }

    MSExecuteURLOnServerAsync('/Preslip/Inicializar', funcReturn, '');
}

function AsignarCombos() {

    viewModel.set("TipoDeNegocioCombo", datosIniAbmPreslip.Datos.TipoDeNegocio);
    viewModel.set("MaterialCombo", datosIniAbmPreslip.Datos.Material);
    viewModel.set("CampañaCombo", datosIniAbmPreslip.Datos.Campaña);
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

    MSExecuteURLOnServerAsync('/Preslip/Buscar', funcReturn, '');
}

function AsignarBotones() {

    $("#butAgregar").click(function () {
        Agregar();
    });

    $("#butModificar").click(function () {
        Modificar();
    });

    $("#gridIniPreslip").on("dblclick", "tr.k-state-selected", function () {
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

    if (model.Preslip.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var preslip = {
        "PreslipId": model.Preslip.PreslipId,
        "TipoDeNegocioId": model.Preslip.TipoDeNegocioId,
        "MaterialId": model.Preslip.MaterialId,
        "CampañaId": model.Preslip.CampañaId,
        "Cantidad": model.Preslip.Cantidad,
        "Precio": model.Preslip.Precio,
        "FechaDesde": model.Preslip.FechaDesde,
        "FechaHasta": model.Preslip.FechaHasta,
        "FechaDeEntrega": model.Preslip.FechaDeEntrega,
        "ProveedorId": model.Preslip.ProveedorId,
        "EstadoPreslipId": model.Preslip.EstadoPreslipId,
    };

    viewModel.set("Preslip", preslip);

    viewModel.Preslip.TipoDeNegocioId = $("#TipoDeNegocioId").data("kendoDropDownList").dataItem();
    viewModel.Preslip.MaterialId = $("#MaterialId").data("kendoDropDownList").dataItem();
    viewModel.Preslip.CampañaId = $("#CampañaId").data("kendoDropDownList").dataItem();
    viewModel.Preslip.FechaDesde = $("#FechaDesde").data("kendoDatePicker").value();
    viewModel.Preslip.FechaHasta = $("#FechaHasta").data("kendoDatePicker").value();
    viewModel.Preslip.FechaDeEntrega = $("#FechaDeEntrega").data("kendoDatePicker").value();
}

function LimpiarValidaciones() {

    $("#errTipoDeNegocioId").css("display", "none");
    $("#errMaterialId").css("display", "none");
    $("#errCampañaId").css("display", "none");
    $("#errCantidad").css("display", "none");
    $("#errPrecio").css("display", "none");
    $("#errFechaDesde").css("display", "none");
    $("#errFechaHasta").css("display", "none");
    $("#errFechaDeEntrega").css("display", "none");
    $("#errProveedorId").css("display", "none");
    $("#errEstadoPreslipId").css("display", "none");
}

function HabilitarInicio() {

    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function HabilitarAgregar() {

    var grid = $("#gridIniPreslip").data("kendoGrid");

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

    var result = MSExecuteURLOnServer('/Preslip/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function Modificar() {

    var grid = $("#gridIniPreslip").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    if (data == null) {
       return;
    }

    var param = {
        "PreslipId": data.PreslipId,
    };

    var result = MSExecuteOnServer('/Preslip/Aplicar', param);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            viewModel.set("isModifyDisabled", true);
            HabilitarEdicion();
            LimpiarValidaciones();
            UpdateViewModel(result);
        }
    }
}

function Eliminar() {

    Confirma('¿ Confirma la eliminación de este registro ?',
               function (dialogItself) {
                   EjecutarEliminar();
                   dialogItself.close();
               });
}

function EjecutarEliminar() {

    var grid = $("#gridIniPreslip").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "PreslipId": data.PreslipId,
    };

    var result = MSExecuteOnServer('/Preslip/Eliminar', param);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            UpdateViewModel(result);
            LimpiarValidaciones();
            HabilitarInicio();
            InicializarBusquedaInicial();
        }
    }
}

function Grabar() {

    var grid = $("#gridIniPreslip").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();

    var errores = [];

    ValidDate(errores, "FechaDesde");
    ValidDate(errores, "FechaHasta");
    ValidDate(errores, "FechaDeEntrega");

    if (errores.length > 0) {
        AddIncorectMessage(errores);
        ShowTooltipMessages("err", errores);
        return;
    }


    var datos = {
        "ObjectState": objectstate,
        "PreslipId": viewModel.get("Preslip.PreslipId"),
        "TipoDeNegocioId": GetDropDownValue(viewModel, "Preslip.TipoDeNegocioId.TipoDeNegocioId"),
        "MaterialId": GetDropDownValue(viewModel, "Preslip.MaterialId.MaterialId"),
        "CampañaId": GetDropDownValue(viewModel, "Preslip.CampañaId.CampañaId"),
        "Cantidad": viewModel.get("Preslip.Cantidad"),
        "Precio": viewModel.get("Preslip.Precio"),
        "FechaDesde": viewModel.get("Preslip.FechaDesde"),
        "FechaHasta": viewModel.get("Preslip.FechaHasta"),
        "FechaDeEntrega": viewModel.get("Preslip.FechaDeEntrega"),
        "ProveedorId": viewModel.get("Preslip.ProveedorId"),
        "EstadoPreslipId": viewModel.get("Preslip.EstadoPreslipId"),
    };

    var result = MSExecuteOnServer('/Preslip/Grabar', datos);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            UpdateViewModel(result);
            InicializarBusquedaInicial();
            MensInfo("Grabación Realizada Correctamente");
            HabilitarInicio();
        }
    }
}

function Cancelar() {

    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

