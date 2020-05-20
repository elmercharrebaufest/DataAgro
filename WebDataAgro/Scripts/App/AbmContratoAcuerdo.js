var viewModel;
var datosIniAbmContratoAcuerdo;
kendo.culture("es-AR");
var modifica;
var elimina;

$(document).ready(function () {
    modifica = ConvertirStringABool(modifica);
    elimina = ConvertirStringABool(elimina);
    InicializarElementos();
    InicializarDatos();
    InicializarDate();
    CreateGridContratoAcuerdo();
    CrearViewModel();
    AsignarBotones();
    InicializarBusquedaInicial();

});

function InicializarElementos() {
    
    $("#butEliminar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Eliminar.png")
    });

    $("#butConfirmar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Aceptar.png")
    });

    $("#butAceptar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Aceptar.png")
    });

    $("#butCancelar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Cancelar.png")
    });

    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
    });

    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#buscadorProveedor").val().split('|').length > 1) {
                $("#buscadorProveedor").val($("#buscadorProveedor").val().split('|')[1]);
            }
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarProveedoresConCorredor"
                },
                parameterMap: function (data, type) {
                    var cuitAux = $("#buscadorCorredor").val().split('(');
                    if (cuitAux[1] != null) {
                        var cuit = cuitAux[1].split(')');
                    }
                    else {
                        cuit = cuitAux;
                    }
                    return { filtro: cuit[0], filtroProveedor: $('#buscadorProveedor').val(), corredor: false };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

    $("#buscadorCorredor").click(function () {
        $("#buscadorCorredor").data("kendoAutoComplete").value("");
        $("#buscadorCorredor").data("kendoAutoComplete").trigger("change");
    });
    $("#buscadorCorredor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#buscadorCorredor").val().split('|').length > 1) {
                $("#buscadorCorredor").val($("#buscadorCorredor").val().split('|')[1]);
            }
        },
        select: function (e) {
            $("#buscadorProveedor").val("");
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarCorredores"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#buscadorCorredor').val(), corredor: true };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }

    });
       
    $("#comercialId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId"
    });

    $("#comercialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#destinoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#destinoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinolId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });


    $("#material").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId",
        change: function () {

            $("#contratoId").val("");

        }
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#moneda").kendoDropDownList({
        optionLabel: "SELECCIONE UNA MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId",
        change: function () {

            $("#contratoId").val("");

        }
    });

    $("#moneda").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#moneda").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#Cantidad").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        change: function () {
            if ($("#cargarCantidadCamiones").is(':checked')) {
                $("#cantidadCamionesId").data("kendoNumericTextBox").value(Math.ceil(this.value() / 30000));
            }
        }
    });

    $("#Precio").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
}

function InicializarDate() {
    kendo.culture("es-AR");
    var date = ObtenerFechaDesde();
    var datehasta = ObtenerFechaHasta();

    $("#fechaHasta").kendoDatePicker({
        value: datehasta,
        format: "dd/MM/yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesde").kendoDatePicker({
        value: date,
        format: "dd/MM/yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            $("#fechaHasta").val(ObtenerFechaHasta(this.value()));
        }
    });

}
function ObtenerFechaDesde(fechaBase) {
    var hoy = fechaBase != undefined ? fechaBase : new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

function ObtenerFechaHasta(fechaBase) {
    var hoy = fechaBase != undefined ? fechaBase : new Date();
    var anio = hoy.getFullYear();
    var mesPost = hoy.getMonth() + 2;
    var dia = hoy.getDate();
    var ultimoDia = new Date(anio, hoy.getMonth() + 1, 0).getDate();

    if (dia === 1) {
        dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
        mesPost = hoy.getMonth() + 1;
    }
    if (dia === ultimoDia || (mesPost === 2 && dia >= 29)) {
        dia = new Date(anio, mesPost, 0).getDate();
    }
    if (mesPost === 13) {
        mesPost = 1;
        anio += 1;
    }
    if (mesPost < 10) {
        mesPost = "0" + mesPost.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '/' + mesPost + '/' + anio;
}


function InicializarDatos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniAbmContratoAcuerdo = data;
            AsignarDatos();
        }
    };
    MSExecuteURLOnServerAsync('/ContratoAcuerdo/InicializarContratoAcuerdo', funcReturn, '');
}

function AsignarDatos() {
    viewModel.set("MaterialCombo", datosIniAbmContratoAcuerdo.Datos.material);
    viewModel.set("ComercialCombo", datosIniAbmContratoAcuerdo.Datos.comercial);
    viewModel.set("DestinoCombo", datosIniAbmContratoAcuerdo.Datos.destino);
    viewModel.set("MonedaCombo", datosIniAbmContratoAcuerdo.Datos.moneda);
}

