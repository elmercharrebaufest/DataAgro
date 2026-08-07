// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    "use strict";

    var config = {
        urls: {
            getMateriales: "/ControlDeBoletos/GetMateriales",
            getProveedores: "/ControlDeBoletos/GetProveedores",
            getBolsaCompraNet: "/ControlDeBoletos/GetBolsaCompraNet",
            getEstados: "/ValidacionBoletoIA/GetEstadosValidacion",
            getValidacionBoletosPendientes: "/ValidacionBoletoIA/GetValidacionBoletosPendientes",
            exportBoletosExcel: "/ValidacionBoletoIA/ExportarBoletosValidadosExcel"        }
    };

    let controlMaterial = $("#frmValidacionBoletos #materialId");
    let controlEstadoValidacion = $("#frmValidacionBoletos #estadoValidacionId");
    let controlProveedor = $("#frmValidacionBoletos #proveedorId");
    let controlBolsaCompraNet = $("#frmValidacionBoletos #bolsaCompraNetId");

    let controlNegocioSAP = $("#frmValidacionBoletos #NegocioSAP");
    let controlFechaValidacionDesde = $("#frmValidacionBoletos #fechaValidacionDesde");
    let controlFechaValidacionHasta = $("#frmValidacionBoletos #fechaValidacionHasta");

    let controlFiltrarBoletos = $("#frmValidacionBoletos #filtrarBoletos");
    let controlLimpiarFiltros = $("#frmValidacionBoletos #limpiarFiltros");
    let controlExportarExcel = $("#frmValidacionBoletos #exportarExcel");
    let controlCargarBoleto = $("#frmValidacionBoletos #cargarBoleto");

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false,
        columnasAjustadas: false,
        columnWidths: [], // Guardar anchos para preservarlos en reordenamiento
    };

    // Funciones privadas
    function mostrarSpinner(mostrar) {
        var spinner = document.getElementById("loadingSpinner");
        if (spinner) {
            spinner.style.display = mostrar ? "flex" : "none";
        }
    }

    function mostrarMensaje(titulo, mensaje, tipo) {
        tipo = tipo || "info";
        $("#mensajeModalTitle").text(titulo);
        $("#mensajeModalBody").html(
            '<div class="alert alert-' + tipo + '">' + mensaje + "</div>",
        );
        $("#mensajeModal").modal("show");
    }

    function actualizarBoton(selector, habilitado, texto) {
        var btn = selector;
        btn.prop("disabled", !habilitado);
        if (texto) {
            btn.find("span").text(texto);
        }
    }

    async function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");

        try {
            var data = await MSExecuteGetOnServerAsync(url);
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

    // ── Estilos globales del grid ────────────────────────────────────────────

    function inyectarEstilosGrid() {
        if ($("#grid-boletos-styles").length) return;
        $("<style id='grid-boletos-styles'>").text(`
            #boletos-grid .k-grid-header th {
                font-weight: bold !important;
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
                white-space: nowrap;
                background-color: #f5f5f5;
            }
            #boletos-grid .k-grid-content td {
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
            }
            #boletos-grid .k-grid-header-wrap {
                overflow: hidden !important;
            }
            #boletos-grid .k-grid-content {
                overflow-x: auto !important;
                overflow-y: auto !important;
            }
            #boletos-grid .k-grid-header-wrap table,
            #boletos-grid .k-grid-content table {
                table-layout: fixed;
            }
            #boletos-grid .k-grid-content tr:hover td,
            #boletos-grid .k-grid-content tr.k-state-hover td {
                color: #333 !important;
            }
            #boletos-grid .k-grid-content tr.k-state-selected td {
                color: #333 !important;
            }
        `).appendTo("head");
    }

    // ── Auto-ajuste de columnas ──────────────────────────────────────────────

    var _canvas = document.createElement("canvas");
    function medirTexto(texto, fuente) {
        var ctx = _canvas.getContext("2d");
        ctx.font = fuente;
        return Math.ceil(ctx.measureText(texto).width);
    }

    // Extrae las columnas hoja (sin hijos) de una estructura multinivel
    function getLeafColumns(cols) {
        var result = [];
        (cols || []).forEach(function (col) {
            if (col.columns && col.columns.length) {
                getLeafColumns(col.columns).forEach(function (c) { result.push(c); });
            } else {
                result.push(col);
            }
        });
        return result;
    }

    function autoFitColumnas(grid) {
        var $wrapper = grid.element;
        var $headerCols = $wrapper.find(".k-grid-header-wrap colgroup col");
        var $contentCols = $wrapper.find(".k-grid-content   colgroup col");
        // tr:last selecciona la fila de cabeceras hoja (con cabeceras multinivel hay 2 filas)
        var $headerCells = $wrapper.find(".k-grid-header-wrap tr:last th");
        var $rows = $wrapper.find(".k-grid-content tbody tr");
        var columns = getLeafColumns(grid.columns); // columnas hoja aplanadas

        state.columnWidths = []; // Resetear anchos guardados

        $headerCells.each(function (colIdx) {
            var colDef = columns[colIdx];
            var hasField = colDef && colDef.field && colDef.field !== "Selected";

            if (!hasField) {
                var fixedW = (colDef && colDef.width) ? colDef.width : 50;
                $headerCols.eq(colIdx).css("width", fixedW + "px");
                $contentCols.eq(colIdx).css("width", fixedW + "px");
                state.columnWidths.push(fixedW + "px");
                return;
            }

            // Solo la columna Proveedor ajusta su ancho al contenido;
            // el resto conserva el ancho definido en la columna.
            var isProveedor = colDef && colDef.field === "Proveedor";

            if (!isProveedor) {
                var definedW = (colDef && colDef.width) ? colDef.width : 80;
                $headerCols.eq(colIdx).css("width", definedW + "px");
                $contentCols.eq(colIdx).css("width", definedW + "px");
                state.columnWidths.push(definedW + "px");
                return;
            }

            var headerText = $(this).find(".k-link").text().trim() || $(this).text().trim();
            var maxPx = medirTexto(headerText, "bold 13px Arial") + 32;

            $rows.each(function () {
                var cellPx = medirTexto($(this).find("td").eq(colIdx).text().trim(), "13px Arial") + 24;
                if (cellPx > maxPx) maxPx = cellPx;
            });

            maxPx = Math.max(maxPx, 60);
            $headerCols.eq(colIdx).css("width", maxPx + "px");
            $contentCols.eq(colIdx).css("width", maxPx + "px");
            state.columnWidths.push(maxPx + "px");
        });

        var totalWidth = Array.from($headerCols).reduce(function (sum, col) {
            return sum + (parseInt($(col).css("width")) || 0);
        }, 0);
        $wrapper.find(".k-grid-header-wrap table, .k-grid-content table").css("width", totalWidth + "px");
    }

    // ── Restaurar anchos de columnas ────────────────────────────────────────

    function restaurarAnchosColumnas(grid) {
        if (!state.columnWidths || state.columnWidths.length === 0) return;

        var $wrapper = grid.element;
        var $headerCols = $wrapper.find(".k-grid-header-wrap colgroup col");
        var $contentCols = $wrapper.find(".k-grid-content colgroup col");

        state.columnWidths.forEach(function (width, idx) {
            $headerCols.eq(idx).css("width", width);
            $contentCols.eq(idx).css("width", width);
        });

        var totalWidth = state.columnWidths.reduce(function (sum, widthStr) {
            return sum + (parseInt(widthStr) || 0);
        }, 0);
        $wrapper.find(".k-grid-header-wrap table, .k-grid-content table").css("width", totalWidth + "px");
    }

    // Funciones públicas
    return {
        init: async function () {
            if (state.datosInicializados) return;
            this.inicializarFechas();
            await this.cargarDatosIniciales();
            this.configurarEventos();
            this.inicializarGrid();

            state.datosInicializados = true;
        },
        cargarDatosIniciales: async function () {
            await Promise.all([
                cargarDropdown(
                    config.urls.getMateriales,
                    controlMaterial,
                    "Cargando...",
                    "Todos los materiales",
                ),
                cargarDropdown(
                    config.urls.getEstados,
                    controlEstadoValidacion,
                    "Cargando...",
                    "Todos los estados",
                ),
                cargarDropdown(
                    config.urls.getProveedores,
                    controlProveedor,
                    "Cargando...",
                    "Todos los proveedores",
                ),
                cargarDropdown(
                    config.urls.getBolsaCompraNet,
                    controlBolsaCompraNet,
                    "Cargando...",
                    "Todas las Bolsas",
                ),
            ]);
        },
        configurarEventos: function () {
            var self = this;

            controlNegocioSAP
                .on("paste", function (e) {

                    e.preventDefault();
                    controlEstadoValidacion.val("").trigger('change');
                    let texto = (e.originalEvent.clipboardData || window.clipboardData)
                        .getData("text");

                    // Separar por saltos de línea
                    let valores = texto
                        .split(/\r?\n/)           // soporta Excel / Windows / Linux
                        .map(v => v.trim())       // quitar espacios
                        .filter(v => v !== "");   // eliminar vacíos

                    // eliminar duplicados
                    valores = [...new Set(valores)];

                    // unir en una sola línea con ;
                    $(this).val(valores.join(";"));

                })
                .on("keypress", e => {
                    if (e.which === 32) e.preventDefault(); // bloquear espacios
                });

            // Eventos de botones
            controlFiltrarBoletos
                .off("click")
                .on("click", function () {
                    self.filtrarBoletos();
                });

            controlLimpiarFiltros
                .off("click")
                .on("click", function () {
                    self.limpiarFiltros();
                });

            controlExportarExcel
                .off("click")
                .on("click", function () {
                    self.exportarExcel();
                });

            controlCargarBoleto
                .off("click")
                .on("click", function () {
                    self.cargarBoleto();
                });
        },
        inicializarGrid: function () {
            var self = this;
            inyectarEstilosGrid();
            try {

                $("#boletos-grid").kendoTooltip({
                    filter: ".acciones-tooltip",
                    position: "top",
                    width: 400,
                    content: function (e) {
                        return e.target.data("texto");
                    }
                });

                state.grid = $("#boletos-grid")
                    .kendoGrid({
                        dataSource: {
                            type: "json",
                            serverPaging: true,
                            serverSorting: true,
                            serverFiltering: true,
                            pageSize: 20,
                            transport: {
                                read: function (options) {
                                    var data = options.data || {};
                                    var filtros = self.obtenerFiltros();
                                    var parametros = {
                                        // Opciones de Kendo (paginación, sorting)
                                        page: data.page || 1,
                                        pageSize: data.pageSize || 50,
                                        skip: data.skip || 0,
                                        take: data.take || 50,
                                        sort: data.sort || [],
                                        // Filtros personalizados
                                        negocioSAP: filtros.negocioSAP,
                                        materialId: filtros.materialId,
                                        estadoValidacionId: filtros.estadoValidacionId,
                                        fechaValidacionDesde: filtros.fechaValidacionDesde,
                                        fechaValidacionHasta: filtros.fechaValidacionHasta,
                                        proveedorId: filtros.proveedorId,
                                        bolsaId: filtros.bolsaId
                                    };

                                    MSExecuteOnServerAsync(config.urls.getValidacionBoletosPendientes, parametros)
                                        .then(function (response) {
                                            options.success(response || { Data: [], Total: 0 });
                                        })
                                        .catch(function (error) {
                                            options.error(error);
                                        });
                                },
                            },
                            schema: {
                                data: "Data",
                                total: "Total",
                                errors: "Errors",
                                model: {
                                    id: "Id",
                                    fields: {
                                        Id: { type: "number" },
                                        TipoBoleto: { type: "string" },
                                        ContratoSAP: { type: "string" },
                                        Version: { type: "string" },
                                        FechaGeneracion: { type: "date" },
                                        Proveedor: { type: "string" },
                                        Material: { type: "string" },
                                        Comercial: { type: "string" },
                                        BolsaCompraNet: { type: "string" },
                                        ValidacionBoletosEstado: { type: "string" },
                                        FechaValidacion: { type: "date" },
                                        FechaRechazo: { type: "date" },
                                    },
                                },
                            },
                            error: function (e) {
                                mostrarMensaje(
                                    "Error",
                                    "Error al cargar los datos: " +
                                    (e.errors || "Error desconocido"),
                                    "danger",
                                );
                                mostrarSpinner(false);
                            },
                            requestStart: function () {
                                mostrarSpinner(true);
                                state.cargandoDatos = true;
                            },
                            requestEnd: function () {
                                mostrarSpinner(false);
                                state.cargandoDatos = false;
                                actualizarBoton(controlExportarExcel, true);
                            },
                        },
                        height: 550,
                        scrollable: { virtual: false },
                        sortable: {
                            mode: "single",
                            allowUnsort: false,
                        },
                        filterable: false,
                        reorderable: true,
                        resizable: false,
                        columnReorder: function (e) {
                            // Recalcular anchos según el contenido en la nueva posición
                            setTimeout(function () {
                                autoFitColumnas(e.sender);
                            }, 0);
                        },
                        pageable: {
                            refresh: true,
                            buttonCount: 5,
                            messages: {
                                display: "Mostrando {0}-{1} de {2} registros",
                                empty: "No se encontraron registros",
                                page: "Página",
                                of: "de {0}",
                                itemsPerPage: "registros por página",
                                first: "Primera página",
                                last: "Última página",
                                next: "Página siguiente",
                                previous: "Página anterior",
                                refresh: "Actualizar",
                            },
                        },
                        navigatable: false,
                        columns: [
                            {
                                template: function (dataItem) {

                                    var color = "";

                                    switch (dataItem.EstadoValidacionAgente) {
                                        case "OK":
                                            color = "#28A745";
                                            break;
                                        case "RECHAZAR":
                                            color = "#DC3545";
                                            break;
                                        case "REVISAR":
                                            color = "#FFC107";
                                            break;
                                        case "DIFERENCIA":
                                            color = "#FD7E14";
                                            break;
                                    }

                                    return '<i class="fa fa-circle" style="color:' + color + ';font-size:16px;" ' +
                                        'title="' + dataItem.EstadoValidacionAgente + '"></i>';
                                },
                                attributes: {
                                    style: "text-align:center;"
                                }                                
                            },
                            {
                                title: "Información del Boleto",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "TipoBoleto",
                                        title: "Boleto",
                                        width: 100,
                                    },
                                    {
                                        field: "ContratoSAP",
                                        title: "Contrato SAP",
                                        width: 120,
                                        template: "<span class='font-weight-bold'>#=ContratoSAP#</span>",
                                    },
                                    {
                                        field: "Version",
                                        title: "Versión",
                                        width: 80,
                                    },
                                    {
                                        field: "FechaGeneracion",
                                        title: "Fecha Generación",
                                        width: 150,
                                        template: "#= formatearFecha(FechaGeneracion) #",
                                    },
                                ]
                            },
                            {
                                title: "Información Comercial",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "BolsaCompraNet",
                                        title: "Bolsa",
                                        width: 100,
                                    },
                                    {
                                        field: "Material",
                                        title: "Material",
                                        width: 120,
                                    },
                                    {
                                        field: "Proveedor",
                                        title: "Proveedor",
                                        width: 250,
                                    },
                                ]
                            },
                            {
                                title: "Revisión DataAgro",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "ValidacionBoletosEstado",
                                        title: "Estado Revisión",
                                        width: 150,
                                    },
                                    {
                                        field: "FechaValidacion",
                                        title: "Fecha Revisión",
                                        width: 120,
                                        template: "#= formatearFecha(FechaValidacion) #",
                                    },
                                    {
                                        field: "FechaRechazo",
                                        title: "Fecha Rechazo",
                                        width: 120,
                                        template: "#= formatearFecha(FechaRechazo) #",
                                    },
                                ]
                            },
                            {
                                title: "Validación Agente IA",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "EstadoValidacionAgente",
                                        title: "Estado Validación",
                                        width: 150,
                                    },
                                    {
                                        field: "AccionesRecomendadas",
                                        title: "Acciones Recomendadas",
                                        width: 180,
                                        template: function (dataItem) {

                                            var texto = dataItem.AccionesRecomendadas || "";

                                            return '<span class="acciones-tooltip" data-texto="' +
                                                kendo.htmlEncode(texto) + '">' +
                                                kendo.htmlEncode(texto.length > 20 ? texto.substring(0, 20) + "..." : texto) +
                                                '</span>';
                                        }
                                    },
                                ]
                            },
                            {
                                title: "Acciones",
                                width: 100,
                                template: function (dataItem) {
                                    return self.generarBotonesAccion(dataItem);
                                },
                                sortable: false,
                                filterable: false,
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                attributes: {
                                    style: "text-align:center;"
                                }
                            },
                        ],
                        dataBound: function (e) {
                            // Ejecutar autoFitColumnas solo en la primera carga
                            if (!state.columnasAjustadas) {
                                autoFitColumnas(e.sender);
                                state.columnasAjustadas = true;
                            }
                        },
                    })
                    .data("kendoGrid");

                // Ocultar spinner inicial después de crear el grid
                setTimeout(function () {
                    mostrarSpinner(false);
                }, 1000);
            } catch (error) {
                mostrarMensaje(
                    "Error",
                    "Error al inicializar la tabla de datos",
                    "danger",
                );
                mostrarSpinner(false);
            }
        },
        obtenerFiltros: function () {
            return {
                negocioSAP: controlNegocioSAP.val().trim() || null,
                materialId: controlMaterial.val() || null,
                estadoValidacionId: controlEstadoValidacion.val() || null,
                fechaValidacionDesde: (function () { var dp = controlFechaValidacionDesde.data("kendoDatePicker"); var v = dp ? dp.value() : null; return v ? v.toISOString() : null; })(),
                fechaValidacionHasta: (function () { var dp = controlFechaValidacionHasta.data("kendoDatePicker"); var v = dp ? dp.value() : null; return v ? v.toISOString() : null; })(),
                proveedorId: controlProveedor.val() || null,
                bolsaId: controlBolsaCompraNet.val() || null,
            };
        },
        validarFiltros: function () {
            var filtros = this.obtenerFiltros();

            var sinFiltrosPrincipales =
                !filtros.negocioSAP &&
                !filtros.fechaValidacionDesde &&
                !filtros.fechaValidacionHasta;

            var sinFiltrosSecundarios =
                !filtros.proveedorId &&
                !filtros.materialId &&
                !filtros.bolsaId;

            return !(sinFiltrosPrincipales && sinFiltrosSecundarios);
        },
        filtrarBoletos: function () {
            if (!this.validarFiltros()) {
                mostrarMensaje("Error de Validación", "No se ha seleccionado ningún filtro para la búsqueda.", "warning");
                return;
            }

            // Resetear ajuste de columnas para que se recalcule con nuevo contenido
            state.columnasAjustadas = false;

            if (state.grid) {
                // Si el grid ya existe, solo recargar los datos
                state.grid.dataSource.page(1); // Volver a la página 1
                state.grid.dataSource.read(); // Recargar datos con los nuevos filtros
            } else {
                // Si el grid no existe, inicializarlo
                this.inicializarGrid();
            }
        },
        limpiarFiltros: function () {
            controlNegocioSAP.val("");
            controlMaterial.val("");
            controlEstadoValidacion.val("");
            controlProveedor.val("");
            controlBolsaCompraNet.val("");

            var dpDesde = controlFechaValidacionDesde.data("kendoDatePicker");
            var dpHasta = controlFechaValidacionHasta.data("kendoDatePicker");
            if (dpDesde) dpDesde.value(null);
            if (dpHasta) dpHasta.value(null);

            controlEsConfirma.prop("checked", false);

            // Resetear ajuste de columnas para recalcular con nuevo contenido
            state.columnasAjustadas = false;

            this.filtrarBoletos();
        },
        exportarExcel: function () {
            var filtros = this.obtenerFiltros();
            var form = document.createElement("form");
            form.method = "POST";
            form.action = config.urls.exportBoletosExcel;
            form.style.display = "none";

            for (var key in filtros) {
                if (filtros.hasOwnProperty(key)) {
                    var input = document.createElement("input");
                    input.type = "hidden";
                    input.name = key;
                    input.value = filtros[key] || "";
                    form.appendChild(input);
                }
            }
            document.body.appendChild(form);
            form.submit();
            document.body.removeChild(form);
        },
        cargarBoleto: function () {
            if (
                typeof window.ValidacionBoletosCargarContrato !== "undefined" &&
                window.ValidacionBoletosCargarContrato &&
                typeof window.ValidacionBoletosCargarContrato.abrir === "function"
            ) {
                window.ValidacionBoletosCargarContrato.abrir();
                return;
            }

            console.error("ValidacionBoletosCargarContrato no está cargado.");
            mostrarMensaje(
                "Error",
                "No se pudo abrir el formulario porque el script de carga no está disponible. Recargue la página (Ctrl+F5).",
                "danger",
            );
        },
        inicializarFechas: function () {
            const hoy = new Date();
            const desde = new Date(hoy);
            desde.setDate(hoy.getDate() - 60);
            [
                { $el: controlFechaValidacionDesde, value: desde },
                { $el: controlFechaValidacionHasta, value: hoy }
            ].forEach(function ({ $el, value }) {
                if ($el.data("kendoDatePicker")) {
                    $el.data("kendoDatePicker").destroy();
                }
                $el.kendoDatePicker({
                    weekNumber: true,
                    format: "dd/MM/yyyy",
                    value: value
                });
            });
        },
        generarBotonesAccion: function (data) {

            var botones = [];
            botones.push(`
                <button class="btn btn-sm btn-outline-secondary btn-acciones tooltip-custom"
                    onclick="ValidacionBoletosResultados.abrir(${data.Id}, '${data.ContratoSAP}', '${data.BolsaCompraNet}', '${data.Material}', '${data.Proveedor}', '${data.EstadoValidacionAgente}', '${data.AccionesRecomendadas}', '${formatearFecha(data.FechaValidacion)}')"
                    title="Visualizar validación del boleto">
                    <i class="fa fa-eye"></i>
                    <span class="tooltiptext"></span>
                </button>
            `);

            return (
                '<div class="btn-group" role="group">' + botones.join(" ") + "</div>"
            );
        }
    };
})();

// Función global para formatear fechas (necesaria para el template del grid)
function formatearFecha(fecha) {
    if (!fecha) return "";
    var date = new Date(fecha);
    return date.toLocaleDateString("es-AR");
}
// Inicializar cuando el DOM esté listo
$(document).ready(function () {
    ControlBoletos.init().catch(function (error) {
        console.error("Error inicializando Control de Boletos:", error);
        alert("Error al inicializar la aplicación. Por favor, recargue la página.");
    });
});

// Manejar errores globales
window.addEventListener("error", function (e) {
    console.error("Error global:", e.error);
});
