var externo;
var dataTabla = [];
var stockDisponible = true;
//var stockInsuficiente = false;
var negocioIdSE, contratoSapSE, kgPendientes, cuposRestantes;

$(document).ready(function () {
    $('#menuproveedor').hide();
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    CargarGrilla();
    $('#CargaCupos').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
        $(".cantidad-masiva-cupos").val($("#cantidad").data('kendoNumericTextBox').value());
        $("#boton-carga-masiva").hide();
        dataTabla = [];
        var table = document.getElementById("cargaMasiva-cupos-table");
        for (var i = 1; i < table.rows.length; i++) {
            table.deleteRow(i);
        }
    });

    $('#modalSolicitudExtraordinaria').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
        var datepicker = $("#FechaHastaSE").data("kendoDatePicker");
        datepicker.value("");
        $("#boton-carga-masiva").hide();
        dataTabla = [];
        var table = document.getElementById("cargaMasiva-cupos-table");
        for (var i = 1; i < table.rows.length; i++) {
            table.deleteRow(i);
        }
        $('#grid').empty();
        $("#gridContainer").hide();
    });
    AutoRecargarSolicitudes();
    InicializarElementos();

    $("#CantidadCupoSE").change(function () {
        //console.log($(this).val());
        var table = document.getElementById("cargaMasiva-cupos-table");
        for (var i = 0; i < table.rows.length - 1; i++) {
            $('[name="Dias[' + i + '].Cantidad"]').data('kendoNumericTextBox').value($(this).val());
        }
    });
});
function CargarGrilla() {
    var Centros = JSON.parse(document.getElementById('Centros').getAttribute('data-value'));
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/AdministracionCupo/BuscarDatosSolicitudCupo',
                data: additionalInfo()
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
                    Observacion: { type: "string" },
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "EstadoId", dir: "desc" }, { field: "Fecha", dir: "asc" }],
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
        },
        columns: [
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
                field: "Proveedor", type: "string", width: 150,
                editable: function (dataItem) {
                    return false;
                },
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
                        Material: "Sorgo"
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
                }, attributes: { "class": "mobile-xs mobile-md" }, filterable: {
                    multi: true, dataSource: Centros.map(function (centro) {
                        return { Centro: centro };
                    })
                }, width: 130, template: "#=Centro#",
            },
            {
                field: "Fecha", title: "Fecha Solicitud", type: "date", editable: function (dataItem) {
                    return false;
                }, format: _DefaultDateTemplate
            },
            {
                field: "FechaCreacion", title: "Fecha Creación", type: "date", editable: function (dataItem) {
                    return false;
                }, format: _DefaultDateTemplate
            },
            {
                field: "CantidadDeCupo", title: "Cantidad de Cupos", width: "110px",
                filterable: { extra: false },
                editable: function (dataItem) {
                    return (dataItem.EstadoId == 3 && dataItem.TipoAdministracionCupo == "Extraordinaria") ? true : false;
                },
                editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);
                    $("#CantidadDeCupo").val(options.model.CantidadDeCupo);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        //max: options.model.CantidadDeCupoMax,
                        min: 0,
                        change: function () {
                            ActualizarSolicitud(options.model.Id)
                        },
                    });
                }
            },
            {
                field: "CantidadFleteProcedencia", title: "Cantidad Flete Procedencia",
                filterable: { extra: false },
                editable: function (dataItem) {
                    return (dataItem.EstadoId == 3 && dataItem.TipoAdministracionCupo == "Extraordinaria") ? true : false;
                },
                editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);

                    $("#CantidadCupoFlete").val(options.model.CantidadFleteProcedencia);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        //max: options.model.CantidadFleteProcedenciaMax,
                        min: 0,
                        change: function () {
                            ActualizarSolicitud(options.model.Id)
                        },
                    });
                }
            },
            {
                field: "Observacion", type: "string", title: "Observación", editable: function (dataItem) {
                    return false;
                },
                headerAttributes: { "class": classExterno },
                attributes: { "class": "mobile-xs " + classExterno }
            },
            {
                field: "Estado", title: "Estado", editable: function (dataItem) {
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
                    var estadoContent = ''
                    var iconoSustentable = dataItem.Sustentable == true ? botonSustentable('fa-solid fa-leaf') : '';
                    var iconoEPA = dataItem.EPA == true || dataItem.EUDR == true ? botonEPA('fa-pagelines') : '';
                    if (dataItem.EstadoId == 4) { //pendiente
                        if (dataItem.ConDescarga == true) {
                            return '<div class="status pendiente" style="text-align: center;">Pendiente <i class="fa fa-truck" aria-hidden="true" title="Con Descarga"></i></div>' +
                                botonBorrar(dataItem, 'fa-trash pend') +
                                iconoSustentable + iconoEPA;
                        } else {
                            return '<div class="status pendiente" style="text-align: center;">Pendiente</div>' +
                                botonBorrar(dataItem, 'fa-trash pend') +
                                iconoSustentable + iconoEPA;
                        }
                    }
                    if (dataItem.EstadoId == 3) { //aceptado
                        estadoContent += '<div class="status confirmado">Aceptado';
                        //return '<div class="status confirmado">Confirmado</div>';
                    }
                    if (dataItem.EstadoId == 2) { //Rechazado
                        estadoContent += '<div class="status borrado">Rechazado';
                        //return '<div class="status borrado">Rechazado</div>';
                    }
                    if (dataItem.EstadoId == 1) { //anulado
                        estadoContent += '<div class="status anulado">Anulado';
                    }
                    if (dataItem.ConDescarga == true) {
                        estadoContent += '  <i class="fa fa-truck" aria-hidden="true" title="Con Descarga"></i>'
                    }
                    estadoContent += '</div>'
                    return estadoContent + iconoSustentable + iconoEPA;
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
        editable: true,
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

        //,
        //filter: defaultFilter
    });

    //var fecha = new Date();
    //fecha.setDate(fecha.getDate() - 7);
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    //addOrRemoveFilter(grilla, "Fecha", "gte", fecha);
    addOrRemoveFilter(grilla, "FechaCreacion", "gte", new Date());

    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };
}
function createMultiSelect(element, textField, valueField, url, columna) {
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
                addOrRemoveFilter(grilla, columna, "eq", null);
            }
        }
    });
    setTimeout(function () {
        $(".k-multiselect").parent().children(".k-dropdown").remove();
        $(".k-multiselect").parent().children("div").find('button').remove();
    }, 200);
}

