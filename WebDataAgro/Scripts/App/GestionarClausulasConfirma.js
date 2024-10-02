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
            BootstrapDialog.show({
                title: 'Generar confirma',
                message: "\n" + result[0].Mensaje,
                draggable: true,
                buttons: [{
                    label: 'Cerrar y volver',
                    cssClass: 'k-button',
                    action: function (dialogItself) {
                        dialogItself.close();
                        window.history.back();
                    }
                }]
            });
        }
    }, 100);
});

$("#volver").click(function () {
    window.history.back();
});