function ObtenerFechaFormato() {
    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return anio + '-' + mes + '-' + dia;
}

function CrearResultadosDataSource(datos) {
    var defaultFilter = { field: "Fecha", operator: "eq", value: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()) };
    var ds = new kendo.data.DataSource({
        data: datos,
        pageSize: 10,
        schema: {
            model: {
                id: 'Id',
                fields: {
                    Corredor: { type: "string", editable: false },
                    Proveedor: { type: "string", editable: false },
                    Cantidad: { type: "number", editable: false },
                    Precio: { type: "number", editable: false },
                    Moneda: { type: "string", editable: false },
                    Material: { type: "string", editable: false },
                    Comercial: { type: "string", editable: false },
                    Destino: { type: "string", editable: false },
                    Fecha: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" }
                }
            }
        },
        sort: [{ field: "Fecha", dir: "desc" }],

        filter: defaultFilter
    });
    return ds;
}

function CreateGridContratoAcuerdo() {
    $("#gridIni").kendoGrid({
        columns: [
            { selectable: true, width: "50px" },
            {
                field: "Id", title: "Acuerdo",
                template: function (dataItem) {
                    if (dataItem.EstadoId == 1) {
                        return '<div class="statuspendiente "></div>' + dataItem.Id;
                    } else if (dataItem.EstadoId == 2) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Id;
                    }
                }
            },
            { field: "Corredor", title: "Corredor" },
            { field: "Proveedor", title: "Proveedor" },
            {
                field: "Cantidad", title: "Cantidad",
                template: function (dataItem) {
                    return kendo.toString(dataItem.Cantidad, "n0");
                }
            },
            {
                field: "Precio", title: "Precio",
                template: function (dataItem) {
                    return kendo.toString(dataItem.Precio, "n2") + " " + dataItem.Moneda;
                }
            },
            //{ field: "Moneda", title: "Moneda" },
            { field: "Material", title: "Material" },
            { field: "Comercial", title: "Comercial" },
            { field: "Destino", title: "Destino" },
            { field: "Fecha", type: "date", title: "Fecha", format: _DefaultDateTemplate },
            { field: "FechaDesde", type: "date", title: "F. E. Desde", format: _DefaultDateTemplate },
            { field: "FechaHasta", type: "date", title: "F. E. Hasta", format: _DefaultDateTemplate },
            { field: "PorcentajeCargado", title: "Cargado (%)", filterable: false },
            {
                field: "Estado", title: "Estado",
                itemTemplate: function (e) {
                    return "<span><label><span>#= data.Estado || data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Estado#'/></label></span>";
                },

                template: function (dataItem) {
                    if (dataItem.EstadoId == 1 ) {
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonPendiente(dataItem, 'fa-pencil pend') +
                            botonConfirmadoTilde(dataItem, 'fa-check pend') +
                            botonBorrar(dataItem, 'fa-trash pend');
                    } 
                    if (dataItem.EstadoId == 2) {
                        return '<div class="status confirmado">Confirmado</div>' +
                            botonPendiente(dataItem, 'fa-pencil conf') +
                            botonBorrar(dataItem, 'fa-trash conf');
                    }
                }
            }
        ],

        sortable: true,
        change: onChangeGridInicial,
        scrollable: false,
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
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
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
        },
        pageable: true,
        dataBound: function () {
            $("td:has(div.statuspendiente)").attr('id', 'border-orange');
            $("td:has(div.statusconfirmado)").attr('id', 'border-green');
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
        MaterialCombo: [],
        MonedaCombo: [],
        ComercialCombo: [],
        DestinoCombo: [],
        isReadOnly: true,
        isFilterDisabled: true,
        isControlDisabled: false,
        isModifyDisabled: false,
        isAddNewDisabled: true,
        isDeleteDisabled: true,
        ContratoAcuerdo: null
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

    MSExecuteURLOnServerAsync('/ContratoAcuerdo/Buscar', funcReturn, '');
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

    $("#butConfirmar").click(function () {
        ConfirmarMasivo();
    });
}

function UpdateViewModel(model) {
    if (model.ContratoAcuerdo.ObjectState == 0) {
        viewModel.set("isDeleteDisabled", true);
    }
    else {
        viewModel.set("isDeleteDisabled", false);
    }

    var ContratoAcuerdo = {
        "Id": model.ContratoAcuerdo.Id
    };

    viewModel.set("ContratoAcuerdo", ContratoAcuerdo);
}

function LimpiarValidaciones() {
    //completar
    $("#errDescripcion").css("display", "none");
    $("#errCodigoSap").css("display", "none");
}

function HabilitarCancelar() {
    viewModel.set("isFilterDisabled", false);
    viewModel.set("isAddNewDisabled", false);
    viewModel.set("isDeleteDisabled", true);
}

function Agregar() {
    var result = MSExecuteURLOnServer('/ContratoAcuerdo/Cancelar');

    if (result.HayError) {
        MensErr(result.Errores[0].Message);
    } else {
        viewModel.ContratoAcuerdo = null;
        $('#buscadorCorredor').val("");
        $("#buscadorCorredor").trigger("change");
        $('#buscadorProveedor').val("");
        $("#buscadorProveedor").trigger("change");
        $("#material").data("kendoDropDownList").value(3);
        $("#moneda").data("kendoDropDownList").value("ARP  ");
        $("#comercialId").data("kendoDropDownList").value(comercialId);
        $("#destinoId").data("kendoDropDownList").value(1);
        $('#Precio').val();
        $('#Cantidad').val();
        InicializarDate();
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
        var datosContratoAcuerdo = MSExecuteOnServer('/ContratoAcuerdo/ContratoAcuerdoCombo', param);

        if (datosContratoAcuerdo != null) {
            if (ExistsErrorMessages(datosContratoAcuerdo.Errores)) {
                ShowTooltipMessages("err", datosContratoAcuerdo.Errores);
            }
            else {
                viewModel.set("ContratoAcuerdo", datosContratoAcuerdo.ContratoAcuerdo);
            }
        }
    }

    var result = MSExecuteOnServer('/ContratoAcuerdo/Aplicar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {

            $('#buscadorProveedor').val(viewModel.ContratoAcuerdo.Proveedor);
            $("#buscadorProveedor").trigger("change");
            $("#material").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.MaterialId);
            $("#moneda").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.MonedaId);
            $("#destinoId").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.DestinoId);
            $("#comercialId").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.ComercialId);
            $('#Precio').val(viewModel.ContratoAcuerdo.Precio);
            $('#Cantidad').val(viewModel.ContratoAcuerdo.Cantidad);
            $("#fechaHasta").val(viewModel.ContratoAcuerdo.FechaModificacion);
            viewModel.set("isModifyDisabled", true);
            HabilitarEdicion();
            LimpiarValidaciones();
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

    var result = MSExecuteOnServer('/ContratoAcuerdo/Eliminar', param);

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

    if ($("#buscadorProveedor").val() != "") {
        var cuitAux = $("#buscadorProveedor").val().split('(');
        var proveedorId;
        if (cuitAux[1]) {
            var cuit = cuitAux[1].split(')');
            proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0], corredor: false });
        } else {
            proveedorId = -1;
        }
    }

    var corredorId = -1;
    if ($("#buscadorCorredor").val() != "") {
        var cuitAuxCorredor = $("#buscadorCorredor").val().split('(');
        if (cuitAuxCorredor[1]) {
            var cuitCorredor = cuitAuxCorredor[1].split(')');
            corredorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuitCorredor[0], corredor: true });
        }
    }
    if (proveedorId == null || proveedorId == 0) {
        MensErr("No se pudo obtener el proveedor, verificar la segmentación.");
        $.unblockUI();
        return;
    }
    var datos = {
        "ObjectState": objectstate,
        "Id": viewModel.get("ContratoAcuerdo.Id"),
        "ProveedorId": proveedorId,
        "CorredorId": corredorId,
        "Precio": viewModel.get("ContratoAcuerdo.Precio"),
        "Cantidad": viewModel.get("ContratoAcuerdo.Cantidad"),
        "MaterialId": $("#material").val(),
        "MonedaId": $("#moneda").val(),
        "ComercialCreadorId": $("#comercialId").val(),
        "DestinoId": $("#destinoId").val(),
        "FechaHasta": new Date($("#fechaHasta").val().toString().split('/')[2], parseInt($("#fechaHasta").val().toString().split('/')[1], 10) - 1, $("#fechaHasta").val().toString().split('/')[0]),
        "FechaDesde": new Date($("#fechaDesde").val().toString().split('/')[2], parseInt($("#fechaDesde").val().toString().split('/')[1], 10) - 1, $("#fechaDesde").val().toString().split('/')[0]),
    };

    var result = MSExecuteOnServer('/ContratoAcuerdo/Grabar', datos);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            //UpdateViewModel(result);
            InicializarBusquedaInicial();
            HabilitarInicio();
        }
    }
}

