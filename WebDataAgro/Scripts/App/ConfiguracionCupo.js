var cantidadZonas;
$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    InicializarElementos();
    CargarGrillaConfig();

    if ($("#Id").val() != "" && $("#Id").val() > 0) {
        $("#MaterialId").prop('disabled', true);
        $("#CentroId").prop('disabled', true);
        $("#Fecha").data('kendoDatePicker').readonly(true);
        $("#FechaHasta").data('kendoDatePicker').readonly(true);
    }
    $("#formGrabarCupos").submit(function () {
        BlockUi('Grabando...');
    });
});

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

$(".alert").ready(function () {
    //setTimeout(function () { $(".alert").hide(); }, 5000);
});

function InicializarElementos() {
    $("#Fecha").kendoDatePicker({
        value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#FechaHasta").kendoDatePicker({
        value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#CantidadCupo").kendoNumericTextBox({ // Límite Cupo (NO modal)
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });
    $("#CantidadAlgoritmo").kendoNumericTextBox({ // Límite Algoritmo (NO modal)
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    $("#CantidadDescarga").kendoNumericTextBox({ // Límite con Descarga (NO modal)
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });

    $("#totalId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });

    $("#totalConsumidos").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });

    $("#totalDisponibles").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });


    $("#totalIdDescarga").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });

    $("#totalConsumidosDescarga").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });

    $("#totalDisponiblesDescarga").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
    });

    $("#ModalLimiteCupo").on("hidden.bs.modal", function () {
        $("#tablaLimite").empty();
    });

    $("#grabarLimiteCupo").click(function () {
        GuardarLimiteCupo();
    });

    $("[data-hide]").on("click", function () {
        $(this).closest("." + $(this).attr("data-hide")).hide();
    });
}