function createMultiSelectComercial(element) {
    return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial");
}
function createMultiSelectProveedor(element) {
    return createMultiSelect(element, "Proveedor", "Proveedor", "/CompraNet/ListarProveedorTodos");
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
        if (document.getElementById('checkRecarga').checked == true) {
            recargarGrilla();
        }
    }, 30000);
}

function InicializarElementos() {
    $("#CalidadIdSE").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value"
    });
    $("#MaterialIdSE").kendoDropDownList({
        change: function () {
            checkSoja();
            MostrarVisualizarStock();
            $("#ContratoSE").data("kendoNumericTextBox").value('');
            $("#gridContainer").hide();
        }
    });
    $("#CentroIdSE").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        change: function () {
            checkSoja();
            MostrarVisualizarStock();
        }
    });

    $("#ComercialIdSE").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        filter: "contains",
        popup: {
            appendTo: $("#modalSolicitudExtraordinaria"),
        }
    });

    function changeFechaSE() {
        CrearTablaFechaHasta();

        $("#FechaHastaSE").data("kendoDatePicker").value("");
        var datepicker = $("#FechaHastaSE").data("kendoDatePicker");
        datepicker.min(kendo.parseDate($("#FechaSE").val()));
        datepicker.value(kendo.parseDate($("#FechaSE").val()));
    }

    $("#FechaSE").kendoDatePicker({
        min: new Date(),
        close: function () {
            changeFechaSE();
        },
        change: function () {
            changeFechaSE();
        }
    });

    $("#FechaHastaSE").kendoDatePicker({
        min: kendo.parseDate($("#FechaSE").val()),
        change: function () {
            CrearTablaFechaHasta();

            var fechaSEString = $("#FechaSE").val();
            var fechaHastaSEString = $("#FechaHastaSE").val();
            // Convertimos las fechas de formato string a objetos Date
            var fechaSE = new Date(fechaSEString.split('/').reverse().join('/'));
            var fechaHastaSE = new Date(fechaHastaSEString.split('/').reverse().join('/'));

            if ((fechaSE < fechaHastaSE) && $("#FechaHastaSE").val() != "") {
                $("#boton-carga-masiva").show();
            } else $("#boton-carga-masiva").hide();
        }
    });

    $("#MaterialIdSE").change(function () {
        checkSoja();
    });

    $("#Sustentable").change(function () {
        if ($("#Sustentable").is(':checked')) {
            $("#EPA").prop("checked", false).prop("disabled", false);
            $("#EUDR").prop("checked", false).prop("disabled", false);
        }
        MostrarVisualizarStock();
        buscarContratoSE();
    });

    $("#EPA").change(function () {
        if (ActivarSojaTwin == "1") {
            if ($("#EPA").is(':checked')) {
                $("#EUDR").prop("checked", true).prop("disabled", true);
                $("#Sustentable").prop("checked", false);
            } else {
                $("#EUDR").prop("checked", false).prop("disabled", false);
            }
        } else {
            if ($("#EPA").is(':checked')) {
                $("#EUDR").prop("checked", false);
                $("#Sustentable").prop("checked", false);
            }
        }
        MostrarVisualizarStock();
        buscarContratoSE();
    });

    $("#EUDR").change(function () {
        if (ActivarSojaTwin == "1") {
            if ($("#EUDR").is(':checked')) {
                $("#EPA").prop("checked", true).prop("disabled", true);
                $("#Sustentable").prop("checked", false);
            } else {
                $("#EPA").prop("checked", false).prop("disabled", false);
            }
        } else {
            if ($("#EUDR").is(':checked')) {
                $("#EPA").prop("checked", false);
                $("#Sustentable").prop("checked", false);
            }
        }
        MostrarVisualizarStock();
        buscarContratoSE();
    });

    $("#ContratoSE").kendoNumericTextBox({
        culture: "es-AR",
        format: "0:0",
        spinners: false,
        min: 0
    });
}

