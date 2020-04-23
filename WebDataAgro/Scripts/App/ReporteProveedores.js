$(document).ready(function () {
    kendo.culture("es-AR");
    CreateGridDatosProveedor();
    CreateGridDatosContacto();
    CreateGridProduccion();
    CreateGridAlmacenamiento();
});

function CreateGridDatosProveedor() {
    //kendo.ui.FilterMultiCheck.prototype.options.messages =
    //    $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
    //        "selectedItemsFormat": ""
    //    });
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/ReporteProveedores/BuscarDatosProveedor'
            },
            parameterMap: function (options, operation) {
                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    FechaAlta: { type: "date" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        //sort: [{ field: "Cuit", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#grid-datos-proveedor").kendoGrid({
        dataSource: ds,
        columns: [
            { field: "Cuit", type: "string" },            
            { field: "RazonSocial", title: "Razon Social",type: "string" },            
            { field: "Estado", type: "string" },            
            { field: "Calificacion", type: "number" },            
            { field: "Segmentacion", type: "string" },            
            { field: "DomicilioDeActividad", title: "Domicilio de Actividad", type: "string" },            
            { field: "Localidad", type: "string" },            
            { field: "CodigoPostal", title: "Cod. Postal",type: "string" },            
            { field: "CanalDeOperacion", title: "Canal de Operacion", type: "string" },            
            { field: "EntregaA", title: "Entrega A", type: "string" },            
            { field: "Condiciones", type: "string" },            
            { field: "Intermediario", type: "string" },            
            { field: "AreaDeInfluencia", title: "Area de Influencia",type: "string" },            
            { field: "Comentario", type: "string" },
            { field: "Comercial", type: "string" /*template: function (e) {return e.Comerciales.join(", ");}*/ },            
            { field: "ClienteMOA", title: "Cliene MOA",type: "string" },            
            { field: "Zona", type: "string" },            
            { field: "FechaAlta", title: "Fecha de Alta",type: "FechaAlta", format: _DefaultDateTemplate},            
            { field: "Clasificacion", type: "string" },            
            { field: "Boleto", type: "string" },            
            { field: "Bolsa", type: "string" },            
            { field: "Consignatario", type: "string" },            
            { field: "Comision", type: "string" },            
            { field: "LocalidadCompraNet", title: "Localidad CompraNet", type: "string" },            
            { field: "ProvinciaCompraNet", title: "Provincia CompraNet", type: "string" },            
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        filterable: {
            height: 350,
            extra: false,
            checkAll: false,

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
                    eq: "Igual"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        }        
    });

}
function CreateGridDatosContacto() {
    //kendo.ui.FilterMultiCheck.prototype.options.messages =
    //    $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
    //        "selectedItemsFormat": ""
    //    });
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/ReporteProveedores/BuscarDatosContacto'
            },
            parameterMap: function (options, operation) {
                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    FechaNacimiento: { type: "date" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        //sort: [{ field: "Cuit", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#grid-datos-contacto").kendoGrid({
        dataSource: ds,
        columns: [
            { field: "Cuit", type: "string" },
            { field: "RazonSocial", title: "Razon Social", type: "string" },
            { field: "Nombre", type: "string" },
            { field: "Apellido", type: "number" },
            { field: "Email", type: "string" },
            { field: "Telefono", type: "string" },
            { field: "FechaNacimiento", title: "Fecha de Nacimiento", format: "{0:dd/MM/yyyy}", type: "date" },
            { field: "Profesion", type: "string" },
            { field: "Puesto", type: "string" },
            { field: "Interes", type: "string" },
            { field: "OtrosIntereses", title: "Otros Intereses", type: "string" },
            { field: "Principal", type: "string" },
            { field: "PrincipalCupo", title: "Mail Cupo",type: "string" }
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        filterable: {
            height: 350,
            extra: false,
            checkAll: false,

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
                    eq: "Igual"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        }
    });

}
function CreateGridProduccion() {
    //kendo.ui.FilterMultiCheck.prototype.options.messages =
    //    $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
    //        "selectedItemsFormat": ""
    //    });
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/ReporteProveedores/BuscarDatosProduccion'
            },
            parameterMap: function (options, operation) {
                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {

                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        //sort: [{ field: "Cuit", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#grid-produccion").kendoGrid({
        dataSource: ds,
        columns: [
            { field: "Cuit", type: "string" },
            { field: "RazonSocial", title: "Razon Social", type: "string" },
            { field: "Provincia", type: "string" },
            { field: "Localidad", type: "string" },
            { field: "Material", type: "string" },
            { field: "Campaña", type: "string" },
            { field: "Hectareas", format: "{0:n0}", type: "number" },
            { field: "Toneladas", format: "{0:n0}", type: "number" },
            { field: "Alquiladas", type: "string" },
            { field: "Propias", type: "string" }
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        filterable: {
            height: 350,
            extra: false,
            checkAll: false,

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
                    eq: "Igual"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        }
    });

}
function CreateGridAlmacenamiento() {
    //kendo.ui.FilterMultiCheck.prototype.options.messages =
    //    $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
    //        "selectedItemsFormat": ""
    //    });
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/ReporteProveedores/BuscarDatosAlmacenamiento'
            },
            parameterMap: function (options, operation) {
                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {

                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        //sort: [{ field: "Cuit", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#grid-almacenamiento").kendoGrid({
        dataSource: ds,
        columns: [
            { field: "Cuit", type: "string" },
            { field: "RazonSocial", title: "Razon Social", type: "string" },
            { field: "Provincia", type: "string" },
            { field: "Localidad", type: "string" },
            { field: "CampañaPlanta", title: "Campaña Planta",type: "string" },
            { field: "CapacidadPlantaTn", title: "Capacidad en Planta(Tn)",format: "{0:n0}", type: "number" },
            { field: "Alquiladas", type: "string" },
            { field: "Propias", type: "string" },
            { field: "Campaña", type: "string" },
            { field: "Material", type: "string" },
            { field: "Toneladas", format: "{0:n0}", type: "number" },
            { field: "VolumenAnualTn", title: "Volumen Anual(Tn)", format: "{0:n0}", type: "number" },
            { field: "HabilitadoSojaSustentable", title: "Habilitado Soja Sust.", type: "string" }
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        filterable: {
            height: 350,
            extra: false,
            checkAll: false,

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
                    eq: "Igual"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        }
    });

}