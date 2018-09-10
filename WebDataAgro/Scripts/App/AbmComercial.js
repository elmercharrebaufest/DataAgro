var viewModel;

var datosIniAbmCentro;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CreateGridCentro();

    CrearViewModel();

    AsignarBotones();

    InicializarCombos();

    var idPerfil = $("#PerfilId").val();

    if (idPerfil == 4 || idPerfil == 5) {
        //var EsAdmin = $("#Administrador");
        $("#Administrador").prop("checked", false);
        $("#Administrador").prop("disabled", true);
    }
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#PerfilId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "PerfilId",
        change: function (e) {
            var idPerfil = this.dataItem().PerfilId;
            var empleadoACargoDDList = $("#EmpleadorACargo").data("kendoDropDownList");

            if (idPerfil == 4 || idPerfil == 5) {
                //var EsAdmin = $("#Administrador");
                $("#Administrador").prop("checked", false);
                $("#Administrador").prop("disabled", true);
                viewModel.set("Comercial.Administrador", false);
                $("#EmpleadorACargo").data("kendoDropDownList").text("");
                //$("#EmpleadorACargo").data("kendoDropDownList").value(null);
                empleadoACargoDDList.enable(false);
                viewModel.Comercial.EmpleadorACargo = null;
                //console.log(GetDropDownValue(viewModel, "Comercial.EmpleadorACargo.ComercialId"));
            }
            else {
                empleadoACargoDDList.enable(true);
                $("#Administrador").prop("disabled", false);
            }
            //alert();
            // Use the value of the widget
        }
    });

    $("#PerfilId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#PerfilId").data("kendoDropDownList").text("");
        }
    });

    $("#EmpleadorACargo").kendoDropDownList({
        //dataTextField: "Apellido" + " " + "Nombre",
        dataTextField: "Apellido",
        dataValueField: "ComercialId",
        //template: "#=Apellido # #=Nombres #",
        // dataSource: {
        /*
        schema: {
                 parse: function(response) {
                 $.each(response, function(idx, elem) {
                     elem.prueba = elem.Apellido + " " + elem.Nombres;
                 });
                     return response;
                }
            }*/
        //}
    });

    $("#EmpleadorACargo").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#EmpleadorACargo").data("kendoDropDownList").text("");
        }
    });

    /*$("#GrupoDeCompras").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#GrupoDeCompras").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#GrupoDeCompras").data("kendoDropDownList").text("");
        }
    });*/

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
                    ComercialId: { type: "number", editable: false },
                    Apellido: { type: "string", editable: false },
                    Nombres: { type: "string", editable: false },
                    PerDescripcion: { type: "string", editable: false },
                }
            }
        },
    });

    return ds;
}

function CreateGridCentro() {
    $("#gridIniComercial").kendoGrid({
        columns: [
            { field: "Apellido", title: "Apellido" },
            { field: "Nombres", title: "Nombres" },
            { field: "PerDescripcion", title: "Perfil" },
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

        PerfilCombo: [],
        ComercialCombo: [],
        //GrupoDeComprasCombo: [],

        Comercial: null,
    });

    kendo.bind($("#Abm"), viewModel);
}

function InicializarCombos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniAbmCentro = data;
            AsignarCombos();
            InicializarBusquedaInicial();
        }
    }

    MSExecuteURLOnServerAsync('/Comercial/Inicializar', funcReturn, '');
}

function AsignarCombos() {
    viewModel.set("PerfilCombo", datosIniAbmCentro.Datos.Perfil);
    viewModel.set("ComercialCombo", datosIniAbmCentro.Datos.Comercial);
    //viewModel.set("GrupoDeComprasCombo", datosIniAbmComercial.Datos.GrupoDeCompras);
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

    MSExecuteURLOnServerAsync('/Comercial/Buscar', funcReturn, '');
}

