$("#document").ready(function () {
    $("#ComercialId").kendoDropDownList({
        //optionLabel: "Seleccione uno",
        filter: "contains",
        ignoreCase: true,
        change: onComercialChange
    });
    inicializarGrids();
});



function inicializarGrids() {

    gridComerciales = $("#gridComerciales").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        ComercialId: { type: "string" },
                        Apellido: { type: "string" },
                        Nombres: { type: "string" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "ComercialId", title: "ID" },
            { field: "Apellido", title: "Apellido" },
            { field: "Nombres", title: "Nombres" }
        ]
    }).data("kendoGrid");


    gridRoles = $("#gridRoles").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        Descripcion: { type: "string" },
                        Permisos: { type: "string" },
                        Disabled: { type: "string" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "Descripcion", title: "Rol", width: "120px" },
            { field: "Permisos", title: "Permisos" },
            { field: "Disabled", title: "Estado", width: "80px" }
        ]
    }).data("kendoGrid");
}

function filtroContains() {
    return {
        mode: "menu",
        extra: false, // Oculta el segundo parámetro (operador)
        operators: {
            string: {
                contains: "Contiene"
            }
        }
    };
}


function onComercialChange(e) {
    var comercialId = this.value();

    limpiarGrids();
    if (!comercialId) {
        return;
    }

    cargarDatosComercial(comercialId);
}

function cargarDatosComercial(comercialId) {
    var response = MSRedirectURLOnServer('/Home/ComercialDatos', { ComercialId: comercialId });

    // Normalizamos Disabled a texto
    $.each(response.rolesPermisos, function (i, r) {
        r.Disabled = r.Disabled ? "Inactivo" : "Activo";
    });

    gridComerciales.dataSource.data(response.comercialesAsociados);
    gridRoles.dataSource.data(response.rolesPermisos);
}

function limpiarGrids() {
    gridComerciales.dataSource.data([]);
    gridRoles.dataSource.data([]);
}