$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");

    //inicializarTodosKendoDate($(".filtroFecha"));
    inicializarElementos();
    InicializarGrid();

});

function additionalData() {
    return {
        FechaDesde: $("#FechaDesde").val(),
        FechaHasta: $("#FechaHasta").val(),
        MaterialId: $("#MaterialId").val(),
        ZonaId: $("#ZonaId").val(),
        CentroId: $("#CentroId").data("kendoMultiSelect").value(),
    };
}

function InicializarGrid() {

    var ds = {
        transport: {
            parameterMap: function (options) {
                return JSON.stringify(additionalData());
            },
            read: {
                type: 'POST',
                dataType: 'json',
                contentType: "application/json",
                url: '/Cupo/BuscaDatosTablaDisponibilidad'
               
            }
        },
        
        serverPaging: false,
        serverSorting: false,
        sort: [
        ],
        //pageSize: 20,
        serverFiltering: true
    };

    $("#grid").kendoGrid({
        //toolbar: ["excel"],
        //excel: {
        //    fileName: "Reporte Disponibilidad.xlsx",
        //    allPages: true
        //},
        dataSource: ds,
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            var view = grid.dataSource.data();
            //for (var i = 0; i < view.length; i++) {
            //    if (view[i].FleteProcedencia) {
            //        grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
            //            .addClass("flete-procedencia");
            //    }
            //    $('[data-toggle="tooltip"]').tooltip();
            //}
        },
        columns: [
            { field: "Fecha", title: "Fecha", type: "date", format: "{0:dd/MM/yyyy}"},
            { field: "MaterialNombre", title: "Material", type: "string" },
            { field: "ZonaId", title: "Zona", type: "string" },
            { field: "Limite", title: "Limite", type: "string" },
            { field: "Consumidos", title: "Consumidos", type: "string" },
            { field: "Disponibles", title: "Disponibles", type: "string" },
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


function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}


function inicializarElementos() {
    $("#FechaDesde").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });

    $("#FechaDesde").click(function () {
        $("#Fecha").val("");
    });

    $("#FechaHasta").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });

    $("#FechaHasta").click(function () {
        $("#Fecha").val("");
    });

    $("#CentroId").kendoMultiSelect({
        autoClose: false
    });

}



function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}










