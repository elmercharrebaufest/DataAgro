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

$(".alert").ready(function () {
    setTimeout(function () { $(".alert").hide(); }, 5000);
});

function LimpiarForm () {
    $(".limpiar").val("");
    $("#LocalidadCosecha").data("kendoAutoComplete").value("");
    $("#Localidad").data("kendoAutoComplete").value("");
    $("#LocalidadSituacionCultivo").data("kendoAutoComplete").value("");
    $("#LocalidadVentaStock").data("kendoAutoComplete").value("");
    $("#PartidoCosecha").val("");
    $("#MaterialAvanceCosechaId").val("");
    $("#MaterialAvanceSiembraId").val("");
    $("#MaterialVentaStockId").val("");
    $("#MaterialSituacionCultivoId").val("");
    $("#SituacionId").val("");
    var estadioSelect = $("#EstadioId");
    estadioSelect.empty();
    estadioSelect.append($('<option/>', {
        value: null,
        text: "Seleccione Estadío"
    }));
    var campaniaAvanceSiembraSelect = $("#CampaniaAvanceSiembraId");
    campaniaAvanceSiembraSelect.empty();
    campaniaAvanceSiembraSelect.append($('<option/>', {
        value: null,
        text: "Seleccione Campaña"
    }));
    var campaniaSituacionCultivoSelect = $("#CampaniaSituacionCultivoId");
    campaniaSituacionCultivoSelect.empty();
    campaniaSituacionCultivoSelect.append($('<option/>', {
        value: null,
        text: "Seleccione Campaña"
    }));
    var campaniaVentaStockSelect = $("#CampaniaVentaStockId");
    campaniaVentaStockSelect.empty();
    campaniaVentaStockSelect.append($('<option/>', {
        value: null,
        text: "Seleccione Campaña"
    }));
    var campaniaAvanceCosechaSelect = $("#CampaniaAvanceCosechaId");
    campaniaAvanceCosechaSelect.empty();
    campaniaAvanceCosechaSelect.append($('<option/>', {
        value: null,
        text: "Seleccione Campaña"
    }));
    
}

$(".cancelar").click(function () {
    LimpiarForm();
});

$("#AvanceSiembraTab").click(function () {
    DeseleccionarForms();
    LimpiarForm();
    $("#AvanceSiembraTab").children().addClass("whc-selected");
    $("#AvanceSiembra").show();
});
$("#AvanceCosechaTab").click(function () {
    DeseleccionarForms();
    LimpiarForm();
    $("#AvanceCosechaTab").children().addClass("whc-selected");
    $("#AvanceCosecha").show();
});
$("#SituacionCultivoTab").click(function () {
    DeseleccionarForms();
    LimpiarForm();
    $("#SituacionCultivoTab").children().addClass("whc-selected");
    $("#SituacionCultivo").show();
});
$("#VentaStockTab").click(function () {
    DeseleccionarForms();
    LimpiarForm();
    $("#VentaStockTab").children().addClass("whc-selected");
    $("#VentaStock").show();
});

