/**
 * OptimizationCache.js
 * 
 * Sistema de caché para reducir llamadas redundantes a endpoints del servidor
 * Debe cargarse ANTES que CrearContrato.js y CopiarContrato.js
 */

var ApiCacheManager = (function () {
    'use strict';

    var cache = {
        feriados: null,
        feriadosFormateados: null,  // Nuevo: feriados ya formateados en dd-MM-yyyy
        calidades: {},              // { materialId: data }
        campanaActual: {},          // { materialId: data }
        campana: {},                // { materialId: data }
        ultimoDiaHabil: null
    };

    var pendingRequests = {
        feriados: null,
        calidades: {},
        campanaActual: {},
        campana: {}
    };

    return {
        /**
         * Obtiene feriados con caché
         */
        getFeriados: function (callback) {
            if (cache.feriados !== null) {
                if (callback) callback(cache.feriados);
                return;
            }

            if (pendingRequests.feriados) {
                pendingRequests.feriados.done(function (data) {
                    if (callback) callback(data);
                });
                return;
            }

            pendingRequests.feriados = $.ajax({
                url: '/CompraNet/FechaFeriados',
                type: 'GET',
                dataType: 'json',
                global: false
            }).done(function (data) {
                cache.feriados = data;
                if (callback) callback(data);
                pendingRequests.feriados = null;
            }).fail(function () {
                pendingRequests.feriados = null;
            });
        },

        /**
         * Obtiene feriados en formato dd-MM-yyyy para el datepicker (OPTIMIZADO)
         * Evita procesar los feriados múltiples veces
         */
        getFeriadosFormateados: function (callback) {
            if (cache.feriadosFormateados !== null) {
                if (callback) callback(cache.feriadosFormateados);
                return;
            }

            // Si ya tenemos feriados crudos, los procesamos
            if (cache.feriados !== null) {
                var fechas = [];
                for (var i = 0; i < cache.feriados.length; i++) {
                    var src = cache.feriados[i];
                    src = src.replace(/[^0-9 +]/g, '');
                    fechas.push(kendo.toString(new Date(parseInt(src)), "dd-MM-yyyy"));
                }
                cache.feriadosFormateados = fechas;
                if (callback) callback(cache.feriadosFormateados);
                return;
            }

            // Si no tenemos nada, traer del servidor
            this.getFeriados(function (data) {
                var fechas = [];
                for (var i = 0; i < data.length; i++) {
                    var src = data[i];
                    src = src.replace(/[^0-9 +]/g, '');
                    fechas.push(kendo.toString(new Date(parseInt(src)), "dd-MM-yyyy"));
                }
                cache.feriadosFormateados = fechas;
                if (callback) callback(cache.feriadosFormateados);
            });
        },

        /**
         * Establece los feriados formateados en el caché (para uso interno)
         */
        _setFeriadosFormateados: function (fechasFormateadas) {
            cache.feriadosFormateados = fechasFormateadas;
        },

        /**
         * Obtiene calidades por material con caché
         */
        getCalidades: function (materialId, callback) {
            if (cache.calidades[materialId]) {
                if (callback) callback(cache.calidades[materialId]);
                return;
            }

            if (pendingRequests.calidades[materialId]) {
                pendingRequests.calidades[materialId].done(function (data) {
                    if (callback) callback(data);
                });
                return;
            }

            pendingRequests.calidades[materialId] = $.ajax({
                url: '/CompraNet/TraerCalidadesPorMaterial',
                type: 'POST',
                data: JSON.stringify({ MaterialId: materialId }),
                dataType: 'json',
                contentType: 'application/json',
                global: false
            }).done(function (data) {
                cache.calidades[materialId] = data;
                if (callback) callback(data);
                delete pendingRequests.calidades[materialId];
            }).fail(function () {
                delete pendingRequests.calidades[materialId];
            });
        },

        /**
         * Obtiene campaña actual por material con caché
         */
        getCampanaActual: function (materialId, callback) {
            if (cache.campanaActual[materialId] !== undefined) {
                if (callback) callback(cache.campanaActual[materialId]);
                return;
            }

            if (pendingRequests.campanaActual[materialId]) {
                pendingRequests.campanaActual[materialId].done(function (data) {
                    if (callback) callback(data);
                });
                return;
            }

            pendingRequests.campanaActual[materialId] = $.ajax({
                url: '/CompraNet/TraerCampanaActualMaterial',
                type: 'POST',
                data: JSON.stringify({ MaterialId: materialId }),
                dataType: 'json',
                contentType: 'application/json',
                global: false
            }).done(function (data) {
                cache.campanaActual[materialId] = data;
                if (callback) callback(data);
                delete pendingRequests.campanaActual[materialId];
            }).fail(function () {
                delete pendingRequests.campanaActual[materialId];
            });
        },

        /**
         * Obtiene campañas por material con caché
         */
        getCampana: function (materialId, callback) {
            if (cache.campana[materialId]) {
                if (callback) callback(cache.campana[materialId]);
                return;
            }

            if (pendingRequests.campana[materialId]) {
                pendingRequests.campana[materialId].done(function (data) {
                    if (callback) callback(data);
                });
                return;
            }

            pendingRequests.campana[materialId] = $.ajax({
                url: '/CompraNet/TraerCampanaPorMaterial',
                type: 'POST',
                data: JSON.stringify({ MaterialId: materialId }),
                dataType: 'json',
                contentType: 'application/json',
                global: false
            }).done(function (data) {
                cache.campana[materialId] = data;
                if (callback) callback(data);
                delete pendingRequests.campana[materialId];
            }).fail(function () {
                delete pendingRequests.campana[materialId];
            });
        },

        /**
         * Limpia la caché (útil para testing o forzar actualización)
         */
        clearCache: function () {
            cache = {
                feriados: null,
                feriadosFormateados: null,
                calidades: {},
                campanaActual: {},
                campana: {},
                ultimoDiaHabil: null
            };
        },

        /**
         * Obtiene el estado actual del caché
         */
        getCacheStatus: function () {
            return {
                feriados: cache.feriados !== null,
                feriadosFormateados: cache.feriadosFormateados !== null,
                calidades: Object.keys(cache.calidades),
                campanaActual: Object.keys(cache.campanaActual),
                campana: Object.keys(cache.campana)
            };
        }
    };
})();
