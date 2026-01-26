var fechaString;
var url = "/ReporteCompraNet/ReporteComprasDelDia";
var viewModel;
var datosIniCrearContrato;
var getUrl = window.location;
var baseUrl =
  getUrl.protocol + "//" + getUrl.host + "/" + getUrl.pathname.split("/")[1];

$(document).ready(function () {
  kendo.culture("es-AR");
  $("#menuproveedor").hide();
  fechaString = ObtenerFechaDesde();
  InicializarDate();
  setInterval(Refrescar, 300000);
  CargarComboCentro();
  CargarComboMaterial();
  $("#verFijaciones").on("change", function () {
    setearValoresComboDeInicio();
  });
  //InicializarModalFijacion();
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
      var fechaMax = new Date(
        $("#fecha").val().toString().split("-")[2],
        parseInt($("#fecha").val().toString().split("-")[1], 10) - 1,
        $("#fecha").val().toString().split("-")[0],
      );
      fechaMax.setMonth(fechaMax.getMonth() + 6);
      $("#fechaHasta").kendoDatePicker({
        value: $("#fecha").val(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        min: new Date(
          $("#fecha").val().toString().split("-")[2],
          parseInt($("#fecha").val().toString().split("-")[1], 10) - 1,
          $("#fecha").val().toString().split("-")[0],
        ),
        max: fechaMax,
      });
      fechaString = $("#fecha").val();
      fechaHastaString = $("#fechaHasta").val();
      $("#descargaReporte").attr(
        "href",
        url +
          "?fechaString=" +
          fechaString +
          "&fechaHastaString=" +
          fechaHastaString +
          "&centroId=" +
          ObtenerValorCentroId() +
          "&materialId=" +
          ObtenerValorMaterialId().toString() +
          "&verFijaciones=" +
          getVerFijaciones(),
      );
    },
  });
  var fechaMax = new Date(
    $("#fecha").val().toString().split("-")[2],
    parseInt($("#fecha").val().toString().split("-")[1], 10) - 1,
    $("#fecha").val().toString().split("-")[0],
  );
  fechaMax.setMonth(fechaMax.getMonth() + 6);
  $("#fechaHasta").kendoDatePicker({
    value: date,
    format: "dd-MM-yyyy",
    parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
    max: fechaMax,
    min: new Date(
      $("#fecha").val().toString().split("-")[2],
      parseInt($("#fecha").val().toString().split("-")[1], 10) - 1,
      $("#fecha").val().toString().split("-")[0],
    ),
  });
  $("#fechaHasta").change(function () {
    fechaString = $("#fecha").val();
    fechaHastaString = $("#fechaHasta").val();
    $("#descargaReporte").attr(
      "href",
      url +
        "?fechaString=" +
        fechaString +
        "&fechaHastaString=" +
        fechaHastaString +
        "&centroId=" +
        ObtenerValorCentroId() +
        "&materialId=" +
        ObtenerValorMaterialId().toString() +
        "&verFijaciones=" +
        getVerFijaciones(),
    );
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
  return dia + "-" + mes + "-" + anio;
}

function Refrescar() {
  $("#buscar-reporte").click();
}

function ObtenerValorCentroId() {
  return $("#centroId").val() ? $("#centroId").val() : "0";
}
function ObtenerValorMaterialId() {
  return $("#materialId").data("kendoMultiSelect") == null
    ? ""
    : $("#materialId").data("kendoMultiSelect").value();
}
function AbrirModal(
  material,
  mes,
  anio,
  fechaDesde,
  fechaHasta,
  materialNombre,
  mesNombre,
) {
  var calidad;
  if (materialNombre == "TRIGO CALIDAD") calidad = 2;
  else if (materialNombre == "TRIGO GRADO 2") calidad = 7;
  else if (materialNombre == "TRIGO CÁMARA") calidad = 3;
  else calidad = null;
  setearTituloModal(materialNombre, mesNombre, anio);
  var href = window.location.href;
  href =
    href +
    "/DetalleExcelModal?mes=" +
    mes +
    "&anio=" +
    anio +
    "&materialId=" +
    material +
    "&fechaString=" +
    fechaDesde +
    "&fechaHastaString=" +
    fechaHasta +
    "&centroId=" +
    ObtenerValorCentroId() +
    "&clasificacion=" +
    calidad +
    "&verFijaciones=" +
    getVerFijaciones();

  $.get(href, function (data) {
    crearGrilladetallePosicion(data);
  });
  return false;
}

function AbrirModalIds(
  anio,
  materialNombre,
  mesNombre,
  negocioids,
  tiponegocioids,
  moneda,
  esFijacion,
  verDepositoTipoNegocio,
) {
  setearTituloModal(materialNombre, mesNombre, anio);
  var href = window.location.href;
  if (moneda == null) {
    moneda = "";
  }
  href =
    href +
    "/DetalleIdsModal?" /*+ "&tiponegocioids=" + tiponegocioids*/ +
    "&negocioids=" +
    negocioids +
    "&moneda=" +
    moneda;

  $("#modalContentId").removeAttr("style");
  //$.get(href, function (data) { crearGrilladetallePosicion(data); });
  var list = negocioids.split(",");
  $.post(
    window.location.href + "/DetalleIdsModal",
    {
      negocioids: list,
      moneda: moneda,
      verDepositoTipoNegocio: verDepositoTipoNegocio,
    },
    function (data) {
      if (esFijacion == true) {
        crearGrilladetalleFijacion(data);
        var jsonD = JSON.parse(data);
        jsonD.items = jsonD.items.filter(function (el) {
          return el.TipoNegocioId == 3;
        });
        var dataFijLargas = jsonD.items.filter(function (el) {
          var diferenciaDias = DiferenciaFechasEnDias(
            el.HastaFijacion,
            el.FechaOperacion,
          );
          return diferenciaDias > 31;
        });
        var dataFijCortas = jsonD.items.filter(function (el) {
          var diferenciaDias = DiferenciaFechasEnDias(
            el.HastaFijacion,
            el.FechaOperacion,
          );
          return diferenciaDias <= 31;
        });

        dataFijLargas.sort((a, b) =>
          parseFloat(a.Precio) > parseFloat(b.Precio)
            ? -1
            : parseFloat(b.Precio) > parseFloat(a.Precio)
              ? 1
              : 0,
        );
        dataFijCortas.sort((a, b) =>
          parseFloat(a.Precio) > parseFloat(b.Precio)
            ? -1
            : parseFloat(b.Precio) > parseFloat(a.Precio)
              ? 1
              : 0,
        );

        var dFijLargas = [];
        dataFijLargas.reduce(function (res, value) {
          if (!res[value.HastaFijacion + value.Moneda]) {
            res[value.HastaFijacion + value.Moneda] = {
              CantidadTotal: 0,
              HastaFijacion: value.HastaFijacion,
              Moneda: value.Moneda,
              Precio: value.Precio,
              Cantidad: value.CantidadD,
            };
            dFijLargas.push(res[value.HastaFijacion + value.Moneda]);
          }
          res[value.HastaFijacion + value.Moneda].CantidadTotal +=
            value.CantidadD;
          return res;
        }, {});
        var dFijCortas = [];
        dataFijCortas.reduce(function (res, value) {
          if (!res[value.HastaFijacion + value.Moneda]) {
            res[value.HastaFijacion + value.Moneda] = {
              CantidadTotal: 0,
              HastaFijacion: value.HastaFijacion,
              Moneda: value.Moneda,
              Precio: value.Precio,
              Cantidad: value.CantidadD,
            };
            dFijCortas.push(res[value.HastaFijacion + value.Moneda]);
          }
          res[value.HastaFijacion + value.Moneda].CantidadTotal +=
            value.CantidadD;
          return res;
        }, {});

        crearGrillaFijacionLargaCorta(
          JSON.stringify({ items: dFijLargas, total: dFijLargas.length }),
          "grillaFijacionLarga",
        );
        crearGrillaFijacionLargaCorta(
          JSON.stringify({ items: dFijCortas, total: dFijCortas.length }),
          "grillaFijacionCorta",
        );
      } else {
        crearGrilladetallePosicion(data);
      }
    },
    "json",
  );
  return false;
}

function AbrirModalSustentableIds(
  anio,
  materialNombre,
  mesNombre,
  negocioids,
  tiponegocioids,
  moneda,
) {
  setearTituloModal(materialNombre, mesNombre, anio);
  var href = window.location.href;
  if (moneda == null) {
    moneda = "";
  }
  href =
    href +
    "/DetalleIdsSojaModal?" /*+ "&tiponegocioids=" + tiponegocioids*/ +
    "&negocioids=" +
    negocioids +
    "&moneda=" +
    moneda;

  //$.get(href, function (data) { crearGrilladetallePosicion(data); });
  var list = negocioids.split(",");
  $.post(
    window.location.href + "/DetalleIdsSojaSustentableModal",
    { negocioids: list, moneda: moneda },
    function (data) {
      crearGrillaSustentablePosicion(data);
    },
    "json",
  );
  return false;
}

//Formato de Fechas DD/MM/YYYY
function DiferenciaFechasEnDias(fechaA, fechaB) {
  var partsA = fechaA.split("/");
  var fechaADate = new Date(
    Date.parse(partsA[2] + "/" + partsA[1] + "/" + partsA[0]),
  );
  var partsB = fechaB.split("/");
  var fechaBDate = new Date(
    Date.parse(partsB[2] + "/" + partsB[1] + "/" + partsB[0]),
  );
  return (fechaADate.getTime() - fechaBDate.getTime()) / (1000 * 3600 * 24);
}

function setearTituloModal(materialNombre, mesNombre, anio) {
  $("#titulo").empty();
  if (anio == "") {
    $("#titulo").text("DETALLE " + materialNombre + " " + mesNombre);
  } else {
    $("#titulo").text(
      "DETALLE " + materialNombre + " " + mesNombre + " - " + anio,
    );
  }
}

function ModalAgenteCompras(fecha) {
  var href = window.location.href;
  href =
    href +
    "/DetalleAgenteModal?fechaString=" +
    fecha +
    "&materialId=" +
    ObtenerValorMaterialId().toString();
  $.get(href, function (data) {
    crearGrillaAgente(data);
  });
  return false;
}

function CrearGraficoHedgeObjetivo(data) {
  var cumplirPricing =
    data.PricingObjetivo > data.PricingCumplido
      ? data.PricingCumplido
      : data.PricingObjetivo;
  var cumplirRemitir =
    data.RemitirObjetivo > data.RemitirCumplido
      ? data.RemitirCumplido
      : data.RemitirObjetivo;
  var objetivoPricing =
    data.PricingObjetivo > data.PricingCumplido
      ? data.PricingObjetivo - data.PricingCumplido
      : 0;
  var objetivoRemitir =
    data.RemitirObjetivo > data.RemitirCumplido
      ? data.RemitirObjetivo - data.RemitirCumplido
      : 0;
  var excedidoPricing =
    data.PricingObjetivo < data.PricingCumplido
      ? data.PricingCumplido - data.PricingObjetivo
      : 0;
  var excedidoRemitir =
    data.RemitirObjetivo < data.RemitirCumplido
      ? data.RemitirCumplido - data.RemitirObjetivo
      : 0;

  var popCanvas = document.getElementById("graficoHedge");

  var barChart = new Chart(popCanvas, {
    type: "horizontalBar",

    data: {
      labels: ["Pricing", "A Remitir"],
      datasets: [
        {
          label: "Cumplido",
          data: [cumplirPricing, cumplirRemitir],
          backgroundColor: "#92d050",
          borderColor: "#ff1414",
          borderWidth: 1,
          segmentShowStroke: false,
        },
        {
          label: "A Cumplir",
          data: [objetivoPricing, objetivoRemitir],
          backgroundColor: "#fff",
          borderColor: "#ff1414",
          borderWidth: 1,
        },
        {
          label: "Excedido",
          data: [excedidoPricing, excedidoRemitir],
          backgroundColor: "#92d050",
          //borderColor: '#085200',
          borderWidth: 1,
        },
      ],
    },
    options: {
      legend: {
        display: false,
      },
      scales: {
        xAxes: [
          {
            stacked: true,
            barThickness: 10,
            ticks: {
              beginAtZero: true,
              userCallback: function (value, index, values) {
                value = value.toString();
                value = value.split(/(?=(?:...)*$)/);
                value = value.join(".");
                return value;
              },
            },
          },
        ],
        yAxes: [
          {
            stacked: true,
            barThickness: 10,
          },
        ],
      },
      tooltips: {
        enabled: false,
      },
    },
  });
}

function crearGrilladetallePosicion(href, esFijacion) {
  // Destruir grilla anterior si existe
  var gridElement = $("#grilla");
  if (gridElement.data("kendoGrid")) {
    gridElement.data("kendoGrid").destroy();
    gridElement.empty();
  }

  $("#grilla").kendoGrid({
    culture: "es-AR",
    dataSource: {
      data: JSON.parse(href),
      type: JSON,
      schema: {
        data: "items",
        total: "total",
        model: {
          fields: {
            FechaDesde: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            FechaHasta: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            Fecha: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            DesdeFijacion: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            HastaFijacion: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
          },
        },
      },
      pageSize: 20,

      aggregate: [{ field: "CantidadD", aggregate: "sum" }],
    },
    dataBound: esFijacion == true ? ShowModalFijacion : ShowModal,
    sortable: true,
    height: 400,
    scrollable: {
      virtual: false,
    },
    reorderable: false,
    groupable: false,
    resizable: true,
    filterable: {
      checkAll: false,
      height: 350,
      extra: false,
      messages: {
        info: "Filtros:",
        filter: "Filtrar",
        clear: "Limpiar",
        isTrue: "SI",
        isFalse: "NO",
        and: "Y",
        or: "O",
      },
      operators: {
        string: {
          eq: "Igual",
          neq: "Distinto",
          startswith: "Comienza con",
          contains: "Contiene",
          endswith: "Finaliza con",
          gte: "Mayor que o igual a",
          lte: "Menor que o igual a",
        },
        date: {
          eq: "Igual",
          gte: "Despu&eacute;s o igual a",
          lte: "Antes o igual a",
        },
        number: {
          eq: "Igual a",
          gte: "Mayor que o igual a",
          lte: "Menor que o igual a",
        },
      },
    },
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
        refresh: "Recargar",
      },
      input: true,
      numeric: true,
    },
    columns: [
      {
        field: "Contrato",
        title: "Nro",
        width: 80,
      },
      {
        field: "RazonSocial",
        title: "Razon Social",
        width: 150,
      },
      {
        field: "Cuit",
        title: "CUIT",
        width: 120,
      },
      {
        field: "RazonCorredor",
        title: "Corredor",
        width: 150,
      },
      {
        field: "CuitCorredor",
        title: "CUIT",
        width: 120,
      },
      {
        field: "Material",
        title: "Material",
        width: 100,
      },
      {
        field: "TipoNegocio",
        title: "Negocio",
        width: 100,
      },
      {
        field: "Comercial",
        title: "Comercial",
        width: 150,
      },
      {
        field: "CantidadD",
        title: "Cantidad",
        width: 110,
        format: "{0:n0}",
        type: "number",
        aggregates: ["sum"],
        footerTemplate: '#=kendo.toString(sum, "n0")#',
      },
      {
        field: "CantidadCamiones",
        title: "Camiones",
        width: 110,
      },
      {
        field: "Campana",
        title: "Campaña",
        width: 110,
      },
      {
        field: "FechaDesde",
        title: "Fecha<br> Desde",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "FechaHasta",
        title: "Fecha <br>Hasta",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "Precio",
        title: "Precio",
        width: 100,
      },
      {
        field: "PrecioNeto",
        title: "Precio <br>Neto",
        width: 100,
      },
      {
        field: "Moneda",
        title: "Moneda",
        width: 90,
      },
      {
        field: "Fecha",
        title: "Fecha <br> Operación",
        width: 120,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "Provincia",
        title: "Provincia",
        width: 150,
      },
      {
        field: "Localidad",
        title: "Localidad",
        width: 150,
      },
      {
        field: "Boleto",
        title: "Boleto",
        width: 150,
      },
      {
        field: "Bolsa",
        title: "Bolsa",
        width: 150,
      },
      {
        field: "Destino",
        title: "Destino",
        width: 150,
      },
      {
        field: "CondicionFijacion",
        title: "Condición<br> Fijacion",
        width: 150,
      },
      {
        field: "DesdeFijacion",
        title: "Desde<br> Fijacion",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "HastaFijacion",
        title: "Hasta<br>Fijacion",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "Base",
        title: "Base",
        width: 150,
      },
      {
        field: "ImporteSustentable",
        title: "Importe<br> Sustentable",
        width: 150,
      },
      {
        field: "FechaDolarizado",
        title: "Fecha<br> Dolarizado",
        width: 150,
      },
      {
        field: "DiasPesificado",
        title: "Días<br> Pesificado",
        width: 150,
      },
      {
        field: "NoInformaSio",
        title: "No Informa Sio",
        width: 150,
      },
      {
        field: "Ampliaciones",
        title: "Ampliaciones",
        width: 150,
      },
      {
        field: "Consignatario",
        title: "Consignatario",
        width: 150,
      },
      {
        field: "PlanCanje",
        title: "Plan Canje",
        width: 150,
      },
      {
        field: "Consignatario",
        title: "Consignatario",
        width: 150,
      },
      {
        field: "Pago",
        title: "Pago",
        width: 150,
      },
      //, {
      //    field: "CalidadEspecial",
      //    title: "Calidad <br>Especial",
      //    width: 150,
      //}
      {
        field: "CalidadEspecial",
        title: "Calidad <br>Especial",
        width: 110,
      },
      {
        field: "MercsDeposito",
        title: "Merc. en <br>Deposito",
        width: 110,
      },
      {
        field: "Observación",
        title: "Observación",
        width: 150,
      },
    ],
  });
}

