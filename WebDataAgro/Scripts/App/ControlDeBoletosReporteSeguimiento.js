
// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletosReporteSeguimiento = (function () {
    "use strict";

    var config = {
        urls: {
            getMateriales: "/ControlDeBoletos/GetMateriales",
            getProveedores: "/ControlDeBoletos/GetProveedores",
            getBolsaCompraNet: "/ControlDeBoletos/GetBolsaCompraNet",
            getReporteSeguimiento: "/ControlDeBoletos/GetReporteDeSeguimientoBoletos",
            getContrato: "/ControlDeBoletos/ObtenerDetalleContrato",
            exportBoletosExcel: "/ControlDeBoletos/ExportarReporteDeSeguimientoBoletosExcel"
        }
    };

    // Controles — se inicializan en bindControls() dentro de init()
    var controlMaterial;
    var controlProveedor;
    var controlBolsaCompraNet;
    var controlNegocioSAP;
    var controlFechaCertificacionDesde;
    var controlFechaCertificacionHasta;
    var controlFechaVencimientoCertificacionDesde;
    var controlFechaVencimientoCertificacionHasta;
    var controlFechaRecepBoletoDesde;
    var controlFechaRecepBoletoHasta;
    var controlFechaEnviadoFirmaDesde;
    var controlFechaEnviadoFirmaHasta;
    var controlFechaRecibFirmaDesde;
    var controlFechaRecibFirmaHasta;
    var controlFechaEnvioBolsaDesde;
    var controlFechaEnvioBolsaHasta;
    var controlFechaVueltaBolsaDesde;
    var controlFechaVueltaBolsaHasta;
    var controlFechaEnvioAfipDesde;
    var controlFechaEnvioAfipHasta;
    var controlFechaVueltaAfipDesde;
    var controlFechaVueltaAfipHasta;
    var controlFiltrarBoletos;
    var controlLimpiarFiltros;
    var controlExportarExcel;

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false,
    };

    // Cachea todos los controles del formulario (debe llamarse cuando el DOM ya existe)
    function inicializarFechas() {
        [
            controlFechaCertificacionDesde, controlFechaCertificacionHasta, controlFechaVencimientoCertificacionDesde,
            controlFechaVencimientoCertificacionHasta, controlFechaRecepBoletoDesde, controlFechaRecepBoletoHasta,
            controlFechaEnviadoFirmaDesde, controlFechaEnviadoFirmaHasta, controlFechaRecibFirmaDesde,
            controlFechaRecibFirmaHasta, controlFechaEnvioBolsaDesde, controlFechaEnvioBolsaHasta,
            controlFechaVueltaBolsaDesde, controlFechaVueltaBolsaHasta, controlFechaEnvioAfipDesde,
            controlFechaEnvioAfipHasta, controlFechaVueltaAfipDesde, controlFechaVueltaAfipHasta
        ].forEach(function ($el) {
            if ($el.data("kendoDatePicker")) $el.data("kendoDatePicker").destroy();
            $el.kendoDatePicker({ weekNumber: true, format: "dd/MM/yyyy", value: null });
        });
    }
    function bindControls() {
        var $form = $("#frmReporteSeguimientoBoletos");

        controlMaterial         = $form.find("#materialId");
        controlProveedor        = $form.find("#proveedorId");
        controlBolsaCompraNet   = $form.find("#bolsaCompraNetId");
        controlNegocioSAP       = $form.find("#NegocioSAP");

        controlFechaCertificacionDesde              = $form.find("#fechaCertificacionDesde");
        controlFechaCertificacionHasta              = $form.find("#fechaCertificacionHasta");
        controlFechaVencimientoCertificacionDesde   = $form.find("#fechaVencimientoCertificacionDesde");
        controlFechaVencimientoCertificacionHasta   = $form.find("#fechaVencimientoCertificacionHasta");
        controlFechaRecepBoletoDesde                = $form.find("#fechaRecepBoletoDesde");
        controlFechaRecepBoletoHasta                = $form.find("#fechaRecepBoletoHasta");
        controlFechaEnviadoFirmaDesde               = $form.find("#fechaEnviadoFirmaDesde");
        controlFechaEnviadoFirmaHasta               = $form.find("#fechaEnviadoFirmaHasta");
        controlFechaRecibFirmaDesde                 = $form.find("#fechaRecibFirmaDesde");
        controlFechaRecibFirmaHasta                 = $form.find("#fechaRecibFirmaHasta");
        controlFechaEnvioBolsaDesde                 = $form.find("#fechaEnvioBolsaDesde");
        controlFechaEnvioBolsaHasta                 = $form.find("#fechaEnvioBolsaHasta");
        controlFechaVueltaBolsaDesde                = $form.find("#fechaVueltaBolsaDesde");
        controlFechaVueltaBolsaHasta                = $form.find("#fechaVueltaBolsaHasta");
        controlFechaEnvioAfipDesde                  = $form.find("#fechaEnvioAfipDesde");
        controlFechaEnvioAfipHasta                  = $form.find("#fechaEnvioAfipHasta");
        controlFechaVueltaAfipDesde                 = $form.find("#fechaVueltaAfipDesde");
        controlFechaVueltaAfipHasta                 = $form.find("#fechaVueltaAfipHasta");

        controlFiltrarBoletos   = $form.find("#filtrarBoletos");
        controlLimpiarFiltros   = $form.find("#limpiarFiltros");
        controlExportarExcel    = $form.find("#exportarExcel");
    }

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

    function validarNumero(valor) {
        return valor === "" || (!isNaN(valor) && parseInt(valor) >= 0);
    }

    function esNegocioSAPValido() {
        const val = controlNegocioSAP.val()?.trim();
        return !val || /^[0-9;]+$/.test(val);
    }

    function cargarDropdown(url, $select, textoCarga, textoDefault) {
        $select.html('<option value="">' + textoCarga + "</option>");
        return MSExecuteGetOnServerAsync(url)
            .then(function (data) {
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
            })
            .catch(function (error) {
                console.error("Error cargando dropdown:", error);
                $select.html('<option value="">Error al cargar datos</option>');
            });
    }

    // ── Estilos globales del grid ────────────────────────────────────────────

    function inyectarEstilosGrid() {
        if ($("#grid-reporte-seguimiento-styles").length) return;
        $("<style id='grid-reporte-seguimiento-styles'>").text(
            "#boletos-grid .k-grid-header th {" +
            "  font-weight: bold !important; font-size: 13px !important;" +
            "  font-family: Arial, sans-serif !important; white-space: nowrap; background-color: #f5f5f5; }" +
            "#boletos-grid .k-grid-content td {" +
            "  font-size: 13px !important; font-family: Arial, sans-serif !important; }" +
            "#boletos-grid .k-grid-header-wrap { overflow: hidden !important; }" +
            "#boletos-grid .k-grid-content { overflow-x: auto !important; overflow-y: auto !important; }" +
            "#boletos-grid .k-grid-header-wrap table, #boletos-grid .k-grid-content table { table-layout: fixed; }" +
            "#boletos-grid .k-grid-content tr:hover td, #boletos-grid .k-grid-content tr.k-state-hover td { color: #333 !important; }" +
            "#boletos-grid .k-grid-content tr.k-state-selected td { color: #333 !important; }"
        ).appendTo("head");
    }

    // ── Auto-ajuste de columnas ──────────────────────────────────────────────

    var _canvas = document.createElement("canvas");

    function medirTexto(texto, fuente) {
        var ctx = _canvas.getContext("2d");
        ctx.font = fuente;
        return Math.ceil(ctx.measureText(texto).width);
    }

    function autoFitColumnas(grid) {
        var $wrapper     = grid.element;
        var $headerCols  = $wrapper.find(".k-grid-header-wrap colgroup col");
        var $contentCols = $wrapper.find(".k-grid-content colgroup col");
        var $headerCells = $wrapper.find(".k-grid-header-wrap tr:first th");
        var $rows        = $wrapper.find(".k-grid-content tbody tr");
        var columns      = grid.columns;

        $headerCells.each(function (colIdx) {
            var colDef   = columns[colIdx];
            var hasField = colDef && colDef.field;

            if (!hasField) {
                var fixedW = (colDef && colDef.width) ? colDef.width : 50;
                $headerCols.eq(colIdx).css("width", fixedW + "px");
                $contentCols.eq(colIdx).css("width", fixedW + "px");
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
        });

        var totalWidth = Array.from($headerCols).reduce(function (sum, col) {
            return sum + (parseInt($(col).css("width")) || 0);
        }, 0);
        $wrapper.find(".k-grid-header-wrap table, .k-grid-content table").css("width", totalWidth + "px");
    }

    // ── API pública ──────────────────────────────────────────────────────────

    return {
        init: function () {
            if (state.datosInicializados) return;

            bindControls();
            inyectarEstilosGrid();
            inicializarFechas();
            var self = this;
            this.cargarDatosIniciales().then(function () {
                self.configurarEventos();
                self.inicializarGrid();
                state.datosInicializados = true;
            });
        },

        cargarDatosIniciales: function () {
            return Promise.all([
                cargarDropdown(config.urls.getMateriales,    controlMaterial,       "Cargando...", "Todos los materiales"),
                cargarDropdown(config.urls.getProveedores,   controlProveedor,      "Cargando...", "Todos los proveedores"),
                cargarDropdown(config.urls.getBolsaCompraNet, controlBolsaCompraNet, "Cargando...", "Todas las Bolsas"),
            ]);
        },

        configurarEventos: function () {
            var self = this;

            controlNegocioSAP
                .on("paste", function (e) {

                    e.preventDefault();

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
        },

        inicializarGrid: function () {
            var self = this;

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
                                        page:     data.page     || 1,
                                        pageSize: data.pageSize || 50,
                                        skip:     data.skip     || 0,
                                        take:     data.take     || 50,
                                        sort:     data.sort     || [],
                                        contratoSAP:                         filtros.contratoSAP,
                                        materialId:                          filtros.materialId,
                                        bolsaId:                             filtros.bolsaId,
                                        proveedor:                           filtros.proveedor,
                                        fechaCertificacionDesde:             filtros.fechaCertificacionDesde,
                                        fechaCertificacionHasta:             filtros.fechaCertificacionHasta,
                                        fechaVencimientoCertificacionDesde:  filtros.fechaVencimientoCertificacionDesde,
                                        fechaVencimientoCertificacionHasta:  filtros.fechaVencimientoCertificacionHasta,
                                        fechaRecepBoletoDesde:               filtros.fechaRecepBoletoDesde,
                                        fechaRecepBoletoHasta:               filtros.fechaRecepBoletoHasta,
                                        fechaEnviadoFirmaDesde:              filtros.fechaEnviadoFirmaDesde,
                                        fechaEnviadoFirmaHasta:              filtros.fechaEnviadoFirmaHasta,
                                        fechaRecibFirmaDesde:                filtros.fechaRecibFirmaDesde,
                                        fechaRecibFirmaHasta:                filtros.fechaRecibFirmaHasta,
                                        fechaEnvioBolsaDesde:                filtros.fechaEnvioBolsaDesde,
                                        fechaEnvioBolsaHasta:                filtros.fechaEnvioBolsaHasta,
                                        fechaVueltaBolsaDesde:               filtros.fechaVueltaBolsaDesde,
                                        fechaVueltaBolsaHasta:               filtros.fechaVueltaBolsaHasta,
                                        fechaEnvioAfipDesde:                 filtros.fechaEnvioAfipDesde,
                                        fechaEnvioAfipHasta:                 filtros.fechaEnvioAfipHasta,
                                        fechaVueltaAfipDesde:                filtros.fechaVueltaAfipDesde,
                                        fechaVueltaAfipHasta:                filtros.fechaVueltaAfipHasta,
                                    };

                                    MSExecuteOnServerAsync(config.urls.getReporteSeguimiento, parametros)
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
                                        FechaCreacion: { type: "date" },
                                        Proveedor: { type: "string" },
                                        Comercial: { type: "string" },
                                    },
                                },
                            },
                            error: function (e) {
                                console.error("Error cargando datos del grid:", e);
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
                        sortable: {
                            mode: "single",
                            allowUnsort: false,
                        },
                        filterable: false,
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
                        scrollable: { virtual: false },
                        navigatable: false,
                        columns: [
                            {
                                field: "TipoBoleto",
                                title: "Boleto",
                                width: 80,
                            },
                            {
                                field: "BolsaCompraNet",
                                title: "Bolsa",
                                width: 120
                            },
                            {
                                field: "ContratoSAP",
                                title: "Contrato SAP",
                                width: 120,
                                template:
                                    "<span class='font-weight-bold'>#=ContratoSAP#</span>",
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
                            {
                                field: "FechaCertificacion",
                                title: "Fecha Certificación",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaCertificacion) #",
                            },
                            {
                                field: "FechaVencimientoCertificacion",
                                title: "Fecha Vencimiento Certificación",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVencimientoCertificacion) #",
                            },
                            {
                                field: "FechaEnvio",
                                title: "Fecha Envío Boleto",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvio) #",
                            },
                            {
                                field: "FechaRecepBoleto",
                                title: "Fecha Recepción Boleto",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaRecepBoleto) #",
                            },
                            {
                                field: "FechaEnviadoFirma",
                                title: "Fecha Envío a Firma",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnviadoFirma) #",
                            },
                            {
                                field: "FechaRecibFirma",
                                title: "Fecha Recibido de Firma",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaRecibFirma) #",
                            },
                            {
                                field: "FechaEnvioBolsa",
                                title: "Fecha Envío a Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioBolsa) #",
                            },
                            {
                                field: "FechaVueltaBolsa",
                                title: "Fecha Vuelta de Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVueltaBolsa) #",
                            },
                            {
                                field: "FechaEnvioAfip",
                                title: "Fecha Envío a AFIP",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioAfip) #",
                            },
                            {
                                field: "FechaVueltaAfip",
                                title: "Fecha Vuelta de AFIP",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVueltaAfip) #",
                            },
                        ],
                        dataBound: function (e) {
                            autoFitColumnas(e.sender);
                        }
                    })
                    .data("kendoGrid");

                // Ocultar spinner inicial después de crear el grid
                setTimeout(function () {
                    mostrarSpinner(false);
                }, 1000);
            } catch (error) {
                console.error("Error inicializando grid:", error);
                mostrarMensaje(
                    "Error",
                    "Error al inicializar la tabla de datos",
                    "danger",
                );
                mostrarSpinner(false);
            }
        },

        actualizarContadores: function () {
            if (state.grid) {
                var total = state.grid.dataSource.total();
                $("#totalRegistros").text(
                    total + " registro" + (total !== 1 ? "s" : ""),
                );
            }
        },

        obtenerFiltros: function () {
            return {
                contratoSAP:                        controlNegocioSAP.val().trim()                        || null,
                materialId:                         controlMaterial.val()                                 || null,
                proveedor:                          controlProveedor.val()                                || null,
                bolsaId:                            controlBolsaCompraNet.val()                           || null,
                fechaCertificacionDesde:            controlFechaCertificacionDesde.val()                  || null,
                fechaCertificacionHasta:            controlFechaCertificacionHasta.val()                  || null,
                fechaVencimientoCertificacionDesde: controlFechaVencimientoCertificacionDesde.val()       || null,
                fechaVencimientoCertificacionHasta: controlFechaVencimientoCertificacionHasta.val()       || null,
                fechaRecepBoletoDesde:              controlFechaRecepBoletoDesde.val()                    || null,
                fechaRecepBoletoHasta:              controlFechaRecepBoletoHasta.val()                    || null,
                fechaEnviadoFirmaDesde:             controlFechaEnviadoFirmaDesde.val()                   || null,
                fechaEnviadoFirmaHasta:             controlFechaEnviadoFirmaHasta.val()                   || null,
                fechaRecibFirmaDesde:               controlFechaRecibFirmaDesde.val()                     || null,
                fechaRecibFirmaHasta:               controlFechaRecibFirmaHasta.val()                     || null,
                fechaEnvioBolsaDesde:               controlFechaEnvioBolsaDesde.val()                     || null,
                fechaEnvioBolsaHasta:               controlFechaEnvioBolsaHasta.val()                     || null,
                fechaVueltaBolsaDesde:              controlFechaVueltaBolsaDesde.val()                    || null,
                fechaVueltaBolsaHasta:              controlFechaVueltaBolsaHasta.val()                    || null,
                fechaEnvioAfipDesde:                controlFechaEnvioAfipDesde.val()                      || null,
                fechaEnvioAfipHasta:                controlFechaEnvioAfipHasta.val()                      || null,
                fechaVueltaAfipDesde:               controlFechaVueltaAfipDesde.val()                     || null,
                fechaVueltaAfipHasta:               controlFechaVueltaAfipHasta.val()                     || null,
            };
        },

        validarFiltros: function () {
            var filtros = this.obtenerFiltros();
            var errores = [];

            if (!esNegocioSAPValido()) {
                errores.push("Solo se admiten números y el ';' en el campo Negocio SAP.");
                return;
            }

            if (
                filtros.fechaCargaDesde &&
                filtros.fechaCargaHasta &&
                new Date(filtros.fechaCargaDesde) > new Date(filtros.fechaCargaHasta)
            ) {
                errores.push("La fecha desde debe ser anterior a la fecha hasta");
            }

            return errores;
        },

        filtrarBoletos: function () {
            var errores = this.validarFiltros();
            if (errores.length > 0) {
                mostrarMensaje("Error de Validación", errores.join("<br>"), "warning");
                return;
            }

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
            controlNegocioSAP.val("").removeClass("is-invalid");
            controlMaterial.val("");
            controlProveedor.val("");
            controlBolsaCompraNet.val("");
            controlFechaCertificacionDesde.val("");
            controlFechaCertificacionHasta.val("");
            controlFechaVencimientoCertificacionDesde.val("");
            controlFechaVencimientoCertificacionHasta.val("");
            controlFechaRecepBoletoDesde.val("");
            controlFechaRecepBoletoHasta.val("");
            controlFechaEnviadoFirmaDesde.val("");
            controlFechaEnviadoFirmaHasta.val("");
            controlFechaRecibFirmaDesde.val("");
            controlFechaRecibFirmaHasta.val("");
            controlFechaEnvioBolsaDesde.val("");
            controlFechaEnvioBolsaHasta.val("");
            controlFechaVueltaBolsaDesde.val("");
            controlFechaVueltaBolsaHasta.val("");
            controlFechaEnvioAfipDesde.val("");
            controlFechaEnvioAfipHasta.val("");
            controlFechaVueltaAfipDesde.val("");
            controlFechaVueltaAfipHasta.val("");
            this.filtrarBoletos();
        },

        exportarExcel: function () {
            var filtros = this.obtenerFiltros();
            var form = document.createElement("form");
            form.method = "POST";
            form.action = config.urls.exportBoletosExcel;
            form.style.display = "none";

            // Agrega los filtros como inputs
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
    };
})();

