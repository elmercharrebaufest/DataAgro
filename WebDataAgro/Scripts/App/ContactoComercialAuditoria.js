$(document).ready(function () {
    inicializarControles();
    inicializarGrids();
});

function inicializarControles() {
    // Inicializar input de fecha con Kendo DatePicker
    $("#FechaBusqueda").kendoDatePicker({
        value: new Date(),
        format: "dd/MM/yyyy"
    });

    // Inicializar ContactoComercialId con Kendo NumericTextBox
    $("#ContactoComercialId").kendoNumericTextBox({
        format: "0",
        min: 0,
        value: 646
    });

    // Inicializar input de usuario
    $("#UsuarioBusqueda").val("gsian");

    // Inicializar input de días
    $("#DiasBusqueda").kendoNumericTextBox({
        format: "0",
        min: 1,
        value: 7
    });
}

function inicializarGrids() {
    // Grid para historial
    window.gridHistorial = $("#gridHistorial").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        ContactoComercialId: { type: "number" },
                        ProveedorId: { type: "number" },
                        Apellido: { type: "string" },
                        Nombres: { type: "string" },
                        TipoOperacion: { type: "string" },
                        UsuarioModificacion: { type: "string" },
                        FechaModificacion: { type: "date" },
                        ValoresAnteriores: { type: "string" },
                        ValoresNuevos: { type: "string" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "ContactoComercialId", title: "ID", width: "80px" },
            { field: "Apellido", title: "Apellido" },
            { field: "Nombres", title: "Nombres" },
            { field: "TipoOperacion", title: "Operación", width: "100px" },
            { field: "UsuarioModificacion", title: "Usuario", width: "100px" },
            {
                field: "FechaModificacion",
                title: "Fecha",
                type: "date",
                format: "{0:dd/MM/yyyy HH:mm:ss}",
                width: "150px"
            },
            {
                field: "ValoresAnteriores",
                title: "Valores Anteriores",
                template: "<span title='#:ValoresAnteriores#'>#: (ValoresAnteriores || '').substring(0, 50) #...</span>"
            },
            {
                field: "ValoresNuevos",
                title: "Valores Nuevos",
                template: "<span title='#:ValoresNuevos#'>#: (ValoresNuevos || '').substring(0, 50) #...</span>"
            }
        ]
    }).data("kendoGrid");

    // Grid para cambios recientes
    window.gridCambiosRecientes = $("#gridCambiosRecientes").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        ContactoComercialId: { type: "number" },
                        Apellido: { type: "string" },
                        Nombres: { type: "string" },
                        TipoOperacion: { type: "string" },
                        UsuarioModificacion: { type: "string" },
                        FechaModificacion: { type: "date" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "ContactoComercialId", title: "ID", width: "80px" },
            { field: "Apellido", title: "Apellido" },
            { field: "Nombres", title: "Nombres" },
            { field: "TipoOperacion", title: "Operación", width: "100px" },
            { field: "UsuarioModificacion", title: "Usuario", width: "100px" },
            {
                field: "FechaModificacion",
                title: "Fecha",
                type: "date",
                format: "{0:dd/MM/yyyy HH:mm:ss}",
                width: "150px"
            }
        ]
    }).data("kendoGrid");

    // Grid para cambios por usuario
    window.gridCambiosPorUsuario = $("#gridCambiosPorUsuario").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        ContactoComercialId: { type: "number" },
                        Apellido: { type: "string" },
                        Nombres: { type: "string" },
                        TipoOperacion: { type: "string" },
                        UsuarioModificacion: { type: "string" },
                        FechaModificacion: { type: "date" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "ContactoComercialId", title: "ID", width: "80px" },
            { field: "Apellido", title: "Apellido" },
            { field: "Nombres", title: "Nombres" },
            { field: "TipoOperacion", title: "Operación", width: "100px" },
            {
                field: "FechaModificacion",
                title: "Fecha",
                type: "date",
                format: "{0:dd/MM/yyyy HH:mm:ss}",
                width: "150px"
            }
        ]
    }).data("kendoGrid");

    // Grid para último cambio
    window.gridUltimoCambio = $("#gridUltimoCambio").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        ContactoComercialId: { type: "number" },
                        Apellido: { type: "string" },
                        Nombres: { type: "string" },
                        TipoOperacion: { type: "string" },
                        UsuarioModificacion: { type: "string" },
                        FechaModificacion: { type: "date" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "ContactoComercialId", title: "ID", width: "80px" },
            { field: "Apellido", title: "Apellido" },
            { field: "Nombres", title: "Nombres" },
            { field: "TipoOperacion", title: "Operación", width: "100px" },
            { field: "UsuarioModificacion", title: "Usuario", width: "100px" },
            {
                field: "FechaModificacion",
                title: "Fecha",
                type: "date",
                format: "{0:dd/MM/yyyy HH:mm:ss}",
                width: "150px"
            }
        ]
    }).data("kendoGrid");

    // Grid para resumen por tipo de operación
    window.gridResumenOperacion = $("#gridResumenOperacion").kendoGrid({
        dataSource: {
            data: [],
            schema: {
                model: {
                    fields: {
                        TipoOperacion: { type: "string" },
                        Cantidad: { type: "number" },
                        UltimaModificacion: { type: "date" }
                    }
                }
            }
        },
        sortable: true,
        pageable: true,
        filterable: filtroContains(),
        columns: [
            { field: "TipoOperacion", title: "Tipo de Operación", width: "150px" },
            { field: "Cantidad", title: "Cantidad", width: "100px" },
            {
                field: "UltimaModificacion",
                title: "Última Modificación",
                type: "date",
                format: "{0:dd/MM/yyyy HH:mm:ss}"
            }
        ]
    }).data("kendoGrid");
}

