var viewModelProveedor;
function Rol(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
    
}

function Comercial(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.ComercialId = id;
        this.NompreCompleto = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.ComercialId = id.ComercialId;
        this.NompreCompleto = id.NompreCompleto;
    }
    this.removeComercial = function () {
        console.log(this);
        viewModel.ComercialesSeleccionados.remove(this);

        var select = $("#ComercialId").data("kendoDropDownList");
        for (var i = 0; i <= select.dataSource.data().length; i++) {
            var option = $("#ComercialId").data("kendoDropDownList").dataItem(i);
            if (option.ComercialId == this.ComercialId) {
                option.set("Disabled", false);
            }
        }
    };
}
//var datosIniAbmCentro;

$(document).ready(function () {
    InicializarElementos();

    CrearViewModel();
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#ComercialId").kendoDropDownList({
        dataTextField: "NombreCompleto",
        dataValueField: "ComercialId",
        optionLabel: "Seleccione el Comercial...",
        select: function (e) {
            if (e.dataItem.Disabled) {
                e.preventDefault();
            }
        },
        template: kendo.template($("#templateComerciales").html())

    });

    $("#ComercialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            $("#ComercialId").data("kendoDropDownList").text("");
        }
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
        select: function (e) {
            if (e.dataItem.Id != 0) {
                $("#buscadorResult").val(e.dataItem.Id);
                $("#datos-proveedor").show();
                $("#comercial-proveedor").show();
                $("#divBotones").show();
                
                CargarViewModel(e.dataItem.Id);
            } 
        },        
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarProveedores"
                },
                parameterMap: function (data, type) {
                   
                    return {  filtroProveedor: $('#buscadorProveedor').val() };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
    $("#buscadorProveedor").click(function () {
        LimpiarViewModel();
    });
    $("#butAceptar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Aceptar.png")
    });
    $("#butAceptar").click(Grabar);
    $("#butCancelar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Cancelar.png")
    });
    $("#butCancelar").click(LimpiarViewModel);
}

function CrearViewModel() {

    var data = MSExecuteURLOnServer('/AdministracionProveedor/Inicializar');
    
    viewModel = kendo.observable({

        RolCombo: [],
        RolesSeleccionados: [],

        ComercialCombo: [],
        ComercialesSeleccionados: [],

        addComercial: function () {
            if ($('#ComercialId option:selected').text() != "Seleccione el Comercial...") {
                this.ComercialesSeleccionados.push(new Comercial($('#ComercialId option:selected').val(), $('#ComercialId option:selected').text()));

                var option = $("#ComercialId").data("kendoDropDownList").dataItem();
                option.set("Disabled", true);

                $("#ComercialId").data("kendoDropDownList").value("");
            }
        }
    });
    kendo.bind($("#proveedor-abm"), viewModel);

    viewModel.set("RolCombo", data.roles);
    viewModel.set("ComercialCombo", data.comerciales);
}
function LimpiarViewModel() {
    $("#buscadorProveedor").val("");
    $("#datos-proveedor").hide();
    $("#comercial-proveedor").hide();
    $("#divBotones").hide();
    $("#buscadorResult").val("");

    $("#ComercialId").data("kendoDropDownList").value("");
    var select = $("#ComercialId").data("kendoDropDownList");
    for (var i = 0; i <= select.dataSource.data().length; i++) {
        $("#ComercialId").data("kendoDropDownList").dataItem(i).set("Disabled", false);
    }
    viewModel.set("RolesSeleccionados", []);
    viewModel.set("ComercialesSeleccionados", []);
}

function Grabar() {
    var result = MSExecuteOnServer('/AdministracionProveedor/GrabarProveedor', { id: $("#buscadorResult").val(), roles: viewModel.RolesSeleccionados, comerciales: viewModel.ComercialesSeleccionados });

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            MensInfo("Grabación Realizada Correctamente");
            LimpiarViewModel();
        }
    }
}
function CargarViewModel(id) {
    var data = MSExecuteOnServer('/AdministracionProveedor/TraerDatosProveedor', { id });

    if (data.comerciales) {
        for (var i = 0; i < data.comerciales.length; i++) {
            viewModel.ComercialesSeleccionados.push(new Comercial(data.comerciales[i].ComercialId, data.comerciales[i].NombreCompleto));

            var select = $("#ComercialId").data("kendoDropDownList");
            for (var j = 1; j <= select.dataSource.data().length; j++) {
                var option = $("#ComercialId").data("kendoDropDownList").dataItem(j);
                if (option.ComercialId == data.comerciales[i].ComercialId) {
                    option.set("Disabled", true);
                }
            }
        }
    }
}


function descargaReporteProveedorComerciales() {
    window.location = '/AdministracionProveedor/ReporteProveedorComerciales?IdProveedor=' + $("#buscadorResult").val();
}
