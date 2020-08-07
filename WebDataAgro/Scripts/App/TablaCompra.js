
var datosCompra;

function ArmarTablaCompra(result, materiales, campanias) {
    datosCompra = result;
    viewModel = kendo.observable({
        Compra: []
    });
    for (var i = 0; i < datosCompra.length; i++) {

        datosCompra[i].ConCorredor.ComprasConPrecio = kendo.toString(datosCompra[i].ConCorredor.ComprasConPrecio, "n0")
        datosCompra[i].DirectoAcopiador.ComprasConPrecio = kendo.toString(datosCompra[i].DirectoAcopiador.ComprasConPrecio, "n0")
        datosCompra[i].DirectoProductor.ComprasConPrecio = kendo.toString(datosCompra[i].DirectoProductor.ComprasConPrecio, "n0")
        datosCompra[i].ConCorredor.RecibidoSinPrecio = kendo.toString(datosCompra[i].ConCorredor.RecibidoSinPrecio, "n0")
        datosCompra[i].DirectoAcopiador.RecibidoSinPrecio = kendo.toString(datosCompra[i].DirectoAcopiador.RecibidoSinPrecio, "n0")
        datosCompra[i].DirectoProductor.RecibidoSinPrecio = kendo.toString(datosCompra[i].DirectoProductor.RecibidoSinPrecio, "n0")
        datosCompra[i].ConCorredor.ARecibirAFijar = kendo.toString(datosCompra[i].ConCorredor.ARecibirAFijar, "n0")
        datosCompra[i].DirectoAcopiador.ARecibirAFijar = kendo.toString(datosCompra[i].DirectoAcopiador.ARecibirAFijar, "n0")
        datosCompra[i].DirectoProductor.ARecibirAFijar = kendo.toString(datosCompra[i].DirectoProductor.ARecibirAFijar, "n0")
        datosCompra[i].ConCorredor.FasonFas = kendo.toString(datosCompra[i].ConCorredor.FasonFas, "n0")
        datosCompra[i].DirectoAcopiador.FasonFas = kendo.toString(datosCompra[i].DirectoAcopiador.FasonFas, "n0")
        datosCompra[i].DirectoProductor.FasonFas = kendo.toString(datosCompra[i].DirectoProductor.FasonFas, "n0")
    }
    kendo.bind($("#tabla-compra"), viewModel);
    viewModel.set("Compra", datosCompra);
    //BorrarFilasVacias();
    var material = '<option> Todos </option>';
    var campania = '<option> Todas </option>';

    if (materiales.length > 0) {
        for (var i = 0; i < materiales.length; i++) {
            material += '<option> '+materiales[i] +' </option>'
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
   
}

function FiltrarCampos() {
    
    var filtrado = datosCompra.filter(function (x) { return (x.Material == $('#MaterialId').val() || $('#MaterialId').val() == "Todos") && (x.Campana == $('#CampaniaId').val() || $('#CampaniaId').val() == "Todas" ) })
    viewModel.set("Compra", filtrado);
    //BorrarFilasVacias();
}

function BorrarFilasVacias() {
    var $filasEncabezado = $("#tabla tr:not('.encabezado')");
    var nColumnas = $("#tabla tr:last td").length;
    //Remover($filasEncabezado, nColumnas);   

}

function Remover($filasEncabezado, nColumnas) {
    var totales = [];
    for (var i = 1; i < nColumnas; i++) {
        totales.push(0);
    }

    $filasEncabezado.each(function () {
        var valorFila = 0
        $(this).find('td').each(function (i) {
            if (i != 0) {
                valorFila += parseInt($(this).context.innerText);
            }
        });
        if (valorFila == 0) {
            $(this).remove();
        }
    });
}
