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
    let controlVendedor;
    let controlCorredor;
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
    let controlVersionBoleto;
    let controlFechaGeneracion;
    let controlNumeroSio;
    let controlFechaCierta;
    let controlCantidadFijacionMaxima;
    let controlCantidadFijacionMinima;
    let controlCesion;
    let controlCompensacion;
    let controlDolarizadoExpress;
    let controlEUDR;
    let controlEPA;
    let controlSustentable;
    let controlCD;
    let controlPagoDirectoVendedor;
    let controlCanje;

    function setControlText(control, value, defaultValue) {
        control.text(value != null ? value : (defaultValue || ""));
    }

    function boolToSiNo(value) {
        return value ? "SI" : "NO";
    }


    async function configurarEventos() {

        $('a[href="#tabContrato"]').on("shown.bs.tab", async function () {
            if (state.negocioId > 0) {
                ControlDeBoletosModificarContrato.inicializar(state.negocioId);
            }
        });

        $('a[href="#tabCertificacion"]').on("shown.bs.tab", async function () {
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

        $('a[href="#tabSeguimiento"]').on("shown.bs.tab", async function () {
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
        let $form = $("#tabsGestionBoleto #frmGestionarContrato");

        if (!$form.length) {
            $form = $("#frmGestionarContrato").first();
        }

        controlContratoSAP = $form.find("#ContratoSAP");
        controlCuitVendedor = $form.find("#CuitVendedor");
        controlCuitCorredor = $form.find("#CuitCorredor");
        controlVendedor = $form.find("#Vendedor");
        controlCorredor = $form.find("#Corredor");
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
        controlVersionBoleto = $form.find("#VersionBoleto");
        controlFechaGeneracion = $form.find("#FechaGeneracion");
        controlNumeroSio = $form.find("#NumeroSio");
        controlFechaCierta = $form.find("#FechaCierta");
        controlCantidadFijacionMaxima = $form.find("#CantidadFijacionMaxima");
        controlCantidadFijacionMinima = $form.find("#CantidadFijacionMinima");
        controlCesion = $form.find("#Cesion");
        controlCompensacion = $form.find("#Compensacion");
        controlDolarizadoExpress = $form.find("#DolarizadoExpress");
        controlEUDR = $form.find("#EUDR");
        controlEPA = $form.find("#EPA");
        controlSustentable = $form.find("#Sustentable");
        controlCD = $form.find("#CD");
        controlPagoDirectoVendedor = $form.find("#PagoDirectoVendedor");
        controlCanje = $form.find("#Canje");
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
            controlVendedor.text(contrato.Proveedor);
            controlCorredor.text(contrato.Corredor);
            controlMaterial.text(contrato.Material);
            controlProvincia.text(contrato.Provincia);
            controlKilos.text(contrato.Cantidad);
            controlProcedencia.text(contrato.Localidad);
            setControlText(controlPrecioXTonelada, contrato.PrecioNeto, '');
            setControlText(controlMoneda, contrato.Moneda, '');
            controlDestino.text(contrato.Destino);
            controlClasificacionProveedor.text(contrato.Clasificacion + ' - Consignatario:' + contrato.Consignatario);
            controlStandardCalidad.text(contrato.StandarCalidad);
            controlPeriodoEntrega.text(contrato.PeriodoEntrega);
            controlCampania.text(contrato.Campana);
            controlTipoBoleto.text(contrato.TipoBoleto);
            controlPeriodoOperacion.text(contrato.FechaOperacion);
            setControlText(controlVersionBoleto, contrato.VersionBoleto, '');
            setControlText(controlFechaGeneracion, contrato.FechaGeneracion, '');
            setControlText(controlNumeroSio, contrato.NumeroSio, '');

            setControlText(controlFechaCierta, contrato.FechaCierta, '');
            setControlText(controlCantidadFijacionMaxima, contrato.CantidadFijacionMaxima, '');
            setControlText(controlCantidadFijacionMinima, contrato.CantidadFijacionMinima, '');
            controlCesion.text(boolToSiNo(contrato.Cesion));
            controlCompensacion.text(boolToSiNo(contrato.Compensacion));
            controlDolarizadoExpress.text(boolToSiNo(contrato.DolarizadoExpress));
            controlEUDR.text(boolToSiNo(contrato.EUDR));
            controlEPA.text(boolToSiNo(contrato.EPA));
            controlSustentable.text(boolToSiNo(contrato.Sustentable));
            controlCD.text(boolToSiNo(contrato.CD));
            controlPagoDirectoVendedor.text(boolToSiNo(contrato.PagoDirectoVendedor));
            controlCanje.text(boolToSiNo(contrato.Canje));

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
