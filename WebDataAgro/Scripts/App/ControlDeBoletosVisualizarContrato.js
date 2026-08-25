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
        var campos = [
            { id: "visualizar_contratoSap", valor: d.ContratoSAP },
            { id: "visualizar_proveedor", valor: d.Proveedor },
            { id: "visualizar_fecha_operacion", valor: d.FechaOperacion },
            { id: "visualizar_fecha_desde_hasta_entrega", valor: d.PeriodoEntrega },
            { id: "visualizar_fecha_desde_hasta_original", valor: formatearFechaHora(d.FechaDesde) },
            { id: "visualizar_clasificacion", valor: d.Clasificacion },
            { id: "visualizar_material", valor: d.Material },
            { id: "visualizar_campana", valor: d.Campana },
            { id: "visualizar_cantidad", valor: formatNumber(d.Cantidad) },
            { id: "visualizar_fijacion_cantidad_minimo", valor: formatNumber(d.CantidadFijacionMinima) },
            { id: "visualizar_fijacion_cantidad_maximo", valor: formatNumber(d.CantidadFijacionMaxima) },
            { id: "visualizar_precio", valor: formatNumber(d.Precio) + " " + d.Moneda },
            { id: "visualizar_comercial", valor: d.Comercial },
            { id: "visualizar_porcentaje_pago", valor: d.PorcentajeDePago },
            { id: "visualizar_precio_neto", valor: formatNumber(d.PrecioNeto) + " " + d.Moneda },
            { id: "visualizar_redespacho", valor: formatNumber(d.ImporteRedespacho) + " " + d.MonedaRedespacho },
            { id: "visualizar_tipo", valor: d.TipoNegocio },
            { id: "visualizar_destino", valor: d.Destino },
            { id: "visualizar_procedencia", valor: d.Localidad },
            { id: "visualizar_dolarizo_origen", valor: d.FechaDolarizadoOriginal },
            { id: "visualizar_mercaderia_deposito", valor: d.MercaderaDeposito },
            { id: "visualizar_cantidad_deposito", valor: d.CantidadDeposito },
            { id: "visualizar_boleto", valor: d.TipoBoleto },
            { id: "visualizar_bolsa", valor: d.Bolsa },
            { id: "visualizar_calidad", valor: d.StandarCalidad },
            { id: "visualizar_observacion", valor: d.Observacion }
        ];

        campos.forEach(function (campo) {
            setText(campo.id, campo.valor);
        });
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
