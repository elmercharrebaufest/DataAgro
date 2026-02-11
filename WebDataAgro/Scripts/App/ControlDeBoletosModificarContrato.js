var ControlBoletosModificacion = (function () {
    "use strict";
    var config = {
        urls: {
            getProvincias: "/ControlDeBoletos/GetProvincias",
            getClasificaciones: "/ControlDeBoletos/GetClasificaciones",
            getCosechas: "/ControlDeBoletos/GetCosechas",
            getProcedencias: "/ControlDeBoletos/GetProcedencias",
            getContrato: "/ControlDeBoletos/ObtenerDetalleContrato",
            modificar: "/ControlDeBoletos/ModificarContrato",
        },
        modalId: "#modalModificarBoleto",
    };

    var state = {
        contratoSAP: null,
        negocioId: null,
        contrato: null,
        cargando: false,
    };

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

    async function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = $(selector);

        try {
            $select.html('<option value="">' + textoCarga + "</option>");

            const data = await MSExecuteGetOnServerAsync(url);

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
            Contrato: state.contratoSAP,
            ProvinciaId: $("#ImProvincia").val(),
            ClasificacionId: $("#ImClasificacion").val(),
            CosechaId: $("#ImCosecha").val(),
            ProcedenciaId: $("#ImProcedencia").val(),
            Fecha: $("#ImFecha").val(),
            Hora: $("#ImHora").val(),
            Usuario: "",
            NegocioId : state.negocioId
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!$("#ImCosecha").val()) errores.push("Debe seleccionar una cosecha");

        if (!$("#ImProvincia").val())
            errores.push("Debe seleccionar una provincia");

        if (!$("#ImFecha").val()) errores.push("Debe ingresar la fecha");

        if (!$("#ImHora").val()) errores.push("Debe ingresar la hora");

        return errores;
    }

    // ======================
    // API pública
    // ======================
    return {
        abrir: async function (NegocioId, data) {
            state.negocioId = NegocioId;
            await this.cargarContrato(state.negocioId);
            $(config.modalId).modal("show");
        },
        cargarContrato: async function (id) {
            const url = config.urls.getContrato + "?id=" + id;
            const contrato = await MSExecuteGetOnServerAsync(url);
            state.contratoSAP = contrato.Data.ContratoSAP;
            await this.cargarCombos(contrato.Data);
        },
        cargarCombos: async function (contrato) {
            let url = config.urls.getProvincias;
            await cargarDropdown(
                url,
                "#ImProvincia",
                "Cargando...",
                "Seleccione provincia",
            );
            url = config.urls.getClasificaciones;
            await cargarDropdown(
                url,
                "#ImClasificacion",
                "Cargando...",
                "Seleccione clasificación",
            );
            url = config.urls.getCosechas;
            await cargarDropdown(
                url,
                "#ImCosecha",
                "Cargando...",
                "Seleccione cosecha",
            );
            url =
                config.urls.getProcedencias + "?provinciaId=" + contrato.ProvinciaId;
            await cargarDropdown(
                url,
                "#ImProcedencia",
                "Cargando...",
                "Seleccione procedencia",
            );

            $("#ImProvincia").val(contrato.ProvinciaId);
            $("#ImClasificacion").val(contrato.ClasificacionId);
            $("#ImCosecha").val(contrato.CampanaId);
            $("#ImProcedencia").val(contrato.LocalidadId);
        },

        guardar: function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                mostrarMensaje("Validación", errores.join("<br>"), "warning");
                return;
            }

            state.cargando = true;
            //mostrarSpinner(true);
            try {
                const request = obtenerRequest();
                console.log('request-->>', request);
                const response = MSExecuteOnServer(config.urls.modificar, request);
                state.cargando = false;
                //mostrarSpinner(false);
                console.log(response);
                if (response != null) {
                    $.unblockUI();
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
