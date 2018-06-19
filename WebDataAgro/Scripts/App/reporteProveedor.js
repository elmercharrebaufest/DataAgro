$(document).ready(function () {
    InicializarElementos();
});

function InicializarElementos() {
    kendo.culture("es-AR");
    $("#buscar").click(function () {
        buscar();
    });
    $("#valor").keyup(function (x) { 
        var code = x.which;
        if(code==13){
            buscar();
        }
    });
    $("#volver").click(function () {
        volver();
    });
}

function buscar() {
    var valor = $("#valor").val();
    if (!valor || !valor.trim()) {
        MensErr("El campo no puede estar vacío");
        return false;
    }

    var obj = {};
    obj.Valor = valor;
    var result = MSExecuteOnServer('/Proveedor/ObtenerReporteProveedor', obj);

    console.log("result", result);

    if (result.length == 0) {
        MensErr("No se encontraron resultados");
        return false;
    } else {
        
        armarResultado(result);
    }
}

function armarResultado(result) {
    $(".cuerpo-tabla").empty();
    $(".formulario").hide();
    $(".resultado").show();
    $(".Titulo").html("Resultados").css({ "margin": 20, 'text-align': 'center' });
    kendo.culture("es-AR");
    /*var htmlResult = "";
    for (var ii in result) {
        (function (i) {
            htmlResult += '<div class="linea-cuerpo-tabla">'
            + '<div class="celda-tabla">'
            + result[i].ProveedorId
            + '</div>'
            + '<div class="celda-tabla">'
            + result[i].CUIT
            + '</div>'
            + '<div class="celda-tabla">'
            //+ '<a target="_blank" href="' + MSGetUrl("/proveedor/Detalle?ProveedorId=" + result[i].ProveedorId) + '">' + result[i].RazonSocial + '</a>'
            + result[i].RazonSocial
            + '</div>'
            + '<div class="celda-tabla">'
            + result[i].Estado
            + '</div>'
            + '<div class="celda-tabla">'
            + result[i].Comerciales
            + '</div>'
            + '</div>';
        })(ii);
    }

    $(".cuerpo-tabla").append(htmlResult);*/

    $("#grid").kendoGrid({
        dataSource: result,
        height: 472,
        //groupable: true,
        sortable: true,
        pageable: {
            refresh: true
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
                    lt: "Antes",
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
        },
        columns: [{
            field: "ProveedorId",
            title: "Id",
            width: 70,
            filterable:true
        }, {
            field: "CUIT",
            title: "CUIT",
            width: 120,
            filterable: true
        }, {
            field: "RazonSocial",
            title: "Razón Social",
            filterable: true
        }, {
            field: "Estado",
            title: "Estado",
            width: 120,
            filterable: true
        }, {
            field: "Comerciales",
            title: "Comerciales",
            filterable: true
        }]
    });

    var grid = $("#grid").data("kendoGrid");
    grid.dataSource.pageSize(12);
    grid.refresh();


}

function volver() {
    $(".resultado").hide();
    $(".Titulo").html("Reporte de Proveedor").css({ "margin": '', 'text-align': '' })
    $(".formulario").show();
}