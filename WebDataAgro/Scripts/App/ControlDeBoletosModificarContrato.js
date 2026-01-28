var ControlBoletosModificacion = (function () {
    'use strict';
    var config = {
        urls: {
            getProvincias: '/ControlDeBoletos/GetProvincias',
            getClasificaciones: '/ControlDeBoletos/GetClasificaciones',
            getCosechas: '/ControlDeBoletos/GetCosechas',
            getProcedencias: '/ControlDeBoletos/GetProcedencias',
            modificar: '/ControlDeBoletos/Modificar'
        },
        modalId: '#modalModificarBoleto'
    };

    var state = {
        boletoId: null,
        cargando: false
    };

    // ======================
    // Funciones privadas
    // ======================

    function mostrarSpinner(mostrar) {
        var spinner = document.getElementById('loadingSpinner');
        if (spinner) {
            spinner.style.display = mostrar ? 'flex' : 'none';
        }
    }

    function mostrarMensaje(titulo, mensaje, tipo) {
        tipo = tipo || 'info';
        $('#mensajeModalTitle').text(titulo);
        $('#mensajeModalBody')
            .html('<div class="alert alert-' + tipo + '">' + mensaje + '</div>');
        $('#mensajeModal').modal('show');
    }

    function cargarDropdown(url, selector, textoCarga, textoDefault) {
        var $select = $(selector);
        $select.html('<option value="">' + textoCarga + '</option>');

        $.ajax({
            url: url,
            type: 'GET',
            cache: true,
            success: function (data) {
                $select.empty().append('<option value="">' + textoDefault + '</option>');
                if (data && Array.isArray(data)) {
                    $.each(data, function (i, item) {
                        $select.append(
                            '<option value="' + item.Value + '">' + item.Text + '</option>'
                        );
                    });
                }
            },
            error: function () {
                $select.html('<option value="">Error al cargar</option>');
            }
        });
    }

    function obtenerRequest() {
        return {
            Id: state.boletoId,
            ImProvincia: $('#ImProvincia').val(),
            ImClasificacion: $('#ImClasificacion').val(),
            ImCosecha: $('#ImCosecha').val(),
            ImProcedencia: $('#ImProcedencia').val(),
            ImFecha: $('#ImFecha').val(),
            ImHora: $('#ImHora').val()
        };
    }

    function validarFormulario() {
        var errores = [];

        if (!$('#ImCosecha').val())
            errores.push('Debe seleccionar una cosecha');

        if (!$('#ImProvincia').val())
            errores.push('Debe seleccionar una provincia');

        if (!$('#ImFecha').val())
            errores.push('Debe ingresar la fecha');

        if (!$('#ImHora').val())
            errores.push('Debe ingresar la hora');

        return errores;
    }

    // ======================
    // API pública
    // ======================
    return {

        abrir: function (boletoId, data) {
            state.boletoId = boletoId;
            this.cargarCombos();

            if (data) {
                $('#ImProvincia').val(data.ImProvincia);
                $('#ImClasificacion').val(data.ImClasificacion);
                $('#ImCosecha').val(data.ImCosecha);
                $('#ImProcedencia').val(data.ImProcedencia);
                $('#ImFecha').val(data.ImFecha);
                $('#ImHora').val(data.ImHora);
            }

            $(config.modalId).modal('show');
        },

        cargarCombos: function () {
            cargarDropdown(config.urls.getProvincias, '#ImProvincia', 'Cargando...', 'Seleccione provincia');
            cargarDropdown(config.urls.getClasificaciones, '#ImClasificacion', 'Cargando...', 'Seleccione clasificación');
            cargarDropdown(config.urls.getCosechas, '#ImCosecha', 'Cargando...', 'Seleccione cosecha');
            cargarDropdown(config.urls.getProcedencias, '#ImProcedencia', 'Cargando...', 'Seleccione procedencia');
        },

        guardar: function () {
            if (state.cargando) return;

            var errores = validarFormulario();
            if (errores.length > 0) {
                mostrarMensaje('Validación', errores.join('<br>'), 'warning');
                return;
            }

            state.cargando = true;
            mostrarSpinner(true);

            $.ajax({
                url: config.urls.modificar,
                type: 'POST',
                data: obtenerRequest(),
                success: function (response) {
                    if (response && response.success) {
                        mostrarMensaje('Éxito', 'Boleto modificado correctamente', 'success');
                        $(config.modalId).modal('hide');

                        // Refrescar grilla principal
                        if (window.ControlBoletos) {
                            ControlBoletos.filtrarBoletos();
                        }
                    } else {
                        mostrarMensaje('Error', response.message || 'Error al modificar el boleto', 'danger');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error al modificar boleto:', error);
                    mostrarMensaje('Error', 'Error de comunicación con el servidor', 'danger');
                },
                complete: function () {
                    state.cargando = false;
                    mostrarSpinner(false);
                }
            });
        },

        cerrar: function () {
            $(config.modalId).modal('hide');
        }
    };

})();
