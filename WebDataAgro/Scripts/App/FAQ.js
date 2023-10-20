var manuales;
var manualSeleccionado;

$(document).ready(function () {
    kendo.culture("es-AR");
    CargarManuales();
});

function CargarManuales() {
    manuales = MSExecuteOnServer("/FAQ/TraerManuales");
    var time = new Date().getTime();
    var table = "";
    var table2 = "";
    if (manuales.length > 0) {
        for (var i = 0; i < manuales.length; i++) {
            table = '<div class="col-xs-12 col-sm-12 col-md-12 centrarFAQ" id="card' + manuales[i].Id + '">';
            table += '<div class="documento">';
            table += '<a style="text-decoration:none!important" target="_blank" href="' + manuales[i].Path + '?v='+time+'" onclick="contarVisita(' + manuales[i].Id + ')">';
            table += '<div class="doc-header" id="titulo' + manuales[i].Id + '">';
            table += '<h5><i aria-hidden="true" class="fa fa-file-pdf-o"></i>   ' + manuales[i].Titulo + '  <span class="badge bg-info text-dark">Ver.' + manuales[i].Version + '</span>    <span class="badge bg-info text-dark">Últ.actualización: ' + kendo.toString(kendo.parseDate(manuales[i].FechaUltimaActualizacion), "dd/MM/yy HH:mm") + '</span>    <span class="badge bg-info text-dark">' + manuales[i].CantidadVisitas + ' visitas</span></h5>';
            table += '</div>';
            table += '<div class="doc-description" id="descripcion' + manuales[i].Id + '"><p>' + manuales[i].Descripcion + '</p></div>';
            table += '</a><button class="btn btn-primary" onclick="AbrirSugerencia(' + manuales[i].Id + ')">Realizar Sugerencia</button></div></div>';

            table2 += table;
        }

        $(".contenedorManuales").html(table2);
    } else {
        console.log("NO HAY MANUALES. AGREGAR MENSAJE.");
    }
}

function AbrirSugerencia(idManual) {
    manualSeleccionado = manuales.filter(m => m.Id === idManual);

    $(document).ready(function () {
        $("#nombreManual").text(manualSeleccionado[0].Titulo);
    });

    $("#modalEnviarSugerencia").modal('show');
}

function cancelarCuposConDescarga() {
    $("#modalEnviarSugerencia").modal("hide");
}

function enviarSugerencia() {
    var datos = {
        manual: manualSeleccionado[0],
        sugerencia: $("#sugerenciaManual").val(),
    }

    respuesta = MSExecuteOnServer("/FAQ/EnviarSugerencia", datos);

    $("#sugerenciaManual").text("");
    $("#modalEnviarSugerencia").modal("hide");
}

function contarVisita(idManual) {
    respuesta = MSExecuteOnServer("/FAQ/ContarVisita", { idManual });
}

function myFunction() {
    var input, filter, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    for (i = 0; i < manuales.length; i++) {
        txtValue = manuales[i].Titulo + ' ' + manuales[i].Descripcion;
        var id = "#card" + (manuales[i].Id).toString();
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            $(id).show();
        } else {
            $(id).hide();
        }
    }
}