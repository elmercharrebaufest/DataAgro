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


    function validarFechas() {

        var feRecepBoleto = parseDate("#dtFeRecepBoleto");
        var feEnvioFirmas = parseDate("#dtFeEnvio");
        var feEnvioBolsa = parseDate("#dtFeEnvioBolsa");
        var feVueltaBolsa = parseDate("#dtFeVueltaBolsa");
        var feEnvioAfip = parseDate("#dtFeEnvioAfip");
        var feVueltaAfip = parseDate("#dtFeVueltaAfip");
        var feEnviadoFirma = parseDate("#dtFeEnviadoFirma");
        var feRecibFirma = parseDate("#dtFeRecibFirma");

        // 🔴 Base obligatoria para validar relaciones
        if (!feRecepBoleto) {
            return "La Fecha de recepción de boleto es obligatoria.";
        }

        // 📌 Envío Firmas >= Recepción boleto
        if (feEnvioFirmas && feEnvioFirmas < feRecepBoleto)
            return "La Fecha de envío Firmas no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Envío Obleado Bolsa >= Recepción boleto
        if (feEnvioBolsa && feEnvioBolsa < feRecepBoleto)
            return "La Fecha de envío Obleado Bolsa no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Recepción Obleado Bolsa
        if (feVueltaBolsa) {
            if (feVueltaBolsa < feRecepBoleto)
                return "La Fecha de recepción Obleado Bolsa no puede ser anterior a la Fecha de recepción de boleto.";

            if (feEnvioBolsa && feVueltaBolsa < feEnvioBolsa)
                return "La Fecha de recepción Obleado Bolsa debe ser igual o mayor a la Fecha de envío Obleado Bolsa.";
        }

        // 📌 Envío Certificación AFIP >= Recepción boleto
        if (feEnvioAfip && feEnvioAfip < feRecepBoleto)
            return "La Fecha de envío Certificación AFIP no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Recepción Certificación AFIP
        if (feVueltaAfip) {
            if (feVueltaAfip < feRecepBoleto)
                return "La Fecha de recepción Certificación AFIP no puede ser anterior a la Fecha de recepción de boleto.";

            if (feEnvioAfip && feVueltaAfip < feEnvioAfip)
                return "La Fecha de recepción Certificación AFIP debe ser igual o mayor a la Fecha de envío Certificación AFIP.";
        }

        // 📌 Envío Sellado (dtFeEnviadoFirma)
        if (feEnviadoFirma) {
            if (feEnviadoFirma < feRecepBoleto)
                return "La Fecha de envío Sellado no puede ser anterior a la Fecha de recepción de boleto.";

            if (feVueltaBolsa && feEnviadoFirma < feVueltaBolsa)
                return "La Fecha de envío Sellado debe ser igual o mayor a la Fecha de recepción Obleado Bolsa.";
        }

        // 📌 Recepción Firmas
        if (feRecibFirma) {
            if (feRecibFirma < feRecepBoleto)
                return "La Fecha de recepción Firmas no puede ser anterior a la Fecha de recepción de boleto.";

            if (feEnvioFirmas && feRecibFirma < feEnvioFirmas)
                return "La Fecha de recepción Firmas debe ser igual o mayor a la Fecha de envío Firmas.";
        }

        return null; // ✅ Todo correcto
    }

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

                var errorValidacion = validarFechas();

                if (errorValidacion) {
                    alert(errorValidacion);
                    state.cargando = false;
                    return;
                }
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
