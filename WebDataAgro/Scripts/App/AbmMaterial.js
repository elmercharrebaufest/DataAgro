var viewModel;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CreateGridMaterial();

    CrearViewModel();

    AsignarBotones();

    InicializarBusquedaInicial();

    InicializarCombos();
});

function CreateGridMaterial() {
    $("#gridIni").kendoGrid({
        columns: [
            { field: "Descripcion", title: "Material",  filterable: false },
            { field: "Codigo", title: "Codigo", filterable: false },
            { field: "CampaniaActual", title: "Campaña", filterable: false },
            { field: "CampaniaTablero", title: "Campaña Tablero", filterable: false }

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

        Material: null,

        CampaniaCombo: [],
        CampaniaTableroCombo: []
    });

    kendo.bind($("#Abm"), viewModel);
}

function InicializarCombos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniAbmMaterial = data;
            AsignarCombos();
            InicializarBusquedaInicial();
        }
    }

    MSExecuteURLOnServerAsync('/Material/Inicializar', funcReturn, '');
}

function AsignarCombos() {
    viewModel.set("CampaniaCombo", datosIniAbmMaterial.Datos.Campania);
    viewModel.set("CampaniaTableroCombo", datosIniAbmMaterial.Datos.CampaniaTablero);
    
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

function CrearResultadosDataSource(datos) {
    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    MaterialId: { type: "number", editable: false },
                    Descripcion: { type: "string", editable: false },
                    Codigo: { type: "string", editable: false },
                    CampaniaActual: { type: "string", editable: false },
                    CampaniaTablero: { type: "string", editable: false }


                }
            }
        },
    });
    return ds;
}

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#CampaniaId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "CampaniaId",
    });

    $("#CampaniaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#CampaniaId").data("kendoDropDownList").text("");
        }
    });

    $("#CampaniaTableroId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "CampaniaTableroId",
    });

    $("#CampaniaTableroId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#CampaniaTableroId").data("kendoDropDownList").text("");
        }
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

    MSExecuteURLOnServerAsync('/Material/Buscar', funcReturn, '');
}

function HabilitarCancelar() {
    viewModel.set("isFilterDisabled", false);
    viewModel.set("isAddNewDisabled", false);
    viewModel.set("isDeleteDisabled", true);
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
    if (model.Material.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var material = {
        "MaterialId": model.Material.MaterialId,
        "Descripcion": model.Material.Descripcion,
        "Codigo": model.Material.Codigo,
        "CampaniaId": model.Material.CampañaId,
        "CampaniaTableroId": model.Material.CampaniaTableroId
    };

    viewModel.set("Material", material);

    viewModel.Material.CampaniaId = $("#CampaniaId").data("kendoDropDownList").dataItem();
    viewModel.Material.CampaniaTableroId = $("#CampaniaTableroId").data("kendoDropDownList").dataItem();

}

function LimpiarValidaciones() {
    //completar
    $("#errDescripcion").css("display", "none");
    $("#errCodigo").css("display", "none");
    $("#errCampaña").css("display", "none");
    $("#errCampaniaTablero").css("display", "none");

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
    var result = MSExecuteURLOnServer('/Material/Cancelar');

    if (result != null) {
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

    var oParam = {
        "Id": data.MaterialId,
    };

    if (data.MaterialId > 0) {
        var datosMaterial = MSExecuteOnServer('/Material/MaterialCombo', oParam);

        if (datosMaterial != null) {
            if (ExistsErrorMessages(datosMaterial.Errores)) {
                ShowTooltipMessages("err", datosMaterial.Errores);
            }
            else {
                viewModel.set("Material", datosMaterial.Material);
            }
        }
    }

    var result = MSExecuteOnServer('/Material/Aplicar', oParam);

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

    var oParam = {
        "Id": data.MaterialId,
    };

    var result = MSExecuteOnServer('/Material/Eliminar', oParam);

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
        "MaterialId": viewModel.get("Material.MaterialId"),
        "Descripcion": viewModel.get("Material.Descripcion"),
        "Codigo": viewModel.get("Material.Codigo"),
        "CampañaId": GetDropDownValue(viewModel, "Material.CampaniaId.CampaniaId"),
        "CampaniaTableroId": GetDropDownValue(viewModel, "Material.CampaniaTableroId.CampaniaTableroId"),


        
    };

    var result = MSExecuteOnServer('/Material/Grabar', datos);

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