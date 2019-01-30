var viewModel;

var datosIniAbmOperador;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CreateGridOperador();

    CrearViewModel();

    AsignarBotones();

    InicializarBusquedaInicial();

    var idPerfil = $("#PerfilId").val();

    if (idPerfil == 4 || idPerfil == 5) {
        //var EsAdmin = $("#Administrador");
        $("#Administrador").prop("checked", false);
        $("#Administrador").prop("disabled", true);
    }
});

function InicializarElementos() {
    kendo.culture("es-AR");

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
                    Id: { type: "number", editable: false },
                    Descripcion: { type: "string", editable: false },
                    CodigoSap: { type: "string", editable: false }
                }
            }
        }
    });

    return ds;
}

function CreateGridOperador() {
    $("#gridIni").kendoGrid({
        columns: [
            { field: "Descripcion", title: "Operador", filterable: false }
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
                    lt: "Antes"
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

        Operador: null
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
    };

    MSExecuteURLOnServerAsync('/Operador/Buscar', funcReturn, '');
}

function AsignarBotones() {
    $("#butAgregar").click(function () {
        Agregar();
    });

    $("#butModificar").click(function () {
        Modificar();
    });

    $("#gridIni").on("dblclick", "tr.k-state-selected", function () {
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
    if (model.Operador.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var operador = {
        "Id": model.Operador.Id,
        "Descripcion": model.Operador.Descripcion
    };

    viewModel.set("Operador", operador);
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
    var grid = $("#gridIni").data("kendoGrid");

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
    var result = MSExecuteURLOnServer('/Operador/Cancelar');

    if (result.HayError) {
        MensErr(result.Errores[0].Message);
    } else {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function Modificar() {
    var grid = $("#gridIni").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    if (data == null) {
        return;
    }

    var param = {
        "Id": data.Id
    };

    if (data.Id > 0) {
        var datosOperador = MSExecuteOnServer('/Operador/OperadorCombo', param);

        if (datosOperador != null) {
            if (ExistsErrorMessages(datosOperador.Errores)) {
                ShowTooltipMessages("err", datosOperador.Errores);
            }
            else {
                viewModel.set("Operador", datosOperador.Operador);
            }
        }
    }

    var result = MSExecuteOnServer('/Operador/Aplicar', param);

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
    var grid = $("#gridIni").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "Id": data.Id
    };

    var result = MSExecuteOnServer('/Operador/Eliminar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            //UpdateViewModel(result);
            LimpiarValidaciones();
            HabilitarInicio();
            InicializarBusquedaInicial();
        }
    }
}

function Grabar() {
    var grid = $("#gridIni").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();

    var datos = {
        "ObjectState": objectstate,
        "Id": viewModel.get("Operador.Id"),
        "Descripcion": viewModel.get("Operador.Descripcion"),
        "CodigoSap": viewModel.get("Operador.CodigoSap")
    };

    var result = MSExecuteOnServer('/Operador/Grabar', datos);

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