function Cancelar() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function botonPendiente(dataItem, icono) {
    if (modifica) {
        return '<button data-toggle="tooltip" title="Editar" onclick="ModificarPorId(' + dataItem.Id + ')"><i class="fa ' + icono + '"></i></button>';
    } else {
        return '<div></div>';
    }
}


function botonConfirmadoTilde(dataItem, icono) {

    return '<button data-toggle="tooltip" title="Confirmar" onclick="ConfirmarConId(' + dataItem.Id + ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
}




function botonBorrar(dataItem, icono) {
    if (elimina) {
        return '<button data-toggle="tooltip" title="Rechazar" onclick="EliminarConId(' + dataItem.Id + ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return '<div></div>';
    }
}

function ModificarPorId(id) {
    var param = {
        "Id": id
    };

    if (id > 0) {
        var datosContratoAcuerdo = MSExecuteOnServer('/ContratoAcuerdo/ContratoAcuerdoCombo', param);

        if (datosContratoAcuerdo != null) {
            if (ExistsErrorMessages(datosContratoAcuerdo.Errores)) {
                ShowTooltipMessages("err", datosContratoAcuerdo.Errores);
            }
            else {
                viewModel.set("ContratoAcuerdo", datosContratoAcuerdo.ContratoAcuerdo);
            }
        }
    }

    var result = MSExecuteOnServer('/ContratoAcuerdo/Aplicar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            console.log(viewModel.ContratoAcuerdo);
            $('#buscadorProveedor').val(viewModel.ContratoAcuerdo.Proveedor);
            $("#buscadorProveedor").trigger("change");
            $('#buscadorCorredor').val(viewModel.ContratoAcuerdo.Corredor);
            $("#buscadorCorredor").trigger("change");
            $("#material").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.MaterialId);
            $("#moneda").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.MonedaId);
            $("#destinoId").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.DestinoId);
            $("#comercialId").data("kendoDropDownList").value(viewModel.ContratoAcuerdo.ComercialId);
            $('#Precio').val(viewModel.ContratoAcuerdo.Precio);
            $('#Cantidad').val(viewModel.ContratoAcuerdo.Cantidad);
            $("#fechaDesde").val(viewModel.ContratoAcuerdo.FechaModificacionDesde);
            $("#fechaHasta").val(viewModel.ContratoAcuerdo.FechaModificacion);
            viewModel.set("isModifyDisabled", true);
            HabilitarEdicion();
            LimpiarValidaciones();
        }
    }
}