function CrearTablaFechaHasta() {
    $(".fila-carga").remove();

    var date1 = $("#FechaSE").val();
    var date2 = $("#FechaHastaSE").val();
    var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

    for (var i = 0; i <= diffDays; i++) {

        var fila = '<tr class="fila-carga">' +
            '<input name="Dias[' + i + '].Fecha" value="' + date1 + '" type="hidden"/>' +
            '<td>' + date1 + '</td>' +
            '<td><input name="Dias[' + i + '].Cantidad" class="cantidad-masiva-cupos" value="' + $("#CantidadCupoSE").data('kendoNumericTextBox').value() + '"/></td>' +
            '</tr>';
        $("#cargaMasiva-cupos-table").append(fila);

        var newdate = kendo.parseDate(date1);
        newdate.setDate(newdate.getDate() + 1);
        var dd = newdate.getDate();
        var mm = newdate.getMonth() + 1;
        var y = newdate.getFullYear();

        date1 = dd + '/' + mm + '/' + y;
    }
    $(".cantidad-masiva-cupos").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#cancelar-carga").click(function () {
        //$(".cantidad-masiva-cupos").val($("#cantidad").data('kendoNumericTextBox').value());
        //$("#boton-carga-masiva").hide();
        $("#cancelar-carga").unbind('click');
    });

    $('[name="Dias[0].Cantidad"]').change(function () {
        if ($("#cantidad").val() == 0) {
            $("#cantidad").data('kendoNumericTextBox').value($('[name="Dias[0].Cantidad"]').val());
            $("#cantidad").data("kendoNumericTextBox").trigger("change");
        }
    });
}

function MostrarCarga() {
    $("#CargarCupos").modal('toggle');
}

