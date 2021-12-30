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
        //value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#FechaHasta").kendoDatePicker({
        //value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#CantidadCupo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        value: 0
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

    $("#CantidadAlgoritmo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
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
                    }, {
                        Centro: "SAN LORENZO SUSTENTABLE / CALIDAD"
                    }, {
                        Centro: "Rio del Valle"
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
        '¿<th colspan="2"  style="text-align: center;background: gray; color: white">Zona</th>' +
        '<th  style="text-align: center; background: gray; color: white">Cupos</th>' +
        '<th  style="text-align: center; background: gray; color: white">Consumidos</th>' +
        '<th style="text-align: center; background: gray; color: white">Disponibles</th></tr > ');

    if (zonas.ZonaCupo) {
        cantidadZonas = zonas.ZonaCupo.length;
        for (var i = 0; i < cantidadZonas; i++) {
            var fila =
                '<tr>' +
                '<td>' + zonas.ZonaCupo[i].CodigoSap + '<input id="zonaId' + i + '" value="' + zonas.ZonaCupo[i].Id + '" hidden><input id="limiteAnterior' + zonas.ZonaCupo[i].Id + '" class="number-input hidden" hidden /></td>' +
                '<td>' + zonas.ZonaCupo[i].Descripcion + '</td>' +
                '<td style="width: 15%;"><input min="0" onchange="CalcularTotal(' + id + ')" id="cantidad' + zonas.ZonaCupo[i].Id + '" class="number-input" /><input id="limiteCupoId' + zonas.ZonaCupo[i].Id + '" class="limite-cupo" hidden /></td>' +
                '<td><span id="consumido' + zonas.ZonaCupo[i].Id + '"></span></td>' +
                '<td><span id="disponible' + zonas.ZonaCupo[i].Id + '"></span></td>' +
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
                MensErr("Las configuraciones seleccionadas no tienen el mismo limite de Cupos.");
                return false;
            }
            limite = configuraciones[i].LimiteCupo;
        }
        if (ids.length <= 0) {
            MensErr("No se seleccionó ninguna configuración");
            return false;
        }
    }
    $("#ModalLimiteCupo").modal('show');
    var tieneId = EsMasivo();
    var idConf = tieneId == undefined ? id : tieneId;
    $("#IdConfig").val(idConf);
    CargarConfiguracion(idConf, tieneId > 0);
    CalcularTotal(idConf);
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
        '<th  style="text-align: center">Cupo</th>' +
        '<th  style="text-align: center">Algoritmo</th></tr>');
    var configuracion = MSExecuteOnServer("/ConfiguracionCupo/TraerConfiguracionCupo", { id: id });
    var fecha = mostrarFecha == true ? "-" : kendo.toString(kendo.parseDate(configuracion.Fecha), "dd/MM/yyyy");
    var fila =
        '<tr>' +
        '<td>' + configuracion.Centro + '</td>' +
        '<td>' + configuracion.Material + '</td>' +
        '<td>' + fecha + '</td>' +
        '<td>' + configuracion.LimiteCupo + '</td>' +
        '<td>' + configuracion.LimiteAlgoritmo + '</td>' +
        '<td style="text-align: left; width: 15%;"><input min="' + configuracion.Consumidos + '" id="limiteCupo' + configuracion.Id + '" class="number" /></td>' +
        '<td style="text-align: left; width: 15%;"><input id="limiteAlgoritmo' + configuracion.Id + '" class="number" /></td>' +
        '</tr>';
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
    $('#totalConsumidos').data("kendoNumericTextBox").value(configuracion.CuposConsumidos);
    $('#totalDisponibles').data("kendoNumericTextBox").value(configuracion.CuposDisponibles);
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

function GuardarLimiteCupo() {
    BlockUi("Guardando...");
    var limites = [];
    var total = 0;
    for (var i = 0; i < cantidadZonas; i++) {
        var zonaId = $('#zonaId' + i).val();
        var obj = {
            Id: $("#limiteCupoId" + zonaId).val(),
            ZonaCupoId: zonaId,
            CantidadCupo: $("#cantidad" + zonaId).val(),
            ConfiguracionCupoId: $("#configuracionCupoId").val(),
            LimiteAnterior: $("#limiteAnterior" + zonaId).val()
        };
        limites.push(obj);
        total += parseInt($("#cantidad" + zonaId).val());
    }

    var configuraciones = SeleccionarElementos();
    var configuracionesIds = [];
    for (var i = 0; i < configuraciones.length; i++) {
        configuracionesIds.push(configuraciones[i].id);
        if (total != configuraciones[i].LimiteCupo) {
            MensErr("La cantidad ingresada es diferente al limite configurado.");
            $.unblockUI();
            return false;
        }
    }
    if ($("#configuracionCupoId").val() != "") {
        var limitesSAP = MSExecuteOnServer("/ConfiguracionCupo/TraerLimitesCupo", { id: $("#configuracionCupoId").val() });
        for (var i = 0; i < 10; i++) {
            limiteSAP = limitesSAP[i];
            $('#consumido' + (i + 1)).text(limiteSAP.Consumidos);
            $('#disponible' + (i + 1)).text(limiteSAP.Disponible);
        }
        for (var i = 0; i < 10; i++) {
            limiteSAP = limitesSAP[i];
            limite = limites[i];
            if (limite.CantidadCupo < limiteSAP.Consumidos) {
                MensErr(`La cantidad ingresada en la zona ${limiteSAP.ZonaCupo} es menor a la cantidad de cupos consumidos.`);
                $.unblockUI();
                return false;
            }
        }
    }



    var resultado;
    if (configuracionesIds.length == 0) {
        var idConfiguracion = $("#configuracionCupoId").val()
        if (ValidarZona(idConfiguracion)) {
            resultado = MSExecuteOnServer('/ConfiguracionCupo/ModificarConfiguracion', {
                id: idConfiguracion,
                limite: $("#limiteCupo" + idConfiguracion).val() == 0 ? null : $("#limiteCupo" + idConfiguracion).val(),
                algoritmo: $("#limiteAlgoritmo" + idConfiguracion).val() == 0 ? null : $("#limiteAlgoritmo" + idConfiguracion).val(),
                bloquear: null,
                liberar: null
            });
            $.unblockUI();
            var errores = new Array();
            var cuposGenerados = new Array();
            if (resultado.HayError) {
                $("#error-alert").text(resultado.Errores[0].Message);
                $(".alert-danger").show();
                setTimeout(function () { $(".alert-danger").hide(); }, 5000);
            }
            if (errores.length == 0) {
                resultado = MSExecuteOnServer("/ConfiguracionCupo/GrabarLimitesCupo", { limites });
            }
        } else {
            MensErr("La cantidad configurada de las zonas(" + $('#totalId').data("kendoNumericTextBox").value() + ") excede el limite de cupos(" + $("#limiteCupo" + idConfiguracion).val() + ")")
        }

    } else {

        resultado = MSExecuteOnServer("/ConfiguracionCupo/GrabarLimitesCupoMasivo", { limites, configuracionesIds });
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
        MensErr("No se seleccionó ninguna configuración");
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
    var bloquear = $("#BloquearCupera" + id).is(":checked") ?? null;
    var liberar = $("#LiberarCupera" + id).is(":checked") ?? null;

    result = MSExecuteOnServer('/ConfiguracionCupo/ModificarConfiguracion', { id: id, limite: cantidadCupos, algoritmo: algoritmo, bloquear: bloquear, liberar: liberar });

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
}