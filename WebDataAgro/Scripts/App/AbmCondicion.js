var viewModel;

var datosIniAbmCondicion;

$(document).ready(function () {

    InicializarElementos();

    CreateGridCondicion();

    CrearViewModel();

    AsignarBotones();

    HabilitarInicio();

    InicializarBusquedaInicial();
});

function InicializarElementos() {

    kendo.culture("es-AR");


    $("#butAgregar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Agregar.png")
    });

    $("#butGrabar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Grabar.png")
    });

    $("#butEliminar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Eliminar.png")
    });

    $("#butCancelar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Cancelar.png")
    });

    $("#Inhabilitado").prop("checked", true);

}

function CrearResultadosDataSource(datos) {

    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    CondicionId: { type: "number", editable: false },
                    Descripcion: { type: "string", editable: false },
                    Inhabilitado: { type: "checkbox", editable: false },
                }
            }
        },
    });

    return ds;
}

function CreateGridCondicion() {

    $("#gridIniCondicion").kendoGrid({
        columns: [
            { field: "Descripcion", title: "Descripción" }/*,
            { field: "Inhabilitado", title: "inhabilitado-condicion" },*/
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
       return;
    }

    var param = {
        "CondicionId": data.CondicionId,
    };

    var result = MSExecuteOnServer('/Condicion/Aplicar', param);

    if (result != null) {

        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            viewModel.set("isModifyDisabled", true);
            LimpiarValidaciones();
            UpdateViewModel(result);
            HabilitarEdicion();
        }
    }
}

function CrearViewModel() {

    var ResultadosDataSource = CrearResultadosDataSource([]);

    viewModel = kendo.observable({

        Resultados: ResultadosDataSource,

        isReadOnly: true,
        isFilterDisabled: true,
        isModifyDisabled: false,
        isControlDisabled: true,
        isDeleteDisabled: true,


        Condicion: null,

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
            HabilitarCancelar();
        }
    }

    MSExecuteURLOnServerAsync('/Condicion/Buscar', funcReturn, '');
}

function AsignarBotones() {

    $("#butAgregar").click(function () {
        Agregar();
    });

    $("#butGrabar").click(function () {
        Grabar();
    });

    $("#butEliminar").click(function () {
        Eliminar();
    });

    $("#butCancelar").click(function () {
        Cancelar();
    });
}

function UpdateViewModel(model) {

    if (model.Condicion.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var condicion = {
        "CondicionId": model.Condicion.CondicionId,
        "Descripcion": model.Condicion.Descripcion,
        "Inhabilitado": model.Condicion.Inhabilitado
    };

    viewModel.set("Condicion", condicion);

}

function HabilitarInicio() {

    viewModel.set("isControlDisabled", true);
    viewModel.set("isAddDisabled", true);
    viewModel.set("isDeleteDisabled", true);
}

function HabilitarCancelar() {

    viewModel.set("isFilterDisabled", false);
    viewModel.set("isControlDisabled", true);
    viewModel.set("isAddDisabled", false);
    viewModel.set("isDeleteDisabled", true);

    var grid = $("#gridIniCondicion").data("kendoGrid");

    grid.clearSelection();
}

function HabilitarEdicion() {

    viewModel.set("isControlDisabled", false);
    viewModel.set("isAddDisabled", false);
    viewModel.set("isDeleteDisabled", false);
}

function HabilitarAgregar() {

    viewModel.set("isControlDisabled", false);
    viewModel.set("isAddDisabled", true);
    viewModel.set("isDeleteDisabled", true);

    var grid = $("#gridIniCondicion").data("kendoGrid");

    grid.clearSelection();

    $("#Descripcion").focus();
    $("#Inhabilitado").attr("checked", false);
}

function Agregar() {

    var result = MSExecuteURLOnServer('/Condicion/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function LimpiarValidaciones() {

    $("#errDescripcion").css("display", "none");
}

function Grabar() {

    var grid = $("#gridIniCondicion").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();


    var datos = {
        "ObjectState": objectstate,
        "CondicionId": viewModel.get("Condicion.CondicionId"),
        "Descripcion": viewModel.get("Condicion.Descripcion"),
        "Inhabilitado": viewModel.get("Condicion.Inhabilitado"),
    };

    var result = MSExecuteOnServer('/Condicion/Grabar', datos);

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

function Eliminar() {

    Confirma('¿ Confirma la eliminación de este registro ?',
               function (dialogItself) {
                   EjecutarEliminar();
                   dialogItself.close();
               });
}

function EjecutarEliminar() {

    var grid = $("#gridIniCondicion").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "CondicionId": data.CondicionId,
    };

    var result = MSExecuteOnServer('/Condicion/Eliminar', param);

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

function Cancelar() {

    var result = MSExecuteURLOnServer('/Condicion/Cancelar');

    if (result != null) {
        UpdateViewModel(result);
        LimpiarValidaciones();
        HabilitarCancelar();
    }

}