function LimpiarModalSolicitudExtraordinaria() {
    //$("#CalidadIdSE").data("kendoDropDownList").value('');
    //$("#MaterialIdSE").data("kendoDropDownList").value('');
    $("#buscadorProveedorSE").click();
    $("#ProveedorIdSE").val('');
    $("#FechaSE").val('');
    //$("#ComercialIdSE").data("kendoDropDownList").value('');
    $("#ObservacionSE").val('');
    $("#CantidadCupoSE").data("kendoNumericTextBox").value('');
    $("#ContratoSE").data("kendoNumericTextBox").value('');
    $("#CuitES").val('');
    $("#CuitES").hide();
    $("#FasonES").prop("checked", false);
    $("#FleteAcarreoES").prop("checked", false);
    checkSoja();
    $("#Sustentable").prop("checked", false);
    $("#EPA").prop("checked", false);
    $("#EUDR").prop("checked", false);

    // Si tiene permiso "Cupos_Acopios", establecer valores de "ProveedorIdSE" y buscadorProveedorSE
    if (Cupos_Acopios == "True") {
        $("#ProveedorIdSE").val(2852);
        $("#buscadorProveedorSE").val("MOLINOS AGRO S.A.(30715118773)");
        $("#buscadorProveedorSE").trigger("change");
        $("#buscadorProveedorSE").prop("disabled", true);
        $(".k-clear-value").hide();
    }
}

function AbrirModalSolicitudExtraordinaria() {
    LimpiarModalSolicitudExtraordinaria();
    $("#modalSolicitudExtraordinaria").modal("show");
}

function checkSoja() {
    if ($("#MaterialIdSE").val() !== "3") {
        $("#calidadDivSE").hide();
        $("#CalidadIdSE").data("kendoDropDownList").value("");

        $("#sustentableDivSE").hide();
        $("#Sustentable").prop("checked", false);
        $("#EPADivSE").hide();
        $("#EPA").prop("checked", false).prop("disabled", false);
        $("#EUDRDivSE").hide();
        $("#EUDR").prop("checked", false).prop("disabled", false);
    } else {
        $("#calidadDivSE").show();
        $("#sustentableDivSE").show();
        $("#EPADivSE").show();
        $("#EUDRDivSE").show();
    }
}

function ActualizarCantidad(cantidadDias) {
    $(document).ready(function () {
        for (var i = 0; i < cantidadDias.length; i++) {
            $('[name="Dias[' + i + '].Cantidad"]').data('kendoNumericTextBox').value(cantidadDias[i].Cantidad);
        }
    });
}

