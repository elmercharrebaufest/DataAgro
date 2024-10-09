var indexClausulaAEliminar = null;

$(document).ready(function () {

    renderizarClausulas();

    if (tipoNegocio == 2)
        $("#numero-contrato").text("Cláusulas para la fijación N° " + negocioSAP);
    else
        $("#numero-contrato").text("Cláusulas para el contrato N° " + negocioSAP);
});

function renderizarClausulas() {
    $("#clausulas-body").empty();
    clausulas.forEach(function (clausula, index) {
        var fila = $("<tr></tr>");

        var columnaNumero = $("<td></td>").addClass("numero-clausula").text(index + 1);
        var columnaTexto = $("<td></td>").addClass("texto-clausula").text(clausula);
        var columnaAcciones = $("<td></td>").addClass("acciones");

        var botonEditar = $("<button></button>").addClass("btn btn-default btn-editar").html('<i class="fa fa-pencil"></i> Editar').data("index", index);
        var botonEliminar = $("<button></button>").addClass("btn btn-danger btn-eliminar").html('<i class="fa fa-trash"></i> Eliminar').data("index", index);

        columnaAcciones.append(botonEditar).append(botonEliminar);

        fila.append(columnaNumero).append(columnaTexto).append(columnaAcciones);
        $("#clausulas-body").append(fila);
    });
}

// Evento para editar
$(document).on("click", ".btn-editar", function () {
    var index = $(this).data("index");
    $("#clausula-input").val(clausulas[index]).data("editIndex", index);
    // Ir hasta el final de la página
    $("html, body").animate({ scrollTop: $(document).height() }, "slow");
});

// Evento para eliminar
$(document).on("click", ".btn-eliminar", function () {
    indexClausulaAEliminar = $(this).data("index");
    var numeroClausula = indexClausulaAEliminar + 1;
    $("#numeroClausulaModal").text(numeroClausula);
    $("#confirmarEliminarClausula").modal("show");
});

$("#confirmarEliminacionBtn").on("click", function () {
    if (indexClausulaAEliminar !== null) {
        clausulas.splice(indexClausulaAEliminar, 1);
        renderizarClausulas();
        $("#confirmarEliminarClausula").modal("hide");
        indexClausulaAEliminar = null;
    }
});

// Evento para guardar cláusula
$("#guardar-clausula").click(function () {
    var nuevoTexto = $("#clausula-input").val();
    var editIndex = $("#clausula-input").data("editIndex");

    if (nuevoTexto != "") {
        if (editIndex !== undefined) { //Editar existente
            clausulas[editIndex] = nuevoTexto;
            $("#clausula-input").removeData("editIndex");
        } else { // Agregar nueva
            clausulas.push(nuevoTexto);
        }
    } else {
        MensAlerta("No se puede guardar una cláusula sin texto");
    }

    $("#clausula-input").val("");
    renderizarClausulas();
});

$("#cancelar-clausula").click(function () {
    $("#clausula-input").val("").removeData("editIndex");
});

// Evento para generar boleto
$("#generar-confirma").click(function () {
    BlockUi('Generando...');
    setTimeout(function () {
        var data = {
            ContratoSAP: negocioSAP,
            TipoNegocioId: tipoNegocio,
            IsWebService: $("#serviceConfirma").is(":checked"),
            Clausulas: clausulas
        };
        var result = MSExecuteOnServer('/Confirma/GenerarConfirma', data);
        $.unblockUI();
        if (result.length == 0) {
            MensErr("No se generó ningún confirma.");
        } else {
            var tablaFila = '<tr><td>'
                + result.confirmasGenerados[0].NegocioSAP
                + '</td><td>'
                + result.confirmasGenerados[0].FechaGeneracionFormateada
                + '</td><td>'
                + (result.confirmasGenerados[0].Generado ? '<i class="fa fa-check generado" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
                + '</td><td>'
                + (result.confirmasGenerados[0].IsWebService ? '<i class="fa fa-check web" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
                + (result.confirmasGenerados[0].Generado ? ('<a href="/Confirma/DescargarArchivoConfirma?nombreArchivo=' + result.confirmasGenerados[0].Archivo + '" class="k-button k-button-icontext" style="height: 34px;text-align: center;margin-left: 1rem;"><i class="fa fa-download generado" style="text-align: center"></i></a>') : "")
                + '</td><td>'
                + result.confirmasGenerados[0].Mensaje
                + '</td></tr>';
            BootstrapDialog.show({
                title: 'Generar confirma',
                message: `
                <div class="dialogTabla">
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Negocio SAP</th>
                                <th>Fecha Generación</th>
                                <th>Generado</th>
                                <th>Servicio Web</th>
                                <th>Mensaje</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${tablaFila}
                        </tbody>
                    </table>
                 </div>
                `,
                draggable: true,
                closable: false,
                buttons: [{
                    label: 'Cerrar y volver',
                    cssClass: 'k-button',
                    action: function (dialogItself) {
                        dialogItself.close();
                        window.location.href = `/Confirma/GenerarConfirma`;
                    }
                }]
            });
        }
    }, 100);
});

$("#volver").click(function () {
    window.history.back();
});

function descargarArchivoConfirma(nombreArchivo) {
    // Cambia la URL según la ruta correcta de tu controlador
    window.location.href = '/Confirma/DescargarArchivoConfirma?nombreArchivo=' + encodeURIComponent(nombreArchivo);
}