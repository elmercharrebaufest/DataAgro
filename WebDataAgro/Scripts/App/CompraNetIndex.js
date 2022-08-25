
var viewModel;
var datosIniCrearContrato;
var filasSeleccionadas = {};
var comercialId;
var creaNegocios;
var modificaNegocios;
var confirmaNegocios;
var confirmarNegocioCorredoresBsAs;
var confirmaNegocios;
var confirmaNegocios;
var confirmaNegocios;
var confirmaNegocios;
var eliminaNegocios;
var finalizaNegocios;
var ampliaNegocios;
var verMesa;
var modificaFinalizados;
var externo;
var precioMoa;
var ocultarEnTablero;
var preanular;
var anular;
var esCanje;
var esPrestamo;
var modificacionFijacionDolarizado;
var modificacionFijacionDolarizadoExpress;
var esVenta;
var confirmarNegociosMaiz;
var confirmarNegociosSoja;
var confirmarNegociosTrigo;
var confirmarNegociosGirasol;
var confirmarNegociosGirasolAO;
var esVirtual;
var asociarNegocios;
var asociados = [];
var actualizacionMs;

$(document).ready(function () {
    creaNegocios = ConvertirStringABool(creaNegocios);
    modificaNegocios = ConvertirStringABool(modificaNegocios);
    ocultarEnTablero = ConvertirStringABool(ocultarEnTablero);
    eliminaNegocios = ConvertirStringABool(eliminaNegocios);
    finalizaNegocios = ConvertirStringABool(finalizaNegocios);
    confirmaNegocios = ConvertirStringABool(confirmaNegocios);
    confirmaNegocioCorredoresBsAs = ConvertirStringABool(confirmaNegocioCorredoresBsAs);
    confirmaNegocioCorredoresRosario = ConvertirStringABool(confirmaNegocioCorredoresRosario);
    confirmaNegocioOrigCentro = ConvertirStringABool(confirmaNegocioOrigCentro);
    confirmaNegocioOrigNorte = ConvertirStringABool(confirmaNegocioOrigNorte);
    confirmaNegocioOrigSur = ConvertirStringABool(confirmaNegocioOrigSur);
    ampliaNegocios = ConvertirStringABool(ampliaNegocios);
    verMesa = ConvertirStringABool(verMesa);
    externo = ConvertirStringABool(externo);
    preanular = ConvertirStringABool(preanular);
    anular = ConvertirStringABool(anular);
    esCanje = ConvertirStringABool(esCanje);
    esPrestamo = ConvertirStringABool(esPrestamo);
    modificaFinalizados = ConvertirStringABool(modificaFinalizados);
    modificacionFijacionDolarizado = ConvertirStringABool(modificacionFijacionDolarizado);
    modificacionFijacionDolarizadoExpress = ConvertirStringABool(modificacionFijacionDolarizadoExpress);
    esVenta = ConvertirStringABool(esVenta);
    confirmarNegociosMaiz = ConvertirStringABool(confirmarNegociosMaiz);
    confirmarNegociosSoja = ConvertirStringABool(confirmarNegociosSoja);
    confirmarNegociosTrigo = ConvertirStringABool(confirmarNegociosTrigo);
    confirmarNegociosGirasol = ConvertirStringABool(confirmarNegociosGirasol);
    confirmarNegociosGirasolAO = ConvertirStringABool(confirmarNegociosGirasolAO);
    esVirtual = ConvertirStringABool(esVirtual);
    asociarNegocios = ConvertirStringABool(asociarNegocios);
    kendo.culture("es-AR");

    $('#menuproveedor').hide();
    $("#demo").on("hide.bs.collapse", function () {
        $(".btn").html('<span class="icono"><i class="fa fa-plus-square-o" aria-hidden="true"></i></span>');
    });
    $("#demo").on("show.bs.collapse", function () {
        $(".btn").html('<span class="icono"><i class="fa fa-minus-square-o" aria-hidden="true"></i></span>');
    });

    $("#footwear").on("hide.bs.collapse", function () {
        $(".servicioBtn").html(' <span class="icono servicioBtn margen"> <i class="fa fa-chevron-circle-down black-color" aria-hidden="true"></i></span>');
    });
    $("#footwear").on("show.bs.collapse", function () {
        $(".servicioBtn").html(' <span class="icono servicioBtn margen"> <i class="fa fa-chevron-circle-up black-color" aria-hidden="true"></i></span>');
    });

    CrearViewModel();
    if (!externo) {
        InicializarBuscador();
    } else {
        InicializarPrecioMOA();
    }
    SetearPrecioMoa();
    CreateGridInformeCompraNet();
    AutoRecargar();
    //+ datos modal//
    $("#reenviarMails").click(function () {
        $("#reenviarMails").hide();
        $("#reenviarMailsSpinner").show();
        setTimeout(function () {
            ObtenerDatosModalFinalizado();
            $("#modalFinalizado").modal("hide");
            $("#reenviarMailsSpinner").hide();
            $("#reenviarMails").show();
        }, 0);
    });
    inicializarPopUpSap("Contratos");

    $(".btnSubmit").click(function () {
        var self = this;
        $(self).prop('disabled', true);
        setTimeout(function () { $(self).prop('disabled', false); }, 300);
        $(".modal").modal('hide');
        return true;
    });

    $("#finalizarConfirmado").click(function () {
        ObtenerDatosModalConfirmado();
    });
    $("#aprobar-contrato").click(function () {
        Aprobar();
    });
    $("#finalizarBorrar").click(function () {
        ObtenerDatosModalBorrado();
    });
    $("#rechazarPreanular").click(function () {
        ObtenerDatosModalBorrarPreAnular();
    });
    $("#preAnular").click(function () {
        ObtenerDatosModalPreAnular();
    });

    $("#finalizarConError").click(function () {
        ObtenerDatosModalConError();
    });

    $("#guardarAmpliaciones").click(function () {
        ObtenerDatosModalAmpliaciones();
    });

    $("#cancelarAmpliaciones").click(function () {
        $("#inputAmpliaciones").val('');
        $("#contratoIdAmpliaciones").val('');
    });

    $(".masDatos").on("click", function () {
        $('.datosEditarAdicionales').show();
        $('.masDatos').hide();
        $('.datosEditar').hide();
    });
    $(".menosDatos").on("click", function () {
        $('.datosEditarAdicionales').hide();
        $('.masDatos').show();
        $('.datosEditar').show();
    });

    $(".masDatosPendiente").on("click", function () {
        $('.datosEditarAdicionalesPendiente').show();
        $('.masDatosPendiente').hide();
        $('.datosEditarPendiente').hide();
        CerrarDatosPendientes();
    });
    $(".menosDatosPendiente").on("click", function () {
        $('.datosEditarAdicionalesPendiente').hide();
        $('.masDatosPendiente').show();
        $('.datosEditarPendiente').show();
    });
    $("#crearContrato").click(function () {

        window.location.href = window.location.origin + "/CompraNet/CrearContrato";

    });
    $("#cerrarVarios").click(function () {
        $(".modal").modal('hide');
        recargarGrilla();
    });
    $("#cerrarVariosFinalizado").click(function () {
        $(".modal").modal('hide');
        recargarGrilla();
    });
    $("#cerrarVariosModificar").click(function () {
        $(".modal").modal('hide');
        recargarGrilla();
    });

    $("#modalVisualizar").on("hidden.bs.modal", function () {
        $("#mostrar").hide();
        $("#ocultar").hide();
        $("#datosContrato").show();
        $("#tablaModificacion").hide();
    });

});
function htmlEncode(value) {
    if (value == null) value = "";
    return $('<div/>').text(value.toString().replace(/(\r\n|\n|\r)/gm, " ")).html();
}

function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
}
function FormatearString(string, moneda) {
    var numero = (parseFloat(string)).toLocaleString('es-AR', { minimumFractionDigits: 2 }) + ' ' + moneda;
    return numero;
}
function botonPendiente(dataItem, icono, esModalVisualizar) {
    if ((modificaNegocios || (dataItem.Canje == true && esCanje)) && !externo &&
        (dataItem.PrestamoDevolucion != true && dataItem.Venta != true && dataItem.Virtual != true &&
            (dataItem.Canje == null || dataItem.Canje == false) || (dataItem.Canje == true && esCanje) ||
            (dataItem.Canje != true && dataItem.Venta != true && dataItem.Virtual != true &&
                (dataItem.PrestamoDevolucion == null || dataItem.PrestamoDevolucion == false) || (dataItem.PrestamoDevolucion == true && esPrestamo)) ||
            (dataItem.Canje != true && dataItem.PrestamoDevolucion != true && dataItem.Virtual != true &&
                (dataItem.Venta == null || dataItem.Venta == false) || (dataItem.Venta == true && esVenta)) ||
            (dataItem.Canje != true && dataItem.Venta != true && dataItem.PrestamoDevolucion != true &&
                (dataItem.Virtual == null || dataItem.Virtual == false) || (dataItem.Virtual == true && esVirtual)))) {
        var cerrarModalVisualizar = '';
        if (esModalVisualizar == true) {
            cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
        }
        return '<button data-toggle="tooltip" title="Editar" ' +
            cerrarModalVisualizar +
            'onclick="editarContrato(' +
            "'" + dataItem.Id + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ')"><i class="fa ' + icono + '"></i></button>';
    } else {
        return "<div</div>";
    }
}
function botonNoMostrarEnTablero(dataItem, color) {
    if (ocultarEnTablero) {//crear permiso nuevo
        var title = "Oculto en Tablero";
        var icono = " fa-star-o " + color;
        if (dataItem.OcultarEnTablero == false) {
            title = "Visible en Tablero";
            icono = " fa-star " + color;
        }
        return '<button data-toggle="tooltip" title="' + title + '" onclick="cambiarMarca(' +
            "'" + dataItem.Id + "'" + "," + !dataItem.OcultarEnTablero + ')"><i class="fa ' + icono + '"></i></button>';
    } else {
        return "<div</div>";
    }
}
function cambiarMarca(id, ocultar) {
    var marcar = { id: id, OcultarEnTablero: ocultar };
    result = MSExecuteOnServer('/CompraNet/NoMostrarEnTablero', marcar);
    recargarGrilla();
}
function botonModificarFinalizados(dataItem, icono, esModalVisualizar) {
    var cerrarModalVisualizar = ' ';
    if (esModalVisualizar == true) {
        cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
    }
    if (((modificaFinalizados && (dataItem.Virtual == false || dataItem.Virtual == null)) || (dataItem.Canje == true && esCanje) || dataItem.Virtual == false)
        && (dataItem.ContratoId || dataItem.FijacionDePrecioContratoId)
        && (dataItem.TipoNegocio != "FIJACION PASE")
        && ((dataItem.PrestamoDevolucion != true && dataItem.Canje != true)
            || (dataItem.Canje == true && esCanje)
            || (dataItem.PrestamoDevolucion == true && esPrestamo))
        || (!modificaFinalizados && dataItem.TipoNegocioId == 3 && (modificacionFijacionDolarizado || modificacionFijacionDolarizadoExpress))) {
        return '<button data-toggle="tooltip" title="Editar"' +
            cerrarModalVisualizar +
            'onclick="editarContrato(' +
            "'" + dataItem.Id + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ')"><i class="fa ' + icono + '"></i></button>';
    } else {
        return "<div</div>";
    }
}
function botonConfirmadoTilde(dataItem, icono, esModalVisualizar) {
    if (confirmaNegocios && !externo && puedeConfirmarNegocio(dataItem)) {
        var mensaje = ModificoPrecio(dataItem);
        var cerrarModalVisualizar = '';
        if (esModalVisualizar == true) {
            cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
        }
        return '<button data-toggle="tooltip" title="Confirmar" ' +
            cerrarModalVisualizar +
            'onclick = "ModalConfirmadoTilde(' +
            "'" + dataItem.Estado + "'" + ',' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.ContratoSAP + "'" + ',' +
            "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ',' +
            "'" + dataItem.AcuerdoId + "'" + ',' +
            "'" + dataItem.AgenteId + "'" + ',' +
            "'" + dataItem.FasonId + "'" + ',' +
            "'" + mensaje + "'" + ',' +
            "'" + dataItem.TipoPosicionCBOT + "'" + ',' +
            "'" + dataItem.ServicioModificado + "'" +
            ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
    } else {
        return "<div</div>";
    }

}
function botonConfirmadoTildeFinalizado(dataItem, icono) {
    if (confirmaNegocios && !externo && puedeConfirmarNegocio(dataItem)) {
        return '<button data-toggle="tooltip" title="Confirmar" onclick="ModalConfirmadoTildeFinalizado(' +
            "'" + dataItem.Estado + "'" + ',' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.ContratoSAP + "'" + ',' +
            "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ',' +
            "'" + dataItem.AcuerdoId + "'" + ',' +
            "'" + dataItem.AgenteId + "'" + ',' +
            "'" + dataItem.FasonId + "'" +
            ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
    } else {
        return "<div</div>";
    }

}

function puedeConfirmarNegocio(dataItem) {
    return (
        ((dataItem.ComercialZonaId == 42 && confirmaNegocioOrigNorte)
            || (dataItem.ComercialZonaId == 43 && confirmaNegocioOrigCentro)
            || (dataItem.ComercialZonaId == 44 && confirmaNegocioOrigSur)
            || (dataItem.ComercialZonaId == 45 && confirmaNegocioCorredoresBsAs)
            || (dataItem.ComercialZonaId == 46 && confirmaNegocioCorredoresRosario))
        && ((dataItem.MaterialId == 1 && confirmarNegociosMaiz)
            || (dataItem.MaterialId == 2 && confirmarNegociosTrigo)
            || (dataItem.MaterialId == 3 && confirmarNegociosSoja)
            || (dataItem.MaterialId == 4 && confirmarNegociosGirasol)
            || (dataItem.MaterialId == 5 && confirmarNegociosGirasolAO))
    );
}
function botonFinalizado(dataItem, icono, esModalVisualizar) {
    if (dataItem.TipoNegocioId == 6) {
        return '<div></div>';
    }
    if (finalizaNegocios && !externo) {
        var cerrarModalVisualizar = ' ';
        if (esModalVisualizar == true) {
            cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
        }
        return '<button data-toggle="tooltip" title="Finalizar"' +
            cerrarModalVisualizar +
            'onclick="ModalFinalizado(' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ',' +
            "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
            "'" + dataItem.FasonId + "'" + ',' +
            "'" + dataItem.AgenteId + "'" + ',' +
            "'" + dataItem.AcuerdoId + "'" +
            ')"><i class="fa ' + icono + ' conf"></i></button>';
    }
    else {
        return '<div></div>';
    }
}

function botonAprobar(dataItem, icono) {
    if (!externo) {
        return '<button data-toggle="tooltip" title="Aprobar" onclick="ModalAprobar(' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ',' +
            "'" + dataItem.FijacionDePrecioContratoId + "'" +
            ')"><i class="fa ' + icono + ' conf"></i></button>';
    }
    else {
        return '<div></div>';
    }
}
function botonVisualizar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Visualizar" onclick="ModalVisualizar(' +
        "'" + dataItem.Id + "'" + ',' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.Proveedor + "'" + ',' +
        "'" + dataItem.Corredor + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaDesde) + ' - ' + formatearFecha(dataItem.FechaHasta) + "'" + ',' +
        "'" + formatearFecha(dataItem.Fecha) + "'" + ',' +
        "'" + dataItem.TipoNegocio + "'" + ',' +
        "'" + dataItem.Material + "'" + ',' +
        "'" + dataItem.Cantidad + "'" + ',' +
        "'" + dataItem.Precio + "'" + ',' +
        "'" + dataItem.Comercial + "'" + ',' +
        "'" + dataItem.MonedaId + "'" + ',' +
        "'" + dataItem.Precio + ' (' + dataItem.MonedaId + ')' + "'" + ',' +
        "'" + dataItem.Campania + "'" + ',' +
        "'" + dataItem.Provincia + "'" + ',' +
        "'" + dataItem.Localidad + "'" + ',' +
        "'" + dataItem.ContratoSAP + "'" + ',' +
        "'" + dataItem.Negocio + "'" + ',' +
        "'" + dataItem.Sustentable + "'" + ',' +
        "'" + dataItem.Importe_Sustentable + "'" + ',' +
        "'" + dataItem.MonedaId_Sustentable + "'" + ',' +
        "'" + dataItem.TarifaAConvenir + "'" + ',' +
        "'" + formatearFecha(dataItem.Fecha_Dolarizado) + "'" + ',' +
        "'" + dataItem.Dias_Pesificado + "'" + ',' +
        "'" + dataItem.NoInformaSIO + "'" + ',' +
        "'" + dataItem.CalidadDescripcion + "'" + ',' +
        "'" + dataItem.Estado + "'" + ',' +
        "'" + htmlEncode(dataItem.Observacion) + "'" + ',' +
        "'" + dataItem.Moneda + "'" + ',' +
        "'" + dataItem.Moneda_Sustentable + "'" + ',' +
        "'" + dataItem.DestinoId + "'" + ',' +
        "'" + dataItem.DestinoDescripcion + "'" + ',' +
        "'" + dataItem.CantidadCamiones + "'" + ',' +
        "'" + dataItem.Consignatario + "'" + ',' +
        "'" + dataItem.PlanCanje + "'" + ',' +
        "'" + dataItem.CondicionFijacion + "'" + ',' +
        "'" + dataItem.CD + "'" + ',' +
        "'" + dataItem.Warrant + "'" + ',' +
        "'" + dataItem.PagoDirectoVendedor + "'" + ',' +
        "'" + dataItem.EstablecimientoPropio + "'" + ',' +
        "'" + dataItem.BoletoId + "'" + ',' +
        "'" + dataItem.BolsaId + "'" + ',' +
        "'" + dataItem.BoletoDescripcion + "'" + ',' +
        "'" + dataItem.BolsaDescripcion + "'" + ',' +
        "'" + formatearFecha(dataItem.DesdeFijacion) + ' - ' + formatearFecha(dataItem.HastaFijacion) + "'" + ',' +
        "'" + dataItem.CondicionFijacionDescripcion + "'" + ',' +
        "'" + dataItem.ClasificacionId + "'" + ',' +
        "'" + dataItem.ClasificacionDescripcion + "'" + ',' +
        "'" + dataItem.StandardDeCalidadDescripcion + "'" + ',' +
        "'" + dataItem.CalidadEspecialDescripcion + "'" + ',' +
        "'" + formatearFecha(dataItem.DesdeFijacion) + "'" + ',' +
        "'" + formatearFecha(dataItem.HastaFijacion) + "'" + ',' +
        "'" + dataItem.MercsDeposito + "'" + ',' +

        "'" + dataItem.ContratoCorredor + "'" + ',' +
        "'" + dataItem.ContratoVendedor + "'" + ',' +
        "'" + dataItem.SelCargoMOA + "'" + ',' +
        "'" + dataItem.SelCargoVendedor + "'" + ',' +

        "'" + dataItem.TipoFason + "'" + ',' +
        "'" + dataItem.Posicion + "'" + ',' +
        "'" + dataItem.Operador + "'" + ',' +
        "'" + dataItem.PrecioNeto + "'" + ',' +
        "'" + dataItem.Id + "'" + ',' +
        "'" + dataItem.Pizarra + "'" + ',' +

        "'" + dataItem.ZonaDescripcion + "'" + ',' +
        "'" + dataItem.NivelTarifa + "'" + ',' +
        "'" + dataItem.TarifaFlete + "'" + ',' +
        "'" + dataItem.Compensacion + "'" + ',' +
        "'" + $.trim((dataItem.Rechazo == null ? "" : dataItem.Rechazo.replace(/\n+/g, ' '))) + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaCierta) + "'" + ',' +
        "'" + dataItem.PorcentajeDePago + "'" + ',' +
        "'" + dataItem.TipoAgenteCompraId + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaOperacion) + "'" + ',' +
        "'" + dataItem.MotivoOperacionAnterior + "'" + ',' +
        "'" + dataItem.DescripcionOperacionAnterior + "'" + ',' +
        "'" + dataItem.PagoCBU + "'" + ',' +
        "'" + dataItem.ChequeElectronicoValor + "'" + ',' +
        "'" + dataItem.CalidadTercero + "'" + ',' +
        "'" + dataItem.DolarizadoTercero + "'" + ',' +
        "'" + dataItem.PagoDiferidoTercero + "'" + ',' +
        "'" + dataItem.Canje + "'" + ',' +
        "'" + dataItem.Monto + "'" + ',' +
        "'" + dataItem.MonedaCanjeId + "'" + ',' +
        "'" + dataItem.Insumo + "'" + ',' +
        "'" + dataItem.PrestamoDevolucion + "'" + ',' +
        "'" + dataItem.PlantaDestinoDescripcion + "'" + ',' +
        "'" + dataItem.ObservacionTercero + "'" + ',' +
        "'" + dataItem.SustentableTercero + "'" + ',' +
        "'" + dataItem.Venta + "'" + ',' +
        "'" + dataItem.FechaDesde_SustentableFormateado + "'" + ',' +
        "'" + dataItem.FechaHasta_SustentableFormateado + "'" + ',' +
        "'" + dataItem.ObligatoriedadCostoFinanciero + "'" + ',' +
        "'" + dataItem.PosicionCBOT + "'" + ',' +
        "'" + dataItem.TipoPosicionCBOT + "'" + ',' +
        "'" + dataItem.ProveedorCreador + "'" + ',' +
        "'" + dataItem.Cesion + "'" + ',' +
        "'" + htmlEncode(dataItem.MotivoReemplazo == null ? "" : dataItem.MotivoReemplazo) + "'" + ',' +
        "'" + dataItem.AnulaYReemplazaContratoSAP + "'" + ',' +
        "'" + dataItem.ObligatoriedadBonificacionDesc + "'" + ',' +

        "'" + dataItem.Condicional + "'" + ',' +
        "'" + dataItem.CondicionalCantidad + "'" + ',' +
        "'" + dataItem.CondicionalFechaFormateado + "'" + ',' +
        "'" + dataItem.CondicionalMonedaId + "'" + ',' +
        "'" + dataItem.CondicionalPosicion + "'" + ',' +
        "'" + dataItem.CondicionalPrecio + "'" + ',' +
        "'" + dataItem.CondicionalContratoSAP + "'" + ',' +
        "'" + dataItem.MailVentaBoleto + "'" + ',' +
        "'" + htmlEncode(dataItem.RazonSocialProveedorComisionista == null ? "" : dataItem.RazonSocialProveedorComisionista) + "'" + ',' +
        "'" + dataItem.KgMinimo + "'" + ',' +
        "'" + dataItem.KgMaximo + "'" + ',' +
        "'" + dataItem.ComercialZonaId + "'" + ',' +
        "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" + ',' +
        "'" + dataItem.AcuerdoId + "'" + ',' +
        "'" + dataItem.AgenteId + "'" + ',' +
        "'" + dataItem.FasonId + "'" + ',' +
        "'" + dataItem.Estado + "'" + ',' +
        "'" + dataItem.MaterialId + "'" + ',' +
        "'" + htmlEncode(dataItem.Virtual == null ? "" : dataItem.Virtual) + "'" + ',' +
        "'" + dataItem.CantidadDeposito + "'" + ',' +
        "'" + dataItem.FechaDolarizadoOriginalFormateado + "'" + ',' +
        "'" + dataItem.FechaHastaOriginalFormateado + "'" + ',' +
        "'" + dataItem.ServicioModificado + "'" +
        ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
}

