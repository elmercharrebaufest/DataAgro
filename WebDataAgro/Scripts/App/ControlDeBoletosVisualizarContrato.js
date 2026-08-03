var ModalVisualizar = (function () {
    "use strict";

    var modalId = "#modalVisualizar";

    // =============================
    // Helpers
    // =============================

    function setText(id, value) {
        $("#frmVisualizarContrato #" + id).text(value || "");
    }

    function formatNumber(n) {
        if (!n || n == 0 || isNaN(parseFloat(n))) return "-";
        return kendo.toString(parseFloat(n), "n2");
    }

    function formatearFechaHora(fechaApi) {
        if (!fechaApi) return "";

        const match = /Date\((\d+)\)/.exec(fechaApi);
        if (!match) return "";

        const fecha = new Date(Number(match[1]));
        if (isNaN(fecha)) return "";

        const formatter = new Intl.DateTimeFormat("es-PE", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
        });

        return formatter.format(fecha);
    }
    // =============================
    // LIMPIEZA
    // =============================

    function limpiar() {
        $(modalId + ' [id^="visualizar_"]').text("");
    }

    // =============================
    // CARGA PRINCIPAL
    // =============================

    function cargarDatos(d) {
        setText("visualizar_contratoSap", d.ContratoSAP);
        setText("visualizar_proveedor", d.Proveedor);
        setText("visualizar_fecha_operacion", d.FechaOperacion);
        setText(
            "visualizar_fecha_desde_hasta_entrega",
           d.PeriodoEntrega
        );
        setText(
            "visualizar_fecha_desde_hasta_original",
            formatearFechaHora(d.FechaDesde),
        );
        setText("visualizar_clasificacion", d.Clasificacion);
        setText("visualizar_material", d.Material);
        setText("visualizar_campana", d.Campana);
        setText("visualizar_cantidad", formatNumber(d.Cantidad));
        setText("visualizar_fijacion_cantidad_minimo", formatNumber(d.CantidadFijacionMinima));
        setText("visualizar_fijacion_cantidad_maximo", formatNumber(d.CantidadFijacionMaxima));
        setText("visualizar_precio", formatNumber(d.Precio) + ' ' + d.Moneda);
        setText("visualizar_comercial", d.Comercial);
        setText("visualizar_porcentaje_pago", d.PorcentajeDePago);
        setText("visualizar_precio_neto", formatNumber(d.PrecioNeto) + ' ' + d.Moneda);
        setText("visualizar_redespacho", formatNumber(d.ImporteRedespacho) + ' ' + d.MonedaRedespacho);
        setText("visualizar_tipo", d.TipoNegocio);
        setText("visualizar_destino", d.Destino);
        setText("visualizar_procedencia", d.Localidad);
        setText("visualizar_dolarizo_origen", d.FechaDolarizadoOriginal);
        setText("visualizar_mercaderia_deposito", d.MercaderaDeposito);
        setText("visualizar_cantidad_deposito", d.CantidadDeposito);
        setText("visualizar_boleto", d.TipoBoleto);
        setText("visualizar_bolsa", d.Bolsa);
        setText("visualizar_calidad", d.StandarCalidad);
        setText("visualizar_observacion", d.Observacion);
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
            cargarDatos(data);
            $(modalId).modal("show");
        },

        cerrar: function () {
            $(modalId).modal("hide");
        },

        limpiar: function () {
            limpiar();
        },
    };
})();
