$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    InicializarCuposIndex();

    //$('#menuproveedor').hide();
    //kendo.culture("es-AR");

    inicializarTodosKendoDate($(".filtroFecha"));
    //inicializarElementos();
    //InicializarCuposIndex();

});

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}


function InicializarCuposIndex() {

    var ds = {
        transport: {
            parameterMap: function (options, operation) {

                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            },
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/LogDataAgro/BuscarDatosLogDataAgro',
                data: function () {

                    let filtroCompleto = TraerFiltrosConValores();

                    return filtroCompleto;

                }
            }
        },
        schema: {
            data: "Data",
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Fecha: { type: "date" },
                    Usuario: { type: "string" },
                    AccionRealizada: { type: "string" },
                    Clase: { type: "string" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        sort: [
        ],
        pageSize: 20,
        serverFiltering: true
    };


    $("#grid").kendoGrid({
        toolbar: kendo.template($("#templateToolbar").html()),
       
        dataSource: ds,
        
        columns: [
            //{ field: "Id", type: "string" },
            { field: "Fecha", title: "Fecha de Modificacion", type: "date", /*width: 150,*/ format: "{0:dd/MM/yyyy HH:mm }" },
            { field: "Usuario", title: "Usuario", type: "string" },
            { field: "AccionRealizada", title: "Accion Realizada", type: "string" },
            { field: "Clase", type: "string" },
            {
                field: "Estado_Contrato", sortable: false, title: " ", template: function (dataItem) {

                    return botonVisualizar(dataItem);
                }
            }
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
       
    });

}




function botonVisualizar(dataItem) {

    return '<a role="button" class="k-button k-button-icontext k-grid-vermas k-grid-qw" onclick="traerDatosModificados(' + dataItem.id + ',' +
        "'" + dataItem.Clase.trim() + "'" + ')">Ver Más</a>';
}

function traerDatosModificados(idLogDataAgro, tipoDeClase) {
    $.post('/logdataagro/MostrarDiferencias', { idLogDataAgro: idLogDataAgro }).done(function (res) {
        $("#diffPartial").html(res);
        $("#modalVisualizar").modal('show');
    });
    
}

function customExport() {
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    };
    MSExecuteOnServerAsync('/LogDataAgro/Export', TraerFiltrosConValores(), funcReturn, true);
}