function botonBorrar(dataItem, icono, esModalVisualizar) {
    if (eliminaNegocios && !externo) {
        var cerrarModalVisualizar = ' ';
        if (esModalVisualizar == true) {
            cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
        }
        return '<button data-toggle="tooltip" title="Rechazar"' +
            cerrarModalVisualizar +
            'onclick="ModalBorrar(' +
            "'" + dataItem.Proveedor + "'" + ',' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ',' +
            "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
            "'" + dataItem.FasonId + "'" + ',' +
            "'" + dataItem.AgenteId + "'" + ',' +
            "'" + dataItem.AcuerdoId + "'" + ',' +
            "'" + dataItem.Estado + "'" + ',' +
            "'" + dataItem.ServicioModificado + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return '<div></div>';
    }
}

function botonBorrarPreanulado(dataItem, icono, esModalVisualizar) {
    if (anular) {
        var KilosPendientes = 0;
        if (dataItem.FijacionDePrecioContratoId && dataItem.Virtual == true) {
            var kilospendientesFijacion = MSExecuteOnServer('/CompraNet/DevolverKilosPendientesAnularFijacionCanje', { id: dataItem.FijacionDePrecioContratoId });
            KilosPendientes = kilospendientesFijacion.KilosPendientes;
        }
        var cerrarModalVisualizar = ' ';
        if (esModalVisualizar == true) {
            cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
        }
        return '<button data-toggle="tooltip" title="Rechazar"' +
            cerrarModalVisualizar +
            'onclick="ModalBorrarPreAnulado(' +
            "'" + dataItem.Proveedor + "'" + ',' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.TipoNegocioId + "'" + ',' +
            "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
            "'" + dataItem.FasonId + "'" + ',' +
            "'" + dataItem.AgenteId + "'" + ',' +
            "'" + dataItem.AcuerdoId + "'" + ',' +
            "'" + dataItem.Estado + "'" + ',' +
            "'" + $.trim((dataItem.Rechazo == null ? "" : dataItem.Rechazo.replace(/\n+/g, ' '))) + "'" + ',' +
            "'" + KilosPendientes + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return '<div></div>';
    }
}

function botonPreAnular(dataItem, icono, esModalVisualizar) {
    if (preanular && dataItem.ContratoId || (preanular && (dataItem.FijacionDePrecioContratoId && dataItem.Virtual == true))) {
        var cerrarModalVisualizar = ' ';
        if (esModalVisualizar == true) {
            cerrarModalVisualizar = ' data-dismiss="modal" style="border: 1px solid #848484; border-radius: 5px !important; margin-right: 4px" '
        }
        if (dataItem.FijacionDePrecioContratoId && dataItem.Virtual == true) {
            var kilospendientesFijacion = MSExecuteOnServer('/CompraNet/DevolverKilosPendientesAnularFijacionCanje', { id: dataItem.FijacionDePrecioContratoId });
            if (kilospendientesFijacion.KilosPendientes > 0) {
                return '<button data-toggle="tooltip" title="PreAnular"' +
                    cerrarModalVisualizar +
                    'onclick="ModalPreAnular(' +
                    "'" + dataItem.ContratoId + "'" + ',' +
                    "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
                    "'" + dataItem.Proveedor + "'" + ',' +
                    "'" + dataItem.TipoNegocioId + "'" + ',' +
                    "'" + kilospendientesFijacion.KilosPendientes + "'" +
                    ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
            }
            else {
                return '<div></div>';
            }
        } else {
            return '<button data-toggle="tooltip" title="PreAnular"' +
                cerrarModalVisualizar +
                'onclick="ModalPreAnular(' +
                "'" + dataItem.ContratoId + "'" + ',' +
                "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
                "'" + dataItem.Proveedor + "'" + ',' +
                "'" + dataItem.TipoNegocioId + "'" +
                ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
        }


    } else {
        return '<div></div>';
    }
}



function botonAsociarContratos(dataItem, icono) {
    if (asociarNegocios && dataItem.ContratoId && dataItem.TipoPosicionCBOT == 'PASE' && dataItem.TipoNegocioId == 1) {
        return '<button data-toggle="tooltip" title="Asociar Negocios" onclick="ModalAsociarNegocios(' +
            "'" + dataItem.ContratoId + "'" + ',' +
            "'" + dataItem.Estado + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return '<div></div>';
    }

}




function AutoRecargar() {
    setInterval(function () {
        if (document.getElementById('checkRecarga').checked == true) {
            filasSeleccionadas = SeleccionarElementos();
            recargarGrilla();
            console.log("hola");
        }
    }, Number(actualizacionMs));

}

function recargarGrilla() {
    $('#gridInformeCompraNet').data('kendoGrid').dataSource.read();
    if (viewModel.ContratosPendientes.length > 0) {
        AvisoContratosPendientes();
    }

}
function Filtrar() {
    var grid = $('#gridInformeCompraNet').data('kendoGrid');
    var currentFilters = grid.dataSource.filter();
    let filtroSap = TraerFiltrosConValores();
    currentFilters = { filters: [], logic: 'and' };
    grid.dataSource.filter(currentFilters);
    if (filtroSap.filter != null) {
        //-----------------------------------------
        currentFilters.filters = currentFilters.filters.filter(function (x) {
            return x.field != 'ContratoSAP' && x.field != undefined
        });
        //currentFilters.filters = currentFilters.filters.filter(function (x) {
        //    return x.field != 'ContratoCorredor' && x.field != undefined
        //});

        var contratoSapFilters = { logic: 'or', filters: [] };
        //var contratoCorredorFilters = { logic: 'or', filters: [] };

        for (var i = 0; i < filtroSap.filter.filters[0].filters.length; i++) {
            contratoSapFilters.filters.push({ field: 'ContratoSAP', operator: 'contains', value: filtroSap.filter.filters[0].filters[i].value.padStart(10, '0') });
            contratoSapFilters.filters.push({ field: 'Negocio', operator: 'contains', value: filtroSap.filter.filters[0].filters[i].value.padStart(10, '0') });

            contratoSapFilters.filters.push({ field: 'ContratoCorredor', operator: 'eq', value: filtroSap.filter.filters[0].filters[i].value });
            //contratoCorredorFilters.filters.push({ field: 'ContratoCorredor', operator: 'eq', value: filtroSap.filter.filters[0].filters[i].value });
        }
        currentFilters.filters.push(contratoSapFilters);
        //currentFilters.filters.push(contratoCorredorFilters);    
        grid.dataSource.filter(currentFilters);
    }
    filtrarZona();
    recargarGrilla();

}
function BorrarFiltro() {
    $("#ContratoSAPId").val("");
    $("#ContratoSAPHastaId").val("");
    $("#buscadorFiltroZona").data("kendoMultiSelect").value("");
    var grid = $('#gridInformeCompraNet').data('kendoGrid');
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }
    //Remove filter 
    var removeIndex = -1;
    if (filters != null) {
        for (var x = 0; x < filters.length; x++) {
            var temp = filters[x];
            if (temp.filters != undefined) {

                for (var i = 0; i < temp.filters.length; i++) {
                    if (temp.filters[i].field == 'ContratoSAP' || temp.filters[i].field == 'ContratoCorredor' || temp.filters[i].field == 'GrupoCompraDescripcion') {
                        removeIndex = x;
                        break;
                    }
                }
                break;
            }
        }
        if (removeIndex != -1)
            filters.splice(removeIndex, 1);

    }
    dataSource.filter(filters);
}

function filtrarZona() {
    ////FILTRO MANUAL
    var grid = $('#gridInformeCompraNet').data('kendoGrid');
    var currentFilters = grid.dataSource.filter();
    var value = $("#buscadorFiltroZona").data("kendoMultiSelect").value();

    if (!currentFilters) {
        currentFilters = { filters: [], logic: 'and' }
    }

    currentFilters.filters = currentFilters.filters.filter(function (x) {
        return x.field != 'GrupoCompraDescripcion' && (x.filters == undefined || x.filters[0].field != 'GrupoCompraDescripcion')
        /*&& x.field != undefined*/
    });

    if (!value || value.length < 1) {
        grid.dataSource.filter(currentFilters);
        return;
    }

    var zonaFilters = { logic: 'or', filters: [] }

    $("#buscadorFiltroZona").data("kendoMultiSelect").value().forEach(function (x) {
        zonaFilters.filters.push({ field: 'GrupoCompraDescripcion', operator: 'eq', value: x })
    })

    currentFilters.filters.push(zonaFilters)

    grid.dataSource.filter(currentFilters);
}

function filtrarMesa() {
    //FILTRO MANUAL
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    if (!$("#negociosPropiosDiv").hasClass("selected")) {
        addOrRemoveFilter(grilla, "ComercialCreadorId", "eq", parseInt(comercialId));
        $("#negociosPropiosDiv").addClass("selected");
        $("#negociosPropios").addClass("selected").removeClass("varios");
    } else {
        addOrRemoveFilter(grilla, "ComercialId", "eq", "");
        $("#negociosPropiosDiv").removeClass("selected");
        $("#negociosPropios").addClass("varios").removeClass("selected");
    }
    recargarGrilla();
}

function addOrRemoveFilter(grid, field, operator, value) {

    var newFilter = { field: field, operator: operator, value: value };
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }

    if (value && (value.length > 0 || value != undefined)) {
        //Add filter
        if (filters == null) {
            filters = [newFilter];
        }
        else {
            var isNew = true;
            var index = 0;
            for (index = 0; index < filters.length; index++) {
                if (filters[index].field == field) {
                    isNew = false;
                    break;
                }
            }
            if (isNew) {
                filters.push(newFilter);
            }
            else {
                filters[index] = newFilter;
            }
        }
    }
    else {
        //Remove filter 
        var removeIndex = -1;
        if (filters != null) {
            for (var x = 0; x < filters.length; x++) {
                var temp = filters[x];
                if (temp.field == field) {
                    removeIndex = x;
                    break;
                }
            }
            if (removeIndex != -1)
                filters.splice(removeIndex, 1);
        }
    }
    dataSource.filter(filters);
}

