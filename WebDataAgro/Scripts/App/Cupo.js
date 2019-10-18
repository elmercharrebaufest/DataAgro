var viewModel;
var fecha;
var zonaSeleccionada;
    $(document).ready(function () {
        kendo.culture("es-AR");
        $('#menuproveedor').hide();
        $("#crearCupo").click(function () {
            if ($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial" || $("#perfil").val() == "CorredoresComercial") {
                window.location.href = window.location.origin + "/Cupo/CrearCupo";
            } else {
                MensInfo("No posee permisos para la carga de cupos");
            }
        });

        InicializarCargaCupos();
        $.unblockUI();
        checkFason();
        checkSoja();
        InicializarCuposIndex();
    });

function InicializarCuposIndex() {    
    var defaultFilter = { field: "FechaIngreso", operator: "gte", value: new Date };
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/Cupo/BuscaDatosTabla'
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

    $("#gridCupo").kendoGrid({
        dataSource: ds,
        dataBound: function () {
            var grid = $("#gridCupo").data("kendoGrid");
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].FleteProcedencia) {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("flete-procedencia");
                }
                $('[data-toggle="tooltip"]').tooltip();
                grid.tbody.find('tr').each(function () {
                    var row = $(this);
                    var dataItem = grid.dataItem(row);

                    if (dataItem.CentroId !== 1) {
                        row.find('td').eq(0).empty();
                    }
                });
            }        
        },
        columns: [
            {selectable:true},
            { field: "FechaIngreso", title: "Ingreso", type: "date", width: 150, format: _DefaultDateTemplate },
            { field: "CupoSap", title: "Cupo", type: "string", width: 150 },
            { field: "Material", type: "string", width: 150 },
            { field: "Proveedor", type: "string", width: 150, filterable: { ui: createMultiSelectProveedor } },
            { field: "Destinatario", type: "string", width: 150 },
            { field: "Centro", type: "string", width: 150 },
            { field: "Calidad", type: "string", width: 150 },
            { field: "ZonaCupo", title: "Zona", type: "string", width: 150 },
            {
                field: "FleteProcedencia", title: "Flete", type: "string", width: 150, template: function (dataItem) { return dataItem.FleteProcedencia ? "Si" : "No"; }
            },
            { field: "Observaciones", type: "string", width: 150 },
            { field: "Comercial", type: "string", width: 150, filterable: { ui: createMultiSelectComercial } },
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
                        return '<div class="status sinctg"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                            botonBorrar(dataItem, 'fa-trash ctg');
                    } else if (dataItem.EstadoCupoId == 2) {
                        return '<div class="status activado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 3) {
                        return '<div class="status arribado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 5) {
                        return '<div class="status descargado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 4) {
                        return '<div class="status anulado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 6) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                            botonBorrar(dataItem, 'fa-trash sto') +
                            botonRetransmitir(dataItem, 'fa-mail-forward sto' );
                    } else if (dataItem.EstadoCupoId == 7) {
                        return '<div class="status error" data-toggle="tooltip" data-placement="top" title="' + dataItem.MensajeError +'"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                            botonBorrar(dataItem, 'fa-trash err') +
                            botonRetransmitir(dataItem, 'fa-mail-forward err');
                    }
                }
            }
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
                var grilla = $('#gridCupo').data("kendoGrid");                
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
    $("#finalizarBorrar").click(function () {
        ObtenerDatosModalBorrado();
    });
}
function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalBorrar(' +
        "'" + dataItem.Id + "'" + ',' +
        "'" + dataItem.CupoSap + "'" + ',' +
        ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
}
function botonRetransmitir(dataItem, icono) {
    if (dataItem.CentroId == 1) {
        return '<button data-toggle="tooltip" title="Transmitir a STOP" onclick="Retransmitir(' +
            "'" + dataItem.CupoSap + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return '<div></div>';
    }

}
function ModalBorrar(id, cupo) {
    $("#cupo_a_borrar").text(cupo);
    $("#cupoBorrar").val(id);   

    $("#modalBorrar").modal('show');
}
function retransmitirSelccionados() {
    var grid = $("#gridCupo").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        if ((selectedItem.EstadoCupoId == 6 || selectedItem.EstadoCupoId == 7) && selectedItem.CentroId == 1 ) {
            obj.push(selectedItem.CupoSap);
        }
    });
    Retransmitir(obj);
}

function Retransmitir(listaCupos) {
    var a = [];
    if (!Array.isArray(listaCupos)) {
        a.push(listaCupos);
    } else {
        for (var i = 0; i <= cupos.length; i++) {
            a.push(listaCupos[i]);
        }
    }
    MSExecuteOnServer('/Cupo/TransmitirCupos', {cupos:a});
    recargarGrilla();
}

function ObtenerDatosModalBorrado() {
    var objBorrar = {};
    var result;    
    objBorrar.id = $("#cupoBorrar").val();
    result = MSExecuteOnServer('/Cupo/EliminarCupo', objBorrar);    
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        MensInfo("Borrado Exitoso");
        recargarGrilla();
    }
}
function recargarGrilla() {
    $('#gridCupo').data('kendoGrid').dataSource.read();
}

function InicializarCargaCupos() {
    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#Proveedor").val("");    
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
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
            $("#Proveedor").val(e.dataItem.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Cupo/BuscarProveedor"
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

    $("#FechaEntrega").kendoDatePicker({

        //format: "dd-MM-yyyy",
        //parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#cantidad").kendoNumericTextBox({
        optionLabel: "SELECCIONE CANTIDAD DE CUPOS...",
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0        
    });
    $("#cuit").mask("00000000000");
    $("#fason").click(function () {
        checkFason();
    });
    $("#material").change(function () {
        checkSoja();
    });
    
}
function checkFason() {
    if ($("#fason").is(':checked')) {
        $("#cuit").show();
    }
    else {
        $("#cuit").hide("hidden");
        $("#cuit").val("");
    }
}
function checkSoja() {
    if ($("#material").val() !== "3") {
        $("#calidadDiv").hide();
        $("#calidad").val("");
    } else {
        $("#calidadDiv").show();
    }
}
