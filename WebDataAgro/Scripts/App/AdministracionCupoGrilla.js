var externo;
$(document).ready(function () {
    $('#menuproveedor').hide();
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })  
   
    CargarGrillaConfig();
    $('#CargaCupos').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
    });

});
function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/AdministracionCupo/DatosAdministracion'
            },
            parameterMap: function (options, operation) {
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }, 
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Fecha: { type: "date" },
                    LimiteCupo: { type: "number" }
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Fecha", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };
    var classExterno = externo ? "hide" : "";
    $("#gridInformeCompraNet").kendoGrid({
        dataSource: ds,
        parameterMap: function (options, operation) {

            if (operation == "read") {
                return JSON.stringify(options)
            }
            if (options.filter) {
                KendoGrid_FixFilter(ds, options.filter);
            }
            return options;
        },
        dataBound: function () {
            $("td:has(div.statuspendiente)").attr('id', 'border-orange');
            $("td:has(div.statusconfirmado)").attr('id', 'border-green');
            $("td:has(div.statuseliminado)").attr('id', 'border-grey');
        },
        columns: [
            //{ selectable: true, width: "50px" },
            {
                field: "Proveedor", type: "string", width: 150,
                editable: function (dataItem) {
                    return false;
                },
                headerAttributes: { "class": classExterno }, attributes: { "id": "line", "class": classExterno },
                template: function (dataItem) {
                    if (dataItem.EstadoId == 3) {
                        return '<div class="statuspendiente "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 1) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 2) {
                        return '<div class="statuseliminado "></div>' + dataItem.Proveedor;
                    }
                },
                filterable: { ui: createMultiSelectProveedor }
            },
            {
                field: "Comercial", type: "string", title: "Comercial", width: 70, editable: function (dataItem) {
                    return false;
                }, filterable: { ui: createMultiSelectComercial }, headerAttributes: {
                    "class": classExterno
                },
                attributes: { "class": "mobile-xs " + classExterno }
            },
            {
                field: "Material", type: "string", editable: function (dataItem) {
                    return false;
                }, filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz"
                    }, {
                        Material: "Trigo"
                    }, {
                        Material: "Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Girasol AO"
                    }]
                }, width: 95, attributes: {
                    "class": "mobile-xs"
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Material|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Material#'/></label></span>";
                }, template: "#=Material#"
            },
            {
                field: "Zona", title: "Zona", type: "string", editable: function (dataItem) {
                    return false;
                }, width: 150,
                filterable: {
                    multi: true,

                    dataSource: [
                        { ZonaCupo: "CORREDOR BS AS" },
                        { ZonaCupo: "CORREDOR ROSARIO" },
                        { ZonaCupo: "Fasones CAGSA/MOLCA, YPF y AMAGGI" },
                        { ZonaCupo: "MAT-ROFEX" },
                        { ZonaCupo: "ORIG INTERIOR CENTRO" },
                        { ZonaCupo: "ORIG INTERIOR NORTE" },
                        { ZonaCupo: "ORIG INTERIOR SUR" },
                        { ZonaCupo: "PRODUCCION PROPIA" },
                        { ZonaCupo: "REDESPACHOS" },
                        { ZonaCupo: "SOLIDARIDAD" }],
                    itemTemplate: function (e) {

                        return "<span><label><input type='checkbox' name='" + e.field + "' value='#= data.ZonaCupo#'/><span>#= data.ZonaCupo|| data.all #</span></label></span><br>";
                    }
                },
            },
            {
                field: "Centro", type: "string", title: "Destino", editable: function (dataItem) {
                    return false;
                }, attributes: { "class": "mobile-xs mobile-md" } },
            {
                field: "Fecha", title: "Fecha Sugerencia", type: "date", editable: function (dataItem) {
                    return false;
                }, format: _DefaultDateTemplate },
            {
                field: "CantidadCupo", title: "Cantidad de Cupos", width: "110px",               
                editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);

                    $("#CantidadCupo").val(options.model.CantidadCupo);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        max: options.model.CantidadCupo,
                        min: 0
                    });
                }
            },
            {
                field: "CantidadFleteProcedencia", title: "Cantidad Flete Procedencia",
                editor: function (container, options) {
                // create an input element
                var input = $("<input name='" + options.field + "'/>");
                // append it to the container
                input.appendTo(container);

                    $("#CantidadCupoFlete").val(options.model.CantidadFleteProcedencia);
                // initialize a Kendo UI numeric text box and set max value
                input.kendoNumericTextBox({
                    max: options.model.CantidadFleteProcedencia,
                    min: 0
                });
            }
            },
            {
                field: "EstadoId", title: "Estado", editable: function (dataItem) {
                    return false;
                },
                filterable: {
                    multi: true,

                    dataSource: [
                        { EstadoId: "Aceptado" },
                        { EstadoId: "Rechazado" },
                        { EstadoId: "Pendiente" }]              
                 },               
                itemTemplate: function (e) {
                    return "<span><label><span>#= data.EstadoId || data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.EstadoId#'/></label></span>";
                }, template: function (dataItem) {
                    if (dataItem.EstadoId == 3) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonAprobar(dataItem, 'fa-check pend') +
                            botonBorrar(dataItem, 'fa-trash pend');
                           
                    }
                    if (dataItem.EstadoId == 1) { //confirmado                       
                        return '<div class="status confirmado">Confirmado</div>';
                    }  
                    if (dataItem.EstadoId == 2) { //Rechazado
                        return '<div class="status borrado">Rechazado</div>';
                    }     
                }
            }      
        ],
        editable: true,
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
    var fecha = new Date();
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    addOrRemoveFilter(grilla, "Fecha", "gte", fecha);
    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };
    function createMultiSelect(element, textField, valueField, url,columna) {
        element.removeAttr("data-bind");
        columna = columna == null ? valueField : columna;
        console.log(columna);
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
                },
            },
            change: function (e) {
                var items = this.ul.find("li");
                checkInputs(items);
                var grilla = $('#gridInformeCompraNet').data("kendoGrid");
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        addOrRemoveFilter(grilla, columna, "eq", v);
                    }
                });

                if (values.length === 0) {
                    addOrRemoveFilter(grilla, columna, "eq", "");
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "Proveedor", "/CompraNet/ListarProveedor");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial");
    }
}

