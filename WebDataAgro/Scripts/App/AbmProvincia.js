var viewModel;

var datosIniAbmProvincia;

$(document).ready(function () {

    InicializarElementos();

    CreateGridProvincia();

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

}

function CrearResultadosDataSource(datos) {

    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    ProvinciaId: { type: "number", editable: false },
                    Nombre: { type: "string", editable: false },
                }
            }
        },
    });

    return ds;
}

function CreateGridProvincia() {

    $("#gridIniProvincia").kendoGrid({
        columns: [
            { field: "Nombre", title: "Nombre" },
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
        "ProvinciaId": data.ProvinciaId,
    };

    var result = MSExecuteOnServer('/Provincia/Aplicar', param);

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


        Provincia: null,

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

    MSExecuteURLOnServerAsync('/Provincia/Buscar', funcReturn, '');
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

    if (model.Provincia.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var provincia = {
        "ProvinciaId": model.Provincia.ProvinciaId,
        "Nombre": model.Provincia.Nombre,
    };

    viewModel.set("Provincia", provincia);

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

    var grid = $("#gridIniProvincia").data("kendoGrid");

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

    var grid = $("#gridIniProvincia").data("kendoGrid");

    grid.clearSelection();

    $("#Nombre").focus();
}

function Agregar() {

    var result = MSExecuteURLOnServer('/Provincia/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function LimpiarValidaciones() {

    $("#errNombre").css("display", "none");
}

function Grabar() {

    var grid = $("#gridIniProvincia").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();


    var datos = {
        "ObjectState": objectstate,
        "ProvinciaId": viewModel.get("Provincia.ProvinciaId"),
        "Nombre": viewModel.get("Provincia.Nombre"),
    };

    var result = MSExecuteOnServer('/Provincia/Grabar', datos);

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

    var grid = $("#gridIniProvincia").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "ProvinciaId": data.ProvinciaId,
    };

    var result = MSExecuteOnServer('/Provincia/Eliminar', param);

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

    var result = MSExecuteURLOnServer('/Provincia/Cancelar');

    if (result != null) {
        UpdateViewModel(result);
        LimpiarValidaciones();
        HabilitarCancelar();
    }

}

