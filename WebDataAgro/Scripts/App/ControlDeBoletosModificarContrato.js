var ControlDeBoletosModificarContrato = (function () {
    "use strict";
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
        tipo = tipo || "info";
        $("#mensajeModalTitle").text(titulo);
        $("#mensajeModalBody").html(
            '<div class="alert alert-' + tipo + '">' + mensaje + "</div>",
        );
        $("#mensajeModal").modal("show");
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
                }
            })
            .catch(function (error) {
                console.error("Error cargando dropdown:", error);
                $select.html('<option value="">Error al cargar</option>');
            });
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
        inicializar: function (NegocioId, Contrato) {
            bindControls();
            state.negocioId = NegocioId;
            this.cargarContrato(Contrato);
        },
        abrir: function (NegocioId) {
            bindControls();
            BlockUi('Cargando...');
            //state.negocioId = NegocioId;
            //this.cargarContrato(state.negocioId);
            $("#modalModificarBoleto").modal("show");
            setTimeout(function () { $.unblockUI() }, 1000);
        },
        cargarContrato: function (contrato) {
            state.contratoSAP = contrato.ContratoSAP;
            this.cargarCombos(contrato);
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

        guardar: function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                MensAlerta(errores.join("<br>"));
                return;
            }

            var self = this;
            state.cargando = true;
            BlockUi('Guardando...');
            var request = obtenerRequest();
            MSExecuteOnServerAsync(config.urls.updateContrato, request)
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
                    console.error("Error al guardar contrato:", e);
                })
                .then(function () {
                    state.cargando = false;
                });
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        },
    };
})();
