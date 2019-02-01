var fechaString;
var url;

$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    fechaString = ObtenerFechaDesde();
    url = $('#descargaReporte').attr('href');
    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaString);
    InicializarDate();
    setInterval(Refrescar, 300000);

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
            $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString);
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
        $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString);
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


function AbrirModal(material, mes, anio, fechaDesde, fechaHasta, materialNombre, mesNombre) {
    setearTituloModal(materialNombre, mesNombre, anio);
    var href = window.location.href;
    href = href + "/DetalleExcelModal?mes=" + mes + "&anio=" + anio + "&materialId=" + material + "&fechaString=" + fechaDesde + "&fechaHastaString=" + fechaHasta;
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
                    backgroundColor: '#92d050', // yellow
                    borderColor: '#4a682a',
                    borderWidth: 1
                }, {
                    label: 'A Cumplir',
                    data: [objetivoPricing, objetivoRemitir],
                    backgroundColor: '#ddd', // red,
                    borderColor: 'black',
                    borderWidth: 1
                },
                {
                    label: 'Excedido',
                    data: [excedidoPricing, excedidoRemitir],
                    backgroundColor: '#12bd00', // green
                    borderColor: '#085200',
                    borderWidth: 1
                }]
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
            pageSize: 5
        },
        dataBound: ShowModal,
        sortable: true,
        reorderable: false,
        groupable: false,
        resizable: true,
        filterable: true,
        columnMenu: true,
        pageable: true,
        columns: [
            {
                field: "Contrato",
                title: "Nro Contrato",
                width: 150
            }, {
                field: "RazonSocial",
                title: "Razon Social",
                width: 150
            }, {
                field: "Cuit",
                title: "CUIT",
                width: 120
            }, {
                field: "Material",
                title: "Material",
                width: 150
            }, {
                field: "TipoNegocio",
                title: "Tipo Negocio",
                width: 150
            }, {
                field: "Comercial",
                title: "Comercial",
                width: 150
            }, {
                field: "Cantidad",
                title: "Cantidad",
                width: 150
            }, {
                field: "CantidadCamiones",
                title: "Cantidad Camiones",
                width: 150
            }, {
                field: "Campana",
                title: "Campaña",
                width: 150
            }, {
                field: "FechaDesde",
                title: "Fecha Desde",
                width: 150
            }, {
                field: "FechaHasta",
                title: "Fecha Hasta",
                width: 150
            }, {
                field: "Precio",
                title: "Precio",
                width: 150
            }, {
                field: "Moneda",
                title: "Moneda",
                width: 150
            }, {
                field: "Fecha",
                title: "Fecha Operación",
                width: 150
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
                title: "Condición Fijacion",
                width: 150
            }, {
                field: "DesdeFijacion",
                title: "Desde Fijacion",
                width: 150
            }, {
                field: "HastaFijacion",
                title: "Hasta Fijacion",
                width: 150
            }, {
                field: "Base",
                title: "Base",
                width: 150
            }, {
                field: "ImporteSustentable",
                title: "Importe Sustentable",
                width: 150
            }, {
                field: "FechaDolarizado",
                title: "Fecha Dolarizado",
                width: 150
            }, {
                field: "DiasPesificado",
                title: "Días Pesificado",
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
                title: "Calidad Especial",
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
            }],

    });

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
                title: "Nro Agente",
                width: 150
            }, {
                field: "Operador",
                title: "Operador",
                width: 150
            }, {
                field: "Material",
                title: "Material",
                width: 120
            }, {
                field: "Posicion",
                title: "Posicion",
                width: 150
            }, {
                field: "Cantidad",
                title: "Cantidad",
                width: 150
            }, {
                field: "Precio",
                title: "Precio",
                width: 150
            }, {
                field: "Fecha",
                title: "Fecha",
                width: 150
            }, {
                field: "Comercial",
                title: "Comercial",
                width: 150
            }]
    });

}

function ShowModal(e) {
    //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
    $("#ModalDetallePosicion").on('shown.bs.modal', function () {
        $(document).off('focusin.modal');
    });
    $("#ModalDetallePosicion").on('hidden.bs.modal', function () {
        $("#grilla").kendoGrid().destroy;
    });
    $("#ModalDetallePosicion").modal('show');
}

function ShowModalAgente() {
    //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
    $("#ModalAgenteCompra").on('shown.bs.modal', function () {
        $(document).off('focusin.modal');
    });
    $("#ModalAgenteCompra").modal('show');
}