var ControlBoletosTracking = (function () {
    'use strict';

    var config = {
        urls: {
            trackingPartial: '/ControlDeBoletos/_TrackingControlDeBoletos',
            getTracking: '/ControlDeBoletos/GetTrackingBoleto'
        }
    };

    function abrir(controlDeBoletosId) {
        $("#modalTrackingBoletoBody").html("");

        $.get(config.urls.trackingPartial, { controlDeBoletosId: controlDeBoletosId })
            .done(function (html) {
                $("#modalTrackingBoletoBody").html(html);
                $("#modalTrackingBoleto").modal("show");
            });
        this.init(controlDeBoletosId);
    }

    function init(controlDeBoletosId) {
        cargarGrid(controlDeBoletosId);
    }

    function cargarGrid(controlDeBoletosId) {
        $("#gridTrackingBoleto").kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: config.urls.getTracking,
                        data: { controlDeBoletosId: controlDeBoletosId },
                        dataType: "json"
                    }
                },
                pageSize: 10
            },
            pageable: true,
            sortable: true,
            filterable: true,
            columns: [
                { field: "Accion", title: "Acción" },
                { field: "Resultado", title: "Resultado" },
                { field: "ValorAnterior", title: "Valor Anterior" },
                { field: "ValorNuevo", title: "Valor Nuevo" },
                {
                    field: "FechaModificacion",
                    title: "Fecha",
                    template: "#= kendo.toString(kendo.parseDate(FechaModificacion), 'dd/MM/yyyy HH:mm') #"
                },
                { field: "UsuarioModificacion", title: "Usuario" }
            ]
        });
    }

    return {
        abrir: abrir,
        init: init
    };

})();