function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ConfiguracionCupo/DatosConfiguracion'
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
                    Fecha: { type: "date" },
                    LimiteCupo: { type: "number" },
                    LimiteAlgoritmo: { type: "number" },
                    LimiteDescarga: { type: "number" },
                    LimiteCupoAnterior: { type: "number" },
                    LiberarCupera: { type: "boolean" },
                    Bloquear: { type: "boolean" },
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Fecha", dir: "asc" }, { field: "Material", dir: "asc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#gridConfiguracionCupo").kendoGrid({
        dataSource: ds,
        columns: [
            {
                field: "", title: "", width: "10px", editable: function (dataItem) {
                    return false;
                },
            },
            {
                selectable: true, width: "50px", editable: function (dataItem) {
                    return false;
                },
            },
            {
                field: "Centro", type: "string", editable: function (dataItem) {
                    return false;
                }, filterable: {
                    multi: true, dataSource: [{
                        Centro: "S. Lorenzo"
                    },
                    //{ Centro: "SAN LORENZO SUSTENTABLE / CALIDAD" },
                    {
                        Centro: "Rio del Valle (Planta Soto)"
                    }, {
                        Centro: "General Pinedo"
                    }, {
                        Centro: "Vicentin Virtual"
                    }, {
                        Centro: "Bahia Blanca"
                    }, {
                        Centro: "Pergamino"
                    }, {
                        Centro: "Bandera"
                    },
                    {
                        Centro: "La Cautiva"
                    },
                    {
                        Centro: "Lincoln"
                    },
                    {
                        Centro: "Prest Dev. Buenos Aires"
                    },
                    {
                        Centro: "Prest Dev. Santa Fe"
                    },
                    {
                        Centro: "Chivilcoy"
                    }, {
                        Centro: "LE"
                    }]
                }, width: 130, template: "#=Centro#",
            },
            {
                field: "Material", title: "Cultivo", editable: function (dataItem) {
                    return false;
                },
                filterable: {
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
                }, width: 130, template: "#=Material#"
            },
            {
                field: "Fecha", type: "date", format: _DefaultDateTemplate, editable: function (dataItem) {
                    return false;
                },
            },
            {
                field: "CuposConsumidos", type: "number", title: "Cupos Consumidos", editable: function (dataItem) {
                    return false;
                }, filterable: { extra: false },
            },
            {
                field: "LimiteCupo", type: "number", title: "Límite de Cupo", editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);

                    $("#LimiteCupo" + options.model.Id).val(options.model.LimiteCupo);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        min: options.model.CuposConsumidos,
                        change: function () {
                            AceptarConfiguracion(options.model.Id)
                        },
                    });
                }, filterable: { extra: false },
                editable: function (dataItem) {
                    if (dataItem.NoPropio == true)
                        return false;
                    else
                        return true;
                }
            },
            {
                field: "LimiteAlgoritmo", type: "number", title: "Límite Algoritmo", editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);

                    $("#LimiteAlgoritmo" + options.model.Id).val(options.model.LimiteAlgoritmo);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        max: options.model.LimiteCupo,
                        min: options.model.CuposConsumidos,
                        change: function () {
                            AceptarConfiguracion(options.model.Id)
                        },
                    });
                }, filterable: { extra: false },
                editable: function (dataItem) {
                    if (dataItem.NoPropio == true)
                        return false;
                    else
                        return true;
                }
            },
            {
                field: "LimiteDescarga", type: "number", title: "Límite Descarga", editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);

                    $("#LimiteDescarga" + options.model.Id).val(options.model.LimiteDescarga);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        max: options.model.LimiteCupo,
                        min: options.model.CuposConsumidosConDescarga,
                        change: function () {
                            AceptarConfiguracion(options.model.Id)
                        },
                    });
                }, filterable: { extra: false },
                editable: function (dataItem) {
                    if (dataItem.NoPropio == true)
                        return false;
                    else
                        return true;
                }
            },
            {
                field: "LimiteCupoAnterior", type: "number", title: "Límite Anterior", editable: function (dataItem) {
                    return false;
                }, filterable: { extra: false },
            },
            {
                field: "Bloquear", type: "string", title: "Bloqueo Cupera", width: 40, editable: function (dataItem) {
                    return false;
                }, template: function (dataItem) {
                    var bloqueado = dataItem.BloquearCupera == "Si" || dataItem.BloquearCupera == true ? "checked" : "";
                    return "<label class='content-input' style='cursor:pointer;'><input id='BloquearCupera" + dataItem.Id + "'onclick='AceptarConfiguracion(" + dataItem.Id + ")'  type='checkbox'" + bloqueado + "><i style='color:white'></i></label>";

                }, filterable: true,
            },
            {
                field: "LiberarCupera", type: "string", title: "Liberar Algoritmo", width: 40, editable: function (dataItem) {
                    return false;
                }, template: function (dataItem) {
                    var liberada = dataItem.LiberarCuperaDesc == "Si" ? "checked" : "";
                    return "<label class='content-input' style='cursor:pointer;'><input id='LiberarCupera" + dataItem.Id + "' onclick='AceptarConfiguracion(" + dataItem.Id + ")' type='checkbox'" + liberada + "><i></i></label>";

                }, filterable: true,
            },
            {
                field: "Id", title: " ", filterable: false, sortable: false, width: 80, editable: function (dataItem) {
                    return false;
                }, template: function (dataItem) {

                    return (dataItem.BloquearCupera === "No") ? '<a data-toggle="tooltip" title="Limite Cupo" class="abrirModalLimite links-grid" onclick="AbrirModal(' + dataItem.Id + ')">' +
                        '<span class="botonVarios"> <i class="fa fa-cog varios"></i> </span ></a >' : "";
                }
            }
        ],
        editable: true,
        dataBound: function (e) {
            $(".cerrado").each(function (index) {
                var dataItem = e.sender.dataItem($(this).parent());
                if (dataItem.BloquearCupera == "Si") {
                    $(this).addClass('line');
                }
            });
            var grid = $("#gridConfiguracionCupo").data("kendoGrid");
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].Material == "Maiz") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "'] td:eq(0)")
                        .addClass("maiz");
                }
                if (view[i].Material == "Soja") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "'] td:eq(0)")
                        .addClass("soja");
                }
                if (view[i].Material == "Trigo") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "'] td:eq(0)")
                        .addClass("trigo");
                }
                if (view[i].Material == "Girasol") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "'] td:eq(0)")
                        .addClass("girasol");
                }
                if (view[i].Material == "Girasol AO") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "'] td:eq(0)")
                        .addClass("girasolAO");
                }
            }
        },
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
        filterMenuOpen: function (e) {
            if (e.field == "Fecha") {
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
            /* extra: false,*/
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
    });
}

function recargarGrilla() {
    $('#gridConfiguracionCupo').data('kendoGrid').dataSource.read();
}

