var ControlDeBoletosDatosCertificacion = (function () {
    "use strict";

    var config = {
        urls: {
            createPreCertificacion: "/ControlDeBoletos/RegistrarDatosPreCertificacion",
            getPreCertificacion: "/ControlDeBoletos/GetDatosPreCertificacion",
            getBolsaCompraNet: "/ControlDeBoletos/GetBolsaCompraNet",
            getTipoOblea: "/ControlDeBoletos/GetTipoOblea",
        },
        modalId: "#modalCertificacion"
    };

    var state = {
        cargando: false,
        PreCertificacionId: null,
        ControlDeBoletosId: null
    };

    // ======================
    // Funciones privadas
    // ======================

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
        var $form = $("#accordionGestionBoleto #frmCertificacion");
        if (!$form.length) $form = $("#frmCertificacion").first();
        [$form.find("#FechaCertificacion"), $form.find("#FechaVencimientoCertificacion")].forEach(function ($el) {
            if ($el.data("kendoDatePicker")) $el.data("kendoDatePicker").destroy();
            $el.kendoDatePicker({ weekNumber: true, format: "dd/MM/yyyy", value: null });
        });
    }

    function limpiarFormularioCertificacion() {

        // Inputs tipo date (kendo)
        var $form = $("#accordionGestionBoleto #frmCertificacion");
        if (!$form.length) $form = $("#frmCertificacion").first();
        setKendoDate($form.find("#FechaCertificacion"), null);
        setKendoDate($form.find("#FechaVencimientoCertificacion"), null);

        // Input texto
        $("#Oblea").val('');

        // Selects
        $("#frmCertificacion #Bolsa").val(null).trigger('change');
        $("#frmCertificacion #TipoOblea").val(null).trigger('change');
    }
    function mostrarMensaje(titulo, mensaje, tipo) {
        tipo = tipo || "info";
        alert(titulo + ": " + mensaje);
    }
    function formatearFecha(value) {
        if (!value) return '';

        const match = /\/Date\((\d+)\)\//.exec(value);
        const date = match
            ? new Date(parseInt(match[1], 10))
            : new Date(value);

        if (isNaN(date)) return '';

        const day = String(date.getUTCDate()).padStart(2, '0');
        const month = String(date.getUTCMonth() + 1).padStart(2, '0');
        const year = date.getUTCFullYear();

        return `${year}-${month}-${day}`;
    }
    function obtenerRequest() {
        return {
            Id: state.PreCertificacionId ? state.PreCertificacionId : 0,
            ControlDeBoletosId: state.ControlDeBoletosId,
            BolsaCompraNetId: $("#frmCertificacion #Bolsa").val(),
            FechaCertificacion: getKendoDateISO($("#frmCertificacion #FechaCertificacion")),
            FechaVencimiento: getKendoDateISO($("#frmCertificacion #FechaVencimientoCertificacion")),
            Oblea: $("#Oblea").val(),
            TipoObleaId: $("#frmCertificacion #TipoOblea").val(),
            Rechazado: $("#Rechazado").is(":checked")
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!$("#frmCertificacion #Bolsa").val())
            errores.push("Debe ingresar la bolsa");

        if (!getKendoDate($("#frmCertificacion #FechaCertificacion")))
            errores.push("Debe ingresar la fecha de certificación");

        return errores;
    }

    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = $(selector);
        $select.html('<option value="">' + textoCarga + "</option>");

        try {
            var data = MSExecuteGetOnServer(url);
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

    // ======================
    // API pública
    // ======================
    return {

        inicializar: function (PreCertificacionId, ControlDeBoletosId) {
            inicializarFechas();
            state.ControlDeBoletosId = ControlDeBoletosId;
            state.PreCertificacionId = PreCertificacionId;

            cargarDropdown(
                config.urls.getBolsaCompraNet,
                "#frmCertificacion #Bolsa",
                "Cargando...",
                "Todos las bolsas",
            );

            cargarDropdown(
                config.urls.getTipoOblea,
                "#frmCertificacion #TipoOblea",
                "Cargando...",
                "Todos los tipos de oblea",
            );

            limpiarFormularioCertificacion();

            if (PreCertificacionId != null && PreCertificacionId > 0) {
                var url = config.urls.getPreCertificacion + "?datosPreCertificacionId=" + PreCertificacionId;
                var response = MSExecuteGetOnServer(url);
                if (response != null) {
                    setKendoDate($("#frmCertificacion #FechaCertificacion"), response.FechaCertificacion);
                    setKendoDate($("#frmCertificacion #FechaVencimientoCertificacion"), response.FechaVencimiento);
                    $("#frmCertificacion #Oblea").val(response.Oblea);
                    $("#frmCertificacion #Bolsa").val(response.BolsaCompraNetId).trigger('change');
                    $("#frmCertificacion #TipoOblea").val(response.TipoObleaId).trigger('change');
                }
            }
        },

        abrir: function (PreCertificacionId, ControlDeBoletosId) {

            inicializarFechas();
            BlockUi('Cargando...');
            state.ControlDeBoletosId = ControlDeBoletosId;
            state.PreCertificacionId = PreCertificacionId;

            cargarDropdown(
                config.urls.getBolsaCompraNet,
                "#frmCertificacion #Bolsa",
                "Cargando...",
                "Todos las bolsas",
            );

            cargarDropdown(
                config.urls.getTipoOblea,
                "#frmCertificacion #TipoOblea",
                "Cargando...",
                "Todos los tipos de oblea",
            );

            limpiarFormularioCertificacion();

            if (PreCertificacionId != null && PreCertificacionId > 0) {
                var url = config.urls.getPreCertificacion + "?datosPreCertificacionId=" + PreCertificacionId;
                var response = MSExecuteGetOnServer(url);
                if (response != null) {
                    setKendoDate($("#frmCertificacion #FechaCertificacion"), response.FechaCertificacion);
                    setKendoDate($("#frmCertificacion #FechaVencimientoCertificacion"), response.FechaVencimiento);
                    $("#Oblea").val(response.Oblea);
                    $("#frmCertificacion #Bolsa").val(response.BolsaCompraNetId).trigger('change');
                    $("#frmCertificacion #TipoOblea").val(response.TipoObleaId).trigger('change');
                }
            }

            $(config.modalId).modal("show");
            setTimeout(function () { $.unblockUI() }, 1000);
        },

        guardar: function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                MensAlerta(errores.join("<br>"));
                return;
            }

            state.cargando = true;

            try {
                BlockUi('Guardando...');
                var request = obtenerRequest();
                var response = MSExecuteOnServer(
                    config.urls.createPreCertificacion,
                    request
                );
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
            } finally {
                state.cargando = false;
            }
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };

})();
