var ValidacionBoletosResultados = (function () {
    "use strict";

    // ======================
    // Configuración
    // ======================
    var config = {
        urls: {
            getValidacionBoletoSap: "/ValidacionBoletoIA/GetValidacionBoletosResultados",
            rechazarValidacionResultado: "/ValidacionBoletoIA/RechazarValidacionResultado",
            aprobarValidacionResultado: "/ValidacionBoletoIA/AprobarValidacionResultado",

        },
        modalId: "#modalResultadoBoleto",
        modalBodyId: "#modalResultadoBoletoBody",
        gridId: "#gridResultadoValidacion"
    };
    // ======================
    // Estado interno
    // ======================
    var state = {
        validacionBoletosId: null,
        contratoSAP: null,
        bolsaCompraNet: null,
        material: null,
        proveedor: null,
        estadoValidacionAgente: null,
        accionesRecomendadas: null,
        fechaValidacion: null,
    };

    let controlContratoSap = $("#frmValidacionBoleto #lblContratoSap");
    let controlMaterial = $("#frmValidacionBoleto #lblMaterial");
    let controlEstadoValidacionAgente = $("#frmValidacionBoleto #lblEstadoValidacionAgente");
    let controlAccionesRecomendadas = $("#frmValidacionBoleto #lblAccionesRecomendadas");
    let controlFechaValidacion = $("#frmValidacionBoleto #lblFechaValidacion");
    let controlObservaciones = $("#frmValidacionBoleto #txtObservacion");

    let controlCancelar = $("#frmValidacionBoleto #btnCancelar");
    let controlRechazar = $("#frmValidacionBoleto #btnRechazar");
    let controlAprobar = $("#frmValidacionBoleto #btnAprobar");
    function mostrarModal() {
        $(config.modalId).modal("show");
    }

    // ======================
    // Estilos del grid
    // ======================
    function inyectarEstilosGrid() {
        if ($("#grid-tracking-styles").length) return;
        $("<style id='grid-tracking-styles'>").text(`
            #gridResultadoValidacion .k-grid-header th {
                font-weight: bold !important;
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
                white-space: nowrap;
                background-color: #f5f5f5;
            }
            #gridResultadoValidacion .k-grid-content td {
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
            }
            #gridResultadoValidacion .k-grid-header-wrap {
                overflow: hidden !important;
            }
            #gridResultadoValidacion .k-grid-content {
                overflow-x: auto !important;
                overflow-y: auto !important;
            }
            #gridResultadoValidacion .k-grid-header-wrap table,
            #gridResultadoValidacion .k-grid-content table {
                table-layout: fixed;
            }
            #gridResultadoValidacion .k-grid-content tr:hover td,
            #gridResultadoValidacion .k-grid-content tr.k-state-hover td {
                color: #333 !important;
            }
        `).appendTo("head");
    }

    // ======================
    // Auto-ajuste de columnas
    // ======================
    var _canvas = document.createElement("canvas");

    function medirTexto(texto, fuente) {
        var ctx = _canvas.getContext("2d");
        ctx.font = fuente;
        return Math.ceil(ctx.measureText(texto).width);
    }

    function autoFitColumnas(grid) {
        var $wrapper = grid.element;
        var $headerCols = $wrapper.find(".k-grid-header-wrap colgroup col");
        var $contentCols = $wrapper.find(".k-grid-content   colgroup col");
        var $headerCells = $wrapper.find(".k-grid-header-wrap tr:first th");
        var $rows = $wrapper.find(".k-grid-content tbody tr");
        var columns = grid.columns;

        $headerCells.each(function (colIdx) {
            var colDef = columns[colIdx];
            var headerText = $(this).find(".k-link").text().trim() || $(this).text().trim();
            var maxPx = medirTexto(headerText, "bold 13px Arial") + 32;

            $rows.each(function () {
                var cellPx = medirTexto($(this).find("td").eq(colIdx).text().trim(), "13px Arial") + 24;
                if (cellPx > maxPx) maxPx = cellPx;
            });

            maxPx = Math.max(maxPx, colDef && colDef.width ? colDef.width : 80);
            $headerCols.eq(colIdx).css("width", maxPx + "px");
            $contentCols.eq(colIdx).css("width", maxPx + "px");
        });

        var totalWidth = Array.from($headerCols).reduce(function (sum, col) {
            return sum + (parseInt($(col).css("width")) || 0);
        }, 0);
        $wrapper.find(".k-grid-header-wrap table, .k-grid-content table").css("width", totalWidth + "px");
    }

    // ======================
    // Inicialización del grid
    // ======================
    function inicializarGrid() {

        controlContratoSap.text(state.contratoSAP);
        controlMaterial.text(state.material);
        controlFechaValidacion.text(state.fechaValidacion);
        controlEstadoValidacionAgente.text(state.estadoValidacionAgente);
        controlAccionesRecomendadas.text(state.accionesRecomendadas);

        if ($(config.gridId).data("kendoGrid")) {
            $(config.gridId).data("kendoGrid").destroy();
            $(config.gridId).empty();
        }

        inyectarEstilosGrid();

        var url = config.urls.getValidacionBoletoSap + "?validacionBoletosId=" + encodeURIComponent(state.validacionBoletosId);

        state.grid = $(config.gridId).kendoGrid({
            dataSource: {
                transport: {
                    read: function (options) {
                        MSExecuteGetOnServerAsync(url)
                            .then(function (data) {
                                options.success(data || { Data: [], Total: 0 });
                            })
                            .catch(function (error) {
                                console.error("Error cargando tracking:", error);
                                options.error(error);
                            });
                    }
                },
                schema: {
                    data: "Data",
                    total: "Total"
                }
            },
            height: 300,
            scrollable: true,
            sortable: {
                mode: "single",
                allowUnsort: false
            },
            filterable: false,
            pageable: false,
            navigatable: false,
            columns: [
                { field: "Campo", title: "Campo", width: 100 },
                { field: "ValorDocumento", title: "Valor PDF", width: 180 },
                { field: "ValorSistema", title: "Valor DA", width: 180 },
                { field: "Resultado", title: "Resultado", width: 130 },
                { field: "Severidad", title: "Severidad", width: 130 },
                { field: "Mensaje", title: "Mensaje", width: 130 },
                { field: "TipoCoincidencia", title: "Tipo Coincidencia", width: 130 },
            ],
            dataBound: function (e) {
                var data = e.sender.dataSource.data();
                if (data.length === 0) {
                    $(config.gridId).hide();
                    $("#lblNoResultadoValidacionInfo").show();
                } else {
                    $(config.gridId).show();
                    $("#lblNoResultadoValidacionInfo").hide();
                    autoFitColumnas(e.sender);
                }
            }
        }).data("kendoGrid");
    }


    function configurarEventos() {

        controlContratoSap = $("#frmValidacionBoleto #lblContratoSap");
        controlMaterial = $("#frmValidacionBoleto #lblMaterial");
        controlFechaValidacion = $("#frmValidacionBoleto #lblFechaValidacion");
        controlCancelar = $("#frmValidacionBoleto #btnCancelar");
        controlAprobar = $("#frmValidacionBoleto #btnAprobar");
        controlRechazar = $("#frmValidacionBoleto #btnRechazar");
        controlEstadoValidacionAgente = $("#frmValidacionBoleto #lblEstadoValidacionAgente");
        controlAccionesRecomendadas = $("#frmValidacionBoleto #lblAccionesRecomendadas");
        controlObservaciones = $("#frmValidacionBoleto #txtObservacion");
        controlContratoSap.text('');
        controlMaterial.text('');
        controlFechaValidacion.text('');
        controlEstadoValidacionAgente.text('');
        controlAccionesRecomendadas.text('');
        controlObservaciones.val('');

        controlAprobar.on("click", async function (e) {
            var request = {
                validacionBoletosId: state.validacionBoletosId,
                observacion: controlObservaciones.val(),
            };
            BlockUi('Guardando...');
            try {
                var response = await MSExecuteOnServerAsync(config.urls.aprobarValidacionResultado, request);
                if (!response) return;
                if (response.success) {
                    MensInfo(response.message);
                    this.cerrar();
                } else {
                    MensErr(response.message);
                }
            } catch (e) {
                console.error("Error al aprobar la validación de resultados:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
        });

        controlRechazar.on("click", async function (e) {
            var request = {
                validacionBoletosId: state.validacionBoletosId,
                observacion: controlObservaciones.val(),
            };
            BlockUi('Guardando...');
            try {
                var response = await MSExecuteOnServerAsync(config.urls.rechazarValidacionResultado, request);
                if (!response) return;
                if (response.success) {
                    MensInfo(response.message);
                    this.cerrar();
                } else {
                    MensErr(response.message);
                }
            } catch (e) {
                console.error("Error al rechazar la validación de resultados:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
        });

        controlCancelar.on("click", function (e) {
            $(config.modalId).modal("hide");
        });


    }
    // ======================
    // API pública
    // ======================
    return {

        abrir: function (validacionBoletosId, contratoSAP, bolsaCompraNet,
                         material, proveedor, estadoValidacionAgente,
                         accionesRecomendadas, fechaValidacion) {

            state.validacionBoletosId = validacionBoletosId;
            state.contratoSAP = contratoSAP;
            state.bolsaCompraNet = bolsaCompraNet;
            state.material = material;
            state.proveedor = proveedor;
            state.fechaValidacion = fechaValidacion;
            state.estadoValidacionAgente = estadoValidacionAgente;
            state.accionesRecomendadas = accionesRecomendadas;
            var url = "/ValidacionBoletoIA/_ResultadoBoletosValidacion";

            $(config.modalId).remove();

            $.get(url, function (html) {
                $("body").append(html);
                try {
                    mostrarModal();
                    configurarEventos();
                    inicializarGrid();
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