function crearGrilladetalleFijacion(href) {
  // Destruir grilla anterior si existe
  var gridElement = $("#grillaDetalle");
  if (gridElement.data("kendoGrid")) {
    gridElement.data("kendoGrid").destroy();
    gridElement.empty();
  }

  $("#grillaDetalle").kendoGrid({
    culture: "es-AR",
    dataSource: {
      data: JSON.parse(href),
      type: JSON,
      schema: {
        data: "items",
        total: "total",
        model: {
          fields: {
            FechaDesde: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            FechaHasta: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            Fecha: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            DesdeFijacion: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
            HastaFijacion: {
              type: "date",
              parse: function (value) {
                if (!value) return null;
                if (value instanceof Date) return value;
                if (typeof value === "string") {
                  var parts = value.split("/");
                  if (parts.length === 3) {
                    var d = new Date(parts[2], parts[1] - 1, parts[0]);
                    d.setHours(0, 0, 0, 0);
                    return d;
                  }
                }
                return null;
              },
            },
          },
        },
      },
      pageSize: 20,

      aggregate: [{ field: "CantidadD", aggregate: "sum" }],
    },
    dataBound: ShowModalFijacion,
    sortable: true,
    height: 380,
    scrollable: true,
    reorderable: false,
    groupable: false,
    resizable: true,
    filterable: {
      extra: false,
      messages: {
        info: "Filtros:",
        filter: "Filtrar",
        clear: "Limpiar",
      },
    },
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
        refresh: "Recargar",
      },
      input: true,
      numeric: true,
    },
    columns: [
      {
        field: "Contrato",
        title: "Nro",
        width: 80,
      },
      {
        field: "RazonSocial",
        title: "Razon Social",
        width: 150,
      },
      {
        field: "Cuit",
        title: "CUIT",
        width: 120,
      },
      {
        field: "RazonCorredor",
        title: "Corredor",
        width: 150,
      },
      {
        field: "CuitCorredor",
        title: "CUIT",
        width: 120,
      },
      {
        field: "Material",
        title: "Material",
        width: 100,
      },
      {
        field: "TipoNegocio",
        title: "Negocio",
        width: 100,
      },
      {
        field: "Comercial",
        title: "Comercial",
        width: 150,
      },
      {
        field: "CantidadD",
        title: "Cantidad",
        width: 110,
        format: "{0:n0}",
        type: "number",
        aggregates: ["sum"],
        footerTemplate: '#=kendo.toString(sum, "n0")#',
      },
      {
        field: "CantidadCamiones",
        title: "Camiones",
        width: 110,
      },
      {
        field: "Campana",
        title: "Campaña",
        width: 110,
      },
      {
        field: "FechaDesde",
        title: "Fecha<br> Desde",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "FechaHasta",
        title: "Fecha <br>Hasta",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "Precio",
        title: "Precio",
        width: 100,
      },
      {
        field: "PrecioNeto",
        title: "Precio <br>Neto",
        width: 100,
      },
      {
        field: "Moneda",
        title: "Moneda",
        width: 90,
      },
      {
        field: "Fecha",
        title: "Fecha <br> Operación",
        width: 120,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "Provincia",
        title: "Provincia",
        width: 150,
      },
      {
        field: "Localidad",
        title: "Localidad",
        width: 150,
      },
      {
        field: "Boleto",
        title: "Boleto",
        width: 150,
      },
      {
        field: "Bolsa",
        title: "Bolsa",
        width: 150,
      },
      {
        field: "Destino",
        title: "Destino",
        width: 150,
      },
      {
        field: "CondicionFijacion",
        title: "Condición<br> Fijacion",
        width: 150,
      },
      {
        field: "DesdeFijacion",
        title: "Desde<br> Fijacion",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "HastaFijacion",
        title: "Hasta<br>Fijacion",
        width: 100,
        type: "date",
        format: "{0:dd/MM/yyyy}",
      },
      {
        field: "Base",
        title: "Base",
        width: 150,
      },
      {
        field: "ImporteSustentable",
        title: "Importe<br> Sustentable",
        width: 150,
      },
      {
        field: "FechaDolarizado",
        title: "Fecha<br> Dolarizado",
        width: 150,
      },
      {
        field: "DiasPesificado",
        title: "Días<br> Pesificado",
        width: 150,
      },
      {
        field: "NoInformaSio",
        title: "No Informa Sio",
        width: 150,
      },
      {
        field: "Ampliaciones",
        title: "Ampliaciones",
        width: 150,
      },
      {
        field: "Consignatario",
        title: "Consignatario",
        width: 150,
      },
      {
        field: "PlanCanje",
        title: "Plan Canje",
        width: 150,
      },
      {
        field: "Consignatario",
        title: "Consignatario",
        width: 150,
      },
      {
        field: "Pago",
        title: "Pago",
        width: 150,
      },
      //, {
      //    field: "CalidadEspecial",
      //    title: "Calidad <br>Especial",
      //    width: 150,
      //}
      {
        field: "CalidadEspecial",
        title: "Calidad <br>Especial",
        width: 110,
      },
      {
        field: "MercsDeposito",
        title: "Merc. en <br>Deposito",
        width: 110,
      },
      {
        field: "Observación",
        title: "Observación",
        width: 150,
      },
    ],
  });
}

