var viewModel;
function Permiso(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}
$(document).ready(function () {
    kendo.culture("es-AR");
    CreateGridRol();
    CrearViewModel();
    kendo.bind($("#ModalRolPermiso"), viewModel);

    $("#ModalRolPermiso").on("hidden.bs.modal", function () {
        $("#permiso option").removeAttr('disabled');
    });

});


function CreateGridRol() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/Rol/BuscaDatosTabla'
            },
            parameterMap: function (options, operation) {
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            model: {
                id: 'Id',
                fields: {
                    Descripcion: { type: "string" },
                    Permisos: { type: "string" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        serverFiltering: true,
        pageSize: 20,
    };
    
    $("#gridRol").kendoGrid({
        dataSource: ds,
        columns: [
            {
                field: "Descripcion",
                title: "Roles"
            },
            {
                field: "Permisos",
                title: "Permisos"
            },
            {
                field: "Id", title: " ", filterable: false, sortable: false, width: 75, template: function (dataItem) {
                    return '<a data-toggle="tooltip" title="EditarRol" class="abrirModalLimite links-grid" onclick="Editar(' + dataItem.Id + ')">' +
                        '<span> <i class="fa fa-pencil"></i> </span ></a >' +
                    '<a data-toggle="tooltip" title="" class="" onclick="Eliminar(' + dataItem.Id + ')">' +
                    '<span> <i class="fa fa-trash" aria-hidden="true"></i> </span ></a >';
                }
            }
        ]
    });
}
function Eliminar (id) {
    MSExecuteOnServerAsync('/Rol/Eliminar', { id }, function (result) {
        if (result != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr(result.Errores[0].Message);

            } else {
                MensInfo("Se eliminó con éxito");
                $("#ModalRolPermiso").modal('hide');
                recargarGrilla();
            }
        }
    });
}
function Editar(id) {
    LimpiarPermisos();

    $.ajax({
        async: false,
        url: MSGetUrl('/Rol/Modificar/'),
        type: 'GET',
        data: { id: id },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (respuesta) {
            CargarViewModel(respuesta);
        },
        error: function (error) {
            MensErr("No se pudieron enviar los datos al servidor");
        }
    });
    $("#ModalRolPermiso").modal('show');

}

function CargarViewModel(datos) {

    var mappedPermisos = $.map(datos.PermisosEnum, function (item) {
        //inhabilito la opcion
        $("#"+item).prop('checked', true);
        viewModel.get("permisos").push(new Permiso(item, $("#" + item).val()));
    });

    viewModel.set("descripcionRol", datos.Descripcion);
    viewModel.set("id", datos.Id);
}

function LimpiarPermisos() {
    var iteracionesPermisos = viewModel.permisos.length;
    for (var i = 0; i < iteracionesPermisos; i++) {
        viewModel.permisos.pop();
    }
    $("input").prop('checked', false);
}

function AgregarRol() {
    $("#ModalRolPermiso").modal('show');
    LimpiarPermisos();
    viewModel.set("descripcionRol", "");
}

function CrearViewModel() {

    viewModel = kendo.observable({
        id: null,
        newPermisoId: null,
        newPermisoDescripcion: null,
        descripcionRol: null,
        permisos: [],
        //addPermiso: function () {
        //    if ($('#permiso option:selected').text() != "Seleccione Permiso") {
        //        this.permisos.push(new Permiso($('#permiso option:selected').val(), $('#permiso option:selected').text()));

        //        $("#permiso option:selected").attr('disabled', 'disabled');
        //    }
        //    $('#permisosFinales').val(this.permisos.toJSON());

        //},
        guardar: function () {
            var dto = {
                Id: this.id,
                Descripcion: this.descripcionRol,
                PermisosEnum: this.permisos.map(permisos => permisos.Id)
            };
            MSExecuteOnServerAsync('/Rol/CrearModificarPost', dto, function (result) {
                if (result != null) {
                    if (ExistsErrorMessages(result.Errores)) {
                        MensErr(result.Errores[0].Message);                       

                    } else {
                        MensInfo("Se guardó con éxito");
                        $("#ModalRolPermiso").modal('hide');
                        recargarGrilla();
                    }
                }
            });


        },
        
    });
}
function recargarGrilla() {
    $('#gridRol').data('kendoGrid').dataSource.read();   
}

function AgregarRemoverPermiso(e) {
    if (e.checked) {
        viewModel.get("permisos").push(new Permiso(e.id, e.value));
    } else {
        var permi = Remover(e.id);
        viewModel.set("permisos", permi);
    }
}
function Remover(value) {
    return viewModel.permisos.filter(function (ele) {
        return ele.Id != value;
    });
}