function botonAprobar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Confirmar" onclick="ModalAceptarSugerencia(' + dataItem.id + ')"><i class="fa ' + icono + '"></i></button>';
    
}
function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalRechazarSugerencia(' + dataItem.id +') "><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';   
}

function ModalAceptarSugerencia(id) {
    $("#modalAceptarSolicitud").modal("show");    
    var grid = $("#gridInformeCompraNet").data("kendoGrid").dataSource.data();
    var solicitudSeleccionada = grid.filter(function (x) { return (x.Id == id) });
    $("#CantidadCupo").val(solicitudSeleccionada[0].CantidadCupo);
    $("#CantidadCupoFlete").val(solicitudSeleccionada[0].CantidadFleteProcedencia);
    $("#solicitudId").val(id);
}
function ModalRechazarSugerencia(id) {
    $("#modalRechazarSolicitud").modal("show");
    $("#solicitudId").val(id);

}
function AceptarSolicitud() {
    var id = $("#solicitudId").val();

    var cantidad = $("#CantidadCupo").val();
    var cantidadFp = $("#CantidadCupoFlete").val();
    if (cantidad == 0 && cantidadFp == 0) {
        MensErr("La solicitud no se puede aceptar");
    }
    result = MSExecuteOnServer('/AdministracionCupo/Aceptar', { administracionId: id, cantidadCupo: cantidad, cantidadFleteProcedencia : cantidadFp });

    $.unblockUI();
    var errores = new Array();
    var cuposGenerados = new Array();
    if (result.HayError) {
        errores = errores.concat(result.ListaErrores);
    }
    if (result.ListaCupos != null && result.ListaCupos.length > 0) {
        cuposGenerados = cuposGenerados.concat(result.ListaCupos);
    }
    if (cuposGenerados.length > 0) {
        cuposCreados(cuposGenerados);

    }
    if (errores.length > 0) {
        ShowErrorMessages(errores);
    }
}


function RechazarSolicitud() {
    var id = $("#solicitudId").val();
    result = MSExecuteOnServer('/AdministracionCupo/Rechazar', { administracionId: id });
    var errores = new Array();
    for (var i = 0; i < result.length; i++) {
        if (result[i].HayError) {
            errores = errores.concat(result[i].ListaErrores);
        }
    }

    if (errores.length == 0) {
        MensInfo("Se grabo correctamente.");
    } else {
        ShowErrorMessages(errores);
    }
    recargarGrilla();
}
function cuposCreados(lista) {
    $("#cupos-generados-modal").html(lista.join("</br>"));
    $('#resultadoCupo').modal('toggle');

}

function resultadoCupo() {
    var listaCupos = $("#cupos-generados-modal").html().replace(/<br>/g, "\n");
    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', listaCupos);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', listaCupos);
        }
    };
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
}

function addOrRemoveFilter(grid, field, operator, value) {

    var newFilter = { field: field, operator: operator, value: value };
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }

    if (value != null && (value.length > 0 || value != undefined)) {
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
function recargarAceptar() {
    recargarGrilla();
}
function recargarGrilla() {
    $('#gridInformeCompraNet').data('kendoGrid').dataSource.read();
}