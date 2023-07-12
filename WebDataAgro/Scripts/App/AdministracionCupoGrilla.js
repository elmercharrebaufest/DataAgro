var externo;
$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    CargarGrillaConfig();
    $('#CargaCupos').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
    });
    AutoRecargarSolicitudes();
    $('#modalAceptarSolicitud').on('hidden.bs.modal', function () {
        $("#motivo-confirmarSolictud").val("");
    })
    $('#modalRechazarSolicitud').on('hidden.bs.modal', function () {
        $("#motivo-rechazoSolictud").val("");
    })
    $('#modalRechazarSolicitudMasivo').on('hidden.bs.modal', function () {
        $("#motivo").val("");
    })
    $('#modalAceptarSolicitudMasivo').on('hidden.bs.modal', function () {
        $("#motivo-confirmarSolictudMasivo").val("");
    })

});
function CargarGrillaConfig() {
    var Centros = JSON.parse(document.getElementById('Centros').getAttribute('data-value'));
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
                    LimiteCupo: { type: "number" },
                    Proveedor: { type: "string" },
                    Comercial: { type: "string" },
                    Material: { type: "string" },
                    Zona: { type: "string" },
                    Centro: { type: "string" },
                    Fecha: { type: "date" },
                    Estado: { type: "string" },
                    FechaCreacion: { type: "date" },
                    FechaCreacionConHora: { type: "date" },
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "EstadoId", dir: "desc" }, { field: "Fecha", dir: "asc" }, { field: "FechaCreacion", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20,
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
            var grid = $("#gridInformeCompraNet").data("kendoGrid");
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].TipoAdministracionCupo == "Extraordinaria") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("tipoExtraordinario");
                }
            }
        },
        columns: [
            { selectable: true, width: "50px" },
            {
                field: "TipoAdministracionCupo", type: "string", title: "Tipo", width: 70,
                editable: function (dataItem) { return false; }, filterable: {
                    multi: true, dataSource: [{
                        TipoAdministracionCupo: "Extraordinaria"
                    }, {
                        TipoAdministracionCupo: "Algoritmo"
                    }]
                }, width: 130, template: "#=TipoAdministracionCupo#",
            },
            {
                field: "Proveedor", type: "string", minResizableWidth: 100, width: 150,
                editable: function (dataItem) { return false; },
                headerAttributes: { "class": classExterno }, attributes: { "id": "line", "class": classExterno },
                template: function (dataItem) {
                    if (dataItem.EstadoId == 4) {
                        return '<div class="statuspendiente "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 3) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 2) {
                        return '<div class="statuseliminado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 1) {
                        return '<div class="statuseliminado "></div>' + dataItem.Proveedor;
                    }
                },
                filterable: { ui: createMultiSelectProveedor, extra: false }
            },
            {
                field: "Comercial", type: "string", title: "Comercial", width: 150, editable: function (dataItem) {
                    return false;
                }, filterable: { ui: createMultiSelectComercial, extra: false }, headerAttributes: {
                    "class": classExterno
                },
                attributes: { "class": "mobile-xs " + classExterno }
            },
            {
                field: "Material", type: "string", minResizableWidth: 100, width: 150, editable: function (dataItem) {
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
                field: "Centro", type: "string", title: "Destino", minResizableWidth: 100, width: 150, editable: function (dataItem) {
                    return false;
                }, filterable: {
                    multi: true, dataSource: Centros.map(function (centro) {
                        return { Centro: centro };
                    })
                }, width: 130, template: "#=Centro#",
            },
            {
                field: "Fecha", title: "Fecha Sugerida", width: 150, format: _DefaultDateTemplate, type: "date", editable: function (dataItem) {
                    return false;
                }
            },
            {
                field: "FechaCreacionConHora", title: "Fecha Creacion", width: 150, template: function (dataItem) {
                    if (dataItem.FechaCreacion == null) {
                        return "";
                    }
                    return kendo.toString(dataItem.FechaCreacion, "dd/MM/yyyy") + " " + dataItem.Hora;
                },
                type: "date", editable: function (dataItem) {
                    return false;
                }
            },
            {
                field: "CantidadDeCupo", title: "Cantidad de Cupos", width: "150px",
                filterable: { extra: false },
                editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);
                    $("#CantidadDeCupo").val(options.model.CantidadDeCupo);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        max: options.model.CantidadDeCupoMax,
                        min: 0
                    });
                }
            },
            {
                field: "CantidadFleteProcedencia", title: "Cantidad Flete Procedencia", width: "150px",
                filterable: { extra: false },
                editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);

                    $("#CantidadCupoFlete").val(options.model.CantidadFleteProcedencia);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        max: options.model.CantidadFleteProcedenciaMax,
                        min: 0
                    });
                }
            },
            {
                field: "Observacion", type: "string", minResizableWidth: 100, width: 150, filterable: { extra: false }, editable: function (dataItem) { return false; },
            },
            {
                field: "Estado", title: "Estado", width: 150, editable: function (dataItem) {
                    return false;
                },
                filterable: {
                    multi: true,

                    dataSource: [
                        { Estado: "Aceptado" },
                        { Estado: "Rechazado" },
                        { Estado: "Pendiente" },
                        { Estado: "Anulado" }
                    ]
                },
                itemTemplate: function (e) {
                    return "<span><label><span>#= data.EstadoId || data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.EstadoId#'/></label></span>";
                }, template: function (dataItem) {
                    var iconoSustentable = dataItem.Sustentable == true ? botonSustentable('fa-solid fa-leaf') : '';
                    var iconoEPA = dataItem.EPA == true ? botonEPA('fa-pagelines') : '';
                    if (dataItem.EstadoId == 4) { //pendiente
                        return '<div class="status pendiente" style="text-align: center;">Pendiente'
                            + (dataItem.ConDescarga == true ? '  <i class="fa fa-truck" style="font-size: 15px" aria-hidden="true" title="Con Descarga"></i>' : '') +
                            '</div>' +
                            botonAprobar(dataItem, 'fa-check pend') +
                            botonBorrar(dataItem, 'fa-trash pend') +
                            iconoSustentable + iconoEPA;
                    }
                    if (dataItem.EstadoId == 3) { //confirmado                       
                        return '<div class="status confirmado" style="text-align: center;">Confirmado'
                            + (dataItem.ConDescarga == true ? '  <i class="fa fa-truck" style="font-size: 15px" aria-hidden="true" title="Con Descarga"></i>' : '') +
                            '</div>' +
                            iconoSustentable + iconoEPA;
                    }
                    if (dataItem.EstadoId == 2) { //Rechazado
                        return '<div class="status borrado" style="text-align: center;">Rechazado'
                            + (dataItem.ConDescarga == true ? '  <i class="fa fa-truck" aria-hidden="true" title="Con Descarga"></i>' : '')
                            + '</div>' +
                            iconoSustentable + iconoEPA;
                    }
                    if (dataItem.EstadoId == 1) { //anulado
                        return '<div class="status anulado" style="text-align: center;">Anulado'
                            + (dataItem.ConDescarga == true ? '  <i class="fa fa-truck" aria-hidden="true" title="Con Descarga"></i>' : '')
                            + '</div>' +
                            iconoSustentable + iconoEPA;
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
        scrollable: true,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        filterMenuOpen: function (e) {
            if (e.field == "Fecha" || e.field == "FechaCreacionConHora") {
                var beginOperator = e.container.find("[data-role=dropdownlist]:eq(0)").data("kendoDropDownList");
                beginOperator.value("gte");
                beginOperator.trigger("change")
                beginOperator.enable(false);
                var logicDropDown = e.container.find("select:eq(1)").data("kendoDropDownList");
                logicDropDown.value("and");
                logicDropDown.trigger("change");
                logicDropDown.wrapper.hide();
                console.log(e.container.find("select:eq(1)"));
                var endOperator = e.container.find("[data-role=dropdownlist]:eq(2)").data("kendoDropDownList");
                endOperator.value("lte");
                endOperator.trigger("change");
                endOperator.enable(false);
            }
        },
        filterable: {
            //height: 350,
            //extra: false,
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
                    neq: "Distinto",
                    startswith: "Comienza con",
                    contains: "Contiene",
                    endswith: "Finaliza con"
                },
                date: {
                    gte: "Desde",
                    lte: "Hasta"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                },
                bool: {
                    yesText: 'Yes',     // default
                    noText: 'No'        // default
                }
            }
        }

        //,
        //filter: defaultFilter
    });
    $("#gridInformeCompraNet").kendoTooltip({
        filter: "td",
        position: "top",
        content: function (e) {
            var dataItem = $("#gridInformeCompraNet").data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = dataItem.Observacion;
            return content;
        },
        show: function (e) {
            if (this.content.text() != "") {
                $('[role="tooltip"]').css("visibility", "visible");
            }
        },
        hide: function () {
            $('[role="tooltip"]').css("visibility", "hidden");
        }
    }).data("kendoTooltip");


    var fecha = new Date(); // Fecha actual
    fecha.setDate(fecha.getDate() - 7);
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    addOrRemoveFilter(grilla, "Fecha", "gte", fecha);
    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };
    function createMultiSelect(element, textField, valueField, url, columna, serverFiltering, filterType) {
        element.removeAttr("data-bind");
        columna = columna == null ? valueField : columna;
        serverFiltering = serverFiltering == null ? true : serverFiltering;
        filterType = filterType == null ? "starswith" : filterType;

        $(element).replaceWith('<select id="' + columna + '"></select>');
        InicializarMultiSelect(textField, valueField, url, columna, serverFiltering, filterType);

        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "Proveedor", "/CompraNet/ListarProveedorTodos");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial", "ComercialId", false, "contains");
    }

    function InicializarMultiSelect(textField, valueField, url, columna, serverFiltering, filterType) {
        console.log(textField, valueField, url, columna, serverFiltering, filterType);
        $("#" + columna).kendoMultiSelect({
            placeholder: "Seleccione " + textField + "...",
            dataTextField: textField,
            dataValueField: valueField,
            autoBind: false,
            dataSource: {
                serverFiltering: serverFiltering,
                transport: {
                    read: {
                        url: url,
                        data: function () {
                            return {
                                text: $("#" + columna).data("kendoMultiSelect").input.val()
                            };
                        },
                    }
                },
            },
            filter: filterType,
            change: function (e) {
                //var grilla = $('#gridInformeCompraNet').data("kendoGrid");
                //var values = this.value().filter(x => { return x !== '' });
                //if (values.length === 0) {
                //    removerFiltros(grilla, columna, "eq", "");
                //} else {
                //    AddFilters(grilla, columna, "eq", values);
                //}
                var grilla = $('#gridInformeCompraNet').data("kendoGrid");
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        addOrRemoveFilter(grilla, columna, "eq", v);
                    }
                });

                if (values.length === 0) {
                    addOrRemoveFilter(grilla, columna, "eq", null);
                }
                console.log(values);
            },

        });
    }
}

