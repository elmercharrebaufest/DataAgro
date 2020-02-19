var cantidadZonas;
$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarElementos();
    CargarGrillaConfig();
});

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

$(".alert").ready(function () {
    setTimeout(function () { $(".alert").hide(); }, 5000);
});

function InicializarElementos() {
    $("#Fecha").kendoDatePicker({
        value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#CantidadCupo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false
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

    $("#gridConfiguracionCupo").kendoGrid({
        dataSource: ds,
        columns: [
            { field: "Centro", type: "string" },
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
            { field: "Fecha", type: "date", format: _DefaultDateTemplate },
            { field: "LimiteCupo" },
            {
                field: "Id", title: " ", filterable: false, sortable: false, width:75, template: function (dataItem) {
                    return '<a data-toggle="tooltip" title="Editar Configuracion" class="abrirModalLimite links-grid" onclick="Editar(' + dataItem.Id + ')">' +
                        '<span> <i class="fa fa-pencil"></i> </span ></a >' +
                        '<a data-toggle="tooltip" title="Limite Cupo" class="abrirModalLimite links-grid" onclick="AbrirModal(' + dataItem.Id + ')">' +
                        '<span> <i class="fa fa-list"></i> </span ></a >';
                }}
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
        }
    });
}
function recargarGrilla() {
    $('#gridConfiguracionCupo').data('kendoGrid').dataSource.read();
}

function AbrirModal(id) {
    $("#configuracionCupoId").val(id);
    $("#tablaLimite").empty();
    var zonas = MSExecuteOnServer("/ConfiguracionCupo/TraerZonaCupo");
    $("#tablaLimite").append('<tr><th colspan="2">Zona</th><th>Cupos</th></tr>');
    
    if (zonas.ZonaCupo) {
        cantidadZonas = zonas.ZonaCupo.length;
        for (var i = 0; i < cantidadZonas; i++) {
            var fila = '<tr><td>' + zonas.ZonaCupo[i].CodigoSap + '<input id="zonaId' + i + '" value="' + zonas.ZonaCupo[i].Id + '" hidden></td>' + '<td>' + zonas.ZonaCupo[i].Descripcion +'</td><td><input id="cantidad' + zonas.ZonaCupo[i].Id + '" class="number-input"/><input id="limiteCupoId' + zonas.ZonaCupo[i].Id + '" class="limite-cupo" hidden/></td></tr>';
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
        min:0
    });
    CargarLimites(id);
    for (var j = 0; j < cantidadZonas; j++) {        
        var numeric = $("#cantidad" + zonas.ZonaCupo[j].Id).data("kendoNumericTextBox");

        numeric.element.unbind("keydown");
    }
    $("#ModalLimiteCupo").modal('show');
}
function CargarLimites(id) {
    var limites = MSExecuteOnServer("/ConfiguracionCupo/TraerLimitesCupo", {id:id});

    if (limites) {
        for (var i = 0; i < limites.length; i++) {
            $('#cantidad' + limites[i].ZonaCupoId).data("kendoNumericTextBox").value(limites[i].CantidadCupo);
            $('#limiteCupoId' + limites[i].ZonaCupoId).val(limites[i].Id);
        }
    }
}
function GuardarLimiteCupo() {
    var limites = [];
    for (var i = 0; i < cantidadZonas; i++) {
        var zonaId = $('#zonaId' + i).val();
        var obj = {
            Id: $("#limiteCupoId" + zonaId).val(),
            ZonaCupoId: zonaId,
            CantidadCupo: $("#cantidad" + zonaId).val(),
            ConfiguracionCupoId: $("#configuracionCupoId").val()
        };
        limites.push(obj);        
    }
    var resultado = MSExecuteOnServer("/ConfiguracionCupo/GrabarLimitesCupo", { limites });
    if (resultado.HayError) {
        $("#error-alert").text(resultado.Errores[0].Message);
        $(".alert-danger").show();
        setTimeout(function () { $(".alert-danger").hide();},5000);
    } else {
        $("#ModalLimiteCupo").modal('toggle');
        $(".alert-success").show();
        setTimeout(function () { $(".alert-success").hide(); }, 5000);
    }
}

function Editar(id) {
    var cupo = MSExecuteOnServer("/ConfiguracionCupo/EditarConfiguracionCupo", { id: id });
    $("#Id").val(cupo.Id);
    $("#CentroId").val(cupo.CentroId);
    $("#MaterialId").val(cupo.MaterialId);
    var fecha = kendo.toString(kendo.parseDate(cupo.Fecha), "dd-MM-yyyy"); 
    $("#Fecha").val(fecha);
    $("#CantidadCupo").data("kendoNumericTextBox").value(cupo.LimiteCupo);
}

function LimpiarConfiguracion() {
    $("#Id").val(0);
    $('#CentroId>option:eq(0)').prop('selected', true);
    $("#MaterialId").val("");
    var fecha = kendo.toString(kendo.parseDate(new Date()), "dd-MM-yyyy");
    $("#Fecha").val(fecha);
    $("#CantidadCupo").val("");
}