function grabarSolicitudExtraordinaria() {
    if ($("#MaterialIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione el Material"); return;
    }
    if ($("#CentroIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione el Destino"); return;
    }
    if ($("#MaterialIdSE").data("kendoDropDownList").value() == "3" && $("#CalidadIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione la Calidad"); return;
    }
    if ($("#FechaSE").data("kendoDatePicker").value() == null) {
        MensErr("Seleccione una Fecha Desde mayor o igual al día actual."); return;
    }
    if ($("#FechaSE").data("kendoDatePicker").value() < new Date().setHours(0, 0, 0, 0)) {
        MensErr("Seleccione una Fecha Desde mayor o igual al día actual."); return;
    }
    if ($("#FechaHastaSE").data("kendoDatePicker").value() < $("#FechaSE").data("kendoDatePicker").value().setHours(0, 0, 0, 0)) {
        MensErr("La Fecha Hasta no puede ser menor a la Fecha Desde, ni puede estar vacía."); return;
    }
    if ($("#ProveedorIdSE").val() == "") {
        $("#buscadorProveedorSE").click();
        MensErr("Seleccione un Proveedor"); return;
    }
    if ($("#ObservacionSE").val().length > 500) {
        MensErr("La observación es muy larga: 500 caracteres como máximo."); return;
    }
    if ($("#ComercialIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione el Comercial"); return;
    }
    if ($("#CantidadCupoSE").data("kendoNumericTextBox").value() == null || $("#CantidadCupoSE").data("kendoNumericTextBox").value() == 0) {
        MensErr("Ingrese la cantidad de cupos."); return;
    }
    if ($("#FasonES").is(':checked') && $("#CuitES").val() == "") {
        MensErr("Complete el CUIT del destinatario."); return;
    }

    if ($("#buscadorProveedorSE").val() != "" && ($("#Sustentable").is(':checked') || $("#EPA").is(':checked') || $("#EUDR").is(':checked')) && $("#CentroIdSE").val() == "1029" && $("#MaterialIdSE").val() == "3" && grupoSegmentacion == "Productores") {
        VisualizarStock(true);
        if (!stockDisponible) {
            MensErr("No se pudo guardar porque no existen establecimientos con stock disponible.");
            return;
        }
        //else if (stockInsuficiente) {
        //    MensErr("Los establecimientos no cuentan con stock suficiente.");
        //    return;
        //}
    } else {
        stockDisponible = true;
        //stockInsuficiente = false;
    }

    dataTabla = [];
    var table = document.getElementById("cargaMasiva-cupos-table");
    for (let i = 0, n = table.rows.length; i < (n - 1); i++) {
        let row = table.rows[i]
        if ($('[name="Dias[' + i + '].Cantidad"]').val() > 0) {
            dataTabla.push({ Fecha: $('[name="Dias[' + i + '].Fecha"]').val(), Cantidad: $('[name="Dias[' + i + '].Cantidad"]').val() })
        }
        //if ($('[name="Dias[' + i + '].Cantidad"]').val() < 1) {
        //    MensErr("Ingrese la cantidad de cupos para el " + $('[name="Dias[' + i + '].Fecha"]').val() ); return;
        //}
    }
    if (dataTabla.length == 0 && table.rows.length > 2) {
        MensErr("Ingrese la cantidad de cupos para alguno de los días."); return;
    }

    BlockUi('Grabando...');
    setTimeout(function () {
        var solicitud = {
            ProveedorId: $("#ProveedorIdSE").val(),
            Proveedor: $("#buscadorProveedorSE").val(),
            ComercialId: $("#ComercialIdSE").data("kendoDropDownList").value(),
            Fecha: $("#FechaSE").data("kendoDatePicker").value(),
            MaterialId: $("#MaterialIdSE").data("kendoDropDownList").value(),
            CantidadCupo: $("#FleteAcarreoES").is(':checked') ? 0 : $("#CantidadCupoSE").data("kendoNumericTextBox").value(),
            CantidadFleteProcedencia: $("#FleteAcarreoES").is(':checked') ? $("#CantidadCupoSE").data("kendoNumericTextBox").value() : 0,
            TipoAdministracionCupoId: 2,
            Fason: $("#FasonES").is(':checked'),
            Destinatario: $("#CuitES").val(),
            Observacion: $("#ObservacionSE").val(),
            CentroId: $("#CentroIdSE").data("kendoDropDownList").value(),
            Calidad: $("#MaterialIdSE").data("kendoDropDownList").value() == 3 ? $("#CalidadIdSE").data("kendoDropDownList").text() : "",
            ConDescarga: $("#ConDescarga").is(':checked'),
            Sustentable: $("#Sustentable").is(':checked'),
            EPA: $("#EPA").is(':checked'),
            EUDR: $("#EUDR").is(':checked'),
            Dias: dataTabla,
            ContratoSAP: contratoSapSE,
            NegocioId: negocioIdSE,
            KgPendientes: kgPendientes,
            CuposRestantes: cuposRestantes
        };
        result = MSExecuteOnServer('/SugerenciaCupo/GenerarSolicitudExtraordinaria', solicitud);
        ListarRespuesta(result);
        if (!result.HayError) {
            $("#boton-carga-masiva").hide();
        }
        $.unblockUI();
    }, 250);
}

function ListarRespuesta(result) {
    $.unblockUI();
    var erroresTabla = new Array();
    var cuposGeneradosTabla = new Array();

    if (result.ListaCupos != null && result.ListaCupos.length > 0) {
        cuposGeneradosTabla = cuposGeneradosTabla.concat(result.ListaCupos);
        $("#modalSolicitudExtraordinaria").modal("hide");
        $('#grid').empty();
        $("#gridContainer").hide();
    }
    else if (result.ListaErrores != null && result.ListaErrores.length > 0) {
        for (var i = 0; i < result.ListaErrores.length; i++) {
            cuposGeneradosTabla = cuposGeneradosTabla.concat(result.ListaErrores[i].Message);
            //$("#modalSolicitudExtraordinaria").modal("hide");
        }
    } else {
        $("#modalSolicitudExtraordinaria").modal("hide");
        $('#grid').empty();
        $("#gridContainer").hide();
        MensInfo("La solicitud se generó correctamente.");
    }
    if (result.ListaCupos != null && result.ListaCupos.length == 0 &&
        result.ListaErrores != null && result.ListaErrores.length > 0) {
        MensErr(result.ListaErrores[0].Message);
    } else {
        if (cuposGeneradosTabla.length > 0) {
            $("#cupos-generados-modal-solicitud").html(cuposGeneradosTabla.join("</br>"));
            $('#resultadoCupoSolicitud').modal('toggle');
        }
    }

    //if (result.HayError && cuposGeneradosTabla.length < 0) {
    //    for (var i = 0; i < result.ListaErrores.length; i++) {
    //        erroresTabla = erroresTabla.concat(result.ListaErrores[i].Message);
    //    }

    //}
    //if (erroresTabla.length > 0 && cuposGeneradosTabla.length < 0) {
    //    ShowErrorMessages(erroresTabla);
    //}
}

