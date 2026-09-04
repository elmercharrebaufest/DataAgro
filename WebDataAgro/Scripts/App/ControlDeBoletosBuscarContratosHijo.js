var ControlDeBoletosBuscarContratosHijo = (function () {
    "use strict";
    var ui = window.ControlDeBoletosUI;
    var config = {
        urls: {
            getContratosHijos: "/ControlDeBoletos/ListarContratosHijos",
            createControlBoletos: "/ControlDeBoletos/AgregarContratosHijos",
        },
        modalId: "#modalContratosHijo",
        gridId: "#boletos-grid-contratos-hijo",
        spinnerId: "loadingSpinnerContratosHijo",
    };

    var state = {
        grid: null,
        cargando: false,
    };

    let controlContratoHijo;
    let controlBuscarContrato;
    let controlLimpiarFiltros;
    let controlAgregarContratos;
    function bindControls() {
        var $form = $("#modalContratosHijo #frmContratoHijo");
        if (!$form.length) $form = $("#frmContratoHijo").first();
        controlContratoHijo = $form.find("#NegocioSAP");
        controlBuscarContrato = $form.find("#filtrarBoletos");
        controlLimpiarFiltros = $form.find("#limpiarFiltros");
        controlAgregarContratos = $form.find("#agregarContratos");
    }

    // ── Estilos globales del grid ────────────────────────────────────────────
    function inyectarEstilosGrid() {
        if ($("#grid-boletos-contratos-hijo-styles").length) return;
        $("<style id='grid-boletos-contratos-hijo-styles'>").text(`
            #boletos-grid-contratos-hijo .k-grid-header th {
                font-weight: bold !important;
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
                white-space: nowrap;
                background-color: #f5f5f5;
            }
            #boletos-grid-contratos-hijo .k-grid-content td {
                font-size: 13px !important;
                font-family: Arial, sans-serif !important;
            }
            #boletos-grid-contratos-hijo .k-grid-header-wrap {
                overflow: hidden !important;
            }
            #boletos-grid-contratos-hijo .k-grid-content {
                overflow-x: auto !important;
                overflow-y: auto !important;
            }
            #boletos-grid-contratos-hijo .k-grid-header-wrap table,
            #boletos-grid-contratos-hijo .k-grid-content table {
                table-layout: fixed;
            }
            #boletos-grid-contratos-hijo .k-grid-content tr:hover td,
            #boletos-grid-contratos-hijo .k-grid-content tr.k-state-hover td {
                color: #333 !important;
            }
            #boletos-grid-contratos-hijo .k-grid-content tr.k-state-selected td {
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

    // ======================
    // Funciones privadas
    // ======================
    function mostrarSpinner(mostrar) {
        ui.toggleSpinner(mostrar, config.spinnerId);
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
    function obtenerFiltros() {
        return {
            negocioSAP: controlContratoHijo.val()
        };
    }
    function obtenerNegociosIdsSeleccionadosCsv() {
        if (!state.grid) return "";

        var ids = [];

        $(config.gridId)
            .find("tbody tr")
            .each(function () {
                var $row = $(this);
                var $check = $row.find(".chk-contrato-hijo");
                if (!$check.length || !$check.is(":checked")) return;

                var item = state.grid.dataItem($row);
                var negocioId = item ? (item.NegocioId || item.negocioId) : null;

                if (negocioId !== null && negocioId !== undefined && negocioId !== "") {
                    ids.push(String(negocioId));
                }
            });

        // Evitar duplicados por seguridad ante re-renderizaciones.
        ids = ids.filter(function (id, index) {
            return ids.indexOf(id) === index;
        });

        return ids.join(",");
    }
    function obtenerRequest() {
        var negociosIds = obtenerNegociosIdsSeleccionadosCsv();

        return {
            negociosIds: negociosIds,
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!controlContratoHijo.val()) errores.push("Debe seleccionar un contrato hijo");

        return errores;
    }

    // ======================
    // API pública
    // ======================
    return {
        inicializar: async function (NegocioId) {
            bindControls();
        },
        abrir: function (NegocioId) {
            bindControls();
            controlContratoHijo.val("");
            $("#modalContratosHijo").modal("show");
            mostrarSpinner(false);
            this.configurarEventos();
            this.filtrarBoletos();
        },
        inicializarGrid: function () {
            var self = this;

            inyectarEstilosGrid();

            try {
                state.grid = $(config.gridId)
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
                                    var filtros = obtenerFiltros();
                                    var parametros = {
                                        contratosSAP: filtros.negocioSAP
                                    };

                                    MSExecuteOnServerAsync(config.urls.getContratosHijos, parametros)
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
                                        Seleccionado: { type: "boolean", defaultValue: true },
                                        ContratoSAP: { type: "string" },
                                        Material: { type: "string" },
                                        ControlDeBoletosEstado: { type: "string" },
                                        FechaGeneracion: { type: "date" },
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
                            },
                        },
                        height: 400,
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
                        pageable: false,
                        navigatable: false,
                        columns: [
                            {
                                field: "Seleccionado",
                                title: "",
                                width: 45,
                                sortable: false,
                                filterable: false,
                                headerAttributes: {
                                    style: "text-align:center;"
                                },
                                attributes: {
                                    style: "text-align:center;"
                                },
                                template: function (dataItem) {
                                    let mensaje = dataItem.ExisteControlDeBoletos == true ? 'El contrato hijo ya tiene un control de boletos asociado y no puede ser seleccionado.': '';
                                    var checked = dataItem.ExisteControlDeBoletos == true
                                        ? ''
                                        : (dataItem.Seleccionado !== false ? 'checked="checked"' : '');

                                    var disabled = dataItem.ExisteControlDeBoletos == true
                                        ? 'disabled="disabled"'
                                        : '';

                                    return '<input type="checkbox" class="chk-contrato-hijo" ' +
                                        checked + ' ' +
                                        ' title="' + mensaje + '" ' +
                                        disabled + ' />';
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
                                        field: "ContratoSAP",
                                        title: "Contrato Madre",
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
                                    }
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
                        ],
                        dataBound: function (e) {
                            autoFitColumnas(e.sender);
                            state.columnasAjustadas = true;
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
        configurarEventos: function () {
            var self = this;

            $(config.modalId)
                .off("hidden.bs.modal.spinner")
                .on("hidden.bs.modal.spinner", function () {
                    mostrarSpinner(false);
                });

            controlContratoHijo
                .off("paste keypress")
                .each(function () {
                    ui.bindContractsPaste($(this), function () {
                    });
                });

            controlBuscarContrato
                .off("click")
                .on("click", async function () {
                    self.filtrarBoletos();
                });

            controlLimpiarFiltros
                .off("click")
                .on("click", function () {
                    self.limpiarFiltros();
                });
            controlAgregarContratos
                .off("click")
                .on("click", async function () {
                    self.guardar();
                });
            
        },
        filtrarBoletos: function () {
            state.columnasAjustadas = false;

            if (state.grid) {
                if (state.grid.dataSource.page() !== 1) {
                    state.grid.dataSource.page(1);
                } else {
                    state.grid.dataSource.read();
                }
            } else {
                this.inicializarGrid();
            }
        },
        guardar: async function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                MensAlerta(errores.join("<br>"));
                return;
            }

            state.cargando = true;
            BlockUi('Guardando...');
            var request = obtenerRequest();

            if (!request.negociosIds) {
                MensAlerta("Debe seleccionar al menos un contrato hijo.");
                $.unblockUI();
                state.cargando = false;
                return;
            }

            try {
                var response = await MSExecuteOnServerAsync(config.urls.createControlBoletos, request);
                if (!response) return;

                if (response.success) {
                    MensInfo(response.message);
                    this.cerrar();
                } else {
                    var mensajeError = response.message || "";
                    mensajeError = mensajeError
                        .split(";")
                        .map(function (item) { return $.trim(item); })
                        .filter(function (item) { return item !== ""; })
                        .join("<br>");

                    MensErr(mensajeError || response.message);
                }
            } catch (e) {
                console.error("Error al guardar contrato:", e);
            } finally {
                $.unblockUI();
                state.cargando = false;
            }
        },
        limpiarFiltros: function () {
            controlContratoHijo.val("");
            this.filtrarBoletos();
        },
        cerrar: function () {
            mostrarSpinner(false);
            $(config.modalId).modal("hide");
        },
    };
})();
