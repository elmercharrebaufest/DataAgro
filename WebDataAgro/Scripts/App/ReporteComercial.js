$(document).ready(function () {
    inicializarElementos();

    cargarCombos();

    CreateGridReporte();
});

function inicializarElementos() {
    kendo.culture("es-AR");

    $("#compras").click();

    $("#txtdesde").kendoDatePicker();
    $("#txthasta").kendoDatePicker();

    $("#btn-filtrar").click(function () {
        filtrar();
    });

    $("#butDescargar").click(function () {
        DescargarElementos();
    });
}

var DescargarElementos = function () {
    var obj = {
        ComercialID: $("#txtcomercial").val(),
        Cuit: $("#txtcuit").val(),
        MaterialID: $("#txtgrano").val(),
        EstadoId: $("#txtestado").val()
    };

    var funcReturn = function (data) {
        if (data != null) {
            if (data.Errores.length > 0) {
                MensErr("No se encontraron resultados para los filtros elegidos.");
                return false;
            }
            else {
                if (data.DownloadKey.length > 0) {
                    var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                    window.location = url;
                }
            }
        }
        else {
            MensErr("No se encontraron resultados para los filtros elegidos.");
            return false;
        }
    };

    MSExecuteOnServerAsync('/InformeComercial/GenerarExcelIA', obj, funcReturn, true);
};

function cargarCombos() {
    function funcReturn(datos) {
        if (datos.come.length > 1) {
            var comercial = $("#txtcomercial");
            comercial.empty();
            $("<option>").text('- Elegir').val('null').appendTo(comercial);
            for (var ii in datos.come) {
                (function (i) {
                    $("<option>").text(datos.come[i].IdActiveDirectory).val("" + datos.come[i].ComercialId).appendTo(comercial);
                })(ii);
            }
        } else {
            $("#comercial").hide();
        }

        var granos = $("#txtgrano");
        granos.empty();
        $("<option>").text('- Elegir').val('null').appendTo(granos);
        for (ii in datos.mat) {
            (function (i) {
                $("<option>").text(datos.mat[i].Descripcion).val("" + datos.mat[i].MaterialId).appendTo(granos);
            })(ii);
        }

        var estados = $("#txtestado");
        estados.empty();
        $("<option>").text('- Elegir').val('null').appendTo(estados);
        $("<option>").text('Pendiente de Generación').val('-1').appendTo(estados);
        for (ii in datos.estic) {
            (function (i) {
                $("<option>").text(datos.estic[i].Descripcion).val("" + datos.estic[i].EstadoInformeId).appendTo(estados);
            })(ii);
        }
    }

    MSExecuteURLOnServerAsync('/Reportes/TraerDatosCombo', funcReturn, false);
}

function descargarPdf(InformeComercialId) {
    function funcReturn(datos) {
        if (datos.DownloadKey.length > 0) {
            var url = MSGetUrl('/DownLoad/Reporte?key=' + datos.DownloadKey);

            window.open(window.location.origin + "/" + url, '_blank');
        }
    }

    MSExecuteOnServerAsync('/InformeComercial/ReImprimirPDF', { InformeComercialId: InformeComercialId }, funcReturn, false);
}

function CreateGridReporte() {
    $("#grilla-informes").kendoGrid({
        columns: [

            { field: "Cuit", width: 140 },
            { field: "RazonSocial", title: "Razón Social", width: 150 },
            { field: "FechaDeGeneracion", title: "Fecha de Generación", template: "#= FechaDeGeneracion ? kendo.toString(kendo.parseDate(FechaDeGeneracion), 'dd/MM/yyyy') : '' #" },
            { field: "Comercial" },
            { field: "Estado" },
            { field: "Material" },
            { field: "Observaciones" }
        ],
        height: 350,
        scrollable: true,
        sortable: true,
        selectable: "row",
        pageable: {
            messages: {
                display: "{0} - {1} de {2} elementos",
                empty: "No hay elementos para mostrar",
                page: "Página",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por página",
                first: "Ir a la primer página",
                previous: "Ir a la página anterior",
                next: "Ir a la página siguiente",
                last: "Ir a la última página",
                refresh: "Recargar"
            },
            input: true,
            numeric: false
        },
        filterable: {
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
                    neq: "Distinto",
                    gte: "Después o igual a",
                    gt: "Después",
                    lte: "Antes o igual a",
                    lt: "Antes"
                },
                number: {
                    eq: "Igual a",
                    neq: "Distinto a",
                    gte: "Mayor que o igual a",
                    gt: "Mayor que",
                    lte: "Menor que o igual a",
                    lt: "Menor que"
                }
            }
        }
    });
}

function filtrar() {
    kendo.culture("es-AR");

    var obj = {
        ComercialID: $("#txtcomercial").val(),
        Cuit: $("#txtcuit").val(),
        MaterialID: $("#txtgrano").val(),
        EstadoId: $("#txtestado").val()
    };

    function funcReturn(datos) {
        var grid = $("#grilla-informes").data("kendoGrid");
        var dataSource = new kendo.data.DataSource({
            data: datos,
            pageSize: 10
        });

        grid.setDataSource(dataSource);
    }

    MSExecuteOnServerAsync('/InformeComercial/ListarReportes', obj, funcReturn, false);
}