function checkFason() {
    if ($("#FasonES").is(':checked')) {
        $("#CuitES").show();
        var cuitAux = $("#buscadorProveedorSE").val().split('(');
        if (cuitAux[1] != null) {
            var cuit = cuitAux[1].split(')');
        }
        else {
            cuit = cuitAux;
        }
        $("#CuitES").val(cuit[0]);
    }
    else {
        $("#CuitES").hide();
        $("#CuitES").val("");
    }
}

function copiarTablaEstablecimiento() {
    //Para copiar la tabla como una imagen:
    html2canvas($("#cargarDatosEstablecimiento")[0]).then(function (canvas) {
        let image = new Image();
        image.src = canvas.toDataURL();
        $("#out_image").append(image);
        copyImage(image.src);
        $("#out_image").empty();
    }
    );

    /* Para copiarla en formato texto: 
    var copiarEstablecimientos = document.getElementById("cargarDatosEstablecimiento").innerText;

    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', copiarEstablecimientos);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', copiarEstablecimientos);
        }
    };
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
    */
}

async function copyImage(imageURL) {
    const blob = await imageToBlob(imageURL)
    const item = new ClipboardItem({ "image/png": blob });
    navigator.clipboard.write([item]);
}

function imageToBlob(imageURL) {
    const img = new Image;
    const c = document.createElement("canvas");
    const ctx = c.getContext("2d");
    img.crossOrigin = "";
    img.src = imageURL;
    return new Promise(resolve => {
        img.onload = function () {
            c.width = this.naturalWidth;
            c.height = this.naturalHeight;
            ctx.drawImage(this, 0, 0);
            c.toBlob((blob) => {
                // here the image is a blob
                resolve(blob)
            }, "image/png", 0.75);
        };
    })
}

function VisualizarStock(noabrir) {
    var esEPAoEUDR = $("#EPA").is(':checked') || $("#EUDR").is(':checked');
    var cuitProv = $("#buscadorProveedorSE").val().split('(');
    if (cuitProv[1] != null) {
        var cuitP = cuitProv[1].split(')');
    }
    else {
        cuitP = cuitProv;
    }
    var result = MSExecuteOnServer('/Cupo/TraerEstablecimientos', { cuitProveedor: cuitP[0], esEPAoEUDR: esEPAoEUDR });
    if (result != null && result.length > 0) {
        stockDisponible = true;
        //var cantidadCuposEstablecimientos = 0;
        //var cantidadKilosEstablecimientos = 0;
        var table = "<tr>";
        table += '<th colspan = "3">Cosecha ' + result[0].Cosecha + '</th>';
        table += "</tr>";
        table += "<tr>";
        table += "<th> Establecimiento</th>"
        table += "<th> Cantidad (Kg)</th>"
        table += "<th> Localidad (Provincia) </th>"
        table += "</tr>";
        for (var i = 0; i < result.length; i++) {
            table += "<tr>";

            table += '<td>' + result[i].Establecimiento + '</td>';
            table += '<td>' + kendo.toString(result[i].Cantidad, "n0") + '</td>';
            table += '<td>' + result[i].Localidad + ' (' + result[i].Provincia + ')' + '</td>';
            table += "</tr>";

            //cantidadKilosEstablecimientos += result[i].Cantidad;
        }

        //stringToDate("17/9/2014", "dd/MM/yyyy", "/");
        //var fechaDesde = stringToDate($("#FechaSE")[0].value, "dd/MM/yyyy", "/");
        //var fechaHasta = stringToDate($("#FechaHastaSE")[0].value, "dd/MM/yyyy", "/");
        //var cantidadDias = ((fechaHasta.getTime() - fechaDesde.getTime()) / 86400000) + 1;
        //var cantidadCuposDias = parseInt($("#CantidadCupoSE")[0].value) * cantidadDias;

        //cantidadCuposEstablecimientos = cantidadKilosEstablecimientos / 30000;

        //if (cantidadCuposDias > cantidadCuposEstablecimientos) {
        //    stockInsuficiente = true;
        //    if (noabrir != true) {
        //        MensErr("Los establecimientos no cuentan con stock suficiente.")
        //    }
        //} else {
        //    stockInsuficiente = false;
        //}

        $("#cargarDatosEstablecimiento").html(table);
        if (noabrir != true) {
            $("#modalEstablecimientos").modal("show");
        }
    } else {
        stockDisponible = false;
        if (noabrir != true) {
            MensErr("No se encontraron establecimientos con stock disponible.")
        }
    }
}