function botonAprobar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Confirmar" onclick="ModalAceptarSugerencia(' + dataItem.id + ')"><i class="fa ' + icono + '"></i></button>';
}
function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalRechazarSugerencia(' + dataItem.id + ') "><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
}
function botonSustentable(icono) {
    return '<button data-toggle="tooltip" title="Sustentable" disabled><i class="fa ' + icono + '"></i></button>';
}
function botonEPA(icono) {
    return '<button data-toggle="tooltip" title="EPA" disabled><i class="fa ' + icono + '"></i></button>';
}

function ModalAceptarSugerencia(id) {
    $("#modalAceptarSolicitud").modal("show");
    var grid = $("#gridInformeCompraNet").data("kendoGrid").dataSource.data();
    var solicitudSeleccionada = grid.filter(function (x) { return (x.Id == id) });
    $("#CantidadDeCupoAceptado").val(solicitudSeleccionada[0].CantidadDeCupo);
    $("#CantidadCupoFleteAceptado").val(solicitudSeleccionada[0].CantidadFleteProcedencia);
    $("#CantidadDeCupoFleteOriginal").val(solicitudSeleccionada[0].CantidadFleteProcedenciaOriginal);
    $("#CantidadDeCupoOriginal").val(solicitudSeleccionada[0].CantidadDeCupoOriginal);
    $("#solicitudId").val(id);
}
function ModalRechazarSugerencia(id) {
    $("#modalRechazarSolicitud").modal("show");
    $("#solicitudId").val(id);

}
function AceptarSolicitud() {
    var id = $("#solicitudId").val();
    var motivo = $("#motivo-confirmarSolictud").val();
    var cantidadOriginal = $("#CantidadDeCupoOriginal").val();
    var cantidadFleteOriginal = $("#CantidadDeCupoFleteOriginal").val();

    var cantidad = $("#CantidadDeCupoAceptado").val();
    if (cantidad === undefined) {
        cantidad = 0;
    }
    var cantidadFp = $("#CantidadCupoFleteAceptado").val();
    if (cantidadFp === undefined) {
        cantidadFp = 0;
    }
    if (cantidad == 0 && cantidadFp == 0) {
        MensErr("La solicitud no se puede aceptar");
        return;
    }
    BlockUi('Procesando...');
    setTimeout(
        function () {
            result = MSExecuteOnServer('/AdministracionCupo/Aceptar', { administracionId: id, cantidadCupo: cantidad, cantidadFleteProcedencia: cantidadFp, cantidadOriginal: cantidadOriginal, cantidadFleteOriginal: cantidadFleteOriginal, motivo: motivo });

            $.unblockUI();
            var errores = new Array();
            var cuposGenerados = new Array();
            if (result.HayError) {
                errores = errores.concat(result.ListaErrores);
            }
            if (errores.length == 0) {
                MensInfo("Se grabo correctamente.");
            } else {
                ShowErrorMessages(errores);
            }
            recargarGrilla();
            $.unblockUI();
        }
        , 200);

}


