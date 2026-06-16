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
            getBoletoSap: "/ControlDeBoletos/GetBoletoSap",
        },
        modalId: "#modalSeguimientoControlBoleto"
    };

    var state = {
        cargando: false,
        ControlDeBoletosId: null,
        SeguimientoBoletoId: null,
        BoletoSap: null,
        OperaSinOblea: false,
        EsCartaOferta: false,
        EsSinBoleto: false,
        BoletoCompraNet: null,
    };

    let listaBolsaSAP = null;
    let controlBoleto, controlBolsa, controlBolsaSellado, controlCaracterBoleto;
    let controlFechaRecepcionBoleto;
    let controlFechaEnvioFirma, controlFechaEnvioBolsa, controlFechaEnvioAfip;
    let controlFechaRecepcionFirma, controlFechaRecepcionBolsa, controlFechaRecepcionAfip;
    let controlFechaEnvioSellado;
    let controlRechazadoAfip;
    let controlObsCtrlBoleto, controlObsCtrlBoleto2;

    function bindControls() {
        var $form = $("#accordionGestionBoleto #frmSeguimientoControlBoleto");
        if (!$form.length) $form = $("#frmSeguimientoControlBoleto").first();

        controlBoleto = $form.find("#Boleto");
        controlCaracterBoleto = $form.find("#CaracterBoleto");
        controlBolsa         = $form.find("#Bolsa");
        controlBolsaSellado  = $form.find("#BolsaSellado");
        controlRechazadoAfip = $form.find("#txtRechazadoAfip");

        controlFechaRecepcionBoleto = $form.find("#FechaRecepcionBoleto");

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

            if (Array.isArray(state.BoletoSap) && state.BoletoSap.length) {
                var idSeleccionado = $(this).val();
                var caracterSAP = null;

                const boleto = state.BoletoSap.find(x => x.Id == idSeleccionado);
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

    var fechaMinimaFallback = new Date(1900, 0, 1);

    function obtenerFechaRecepcionBoleto() {
        var fecha = getKendoDate(controlFechaRecepcionBoleto);
        return fecha && !isNaN(fecha.getTime()) ? fecha : null;
    }

    function parseFechaTexto(texto) {
        if (!texto) return null;

        var parsed = kendo.parseDate(texto, "dd/MM/yyyy") || kendo.parseDate(texto);
        return parsed && !isNaN(parsed.getTime()) ? parsed : null;
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
        $("#notification").kendoNotification({
            position: {
                pinned: true,
                top: 50,
                left: "50%"
            },
            autoHideAfter: 3000,
            stacking: "down"
        });
        var notification = $("#notification").data("kendoNotification");
        notification.show(mensaje, tipo);
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
          controlFechaEnvioSellado
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
          controlFechaEnvioSellado
        ].forEach(function ($el) {
            if ($el && $el.length) {
                inicializarDatePicker($el, $el.is(controlFechaRecepcionBoleto));
            }
        });
    }

    function bloqueaControlesSinOblea() {
        var habilitaOperaSinOblea = state.OperaSinOblea ? true : false;
        var habilitaCartaOferta = state.EsCartaOferta ? true : false;
        var desHabilitaSinBoleta = state.EsSinBoleto ? true : false;

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


        var feRecepcionBoleto    = getKendoDate(controlFechaRecepcionBoleto);
        var feEnvioFirmas = getKendoDate(controlFechaEnvioFirma);
        var feEnvioBolsa     = getKendoDate(controlFechaEnvioBolsa);
        var feEnvioAfip = getKendoDate(controlFechaEnvioAfip);
        var feRecepcionFirma = getKendoDate(controlFechaRecepcionFirma);
        var feRecepcionBolsa = getKendoDate(controlFechaRecepcionBolsa);
        var feRecepcionAfip  = getKendoDate(controlFechaRecepcionAfip);
        var feEnvioSellado = getKendoDate(controlFechaEnvioSellado);

        if (controlBoleto.val() == '' || controlCaracterBoleto.val() == '') {
            return "Debe seleccionar un boleto y un carácter.";
        }

        if (controlBolsa.val() == '') {
            return "Debe seleccionar una bolsa.";
        }

        if(state.EsSinBoleto) {
            return null;
        }

        // 🔴 Base obligatoria para validar relaciones
        if (!feRecepcionBoleto) {
            return "La Fecha de recepción de boleto es obligatoria.";
        }

        // 📌 Envío Firmas >= Recepción boleto
        if (feEnvioFirmas && feEnvioFirmas < feRecepcionBoleto)
            return "La Fecha de envío Firmas no puede ser anterior a la Fecha de recepción de boleto.";

        // 📌 Envío Obleado Bolsa >= Recepción boleto
        if ((feEnvioBolsa && !state.OperaSinOblea) && feEnvioBolsa < feRecepcionBoleto)
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
        if (feRecepcionBolsa && !state.OperaSinOblea) {
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
        if (feEnvioSellado && !state.EsCartaOferta) {
            if (feEnvioSellado < feRecepcionBoleto)
                return "La Fecha de envío Sellado no puede ser anterior a la Fecha de recepción de boleto.";

            if (feRecepcionBolsa && feEnvioSellado < feRecepcionBolsa)
                return "La Fecha de envío Sellado debe ser igual o mayor a la Fecha de recepción Obleado Bolsa.";
        }


        return null; // ✅ Todo correcto
    }
    function obtenerRequest() {
        return {
            Id: state.SeguimientoBoletoId ? state.SeguimientoBoletoId : 0,
            ControlDeBoletosId: state.ControlDeBoletosId,
            BolsaCompraNetId: controlBolsa.val(),
            BolsaSellado: controlBolsaSellado.val(),
            BoletoSapId: controlBoleto.val(),
            BoletoSapCaracter: controlCaracterBoleto.val(),
            RechazadoAfip: controlRechazadoAfip.val(),
            FechaRecepcionBoleto: getKendoDateISO(controlFechaRecepcionBoleto),
            FechaEnvioFirma: getKendoDateISO(controlFechaEnvioFirma),
            FechaEnvioAfip: getKendoDateISO(controlFechaEnvioAfip),
            FechaEnvioBolsa: getKendoDateISO(controlFechaEnvioBolsa),

            FechaRecepcionFirma: getKendoDateISO(controlFechaRecepcionFirma),           
            FechaRecepcionAfip: getKendoDateISO(controlFechaRecepcionAfip),
            FechaRecepcionBolsa: getKendoDateISO(controlFechaRecepcionBolsa),

            FechaEnvioSellado: getKendoDateISO(controlFechaEnvioSellado),
            ObsCtrlBoleto: controlObsCtrlBoleto.val(),
            ObsCtrlBoleto2: controlObsCtrlBoleto2.val()
        };
    }
    async function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");
        try {
            var data = await MSExecuteGetOnServerAsync(url);
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
    async function cargarDropdownBoletoSap(selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");
        try {
            var data = state.BoletoSap;
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
            setKendoDate(controlFechaEnvioFirma, response.FechaEnvioFirmas);
            setKendoDate(controlFechaEnvioBolsa, response.FechaEnvioBolsa);
            setKendoDate(controlFechaEnvioAfip, response.FechaEnvioAfip);

            setKendoDate(controlFechaRecepcionFirma, response.FechaRecepcionFirma);
            setKendoDate(controlFechaRecepcionBolsa, response.FechaRecepcionBolsa);
            setKendoDate(controlFechaRecepcionAfip, response.FechaRecepcionAfip);

            setKendoDate(controlFechaEnvioSellado, response.FechaEnvioSellado);
            state.SeguimientoBoletoId = response.Id;

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

        actualizarMinimosFechas();
    }

    return {

        inicializar: async function (ControlDeBoletosId, OperaSinOblea, EsCartaOferta, EsSinBoleto, BoletoCompraNet) {
            bindControls();
            inicializarFechas();
            state.ControlDeBoletosId = ControlDeBoletosId;
            state.OperaSinOblea = OperaSinOblea;
            state.EsCartaOferta = EsCartaOferta;
            state.EsSinBoleto = EsSinBoleto;
            state.BoletoCompraNet = BoletoCompraNet;
            limpiarSeguimientoControlBoleto();
            bloqueaControlesSinOblea();
            await Promise.all([
                cargarDropdown(config.urls.getBolsa, controlBolsa, "Cargando...", "Seleccione una bolsa"),
                cargarBolsaSAP()
            ]);
            var parametros = "?boletoCompraNetId=" + state.BoletoCompraNet;
            var urlBoletoSap = config.urls.getBoletoSap + parametros;
            var dataBoletoSap = await MSExecuteGetOnServerAsync(urlBoletoSap);
            state.BoletoSap = dataBoletoSap || [];
            cargarDropdownBoletoSap(controlBoleto, "Cargando...", "Seleccione un boleto");

            if (state.ControlDeBoletosId != null && state.ControlDeBoletosId > 0) {
                await obtener(state.ControlDeBoletosId);
            }
        },

        abrir: async function (ControlDeBoletosId) {
            bindControls();
            inicializarFechas();
            state.ControlDeBoletosId = ControlDeBoletosId;
            limpiarSeguimientoControlBoleto();
            BlockUi('Cargando...');

            try {
                await Promise.all([
                    cargarDropdown(config.urls.getBoletoCompraNet, controlBoleto, "Cargando...", "Seleccione un boleto"),
                    cargarDropdown(config.urls.getBolsa, controlBolsa, "Cargando...", "Seleccione una bolsa"),
                    cargarBolsaSAP()
                ]);

                if (state.ControlDeBoletosId != null && state.ControlDeBoletosId > 0) {
                    await obtener(state.ControlDeBoletosId);
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

            try {
                var response = await MSExecuteOnServerAsync(config.urls.createSeguimiento, request);
                if (!response) return;

                if (response.success) {
                    MensInfo(response.message);
                    this.cerrar();
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

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };
})();
