$(document).ready(function () {
    kendo.culture("es-AR");
    CreateGrid();
});

function CreateGrid() {
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ReporteResearch/BuscarDatosVentaStock'
            },
            parameterMap: function (options, operation) {
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
                    FechaHora: { type: "date" },
                    Almacenado: { type: "number" },
                    VendidoAPrecio: { type: "number" },
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "FechaHora", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20,
    };

    $("#gridReporteVentaStock").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Venta Stock.xlsx",
            allPages: true
        },
        dataSource: ds,
        columns: [
            { field: "Localidad", type: "string" },
            { field: "Provincia", type: "string", width: 300, filterable: true },
            { field: "Partido", type: "string" },
            { field: "Material", title: "Cultivo", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz Duro Dentado"
                    }, {
                        Material: "Trigo Pan"
                    }, {
                        Material: "Semilla de Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Girasol Alto Oleico"
                    }]
                }, width: 130, template: "#=Material#" },
            { field: "VendidoAPrecio", title: "Vendido a Precio" },
            { field: "Almacenado", title: "Almacenado" },
            { field: "Comercial", title: "Comercial" },
            { field: "FechaHora", type: "date", title: "Fecha", format: _DefaultDateTemplate, width: 80 },
            { field: "Observaciones", type: "string", filterable: false, attributes: { "class": "ColumnaObservacion" } }
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
        selectable: "row",

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
                    eq: "Igual",
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a",
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a",
                }
            }
        }
    })

}