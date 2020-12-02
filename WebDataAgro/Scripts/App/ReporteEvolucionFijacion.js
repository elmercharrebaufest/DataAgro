var fechaString;
var url;
var viewModel;
var datosIniCrearContrato;

$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    fechaString = ObtenerFechaDesde();
    Inicializar();
    setInterval(Refrescar, 300000);


});

function Inicializar() {
    kendo.culture("es-AR");
    var date = new Date();
    var dateDesde = new Date(date.getFullYear() - 1, date.getMonth(), date.getDate());
    var dateHasta = new Date(date.getFullYear() + 1, date.getMonth(), date.getDate());
    $("#fecha").kendoDatePicker({
        value: dateDesde,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        //change: function () {
        //    fechaString = $("#fecha").val();
        //    fechaHastaString = $("#fechaHasta").val();
        //    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString + '&centroId=' + ObtenerValorCentroId() + '&materialId=' + ObtenerValorMaterialId().toString());
        //}
    });
    //var fechaMax = new Date($("#fecha").val().toString().split('-')[2], parseInt($("#fecha").val().toString().split('-')[1], 10) - 1, $("#fecha").val().toString().split('-')[0]);
    //fechaMax.setMonth(fechaMax.getMonth() + 6);
    $("#fechaHasta").kendoDatePicker({
        value: dateHasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        //max: fechaMax,
        //min: new Date($("#fecha").val().toString().split('-')[2], parseInt($("#fecha").val().toString().split('-')[1], 10) - 1, $("#fecha").val().toString().split('-')[0])
    });
    //$("#fechaHasta").change(function () {
    //    fechaString = $("#fecha").val();
    //    fechaHastaString = $("#fechaHasta").val();
    //    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaHastaString + '&centroId=' + ObtenerValorCentroId() + '&materialId=' + ObtenerValorMaterialId().toString());
    //});


    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").val("");
        $("#ProveedorId").val("");
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
            $("#ProveedorId").val(e.dataItem.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarProveedoresConCorredor"
                },
                parameterMap: function (data, type) {
                    return { filtro: "", filtroProveedor: $('#buscadorProveedor').val(), corredor: 0 };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

}

function ObtenerFechaDesde() {
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

function Refrescar() {
    $("#buscar-reporte").click();
}

//function ObtenerValorCentroId() {
//    return $("#centroId").val() ? $("#centroId").val() : "0";
//}
//function ObtenerValorMaterialId() {
//    //console.log($("#materialId").data("kendoMultiSelect").value());
//    //return $("#materialId").data("kendoMultiSelect").value();
//    return $("#materialId").val();
//}

function setearValoresComboDeInicio() {
    url = $('#descargaReporte').attr('href');
    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaString + '&centroId=' + ObtenerValorCentroId() + '&materialId=' + ObtenerValorMaterialId().toString());
}

$("#descargar-reporte").click(function () {
    var url = '/ReporteEvolucionFijacion/DescargarReporte';
    var fecha = $("#fecha").val();
    var fechaHasta = $("#fechaHasta").val();
    var ProveedorId = $("#ProveedorId").val();
    var ComercialId = $("#ComercialId").val();
    var CampanaId = $("#CampanaId").val();
    var MaterialId = $("#MaterialId").val();
    var GrupoCompraId = $("#GrupoCompraId").val();
    var ClasificacionId = $("#ClasificacionId").val();
    var DestinoId = $("#DestinoId").val();

    $('#descargarReporte').attr('href', url + '?fecha=' + fecha + '&fechaHasta=' + fechaHasta + '&ProveedorId=' + ProveedorId + '&ComercialId=' + ComercialId
        + '&CampanaId=' + CampanaId + '&MaterialId=' + MaterialId + '&GrupoCompraId=' + GrupoCompraId + '&ClasificacionId=' + ClasificacionId + '&DestinoId=' + DestinoId);
    document.getElementById("descargarReporte").click();
});