function AsignarBotones() {
    $("#butAgregar").click(function () {
        Agregar();
    });

    $("#butModificar").click(function () {
        Modificar();
    });

    $("#gridIniComercial").on("dblclick", "tr.k-state-selected", function () {
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
    if (model.Comercial.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var comercial = {
        "ComercialId": model.Comercial.ComercialId,
        "Apellido": model.Comercial.Apellido,
        "Nombres": model.Comercial.Nombres,
        "PerfilId": model.Comercial.PerfilId,
        "EmpleadorACargo": model.Comercial.EmpleadorACargoId,
        "IdActiveDirectory": model.Comercial.IdActiveDirectory,
        //"GrupoDeCompras": model.Comercial.GrupoDeCompras,
        "Administrador": model.Comercial.Administrador,
    };

    viewModel.set("Comercial", comercial);

    viewModel.Comercial.PerfilId = $("#PerfilId").data("kendoDropDownList").dataItem();
    viewModel.Comercial.EmpleadorACargo = $("#EmpleadorACargo").data("kendoDropDownList").dataItem();
    //viewModel.Comercial.GrupoDeCompras = $("#GrupoDeCompras").data("kendoDropDownList").dataItem();
}

function LimpiarValidaciones() {
    $("#errApellido").css("display", "none");
    $("#errNombres").css("display", "none");
    $("#errPerfilId").css("display", "none");
    $("#errEmpleadorACargo").css("display", "none");
    $("#errIdActiveDirectory").css("display", "none");
    //$("#errGrupoDeCompras").css("display", "none");
    $("#errAdministrador").css("display", "none");
}

function HabilitarInicio() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function HabilitarAgregar() {
    var grid = $("#gridIniComercial").data("kendoGrid");

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
    var result = MSExecuteURLOnServer('/Comercial/Cancelar');

    if (result != null) {
        viewModel.set("isModifyDisabled", false);
        HabilitarAgregar();
        UpdateViewModel(result);
        LimpiarValidaciones();
    }
}

function Modificar() {
    var grid = $("#gridIniComercial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    if (data == null) {
        return;
    }

    var param = {
        "ComercialId": data.ComercialId,
    };

    if (data.ComercialId > 0) {
        var datosComerciales = MSExecuteOnServer('/Comercial/ComercialCombo', param);

        if (datosComerciales != null) {
            if (ExistsErrorMessages(datosComerciales.Errores)) {
                ShowTooltipMessages("err", datosComerciales.Errores);
            }
            else {
                viewModel.set("ComercialCombo", datosComerciales.Comercial);
            }
        }
    }

    var result = MSExecuteOnServer('/Comercial/Aplicar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            viewModel.set("isModifyDisabled", true);
            HabilitarEdicion();
            LimpiarValidaciones();
            UpdateViewModel(result);

            var idPerfil = $("#PerfilId").val();

            if (idPerfil == 4 || idPerfil == 5) {
                //var EsAdmin = $("#Administrador");
                $("#Administrador").prop("checked", false);
                $("#Administrador").prop("disabled", true);
                viewModel.set("Comercial.Administrador", false);

                $("#EmpleadorACargo").data("kendoDropDownList").text("");
                //$("#EmpleadorACargo").attr("disabled", true);
                var empleadoACargoDDList = $("#EmpleadorACargo").data("kendoDropDownList");
                empleadoACargoDDList.enable(false);
                //$("#EmpleadorACargo").enable(false);
                viewModel.Comercial.EmpleadorACargo = null;
            }
            else {
                var empleadoACargoDDList = $("#EmpleadorACargo").data("kendoDropDownList");
                empleadoACargoDDList.enable(true);
                $("#Administrador").prop("disabled", false);
            }
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
    var grid = $("#gridIniComercial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var param = {
        "ComercialId": data.ComercialId,
    };

    var result = MSExecuteOnServer('/Comercial/Eliminar', param);

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
    var grid = $("#gridIniComercial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();

    var datos = {
        "ObjectState": objectstate,
        "ComercialId": viewModel.get("Comercial.ComercialId"),
        "Apellido": viewModel.get("Comercial.Apellido"),
        "Nombres": viewModel.get("Comercial.Nombres"),
        "PerfilId": GetDropDownValue(viewModel, "Comercial.PerfilId.PerfilId"),
        "EmpleadorACargo": GetDropDownValue(viewModel, "Comercial.EmpleadorACargo.ComercialId"),
        "IdActiveDirectory": viewModel.get("Comercial.IdActiveDirectory"),
        //"GrupoDeCompras": GetDropDownValue(viewModel, "Comercial.GrupoDeCompras.Id"),
        "Administrador": viewModel.get("Comercial.Administrador"),
    };

    var result = MSExecuteOnServer('/Comercial/Grabar', datos);

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