function EliminarConId(id) {
    Confirma('¿ Confirma la eliminación de este registro ?',
        function (dialogItself) {
            EjecutarEliminarConId(id);
            dialogItself.close();
        });
}

function EjecutarEliminarConId(id) {
    var param = {
        "Id": id
    };

    var result = MSExecuteOnServer('/ContratoAcuerdo/Eliminar', param);

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



function ConfirmarConId(id) {
    Confirma('¿ Esta seguro de confirmar este contrato ?',
        function (dialogItself) {
            EjecutarConfirmarConId(id);
            dialogItself.close();
        });
}

function EjecutarConfirmarConId(id) {
    var param = {
        "Id": id
    };

    var result = MSExecuteOnServer('/ContratoAcuerdo/Confirmar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            recargarGrilla();
        }
    }
}


function recargarGrilla() {
    InicializarBusquedaInicial();
    $('#gridIni').data('kendoGrid').dataSource.read();
}

function ConfirmarMasivo() {
    Confirma('¿ Esta seguro de confirmar los contratos seleccionados ?',
        function (dialogItself) {
            EjecutarConfirmarMasivo();
            dialogItself.close();
        });
}

function EjecutarConfirmarMasivo() {
    var negocios = SeleccionarElementos();
    $(".loader").show();
    var result = null;
    for (var i in negocios) {
        var objConfirmado = {};
        $("#estado" + i).empty();
        var param = {
            "Id": negocios[i].Id
        };
        result = MSExecuteOnServer('/ContratoAcuerdo/Confirmar', param);
    }
    finalizacionCallBack(i, result);
}


function SeleccionarElementos() {
    var grid = $("#gridIni").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        obj.push(selectedItem);
    });
    return obj;
}



function finalizacionCallBack(i, result) {
    if (result !== null && result.Errores !== null && ExistsErrorMessages(result.Errores)) {
        $("#estado" + i).removeClass("loader");
        $("#estado" + i).append('<img src="/Content/Images/Cancelar.png" alt="error" title="' + result.Errores[0].Message + '" />');
        $("#error" + i).append(result.Errores[0].Message);
    } else {
        $("#estado" + i).removeClass("loader");
        $("#estado" + i).append('<img src="/Content/Images/Aceptar.png" alt="Confirmado"/>');
        recargarGrilla();
    }
}