var ControlDeBoletosGestion = (function () {
    "use strict";

    var config = {
        modalId: "#modalGestionControlBoleto"
    };

    var state = {
        ControlDeBoletosId: null,
        SeguimientoBoletoId: null,
        PreCertificacionId: null,
        NegocioId: null
    };

    function cargarDatos() {
        $("#txtContratoGestion").text(state.ControlDeBoletosId || "");

        BlockUi("Cargando...");
        try {
            ControlDeBoletosSeguimiento.inicializar(
                state.SeguimientoBoletoId,
                state.ControlDeBoletosId
            );
            ControlDeBoletosDatosCertificacion.inicializar(
                state.PreCertificacionId,
                state.ControlDeBoletosId
            );
            if (state.NegocioId > 0) {
                ControlDeBoletosModificarContrato.inicializar(state.NegocioId);
            }
        } finally {
            setTimeout(function () { $.unblockUI(); }, 300);
        }
    }

    return {
        abrir: function (params) {
            state.ControlDeBoletosId  = params.ControlDeBoletosId  || 0;
            state.SeguimientoBoletoId = params.SeguimientoBoletoId || 0;
            state.PreCertificacionId  = params.PreCertificacionId  || 0;
            state.NegocioId           = params.NegocioId           || 0;

            var parametros = "?controlDeBoletosId=" + state.ControlDeBoletosId + "&seguimientoBoletoId=" + state.SeguimientoBoletoId + "&preCertificacionId=" + state.PreCertificacionId + "&negocioId=" + state.NegocioId;
            var url = "/ControlDeBoletos/_GestionControlBoleto" + parametros;

            // Remover modal anterior si existe
            $(config.modalId).remove();

            $.get(url, function (html) {
                $('body').append(html);

                try {
                    cargarDatos();
                    $(config.modalId).modal("show");
                } catch (error) {
                    console.error("Error cargando tracking:", error);
                }
            });


        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };
})();
