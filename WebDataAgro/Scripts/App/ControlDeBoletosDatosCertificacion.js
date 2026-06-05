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
        verificaDatosSeguimiento: false
    };

    // Controles cacheados del formulario
    var ctrl = {};

    // ======================
    // Funciones privadas
    // ======================

    function bindControls() {
        var $form = $("#accordionGestionBoleto #frmCertificacion");
        if (!$form.length) $form = $("#frmCertificacion").first();

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

    function inicializarDatePicker($el) {
        if ($el.data("kendoDatePicker")) $el.data("kendoDatePicker").destroy();
        $el.kendoDatePicker({ weekNumber: true, format: "dd/MM/yyyy", value: null });
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

    function bloqueaControlesSinOblea() {
        var enabledOperaSinOblea = state.operaSinOblea? true: false;
        var enabledPlanCanje = !state.planCanje? true: false;

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
        bindControls();
        limpiarFormulario();
        inicializarFechas();
        return Promise.all([
            cargarDropdown(config.urls.getBolsaCompraNet, ctrl.bolsa,         "Seleccione una bolsa"),
            cargarDropdown(config.urls.getBolsaCompraNet, ctrl.bolsaPlanCanje, "Seleccione una bolsa")
        ]);
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

        inicializar: async function (ControlDeBoletosId, OperaSinOblea, PlanCanje) {
            BlockUi('Cargando...');
            state.controlDeBoletosId = ControlDeBoletosId;
            state.operaSinOblea = OperaSinOblea;
            state.planCanje = PlanCanje
            try {
                await setup();
                await verificarTipoBoletoYFechaRecepcion();
                if (state.controlDeBoletosId > 0) {
                    await cargarDatosExistentes();
                }
                this.configurarEventos();
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
                if (state.controlDeBoletosId > 0) {
                    await cargarDatosExistentes();
                }
                $(config.modalId).modal("show");
            } finally {
                $.unblockUI();
            }
        },
        configurarEventos: function () {

            if (!ctrl || !ctrl.oblea) {
                console.error("No se encontró ctrl.oblea");
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

