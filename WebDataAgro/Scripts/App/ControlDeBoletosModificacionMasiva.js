// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    "use strict";

    var ui = window.ControlDeBoletosUI;

    var config = {
        urls: {
            getBoletosParaModificar: "/ControlDeBoletos/GetBoletosParaModificar",
            getBolsa: "/ControlDeBoletos/GetBolsaCompraNet",
            getBolsaSAP: "/ControlDeBoletos/GetBolsaCompraNetSAP",
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
        selectedPasteRowIndex: 0,
        selectedPasteColIndex: 0,
        selectedPasteOrigin: "cell",
        rangeAnchor: null,
        selectedRange: null,
        selectedCells: {},
        columnClipboard: null,
        listaBolsaSAP: null,
        listaBolsa: null,
        opcionesBolsa: [],
        bolsaLookup: {},
        comboPopupAbierto: false,
        _editandoCeldaExplicito: false
    };

    // Funciones privadas
    function mostrarSpinner(mostrar) {
        ui.toggleSpinner(mostrar);
    }

    function mostrarMensaje(titulo, mensaje, tipo) {
        ui.showModalMessage(titulo, mensaje, tipo);

        // Al cerrar el modal de validación, devolver foco a la celda actual de la grilla.
        $("#mensajeModal").off("hidden.bs.modal.modifMasiva").one("hidden.bs.modal.modifMasiva", function () {
            restaurarFocoGrilla();
        });
    }

    function restaurarFocoGrilla() {
        if (!state.grid) return;

        var rowIndex = Math.max(0, state.selectedPasteRowIndex || 0);
        var colIndex = Math.max(0, state.selectedPasteColIndex || 0);
        var $rows = state.grid.tbody.find("tr");
        if (!$rows.length) return;

        if (rowIndex >= $rows.length) rowIndex = $rows.length - 1;
        var $cells = $rows.eq(rowIndex).find("td");
        if (!$cells.length) return;
        if (colIndex >= $cells.length) colIndex = $cells.length - 1;

        var $cell = $cells.eq(colIndex);
        if ($cell.length) {
            state.grid.current($cell);
            state.grid.wrapper.focus();
        }

        if (hayCeldasSeleccionadas()) {
            refrescarSeleccionVisual();
        } else if (state.selectedRange) {
            aplicarSeleccionRango(state.selectedRange);
        } else {
            setAnchorYSeleccion(rowIndex, colIndex);
        }
    }

    function actualizarBoton(selector, habilitado, texto) {
        ui.setButtonState(selector, habilitado, texto);
    }

    async function cargarBolsaSAP() {
        state.listaBolsaSAP = await MSExecuteGetOnServerAsync(config.urls.getBolsaSAP);
        state.listaBolsa = await MSExecuteGetOnServerAsync(config.urls.getBolsa);
        reconstruirCacheBolsas();
    }

    function reconstruirCacheBolsas() {
        var listaBase = state.listaBolsa || [];
        var listaSap = state.listaBolsaSAP || [];

        var sapPorId = {};
        for (var i = 0; i < listaSap.length; i++) {
            var idKey = String(listaSap[i].Text || "").trim();
            var sapValue = String(listaSap[i].Value || "").trim();
            if (idKey && sapValue) sapPorId[idKey] = sapValue;
        }

        var opciones = [];
        var lookup = {};
        for (var j = 0; j < listaBase.length; j++) {
            var item = listaBase[j];
            var id = String(item.Value || "").trim();
            var codigoSap = sapPorId[id] || "";
            if (!codigoSap) continue;

            opciones.push({ Text: item.Text, Value: codigoSap });
            lookup[codigoSap.toUpperCase()] = codigoSap;
        }

        state.opcionesBolsa = opciones;
        state.bolsaLookup = lookup;

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
            background-color: #fff !important;
        }

        /* Todas las filas siempre blancas */
        #boletos-grid .k-grid-content tr,
        #boletos-grid .k-grid-content tr.k-alt {
            background-color: #fff !important;
        }

        #boletos-grid .k-grid-content tr td,
        #boletos-grid .k-grid-content tr.k-alt td {
            background-color: #fff !important;
        }

        /* Hover sin cambiar el color */
        #boletos-grid .k-grid-content tr:hover td,
        #boletos-grid .k-grid-content tr.k-state-hover td {
            background-color: #fff !important;
            color: #333 !important;
        }

        /* Selección sin cambiar el color */
        #boletos-grid .k-grid-content tr.k-state-selected td,
        #boletos-grid .k-grid-content tr.k-selected td {
            background-color: #fff !important;
            color: #333 !important;
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

        /* Celdas bloqueadas: siempre grises, máxima prioridad sobre hover, selección y rango */
        #boletos-grid .k-grid-content tr td.campo-bloqueado,
        #boletos-grid .k-grid-content tr.k-alt td.campo-bloqueado,
        #boletos-grid .k-grid-content tr:hover td.campo-bloqueado,
        #boletos-grid .k-grid-content tr.k-state-hover td.campo-bloqueado,
        #boletos-grid .k-grid-content tr.k-state-selected td.campo-bloqueado,
        #boletos-grid .k-grid-content tr.k-selected td.campo-bloqueado {
            background-color: #e0e0e0 !important;
            color: #a0a0a0 !important;
            cursor: not-allowed !important;
        }

        #boletos-grid .k-grid-content tr td.campo-bloqueado .k-input,
        #boletos-grid .k-grid-content tr td.campo-bloqueado .k-textbox,
        #boletos-grid .k-grid-content tr td.campo-bloqueado .k-dropdown,
        #boletos-grid .k-grid-content tr td.campo-bloqueado .k-datepicker {
            background-color: #e0e0e0 !important;
            color: #a0a0a0 !important;
        }

        /* Selección de rango: no afecta a celdas bloqueadas */
        #boletos-grid .k-grid-content td.range-selected:not(.campo-bloqueado) {
            background-color: #e8f0ff !important;
        }

        #boletos-grid .k-grid-content tr.k-state-selected td.range-selected:not(.campo-bloqueado),
        #boletos-grid .k-grid-content tr.k-selected td.range-selected:not(.campo-bloqueado),
        #boletos-grid .k-grid-content tr:hover td.range-selected:not(.campo-bloqueado),
        #boletos-grid .k-grid-content tr.k-state-hover td.range-selected:not(.campo-bloqueado) {
            background-color: #e8f0ff !important;
        }

        #boletos-grid .k-grid-content td.range-anchor:not(.campo-bloqueado) {
            outline: 2px solid #4d90fe !important;
            outline-offset: -2px;
            background-color: #dbe9ff !important;
        }

        #boletos-grid .k-grid-content tr.k-state-selected td.range-anchor:not(.campo-bloqueado),
        #boletos-grid .k-grid-content tr.k-selected td.range-anchor:not(.campo-bloqueado),
        #boletos-grid .k-grid-content tr:hover td.range-anchor:not(.campo-bloqueado),
        #boletos-grid .k-grid-content tr.k-state-hover td.range-anchor:not(.campo-bloqueado) {
            background-color: #dbe9ff !important;
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
        var bloqueado = esCampoBloqueadoPorFila(options.model, options.field);
        var input = $('<input name="' + options.field + '" />');
        input.appendTo(container);

        if (bloqueado) {
            input.prop("readonly", true).prop("disabled", true).addClass("campo-bloqueado");
            container.closest("td").addClass("campo-bloqueado");
            return;
        }

        var fechaMinima = null;
        if (options.field !== "FechaRecepBoleto") {
            var feRecepBoleto = obtenerValorFila(options.model, "FechaRecepBoleto");
            if (feRecepBoleto && typeof feRecepBoleto.getTime === "function") {
                fechaMinima = feRecepBoleto;
            } else if (feRecepBoleto) {
                fechaMinima = parseDateDdMmYyyy(feRecepBoleto);
            }

            input.kendoDatePicker({
                format: "dd/MM/yyyy",
                parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"],
                min: fechaMinima
            });
        } else {
            input.kendoDatePicker({
                format: "dd/MM/yyyy",
                parseFormats: ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"]
            });
        }
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
                var error = validarFechaRelacional(options.model, options.field, parsed);
                if (error) {
                    mostrarMensaje("Validaci\u00f3n", error, "warning");
                    dp.value("");
                    return;
                }
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
            "FechaEnvioSellado",
            "FechaCertificacion",
            "FechaVencimientoCertificacion"
        ].indexOf(field) >= 0;
    }

    function esCampoComboBolsa(field) {
        return ["PreCertificacionBolsa", "BolsaSellado"].indexOf(field) >= 0;
    }

    function esCampoTextoEditable(field) {
        return ["Oblea"].indexOf(field) >= 0;
    }

    function obtenerValorFila(item, field) {
        if (!item || !field) return null;
        if (typeof item.get === "function") {
            var valor = item.get(field);
            if (valor !== undefined) return valor;
        }
        return item[field];
    }

    function esValorVerdadero(valor) {
        if (valor === true || valor === 1) return true;
        if (valor === false || valor === 0 || valor === null || valor === undefined) return false;

        var texto = String(valor).trim().toUpperCase();
        return texto === "1" || texto === "TRUE" || texto === "SI" || texto === "S" || texto === "YES" || texto === "Y";
    }

    function esCampoBloqueadoPorFila(item, field) {
        if (!item || !field) return false;

        var esCartaOferta = esValorVerdadero(obtenerValorFila(item, "EsCartaOferta"));
        var operaSinOblea = esValorVerdadero(obtenerValorFila(item, "OperaSinOblea"));
        var esSinBoleto = esValorVerdadero(obtenerValorFila(item, "EsSinBoleto"));
        if (esCartaOferta && field === "FechaEnvioSellado") return true;
        if (operaSinOblea) {
            return [
                "Oblea",
                "FechaCertificacion",
                "FechaVencimientoCertificacion",
                "PreCertificacionBolsa",
                "FechaEnvioBolsa",
                "FechaVueltaBolsa",
            ].indexOf(field) >= 0;
        }
        if (esSinBoleto) {
            return [
                "Oblea",
                "FechaCertificacion",
                "FechaVencimientoCertificacion",
                "PreCertificacionBolsa",
                "FechaEnviadoFirma",
                "FechaEnvioBolsa",
                "FechaEnvioAfip",
                "FechaRecibFirma",
                "FechaVueltaBolsa",
                "FechaVueltaAfip",
                "FechaEnvioSellado"
            ].indexOf(field) >= 0;
        }
        return false;
    }

    function esCampoEditablePegado(field) {
        return esCampoFecha(field) || esCampoComboBolsa(field) || esCampoTextoEditable(field);
    }

    function esCampoEditablePegadoEnFila(item, field) {
        return esCampoEditablePegado(field) && !esCampoBloqueadoPorFila(item, field);
    }

    function validarFechaRelacional(item, field, valorFecha) {
        if (!valorFecha || !esCampoFecha(field)) return null;

        var esCartaOferta = esValorVerdadero(obtenerValorFila(item, "EsCartaOferta"));
        var operaSinOblea = esValorVerdadero(obtenerValorFila(item, "OperaSinOblea"));
        var esSinBoleto = esValorVerdadero(obtenerValorFila(item, "EsSinBoleto"));

        if (esSinBoleto) return null;

        var feRecepcionBoleto = obtenerValorFila(item, "FechaRecepBoleto");
        if (feRecepcionBoleto && typeof feRecepcionBoleto.getTime === "function") {
            feRecepcionBoleto = feRecepcionBoleto;
        } else if (feRecepcionBoleto) {
            feRecepcionBoleto = parseDateDdMmYyyy(feRecepcionBoleto);
        }

        if (field === "FechaRecepBoleto") return null;

        if (!feRecepcionBoleto) {
            return "La Fecha de recepci\u00f3n de boleto es obligatoria.";
        }

        if (valorFecha < feRecepcionBoleto) {
            return "La fecha no puede ser anterior a la Fecha de recepci\u00f3n de boleto.";
        }

        var feEnvioFirmas = obtenerValorFila(item, "FechaEnviadoFirma");
        var feEnvioBolsa = obtenerValorFila(item, "FechaEnvioBolsa");
        var feEnvioAfip = obtenerValorFila(item, "FechaEnvioAfip");
        var feRecepcionBolsa = obtenerValorFila(item, "FechaVueltaBolsa");
        var feRecepcionAfip = obtenerValorFila(item, "FechaVueltaAfip");

        if (feEnvioFirmas && typeof feEnvioFirmas.getTime !== "function") feEnvioFirmas = parseDateDdMmYyyy(feEnvioFirmas);
        if (feEnvioBolsa && typeof feEnvioBolsa.getTime !== "function") feEnvioBolsa = parseDateDdMmYyyy(feEnvioBolsa);
        if (feEnvioAfip && typeof feEnvioAfip.getTime !== "function") feEnvioAfip = parseDateDdMmYyyy(feEnvioAfip);
        if (feRecepcionBolsa && typeof feRecepcionBolsa.getTime !== "function") feRecepcionBolsa = parseDateDdMmYyyy(feRecepcionBolsa);
        if (feRecepcionAfip && typeof feRecepcionAfip.getTime !== "function") feRecepcionAfip = parseDateDdMmYyyy(feRecepcionAfip);

        if (field == "FechaRecibFirma" && feEnvioFirmas && valorFecha < feEnvioFirmas) {
            return "La Fecha de recepci\u00f3n Firmas debe ser igual o mayor a la Fecha de env\u00edo Firmas.";
        }

        if (field == "FechaVueltaBolsa" && !operaSinOblea && feEnvioBolsa && valorFecha < feEnvioBolsa) {
            return "La Fecha de recepci\u00f3n Oblea debe ser igual o mayor a la Fecha de env\u00edo Oblea.";
        }

        if (field == "FechaVueltaAfip" && feEnvioAfip && valorFecha < feEnvioAfip) {
            return "La Fecha de recepci\u00f3n Arca debe ser igual o mayor a la Fecha de env\u00edo Arca.";
        }

        if (field == "FechaEnvioSellado" && !esCartaOferta && feRecepcionBolsa && valorFecha < feRecepcionBolsa) {
            return "La Fecha de env\u00edo Sellado debe ser igual o mayor a la Fecha de recepci\u00f3n Obleado Bolsa.";
        }

        return null;
    }

    function obtenerOpcionesBolsaParaCombo() {
        return state.opcionesBolsa || [];
    }

    function obtenerValorComboBolsaValido(valorTexto) {
        var texto = (valorTexto || "").trim();
        if (!texto) return null;

        var lookup = state.bolsaLookup || {};
        return lookup[texto.toUpperCase()] || null;
    }

    function comboBolsaEditor(container, options) {
        var bloqueado = esCampoBloqueadoPorFila(options.model, options.field);
        var input = $('<input name="' + options.field + '" />');
        input.appendTo(container);

        if (bloqueado) {
            input.prop("readonly", true).prop("disabled", true).addClass("campo-bloqueado");
            container.closest("td").addClass("campo-bloqueado");
            return;
        }

        input.kendoDropDownList({
            dataTextField: "Value",
            dataValueField: "Value",
            optionLabel: "Seleccione...",
            filter: "contains",
            ignoreCase: true,
            dataSource: obtenerOpcionesBolsaParaCombo(),
            valuePrimitive: true,
            open: function () {
                state.comboPopupAbierto = true;
            },
            close: function () {
                state.comboPopupAbierto = false;
            }
        });
    }

    function textCellEditor(container, options) {
        var bloqueado = esCampoBloqueadoPorFila(options.model, options.field);
        var maxLength = options.field === "Oblea" ? 18 : 50;
        var input = $('<input class="k-input k-textbox" name="' + options.field + '" maxlength="' + maxLength + '" />');
        input.appendTo(container);

        if (bloqueado) {
            input.prop("readonly", true).prop("disabled", true).addClass("campo-bloqueado");
            container.closest("td").addClass("campo-bloqueado");
        }
    }

    function parsearValoresFechaPegados(texto) {
        if (!texto) return [];

        var raw = String(texto).replace(/\u0000/g, "");

        var valores = raw
            .split(/(?:\r\n|\n|\r)+/)
            .map(function (linea) {
                var limpio = (linea || "").trim();
                if (!limpio) return "";
                return limpio.split("\t")[0].trim();
            })
            .filter(function (v) { return v !== ""; });

        if (valores.length <= 1) {
            var matches = raw.match(/\b\d{1,2}\/\d{1,2}\/\d{4}\b/g);
            if (matches && matches.length > 1) {
                return matches;
            }
        }

        return valores;
    }

    function parsearValoresPegadosPorCampo(field, texto) {
        if (!texto) return [];
        if (esCampoFecha(field)) return parsearValoresFechaPegados(texto);

        return String(texto)
            .replace(/\u0000/g, "")
            .split(/(?:\r\n|\n|\r)+/)
            .map(function (linea) {
                var limpio = (linea || "").trim();
                if (!limpio) return "";
                return limpio.split("\t")[0].trim();
            })
            .filter(function (v) { return v !== ""; });
    }

    function normalizarValorPegado(field, valorTexto) {
        if (esCampoFecha(field)) return parseDateDdMmYyyy(valorTexto);
        if (esCampoComboBolsa(field)) return obtenerValorComboBolsaValido(valorTexto);
        if (esCampoTextoEditable(field)) {
            var texto = (valorTexto || "").trim();
            if (field === "Oblea" && texto.length > 18) return null;
            return texto;
        }
        return valorTexto;
    }

    function aplicarPegadoMasivoEnColumna(field, filaInicial, valoresTexto) {
        if (!state.grid || !field || !valoresTexto || !valoresTexto.length) return;
        if (!esCampoEditablePegado(field)) return;

        var view = state.grid.dataSource.view();
        var start = Math.max(0, filaInicial || 0);
        var huboValorInvalido = false;
        var huboValorInvalidoOblea = false;
        var huboCampoBloqueado = false;
        var huboFechaInvalida = false;
        var mensajeFechasInvalidas = [];
        for (var i = 0; i < valoresTexto.length; i++) {
            var filaIdx = start + i;
            if (filaIdx >= view.length) break;

            var valorTexto = (valoresTexto[i] || "").trim();
            if (valorTexto === "") continue;

            var item = view[filaIdx];
            if (!item) continue;

            if (!esCampoEditablePegadoEnFila(item, field)) {
                huboCampoBloqueado = true;
                continue;
            }

            var valor = normalizarValorPegado(field, valorTexto);
            if (esCampoComboBolsa(field) && !valor) {
                huboValorInvalido = true;
                continue;
            }
            if (field === "Oblea" && valor === null) {
                huboValorInvalidoOblea = true;
                continue;
            }

            // Validar fechas relacionales antes de pegar
            if (esCampoFecha(field) && valor) {
                var errorFechaValidacion = validarFechaRelacional(item, field, valor);
                if (errorFechaValidacion) {
                    mensajeFechasInvalidas.push(
                        `Para el contrato ${item.ContratoSAP}, se encontró la siguiente observación: ${errorFechaValidacion}`
                    );
                    huboFechaInvalida = true;
                    continue;
                }
            }

            item.set(field, valor);
            state.dirtyItems[item.ControlDeBoletosId] = item.toJSON();
        }

        if (huboValorInvalido) {
            mostrarMensaje("Validación", "Se ignoraron los valores pegados que no coinciden con los códigos de bolsas existentes.", "warning");
        }
        if (huboValorInvalidoOblea) {
            mostrarMensaje("Validación", "Se ignoraron los valores de oblea porque superan el límite de 18 caracteres.", "warning");
        }
        if (huboCampoBloqueado) {
            mostrarMensaje("Validación", "Se ignoraron valores pegados en filas bloqueadas por reglas de negocio.", "warning");
        }
        if (huboFechaInvalida) {
            mostrarMensaje("Validación", mensajeFechasInvalidas.join("<br>"), "warning");
        }

        actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
    }

    function obtenerMatrizDesdeTextoPortapapeles(texto) {
        var raw = String(texto || "").replace(/\u0000/g, "");
        var lineas = raw.split(/\r\n|\n|\r/).filter(function (linea) {
            return linea !== "";
        });
        if (!lineas.length) return [];

        return lineas.map(function (linea) {
            return linea.split("\t").map(function (valor) {
                return (valor || "").trim();
            });
        });
    }

    function normalizarTextoContratoSAP(texto) {
        if (texto == null) return "";
        return String(texto).trim();
    }

    function formatearValorParaCopiar(field, valor) {
        if (valor == null) return "";
        if (esCampoFecha(field)) return valor ? kendo.toString(valor, "dd/MM/yyyy") : "";
        return String(valor);
    }

    function obtenerValorNormalizadoParaCampoDesdeTexto(field, texto) {
        var textoNormalizado = texto == null ? "" : String(texto).trim();
        if (textoNormalizado === "") {
            return esCampoTextoEditable(field) ? "" : null;
        }
        return normalizarValorPegado(field, textoNormalizado);
    }

    function copiarColumnaAlPortapapeles(field) {
        if (!state.grid || !field) return;

        var view = state.grid.dataSource.view() || [];
        var valoresTexto = [];

        for (var i = 0; i < view.length; i++) {
            var item = view[i];
            if (!item) {
                valoresTexto.push("");
                continue;
            }

            var valor = typeof item.get === "function" ? item.get(field) : item[field];
            valoresTexto.push(formatearValorParaCopiar(field, valor));
        }

        state.columnClipboard = {
            sourceField: field,
            values: valoresTexto
        };

        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(valoresTexto.join("\n"));
        }
    }

    function aplicarPegadoColumnaEntreCampos(sourceField, targetField, valoresTexto) {
        if (!state.grid || !targetField || !valoresTexto || !valoresTexto.length) return;
        if (!esCampoEditablePegado(targetField)) return;

        var view = state.grid.dataSource.view() || [];
        var huboValorInvalido = false;
        var huboValorInvalidoOblea = false;
        var huboCampoBloqueado = false;
        var huboFechaInvalida = false;
        var mensajesFechas = [];

        for (var i = 0; i < view.length && i < valoresTexto.length; i++) {
            var item = view[i];
            if (!item) continue;

            if (!esCampoEditablePegadoEnFila(item, targetField)) {
                huboCampoBloqueado = true;
                continue;
            }

            var valorTexto = valoresTexto[i];
            var valor = obtenerValorNormalizadoParaCampoDesdeTexto(targetField, valorTexto);

            if ((valorTexto || "").trim() !== "") {
                if (esCampoComboBolsa(targetField) && !valor) {
                    huboValorInvalido = true;
                    continue;
                }
                if (targetField === "Oblea" && valor === null) {
                    huboValorInvalidoOblea = true;
                    continue;
                }
            }

            if (esCampoFecha(targetField) && valor) {
                var errorFecha = validarFechaRelacional(item, targetField, valor);
                if (errorFecha) {
                    mensajesFechas.push(
                        "Para el contrato " + normalizarTextoContratoSAP(item.ContratoSAP) + ", se encontró la siguiente observación: " + errorFecha
                    );
                    huboFechaInvalida = true;
                    continue;
                }
            }

            item.set(targetField, valor);
            state.dirtyItems[item.ControlDeBoletosId] = item.toJSON();
        }

        if (huboValorInvalido) {
            mostrarMensaje("Validación", "Se ignoraron valores de columna copiada que no coinciden con códigos de bolsa válidos.", "warning");
        }
        if (huboValorInvalidoOblea) {
            mostrarMensaje("Validación", "Se ignoraron valores de columna copiada para oblea porque superan el límite de 18 caracteres.", "warning");
        }
        if (huboCampoBloqueado) {
            mostrarMensaje("Validación", "Se ignoraron filas bloqueadas por reglas de negocio al pegar la columna.", "warning");
        }
        if (huboFechaInvalida) {
            mostrarMensaje("Validación", mensajesFechas.join("<br>"), "warning");
        }

        state.selectedPasteField = targetField;
        state.selectedPasteRowIndex = 0;
        state.selectedPasteOrigin = "header";
        marcarColumnaSeleccionada(state.selectedPasteField);
        actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
    }

    function limpiarSeleccionRangoVisual() {
        if (!state.grid || !state.grid.tbody) return;
        state.grid.tbody.find("td").removeClass("range-selected range-anchor");
    }

    function obtenerKeyCelda(rowIndex, colIndex) {
        return rowIndex + ":" + colIndex;
    }

    function limpiarSeleccionCeldas() {
        state.selectedCells = {};
    }

    function setSeleccionCelda(rowIndex, colIndex, selected) {
        var key = obtenerKeyCelda(rowIndex, colIndex);
        if (selected) {
            state.selectedCells[key] = true;
        } else {
            delete state.selectedCells[key];
        }
    }

    function hayCeldasSeleccionadas() {
        return Object.keys(state.selectedCells || {}).length > 0;
    }

    function obtenerCeldasSeleccionadasOrdenadas() {
        return Object.keys(state.selectedCells || {})
            .map(function (key) {
                var parts = key.split(":");
                return {
                    row: parseInt(parts[0], 10),
                    col: parseInt(parts[1], 10)
                };
            })
            .filter(function (x) { return !isNaN(x.row) && !isNaN(x.col); })
            .sort(function (a, b) {
                if (a.row !== b.row) return a.row - b.row;
                return a.col - b.col;
            });
    }

    function refrescarSeleccionVisual() {
        if (!state.grid || !state.grid.tbody) return;

        var $rows = state.grid.tbody.find("tr");
        limpiarSeleccionRangoVisual();

        var seleccionadas = obtenerCeldasSeleccionadasOrdenadas();
        for (var i = 0; i < seleccionadas.length; i++) {
            var celda = seleccionadas[i];
            $rows.eq(celda.row).find("td").eq(celda.col).addClass("range-selected");
        }

        var anchor = state.rangeAnchor;
        if (anchor) {
            $rows.eq(anchor.rowIndex).find("td").eq(anchor.colIndex).addClass("range-anchor");
        }
    }

    function normalizarRango(rango) {
        if (!rango) return null;
        return {
            startRow: Math.min(rango.startRow, rango.endRow),
            endRow: Math.max(rango.startRow, rango.endRow),
            startCol: Math.min(rango.startCol, rango.endCol),
            endCol: Math.max(rango.startCol, rango.endCol)
        };
    }

    function aplicarSeleccionRango(rango) {
        if (!state.grid || !state.grid.tbody || !rango) return;

        var normalizado = normalizarRango(rango);
        limpiarSeleccionCeldas();

        for (var r = normalizado.startRow; r <= normalizado.endRow; r++) {
            for (var c = normalizado.startCol; c <= normalizado.endCol; c++) {
                setSeleccionCelda(r, c, true);
            }
        }

        refrescarSeleccionVisual();

        state.selectedRange = {
            startRow: normalizado.startRow,
            endRow: normalizado.endRow,
            startCol: normalizado.startCol,
            endCol: normalizado.endCol
        };
    }

    function setAnchorYSeleccion(rowIndex, colIndex) {
        state.rangeAnchor = { rowIndex: rowIndex, colIndex: colIndex };
        aplicarSeleccionRango({
            startRow: rowIndex,
            endRow: rowIndex,
            startCol: colIndex,
            endCol: colIndex
        });
    }

    function expandirSeleccionHasta(rowIndex, colIndex) {
        if (!state.rangeAnchor) {
            setAnchorYSeleccion(rowIndex, colIndex);
            return;
        }

        aplicarSeleccionRango({
            startRow: state.rangeAnchor.rowIndex,
            endRow: rowIndex,
            startCol: state.rangeAnchor.colIndex,
            endCol: colIndex
        });
    }

    function copiarRangoSeleccionadoAlPortapapeles() {
        if (!state.grid) return;

        var seleccionadas = obtenerCeldasSeleccionadasOrdenadas();
        if (!seleccionadas.length && state.selectedRange) {
            var rango = normalizarRango(state.selectedRange);
            if (rango) {
                for (var rr = rango.startRow; rr <= rango.endRow; rr++) {
                    for (var cc = rango.startCol; cc <= rango.endCol; cc++) {
                        seleccionadas.push({ row: rr, col: cc });
                    }
                }
            }
        }
        if (!seleccionadas.length) return;

        var view = state.grid.dataSource.view();
        var lineasPorFila = {};
        var filas = [];

        for (var i = 0; i < seleccionadas.length; i++) {
            var sel = seleccionadas[i];
            var item = view[sel.row];
            if (!item) continue;

            if (!lineasPorFila[sel.row]) {
                lineasPorFila[sel.row] = [];
                filas.push(sel.row);
            }

            var columna = state.grid.columns[sel.col];
            var field = columna && columna.field ? columna.field : null;
            var valor = field ? item.get(field) : "";
            lineasPorFila[sel.row].push(formatearValorParaCopiar(field, valor));
        }

        filas.sort(function (a, b) { return a - b; });
        var lineas = filas.map(function (row) {
            return (lineasPorFila[row] || []).join("\t");
        });

        var texto = lineas.join("\n");
        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(texto);
        }
    }

    function aplicarPegadoEnRango(startRow, startCol, matrizValores) {
        if (!state.grid || !matrizValores || !matrizValores.length) return;

        var view = state.grid.dataSource.view();
        var huboValorInvalido = false;
        var huboValorInvalidoOblea = false;
        var huboCampoBloqueado = false;
        var huboFechaInvalida = false;
        var mensajesFechas = [];

        for (var r = 0; r < matrizValores.length; r++) {
            var rowIndex = startRow + r;
            if (rowIndex >= view.length) break;

            var item = view[rowIndex];
            if (!item) continue;

            var fila = matrizValores[r] || [];
            for (var c = 0; c < fila.length; c++) {
                var colIndex = startCol + c;
                if (colIndex >= state.grid.columns.length) break;

                var columna = state.grid.columns[colIndex];
                var field = columna && columna.field ? columna.field : null;
                if (!field || !esCampoEditablePegado(field)) continue;

                var texto = (fila[c] || "").trim();
                if (texto === "") continue;

                if (!esCampoEditablePegadoEnFila(item, field)) {
                    huboCampoBloqueado = true;
                    continue;
                }

                var valor = normalizarValorPegado(field, texto);
                if (esCampoComboBolsa(field) && !valor) {
                    huboValorInvalido = true;
                    continue;
                }
                if (field === "Oblea" && valor === null) {
                    huboValorInvalidoOblea = true;
                    continue;
                }

                if (esCampoFecha(field) && valor) {
                    var errorFecha = validarFechaRelacional(item, field, valor);
                    if (errorFecha) {
                        mensajesFechas.push(
                            "Para el contrato " + normalizarTextoContratoSAP(item.ContratoSAP) + ", se encontró la siguiente observación: " + errorFecha
                        );
                        huboFechaInvalida = true;
                        continue;
                    }
                }

                item.set(field, valor);
                state.dirtyItems[item.ControlDeBoletosId] = item.toJSON();
            }
        }

        if (huboValorInvalido) {
            mostrarMensaje("Validación", "Se ignoraron valores pegados que no coinciden con códigos de bolsa válidos.", "warning");
        }
        if (huboValorInvalidoOblea) {
            mostrarMensaje("Validación", "Se ignoraron valores de oblea porque superan el límite de 18 caracteres.", "warning");
        }
        if (huboCampoBloqueado) {
            mostrarMensaje("Validación", "Se ignoraron valores pegados en celdas bloqueadas por reglas de negocio.", "warning");
        }
        if (huboFechaInvalida) {
            mostrarMensaje("Validación", mensajesFechas.join("<br>"), "warning");
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

        function manejarPegado(field, filaInicial, textoClipboard) {
            if (!esCampoEditablePegado(field)) return;
            var valores = parsearValoresPegadosPorCampo(field, textoClipboard);
            if (!valores.length) return;

            var viewActual = state.grid.dataSource.view();
            var itemInicial = viewActual[Math.max(0, filaInicial || 0)];
            if (!esCampoEditablePegadoEnFila(itemInicial, field)) return;

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

            if (colIndex >= 0) {
                state.selectedPasteColIndex = colIndex;
            }

            aplicarPegadoMasivoEnColumna(field, filaInicial, valores);

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
            state.selectedPasteField = esCampoEditablePegado(field) ? field : null;
            state.selectedPasteRowIndex = 0;
            state.selectedPasteColIndex = $(this).index();
            state.selectedPasteOrigin = "header";
            state.selectedRange = null;
            limpiarSeleccionCeldas();
            limpiarSeleccionRangoVisual();
            marcarColumnaSeleccionada(state.selectedPasteField);
            state.grid.wrapper.focus();
        });

        // ── Selección de celda por clic en el cuerpo ──
        state.grid.tbody.off("click.modifMasivaCol").on("click.modifMasivaCol", "td", function (e) {
            // Si el click ocurre dentro de DatePicker o Combo/Popup, no forzar foco al wrapper
            // para evitar que el popup se cierre inmediatamente.
            if ($(e.target).closest(".k-datepicker, .k-date-picker, .k-dropdown, .k-dropdownlist, .k-picker, .k-animation-container, .k-list-container, .k-calendar-container, .k-popup").length) {
                return;
            }

            var $cell = $(this);
            var colIndex = $cell.index();
            var columna = state.grid.columns[colIndex];
            var field = columna && columna.field ? columna.field : null;
            var rowIndex = $cell.closest("tr").index();

            state.grid.current($cell);
            state.selectedPasteField = esCampoEditablePegado(field) ? field : null;
            state.selectedPasteRowIndex = rowIndex >= 0 ? rowIndex : 0;
            state.selectedPasteColIndex = colIndex >= 0 ? colIndex : 0;
            state.selectedPasteOrigin = "cell";
            marcarColumnaSeleccionada(state.selectedPasteField);

            if ((e.ctrlKey || e.metaKey) && !e.shiftKey) {
                var key = obtenerKeyCelda(state.selectedPasteRowIndex, state.selectedPasteColIndex);
                var estabaSeleccionada = !!state.selectedCells[key];
                setSeleccionCelda(state.selectedPasteRowIndex, state.selectedPasteColIndex, !estabaSeleccionada);

                if (!hayCeldasSeleccionadas()) {
                    setSeleccionCelda(state.selectedPasteRowIndex, state.selectedPasteColIndex, true);
                }

                state.rangeAnchor = { rowIndex: state.selectedPasteRowIndex, colIndex: state.selectedPasteColIndex };
                state.selectedRange = {
                    startRow: state.selectedPasteRowIndex,
                    endRow: state.selectedPasteRowIndex,
                    startCol: state.selectedPasteColIndex,
                    endCol: state.selectedPasteColIndex
                };
                refrescarSeleccionVisual();
                return;
            }

            if (e.shiftKey && state.rangeAnchor) {
                expandirSeleccionHasta(state.selectedPasteRowIndex, state.selectedPasteColIndex);
            } else {
                setAnchorYSeleccion(state.selectedPasteRowIndex, state.selectedPasteColIndex);
            }
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
            if (!esCampoEditablePegado(field)) return;
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

            if (state.comboPopupAbierto) return;

            // No interferir si el usuario está escribiendo o navegando dentro de un editor/popup
            var $target = $(e.target);
            if ($target.closest(".k-dropdown, .k-dropdownlist, .k-picker, .k-list-container, .k-animation-container, .k-popup").length) return;
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
                state.selectedPasteField = esCampoEditablePegado(field) ? field : null;
                state.selectedPasteRowIndex = rowIndex;
                state.selectedPasteColIndex = colIndex;
                state.selectedPasteOrigin = "cell";
                marcarColumnaSeleccionada(state.selectedPasteField);

                if (e.shiftKey) {
                    expandirSeleccionHasta(rowIndex, colIndex);
                } else {
                    setAnchorYSeleccion(rowIndex, colIndex);
                }

                e.preventDefault();
            }
        });

        // ── Teclado: Enter / F2 (abrir editor) y Escape (cerrar editor) ──
        state.grid.wrapper.off("keydown.modifMasivaEditorCtrl").on("keydown.modifMasivaEditorCtrl", function (e) {
            if (state.comboPopupAbierto) return;

            var $editCell = state.grid.tbody.find("td.k-edit-cell");
            var estaEditando = $editCell.length > 0;

            // Escape: cerrar editor y volver a navegación
            if (e.key === "Escape" || e.keyCode === 27) {
                if (estaEditando) {
                    state.grid.closeCell();
                    setTimeout(function () { state.grid.wrapper.focus(); }, 0);
                    e.preventDefault();
                }
                return;
            }

            // Enter o F2: abrir editor en la celda actual (solo si no estamos editando ya)
            if (!estaEditando) {
                var esEnter = (e.key === "Enter" || e.keyCode === 13) && !e.shiftKey;
                var esF2 = e.key === "F2" || e.keyCode === 113;
                if (esEnter || esF2) {
                    var $target = $(e.target);
                    if ($target.closest(".k-dropdown, .k-dropdownlist, .k-picker, .k-list-container, .k-animation-container, .k-popup").length) return;

                    var current = state.grid.current();
                    if (!current || !current.length) return;

                    var colIdx = current.index();
                    var rowIdx = current.closest("tr").index();
                    var col = state.grid.columns[colIdx];
                    var fld = col && col.field ? col.field : null;
                    var view = state.grid.dataSource.view();
                    var itm = view[rowIdx];

                    if (fld && esCampoEditablePegado(fld) && !esCampoBloqueadoPorFila(itm, fld)) {
                        state._editandoCeldaExplicito = true;
                        state.grid.editCell(current);
                    }
                    e.preventDefault();
                }
            }
        });

        // ── Doble clic: abrir editor de celda ──
        state.grid.tbody.off("dblclick.modifMasivaEdit").on("dblclick.modifMasivaEdit", "td", function (e) {
            if ($(e.target).closest(".k-animation-container, .k-list-container, .k-popup, .k-calendar-container").length) return;

            var $cell = $(this);
            var colIndex = $cell.index();
            var rowIndex = $cell.closest("tr").index();
            var columna = state.grid.columns[colIndex];
            var field = columna && columna.field ? columna.field : null;
            var view = state.grid.dataSource.view();
            var item = view[rowIndex];

            if (!field || !esCampoEditablePegado(field)) return;
            if (esCampoBloqueadoPorFila(item, field)) return;

            state._editandoCeldaExplicito = true;
            state.grid.editCell($cell);
        });

        // ── Teclado: Delete/Supr + Ctrl+D ──
        state.grid.wrapper.off("keydown.modifMasivaDelete").on("keydown.modifMasivaDelete", function (e) {
            if (state.comboPopupAbierto) return;

            var $target = $(e.target);
            if ($target.closest(".k-dropdown, .k-dropdownlist, .k-picker, .k-list-container, .k-animation-container, .k-popup").length) return;

            var current = state.grid.current();
            if (!current || !current.length) return;

            var colIndex = current.index();
            var columna = state.grid.columns[colIndex];
            var field = columna && columna.field ? columna.field : null;
            if (!esCampoEditablePegado(field)) return;

            var row = current.closest("tr");
            var rowIndex = row.index();
            var view = state.grid.dataSource.view();
            var item = view[rowIndex];
            if (!item || esCampoBloqueadoPorFila(item, field)) return;

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
                if (field === "Oblea" && valorOrigen != null && String(valorOrigen).length > 18) {
                    mostrarMensaje("Validación", "Oblea no puede superar 18 caracteres.", "warning");
                    return;
                }
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
            if (state.comboPopupAbierto) return;

            var $target = $(e.target);
            if ($target.closest(".k-dropdown, .k-dropdownlist, .k-picker, .k-list-container, .k-animation-container, .k-popup").length) return;

            var current = state.grid.current();
            var colIndex = current && current.length ? current.index() : -1;
            var columna = colIndex >= 0 ? state.grid.columns[colIndex] : null;
            var fieldActual = columna && columna.field ? columna.field : state.selectedPasteField;
            var rowIndexActual = current && current.length ? current.closest("tr").index() : state.selectedPasteRowIndex;

            // Ctrl+C en campo editable: copiar valor actual al portapapeles
            if ((e.ctrlKey || e.metaKey) && (e.key === "c" || e.key === "C" || e.keyCode === 67)) {
                if (state.selectedPasteOrigin === "header" && esCampoEditablePegado(state.selectedPasteField)) {
                    e.preventDefault();
                    copiarColumnaAlPortapapeles(state.selectedPasteField);
                    return;
                }

                if (hayCeldasSeleccionadas() || state.selectedRange) {
                    e.preventDefault();
                    state.columnClipboard = null;
                    copiarRangoSeleccionadoAlPortapapeles();
                    return;
                }

                if (!esCampoEditablePegado(fieldActual) || rowIndexActual < 0) return;

                var viewCopy = state.grid.dataSource.view();
                var itemCopy = viewCopy[rowIndexActual];
                if (!itemCopy || esCampoBloqueadoPorFila(itemCopy, fieldActual)) return;

                var valor = itemCopy.get(fieldActual);
                var texto = "";
                if (esCampoFecha(fieldActual)) texto = valor ? kendo.toString(valor, "dd/MM/yyyy") : "";
                else texto = valor == null ? "" : String(valor);

                if (navigator.clipboard && navigator.clipboard.writeText) {
                    e.preventDefault();
                    state.columnClipboard = null;
                    navigator.clipboard.writeText(texto);
                }
                return;
            }

            // Ctrl+V en campo editable: pegar desde portapapeles comenzando en la celda actual
            if ((e.ctrlKey || e.metaKey) && (e.key === "v" || e.key === "V" || e.keyCode === 86)) {
                if (state.selectedPasteOrigin === "header" && esCampoEditablePegado(state.selectedPasteField)) {
                    e.preventDefault();

                    if (state.columnClipboard && state.columnClipboard.values && state.columnClipboard.values.length) {
                        aplicarPegadoColumnaEntreCampos(
                            state.columnClipboard.sourceField,
                            state.selectedPasteField,
                            state.columnClipboard.values
                        );
                    } else if (navigator.clipboard && navigator.clipboard.readText) {
                        navigator.clipboard.readText().then(function (textoPegado) {
                            var valores = parsearValoresPegadosPorCampo(state.selectedPasteField, textoPegado);
                            if (!valores.length) {
                                valores = obtenerMatrizDesdeTextoPortapapeles(textoPegado).map(function (fila) {
                                    return (fila && fila.length) ? (fila[0] || "") : "";
                                });
                            }
                            aplicarPegadoColumnaEntreCampos(null, state.selectedPasteField, valores);
                        }).catch(function () {
                            state.grid.wrapper.focus();
                        });
                    }
                    return;
                }

                if (!esCampoEditablePegado(fieldActual) && !state.selectedRange) return;

                var viewPaste = state.grid.dataSource.view();
                var itemPaste = viewPaste[rowIndexActual];
                if (!esCampoEditablePegadoEnFila(itemPaste, fieldActual)) return;

                state.selectedPasteField = fieldActual;
                state.selectedPasteRowIndex = rowIndexActual >= 0 ? rowIndexActual : 0;
                state.selectedPasteColIndex = colIndex >= 0 ? colIndex : state.selectedPasteColIndex;
                marcarColumnaSeleccionada(state.selectedPasteField);

                if (navigator.clipboard && navigator.clipboard.readText) {
                    e.preventDefault();
                    navigator.clipboard.readText().then(function (textoPegado) {
                        var matriz = obtenerMatrizDesdeTextoPortapapeles(textoPegado);
                        if (matriz.length && (matriz.length > 1 || (matriz[0] && matriz[0].length > 1))) {
                            aplicarPegadoEnRango(state.selectedPasteRowIndex, state.selectedPasteColIndex, matriz);
                        } else {
                            manejarPegado(state.selectedPasteField, state.selectedPasteRowIndex, textoPegado);
                        }
                    }).catch(function () {
                        // Mantener foco/celda actual si no hay permiso de clipboard
                        state.grid.wrapper.focus();
                    });
                }
            }
        });

        // Inicializa selección visible al primer foco de celda
        var current = state.grid.current();
        if (current && current.length) {
            setAnchorYSeleccion(current.closest("tr").index(), current.index());
        }
    }

    // Funciones públicas
    return {
        init: async function () {
            if (state.datosInicializados) return;
            this.configurarEventos();
            await cargarBolsaSAP();
            this.inicializarGrid();
            state.datosInicializados = true;
        },

        configurarEventos: function () {
            var self = this;

            controlNegocioSAP
                .off("paste keypress")
                .each(function () {
                    ui.bindContractsPaste($(this));
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
                            serverPaging: false,
                            serverSorting: false,
                            serverFiltering: false,
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
                                errors: "Errors",
                                model: {
                                    id: "ControlDeBoletosId",
                                    fields: {
                                        ControlDeBoletosId: { type: "number", editable: false },
                                        NegocioId: { type: "number", editable: false },
                                        ContratoSAP: { type: "string", editable: false },
                                        TipoBoleto: { type: "string", editable: false },
                                        SeguimientoBoletoId: { type: "number", editable: false },
                                        PreCertificacionId: { type: "number", editable: false },
                                        Oblea: { type: "string" },
                                        PreCertificacionBolsa: { type: "string" },
                                        BolsaSellado: { type: "string" },
                                        FechaCertificacion: { type: "date" },
                                        FechaVencimientoCertificacion: { type: "date" },
                                        FechaRecepBoleto: { type: "date" },
                                        FechaEnviadoFirma: { type: "date" },
                                        FechaEnvioBolsa: { type: "date" },
                                        FechaEnvioAfip: { type: "date" },
                                        FechaRecibFirma: { type: "date" },
                                        FechaVueltaBolsa: { type: "date" },
                                        FechaVueltaAfip: { type: "date" },
                                        FechaEnvioSellado: { type: "date" },
                                        EsCartaOferta: { type: "boolean" },
                                        OperaSinOblea: { type: "boolean" }
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
                        scrollable: { virtual: true },
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
                        pageable: false,
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
                                field: "BolsaSellado",
                                title: "Bolsa",
                                width: 140,
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "BolsaSellado"); },
                                editor: comboBolsaEditor
                            },
                            {
                                field: "FechaRecepBoleto",
                                title: "Fecha Recepción Boleto",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaRecepBoleto) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaRecepBoleto"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnviadoFirma",
                                title: "Fecha Envío a Firma",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnviadoFirma) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaEnviadoFirma"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnvioBolsa",
                                title: "Fecha Envío Obleado Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioBolsa) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaEnvioBolsa"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnvioAfip",
                                title: "Fecha Envío Arca",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioAfip) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaEnvioAfip"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaRecibFirma",
                                title: "Fecha Recibido de Firma",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaRecibFirma) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaRecibFirma"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaVueltaBolsa",
                                title: "Fecha Recepción Obleado Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVueltaBolsa) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaVueltaBolsa"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaVueltaAfip",
                                title: "Fecha Recepción Arca",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVueltaAfip) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaVueltaAfip"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "Oblea",
                                title: "Oblea",
                                width: 120,
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "Oblea"); },
                                editor: textCellEditor
                            },
                            {
                                field: "PreCertificacionBolsa",
                                title: "Codigo Bolsa",
                                width: 140,
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "PreCertificacionBolsa"); },
                                editor: comboBolsaEditor
                            },
                            {
                                field: "FechaCertificacion",
                                title: "F. Certificación Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaCertificacion) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaCertificacion"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaVencimientoCertificacion",
                                title: "F. Vencimiento Bolsa",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaVencimientoCertificacion) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaVencimientoCertificacion"); },
                                editor: dateCellEditor
                            },
                            {
                                field: "FechaEnvioSellado",
                                title: "Fecha Envío Sellado",
                                width: 200,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaEnvioSellado) #",
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "FechaEnvioSellado"); },
                                editor: dateCellEditor
                            },
                        ],
                        beforeEdit: function (e) {
                            // Evita apertura de editor por click simple (incluye fechas/calendario).
                            // Solo permitimos edición iniciada explícitamente por doble click, Enter o F2.
                            if (!state._editandoCeldaExplicito) {
                                e.preventDefault();
                            }
                        },
                        edit: function (e) {
                            // Bloquear apertura automática de editor con un solo clic.
                            // El editor solo abre con doble clic, Enter o F2.
                            if (!state._editandoCeldaExplicito) {
                                e.preventDefault();
                                return;
                            }
                            state._editandoCeldaExplicito = false;
                        },
                        cellClose: function (e) {
                            state._editandoCeldaExplicito = false;
                            if (!e.model) return;
                            state.dirtyItems[e.model.ControlDeBoletosId] = e.model.toJSON();
                            actualizarBoton(controlGuardarFechas, Object.keys(state.dirtyItems).length > 0);
                        },
                        dataBound: function (e) {
                            if (!state.columnasAjustadas) {
                                autoFitColumnas(e.sender);
                                state.columnasAjustadas = true;
                            }

                            var $rows = e.sender.tbody.find("tr");
                            $rows.each(function () {
                                var item = e.sender.dataItem(this);
                                var $tds = $(this).find("td");
                                e.sender.columns.forEach(function (col, idx) {
                                    if (!col || !col.field) return;
                                    var bloqueado = esCampoBloqueadoPorFila(item, col.field);
                                    $tds.eq(idx).toggleClass("campo-bloqueado", bloqueado);
                                });
                            });

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

        guardarFechas: async function () {
            var self = this;
            var items = Object.keys(state.dirtyItems).map(function (k) { return state.dirtyItems[k]; });

            if (!items.length) {
                mostrarMensaje("Información", "No hay fechas modificadas para guardar.", "info");
                return;
            }

            BlockUi('Guardando...');
            try {

                var response = await MSExecuteOnServerAsync(config.urls.guardarBoletosParaModificarFechas, items);
                if (response.success) {
                    actualizarBoton(controlGuardarFechas, false);
                    MensInfo("Se guardaron los cambios a los contratos de manera exitosa.");
                    if (state.grid) {
                        state.grid.dataSource.read();
                    }
                } else {
                    MensErr(response.message);
                }


            } catch (e) {
                console.error("Error al guardaron los cambios a los contratos de manera exitosa:", e);
            } finally {
                $.unblockUI();
            }
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