function AbrirModal(id) {
    if (id) {
        $("#gridConfiguracionCupo").data("kendoGrid").clearSelection();
    }
    $('#ModalLimiteCupo').on('hidden.bs.modal', function () {
        EliminarTablaConfiguracion(id);
    });
    $("#configuracionCupoId").val(id);

    $("#tablaLimite").empty();
    var zonas = MSExecuteOnServer("/ConfiguracionCupo/TraerZonaCupo");
    $("#tablaLimite").append('<tr>' +
        '<th colspan="2" style="text-align: center;background: black; color: white">Zona</th>' +
        '<th style="text-align: center; background: gray; color: white">Cupos</th>' +
        '<th style="text-align: center; background: gray; color: white">Consumidos</th>' +
        '<th style="text-align: center; background: gray; color: white">Disponibles</th>' +
        '<th style="text-align: center; background: steelblue; color: white">Cupos c/Desc.</th>' +
        '<th style="text-align: center; background: steelblue; color: white">Cons. c/Desc.</th>' +
        '<th style="text-align: center; background: steelblue; color: white">Disp. c/Desc.</th>' +
        '</tr>');

    if (zonas.ZonaCupo) {
        cantidadZonas = zonas.ZonaCupo.length;
        for (var i = 0; i < cantidadZonas; i++) {
            var fila =
                '<tr>' +
                '<td>' + zonas.ZonaCupo[i].CodigoSap + '<input id="zonaId' + i + '" value="' + zonas.ZonaCupo[i].Id + '" hidden><input id="limiteAnterior' + zonas.ZonaCupo[i].Id + '" class="number-input hidden" hidden /></td>' +
                '<td>' + zonas.ZonaCupo[i].Descripcion + '</td>' +
                '<td>' +
                '<input min="0" onchange="CalcularTotal(' + id + ')" id="cantidad' + zonas.ZonaCupo[i].Id + '" class="number-input" />' +
                '<input id="limiteCupoId' + zonas.ZonaCupo[i].Id + '" class="limite-cupo" hidden />' +
                '</td>' +
                '<td><span id="consumido' + zonas.ZonaCupo[i].Id + '"></span></td>' +
                '<td><span id="disponible' + zonas.ZonaCupo[i].Id + '"></span></td>' +
                '<td>' +
                '<input min="0" onchange="CalcularTotalDescarga(' + id + ')" id="cantidadDescarga' + zonas.ZonaCupo[i].Id + '" class="number-input" />' +
                '<input id="limiteCupoDescargaId' + zonas.ZonaCupo[i].Id + '" class="limite-cupo" hidden />' +
                '</td>' +
                '<td><span id="consumidoDescarga' + zonas.ZonaCupo[i].Id + '"></span></td>' +
                '<td><span id="disponibleDescarga' + zonas.ZonaCupo[i].Id + '"></span></td>' +
                '</tr>';
            $("#tablaLimite").append(fila);
        }
    }

    $(".number-input").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: "0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        step: 0
    });

    if (id) {
        CargarLimites(id);
        for (var j = 0; j < cantidadZonas; j++) {
            var numeric = $("#cantidad" + zonas.ZonaCupo[j].Id).data("kendoNumericTextBox");
            numeric.element.unbind("keydown");
        }
    } else {
        var configuraciones = SeleccionarElementos();
        var ids = [];
        var limite = null;
        for (var i = 0; i < configuraciones.length; i++) {
            ids.push(configuraciones[i].id);
            if (limite != null && limite != configuraciones[i].LimiteCupo) {
                MensErr("Las configuraciones seleccionadas no tienen el mismo límite de cupos.");
                return false;
            }
            limite = configuraciones[i].LimiteCupo;
        }
        if (ids.length <= 0) {
            MensErr("No se seleccionó ninguna configuración.");
            return false;
        }
    }
    $("#ModalLimiteCupo").modal('show');
    var tieneId = EsMasivo();
    var idConf = tieneId == undefined ? id : tieneId;
    $("#IdConfig").val(idConf);
    CargarConfiguracion(idConf, tieneId > 0);
    CalcularTotal(idConf);
    CalcularTotalDescarga(idConf);
}

function EsMasivo() {
    var configuraciones = SeleccionarElementos();
    return configuraciones.length <= 0 ? undefined : configuraciones[0].id;
}

