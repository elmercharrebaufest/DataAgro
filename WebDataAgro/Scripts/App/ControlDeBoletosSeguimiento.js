function getDate(id) {
    var v = $(id).val();
    return v ? v : null;
}

var ControlDeBoletosSeguimiento = (function () {
    "use strict";

    var config = {
        urls: {
            registrar: "/SeguimientoControlBoleto/Registrar"
        },
        modalId: "#modalSeguimientoControlBoleto"
    };

    var state = { cargando: false };

    function obtenerRequest() {
        return {
            Contrato: $("#txtContrato").val(),
            Bolsa: $("#txtBolsa").val(),
            BolsaSellado: $("#txtBolsaSellado").val(),
            TipoBoleto: $("#txtTipoBoleto").val(),
            Fijacion: $("#txtFijacion").val(),
            Hora: $("#txtHora").val(),
            RechazadoAfip: $("#txtRechazadoAfip").val(),

            Fecha: getDate("#dtFecha"),
            FeEnvio: getDate("#dtFeEnvio"),
            FeEnvioAfip: getDate("#dtFeEnvioAfip"),
            FeEnvioBolsa: getDate("#dtFeEnvioBolsa"),
            FeRecepBoleto: getDate("#dtFeRecepBoleto"),
            FeRecibFirma: getDate("#dtFeRecibFirma"),
            FeVueltaAfip: getDate("#dtFeVueltaAfip"),
            FeVueltaBolsa: getDate("#dtFeVueltaBolsa"),
            FeEnviadoFirma: getDate("#dtFeEnviadoFirma"),
            FecAcopio: getDate("#dtFecAcopio"),

            ObsCtrlBoleto: $("#txtObsCtrlBoleto").val(),
            ObsCtrlBoleto2: $("#txtObsCtrlBoleto2").val()
        };
    }

    return {

        abrir: function (contrato) {
            $("#txtContrato").val(contrato || "");
            $(config.modalId).modal("show");
        },

        guardar: function () {
            if (state.cargando) return;
            state.cargando = true;

            try {
                var request = obtenerRequest();

                var response = MSExecuteOnServer(
                    config.urls.registrar,
                    request
                );

                if (response && response.success) {
                    alert("Éxito: " + response.message);
                    this.cerrar();
                } else {
                    alert("Error: " + response.message);
                }
            }
            finally {
                state.cargando = false;
            }
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };
})();
