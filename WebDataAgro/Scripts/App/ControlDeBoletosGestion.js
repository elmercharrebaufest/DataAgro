var ControlDeBoletosGestion = (function () {
    "use strict";

    var config = {
        modalId: "#modalGestionControlBoleto",
        getContrato: "/ControlDeBoletos/ObtenerDatosDeContrato",
        eliminarControlDeBoletos: "/ControlDeBoletos/EliminarControlDeBoletos",
    };

    var state = {
        controlDeBoletosId: null,
        seguimientoBoletoId: null,
        preCertificacionId: null,
        negocioId: null,
        operaSinOblea: false,
        planCanje: false,
        esCartaOferta: false,
        esSinBoleto: false,
        boletoCompraNet: null,
        contratoSAP: null,
        bolsa: null
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
            if (state.negocioId > 0) {
                ControlDeBoletosModificarContrato.inicializar(state.negocioId);
            }
        });

        $("#collapseCertificacion").on("shown.bs.collapse", async function () {
            // state.EsSinBoleto = true;
            // state.OperaSinOblea = true;

            await ControlDeBoletosDatosCertificacion.inicializar(
                state.controlDeBoletosId,
                state.operaSinOblea,
                state.planCanje,
                state.esSinBoleto,
                state.bolsa,
                state.contratoSAP
            );
        });

        $("#collapseSeguimiento").on("shown.bs.collapse", async function () {
            await ControlDeBoletosSeguimiento.inicializar(
                state.controlDeBoletosId,
                state.operaSinOblea,
                state.esCartaOferta,
                state.esSinBoleto,
                state.boletoCompraNet,
                state.contratoSAP
            );
        });
        $("#btnEliminarControlBoleto").on("click", async function () {
            BlockUi('Eliminando...');

            try {
                Confirma('¿Desea eliminar todo el control de boletos para el contrato ' + state.contratoSAP + "?", async function () {
                    var request = {
                        ControlDeBoletosId: state.controlDeBoletosId,
                        ContratoSAP: state.contratoSAP
                    };
                    var response = await MSExecuteOnServerAsync(config.eliminarControlDeBoletos, request);
                    if (response.success) {
                        MensInfo(response.message);
                    } else {
                        MensErr(response.message);
                    }
                });
            } catch (e) {
                console.error("Error al guardar:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
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
        var url = config.getContrato + "?id=" + state.negocioId;
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

            state.operaSinOblea = contrato.OperaSinOblea;
            state.planCanje = contrato.PlanCanje;
            state.esCartaOferta = contrato.EsCartaOferta;
            state.esSinBoleto = contrato.EsSinBoleto;
            state.boletoCompraNet = contrato.BoletoCompraNetId;
            state.contratoSAP = contrato.ContratoSAP;
            state.bolsa = contrato.BolsaId;

        } catch (e) {
            console.error("Error cargando datos del contrato:", e);
        } finally {
            $.unblockUI();
        }
    }

    return {
        abrir: function (params) {
            state.controlDeBoletosId = params.ControlDeBoletosId || 0;
            state.seguimientoBoletoId = params.SeguimientoBoletoId || 0;
            state.negocioId = params.NegocioId || 0;

            var parametros = "?controlDeBoletosId=" + state.controlDeBoletosId + "&seguimientoBoletoId=" + state.seguimientoBoletoId + "&negocioId=" + state.negocioId;
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
