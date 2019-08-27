var viewModel;
var zonaSeleccionada;
    $(document).ready(function () {
        kendo.culture("es-AR");
        $('#menuproveedor').hide();
        $("#crearCupo").click(function () {
            if ($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial" || $("#perfil").val() == "CorredoresComercial") {
                window.location.href = window.location.origin + "/Cupo/CrearCupo";
            } else {
                MensInfo("No posee permisos para la carga de cupos");
            }
        });

        InicializarCargaCupos();
        $.unblockUI();
        checkFason();
        checkSoja();
        InicializarCuposIndex();
    });

function InicializarCuposIndex() {    
    var defaultFilter = { field: "FechaIngreso", operator: "eq", value: new Date };
        var ds = {
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: '/Cupo/BuscaDatosTabla'
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
                    fields: {
                        FechaIngreso: { type: "date" }
                    }
                }
            },
            serverPaging: true,
            serverSorting: true,
            sort: [{ field: "FechaIngreso", dir: "desc" }],
            serverFiltering: true,
            pageSize: 20,
            filter: defaultFilter
        };

    $("#gridCupo").kendoGrid({
        dataSource: ds,
        dataBound: function () {
            var grid = $("#gridCupo").data("kendoGrid");
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].FleteProcedencia) {                    
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("flete-procedencia");
                }
            }
        },
        columns: [
            { field: "FechaIngreso", title:"Ingreso", type: "date", width: 150, format: _DefaultDateTemplate },
            { field: "Comercial", type: "string", width: 150 },
            { field: "CupoSap", title: "Cupo", type: "string", width: 150 },
            { field: "Material", type: "string", width: 150 },
            { field: "Proveedor", type: "string", width: 150 },
            { field: "Destinatario", type: "string", width: 150 },
            { field: "Centro", type: "string", width: 150 },
            { field: "Calidad", type: "string", width: 150 },
            { field: "ZonaCupo", title: "Zona", type: "string", width: 150 },
            {
                field: "FleteProcedencia", title: "Flete", type: "string", width: 150, template: function (dataItem) {
                    if (dataItem.FleteProcedencia) {
                        return "Si";
                    } else { return "No";}}
                },
            { field: "Observaciones", type: "string", width: 150 },
            {
                title: "", filterable: false, sortable: false, width: 150,
                template: function (dataItem) {
                    if (dataItem.EstadoCupoId == 1) {
                        return '<div class="status sinctg">' + dataItem.EstadoCupo + '</div>' +
                            botonBorrar(dataItem, 'fa-trash pend');
                    } else if (dataItem.EstadoCupoId == 5) {
                        return '<div class="status anulado">' + dataItem.EstadoCupo + '</div>' +
                            botonBorrar(dataItem, 'fa-trash anu');
                    }
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
        filterable: {
            checkAll: false,
            height: 350,
            extra: false,
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
                    eq: "Igual",
                    neq: "Distinto",
                    startswith: "Comienza con",
                    contains: "Contiene",
                    endswith: "Finaliza con"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a",
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a",
                }
            }
        }
    });
    
}
function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalBorrar(' +
        "'" + dataItem.Id + "'" + ',' +
        "'" + dataItem.CupoSap + "'" + ',' +
        ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
}
function ModalBorrar(id, cupo) {
    $("#cupo_a_borrar").text(cupo);
    $("#cupoBorrar").val(id);
    $("#finalizarBorrar").click(function () {
        ObtenerDatosModalBorrado();
    });

    $("#modalBorrar").modal('show');
}
function ObtenerDatosModalBorrado() {
    var objBorrar = {};
    var result;    
    objBorrar.id = $("#cupoBorrar").val();
    result = MSExecuteOnServer('/Cupo/EliminarCupo', objBorrar);    
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}
function recargarGrilla() {
    $('#gridCupo').data('kendoGrid').dataSource.read();
    AvisoContratosPendientes();
}
function InicializarCargaCupos() {
    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#Proveedor").val("");    
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
    });

    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#buscadorProveedor").val().split('|').length > 1) {
                $("#buscadorProveedor").val($("#buscadorProveedor").val().split('|')[1]);
                
            }
        },
        select: function (e) {
            $("#Proveedor").val(e.dataItem.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Cupo/BuscarProveedor"
                },
                parameterMap: function (data, type) {
                    return { filtroProveedor: $('#buscadorProveedor').val() };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

    $("#fecha").kendoDatePicker({
        value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#cantidad").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min:0
    });
    $("#cuit").mask("00000000000");
    $("#fason").click(function () {
        checkFason();
    });
    $("#material").change(function () {
        checkSoja();
    });
    $("#zona").val(zonaSeleccionada);
    if ($("#buscadorProveedor").val() != null) {
        $("#buscadorProveedor").trigger('change');
    }
}
function checkFason() {
    if ($("#fason").is(':checked')) {
        $("#cuit").show();
    }
    else {
        $("#cuit").hide("hidden");
        $("#cuit").val("");
    }
}
function checkSoja() {
    if ($("#material").val() !== "3") {
        $("#calidadDiv").hide();
        $("#calidad").val("");
    } else {
        $("#calidadDiv").show();
    }
}