function CargarLimites(id) {
    var limites = MSExecuteOnServer("/ConfiguracionCupo/TraerLimitesCupo", { id: id });

    if (limites) {
        for (var i = 0; i < limites.length; i++) {
            $('#cantidad' + limites[i].ZonaCupoId).data("kendoNumericTextBox").value(limites[i].CantidadCupo);
            $('#limiteAnterior' + limites[i].ZonaCupoId).data("kendoNumericTextBox").value(limites[i].CantidadCupo);
            $('#limiteCupoId' + limites[i].ZonaCupoId).val(limites[i].Id);
            $('#consumido' + limites[i].ZonaCupoId).text(limites[i].Consumidos);
            $('#disponible' + limites[i].ZonaCupoId).text(limites[i].Disponible);
            $('#cantidadDescarga' + limites[i].ZonaCupoId).data("kendoNumericTextBox").value(limites[i].CantidadCupoConDescarga);
            $('#consumidoDescarga' + limites[i].ZonaCupoId).text(limites[i].ConsumidosDescarga);
            $('#disponibleDescarga' + limites[i].ZonaCupoId).text(limites[i].DisponibleDescarga);
        }
    }
}

function CargarConfiguracion(id, mostrarFecha) {

    $("#tablaConfiguracion").append('<tr>' +
        '<th  style="text-align: center"><strong>Centro</strong></th>' +
        '<th  style="text-align: center"><strong>Material</strong></th>' +
        '<th  style="text-align: center"><strong>Fecha</strong></th>' +
        '<th  style="text-align: center"><strong>Límite Cupo</strong></th>' +
        '<th  style="text-align: center"><strong>Límite Algoritmo</strong></th>' +
        '<th  style="text-align: center"><strong>Límite Descarga</strong></th></tr>'); //+
    //'<th  style="text-align: center">Cupo</th>' +
    //'<th  style="text-align: center">Algoritmo</th></tr>');
    var configuracion = MSExecuteOnServer("/ConfiguracionCupo/TraerConfiguracionCupo", { id: id });
    var fecha = mostrarFecha == true ? "-" : kendo.toString(kendo.parseDate(configuracion.Fecha), "dd/MM/yyyy");
    var fila =
        '<tr>' +
        '<td>' + configuracion.Centro + '</td>' +
        '<td>' + configuracion.Material + '</td>' +
        '<td>' + fecha + '</td>'; //+
    //'<td>' + configuracion.LimiteCupo + '</td>' +
    //'<td>' + configuracion.LimiteAlgoritmo + '</td>';
    //'<td>' + configuracion.LimiteDescarga + '</td>';
    if (configuracion.NoPropio == true) {
        fila += '<td style="text-align: left; width: 15%;"><input min="' + configuracion.Consumidos + '" value="' + configuracion.LimiteCupo + '" id="limiteCupo' + configuracion.Id + '" class="number" disabled /></td>' +
            '<td style="text-align: left; width: 15%;"><input value="' + configuracion.LimiteAlgoritmo + '" id="limiteAlgoritmo' + configuracion.Id + '" class="number" disabled/></td>' +
            '<td style="text-align: left; width: 15%;"><input value="' + configuracion.LimiteDescarga + '" id="limiteDescarga' + configuracion.Id + '" class="number" disabled/></td>' +
            '</tr>'
    } else {
        fila += '<td style="text-align: left; width: 15%;"><input min="' + configuracion.Consumidos + '" value="' + configuracion.LimiteCupo + '" id="limiteCupo' + configuracion.Id + '" class="number"  /></td>' +
            '<td style="text-align: left; width: 15%;"><input value="' + configuracion.LimiteAlgoritmo + '" id="limiteAlgoritmo' + configuracion.Id + '" class="number" /></td>' +
            '<td style="text-align: left; width: 15%;"><input value="' + configuracion.LimiteDescarga + '" id="limiteDescarga' + configuracion.Id + '" class="number" /></td>' +
            '</tr>';
    }
    $("#tablaConfiguracion").append(fila);

    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: "0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        step: 0
    });
    $('#limiteCupo' + configuracion.Id).data("kendoNumericTextBox").value(configuracion.LimiteCupo);
    $('#limiteAlgoritmo' + configuracion.Id).data("kendoNumericTextBox").value(configuracion.LimiteAlgoritmo);
    $('#limiteDescarga' + configuracion.Id).data("kendoNumericTextBox").value(configuracion.LimiteDescarga);
    $('#totalConsumidos').data("kendoNumericTextBox").value(configuracion.CuposConsumidos);
    $('#totalDisponibles').data("kendoNumericTextBox").value(configuracion.CuposDisponibles);
    $('#totalConsumidosDescarga').data("kendoNumericTextBox").value(configuracion.CuposConsumidosConDescarga);
    $('#totalDisponiblesDescarga').data("kendoNumericTextBox").value(configuracion.CuposDisponiblesConDescarga);
}

