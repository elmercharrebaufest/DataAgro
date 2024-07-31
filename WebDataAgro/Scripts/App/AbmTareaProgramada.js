var viewModel;
var _DefaultDateTemplate = "{0:dd/MM/yyyy hh:mm:ss tt}";
var datosIniAbmCentro;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CreateGrid();

    CrearViewModel();

    InicializarBusquedaInicial();
    AsignarBotones();
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#Inicio").kendoDateTimePicker({
        value: new Date(),
        dateInput: true
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
                    Name: { type: "string", editable: false },
                    LastRunTime: { type: "date", editable: false },
                    NextRunTime: { type: "date", editable: false },
                    ActionURL: { type: "string", editable: false },
                    RepeticionEnMinutos: { type: "string", editable: false },
                    Inicio: { type: "string", editable: false }
                }
            }
        },
    });

    return ds;
}

function CreateGrid() {
    $("#gridIniRango").kendoGrid({
        columns: [
            { field: "Name", title: "Nombre", filterable: false },
            { field: "NextRunTime", title: "Próxima ejecución", format: _DefaultDateTemplate, filterable: false },
            { field: "LastRunTime", title: "Ultima ejecución", format: _DefaultDateTemplate, filterable: false },
            {
                field: "ActionURL", title: "Acción", filterable: false, template: function (dataItem) {
                    return '<div title="' + dataItem.ActionURL + '">' + dataItem.ActionURL + '</div>';
                }
            },
            { field: "RepeticionEnMinutos", title: "Repetición (min)", filterable: false }
        ],
        scrollable: true,
        sortable: false,
        selectable: "row",
        change: onChangeGridInicial,
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

        TareaProgramada: null,
    });

    kendo.bind($("#Abm"), viewModel);
}

function InicializarBusquedaInicial() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            viewModel.set("Resultados", CrearResultadosDataSource(data.Datos));
            HabilitarBotones();
        }
    }

    MSExecuteURLOnServerAsync('/TareaProgramada/Buscar', funcReturn, '');
}


function AsignarBotones() {
    $("#butAgregar").click(function () {
        Agregar();
    });

    //$("#butModificar").click(function () {
    //    Modificar();
    //});

    //$("#gridIniRango").on("dblclick", "tr.k-state-selected", function () {
    //    LimpiarAgregarModificar();
    //    Modificar();
    //});

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

function Agregar() {

    var result = MSExecuteURLOnServer('/TareaProgramada/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
    }
}

function Eliminar() {
    Confirma('¿Confirma la eliminación de este registro?',
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
        "Name": data.Name,
    };

    var result = MSExecuteOnServer('/TareaProgramada/Eliminar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            HabilitarInicio();
            InicializarBusquedaInicial();
        }
    }
}

function Grabar() {
    var grid = $("#gridIniRango").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var datos = {
        "Name": viewModel.get("TareaProgramada.Name"),
        "ActionURL": viewModel.get("TareaProgramada.ActionURL"),
        "RepeticionEnMinutos": viewModel.get("TareaProgramada.RepeticionEnMinutos"),
        "Inicio": viewModel.get("TareaProgramada.Inicio")
    };
    var result = MSExecuteOnServer('/TareaProgramada/Grabar', datos);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            UpdateViewModel(result);
            InicializarBusquedaInicial();
            MensInfo("Grabación Realizada Correctamente");
            HabilitarInicio();
            viewModel.set("TareaProgramada", {});
        }
    }
}

function Cancelar() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function UpdateViewModel(model) {

    var rango = {
        "Name": model.Name,
        "LastRunTime": model.LastRunTime,
        "NextRunTime": model.NextRunTime,
        "ActionURL": model.ActionURL,
        "RepeticionEnMinutos": model.RepeticionEnMinutos,
        "Inicio": model.Inicio
    };

    viewModel.set("TareaProgramada", rango);
}

function HabilitarBotones() {
    viewModel.set("isFilterDisabled", false);
    viewModel.set("isAddNewDisabled", false);
    viewModel.set("isDeleteDisabled", true);
}

function HabilitarAgregar() {
    var grid = $("#gridIniRango").data("kendoGrid");

    grid.clearSelection();

    $('#rootwizard').bootstrapWizard('show', 'tab2');
}

function HabilitarInicio() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}