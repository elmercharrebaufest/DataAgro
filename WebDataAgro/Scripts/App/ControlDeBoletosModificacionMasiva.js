// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    "use strict";

    var config = {
        urls: {
            getBoletosParaModificar: "/ControlDeBoletos/GetBoletosParaModificar",
            guardarBoletosParaModificarFechas: "/ControlDeBoletos/GuardarBoletosParaModificarFechas"
        }
    };

    let controlNegocioSAP = $("#frmPendienteControl #NegocioSAP");
    let controlFiltrarBoletos = $("#frmPendienteControl #filtrarBoletos");
    let controlLimpiarFiltros = $("#frmPendienteControl #limpiarFiltros");
    let controlGuardarFechas = $("#frmPendienteControl #guardarFechas");

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false,
        columnasAjustadas: false,
        columnWidths: [],
        dirtyItems: {},
        selectedPasteField: null,
        selectedPasteRowIndex: 0
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

    function autoFitColumnas(grid) {
        var $wrapper = grid.element;
        var $headerCols = $wrapper.find(".k-grid-header-wrap colgroup col");
        var $contentCols = $wrapper.find(".k-grid-content   colgroup col");
        var $headerCells = $wrapper.find(".k-grid-header-wrap tr:first th");
        var $rows = $wrapper.find(".k-grid-content tbody tr");
        var columns = grid.columns;

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

    function parseDateDdMmYyyy(value) {
        if (!value) return null;
        if (Object.prototype.toString.call(value) === "[object Date]") return isNaN(value.getTime()) ? null : value;

        var text = String(value).trim();
        if (text === "") return null;

        var parsed = kendo.parseDate(text, "dd/MM/yyyy") || kendo.parseDate(text);
        if (parsed && !isNaN(parsed.getTime())) return parsed;

        var parts = text.split("/");
        if (parts.length === 3) {
            var d = parseInt(parts[0], 10), m = parseInt(parts[1], 10) - 1, y = parseInt(parts[2], 10);
            var dt = new Date(y, m, d);
            if (!isNaN(dt.getTime())) return dt;
        }

        return null;
    }

    function dateCellEditor(container, options) {
        var input = $('<input name="' + options.field + '" />');
        input.appendTo(container);
        input.kendoDatePicker({
            format: "dd/MM/yyyy",
            parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"]
        });

        var dp = input.data("kendoDatePicker");

        // Evitar que el click del ícono del calendario cierre el popup por burbujeo
        var $datePickerWrap = input.closest(".k-datepicker, .k-date-picker");
        $datePickerWrap.off("mousedown.modifMasivaCalendar click.modifMasivaCalendar", ".k-select, .k-i-calendar, .k-icon")
            .on("mousedown.modifMasivaCalendar click.modifMasivaCalendar", ".k-select, .k-i-calendar, .k-icon", function (e) {
                e.stopPropagation();
            });

        input.off("paste.modifMasiva").on("paste.modifMasiva", function () {
            var $this = $(this);
            setTimeout(function () {
                var parsed = parseDateDdMmYyyy($this.val());
                dp.value(parsed);
                options.model.set(options.field, parsed);
            }, 0);
        });

        input.off("blur.modifMasiva").on("blur.modifMasiva", function () {
            var parsed = parseDateDdMmYyyy($(this).val());
            dp.value(parsed);
            options.model.set(options.field, parsed);
        });
    }

    function esCampoFecha(field) {
        return [
            "FechaRecepBoleto",
            "FechaEnviadoFirma",
            "FechaEnvioBolsa",
            "FechaEnvioAfip",
            "FechaRecibFirma",
            "FechaVueltaBolsa",
            "FechaVueltaAfip",
            "FechaEnvioSellado"
        ].indexOf(field) >= 0;
    }

    function parsearValoresFechaPegados(texto) {
        if (!texto) return [];

        var raw = String(texto).replace(/\u0000/g, "");

        var valores = raw
            .split(/(?:\r\n|\n|\r)+/)
            .map(function (linea) {
                var limpio = (linea || "").trim();
                if (!limpio) return "";

                // Si viene desde Excel con columnas, tomar solo la primera celda de la fila
                return limpio.split("\t")[0].trim();
            })
            .filter(function (v) { return v !== ""; });

        // Fallback robusto: extraer todas las fechas dd/MM/yyyy del bloque completo
        if (valores.length <= 1) {
            var matches = raw.match(/\b\d{1,2}\/\d{1,2}\/\d{4}\b/g);
            if (matches && matches.length > 1) {
                return matches;
            }
        }

        return valores;
    }

    function aplicarPegadoMasivoEnColumna(field, filaInicial, valoresTexto) {
        if (!state.grid || !field || !valoresTexto || !valoresTexto.length) return;
        if (!esCampoFecha(field)) return;

        // Usar dataSource.view() para acceder a los items reales sin depender del DOM
        var view = state.grid.dataSource.view();
        var start = Math.max(0, filaInicial || 0);

        for (var i = 0; i < valoresTexto.length; i++) {
            var filaIdx = start + i;
            if (filaIdx >= view.length) break;

            var valorTexto = (valoresTexto[i] || "").trim();
            if (valorTexto === "") continue;

            var item = view[filaIdx];
            if (!item) continue;

            var fecha = parseDateDdMmYyyy(valorTexto);
            item.set(field, fecha);
            state.dirtyItems[item.ControlDeBoletosId] = item.toJSON();
        }

        actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
    }

    function marcarColumnaSeleccionada(field) {
        if (!state.grid) return;

        var $ths = state.grid.thead.find("th[data-field]");
        $ths.removeClass("k-state-selected");

        if (!field) return;

        var $target = state.grid.thead.find('th[data-field="' + field + '"]');
        $target.addClass("k-state-selected");
    }

    function obtenerTextoPortapapeles(e) {
        return (e.originalEvent && e.originalEvent.clipboardData)
            ? e.originalEvent.clipboardData.getData("text")
            : (window.clipboardData ? window.clipboardData.getData("Text") : "");
    }

    function configurarPegadoMasivoFechas() {
        if (!state.grid || !state.grid.tbody || !state.grid.thead) return;

        // ── Textarea oculto que recibe el foco para capturar Ctrl+V fiablemente ──
        var $trap = $("#modifMasiva-paste-trap");
        if (!$trap.length) {
            $trap = $('<textarea id="modifMasiva-paste-trap" '
                + 'style="position:absolute;opacity:0;width:1px;height:1px;left:-9999px;top:-9999px;'
                + 'pointer-events:none;" tabindex="-1" autocomplete="off" readonly></textarea>');
            $("body").append($trap);
        }
        $trap.attr("readonly", false);

        function enfocarTrap() {
            $trap.val("");
            $trap[0].focus();
        }

        function manejarPegado(field, filaInicial, textoClipboard) {
            if (!esCampoFecha(field)) return;
            var valores = parsearValoresFechaPegados(textoClipboard);
            if (!valores.length) return;

            state.selectedPasteField = field;
            state.selectedPasteRowIndex = Math.max(0, filaInicial || 0);
            marcarColumnaSeleccionada(field);

            var colIndex = -1;
            for (var c = 0; c < state.grid.columns.length; c++) {
                if (state.grid.columns[c] && state.grid.columns[c].field === field) {
                    colIndex = c;
                    break;
                }
            }

            aplicarPegadoMasivoEnColumna(field, filaInicial, valores);

            // Mantener la celda actual donde se inició el pegado (estilo Excel)
            if (colIndex >= 0) {
                setTimeout(function () {
                    var $celdaActual = state.grid.tbody.find("tr").eq(state.selectedPasteRowIndex).find("td").eq(colIndex);
                    if ($celdaActual.length) {
                        state.grid.current($celdaActual);
                        state.grid.wrapper.focus();
                    }
                }, 0);
            }
        }

        // ── Selección de columna por clic en encabezado ──
        state.grid.thead.off("click.modifMasivaCol").on("click.modifMasivaCol", "th[data-field]", function () {
            var field = $(this).attr("data-field");
            state.selectedPasteField = esCampoFecha(field) ? field : null;
            state.selectedPasteRowIndex = 0;
            marcarColumnaSeleccionada(state.selectedPasteField);
            state.grid.wrapper.focus();
        });

        // ── Selección de celda por clic en el cuerpo ──
        state.grid.tbody.off("click.modifMasivaCol").on("click.modifMasivaCol", "td", function (e) {
            // Si el click ocurre dentro del DatePicker/Popup, no forzar foco al wrapper
            // para evitar que el calendario se cierre inmediatamente.
            if ($(e.target).closest(".k-datepicker, .k-date-picker, .k-animation-container, .k-calendar-container, .k-popup").length) {
                return;
            }

            var $cell = $(this);
            var colIndex = $cell.index();
            var columna = state.grid.columns[colIndex];
            var field = columna && columna.field ? columna.field : null;
            var rowIndex = $cell.closest("tr").index();

            state.grid.current($cell);
            state.selectedPasteField = esCampoFecha(field) ? field : null;
            state.selectedPasteRowIndex = rowIndex >= 0 ? rowIndex : 0;
            marcarColumnaSeleccionada(state.selectedPasteField);
            state.grid.wrapper.focus();
        });

        // ── Paste en el textarea trampa ──
        $trap.off("paste.modifMasivaTrap").on("paste.modifMasivaTrap", function (e) {
            if (!state.selectedPasteField) return;
            var texto = (e.originalEvent && e.originalEvent.clipboardData)
                ? e.originalEvent.clipboardData.getData("text")
                : (window.clipboardData ? window.clipboardData.getData("Text") : "");
            if (!texto) return;
            e.preventDefault();
            manejarPegado(state.selectedPasteField, state.selectedPasteRowIndex, texto);
        });

        // ── Paste nativo en una celda del body (Ctrl+V sobre celda activa) ──
        state.grid.tbody.off("paste.modifMasivaCol").on("paste.modifMasivaCol", "td", function (e) {
            var colIndex = $(this).index();
            var columna = state.grid.columns[colIndex];
            var field = columna && columna.field ? columna.field : null;
            if (!esCampoFecha(field)) return;
            var texto = obtenerTextoPortapapeles(e);
            if (!texto) return;
            e.preventDefault();
            manejarPegado(field, $(this).closest("tr").index(), texto);
        });

        // ── Teclado: navegación por flechas (estilo Excel) ──
        state.grid.wrapper.attr("tabindex", "0");
        state.grid.wrapper.off("keydown.modifMasivaNavigation").on("keydown.modifMasivaNavigation", function (e) {
            var key = e.keyCode || e.which;
            var esFlecha = key === 37 || key === 38 || key === 39 || key === 40;
            if (!esFlecha) return;

            // No interferir si el usuario está escribiendo en un input editable
            var $target = $(e.target);
            if ($target.is("input, textarea, select") && !$target.is("[readonly]")) return;

            var $rows = state.grid.tbody.find("tr");
            if (!$rows.length) return;

            var current = state.grid.current();
            if (!current || !current.length) {
                var $firstCell = $rows.eq(0).find("td").first();
                if ($firstCell.length) {
                    state.grid.current($firstCell);
                    state.grid.wrapper.focus();
                    e.preventDefault();
                }
                return;
            }

            var rowIndex = current.closest("tr").index();
            var colIndex = current.index();
            var maxRow = $rows.length - 1;
            var maxCol = current.closest("tr").find("td").length - 1;

            if (e.ctrlKey || e.metaKey) {
                // Ctrl+flecha: ir a borde (inicio/fin) como Excel
                if (key === 37) colIndex = 0;          // Ctrl+Left
                else if (key === 39) colIndex = maxCol; // Ctrl+Right
                else if (key === 38) rowIndex = 0;      // Ctrl+Up
                else if (key === 40) rowIndex = maxRow; // Ctrl+Down
            } else {
                if (key === 37) colIndex = Math.max(0, colIndex - 1);          // left
                else if (key === 39) colIndex = Math.min(maxCol, colIndex + 1); // right
                else if (key === 38) rowIndex = Math.max(0, rowIndex - 1);      // up
                else if (key === 40) rowIndex = Math.min(maxRow, rowIndex + 1); // down
            }

            var $dest = $rows.eq(rowIndex).find("td").eq(colIndex);
            if ($dest.length) {
                state.grid.current($dest);
                state.grid.wrapper.focus();

                var col = state.grid.columns[colIndex];
                var field = col && col.field ? col.field : null;
                state.selectedPasteField = esCampoFecha(field) ? field : null;
                state.selectedPasteRowIndex = rowIndex;
                marcarColumnaSeleccionada(state.selectedPasteField);

                e.preventDefault();
            }
        });

        // ── Teclado: Delete/Supr + Ctrl+D ──
        state.grid.wrapper.off("keydown.modifMasivaDelete").on("keydown.modifMasivaDelete", function (e) {
            var current = state.grid.current();
            if (!current || !current.length) return;

            var colIndex = current.index();
            var columna = state.grid.columns[colIndex];
            var field = columna && columna.field ? columna.field : null;
            if (!esCampoFecha(field)) return;

            var row = current.closest("tr");
            var rowIndex = row.index();
            var view = state.grid.dataSource.view();
            var item = view[rowIndex];
            if (!item) return;

            // Delete / Supr → limpiar fecha y quedarse en la misma celda
            if (e.key === "Delete" || e.key === "Del" || e.keyCode === 46) {
                e.preventDefault();
                // Guardar índices antes de set() porque puede re-renderizar el DOM
                var savedRowIndex = rowIndex;
                var savedColIndex = colIndex;
                var savedField = field;
                item.set(field, null);
                state.dirtyItems[item.ControlDeBoletosId] = item.toJSON();
                actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);

                // Re-adquirir la celda por índice tras el re-render de Kendo
                setTimeout(function () {
                    var celdaActual = state.grid.tbody.find("tr").eq(savedRowIndex).find("td").eq(savedColIndex);
                    if (celdaActual.length) {
                        state.grid.current(celdaActual);
                        state.selectedPasteField = savedField;
                        state.selectedPasteRowIndex = savedRowIndex;
                    }
                }, 0);
                return;
            }

            // Ctrl+D → copiar valor hacia abajo (estilo Excel)
            if ((e.ctrlKey || e.metaKey) && (e.key === "d" || e.key === "D" || e.keyCode === 68)) {
                e.preventDefault();
                var valorOrigen = item.get(field);
                var siguiente = rowIndex + 1;
                if (siguiente < view.length) {
                    var itemDestino = view[siguiente];
                    if (itemDestino) {
                        itemDestino.set(field, valorOrigen);
                        state.dirtyItems[itemDestino.ControlDeBoletosId] = itemDestino.toJSON();
                        var celdaDestino = state.grid.tbody.find("tr").eq(siguiente).find("td").eq(colIndex);
                        if (celdaDestino && celdaDestino.length) {
                            state.grid.current(celdaDestino);
                            state.selectedPasteField = field;
                            state.selectedPasteRowIndex = siguiente;
                        }
                    }
                }
                actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
            }
        });

        // ── Ctrl+C / Ctrl+V sobre la celda actual (estilo Excel) ──
        state.grid.wrapper.off("keydown.modifMasivaClipboard").on("keydown.modifMasivaClipboard", function (e) {
            var current = state.grid.current();
            var colIndex = current && current.length ? current.index() : -1;
            var columna = colIndex >= 0 ? state.grid.columns[colIndex] : null;
            var fieldActual = columna && columna.field ? columna.field : state.selectedPasteField;
            var rowIndexActual = current && current.length ? current.closest("tr").index() : state.selectedPasteRowIndex;

            // Ctrl+C en fecha: copiar valor actual al portapapeles
            if ((e.ctrlKey || e.metaKey) && (e.key === "c" || e.key === "C" || e.keyCode === 67)) {
                if (!esCampoFecha(fieldActual) || rowIndexActual < 0) return;

                var viewCopy = state.grid.dataSource.view();
                var itemCopy = viewCopy[rowIndexActual];
                if (!itemCopy) return;

                var valor = itemCopy.get(fieldActual);
                var texto = valor ? kendo.toString(valor, "dd/MM/yyyy") : "";

                if (navigator.clipboard && navigator.clipboard.writeText) {
                    e.preventDefault();
                    navigator.clipboard.writeText(texto);
                }
                return;
            }

            // Ctrl+V en fecha: pegar desde portapapeles comenzando en la celda actual
            if ((e.ctrlKey || e.metaKey) && (e.key === "v" || e.key === "V" || e.keyCode === 86)) {
                if (!esCampoFecha(fieldActual)) return;

                state.selectedPasteField = fieldActual;
                state.selectedPasteRowIndex = rowIndexActual >= 0 ? rowIndexActual : 0;
                marcarColumnaSeleccionada(state.selectedPasteField);

                if (navigator.clipboard && navigator.clipboard.readText) {
                    e.preventDefault();
                    navigator.clipboard.readText().then(function (textoPegado) {
                        manejarPegado(state.selectedPasteField, state.selectedPasteRowIndex, textoPegado);
                    }).catch(function () {
                        // Mantener foco/celda actual si no hay permiso de clipboard
                        state.grid.wrapper.focus();
                    });
                }
            }
        });
    }

    // Funciones públicas
    return {
        init: async function () {
            if (state.datosInicializados) return;
            this.configurarEventos();
            this.inicializarGrid();

            state.datosInicializados = true;
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

            controlGuardarFechas
                .off("click")
                .on("click", function () {
                    self.guardarFechas();
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
                                    var filtros = self.obtenerFiltros();
                                    var parametros = {
                                        ContratoSAP: filtros.negocioSAP
                                    };

                                    MSExecuteOnServerAsync(config.urls.getBoletosParaModificar, parametros)
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
                                    id: "ControlDeBoletosId",
                                    fields: {
                                        ControlDeBoletosId: { type: "number", editable: false },
                                        NegocioId: { type: "number", editable: false },
                                        ContratoSAP: { type: "string", editable: false },
                                        TipoBoleto: { type: "string", editable: false },
                                        SeguimientoBoletoId: { type: "number", editable: false },
                                        FechaRecepBoleto: { type: "date" },
                                        FechaEnviadoFirma: { type: "date" },
                                        FechaEnvioBolsa: { type: "date" },
                                        FechaEnvioAfip: { type: "date" },
                                        FechaRecibFirma: { type: "date" },
                                        FechaVueltaBolsa: { type: "date" },
                                        FechaVueltaAfip: { type: "date" },
                                        FechaEnvioSellado: { type: "date" }
                                    }
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
                            },
                        },
                        height: 550,
                        scrollable: { virtual: false },
                        sortable: false,
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
                        navigatable: true,
                        selectable: "cell",
                        editable: "incell",
                        columns: [
                            {
                                field: "TipoBoleto",
                                title: "Boleto",
                                width: 80,
                                editable: function () { return false; }
                            },
                            {
                                field: "ContratoSAP",
                                title: "Contrato SAP",
                                width: 120,
                                editable: function () { return false; }
                            },
                            {
                                field: "FechaRecepBoleto",
                                title: "Fecha Recepción Boleto",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaRecepBoleto) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnviadoFirma",
                                title: "Fecha Envío a Firma",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnviadoFirma) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnvioBolsa",
                                title: "Fecha Envío Obleado Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioBolsa) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnvioAfip",
                                title: "Fecha Envío Arca",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioAfip) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaRecibFirma",
                                title: "Fecha Recibido de Firma",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaRecibFirma) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaVueltaBolsa",
                                title: "Fecha Recepción Obleado Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVueltaBolsa) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaVueltaAfip",
                                title: "Fecha Recepción Arca",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVueltaAfip) #",
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnvioSellado",
                                title: "Fecha Envío Sellado",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioSellado) #",
                                editor: dateCellEditor
                            },
                        ],
                        cellClose: function (e) {
                            if (!e.model) return;
                            state.dirtyItems[e.model.ControlDeBoletosId] = e.model.toJSON();
                            actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
                        },
                        dataBound: function (e) {
                            if (!state.columnasAjustadas) {
                                autoFitColumnas(e.sender);
                                state.columnasAjustadas = true;
                            }
                            configurarPegadoMasivoFechas();
                            actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
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
            };
        },

        validarFiltros: function () {
            var filtros = this.obtenerFiltros();
            var sinFiltrosPrincipales = !filtros.negocioSAP;
            return !(sinFiltrosPrincipales);
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
            state.columnasAjustadas = false;
            state.dirtyItems = {};
            actualizarBoton(controlGuardarFechas, false);
        },

        guardarFechas: function () {
            var self = this;
            var items = Object.keys(state.dirtyItems).map(function (k) { return state.dirtyItems[k]; });

            if (!items.length) {
                mostrarMensaje("Información", "No hay fechas modificadas para guardar.", "info");
                return;
            }

            $.ajax({
                url: config.urls.guardarBoletosParaModificarFechas,
                type: "POST",
                data: { boletos: items },
                success: function (resp) {
                    if (resp && resp.success) {
                        state.dirtyItems = {};
                        actualizarBoton(controlGuardarFechas, false);
                        mostrarMensaje("Éxito", "Fechas guardadas correctamente.", "success");
                        if (state.grid) {
                            state.grid.dataSource.read();
                        }
                    } else {
                        var msg = "Error al guardar fechas.";
                        if (resp && resp.errors && resp.errors.length > 0 && resp.errors[0].Message) {
                            msg = resp.errors[0].Message;
                        }
                        mostrarMensaje("Error", msg, "danger");
                    }
                },
                error: function () {
                    mostrarMensaje("Error", "Error al comunicarse con el servidor al guardar.", "danger");
                }
            });
        },
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
