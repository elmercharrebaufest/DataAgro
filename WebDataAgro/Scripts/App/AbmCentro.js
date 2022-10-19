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

    InicializarBusquedaInicial();

    AutocompleteProcedencia();
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
                    CodigoSap: { type: "string", editable: false },
                    Localidad: { type: "string", editable: false },
                    LocalidadId: { type: "string", editable: false },
                    Direccion: { type: "string", editable: false },
                    CodigoPostal: { type: "string", editable: false },
                    CargaNegocios: { type: "boolean", editable: false },
                    CargaCupos: { type: "boolean", editable: false },
                }
            }
        },
    });

    return ds;
}

function CreateGridCentro() {
    $("#gridIni").kendoGrid({
        columns: [
            { field: "Descripcion", title: "Centro", filterable: false },
            { field: "CodigoSap", title: "Codigo SAP", filterable: false },
            { field: "Localidad", title: "Localidad", filterable: false },
            { field: "Orden", title: "Orden", filterable: false, template: "# if(Orden == null){##}else{ # #: Orden# # }#" },
            { field: "Acopio", title: "Acopio", filterable: false, template: "# if(Acopio){#Si#}else{##}#" },
            { field: "ValidaRedespacho", title: "ValidaRedespacho", filterable: false, template: "# if(ValidaRedespacho){#Si#}else{##}#" },
            { field: "Comision", title: "Comision", filterable: false, template: "# if(Comision){#Si#}else{##}#" },
            { field: "CargaNegocios", title: "Carga Negocios", filterable: false, template: "# if(CargaNegocios){#Si#}else{##}#" },
            { field: "CargaCupos", title: "Carga Cupos", filterable: false, template: "# if(CargaCupos){#Si#}else{##}#" },
            { field: "NoPropio", title: "No Propio", filterable: false, template: "# if(NoPropio){#Si#}else{##}#" },
            { field: "CUIT", title: "CUIT", filterable: false, template: "# if(CUIT == null){##}else{ # #: CUIT# # }#" },
            { field: "RazonSocial", title: "Razón Social", filterable: false, template: "# if(RazonSocial == null){##}else{ # #: RazonSocial# # }#" },
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
                    contains: "Contiene",
                    eq: "Igual",
                    neq: "Distinto",
                    startswith: "Comienza con",
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

        Centro: null,
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

    MSExecuteURLOnServerAsync('/Centro/Buscar', funcReturn, '');
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
    if (model.Centro.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var centro = {
        "Id": model.Centro.Id,
        "Descripcion": model.Centro.Descripcion,
        "NoPropio": model.Centro.NoPropio,
        "CodigoSap": model.Centro.CodigoSap,
        "Acopio": model.Centro.Acopio,
        "ValidaRedespacho": model.Centro.ValidaRedespacho,
        "LocalidadId": model.Centro.LocalidadId,
        "Localidad": model.Centro.Localidad,
        "CodigoPostal": model.Centro.CodigoPostal,
        "Direccion": model.Centro.Direccion,
        "Comision": model.Centro.Comision,
        "CargaNegocios": model.Centro.CargaNegocios,
        "CargaCupos": model.Centro.CargaCupos,
        "Orden": model.Centro.Orden,
        "CUIT": model.Centro.CUIT,
        "RazonSocial": model.Centro.RazonSocial,
    };

    viewModel.set("Centro", centro);
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
    var result = MSExecuteURLOnServer('/Centro/Cancelar');

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

    var param = {
        "Id": data.Id,
    };

    if (data.Id > 0) {
            var datosCentro = MSExecuteOnServer('/Centro/CentroCombo', param);

        if (datosCentro != null) {
            if (ExistsErrorMessages(datosCentro.Errores)) {
                ShowTooltipMessages("err", datosCentro.Errores);
            }
            else {
                viewModel.set("Centro", datosCentro.Centro);
            }
        }
    }

    var result = MSExecuteOnServer('/Centro/Aplicar', param);

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
    var grid = $("#gridIni").data("kendoGrid");
    var row = grid.select();
    var data = grid.dataItem(row);
    if (data == null) return;

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
        "Id": data.Id,
    };

    var result = MSExecuteOnServer('/Centro/Eliminar', param);

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
        "Id": viewModel.get("Centro.Id"),
        "Descripcion": viewModel.get("Centro.Descripcion"),
        "CodigoSap": viewModel.get("Centro.CodigoSap"),
        "NoPropio": viewModel.get("Centro.NoPropio"),
        "Acopio": viewModel.get("Centro.Acopio"),
        "ValidaRedespacho": viewModel.get("Centro.ValidaRedespacho"),
        "LocalidadId": viewModel.get("Centro.LocalidadId"),
        "CodigoPostal": viewModel.get("Centro.CodigoPostal"),
        "Direccion": viewModel.get("Centro.Direccion"),
        "Comision": viewModel.get("Centro.Comision"),
        "CargaNegocios": viewModel.get("Centro.CargaNegocios"),
        "CargaCupos": viewModel.get("Centro.CargaCupos"),
        "Orden": viewModel.get("Centro.Orden"),
        "CUIT": viewModel.get("Centro.CUIT"),
        "RazonSocial": viewModel.get("Centro.RazonSocial"),
    };

    var result = MSExecuteOnServer('/Centro/Grabar', datos);

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

function AutocompleteProcedencia() {
    //$("#LocalidadId").click(function () {
    //    $("#LocalidadCrearContrato").data("kendoAutoComplete").value("");
    //    $("#establecimientoDiv").hide();
    //    $("#LocalidadCrearContrato").trigger("change");
    //    $("#establecimientoPropioId").prop("checked", false);
    //    $("#establecimientoArrendadoId").prop("checked", false);
    //    DatosProveedor();
    //});

    $("#Localidad").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function () {
            if ($("#Localidad").val().split('|').length > 1) {
                $("#Localidad").val($("#Localidad").val().split('|')[1]);
                //HabilitarEstablecimiento();
            }

        },
        select: function (e) {           
            viewModel.set("Centro.LocalidadId", e.dataItem.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#Localidad').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}