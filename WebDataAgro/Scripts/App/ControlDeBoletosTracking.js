var ControlBoletosTracking = (function () {
    "use strict";

    // ======================
    // Configuración
    // ======================
    var config = {
        urls: {
            getTracking: "/ControlDeBoletos/GetTrackingBoleto"
        },
        modalId: "#modalTrackingBoleto",
        modalBodyId: "#modalTrackingBoletoBody",
        gridId: "#gridTrackingBoleto"
    };

    // ======================
    // Estado interno
    // ======================
    var state = {
        controlDeBoletosId: null,
        gridInicializado: false
    };

    // ======================
    // Funciones privadas
    // ======================

    function inicializarGrid() {
        if ($(config.gridId).data("kendoGrid")) {
            $(config.gridId).data("kendoGrid").destroy();
            $(config.gridId).empty();
        }
        const url = `${config.urls.getTracking}?controlDeBoletosId=${encodeURIComponent(state.controlDeBoletosId)}`;
        $(config.gridId).kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json"
                    }
                },
                pageSize: 10,
                schema: {
                    data: "Data",
                    total: "Total"
                }
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
            ],
            dataBound: function(e) {
                var data = e.sender.dataSource.data();
                if (data.length === 0) {
                    $("#gridTrackingBoleto").hide();
                    $("#lblNoTrackingInfo").show();
                } else {
                    $("#gridTrackingBoleto").show();
                    $("#lblNoTrackingInfo").hide();
                }
            }
        });

        state.gridInicializado = true;
    }

    function mostrarModal() {
        $(config.modalId).modal("show");
    }

    // ======================
    // API pública
    // ======================
    return {

        abrir: function (controlDeBoletosId) {
            state.controlDeBoletosId = controlDeBoletosId;

            var url = "/ControlDeBoletos/_TrackingControlDeBoletos?id=" + controlDeBoletosId;
            
            // Remover modal anterior si existe
            $(config.modalId).remove();
            
            $.get(url, function(html) {
                $('body').append(html);
                
                try {
                    inicializarGrid();
                    mostrarModal();
                } catch (error) {
                    console.error("Error cargando tracking:", error);
                }
            });
        },

        cerrar: function () {
            $(config.modalId).modal("hide");
        }
    };

})();
