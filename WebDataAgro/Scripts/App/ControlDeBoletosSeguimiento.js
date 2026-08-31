var ControlDeBoletosSeguimiento = (function () {
    "use strict";

    var ui = window.ControlDeBoletosUI;

    var config = {
        urls: {
            createSeguimiento: "/ControlDeBoletos/RegistrarDatosDeSeguimiento",
            getSeguimiento: "/ControlDeBoletos/GetDatosDeSeguimiento",
            getBolsa: "/ControlDeBoletos/GetBolsaCompraNet",
            getBolsaSAP: "/ControlDeBoletos/GetBolsaCompraNetSAP",
            getBoletoSap: "/ControlDeBoletos/GetBoletoSap",
            eliminarSeguimiento: "/ControlDeBoletos/EliminarDatosDeSeguimiento",

        },
        modalId: "#modalSeguimientoControlBoleto"
    };

    var state = {
        cargando: false,
        controlDeBoletosId: null,
        seguimientoBoletoId: 0,
        boletoSap: null,
        operaSinOblea: false,
        esCartaOferta: false,
        esSinBoleto: false,
        boletoCompraNet: null,
        contratoSAP: null

    };

    let listaBolsaSAP = null;
    let controlBoleto, controlBolsa, controlBolsaSellado, controlCaracterBoleto;
    let controlFechaRecepcionBoleto;
    let controlFechaEnvioFirma, controlFechaEnvioBolsa, controlFechaEnvioAfip;
    let controlFechaRecepcionFirma, controlFechaRecepcionBolsa, controlFechaRecepcionAfip;
    let controlFechaEnvioSellado;
    let controlRechazadoAfip;
    let controlObsCtrlBoleto, controlObsCtrlBoleto2;
    let controlFechaEnvioFisicoBolsa;
    let controlFechaRecepcionBoletoOriginal;

    function bindControls() {
        var $form = $("#accordionGestionBoleto #frmSeguimientoControlBoleto");
        if (!$form.length) $form = $("#frmSeguimientoControlBoleto").first();

        controlBoleto = $form.find("#Boleto");
        controlCaracterBoleto = $form.find("#CaracterBoleto");
        controlBolsa         = $form.find("#Bolsa");
        controlBolsaSellado = $form.find("#BolsaSellado");
        controlRechazadoAfip = $form.find("#RechazadoAfip");

        controlFechaRecepcionBoleto = $form.find("#FechaRecepcionBoleto");
        controlFechaEnvioFisicoBolsa = $form.find("#FechaEnvioFisicoBolsa");
        controlFechaRecepcionBoletoOriginal = $form.find("#FechaRecepcionBoletoOriginal");
        controlFechaEnvioFirma = $form.find("#FechaEnvioFirma");
        controlFechaEnvioBolsa = $form.find("#FechaEnvioBolsa");
        controlFechaEnvioAfip = $form.find("#FechaEnvioAfip");

        controlFechaRecepcionFirma = $form.find("#FechaRecepcionFirma");
        controlFechaRecepcionAfip = $form.find("#FechaRecepcionAfip");
        controlFechaRecepcionBolsa = $form.find("#FechaRecepcionBolsa");


        controlFechaEnvioSellado = $form.find("#FechaEnvioSellado");

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
        controlBoleto.off("change.seg").on("change.seg", function () {

            if (Array.isArray(state.boletoSap) && state.boletoSap.length) {
                var idSeleccionado = $(this).val();
                const boleto = state.boletoSap.find(x => x.Id == idSeleccionado);
                controlCaracterBoleto.empty().append('<option value="">Seleccione</option>');
                controlCaracterBoleto.prop('disabled', false);
                if (boleto)
                    cargarDropdownBoletoCaracterSap(boleto,controlCaracterBoleto, "Cargando...", "Seleccione");
            } else {
                controlCaracterBoleto.val("");
            }

        });
    }
    function getKendoDate($el) {
        return ui.getKendoDate($el);
    }

    function getKendoDateISO($el) {
        return ui.getKendoDateISO($el);
    }

    function setKendoDate($el, value) {
        ui.setKendoDate($el, value);
    }

    var fechaMinimaFallback = new Date(1900, 0, 1);

    function obtenerFechaRecepcionBoleto() {
        var fecha = getKendoDate(controlFechaRecepcionBoleto);
        return fecha && !isNaN(fecha.getTime()) ? fecha : null;
    }

    function parseFechaTexto(texto) {
        return ui.parseDateText(texto);
    }

    function validarFechaDatePicker($el, esRecepcionBoleto) {
        var dp = $el.data("kendoDatePicker");
        if (!dp) return true;

        var texto = $.trim($el.val());
        var valor = dp.value();
        var minFecha = esRecepcionBoleto ? null : obtenerFechaRecepcionBoleto();

        // Si el datepicker no parseó el texto, intentar parsearlo manualmente (caso copy/paste)
        if (texto !== "" && !valor) {
            valor = parseFechaTexto(texto);
            if (valor) {
                dp.value(valor);
            }
        }

        if (texto !== "" && !valor) {
            $el.addClass("fecha-invalida");
            $el.val("");
            mostrarNotificacion("Ingrese una fecha válida", "error");
            setTimeout(function () {
                $el.focus();
            }, 100);
            return false;
        }

        if (!esRecepcionBoleto && valor && minFecha && valor < minFecha) {
            $el.addClass("fecha-invalida");
            dp.value(null);
            $el.val("");
            mostrarNotificacion("La fecha no puede ser anterior a la Fecha de recepción de boleto.", "error");
            setTimeout(function () {
                $el.focus();
            }, 100);
            return false;
        }

        if (valor) {
            $el.removeClass("fecha-invalida");
        }

        return true;
    }
    function mostrarNotificacion(mensaje, tipo) {
        ui.showNotification(mensaje, tipo);
    }
    function aplicarMinimoDatePicker($el) {
        var dp = $el.data("kendoDatePicker");
        if (!dp) return;

        var minFecha = obtenerFechaRecepcionBoleto() || fechaMinimaFallback;
        dp.min(minFecha);

        var valorActual = dp.value();
        if (valorActual && valorActual < minFecha) {
            dp.value(null);
            $el.val("");
            $el.addClass("fecha-invalida");
        }
    }

    function actualizarMinimosFechas() {
        [ controlFechaEnvioFirma,
          controlFechaEnvioBolsa,
          controlFechaEnvioAfip,
          controlFechaRecepcionFirma,
          controlFechaRecepcionBolsa,
          controlFechaRecepcionAfip,
          controlFechaEnvioSellado,
          controlFechaEnvioFisicoBolsa,
          controlFechaRecepcionBoletoOriginal
        ].forEach(function ($el) {
            if ($el && $el.length) {
                aplicarMinimoDatePicker($el);
            }
        });
    }

    function inicializarDatePicker($el, esRecepcionBoleto) {
        if ($el.data("kendoDatePicker")) $el.data("kendoDatePicker").destroy();

        $el.off(".seg");

        var options = { weekNumber: true, format: "dd/MM/yyyy", value: null };
        if (!esRecepcionBoleto) {
            options.min = obtenerFechaRecepcionBoleto() || fechaMinimaFallback;
        }

        $el.kendoDatePicker(options);

        $el.on("change.seg blur.seg", function () {
            validarFechaDatePicker($el, !!esRecepcionBoleto);

            if (esRecepcionBoleto) {
                actualizarMinimosFechas();
            } else {
                aplicarMinimoDatePicker($el);
            }
        });
    }

    function inicializarFechas() {
        [ controlFechaRecepcionBoleto,
          controlFechaEnvioFirma,
          controlFechaEnvioBolsa,
          controlFechaEnvioAfip,
          controlFechaRecepcionFirma,
          controlFechaRecepcionBolsa,
          controlFechaRecepcionAfip,
          controlFechaEnvioSellado,
          controlFechaEnvioFisicoBolsa,
          controlFechaRecepcionBoletoOriginal
        ].forEach(function ($el) {
            if ($el && $el.length) {
                inicializarDatePicker($el, $el.is(controlFechaRecepcionBoleto));
            }
        });
    }

    function bloqueaControlesSinOblea() {
        var habilitaOperaSinOblea = state.operaSinOblea ? true : false;
        var habilitaCartaOferta = state.esCartaOferta ? true : false;
        var desHabilitaSinBoleta = state.esSinBoleto ? true : false;

        if (desHabilitaSinBoleta) {
            [ controlFechaRecepcionBoleto,
              controlFechaEnvioFirma,
              controlFechaEnvioBolsa,
              controlFechaEnvioAfip,
              controlFechaRecepcionFirma,
              controlFechaRecepcionBolsa,
              controlFechaRecepcionAfip,
              controlFechaEnvioSellado
            ].forEach(function (control) {
                control.prop("disabled", true);
            });

            controlRechazadoAfip.prop('disabled', true);
            controlObsCtrlBoleto.prop('disabled', true);
            controlObsCtrlBoleto2.prop('disabled', true);
            return;
        } else {

            [controlFechaRecepcionBoleto,
                controlFechaEnvioFirma,
                controlFechaEnvioBolsa,
                controlFechaEnvioAfip,
                controlFechaRecepcionFirma,
                controlFechaRecepcionBolsa,
                controlFechaRecepcionAfip,
                controlFechaEnvioSellado
            ].forEach(function (control) {
                control.prop("disabled", false);
            });
            controlRechazadoAfip.prop('disabled', false);
            controlObsCtrlBoleto.prop('disabled', false);
            controlObsCtrlBoleto2.prop('disabled', false);
        }

        [
            controlFechaEnvioBolsa,
            controlFechaRecepcionBolsa,
            controlFechaEnvioSellado
        ].forEach(function (control) {
            control.prop("disabled", false);
        });

        [
            controlFechaEnvioBolsa,
            controlFechaRecepcionBolsa
        ].forEach(function (control) {
            control.prop("disabled", habilitaOperaSinOblea);
        });

        [
            controlFechaEnvioSellado
        ].forEach(function (control) {
            control.prop("disabled", habilitaCartaOferta);
        });
    }

    function validarFechas() {

        var feRecepcionBoleto = getKendoDate(controlFechaRecepcionBoleto);
        var feEnvioFirmas = getKendoDate(controlFechaEnvioFirma);
        var feEnvioBolsa = getKendoDate(controlFechaEnvioBolsa);
        var feEnvioAfip = getKendoDate(controlFechaEnvioAfip);
        var feRecepcionFirma = getKendoDate(controlFechaRecepcionFirma);
        var feRecepcionBolsa = getKendoDate(controlFechaRecepcionBolsa);
        var feRecepcionAfip  = getKendoDate(controlFechaRecepcionAfip);
        var feEnvioSellado = getKendoDate(controlFechaEnvioSellado);

        if(state.esSinBoleto) {
            return null;
        }

        // 🔴 Valida que no se ingrese una fecha si hay otras fechas que deben depender de una fecha de recepción de boleto
        if ((feEnvioFirmas ||
            feEnvioBolsa ||
            feEnvioAfip ||
            feRecepcionFirma ||
            feRecepcionBolsa ||
            feRecepcionAfip ||
            feEnvioSellado) && !feRecepcionBoleto
        )
            return "No se puede ingresar ninguna fecha de seguimiento sin haber ingresado previamente la fecha de recepción de boletos."

        // 📌 Envío Firmas >= Recepción boleto
        if (feEnvioFirmas && feEnvioFirmas < feRecepcionBoleto)
            return "La Fecha de envío Firmas no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Envío Obleado Bolsa >= Recepción boleto
        if ((feEnvioBolsa && !state.operaSinOblea) && feEnvioBolsa < feRecepcionBoleto)
            return "La Fecha de envío Obleado Bolsa no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Envío Certificación AFIP >= Recepción boleto
        if (feEnvioAfip && feEnvioAfip < feRecepcionBoleto)
            return "La Fecha de envío Certificación Arca no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Recepción Firmas
        if (feRecepcionFirma) {
            if (feRecepcionFirma < feRecepcionBoleto)
                return "La Fecha de recepción Firmas no puede ser anterior a la Fecha de recepción de boleto.";

            if (feEnvioFirmas && feRecepcionFirma < feEnvioFirmas)
                return "La Fecha de recepción Firmas debe ser igual o mayor a la Fecha de envío Firmas.";
        }

        // 📌 Recepción Obleado Bolsa
        if (feRecepcionBolsa && !state.operaSinOblea) {
            if (feRecepcionBolsa < feRecepcionBoleto)
                return "La Fecha de recepción Obleado Bolsa no puede ser anterior a la Fecha de recepción de boleto.";

            if (feEnvioBolsa && feRecepcionBolsa < feEnvioBolsa)
                return "La Fecha de recepción Obleado Bolsa debe ser igual o mayor a la Fecha de envío Obleado Bolsa.";
        }

        // 📌 Recepción Certificación AFIP
        if (feRecepcionAfip) {
            if (feRecepcionAfip < feRecepcionBoleto)
                return "La Fecha de recepción Certificación Arca no puede ser anterior a la Fecha de recepción de boleto.";

            if (feEnvioAfip && feRecepcionAfip < feEnvioAfip)
                return "La Fecha de recepción Certificación Arca debe ser igual o mayor a la Fecha de envío Certificación Arca.";
        }

        // 📌 Envío Sellado (dtFeEnviadoFirma)
        if (feEnvioSellado && !state.esCartaOferta) {
            if (feEnvioSellado < feRecepcionBoleto)
                return "La Fecha de envío Sellado no puede ser anterior a la Fecha de recepción de boleto.";

            if (feRecepcionBolsa && feEnvioSellado < feRecepcionBolsa)
                return "La Fecha de envío Sellado debe ser igual o mayor a la Fecha de recepción Obleado Bolsa.";
        }

        return null; // ✅ Todo correcto
    }
    function obtenerRequest() {
        var rechazado = controlRechazadoAfip.is(":checked");
        return {
            Id: state.seguimientoBoletoId ? state.seguimientoBoletoId : 0,
            ControlDeBoletosId: state.controlDeBoletosId,
            BolsaCompraNetId: controlBolsa.val(),
            BolsaSellado: controlBolsaSellado.val(),
            BoletoSapId: controlBoleto.val(),
            BoletoSapCaracter: controlCaracterBoleto.val(),
            RechazadoAfip: rechazado ? "X" : "",
            FechaRecepcionBoleto: getKendoDateISO(controlFechaRecepcionBoleto),
            FechaEnvioFisicoBolsa: getKendoDateISO(controlFechaEnvioFisicoBolsa),
            FechaEnvioFirma: getKendoDateISO(controlFechaEnvioFirma),
            FechaEnvioAfip: getKendoDateISO(controlFechaEnvioAfip),
            FechaEnvioBolsa: getKendoDateISO(controlFechaEnvioBolsa),

            FechaRecepcionFirma: getKendoDateISO(controlFechaRecepcionFirma),           
            FechaRecepcionAfip: getKendoDateISO(controlFechaRecepcionAfip),
            FechaRecepcionBolsa: getKendoDateISO(controlFechaRecepcionBolsa),

            FechaEnvioSellado: getKendoDateISO(controlFechaEnvioSellado),
            FechaRecepcionBoletoOriginal: getKendoDateISO(controlFechaRecepcionBoletoOriginal),
            ObsCtrlBoleto: controlObsCtrlBoleto.val(),
            ObsCtrlBoleto2: controlObsCtrlBoleto2.val()
        };
    }
    async function cargarDropdown(url, selector, textoCarga, textoDefault) {
        return ui.loadDropdown(url, selector, textoCarga, textoDefault);
    }
    async function cargarDropdownBoletoSap(selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");
        try {
            var data = state.boletoSap;
            $select.empty().append('<option value="">' + textoDefault + "</option>");
            if (data && Array.isArray(data)) {
                $.each(data, function (i, item) {
                    $select.append(
                        '<option value="' + item.Id + '">' + item.Descripcion + "</option>",
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
    async function cargarDropdownBoletoCaracterSap(data, selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");
        $select.prop('disabled', false);
        try {
            const listaCaracter = data.Caracter
                .split('-')
                .map(caracter => ({
                    Id: data.Id,
                    Descripcion: data.Descripcion,
                    Caracter: caracter.trim(),
                    BoletoCompraNetId: data.BoletoCompraNetId
                }));

            $select.empty().append('<option value="">' + textoDefault + "</option>");
            if (listaCaracter && Array.isArray(listaCaracter)) {


                $.each(listaCaracter, function (i, item) {
                    $select.append(
                        '<option value="' + item.Caracter + '">' + item.Caracter + "</option>",
                    );
                });
                if (listaCaracter.length == 1 ) {
                    $select.val(listaCaracter[0].Caracter);
                    $select.prop('disabled', true);
                }
            } else {
                $select.append('<option value="">Sin datos disponibles</option>');
            }
        } catch (error) {
            console.error("Error cargando dropdown " + selector + ":", error);
            $select.html('<option value="">Error al cargar datos</option>');
        }


    }
    async function obtener(controlDeBoletosId) {
        var url = config.urls.getSeguimiento + "?controlDeBoletosId=" + controlDeBoletosId;
        try {
            var response = await MSExecuteGetOnServerAsync(url);
            if (!response || response.Id == 0) return;

            controlBolsa.val(response.BolsaCompraNetId).trigger('change');
            controlBolsaSellado.val(response.BolsaSellado);
            controlBoleto.val(response.BoletoSapId).trigger('change');
            controlCaracterBoleto.val(response.BoletoSapCaracter).trigger('change');
            controlRechazadoAfip.val(response.RechazadoAfip);
            setKendoDate(controlFechaRecepcionBoleto, response.FechaRecepcionBoleto);
            setKendoDate(controlFechaEnvioFirma, response.FechaEnvioFirma);
            setKendoDate(controlFechaEnvioBolsa, response.FechaEnvioBolsa);
            setKendoDate(controlFechaEnvioAfip, response.FechaEnvioAfip);

            setKendoDate(controlFechaRecepcionFirma, response.FechaRecepcionFirma);
            setKendoDate(controlFechaRecepcionBolsa, response.FechaRecepcionBolsa);
            setKendoDate(controlFechaEnvioFisicoBolsa, response.FechaEnvioFisicoBolsa);
            setKendoDate(controlFechaRecepcionAfip, response.FechaRecepcionAfip);
            setKendoDate(controlFechaRecepcionBoletoOriginal, response.FechaRecepcionBoletoOriginal);

            setKendoDate(controlFechaEnvioSellado, response.FechaEnvioSellado);
            state.seguimientoBoletoId = response.Id;

            actualizarMinimosFechas();

            controlObsCtrlBoleto.val(response.ObsCtrlBoleto);
            controlObsCtrlBoleto2.val(response.ObsCtrlBoleto2);
        } catch (e) {
            console.error("Error cargando seguimiento:", e);
        }
    }

    async function cargarBolsaSAP() {
        listaBolsaSAP = await MSExecuteGetOnServerAsync(config.urls.getBolsaSAP);
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
        setKendoDate(controlFechaRecepcionBoleto, null);
        setKendoDate(controlFechaEnvioFirma, null);
        setKendoDate(controlFechaEnvioBolsa, null);
        setKendoDate(controlFechaEnvioAfip, null);
        setKendoDate(controlFechaRecepcionFirma, null);
        setKendoDate(controlFechaRecepcionBolsa, null);
        setKendoDate(controlFechaRecepcionAfip, null);
        setKendoDate(controlFechaEnvioSellado, null);
        setKendoDate(controlFechaEnvioFisicoBolsa, null);
        setKendoDate(controlFechaRecepcionBoletoOriginal, null);

        actualizarMinimosFechas();
    }

    return {

        inicializar: async function (ControlDeBoletosId, OperaSinOblea, EsCartaOferta, EsSinBoleto, BoletoCompraNet, ContratoSAP) {
            BlockUi('Cargando...');
            try {
                bindControls();
                inicializarFechas();
                state.controlDeBoletosId = ControlDeBoletosId;
                state.operaSinOblea = OperaSinOblea;
                state.esCartaOferta = EsCartaOferta;
                state.esSinBoleto = EsSinBoleto;
                state.boletoCompraNet = BoletoCompraNet;
                state.contratoSAP = ContratoSAP;
                limpiarSeguimientoControlBoleto();
                bloqueaControlesSinOblea();
                await Promise.all([
                    cargarDropdown(config.urls.getBolsa, controlBolsa, "Cargando...", "Seleccione una bolsa"),
                    cargarBolsaSAP()
                ]);
                var parametros = "?boletoCompraNetId=" + state.boletoCompraNet;
                var urlBoletoSap = config.urls.getBoletoSap + parametros;
                var dataBoletoSap = await MSExecuteGetOnServerAsync(urlBoletoSap);
                state.boletoSap = dataBoletoSap || [];
                cargarDropdownBoletoSap(controlBoleto, "Cargando...", "Seleccione un boleto");

                if (state.controlDeBoletosId != null && state.controlDeBoletosId > 0) {
                        await obtener(state.controlDeBoletosId);
                }
            } finally {
                $.unblockUI();
            }
        },

        abrir: async function (ControlDeBoletosId) {
            bindControls();
            inicializarFechas();
            state.controlDeBoletosId = ControlDeBoletosId;
            limpiarSeguimientoControlBoleto();

            try {
                BlockUi('Cargando...');
                await Promise.all([
                    cargarDropdown(config.urls.getBoletoCompraNet, controlBoleto, "Cargando...", "Seleccione un boleto"),
                    cargarDropdown(config.urls.getBolsa, controlBolsa, "Cargando...", "Seleccione una bolsa"),
                    cargarBolsaSAP()
                ]);

                if (state.controlDeBoletosId != null && state.controlDeBoletosId > 0) {
                    await obtener(state.controlDeBoletosId);
                }

                $("#txtContrato").val(ControlDeBoletosId || "");
                $(config.modalId).modal("show");
            } finally {
                $.unblockUI();
            }
        },

        guardar: async function () {
            if (state.cargando) return;

            var errorValidacion = validarFechas();
            if (errorValidacion) {
                MensAlerta(errorValidacion);
                return;
            }

            state.cargando = true;
            BlockUi('Guardando...');
            var request = obtenerRequest();
            console.log('request--->>>>', request);
            try {
                var response = await MSExecuteOnServerAsync(config.urls.createSeguimiento, request);
                if (!response) return;

                if (response.success) {
                    MensInfo(response.message);
                    await obtener(state.controlDeBoletosId);
                } else {
                    MensErr(response.message);
                }
            } catch (e) {
                console.error("Error al guardar seguimiento:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
        },
        eliminar: async function () {
            if (state.seguimientoBoletoId == 0) {
                MensAlerta('No se ha registrado datos de seguimiento para el boleto.');
                return;
            }
            if (state.cargando) return;
            state.cargando = true;
            BlockUi('Guardando...');
            var request = new Object();
            request.controlDeBoletosId = state.controlDeBoletosId;
            try {
                Confirma('¿Desea eliminar todo el seguimiento para el contrato ' + state.contratoSAP + "?", async function () {
                    var response = await MSExecuteOnServerAsync(config.urls.eliminarSeguimiento, request);
                    if (!response) return;

                    if (response.success) {
                        MensInfo(response.message);
                        limpiarSeguimientoControlBoleto();
                        await obtener(state.controlDeBoletosId);
                    } else {
                        MensErr(response.message);
                    }
                });
            } catch (e) {
                console.error("Error al eliminar seguimiento:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
        },
        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };
})();