$("#ReporteAvanceSiembraTab").click(function () {
    DeseleccionarFormsReporte();
    $("#ReporteAvanceSiembraTab").children().addClass("whc-selected");
    $("#AvanceSiembra").show();
});
$("#ReporteAvanceCosechaTab").click(function () {
    DeseleccionarFormsReporte();
    $("#ReporteAvanceCosechaTab").children().addClass("whc-selected");
    $("#AvanceCosecha").show();
});
$("#ReporteSituacionCultivoTab").click(function () {
    DeseleccionarFormsReporte();
    $("#ReporteSituacionCultivoTab").children().addClass("whc-selected");
    $("#SituacionCultivo").show();
});
$("#ReporteVentaStockTab").click(function () {
    DeseleccionarFormsReporte();    
    $("#ReporteVentaStockTab").children().addClass("whc-selected");
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
function DeseleccionarFormsReporte() {
    $("#ReporteAvanceSiembraTab").children().removeClass("whc-selected");
    $("#AvanceSiembra").hide();
    $("#ReporteAvanceCosechaTab").children().removeClass("whc-selected");
    $("#AvanceCosecha").hide();
    $("#ReporteSituacionCultivoTab").children().removeClass("whc-selected");
    $("#SituacionCultivo").hide();
    $("#ReporteVentaStockTab").children().removeClass("whc-selected");
    $("#VentaStock").hide();
}
function InicializarElementos() {

    $(".number-input").kendoNumericTextBox({
        culture: "es-AR",
        format: "n1",
        decimals: 1,
        value: " ",
        restrictDecimals: true,
        spinners: false,
        min: 0,
        max: 100
    });
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    $(".number-negativo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false
    });
}

$("#MaterialSituacionCultivoId").change(function () {
    var materialId = $("#MaterialSituacionCultivoId").val();
    var campaniaSelect = $("#CampaniaSituacionCultivoId");
    var estadioSelect = $("#EstadioId");
    campaniaSelect.empty();
    estadioSelect.empty();

    if (materialId != null && materialId != '') {
        var estadio = MSExecuteOnServer('/Research/TraerEstadio', { materialId: materialId });
        estadioSelect.append($('<option/>', {
            value: null,
            text: "Seleccione Estadío"
        }));

        var campania = MSExecuteOnServer('/Research/TraerCampañaPorMaterial', { materialId: materialId });
        campaniaSelect.append($('<option/>', {
            value: null,
            text: "Seleccione Campaña"
        }));

        if (estadio != null && !jQuery.isEmptyObject(estadio)) {            
            $.each(estadio, function (index, estadio) {
                estadioSelect.append($('<option/>', {
                    value: estadio.Value,
                    text: estadio.Text
                }));
            });
        }

        if (campania != null && !jQuery.isEmptyObject(campania)) {
            $.each(campania, function (index, campania) {
                campaniaSelect.append($('<option/>', {
                    value: campania.Value,
                    text: campania.Text
                }));
            });
        }
    }
});   

$("#MaterialAvanceCosechaId").change(function () {
    var materialId = $("#MaterialAvanceCosechaId").val();
    var campaniaSelect = $("#CampaniaAvanceCosechaId");
    
    campaniaSelect.empty();
    
    if (materialId != null && materialId != '') {
        var campaniaAvance = MSExecuteOnServer('/Research/TraerCampañaPorMaterial', { materialId: materialId });
        campaniaSelect.append($('<option/>', {
            value: null,
            text: "Seleccione Campaña"
        }));

        if (campaniaAvance != null && !jQuery.isEmptyObject(campaniaAvance)) {
            $.each(campaniaAvance, function (index, campaniaAvance) {
                campaniaSelect.append($('<option/>', {
                    value: campaniaAvance.Value,
                    text: campaniaAvance.Text
                }));
            });
        }
    }
});  

$("#MaterialAvanceSiembraId").change(function () {
    var materialId = $("#MaterialAvanceSiembraId").val();
    var campaniaSelect = $("#CampaniaAvanceSiembraId");

    campaniaSelect.empty();

    if (materialId != null && materialId != '') {
        var campania = MSExecuteOnServer('/Research/TraerCampañaPorMaterial', { materialId: materialId });
        campaniaSelect.append($('<option/>', {
            value: null,
            text: "Seleccione Campaña"
        }));

        if (campania != null && !jQuery.isEmptyObject(campania)) {
            $.each(campania, function (index, campania) {
                campaniaSelect.append($('<option/>', {
                    value: campania.Value,
                    text: campania.Text
                }));
            });
        }
    }
});  

$("#MaterialVentaStockId").change(function () {
    var materialId = $("#MaterialVentaStockId").val();
    var campaniaSelect = $("#CampaniaVentaStockId");

    campaniaSelect.empty();

    if (materialId != null && materialId != '') {
        var campania = MSExecuteOnServer('/Research/TraerCampañaPorMaterial', { materialId: materialId });
        campaniaSelect.append($('<option/>', {
            value: null,
            text: "Seleccione Campaña"
        }));

        if (campania != null && !jQuery.isEmptyObject(campania)) {
            $.each(campania, function (index, campania) {
                campaniaSelect.append($('<option/>', {
                    value: campania.Value,
                    text: campania.Text
                }));
            });
        }
    }
}); 

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