var ControlDeBoletosDatosCertificacion = (function () {
    "use strict";

    var config = {
        urls: {
            createPreCertificacion: "/ControlDeBoletos/RegistrarDatosPreCertificacion",
            getPreCertificacion: "/ControlDeBoletos/GetDatosPreCertificacion",
            getBolsaCompraNet: "/ControlDeBoletos/GetBolsaCompraNet",
            getTipoOblea: "/ControlDeBoletos/GetTipoOblea",
        },
        modalId: "#modalCertificacion"
    };

    var state = {
        cargando: false,
        PreCertificacionId: null,
        ControlDeBoletosId: null
    };

    // ======================
    // Funciones privadas
    // ======================
    function limpiarFormularioCertificacion() {

        // Inputs tipo date
        $("#FechaCertificacion").val('');
        $("#FechaVencimientoCertificacion").val('');

        // Input texto
        $("#Oblea").val('');

        // Selects
        $("#Bolsa").val(null).trigger('change');
        $("#TipoOblea").val(null).trigger('change');
    }
    function mostrarMensaje(titulo, mensaje, tipo) {
        tipo = tipo || "info";
        alert(titulo + ": " + mensaje);
    }
    function formatearFecha(value) {
        if (!value) return '';

        const match = /\/Date\((\d+)\)\//.exec(value);
        const date = match
            ? new Date(parseInt(match[1], 10))
            : new Date(value);

        if (isNaN(date)) return '';

        const day = String(date.getUTCDate()).padStart(2, '0');
        const month = String(date.getUTCMonth() + 1).padStart(2, '0');
        const year = date.getUTCFullYear();

        return `${year}-${month}-${day}`;
    }
    function obtenerRequest() {
        return {
            Id: state.PreCertificacionId ? state.PreCertificacionId : 0,
            ControlDeBoletosId: state.ControlDeBoletosId,
            BolsaCompraNetId: $("#Bolsa").val(),
            FechaCertificacion: $("#FechaCertificacion").val(),
            FechaVencimiento: $("#FechaVencimientoCertificacion").val(),
            Oblea: $("#Oblea").val(),
            TipoObleaId: $("#TipoOblea").val(),
            Rechazado: $("#Rechazado").is(":checked")
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!$("#Bolsa").val())
            errores.push("Debe ingresar la bolsa");

        if (!$("#FechaCertificacion").val())
            errores.push("Debe ingresar la fecha de certificación");

        return errores;
    }

    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = $(selector);
        $select.html('<option value="">' + textoCarga + "</option>");

        try {
            var data = MSExecuteGetOnServer(url);
            $select.empty().append('<option value="">' + textoDefault + "</option>");
            if (data && Array.isArray(data)) {
                $.each(data, function (i, item) {
                    $select.append(
                        '<option value="' + item.Value + '">' + item.Text + "</option>",
                    );
                });
            } else {
                $select.append('<option value="">Sin datos disponibles</option>');
            }
        } catch (error) {
            console.error("Error cargando dropdown " + selector + ":", error);
            $select.html('<option value="">Error al cargar datos</option>');
        }
    }

    // ======================
    // API pública
    // ======================
    return {

        abrir: function (PreCertificacionId, ControlDeBoletosId) {

            state.ControlDeBoletosId = ControlDeBoletosId;
            state.PreCertificacionId = PreCertificacionId;

            cargarDropdown(
                config.urls.getBolsaCompraNet,
                "#Bolsa",
                "Cargando...",
                "Todos las bolsas",
            );

            cargarDropdown(
                config.urls.getTipoOblea,
                "#TipoOblea",
                "Cargando...",
                "Todos los tipos de oblea",
            );

            limpiarFormularioCertificacion();

            if (PreCertificacionId != null && PreCertificacionId > 0) {
                var url = config.urls.getPreCertificacion + "?datosPreCertificacionId=" + PreCertificacionId;
                var response = MSExecuteGetOnServer(url);
                if (response != null) {
                    $("#FechaCertificacion").val(formatearFecha(response.FechaCertificacion));
                    $("#FechaVencimientoCertificacion").val(formatearFecha(response.FechaVencimiento));
                    $("#Oblea").val(response.Oblea);
                    $("#Bolsa").val(response.BolsaCompraNetId).trigger('change');
                    $("#TipoOblea").val(response.TipoObleaId).trigger('change');
                }
            }

            $(config.modalId).modal("show");
        },

        guardar: function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                mostrarMensaje("Validación", errores.join("\n"), "warning");
                return;
            }

            state.cargando = true;

            try {
                var request = obtenerRequest();

                var response = MSExecuteOnServer(
                    config.urls.createPreCertificacion,
                    request
                );

                if (response && response.success) {
                    mostrarMensaje("Éxito", response.message, "success");
                    this.cerrar();
                } else {
                    mostrarMensaje("Error", response.message, "danger");
                }

            } finally {
                state.cargando = false;
            }
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };

})();