function CreateGridInformeCompraNet() {
    var grupoDeComprasDatos = MSExecuteOnServer('/CompraNet/BuscarGrupoDeCompras');

    var GrupoCompraDescripcionDatos = new Array();
    for (var i = 0; i < grupoDeComprasDatos.length; i++) {
        if (grupoDeComprasDatos[i].Id != 47) {
            GrupoCompraDescripcionDatos.push({ GrupoCompraDescripcion: grupoDeComprasDatos[i].Descripcion });
        }
    }

    var defaultFilter = { field: "Fecha", operator: "eq", value: new Date };

    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });

    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/CompraNet/BuscaDatosTabla'
            },
            parameterMap: function (options, operation) {
                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Fecha: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    FechaEntrega: { type: "date" },
                    Fecha_Dolarizado: { type: "date" },
                    Cantidad: { type: "number" },
                    Precio: { type: "number" },
                    PrecioPlazo: { type: "string" },
                    Ampliaciones: { type: "number" },
                    DesdeFijacion: { type: "date" },
                    HastaFijacion: { type: "date" },
                    FechaCierta: { type: "date" },
                    FechaOperacion: { type: "date" }
                }
            }
        },
        //aggregate: [
        //    { field: "TotalPesos", aggregate: "sum" },
        //    { field: "TotalDolares", aggregate: "sum" }
        //],
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Fecha", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20,
        filter: defaultFilter
    };
    var classExterno = externo ? "hide" : "";

    $("#pageSize").kendoDropDownList();

    $("#gridInformeCompraNet").kendoGrid({
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").attr('id', 'border-orange');
            $("td:has(div.statusconfirmado)").attr('id', 'border-green');
            $("td:has(div.statusoferta)").attr('id', 'border-blue');
            $("td:has(div.statuserror)").attr('id', 'border-red');
            $("td:has(div.statusfinalizado)").attr('id', 'border-black');
            $("td:has(div.statusborrado)").attr('id', 'border-grey');
            $("td:has(div.statuspreanulado)").attr('id', 'border-grey');
            $("td:has(div.statusreconfirmar)").attr('id', 'border-purple');
            $("td:has(div.statuseliminado)").attr('id', 'border-grey');
            $("td:has(div.statuspreaprobacion)").attr('id', 'border-turquoise');
            $("td:has(div.statusreconfirmarfinalizado)").attr('id', 'border-purple');

            if (!verMesa) {
                $("#gridInformeCompraNet").data("kendoGrid").hideColumn("Comercial");
                $("#gridInformeCompraNet").data("kendoGrid").hideColumn("GrupoCompraDescripcion");
            }
            var grid = $("#gridInformeCompraNet").data("kendoGrid");
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                for (var j = 0; j < filasSeleccionadas.length; j++) {
                    if (filasSeleccionadas[j].ContratoId == view[i].ContratoId && filasSeleccionadas[j].FijacionDePrecioContratoId == view[i].FijacionDePrecioContratoId) {
                        grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                            .addClass("k-state-selected")
                            .find(".k-checkbox")
                            .prop('checked', true);
                    }
                }
                if (view[i].DestinoDescripcion != "S. Lorenzo" && view[i].DestinoDescripcion != "") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("otroDestino");
                } else if ((view[i].ImporteFinanciero > 1 || view[i].ImporteRedespacho < -1 || view[i].ImporteComision > 1 || view[i].ImporteBonificacion > 1)
                    || (view[i].MaterialId == 1 && view[i].StandardCalidadId == 1)
                    || (view[i].MaterialId == 2 && (view[i].StandardCalidadId == 1 || view[i].StandardCalidadId == 2))
                    || (view[i].MaterialId == 3 && (view[i].StandardCalidadId == 2 || view[i].Sustentable == true))
                    || ((view[i].MaterialId == 4 || view[i].MaterialId == 5) && view[i].StandardCalidadId == 6)) {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("calidadEspecialOSustentable");
                }
            }
            filasSeleccionadas = {};
            BuscarTotales();
            $('[data-toggle="tooltip"]').tooltip();
            ajustarTamanio();
        },

        columns: [
            { selectable: true, width: "35px" },
            {
                field: "Proveedor", type: "string", minResizableWidth: 100,
                headerAttributes: { "class": classExterno }, attributes: { "id": "line", "class": classExterno, style: "font-size: 10px" },
                template: function (dataItem) {
                    if (dataItem.Estado == 1) {
                        return '<div class="statuspendiente "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 2) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 3) {
                        return '<div class="statusoferta "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 4) {
                        return '<div class="statuserror "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 5) {
                        return '<div class="statusfinalizado "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 6) {
                        return '<div class="statusborrado "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 7) {
                        return '<div class="statusreconfirmar "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 8) {
                        return '<div class="statuseliminado "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 9) {
                        return '<div class="statuspreaprobacion "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 10) {
                        return '<div class="statuspreanulado "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    } else if (dataItem.Estado == 11) {
                        return '<div class="statusreconfirmarfinalizado "></div>' + dataItem.Proveedor + " " + dataItem.TipoAgenteCompra;
                    }
                },
                filterable: { ui: createMultiSelectProveedor }
            },
            {
                field: "Corredor", type: "string", minResizableWidth: 100, filterable: { ui: createMultiSelectCorredor },
                headerAttributes: {
                    "class": classExterno
                },
                attributes: {
                    "class": classExterno,
                    style: "font-size: 10px"
                }
            },
            {
                field: "FechaDesde", type: "date", width: 75, minResizableWidth: 75, title: "Desde"/*, format: _DefaultDateTemplate*/, attributes: {
                    "class": "mobile-sm"
                }, template: function (dataItem) {
                    var p = ArmarFechaDesde(dataItem);
                    return p;
                }
            },
            {
                field: "FechaHasta", type: "date", width: 75, minResizableWidth: 75, title: "Hasta"/*, format: _DefaultDateTemplate*/, attributes: {
                    "class": "mobile-sm"
                }, template: function (dataItem) {
                    var p = ArmarFechaHasta(dataItem);
                    return p;
                }
            },
            {
                field: "TipoNegocio", type: "string", filterable: {
                    multi: true, dataSource: [{
                        TipoNegocio: "A FIJAR"
                    }, {
                        TipoNegocio: "A FIJAR PASE"
                    }, {
                        TipoNegocio: "A PRECIO"
                    }, {
                        TipoNegocio: "FIJACION"
                    }, {
                        TipoNegocio: "CONVENIO"
                    }, {
                        TipoNegocio: "FIJ. CONVENIO"
                    }, {
                        TipoNegocio: "FASON"
                    }, {
                        TipoNegocio: "AGENTE DE COMPRAS"
                    }, {
                        TipoNegocio: "CONTRATO ACUERDO"
                    }, {
                        TipoNegocio: "FASON MP"
                    }, {
                        TipoNegocio: "AGENTE DE COMPRAS MP"
                    }, {
                        TipoNegocio: "ACUERDO AGENTE"
                    }, {
                        TipoNegocio: "CANJE"
                    }, {
                        TipoNegocio: "PR\u00C9STAMO DEVOLUCI\u00D3N"
                    }, { TipoNegocio: "VENTA" },
                    { TipoNegocio: "FIJACION VIRTUAL" },
                    { TipoNegocio: "FIJACION CANJE" },
                    { TipoNegocio: "FIJACION PASE" },
                    ]
                }, title: "Tipo", width: 80, minResizableWidth: 80, attributes: {
                    "class": "mobile-sm"
                }, template: "#if(AnulaYReemplazaContratoId != null){# <i class='fa fa-recycle fa-2x'></i> &nbsp;#}##=TipoNegocio#"
            },
            {
                field: "Material", title: "Mat.", type: "string", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz"
                    }, {
                        Material: "Trigo"
                    }, {
                        Material: "Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Girasol AO"
                    }]
                }, width: 60, minResizableWidth: 60, attributes: {
                    "class": "mobile-xs"
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Material|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Material#'/></label></span>";
                }, template: "#=Material#"
            },
            {
                field: "Cantidad", type: "number", width: 80, minResizableWidth: 80, format: "{0:n0}", attributes: {
                    "class": "mobile-xs"
                }
            },
            {
                field: "Ampliaciones", type: "number", width: 60, minResizableWidth: 60, title: "Ampl.", attributes: {
                    "class": "mobile-sm " + classExterno
                }, headerAttributes: {
                    "class": classExterno
                }, template: function (dataItem) {
                    if (ampliaNegocios && dataItem.Estado == 2 && !externo && (dataItem.TipoNegocioId == 1 || dataItem.TipoNegocioId == 2)) {
                        return '' + dataItem.Ampliaciones + '<button data-toggle="tooltip" title="Ampliar"onclick="ModalAmpliaciones(' +
                            "'" + dataItem.ContratoId + "'" + ',' + "'" + dataItem.Ampliacion + "'" + ',' + "'" + dataItem.TipoNegocioId + "'" + "," + "'" + dataItem.FijacionDePrecioContratoId + "'" + "," + "'" + dataItem.FasonId + "'" + "," + "'" + dataItem.AgenteId + "'" + ')"><i class="fa fa-plus aria-hidden="true"></i></button>';
                    } else {
                        return kendo.toString(dataItem.Ampliaciones, "n0");
                    }
                }
            },
            {
                field: "PrecioPlazo", type: "string", title: "Precio/Plazo", width: 85, minResizableWidth: 58, filterable: false, sortable: false, template: function (dataItem) {
                    var p = ArmarPrecio(dataItem);
                    return p;
                }
            },
            { field: "Campania", type: "string", title: "Cos", width: 60, minResizableWidth: 60, attributes: { "class": "mobile-md" } /*title: "Campa&ntilde;a"*/ },
            {
                field: "Negocio", type: "number", title: "Negocio", width: 80, minResizableWidth: 70, attributes: { "class": "mobile-md" },
                template: function (dataItem) {
                    if (dataItem.Negocio !== "" && dataItem.Negocio !== null) {
                        return kendo.parseInt(dataItem.Negocio);
                    } else {
                        return "";
                    }
                },
                filterable: {
                    operators: {
                        string: {
                            eq: "Es Igual",
                            contains: "Contine"
                        }
                    }
                }
            },
            {
                field: "Fecha", type: "date", title: "Carga", width: 75, minResizableWidth: 75, format: _DefaultDateTemplate, attributes: { "class": "mobile-xs" },
                template: function (dataItem) {
                    if (dataItem.FechaOperacion != null && kendo.toString(dataItem.FechaOperacion, "dd/MM/yyyy") != kendo.toString(dataItem.Fecha, "dd/MM/yyyy")) {
                        return "<b style='color:darkblue;'>" + kendo.toString(dataItem.FechaOperacion, "dd/MM/yyyy") + "</b>";
                    } else {
                        return "" + kendo.toString(dataItem.Fecha, "dd/MM/yyyy") + "<br>" + dataItem.Hora;
                    }
                }
            },
            {
                field: "GrupoCompraDescripcion", type: "string", width: 90, minResizableWidth: 90, filterable: {
                    multi: true, dataSource: GrupoCompraDescripcionDatos
                }
                , title: "Zona",
                headerAttributes: {
                    "class": classExterno
                },
                attributes: { "class": "mobile-xs mobile-md " + classExterno }
            },
            {
                field: "Comercial", type: "string", title: "Comercial", width: 85, minResizableWidth: 85, filterable: { ui: createMultiSelectComercial }, headerAttributes: {
                    "class": classExterno
                },
                attributes: { "class": "mobile-xs " + classExterno }
            },
            {
                field: "ComercialCreador", type: "string", title: "Creador", width: 85, minResizableWidth: 85, filterable: { ui: createMultiSelectComercialCreador }, headerAttributes: {
                    "class": classExterno
                },
                attributes: { "class": "mobile-xs " + classExterno }
            },
            {
                field: "DestinoDescripcion", type: "string", title: "Destino", width: 80, minResizableWidth: 80, attributes: { "class": "mobile-xs mobile-md" }
            },
            {
                field: "Estado_Contrato", sortable: false, title: "Estado", width: 87, minResizableWidth: 87, filterable: {
                    multi: true,
                    dataSource: [{
                        Estado_Contrato: "Pendiente"
                    }, {
                        Estado_Contrato: "Confirmado"
                    }, {
                        Estado_Contrato: "Con Error"
                    }, {
                        Estado_Contrato: "Oferta"
                    }, {
                        Estado_Contrato: "Finalizado"
                    }, {
                        Estado_Contrato: "Rechazado"
                    }, {
                        Estado_Contrato: "PreAnulado"
                    }, {
                        Estado_Contrato: "Reconfirmar"
                    }, {
                        Estado_Contrato: "ReconfirmarFinalizado"
                    }, {
                        Estado_Contrato: "Eliminado"
                    }, {
                        Estado_Contrato: "Carga"
                    }]
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Estado_Contrato|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Estado_Contrato#'/></label></span>";
                }, template: function (dataItem) {
                    if (dataItem.Estado == 1) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonPendiente(dataItem, 'fa-pencil pend') +
                            botonConfirmadoTilde(dataItem, 'fa-check pend') +
                            botonVisualizar(dataItem, 'fa-eye pend') +
                            botonBorrar(dataItem, 'fa-trash pend') +
                            botonAsociarContratos(dataItem, 'fa fa-inbox pend');
                    }
                    if (dataItem.Estado == 2) { //confirmado
                        if (dataItem.TipoNegocioId == 6 || dataItem.TipoNegocioId == 5) {
                            return '<div class="status confirmado">Confirmado</div>' +
                                botonNoMostrarEnTablero(dataItem, 'conf') +
                                botonPendiente(dataItem, 'fa-pencil conf') +
                                botonVisualizar(dataItem, 'fa-eye conf') +
                                botonBorrar(dataItem, 'fa-trash conf');
                        } else {
                            var descripcion = externo ? ' data-toggle="tooltip" title="Fijaci&oacute;n aprobada por MOA" ' : '';
                            return '<div' + descripcion + ' class="status confirmado">Confirmado</div>' +
                                botonNoMostrarEnTablero(dataItem, 'conf') +
                                botonPendiente(dataItem, 'fa-pencil conf') +
                                botonFinalizado(dataItem, 'fa-flag-checkered conf') +
                                botonVisualizar(dataItem, 'fa-eye conf') +
                                botonBorrar(dataItem, 'fa-trash conf') +
                                botonAsociarContratos(dataItem, 'fa fa-inbox conf');
                        }
                    }
                    if (dataItem.Estado == 3) { //Oferta
                        return '<div class="status oferta">Oferta</div>' +
                            botonNoMostrarEnTablero(dataItem, 'ofe') +
                            botonPendiente(dataItem, 'fa-pencil ofe') +
                            botonConfirmadoTilde(dataItem, 'fa-check ofe') +
                            botonVisualizar(dataItem, 'fa-eye ofe') +
                            botonBorrar(dataItem, 'fa-trash ofe');
                    }

                    if (dataItem.Estado == 4) { //error
                        if (dataItem.TipoNegocioId == 6) {
                            return '<div class="status error">Con Error</div>' +
                                botonPendiente(dataItem, 'fa-pencil err') +
                                botonVisualizar(dataItem, 'fa-eye err') +
                                botonBorrar(dataItem, 'fa-trash err');
                        } else {
                            return '<div class="status error">Con Error</div>' +
                                botonPendiente(dataItem, 'fa-pencil err') +
                                botonFinalizado(dataItem, 'fa-flag-checkered err') +
                                botonVisualizar(dataItem, 'fa-eye err') +
                                botonBorrar(dataItem, 'fa-trash err');
                        }
                    }

                    if (dataItem.Estado == 5) { //Finalizado
                        if (verMesa && (dataItem.ContratoId || dataItem.FasonId || dataItem.AgenteId)) {
                            return '<div class="status finalizado">Finalizado</div>' +
                                botonNoMostrarEnTablero(dataItem, 'fin') +
                                botonVisualizar(dataItem, 'fa-eye fin') +
                                ((dataItem.Virtual != true && dataItem.TipoNegocioId == 3) ? "" : botonPreAnular(dataItem, 'fa-trash fin')) +
                                ((dataItem.Virtual == true) ? "" : botonModificarFinalizados(dataItem, 'fa-pencil fin')) +
                                botonAsociarContratos(dataItem, 'fa fa-inbox fin');

                        } else {
                            descripcion = externo ? ' data-toggle="tooltip" title="Fijaci&oacute;n cerrada" ' : '';

                            return '<div ' + descripcion + 'class="status finalizado">Finalizado</div>' +
                                botonNoMostrarEnTablero(dataItem, 'fin') +
                                botonVisualizar(dataItem, 'fa-eye fin') +
                                ((dataItem.Virtual != true && dataItem.TipoNegocioId == 3) ? "" : botonPreAnular(dataItem, 'fa-trash fin')) +
                                ((dataItem.Virtual == true) ? "" : botonModificarFinalizados(dataItem, 'fa-pencil fin'));
                        }
                    }
                    if (dataItem.Estado == 6) { //Rechazado
                        descripcion = externo ? ' data-toggle="tooltip" title="Fijaci&oacute;n rechazada por MOA" ' : '';

                        return '<div ' + descripcion + ' class="status borrado" ' + (dataItem.Rechazo == "" || dataItem.Rechazo == null ? "" : 'data-toggle="tooltip" data-placement="top" title="' + dataItem.Rechazo + '"') + '>Rechazado</div>' +
                            botonVisualizar(dataItem, 'fa-eye bor');
                    }
                    if (dataItem.Estado == 7) { //reconfirmar
                        return '<div class="status reconfirmar">Reconfirmar</div>' +
                            botonPendiente(dataItem, 'fa-pencil reconf') +
                            botonConfirmadoTilde(dataItem, 'fa-check reconf') +
                            botonVisualizar(dataItem, 'fa-eye reconf') +
                            botonBorrar(dataItem, 'fa-trash reconf') +
                            botonAsociarContratos(dataItem, 'fa fa-inbox reconf');
                    }
                    if (dataItem.Estado == 8) { //Anulado
                        return '<div class="status anulado"' + (dataItem.Rechazo == "" || dataItem.Rechazo == null ? "" : ' data-toggle="tooltip" data-placement="top" title="' + dataItem.Rechazo + '"') + '>Anulado</div>' +
                            botonVisualizar(dataItem, 'fa-eye anu') +
                            botonAsociarContratos(dataItem, 'fa fa-inbox anu');
                    }
                    if (dataItem.Estado == 9) { //preaprobacion
                        descripcion = externo ? ' data-toggle="tooltip" title="Fijaci&oacute;n pendiente aprobaci&oacute;n MOA" ' : '';
                        return '<div' + descripcion + ' class="status preaprobacion">Carga</div>' +
                            botonPendiente(dataItem, 'fa-pencil pre') +
                            botonAprobar(dataItem, ' fa-check-square-o pre') +
                            botonVisualizar(dataItem, 'fa-eye pre') +
                            botonBorrar(dataItem, 'fa-trash pre');
                    }
                    if (dataItem.Estado == 10) { //PreAnulado                        
                        return '<div class="status borrado">PreAnulado</div>' +
                            botonVisualizar(dataItem, 'fa-eye bor') +
                            botonBorrarPreanulado(dataItem, 'fa-trash bor');
                    }
                    if (dataItem.Estado == 11) { //ReconfirmarFinalizado                        
                        return '<div class="status reconfirmar">Reconfirmar Finalizado</div>' +
                            botonPendiente(dataItem, 'fa-pencil reconf') +
                            botonConfirmadoTildeFinalizado(dataItem, 'fa-check reconf') +
                            botonVisualizar(dataItem, 'fa-eye reconf') +
                            botonBorrar(dataItem, 'fa-trash reconf') +
                            botonAsociarContratos(dataItem, 'fa fa-inbox reconf');
                    }
                }
            },
            {
                field: "ContratoSAP", type: "string", title: "Contrato Sap", filterable: {
                    multi: true, dataSource: new Array()
                }
            },
            {
                field: "ContratoCorredor", type: "string", title: "Contrato Corredor", filterable: {
                    multi: true, dataSource: new Array()
                }
            },
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true,
            pageSize: 20,
            //pageSizes: [5, 10, 20, 40]
        },
        scrollable: true,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        height: 550,
        filterable: {
            checkAll: false,
            height: 350,
            extra: false,
            messages: {
                info: "Filtros:",
                filter: "Filtrar",
                clear: "Limpiar",
                isTrue: "SI",
                isFalse: "NO",
                and: "Y",
                or: "O"
            },
            operators: {
                string: {
                    eq: "Igual",
                    neq: "Distinto",
                    startswith: "Comienza con",
                    contains: "Contiene",
                    endswith: "Finaliza con"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a",
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a",
                }
            }
        },
        filterMenuInit: function (e) {
            if (e.field == "Proveedor" || e.field == "Comercial" || e.field == "Corredor" || e.field == "ComercialCreador") {
                $(e.container).css("width", "300px");
            }
        },
        resizable: true

    });
    $('#gridInformeCompraNet').data('kendoGrid').hideColumn("ContratoSAP");
    $('#gridInformeCompraNet').data('kendoGrid').hideColumn("ContratoCorredor");


    var minTableWidth;
    var minColumnWidth = 300;
    var th;
    var idx;
    var grid;


    $("#gridInformeCompraNet").data("kendoGrid").resizable.bind("start", function (e) {
        th = $(e.currentTarget).data("th");
        idx = th.index();
        grid = th.closest(".k-grid").data("kendoGrid");

        field = th.data("field");

        var column = $.grep(grid.columns, function (item) {
            return item.field === field;
        })[0];

        minColumnWidth = parseInt(column.minResizableWidth, 10);
    });

    $("#gridInformeCompraNet").data("kendoGrid").resizable.bind("resize", function (e) {
        if (th.width() >= minColumnWidth) {
            minTableWidth = grid.tbody.closest("table").width();
        }

        if (th.width() < minColumnWidth) {
            // the next line is ONLY needed if Grid scrolling is enabled
            grid.thead.closest("table").width(minTableWidth).children("colgroup").find("col").eq(idx).width(minColumnWidth);

            grid.tbody.closest("table").width(minTableWidth).children("colgroup").find("col").eq(idx).width(minColumnWidth);
        }
    });

    if ($("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(1).width() < 100) {
        $("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(1).width(100);
        $("#gridInformeCompraNet .k-grid-content").find("colgroup col").eq(1).width(100);
        $("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(2).width(100);
        $("#gridInformeCompraNet .k-grid-content").find("colgroup col").eq(2).width(100);
    }

    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };
    function createMultiSelect(element, textField, valueField, url, columna) {
        element.removeAttr("data-bind");
        columna = columna == null ? valueField : columna;
        element.kendoMultiSelect({
            itemTemplate: "<input type='checkbox'/> #:data." + textField + "#",
            dataBound: function () {
                var items = this.ul.find("li");
                setTimeout(function () {
                    checkInputs(items);
                });
            },

            dataTextField: textField,
            dataValueField: valueField,
            autoClose: false,
            autoBind: false,
            delay: 300,
            dataSource: {
                serverFiltering: true,
                filter: [],
                transport: {
                    read: {
                        url: url,
                        data: function () {
                            return {
                                text: element.data("kendoMultiSelect").input.val()
                            };
                        },
                        prefix: ""
                    }
                },
            },
            change: function (e) {
                var items = this.ul.find("li");
                checkInputs(items);
                var grilla = $('#gridInformeCompraNet').data("kendoGrid");
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        addOrRemoveFilter(grilla, columna, "eq", v);
                    }
                });

                if (values.length === 0) {
                    addOrRemoveFilter(grilla, columna, "eq", "");
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }
    function ajustarTamanio() {
        if ($("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(1).width() < 100) {
            $("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(1).width(100);
            $("#gridInformeCompraNet .k-grid-content").find("colgroup col").eq(1).width(100);
        }
        if ($("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(2).width() < 100) {
            $("#gridInformeCompraNet .k-grid-header-wrap").find("colgroup col").eq(2).width(100);
            $("#gridInformeCompraNet .k-grid-content").find("colgroup col").eq(2).width(100);
        }
    }
    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "Proveedor", "/CompraNet/ListarProveedor");
    }
    function createMultiSelectCorredor(element) {
        return createMultiSelect(element, "Proveedor", "ProveedorId", "/CompraNet/ListarCorredor", "CorredorId");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial");
    }
    function createMultiSelectComercialCreador(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial", "ComercialCreadorId");
    }

    AvisoContratosPendientes();
}
function BuscarTotales() {
    var grid = $("#gridInformeCompraNet").data("kendoGrid");
    var filter = grid.dataSource.filter();

    var totales = MSExecuteOnServer('/CompraNet/BuscarTotales', filter);

    if (totales != null) {
        $("#totalPesos").text(kendo.toString(totales.TotalPesos, "n0"));
        $("#totalDolares").text(kendo.toString(totales.TotalDolares, "n0"));

        //Cargar Toneladas de Materiales
        var total = totales.TotalSoja + totales.TotalMaiz + totales.TotalTrigo + totales.TotalGirasol + totales.TotalGirasolAlto;
        //Soja
        if (totales.TotalSoja != 0) {
            $("#totalSoja").show();
            $("#toneladasSoja").text(kendo.toString(totales.TotalSoja, "n0"));
        } else {
            $("#totalSoja").hide();
        }
        //Maiz
        if (totales.TotalMaiz != 0) {
            $("#totalMaiz").show();
            $("#toneladasMaiz").text(kendo.toString(totales.TotalMaiz, "n0"));
        } else {
            $("#totalMaiz").hide();
        }
        //Trigo
        if (totales.TotalTrigo != 0) {
            $("#totalTrigo").show();
            $("#toneladastrigo").text(kendo.toString(totales.TotalTrigo, "n0"));
        } else {
            $("#totalTrigo").hide();
        }
        //Girasol
        if (totales.TotalGirasol != 0) {
            $("#totalGirasol").show();
            $("#toneladasGirasol").text(kendo.toString(totales.TotalGirasol, "n0"));
        } else {
            $("#totalGirasol").hide();
        }
        //Girasol Alto
        if (totales.TotalGirasolAlto != 0) {
            $("#totalGirasolAlto").show();
            $("#toneladasGirasolAlto").text(kendo.toString(totales.TotalGirasolAlto, "n0"));
        } else {
            $("#totalGirasolAlto").hide();
        }
        //Total
        if (total != 0) {
            $("#totalizador-materiales").addClass("grupo-totales");
            $("#totalMaterial").show();
            $("#toneladasMaterial").text(kendo.toString(total, "n0"));
        } else {
            $("#totalizador-materiales").removeClass("grupo-totales");
            $("#totalMaterial").hide();
        }

    }
}
function isValidDate(date) {
    return date && Object.prototype.toString.call(date) === "[object Date]" && !isNaN(date);
}

function SeleccionarElementos() {
    var grid = $("#gridInformeCompraNet").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        obj.push(selectedItem);
    });
    return obj;
}

function editarContrato(id, tipoId, siguientes) {

    var result = MSExecuteOnServer('/CompraNet/ValidarModificarFinalizado', { id: id });
    if ((result.Status == "" || result.Status == null) && result.NumeroSio == 0) {
        window.location.href = window.location.origin + "/CompraNet/CrearContrato?id=" + id + '&tipoId=' + tipoId + (siguientes != undefined ? "&siguientes=" + JSON.stringify(siguientes) : "");
    } else {
        MensErr("El contrato ya no se encuentra en slip o fue informado a SIO granos");
    }
}

function ModalFinalizado(contratoId, tipoId, fijacionDePrecioContratoId, fasonId, agenteId, acuerdoId) {
    $(".modal-title-finalizado").empty();

    if (tipoId === "3") {
        $("#contratoModalFinalizado").val(fijacionDePrecioContratoId);
        $(".modal-title-finalizado").append("Fijaci&oacute;n Nro: " + fijacionDePrecioContratoId);
    } else if (tipoId === "4") {
        $("#contratoModalFinalizado").val(fasonId);
        $(".modal-title-finalizado").append("Fas&oacute;n Nro: " + fasonId);
    } else if (tipoId === "5") {
        $("#contratoModalFinalizado").val(agenteId);
        $(".modal-title-finalizado").append("Agente de Compras Nro: " + agenteId);
    } else if (tipoId === "6") {
        $("#contratoModalFinalizado").val(acuerdoId);
        $(".modal-title-finalizado").append("Agente de Compras Nro: " + acuerdoId);
    } else {
        $("#contratoModalFinalizado").val(contratoId);
        $(".modal-title-finalizado").append("Contrato DataAgro: " + contratoId);
    }
    $("#tipoNegocioModalFinalizado").val(tipoId);
    $("#buttonFinalizar").show();
    $("#modalFinalizado").modal('show');
}

function ObtenerDatosModalFinalizado() {
    $("#buttonFinalizar").hide();

    var objFinalizado = {};

    if ($("#tipoNegocioModalFinalizado").val() === "3") {
        objFinalizado.fijacionDePrecioContratoId = $("#contratoModalFinalizado").val();
    } else if ($("#tipoNegocioModalFinalizado").val() === "4") {
        objFinalizado.fasonId = $("#contratoModalFinalizado").val();
    } else if ($("#tipoNegocioModalFinalizado").val() === "5") {
        objFinalizado.agenteId = $("#contratoModalFinalizado").val();
    } else if ($("#tipoNegocioModalFinalizado").val() === "6") {
        objFinalizado.acuerdoId = $("#contratoModalFinalizado").val();
    } else {
        objFinalizado.contratoId = $("#contratoModalFinalizado").val();
    }

    Finalizar(objFinalizado);
}

function Finalizar(finalizarContratoFijacion) {
    var result;
    if ($("#tipoNegocioModalFinalizado").val() === "3") {
        result = MSExecuteOnServer('/CompraNet/FinalizarFijacion', finalizarContratoFijacion);
    } else if ($("#tipoNegocioModalFinalizado").val() === "4") {
        result = MSExecuteOnServer('/CompraNet/FinalizarFason', finalizarContratoFijacion);
    } else if ($("#tipoNegocioModalFinalizado").val() === "5") {
        result = MSExecuteOnServer('/CompraNet/FinalizarAgente', finalizarContratoFijacion);
    } else if ($("#tipoNegocioModalFinalizado").val() === "6") {
        result = MSExecuteOnServer('/CompraNet/FinalizarAcuerdo', finalizarContratoFijacion);
    } else {
        result = MSExecuteOnServer('/CompraNet/FinalizarContrato', finalizarContratoFijacion);
    }

    if (result != null) {
        if (result.Errores != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr(result.Errores[0].Message);
            } else {
                recargarGrilla();
            }
        }
    }
    recargarGrilla();
}
function ModalAprobar(contratoId, tipoId, fijacionDePrecioContratoId) {
    $(".modal-title-aprobado").empty();
    if (tipoId === "3") {
        $("#contratoModalAprobar").val(fijacionDePrecioContratoId);
        $(".modal-title-aprobado").append("Fijaci&oacute;n Nro: " + fijacionDePrecioContratoId);
    } else {
        $("#contratoModalAprobar").val(contratoId);
        $(".modal-title-aprobado").append("Contrato DataAgro: " + contratoId);
    }
    $("#tipoNegocioModalFinalizado").val(tipoId);
    $("#modalAprobar").modal('show');
}
function Aprobar() {
    var result;
    var aprobar = $("#contratoModalAprobar").val();
    var tipoAprobar = $("#tipoNegocioModalFinalizado").val();
    if (tipoAprobar == 3) {
        result = MSExecuteOnServer('/CompraNet/AprobarFijacion', { id: aprobar });
    }
    if (tipoAprobar == 1 || tipoAprobar == 2) {
        result = MSExecuteOnServer('/CompraNet/AprobarContrato', { id: aprobar });
    }

    if (result != null) {
        if (result.Errores != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr(result.Errores[0].Message);
            } else {
                recargarGrilla();
            }
        }
    }
    recargarGrilla();
}

