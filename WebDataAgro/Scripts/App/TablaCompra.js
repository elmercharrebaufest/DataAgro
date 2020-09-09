
var datosCompra;

function ArmarTablaCompra(result, materiales, campanias) {  
    datosCompra = result;
    viewModel = kendo.observable({
        Compra: []
    });
    for (var i = 0; i < datosCompra.length; i++) {

        datosCompra[i].ConCorredor.ComprasConPrecio = kendo.toString(datosCompra[i].ConCorredor.ComprasConPrecio, "n2")
        datosCompra[i].DirectoAcopiador.ComprasConPrecio = kendo.toString(datosCompra[i].DirectoAcopiador.ComprasConPrecio, "n2")
        datosCompra[i].DirectoProductor.ComprasConPrecio = kendo.toString(datosCompra[i].DirectoProductor.ComprasConPrecio, "n2")
        datosCompra[i].ConCorredor.RecibidoSinPrecio = kendo.toString(datosCompra[i].ConCorredor.RecibidoSinPrecio, "n2")
        datosCompra[i].DirectoAcopiador.RecibidoSinPrecio = kendo.toString(datosCompra[i].DirectoAcopiador.RecibidoSinPrecio, "n2")
        datosCompra[i].DirectoProductor.RecibidoSinPrecio = kendo.toString(datosCompra[i].DirectoProductor.RecibidoSinPrecio, "n2")
        datosCompra[i].ConCorredor.ARecibirAFijar = kendo.toString(datosCompra[i].ConCorredor.ARecibirAFijar, "n2")
        datosCompra[i].DirectoAcopiador.ARecibirAFijar = kendo.toString(datosCompra[i].DirectoAcopiador.ARecibirAFijar, "n2")
        datosCompra[i].DirectoProductor.ARecibirAFijar = kendo.toString(datosCompra[i].DirectoProductor.ARecibirAFijar, "n2")
        datosCompra[i].ConCorredor.FasonFas = kendo.toString(datosCompra[i].ConCorredor.FasonFas, "n2")
        datosCompra[i].DirectoAcopiador.FasonFas = kendo.toString(datosCompra[i].DirectoAcopiador.FasonFas, "n2")
        datosCompra[i].DirectoProductor.FasonFas = kendo.toString(datosCompra[i].DirectoProductor.FasonFas, "n2")
    }
    kendo.bind($("#tabla-compra"), viewModel);
    viewModel.set("Compra", datosCompra);
    BorrarFilasVacias();
    var material = '<option> Todos </option>';
    var campania = '<option> Todas </option>';

    if (materiales.length > 0) {
        for (var i = 0; i < materiales.length; i++) {
            material += '<option> ' + materiales[i] + ' </option>'
        }
    }
    $('#MaterialId').append(
        material
    );
  
    if (campanias.length > 0) {
        for (var j = 0; j < campanias.length; j++) {
            campania += '<option>' + campanias[j] + ' </option>'
        }
    }

    $('#CampaniaId').append(
        campania
    );
    $('.Maiz td').css('background-color', 'cornsilk');
    $('.Trigo td').css('background-color', 'cornsilk');
    $('.Soja td').css('background-color', 'lightcyan');
    $('.Girasol td').css('background-color', 'lightcyan');


}

function FiltrarCampos() {

    var filtrado = datosCompra.filter(function (x) { return (x.Material == $('#MaterialId').val() || $('#MaterialId').val() == "Todos") && (x.Campana == $('#CampaniaId').val() || $('#CampaniaId').val() == "Todas") })
    viewModel.set("Compra", filtrado);
    BorrarFilasVacias();
    $('.Maiz td').css('background-color', 'cornsilk');
    $('.Trigo td').css('background-color', 'cornsilk');
    $('.Soja td').css('background-color', 'lightcyan');
    $('.Girasol td').css('background-color', 'lightcyan');
}

function BorrarFilasVacias() {
    var $filasEncabezado = $("#tabla tr:not('.encabezado')");
    Remover($filasEncabezado);

}

function Remover($filasEncabezado) {
    var listaMateriales = new Array();
    $filasEncabezado.each(function () {
        var valorFila = 0
        $(this).find('td').each(function (i) {
            if (i != 0 && i != 1 && i != 2) {
                valorFila += parseInt($(this).context.innerText);
            }
        });
        var material = $(this)[0].getAttribute('data-material');
        listaMateriales.push(material);
        if (valorFila == 0) {
            $(this).remove();
        }
    });

    listaMateriales = listaMateriales.filter((v, i, a) => a.indexOf(v) === i);
    for (var i = 0; i < listaMateriales.length; i++) {
        var material = listaMateriales[i];
        var count = $(".Material." + material).length;
        for (var j = 0; j < count; j++) {
            if (j == 0) {
                $(".Material." + material)[j].setAttribute('rowspan', count);
                $(".Campana." + material)[j].setAttribute('rowspan', count);
            } else {
                $(".Material." + material)[1].remove();
                $(".Campana." + material)[1].remove();
            }

        }
    }


}
