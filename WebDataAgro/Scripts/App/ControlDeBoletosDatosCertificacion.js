var ControlDeBoletosDatosCertificacion = (function () {
    "use strict";

    var ui = window.ControlDeBoletosUI;

    var config = {
        urls: {
            createPreCertificacion: "/ControlDeBoletos/RegistrarDatosPreCertificacion",
            getPreCertificacion:    "/ControlDeBoletos/GetDatosPreCertificacion",
            getBolsaCompraNet:      "/ControlDeBoletos/GetBolsaCompraNet",
            getTipoObleaConCodigo: "/ControlDeBoletos/GetTipoObleaConCodigo",
            getVerificarTipoBoletoyFechaRecepcion: "/ControlDeBoletos/GetVerificarTipoBoletoyFechaRecepcion",
            getVerificarDuplicidadObleaCodigoArca: "/ControlDeBoletos/GetVerificarDuplicidadObleaCodigoArca",
            eliminarDatosPreCertificacion: "/ControlDeBoletos/EliminarDatosPreCertificacion",

        },
        modalId: "#modalCertificacion"
    };

    var state = {
        cargando: false,
        controlDeBoletosId: null,
        operaSinOblea: false,
        planCanje: false,
        verificaDatosSeguimiento: false,
        esSinBoleto: false,
        fechaRecepcionBoleto: null,
        bolsa: null,
        contratoSAP: null,
        existeDatosCerificacion: false
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
        return ui.getKendoDate($el);
    }

    function getKendoDateISO($el) {
        return ui.getKendoDateISO($el);
    }

    function setKendoDate($el, value) {
        ui.setKendoDate($el, value);
    }

    function parseFechaTexto(texto) {
        return ui.parseDateText(texto);
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
        if (minDateActual && minDateActual instanceof Date && !isNaN(minDateActual.getTime())) {
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

        if (minDate && minDate instanceof Date && !isNaN(minDate.getTime()) && selectedDate && selectedDate < minDate) {
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
        return ui.parseServerDate(dateValue);
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

        // Validar que minDate sea una instancia de Date válida
        if (!minDate || !(minDate instanceof Date) || isNaN(minDate.getTime())) {
            return;
        }

        [
            ctrl.fechaCertificacion,
            ctrl.fechaVencimiento,
            ctrl.fechaVencimientoProvisoria,
            ctrl.fechaRegistracionAfip,
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
        return ui.loadDropdown(url, $select, "Cargando...", textoDefault);
    }
    function cargarFechasPorDefecto() {
        var ahora = new Date();
        var ultimoDiaDelAnio = new Date(ahora.getFullYear(), 11, 31);
        if (!state.operaSinOblea) {
            setKendoDate(ctrl.fechaVencimiento, ultimoDiaDelAnio);
        }
        /*
        if (state.planCanje) {
            setKendoDate(ctrl.fechaVencimientoPlanCanje, ultimoDiaDelAnio);
        } else {
            if (!state.operaSinOblea) {
                setKendoDate(ctrl.fechaVencimiento, ultimoDiaDelAnio);
            }
        }
        */
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
        ui.showNotification(mensaje, tipo);
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
            if (!response || response.Detalle.length == 0) {
                cargarFechasPorDefecto();
                // Parsear fechaRecepcionBoleto correctamente
                state.fechaRecepcionBoleto = parseDateFromResponse(response.FechaRecepcionBoleto);

                // Recalcular mínimos con la fecha de recepción cargada
                actualizarMinimosFechas();
                return;
            }
            state.existeDatosCerificacion = true;
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
                    ctrl.rechazado.prop('checked', item.Rechazado == 'X');
                }
                if (codigo === 'A') {
                    // Registración AFIP
                    ctrl.codigoRegistracionAfip.val(item.Oblea || '');
                    setKendoDate(ctrl.fechaRegistracionAfip, item.FechaCertificacion);
                    ctrl.rechazadoAfip.prop('checked', item.Rechazado == 'X');
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
        var rechazado = ctrl.rechazado.is(":checked");
        bolsaId = oblea == '' ? null : bolsaId;
        fechaCert = oblea == '' ? null : fechaCert;
        fechaVenc = oblea == '' ? null : fechaVenc;
        rechazado = oblea == '' ? false : rechazado;

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
        var rechazadoAfip = ctrl.rechazadoAfip.is(":checked");
        fechaAfip = codigoAfip == '' ? null : fechaAfip;
        rechazadoAfip = codigoAfip == '' ? false : rechazadoAfip;

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
        var fechaVencCanje = getKendoDateISO(ctrl.fechaVencimientoPlanCanje);

        bolsaCanjeId = obleaCanje == '' ? null : bolsaCanjeId;
        fechaCertCanje = obleaCanje == '' ? null : fechaCertCanje;
        fechaVencCanje = obleaCanje == '' ? null : fechaVencCanje;

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
    function sumar72HorasHabiles(fechaTexto) {
        var partes = fechaTexto.split('/');
        var fecha = new Date(
            parseInt(partes[2], 10),
            parseInt(partes[1], 10) - 1,
            parseInt(partes[0], 10)
        );
        var incremento = [3, 3, 3, 5, 5, 5, 4][fecha.getDay()];
        fecha.setDate(fecha.getDate() + incremento);
        return fecha;
    }
    function validarDatosPreCertificacion(request) {
        let mensaje = '';
        const detalleObleaBolsa = request.find(x => x.CodigoTipoOblea === 'O');
        const detalleRegistracionArca = request.find(x => x.CodigoTipoOblea === 'A');
        const detallePlanCanje = request.find(x => x.CodigoTipoOblea === 'F');
        if (detalleObleaBolsa) {
            if (detalleObleaBolsa.Oblea != ''
                && (
                (detalleObleaBolsa.BolsaCompraNetId == null || detalleObleaBolsa.BolsaCompraNetId == '') ||
                (detalleObleaBolsa.FechaCertificacion == null || detalleObleaBolsa.FechaCertificacion == '') ||
                (detalleObleaBolsa.FechaVencimiento == null || detalleObleaBolsa.FechaVencimiento == '')
                    )
            ) {
                mensaje = 'Falto ingresar datos para el registro de oblea de bolsa.';
                return mensaje;
            }
        }
        if (detalleRegistracionArca) {
            if (detalleRegistracionArca.Oblea != ''
                && (detalleRegistracionArca.FechaCertificacion == null || detalleRegistracionArca.FechaCertificacion == '')
            ) {
                mensaje = 'Falto ingresar datos para el registro registración arca.';
                return mensaje;
            }
        }
        if (detallePlanCanje) {
            if (detallePlanCanje.Oblea != ''
                && (
                (detallePlanCanje.BolsaCompraNetId == null || detallePlanCanje.BolsaCompraNetId == '') ||
                (detallePlanCanje.FechaCertificacion == null || detallePlanCanje.FechaCertificacion == '') ||
                (detallePlanCanje.FechaVencimiento == null || detallePlanCanje.FechaVencimiento == '')
                )
            ) {
                mensaje = 'Falto ingresar datos para el registro de oblea plan de canje.';
                return mensaje;
            }
        }
        return mensaje;
    }
    // ======================
    // API pública
    // ======================
    return {

        inicializar: async function (ControlDeBoletosId, OperaSinOblea, PlanCanje, EsSinBoleto, Bolsa, ContratoSAP) {
            BlockUi('Cargando...');
            state.controlDeBoletosId = ControlDeBoletosId;
            state.operaSinOblea = OperaSinOblea;
            state.planCanje = PlanCanje;
            state.esSinBoleto = EsSinBoleto;
            state.bolsa = Bolsa;
            state.contratoSAP = ContratoSAP;
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
            ctrl.fechaCertificacionPlanCanje
                .off("blur")
                .on("blur", function () {
                    var valor = $.trim($(this).val());
                    if (!valor) {
                        setKendoDate(ctrl.fechaVencimientoPlanCanje, null);
                        return;
                    }
                    var fechaVencimiento = sumar72HorasHabiles(valor);
                    setKendoDate(
                        ctrl.fechaVencimientoPlanCanje,
                        fechaVencimiento
                    );
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
                var request = obtenerRequest();
                var mensaje = validarDatosPreCertificacion(request.Detalle);
                if (mensaje != '') {
                    MensAlerta(mensaje);
                    $.unblockUI();
                    return;
                }
                var oblea = request.Detalle.find(d => d.CodigoTipoOblea === 'O')?.Oblea || '';
                var codigoArca = request.Detalle.find(d => d.CodigoTipoOblea === 'A')?.Oblea || '';
                var urlValidacion = config.urls.getVerificarDuplicidadObleaCodigoArca + "?controlDeBoletosId=" + state.controlDeBoletosId + "&numeroOblea=" + oblea + "&codigoArca=" + codigoArca;
                var validacion = await MSExecuteGetOnServerAsync(urlValidacion);
                if (validacion!=null && validacion > '') {
                    MensErr(validacion);
                    return;
                }
                var response = await MSExecuteOnServerAsync(config.urls.createPreCertificacion, request);
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
        eliminar: async function () {
            if (!state.existeDatosCerificacion) {
                MensAlerta('No se ha registrado datos de certificación para el boleto.');
                return;
            }
            if (state.cargando) return;
            state.cargando = true;
            BlockUi('Guardando...');
            var request = new Object();
            request.controlDeBoletosId = state.controlDeBoletosId;
            try {
                Confirma('¿Desea eliminar todo los datos de precertificación para el contrato ' + state.contratoSAP + "?", async function () {
                    var response = await MSExecuteOnServerAsync(config.urls.eliminarDatosPreCertificacion, request);
                    if (!response) return;

                    if (response.success) {
                        MensInfo(response.message);
                        limpiarFormulario();
                        await cargarDatosExistentes();
                    } else {
                        MensErr(response.message);
                    }
                });
            } catch (e) {
                console.error("Error al eliminar datos de precertificación:", e);
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