function crearGrillaFijacionLargaCorta(href, grilla) {
  $("#" + grilla + "").kendoGrid({
    culture: "es-AR",
    dataSource: {
      data: JSON.parse(href),
      type: JSON,
      schema: {
        data: "items",
        total: "total",
      },
      pageSize: 20,

      aggregate: [{ field: "CantidadTotal", aggregate: "sum" }],
    },
    //dataBound: ShowModalFijacion,
    sortable: false,
    scrollable: false,
    reorderable: false,
    groupable: false,
    resizable: true,
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
        refresh: "Recargar",
      },
      //input: true,
      //numeric: true
    },
    columns: [
      {
        title:
          grilla == "grillaFijacionLarga"
            ? "Fijaciones Largas"
            : "Fijaciones Cortas",
        columns: [
          {
            field: "HastaFijacion",
            title: "Fijación Hasta",
            width: 50,
          },
          {
            field: "Moneda",
            title: "Moneda",
            width: 80,
          },
          {
            field: "Precio",
            title: "Precio",
            width: 100,
            template: function (dataItem) {
              return kendo.toString(dataItem.Precio, "n0");
            },
          },
          {
            field: "CantidadTotal",
            title: "Toneladas",
            width: 150,
            template: function (dataItem) {
              return kendo.toString(dataItem.CantidadTotal, "n0");
            },
            aggregates: ["sum"],
            footerTemplate: '#=kendo.toString(sum, "n0")#',
          },
        ],
      },
    ],
  });
}

