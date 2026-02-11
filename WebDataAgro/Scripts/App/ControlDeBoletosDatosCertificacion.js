var ControlDeBoletosDaCertificacion = (function () {
    "use strict";

    var config = {
        urls: {
            registrar: "/Certificacion/Registrar"
        },
        modalId: "#modalCertificacion"
    };

    var state = {
        cargando: false
    };

    // ======================
    // Funciones privadas
    // ======================

    function mostrarMensaje(titulo, mensaje, tipo) {
        tipo = tipo || "info";
        alert(titulo + ": " + mensaje);
    }

    function obtenerRequest() {
        return {
            Contrato: $("#ImContrato").val(),
            Usuario: "",
            Fijacion: "",
            Detalle: [
                {
                    Bolsa: $("#Bolsa").val(),
                    FeCertificacion: $("#FeCertificacion").val(),
                    FeVencCerti: $("#FeVencCerti").val(),
                    Oblea: $("#Oblea").val(),
                    Tipo: $("#Tipo").val(),
                    Rechazado: $("#Rechazado").is(":checked")
                }
            ]
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!$("#ImContrato").val())
            errores.push("Debe ingresar el contrato");

        if (!$("#DetBolsa").val())
            errores.push("Debe ingresar la bolsa");

        if (!$("#DetFeCertificacion").val())
            errores.push("Debe ingresar la fecha de certificación");

        return errores;
    }

    // ======================
    // API pública
    // ======================
    return {

        abrir: function (contrato) {
            $("#ImContrato").val(contrato || "");
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
                console.log("Request:", request);

                var response = MSExecuteOnServer(
                    config.urls.registrar,
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
