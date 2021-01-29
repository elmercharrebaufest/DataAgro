var cantidadZonas;
$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarElementos();
    CargarGrillaConfig();

    if ($("#Id").val() != "" && $("#Id").val() > 0) {
        //$("#DestinolId").prop('disabled', true);
        //$("#BoletoId").prop('disabled', true);
        //$("#ProvincialId").prop('disabled', true);
    }
    $("#formGrabarCupos").submit(function () {
        BlockUi('Grabando...');
    });
});

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

$(".alert").ready(function () {
    //setTimeout(function () { $(".alert").hide(); }, 5000);
});

function InicializarElementos() {    

    $("[data-hide]").on("click", function () {
        $(this).closest("." + $(this).attr("data-hide")).hide();
    });
    
}

function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ConfiguracionBolsa/DatosConfiguracion'
            },
            parameterMap: function (options, operation) {
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                //fields: {
                //    Fecha: { type: "date" },
                //    LimiteCupo: { type: "number" }
                //}
            }
        },
        serverPaging: true,
        serverSorting: true,
        //sort: [{ field: "Fecha", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#gridConfiguracionBolsa").kendoGrid({
        dataSource: ds,
        columns: [
            { selectable: true, width: "50px" },
            {
                field: "Bolsa", type: "string" 
            },
            {
                field: "Destino", type: "string"
            },
            {
                field: "Provincia", type: "string"
            },
            {
                field: "Id", title: " ", filterable: false, sortable: false, width: 80, template: function (dataItem) {

                    return '<span> <i class="fa fa-pencil" onclick="Editar(' + dataItem.Id + ')"></i> </span ></a >' +
                        '<a data-toggle="tooltip" title="Eliminar Configuracion"f class="links-grid" onclick="Eliminar(' + dataItem.Id + ')"><span> <i class="fa fa-trash"></i> </span ></a >';;
                        
                }
            }            
        ],
        dataBound: function (e) {
            //$(".cerrado").each(function (index) {
            //    var dataItem = e.sender.dataItem($(this).parent());
            //    if (dataItem.BloquearCupera == "Si") {
            //        $(this).addClass('line');
            //    }
            //});
        },
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
        filterable: {
            height: 350,
            extra: false,
            checkAll: false,

            messages: {
                info: "Filtros:",
                filter: "Filtrar",
                clear: "Limpiar",
                isTrue: "SI",
                isFalse: "NO",
                and: "Y",
                or: "O"
            },
            operators: {
                string: {
                    eq: "Igual"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        }
    }); 

    
}
function recargarGrilla() {
    $('#gridConfiguracionBolsa').data('kendoGrid').dataSource.read();
}

function Editar(id) {
    var bolsa = MSExecuteOnServer("/ConfiguracionBolsa/EditarConfiguracionBolsa", { id: id });
    $("#alta").collapse('show');
    $("#Id").val(bolsa.Id);
    $("#DestinoId").val(bolsa.DestinoId)
    $("#BolsaId").val(bolsa.BolsaId)
    $("#ProvinciaId").val(bolsa.ProvinciaId);
}

function Cancelar() {
    $("#Id").val(0);
    $("#DestinoId").val(1)
    $("#BolsaId").val("")
    $("#ProvinciaId").val("");
  
}

//function LimpiarConfiguracion() {
//    $("#Id").val(0);
//    $("#DestinolId").val("")
//    $("#BoletoId").val("")
//    $("#ProvincialId").val("");
//}

function SeleccionarElementos() {
    var grid = $("#gridConfiguracionBolsa").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        obj.push(selectedItem);
    });
    return obj;
}
function DeseleccionarElementos() {
    var grid = $("#gridConfiguracionBolsa").data("kendoGrid");
    grid.clearSelection();
}

function Eliminar(id) {
    var cupo = MSExecuteOnServer("/ConfiguracionBolsa/EliminarConfiguracionBolsa", { id: id });
    MensInfo(cupo.Errores[0].Message);
    recargarGrilla();
}


//function CierreMasivo() {

//    var configuraciones = SeleccionarElementos();
//    var ids = [];
//    for (var i = 0; i < configuraciones.length; i++) {
//        ids.push(configuraciones[i].id);
//    }
//    if (ids.length <= 0) {
//        MensErr("No se seleccionó ninguna configuración");
//    } else {
//        var limites = MSExecuteOnServer("/ConfiguracionCupo/CambioMasivo", { ids: ids, aceptar: true });
//        MensInfo("Se guardó correctamente");
//        recargarGrilla();
//    }
//}

//function AbrirMasivo() {

//    var configuraciones = SeleccionarElementos();
//    var ids = [];
//    for (var i = 0; i < configuraciones.length; i++) {
//        ids.push(configuraciones[i].id);
//    }
//    if (ids.length <= 0) {
//        MensErr("No se seleccionó ninguna configuración");
//    } else {
//        var limites = MSExecuteOnServer("/ConfiguracionCupo/CambioMasivo", { ids: ids, aceptar: false });
//        MensInfo("Se guardó correctamente");
//        recargarGrilla();
//    }
//}