function MostrarVisualizarStock() {
    if ($("#buscadorProveedorSE").val() != "" && ($("#Sustentable").is(':checked') || $("#EPA").is(':checked') || $("#EUDR").is(':checked')) && $("#CentroIdSE").val() == "1029" && $("#MaterialIdSE").val() == "3") {
        $("#stock").show();
    } else {
        $("#stock").hide();
    }
}

function ObtenerIdSolicitudSeleccionada(id) {
    var grid = $("#gridInformeCompraNet").data("kendoGrid").dataSource.data();
    return grid.filter(function (x) { return (x.Id == id) });
}

function ActualizarSolicitud(id) {

    var solicitudSeleccionada = ObtenerIdSolicitudSeleccionada(id)
    $("#CantidadDeCupoAceptado").val(solicitudSeleccionada[0].CantidadDeCupo);
    $("#CantidadCupoFleteAceptado").val(solicitudSeleccionada[0].CantidadFleteProcedencia);

    var cantidad = $("#CantidadDeCupoAceptado").val();
    if (cantidad === undefined) {
        cantidad = 0;
    }
    var cantidadFp = $("#CantidadCupoFleteAceptado").val();
    if (cantidadFp === undefined) {
        cantidadFp = 0;
    }
    if (cantidad == 0 && cantidadFp == 0) {
        MensErr("Ingrese una cantidad de cupos válida.");
        recargarGrilla();
        return;
    }

    BlockUi('Procesando...');
    setTimeout(
        function () {
            result = MSExecuteOnServer('/AdministracionCupo/ActualizarSolicitud', { id: id, cantidadCupo: cantidad, cantidadFlete: cantidadFp, estado: false });
            if (result != "Ok") {
                MensErr("La solicitud no puede editarse");
            }
            recargarGrilla();
            $.unblockUI();
        }, 200);
}

function AbrirModalRechazar(id) {
    $("#modalAnularSolicitud").modal("show");
    $("#solicitudId").val(id);
}


function AnularSolicitud() {
    BlockUi('Procesando...');
    var id = $("#solicitudId").val();
    setTimeout(
        function () {
            result = MSExecuteOnServer('/AdministracionCupo/ActualizarSolicitud', { id: id, cantidadCupo: 0, cantidadFlete: 0, estado: true });
            if (result == "Ok") {
                MensInfo("Guardado Correctamente");
            } else {
                MensErr("La solicitud no puede ser anulada");
            }
            recargarGrilla();
            $.unblockUI();
        }, 200);
}

function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Anular" style="color: #ffc100;" onclick="AbrirModalRechazar(' + dataItem.Id + ') "><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
}
function botonSustentable(icono) {
    return '<button data-toggle="tooltip" title="Sustentable" disabled><i class="fa ' + icono + '"></i></button>';
}
function botonEPA(icono) {
    return '<button data-toggle="tooltip" title="EPA/EUDR" disabled><i class="fa ' + icono + '"></i></button>';
}

function OcultarCargaMasiva() {

    var datepicker = $("#FechaHastaSE").data("kendoDatePicker");
    datepicker.value("");
    $("#boton-carga-masiva").hide();
}

