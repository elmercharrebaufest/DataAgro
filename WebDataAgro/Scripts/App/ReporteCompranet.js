var fechaString;
var url;
var viewModel;
var datosIniCrearContrato;

$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    fechaString = ObtenerFechaDesde();
    InicializarDate();
    setInterval(Refrescar, 300000);
    CargarComboCentro();

});

function InicializarDate() {
    kendo.culture("es-AR");
    var date = ObtenerFechaDesde();
    $("#fecha").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            var datepicker = $("#fechaHasta").data("kendoDatePicker");
            datepicker.destroy();
            var fechaMax = new Date($("#fecha").val().toString().split('-')[2], parseInt($("#fecha").val().toString().split('-')[1], 10) - 1, $("#fecha").val().toString().split('-')[0]);
            fechaMax.setMonth(fechaMax.getMonth() + 6);
            $("#fechaHasta").kendoDatePicker({
                value: $("#fecha").val(),
                format: "dd-MM-yyyy",
                parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
                min: new Date($("#fecha").val().toString().split('-')[2], parseInt($("#fecha").val().toString().split('-')[1], 10) - 1, $("#fecha").val().toString().split('-')[0]),
                max: fechaMax
            });
            fechaString = $("#fecha").val();
            fechaHastaString = $("#fechaHasta").val();
            $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString + '&centroId=' + ObtenerValorCentroId());
        }
    });
    var fechaMax = new Date($("#fecha").val().toString().split('-')[2], parseInt($("#fecha").val().toString().split('-')[1], 10) - 1, $("#fecha").val().toString().split('-')[0]);
    fechaMax.setMonth(fechaMax.getMonth() + 6);
    $("#fechaHasta").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        max: fechaMax,
        min: new Date($("#fecha").val().toString().split('-')[2], parseInt($("#fecha").val().toString().split('-')[1], 10) - 1, $("#fecha").val().toString().split('-')[0])
    });
    $("#fechaHasta").change(function () {
        fechaString = $("#fecha").val();
        fechaHastaString = $("#fechaHasta").val();
        $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString + '&centroId=' + ObtenerValorCentroId());
    });

}

