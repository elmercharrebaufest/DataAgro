$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    InicializarCuposIndex();
        
});

function InicializarCuposIndex() {
    var defaultFilter = { field: "FechaIngreso", operator: "gte", value: new Date };
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ReporteCupo/BuscaDatosTabla'
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
                    FechaIngreso: { type: "date" },
                    FechaGeneracion: { type: "date" },
                    FleteProcedencia: { type: "boolean" }
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "FechaIngreso", dir: "asc" }],
        serverFiltering: true,
        pageSize: 20,
        filter: defaultFilter
    };

    $("#grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Cupos.xlsx",
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].FleteProcedencia) {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("flete-procedencia");
                }
                $('[data-toggle="tooltip"]').tooltip();
            }
        },
        columns: [            
            { field: "Fecha", title: "Ingreso", type: "date", width: 150 },
            { field: "Hora", title: "Hora", type: "date", width: 150 },
            { field: "CupoSap", title: "Cupo", type: "string", width: 150 },
            { field: "Material", type: "string", width: 150 },
            { field: "Proveedor", type: "string", width: 150, filterable: { ui: createMultiSelectProveedor } },
            { field: "Destinatario", type: "string", width: 150 },
            { field: "Centro", title:"Planta", type: "string", width: 150 },
            { field: "Calidad", type: "string", width: 150 },
            { field: "ZonaCupo", title: "Zona", type: "string", width: 150 },
            {
                field: "FleteProcedencia", title: "Flete", type: "string", width: 150, template: function (dataItem) { return dataItem.FleteProcedencia ? "Si" : "No"; }
            },
            { field: "CupoStop", title: "Cupo STOP", type: "string", width: 150 },
            {
                field: "EstadoCupo", title: "Estado", filterable: {
                    multi: true,
                    dataSource: [{
                        EstadoCupo: "Sin CTG"
                    }, {
                        EstadoCupo: "Activado"
                    }, {
                        EstadoCupo: "Arribado"
                    }, {
                        EstadoCupo: "Descargado"
                    }, {
                        EstadoCupo: "Anulado"
                    }, {
                        EstadoCupo: "Disponible"
                    }, {
                        EstadoCupo: "Sin STOP"
                    }, {
                        EstadoCupo: "Error STOP"
                    }]
                }, sortable: false, width: 200,
                itemTemplate: function (e) {
                    return "<span><label><span>#= data.EstadoCupo|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.EstadoCupo#'/></label></span>";
                },
                template: function (dataItem) {
                    if (dataItem.EstadoCupoId == 1) {
                        return '<div class="status sinctg"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 2) {
                        return '<div class="status activado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 3) {
                        return '<div class="status arribado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 5) {
                        return '<div class="status descargado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 4) {
                        return '<div class="status anulado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 6) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 7) {
                        return '<div class="status error" data-toggle="tooltip" data-placement="top" title="' + dataItem.MensajeError + '"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 8) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    }
                }
            },
            { field: "Comercial", type: "string", width: 150, filterable: { ui: createMultiSelectComercial } },
            { field: "Observaciones", type: "string", width: 150 }
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
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var templateFlete = kendo.template(this.columns[9].template);
            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataItem = { FleteProcedencia: row.cells[9].value };

                row.cells[9].value = templateFlete(dataItem);
            }
        },
        filterable: {
            checkAll: false,
            height: 350,
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
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        },
        filterMenuInit: function (e) {
            if (e.field == "Proveedor" || e.field == "Comercial") {
                $(e.container).css("width", "300px");
            }
        }
    });

    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };

    function createMultiSelect(element, textField, valueField, url) {
        element.removeAttr("data-bind");

        element.kendoMultiSelect({
            itemTemplate: "<input type='checkbox'/> #:data." + textField + "#",
            dataBound: function () {
                var items = this.ul.find("li");
                setTimeout(function () {
                    checkInputs(items);
                });
            },

            dataTextField: textField,
            dataValueField: valueField,
            autoClose: false,
            autoBind: false,
            delay: 300,
            dataSource: {
                serverFiltering: true,
                filter: [],
                transport: {
                    read: {
                        url: url,
                        data: function () {
                            return {
                                text: element.data("kendoMultiSelect").input.val()
                            };
                        },
                        prefix: ""
                    }
                }
            },
            change: function (e) {
                var items = this.ul.find("li");
                checkInputs(items);
                var grilla = $('#grid').data("kendoGrid");
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        addOrRemoveFilter(grilla, valueField, "eq", v);
                    }
                });

                if (values.length === 0) {
                    addOrRemoveFilter(grilla, valueField, "eq", "");
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }
    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "Proveedor", "/Cupo/ListarProveedor");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "Comercial", "/Cupo/ListarComercial");
    }

    function addOrRemoveFilter(grid, field, operator, value) {

        var newFilter = { field: field, operator: operator, value: value };
        var dataSource = grid.dataSource;
        var filters = null;
        if (dataSource.filter() != null) {
            filters = dataSource.filter().filters;
        }

        if (value && value.length > 0) {
            //Add filter
            if (filters == null) {
                filters = [newFilter];
            }
            else {
                var isNew = true;
                var index = 0;
                for (index = 0; index < filters.length; index++) {
                    if (filters[index].field == field) {
                        isNew = false;
                        break;
                    }
                }
                if (isNew) {
                    filters.push(newFilter);
                }
                else {
                    filters[index] = newFilter;
                }
            }
        }
        else {
            //Remove filter 
            var removeIndex = -1;
            if (filters != null) {
                for (var x = 0; x < filters.length; x++) {
                    var temp = filters[x];
                    if (temp.field == field) {
                        removeIndex = x;
                        break;
                    }
                }
                if (removeIndex != -1)
                    filters.splice(removeIndex, 1);
            }
        }
        dataSource.filter(filters);
    }
}