function RechazarSolicitud() {
    var id = $("#solicitudId").val();
    var motivo = $("#motivo-rechazoSolictud").val();
    BlockUi('Procesando...');
    setTimeout(
        function () {
            result = MSExecuteOnServer('/AdministracionCupo/Rechazar', { administracionId: id, motivo: motivo });
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
            $.unblockUI();
        }
        , 200);

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

function AutoRecargarSolicitudes() {
    setInterval(function () {
        if ($('#checkRecarga').is(":checked") == true) {
            recargarGrilla();
            $("#panel").html(MSExecuteURLOnServer('/AdministracionCupo/PartialPanel'));
        }
    }, 60000);

}

function Recargar() {
    BlockUi('Cargando...');
    setTimeout(function () {
        recargarGrilla();
        $("#panel").html(MSExecuteURLOnServer('/AdministracionCupo/PartialPanel'));
        $.unblockUI();
    }, 250);

}

function SeleccionarElementos() {
    var grid = $("#gridInformeCompraNet").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        if (selectedItem.EstadoId == 4) {
            obj.push(selectedItem);
        }
    });
    return obj;
}
function DeseleccionarElementos() {
    var grid = $("#gridInformeCompraNet").data("kendoGrid");
    grid.clearSelection();
}

function AbrilModalMasivo() {
    $("#modalRechazarSolicitudMasivo").modal("show");
}