function crearGrillaSustentablePosicion(href) {
  var modal = document.getElementById("ModalDetallePosicion");
  modal.style.textAlign = "-webkit-center";
  $("#modalContentId").attr("style", "max-width: 500px;");
  $("#grilla").kendoGrid({
    culture: "es-AR",
    dataSource: {
      data: JSON.parse(href),
      type: JSON,
      schema: {
        data: "items",
        total: "total",
      },
      aggregate: [],
    },
    dataBound: ShowModal,
    //sortable: true,
    height: 380,
    scrollable: true,
    reorderable: false,
    groupable: false,
    resizable: true,

    columnMenu: false,
    columns: [
      {
        field: "posicion",
        title: "Posición",
        width: 180,
        headerAttributes: {
          style: "background-color: #017940; text-align: center; color: white",
        },
        template: function (dataItem) {
          return (
            '<span style="font-size: 13px;">' + dataItem.posicion + "</span>"
          );
        },
      },
      {
        field: "cantidad",
        title: "Total (Tn)",
        headerAttributes: {
          style: "background-color: #017940; text-align: center; color: white",
        },
        template: function (dataItem) {
          return (
            '<span style="font-size: 13px;">' +
            kendo.toString(dataItem.cantidad, "n1") +
            "</span>"
          );
        },
        width: 120,
      },
    ],
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
      if (
        data[i][field] != 0 &&
        data[i][field] != "" &&
        data[i][field] != null
      ) {
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
        data: "items",
      },
      pageSize: 20,
    },
    dataBound: ShowModalAgente,
    sortable: true,
    filterable: {
      checkAll: false,
      height: 350,
      extra: false,
      messages: {
        info: "Filtros:",
        filter: "Filtrar",
        clear: "Limpiar",
        isTrue: "SI",
        isFalse: "NO",
        and: "Y",
        or: "O",
      },
      operators: {
        string: {
          eq: "Igual",
          neq: "Distinto",
          startswith: "Comienza con",
          contains: "Contiene",
          endswith: "Finaliza con",
        },
        date: {
          eq: "Igual",
          gte: "Despu&eacute;s o igual a",
          lte: "Antes o igual a",
        },
        number: {
          eq: "Igual a",
          gte: "Mayor que o igual a",
          lte: "Menor que o igual a",
        },
      },
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
        refresh: "Recargar",
      },
      input: true,
      numeric: true,
    },
    columns: [
      {
        field: "Agente",
        title: "Nro",
        width: 50,
      },
      {
        field: "Operador",
        title: "Operador",
        width: 100,
      },
      {
        field: "Material",
        title: "Material",
        width: 100,
      },
      {
        field: "Posicion",
        title: "Posicion",
        width: 100,
      },
      {
        field: "Cantidad",
        title: "Cantidad",
        width: 100,
      },
      {
        field: "Precio",
        title: "Precio",
        width: 100,
      },
      {
        field: "Fecha",
        title: "Fecha",
        width: 100,
      },
      {
        field: "Comercial",
        title: "Comercial",
        width: 100,
      },
    ],
  });
}

