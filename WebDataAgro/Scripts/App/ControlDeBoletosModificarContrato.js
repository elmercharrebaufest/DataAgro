var ControlDeBoletosModificarContrato = (function () {
    "use strict";
    var config = {
        urls: {
            getProvincias: "/ControlDeBoletos/GetProvincias",
            getClasificaciones: "/ControlDeBoletos/GetClasificaciones",
            getCosechas: "/ControlDeBoletos/GetCosechas",
            getProcedencias: "/ControlDeBoletos/GetProcedencias",
            getContrato: "/ControlDeBoletos/ObtenerDetalleContrato",
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

        try {
            $select.html('<option value="">' + textoCarga + "</option>");

            const data =  MSExecuteGetOnServer(url);

            $select.empty().append('<option value="">' + textoDefault + "</option>");

            if (data && Array.isArray(data)) {
                $.each(data, function (i, item) {
                    $select.append(
                        '<option value="' + item.Value + '">' + item.Text + "</option>",
                    );
                });
            }
        } catch (error) {
            console.error("Error cargando dropdown:", error);
            $select.html('<option value="">Error al cargar</option>');
        }
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
        inicializar: function (NegocioId) {
            bindControls();
            state.negocioId = NegocioId;
            this.cargarContrato(state.negocioId);
        },
        abrir: function (NegocioId) {
            bindControls();
            BlockUi('Cargando...');
            state.negocioId = NegocioId;
            this.cargarContrato(state.negocioId);
            $("#modalModificarBoleto").modal("show");
            setTimeout(function () { $.unblockUI() }, 1000);
        },
        cargarContrato: function (id) {
            const url = config.urls.getContrato + "?id=" + id;
            const contrato = MSExecuteGetOnServer(url);
            state.contratoSAP = contrato.Data.ContratoSAP;
            this.cargarCombos(contrato.Data);
        },
        cargarCombos: function (contrato) {
            let url = config.urls.getProvincias;
            cargarDropdown(
                url,
                controlProvincia,
                "Cargando...",
                "Seleccione provincia",
            );
            url = config.urls.getClasificaciones;
            cargarDropdown(
                url,
                controlClasificacion,
                "Cargando...",
                "Seleccione clasificación",
            );
            url = config.urls.getCosechas;
            cargarDropdown(
                url,
                controlCosecha,
                "Cargando...",
                "Seleccione cosecha",
            );
            url =
                config.urls.getProcedencias + "?provinciaId=" + contrato.ProvinciaId;
            cargarDropdown(
                url,
                controlProcedencia,
                "Cargando...",
                "Seleccione procedencia",
            );

            controlProvincia.val(contrato.ProvinciaId);
            controlClasificacion.val(contrato.ClasificacionId);
            controlCosecha.val(contrato.CampanaId);
            controlProcedencia.val(contrato.LocalidadId);
        },

        guardar: function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                MensAlerta(errores.join("<br>"));
                return;
            }

            state.cargando = true;
            //mostrarSpinner(true);
            try {
                BlockUi('Guardando...');
                const request = obtenerRequest();
                const response = MSExecuteOnServer(config.urls.updateContrato, request);
                state.cargando = false;
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

                $.unblockUI();
            } finally {
                state.cargando = false;
                //mostrarSpinner(false);
            }
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        },
    };
})();