function EliminarTablaConfiguracion(idConfiguracion) {
    //$("#limiteCupo" + idConfiguracion).data("kendoNumericTextBox").destroy();
    //$("#limiteAlgoritmo" + idConfiguracion).data("kendoNumericTextBox").destroy();
    $("#tablaConfiguracion").html('');
}

function CalcularTotal(id) {
    var total = 0;
    id = id ? id : EsMasivo();
    //$('#totalId').data("kendoNumericTextBox").value(0);
    //var limites = MSExecuteOnServer("/ConfiguracionCupo/TraerLimitesCupo", { id: id });
    //if (limites) {
    for (var i = 1; i < 11; i++) {
        total += Number($('#cantidad' + i).val());
    }
    //}
    $('#totalId').data("kendoNumericTextBox").value(total);
}

function CalcularTotalDescarga(id) {
    var total = 0;
    id = id ? id : EsMasivo();

    for (var i = 1; i < 11; i++) {
        total += Number($('#cantidadDescarga' + i).val());
    }

    $('#totalIdDescarga').data("kendoNumericTextBox").value(total);
}

function GuardarLimiteCupo() {
    BlockUi("Guardando...");
    var limites = [];
    var total = 0;
    var cantErr = 0;
    var totalDescarga = 0;

    for (var i = 0; i < cantidadZonas; i++) {
        var zonaId = $('#zonaId' + i).val();
        var obj = {
            Id: $("#limiteCupoId" + zonaId).val(),
            ZonaCupoId: zonaId,
            CantidadCupo: $("#cantidad" + zonaId).val(),
            ConfiguracionCupoId: $("#configuracionCupoId").val(),
            LimiteAnterior: $("#limiteAnterior" + zonaId).val(),
            CantidadCupoConDescarga: $("#cantidadDescarga" + zonaId).val(),
        };
        limites.push(obj);
        total += parseInt($("#cantidad" + zonaId).val());
        totalDescarga += parseInt($("#cantidadDescarga" + zonaId).val());

        if ($("#cantidad" + zonaId).val() == "" || $("#cantidadDescarga" + zonaId).val() == "") {
            cantErr += 1
        };
    }

    if (cantErr > 0) {
        MensErr("Por favor, completar la cantidad de cupos para cada zona.");
        $.unblockUI();
        return false;
    }

    var configuraciones = SeleccionarElementos();
    var configuracionesIds = [];

    if (configuraciones.length > 0 && $("#limiteCupo" + configuraciones[0].Id).val() == "") {
        MensErr("Debe ingresar un valor para el Límite del Cupo.");
        $.unblockUI();
        return false;
    }
    if (configuraciones.length > 0 && $("#limiteDescarga" + configuraciones[0].Id).val() == "") {
        MensErr("Debe ingresar un valor para el Límite del Cupo con Descarga.");
        $.unblockUI();
        return false;
    }

    if (configuraciones.length > 0 && total != Number($("#limiteCupo" + configuraciones[0].Id).val())) {
        MensErr("La cantidad ingresada es diferente al límite configurado.");
        $.unblockUI();
        return false;
    }
    if (configuraciones.length > 0 && totalDescarga != Number($("#limiteDescarga" + configuraciones[0].Id).val())) {
        MensErr("La cantidad ingresada es diferente al límite de Cupo con Descarga configurado.");
        $.unblockUI();
        return false;
    }

    for (var i = 0; i < configuraciones.length; i++) {
        configuracionesIds.push(configuraciones[i].id);
    }
    if ($("#configuracionCupoId").val() != "") {

        var limiteMinimoCupoConDescarga = MSExecuteOnServer("/ConfiguracionCupo/TraerLimiteMinimoCupoConDescarga", { id: $("#configuracionCupoId").val() });
        var limiteDescargaId = "#limiteDescarga" + $("#configuracionCupoId").val();
        if (parseInt($(limiteDescargaId).val()) < limiteMinimoCupoConDescarga) {
            MensErr("El Límite de Descarga ingresado es menor a los Cupos con Descarga ya existentes.");
            $.unblockUI();
            return false;
        }

        var limitesSAP = MSExecuteOnServer("/ConfiguracionCupo/TraerLimitesCupo", { id: $("#configuracionCupoId").val() });
        for (var i = 0; i < 10; i++) {
            limiteSAP = limitesSAP[i];
            $('#consumido' + (i + 1)).text(limiteSAP.Consumidos);
            $('#disponible' + (i + 1)).text(limiteSAP.Disponible);
            $('#consumidoDescarga' + (i + 1)).text(limiteSAP.ConsumidosDescarga);
            $('#disponibleDescarga' + (i + 1)).text(limiteSAP.DisponibleDescarga);
        }
        for (var i = 0; i < 10; i++) {
            limiteSAP = limitesSAP[i];
            limite = limites[i];
            if (limite.CantidadCupo < limiteSAP.Consumidos) {
                MensErr(`La cantidad ingresada en la zona ${limiteSAP.ZonaCupo} es menor a la cantidad de cupos consumidos.`);
                $.unblockUI();
                return false;
            }
            if (limite.CantidadCupoConDescarga < limiteSAP.ConsumidosDescarga) {
                MensErr(`La cantidad ingresada en la zona ${limiteSAP.ZonaCupo} es menor a la cantidad de Cupos con Descarga consumidos.`);
                $.unblockUI();
                return false;
            }
        }
    }

    var resultado;
    if (configuracionesIds.length == 0) { // Configurar zonas INDIVIDUAL
        var idConfiguracion = $("#configuracionCupoId").val()
        if (ValidarZona(idConfiguracion) && ValidarZonaDescarga(idConfiguracion)) {

            if ($("#limiteCupo" + idConfiguracion).val() == "" || $("#limiteAlgoritmo" + idConfiguracion).val() == "" || $("#limiteDescarga" + idConfiguracion).val() == "") {
                MensErr("Los límites de Cupo, Algoritmo y Descarga deben completarse.\n");
                $.unblockUI();
                return false;
            }

            resultado = MSExecuteOnServer('/ConfiguracionCupo/ModificarConfiguracion', {
                id: idConfiguracion,
                limite: $("#limiteCupo" + idConfiguracion).val() == 0 ? null : $("#limiteCupo" + idConfiguracion).val(),
                algoritmo: $("#limiteAlgoritmo" + idConfiguracion).val() == 0 ? null : $("#limiteAlgoritmo" + idConfiguracion).val(),
                descarga: $("#limiteDescarga" + idConfiguracion).val() == 0 ? null : $("#limiteDescarga" + idConfiguracion).val(),
                bloquear: null,
                liberar: null
            });
            $.unblockUI();

            if (resultado.HayError) {
                $("#error-alert").text(resultado.Errores[0].Message);
                $(".alert-danger").show();
                setTimeout(function () { $(".alert-danger").hide(); }, 5000);
            } else {
                resultado = MSExecuteOnServer("/ConfiguracionCupo/GrabarLimitesCupo", { limites });
            }
        } else if (!ValidarZona(idConfiguracion)) {
            MensErr("La cantidad configurada de las zonas(" + $('#totalId').data("kendoNumericTextBox").value() + ") excede el límite de cupos(" + $("#limiteCupo" + idConfiguracion).val() + ")")
            $.unblockUI();
            return false;
        } else {
            MensErr("La cantidad de Cupos con Descarga configurada en las zonas(" + $('#totalIdDescarga').data("kendoNumericTextBox").value() + ") excede el límite de cupos con Descarga(" + $("#limiteDescarga" + idConfiguracion).val() + ")")
            $.unblockUI();
            return false;
        }

    } else { // Configurar zonas MASIVO
        var limiteCupo = $("#limiteCupo" + configuraciones[0].Id).val();
        var limiteAlgoritmo = $('#limiteAlgoritmo' + configuraciones[0].Id).val();
        var limiteDescarga = $('#limiteDescarga' + configuraciones[0].Id).val();
        if (limiteAlgoritmo == "" || limiteDescarga == "") {
            MensErr("El Límite del Algoritmo y el Límite con Descarga no pueden estar vacíos.")
            $.unblockUI();
            return false;
        } else if ((Number(limiteAlgoritmo) + Number(limiteDescarga)) > limiteCupo) {
            console.log(Number(limiteAlgoritmo) + Number(limiteDescarga));
            MensErr("La suma del límite del Algoritmo y del límite con Descarga no debe superar el total de " + limiteCupo + " cupos disponibles.")
            $.unblockUI();
            return false;
        } else
            resultado = MSExecuteOnServer("/ConfiguracionCupo/GrabarLimitesCupoMasivo", { limites, configuracionesIds, limiteAlgoritmo, limiteDescarga });
    }

    if (resultado.HayError) {
        //$("#error-alert").text(resultado.Errores[0].Message);
        //$(".alert-danger").show();
        //setTimeout(function () { $(".alert-danger").hide(); }, 5000);
        MensErr(resultado.Errores[0].Message)
    } else {
        $("#ModalLimiteCupo").modal('toggle');
        MensInfo("Grabado Correctamente")
        //$(".alert-success").show();
        //setTimeout(function () { $(".alert-success").hide(); }, 5000);
        recargarGrilla();
        EliminarTablaConfiguracion(idConfiguracion);
    }

    $.unblockUI();
}

