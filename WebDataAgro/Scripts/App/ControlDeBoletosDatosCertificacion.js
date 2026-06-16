var ControlDeBoletosDatosCertificacion = (function () {
    "use strict";

    var config = {
        urls: {
            createPreCertificacion: "/ControlDeBoletos/RegistrarDatosPreCertificacion",
            getPreCertificacion:    "/ControlDeBoletos/GetDatosPreCertificacion",
            getBolsaCompraNet:      "/ControlDeBoletos/GetBolsaCompraNet",
            getTipoObleaConCodigo: "/ControlDeBoletos/GetTipoObleaConCodigo",
            getVerificarTipoBoletoyFechaRecepcion: "/ControlDeBoletos/GetVerificarTipoBoletoyFechaRecepcion"
        },
        modalId: "#modalCertificacion"
    };

    var state = {
        cargando:           false,
        controlDeBoletosId: null,
        operaSinOblea: false,
        planCanje: false,
        verificaDatosSeguimiento: false,
        esSinBoleto: false,
        fechaRecepcionBoleto: null
    };

    // Controles cacheados del formulario
    var ctrl = {};

    // ======================
    // Funciones privadas
    // ======================

    function bindControls() {
        var $form = $("#accordionGestionBoleto #frmCertificacion");
        if (!$form.length) $form = $("#frmCertificacion").first();

        if (!$form.length) {
            console.error("bindControls: No se encontró el formulario #frmCertificacion");
            return false;
        }

        ctrl.rechazado                   = $form.find("#Rechazado");
        ctrl.oblea                       = $form.find("#Oblea");
        ctrl.bolsa                       = $form.find("#Bolsa");
        ctrl.fechaCertificacion          = $form.find("#FechaCertificacion");
        ctrl.fechaVencimiento            = $form.find("#FechaVencimiento");

        ctrl.rechazadoAfip               = $form.find("#RechazadoAfip");
        ctrl.codigoRegistracionAfip      = $form.find("#CodigoRegistracionAfip");
        ctrl.fechaRegistracionAfip       = $form.find("#FechaRegistracionAfip");

        ctrl.fechaVencimientoProvisoria  = $form.find("#FechaVencimientoObleaProvisoria");

        ctrl.obleaPlanCanje              = $form.find("#ObleaPlanCanje");
        ctrl.bolsaPlanCanje              = $form.find("#BolsaPlanCanje");
        ctrl.fechaCertificacionPlanCanje = $form.find("#FechaCertificacionPlanCanje");
        ctrl.fechaVencimientoPlanCanje   = $form.find("#FechaVencimientoPlanCanje");

        return true;
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
        var date  = match ? new Date(parseInt(match[1], 10)) : new Date(value);
        dp.value(isNaN(date.getTime()) ? null : date);
    }

    function parseFechaTexto(texto) {
        if (!texto) return null;
        var parsed = kendo.parseDate(texto, "dd/MM/yyyy") || kendo.parseDate(texto);
        return parsed && !isNaN(parsed.getTime()) ? parsed : null;
    }

    function inicializarDatePicker($el) {
        if ($el.data("kendoDatePicker")) $el.data("kendoDatePicker").destroy();

        $el.off(".seg");

        var options = {
            weekNumber: true,
            format: "dd/MM/yyyy",
            value: null
        };

        var minDateActual = state.fechaRecepcionBoleto;
        if (minDateActual && !isNaN(minDateActual.getTime())) {
            options.min = minDateActual;
        }

        $el.kendoDatePicker(options);

        $el.on("change.seg blur.seg", function () {
            validarFechaMinima($el);
        });
    }

    function validarFechaMinima($el) {
        var dp = $el.data("kendoDatePicker");
        if (!dp) return true;

        var texto = $.trim($el.val());
        var selectedDate = dp.value();
        var minDate = state.fechaRecepcionBoleto;

        if (texto !== "" && !selectedDate) {
            selectedDate = parseFechaTexto(texto);
            if (selectedDate) {
                dp.value(selectedDate);
            }
        }

        if (texto !== "" && !selectedDate) {
            $el.addClass("fecha-invalida");
            $el.val("");
            mostrarNotificacion("Ingrese una fecha válida", "error");
            setTimeout(function () {
                $el.focus();
            }, 100);
            return false;
        }

        if (minDate && selectedDate && selectedDate < minDate) {
            $el.addClass("fecha-invalida");
            dp.value(null);
            $el.val("");
            mostrarNotificacion("La fecha no puede ser anterior a " + formatDateForDisplay(minDate), "error");
            setTimeout(function () {
                $el.focus();
            }, 100);
            return false;
        }

        if (selectedDate) {
            $el.removeClass("fecha-invalida");
        }

        return true;
    }

    function parseDateFromResponse(dateValue) {
        if (!dateValue) return null;

        // Si es una cadena vacía
        if (typeof dateValue === 'string' && $.trim(dateValue) === '') return null;

        var date = null;

        // Formato JSON de .NET: /Date(1234567890)/
        var match = /\/Date\((\d+)\)\//.exec(dateValue);
        if (match) {
            date = new Date(parseInt(match[1], 10));
        } else {
            // Intenta parsear como ISO o fecha normal
            date = new Date(dateValue);
        }

        // Verificar que la fecha sea válida
        if (isNaN(date.getTime())) {
            console.warn("Invalid date format received:", dateValue);
            return null;
        }

        return date;
    }
    function formatDateForDisplay(date) {
        if (!date) return "";
        var day = ("0" + date.getDate()).slice(-2);
        var month = ("0" + (date.getMonth() + 1)).slice(-2);
        var year = date.getFullYear();
        return day + "/" + month + "/" + year;
    }
    function inicializarFechas() {
        [
            ctrl.fechaCertificacion,
            ctrl.fechaVencimiento,
            ctrl.fechaRegistracionAfip,
            ctrl.fechaVencimientoProvisoria,
            ctrl.fechaCertificacionPlanCanje,
            ctrl.fechaVencimientoPlanCanje
        ].forEach(function ($el) {
            if ($el && $el.length) inicializarDatePicker($el);
        });
    }

    function actualizarMinimosFechas() {
        var minDate = state.fechaRecepcionBoleto;
        if (!minDate || isNaN(minDate.getTime())) return;

        [
            ctrl.fechaCertificacion,
            ctrl.fechaVencimiento,
            ctrl.fechaVencimientoProvisoria,
            ctrl.fechaCertificacionPlanCanje,
            ctrl.fechaVencimientoPlanCanje
        ].forEach(function ($el) {
            if (!$el || !$el.length) return;

            var dp = $el.data("kendoDatePicker");
            if (!dp) return;

            dp.min(minDate);
            var actual = dp.value();
            if (actual && actual < minDate) {
                dp.value(null);
                $el.val("");
                $el.addClass("fecha-invalida");
            }
        });
    }
    function limpiarFormulario() {
        setKendoDate(ctrl.fechaCertificacion,          null);
        setKendoDate(ctrl.fechaVencimiento,            null);
        setKendoDate(ctrl.fechaRegistracionAfip,       null);
        setKendoDate(ctrl.fechaVencimientoProvisoria,  null);
        setKendoDate(ctrl.fechaCertificacionPlanCanje, null);
        setKendoDate(ctrl.fechaVencimientoPlanCanje,   null);

        ctrl.oblea.val('');
        ctrl.obleaPlanCanje.val('');
        ctrl.codigoRegistracionAfip.val('');

        ctrl.rechazado.prop('checked', false);
        ctrl.rechazadoAfip.prop('checked', false);

        ctrl.bolsa.val(null).trigger('change');
        ctrl.bolsaPlanCanje.val(null).trigger('change');
    }
    async function cargarDropdown(url, $select, textoDefault) {
        $select.html('<option value="">Cargando...</option>');
        try {
            var data = await MSExecuteGetOnServerAsync(url);
            $select.empty().append('<option value="">' + textoDefault + '</option>');
            if (data && Array.isArray(data)) {
                $.each(data, function (i, item) {
                    $select.append('<option value="' + item.Value + '">' + item.Text + '</option>');
                });
            }
        } catch (e) {
            $select.html('<option value="">Error al cargar datos</option>');
        }
    }
    function cargarFechasPorDefecto() {
        if (state.esSinBoleto) return;

        var ahora = new Date();
        var ultimoDiaDelAnio = new Date(ahora.getFullYear(), 11, 31);

        if (state.planCanje) {
            setKendoDate(ctrl.fechaVencimientoPlanCanje, ultimoDiaDelAnio);
        } else {
            if (!state.operaSinOblea) {
                setKendoDate(ctrl.fechaVencimiento, ultimoDiaDelAnio);
            }
        }
    }
    function bloqueaControlesSinOblea() {
        var enabledOperaSinOblea = state.operaSinOblea? true: false;
        var enabledPlanCanje = !state.planCanje ? true : false;
        var esSinBoleto = state.esSinBoleto ? true : false;
        if (esSinBoleto) {
            [
                ctrl.fechaCertificacion,
                ctrl.fechaVencimiento,
                ctrl.fechaVencimientoProvisoria,
                ctrl.fechaCertificacionPlanCanje,
                ctrl.fechaVencimientoPlanCanje,
                ctrl.rechazado,
                ctrl.oblea,
                ctrl.bolsa,
                ctrl.obleaPlanCanje,
                ctrl.bolsaPlanCanje
            ].forEach(function (control) {
                control.prop("disabled", true);
            });
            return;
        } else {
            [
                ctrl.fechaCertificacion,
                ctrl.fechaVencimiento,
                ctrl.fechaVencimientoProvisoria,
                ctrl.fechaCertificacionPlanCanje,
                ctrl.fechaVencimientoPlanCanje,
                ctrl.rechazado,
                ctrl.oblea,
                ctrl.bolsa,
                ctrl.obleaPlanCanje,
                ctrl.bolsaPlanCanje
            ].forEach(function (control) {
                control.prop("disabled", false);
            });
        }

        [
            ctrl.obleaPlanCanje,
            ctrl.bolsaPlanCanje,
            ctrl.fechaCertificacionPlanCanje,
            ctrl.fechaVencimientoPlanCanje
        ].forEach(function (control) {
            control.prop("disabled", enabledPlanCanje);
        });

        [
            ctrl.fechaCertificacionPlanCanje,
            ctrl.fechaVencimientoPlanCanje
        ].forEach(function (control) {
            control.data("kendoDatePicker").enable(!enabledPlanCanje);
        });

        [
            ctrl.rechazado,
            ctrl.oblea,
            ctrl.bolsa,
            ctrl.fechaCertificacion,
            ctrl.fechaVencimiento
        ].forEach(function (control) {
            control.prop("disabled", enabledOperaSinOblea);
        });

        [
            ctrl.fechaCertificacion,
            ctrl.fechaVencimiento
        ].forEach(function (control) {
            control.data("kendoDatePicker").enable(!enabledOperaSinOblea);
        });
    }
    function bloqueaControles() {
        var enabledControles = !state.verificaDatosSeguimiento ? true : false;
        [ctrl.rechazado,
        ctrl.oblea,
        ctrl.bolsa,
        ctrl.rechazadoAfip,
        ctrl.codigoRegistracionAfip,
        ctrl.obleaPlanCanje,
        ctrl.bolsaPlanCanje
        ].forEach(function (control) {
            control.prop("disabled", enabledControles);
        });

        [
            ctrl.fechaCertificacion,
            ctrl.fechaVencimiento,
            ctrl.fechaRegistracionAfip,
            ctrl.fechaVencimientoProvisoria,
            ctrl.fechaCertificacionPlanCanje,
            ctrl.fechaVencimientoPlanCanje,
        ].forEach(function (control) {
            control.data("kendoDatePicker").enable(!enabledControles);
        });

    }		
    function setup() {
        if (!bindControls()) {
            throw new Error("No se pudieron enlazar los controles del formulario");
        }
        limpiarFormulario();
        inicializarFechas();
        return Promise.all([
            cargarDropdown(config.urls.getBolsaCompraNet, ctrl.bolsa,         "Seleccione una bolsa"),
            cargarDropdown(config.urls.getBolsaCompraNet, ctrl.bolsaPlanCanje, "Seleccione una bolsa")
        ]);
    }
    function mostrarNotificacion(mensaje, tipo) {
        var $notification = $("#notification");
        var notification = $notification.data("kendoNotification");

        if (!notification) {
            $notification.kendoNotification({
                position: {
                    pinned: true,
                    top: 50,
                    left: "50%"
                },
                autoHideAfter: 3000,
                stacking: "down"
            });
            notification = $notification.data("kendoNotification");
        }

        notification.show(mensaje, tipo);
    }
    async function verificarTipoBoletoYFechaRecepcion() {
        if (!state.controlDeBoletosId) return;
        var url = config.urls.getVerificarTipoBoletoyFechaRecepcion + "?controlDeBoletosId=" + state.controlDeBoletosId;
        try {
            var response = await MSExecuteGetOnServerAsync(url);
            if (!response) state.verificaDatosSeguimiento = false;
            if (response) {
                state.verificaDatosSeguimiento = response == "SI" ? true : false;
            }

            bloqueaControles();

            if (state.verificaDatosSeguimiento)
                bloqueaControlesSinOblea();

        } catch (e) {
            console.error("Error al verificar tipo de boleto y fecha de recepción:", e);
        }
    }
    async function cargarDatosExistentes() {
        if (!state.controlDeBoletosId) return;

        var url = config.urls.getPreCertificacion + "?controlDeBoletosId=" + state.controlDeBoletosId;
        try {
            var response = await MSExecuteGetOnServerAsync(url);
            if (!response || !response.Detalle || !response.Detalle.length) return;

            // Re-bind controls para garantizar que apunten al DOM correcto
            bindControls();

            ctrl.rechazadoAfip.prop('checked', false);
            ctrl.rechazado.prop('checked', false);
            state.controlDeBoletosId = response.ControlDeBoletosId || state.controlDeBoletosId;

            // Parsear fechaRecepcionBoleto correctamente
            state.fechaRecepcionBoleto = parseDateFromResponse(response.FechaRecepcionBoleto);

            // Recalcular mínimos con la fecha de recepción cargada
            actualizarMinimosFechas();

            $.each(response.Detalle, function (i, item) {
                var codigo = item.CodigoTipoOblea;

                if (codigo === 'O') {
                    // Precertificación Oblea Bolsa
                    ctrl.oblea.val(item.Oblea || '');
                    ctrl.bolsa.val(item.BolsaCompraNetId || '').trigger('change');
                    setKendoDate(ctrl.fechaCertificacion, item.FechaCertificacion);
                    setKendoDate(ctrl.fechaVencimiento,   item.FechaVencimiento);
                    ctrl.rechazado.prop('checked', item.Rechazado === 'X');
                }
                if (codigo === 'A') {
                    // Registración AFIP
                    ctrl.codigoRegistracionAfip.val(item.Oblea || '');
                    setKendoDate(ctrl.fechaRegistracionAfip, item.FechaCertificacion);
                    ctrl.rechazadoAfip.prop('checked', item.Rechazado === 'X');
                }
                if (codigo === 'P') {
                    // Oblea Provisoria
                    setKendoDate(ctrl.fechaVencimientoProvisoria, item.FechaVencimiento);
                }
                if (codigo === 'F') {
                    // Oblea Plan Canje
                    ctrl.obleaPlanCanje.val(item.Oblea || '');
                    ctrl.bolsaPlanCanje.val(item.BolsaCompraNetId || '').trigger('change');
                    setKendoDate(ctrl.fechaCertificacionPlanCanje, item.FechaCertificacion);
                    setKendoDate(ctrl.fechaVencimientoPlanCanje,   item.FechaVencimiento);
                }
            });
            if (ctrl.fechaVencimientoPlanCanje.val() == '' ||
                ctrl.fechaVencimiento.val() == '')
                cargarFechasPorDefecto();

        } catch (e) {
            console.error("Error cargando datos existentes:", e);
        }
    }

    function obtenerRequest() {
        var detalle = [];

        // Precertificación Oblea Bolsa (código 'O')
        var oblea              = ctrl.oblea.val();
        var bolsaId            = ctrl.bolsa.val() ? parseInt(ctrl.bolsa.val(), 10) : null;
        var fechaCert          = getKendoDateISO(ctrl.fechaCertificacion);
        var fechaVenc          = getKendoDateISO(ctrl.fechaVencimiento);
        var rechazado          = ctrl.rechazado.is(":checked");
        if (oblea || bolsaId || fechaCert || fechaVenc) {
            detalle.push({
                ControlDeBoletosId: state.controlDeBoletosId,
                CodigoTipoOblea:    'O',
                Oblea:              oblea,
                BolsaCompraNetId:   bolsaId,
                FechaCertificacion: fechaCert,
                FechaVencimiento:   fechaVenc,
                Rechazado:          rechazado ? "X" : ""
            });
        }

        // Registración AFIP (código 'A')
        var codigoAfip         = ctrl.codigoRegistracionAfip.val();
        var fechaAfip          = getKendoDateISO(ctrl.fechaRegistracionAfip);
        var rechazadoAfip      = ctrl.rechazadoAfip.is(":checked");
        if (codigoAfip || fechaAfip) {
            detalle.push({
                ControlDeBoletosId: state.controlDeBoletosId,
                CodigoTipoOblea:    'A',
                Oblea: codigoAfip,
                FechaCertificacion: fechaAfip,
                Rechazado:          rechazadoAfip ? "X" : ""
            });
        }

        // Oblea Provisoria (código 'P')
        var fechaVencProvisoria = getKendoDateISO(ctrl.fechaVencimientoProvisoria);
        if (fechaVencProvisoria) {
            detalle.push({
                ControlDeBoletosId: state.controlDeBoletosId,
                CodigoTipoOblea:    'P',
                FechaVencimiento:   fechaVencProvisoria
            });
        }

        // Oblea Plan Canje (código 'F')
        var obleaCanje         = ctrl.obleaPlanCanje.val();
        var bolsaCanjeId       = ctrl.bolsaPlanCanje.val() ? parseInt(ctrl.bolsaPlanCanje.val(), 10) : null;
        var fechaCertCanje     = getKendoDateISO(ctrl.fechaCertificacionPlanCanje);
        var fechaVencCanje     = getKendoDateISO(ctrl.fechaVencimientoPlanCanje);
        if (obleaCanje || bolsaCanjeId || fechaCertCanje || fechaVencCanje) {
            detalle.push({
                ControlDeBoletosId: state.controlDeBoletosId,
                CodigoTipoOblea:    'F',
                Oblea:              obleaCanje,
                BolsaCompraNetId:   bolsaCanjeId,
                FechaCertificacion: fechaCertCanje,
                FechaVencimiento:   fechaVencCanje
            });
        }

        return {
            ControlDeBoletosId: state.controlDeBoletosId,
            Detalle: detalle
        };
    }

    // ======================
    // API pública
    // ======================
    return {

        inicializar: async function (ControlDeBoletosId, OperaSinOblea, PlanCanje, EsSinBoleto) {
            BlockUi('Cargando...');
            state.controlDeBoletosId = ControlDeBoletosId;
            state.operaSinOblea = OperaSinOblea;
            state.planCanje = PlanCanje;
            state.esSinBoleto = EsSinBoleto;
            try {
                await setup();
                await verificarTipoBoletoYFechaRecepcion();
                if (state.controlDeBoletosId > 0)
                    await cargarDatosExistentes();
                this.configurarEventos();
            } catch (e) {
                console.error("Error al inicializar ControlDeBoletosDatosCertificacion:", e);
                MensErr("Error cargando datos del contrato: " + e.message);
            } finally {
                $.unblockUI();
            }
        },

        abrir: async function (ControlDeBoletosId) {
            BlockUi('Cargando...');
            state.controlDeBoletosId = ControlDeBoletosId;

            try {
                await setup();
                bloqueaControlesSinOblea();
                if (state.controlDeBoletosId > 0)
                    await cargarDatosExistentes();
                $(config.modalId).modal("show");
            } finally {
                $.unblockUI();
            }
        },
        configurarEventos: function () {

            if (!ctrl || !ctrl.oblea || !ctrl.oblea.length) {
                console.warn("configurarEventos: ctrl.oblea no está disponible");
                return;
            }

            if (!ctrl.fechaVencimientoProvisoria || !ctrl.fechaVencimientoProvisoria.length) {
                console.warn("configurarEventos: ctrl.fechaVencimientoProvisoria no está disponible");
                return;
            }

            ctrl.oblea
                .off("blur")
                .on("blur", function () {

                    var datePicker = ctrl.fechaVencimientoProvisoria.data("kendoDatePicker");

                    if (!datePicker) {
                        return;
                    }

                    if ($.trim(ctrl.oblea.val()) !== '') {
                        ctrl.fechaVencimientoProvisoria.val('');
                        datePicker.enable(false);
                    } else {
                        ctrl.fechaVencimientoProvisoria.val('');
                        datePicker.enable(true);
                    }
                });
        },
        guardar: async function () {

            if (!state.verificaDatosSeguimiento) {
                MensInfo("No se ha ingresado el tipo de boleto y fecha de recepción para el contrato.");
                return;
            }

            if (state.cargando) return;

            state.cargando = true;
            BlockUi('Guardando...');

            try {
                var response = await MSExecuteOnServerAsync(config.urls.createPreCertificacion, obtenerRequest());
                if (!response) return;

                if (response.success) {
                    MensInfo(response.message);
                    await cargarDatosExistentes();
                    this.cerrar();
                } else {
                    MensErr(response.message);
                }
            } catch (e) {
                console.error("Error al guardar:", e);
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