function ReenviarMails(reenviarMail) {
    var result = MSExecuteOnServer('/CompraNet/ReenviarMails', reenviarMail);

    if (result !== null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
    }
}

function ModalConfirmadoVarios() {
    $("#negocioConfirmado-modal").empty();
    $("#confirmarVarios").show();
    $("#cancelarVarios").show();
    $("#cerrarVarios").hide();
    $("#negocioFinalizado-modal").html('');
    $("#negocioConfirmado-modal").html('');
    $("#confirmarVarios").prop("disabled", false);
    $("#confirmarVarios").addClass('myBtn').removeClass('myBtn-disabled');


    var negocios = SeleccionarElementos();
    if (negocios != null && negocios.length > 0) {
        var n = negocios.filter(function (x) { return x.ServicioModificado == true });
        if (n != null && n.length > 0) {
            $(".calidadModificada").show();
            $("#confirmarVariosSinCalidad").show();            
        } else {
            $(".calidadModificada").hide();
            $("#confirmarVariosSinCalidad").hide();   
        }
    }
    if (negocios.length > 0) {
        for (var i in negocios) {
            var loader = '<div class="col-xs-1"><div id="estado' + i + '" class="loader" hidden></div></div><div id="error' + i + '" class="col-xs-8"> </div>';
            if ((negocios[i].Estado === 1 || negocios[i].Estado === 3 || negocios[i].Estado === 7) && puedeConfirmarNegocio(negocios[i])) {
                if (negocios[i].TipoNegocioId === 3) {
                    $("#negocioConfirmado-modal").append('<div class="row"><div class="col-xs-3">Fijaci&oacute;n: ' + negocios[i].Id + '</div>' + loader + '</div>');
                } else {
                    $("#negocioConfirmado-modal").append('<div class="row"><div class="col-xs-3">Contrato: ' + negocios[i].Id + '</div>' + loader + '</div>');
                }
            }
        }
        //ConfirmarVariosContratos();
    } else {
        $("#negocioConfirmado-modal").append('<div style="text-align:center"> Se debe seleccionar negocios</div>');
        $("#confirmarVarios").hide();
        $("#confirmarVariosSinCalidad").hide();
        $(".calidadModificada").hide();
        $("#cancelarVarios").hide();
        $("#cerrarVarios").show();
    }
    $("#ModalConfirmarVarios").modal('show');
}



function ConfirmarVariosContratos(modificarSinCalidad) {
    $("#confirmarVarios").hide();
    $("#confirmarVariosSinCalidad").hide();
    $("#cancelarVarios").hide();
    $("#cerrarVarios").show();

    var negocios = SeleccionarElementos();

    $(".loader").show();
    negocios = modificarSinCalidad ? negocios.filter(function (x) { return x.ServicioModificado == false }) : negocios;
    if (negocios != null && negocios.length == 0) {
        $("#negocioConfirmado-modal").hide();
        $(".mensajeServicio").show();
    } else {
        $("#negocioConfirmado-modal").show();
        $(".mensajeServicio").hide();
        for (var i in negocios) {
            if ((negocios[i].Estado === 1 || negocios[i].Estado === 3 || negocios[i].Estado === 7) && puedeConfirmarNegocio(negocios[i])) {
                var objConfirmado = {};
                $("#estado" + i).empty();
                var result = null;
                if (negocios[i].TipoNegocioId === 3) {
                    objConfirmado.fijacionDePrecioContratoId = negocios[i].FijacionDePrecioContratoId;
                    result = MSExecuteOnServer('/CompraNet/ConfirmarFijacion', objConfirmado);
                } else if (negocios[i].TipoNegocioId === 2 || negocios[i].TipoNegocioId === 1) {
                    objConfirmado.contratoId = negocios[i].ContratoId;
                    result = MSExecuteOnServer('/CompraNet/ConfirmarContrato', objConfirmado);
                } else if (negocios[i].TipoNegocioId === 6) {
                    objConfirmado.fijacionDePrecioContratoId = negocios[i].Id;
                    result = MSExecuteOnServer('/CompraNet/ConfirmarAcuerdo', objConfirmado);
                }
                finalizacionCallBack(i, result);
            }
        }
    }
}

function ModalFinalizarVarios() {
    $("#negocioFinalizado-modal").empty();
    $("#finalizarVarios").show();
    $("#cancelarVariosFinalizado").show();
    $("#cerrarVariosFinalizado").hide();

    $("#negocioFinalizado-modal").html('');
    $("#negocioConfirmado-modal").html('');

    $("#finalizarVarios").prop("disabled", false);
    $("#finalizarVarios").addClass('myBtn').removeClass('myBtn-disabled');

    var negocios = SeleccionarElementos();
    negocios = negocios.filter(function (neg) { return neg.TipoNegocioId == 1 || neg.TipoNegocioId == 2 || neg.TipoNegocioId == 3; });
    if (negocios.length > 0) {
        for (var i in negocios) {
            var loader = '<div class="col-xs-1"><div id="estado' + i + '" class="loader" hidden></div></div><div id="error' + i + '" class="col-xs-8"> </div>';
            if (negocios[i].Estado === 2 || negocios[i].Estado === 4) {
                if (negocios[i].TipoNegocioId === 3) {
                    $("#negocioFinalizado-modal").append('<div class="row"><div class="col-xs-4">Fijaci&oacute;n: ' + negocios[i].FijacionDePrecioContratoId + '</div>' + loader + '</div>');
                } else if (negocios[i].TipoNegocioId === 4) {
                    $("#negocioFinalizado-modal").append('<div class="row"><div class="col-xs-4">Fas&oacute;n: ' + negocios[i].FasonId + '</div>' + loader + '</div>');
                } else if (negocios[i].TipoNegocioId === 5) {
                    $("#negocioFinalizado-modal").append('<div class="row"><div class="col-xs-4">Agente de Compras: ' + negocios[i].AgenteId + '</div>' + loader + '</div>');
                } else {
                    $("#negocioFinalizado-modal").append('<div class="row"><div class="col-xs-4">Contrato: ' + negocios[i].ContratoId + '</div>' + loader + '</div>');
                }
            }
        }
    } else {
        $("#negocioFinalizado-modal").append('<div style="text-align:center"> Se debe seleccionar negocios</div>');
        $("#finalizarVarios").hide();
        $("#cancelarVariosFinalizado").hide();
        $("#cerrarVariosFinalizado").show();
    }
    $("#ModalFinalizarVarios").modal('show');
}

function FinalizarVariosContratos() {
    $("#finalizarVarios").hide();
    $("#cancelarVariosFinalizado").hide();
    $("#cerrarVariosFinalizado").show();

    var negocios = SeleccionarElementos();
    negocios = negocios.filter(function (neg) { return neg.TipoNegocioId == 1 || neg.TipoNegocioId == 2 || neg.TipoNegocioId == 3; });
    $(".loader").show();
    for (var i in negocios) {
        if (negocios[i].Estado === 2 || negocios[i].Estado === 4) {
            var objFinalizado = {};
            $("#estado" + i).empty();
            var result = null;
            if (negocios[i].TipoNegocioId === 3) {
                objFinalizado.fijacionDePrecioContratoId = negocios[i].FijacionDePrecioContratoId;
                result = MSExecuteOnServer('/CompraNet/FinalizarFijacion', objFinalizado);
            } else if (negocios[i].TipoNegocioId === 4) {
                objFinalizado.fasonId = negocios[i].FasonId;
                result = MSExecuteOnServer('/CompraNet/FinalizarFason', objFinalizado);
            } else if (negocios[i].TipoNegocioId === 5) {
                objFinalizado.agenteId = negocios[i].AgenteId;
                result = MSExecuteOnServer('/CompraNet/FinalizarAgente', objFinalizado);
            } else if (negocios[i].TipoNegocioId === 6) {
                objFinalizado.acuerdoId = negocios[i].AcuerdoId;
                result = MSExecuteOnServer('/CompraNet/FinalizarAcuerdo', objFinalizado);
            } else {
                objFinalizado.contratoId = negocios[i].ContratoId;
                result = MSExecuteOnServer('/CompraNet/FinalizarContrato', objFinalizado);
            }
            finalizacionCallBack(i, result);
        }
    }
}

function ModalModificarVarios() {
    $("#negocioModificar-modal").empty();
    $("#modificarVarios").show();
    $("#cancelarVariosModificar").show();
    $("#cerrarVariosModificar").hide();

    $("#negocioFinalizado-modal").html('');
    $("#negocioConfirmado-modal").html('');

    $("#modificarVarios").prop("disabled", false);
    $("#modificarVarios").addClass('myBtn').removeClass('myBtn-disabled');
    $("#spanModificar").html('<span>Se modificaran los siguientes negocios:</span>');

    var negocios = SeleccionarElementos();
    var hayNegociosParaModificar = false;
    if (negocios.length > 0) {
        for (var i in negocios) {
            if (negocios[i].Estado !== 5 && negocios[i].Estado !== 6 && negocios[i].Estado !== 8) {
                if (negocios[i].TipoNegocioId === 3) {
                    $("#negocioModificar-modal").append('<div class="row"><div class="col-xs-4">Fijaci&oacute;n: ' + negocios[i].FijacionDePrecioContratoId + '</span></div>');
                } else if (negocios[i].TipoNegocioId === 4) {
                    $("#negocioModificar-modal").append('<div class="row"><div class="col-xs-4">Fas&oacute;n: ' + negocios[i].FasonId + '</div>');
                } else if (negocios[i].TipoNegocioId === 5) {
                    $("#negocioModificar-modal").append('<div class="row"><div class="col-xs-4">Agente de Compras: ' + negocios[i].AgenteId + '</div>');
                } else if (negocios[i].TipoNegocioId === 6) {
                    $("#negocioModificar-modal").append('<div class="row"><div class="col-xs-4">Contrato Acuerdo: ' + negocios[i].AcuerdoId + '</div>');
                } else {
                    $("#negocioModificar-modal").append('<div class="row"><div class="col-xs-4">Contrato: ' + negocios[i].ContratoId + '</div>');
                }
                hayNegociosParaModificar = true;
            }
        }
    }
    if (hayNegociosParaModificar == false) {
        $("#spanModificar").html('<div style="text-align:center"> Se debe seleccionar negocios</div>');
        $("#modificarVarios").hide();
        $("#cancelarVariosModificar").hide();
        $("#cerrarVariosModificar").show();
    }
    $("#ModalModificarVarios").modal('show');
}

function ModificarVariosContratos() {
    $("#ModalModificarVarios").modal('hide');
    var negocios = SeleccionarElementos();
    $(".loader").show();
    var objs = [];
    var id = 0;
    var tipoId = 0;
    if (negocios.length > 0) {
        for (var i in negocios) {
            if (negocios[i].Estado != 5 && negocios[i].Estado !== 6 && negocios[i].Estado !== 8) {
                if (id == 0) {
                    id = negocios[i].Id;
                    tipoId = negocios[i].TipoNegocioId;
                } else {
                    var obj = {};
                    obj.TipoNegocioId = negocios[i].TipoNegocioId;

                    obj.Id = negocios[i].Id;
                    objs.push(obj);
                }
            }
        }
        editarContrato(id, tipoId, objs);
    }
}

function finalizacionCallBack(i, result) {
    if (result !== null && result.Errores !== null && ExistsErrorMessages(result.Errores)) {
        $("#estado" + i).removeClass("loader");
        $("#estado" + i).append('<img src="/Content/Images/Cancelar.png" alt="error" title="' + result.Errores[0].Message + '" />');
        $("#error" + i).append(result.Errores[0].Message);
    } else {
        $("#estado" + i).removeClass("loader");
        $("#estado" + i).append('<img src="/Content/Images/Aceptar.png" alt="Confirmado"/>');
    }
}

function ObtenerDatosModalConfirmado() {
    var objConfirmado = {};
    if ($("#tipoNegocioModalConTilde").val() === '3') {
        objConfirmado.fijacionDePrecioContratoId = $("#contratoModalConTilde").val();
    } else if ($("#tipoNegocioModalConTilde").val() === '6') {
        objConfirmado.fijacionDePrecioContratoId = $("#contratoModalConTilde").val();
    } else if ($("#tipoNegocioModalConTilde").val() === '5') {
        objConfirmado.fijacionDePrecioContratoId = $("#contratoModalConTilde").val();
    } else if ($("#tipoNegocioModalConTilde").val() === '4') {
        objConfirmado.fijacionDePrecioContratoId = $("#contratoModalConTilde").val();
    } else {
        objConfirmado.contratoId = $("#contratoModalConTilde").val();
    }
    Confirmar(objConfirmado);
}

function ObtenerDatosModalConfirmadoFinalizado() {

    var contratoId = $("#contratoModalConTildeFinalizado").val();
    result = MSExecuteOnServer('/CompraNet/ReconfirmarFinalizado', { contratoId: contratoId });
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}

function Confirmar(confirmarContratoFijacion) {
    var result = null;
    var confirmar = true;
    if ($("#tipoNegocioModalConTilde").val() === '3') {
        result = MSExecuteOnServer('/CompraNet/ConfirmarFijacion', confirmarContratoFijacion);
    } else if ($("#tipoNegocioModalConTilde").val() === '6') {
        result = MSExecuteOnServer('/CompraNet/ConfirmarAcuerdo', confirmarContratoFijacion);
    } else if ($("#tipoNegocioModalConTilde").val() === '5') {
        result = MSExecuteOnServer('/CompraNet/ConfirmarAgente', confirmarContratoFijacion);
    } else if ($("#tipoNegocioModalConTilde").val() === '4') {
        result = MSExecuteOnServer('/CompraNet/ConfirmarFason', confirmarContratoFijacion);
    } else if ($("#tipoNegocioModalConTilde").val() === '2') {
        result = MSExecuteOnServer('/CompraNet/ConfirmarContrato', confirmarContratoFijacion);
    } else if ($("#tipoNegocioModalConTilde").val() === '1') {
        var asociados = MSExecuteOnServer('/CompraNet/DevolverSiTieneAsociados', confirmarContratoFijacion);
        if ($("#posicion").val() != null && $("#posicion").val() == "PASE" && asociados == false) {
            confirmar = false;
        }
        else {
            result = MSExecuteOnServer('/CompraNet/ConfirmarContrato', confirmarContratoFijacion);
        }
    }

    if (confirmar == true) {
        if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            recargarGrilla();
        }
    } else {
        MensErr("Los contratos A fijar pase deben tener al menos un contrato asociado antes de poder confirmarse. Por favor asociar contratos primero")
    }

}
function ObtenerDatosModalPreAnular() {
    var motivo = $("#motivo-rechazoNegocio").val();
    if (motivo == null || motivo == "" || motivo == undefined) {
        MensErr("Ingrese un motivo de solicitud");
        return;
    } else {
        if (motivo.length > 1000) {
            MensErr("El motivo de rechazo es demasiado largo.");
            return;
        }
    }
    var result;
    var id = $("#contratoModalAnular").val();
    var tipoNegocio = $("#tipoNegocioModalBorrar").val();
    var fijacionId = $("#fijacionModalAnular").val();
    if (tipoNegocio === '3') {
        result = MSExecuteOnServer('/CompraNet/PreAnularFijacionVirtual', { fijacionId: fijacionId, motivo });
    } else {
        result = MSExecuteOnServer('/CompraNet/PreAnularContrato', { contratoId: id, motivo });
    }

    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}