function ShowModal(e) {
  //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
  $("#ModalDetallePosicion").on("shown.bs.modal", function () {
    $(document).off("focusin.modal");
    var grid = $("#grilla").data("kendoGrid");
    if (grid) {
      grid.refresh();
    }
  });
  //$("#ModalDetallePosicion").on('hidden.bs.modal', function () {
  //    $('#grilla').kendoGrid('destroy').empty();
  //});
  $("#ModalDetallePosicion").on("hidden.bs.modal", function () {
    if ($("#grilla").data("kendoGrid")) {
      $("#grilla").data("kendoGrid").destroy();
    }
    $("#grilla").empty();
  });

  $("#ModalDetallePosicion").modal("show");
  OcultarColumnasVacias($("#grilla").data("kendoGrid"));
}

function ShowModalFijacion(e) {
  //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
  $("#ModalDetalleFijacion").on("shown.bs.modal", function () {
    $(document).off("focusin.modal");
  });
  $("#ModalDetalleFijacion").on("hidden.bs.modal", function () {
    if ($("#grilla").data("kendoGrid")) {
      $("#grilla").data("kendoGrid").destroy();
    }
    $("#grilla").empty();
  });

  $("#ModalDetalleFijacion").modal("show");
  OcultarColumnasVacias($("#grillaDetalle").data("kendoGrid"));
}

