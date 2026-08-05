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
            exportBoletosExcel: "/ValidacionBoletoIA/ExportarBoletosValidadosExcel",
            cargarBoletosValidacion: "/ValidacionBoletoIA/CargarBoletosValidacion",
        }
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
                            pageSizes: [25, 50, 100, 200],
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
                                title: "Información del Boleto",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "TipoBoleto",
                                        title: "Boleto",
                                        width: 80,
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
                                        width: 140,
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
                                title: "Validación",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "ValidacionBoletosEstado",
                                        title: "Estado Validación",
                                        width: 135,
                                    },
                                    {
                                        field: "FechaValidacion",
                                        title: "Fecha Validación",
                                        width: 130,
                                        template: "#= formatearFecha(FechaValidacion) #",
                                    },
                                ]
                            },
                            {
                                title: "Acciones",
                                width: 80,
                                template: function (dataItem) {
                                    return self.generarBotonesAccion(dataItem);
                                },
                                sortable: false,
                                filterable: false,
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                }
                            },
                        ],
                        dataBound: function (e) {
                            // Ejecutar autoFitColumnas solo en la primera carga
                            if (!state.columnasAjustadas) {
                                autoFitColumnas(e.sender);
                                state.columnasAjustadas = true;
                            }
                            self.configurarEventosGrid();
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

        configurarEventosGrid: function () {
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
                    title="Visualizar Contrato">
                    <i class="fa fa-info-circle"></i>
                    <span class="tooltiptext"></span>
                </button>
            `);

            return (
                '<div class="btn-group" role="group">' + botones.join(" ") + "</div>"
            );
        },

        visualizarContrato: async function (id) {
            // Mostrar loader o indicador de carga

            if (!id || id <= 0) {
                alert("Error: ID de contrato inválido");
                return;
            }

            var loadingMessage = "Cargando datos del contrato...";
            if (typeof showLoading === "function") {
                showLoading(loadingMessage);
            }

            try {
                var response = await MSExecuteGetOnServerAsync(config.urls.getContrato, { id: id });

                if (!response) {
                    alert("Error al obtener los datos del contrato: respuesta vacía");
                    return;
                }

                // Normalizar la respuesta - el backend devuelve { Data: [objetos], Total: n }
                var contrato = null;

                if (response.Data !== undefined) {
                    if (Array.isArray(response.Data)) {
                        // Es un array, tomar el primer elemento
                        contrato = response.Data.length > 0 ? response.Data[0] : null;
                    } else if (
                        typeof response.Data === "object" &&
                        response.Data !== null
                    ) {
                        // Es un objeto, usar directamente
                        contrato = response.Data;
                    }
                } else {
                    // La respuesta directamente es el contrato
                    contrato = response;
                }

                if (contrato && typeof contrato === "object") {
                    // Verificar que ModalVisualizar esté disponible
                    if (
                        typeof ModalVisualizar !== "undefined" &&
                        typeof ModalVisualizar.abrir === "function"
                    ) {
                        ModalVisualizar.abrir(contrato);
                    } else {
                        console.error("ModalVisualizar no está disponible");
                        alert("Error: El módulo de visualización no está cargado");
                    }
                } else {
                    console.error("No se pudo extraer el contrato de la respuesta");
                    alert("No se encontró el contrato solicitado.");
                }
            } catch (error) {
                console.error("Error al obtener detalle del contrato:", error);

                var mensajeError = "Error al comunicarse con el servidor.";
                var status = error && error.xhr ? error.xhr.status : 0;
                if (status === 404) {
                    mensajeError = "No se encontró el contrato solicitado.";
                } else if (status === 500) {
                    mensajeError = "Error interno del servidor.";
                }

                alert(mensajeError + " Por favor, intente nuevamente.");
            } finally {
                if (typeof hideLoading === "function") {
                    hideLoading();
                }
            }
        },
        descargarPDFConfirma: async function (id) {

            try {

                await MSDownloadFileAsync(
                    config.urls.getDocumentoConfirmaPDF,
                    { controlDeBoletoId: id }
                );
            } catch (error) {
                console.error(error);
            }
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