function RechazarMasivo() {
    BlockUi('Procesando...');
    var solicitudes = SeleccionarElementos();
    var motivo = $("#motivo").val();
    var ids = [];
    //for (var i = 0; i < configuraciones.length; i++) {
    //    ids.push(configuraciones[i].id);
    //}
    if (solicitudes.length <= 0) {
        MensErr("No se seleccionó ninguna solicitud pendiente.");
    } else {
        var limites = MSExecuteOnServer("/AdministracionCupo/RechazarMasivo", { solicitudes: solicitudes, motivo: motivo });
        MensInfo("Se guardó correctamente");
        recargarGrilla();
    }
    $.unblockUI();
}
function AbrilConfirmarModalMasivo() {
    $("#modalAceptarSolicitudMasivo").modal("show");
}

function AceptarMasivo() {
    BlockUi('Procesando...');
    var motivo = $("#motivo-confirmarSolictudMasivo").val();
    setTimeout(
        function () {
            var solicitudes = SeleccionarElementos();
            var ids = [];
            if (solicitudes.length <= 0) {
                MensErr("No se seleccionó ninguna solicitud pendiente.");
            } else {
                var result = MSExecuteOnServer("/AdministracionCupo/AceptarMasivo", { solicitudes: solicitudes, motivo: motivo });
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
                    errores.forEach(function (e) {
                        e.Message += '<br>';
                    });
                    ShowErrorMessages(errores);
                }
                recargarGrilla();

            }
            $.unblockUI();
        }
        , 200);


}
