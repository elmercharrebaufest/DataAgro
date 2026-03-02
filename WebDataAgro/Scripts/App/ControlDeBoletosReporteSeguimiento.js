
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

    let controlMaterial = $("#frmReporteSeguimientoBoletos #materialId");
    let controlProveedor = $("#frmReporteSeguimientoBoletos #proveedorId");
    let controlBolsaCompraNet = $("#frmReporteSeguimientoBoletos #bolsaCompraNetId");

    let controlNegocioSAPDesde = $("#frmReporteSeguimientoBoletos #NegocioSAP-desde");
    let controlNegocioSAPHasta = $("#frmReporteSeguimientoBoletos #NegocioSAP-hasta");

    let controlFechaVueltaAfipHasta = $("#frmReporteSeguimientoBoletos #fechaCertificacionDesde");
    let controlFechaVueltaAfipDesde = $("#frmReporteSeguimientoBoletos #fechaCertificacionHasta");
    let controlFechaEnvioAfipHasta = $("#frmReporteSeguimientoBoletos #fechaVencimientoCertificacionDesde");
    let controlFechaEnvioAfipDesde = $("#frmReporteSeguimientoBoletos #fechaVencimientoCertificacionHasta");
    let controlFechaVueltaBolsaHasta = $("#frmReporteSeguimientoBoletos #fechaRecepBoletoDesde");
    let controlFechaVueltaBolsaDesde = $("#frmReporteSeguimientoBoletos #fechaRecepBoletoHasta");
    let controlFechaEnvioBolsaHasta = $("#frmReporteSeguimientoBoletos #fechaEnviadoFirmaDesde");
    let controlFechaEnvioBolsaDesde = $("#frmReporteSeguimientoBoletos #fechaEnviadoFirmaHasta");
    let controlFechaRecibFirmaHasta = $("#frmReporteSeguimientoBoletos #fechaRecibFirmaDesde");
    let controlFechaRecibFirmaDesde = $("#frmReporteSeguimientoBoletos #fechaRecibFirmaHasta");
    let controlFechaEnviadoFirmaHasta = $("#frmReporteSeguimientoBoletos #fechaEnvioBolsaDesde");
    let controlFechaEnviadoFirmaDesde = $("#frmReporteSeguimientoBoletos #fechaEnvioBolsaHasta");
    let controlFechaRecepBoletoHasta = $("#frmReporteSeguimientoBoletos #fechaVueltaBolsaDesde");
    let controlFechaRecepBoletoDesde = $("#frmReporteSeguimientoBoletos #fechaVueltaBolsaHasta");
    let controlFechaVencimientoCertificacionHasta = $("#frmReporteSeguimientoBoletos #fechaEnvioAfipDesde");
    let controlFechaVencimientoCertificacionDesde = $("#frmReporteSeguimientoBoletos #fechaEnvioAfipHasta");
    let controlFechaCertificacionHasta = $("#frmReporteSeguimientoBoletos #fechaVueltaAfipDesde");
    let controlFechaCertificacionDesde = $("#frmReporteSeguimientoBoletos #fechaVueltaAfipHasta");

    let controlFiltrarBoletos = $("#frmReporteSeguimientoBoletos #filtrarBoletos");
    let controlLimpiarFiltros = $("#frmReporteSeguimientoBoletos #limpiarFiltros");
    let controlExportarExcel = $("#frmReporteSeguimientoBoletos #exportarExcel");

    let controlSelectAll = $("#frmReporteSeguimientoBoletos #selectAll");

    let controlIniciarControlMasivo = $("#frmReporteSeguimientoBoletos #iniciarControlMasivo");
    let controlFinalizarControlMasivo = $("#frmReporteSeguimientoBoletos #finalizarControlMasivo");

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

    // Funciones públicas
    return {
        init: function () {
            if (state.datosInicializados) return;

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
        },

        autoFitSelectedColumns: function () {
            if (!state.grid) return;
            var colsToFit = ["EstadoConfirma", "Proveedor", "Comercial"];
            var columns = state.grid.columns;
            for (var i = 0; i < columns.length; i++) {
                var field = columns[i].field;
                if (field && colsToFit.indexOf(field) !== -1) {
                    try {
                        state.grid.autoFitColumn(i);
                    } catch (e) {
                        // autoFitColumn puede no estar disponible en algunas versiones; ignorar errores
                    }
                }
            }
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
                                    url: config.urls.getReporteSeguimiento,
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
                                            bolsaId: filtros.bolsaId,
                                            proveedor: filtros.proveedor,
                                            fechaCertificacionDesde: filtros.fechaVueltaAfipHasta,
                                            fechaCertificacionHasta: filtros.fechaVueltaAfipDesde,
                                            fechaVencimientoCertificacionDesde: filtros.fechaEnvioAfipHasta,
                                            fechaVencimientoCertificacionHasta: filtros.fechaEnvioAfipDesde,
                                            fechaRecepBoletoDesde: filtros.fechaVueltaBolsaHasta,
                                            fechaRecepBoletoHasta: filtros.fechaVueltaBolsaDesde,
                                            fechaEnviadoFirmaDesde: filtros.fechaEnvioBolsaHasta,
                                            fechaEnviadoFirmaHasta: filtros.fechaEnvioBolsaDesde,
                                            fechaRecibFirmaDesde: filtros.fechaRecibFirmaHasta,
                                            fechaRecibFirmaHasta: filtros.fechaRecibFirmaDesde,
                                            fechaEnvioBolsaDesde: filtros.fechaEnviadoFirmaHasta,
                                            fechaEnvioBolsaHasta: filtros.fechaEnviadoFirmaDesde,
                                            fechaVueltaBolsaDesde: filtros.fechaRecepBoletoHasta,
                                            fechaVueltaBolsaHasta: filtros.fechaRecepBoletoDesde,
                                            fechaEnvioAfipDesde: filtros.fechaVencimientoCertificacionHasta,
                                            fechaEnvioAfipHasta: filtros.fechaVencimientoCertificacionDesde,
                                            fechaVueltaAfipDesde: filtros.fechaCertificacionHasta,
                                            fechaVueltaAfipHasta: filtros.fechaCertificacionDesde,
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
                        navigatable: true,
                        selectable: {
                            mode: "multiple",
                            type: "row",
                        },
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
                            // Ajustar automáticamente solo las columnas solicitadas
                            //self.autoFitSelectedColumns();
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
                contratoSAPDesde: controlNegocioSAPDesde.val().trim() || null,
                contratoSAPHasta: controlNegocioSAPHasta.val().trim() || null,
                materialId: controlMaterial.val() || null,
                proveedor: controlProveedor.val() || null,
                bolsaId: controlBolsaCompraNet.val() || null,
                fechaCertificacionDesde: controlFechaCertificacionDesde.val() || null,
                fechaCertificacionHasta: controlFechaCertificacionHasta.val() || null,
                fechaVencimientoCertificacionDesde: controlFechaVencimientoCertificacionDesde.val() || null,
                fechaVencimientoCertificacionHasta: controlFechaVencimientoCertificacionHasta.val() || null,
                fechaRecepBoletoDesde: controlFechaRecepBoletoDesde.val() || null,
                fechaRecepBoletoHasta: controlFechaRecepBoletoHasta.val() || null,
                fechaEnviadoFirmaDesde: controlFechaEnviadoFirmaDesde.val() || null,
                fechaEnviadoFirmaHasta: controlFechaEnviadoFirmaHasta.val() || null,
                fechaRecibFirmaDesde: controlFechaRecibFirmaDesde.val() || null,
                fechaRecibFirmaHasta: controlFechaRecibFirmaHasta.val() || null,
                fechaEnvioBolsaDesde: controlFechaEnvioBolsaDesde.val() || null,
                fechaEnvioBolsaHasta: controlFechaEnvioBolsaHasta.val() || null,
                fechaVueltaBolsaDesde: controlFechaVueltaBolsaDesde.val() || null,
                fechaVueltaBolsaHasta: controlFechaVueltaBolsaHasta.val() || null,
                fechaEnvioAfipDesde: controlFechaEnvioAfipDesde.val() || null,
                fechaEnvioAfipHasta: controlFechaEnvioAfipHasta.val() || null,
                fechaVueltaAfipDesde: controlFechaVueltaAfipDesde.val() || null,
                fechaVueltaAfipHasta: controlFechaVueltaAfipHasta.val() || null,
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
