var ControlBoletosTracking = (function () {
    "use strict";

    // ======================
    // Configuración
    // ======================
    var config = {
        urls: {
            getTracking: "/ControlDeBoletos/GetTrackingBoleto"
        },
        modalId: "#modalTrackingBoleto",
        modalBodyId: "#modalTrackingBoletoBody",
        gridId: "#gridTrackingBoleto"
    };

    // ======================
    // Estado interno
    // ======================
    var state = {
        controlDeBoletosId: null,
        grid: null
    };

    // ======================
    // Estilos del grid
    // ======================
    function inyectarEstilosGrid() {
        if ($("#grid-tracking-styles").length) return;
        $("<style id='grid-tracking-styles'>").text(`
            #gridTrackingBoleto .k-grid-header th {
                font-weight: bold !important;
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
                white-space: nowrap;
                background-color: #f5f5f5;
            }
            #gridTrackingBoleto .k-grid-content td {
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
            }
            #gridTrackingBoleto .k-grid-header-wrap {
                overflow: hidden !important;
            }
            #gridTrackingBoleto .k-grid-content {
                overflow-x: auto !important;
                overflow-y: auto !important;
            }
            #gridTrackingBoleto .k-grid-header-wrap table,
            #gridTrackingBoleto .k-grid-content table {
                table-layout: fixed;
            }
            #gridTrackingBoleto .k-grid-content tr:hover td,
            #gridTrackingBoleto .k-grid-content tr.k-state-hover td {
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
        var $headerCols  = $wrapper.find(".k-grid-header-wrap colgroup col");
        var $contentCols = $wrapper.find(".k-grid-content   colgroup col");
        var $headerCells = $wrapper.find(".k-grid-header-wrap tr:first th");
        var $rows        = $wrapper.find(".k-grid-content tbody tr");
        var columns      = grid.columns;

        $headerCells.each(function (colIdx) {
            var colDef     = columns[colIdx];
            var headerText = $(this).find(".k-link").text().trim() || $(this).text().trim();
            var maxPx      = medirTexto(headerText, "bold 13px Arial") + 32;

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
        if ($(config.gridId).data("kendoGrid")) {
            $(config.gridId).data("kendoGrid").destroy();
            $(config.gridId).empty();
        }

        inyectarEstilosGrid();

        var url = config.urls.getTracking + "?controlDeBoletosId=" + encodeURIComponent(state.controlDeBoletosId);

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
                { field: "FechaHora",     title: "Fecha y Hora",  width: 150 },
                { field: "Accion",        title: "Acción",        width: 180 },
                { field: "Resultado",     title: "Resultado",     width: 180 },
                { field: "Apellido",      title: "Apellido",      width: 130 },
                { field: "Nombre",        title: "Nombre",        width: 130 },
            ],
            dataBound: function (e) {
                var data = e.sender.dataSource.data();
                if (data.length === 0) {
                    $(config.gridId).hide();
                    $("#lblNoTrackingInfo").show();
                } else {
                    $(config.gridId).show();
                    $("#lblNoTrackingInfo").hide();
                    autoFitColumnas(e.sender);
                }
            }
        }).data("kendoGrid");
    }

    function mostrarModal() {
        $(config.modalId).modal("show");
    }

    // ======================
    // API pública
    // ======================
    return {

        abrir: function (controlDeBoletosId) {
            state.controlDeBoletosId = controlDeBoletosId;

            var url = "/ControlDeBoletos/_TrackingControlDeBoletos?id=" + controlDeBoletosId;

            $(config.modalId).remove();

            $.get(url, function (html) {
                $("body").append(html);
                try {
                    inicializarGrid();
                    mostrarModal();
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
