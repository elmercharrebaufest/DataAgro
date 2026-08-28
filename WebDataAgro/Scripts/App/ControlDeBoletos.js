// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    "use strict";

    var ui = window.ControlDeBoletosUI;

    var config = {
        urls: {
            getMateriales: "/ControlDeBoletos/GetMateriales",
            getEstados: "/ControlDeBoletos/GetEstadosControl",
            getComerciales: "/ControlDeBoletos/GetComerciales",
            getProveedores: "/ControlDeBoletos/GetProveedores",
            getBolsaCompraNet: "/ControlDeBoletos/GetBolsaCompraNet",
            getBoletos: "/ControlDeBoletos/GetBoletos",
            getContrato: "/ControlDeBoletos/ObtenerDetalleContrato",
            getDocumentoConfirmaPDF: "/ControlDeBoletos/GetDocumentoConfirmaPDF",
            exportBoletosExcel: "/ControlDeBoletos/ExportarBoletosExcel",
        }
    };

    let controlMaterial = $("#frmPendienteControl #materialId");
    let controlEstadoControl = $("#frmPendienteControl #estadoControlId");
    let controlComercial = $("#frmPendienteControl #comercialId");
    let controlProveedor = $("#frmPendienteControl #proveedorId");
    let controlBolsaCompraNet = $("#frmPendienteControl #bolsaCompraNetId");

    let controlNegocioSAP = $("#frmPendienteControl #NegocioSAP");
    let controlFechaCargaDesde = $("#frmPendienteControl #fechaCargaDesde");
    let controlFechaCargaHasta = $("#frmPendienteControl #fechaCargaHasta");

    let controlEsConfirma = $("#frmPendienteControl #esConfirma");
    let controlEsBoletoFisico = $("#frmPendienteControl #esBoletoFisico");
    let controlEsCartaOferta = $("#frmPendienteControl #esCartaOferta");
    let controlEsNinguno = $("#frmPendienteControl #esNinguno");
    let controlEsSinBoleto = $("#frmPendienteControl #esSinBoleto");

    let controlFiltrarBoletos = $("#frmPendienteControl #filtrarBoletos");
    let controlLimpiarFiltros = $("#frmPendienteControl #limpiarFiltros");
    let controlExportarExcel = $("#frmPendienteControl #exportarExcel");

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false,
        columnasAjustadas: false,
        columnWidths: [], // Guardar anchos para preservarlos en reordenamiento
    };

    // Funciones privadas
    function mostrarSpinner(mostrar) {
        ui.toggleSpinner(mostrar);
    }

    function mostrarMensaje(titulo, mensaje, tipo) {
        ui.showModalMessage(titulo, mensaje, tipo);
    }

    function actualizarBoton(selector, habilitado, texto) {
        ui.setButtonState(selector, habilitado, texto);
    }

    async function cargarDropdown(url, selector, textoCarga, textoDefault) {
        return ui.loadDropdown(url, selector, textoCarga, textoDefault);
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

    function obtenerColumnasHoja(columns, result) {
        result = result || [];
        (columns || []).forEach(function (col) {
            if (col && Array.isArray(col.columns) && col.columns.length > 0) {
                obtenerColumnasHoja(col.columns, result);
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
        var $headerCells = $wrapper.find(".k-grid-header-wrap th[data-field]");
        var $rows = $wrapper.find(".k-grid-content tbody tr");
        var leafColumns = obtenerColumnasHoja(grid.columns);
        var autoFitPorContenido = {
            Proveedor: true,
            Comercial: true,
        };

        state.columnWidths = []; // Resetear anchos guardados

        $headerCells.each(function (colIdx) {
            var colDef = leafColumns[colIdx] || {};
            var field = colDef.field || $(this).attr("data-field");
            var headerText = $(this).find(".k-link").text().trim() || $(this).text().trim();

            var widthDefinido = parseInt(colDef.width, 10);
            var widthTitulo = medirTexto(headerText, "bold 13px Arial") + 36;
            var maxPx = Math.max(isNaN(widthDefinido) ? 0 : widthDefinido, widthTitulo, 60);

            if (autoFitPorContenido[field]) {
                $rows.each(function () {
                    var cellPx = medirTexto($(this).find("td").eq(colIdx).text().trim(), "13px Arial") + 24;
                    if (cellPx > maxPx) maxPx = cellPx;
                });
            }

            $headerCols.eq(colIdx).css("width", maxPx + "px");
            $contentCols.eq(colIdx).css("width", maxPx + "px");
            state.columnWidths.push(maxPx + "px");
        });

        var totalWidth = Array.from($headerCols).reduce(function (sum, col) {
            return sum + (parseInt($(col).css("width")) || 0);
        }, 0);
        $wrapper.find(".k-grid-header-wrap table, .k-grid-content table").css("width", totalWidth + "px");
    }

    // Funciones públicas
    return {
        init: async function () {
            if (state.datosInicializados) return;

            controlEsConfirma.prop('checked', true);
            controlEsBoletoFisico.prop('checked', true);
            controlEsCartaOferta.prop('checked', true);
            controlEsSinBoleto.prop('checked', false);
            controlEsNinguno.prop('checked', false);

            this.inicializarFechas();
            await this.cargarDatosIniciales();
            controlEstadoControl.val(1).trigger('change');
            this.configurarEventos();
            this.inicializarGrid();
            state.datosInicializados = true;
            controlEsConfirma.prop('checked', true);
            controlEsBoletoFisico.prop('checked', true);
            controlEsCartaOferta.prop('checked', true);
            controlEsSinBoleto.prop('checked', false);
            controlEsNinguno.prop('checked', false);

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
                    controlEstadoControl,
                    "Cargando...",
                    "Todos los estados",
                ),
                cargarDropdown(
                    config.urls.getComerciales,
                    controlComercial,
                    "Cargando...",
                    "Todos los comerciales",
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
                .off("paste keypress")
                .each(function () {
                    ui.bindContractsPaste($(this), function () {
                        controlEstadoControl.val("").trigger("change");
                    });
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
                            pageSize: 10,
                            transport: {
                                read: function (options) {
                                    var data = options.data || {};
                                    var filtros = self.obtenerFiltros();
                                    var parametros = {
                                        // Opciones de Kendo (paginación, sorting)
                                        page: data.page || 1,
                                        pageSize: data.pageSize || 10,
                                        skip: data.skip || 0,
                                        take: data.take || 10,
                                        sort: data.sort || [],
                                        // Filtros personalizados
                                        negocioSAP: filtros.negocioSAP,
                                        materialId: filtros.materialId,
                                        estadoControlId: filtros.estadoControlId,
                                        esConfirma: filtros.esConfirma,
                                        esBoletoFisico: filtros.esBoletoFisico,
                                        esCartaOferta: filtros.esCartaOferta,
                                        esSinBoleto: filtros.esSinBoleto,
                                        esNinguno: filtros.esNinguno,
                                        fechaCargaDesde: filtros.fechaCargaDesde,
                                        fechaCargaHasta: filtros.fechaCargaHasta,
                                        proveedor: filtros.proveedor,
                                        bolsaId: filtros.bolsaId,
                                        comercialId: filtros.comercialId,
                                    };

                                    MSExecuteOnServerAsync(config.urls.getBoletos, parametros)
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
                                        ContratoSAP: { type: "string" },
                                        Material: { type: "string" },
                                        ControlDeBoletosEstado: { type: "string" },
                                        FechaGeneracion: {type: "date"},
                                        Proveedor: { type: "string" },
                                        Comercial: { type: "string" },
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
                        height: 450,
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
                                        field: "ControlDeBoletosEstado",
                                        title: "Estado Control Boleto",
                                        width: 200,
                                    },                                    
                                    {
                                        field: "TipoAltaConfirma",
                                        title: "Tipo Alta Confirma",
                                        width: 150
                                    },
                                    {
                                        field: "BolsaCompraNet",
                                        title: "Bolsa",
                                        width: 100  
                                    },
                                    {
                                        field: "Material",
                                        title: "Material",
                                        width: 80,
                                    },
                                    {
                                        field: "ContratoSAP",
                                        title: "Contrato SAP",
                                        width: 120,
                                        template:
                                            "<span class='font-weight-bold'>#=ContratoSAP#</span>",
                                    },
                                    {
                                        field: "Version",
                                        title: "Version",
                                        width: 80
                                    },
                                    {
                                        field: "FechaGeneracion",
                                        title: "Fecha Generación",
                                        width: 130,
                                        format: "{0:dd/MM/yyyy}",
                                        template: "#= formatearFecha(FechaGeneracion) #",
                                    },
                                    {
                                        field: "EstadoVersion",
                                        title: "Estado Versión",
                                        width: 120
                                    },
                                ]
                            },
                            {
                                title: "Datos Comerciales",
                                headerAttributes: {
                                    style: "text-align:center;font-weight:bold;"
                                },
                                columns: [
                                    {
                                        field: "Proveedor",
                                        title: "Proveedor",
                                        width: 250,
                                    },
                                    {
                                        field: "Comercial",
                                        title: "Comercial",
                                        width: 150,
                                    },
                                ]
                            },
                            {
                                field: "EstadoConfirma",
                                title: "Estado en Web Confirma",
                                width: 200,
                            },
                            {
                                title: "Acciones",
                                width: 100,
                                template: function (dataItem) {
                                    return self.generarBotonesAccion(dataItem);
                                },
                                sortable: false,
                                filterable: false,
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
                estadoControlId: controlEstadoControl.val() || null,
                esConfirma: controlEsConfirma.is(":checked"),
                esBoletoFisico: controlEsBoletoFisico.is(":checked"),
                esCartaOferta: controlEsCartaOferta.is(":checked"),
                esSinBoleto: controlEsSinBoleto.is(":checked"),
                esNinguno: controlEsNinguno.is(":checked"),
                fechaCargaDesde: (function() { var dp = controlFechaCargaDesde.data("kendoDatePicker"); var v = dp ? dp.value() : null; return v ? v.toISOString() : null; })(),
                fechaCargaHasta: (function() { var dp = controlFechaCargaHasta.data("kendoDatePicker"); var v = dp ? dp.value() : null; return v ? v.toISOString() : null; })(),
                proveedor: controlProveedor.val() || null,
                bolsaId: controlBolsaCompraNet.val() || null,
                comercialId: controlComercial.val() || null,
            };
        },

        validarFiltros: function () {
            var filtros = this.obtenerFiltros();

            var sinFiltrosPrincipales =
                !filtros.negocioSAP &&
                !filtros.fechaCargaDesde &&
                !filtros.fechaCargaHasta;

            var sinFiltrosSecundarios =
                !filtros.proveedor &&
                !filtros.comercialId &&
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
                if (state.grid.dataSource.page() !== 1) {
                    state.grid.dataSource.page(1); // Volver a la página 1 (ya dispara read)
                } else {
                    state.grid.dataSource.read(); // Si ya está en página 1, forzar recarga
                }
            } else {
                // Si el grid no existe, inicializarlo
                this.inicializarGrid();
            }
        },

        limpiarFiltros: function () {
            controlNegocioSAP.val("");
            controlMaterial.val("");
            controlEstadoControl.val("");
            controlComercial.val("");
            controlProveedor.val("");
            controlBolsaCompraNet.val("");

            var dpDesde = controlFechaCargaDesde.data("kendoDatePicker");
            var dpHasta = controlFechaCargaHasta.data("kendoDatePicker");
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

        inicializarFechas: function () {
            const hoy = new Date();
            const desde = new Date(hoy);
            desde.setDate(hoy.getDate() - 60);
            [
                { $el: controlFechaCargaDesde, value: desde },
                { $el: controlFechaCargaHasta, value: hoy }
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
            botones.push(
                '<button class="btn btn-sm btn-outline-secondary btn-acciones tooltip-custom" onclick="ControlBoletos.visualizarContrato(' +
                data.NegocioId +
                ')" title="Visualizar Contrato"><i class="fa fa-file-text-o"></i><span class="tooltiptext"></span></button>',
            );
            var esValidoParaControl = true;
            if (data.TipoBoleto == 'Confirma' || data.TipoBoleto == 'Físico' || data.TipoBoleto == 'Carta Oferta') {
                esValidoParaControl = data.EstadoVersion == "Vigente";
            }
            
            if (esValidoParaControl) {
                botones.push(
                    '<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlDeBoletosGestion.abrir({ControlDeBoletosId:' +
                    data.Id + ',SeguimientoBoletoId:' + (data.SeguimientoBoletoId || 0) +
                    ',PreCertificacionId:' + (data.PreCertificacionId || 0) +
                    ',NegocioId:' + data.NegocioId +
                    '})" title="Gestión de control de boletos"><i class="fa fa-tasks"></i><span class="tooltiptext"></span></button>',
                );
            }

            if (data.EsConfirma && data.TipoAltaConfirma == 'Alta Definitiva') {

                botones.push(
                    '<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlBoletosTracking.abrir(' +
                    data.Id +
                    ')" title="Tracking Boleto"><i class="fa fa-history"></i><span class="tooltiptext"></span></button>',
                );
                // botones.push(
                //     '<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlBoletos.descargarPDFConfirma(' +
                //     data.Id +
                //     ')" title="Descargar PDF Confirma"><i class="fa fa-file-pdf-o"></i><span class="tooltiptext"></span></button>',
                // );
            }
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
