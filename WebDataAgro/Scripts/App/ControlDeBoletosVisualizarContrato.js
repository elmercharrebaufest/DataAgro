var ModalVisualizar = (function () {
  "use strict";

  var modalId = "#modalVisualizar";

  var state = {
    data: null,
  };

  // =============================
  // Helpers
  // =============================

  function setText(id, value) {
    $("#" + id).text(value || "");
  }

  function setHtml(id, value) {
    $("#" + id).html(value || "");
  }

  function show(id, visible) {
    $("#" + id).toggle(!!visible);
  }

  function formatNumber(n) {
    if (!n || n == 0 || isNaN(parseFloat(n))) return "-";
    return kendo.toString(parseFloat(n), "n2");
  }

  function formatDate(dt) {
    if (!dt) return "-";
    return kendo.toString(dt, "dd/MM/yyyy");
  }

  // =============================
  // LIMPIEZA
  // =============================

  function limpiar() {
    state.data = null;
    $(".status").hide();
    // Limpiar todos los campos de texto
    $(modalId + ' [id^="visualizar_"]').text("");
    $("#lineModalLabel").text("");
    $("#fechacontrato").text("");
    $("#visualizar_observacion").html("");
  }

  // =============================
  // ESTADO VISUAL
  // =============================

  function pintarEstado(estado) {
    $(".status").hide();

    switch (estado) {
      case 1:
        $(".pendiente-modal").show();
        break;
      case 2:
        $(".confirmado-modal").show();
        break;
      case 3:
        $(".oferta-modal").show();
        break;
      case 4:
        $(".conerror-modal").show();
        break;
      case 5:
        $(".finalizado-modal").show();
        break;
      case 6:
        $(".borrado-modal").show();
        break;
      case 7:
        $(".reconfirmar-modal").show();
        break;
    }
  }

  // =============================
  // CARGA PRINCIPAL
  // =============================

  function cargarDatos(d) {
    state.data = d;

    console.log("state.data---->>>", state.data);

    // HEADER
    setText("lineModalLabel", d.ContratoSAP);
    setText("visualizar_proveedor", d.Proveedor);
    setText("visualizar_material", d.Material);
    setText("visualizar_cantidad", formatNumber(d.Cantidad));
    setText("visualizar_precio", formatNumber(d.PrecioNeto));

    // GENERAL
    setText("fechacontrato", formatDate(d.Fecha));
    setText("visualizar_campana", d.Campania);
    setText("visualizar_clasificacion", d.ClasificacionDescripcion);
    setText("visualizar_destino", d.DestinoDescripcion);

    // APERTURA
    setText(
      "visualizar_aperturaFinancieroPrecioNeto",
      formatNumber(d.PrecioNeto),
    );
    setText("visualizar_aperturaFinanciero", d.Financiero);
    setText("visualizar_aperturaRedespacho", d.Redespacho);
    setText("visualizar_aperturaComisiones", d.Comisiones);

    // LOGÍSTICA
    setText("visualizar_procedencia", d.Procedencia);
    setText("visualizar_tarifaFlete", d.TarifaFlete);
    setText("visualizar_nivelFlete", d.NivelFlete);

    // OBS
    setHtml("visualizar_observacion", d.Observacion);

    pintarEstado(d.EstadoId);

    // Normalize date fields in arrays so Kendo templates receive Date objects
    try {
      if (Array.isArray(d.PreciosPactados)) {
        d.PreciosPactados = d.PreciosPactados.map(function (p) {
          return Object.assign({}, p, {
            Desde: parseDate(p.Desde) || p.Desde,
            Hasta: parseDate(p.Hasta) || p.Hasta,
          });
        });
      }
    } catch (e) {
      console.warn("Error normalizando fechas de precios pactados:", e);
    }

    bindKendo(d);
  }

  // =============================
  // KENDO BINDING
  // =============================

  function bindKendo(data) {
    var viewModel = kendo.observable({
      DescuentosVisualizar: data.Descuentos || [],
      CalidadesVisualizar: data.Calidades || [],
      Servicios: data.Servicios || [],
      PreciosVisualizar: data.PreciosPactados || [],
    });

    kendo.bind($(modalId), viewModel);
  }

  // =============================
  // INICIALIZACIÓN
  // =============================

  $(document).ready(function () {
    // Event handler para limpiar al cerrar
    $(modalId).on("hidden.bs.modal", function () {
      limpiar();
    });
  });

  // =============================
  // API PÚBLICA
  // =============================

  return {
    abrir: function (data) {
      /*
                  if (!data) {
                console.error("ModalVisualizar: No se recibieron datos");
                alert("Error: No hay datos para mostrar");
                return;
                  }
            */
      console.log("data--->>", data);
      cargarDatos(data);
      $(modalId).modal("show");
    },

    cerrar: function () {
      $(modalId).modal("hide");
    },

    recargarEstado: function (nuevoEstado) {
      if (state.data) {
        state.data.EstadoId = nuevoEstado;
        pintarEstado(nuevoEstado);
      }
    },

    limpiar: function () {
      limpiar();
    },
  };
})();