function ValidarZona(idConfiguracion) {
    if ($("#limiteCupo" + idConfiguracion).val() > 0 && $("#limiteCupo" + idConfiguracion).val() < $('#totalId').data("kendoNumericTextBox").value()) {
        return false;
    }
    return true;
}
function ValidarZonaDescarga(idConfiguracion) {
    if ($("#limiteDescarga" + idConfiguracion).val() > 0 && $("#limiteDescarga" + idConfiguracion).val() < $('#totalIdDescarga').data("kendoNumericTextBox").value()) {
        return false;
    }
    return true;
}

function Editar(id) {
    var cupo = MSExecuteOnServer("/ConfiguracionCupo/EditarConfiguracionCupo", { id: id });
    $("#alta").collapse('show');
    $("#Id").val(cupo.Id);
    $("#CentroId").val(cupo.CentroId);
    $("#MaterialId").val(cupo.MaterialId);
    $("#centro").val(cupo.CentroId);
    $("#material").val(cupo.MaterialId);
    var fecha = kendo.toString(kendo.parseDate(cupo.Fecha), "dd-MM-yyyy");
    $("#Fecha").val(fecha);
    $("#FechaHasta").val(fecha);
    $("#CantidadCupo").data("kendoNumericTextBox").value(cupo.LimiteCupo);
    $("#CantidadAlgoritmo").data("kendoNumericTextBox").value(cupo.LimiteAlgoritmo);
    $("#CantidadDescarga").data("kendoNumericTextBox").value(cupo.LimiteDescarga);
    $("#CierreCupera").attr("checked", cupo.CierreCupera);
    $("#LiberarCupera").attr("checked", cupo.LiberarCupera);
    $("#MaterialId").prop('disabled', true);
    $("#CentroId").prop('disabled', true);
    $("#Fecha").data('kendoDatePicker').readonly(true);
    $("#FechaHasta").data('kendoDatePicker').readonly(true);

}

