// Gestionar Cláusulas Confirma
const GestionarClausulasConfirmaModule = (() => {
    "use strict";

    const config = {
        urls: {
            generarConfirma: "/Confirma/GenerarConfirma",
            generarConfirmaAltaBorrador: "/Confirma/GenerarConfirmaAltaBorrador",
            descargarArchivo: "/Confirma/DescargarArchivoConfirma",
            volverConfirma: "/Confirma/GenerarConfirma"
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
        generarConfirmaBtn: () => $("#generar-confirma"),
        generarConfirmaAltaBorradorBtn: () => $("#generar-confirma-alta-borrador"),
        checkDescargar: () => $("#checkDescargar"),
        serviceConfirma: () => $("#serviceConfirma"),
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
        const descargaHabilitada = el.checkDescargar().is(":checked");
        const c = result.confirmasGenerados[0];
        const iconCheck = '<i class="fa fa-check generado" aria-hidden="true" style="color:green;text-align:center"></i>';
        const iconTimes = '<i class="fa fa-times generado" aria-hidden="true" style="color:red;text-align:center"></i>';
        const btnDescarga = c.Generado && descargaHabilitada
            ? `<a href="${config.urls.descargarArchivo}?nombreArchivo=${c.Archivo}" class="k-button k-button-icontext" style="height:34px;text-align:center;margin-left:1rem;"><i class="fa fa-download generado" style="text-align:center"></i></a>`
            : "";

        const tablaFila = `<tr>
            <td>${c.NegocioSAP}</td>
            <td>${c.FechaGeneracionFormateada}</td>
            <td>${c.Generado ? iconCheck : iconTimes}</td>
            <td>${c.IsWebService ? iconCheck : iconTimes}${btnDescarga}</td>
            <td>${c.Mensaje}</td>
        </tr>`;

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
                        <tbody>${tablaFila}</tbody>
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
                    window.history.back();
                }
            }],
            onshown: function (dialog) {
                $(dialog.getModalDialog()).addClass('modal-confirma');
            }
        });
    }

    // ── Inicialización ───────────────────────────────────────────────────────────

    function mostrarValidacion() {
        BootstrapDialog.show({
            type: BootstrapDialog.TYPE_DANGER,
            title: "Generación Contrato Confirma",
            closable: false,
            message: validacionContrato,
            buttons: [{
                label: 'Cerrar',
                action: function () {
                    window.location.href = config.urls.volverConfirma;
                }
            }]
        });
    }

    function inicializarEncabezado() {
        const texto = tipoNegocio == 2
            ? `Cláusulas para la fijación N° ${negocioSAP}`
            : `Cláusulas para el contrato N° ${negocioSAP}`;
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

        // Generar confirma
        el.generarConfirmaBtn().on("click", function () {
            BlockUi('Generando...');
            setTimeout(function () {
                MSExecuteOnServerAsync(config.urls.generarConfirma, {
                    ContratoSAP: negocioSAP,
                    TipoNegocioId: tipoNegocio,
                    IsWebService: el.serviceConfirma().is(":checked"),
                    Clausulas: clausulas
                })
                    .then(function (result) {
                        $.unblockUI();
                        if (!result || !result.confirmasGenerados || !result.confirmasGenerados.length) {
                            MensErr("No se generó ningún confirma.");
                        } else {
                            mostrarResultado(result);
                        }
                    })
                    .catch(function (e) {
                        $.unblockUI();
                        console.error("Error al generar confirma:", e);
                        MensErr("Ocurrió un error inesperado. Inténtelo nuevamente.");
                    });
            }, 100);
        });

        // Generar confirma alta borrador
        el.generarConfirmaAltaBorradorBtn().on("click", function () {
            BlockUi('Generando...');
            setTimeout(function () {
                MSExecuteOnServerAsync(config.urls.generarConfirmaAltaBorrador, {
                    ContratoSAP: negocioSAP,
                    IsWebService: el.serviceConfirma().is(":checked"),
                    Clausulas: clausulas
                })
                    .then(function (result) {
                        $.unblockUI();
                        if (!result || !result.confirmasGenerados || !result.confirmasGenerados.length) {
                            MensErr("No se generó ningún confirma alta borrador.");
                        } else {
                            mostrarResultado(result);
                        }
                    })
                    .catch(function (e) {
                        $.unblockUI();
                        console.error("Error al generar confirma alta borrador:", e);
                        MensErr("Ocurrió un error inesperado. Inténtelo nuevamente.");
                    });
            }, 100);
        });

        // Volver
        el.volver().on("click", function () {
            window.history.back();
        });
    }

    // ── API pública ──────────────────────────────────────────────────────────────

    return {
        init: function () {
            if (validacionContrato > '') {
                mostrarValidacion();
                return;
            }
            renderizarClausulas();
            inicializarEncabezado();
            inicializarEventos();
        }
    };
})();

$(document).ready(function () { GestionarClausulasConfirmaModule.init(); });

// Función global para compatibilidad con HTML
function DescargarArchivoConfirma(nombreArchivo) {
    window.location.href = '/Confirma/DescargarArchivoConfirma?nombreArchivo=' + encodeURIComponent(nombreArchivo);
}
