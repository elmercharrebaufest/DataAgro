var fechaActualizaPrecioString = "";

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

    $("#fechaActualizaPrecio").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"],
        change: function () {
            fechaActualizaPrecioString = $("#fechaActualizaPrecio").val();
        }
    });
    fechaActualizaPrecioString = $("#fechaActualizaPrecio").val();
}

function LimpiarForm() {
    $(".limpiar").val("");
    $("#MonedaId").val("ARP  ");
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
    min: 0
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
                '<th>Moneda</th><th>Unidad de Medida</th><th>Material</th><th></th></tr>');
            for (var i = 0; i < data.length; i++) {
                var linea = '<tr><td>' + data[i].Pizarra + '</td>';
                linea += '<td>' + kendo.toString(data[i].FechaDesde, "dd/MM/yyyy hh:mm tt") + '</td>';
                linea += '<td>' + kendo.toString(data[i].FechaHasta, "dd/MM/yyyy hh:mm tt") + '</td>';
                linea += '<td>' + data[i].Precio.toLocaleString('es-ES') + '</td>';
                linea += '<td>' + data[i].Moneda + '</td>';
                linea += '<td>' + data[i].UnidadMedida + '</td>';
                linea += '<td>' + data[i].Material + '</td>';
                linea += '<td> <a class="fa fa-minus-circle danger" data-ajax="true" title="Eliminar precio" data-ajax-mode="replace" data-ajax-update="#listaPrecioPizarra" href="/PrecioPizarra/EliminarPrecio/' + data[i].Id + '"> </a></td></tr>';
                tabla.append(linea);
            }
        }
    }
}

function ActualizarPrecioPizarraBCR() {

    var fechaActualizaPrecio = formatoFechaString(fechaActualizaPrecioString);
    //var currentDateObj = new Date();
    //var numberOfMlSeconds = currentDateObj.getTime();
    //var addMlSeconds = 60 * 60000 * -24;
    //var newDateObj = new Date(numberOfMlSeconds + addMlSeconds);

    //var fecha2 = formatoFecha(newDateObj, 'dd/mm/yy');
    //var fecha1 = fecha2 + " 00:00:00";

    var data = MSExecuteOnServer('/PrecioPizarra/ActualizarPrecioPizarra', { fecha: fechaActualizaPrecio, manual: true });
    if (data == "Ok") {
        MensInfo("La actualización de precios pizarra finalizó correctamente.\n\n");
    } else {
        MensInfo("La actualización de precios pizarra tuvo un error en su ejecución.\n\n");
    }
}

function formatoFecha(fecha, formato) {
    const map = {
        dd: fecha.getDate(),
        mm: fecha.getMonth() + 1,
        yy: fecha.getFullYear().toString().slice(-2),
        yyyy: fecha.getFullYear()
    }

    return formato.replace(/dd|mm|yy|yyy/gi, matched => map[matched])
}

function formatoFechaString(fechaString) {
    var partes = fechaString.split('-');
    var fechaNuevaString = `${partes[2]}/${partes[1]}/${partes[0]}`;
    var fechaDate = new Date(fechaNuevaString);

    var dia = fechaDate.getDate();
    var mes = fechaDate.getMonth() + 1; // Nota: en JavaScript, los meses comienzan desde 0
    var año = fechaDate.getFullYear() % 100; // Tomar solo los últimos dos dígitos del año
    var fechaFormateada = `${dia}/${mes}/${año} 00:00:00`;

    return fechaFormateada;
}