var viewModel;

var datosIniAbmFijacionDePrecio;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InitMaskedDatePicker();

    InicializarElementos();

    CreateGridFijacionDePrecio();

    CrearViewModel();

    AsignarBotones();

    InicializarCombos();
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#MaterialId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#MaterialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#MaterialId").data("kendoDropDownList").text("");
        }
    });

    $("#Precio").kendoNumericTextBox({
        format: "#.00"
    });

    $("#Fecha").kendoMaskedDatePicker();

    $("#ProveedorId").kendoNumericTextBox({
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
                    FijacionId: { type: "number", editable: false },
                    MatDescripcion: { type: "string", editable: false },
                    Precio: { type: "number", editable: false },
                    Fecha: { type: "date", editable: false },
                    ProveedorId: { type: "number", editable: false },
                }
            }
        },
    });

    return ds;
}

function CreateGridFijacionDePrecio() {
    $("#gridIniFijacionDePrecio").kendoGrid({
        columns: [
            { field: "MatDescripcion", title: "Material", width: 120 },
            { field: "Precio", title: "Precio", width: "120px", format: "{0:n2}", attributes: { style: "text-align: right;" } },
            { field: "Fecha", title: "Fecha", width: "130px", format: "{0:dd/MM/yyyy HH:mm}" },
            { field: "ProveedorId", title: "Proveedor", width: "90px", format: "{0:n0}", attributes: { style: "text-align: right;" } },

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

        MaterialCombo: [],

        FijacionDePrecio: null,
    });

    kendo.bind($("#Abm"), viewModel);
}

function InicializarCombos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniAbmFijacionDePrecio = data;
            AsignarCombos();
            InicializarBusquedaInicial();
        }
    }

    MSExecuteURLOnServerAsync('/FijacionDePrecio/Inicializar', funcReturn, '');
}

function AsignarCombos() {
    viewModel.set("MaterialCombo", datosIniAbmFijacionDePrecio.Datos.Material);
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

    MSExecuteURLOnServerAsync('/FijacionDePrecio/Buscar', funcReturn, '');
}

function AsignarBotones() {
    $("#butAgregar").click(function () {
        Agregar();
    });

    $("#butModificar").click(function () {
        Modificar();
    });

    $("#gridIniFijacionDePrecio").on("dblclick", "tr.k-state-selected", function () {
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
    if (model.FijacionDePrecio.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var fijaciondeprecio = {
        "FijacionId": model.FijacionDePrecio.FijacionId,
        "MaterialId": model.FijacionDePrecio.MaterialId,
        "Precio": model.FijacionDePrecio.Precio,
        "Fecha": model.FijacionDePrecio.Fecha,
        "ProveedorId": model.FijacionDePrecio.ProveedorId,
    };

    viewModel.set("FijacionDePrecio", fijaciondeprecio);

    viewModel.FijacionDePrecio.MaterialId = $("#MaterialId").data("kendoDropDownList").dataItem();
    viewModel.FijacionDePrecio.Fecha = $("#Fecha").data("kendoDatePicker").value();
}

function LimpiarValidaciones() {
    $("#errMaterialId").css("display", "none");
    $("#errPrecio").css("display", "none");
    $("#errFecha").css("display", "none");
    $("#errProveedorId").css("display", "none");
}

function HabilitarInicio() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function HabilitarAgregar() {
    var grid = $("#gridIniFijacionDePrecio").data("kendoGrid");

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
    var result = MSExecuteURLOnServer('/FijacionDePrecio/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function Modificar() {
    var grid = $("#gridIniFijacionDePrecio").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    if (data == null) {
        return;
    }

    var param = {
        "FijacionId": data.FijacionId,
    };

    var result = MSExecuteOnServer('/FijacionDePrecio/Aplicar', param);

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
    var grid = $("#gridIniFijacionDePrecio").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "FijacionId": data.FijacionId,
    };

    var result = MSExecuteOnServer('/FijacionDePrecio/Eliminar', param);

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
    var grid = $("#gridIniFijacionDePrecio").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();

    var errores = [];

    ValidDate(errores, "Fecha");

    if (errores.length > 0) {
        AddIncorectMessage(errores);
        ShowTooltipMessages("err", errores);
        return;
    }

    var datos = {
        "ObjectState": objectstate,
        "FijacionId": viewModel.get("FijacionDePrecio.FijacionId"),
        "MaterialId": GetDropDownValue(viewModel, "FijacionDePrecio.MaterialId.MaterialId"),
        "Precio": viewModel.get("FijacionDePrecio.Precio"),
        "Fecha": viewModel.get("FijacionDePrecio.Fecha"),
        "ProveedorId": viewModel.get("FijacionDePrecio.ProveedorId"),
    };

    var result = MSExecuteOnServer('/FijacionDePrecio/Grabar', datos);

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