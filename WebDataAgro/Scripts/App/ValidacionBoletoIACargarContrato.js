var ValidacionBoletosCargarContrato = (function () {
    "use strict";

    // ======================
    // Configuración
    // ======================
    var config = {
        urls: {
            getTracking: "/ValidacionBoletoIA/GetTrackingBoleto"
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

    function mostrarModal() {
        $(config.modalId).modal("show");
    }
    function configurarEventos() {
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