function Cancelar() {
    $("#Id").val(0);
    $("#CentroId").val(1);
    $("#MaterialId").val("");
    var hoy = new Date();
    var fecha = kendo.toString(kendo.parseDate(hoy), "dd-MM-yyyy");
    $("#Fecha").val(fecha);
    $("#FechaHasta").val(fecha);
    $("#CantidadCupo").data("kendoNumericTextBox").value("");
    $("#CantidadAlgoritmo").data("kendoNumericTextBox").value("");
    $("#CantidadDescarga").data("kendoNumericTextBox").value("");
    $("#CierreCupera").attr("checked", false);
    $("#LiberarCupera").attr("checked", false);
    $("#MaterialId").prop('disabled', false);
    $("#CentroId").prop('disabled', false);
    $("#Fecha").data('kendoDatePicker').readonly(false);
    $("#FechaHasta").data('kendoDatePicker').readonly(false);
}

function LimpiarConfiguracion() {
    $("#Id").val(0);
    $('#CentroId>option:eq(0)').prop('selected', true);
    $("#MaterialId").val("");
    var fecha = kendo.toString(kendo.parseDate(new Date()), "dd-MM-yyyy");
    $("#Fecha").val(fecha);
    $("#FechaHasta").val(fecha);
    $("#CantidadCupo").val("");
    $("#CantidadAlgoritmo").val("");
    $("#CantidadDescarga").val("");
}

function SeleccionarElementos() {
    var grid = $("#gridConfiguracionCupo").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        obj.push(selectedItem);
    });
    return obj;
}

