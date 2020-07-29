
var datosCompra;

function ArmarTablaCompra(result, materiales, campanias) {
    datosCompra = result;
    viewModel = kendo.observable({
        Compra: []
    });
    kendo.bind($("#tabla-compra"), viewModel);
    viewModel.set("Compra", datosCompra);

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
}
