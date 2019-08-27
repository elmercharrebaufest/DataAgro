$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarDate();
});

function InicializarDate() {
    kendo.culture("es-AR");
    var date = ObtenerFecha();

    $("#fechaDesde").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"],
        change: function () {
            $("#fechaHasta").data('kendoDatePicker').value($("#fechaDesde").val());
        }
    });
    fechaDesdeString = $("#fechaDesde").val();

    $("#fechaHasta").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });
    fechaHastaString = $("#fechaHasta").val();
}

function LimpiarForm() {
    $(".limpiar").val("");
    $("#MaterialId").val("");
    $("#MonedaId").val("");
    InicializarDate();
   
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

$(".alert").ready(function () {
    setTimeout(function () { $(".alert").hide(); }, 5000);
});


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

$(".number").kendoNumericTextBox({
    culture: "es-AR",
    format: "n1",
    value: " ",
    decimals: 2,
    restrictDecimals: true,
    spinners: false,
    min: 0,
});

$("#MaterialId").change(CargarPizarraHistorico);  
$("#PizarraId").change(CargarPizarraHistorico);  

function CargarPizarraHistorico() {
    var materialId = $("#MaterialId").val();
    var pizarraId = $("#PizarraId").val();
    if (materialId != null && materialId != '') {
        var tabla = $('#historico-pizarra');
        var data = (MSExecuteOnServer('/PrecioPizarra/BuscarPorPizarraYMaterial', { materialId: materialId, pizarraId: pizarraId }));
        if (data) {
            tabla.empty();
            tabla.append('<tr><th> Pizarra</th ><th>Fecha Desde</th><th>Fecha Hasta</th><th>Precio</th>' +
                '<th>Moneda</th><th>Unidad de Medida</th><th>Material</th></tr>');
            for (var i = 0; i < data.length; i++) {
                var linea = '<tr><td>' + data[i].Pizarra + '</td>';
                linea += '<td>' + kendo.toString(data[i].FechaDesde, "dd/MM/yyyy hh:mm tt") + '</td>';
                linea += '<td>' + kendo.toString(data[i].FechaHasta, "dd/MM/yyyy hh:mm tt") + '</td>';
                linea += '<td>' + data[i].Precio + '</td>';
                linea += '<td>' + data[i].Moneda + '</td>';
                linea += '<td>' + data[i].UnidadMedida + '</td>';
                linea += '<td>' + data[i].Material + '</td></tr>';
                tabla.append(linea);
            }
        }
    }

}