function DeseleccionarElementos() {
    var grid = $("#gridConfiguracionCupo").data("kendoGrid");
    grid.clearSelection();
}

function CierreMasivoAceptar() {

    //var configuraciones = SeleccionarElementos();
    //var ids = [];
    //for (var i = 0; i < configuraciones.length; i++) {
    //    ids.push(configuraciones[i].id);
    //}
    //if (ids.length <= 0) {
    //    MensErr("No se seleccionó ninguna configuración");
    //} else {
    var limites = MSExecuteOnServer("/ConfiguracionCupo/CambioMasivo", { cambio: false });
    MensInfo("Se guardó correctamente");
    recargarGrilla();

}

function CierreMasivoCerrar() {

    var limites = MSExecuteOnServer("/ConfiguracionCupo/CambioMasivo", { cambio: true });
    MensInfo("Se guardó correctamente");
    recargarGrilla();

}

function AbrirMasivo() {


    var configuraciones = SeleccionarElementos();
    var ids = [];
    for (var i = 0; i < configuraciones.length; i++) {
        ids.push(configuraciones[i].id);
    }
    if (ids.length <= 0) {
        MensErr("No se seleccionó ninguna configuración.");
    } else {
        var limites = MSExecuteOnServer("/ConfiguracionCupo/CambioMasivo", { ids: ids, aceptar: false });
        MensInfo("Se guardó correctamente");
        recargarGrilla();
    }
}

function SetearEnCero() {
    if ($("#CantidadCupo").val() == "") {
        $("#CantidadCupo").data("kendoNumericTextBox").value("0");
    }
    if ($("#CantidadAlgoritmo").val() == "") {
        $("#CantidadAlgoritmo").data("kendoNumericTextBox").value("0");
    }
    if ($("#CantidadDescarga").val() == "") {
        $("#CantidadDescarga").data("kendoNumericTextBox").value("0");
    }
}

function SetearMasivoEnCero() {
    var zonas = MSExecuteOnServer("/ConfiguracionCupo/TraerZonaCupo");

    if (zonas.ZonaCupo) {
        var cantidadZonas = zonas.ZonaCupo.length;
        for (var i = 0; i < cantidadZonas; i++) {
            if ($("#cantidad" + zonas.ZonaCupo[i].Id).val() == '') {
                $("#cantidad" + zonas.ZonaCupo[i].Id).val("0");
            }
        }
    }
}

function AceptarConfiguracion(idConfiguracion) {
    var id = idConfiguracion;

    var grid = $("#gridConfiguracionCupo").data("kendoGrid").dataSource.data();
    var configuracionSeleccionada = grid.filter(function (x) { return (x.Id == id) });
    var cantidadCupos = configuracionSeleccionada != null && configuracionSeleccionada.length > 0 && configuracionSeleccionada[0].LimiteCupo ? configuracionSeleccionada[0].LimiteCupo : null;
    var algoritmo = configuracionSeleccionada != null && configuracionSeleccionada.length > 0 && configuracionSeleccionada[0].LimiteAlgoritmo ? configuracionSeleccionada[0].LimiteAlgoritmo : null;
    var descarga = configuracionSeleccionada != null && configuracionSeleccionada.length > 0 && configuracionSeleccionada[0].LimiteDescarga ? configuracionSeleccionada[0].LimiteDescarga : configuracionSeleccionada[0].CuposConsumidosConDescarga;
    var bloquear = $("#BloquearCupera" + id).is(":checked") ?? null;
    var liberar = $("#LiberarCupera" + id).is(":checked") ?? null;

    var limiteMinimoCupoConDescarga = MSExecuteOnServer("/ConfiguracionCupo/TraerLimiteMinimoCupoConDescarga", { id: id });
    if (descarga < limiteMinimoCupoConDescarga) {
        MensErr("El Límite de Descarga ingresado es menor a los Cupos con Descarga ya existentes.");
        $.unblockUI();
        return false;
    }

    result = MSExecuteOnServer('/ConfiguracionCupo/ModificarConfiguracion', { id: id, limite: cantidadCupos, algoritmo: algoritmo, descarga: descarga, bloquear: bloquear, liberar: liberar });

    $.unblockUI();
    var errores = new Array();
    if (result.HayError) {
        errores = errores.concat(result.ListaErrores);
    }
    if (errores.length == 0) {
        MensInfo("Se grabó correctamente.");
    } else {
        ShowErrorMessages(errores);
    }
    recargarGrilla();
}