function ShowModalSojaSustentable(e) {
  //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
  $("#ModalDetalleSojaSustentable").on("shown.bs.modal", function () {
    $(document).off("focusin.modal");
  });
  $("#ModalDetalleSojaSustentable").on("hidden.bs.modal", function () {
    if ($("#grilla").data("kendoGrid")) {
      $("#grilla").data("kendoGrid").destroy();
    }
    $("#grilla").empty();
  });

  $("#ModalDetalleSojaSustentable").modal("show");
  //OcultarColumnasVacias($("#grilla").data("kendoGrid"));
}

function ShowModalAgente() {
  //A saber: Esto sirve para que se pueda escribir en los input de los filtros cuando la grilla de Kendo esta dentro de un modal (error de Kendo).
  $("#ModalAgenteCompra").on("shown.bs.modal", function () {
    $(document).off("focusin.modal");
  });
  $("#ModalAgenteCompra").modal("show");
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
          data: "data",
        },
      },
      optionLabel: {
        Descripcion: "TODOS",
        Id: "0",
      },
      dataTextField: "Descripcion",
      dataValueField: "Id",
      change: function (e) {
        var fechaString = $("#fecha").val();
        var fechaHastaString = $("#fechaHasta").val();
        $("#descargaReporte").attr(
          "href",
          url +
            "?fechaString=" +
            fechaString +
            "&fechaHastaString=" +
            fechaHastaString +
            "&centroId=" +
            ObtenerValorCentroId() +
            "&materialId=" +
            ObtenerValorMaterialId().toString() +
            "&verFijaciones=" +
            getVerFijaciones(),
        );
      },
      dataBound: setearValoresComboDeInicio,
    });

    $("#centroId")
      .closest(".k-dropdown.k-widget")
      .keydown(function (e) {
        if (e.keyCode == 46) {
          var dropdownlist = $("#centroId").data("kendoDropDownList");
          dropdownlist.text("");
        }
      });
  });
}