function ObtenerFechaDesde() {
    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

function Refrescar() {
    $("#buscar-reporte").click();
}

function ObtenerValorCentroId() {
    return $("#centroId").val() ? $("#centroId").val() : "0";
}

function AbrirModal(material, mes, anio, fechaDesde, fechaHasta, materialNombre, mesNombre) {
    var calidad;
    if (materialNombre == "TRIGO CALIDAD") calidad = 2;
    else if (materialNombre == "TRIGO GRADO 2") calidad = 7;
    else if (materialNombre == "TRIGO CÁMARA") calidad = 3;
    else calidad = null;
    setearTituloModal(materialNombre, mesNombre, anio);
    var href = window.location.href;
    href = href + "/DetalleExcelModal?mes=" + mes + "&anio=" + anio + "&materialId=" + material + "&fechaString=" + fechaDesde + "&fechaHastaString=" + fechaHasta + "&centroId=" + ObtenerValorCentroId() + "&clasificacion=" + calidad;

    $.get(href, function (data) { crearGrilladetallePosicion(data); });
    return false;
}
function AbrirModalIds(anio, materialNombre, mesNombre, negocioids, tiponegocioids, moneda) {
    setearTituloModal(materialNombre, mesNombre, anio);
    var href = window.location.href;
    if (moneda == null) {
        moneda = "";
    }
    href = href + "/DetalleIdsModal?" /*+ "&tiponegocioids=" + tiponegocioids*/ + "&negocioids=" + negocioids + "&moneda=" + moneda;

    $.get(href, function (data) { crearGrilladetallePosicion(data); });
    return false;
}
function setearTituloModal(materialNombre, mesNombre, anio) {
    $('#titulo').empty();
    $('#titulo').text('DETALLE ' + materialNombre + ' ' + mesNombre + ' - ' + anio);
}

function ModalAgenteCompras(fecha) {
    var href = window.location.href;
    href = href + "/DetalleAgenteModal?fechaString=" + fecha;
    $.get(href, function (data) { crearGrillaAgente(data); });
    return false;
}

function CrearGraficoHedgeObjetivo(data) {

    var cumplirPricing = data.PricingObjetivo > data.PricingCumplido ? data.PricingCumplido : data.PricingObjetivo;
    var cumplirRemitir = data.RemitirObjetivo > data.RemitirCumplido ? data.RemitirCumplido : data.RemitirObjetivo;
    var objetivoPricing = data.PricingObjetivo > data.PricingCumplido ? data.PricingObjetivo - data.PricingCumplido : 0;
    var objetivoRemitir = data.RemitirObjetivo > data.RemitirCumplido ? data.RemitirObjetivo - data.RemitirCumplido : 0;
    var excedidoPricing = data.PricingObjetivo < data.PricingCumplido ? data.PricingCumplido - data.PricingObjetivo : 0;
    var excedidoRemitir = data.RemitirObjetivo < data.RemitirCumplido ? data.RemitirCumplido - data.RemitirObjetivo : 0;

    var popCanvas = document.getElementById("graficoHedge");

    var barChart = new Chart(popCanvas, {
        type: 'horizontalBar',

        data: {
            labels: ["Pricing", "A Remitir"],
            datasets: [
                {
                    label: 'Cumplido',
                    data: [cumplirPricing, cumplirRemitir],
                    backgroundColor: '#92d050',
                    borderColor: '#ff1414',
                    borderWidth: 1,
                    segmentShowStroke: false

                }, {
                    label: 'A Cumplir',
                    data: [objetivoPricing, objetivoRemitir],
                    backgroundColor: '#fff',
                    borderColor: '#ff1414',
                    borderWidth: 1
                },
                {
                    label: 'Excedido',
                    data: [excedidoPricing, excedidoRemitir],
                    backgroundColor: '#92d050',
                    //borderColor: '#085200',
                    borderWidth: 1
                },
            ]
        },
        options: {
            legend: {
                display: false
            },
            scales: {
                xAxes: [{
                    stacked: true, barThickness: 10,
                    ticks: {
                        beginAtZero: true,
                        userCallback: function (value, index, values) {
                            value = value.toString();
                            value = value.split(/(?=(?:...)*$)/);
                            value = value.join('.');
                            return value;
                        }
                    }
                }],
                yAxes: [{
                    stacked: true, barThickness: 10
                }]
            },
            tooltips: {
                enabled: false
            }

        }
    });
}

function crearGrilladetallePosicion(href) {
    $("#grilla").kendoGrid({
        culture: "es-AR",
        dataSource: {
            data: JSON.parse(href),
            type: JSON,
            schema: {
                data: "items",
                total: "total"
            },
            pageSize: 20
        },
        dataBound: ShowModal,
        sortable: true,
        reorderable: false,
        groupable: false,
        resizable: true,
        filterable: true,
        columnMenu: true,
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
        columns: [
            {
                field: "Contrato",
                title: "Nro",
                width: 50
            }, {
                field: "RazonSocial",
                title: "Razon Social",
                width: 150
            }, {
                field: "Cuit",
                title: "CUIT",
                width: 120
            }, {
                field: "RazonCorredor",
                title: "Corredor",
                width: 150
            }, {
                field: "CuitCorredor",
                title: "CUIT",
                width: 120
            }, {
                field: "Material",
                title: "Material",
                width: 100
            }, {
                field: "TipoNegocio",
                title: "Negocio",
                width: 100
            }, {
                field: "Comercial",
                title: "Comercial",
                width: 150
            }, {
                field: "Cantidad",
                title: "Cantidad",
                width: 100
            }, {
                field: "CantidadCamiones",
                title: "Camiones",
                width: 100
            }, {
                field: "Campana",
                title: "Campaña",
                width: 100
            }, {
                field: "FechaDesde",
                title: "Fecha<br> Desde",
                width: 100
            }, {
                field: "FechaHasta",
                title: "Fecha <br>Hasta",
                width: 100
            }, {
                field: "Precio",
                title: "Precio",
                width: 100
            }, {
                field: "PrecioNeto",
                title: "Precio <br>Neto",
                width: 100
            }, {
                field: "Moneda",
                title: "Moneda",
                width: 80
            }, {
                field: "Fecha",
                title: "Fecha <br> Operación",
                width: 110
            }, {
                field: "Provincia",
                title: "Provincia",
                width: 150
            }, {
                field: "Localidad",
                title: "Localidad",
                width: 150
            }, {
                field: "Boleto",
                title: "Boleto",
                width: 150
            }, {
                field: "Bolsa",
                title: "Bolsa",
                width: 150
            }, {
                field: "Destino",
                title: "Destino",
                width: 150
            }, {
                field: "CondicionFijacion",
                title: "Condición<br> Fijacion",
                width: 150
            }, {
                field: "DesdeFijacion",
                title: "Desde<br> Fijacion",
                width: 100
            }, {
                field: "HastaFijacion",
                title: "Hasta<br>Fijacion",
                width: 100
            }, {
                field: "Base",
                title: "Base",
                width: 150
            }, {
                field: "ImporteSustentable",
                title: "Importe<br> Sustentable",
                width: 150
            }, {
                field: "FechaDolarizado",
                title: "Fecha<br> Dolarizado",
                width: 150
            }, {
                field: "DiasPesificado",
                title: "Días<br> Pesificado",
                width: 150
            }, {
                field: "NoInformaSio",
                title: "No Informa Sio",
                width: 150
            }, {
                field: "Ampliaciones",
                title: "Ampliaciones",
                width: 150
            }, {
                field: "Consignatario",
                title: "Consignatario",
                width: 150
            }, {
                field: "PlanCanje",
                title: "Plan Canje",
                width: 150
            }, {
                field: "Consignatario",
                title: "Consignatario",
                width: 150
            }
            , {
                field: "Pago",
                title: "Pago",
                width: 150
            }
            , {
                field: "CalidadEspecial",
                title: "Calidad <br>Especial",
                width: 150
            }
            , {
                field: "Establecimiento",
                title: "Establecimiento",
                width: 150
            }
            , {
                field: "Observación",
                title: "Observación",
                width: 150
            }]

    });

  
}

function OcultarColumnasVacias(grid) {
    //var grid = $("#grid").data("kendoGrid");
    var data = grid.dataSource.data();
    for (var j = 0; j < grid.columns.length; j++) {

        var field = grid.columns[j].field;
        var title = grid.columns[j].title;

        var hideColumn = true;

        for (var i = 0; i < data.length; i++) {
            if (data[i][field] != 0 && data[i][field] != "" && data[i][field] != null) {
                hideColumn = false;
                break;
            }
        }
        
        if (hideColumn) {
            grid.hideColumn(j);
           
        } 
    }

}

function crearGrillaAgente(href) {
    $("#grillaAgente").kendoGrid({
        dataSource: {
            data: JSON.parse(href),
            type: JSON,
            schema: {
                data: "items"
            },
            pageSize: 20
        },
        dataBound: ShowModalAgente,
        sortable: true,
        filterable: true,
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
        columns: [
            {
                field: "Agente",
                title: "Nro",
                width: 50
            }, {
                field: "Operador",
                title: "Operador",
                width: 100
            }, {
                field: "Material",
                title: "Material",
                width: 100
            }, {
                field: "Posicion",
                title: "Posicion",
                width: 100
            }, {
                field: "Cantidad",
                title: "Cantidad",
                width: 100
            }, {
                field: "Precio",
                title: "Precio",
                width: 100
            }, {
                field: "Fecha",
                title: "Fecha",
                width: 100
            }, {
                field: "Comercial",
                title: "Comercial",
                width: 100
            }]
    });

}

function ShowModal(e) {
    //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
    $("#ModalDetallePosicion").on('shown.bs.modal', function () {
        $(document).off('focusin.modal');
    });
    $("#ModalDetallePosicion").on('hidden.bs.modal', function () {
        //$("#grilla").kendoGrid().destroy;
        $('#grilla').kendoGrid('destroy').empty();
    });
    $("#ModalDetallePosicion").modal('show');
    OcultarColumnasVacias($("#grilla").data("kendoGrid"));
}

function ShowModalAgente() {
    //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
    $("#ModalAgenteCompra").on('shown.bs.modal', function () {
        $(document).off('focusin.modal');
    });
    $("#ModalAgenteCompra").modal('show');
}


function CargarComboCentro() {
    var href = window.location.href;
    href = href + "/ObtenerCentros";
    $.get(href, function (data) {
        $("#centroId").kendoDropDownList({
            dataSource: {
                data: JSON.parse(data),
                type: JSON,
                schema: {
                    data: "data"
                }
            },
            optionLabel: {
                Descripcion: "TODOS",
                Id: "0"
            },
            dataTextField: "Descripcion",
            dataValueField: "Id",
            change: function (e) {
                var fechaString = $("#fecha").val();
                var fechaHastaString = $("#fechaHasta").val();
                $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString + '&centroId=' + ObtenerValorCentroId());
            },
            dataBound: setearValoresComboDeInicio
        });

        $("#centroId").closest('.k-dropdown.k-widget').keydown(function (e) {
            if (e.keyCode == 46) {
                var dropdownlist = $("#centroId").data("kendoDropDownList");
                dropdownlist.text("");
            }
        });
    });
}


function setearValoresComboDeInicio() {
    url = $('#descargaReporte').attr('href');
    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaString + '&centroId=' + ObtenerValorCentroId());
}

