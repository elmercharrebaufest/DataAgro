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
    let controlBoleto, controlBolsa, controlBolsaSellado;
    let controlRechazadoAfip, controlFechaEnvio, controlFechaEnvioAfip;
    let controlFechaEnvioBolsa, controlFechaRecepBoleto, controlFechaRecibFirma;
    let controlFechaVueltaAfip, controlFechaVueltaBolsa, controlFechaEnviadoFirma;
    let controlObsCtrlBoleto, controlObsCtrlBoleto2;

    function bindControls() {
        var $form = $("#accordionGestionBoleto #frmSeguimientoControlBoleto");
        if (!$form.length) $form = $("#frmSeguimientoControlBoleto").first();

        controlBoleto        = $form.find("#Boleto");
        controlBolsa         = $form.find("#Bolsa");
        controlBolsaSellado  = $form.find("#BolsaSellado");
        controlRechazadoAfip = $form.find("#txtRechazadoAfip");
        controlFechaEnvio       = $form.find("#FechaEnvio");
        controlFechaEnvioAfip   = $form.find("#FechaEnvioAfip");
        controlFechaEnvioBolsa  = $form.find("#FechaEnvioBolsa");
        controlFechaRecepBoleto = $form.find("#FechaRecepBoleto");
        controlFechaRecibFirma  = $form.find("#FechaRecibFirma");
        controlFechaVueltaAfip  = $form.find("#FechaVueltaAfip");
        controlFechaVueltaBolsa = $form.find("#FechaVueltaBolsa");
        controlFechaEnviadoFirma = $form.find("#FechaEnviadoFirma");
        controlObsCtrlBoleto  = $form.find("#ObsCtrlBoleto");
        controlObsCtrlBoleto2 = $form.find("#ObsCtrlBoleto2");

        controlBolsa.off("change.seg").on("change.seg", function () {
            if (Array.isArray(listaBolsaSAP) && listaBolsaSAP.length) {
                var idSeleccionado = $(this).val();
                var bolsaSAP = null;
                for (var i = 0; i < listaBolsaSAP.length; i++) {
                    if (listaBolsaSAP[i].Text === idSeleccionado) { bolsaSAP = listaBolsaSAP[i]; break; }
                }
                controlBolsaSellado.val(bolsaSAP ? bolsaSAP.Value : "");
            } else {
                controlBolsaSellado.val("");
            }
        });
    }
    function getKendoDate($el) {
        var dp = $el.data("kendoDatePicker");
        return dp ? dp.value() : null;
    }

    function getKendoDateISO($el) {
        var d = getKendoDate($el);
        return d ? d.toISOString() : null;
    }

    function setKendoDate($el, value) {
        var dp = $el.data("kendoDatePicker");
        if (!dp) return;
        if (!value) { dp.value(null); return; }
        var match = /\/Date\((\d+)\)\//.exec(value);
        var date = match ? new Date(parseInt(match[1], 10)) : new Date(value);
        dp.value(isNaN(date.getTime()) ? null : date);
    }

    function inicializarFechas() {
        [
            controlFechaRecepBoleto, controlFechaEnviadoFirma, controlFechaEnvioBolsa,
            controlFechaEnvioAfip, controlFechaRecibFirma, controlFechaVueltaBolsa,
            controlFechaVueltaAfip, controlFechaEnvio
        ].forEach(function ($el) {
            if ($el.data("kendoDatePicker")) $el.data("kendoDatePicker").destroy();
            $el.kendoDatePicker({ weekNumber: true, format: "dd/MM/yyyy", value: null });
        });
    }

    function validarFechas() {

        var feRecepBoleto    = getKendoDate(controlFechaRecepBoleto);
        var feEnvioFirmas    = getKendoDate(controlFechaEnvio);
        var feEnvioBolsa     = getKendoDate(controlFechaEnvioBolsa);
        var feVueltaBolsa    = getKendoDate(controlFechaVueltaBolsa);
        var feEnvioAfip      = getKendoDate(controlFechaEnvioAfip);
        var feVueltaAfip     = getKendoDate(controlFechaVueltaAfip);
        var feEnviadoFirma   = getKendoDate(controlFechaEnviadoFirma);
        var feRecibFirma     = getKendoDate(controlFechaRecibFirma);

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
            FechaEnvio: getKendoDateISO(controlFechaEnvio),
            FechaEnvioAfip: getKendoDateISO(controlFechaEnvioAfip),
            FechaEnvioBolsa: getKendoDateISO(controlFechaEnvioBolsa),
            FechaRecepBoleto: getKendoDateISO(controlFechaRecepBoleto),
            FechaRecibFirma: getKendoDateISO(controlFechaRecibFirma),
            FechaVueltaAfip: getKendoDateISO(controlFechaVueltaAfip),
            FechaVueltaBolsa: getKendoDateISO(controlFechaVueltaBolsa),
            FechaEnviadoFirma: getKendoDateISO(controlFechaEnviadoFirma),
            ObsCtrlBoleto: controlObsCtrlBoleto.val(),
            ObsCtrlBoleto2: controlObsCtrlBoleto2.val()
        };
    }
    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");
        return MSExecuteGetOnServerAsync(url)
            .then(function (data) {
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
            })
            .catch(function (error) {
                console.error("Error cargando dropdown " + selector + ":", error);
                $select.html('<option value="">Error al cargar datos</option>');
            });
    }
    function obtener(datosSeguimientoId) {
        var url = config.urls.getSeguimiento + "?datosSeguimientoId=" + datosSeguimientoId;
        return MSExecuteGetOnServerAsync(url)
            .then(function (response) {
                if (!response) return;
                controlBolsa.val(response.BolsaCompraNetId).trigger('change');
                controlBolsaSellado.val(response.BolsaSellado);
                controlBoleto.val(response.BoletoCompraNetId).trigger('change');
                controlRechazadoAfip.val(response.RechazadoAfip);

                setKendoDate(controlFechaEnvio,        response.FechaEnvio);
                setKendoDate(controlFechaEnvioAfip,     response.FechaEnvioAfip);
                setKendoDate(controlFechaEnvioBolsa,    response.FechaEnvioBolsa);
                setKendoDate(controlFechaRecepBoleto,   response.FechaRecepBoleto);
                setKendoDate(controlFechaRecibFirma,    response.FechaRecibFirma);
                setKendoDate(controlFechaVueltaAfip,    response.FechaVueltaAfip);
                setKendoDate(controlFechaVueltaBolsa,   response.FechaVueltaBolsa);
                setKendoDate(controlFechaEnviadoFirma,  response.FechaEnviadoFirma);

                controlObsCtrlBoleto.val(response.ObsCtrlBoleto);
                controlObsCtrlBoleto2.val(response.ObsCtrlBoleto2);
            })
            .catch(function (e) {
                console.error("Error cargando seguimiento:", e);
            });
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
        setKendoDate(controlFechaEnvio, null);
        setKendoDate(controlFechaEnvioAfip, null);
        setKendoDate(controlFechaEnvioBolsa, null);
        setKendoDate(controlFechaRecepBoleto, null);
        setKendoDate(controlFechaRecibFirma, null);
        setKendoDate(controlFechaVueltaAfip, null);
        setKendoDate(controlFechaVueltaBolsa, null);
        setKendoDate(controlFechaEnviadoFirma, null);
    }

    return {

        inicializar: function (SeguimientoBoletoId, ControlDeBoletosId) {
            bindControls();
            inicializarFechas();
            state.SeguimientoBoletoId = SeguimientoBoletoId || 0;
            state.ControlDeBoletosId = ControlDeBoletosId;
            limpiarSeguimientoControlBoleto();

            var pBoleto  = cargarDropdown(config.urls.getBoletoCompraNet, controlBoleto, "Cargando...", "Todos los boletos");
            var pBolsa   = cargarDropdown(config.urls.getBolsa,           controlBolsa,  "Cargando...", "Todas las bolsas");
            var pSAP     = MSExecuteGetOnServerAsync(config.urls.getBolsaSAP).then(function (data) { listaBolsaSAP = data; });

            Promise.all([pBoleto, pBolsa, pSAP]).then(function () {
                if (state.SeguimientoBoletoId != null && state.SeguimientoBoletoId > 0) {
                    obtener(state.SeguimientoBoletoId);
                }
            });
        },

        abrir: function (SeguimientoBoletoId, ControlDeBoletosId) {
            bindControls();
            inicializarFechas();
            state.SeguimientoBoletoId = SeguimientoBoletoId || 0;
            state.ControlDeBoletosId = ControlDeBoletosId;
            limpiarSeguimientoControlBoleto();
            BlockUi('Cargando...');

            var pBoleto = cargarDropdown(config.urls.getBoletoCompraNet, controlBoleto, "Cargando...", "Todos los boletos");
            var pBolsa  = cargarDropdown(config.urls.getBolsa,           controlBolsa,  "Cargando...", "Todas las bolsas");
            var pSAP    = MSExecuteGetOnServerAsync(config.urls.getBolsaSAP).then(function (data) { listaBolsaSAP = data; });

            Promise.all([pBoleto, pBolsa, pSAP]).then(function () {
                if (state.SeguimientoBoletoId != null && state.SeguimientoBoletoId > 0) {
                    return obtener(state.SeguimientoBoletoId);
                }
            }).then(function () {
                $("#txtContrato").val(ControlDeBoletosId || "");
                $(config.modalId).modal("show");
                $.unblockUI();
            });
        },

        guardar: function () {
            if (state.cargando) return;

            var errorValidacion = validarFechas();
            if (errorValidacion) {
                MensAlerta(errorValidacion);
                return;
            }

            var self = this;
            state.cargando = true;
            BlockUi('Guardando...');
            var request = obtenerRequest();
            MSExecuteOnServerAsync(config.urls.createSeguimiento, request)
                .then(function (response) {
                    $.unblockUI();
                    if (!response) return;
                    if (response.success) {
                        MensInfo(response.message);
                        self.cerrar();
                    } else {
                        MensErr(response.message);
                    }
                })
                .catch(function (e) {
                    $.unblockUI();
                    console.error("Error al guardar seguimiento:", e);
                })
                .then(function () {
                    state.cargando = false;
                });
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };
})();
