var ValidacionBoletosCargarContrato = (function () {
    "use strict";

    // ======================
    // Configuración
    // ======================
    var config = {
        urls: {
            getTracking: "/ValidacionBoletoIA/GetTrackingBoleto",
            getValidacionBoletoSap: "/ValidacionBoletoIA/GetValidacionBoletoSap",
        },
        modalId: "#modalCargarBoleto",
        modalBodyId: "#modalCargarBoletoBody",
        gridId: "#gridCargarBoleto"
    };

    let controlContratoSap = $("#frmCargarBoleto #contratoSap");
    let controlTipoBoleto = $("#frmCargarBoleto #tipoBoleto");
    let controlBolsa = $("#frmCargarBoleto #bolsa");
    let controlArchivoPDF = $("#frmCargarBoleto #filArchivoPDF");
    let controlNombreArchivoPDF = $("#frmCargarBoleto #filNombreArchivoPDF");
    let controlUploadBoxPDF = $("#frmCargarBoleto #uploadBoxPDF");
    let controlCargarValidar = $("#frmCargarBoleto #btnCargarValidar");
    function mostrarModal() {
        $(config.modalId).modal("show");
    }
    function habilitaControles(disabled) {
        controlArchivoPDF.prop("disabled", disabled);
        controlNombreArchivoPDF.prop("disabled", disabled);
        controlUploadBoxPDF.prop("disabled", disabled);
        controlCargarValidar.prop("disabled", disabled);
    }
    function configurarEventos() {

        controlContratoSap = $("#frmCargarBoleto #contratoSap");
        controlTipoBoleto = $("#frmCargarBoleto #tipoBoleto");
        controlBolsa = $("#frmCargarBoleto #bolsa");
        controlArchivoPDF = $("#frmCargarBoleto #filArchivoPDF");
        controlNombreArchivoPDF = $("#frmCargarBoleto #filNombreArchivoPDF");
        controlUploadBoxPDF = $("#frmCargarBoleto #uploadBoxPDF");
        controlCargarValidar = $("#frmCargarBoleto #btnCargarValidar");

        controlTipoBoleto.text('');
        controlBolsa.text('');
        habilitaControles(true);

        controlContratoSap
            .on("blur", async function (e) {
                let texto = $(this).val();
                if (texto == '') return;
                controlTipoBoleto.text('');
                controlBolsa.text('');
                BlockUi('Buscando contrato...');
                try {
                    var response = await MSExecuteGetOnServerAsync(config.urls.getValidacionBoletoSap, { contratoSAP: texto });
                    if (!response) return;
                    if (response.Data) {
                        if (response.Data.length > 0) {
                            var contrato = response.Data[0];
                            controlTipoBoleto.text(contrato.TipoBoleto);
                            controlBolsa.text(contrato.Bolsa);
                            habilitaControles(false);
                        } else {
                            MensErr('No se encontro el contrato.');
                        }
                    }
                } catch (e) {
                    console.error("Error al buscar el contrato:", e);
                } finally {
                    $.unblockUI();
                }
            });

        controlUploadBoxPDF.on("click", function (e) {
            // Evita abrir el selector si se hace clic sobre el input
            if (!$(e.target).is("#filArchivoPDF")) {
                controlArchivoPDF.click();
            }
        });

        controlArchivoPDF.on("change", function () {
            if (this.files.length > 0) {
                controlNombreArchivoPDF.html(
                    '<i class="fa fa-file-pdf-o"></i> ' +
                    this.files[0].name
                );
            } else {
                controlNombreArchivoPDF.html(
                    '<i class="fa fa-folder-open-o"></i> Ningún archivo seleccionado'
                );
            }
        });
    }
    // ======================
    // API pública
    // ======================
    return {

        abrir: function () {

            var url = "/ValidacionBoletoIA/_CargarBoletosValidacion";

            $(config.modalId).remove();

            $.get(url, function (html) {
                $("body").append(html);
                try {
                    //inicializarGrid();
                    mostrarModal();
                    configurarEventos();
                } catch (error) {
                    console.error("Error cargando tracking:", error);
                }
            });
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        },

    };

})();
