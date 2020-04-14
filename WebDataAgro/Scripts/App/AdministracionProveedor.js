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
    this.removeRol = function () {
        viewModel.RolesSeleccionados.remove(this);

        var select = $("#RolId").data("kendoDropDownList");
        for (var i = 0; i <= select.dataSource.data().length; i++) {
            var option = $("#RolId").data("kendoDropDownList").dataItem(i);
            if (option.Id == this.Id) {
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
                   
                    return { filtroProveedor: $('#buscadorProveedor').val() };
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

    var roles = MSExecuteURLOnServer('/AdministracionProveedor/Inicializar');
    
    viewModel = kendo.observable({

        RolCombo: [],
        RolesSeleccionados: [],

        addRol: function () {
            if ($('#RolId option:selected').text() != "Seleccione el Rol...") {
                this.RolesSeleccionados.push(new Rol($('#RolId option:selected').val(), $('#RolId option:selected').text()));
                
                var option = $("#RolId").data("kendoDropDownList").dataItem();
                option.set("Disabled", true);

                $("#RolId").data("kendoDropDownList").value("");
            }
        }
    });
    kendo.bind($("#proveedor-abm"), viewModel);

    viewModel.set("RolCombo", roles);
}
function LimpiarViewModel() {
    $("#buscadorProveedor").val("");
    $("#datos-proveedor").hide();
    $("#buscadorResult").val("");
    $("#RolId").data("kendoDropDownList").value("");
    var select = $("#RolId").data("kendoDropDownList");
    for (var i = 0; i <= select.dataSource.data().length; i++) {
        $("#RolId").data("kendoDropDownList").dataItem(i).set("Disabled", false);
    }
    viewModel.set("RolesSeleccionados", []);
}

function Grabar() {
    var result = MSExecuteOnServer('/AdministracionProveedor/GrabarRolProveedor', { id: $("#buscadorResult").val(), roles: viewModel.RolesSeleccionados });

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
    var roles = MSExecuteOnServer('/AdministracionProveedor/TraerRolesProveedor', { id });
    if (roles) {
        for (var i = 0; i < roles.length; i++) {
            viewModel.RolesSeleccionados.push(new Rol(roles[i].Id, roles[i].Descripcion));

            var select = $("#RolId").data("kendoDropDownList");
            for (var j = 1; j <= select.dataSource.data().length; j++) {
                var option = $("#RolId").data("kendoDropDownList").dataItem(j);
                if (option.Id == roles[i].Id) {
                    option.set("Disabled", true);
                }
            }
        }
    }
}