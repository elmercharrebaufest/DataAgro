function getDate(id) {
    var v = $(id).val();
    return v ? v : null;
}

var ControlDeBoletosSeguimiento = (function () {
    "use strict";

    var config = {
        urls: {
            createSeguimiento: "/ControlDeBoletos/RegistrarDatosDeSeguimiento",
            getSeguimiento: "/ControlDeBoletos/GetDatosDeSeguimiento",
            getBolsa: "/ControlDeBoletos/GetBolsaCompraNet",
            getBolsaSAP: "/ControlDeBoletos/GetBolsaCompraNetSAP",
            getBoletoCompraNet: "/ControlDeBoletos/GetBoletoCompraNet",
        },
        modalId: "#modalSeguimientoControlBoleto"
    };

    var state = {
        cargando: false,
        ControlDeBoletosId: null,
        SeguimientoBoletoId: null
    };

    let listaBolsaSAP = null;
    let controlBoleto = $("#frmSeguimientoControlBoleto #Boleto");
    let controlBolsa = $("#frmSeguimientoControlBoleto #Bolsa");
    let controlBolsaSellado = $("#frmSeguimientoControlBoleto #BolsaSellado");

    let controlRechazadoAfip = $("#frmSeguimientoControlBoleto #txtRechazadoAfip");
    let controlFechaEnvio = $("#frmSeguimientoControlBoleto #FechaEnvio");
    let controlFechaEnvioAfip = $("#frmSeguimientoControlBoleto #FechaEnvioAfip");
    let controlFechaEnvioBolsa = $("#frmSeguimientoControlBoleto #FechaEnvioBolsa");
    let controlFechaRecepBoleto = $("#frmSeguimientoControlBoleto #FechaRecepBoleto");
    let controlFechaRecibFirma = $("#frmSeguimientoControlBoleto #FechaRecibFirma");
    let controlFechaVueltaAfip = $("#frmSeguimientoControlBoleto #FechaVueltaAfip");
    let controlFechaVueltaBolsa = $("#frmSeguimientoControlBoleto #FechaVueltaBolsa");
    let controlFechaEnviadoFirma = $("#frmSeguimientoControlBoleto #FechaEnviadoFirma");
    let controlObsCtrlBoleto = $("#frmSeguimientoControlBoleto #ObsCtrlBoleto");
    let controlObsCtrlBoleto2 = $("#frmSeguimientoControlBoleto #ObsCtrlBoleto2");

    controlBolsa.on("change", function () {
        if (Array.isArray(listaBolsaSAP) && listaBolsaSAP.length) {
            const idSeleccionado = $(this).val();
            const bolsaSAP = listaBolsaSAP.find(x => x.Text === idSeleccionado);
            controlBolsaSellado.val(bolsaSAP ? bolsaSAP.Value : "");
        } else {
            controlBolsaSellado.val("");
        }
    });
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
    function validarFechas() {

        var feRecepBoleto = controlFechaRecepBoleto;
        var feEnvioFirmas = controlFechaEnvio;
        var feEnvioBolsa = controlFechaEnvioBolsa;
        var feVueltaBolsa = controlFechaVueltaBolsa;
        var feEnvioAfip = controlFechaEnvioAfip;
        var feVueltaAfip = controlFechaVueltaBolsa;
        var feEnviadoFirma = controlFechaEnviadoFirma;
        var feRecibFirma = controlFechaRecibFirma;

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
            Id: state.SeguimientoBoletoId ? state.SeguimientoBoletoId : 0,
            ControlDeBoletosId: state.ControlDeBoletosId,
            BolsaCompraNetId: controlBolsa.val(),
            BolsaSellado: controlBolsaSellado.val(),
            BoletoCompraNetId: controlBoleto.val(),
            RechazadoAfip: controlRechazadoAfip.val(),
            FechaEnvio: controlFechaEnvio.val(),
            FechaEnvioAfip: controlFechaEnvioAfip.val(),
            FechaEnvioBolsa: controlFechaEnvioBolsa.val(),
            FechaRecepBoleto: controlFechaRecepBoleto.val(),
            FechaRecibFirma: controlFechaRecibFirma.val(),
            FechaVueltaAfip: controlFechaVueltaAfip.val(),
            FechaVueltaBolsa: controlFechaVueltaBolsa.val(),
            FechaEnviadoFirma: controlFechaEnviadoFirma.val(),
            ObsCtrlBoleto: controlObsCtrlBoleto.val(),
            ObsCtrlBoleto2: controlObsCtrlBoleto2.val()
        };
    }
    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = selector;
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
    function obtener(datosSeguimientoId) {
        var url = config.urls.getSeguimiento + "?datosSeguimientoId=" + datosSeguimientoId;
        var response = MSExecuteGetOnServer(url);
        if (response != null) {
            controlBolsa.val(response.BolsaCompraNetId).trigger('change');
            controlBolsaSellado.val(response.BolsaSellado);
            controlBoleto.val(response.BoletoCompraNetId).trigger('change');
            controlRechazadoAfip.val(response.RechazadoAfip);

            controlFechaEnvio.val(formatearFecha(response.FechaEnvio));
            controlFechaEnvioAfip.val(formatearFecha(response.FechaEnvioAfip));
            controlFechaEnvioBolsa.val(formatearFecha(response.FechaEnvioBolsa));
            controlFechaRecepBoleto.val(formatearFecha(response.FechaRecepBoleto));
            controlFechaRecibFirma.val(formatearFecha(response.FechaRecibFirma));
            controlFechaVueltaAfip.val(formatearFecha(response.FechaVueltaAfip));
            controlFechaVueltaBolsa.val(formatearFecha(response.FechaVueltaBolsa));
            controlFechaEnviadoFirma.val(formatearFecha(response.FechaEnviadoFirma));

            controlObsCtrlBoleto.val(response.ObsCtrlBoleto);
            controlObsCtrlBoleto2.val(response.ObsCtrlBoleto2);
        }
    }
    function limpiarSeguimientoControlBoleto() {

        // Selects
        controlBoleto.val(null).trigger('change');
        controlBolsa.val(null).trigger('change');

        // Inputs texto
        controlBolsaSellado.val('');
        controlRechazadoAfip.val('');
        controlObsCtrlBoleto.val('');
        controlObsCtrlBoleto2.val('');

        // Inputs fecha
        controlFechaEnvio.val('');
        controlFechaEnvioAfip.val('');
        controlFechaEnvioBolsa.val('');
        controlFechaRecepBoleto.val('');
        controlFechaRecibFirma.val('');
        controlFechaVueltaAfip.val('');
        controlFechaVueltaBolsa.val('');
        controlFechaEnviadoFirma.val('');
    }

    return {

        abrir: function (SeguimientoBoletoId, ControlDeBoletosId) {
            state.SeguimientoBoletoId = SeguimientoBoletoId || 0;
            state.ControlDeBoletosId = ControlDeBoletosId;
            limpiarSeguimientoControlBoleto();
            BlockUi('Cargando...');

            cargarDropdown(
                config.urls.getBoletoCompraNet,
                controlBoleto,
                "Cargando...",
                "Todos los boletos",
            );

            cargarDropdown(
                config.urls.getBolsa,
                controlBolsa,
                "Cargando...",
                "Todas las bolsas",
            );

            listaBolsaSAP = MSExecuteGetOnServer(config.urls.getBolsaSAP);
            if (state.SeguimientoBoletoId != null && state.SeguimientoBoletoId > 0) {
                obtener(state.SeguimientoBoletoId);
            }

            $("#txtContrato").val(ControlDeBoletosId || "");
            $(config.modalId).modal("show");
            setTimeout(function () { $.unblockUI() }, 100);
        },

        guardar: function () {
            if (state.cargando) return;
            state.cargando = true;

            try {

                var errorValidacion = validarFechas();
                if (errorValidacion) {
                    MensAlerta(errorValidacion);
                    state.cargando = false;
                    return;
                }
                BlockUi('Guardando...');
                var request = obtenerRequest();
                var response = MSExecuteOnServer(
                    config.urls.createSeguimiento,
                    request
                );
                if (response != null) {
                    $.unblockUI();

                    if (!response.success) {
                        MensErr(response.message);
                        return;
                    }

                    if (response.success) {
                        MensInfo(response.message);
                        this.cerrar();
                    }

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
