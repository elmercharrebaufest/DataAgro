$(document).ready(function () {
    kendo.culture("es-AR");
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
    $("#fechaDesde").kendoDatePicker({
        change: function () {
            $("#fechaHasta").data("kendoDatePicker").value("");
            var datepicker = $("#fechaHasta").data("kendoDatePicker");
            datepicker.min(kendo.parseDate($("#fechaDesde").val()));
            datepicker.value(kendo.parseDate($("#fechaDesde").val()));
            CrearTablaFechaHasta();
        }
    });

    $("#fechaHasta").kendoDatePicker({
        min: kendo.parseDate($("#fechaDesde").val()),
        change: function () {
            CrearTablaFechaHasta();
            $("#boton-carga-masiva").show();
        }
    });
    if ($("#fechaHasta").val() != $("#fechaDesde").val()) {
        $("#boton-carga-masiva").show();
    }
    $("#cantidad").kendoNumericTextBox({
        optionLabel: "SELECCIONE CANTIDAD DE CUPOS...",
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        change: function () {
            $(".cantidad-masiva").val($("#cantidad").data('kendoNumericTextBox').value());
        }
    });
    CrearTablaFechaHasta();
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
    var fechaDesde = kendo.toString(kendo.parseDate(new Date()), "dd-MM-yyyy");
    $("#FechaDesde").val(fechaDesde);
    var fechaHasta = kendo.toString(kendo.parseDate(new Date()), "dd-MM-yyyy");
    $("#FechaDesde").val(fechaHasta);
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
                        Material: "Maiz"
                    }, {
                        Material: "Trigo"
                    }, {
                        Material: "Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Sorgo"
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

function CrearTablaFechaHasta() {
    $(".fila-carga").remove();

    var date1 = $("#fechaDesde").val();
    var date2 = $("#fechaHasta").val();
    var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

    for (var i = 0; i <= diffDays; i++) {

        var fila = '<tr class="fila-carga"><input name="Dias[' + i + '].Fecha" value="' + date1 + '" type="hidden"/><td>' + date1 + '</td><td><input min="0" id="cantidad' + i + '"  name="Dias[' + i + '].Cantidad" class="cantidad-masiva" value="' + $("#cantidad").data('kendoNumericTextBox').value() + '"/></td></tr>';
        $("#carga-cupos-table").append(fila);
        var newdate = kendo.parseDate(date1);

        newdate.setDate(newdate.getDate() + 1); var dd = newdate.getDate();
        var mm = newdate.getMonth() + 1;
        var y = newdate.getFullYear();

        date1 = dd + '/' + mm + '/' + y;
    }
    $(".cantidad-masiva").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        step: 0
    });

    $("#cancelar-carga").click(function () {
        $(".cantidad-masiva").val($("#cantidad").data('kendoNumericTextBox').value());
        $("#cancelar-carga").unbind('click');
        var date1 = $("#fechaDesde").val();
        var date2 = $("#fechaHasta").val();
        var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

        for (var i = 0; i <= diffDays; i++) {
            $("#cantidad" + i).val("0");
        }
    });
    $('[name="Dias[0].Cantidad"]').change(function () {
        if ($("#cantidad").val() == 0) {
            $("#cantidad").data('kendoNumericTextBox').value($('[name="Dias[0].Cantidad"]').val());
            $("#cantidad").data("kendoNumericTextBox").trigger("change");
        }
    });
}
function ActualizarCantidad(cantidadDias) {
    $(document).ready(function () {
        for (var i = 0; i < cantidadDias.length; i++) {
            $('[name="Dias[' + i + '].Cantidad"]').data('kendoNumericTextBox').value(cantidadDias[i].Cantidad);
        }
    });
}
function MostrarCarga() {
    $("#CargaCupos").modal('toggle');
}
function checkSoja() {
    if ($("#MaterialId").val() !== "3") {
        $("#calidadDiv").hide();
        $("#CalidadId").data("kendoDropDownList").value("");
    } else {
        $("#calidadDiv").show();
    }
}

//function SetearMasivoEnCero () {
//    var date1 = $("#fechaDesde").val();
//    var date2 = $("#fechaHasta").val();
//    var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

//    for (var i = 0; i <= diffDays; i++) {
//        if ($("#cantidad" + i).val() !== '') {
//            $("#cantidad" + i).val("0");
//        }
//    }
//}

function CancelarModal() {
    var date1 = $("#fechaDesde").val();
    var date2 = $("#fechaHasta").val();
    var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

    for (var i = 0; i <= diffDays; i++) {
        $("#cantidad" + i).data('kendoNumericTextBox').value(0);
    }
}
