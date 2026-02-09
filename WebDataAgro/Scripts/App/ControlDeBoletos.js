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
            getContadores: "/ControlDeBoletos/GetContadores",
            getContrato: "/ControlDeBoletos/ObtenerDetalleContrato",
            controlMasivo: "/ControlDeBoletos/ControlMasivo",
            exportarExcel: "/ControlDeBoletos/ExportarExcel",
        },
    };

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false,
    };

    const controlMasivoAccion = Object.freeze({
        ControlIniciado: 1,
        RegistroDatosOblea: 2,
        CertificacionCompletada: 3,
        ControlFinalizado: 4
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
        var btn = $(selector);
        btn.prop("disabled", !habilitado);
        if (texto) {
            btn.find("span").text(texto);
        }
    }

    function validarNumero(valor) {
        return valor === "" || (!isNaN(valor) && parseInt(valor) >= 0);
    }

    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = $(selector);
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

    // Funciones públicas
    return {
        init: function () {
            if (state.datosInicializados) return;

            this.cargarDatosIniciales();
            this.configurarEventos();
            this.inicializarGrid();
            this.cargarContadores();

            state.datosInicializados = true;
        },

        cargarDatosIniciales: function () {
            cargarDropdown(
                config.urls.getMateriales,
                "#materialId",
                "Cargando...",
                "Todos los materiales",
            );
            cargarDropdown(
                config.urls.getEstados,
                "#estadoControlId",
                "Cargando...",
                "Todos los estados",
            );
            cargarDropdown(
                config.urls.getComerciales,
                "#comercialId",
                "Cargando...",
                "Todos los comerciales",
            );
            cargarDropdown(
                config.urls.getProveedores,
                "#proveedorId",
                "Cargando...",
                "Todos los proveedores",
            );
            cargarDropdown(
                config.urls.getBolsaCompraNet,
                "#bolsaCompraNetId",
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
            $("#filtrarBoletos")
                .off("click")
                .on("click", function () {
                    self.filtrarBoletos();
                });

            $("#limpiarFiltros")
                .off("click")
                .on("click", function () {
                    self.limpiarFiltros();
                });

            $("#exportarExcel")
                .off("click")
                .on("click", function () {
                    self.exportarExcel();
                });

            // Selección múltiple
            $("#selectAll")
                .off("change")
                .on("change", function () {
                    self.seleccionarTodos(this.checked);
                });

            // Control masivo
            $("#iniciarControlMasivo")
                .off("click")
                .on("click", function () {
                    self.iniciarControlMasivo();
                });

            $("#finalizarControlMasivo")
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
                                            contratoSAPDesde: filtros.contratoSAPDesde,
                                            contratoSAPHasta: filtros.contratoSAPHasta,
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
                                actualizarBoton("#exportarExcel", true);
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
                        navigatable: true,
                        selectable: {
                            mode: "multiple",
                            type: "row",
                        },
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
                                field: "Acciones",
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
                            self.configurarEventosGrid();
                            self.actualizarContadores();
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
                console.error("Error inicializando grid:", error);
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

            // Checkbox del header
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
                contratoSAPDesde: $("#NegocioSAP-desde").val().trim() || null,
                contratoSAPHasta: $("#NegocioSAP-hasta").val().trim() || null,
                materialId: $("#materialId").val() || null,
                estadoControlId: $("#estadoControlId").val() || null,
                esConfirma: $("#esConfirma").is(":checked"),
                fechaCargaDesde: $("#fechaCargaDesde").val() || null,
                fechaCargaHasta: $("#fechaCargaHasta").val() || null,
                proveedor: $("#proveedorId").val() || null,
                bolsaId: $("#bolsaCompraNetId").val() || null,
                comercialId: $("#comercialId").val() || null,
            };
        },

        validarFiltros: function () {
            var filtros = this.obtenerFiltros();
            var errores = [];

            if (
                filtros.contratoSAPDesde &&
                !validarNumero(filtros.contratoSAPDesde)
            ) {
                errores.push("El número SAP desde debe ser un número válido");
            }

            if (
                filtros.contratoSAPHasta &&
                !validarNumero(filtros.contratoSAPHasta)
            ) {
                errores.push("El número SAP hasta debe ser un número válido");
            }

            if (
                filtros.contratoSAPDesde &&
                filtros.contratoSAPHasta &&
                parseInt(filtros.contratoSAPDesde) > parseInt(filtros.contratoSAPHasta)
            ) {
                errores.push(
                    "El número SAP desde debe ser menor o igual al número SAP hasta",
                );
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
            $("#NegocioSAP-desde, #NegocioSAP-hasta")
                .val("")
                .removeClass("is-invalid");
            $("#materialId, #estadoControlId, #comercialId").val("");
            $("#esConfirma").prop("checked", false);
            $("#fechaCargaDesde, #fechaCargaHasta").val("");
            $("#proveedorId").val("");
            $("#bolsaCompraNetId").val("");

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
                $("#estadoControlId").val(estadoId);
                this.filtrarBoletos();
            }
        },

        exportarExcel: function () {
            var filtros = this.obtenerFiltros();
            var queryString = $.param(filtros);

            var url = config.urls.exportarExcel + "?" + queryString;

            // Crear elemento temporal para descarga
            var link = document.createElement("a");
            link.href = url;
            link.download =
                "ControlBoletos_" + new Date().toISOString().slice(0, 10) + ".xlsx";
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
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

            actualizarBoton("#iniciarControlMasivo", habilitado);
            actualizarBoton("#finalizarControlMasivo", habilitado);

            // Actualizar checkbox principal
            var total = $(".row-checkbox").length;
            $("#selectAll").prop("checked", seleccionados === total && total > 0);
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
            actualizarBoton("#iniciarControlMasivo", false);
            actualizarBoton("#finalizarControlMasivo", false);

            let request = {
                AccionControlDeBoletos: accion,
                ControlDeBoletoIds: ids,
            };

            const response = MSExecuteOnServer(config.urls.controlMasivo, request);
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
            actualizarBoton("#iniciarControlMasivo", false);
            actualizarBoton("#finalizarControlMasivo", false);

            let request = {
                AccionControlDeBoletos: accion,
                ControlDeBoletoIds: ids,
            };

            const response = MSExecuteOnServer(config.urls.controlMasivo , request);

            if (response) {
                self.actualizarBotonesControlMasivo();
            }
        },

        cargarContadores: function () {
            $.ajax({
                url: config.urls.getContadores,
                type: "GET",
                timeout: 10000,
                success: function (data) {
                    if (data) {
                        $("#countPendientes").text(data.Pendientes || 0);
                        $("#countEnProceso").text(data.EnProceso || 0);
                        $("#countCompletados").text(data.Completados || 0);
                        $("#countCertificados").text(data.Certificados || 0);
                    }
                },
                error: function () {
                    console.warn("Error cargando contadores");
                },
            });
        },

        actualizarContadores: function () {
            this.cargarContadores();
            if (state.grid) {
                var total = state.grid.dataSource.total();
                $("#totalRegistros").text(
                    total + " registro" + (total !== 1 ? "s" : ""),
                );
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
                    '<button class="btn btn-sm btn-outline-warning btn-acciones tooltip-custom" onclick="ControlBoletosModificacion.abrir(' +
                    data.NegocioId +
                    ')" title="Modificar Contrato"><i class="fa fa-edit"></i><span class="tooltiptext"></span></button>',
                );

                // Solo agregar botón de Tracking si el módulo está cargado
                if (typeof ControlBoletosTracking !== "undefined") {
                    botones.push(
                        '<button class="btn btn-sm btn-outline-info btn-acciones tooltip-custom" onclick="ControlBoletosTracking.abrir(' +
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

            if (data.ControlIniciado) {
                botones.push(
                    '<button class="btn btn-sm btn-outline-success btn-acciones tooltip-custom" onclick="ControlDeBoletosDaCertificacion.abrir(' +
                    data.Id +
                    ')" title="Registro de Obleado"><i class="fa fa-list-alt"></i><span class="tooltiptext"></span></button>',
                );

                botones.push(
                    '<button class="btn btn-sm btn-outline-success btn-acciones tooltip-custom" onclick="ControlDeBoletosSeguimiento.abrir(' +
                    data.Id +
                    ')" title="Registro de Certificación"><i class="fa fa-certificate"></i><span class="tooltiptext"></span></button>',
                );

                botones.push(
                    '<button class="btn btn-sm btn-outline-success btn-acciones tooltip-custom" onclick="ControlBoletos.finalizarControl(' +
                    data.Id +
                    ')" title="Finalizar Control"><i class="fa fa-check"></i><span class="tooltiptext"></span></button>',
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
            console.log("visualizarContrato - ID recibido:", id);

            if (!id || id <= 0) {
                console.error("ID inválido para visualizar contrato:", id);
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
                    console.log("Respuesta recibida:", response);

                    if (typeof hideLoading === "function") {
                        hideLoading();
                    }

                    if (!response) {
                        console.error("Respuesta vacía al solicitar contrato");
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

                    console.log("Contrato extraído para modal:", contrato);

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
