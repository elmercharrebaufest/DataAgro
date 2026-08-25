var ControlDeBoletosModificarContrato = (function () {
    "use strict";
    var ui = window.ControlDeBoletosUI;
    var config = {
        urls: {
            getProvincias: "/ControlDeBoletos/GetProvincias",
            getClasificaciones: "/ControlDeBoletos/GetClasificaciones",
            getCosechas: "/ControlDeBoletos/GetCosechas",
            getProcedencias: "/ControlDeBoletos/GetProcedencias",
            getContrato: "/ControlDeBoletos/ObtenerDatosDeContrato",
            updateContrato: "/ControlDeBoletos/ModificarContrato",
        },
        modalId: "#modalModificarBoleto",
    };

    var state = {
        contratoSAP: null,
        negocioId: null,
        contrato: null,
        cargando: false,
    };

    let controlProvincia;
    let controlClasificacion;
    let controlCosecha;
    let controlProcedencia;

    function bindControls() {
        var $form = $("#accordionGestionBoleto #frmModificarContrato");
        if (!$form.length) $form = $("#frmModificarContrato").first();
        controlProvincia     = $form.find("#Provincia");
        controlClasificacion = $form.find("#Clasificacion");
        controlCosecha       = $form.find("#Cosecha");
        controlProcedencia   = $form.find("#Procedencia");
    }

    // ======================
    // Funciones privadas
    // ======================

    function mostrarMensaje(titulo, mensaje, tipo) {
        ui.showModalMessage(titulo, mensaje, tipo);
    }
    async function cargarDropdown(url, selector, textoCarga, textoDefault) {
        return ui.loadDropdown(url, selector, textoCarga, textoDefault);
    }

    function obtenerRequest() {
        return {
            ProvinciaId: controlProvincia.val(),
            ClasificacionId: controlClasificacion.val(),
            CosechaId: controlCosecha.val(),
            ProcedenciaId: controlProcedencia.val(),
            NegocioId: state.negocioId
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!controlCosecha.val()) errores.push("Debe seleccionar una cosecha");
        if (!controlProvincia.val()) errores.push("Debe seleccionar una provincia");
        if (!controlClasificacion.val()) errores.push("Debe seleccionar una clasificacion");
        if (!controlProcedencia.val()) errores.push("Debe seleccionar una procedencia");

        return errores;
    }

    // ======================
    // API pública
    // ======================
    return {
        inicializar: async function (NegocioId) {
            bindControls();
            state.negocioId = NegocioId;
            await this.cargarContrato();
        },
        abrir: function (NegocioId) {
            bindControls();
            BlockUi('Cargando...');
            $("#modalModificarBoleto").modal("show");
            setTimeout(function () { $.unblockUI() }, 1000);
        },
        cargarContrato: async function () {
            BlockUi('Cargando...');
            var url = config.urls.getContrato + "?id=" + state.negocioId;
            try {
                var contrato = await MSExecuteGetOnServerAsync(url);
                if (!contrato) {
                    return;
                }
                state.contrato = contrato;
                state.contratoSAP = contrato.ContratoSAP;
                await this.cargarCombos(contrato);
                $.unblockUI() 
            } catch (e) {
                console.error("Error cargando contrato:", e);
            }
        },
        cargarCombos: function (contrato) {
            var self = this;
            var p1 = cargarDropdown(config.urls.getProvincias,     controlProvincia,     "Cargando...", "Seleccione provincia");
            var p2 = cargarDropdown(config.urls.getClasificaciones, controlClasificacion, "Cargando...", "Seleccione clasificación");
            var p3 = cargarDropdown(config.urls.getCosechas,        controlCosecha,       "Cargando...", "Seleccione cosecha");
            var p4 = cargarDropdown(
                config.urls.getProcedencias + "?provinciaId=" + contrato.ProvinciaId,
                controlProcedencia,
                "Cargando...",
                "Seleccione procedencia"
            );

            return Promise.all([p1, p2, p3, p4]).then(function () {
                controlProvincia.val(contrato.ProvinciaId);
                controlClasificacion.val(contrato.ClasificacionId);
                controlCosecha.val(contrato.CampanaId);
                controlProcedencia.val(contrato.LocalidadId);
            });
        },

        guardar: async function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                MensAlerta(errores.join("<br>"));
                return;
            }

            state.cargando = true;
            BlockUi('Guardando...');
            var request = obtenerRequest();

            try {
                var response = await MSExecuteOnServerAsync(config.urls.updateContrato, request);
                if (!response) return;

                if (response.success) {
                    MensInfo(response.message);
                    this.cerrar();
                } else {
                    MensErr(response.message);
                }
            } catch (e) {
                console.error("Error al guardar contrato:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        },
    };
})();
