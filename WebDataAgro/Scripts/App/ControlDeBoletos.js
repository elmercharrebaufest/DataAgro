// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    "use strict";

    var config = {
        urls: {
            getMateriales: "/ControlDeBoletos/GetMateriales",
            getEstados: "/ControlDeBoletos/GetEstadosControl",
            getComerciales: "/ControlDeBoletos/GetComerciales",
            getProveedores: "/ControlDeBoletos/GetProveedores",
            getBolsaCompraNet: "/ControlDeBoletos/GetBolsaCompraNet",
            getBoletos: "/ControlDeBoletos/GetBoletos",
            getContrato: "/ControlDeBoletos/ObtenerDetalleContrato",
            processControlMasivo: "/ControlDeBoletos/ControlMasivo",
            exportBoletosExcel: "/ControlDeBoletos/ExportarBoletosExcel"
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

    let controlFiltrarBoletos = $("#frmPendienteControl #filtrarBoletos");
    let controlLimpiarFiltros = $("#frmPendienteControl #limpiarFiltros");
    let controlExportarExcel = $("#frmPendienteControl #exportarExcel");

    let controlSelectAll = $("#frmPendienteControl #selectAll");

    let controlIniciarControlMasivo = $("#frmPendienteControl #iniciarControlMasivo");
    let controlFinalizarControlMasivo = $("#frmPendienteControl #finalizarControlMasivo");

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false,
    };

    const controlMasivoAccion = Object.freeze({
        ControlIniciado: 1,
        RegistroDatosOblea: 2,
        CertificacionCompletada: 3,
        ControlFinalizado: 4,
    });

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

    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = selector;
        $select.html('<option value="">' + textoCarga + "</option>");

        try {
            var data = MSExecuteGetOnServer(url);
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

        $headerCells.each(function (colIdx) {
            var colDef = columns[colIdx];
            var hasField = colDef && colDef.field && colDef.field !== "Selected";

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

    // Funciones públicas
    return {
        init: function () {
            if (state.datosInicializados) return;
            this.inicializarFechas();
            this.cargarDatosIniciales();
            this.configurarEventos();
            this.inicializarGrid();

            state.datosInicializados = true;
        },

        cargarDatosIniciales: function () {
            cargarDropdown(
                config.urls.getMateriales,
                controlMaterial,
                "Cargando...",
                "Todos los materiales",
            );
            cargarDropdown(
                config.urls.getEstados,
                controlEstadoControl,
                "Cargando...",
                "Todos los estados",
            );
            cargarDropdown(
                config.urls.getComerciales,
                controlComercial,
                "Cargando...",
                "Todos los comerciales",
            );
            cargarDropdown(
                config.urls.getProveedores,
                controlProveedor,
                "Cargando...",
                "Todos los proveedores",
            );
            cargarDropdown(
                config.urls.getBolsaCompraNet,
                controlBolsaCompraNet,
                "Cargando...",
                "Todas las Bolsas",
            );
        },

        configurarEventos: function () {
            var self = this;

            // Validación en tiempo real para campos numéricos
            $("#NegocioSAP-desde, #NegocioSAP-hasta").on("input", function () {
                var valor = this.value;
                if (!validarNumero(valor)) {
                    this.setCustomValidity("Ingrese un número válido");
                    $(this).addClass("is-invalid");
                } else {
                    this.setCustomValidity("");
                    $(this).removeClass("is-invalid");
                }
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

            // Selección múltiple
            controlSelectAll
                .off("change")
                .on("change", function () {
                    self.seleccionarTodos(this.checked);
                });

            // Control masivo
            controlIniciarControlMasivo
                .off("click")
                .on("click", function () {
                    self.iniciarControlMasivo();
                });

            controlFinalizarControlMasivo
                .off("click")
                .on("click", function () {
                    self.finalizarControlMasivo();
                });

            // Cards de progreso como filtros rápidos
            $(".progress-card[data-filter]")
                .off("click")
                .on("click", function () {
                    var filtro = $(this).data("filter");
                    self.aplicarFiltroRapido(filtro);
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
                                read: {
                                    url: config.urls.getBoletos,
                                    type: "POST",
                                    dataType: "json",
                                    contentType: "application/json; charset=utf-8",
                                },
                                parameterMap: function (options, operation) {
                                    if (operation === "read") {
                                        // Combinar los filtros personalizados con las opciones de Kendo
                                        var filtros = self.obtenerFiltros();
                                        var parametros = {
                                            // Opciones de Kendo (paginación, sorting)
                                            page: options.page || 1,
                                            pageSize: options.pageSize || 50,
                                            skip: options.skip || 0,
                                            take: options.take || 50,
                                            sort: options.sort || [],
                                            // Filtros personalizados
                                            negocioSAP: filtros.negocioSAP,
                                            materialId: filtros.materialId,
                                            estadoControlId: filtros.estadoControlId,
                                            esConfirma: filtros.esConfirma,
                                            fechaCargaDesde: filtros.fechaCargaDesde,
                                            fechaCargaHasta: filtros.fechaCargaHasta,
                                            proveedor: filtros.proveedor,
                                            bolsaId: filtros.bolsaId,
                                            comercialId: filtros.comercialId,
                                        };
                                        return kendo.stringify(parametros);
                                    }
                                    return kendo.stringify(options);
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
                                field: "Selected",
                                title:
                                    "<input type='checkbox' id='gridSelectAll' class='form-check-input' />",
                                template:
                                    "<input type='checkbox' class='row-checkbox form-check-input' data-id='#=Id#' />",
                                width: 50,
                                sortable: false,
                                filterable: false,
                            },
                            {
                                field: "TipoBoleto",
                                title: "Boleto",
                                width: 80,
                            },
                            {
                                field: "TipoAltaConfirma",
                                title: "Tipo Alta Confirma",
                                width: 80
                            },
                            {
                                field: "BolsaCompraNet",
                                title: "Bolsa",
                                width: 100
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
                                field: "EstadoConfirma",
                                title: "Estado Confirma",
                                width: 100,
                            },
                            {
                                field: "ControlDeBoletosEstado",
                                title: "Estado",
                                width: 100,
                            },
                            {
                                field: "FechaCreacion",
                                title: "Fecha Creación",
                                width: 120,
                                format: "{0:dd/MM/yyyy}",
                                template: "#= formatearFecha(FechaCreacion) #",
                            },
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
                            {
                                title: "Acciones",
                                width: 150,
                                template: function (dataItem) {
                                    return self.generarBotonesAccion(dataItem);
                                },
                                sortable: false,
                                filterable: false,
                            },
                        ],
                        dataBound: function (e) {
                            autoFitColumnas(e.sender);
                            self.configurarEventosGrid();
                            self.actualizarSeleccion();
                        },
                        change: function (e) {
                            self.actualizarBotonesControlMasivo();
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
            var self = this;

            $("#gridSelectAll")
                .off("change")
                .on("change", function () {
                    self.seleccionarTodos(this.checked);
                });

            // Checkboxes individuales
            $(".row-checkbox")
                .off("change")
                .on("change", function () {
                    self.actualizarBotonesControlMasivo();

                    // Actualizar estado del checkbox principal
                    var total = $(".row-checkbox").length;
                    var seleccionados = $(".row-checkbox:checked").length;
                    $("#gridSelectAll").prop(
                        "indeterminate",
                        seleccionados > 0 && seleccionados < total,
                    );
                    $("#gridSelectAll").prop("checked", seleccionados === total);
                });
        },

        obtenerFiltros: function () {
            return {
                negocioSAP: controlNegocioSAP.val().trim() || null,
                materialId: controlMaterial.val() || null,
                estadoControlId: controlEstadoControl.val() || null,
                esConfirma: controlEsConfirma.is(":checked"),
                fechaCargaDesde: controlFechaCargaDesde.val() || null,
                fechaCargaHasta: controlFechaCargaHasta.val() || null,
                proveedor: controlProveedor.val() || null,
                bolsaId: controlBolsaCompraNet.val() || null,
                comercialId: controlComercial.val() || null,
            };
        },

        validarFiltros: function () {
            var filtros = this.obtenerFiltros();
            var errores = [];

            const sinFiltrosPrincipales =
                !filtros.negocioSAP &&
                !filtros.fechaCargaDesde &&
                !filtros.fechaCargaHasta;

            const sinFiltrosSecundarios =
                !filtros.proveedorId &&
                !filtros.comercialId &&
                !filtros.materialId &&
                !filtros.bolsaId;

            if (sinFiltrosPrincipales && sinFiltrosSecundarios) {
                return false;
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

            controlNegocioSAPDesde
                .val("")
                .removeClass("is-invalid");

            controlNegocioSAPHasta
                .val("")
                .removeClass("is-invalid");

            controlMaterial.val("");
            controlEstadoControl.val("");
            controlComercial.val("");
            controlProveedor.val("");
            controlBolsaCompraNet.val("");

            controlFechaCargaDesde.val("");
            controlFechaCargaHasta.val("");

            controlEsConfirma.prop("checked", false);

            this.filtrarBoletos();
        },

        aplicarFiltroRapido: function (estado) {
            this.limpiarFiltros();

            var estadoId = "";
            switch (estado) {
                case "pendiente":
                    estadoId = "1";
                    break;
                case "proceso":
                    estadoId = "2";
                    break;
                case "completado":
                    estadoId = "3";
                    break;
                case "certificado":
                    estadoId = "4";
                    break;
            }

            if (estadoId) {
                controlEstadoControl.val(estadoId);
                this.filtrarBoletos();
            }
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

        seleccionarTodos: function (seleccionar) {
            $(".row-checkbox").prop("checked", seleccionar);
            this.actualizarBotonesControlMasivo();
        },

        actualizarSeleccion: function () {
            this.actualizarBotonesControlMasivo();
        },

        actualizarBotonesControlMasivo: function () {
            var seleccionados = $(".row-checkbox:checked").length;
            var habilitado = seleccionados > 0;

            actualizarBoton(controlIniciarControlMasivo, habilitado);
            actualizarBoton(controlFinalizarControlMasivo, habilitado);

            // Actualizar checkbox principal
            var total = $(".row-checkbox").length;
            $("#selectAll").prop("checked", seleccionados === total && total > 0);
        },
        inicializarFechas: function () {
            const hoy = new Date();
            const desde = new Date(hoy);
            desde.setDate(hoy.getDate() - 30);

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
        iniciarControlMasivo: function () {
            var ids = this.obtenerSeleccionados();
            if (ids.length === 0) {
                mostrarMensaje(
                    "Atención",
                    "Debe seleccionar al menos un boleto para iniciar el control",
                    "warning",
                );
                return;
            }

            var mensaje =
                "¿Está seguro de iniciar el control para " +
                ids.length +
                " boleto(s) seleccionado(s)?";
            if (confirm(mensaje)) {
                this.procesarControlMasivo("iniciar", ids);
            }
        },

        finalizarControlMasivo: function () {
            var ids = this.obtenerSeleccionados();
            if (ids.length === 0) {
                mostrarMensaje(
                    "Atención",
                    "Debe seleccionar al menos un boleto para finalizar el control",
                    "warning",
                );
                return;
            }

            var mensaje =
                "¿Está seguro de finalizar el control para " +
                ids.length +
                " boleto(s) seleccionado(s)?";
            if (confirm(mensaje)) {
                this.procesarControlMasivo("finalizar", ids);
            }
        },

        obtenerSeleccionados: function () {
            var ids = [];
            $(".row-checkbox:checked").each(function () {
                var id = $(this).data("id");
                if (id) {
                    ids.push(id);
                }
            });
            return ids;
        },

        procesarControlBoleto: function (accion, ids) {
            var self = this;

            if (!ids || ids.length === 0) {
                mostrarMensaje("Error", "No hay elementos seleccionados", "warning");
                return;
            }

            // Deshabilitar botones durante el procesamiento
            actualizarBoton(controlIniciarControlMasivo, false);
            actualizarBoton(controlFinalizarControlMasivo, false);

            let request = {
                AccionControlDeBoletos: accion,
                ControlDeBoletoIds: ids,
            };

            const response = MSExecuteOnServer(config.urls.processControlMasivo, request);
            if (response.success) {
                self.actualizarBotonesControlMasivo();
                self.filtrarBoletos();
            }
        },

        procesarControlMasivo: function (accion, ids) {
            var self = this;

            if (!ids || ids.length === 0) {
                mostrarMensaje("Error", "No hay elementos seleccionados", "warning");
                return;
            }

            // Deshabilitar botones durante el procesamiento
            actualizarBoton(controlIniciarControlMasivo, false);
            actualizarBoton(controlFinalizarControlMasivo, false);

            let request = {
                AccionControlDeBoletos: accion,
                ControlDeBoletoIds: ids,
            };

            const response = MSExecuteOnServer(config.urls.processControlMasivo, request);

            if (response) {
                self.actualizarBotonesControlMasivo();
            }
        },

        generarBotonesAccion: function (data) {
            var botones = [];
            botones.push(
                '<button class="btn btn-sm btn-outline-secondary btn-acciones tooltip-custom" onclick="ControlBoletos.visualizarContrato(' +
                data.NegocioId +
                ')" title="Visualizar Contrato"><i class="fa fa-file-text-o"></i><span class="tooltiptext"></span></button>',
            );
            if (data.ControlIniciado) {
                botones.push(
                    '<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlDeBoletosGestion.abrir({ControlDeBoletosId:' +
                    data.Id + ',SeguimientoBoletoId:' + (data.SeguimientoBoletoId || 0) +
                    ',PreCertificacionId:' + (data.PreCertificacionId || 0) +
                    ',NegocioId:' + data.NegocioId +
                    '})" title="Gestión Control"><i class="fa fa-tasks"></i><span class="tooltiptext"></span></button>',
                );
                if (data.EsConfirma) {

                    botones.push(
                        '<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlBoletosTracking.abrir(' +
                        data.Id +
                        ')" title="Tracking Boleto"><i class="fa fa-history"></i><span class="tooltiptext"></span></button>',
                    );
                }
            }
            if (!data.ControlIniciado) {
                botones.push(
                    '<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlBoletos.iniciarControl(' +
                    data.Id +
                    ')" title="Iniciar Control"><i class="fa fa-play"></i><span class="tooltiptext"></span></button>',
                );
            }

            return (
                '<div class="btn-group" role="group">' + botones.join(" ") + "</div>"
            );
        },

        iniciarControl: function (id) {
            this.procesarControlBoleto(controlMasivoAccion.ControlIniciado, [id]);
        },

        finalizarControl: function (id) {
            this.procesarControlBoleto(controlMasivoAccion.ControlFinalizado, [id]);
        },

        visualizarContrato: function (id) {
            // Mostrar loader o indicador de carga

            if (!id || id <= 0) {
                alert("Error: ID de contrato inválido");
                return;
            }

            var loadingMessage = "Cargando datos del contrato...";
            if (typeof showLoading === "function") {
                showLoading(loadingMessage);
            }

            // Llamar al servidor para obtener los datos del contrato
            $.ajax({
                url: config.urls.getContrato,
                type: "GET",
                data: { id: id },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                cache: false,
                success: function (response) {

                    if (typeof hideLoading === "function") {
                        hideLoading();
                    }

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
                },
                error: function (xhr, status, error) {
                    if (typeof hideLoading === "function") {
                        hideLoading();
                    }
                    console.error("Error AJAX al obtener detalle del contrato:");
                    console.error("Status:", status);
                    console.error("Error:", error);
                    console.error("Response:", xhr.responseText);

                    var mensajeError = "Error al comunicarse con el servidor.";
                    if (xhr.status === 404) {
                        mensajeError = "No se encontró el contrato solicitado.";
                    } else if (xhr.status === 500) {
                        mensajeError = "Error interno del servidor.";
                    }

                    alert(mensajeError + " Por favor, intente nuevamente.");
                },
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
    try {
        ControlBoletos.init();
    } catch (error) {
        console.error("Error inicializando Control de Boletos:", error);
        alert("Error al inicializar la aplicación. Por favor, recargue la página.");
    }
});

// Manejar errores globales
window.addEventListener("error", function (e) {
    console.error("Error global:", e.error);
});
