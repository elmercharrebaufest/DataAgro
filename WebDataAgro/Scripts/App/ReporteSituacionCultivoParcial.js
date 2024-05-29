$(document).ready(function () {
    kendo.culture("es-AR");
    CreateGridInformeCompraNetSituacion();
});

function CreateGridInformeCompraNetSituacion() {

    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ReporteResearch/BuscarDatosSituacionCultivoParcial'
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
                    LocalidadId: { type: "number" },
                    MaterialId: { type: "number" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "FechaHora", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };
    $("#gridSituacionCultivoParcial").kendoGrid({
        toolbar: ["excel"],
        excel: {
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {

        },
        columns: [
            { field: "Localidad", type: "string", width: 300, filterable: true },
            { field: "Provincia", type: "string", width: 300, filterable: true },
            { field: "Partido", type: "string", width: 300, filterable: true },
            { field: "Material", title: "Cultivo", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz"
                    }, {
                        Material: "Trigo"
                    }, {
                        Material: "Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Sorgo"
                    }]
            }, width: 130, template: "#=Material#"
            },
            { field: "Campania", title: "Campaña" },
            { field: "Estadio", type: "string", width: 300, filterable: true },
            { field: "Situacion", title: "Situación" , type: "string", width: 300, filterable: true },
            { field: "Comercial", type: "string", width: 300, filterable: true },
            { field: "FechaHora", title: "Fecha", filterable: { extra: true }, format: _DefaultDateTemplate },
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
        },
        excelExport: function (e) {
            var stringFecha = kendo.toString(new Date, "dd/MM/yyyy");
            e.workbook.fileName = "Reporte Situacion Cultivo " + stringFecha + ".xlsx";
            var sheet = e.workbook.sheets[0];
            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];
                var operacionFecha = row.cells[8].value;
                operacionFecha.setHours(operacionFecha.getHours() + 1);
                row.cells[8].value = operacionFecha;
            }
        }
    });
}