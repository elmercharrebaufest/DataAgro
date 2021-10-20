$(document).ready(function () {

    kendo.culture("es-AR");

    inicializarTodosKendoDate($(".filtroFecha"));
    //$("#fechaCargaId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
    CreateGridInformeCompraNet();
    InicializarElementos();


});


function CreateGridInformeCompraNet() {
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });

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
                url: '/ReporteContratosAFijarPase/BuscaDatosTabla',

                data: function () {

                    let filtroCompleto = TraerFiltrosConValores();

                    return filtroCompleto;
                }
            }

        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Cuit: { type: "number" },
                    Negocio: { type: "number" },
                    Fecha: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    HastaFijacion: { type: "date" },
                    FechaEntrega: { type: "date" },
                    Cantidad: { type: "number" },
                    KilosPendiente: { type: "number" },
                    PrecioPonderado: { type: "number", format: "n2" },
                    PrecioNetoPonderado: { type: "number", format: "n2" },
                    FechaOperacion: { type: "date" },
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],

        pageSize: 20,
    };

    $("#grid").kendoGrid({
        //toolbar: kendo.template($("#templateToolbar").html()),
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Contratos a Fijar Pase.xlsx",
            allPages: true,
        },
        dataSource: ds,
        //dataBound: function () {
        //    $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
        //    $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
        //    $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
        //    $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
        //    $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
        //    $("td:has(div.statusborrado)").css('border-bottom', '5px solid #848484');
        //    $("td:has(div.statuspreaprobacion)").css('border-bottom', '5px solid #15deca');
        //    $("td:has(div.statuspreanulado)").css('border-bottom', 'border-grey');
        //    $("td:has(div.statusreconfirmarfinalizado)").css('border-bottom', '5px solid #ac67ca');
        //},
        columns: [
            { field: "FechaOperacion", type: "date", title: "Fecha Operacion", format: _DefaultDateTemplate, width: 100 },
            { field: "FechaHasta", type: "date", title: "Fecha de  Entrega", format: _DefaultDateTemplate, width: 100 },
            { field: "HastaFijacion", type: "date", title: "Fecha de  Fijacion", format: _DefaultDateTemplate, width: 100 },
            { field: "Posicion", width: 90 },
            { field: "Negocio", width: 90, title: "Contrato MOA" },
            { field: "Destino", type: "string" },
            { field: "Proveedor", type: "string" },
            { field: "Corredor", type: "string" },
            { field: "Material", width: 90, template: "#=Material#" },
            { field: "Cantidad", title: "Cantidad (Kg)", format: "{0:n0}", width: 90 },
            { field: "KilosPendiente", title: "Pendiente (Kg)", format: "{0:n0}", width: 90 },            
            { field: "Plus", title: "Plus", type: "number", format: "{0:n2}", width: 90 },
            { field: "PrecioPonderado", title: "Precio MAT", type: "number", format: "{0:n2}", width: 90 },
            { field: "PrecioNetoPonderado", title: "Precio Dispo", type: "number", format: "{0:n2}", width: 90 },
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
        scrollable: true,
        height: 550,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        selectable: "row",

        filterable: false,
        filterMenuInit: function (e) {
            if (e.field == "Proveedor" || e.field == "Comercial" || e.field == "Provincia" || e.field == "Localidad") {
                $(e.container).css("width", "300px");
            } else {
                $(e.container).css("width", "150px");
            }
        }
    });
}

function ObtenerFecha() {
    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

function ObtenerFechaMas30() {
    var hoy = new Date();
    hoy.setDate(hoy.getDate() + 30);
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();

    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

function InicializarElementos() {
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    kendo.culture("es-AR");

    $("#nuevoNumContrato").kendoNumericTextBox({
        culture: "es-AR",
        format: "######################",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/ReporteContratosAFijarPase/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Corredor", "CorredorId", "/ReporteContratosAFijarPase/ListarCorredor");
    CrearMultiSelectFiltro("#ClasificacionId", "Clasificacion", "ClasificacionId", "/ReporteContratosAFijarPase/ListarClasificacion");


    $("#SustentableId").click(function () {
        if ($("#SustentableId").is(':checked')) {
            $("#Importe_SustentableId").prop('disabled', true);
            $("#Importe_SustentableId").data("kendoNumericTextBox").value("");
        } else {
            $("#Importe_SustentableId").prop('disabled', false);
        }
    });
    $("#PesificadoId").click(function () {
        if ($("#PesificadoId").is(':checked')) {
            $("#Dias_PesificadoId").prop('disabled', true);
            $("#Dias_PesificadoId").data("kendoNumericTextBox").value("");
        } else {
            $("#Dias_PesificadoId").prop('disabled', false);
        }
    });

    inicializarPopUpSap("Contratos");
}

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}

$("#DolarizadoId").click(function () {

    if ($("#DolarizadoId").is(':checked')) {
        $("#fechalimiteId").data("kendoDatePicker").enable(false);
        $("#fechalimiteId").data("kendoDatePicker").value("");
    } else {
        $("#fechalimiteId").data("kendoDatePicker").enable();
    }
});

//function filtroContratoSap(nombreDelFiltro) {

//    let filtrosContratosSap = [];
//    $("#contratos-table td").each(function (e) {

//        let valorBuscado = $("#contratos-table td")[e].innerText;

//        (valorBuscado != "" &&
//            $(valorBuscado != null)) ? filtrosContratosSap.push(new FiltroHijo(nombreDelFiltro, valorBuscado, "eq")) : null;
//    });
//    return (filtrosContratosSap.length > 0) ? new FiltroPadre("or", filtrosContratosSap) : null;
//}


function customExport() {
    //TraerFiltrosConValores();
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    };
    MSExecuteOnServerAsync('/ReporteContratosAFijarPase/Export', TraerFiltrosConValores(), funcReturn, true);
}

