$(document).ready(function () {
    $('#menuproveedor').hide();
    //$('#rootwizard').bootstrapWizard({
    //    'withVisible': false
    //});
    //CrearViewModel();
    InicializarElementos();
    //InicializarDatos();
    //AutocompleteProcedencia();
    CargarGrillaConfig();
    checkSoja();
});

function InicializarElementos() {
    $("#CalidadId").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value"
    });
    $("#MaterialId").kendoDropDownList({
        change: function () {
            checkSoja();
        }
    });
    $("#CentroId").kendoDropDownList({

    });
    $("#ComercialId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId"
    });
    $("#ComercialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#ComercialId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#Fecha").kendoDatePicker({
        value: new Date(),
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#CantidadCupo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false
    });
    $("#buscadorProveedor").click(function () {
        $("#ProveedorId").val("");
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
    });


    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="#:data.Corredor# buscar-nomb" value="#:data.RazonSocial#" >#: data.RazonSocial#(#: data.Cuit#)</p>',
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
            $("#ProveedorId").val(e.dataItem.Id);
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
        //template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
        //    '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        //minLength: 3,
        //enforceMinLength: true,
        //dataTextField: "Filtro",
        //dataValueField: "Id",
        //autoWidth: true,
        //filter: "contains",
        //change: function () {
        //    if ($("#buscadorProveedor").val().split('|').length > 1) {
        //        $("#buscadorProveedor").val($("#buscadorProveedor").val().split('|')[1]);
        //    }

        //    InicializarBordesRojos();
        //},
        //select: function (e) {
        //    $("#ProveedorId").val(e.dataItem.Id);
        //},
        //dataSource: {
        //    severFiltering: true,
        //    serverPaging: true,
        //    transport: {
        //        read: {
        //            type: 'post',
        //            dataType: 'json',
        //            url: "/Proveedor/BuscarProveedoresConCorredor"
        //        },
        //        parameterMap: function (data, type) {
        //            return { filtro: "", filtroProveedor: $('#buscadorProveedor').val(), corredor: 0 };
        //        }
        //    }

        //},
        //filtering: function (e) {
        //    if (!e.filter.value) {
        //        e.preventDefault();
        //    }
        //}
    });
    $("#MaterialId").change(function () {
        checkSoja();
    });
}

function LimpiarConfiguracion() {
    $("#Id").val(0);
    $('#CentroId>option:eq(0)').prop('selected', true);
    $("#MaterialId").val("");
    var fecha = kendo.toString(kendo.parseDate(new Date()), "dd-MM-yyyy");
    $("#Fecha").val(fecha);
    $("#CantidadCupo").val("");
    $("#buscadorProveedor").data("kendoAutoComplete").value("");
    $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
}

function InicializarBordesRojos() {
    $("select.required-box, input.required-box").on("change", function (e) {
        var padre = $(this).hasClass("required-box-parent") ? $(this) : $(this).parent().parent();

        if ($(this).val() == "") {
            padre.addClass("required-border");
        } else {
            padre.removeClass("required-border");
        }
    });

    $("select.required-box, input.required-box").trigger("change");
}

function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ConfiguracionEspacioDinamico/DatosConfiguracion'
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
                    Fecha: { type: "date" },
                    LimiteCupo: { type: "number" },
                    Proveedor: { type: "string" },
                    ComercialNombre: { type: "string" },
                    Calidad: { type: "string" },
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Fecha", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#gridConfiguracionEspacioDinamico").kendoGrid({
        dataSource: ds,
        columns: [
            { field: "Centro", type: "string" },
            {
                field: "Material", title: "Cultivo", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz Duro Dentado"
                    }, {
                        Material: "Trigo Pan"
                    }, {
                        Material: "Semilla de Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Girasol Alto Oleico"
                    }]
                }, width: 130, template: "#=Material#"
            },
            { field: "Fecha", type: "date", format: _DefaultDateTemplate },
            { field: "ProveedorRazonSocial", title: "Proveedor" },
            { field: "ComercialNombre", title: "Comercial" },
            { field: "Calidad", title: "Calidad" },
            { field: "CantidadDeCupo", title: "Cantidad de Cupos" },
            {
                field: "Id", title: "Eliminar", filterable: false, sortable: false, width: 75, template: function (dataItem) {
                    return '<a data-toggle="tooltip" title="Eliminar Configuracion" class="links-grid" onclick="Eliminar(' + dataItem.Id + ')">' +
                        '<span> <i class="fa fa-trash"></i> </span ></a >';
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
    $('#gridConfiguracionEspacioDinamico').data('kendoGrid').dataSource.read();
}

function Eliminar(id) {
    BlockUi("Grabando...");

    result = MSExecuteOnServer('/ConfiguracionEspacioDinamico/Eliminar', { id: id });
    recargarGrilla();
    $.unblockUI();

    if (result.HayError) {
        MensInfo("Se elimino correctamente.");
    } else {
        ShowErrorMessages(result);
    }
}



function checkSoja() {
    if ($("#MaterialId").val() !== "3") {
        $("#calidadDiv").hide();
        $("#CalidadId").data("kendoDropDownList").value("");
    } else {
        $("#calidadDiv").show();
    }
}