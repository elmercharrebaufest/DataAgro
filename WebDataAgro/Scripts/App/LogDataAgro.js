$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");

    //$('#menuproveedor').hide();
    //kendo.culture("es-AR");

    inicializarTodosKendoDate($(".filtroFecha"));
    $("#FechaDesdeId").data("kendoDatePicker").value(new Date());
    InicializarCuposIndex();

    //inicializarElementos();
    //InicializarCuposIndex();
    $("#ProveedorDiv").hide();
    $("#NegocioDiv").hide();
    $("#CupoDiv").hide();
});

function Filtrar() {
    var clase = $("#Clase").val();
    if (clase == "Negocio") {
        FiltrarNegocio();
    } else if (clase == "Cupo") {
        FiltrarCupo();
    } else {
        var grid = $('#grid').data('kendoGrid');
        let currentFilters = TraerFiltrosConValores();
        grid.dataSource.filter(currentFilters.filter);
    }
    
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
                    filtroCompleto = BorrarFiltroSap(filtroCompleto);
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
            { field: "Fecha", title: "Fecha de Modificacion", type: "date", /*width: 150,*/ format: "{0:dd/MM/yyyy HH:mm:ss }" },
            { field: "Usuario", title: "Usuario", type: "string" },
            { field: "AccionRealizada", title: "Accion Realizada", type: "string" },
            { field: "Clase", type: "string" },
            { field: "Descripcion", type: "string" },
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

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/LogDataagro/BuscarProveedor");
    //CrearMultiSelectFiltro("#buscadorCupo", "CupoSap", "Id", "/cupo/ListarCupo");
    //CrearMultiSelectFiltro("#buscadorNegocio", "Descripcion", "Id", "/logdataagro/ListarNegocios");
    //var kendoDropDown = $('#buscadorNegocio').data('kendoMultiSelect');
    //kendoDropDown.list.width(550);

    inicializarPopUpSap("Contratos");

}




function botonVisualizar(dataItem) {

    return '<a role="button" class="k-button k-button-icontext k-grid-vermas k-grid-qw" onclick="traerDatosModificados(' + dataItem.id + ',' +
        "'" + dataItem.Clase.trim() + "'" + ')">Ver Más</a>';
}

function traerDatosModificados(idLogDataAgro, tipoDeClase) {
    $.post('/logdataagro/MostrarDiferenciasTabla', { idLogDataAgro: idLogDataAgro }).done(function (res) {
        $("#diffPartial").html(res);
        $("#modalVisualizar").modal('show');
    });

}

function customExport() {
    BlockUi('Procesando...');
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
        $.unblockUI();
    };
    MSExecuteOnServerAsync('/LogDataAgro/Export', TraerFiltrosConValores(), funcReturn, true);
}

$('#Clase').on('change', function () {
    $("#ProveedorDiv").hide();
    $("#NegocioDiv").hide();
    $("#CupoDiv").hide();
    $("#buscadorProveedor").data("kendoMultiSelect").value('');
    //BorrarFiltro();
    if (this.value == "Proveedor") {
        $("#ProveedorDiv").show();
    }
    if (this.value == "Negocio") {
        $("#ProveedorDiv").show();
        $("#NegocioDiv").show();
    }
    if (this.value == "Cupo") {
        $("#ProveedorDiv").show();
        $("#CupoDiv").show();
    }
});



function FiltrarNegocio() {
    var grid = $('#grid').data('kendoGrid');
    let currentFilters = TraerFiltrosConValores();

    let filtrosContratosSap = new Array();
    $("#contratos-table td").each(function (e) {
        let valorBuscado = $("#contratos-table td")[e].innerText;
        if (valorBuscado != "" && $(valorBuscado != null)) {
            var valoresEnValor = valorBuscado.split(" ");
            for (var i = 0; i < valoresEnValor.length; i++) {
                filtrosContratosSap.push(valoresEnValor[i].padStart(10, '0'));
            }
        }
    });
    if (filtrosContratosSap.length > 100) {
        MensErr("El liminte de contratos es 100");
        return;
    }
    if (filtrosContratosSap.length == 0) {
        let currentFilters = TraerFiltrosConValores();
        grid.dataSource.filter(currentFilters.filter);
        return;
    }
    var result = MSExecuteOnServer('/LogDataAgro/ObtenerNegociosId', { contratosSap: filtrosContratosSap });//2647187
    if (result.length > 0) {
        currentFilters.filter.filters = currentFilters.filter.filters.filter(function (x) {
            return x.field != 'ClaseId' && x.field != undefined
        });

        var contratoSapFilters = { logic: 'or', filters: [] };
        for (var i = 0; i < result.length; i++) {
            contratoSapFilters.filters.push({ field: 'ClaseId', operator: 'eq', value: result[i] });
        }
        currentFilters.filter.filters.push(contratoSapFilters);
        grid.dataSource.filter(currentFilters.filter);


    } else {
        let currentFilters = TraerFiltrosConValores();
        grid.dataSource.filter(currentFilters.filter);
    }


}

function FiltrarCupo() {
    var grid = $('#grid').data('kendoGrid');
    let currentFilters = TraerFiltrosConValores();

    var filtrosSap = $("#CupoSAPId").val();
    if (filtrosSap.length == 0) {
        let currentFilters = TraerFiltrosConValores();
        grid.dataSource.filter(currentFilters.filter);
        return;
    }
    var filtrosCuposSap = new Array();
    var valoresEnValor = filtrosSap.split(" ");
    for (var i = 0; i < valoresEnValor.length; i++) {
        filtrosCuposSap.push(valoresEnValor[i]);
    }
    var result = MSExecuteOnServer('/LogDataAgro/ObtenerCuposId', { cupoSap: filtrosCuposSap });//2647187
    if (result.length > 0) {
        currentFilters.filter.filters = currentFilters.filter.filters.filter(function (x) {
            return x.field != 'ClaseId' && x.field != undefined
        });

        var contratoSapFilters = { logic: 'or', filters: [] };
        for (var i = 0; i < result.length; i++) {
            contratoSapFilters.filters.push({ field: 'ClaseId', operator: 'eq', value: result[i] });
        }
        currentFilters.filter.filters.push(contratoSapFilters);
        grid.dataSource.filter(currentFilters.filter);


    } else {
        let currentFilters = TraerFiltrosConValores();
        grid.dataSource.filter(currentFilters.filter);
    }


}

function BorrarFiltro() {
    $("#CupoSAPId").val("");
    $("#ContratoSAPId").val("");
    $("#ContratoSAPHastaId").val("");
    var grid = $('#grid').data('kendoGrid');
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }
    //Remove filter 
    var removeIndex = -1;
    if (filters != null) {
        for (var x = 0; x < filters.length; x++) {
            var temp = filters[x];
            if (temp.logic == "or") {

                for (var i = 0; i < temp.filters.length; i++) {
                    if (temp.filters[i].field == 'ClaseId' ) {
                        removeIndex = x;
                        break;
                    }
                }
            //    break;
            }
        }
        if (removeIndex != -1)
            filters.splice(removeIndex, 1);

    }
    dataSource.filter(filters);
    //Filtrar();
}

function BorrarFiltroSap(filtroCompleto) {
    //if ($("#CupoSAPId").val() == "" && $("#ContratoSAPId").val() == "" && $("#ContratoSAPHastaId").val() == "") {
    //    filtroCompleto.filter.filters = filtroCompleto.filter.filters.filter(function (x) {
    //        return x.field != 'ClaseId' && x.field != undefined
    //    });
    //}
    return filtroCompleto;
}