function filtroContains() {
    return {
        mode: "menu",
        extra: false,
        operators: {
            string: {
                contains: "Contiene"
            }
        }
    };
}

// ============= MÉTODOS DE BÚSQUEDA =============

function buscarHistorial() {
    var contactoId = $("#ContactoComercialId").data("kendoNumericTextBox").value();
    
    if (!contactoId || contactoId <= 0) {
        alert("Ingrese un ID de Contacto Comercial válido");
        return;
    }

    $.ajax({
        url: '/ContactoComercialAuditoria/Historial',
        data: { contactoComercialId: contactoId },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data) {
                window.gridHistorial.dataSource.data(response.data);
            }
        },
        error: function () {
            alert("Error al cargar el historial");
        }
    });
}

function buscarCambiosRecientes() {
    var dias = $("#DiasBusqueda").data("kendoNumericTextBox").value();
    
    if (!dias || dias <= 0) {
        alert("Ingrese un número de días válido");
        return;
    }

    $.ajax({
        url: '/ContactoComercialAuditoria/CambiosRecientes',
        data: { dias: dias },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data) {
                window.gridCambiosRecientes.dataSource.data(response.data);
            }
        },
        error: function () {
            alert("Error al cargar los cambios recientes");
        }
    });
}

function buscarCambiosPorUsuario() {
    var usuario = $("#UsuarioBusqueda").val();
    
    if (!usuario || usuario.trim() === "") {
        alert("Ingrese un nombre de usuario");
        return;
    }

    $.ajax({
        url: '/ContactoComercialAuditoria/CambiosPorUsuario',
        data: { usuario: usuario },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data) {
                window.gridCambiosPorUsuario.dataSource.data(response.data);
            }
        },
        error: function () {
            alert("Error al cargar los cambios por usuario");
        }
    });
}

function buscarUltimoCambio() {
    var contactoId = $("#ContactoComercialId").data("kendoNumericTextBox").value();
    
    if (!contactoId || contactoId <= 0) {
        alert("Ingrese un ID de Contacto Comercial válido");
        return;
    }

    $.ajax({
        url: '/ContactoComercialAuditoria/UltimoCambio',
        data: { contactoComercialId: contactoId },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data) {
                var data = Array.isArray(response.data) ? response.data : [response.data];
                window.gridUltimoCambio.dataSource.data(data);
            }
        },
        error: function () {
            alert("Error al cargar el último cambio");
        }
    });
}

function buscarResumenOperacion() {
    var contactoId = $("#ContactoComercialId").data("kendoNumericTextBox").value();
    
    if (!contactoId || contactoId <= 0) {
        alert("Ingrese un ID de Contacto Comercial válido");
        return;
    }

    $.ajax({
        url: '/ContactoComercialAuditoria/ResumenPorTipoOperacion',
        data: { contactoComercialId: contactoId },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.data) {
                window.gridResumenOperacion.dataSource.data(response.data);
            }
        },
        error: function () {
            alert("Error al cargar el resumen");
        }
    });
}

function limpiarTodos() {
    window.gridHistorial.dataSource.data([]);
    window.gridCambiosRecientes.dataSource.data([]);
    window.gridCambiosPorUsuario.dataSource.data([]);
    window.gridUltimoCambio.dataSource.data([]);
    window.gridResumenOperacion.dataSource.data([]);
}
