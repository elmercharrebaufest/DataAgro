var viewModel;
function Rol(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
    this.removeRol = function () {
        viewModel.RolesSeleccionados.remove(this);

        var select = $("#RolId").data("kendoDropDownList");
        for (var i = 0; i <= select.dataSource.data().length; i++) {
            var option = $("#RolId").data("kendoDropDownList").dataItem(i);
            if (option.Id == this.Id) {
                option.set("Disabled", false);
            }
        }
    }
}
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
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#PerfilId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "PerfilId",
        
    });

    $("#PerfilId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#PerfilId").data("kendoDropDownList").text("");
        }
    });



    $("#EmpleadorACargo").kendoDropDownList({
        dataTextField: "Apellido",
        dataValueField: "ComercialId",
    });

    $("#EmpleadorACargo").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#EmpleadorACargo").data("kendoDropDownList").text("");
        }
    });

    $("#RolId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        optionLabel: "Seleccione el Rol...",
        select: function (e) {
            if (e.dataItem.Disabled) {
                e.preventDefault();
            }
        },
        template: kendo.template($("#template").html())

    });

    $("#RolId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#RolId").data("kendoDropDownList").text("");
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

function CrearResultadosDataSource(datos) {
    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    ComercialId: { type: "number", editable: false },
                    Apellido: { type: "string", editable: false },
                    Nombres: { type: "string", editable: false },
                    Roles: { type: "string", editable: false },
                    Deshabilitado: { type: "boolean", editable: false },
                    AsignarNegocios: { type: "boolean", editable: false },
                    FechaDeshabilitado: { type: "date", editable: false },
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
            { field: "Rol", title: "Roles", attributes: { style: 'white-space: nowrap ' }},
            { field: "Deshabilitado", title: "Deshabilitado", template: "#if(Deshabilitado){#Si (#=kendo.toString(kendo.parseDate(FechaDeshabilitado, 'yyyy-MM-dd hh:mm:sss'), 'MM/dd/yyyy HH:mm:ss')#) #}else{}# " },
            { field: "AsignarNegocios", title: "Asignar Negocios", template: "#if(AsignarNegocios){#Si#}else{#No#}# " },
            { filed: "Equipo", title: "Equipo", template: "#= Equipo.map(a => a.Apellido).join(', ') #" }
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
    $("#gridIniComercial").data("kendoGrid").hideColumn(5);
    $("#gridIniComercial").kendoTooltip({
        filter: "td:nth-child(3)",
        position: "top",
        content: function (e) {
            var dataItem = $("#gridIniComercial").data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = dataItem.Rol;

            if (content.length > 30) {
                return content;
            } else {
                return "";
            }
        },
        show: function (e) {
            if (this.content.text() != "") {
                $('[role="tooltip"]').css("visibility", "visible");
            }
        },
        hide: function () {
            $('[role="tooltip"]').css("visibility", "hidden");
        }
    }).data("kendoTooltip");

    $("#gridIniComercial").kendoTooltip({
        filter: "td:nth-child(5)",
        position: "top",
        content: function (e) {
            var dataItem = $("#gridIniComercial").data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = dataItem.Equipo.map(a => a.Apellido);
            content = content.join(', ');
            if (content.length > 30) {
                return content;
            } else {
                return "";
            }
        },
        show: function (e) {
            if (this.content.text() != "") {
                $('[role="tooltip"]').css("visibility", "visible");
            }
        },
        hide: function () {
            $('[role="tooltip"]').css("visibility", "hidden");
        }
    }).data("kendoTooltip");
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
        isModifyDisabled: false,
        isAddNewDisabled: true,
        isDeleteDisabled: true,

        PerfilCombo: [],
        ComercialCombo: [],
        RolCombo: [],
        RolesSeleccionados: [],
        Comercial: null,

        addRol: function () {
            if ($('#RolId option:selected').text() != "Seleccione el Rol...") {
                this.RolesSeleccionados.push(new Rol($('#RolId option:selected').val(), $('#RolId option:selected').text()));
                
                var option = $("#RolId").data("kendoDropDownList").dataItem();
                option.set("Disabled", true);

                //.Disabled = true;

                $("#RolId").data("kendoDropDownList").value("");
            }
        }
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
    viewModel.set("RolCombo", datosIniAbmCentro.Datos.Rol);
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
        "IdUsuarioSAP": model.Comercial.IdUsuarioSAP,
        "Administrador": model.Comercial.Administrador,
        "Cupera": model.Comercial.Cupera,        
        "Deshabilitado": model.Comercial.Deshabilitado,
        "AsignarNegocios": model.Comercial.AsignarNegocios,
        "FechaDeshabilitado": model.Comercial.FechaDeshabilitado,
    };
    
    viewModel.set("Comercial", comercial);
    viewModel.set("RolesSeleccionados", []);
    viewModel.Comercial.PerfilId = $("#PerfilId").data("kendoDropDownList").dataItem();
    viewModel.Comercial.EmpleadorACargo = $("#EmpleadorACargo").data("kendoDropDownList").dataItem();

    var select = $("#RolId").data("kendoDropDownList");
    
    if (model.Comercial.RolesAsociados != null) {
        for (var i = 0; i < model.Comercial.RolesAsociados.length; i++) {
            viewModel.RolesSeleccionados.push(new Rol(model.Comercial.RolesAsociados[i].Id, model.Comercial.RolesAsociados[i].Descripcion));

            for (var j = 1; j <= select.dataSource.data().length; j++) {
                var option = $("#RolId").data("kendoDropDownList").dataItem(j);
                if (option.Id == model.Comercial.RolesAsociados[i].Id) {
                    option.set("Disabled", true);
                }
            }
        }
    } else {
        for (var j = 1; j <= select.dataSource.data().length; j++) {
            var option = $("#RolId").data("kendoDropDownList").dataItem(j);
            option.set("Disabled", false);
        }
    }
}

function LimpiarValidaciones() {
    $("#errApellido").css("display", "none");
    $("#errNombres").css("display", "none");
    $("#errDeshabilitado").css("display", "none");
    $("#errAsignarNegocios").css("display", "none");    
    $("#errPerfilId").css("display", "none");
    $("#errEmpleadorACargo").css("display", "none");
    $("#errIdActiveDirectory").css("display", "none");
    $("#errIdUsuarioSAP").css("display", "none");
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
        "Deshabilitado": viewModel.get("Comercial.Deshabilitado"),
        "FechaDeshabilitado": new Date(),
        "AsignarNegocios": viewModel.get("Comercial.AsignarNegocios"),
        "PerfilId": GetDropDownValue(viewModel, "Comercial.PerfilId.PerfilId"),
        "EmpleadorACargoId": GetDropDownValue(viewModel, "Comercial.EmpleadorACargo.ComercialId"),
        "IdActiveDirectory": viewModel.get("Comercial.IdActiveDirectory"),
        "IdUsuarioSAP": viewModel.get("Comercial.IdUsuarioSAP"),
        "Administrador": viewModel.get("Comercial.Administrador"),
        "Cupera": viewModel.get("Comercial.Cupera")
        
    };

    var result = MSExecuteOnServer('/Comercial/Grabar', { oComercial: datos, roles: viewModel.RolesSeleccionados });

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            //ShowTooltipMessages("err", result.Errores);
            MensErr(result.Errores[0].Message);
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