function stringToDate(_date, _format, _delimiter) {
    var formatLowerCase = _format.toLowerCase();
    var formatItems = formatLowerCase.split(_delimiter);
    var dateItems = _date.split(_delimiter);
    var monthIndex = formatItems.indexOf("mm");
    var dayIndex = formatItems.indexOf("dd");
    var yearIndex = formatItems.indexOf("yyyy");
    var month = parseInt(dateItems[monthIndex]);
    month -= 1;
    var formatedDate = new Date(dateItems[yearIndex], month, dateItems[dayIndex]);
    return formatedDate;
}

function buscarContratoSE() {
    var contratoSap = $("#ContratoSE").val();
    var proveedorId = $("#ProveedorIdSE").val();
    var materialId = $("#MaterialIdSE").data("kendoDropDownList").value();
    var estadoId = $('input[name="estadoContrato"]:checked').val();
    var sustentable = $("#Sustentable").is(':checked');
    var epa = $("#EPA").is(':checked');
    var eudr = $("#EUDR").is(':checked');
    contratoSapSE = null; negocioIdSE = null; kgPendientes = 0; cuposRestantes = 0;

    if (!proveedorId || !materialId) {
        MensInfo("Elija un proveedor y material para buscar contratos.");
        return;
    }
    $("body").css("cursor", "wait");
    $("#noResultsMessage").hide();
    $("#gridContainer").show();
    $("#grid").html("<div class='loading-message'>Buscando...</div>").show();

    $.ajax({
        url: '/Cupo/ListarNegociosParaSolicitarCupo',
        type: 'GET',
        data: { contratoSap: contratoSap, proveedorId: proveedorId, materialId: materialId, estadoId: estadoId, sustentable: sustentable, epa: epa, eudr: eudr },
        success: function (result) {
            $("#grid").empty();
            if (result && result.length > 0) {
                $("#grid").show().kendoGrid({
                    dataSource: {
                        data: result,
                        schema: {
                            model: {
                                fields: {
                                    ContratoSAP: { type: "string" },
                                    NegocioId: { type: "number" },
                                    RazonSocialProveedor: { type: "string" },
                                    EstadoNegocio: { type: "string" },
                                    KgPendientes: { type: "number" },
                                    CuposRestantes: { type: "number" },
                                    FechaHasta: { type: "date" }
                                }
                            }
                        }
                    },
                    height: 150,
                    contentHeight: 150,
                    scrollable: true,
                    selectable: "row",
                    sortable: true,
                    columns: [
                        { field: "NegocioId", title: "ID interno", width: 80, attributes: { style: "text-align: center;" } },
                        { field: "ContratoSAP", title: "Contrato SAP", width: 110, attributes: { style: "text-align: center;" } },
                        { field: "RazonSocialProveedor", title: "Proveedor", width: 150 },
                        { field: "KgPendientes", title: "Kilos pend.", format: "{0:N2}", width: 100 },
                        { field: "CuposRestantes", title: "Cupos pend.", width: 100, attributes: { style: "text-align: center;" }, headerAttributes: { "title": "Disponibles según kilos pend. menos solicitudes pend. y cupos generados" } },
                        { field: "FechaHasta", title: "Fecha Hasta", width: 100, format: "{0:dd/MM/yyyy}", attributes: { style: "text-align: center;" } },
                        { field: "EstadoNegocio", title: "Estado", width: 90, attributes: { style: "text-align: center;" } }
                    ],
                    change: onSelect
                });
            } else {
                $('#grid').empty();
                $("#gridContainer").hide();
                $("#noResultsMessage").show();
                $("#ContratoSE").data("kendoNumericTextBox").value('');
            }
        },
        complete: function () {
            $("body").css("cursor", "default");
        }
    });
}

function onSelect() {
    var selectedData = this.dataItem(this.select());
    contratoSapSE = selectedData.ContratoSAP;
    negocioIdSE = selectedData.NegocioId;
    kgPendientes = selectedData.KgPendientes;
    cuposRestantes = selectedData.CuposRestantes;
    var mostrar = contratoSapSE || negocioIdSE;
    $("#ContratoSE").data("kendoNumericTextBox").value(mostrar);
}
