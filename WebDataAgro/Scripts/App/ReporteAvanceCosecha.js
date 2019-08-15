$(document).ready(function () {
    kendo.culture("es-AR");
    CreateGridInformeCompraNet();
});

function CreateGridInformeCompraNet() {

    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ReporteResearch/BuscarDatosAvanceCosecha'
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
                    RangoDesde: { type: "number" },
                    RangoHasta: { type: "number" },
                    Rendimiento: { type: "number" },
                    Avance: { type: "number" },
                    FechaHora: { type: "date" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "FechaHora", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20,
    };

    $("#gridAvanceCosecha").kendoGrid({
        toolbar: ["excel"],
        excel: {
            allPages: true
        },
        dataSource: ds,
        columns: [
            { field: "Localidad", type: "string" },
            { field: "Provincia", type: "string", width: 300, filterable: true },
            { field: "Partido", type: "string" },
            {
                field: "Material", title: "Cultivo", filterable: {
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
                }, width: 130, template: "#=Material#"
            },
            { field: "Campania", title: "Campaña"},
            { field: "Avance", title: "Avance %" },
            { field: "Rendimiento", title: "Rendimiento (Tn)"},
            { field: "RangoDesde", title: "Rango Desde" },
            { field: "RangoHasta", title: "Rango Hasta" },
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
        },
        excelExport: function (e) {
            var stringFecha = kendo.toString(new Date, "dd/MM/yyyy");
            e.workbook.fileName = "Reporte Avance Cosecha " + stringFecha + ".xlsx";
            var sheet = e.workbook.sheets[0];
            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];
                var operacionFecha = row.cells[10].value;
                operacionFecha.setHours(operacionFecha.getHours() + 1);
                row.cells[10].value = operacionFecha;
            }
        }
    });
}