// Función global para formatear fechas (necesaria para el template del grid)
// Cached formatter to avoid recreating for each call
// Formateador de fecha para español (Argentina)
const _esArDateFormatter = new Intl.DateTimeFormat('es-AR');

function formatearFecha(fecha) {
    if (!fecha) return "";

    let date;

    try {
        // Si es un objeto Date válido
        if (fecha instanceof Date) {
            date = fecha;
        }
        // Si es un número (milisegundos desde Epoch)
        else if (typeof fecha === 'number') {
            date = new Date(fecha);
        }
        // Si es una cadena
        else if (typeof fecha === 'string') {
            // Formato MS Ajax: "/Date(1771995600000)/"
            const msMatch = /\/Date\((-?\d+)(?:[+-]\d+)?\)\//.exec(fecha);
            if (msMatch) {
                date = new Date(parseInt(msMatch[1], 10));
            }
            // Formato dd/MM/yyyy
            else if (/^\d{2}\/\d{2}\/\d{4}$/.test(fecha)) {
                const [day, month, year] = fecha.split("/").map(Number);
                date = new Date(year, month - 1, day); // mes base 0 en JS
            }
            // Fallback: constructor Date (ISO, etc.)
            else {
                date = new Date(fecha);
            }
        } else {
            return ""; // Tipo no soportado
        }

        // Validar que la fecha sea válida
        if (!date || isNaN(date.getTime())) return "";

        // Formatear fecha
        return _esArDateFormatter.format(date);
    } catch (e) {
        return "";
    }
}

// Inicializar cuando el DOM esté listo
$(document).ready(function () {
    try {
        ControlBoletosReporteSeguimiento.init();
    } catch (error) {
        console.error("Error inicializando Control de Boletos:", error);
        alert("Error al inicializar la aplicación. Por favor, recargue la página.");
    }
});

// Manejar errores globales
window.addEventListener("error", function (e) {
    console.error("Error global:", e.error);
});
