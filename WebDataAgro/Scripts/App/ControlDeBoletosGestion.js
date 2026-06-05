var ControlDeBoletosGestion = (function () {
    "use strict";

    var config = {
        modalId: "#modalGestionControlBoleto",
        getContrato: "/ControlDeBoletos/ObtenerDatosDeContrato",
    };

    var state = {
        ControlDeBoletosId: null,
        SeguimientoBoletoId: null,
        PreCertificacionId: null,
        NegocioId: null,
        OperaSinOblea: false,
        PlanCanje: false,
    };


    let controlContratoSAP;
    let controlCuitVendedor;
    let controlCuitCorredor;
    let controlPizarraReferencia;
    let controlMaterial;
    let controlProvincia;
    let controlKilos;
    let controlProcedencia;
    let controlPrecioXTonelada;
    let controlDestino;
    let controlClasificacionProveedor;
    let controlStandardCalidad;
    let controlPeriodoEntrega;
    let controlCampania;
    let controlTipoBoleto;
    let controlPeriodoOperacion;
    let controlRecepcionBoleto;
    let controlMoneda;

    async function configurarEventos() {

        $("#collapseModificar").on("show.bs.collapse", async function () {
            if (state.NegocioId > 0) {
                ControlDeBoletosModificarContrato.inicializar(state.NegocioId);
            }
        });

        $("#collapseCertificacion").on("shown.bs.collapse", async function () {
            await ControlDeBoletosDatosCertificacion.inicializar(
                state.ControlDeBoletosId,
                state.OperaSinOblea,
                state.PlanCanje
            );
        });

        $("#collapseSeguimiento").on("shown.bs.collapse", async function () {
            await ControlDeBoletosSeguimiento.inicializar(
                state.ControlDeBoletosId
            );
        });

    }
    function bindControls() {
        let $form = $("#accordionGestionBoleto #frmGestionarContrato");

        if (!$form.length) {
            $form = $("#frmGestionarContrato").first();
        }

        controlContratoSAP = $form.find("#ContratoSAP");
        controlCuitVendedor = $form.find("#CuitVendedor");
        controlCuitCorredor = $form.find("#CuitCorredor");
        controlPizarraReferencia = $form.find("#PizarraReferencia");
        controlMaterial = $form.find("#Material");
        controlProvincia = $form.find("#Provincia");
        controlKilos = $form.find("#Kilos");
        controlProcedencia = $form.find("#Procedencia");
        controlPrecioXTonelada = $form.find("#PrecioXTonelada");
        controlDestino = $form.find("#Destino");
        controlClasificacionProveedor = $form.find("#ClasificacionProveedor");
        controlStandardCalidad = $form.find("#StandardCalidad");
        controlPeriodoEntrega = $form.find("#PeriodoEntrega");
        controlCampania = $form.find("#Campania");
        controlTipoBoleto = $form.find("#TipoBoleto");
        controlPeriodoOperacion = $form.find("#PeriodoOperacion");
        controlRecepcionBoleto = $form.find("#RecepcionBoleto");
        controlMoneda = $form.find("#Moneda");
    }

    async function cargarDatos() {
        var url = config.getContrato + "?id=" + state.NegocioId;
        BlockUi("Cargando...");
        try {
            var contrato = await MSExecuteGetOnServerAsync(url);
            if (!contrato) return;

            controlContratoSAP.text(contrato.ContratoSAP);
            controlCuitCorredor.text(contrato.CuitCorredor);
            controlCuitVendedor.text(contrato.CuitVendedor);
            controlMaterial.text(contrato.Material);
            controlProvincia.text(contrato.Provincia);
            controlKilos.text(contrato.Cantidad);
            controlProcedencia.text(contrato.Localidad);
            controlPrecioXTonelada.text(contrato.Precio);
            controlMoneda.text(contrato.Moneda);
            controlDestino.text(contrato.Destino);
            controlClasificacionProveedor.text(contrato.Clasificacion);
            controlStandardCalidad.text(contrato.StandarCalidad);
            controlPeriodoEntrega.text(contrato.PeriodoEntrega);
            controlCampania.text(contrato.Campana);
            controlTipoBoleto.text(contrato.TipoBoleto);
            controlPeriodoOperacion.text(contrato.FechaOperacion);

            state.OperaSinOblea = contrato.OperaSinOblea;
            state.PlanCanje = contrato.PlanCanje;

        } catch (e) {
            console.error("Error cargando datos del contrato:", e);
        } finally {
            $.unblockUI();
        }
    }

    return {
        abrir: function (params) {
            state.ControlDeBoletosId  = params.ControlDeBoletosId  || 0;
            state.SeguimientoBoletoId = params.SeguimientoBoletoId || 0;
            state.NegocioId           = params.NegocioId           || 0;

            var parametros = "?controlDeBoletosId=" + state.ControlDeBoletosId + "&seguimientoBoletoId=" + state.SeguimientoBoletoId + "&negocioId=" + state.NegocioId;
            var url = "/ControlDeBoletos/_GestionControlBoleto" + parametros;

            // Remover modal anterior si existe
            $(config.modalId).remove();

            $.get(url, function (html) {
                $('body').append(html);

                try {
                    bindControls();
                    cargarDatos();
                    configurarEventos();
                    $(config.modalId).modal("show");
                } catch (error) {
                    console.error("Error cargando tracking:", error);
                }
            });


        },
        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };
})();
