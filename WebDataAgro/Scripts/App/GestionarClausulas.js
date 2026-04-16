// Gestionar Cláusulas Boleto
const GestionarClausulasModule = (() => {
    "use strict";

    const config = {
        urls: {
            generarBoleto: "/Boleto/GenerarBoletos",
            volver: "/Boleto/GenerarBoletos"
        }
    };

    const state = {
        indexClausulaAEliminar: null
    };

    const el = {
        clausulasBody: () => $("#clausulas-body"),
        numeroContrato: () => $("#numero-contrato"),
        clausulaInput: () => $("#clausula-input"),
        modalEliminar: () => $("#confirmarEliminarClausula"),
        confirmarEliminacionBtn: () => $("#confirmarEliminacionBtn"),
        numeroClausulaModal: () => $("#numeroClausulaModal"),
        guardarClausula: () => $("#guardar-clausula"),
        cancelarClausula: () => $("#cancelar-clausula"),
        generarBoletoBtn: () => $("#generar-boleto"),
        checkMail: () => $("#enviarMailBoleto"),
        volver: () => $("#volver")
    };

    // ── Renderizado ──────────────────────────────────────────────────────────────

    function renderizarClausulas() {
        el.clausulasBody().empty();
        clausulas.forEach(function (clausula, index) {
            const fila = $("<tr></tr>");
            const columnaNumero = $("<td></td>").addClass("numero-clausula").text(index + 1);
            const columnaTexto = $("<td></td>").addClass("texto-clausula").text(clausula);
            const columnaAcciones = $("<td></td>").addClass("acciones");
            const botonEditar = $("<button></button>").addClass("btn btn-default btn-editar").html('<i class="fa fa-pencil"></i> Editar').data("index", index);
            const botonEliminar = $("<button></button>").addClass("btn btn-danger btn-eliminar").html('<i class="fa fa-trash"></i> Eliminar').data("index", index);
            columnaAcciones.append(botonEditar).append(botonEliminar);
            fila.append(columnaNumero).append(columnaTexto).append(columnaAcciones);
            el.clausulasBody().append(fila);
        });
    }

    // ── Resultado ────────────────────────────────────────────────────────────────

    function mostrarResultado(result) {
        BootstrapDialog.show({
            title: 'Generar boleto',
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

    // ── Inicialización ───────────────────────────────────────────────────────────

    function mostrarValidacion() {
        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_DANGER,
            title: "Generación Contrato Boleto Fisico/Carta Oferta",
            closable: false,
            message: validacionContrato,
            buttons: [{
                label: 'Cerrar',
                action: function () {
                    window.location.href = config.urls.volver;
                }
            }]
        });
    }

    function inicializarEncabezado() {
        const texto = `Cláusulas para el contrato N° ${negocioSAP}`;
        el.numeroContrato().text(texto);
    }

    function inicializarEventos() {
        // Editar cláusula
        $(document).on("click", ".btn-editar", function () {
            const index = $(this).data("index");
            el.clausulaInput().val(clausulas[index]).data("editIndex", index);
            $("html, body").animate({ scrollTop: $(document).height() }, "slow");
        });

        // Solicitar eliminación
        $(document).on("click", ".btn-eliminar", function () {
            state.indexClausulaAEliminar = $(this).data("index");
            el.numeroClausulaModal().text(state.indexClausulaAEliminar + 1);
            el.modalEliminar().modal("show");
        });

        // Confirmar eliminación
        el.confirmarEliminacionBtn().on("click", function () {
            if (state.indexClausulaAEliminar !== null) {
                clausulas.splice(state.indexClausulaAEliminar, 1);
                renderizarClausulas();
                el.modalEliminar().modal("hide");
                state.indexClausulaAEliminar = null;
            }
        });

        // Guardar cláusula
        el.guardarClausula().on("click", function () {
            const nuevoTexto = el.clausulaInput().val();
            const editIndex = el.clausulaInput().data("editIndex");
            if (nuevoTexto !== "") {
                if (editIndex !== undefined) {
                    clausulas[editIndex] = nuevoTexto;
                    el.clausulaInput().removeData("editIndex");
                } else {
                    clausulas.push(nuevoTexto);
                }
            } else {
                MensAlerta("No se puede guardar una cláusula sin texto");
            }
            el.clausulaInput().val("");
            renderizarClausulas();
        });

        // Cancelar edición
        el.cancelarClausula().on("click", function () {
            el.clausulaInput().val("").removeData("editIndex");
        });

        // Generar boleto
        el.generarBoletoBtn().on("click", function () {
            BlockUi('Generando...');
            setTimeout(function () {
                const result = MSExecuteOnServer(config.urls.generarBoleto, {
                    ContratoSAP: negocioSAP,
                    Mail: el.checkMail().is(":checked"),
                    Clausulas: clausulas
                });
                $.unblockUI();
                if (!result || result.length === 0) {
                    MensErr("No se generó ningún boleto.");
                } else {
                    mostrarResultado(result);
                }
            }, 100);
        });

        // Volver
        el.volver().on("click", function () {
            window.history.back();
        });
    }

    // ── Init ─────────────────────────────────────────────────────────────────────

    function init() {
        if (validacionContrato > '') {
            mostrarValidacion();
        } else {
            inicializarEncabezado();
            renderizarClausulas();
            inicializarEventos();
        }
    }

    return { init };
})();

$(document).ready(() => GestionarClausulasModule.init());