function CargarComboMaterial() {
  var href = window.location.href;
  href = href + "/ObtenerMateriales";
  $.get(href, function (data) {
    $("#materialId").kendoMultiSelect({
      dataSource: {
        serverFiltering: false,
        data: JSON.parse(data),
        type: JSON,
        schema: {
          data: "data",
        },
      },
      autoClose: false,
      delay: 300,
      dataTextField: "Descripcion",
      dataValueField: "MaterialId",
      change: function (e) {
        var fechaString = $("#fecha").val();
        var fechaHastaString = $("#fechaHasta").val();
        $("#descargaReporte").attr(
          "href",
          url +
            "?fechaString=" +
            fechaString +
            "&fechaHastaString=" +
            fechaHastaString +
            "&centroId=" +
            ObtenerValorCentroId() +
            "&materialId=" +
            ObtenerValorMaterialId().toString() +
            "&verFijaciones=" +
            getVerFijaciones(),
        );
      },
      dataBound: setearValoresComboDeInicio,
    });

    $("#materialId")
      .closest(".k-dropdown.k-widget")
      .keydown(function (e) {
        if (e.keyCode == 46) {
          var dropdownlist = $("#materialId").data("kendoDropDownList");
          dropdownlist.text("");
        }
      });
  });
}

function setearValoresComboDeInicio() {
  $("#descargaReporte").attr(
    "href",
    url +
      "?fechaString=" +
      fechaString +
      "&fechaHastaString=" +
      fechaString +
      "&centroId=" +
      ObtenerValorCentroId() +
      "&materialId=" +
      ObtenerValorMaterialId().toString() +
      "&verFijaciones=" +
      getVerFijaciones(),
  );
}

function getVerFijaciones() {
  return $("#verFijaciones").is(":checked") ? "True" : "False";
}
