// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    "use strict";

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
        listaBolsaSAP: null,
        listaBolsa: null,
        comboPopupAbierto: false
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

    async function cargarBolsaSAP() {
        state.listaBolsaSAP = await MSExecuteGetOnServerAsync(config.urls.getBolsaSAP);
        state.listaBolsa = await MSExecuteGetOnServerAsync(config.urls.getBolsa);

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
                        '<option value="' + item.Value + '">' + item.Value + "</option>",
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
            #boletos-grid .k-grid-content td.campo-bloqueado {
                background-color: #efefef !important;
                color: #999 !important;
                cursor: not-allowed !important;
            }
            #boletos-grid .k-grid-content td.campo-bloqueado .k-input,
            #boletos-grid .k-grid-content td.campo-bloqueado .k-textbox,
            #boletos-grid .k-grid-content td.campo-bloqueado .k-dropdown,
            #boletos-grid .k-grid-content td.campo-bloqueado .k-datepicker {
                background-color: #efefef !important;
                color: #999 !important;
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
        var preCertificacionId = obtenerValorFila(item, "PreCertificacionId");
        var existePreCertificacion = preCertificacionId !== null && preCertificacionId !== undefined && preCertificacionId !== "";
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
                "FechaEnvioBolsa",
                "FechaVueltaBolsa",
                "FechaRecibFirma",
                "FechaEnviadoFirma",
                "FechaEnvioSellado",
                "FechaEnvioAfip",
                "FechaVueltaAfip",
                "FechaEnvioSellado"
            ].indexOf(field) >= 0;
        }
        if (!existePreCertificacion) {
            return [
                "Oblea",
                "FechaCertificacion",
                "FechaVencimientoCertificacion",
                "PreCertificacionBolsa"
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
        var listaBase = state.listaBolsa || [];
        var listaSap = state.listaBolsaSAP || [];

        var sapPorId = {};
        for (var i = 0; i < listaSap.length; i++) {
            var idKey = String(listaSap[i].Text || "").trim();
            var sapValue = String(listaSap[i].Value || "").trim();
            if (idKey && sapValue) sapPorId[idKey] = sapValue;
        }

        var opciones = [];
        for (var j = 0; j < listaBase.length; j++) {
            var item = listaBase[j];
            var id = String(item.Value || "").trim();
            var codigoSap = sapPorId[id] || "";
            if (!codigoSap) continue;
            opciones.push({ Text: item.Text, Value: codigoSap });
        }

        return opciones;
    }

    function obtenerValorComboBolsaValido(valorTexto) {
        var texto = (valorTexto || "").trim();
        if (!texto) return null;

        var opciones = obtenerOpcionesBolsaParaCombo();
        var upper = texto.toUpperCase();
        for (var i = 0; i < opciones.length; i++) {
            var value = String(opciones[i].Value || "").trim();
            if (value && value.toUpperCase() === upper) return value;
        }
        return null;
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
            marcarColumnaSeleccionada(state.selectedPasteField);
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
            var currentItem = state.grid.dataSource.view()[rowIndex];
            var currentField = state.grid.columns[colIndex] && state.grid.columns[colIndex].field ? state.grid.columns[colIndex].field : null;
            if (esCampoBloqueadoPorFila(currentItem, currentField)) return;

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
                marcarColumnaSeleccionada(state.selectedPasteField);

                e.preventDefault();
            }
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
                    navigator.clipboard.writeText(texto);
                }
                return;
            }

            // Ctrl+V en campo editable: pegar desde portapapeles comenzando en la celda actual
            if ((e.ctrlKey || e.metaKey) && (e.key === "v" || e.key === "V" || e.keyCode === 86)) {
                if (!esCampoEditablePegado(fieldActual)) return;

                var viewPaste = state.grid.dataSource.view();
                var itemPaste = viewPaste[rowIndexActual];
                if (!esCampoEditablePegadoEnFila(itemPaste, fieldActual)) return;

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
            await cargarBolsaSAP();
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
                                field: "PreCertificacionBolsa",
                                title: "Codigo Bolsa",
                                width: 140,
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "PreCertificacionBolsa"); },
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
                                field: "BolsaSellado",
                                title: "Bolsa",
                                width: 140,
                                editable: function (dataItem) { return !esCampoBloqueadoPorFila(dataItem, "BolsaSellado"); },
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