function ObtenerDatosModalBorrarPreAnular() {
    //var motivoRechazo = $("#motivo-rechazoSolictud").val();
    //var objConfirmado = {};
    //objConfirmado.MotivoRechazo = motivoRechazo;
    //if (motivoRechazo == null || motivoRechazo == "" || motivoRechazo == undefined) {
    //    MensErr("Ingrese un motivo de rechazo");
    //    return;
    //} else {
    //    if (motivoRechazo.length > 1000) {
    //        MensErr("El motivo de rechazo es demasiado largo.");
    //        return;
    //    }
    //}
    var result;
    var id = $("#contratoModalBorrar").val();
    var fijacionId = $("#fijacionModalAnular").val();
    var tipoNegocio = $("#tipoNegocioModalBorrar").val();
    if (tipoNegocio === '3') {
        result = MSExecuteOnServer('/CompraNet/RechazarPreAnularFijacionVirtual', { fijacionId: fijacionId/*, motivoRechazo*/ });
    } else {
        result = MSExecuteOnServer('/CompraNet/RechazarPreAnularContrato', { contratoId: id/*, motivoRechazo*/ });
    }
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}
function AnularContratoPreAnulado() {
    var result;
    var id = $("#contratoModalBorrar").val();
    var fijacionId = $("#fijacionModalAnular").val();
    var tipoNegocio = $("#tipoNegocioModalBorrar").val();
    if (tipoNegocio === '3') {
        result = MSExecuteOnServer('/CompraNet/AnularFijacionVirtual', { fijacionId: fijacionId/*, motivoRechazo*/ });
    } else {
        result = MSExecuteOnServer('/CompraNet/AnularContratoPreAnulado', { contratoId: id });
    }
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}
function ObtenerDatosModalBorrado() {
    var motivoRechazo = $("#motivo-rechazo").val();
    var objConfirmado = {};
    objConfirmado.MotivoRechazo = motivoRechazo;
    if (motivoRechazo == null || motivoRechazo == "" || motivoRechazo == undefined) {
        MensErr("Ingrese un motivo de rechazo");
        return;
    } else {
        if (motivoRechazo.length > 1000) {
            MensErr("El motivo de rechazo es demasiado largo.");
            return;
        }
    }
    var result;
    if ($("#tipoNegocioModalBorrar").val() === '3') {
        if ($("#estadoModalBorrar").val() != '9') {
            objConfirmado.Id = $("#contratoModalBorrar").val();
            result = MSExecuteOnServer('/CompraNet/BorrarFijacion', objConfirmado);
        } else {
            id = $("#contratoModalBorrar").val();
            result = MSExecuteOnServer('/CompraNet/BorrarFijacionPreAprobacion', { id: id, motivoRechazo: motivoRechazo });
        }
    } else if ($("#tipoNegocioModalBorrar").val() === '4') {
        objConfirmado.Id = $("#contratoModalBorrar").val();
        result = MSExecuteOnServer('/CompraNet/BorrarFason', objConfirmado);
    } else if ($("#tipoNegocioModalBorrar").val() === '5') {
        objConfirmado.Id = $("#contratoModalBorrar").val();
        result = MSExecuteOnServer('/CompraNet/BorrarAgente', objConfirmado);
    } else if ($("#tipoNegocioModalBorrar").val() === '6') {
        objConfirmado.Id = $("#contratoModalBorrar").val();
        result = MSExecuteOnServer('/CompraNet/BorrarAcuerdo', objConfirmado);
    } else {
        objConfirmado.Id = $("#contratoModalBorrar").val();

        if ($("#estadoModalBorrar").val() == '5') {
            result = MSExecuteOnServer('/CompraNet/AnularContrato', objConfirmado);
        } else {
            if ($("#estadoModalBorrar").val() == '9') {
                result = MSExecuteOnServer('/CompraNet/BorrarContratoPreAprobacion', objConfirmado);
            } else {
                result = MSExecuteOnServer('/CompraNet/BorrarContrato', objConfirmado);
            }
        }
    }

    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}

function ModalConError(contratoId, proveedor, fechaDesdeHasta, tipo, material, cantidad, precio, campana, provincia, localidad, nroSAP, sustentablePrecio, sustentableMoneda, dolarizadoFecha, pesificadoDias, informaSIO, trigoEspecial) {
    if (nroSAP == "null" || "undefined ") {
        nroSAP = "";
    }

    $(".modal-title-conError").empty();
    $(".modal-title-conError").append("Contrato Nro SAP: " + nroSAP);
    $("#proveedorDivConError").append(proveedor);
    $("#fechaDesdeHastaDivConError").append(fechaDesdeHasta);
    $("#tipoDivConError").append(tipo);
    $("#materialDivConError").append(material);
    $("#cantidadDivConError").append(cantidad);
    $("#precioDivConError").append(precio);
    $("#monedaStrongConError").append(fechaDesdeHasta);
    $("#campanaDivConError").append(campana);
    $("#procedenciaDivConError").append(provincia + ', ' + localidad);
    $("#sustentableDivConError").append(sustentablePrecio);
    $("#sustentableMonedaStrongConError").append(sustentableMoneda);
    $("#dolarizadoFechaDivConError").append(dolarizadoFecha);
    $("#pesificadoDiasDivConError").append(pesificadoDias);
    $("#informaSIODivConError").append(informaSIO);
    $("#trigoEspecialDivConError").append(trigoEspecial);
    $("#modalConError").modal('show');
}


function ModalAsociarNegocios(contratoId, estadoId) {
    $("#paseId").val(contratoId);
    var editar = estadoId == '1' ? true : false;
    $("#sePuedeEditar").val(editar);
    ActualizarAsociados(contratoId);
    $("#PanelAsociados").modal('show');
}

function ActualizarAsociados(contrato) {
    var datos = { contratoId: contrato };
    viewModelAsociados.set("Asociados", MSExecuteOnServer("/CompraNet/DevolverContratoAsociadosPase", datos));
    asociados = viewModelAsociados.Asociados;
    var grid = $("#gridAsociarNegocios").data("kendoGrid");
    var dataSource = new kendo.data.DataSource({
        data: asociados
    });
    grid.setDataSource(dataSource);
    CalcularPonderado();
    setTimeout(function () { grid.setOptions({ height: 200 }) }, 200);
    if ($("#sePuedeEditar").val() == true) {
        $(".botonAgregar").show();
    } else {
        $(".botonAgregar").hide();
    }
}

function ObtenerDatosModalConError() {
    Finalizar(objFinalizado);
}

function ModalConfirmadoTilde(estado, contratoId, nroSAP, fijacionDePrecioContratoId, tipoId, acuerdoId, agenteId, fasonId, mensaje, posicion, servicioModificado) {
    $(".modal-title-confirmadoTilde").empty();

    if (tipoId === '3') {
        $("#contratoModalConTilde").val(fijacionDePrecioContratoId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + contratoId);
    } else if (tipoId === '6') {
        $("#contratoModalConTilde").val(acuerdoId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + acuerdoId);
    } else if (tipoId === '5') {
        $("#contratoModalConTilde").val(agenteId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + agenteId);
    } else if (tipoId === '4') {
        $("#contratoModalConTilde").val(fasonId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + fasonId);
    } else {
        $("#contratoModalConTilde").val(contratoId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + contratoId);
    }
    $("#mensajeModalConTilde").html(mensaje);

    if (servicioModificado == 'true') {
        $(".calidadModificadaManual").show();
    } else {
        $(".calidadModificadaManual").hide();
    }
    $("#tipoNegocioModalConTilde").val(tipoId);
    $("#modalConfirmadoTilde").modal('show');
    $("#contratoId").val(contratoId);
    $("#posicion").val(posicion);
}

function ModalConfirmadoTildeFinalizado(estado, contratoId, nroSAP, fijacionDePrecioContratoId, tipoId, acuerdoId, agenteId, fasonId) {
    $(".modal-title-confirmadoTilde").empty();

    if (tipoId === '3') {
        $("#contratoModalConTildeFinalizado").val(fijacionDePrecioContratoId);
        $(".modal-title-confirmadoTildeFinalizado").append("Contrato DataAgro: " + contratoId);
    } else if (tipoId === '6') {
        $("#contratoModalConTildeFinalizado").val(acuerdoId);
        $(".modal-title-confirmadoTildeFinalizado").append("Contrato DataAgro: " + acuerdoId);
    } else if (tipoId === '5') {
        $("#contratoModalConTildeFinalizado").val(agenteId);
        $(".modal-title-confirmadoTildeFinalizado").append("Contrato DataAgro: " + agenteId);
    } else if (tipoId === '4') {
        $("#contratoModalConTildeFinalizado").val(fasonId);
        $(".modal-title-confirmadoTildeFinalizado").append("Contrato DataAgro: " + fasonId);
    } else {
        $("#contratoModalConTildeFinalizado").val(contratoId);
        $(".modal-title-confirmadoTildeFinalizado").append("Contrato DataAgro: " + contratoId);
    }

    $("#tipoNegocioModalConTildeFinalizado").val(tipoId);

    $("#modalConfirmadoTildeFinalizado").modal('show');
}

function ModalAmpliaciones(contrato, ampliacion, tipoNegocio, FijacionDePrecioContratoId, fasonId, agenteId) {
    $(".modal-title-ampliaciones").empty();
    $(".modal-title-ampliaciones").append("Contrato DataAgro: " + contrato);

    if (tipoNegocio === "3") {
        $("#contratoIdAmpliaciones").val(FijacionDePrecioContratoId);
    } else if (tipoNegocio === "4") {
        $("#contratoIdAmpliaciones").val(fasonId);
    } else if (tipoNegocio === "5") {
        $("#contratoIdAmpliaciones").val(agenteId);
    } else {
        $("#contratoIdAmpliaciones").val(contrato);
    }

    $("#tipoNegocioIdAmpliaciones").val(tipoNegocio);

    if (ampliacion != 0 && ampliacion != "undefined") $("#inputAmpliaciones").val(ampliacion);

    $("#modalAmpliaciones").modal('show');
}

function ObtenerDatosModalAmpliaciones() {
    var objAmpliaciones = {};

    objAmpliaciones.ContratoId = $("#contratoIdAmpliaciones").val();
    objAmpliaciones.Ampliaciones = $("#inputAmpliaciones").val();
    //if ($("#tipoNegocioIdAmpliaciones").val() == 3) {
    //    objAmpliaciones.FijacionDePrecioContratoId = $("#contratoIdAmpliaciones").val();
    //} else if ($("#tipoNegocioIdAmpliaciones").val() == 4 || $("#tipoNegocioIdAmpliaciones").val() == 5) {
    //    objAmpliaciones.Id = $("#contratoIdAmpliaciones").val();
    //}
    objAmpliaciones.Id = $("#contratoIdAmpliaciones").val();
    GuardarAmpliacion(objAmpliaciones);
}

function GuardarAmpliacion(ampliacion) {
    var result;
    if ($("#tipoNegocioIdAmpliaciones").val() === "3") {
        result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionFijacion', ampliacion);
    } else if ($("#tipoNegocioIdAmpliaciones").val() === "4") {
        result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionFason', ampliacion);
    } else if ($("#tipoNegocioIdAmpliaciones").val() === "5") {
        result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionAgente', ampliacion);
    } else {
        result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionContrato', ampliacion);
    }

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            recargarGrilla();
            MensInfo("Se ha guardado la ampliacion con exito");
        }
    }

    $("#inputAmpliaciones").val('');
    $("#contratoIdAmpliaciones").val('');
}

