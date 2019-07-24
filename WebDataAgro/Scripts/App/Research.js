$(document).ready(function () {
    $('#menuproveedor').hide();

    InicializarElementos();
    AutocompleteProcedencia();
    AutocompleteProcedenciaCosecha();
    AutocompleteProcedenciaSituacionCultivo();
    AutocompleteProcedenciaVentaStock();

});

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

$("#AvanceSiembraTab").click(function () {
    DeseleccionarForms();
    $("#AvanceSiembraTab").children().addClass("whc-selected");
    $("#AvanceSiembra").show();
});
$("#AvanceCosechaTab").click(function () {
    DeseleccionarForms();
    $("#AvanceCosechaTab").children().addClass("whc-selected");
    $("#AvanceCosecha").show();
});
$("#SituacionCultivoTab").click(function () {
    DeseleccionarForms();
    $("#SituacionCultivoTab").children().addClass("whc-selected");
    $("#SituacionCultivo").show();
});
$("#VentaStockTab").click(function () {
    DeseleccionarForms();
    $("#VentaStockTab").children().addClass("whc-selected");
    $("#VentaStock").show();
});

function AutocompleteProcedencia() {
    $("#Localidad").click(function () {
        $("#Localidad").data("kendoAutoComplete").value("");
        $("#Localidad").trigger("change");
        if ($("#Localidad").val() == "") {
            $("#Partido").val('');
            $("#LocalidadId").val("");
        }
    });

    $("#Localidad").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function () {
            if ($("#Localidad").val().split('|').length > 1) {
                $("#Localidad").val($("#Localidad").val().split('|')[1]);
                
            }
           
        },
        select: function (e) {
            $("#Partido").val(e.dataItem.Partido);
            $("#LocalidadId").val(e.dataItem.Id);
            $("#LocalidadNombre").val(e.dataItem.Localidad);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#Localidad').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
}


function DeseleccionarForms() {
    $("#AvanceSiembraTab").children().removeClass("whc-selected");
    $("#AvanceSiembra").hide();
    $("#AvanceCosechaTab").children().removeClass("whc-selected");
    $("#AvanceCosecha").hide();
    $("#SituacionCultivoTab").children().removeClass("whc-selected");
    $("#SituacionCultivo").hide();
    $("#VentaStockTab").children().removeClass("whc-selected");
    $("#VentaStock").hide();
}
function InicializarElementos() {

    $(".number-input").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,
        max: 100
    });
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0,        
    });   
}

function AutocompleteProcedenciaCosecha() {
    $("#LocalidadCosecha").click(function () {
        $("#LocalidadCosecha").data("kendoAutoComplete").value("");
        $("#LocalidadCosecha").trigger("change");
        if ($("#LocalidadCosecha").val() == "") {
            $("#PartidoC").val('');
            $("#LocalidadIdCosecha").val("");
        }
    });

    $("#LocalidadCosecha").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function () {
            if ($("#LocalidadCosecha").val().split('|').length > 1) {
                $("#LocalidadCosecha").val($("#LocalidadCosecha").val().split('|')[1]);

            }

        },
        select: function (e) {
            $("#PartidoCosecha").val(e.dataItem.Partido);
            $("#LocalidadIdCosecha").val(e.dataItem.Id);
            $("#LocalidadNombreCosecha").val(e.dataItem.Localidad);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#LocalidadCosecha').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
}

function AutocompleteProcedenciaSituacionCultivo() {
    $("#LocalidadSituacionCultivo").click(function () {
        $("#LocalidadSituacionCultivo").data("kendoAutoComplete").value("");
        $("#LocalidadSituacionCultivo").trigger("change");
        if ($("#LocalidadSituacionCultivo").val() == "") {
            $("#PartidoSituacionCultivo").val('');
            $("#LocalidadIdSituacionCultivo").val("0");
        }
    });

    $("#LocalidadSituacionCultivo").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function () {
            if ($("#LocalidadSituacionCultivo").val().split('|').length > 1) {
                $("#LocalidadSituacionCultivo").val($("#LocalidadSituacionCultivo").val().split('|')[1]);

            }

        },
        select: function (e) {
            $("#PartidoSituacionCultivo").val(e.dataItem.Partido);
            $("#LocalidadIdSituacionCultivo").val(e.dataItem.Id);
            $("#LocalidadNombreSituacionCultivo").val(e.dataItem.Localidad);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#LocalidadSituacionCultivo').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
}
function AutocompleteProcedenciaVentaStock() {
    $("#LocalidadVentaStock").click(function () {
        $("#LocalidadVentaStock").data("kendoAutoComplete").value("");
        $("#LocalidadVentaStock").trigger("change");
        if ($("#LocalidadVentaStock").val() == "") {
            $("#PartidoVentaStock").val('');
            $("#LocalidadVentaStockId").val("");
        }
    });

    $("#LocalidadVentaStock").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function () {
            if ($("#LocalidadVentaStock").val().split('|').length > 1) {
                $("#LocalidadVentaStock").val($("#LocalidadVentaStock").val().split('|')[1]);

            }

        },
        select: function (e) {
            $("#PartidoVentaStock").val(e.dataItem.Partido);
            $("#LocalidadVentaStockId").val(e.dataItem.Id);
            $("#LocalidadNombreVentaStock").val(e.dataItem.Localidad);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#LocalidadVentaStock').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
}