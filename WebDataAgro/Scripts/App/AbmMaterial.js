var viewModel;

var datosIniAbmMaterial;

$(document).ready(function () {

    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CreateGridMaterial();

    CrearViewModel();

    AsignarBotones();

    LlenarGrilla(false);
});

function InicializarElementos() {

    kendo.culture("es-AR");


    $("#butFiltrar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Find.png")
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
                    MaterialId: { type: "number", editable: false },
                    Codigo: { type: "string", editable: false },
                    Descripcion: { type: "string", editable: false },
                }
            }
        },
        change: function (e) {

            var mens = "";            var tot = this.data().length;
            var cant = this.view().length;

            viewModel.set("recordMessage", "");

            if (tot >= 500) {
               mens = "Es posible que existan mas registros, ajuste los parámetros de busqueda, para reducir el número de resultados.";
            }

            if (tot > 0) {

                if (cant == 1) {
                    viewModel.set("recordMessage", cant.toString() + " Registro. " + mens);
                }
                else {
                    viewModel.set("recordMessage", cant.toString() + " Registros. "+ mens);
                }
            }
        }
    });

    return ds;
}

function CreateGridMaterial() {

    $("#gridIniMaterial").kendoGrid({
        columns: [
            { field: "Codigo", title: "Código", width: "150px" },
            { field: "Descripcion", title: "Descripción" },
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

    var param = {
        "Codigo": "",
        "Descripcion": "",
    };

    var ResultadosDataSource = CrearResultadosDataSource([]);

    viewModel = kendo.observable({

        Parametros: param,

        Resultados: ResultadosDataSource,

        recordMessage: "",

        isReadOnly: true,
        isFilterDisabled: true,
        isControlDisabled: false,
        isModifyDisabled: false,
        isAddNewDisabled: true,
        isDeleteDisabled: true,


        Material: null,

    });

    kendo.bind($("#Abm"), viewModel);
}

function AsignarBotones() {

    $("#butFiltrar").click(function () {
        LlenarGrilla(true);
    });

    $("#butAgregar").click(function () {
        Agregar();
    });

    $("#butModificar").click(function () {
        Modificar();
    });

    $("#gridIniMaterial").on("dblclick", "tr.k-state-selected", function () {
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

function LimpiarValidacionesParam() {

    $("#errParamCodigo").css("display", "none");
    $("#errParamDescripcion").css("display", "none");
}

function LlenarGrilla(showMessage) {

    LimpiarValidacionesParam();

    viewModel.set("recordMessage", "");

    var errores = [];


    if (errores.length > 0) {
        AddIncorectMessage(errores);
        ShowTooltipMessages("err", errores);
        return;
    }

    var param = {
        "Codigo": viewModel.get("Parametros.Codigo"),
        "Descripcion": viewModel.get("Parametros.Descripcion"),
    };

    var result = MSExecuteOnServer('/Material/Filtrar', param);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            viewModel.set("Resultados", CrearResultadosDataSource([]));
            ShowTooltipMessages("errParam", result.Errores);
        }
        else if (result.Datos.length > 0) {

            viewModel.set("Resultados", CrearResultadosDataSource(result.Datos));

            if (result.Datos.length >= 500) {
                viewModel.set("recordMessage", result.Datos.length.toString() + " Registros. Es posible que existan mas registros, ajuste los parámetros de busqueda, para reducir el número de resultados.");
            }
            else if (result.Datos.length > 1) {
                viewModel.set("recordMessage", result.Datos.length.toString() + " Registros.");
            }
            else {
                viewModel.set("recordMessage", result.Datos.length.toString() + " Registro.");
            }

            HabilitarCancelar();
        }
        else {
            viewModel.set("Resultados", CrearResultadosDataSource([]));
            HabilitarCancelar();

            if (showMessage) {
                MensInfo("No se encontraron datos que cumplan con el filtro indicado");
            }
        }
    }
}

function UpdateViewModel(model) {

    if (model.Material.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var material = {
        "MaterialId": model.Material.MaterialId,
        "Codigo": model.Material.Codigo,
        "Descripcion": model.Material.Descripcion,
    };

    viewModel.set("Material", material);

}

function LimpiarValidaciones() {

    $("#errCodigo").css("display", "none");
    $("#errDescripcion").css("display", "none");
}

function HabilitarInicio() {

    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function HabilitarAgregar() {

    var grid = $("#gridIniMaterial").data("kendoGrid");

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

    var result = MSExecuteURLOnServer('/Material/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function Modificar() {

    var grid = $("#gridIniMaterial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    if (data == null) {
       return;
    }

    var param = {
        "MaterialId": data.MaterialId,
    };

    var result = MSExecuteOnServer('/Material/Aplicar', param);

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

    var grid = $("#gridIniMaterial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "MaterialId": data.MaterialId,
    };

    var result = MSExecuteOnServer('/Material/Eliminar', param);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            UpdateViewModel(result);
            LimpiarValidaciones();
            HabilitarInicio();
            LlenarGrilla(false);
        }
    }
}

function Grabar() {

    var grid = $("#gridIniMaterial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();


    var datos = {
        "ObjectState": objectstate,
        "MaterialId": viewModel.get("Material.MaterialId"),
        "Codigo": viewModel.get("Material.Codigo"),
        "Descripcion": viewModel.get("Material.Descripcion"),
    };

    var result = MSExecuteOnServer('/Material/Grabar', datos);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            UpdateViewModel(result);
            LlenarGrilla(false);
            MensInfo("Grabación Realizada Correctamente");
            HabilitarInicio();
        }
    }
}

function Cancelar() {

    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