function ModalVisualizar(id, contrato, proveedor, corredor, fecha, desdeHasta, tipo, material, cantidad, precio, comercial, monedaId, precioMoneda,
    campana, provincia, localidad, nro_SAP, negocio, sustentable, sustentablePrecio, sustentableMonedaId, tarifaAConvenir, dolarizadoFecha, pesificadoDias, informaSIO,
    trigoEspecial, status, Observacion, moneda, sustentableMoneda, destino, destinoDescripcion, cantidadCamiones, consignatario, planCanje, condicionFijacionId,
    cd, warrant, pagoDirectoVendedor, establecimientoPropio, boletoId, bolsaId, boletoDescripcion, bolsaDescripcion, desdeHastaFijacion, condicionFijacionDescripcion,
    clasificacionId, clasificacionDescripcion, standardDeCalidadDescripcion, calidadEspecialDescripcion, desdeFijacion, hastaFijacion, mercsFijacion,
    contratoCorredor, contratoVendedor, selCargoMOA, selCargoVendedor, tipoFason, posicion, operador, precioNeto, id, pizarra, zona, nivelTarifa, tarifaFlete,
    compensacion, rechazo, fechaCierta, porcentajeDePago, agenteDeCompra, FechaOperacion, MotivoOperacionAnterior, descripcionOperacionAnterior, pagoCbu, cheque, CalidadTercero, DolarizadoTercero, PagoDiferidoTercero,
    Canje, Monto, MonedaCanje, Insumo, Prestamo, PlantaDestino, ObservacionTercero, SustentableTercero, Venta, fechaDesdeSustentable, fechaHastaSustentable, obligatoriedad, PosicionCBOT, TipoPosicionCBOT, ProveedorCreador,
    Cesion, MotivoReemplazo, AnulaYReemplazaContratoSAP, obligatoriedadBond, Condicional,
    CondicionalCantidad, CondicionalFechaFormateado, CondicionalMonedaId, CondicionalPosicion, CondicionalPrecio, CondicionalContratoSAP, mailVenta, RazonsocialProveedorComisionista, minimo, maximo,
    ComercialZonaId, FijacionDePrecioContratoId, TipoNegocioId, AcuerdoId, AgenteId, FasonId, Estado, MaterialId, virtual, CantidadDeposito, dolarizadoOriginal, hastaOriginal, servicioModificado) {
    var dataItem = {
        Id: id,
        Estado: Estado,
        ContratoId: contrato,
        ContratoSAP: nro_SAP,
        FijacionDePrecioContratoId: FijacionDePrecioContratoId,
        TipoNegocioId: TipoNegocioId,
        AcuerdoId: AcuerdoId,
        AgenteID: AgenteId,
        FasonId: FasonId,
        TipoPosicionCBOT: TipoPosicionCBOT,

        ComercialZonaId: ComercialZonaId,
        MaterialId: MaterialId,

        ComercialCreadorId: comercialId,
        DestinoId: destino,
        Moneda: moneda,
        PrecioPlazo: precio,
        Precio: precio,
        Virtual: virtual,
        Proveedor: proveedor,
        ServicioModificado: servicioModificado
    };
    $("#modalVisualizar").modal('show');
    if (Estado == '1') {//pendiente
        $("#botoneriaPendDiv").html(botonPendiente(dataItem, 'fa-pencil pend', true) +
            botonConfirmadoTilde(dataItem, 'fa-check pend', true) +
            botonBorrar(dataItem, 'fa-trash pend', true))
    }
    if (Estado == '2') {//confirmado
        if (dataItem.TipoNegocioId == 6 || dataItem.TipoNegocioId == 5) {
            $("#botoneriaConfDiv").html(botonPendiente(dataItem, 'fa-pencil conf', true) +
                botonBorrar(dataItem, 'fa-trash conf'));
        } else {
            $("#botoneriaConfDiv").html(botonPendiente(dataItem, 'fa-pencil conf', true) +
                botonFinalizado(dataItem, 'fa-flag-checkered conf', true) +
                botonBorrar(dataItem, 'fa-trash conf', true));
        }
    }
    if (Estado == '3') {//oferta
        $("#botoneriaOferDiv").html(botonPendiente(dataItem, 'fa-pencil ofe', true) +
            botonConfirmadoTilde(dataItem, 'fa-check ofe', true) +
            botonBorrar(dataItem, 'fa-trash ofe', true))
    }
    if (Estado == '4') {//error
        if (dataItem.TipoNegocioId == 6) {
            $("#botoneriaErroDiv").html(botonPendiente(dataItem, 'fa-pencil err', true) +
                botonBorrar(dataItem, 'fa-trash err', true));
        } else {
            $("#botoneriaErroDiv").html(botonPendiente(dataItem, 'fa-pencil err', true) +
                botonFinalizado(dataItem, 'fa-flag-checkered err', true) +
                botonBorrar(dataItem, 'fa-trash err', true));
        }
    }
    if (Estado == '5') { //Finalizado
        //if (verMesa && (dataItem.ContratoId || dataItem.FasonId || dataItem.AgenteId)) {
        $("#botoneriaFinaDiv").html(((dataItem.Virtual != true && dataItem.TipoNegocioId == 3) ? "" : botonPreAnular(dataItem, 'fa-trash fin', true)) +
            ((dataItem.Virtual == true) ? "" : botonModificarFinalizados(dataItem, 'fa-pencil fin', true))
        );
        //} else {
        //    $("#botoneriaFinaDiv").html('');
        //}
    }
    if (Estado == 6) { //Rechazado
        //$("#botoneriaDiv").html('');
    }
    if (dataItem.Estado == 7) { //reconfirmar
        $("#botoneriaRecoDiv").html(botonPendiente(dataItem, 'fa-pencil reconf', true) +
            botonConfirmadoTilde(dataItem, 'fa-check reconf', true) +
            botonBorrar(dataItem, 'fa-trash reconf', true))
    }
    if (dataItem.Estado == 8) { //Anulado
        //$("#botoneriaDiv").html('');
    }
    if (dataItem.Estado == 9) { //preaprobacion
        $("#botoneriaDiv").html(botonPendiente(dataItem, 'fa-pencil pre', true) +
            botonBorrar(dataItem, 'fa-trash pre', true));
    }
    if (dataItem.Estado == 10) { //PreAnulado                  
        $("#botoneriaDiv").html(botonBorrarPreanulado(dataItem, 'fa-trash bor', true));
    }
    if (dataItem.Estado == 11) { //ReconfirmarFinalizado 
        $("#botoneriaRecoDiv").html(botonPendiente(dataItem, 'fa-pencil reconf', true) +
            botonConfirmadoTildeFinalizado(dataItem, 'fa-check reconf', true) +
            botonBorrar(dataItem, 'fa-trash reconf', true))
    }


    $("#contrato").text(contrato);
    if (status == "11" || status == "7") {
        $("#mostrar").show();
    } else {
        $("#mostrar").hide();
        $("#ocultar").hide();
    }
    visualizacionRowDoblePrecioCero("precioDivVisualizar", "comercialDivVisualizar", false);
    if (tipo === "FIJACION") {
        $(".noFason").show();
        $(".fason").hide();
        $(".noFijacion").hide();
        $("#VisualizarServicioDiv").hide();
    } else if (tipo === "FASON") {
        $(".noFason").hide();
        $(".fason").show();
        $("#visualizar_tipofason").text(tipoFason);
        $("#visualizar_posicion").text(posicion);
        $("#VisualizarServicioDiv").hide();
    } else if (tipo === "AGENTE DE COMPRAS") {
        $(".noAgente").hide();
        $(".agente").show();
        $("#visualizar_operador").text(operador);
        $("#visualizar_posicion").text(posicion);
        $("#VisualizarServicioDiv").hide();
    } else if (tipo === "A PRECIO") {
        $(".noFason").show();
        $(".noAgente").show();
        $(".noFijacion").show();
        $("#fechaDivVisualizar").show();
        $("fechaDesdeHastaDivVisualizar").show();
        $("fechaDesdeHastaOriginalDivVisualizar").show();
        $(".fason").hide();
        //var mostrarServicio = ArmarServicio(id);
        ArmarServicio(id);
        if (viewModel.Servicios != null && viewModel.Servicios.length > 0) {
        $("#VisualizarServicioDiv").show();
        } else {
            $("#VisualizarServicioDiv").hide();
        }

    } else {
        visualizacionRowDoblePrecioCero("precioDivVisualizar", "comercialDivVisualizar", true);
        $(".noFason").show();
        $(".noAgente").show();
        $(".noFijacion").show();
        $("#fechaDivVisualizar").show();
        $("fechaDesdeHastaDivVisualizar").show();
        $("fechaDesdeHastaOriginalDivVisualizar").show();
        $(".fason").hide();
        $("#VisualizarServicioDiv").hide();
        //var mostrarServicio = ArmarServicio(id);
        if (tipo === "A FIJAR") {
            ArmarServicio(id);
            if (viewModel.Servicios != null && viewModel.Servicios.length > 0) {
                $("#VisualizarServicioDiv").show();
            } else {
                $("#VisualizarServicioDiv").hide();
            }
        }
    }
    if (tipo === "CONTRATO ACUERDO") {
        $("#pactadosDivVisualizar").hide();
        $(".noFijacion").show();
        $("#VisualizarServicioDiv").hide();

    } else {
        $("#pactadosDivVisualizar").show();
    }

    switch (status) {
        case "1": //pendiente
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.pendiente-modal").show();
            break;
        case "2": //confirmado
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.confirmado-modal").show();
            break;
        case "3": //oferta
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.oferta-modal").show();
            break;
        case "4": //con error
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.conerror-modal").show();
            break;
        case "5": //finalizado
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.finalizado-modal").show();
            break;
        case "6": //borrado
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.borrado-modal").show();
            break;
        case "7": //reconfirmar
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.reconfirmar-modal").show();
            break;
        case "11": //reconfirmarFinalizado
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.reconfirmar-modal").show();
            break;
        case "10": //preanular
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.borrado-modal").show();
            break;
    }

    if (tipo == "FIJACION" && status == "5") {
        $(".modal-title-visualizar").empty();
        $(".modal-title-visualizar").append("Fijacion N&deg; SAP: " + (negocio != "null" ? negocio.substr(1, nro_SAP.length - 1) : ""));
    } else {
        $(".modal-title-visualizar").empty();
        $(".modal-title-visualizar").append("Contrato N&deg; SAP: " + (nro_SAP != "null" ? nro_SAP.substr(3, nro_SAP.length - 3) : ""));
    }
    $("#visualizar_proveedor").text(proveedor);
    $("#fechacontrato").text(FechaOperacion == null || FechaOperacion == "" ? desdeHasta : FechaOperacion);
    $("#visualizar_desdeHasta").text(fecha);
    $("#visualizar_desdeHastaOriginal").text(hastaOriginal);
    $("#visualizar_tipo").text(tipo);
    $("#visualizar_comercial").text(comercial);
    $("#visualizar_comercial_AFijar").text(comercial);
    $("#visualizar_material").text(material);
    $("#visualizar_cantidad").text(isNaN(parseInt(cantidad)) ? "" : kendo.toString(parseInt(cantidad), "n0"));
    precio != 0 ? $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2") + " " + moneda) : pizarra ? $("#visualizar_precio").text("Pizarra") : $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2"));
    campana !== "" ? $("#visualizar_campana").text(campana) : $("#visualizar_campana").text("null");

    visualizacionRowDoble("materialDivVisualizar", "visualizar_material", "campanaDivVisualizar", "visualizar_campana");
    if (descripcionOperacionAnterior !== null && descripcionOperacionAnterior !== "" && descripcionOperacionAnterior !== "null") {
        $("#MotivoOperacionAnteriorDivVisualizar").show();
        $("#visualizar_MotivoOperacionAnterior").text(MotivoOperacionAnterior + " - " + descripcionOperacionAnterior);
    } else {
        $("#MotivoOperacionAnteriorDivVisualizar").hide();
    }

    var procedencia = "";
    var provinciaDat = provincia != "undefined" && provincia != "null" ? provincia : "";
    var localidadDat = localidad != "undefined" && localidad != "null" ? localidad : "";
    if (provinciaDat == "" || localidadDat == "") {
        procedencia = provinciaDat + localidadDat;
    }
    else if (provinciaDat != "" && localidadDat != "") {
        procedencia = localidadDat + ", " + provinciaDat;
    }

    if (tipo === "AGENTE DE COMPRAS MP") {
        $("#agenteDeCompraDivVisualizar").show();
        $("#visualizar_agenteDeCompra").text("MATBA ROFEX SA");
    } else {
        $("#agenteDeCompraDivVisualizar").hide();
    }

    if (corredor !== null && corredor !== "") {
        $("#corredorDivVisualizar").show();
        $("#visualizar_corredor").text(corredor);
    } else {
        $("#corredorDivVisualizar").hide();
    }
    contratoCorredor != null && contratoCorredor != "" ? $("#visualizar_contratoCorredor").text(contratoCorredor) : $("#visualizar_contratoCorredor").text("null");
    contratoVendedor != null && contratoVendedor != "" ? $("#visualizar_contratoVendedor").text(contratoVendedor) : $("#visualizar_contratoVendedor").text("null");
    visualizacionRowDoble("contratoCorredorVisualizar", "visualizar_contratoCorredor", "contratoVendedorVisualizar", "visualizar_contratoVendedor");

    selCargoMOA === "true" ? $("#visualizar_selladoACargo").text("MOA") : selCargoVendedor === "true" ? $("#visualizar_selladoACargo").text("Vendedor") : $("#selladoACargoVisualizar").hide();

    if (fechaCierta == "null") {
        $("#FechaCiertaVisualizar").hide();
    } else {
        $("#visualizar_FechaCierta").text(fechaCierta);
        $("#FechaCiertaVisualizar").show();
    }
    if (cheque != "null" && cheque == 'Si') {
        $("#chequeElectronico").show();
    } else {
        $("#chequeElectronico").hide();
    }

    if (pagoCbu != "null") {
        $("#pago").text(pagoCbu);
        $("#PagoCbu").show();
    } else {
        $("#PagoCbu").hide();
    }

    if (porcentajeDePago == "null") {
        $("#porcentajeDePagoDivVisualizar").hide();
    } else {
        $("#visualizar_porcentajeDePago").text(porcentajeDePago + " %");
        $("#porcentajeDePagoDivVisualizar").show();
    }
    $("#visualizar_procedencia").text(procedencia);
    $("#visualizar_nro_SAP").text(nro_SAP != "undefined" && nro_SAP != "null" ? nro_SAP : "");
    $("#visualizar_observacion").text(Observacion != "undefined" && Observacion ? Observacion : "");

    $("#visualizar_condicionFijacion").text(condicionFijacionId);
    $("#visualizar_clasificacion").text(clasificacionDescripcion);
    //sustentablePrecio !== "null" && sustentableMonedaId !== "null" ? $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + sustentableMonedaId) : $("#visualizar_sustentablePrecio").text("null");
    if (sustentable == "true") {
        if (tarifaAConvenir == "true") {
            $("#visualizar_sustentablePrecio").text("Tarifa a Convenir");
        } else {
            $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + sustentableMonedaId);
        }
    } else {
        $("#visualizar_sustentablePrecio").text("null")
    }
    if (fechaDesdeSustentable != null && fechaHastaSustentable != null && fechaDesdeSustentable != "//" && fechaHastaSustentable != "//") {
        $("#visualizar_sustentableDesde").text(fechaDesdeSustentable);
        $("#visualizar_sustentableHasta").text(fechaHastaSustentable);
        $("#sustentableDivVisualizarHasta").show();
        $("#sustentableDivVisualizarDesde").show();
    } else {
        $("#sustentableDivVisualizarHasta").hide();
        $("#sustentableDivVisualizarDesde").hide();
    }
    $("#visualizar_dolarizadoFecha").text(dolarizadoFecha);
    if (dolarizadoFecha != "") {
        $("#visualizar_dolarizadoFecha").show();
    }
    $("#visualizar_dolarizadoOriginalFecha").text(dolarizadoOriginal);
    if (moneda == "USDM ") {
        $("#dolarizadoFechaOriginalDivVisualizar").show();
    }

    $("#visualizar_pesificadoDias").text(pesificadoDias);
    informaSIO === "true" ? $("#visualizar_informaSIO").text("Si") : $("#visualizar_informaSIO").text("null");
    mercsFijacion == "true" ? $("#visualizar_mercsDeposito").text("Si") : "";
    mercsFijacion == "true" ? $("#cantidadDeposito").text(isNaN(parseInt(CantidadDeposito)) ? "" : kendo.toString(parseInt(CantidadDeposito), "n0")) : "";
    cd === "true" ? $("#visualizar_pago").text("CD") : (warrant === "true") ? $("#visualizar_pago").text("Warrant") : (pagoDirectoVendedor === "true") ? $("#visualizar_pago").text("Pago Directo Vendedor") : $("#visualizar_pago").text("null");
    boletoDescripcion === "Ninguno" || boletoDescripcion === null || boletoDescripcion === "" || boletoDescripcion === "undefined" ? ($("#visualizar_boleto").text("null") && $("#visualizar_bolsa").text("null")) : ($("#visualizar_boleto").text(boletoDescripcion) && $("#visualizar_bolsa").text(bolsaDescripcion));

    if (condicionFijacionDescripcion === "undefined" || condicionFijacionDescripcion === "null" || condicionFijacionDescripcion === "false" || condicionFijacionDescripcion === "") {
        $("#desdeHastaFijacionDivVisualizar").hide();
        $("#condicionFijacionDivVisualisar").hide();
    } else {
        $("#visualizar_desdeHastaFijacion").text(desdeHastaFijacion);
        $("#visualizar_condicionFijacion").text(condicionFijacionDescripcion);
    }
    if (tipo === "FIJACION") {
        $("#fechaHastaFijacionDivVisualizar").show();
        $("#visualizar_fechaHastaFijacion").text(hastaFijacion);
    } else {
        $("#fechaHastaFijacionDivVisualizar").hide();

    }
    planCanje === "true" ? $("#visualizar_planCanje").text("Si") : $("#visualizar_planCanje").text("null");
    consignatario === "true" ? $("#visualizar_consignatario").text("Consignatario") : $("#visualizar_consignatario").text("");
    cantidadCamiones !== 0 && cantidadCamiones !== "null" ? $("#visualizar_cantidadDeCamiones").text(cantidadCamiones) : $("#visualizar_cantidadDeCamiones").text("null");
    establecimientoPropio === "true" ? $("#visualizar_establecimiento").text("Propio") : establecimientoPropio === "false" ? $("#visualizar_establecimiento").text("Arrendado") : $("#visualizar_establecimiento").text("null");
    destinoDescripcion != "" ? $("#visualizar_destino").text(destinoDescripcion) : $("#visualizar_destino").text("null");
    visualizacionRowDoble("cantidadDivVisualizar", "visualizar_cantidad", "cantidadDeCamionesDivVisualizar", "visualizar_cantidadDeCamiones");
    visualizacionRowDoble("tipoDivVisualizar", "visualizar_tipo", "destinoDivVisualizar", "visualizar_destino");
    visualizacionRowDoble("mercsDepositoDivVisualizar", "visualizar_mercsDeposito", "mercsDepositoDivVisualizar", "cantidadDeposito");
    if (mercsFijacion == "true") {
        $(".deposito").show();
        $("#mercsDepositoDivVisualizar").show();
    } else {
        $(".deposito").hide();
        $("#mercsDepositoDivVisualizar").hide();
    }
    visualizacionRowDoble("pagoDivVisualizar", "visualizar_pago");
    visualizacionRowDoble("boletoDivVisualizar", "visualizar_boleto", "bolsaDivVisualizar", "visualizar_bolsa");
    visualizacionRowDoble("pesificadoDiasDivVisualizar", "visualizar_pesificadoDias", "informaSIODivVisualizar", "visualizar_informaSIO");
    visualizacionRowDoble("dolarizadoFechaDivVisualizar", "visualizar_dolarizadoFecha", "dolarizadoFechaOriginalDivVisualizar", "visualizar_dolarizadoOriginalFecha");
    visualizacionRowDoble("sustentableDivVisualizar", "visualizar_sustentablePrecio");
    visualizacionRowDoble("planCanjeDivVisualizar", "visualizar_planCanje", "establecimientoDivVisualizar", "visualizar_establecimiento");

    if (!sustentablePrecio === "undefined" || !sustentablePrecio === "null" || !sustentablePrecio === "false") {
        $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + (sustentableMoneda != "undefined" && sustentableMoneda != "null" ? sustentableMoneda : ""));
    }


    var datos;
    console.log(tipo, contrato, id);
    if (tipo == "AGENTE DE COMPRAS" || tipo == "FASON" || tipo == "FIJACION" || tipo == "FIJACION CANJE") {
        datos = MSExecuteOnServer('/CompraNet/TraerCalidadesPorContrato', { contratoId: contrato, acuerdoId: id });
    } else if (tipo == "CONTRATO ACUERDO" || tipo == 'ACUERDO AGENTE') {
        datos = MSExecuteOnServer('/CompraNet/TraerDatosDeContratoAcuerdo', { contratoId: id });

    } else if (tipo != 'FIJACION VIRTUAL') {
        datos = MSExecuteOnServer('/CompraNet/TraerDatosDeContrato', { contratoId: id });

    } else {
        $("#procedenciaDivVisualizar").hide();
        $("#tipoCalidadDiv").hide();
        $("#clasificacionDivVisualizar").hide();

    }
    var iteracionesDescuentos = viewModel.DescuentosVisualizar.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.DescuentosVisualizar.pop();
    }
    if (datos != null && datos != undefined) {
        var descuentosDto = datos.DescuentosBonificaciones;
        $.each(descuentosDto, function (key, descuento) {
            var descuentoKendo = {
                Id: descuento.Id,
                TipoPeriodoDBDesc: descuento.TipoPeriodoDBDesc,
                TipoPeriodoDBId: descuento.TipoPeriodoDBId,
                TipoDBDesc: descuento.TipoDBDesc,
                TipoDBId: descuento.TipoDBId,
                FechaDesde: descuento.FechaDesde,
                FechaHasta: descuento.FechaHasta,
                Importe: descuento.Importe,
                MonedaId: descuento.MonedaId,
                Porcentaje: descuento.Porcentaje,
                ContratoId: descuento.contratoId
            };
            viewModel.DescuentosVisualizar.push(descuentoKendo);
        });

        var iteracionesCalidades = viewModel.CalidadesVisualizar.length;
        for (var j = 0; j < iteracionesCalidades; j++) {
            viewModel.CalidadesVisualizar.pop();
        }

        var calidadesDto = datos.Calidades;
        $("#visualizar_calidad").text(standardDeCalidadDescripcion);

        $.each(calidadesDto, function (key, calidad) {
            var calidadKendo = {
                Id: calidad.Id,
                CalidadEspecialId: calidad.CalidadEspecialId,
                CalidadEspecialDesc: calidad.CalidadEspecialDesc,
                Valor: calidad.Valor,
                ContratoId: calidad.ContratoId,
                PorcentajeDesde: calidad.PorcentajeDesde,
                PorcentajeHasta: calidad.PorcentajeHasta,
                StandardDeCalidadId: 2
            };
            viewModel.CalidadesVisualizar.push(calidadKendo);
        });
        if (calidadesDto != null && calidadesDto.length > 0) {
            $("#calidadesDivVisualizar").show();
        } else {
            $("#calidadesDivVisualizar").hide();

        }
        var iteracionesPrecio = viewModel.PreciosVisualizar.length;
        for (var k = 0; k < iteracionesPrecio; k++) {
            viewModel.PreciosVisualizar.pop();
        }
        var preciosDto = datos.Precios;
        if (preciosDto != null && preciosDto.length > 0)
            $("#pactadosDivVisualizar").show();
        else
            $("#pactadosDivVisualizar").hide();

        $.each(preciosDto, function (key, precio) {
            var precioKendo = {
                Id: precio.Id,
                FechaDesde: precio.FechaDesde,
                FechaHasta: precio.FechaHasta,
                Precio: kendo.toString(precio.Precio ? Number(precio.Precio) : "", "n2"),
                MonedaPactadoId: precio.MonedaPactadoId,
                MonedaPactadoDesc: precio.MonedaPactadoDesc,
                ImportePactado: kendo.toString(precio.ImportePactado ? Number(precio.ImportePactado) : "", "n2"),
                MonedaImportePactadoId: precio.MonedaImportePactadoId != null ? precio.MonedaImportePactadoId : "",
                MonedaImportePactadoDesc: precio.MonedaImportePactadoDesc != null ? precio.MonedaImportePactadoDesc : "",
                Porcentaje: precio.Porcentaje != null ? precio.Porcentaje : ""
            };
            viewModel.PreciosVisualizar.push(precioKendo);
        });
    } else {
        $("#pactadosDivVisualizar").hide();
        $("#calidadesDivVisualizar").hide();
    }
    if (standardDeCalidadDescripcion == "null") $("#tipoCalidadDiv").hide();
    if (standardDeCalidadDescripcion == "Grado 2" || standardDeCalidadDescripcion == "Bonif. SECO de 7% a 10% Por punto") $("#calidadesDivVisualizar").hide();

    if (zona !== "undefined" && zona !== "") {
        $("#visualizar-zona-girasol").text(zona);
    } else {
        $("#visualizar-zona-girasol").text("");
        $("#visualizar_aperturaFinancieroPrecioNeto").text(kendo.toString(parseFloat(precioNeto), "n2") + " " + moneda);
    }
    $("#visualizar_aperturaFinanciero").text(null);
    $("#visualizar_aperturaRedespacho").text(null);
    $("#visualizar_aperturaComisiones").text(null);
    $("#visualizar_aperturaBonificaciones").text(null);
    $("#aperturaFinancieroDivVisualizar").hide();
    $("#aperturaRedespachoDivVisualizar").hide();
    $("#aperturaComisionesDivVisualizar").hide();
    $("#aperturaBonificacionesDivVisualizar").hide();
    $("#aperturaDePrecioVisualizarDiv").hide();
    $("#aperturaDePrecioVisualizarDivPrecioNeto").hide();
    $("#aperturaBasisDivVisualizar").hide();
    $("#divPosicionCBOT").hide();

    if (tipo === "A FIJAR") {
        $(".rangos").show();
        $("#maximoId").text(maximo);
        $("#minimoId").text(minimo);
    } else {
        $(".rangos").hide();
    }

    if (precioNeto != 0 || tipo === "A FIJAR" || tipo === "FIJACION" || tipo === "A PRECIO") {
        if (parseFloat(precioNeto) > 0) {
            $("#visualizar_aperturaFinancieroPrecioNeto").text(kendo.toString(parseFloat(precioNeto), "n2") + " " + moneda);
            $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
        } else {
            $("#aperturaDePrecioVisualizarDivPrecioNeto").hide();
        }

        var aperturaPrecio = MSExecuteOnServer('/CompraNet/TraerAperturaPrecioPorContrato', { contratoId: id, tipo: tipo });
        $.each(aperturaPrecio, function (key, concepto) {
            var moneda = concepto.Moneda == null ? "" : concepto.Moneda;
            switch (concepto.ConceptoAperturaPrecioId) {
                case 1:
                    if (concepto.Importe) {
                        $("#aperturaFinancieroDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        //$("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        $("#visualizar_aperturaFinanciero").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                    }
                    break;
                case 2:
                    if (concepto.Importe) {
                        $("#aperturaRedespachoDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        //$("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        $("#visualizar_aperturaRedespacho").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                    }
                    break;
                case 3:
                    if (concepto.Importe || concepto.Porcentaje) {
                        $("#aperturaComisionesDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        //$("#aperturaDePrecioVisualizarDivPrecioNeto").show();

                        $("#visualizar_aperturaComisiones").text(concepto.Importe ? kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda : concepto.Porcentaje + "%");
                    }
                    break;
                case 4:
                    if (concepto.Importe || concepto.Porcentaje) {
                        $("#visualizar_aperturaBonificaciones").text((concepto.Importe ? (kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda) : "") + (concepto.Porcentaje ? (" - " + concepto.Porcentaje + "%") : ""));
                        if (precioNeto > 0) {
                            if (tipo == "A PRECIO") {
                                $("#condicional").text("Condicional: ");
                            } else {
                                $("#condicional").text("Bonificacion: ");
                            }
                            //$("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        }

                        break;
                    }
                case 5:
                    if (concepto.Importe) {
                        $("#visualizar_aperturaBasis").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                        //visualizacionRowSimple("aperturaBasisDivVisualizar", "visualizar_aperturaBasis");
                        $("#aperturaBasisDivVisualizar").show();
                        $("#visualizar_PosicionCBOT").text(TipoPosicionCBOT + " - " + PosicionCBOT);
                    }

                    break;
            }
            if (PosicionCBOT == "" || PosicionCBOT == "null" || PosicionCBOT == undefined || PosicionCBOT == null) {
                $("#divPosicionCBOT").hide();
            } else {
                $("#divPosicionCBOT").show();
            }
            visualizacionRowDoble("aperturaFinancieroDivVisualizar", "visualizar_aperturaFinanciero", "aperturaRedespachoDivVisualizar", "visualizar_aperturaRedespacho");
            visualizacionRowDoble("aperturaComisionesDivVisualizar", "visualizar_aperturaComisiones", "aperturaBonificacionesDivVisualizar", "visualizar_aperturaBonificaciones");

        });
        if (tipo === "A FIJAR") {
            $("#aperturaDePrecioVisualizarDivPrecioNeto").hide();
        }

    }
    if (tipo === "A FIJAR PASE") {
        $("#visualizar_PosicionCBOT").text(PosicionCBOT);
        $("#divPosicionCBOT").show();
    }
    if ((nivelTarifa == "" || nivelTarifa == "null") && tarifaFlete == "null") {
        $("#fleteDivVisualizar").hide();
    } else {
        $("#fleteDivVisualizar").show();
        $("#visualizar_nivelFlete").text(nivelTarifa);
        $("#visualizar_tarifaFlete").text(tarifaFlete);
        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
    }
    compensacion === "true" || compensacion === true ? $("#compensacionVisualizar").show() : $("#compensacionVisualizar").hide();
    if (rechazo != "null") {
        $(".rechazo").show();
        $("#visualizar_rechazo").text(rechazo);
    } else {
        $(".rechazo").hide();
        $("#visualizar_rechazo").text("");
    }
    if (ObservacionTercero == "" || ObservacionTercero == "null") {
        ObservacionTercero = null;
    }

    if (SustentableTercero == "true" || CalidadTercero == "true" || DolarizadoTercero == "true" || PagoDiferidoTercero == "true" || ObservacionTercero != null) {
        var p = ObservacionTercero.split("|");

        $("#visualizar_observacionTercero").text(p[0]);
        var datosTercero = "";
        if (CalidadTercero == "true") {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Calidad:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Calidad: </strong><span> Si ' + n + '</span><br>';
        }
        if (DolarizadoTercero == "true") {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Dolarizado:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Dolarizado: </strong><span> Si ' + n + '</span><br>';
        }
        if (SustentableTercero == "true") {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Sustentable:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Sustentable: </strong><span> Si ' + n + '</span><br>';
        }
        if (PagoDiferidoTercero == "true") {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Pago Diferido:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Pago Diferido: </strong><span> Si ' + n + '</span><br>';
        }
        $("#visualizar_datosTercero").html(datosTercero);
    }

    if (ProveedorCreador != "" && ProveedorCreador != "null") {
        $("#datosCargaTercero").show();
    } else {
        $("#datosCargaTercero").hide();
    }

    if (Canje == "true") {
        $("#canjeDiv").show();
        $("#canjeId").text("Si");
        if (tipo === "A FIJAR") {
            $("#insumoDiv").show();
            $("#insumoId").text(Insumo);
            $("#montoDiv").show();
            $("#montoId").text(kendo.toString(parseFloat(Monto), "n2"));
            $("#monedaCanjeDiv").show();
            if (MonedaCanje == "USDM ") {
                $("#monedaCanjeId").text(MonedaCanje.slice(0, -2));
            } else {
                $("#monedaCanjeId").text(MonedaCanje);
            }
        }
    } else {
        $("#canjeDiv").hide();
        $("#insumoDiv").hide();
        $("#montoDiv").hide();
        $("#monedaCanjeDiv").hide();
    }

    if (obligatoriedad == "true") {
        $("#obligatorioDiv").show();
        $("#obligatorioId").text("Si");       
    } else {
        if (fechaCierta != "null") {
            if (obligatoriedad == "false") {
                $("#obligatorioDiv").show();
                $("#obligatorioId").text("No");
            }
        }
    }
    if (obligatoriedad == "null") {
        $("#obligatorioDiv").hide();
        $("#obligatorioId").text("");
    }

    if (obligatoriedadBond == "Si") {
        $("#obligatorioBondDiv").show();
        $("#obligatorioBondId").text(obligatoriedadBond);
    } else {
        if (obligatoriedadBond == "No") {
            $("#obligatorioBondDiv").show();
            $("#obligatorioBondId").text(obligatoriedadBond);
        }
    }
    if (obligatoriedadBond == "") {
        $("#obligatorioBondDiv").hide();
        $("#obligatorioBondId").text("");
    }


    if (Prestamo == "true") {
        $("#prestamoDiv").show();
        $("#prestamoId").text("Si");
        $("#plantaDestinoDiv").show();
        $("#plantaDestinoId").text(PlantaDestino);
    } else {
        $("#prestamoDiv").hide();
        $("#plantaDestinoDiv").hide();
    }

    if (Venta == 'true') {
        if (mailVenta != null && mailVenta != "") {
            $("#ventaBoletoDiv").show();
            $("#mailVentaId").text(mailVenta);
        } else {
            $("#ventaBoletoDiv").hide();
        }
    }
    if (Cesion == "true") {
        $("#CesionDiv").show();
        $("#CesionId").text("Si");
    } else {
        $("#CesionDiv").hide();
    }
    if (AnulaYReemplazaContratoSAP != "" && AnulaYReemplazaContratoSAP != 'null' && MotivoReemplazo != "" && MotivoReemplazo != 'null') {
        $("#AnulaYReemplazaDiv").show();
        $("#AnulaYReemplazaId").text(AnulaYReemplazaContratoSAP);
        $("#MotivoReemplazoId").text(MotivoReemplazo);
    } else {
        $("#AnulaYReemplazaDiv").hide();
    }
    if (Condicional == "true") {
        $("#CondicionalDiv").show();
        $("#AnulaYReemplazaId").text("Si");
        $("#CondicionalPrecioId").text(kendo.toString(parseFloat(CondicionalPrecio), "n2") + " " + CondicionalMonedaId);
        $("#CondicionalCantidadId").text(kendo.toString(parseFloat(CondicionalCantidad), "n0") + " Kg");
        $("#CondicionalFechaFormateadoId").text(CondicionalFechaFormateado);
        $("#CondicionalPosicionId").text(CondicionalPosicion);

    } else {
        $("#CondicionalDiv").hide();
    }
    if (CondicionalContratoSAP != "" && CondicionalContratoSAP != 'null') {
        $("#visualizar_CondicionalDiv").show();
        $("#visualizar_Condicional").text(CondicionalContratoSAP);
    } else {
        $("#visualizar_CondicionalDiv").hide();
    }
    if (RazonsocialProveedorComisionista != null && RazonsocialProveedorComisionista != "") {
        $("#comisionistaProveedorDivVisualizar").show();
        $("#visualizar_comisionistaProveedor").text(RazonsocialProveedorComisionista);
    } else {
        $("#comisionistaProveedorDivVisualizar").hide();
    }


}

function visualizacionRowDoblePrecioCero(div1, div2, aFijar) {
    if (aFijar) {
        $("#" + div1).hide();
        $("#" + div2).show();
        $("#" + div2).removeClass("col-sm-6");
        $("#" + div2).addClass("col-sm-12");
    } else {
        $("#" + div1).show();
        $("#" + div1).removeClass("col-sm-12");
        $("#" + div1).addClass("col-sm-6");
        $("#" + div2).show();
        $("#" + div2).removeClass("col-sm-12");
        $("#" + div2).addClass("col-sm-6");
    }
}


function visualizacionRowDoble(div1, span1, div2, span2) {
    if ($("#" + span1).text() === "null" && $("#" + span2).text() === "null") {
        $("#" + div1).hide();
        $("#" + div2).hide();
    } else if ($("#" + span1).text() === "null") {
        $("#" + div1).hide();
        $("#" + div2).show();
        $("#" + div2).removeClass("col-sm-6");
        $("#" + div2).addClass("col-sm-12");
    } else if ($("#" + span2).text() === "null") {
        $("#" + div2).hide();
        $("#" + div1).show();
        $("#" + div1).addClass("col-sm-12");
        $("#" + div1).removeClass("col-sm-6");
    } else {
        $("#" + div1).show();
        $("#" + div1).removeClass("col-sm-12");
        $("#" + div1).addClass("col-sm-6");
        $("#" + div2).show();
        $("#" + div2).removeClass("col-sm-12");
        $("#" + div2).addClass("col-sm-6");
    }
}

function visualizacionRowSimple(dato, idDiv, idText) {
    if (dato === "undefined" || dato === "null" || dato === "false" || dato === "") {
        $("#" + idDiv).hide();
    }
    else {
        $("#" + idDiv).show();
        $("#" + idText).text(dato);
    }
}

function ModalBorrar(proveedor, id, tipoNegocio, fijacionDePrecioContratoId, fasonId, agenteId, acuerdoId, estado) {

    var idAsociado = 0;
    if (tipoNegocio === "5") {
        idAsociado = agenteId
    } else {
        idAsociado = id;
    }
    var asociados = MSExecuteOnServer('/CompraNet/EsUnContratoAsociado', { negocioId: idAsociado });
    if (asociados == true) {
        MensErr("Los contratos asociados a un A Fijar Pase no pueden ser anulados")
    } else {
        $("#proveedor_a_borrar").text(proveedor);
        $("#proveedorBorrarDivVisualizar").show();
        $("#agenteBorrarDivVisualizar").hide();
        $("#estadoModalBorrar").val(estado);
        if (tipoNegocio === "3") {
            $("#contratoModalBorrar").val(fijacionDePrecioContratoId);
        } else if (tipoNegocio === "4") {
            $("#contratoModalBorrar").val(fasonId);
        } else if (tipoNegocio === "5") {
            $("#proveedorBorrarDivVisualizar").hide();
            $("#agenteBorrarDivVisualizar").show();
            $("#contratoModalBorrar").val(agenteId);
            $("#agente_a_borrar").text(agenteId);
        } else if (tipoNegocio === "6") {
            $("#contratoModalBorrar").val(acuerdoId);
        } else {
            $("#contratoModalBorrar").val(id);
        }

        $("#motivo-rechazo").val("");
        //if (estado == 9) {
        //    $("#motivo-rechazo").show();
        //} else {
        //    $("#motivo-rechazo").hide();
        //}
        $("#motivo-rechazo").show();
        $("#tipoNegocioModalBorrar").val(tipoNegocio);

        $("#modalBorrar").modal('show');

    }

}

function ModalBorrarPreAnulado(proveedor, id, tipoNegocio, fijacionDePrecioContratoId, fasonId, agenteId, acuerdoId, estado, Rechazo, kilos) {
    $("#proveedor_a_borrarPreanular").text(proveedor);
    $("#proveedorBorrarDivVisualizar").show();
    $("#agenteBorrarDivVisualizar").hide();
    $("#estadoModalBorrar").val(estado);
    $("#fijacionModalAnular").val(fijacionDePrecioContratoId);
    $("#tiponegocio_a_borrarPreanular").text("el Negocio");

    var kilosMsg = "";
    if (kilos && kilos > 0) {
        kilosMsg = " el saldo de " + kendo.toString(kilos, "n") + " kg de ";
    }
    if (tipoNegocio === "3") {
        $("#contratoModalBorrar").val(fijacionDePrecioContratoId);
        $("#tiponegocio_a_borrarPreanular").text(kilosMsg + "la fijacion");
    } else if (tipoNegocio === "4") {
        $("#contratoModalBorrar").val(fasonId);
        $("#tiponegocio_a_borrarPreanular").text("el Fason");
    } else if (tipoNegocio === "5") {
        $("#proveedorBorrarDivVisualizar").hide();
        $("#agenteBorrarDivVisualizar").show();
        $("#contratoModalBorrar").val(agenteId);
        $("#agente_a_borrar").text(agenteId);
        $("#tiponegocio_a_borrarPreanular").text("el Negocio de Agente de Compras");
    } else if (tipoNegocio === "6") {
        $("#contratoModalBorrar").val(acuerdoId);
        $("#tiponegocio_a_borrarPreanular").text("el Contrato Acuerdo");
    } else {
        $("#contratoModalBorrar").val(id);
    }

    $("#motivo-rechazoPreanular").val("");
    //if (estado == 9) {
    //    $("#motivo-rechazo").show();
    //} else {
    //    $("#motivo-rechazo").hide();
    //}
    //$("#motivo-rechazoPreanular").show();
    $("#tipoNegocioModalBorrar").val(tipoNegocio);
    if (Rechazo != "") {
        $("#div-motivo-rechazoSolictud").show();
        $("#motivo-rechazoSolictud").html(Rechazo);
    } else {
        $("#div-motivo-rechazoSolictud").hide();
    }
    $("#modalBorrarPreanulado").modal('show');
}

function ModalPreAnular(id, fijacionId, proveedor, tipoNegocio, kilos) {
    var asociados = MSExecuteOnServer('/CompraNet/EsUnContratoAsociado', { negocioId: id });
    if (asociados == true) {
        MensErr("Los contratos asociados a un a Fijar Pase no pueden ser anulados")
    } else {
        $("#proveedor_a_preanular").text(proveedor);
        $("#contratoModalAnular").val(id);
        $("#fijacionModalAnular").val(fijacionId);
        $("#tipoNegocioModalBorrar").val(tipoNegocio);
        $("#modalPreAnular").modal('show');
        $("#tiponegocio_a_Preanular").text("el Negocio");
        var kilosMsg = "";
        if (kilos && kilos > 0) {
            kilosMsg = " el saldo de " + kendo.toString(kilos, "n") + " kg de ";
        }
        if (tipoNegocio === "3") {
            $("#tiponegocio_a_Preanular").text(kilosMsg + "la Fijacion");
        } else if (tipoNegocio === "4") {
            $("#tiponegocio_a_Preanular").text("el Fason");
        } else if (tipoNegocio === "5") {
            $("#tiponegocio_a_Preanular").text("el Negocio de Agente de Compras");
        } else if (tipoNegocio === "6") {
            $("#tiponegocio_a_Preanular").text("el Contrato Acuerdo");
        }
    }

    //if (kilos && kilos > 0) {
    //    $("#espaciolineas").html("Usted está intentando preanular una <b>Fijación Virtual</b> de " + proveedor +
    //        " con " + kendo.toString(kilos, "n") + " kg de saldo sobre " + + " kg." +
    //        "<br>Si prosigue se hará una anulación parcial."+
    //        "<br>¿Desea proseguir?"
    //    );

    //}
}

function CrearViewModel() {
    var param = {
        "Descuentos": null,
        "ContratosPendientes": null
    };
    viewModel = kendo.observable({
        Parametros: param,
        DescuentosVisualizar: [],
        CalidadesVisualizar: [],
        ContratosPendientes: [],
        PreciosVisualizar: [],
        Servicios: []
    });

    kendo.bind($("#tabla-descuentos-visualizar"), viewModel);
    kendo.bind($("#tabla-calidades-visualizar"), viewModel);
    kendo.bind($("#tabla-precios-pactados"), viewModel);
    kendo.bind($("#tabla-pendientes"), viewModel);
}

function AvisoContratosPendientes() {
    var iteracionesContratos = viewModel.ContratosPendientes.length;
    for (var i = 0; i < iteracionesContratos; i++) {
        viewModel.ContratosPendientes.pop();
    }
    var contratosPendientes = MSExecuteOnServer('/CompraNet/TraerContratosPendientes');
    if (contratosPendientes.length > 0) {
        $("#contratos-pendientes").show();
    } else {
        $("#contratos-pendientes").hide();
    }
    $.each(contratosPendientes, function (key, contrato) {
        var contratoVM = {
            ContratoId: contrato.ContratoId,
            RazonSocial: contrato.RazonSocial,
            Cantidad: contrato.Cantidad,
            Precio: contrato.Precio,
            Moneda: contrato.Moneda != null ? contrato.Moneda : "",
            Fecha: contrato.Fecha,
            NombreApellido: contrato.NombreApellido
        };
        viewModel.ContratosPendientes.push(contratoVM);
    });
}


function InicializarBuscador() {
    var grupoDeComprasDatos = MSExecuteOnServer('/CompraNet/BuscarGrupoDeCompras');

    var GrupoCompraDescripcionDatos = new Array();
    for (var i = 0; i < grupoDeComprasDatos.length; i++) {
        if (grupoDeComprasDatos[i].Id != 47) {
            GrupoCompraDescripcionDatos.push({ Descripcion: grupoDeComprasDatos[i].Descripcion });
        }
    }
    $("#buscadorFiltroZona").kendoMultiSelect({
        autoClose: false,
        tagMode: "single",
        autoWidth: true,
        placeholder: "Buscar por Zona",
        dataTextField: "Descripcion",
        dataValueField: "Descripcion",
        dataSource: GrupoCompraDescripcionDatos,
        dataBound: function (e) {
            // handle the event
        },
        change: function () {
            filtrarZona();
        },
        messages: {
            singleTag: "item(s)",
        }
    });


    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
}


function InicializarPrecioMOA() {
    var precio = MSExecuteOnServer('/CompraNet/TraerPrecioMoa');
    $("#crearContrato").hide();
    var table = '<tr><th rowspan="2" class="col-xs-2">PRECIO MOA</th>';
    var retirados = true;
    for (var i = 0; i < precio.length; i++) {
        table += '<th class="col-xs-2">' + precio[i][0].Material + '</th>';
    }
    table += '</tr>';
    for (i = 0; i < precio.length; i++) {
        table += '<td>';
        var matRetirado = true;
        if (precio[i][0].Retirado == false ||
            precio[i][1].Retirado == false ||
            precio[i][0].Pizarra == true) {
            retirados = false;
            matRetirado = false;
        }
        if (matRetirado == true) {
            table += '<span class="retirado">Sin precio</span>';
        } else {
            table += precio[i][0].Precio > 0 ? '<span class="precio">' + kendo.toString(precio[i][0].Precio, "n") + ' ' + precio[i][0].MonedaId + '</span><br/>' : '';
            table += precio[i][1].Precio > 0 ? '<span class="precio">' + kendo.toString(precio[i][1].Precio, "n") + ' ' + precio[i][1].MonedaId + '</span><br/>' : '';
            table += precio[i][0].Pizarra == true ? '<span class="precio"> Pizarra </span><br/>' : '';
        }
        table += '</td>';

    }
    if (retirados == false) {
        $("#crearContrato").show();
    }

    $("#tabla-precio-moa").html(table);
}

function SetearPrecioMoa() {
    precioMoa = MSExecuteOnServer('/CompraNet/TraerPrecioMoa', { tipoNegocioId: 0 });
}

function ArmarPrecio(dataItem) {
    if (dataItem.TipoNegocioId !== 1) {
        if (!externo /*&& dataItem.TipoNegocioId === 3*/ && dataItem.Estado === 9) {
            var esPrecioMoa = false;
            for (i = 0; i < precioMoa.length; i++) {
                var p = precioMoa[i].filter(function (e) {
                    return (e.TipoNegocioId === dataItem.TipoNegocioId &&
                        e.MaterialId === dataItem.MaterialId && (e.DestinoId == dataItem.DestinoId || e.DestinoId == null) &&
                        (e.Pizarra == true || e.MonedaId === dataItem.Moneda));
                });

                if (p) {
                    for (var y = 0; y < p.length; y++) {
                        if (esPrecioMoa === false) {
                            esPrecioMoa = p[y].Precio == dataItem.PrecioPlazo || (p[y].Pizarra == true && dataItem.Precio == 0);
                        }
                    }
                }
            }

            if (esPrecioMoa === true) {
                return FormatearString(dataItem.PrecioPlazo, dataItem.Moneda);
            } else {
                return '<strong style="color:red;">' + FormatearString(dataItem.PrecioPlazo, dataItem.Moneda) + '</strong>';
            }
        }
        return FormatearString(dataItem.PrecioPlazo, dataItem.Moneda);
    } else {
        var fecha = kendo.toString(dataItem.HastaFijacion, "dd/MM/yyyy");
        if (dataItem.TipoNegocioId === 1 || dataItem.TipoNegocioId === 2) {
            if (!externo && dataItem.Estado === 9) {
                var esPrecioMoa = false;
                for (i = 0; i < precioMoa.length; i++) {
                    var p = precioMoa[i].filter(function (e) {
                        return e.TipoNegocioId === dataItem.TipoNegocioId &&
                            e.MaterialId === dataItem.MaterialId && (e.DestinoId == dataItem.DestinoId || e.DestinoId == null) &&
                            ((e.Pizarra == true || e.MonedaId === dataItem.Moneda) || e.TipoNegocioId === 1);
                    });

                    if (p) {
                        for (var y = 0; y < p.length; y++) {
                            if (esPrecioMoa === false && p[y].HastaFijacion != null) {
                                esPrecioMoa = kendo.toString(new Date(parseInt(p[y].HastaFijacion.substr(6))), "dd/MM/yyyy") == kendo.toString(dataItem.HastaFijacion, "dd/MM/yyyy");
                            }
                        }
                    }
                }
                if (esPrecioMoa === true) {
                    return fecha;
                } else {
                    return '<strong style="color:red;">' + fecha + '</strong>';
                }
            }
            return fecha;
        } else {
            return dataItem.PrecioPlazo;
        }
    }
}
function ArmarFechaDesde(dataItem) {
    var fecha = kendo.toString(dataItem.FechaDesde, "dd/MM/yyyy");
    if (dataItem.TipoNegocioId === 1 || dataItem.TipoNegocioId === 2) {
        if (!externo && dataItem.Estado === 9) {
            var esPrecioMoa = false;
            for (i = 0; i < precioMoa.length; i++) {
                var p = precioMoa[i].filter(function (e) {
                    return e.TipoNegocioId === dataItem.TipoNegocioId && e.MaterialId === dataItem.MaterialId &&
                        (e.DestinoId == dataItem.DestinoId || e.DestinoId == null) && ((e.Pizarra == true || e.MonedaId === dataItem.Moneda) || e.TipoNegocioId === 1);
                });

                if (p) {
                    for (var y = 0; y < p.length; y++) {
                        if (esPrecioMoa === false && p[y].DesdeEntrega != null) {
                            esPrecioMoa = kendo.toString(new Date(parseInt(p[y].DesdeEntrega.substr(6))), "dd/MM/yyyy") == kendo.toString(dataItem.FechaDesde, "dd/MM/yyyy");
                        }
                    }
                }
            }
            if (esPrecioMoa === true) {
                return fecha;
            } else {
                return '<strong style="color:red;">' + fecha + '</strong>';
            }
        }
        return fecha;
    } else {
        return fecha;
    }
}
function ArmarFechaHasta(dataItem) {
    var fecha = kendo.toString(dataItem.FechaHasta, "dd/MM/yyyy");
    if (dataItem.TipoNegocioId === 1 || dataItem.TipoNegocioId === 2) {
        if (!externo && dataItem.Estado === 9) {
            var esPrecioMoa = false;
            for (i = 0; i < precioMoa.length; i++) {
                var p = precioMoa[i].filter(function (e) {
                    return e.TipoNegocioId === dataItem.TipoNegocioId && e.MaterialId === dataItem.MaterialId
                        && (e.DestinoId == dataItem.DestinoId || e.DestinoId == null) && ((e.Pizarra == true || e.MonedaId === dataItem.Moneda) || e.TipoNegocioId === 1);
                });

                if (p) {
                    for (var y = 0; y < p.length; y++) {
                        if (esPrecioMoa === false && p[y].HastaEntrega != null) {
                            esPrecioMoa = kendo.toString(new Date(parseInt(p[y].HastaEntrega.substr(6))), "dd/MM/yyyy") == kendo.toString(dataItem.FechaHasta, "dd/MM/yyyy");
                        }
                    }
                }
            }
            if (esPrecioMoa === true) {
                return fecha;
            } else {
                return '<strong style="color:red;">' + fecha + '</strong>';
            }
        }
        return fecha;
    } else {
        return fecha;
    }
}
function ModificoPrecio(dataItem) {
    if (dataItem.TipoNegocioId !== 1) {
        if (dataItem.ComercialCreadorId == null) {
            var esPrecioMoa = false;
            var pPrecio = "";
            for (i = 0; i < precioMoa.length; i++) {
                var p = precioMoa[i].filter(function (e) {
                    return e.TipoNegocioId === dataItem.TipoNegocioId && e.MaterialId === dataItem.MaterialId &&
                        (e.DestinoId == dataItem.DestinoId || e.DestinoId == null) && (e.Pizarra == true || e.MonedaId === dataItem.Moneda);
                });
                if (p) {
                    for (var y = 0; y < p.length; y++) {
                        esPrecioMoa = p[y].Precio == dataItem.PrecioPlazo || (p[y].Pizarra == true && dataItem.Precio == 0);
                    }
                }
            }
            if (!esPrecioMoa) {
                return "El precio es diferente al publicado, <br> Precio Cargado: " + dataItem.PrecioPlazo + "<br> Precio MOA: " + pPrecio;
            }
        }
        return "";
    } else {
        return "";
    }
}

function mostrarDetalles() {
    $("#mostrar").show();
    $("#ocultar").hide();
    $("#datosContrato").show();
    $("#tablaModificacion").hide();
}
function compararReconfirmacion() {
    var id = $("#contrato").text();
    $("#mostrar").hide();
    $("#ocultar").show();
    $("#tablaModificacion").show();

    $("#datosContrato").hide();
    var result = MSExecuteOnServer('/CompraNet/CompararNegocioReconfirmado', { contratoId: id });
    var contratoSave = result[0];
    var contrato = result[1];
    var table = "<tr>";
    table += "<th>Original</th>"
    table += "<th>Modificado</th>"
    table += "</tr>";

    table += "<tr>";
    table += '<td>';
    table += '<span> PRECIO: ' + kendo.toString(contrato.Precio, "n") + "  " + contrato.MonedaId + '</span><br/>';
    table += '</td>';
    table += '<td>';
    table += '<span> PRECIO: ' + (contrato.Precio != contratoSave.Precio ? "<strong>" + kendo.toString(contratoSave.Precio, "n") + "  " + contratoSave.MonedaId + "</strong>" : kendo.toString(contratoSave.Precio, "n") + "  " + contrato.MonedaId) + '</span><br/>';
    table += '</td>';
    table += "</tr>";
    table += "<tr>";
    table += '<td>';
    table += '<span> CANTIDAD (Kg): ' + kendo.toString(contrato.Cantidad, "n0") + '</span><br/>';
    table += '</td>';
    table += '<td>';
    table += '<span> CANTIDAD (Kg): ' + (contrato.Cantidad != contratoSave.Cantidad ? "<strong>" + kendo.toString(contratoSave.Cantidad, "n0") + "</strong>" : kendo.toString(contratoSave.Cantidad, "n0")) + '</span><br/>';
    table += '</td>';
    table += "</tr>";
    if (contrato.TipoNegocioId == 1) {
        table += "<tr>";
        table += "<td>";
        table += '<span> DESDE FIJACION: ' + kendo.toString(contrato.DesdeFijacionFormateado, "dd-MM-yyyy") + '</span><br/>';
        table += '</td>';
        table += '<td>';
        table += '<span> DESDE FIJACION: ' + (contrato.DesdeFijacion != contratoSave.DesdeFijacion ? "<strong>" + kendo.toString(contratoSave.DesdeFijacionFormateado, "dd/MM/yyyy") + "</strong>" : kendo.toString(contratoSave.DesdeFijacionFormateado, "dd/MM/yyyy")) + '</span><br/>';
        table += '</td>';
        table += "</tr>";
        table += "<tr>";
        table += '<td>';
        table += '<span> HASTA FIJACION: ' + kendo.toString(contrato.HastaFijacionFormateado, "dd-MM-yyyy") + '</span><br/>';
        table += '</td>';
        table += '<td>';
        table += '<span> HASTA FIJACION: ' + (contrato.HastaFijacion != contratoSave.HastaFijacion ? "<strong>" + kendo.toString(contratoSave.HastaFijacionFormateado, "dd/MM/yyyy") + "</strong>" : kendo.toString(contratoSave.HastaFijacionFormateado, "dd/MM/yyyy")) + '</span><br/>';
        table += '</td>';
        table += "</tr>";
    }

    var result = MSExecuteOnServer('/CompraNet/ValidarCalidades', { contratoId: id });


    table += "<tr>";

    if (contrato.StandardCalidadId == 7) {
        table += '<td>';
        table += '<span> CALIDAD: GRADO 2 </span><br/>';
        table += '</td>';
    }
    else if (contrato.TrigoEspecial == true) {
        table += '<td>';
        table += '<span> CALIDAD: ESPECIAL </span><br/>';
        table += '</td>';
    }
    else if (contrato.StandardCalidadId == 1 || contrato.StandardCalidadId == 4 || contrato.StandardCalidadId == 5) {
        table += '<td>';
        table += '<span> CALIDAD: C&AacuteMARA </span><br/>';
        table += '</td>';
    }
    else if (contrato.StandardCalidadId == 3) {
        table += '<td>';
        table += '<span> CALIDAD: F&AacuteBRICA </span><br/>';
        table += '</td>';
    }
    if (contrato.Calidades.length > 0 && contrato.StandardCalidadId != 7) {
        table += '<td>';
        for (var cal = 0; cal < contrato.Calidades.length; cal++) {
            table += contrato.Calidades[cal].CalidadEspecialDesc + " " + contrato.Calidades[cal].Valor + "<br />";
            if (contrato.Calidades[cal].PorcentajeDesde != null && contrato.Calidades[cal].PorcentajeHasta != null) {
                table += "Porc. Desde " + contrato.Calidades[cal].PorcentajeDesde + "% Hasta " + contrato.Calidades[cal].PorcentajeHasta + "%<br />";
            }
            table += '</td>';
        }
    }


    if (contratoSave.StandardCalidadId == 7) {
        table += '<td>';
        table += '<span>' + (result ? "<strong>CALIDAD: GRADO 2 </strong>" : "CALIDAD: GRADO 2") + "</span><br/>";
        table += '</td>';
    }
    else if (contratoSave.TrigoEspecial == true) {
        table += '<td>';
        table += '<span>' + (result ? "<strong>CALIDAD: ESPECIAL  </strong>" : "CALIDAD: ESPECIAL ") + "</span><br/>";
        table += '</td>';
    }
    else if (contratoSave.StandardCalidadId == 1 || contratoSave.StandardCalidadId == 4 || contratoSave.StandardCalidadId == 5) {
        table += '<td>';
        table += '<span>' + (result ? "<strong>CALIDAD: C&AacuteMARA</strong>" : "CALIDAD: C&AacuteMARA") + "</span><br/>";
        table += '</td>';
    }
    else if (contratoSave.StandardCalidadId == 3) {
        table += '<td>';
        table += '<span>' + (result ? "<strong>CALIDAD: F&AacuteBRICA</strong>  </span><br/>" : "CALIDAD: F&AacuteBRICA") + "</span><br/>";
        table += '</td>';
    }
    if (contratoSave.Calidades.length > 0 && contratoSave.StandardCalidadId != 7) {
        table += '<td>';
        for (var cal = 0; cal < contratoSave.Calidades.length; cal++) {

            table += result ? "<strong>" + contratoSave.Calidades[cal].CalidadEspecialDesc + " " + contratoSave.Calidades[cal].Valor + " <strong><br />" : contratoSave.Calidades[cal].CalidadEspecialDesc + " " + contratoSave.Calidades[cal].Valor + "<br />";
            if (contratoSave.Calidades[cal].PorcentajeDesde != null && contratoSave.Calidades[cal].PorcentajeHasta != null) {
                table += result ? "<strong> Porc.Desde " + contratoSave.Calidades[cal].PorcentajeDesde + " % Hasta " + contratoSave.Calidades[cal].PorcentajeHasta + " % </strong><br />" :
                    "Porc. Desde " + contratoSave.Calidades[cal].PorcentajeDesde + "% Hasta " + contratoSave.Calidades[cal].PorcentajeHasta + "%<br />";
            }

        }
        table += '</td>';
    }
    var servicios = [];
    var modificado = [];
    var largoSave = contratoSave.Servicios.length;
    var largoContrato = contrato.Servicios.length;
    if ((largoContrato == 0 && largoSave > 0) || (largoSave == 0 && largoContrato > 0)) {
        servicios = largoSave > 0 ? contratoSave.Servicios : contrato.Servicios;
    } else {
        modificado = contratoSave.Servicios.filter(function (x) { return x.Modificado == true });
        servicios = contratoSave.Servicios;
    }
    ArmarDescripcionServicio(servicios);
    if (modificado != null || (servicios != null && servicios.length > 0)) {
        table += "<tr>";
        table += '<td colspan="2"> Descripcion de Servicios y Calidades';
        table += '</td>';
        table += "</tr>";
        var largoLista = 0;

        largoLista = Math.max(largoSave, largoContrato);
        for (var i = 0; i < largoLista; i++) {
            table += "<tr>";
            table += '<td>';
            if (largoContrato > 0) {
                table += '<span>' + servicios[i].DescripcionServicio + " " + kendo.toString(contrato.Servicios[i].Importe, "n2") + "  " + contrato.Servicios[i].MonedaDescripcion + '</span><br/>';
            }
            table += '</td>';
            table += '<td>';
            if (largoSave > 0) {
                if (contratoSave.Servicios[i].Modificado == true) {
                    table += '<strong>' + contratoSave.Servicios[i].DescripcionServicio + " " + kendo.toString(contratoSave.Servicios[i].Importe, "n2") + "  " + contratoSave.Servicios[i].MonedaDescripcion + '</strong><br/>';
                } else {
                    table += '<span>' + contratoSave.Servicios[i].DescripcionServicio + " " + kendo.toString(contratoSave.Servicios[i].Importe, "n2") + "  " + contratoSave.Servicios[i].MonedaDescripcion + '</span><br/>';
                }
            }
            table += '</td>';
            table += "</tr>";
        }
        //    for (var i = 0; i < largoLista; i++) {
        //        table += '<tr>';
        //        table += '<td>';
        //        if (contratoSave.Servicios.length > contrato.Servicios.length) {
        //            if (contratoSave.Servicios[i].Modificado == true) {
        //                table += '<strong>' + contratoSave.Servicios[i].DescripcionServicio + " " + kendo.toString(contratoSave.Servicios[i].Importe, "n2") + "  " + contratoSave.Servicios[i].MonedaDescripcion + '</strong><br/>';
        //            } else {
        //                table += '<span>' + contratoSave.Servicios[i].DescripcionServicio + " " + kendo.toString(contratoSave.Servicios[i].Importe, "n2") + "  " + contratoSave.Servicios[i].MonedaDescripcion + '</span><br/>';

        //            }
        //        } else {
        //            table += '<span>' + servicios[i].DescripcionServicio + " " + kendo.toString(contrato.Servicios[i].Importe, "n2") + "  " + contrato.Servicios[i].MonedaDescripcion + '</span><br/>';

        //        }
        //        table += '</td>';
        //        table += '</tr>';
        //}
    }

    $("#tablaComparar").html(table);
    $("#tablaComparar").show();

}

function setPageSize() {
    var grid = $("#gridInformeCompraNet").data("kendoGrid");
    grid.dataSource.pageSize($("#pageSize").val());
    grid.refresh();
}

function imageToBlob(imageURL) {
    const img = new Image;
    const c = document.createElement("canvas");
    const ctx = c.getContext("2d");
    img.crossOrigin = "";
    img.src = imageURL;
    return new Promise(resolve => {
        img.onload = function () {
            c.width = this.naturalWidth;
            c.height = this.naturalHeight;
            ctx.drawImage(this, 0, 0);
            c.toBlob((blob) => {
                // here the image is a blob
                resolve(blob)
            }, "image/png", 0.75);
        };
    })
}

async function copyImage(imageURL) {
    const blob = await imageToBlob(imageURL)
    const item = new ClipboardItem({ "image/png": blob });
    navigator.clipboard.write([item]);
}

function copiarImagen() {
    BlockUi('Copiando...');
    setTimeout(function () {
        $("#lineModalLabelCopiar").show();
        $(".noCopiarDatos").hide();
        $(".status").addClass("anchoEstado");
        $(".datosContrato").addClass("noPadding");
        $("#DivVisualizar").removeClass("tamanioModal");

        html2canvas($(".datosContrato")[0]).then(function (canvas) {
            let image = new Image();
            image.src = canvas.toDataURL();
            $("#out_image").append(image);
            copyImage(image.src);
            $("#out_image").empty();
        });
        $("#lineModalLabelCopiar").hide();
        $(".noCopiarDatos").show();
        $(".status").removeClass("anchoEstado");
        $(".datosContrato").removeClass("noPadding");
        $("#DivVisualizar").addClass("tamanioModal");
        $.unblockUI();
    }, 200);
}


function BuscarDatosServicio(id) {
    var contrato = {
        Id: id,
        Tipo: ""
    };
    var url = '/Compranet/TraerContratoCompleto';
    var data = contrato;
    var result = MSExecuteOnServer(url, data);
    return result;
}

function ArmarServicio(id) {
    var result = BuscarDatosServicio(id)
    ArmarDescripcionServicio(result.Servicios);
    viewModel.set("Servicios", result.Servicios);
    kendo.bind($("#modalVisualizar"), viewModel);
    var modificaciones = viewModel.Servicios.filter(function (x) { return x.Modificado == true });
    var modificado = viewModel.Servicios != null ? (modificaciones != null && modificaciones.length > 0) ? true : false : false;
    return modificado;
}

function ArmarDescripcionServicio(servicio) {
    for (var i = 0; i < servicio.length; i++) {
        servicio[i].Importe = kendo.toString(servicio[i].Importe, "n2");
        servicio[i].DescripcionServicio = servicio[i].Descripcion +
            ((servicio[i].Desde >= 0 && servicio[i].Hasta > 0) ? (" DE " + kendo.toString(servicio[i].Desde, "n2") + " A " + kendo.toString(servicio[i].Hasta, "n2") + "%") :
                (servicio[i].Desde > 0 && servicio[i].Hasta == 0) ? (" MAS DE " + kendo.toString(servicio[i].Desde, "n2") + "%") :
                    (servicio[i].Desde == 0 && servicio[i].Hasta == 0) ? "" : "");
    }
}
