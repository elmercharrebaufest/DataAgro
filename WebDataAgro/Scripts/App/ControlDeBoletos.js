// Control de Boletos - JavaScript optimizado para .NET Framework 4.7.2
var ControlBoletos = (function () {
    'use strict';

    var config = {
        urls: {
            getMateriales: '/ControlDeBoletos/GetMateriales',
            getEstados: '/ControlDeBoletos/GetEstadosControl',
            getComerciales: '/ControlDeBoletos/GetComerciales',
            getProveedores: '/ControlDeBoletos/GetProveedores',
            getBolsaCompraNet: '/ControlDeBoletos/GetBolsaCompraNet',
            getBoletos: '/ControlDeBoletos/GetBoletos',
            getContadores: '/ControlDeBoletos/GetContadores',
            controlMasivo: '/ControlDeBoletos/ControlMasivo',
            exportarExcel: '/ControlDeBoletos/ExportarExcel',
            detalle: '/ControlDeBoletos/Detalle'
        }
    };

    var state = {
        grid: null,
        cargandoDatos: false,
        datosInicializados: false
    };

    // Funciones privadas
    function mostrarSpinner(mostrar) {
        var spinner = document.getElementById('loadingSpinner');
        if (spinner) {
            spinner.style.display = mostrar ? 'flex' : 'none';
        }
    }

    function mostrarMensaje(titulo, mensaje, tipo) {
        tipo = tipo || 'info';
        $('#mensajeModalTitle').text(titulo);
        $('#mensajeModalBody').html('<div class="alert alert-' + tipo + '">' + mensaje + '</div>');
        $('#mensajeModal').modal('show');
    }

    function actualizarBoton(selector, habilitado, texto) {
        var btn = $(selector);
        btn.prop('disabled', !habilitado);
        if (texto) {
            btn.find('span').text(texto);
        }
    }

    function validarNumero(valor) {
        return valor === '' || (!isNaN(valor) && parseInt(valor) >= 0);
    }

    function formatearFecha(fecha) {
        if (!fecha) return '';
        var date = new Date(fecha);
        return date.toLocaleDateString('es-AR');
    }

    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = $(selector);
        $select.html('<option value="">' + textoCarga + '</option>');

        $.ajax({
            url: url,
            type: 'GET',
            cache: true,
            timeout: 10000,
            success: function (data) {
                $select.empty().append('<option value="">' + textoDefault + '</option>');

                if (data && Array.isArray(data)) {
                    $.each(data, function (i, item) {
                        $select.append('<option value="' + item.Value + '">' + item.Text + '</option>');
                    });
                } else {
                    $select.append('<option value="">Sin datos disponibles</option>');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error cargando dropdown ' + selector + ':', error);
                $select.html('<option value="">Error al cargar datos</option>');
            }
        });
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
            cargarDropdown(config.urls.getMateriales, '#materialId', 'Cargando...', 'Todos los materiales');
            cargarDropdown(config.urls.getEstados, '#estadoControlId', 'Cargando...', 'Todos los estados');
            cargarDropdown(config.urls.getComerciales, '#comercialId', 'Cargando...', 'Todos los comerciales');
            cargarDropdown(config.urls.getProveedores, '#proveedorId', 'Cargando...', 'Todos los proveedores');
            cargarDropdown(config.urls.getBolsaCompraNet, '#bolsaCompraNetId', 'Cargando...', 'Todas las Bolsas');

        },

        configurarEventos: function () {
            var self = this;

            // Validación en tiempo real para campos numéricos
            $('#NegocioSAP-desde, #NegocioSAP-hasta').on('input', function () {
                var valor = this.value;
                if (!validarNumero(valor)) {
                    this.setCustomValidity('Ingrese un número válido');
                    $(this).addClass('is-invalid');
                } else {
                    this.setCustomValidity('');
                    $(this).removeClass('is-invalid');
                }
            });

            // Eventos de botones
            $('#filtrarBoletos').off('click').on('click', function () {
                self.filtrarBoletos();
            });

            $('#limpiarFiltros').off('click').on('click', function () {
                self.limpiarFiltros();
            });

            $('#exportarExcel').off('click').on('click', function () {
                self.exportarExcel();
            });

            // Selección múltiple
            $('#selectAll').off('change').on('change', function () {
                self.seleccionarTodos(this.checked);
            });

            // Control masivo
            $('#iniciarControlMasivo').off('click').on('click', function () {
                self.iniciarControlMasivo();
            });

            $('#finalizarControlMasivo').off('click').on('click', function () {
                self.finalizarControlMasivo();
            });

            // Vista
            $('#vistaTabla, #vistaCartas').off('click').on('click', function () {
                self.cambiarVista(this.id);
            });

            // Cards de progreso como filtros rápidos
            $('.progress-card[data-filter]').off('click').on('click', function () {
                var filtro = $(this).data('filter');
                self.aplicarFiltroRapido(filtro);
            });

            // Filtrar al presionar Enter
            $('#proveedorFiltro').off('keypress').on('keypress', function (e) {
                if (e.which === 13) {
                    self.filtrarBoletos();
                }
            });
        },

        inicializarGrid: function () {
            var self = this;

            try {
                state.grid = $("#boletos-grid").kendoGrid({
                    dataSource: {
                        type: "json",
                        serverPaging: true,
                        serverSorting: true,
                        serverFiltering: true,
                        pageSize: 50,
                        transport: {
                            read: {
                                url: config.urls.getBoletos,
                                type: "POST",
                                dataType: "json",
                                data: function () {
                                    return self.obtenerFiltros();
                                }
                            },
                            parameterMap: function (options, operation) {
                                if (operation === "read") {
                                    return kendo.stringify(options);
                                }
                            }
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
                                    Estado: { type: "string" },
                                    FechaCarga: { type: "date" },
                                    Proveedor: { type: "string" },
                                    Comercial: { type: "string" }
                                }
                            }
                        },
                        error: function (e) {
                            console.error("Error cargando datos del grid:", e);
                            mostrarMensaje('Error', 'Error al cargar los datos: ' + (e.errors || 'Error desconocido'), 'danger');
                            mostrarSpinner(false);
                        },
                        requestStart: function () {
                            mostrarSpinner(true);
                            state.cargandoDatos = true;
                        },
                        requestEnd: function () {
                            mostrarSpinner(false);
                            state.cargandoDatos = false;
                            actualizarBoton('#exportarExcel', true);
                        }
                    },
                    height: 550,
                    sortable: {
                        mode: "single",
                        allowUnsort: false
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
                            refresh: "Actualizar"
                        }
                    },
                    selectable: "multiple row",
                    columns: [
                        {
                            field: "Selected",
                            title: "<input type='checkbox' id='gridSelectAll' class='form-check-input' />",
                            template: "<input type='checkbox' class='row-checkbox form-check-input' data-id='#=Id#' />",
                            width: 50,
                            sortable: false,
                            filterable: false
                        },
                        {
                            field: "ContratoSAP",
                            title: "Contrato SAP",
                            width: 120,
                            template: "<span class='font-weight-bold'>#=ContratoSAP#</span>"
                        },
                        {
                            field: "Material",
                            title: "Material",
                            width: 120
                        },
                        {
                            field: "Estado",
                            title: "Estado",
                            width: 100,
                            template: function (dataItem) {
                                return self.generarBadgeEstado(dataItem.Estado);
                            }
                        },
                        {
                            field: "FechaCarga",
                            title: "Fecha Carga",
                            width: 120,
                            format: "{0:dd/MM/yyyy}",
                            template: "#= formatearFecha(FechaCarga) #"
                        },
                        {
                            field: "Proveedor",
                            title: "Proveedor",
                            width: 250
                        },
                        {
                            field: "Comercial",
                            title: "Comercial",
                            width: 150
                        },
                        {
                            field: "Acciones",
                            title: "Acciones",
                            width: 120,
                            template: function (dataItem) {
                                return self.generarBotonesAccion(dataItem);
                            },
                            sortable: false,
                            filterable: false
                        }
                    ],
                    dataBound: function (e) {
                        self.configurarEventosGrid();
                        self.actualizarContadores();
                        self.actualizarSeleccion();
                    },
                    change: function (e) {
                        self.actualizarBotonesControlMasivo();
                    }
                }).data("kendoGrid");

                // Ocultar spinner inicial después de crear el grid
                setTimeout(function () {
                    mostrarSpinner(false);
                }, 1000);

            } catch (error) {
                console.error("Error inicializando grid:", error);
                mostrarMensaje('Error', 'Error al inicializar la tabla de datos', 'danger');
                mostrarSpinner(false);
            }
        },

        configurarEventosGrid: function () {
            var self = this;

            // Checkbox del header
            $('#gridSelectAll').off('change').on('change', function () {
                self.seleccionarTodos(this.checked);
            });

            // Checkboxes individuales
            $('.row-checkbox').off('change').on('change', function () {
                self.actualizarBotonesControlMasivo();

                // Actualizar estado del checkbox principal
                var total = $('.row-checkbox').length;
                var seleccionados = $('.row-checkbox:checked').length;
                $('#gridSelectAll').prop('indeterminate', seleccionados > 0 && seleccionados < total);
                $('#gridSelectAll').prop('checked', seleccionados === total);
            });
        },

        obtenerFiltros: function () {
            return {
                contratoSAPDesde: $('#NegocioSAP-desde').val().trim(),
                contratoSAPHasta: $('#NegocioSAP-hasta').val().trim(),
                materialId: $('#materialId').val(),
                estadoControlId: $('#estadoControlId').val(),
                esConfirma: $('#esConfirma').is(':checked'),
                fechaCargaDesde: $('#fechaCargaDesde').val(),
                fechaCargaHasta: $('#fechaCargaHasta').val(),
                proveedor: $('#proveedorId').val().trim(),
                comercialId: $('#comercialId').val()
            };
        },

        validarFiltros: function () {
            var filtros = this.obtenerFiltros();
            var errores = [];

            if (filtros.contratoSAPDesde && !validarNumero(filtros.contratoSAPDesde)) {
                errores.push('El número SAP desde debe ser un número válido');
            }

            if (filtros.contratoSAPHasta && !validarNumero(filtros.contratoSAPHasta)) {
                errores.push('El número SAP hasta debe ser un número válido');
            }

            if (filtros.contratoSAPDesde && filtros.contratoSAPHasta &&
                parseInt(filtros.contratoSAPDesde) > parseInt(filtros.contratoSAPHasta)) {
                errores.push('El número SAP desde debe ser menor o igual al número SAP hasta');
            }

            if (filtros.fechaCargaDesde && filtros.fechaCargaHasta &&
                new Date(filtros.fechaCargaDesde) > new Date(filtros.fechaCargaHasta)) {
                errores.push('La fecha desde debe ser anterior a la fecha hasta');
            }

            return errores;
        },

        filtrarBoletos: function () {
            var errores = this.validarFiltros();
            if (errores.length > 0) {
                mostrarMensaje('Errores en los filtros', errores.join('<br>'), 'warning');
                return;
            }

            if (state.grid && !state.cargandoDatos) {
                mostrarSpinner(true);
                state.grid.dataSource.read();
            }
        },

        limpiarFiltros: function () {
            $('#NegocioSAP-desde, #NegocioSAP-hasta').val('').removeClass('is-invalid');
            $('#materialId, #estadoControlId, #comercialId').val('');
            $('#esConfirma').prop('checked', false);
            $('#fechaCargaDesde, #fechaCargaHasta').val('');
            $('#proveedorId').val('');

            this.filtrarBoletos();
        },

        aplicarFiltroRapido: function (estado) {
            this.limpiarFiltros();

            var estadoId = '';
            switch (estado) {
                case 'pendiente': estadoId = '1'; break;
                case 'proceso': estadoId = '2'; break;
                case 'completado': estadoId = '3'; break;
                case 'certificado': estadoId = '4'; break;
            }

            if (estadoId) {
                $('#estadoControlId').val(estadoId);
                this.filtrarBoletos();
            }
        },

        exportarExcel: function () {
            var filtros = this.obtenerFiltros();
            var queryString = $.param(filtros);

            var url = config.urls.exportarExcel + '?' + queryString;

            // Crear elemento temporal para descarga
            var link = document.createElement('a');
            link.href = url;
            link.download = 'ControlBoletos_' + new Date().toISOString().slice(0, 10) + '.xlsx';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },

        seleccionarTodos: function (seleccionar) {
            $('.row-checkbox').prop('checked', seleccionar);
            this.actualizarBotonesControlMasivo();
        },

        actualizarSeleccion: function () {
            this.actualizarBotonesControlMasivo();
        },

        actualizarBotonesControlMasivo: function () {
            var seleccionados = $('.row-checkbox:checked').length;
            var habilitado = seleccionados > 0;

            actualizarBoton('#iniciarControlMasivo', habilitado);
            actualizarBoton('#finalizarControlMasivo', habilitado);

            // Actualizar checkbox principal
            var total = $('.row-checkbox').length;
            $('#selectAll').prop('checked', seleccionados === total && total > 0);
        },

        iniciarControlMasivo: function () {
            var ids = this.obtenerSeleccionados();
            if (ids.length === 0) {
                mostrarMensaje('Atención', 'Debe seleccionar al menos un boleto para iniciar el control', 'warning');
                return;
            }

            var mensaje = '¿Está seguro de iniciar el control para ' + ids.length + ' boleto(s) seleccionado(s)?';
            if (confirm(mensaje)) {
                this.procesarControlMasivo('iniciar', ids);
            }
        },

        finalizarControlMasivo: function () {
            var ids = this.obtenerSeleccionados();
            if (ids.length === 0) {
                mostrarMensaje('Atención', 'Debe seleccionar al menos un boleto para finalizar el control', 'warning');
                return;
            }

            var mensaje = '¿Está seguro de finalizar el control para ' + ids.length + ' boleto(s) seleccionado(s)?';
            if (confirm(mensaje)) {
                this.procesarControlMasivo('finalizar', ids);
            }
        },

        obtenerSeleccionados: function () {
            var ids = [];
            $('.row-checkbox:checked').each(function () {
                var id = $(this).data('id');
                if (id) {
                    ids.push(id);
                }
            });
            return ids;
        },

        procesarControlMasivo: function (accion, ids) {
            var self = this;

            if (!ids || ids.length === 0) {
                mostrarMensaje('Error', 'No hay elementos seleccionados', 'warning');
                return;
            }

            // Deshabilitar botones durante el procesamiento
            actualizarBoton('#iniciarControlMasivo', false);
            actualizarBoton('#finalizarControlMasivo', false);

            $.ajax({
                url: config.urls.controlMasivo,
                type: 'POST',
                data: JSON.stringify({
                    accion: accion,
                    ids: ids
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                timeout: 30000,
                success: function (response) {
                    if (response && response.success) {
                        mostrarMensaje('Éxito', response.message || 'Operación completada correctamente', 'success');
                        self.filtrarBoletos(); // Recargar datos
                        $('#selectAll').prop('checked', false); // Limpiar selección
                    } else {
                        mostrarMensaje('Error', response.message || 'Error al procesar la solicitud', 'danger');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error en control masivo:', error);
                    mostrarMensaje('Error', 'Error de comunicación con el servidor: ' + error, 'danger');
                },
                complete: function () {
                    // Rehabilitar botones
                    self.actualizarBotonesControlMasivo();
                }
            });
        },

        cambiarVista: function (vista) {
            $('#vistaTabla, #vistaCartas').removeClass('active');
            $('#' + vista).addClass('active');

            if (vista === 'vistaCartas') {
                this.mostrarVistaCartas();
            } else {
                this.mostrarVistaTabla();
            }
        },

        mostrarVistaTabla: function () {
            $('#boletos-grid').show();
        },

        mostrarVistaCartas: function () {
            mostrarMensaje('Información', 'La vista de cartas estará disponible en una próxima versión', 'info');
            $('#vistaTabla').click(); // Volver a vista tabla
        },

        cargarContadores: function () {
            $.ajax({
                url: config.urls.getContadores,
                type: 'GET',
                timeout: 10000,
                success: function (data) {
                    if (data) {
                        $('#countPendientes').text(data.Pendientes || 0);
                        $('#countEnProceso').text(data.EnProceso || 0);
                        $('#countCompletados').text(data.Completados || 0);
                        $('#countCertificados').text(data.Certificados || 0);
                    }
                },
                error: function () {
                    console.warn('Error cargando contadores');
                }
            });
        },

        actualizarContadores: function () {
            this.cargarContadores();
            if (state.grid) {
                var total = state.grid.dataSource.total();
                $('#totalRegistros').text(total + ' registro' + (total !== 1 ? 's' : ''));
            }
        },

        generarBadgeEstado: function (estado) {
            var claseEstado = '';
            switch (estado) {
                case 'Pendiente': claseEstado = 'estado-pendiente'; break;
                case 'En Proceso': claseEstado = 'estado-proceso'; break;
                case 'Completado': claseEstado = 'estado-completado'; break;
                case 'Certificado': claseEstado = 'estado-certificado'; break;
                default: claseEstado = 'badge-secondary';
            }
            return '<span class="estado-badge ' + claseEstado + '">' + estado + '</span>';
        },

        generarBotonesAccion: function (data) {
            var botones = [];

            if (data.Estado === 'Pendiente') {
                botones.push('<button class="btn btn-sm btn-outline-primary btn-acciones tooltip-custom" onclick="ControlBoletos.iniciarControl(' + data.Id + ')" title="Iniciar Control"><i class="fa fa-play"></i><span class="tooltiptext">Iniciar Control</span></button>');
            }

            if (data.Estado === 'En Proceso') {
                botones.push('<button class="btn btn-sm btn-outline-success btn-acciones tooltip-custom" onclick="ControlBoletos.finalizarControl(' + data.Id + ')" title="Finalizar Control"><i class="fa fa-check"></i><span class="tooltiptext">Finalizar Control</span></button>');
            }

            botones.push('<button class="btn btn-sm btn-outline-info btn-acciones tooltip-custom" onclick="ControlBoletos.verDetalle(' + data.Id + ')" title="Ver Detalle"><i class="fa fa-eye"></i><span class="tooltiptext">Ver Detalle</span></button>');
            botones.push('<button class="btn btn-sm btn-outline-warning btn-acciones tooltip-custom" onclick="ControlBoletosModificacion.abrir(' + data.Id + ')" title="Modificar Contrato"><i class="fa fa-edit"></i><span class="tooltiptext">Modificar Contrato</span></button>');
            botones.push('<button class="btn btn-sm btn-outline-warning btn-acciones tooltip-custom" onclick="ControlBoletosTracking.abrir(' + data.Id + ')" title="Tracking Boleto"><i class="fa fa-edit"></i><span class="tooltiptext">Tracking</span></button>');
            return '<div class="btn-group" role="group">' + botones.join(' ') + '</div>';
        },

        iniciarControl: function (id) {
            this.procesarControlMasivo('iniciar', [id]);
        },

        finalizarControl: function (id) {
            this.procesarControlMasivo('finalizar', [id]);
        },

        verDetalle: function (id) {
            var url = config.urls.detalle + '/' + id;
            window.open(url, '_blank', 'width=800,height=600,scrollbars=yes,resizable=yes');
        }
    };
})();

// Función global para formatear fechas (necesaria para el template del grid)
function formatearFecha(fecha) {
    if (!fecha) return '';
    var date = new Date(fecha);
    return date.toLocaleDateString('es-AR');
}

// Inicializar cuando el DOM esté listo
$(document).ready(function () {
    try {
        ControlBoletos.init();
    } catch (error) {
        console.error('Error inicializando Control de Boletos:', error);
        alert('Error al inicializar la aplicación. Por favor, recargue la página.');
    }
});

// Manejar errores globales
window.addEventListener('error', function (e) {
    console.error('Error global:', e.error);
});