

$(document).ready(function () {

    InicializarAutocompletar();

});


function InicializarAutocompletar() {


    $("#contratoACopiarId").click(function () {
        $("#contratoACopiarId").data("kendoAutoComplete").value("");
        $("#contratoACopiarId").data("kendoAutoComplete").trigger("change");
    });

    $("#contratoACopiarId").kendoAutoComplete({
        template: function (data) {
            if ($(window).width() > 768) {
                return '<p class="buscar-nomb">' + data.ContratoSap + ' - ' + data.Material + ' - ' + data.RazonSocial + ' - ' + data.Fecha + ' - ' + data.Cantidad + ' Kg. </p>';
            } else {
                return '<p class="buscar-nomb letra650">' + data.ContratoSap + ' - ' + data.Material + ' - ' + data.RazonSocial + ' - ' + data.Fecha + '</p>';
            }
        },
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "ContratoSap",
        dataValueField: "Id",
        autoWidth: true,
        select: function (e) {
            CargarCopiaContrato(e.dataItem.Id, "sap");
        },
        dataSource: {
            serverFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerContratosParaCopiar"
                },
                parameterMap: function (data, type) {
                    var valor = $("#contratoACopiarId").val();
                    return { filtro: valor };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });



    $("#contratoAcuerdoId").click(function () {
        $("#contratoAcuerdoId").data("kendoAutoComplete").value("");
        $("#contratoAcuerdoId").data("kendoAutoComplete").trigger("change");
    });

    $("#contratoAcuerdoId").kendoAutoComplete({
        template: function (data) {
            if ($(window).width() > 768) {
                return '<p class="buscar-nomb">' + data.Id + ' - ' + data.Material + ' - ' + data.RazonSocial + ' - ' + data.Fecha + ' - ' + data.Cantidad + ' Kg. </p>';
            } else {
                return '<p class="buscar-nomb letra650">' + data.Id + ' - ' + data.Material + ' - ' + data.RazonSocial + ' - ' + data.Fecha + ' - ' + data.Cantidad + ' Kg. </p>'
            }
        },
        minLength: 1,
        enforceMinLength: true,
        dataTextField: "Id",
        dataValueField: "Id",
        autoWidth: true,
        change: function () {

        },
        select: function (e) {
            CargarCopiaContrato(e.dataItem.Id, "acuerdo");
        },
        dataSource: {
            serverFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerContratosAcuerdo"
                },
                parameterMap: function (data, type) {
                    var valor = $("#contratoAcuerdoId").val();
                    return { filtro: valor };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
}

function CargarCopiaContrato(contratoId, tipo) {
    var datos = { id: contratoId, tipo: tipo };
    contratoCopia = MSExecuteOnServer('/CompraNet/TraerContratoCompleto', datos, function () { $.unblockUI(); });
    if (contratoCopia.HayError) {
        MensErr(contratoCopia.Errores[0].Message);
    } else {
        contratoCopia.Estado = 1;
        modificarContrato(contratoCopia);
        CargarDatosEditar(contratoCopia);
    }
}

function modificarContrato(contratoCopia) {
    contratoCopia.contratoId = 0;
    contratoCopia.ContratoSap = null;
}



function CargarDatosCopiar(contrato, hijo, tipo) {
    InicializarBordesRojos();

    $("#material").data("kendoDropDownList").value(contrato.MaterialId);
    $("#material").data("kendoDropDownList").trigger("change");
    if (tipo == "acuerdo") {
        CargarCampaniaPorMaterial($("#material").val());
        $("#contratoACopiarId").data("kendoAutoComplete").value("");
        $("#contratoACopiarId").data("kendoAutoComplete").trigger("change");
    } else {
        $("#campanaId").data("kendoDropDownList").value(contrato.CampanaId);
        $("#contratoAcuerdoId").data("kendoAutoComplete").value("");
        $("#contratoAcuerdoId").data("kendoAutoComplete").trigger("change");
    }
    $("#buscadorCorredor").val(contrato.Corredor);
    $("#buscadorCorredor").trigger("change");
    if (contrato.Corredor) {
        $('#pagoDirectoDiv').show();
    } else {
        $('#pagoDirectoDiv').hide();
    }

    $("#buscadorProveedor").val(contrato.Proveedor);
    $("#buscadorProveedor").trigger("change");
    $("#fechaDesdeId").val(formatearFecha(contrato.FechaDesdeFormateado));
    $("#fechaHastaId").val(formatearFecha(contrato.FechaHastaFormateado));
    if (!hijo) {

        $("#tipoId").data("kendoDropDownList").value(contrato.TipoNegocioId);
        $("#tipoId").data("kendoDropDownList").trigger("change");
        if (!(contrato.DesdeFijacionFormateado == null && contrato.DesdeFijacionFormateado == undefined && contrato.DesdeFijacionFormateado == "")) {
            $("#fechaDesdeTopeId").val(contrato.DesdeFijacionFormateado);
        } else {
            $("#fechaDesdeTopeId").val("");
        }
        if (!(contrato.HastaFijacionFormateado == "null" && contrato.HastaFijacionFormateado == undefined && contrato.HastaFijacionFormateado == "")) {
            $("#fechaHastaTopeId").val(contrato.HastaFijacionFormateado);
        } else {
            $("#fechaHastaTopeId").val("");
        }
        $("#condicionFijacionId").data("kendoDropDownList").value(contrato.CondicionFijacion);
    }

    $("#observacionId").val(contrato.Observacion);
    $("#cantidadId").data("kendoNumericTextBox").value(contrato.Cantidad);
    $("#cantidadId").trigger("change");

    $("#precioId").data("kendoNumericTextBox").value(contrato.Precio);
    $("#precioId").trigger("change");
    $("#precioMonedaId").data("kendoDropDownList").value(contrato.MonedaId);
    $("#precioMonedaId").data("kendoDropDownList").trigger("change");

    $("#clasificacion").data("kendoDropDownList").value(contrato.ClasificacionId);
    $("#clasificacion").data("kendoDropDownList").trigger("change");

    $("#destinoId").data("kendoDropDownList").value(contrato.DestinoId);
    $("#destinoId").data("kendoDropDownList").trigger("change");
    if (contrato.CantidadCamiones != "null" && contrato.CantidadCamiones != undefined && contrato.CantidadCamiones != 0) {
        $("#cantidadCamionesId").data("kendoNumericTextBox").value(contrato.CantidadCamiones);
        $("#cargarCantidadCamiones").prop("checked", true);
    }

    contrato.PlanCanje == true ? $("#planCanjeId").prop("checked", true) : $("#planCanjeId").prop("checked", false);
    contrato.Consignatario == true ? $("#consignatarioId").prop("checked", true) : $("#consignatarioId").prop("checked", false);
    if (contrato.LocalidadId !== null && contrato.LocalidadId !== "undefined" && contrato.ProvinciaId !== null && contrato.ProvinciaId !== "undefined") {
        $("#LocalidadCrearContrato").val(contrato.Localidad + "(" + contrato.Provincia + ")");
        HabilitarEstablecimiento();
    }
    if (contrato.ComercialId !== "null" && contrato.ComercialId !== "undefined") $("#comercialId").data("kendoDropDownList").value(contrato.ComercialId);
    if (contrato.ComercialId !== "null" && contrato.ComercialId !== "undefined") $("#comercialFijacionId").data("kendoDropDownList").value(contrato.ComercialId);




    if (contrato.Base == true) {
        $("#baseId").prop("checked", true);
    } else {
        $("#baseId").prop("checked", false);
    }

    if (contrato.Importe_Sustentable !== null && contrato.Importe_Sustentable !== undefined && contrato.Importe_Sustentable !== 0) {
        $("#sustentablePrecioId").data("kendoNumericTextBox").value(contrato.Importe_Sustentable);
        $("#sustentableMonedaId").data("kendoDropDownList").value(contrato.Moneda_Sustentable);
        $("#sustentableId").prop("checked", true);
        $(".sustentableDiv").show();
    }

    if (contrato.Fecha_DolarizadoFormateado !== null && contrato.Fecha_DolarizadoFormateado !== undefined && contrato.Fecha_DolarizadoFormateado !== "") {
        $("#dolarizadoId").prop("checked", true);
        $("#dolarizadoDiv").show();
        $("#dolarizadoFechaId").val(contrato.Fecha_DolarizadoFormateado);
    }

    if (contrato.Dias_Pesificado !== null && contrato.Dias_Pesificado !== undefined && contrato.Dias_Pesificado !== "") {
        $("#pesificadoId").prop("checked", true);
        $("#pesificadoDiv").show();
        $("#pesificadoDiasId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    }

    if (contrato.PorcentajeComision !== null && contrato.PorcentajeComision !== undefined && contrato.PorcentajeComision !== "") {
        $("#porcentajeComision").data("kendoNumericTextBox").value(contrato.PorcentajeComision);
    }
    if (contrato.NoInformaSIO == true) {
        $("#noInformaSioId").prop("checked", true);
    } else {
        $("#noInformaSioId").prop("checked", false);
    }

    if (contrato.TrigoEspecial == true) {
        $("#trigoEspecialId").prop("checked", true);
        $("#trigoEspecialFasonId").prop("checked", true);
        $("#especialesId").show();
        $("#agregarCalidad").show();
    } else {
        $("#trigoEspecialId").prop("checked", false);
    }
    LimpiarBoleto();
    if (contrato.BoletoId == 1) {
        $("#boletoConfirmaId").prop("checked", true);
        $("#BolsaConfirmaDiv").show();
        $("#bolsaConfirmaId").data("kendoDropDownList").value(contrato.BolsaId);
    } else if (contrato.BoletoId == 2) {
        $("#boletoFisicoId").prop("checked", true);
        $("#BolsaFisicoDiv").show();
        $("#bolsaFisicoId").data("kendoDropDownList").value(contrato.BolsaId);
    } else if (contrato.BoletoId == 4) {
        $("#boletoCartaId").prop("checked", true);
        $("#BolsaCartaDiv").show();
        $("#bolsaCartaId").data("kendoDropDownList").value(contrato.BolsaId);
    } else if (contrato.BoletoId == 3) {
        $("#boletoNingunoId").prop("checked", true);
    }

    contrato.CD == true ? $("#CDId").prop("checked", true) : $("#CDId").prop("checked", false);
    contrato.Warrant == true ? $("#WarrantId").prop("checked", true) : $("#WarrantId").prop("checked", false);
    contrato.PagoDirectoVendedor == true ? $("#pagoDirectoId").prop("checked", true) : $("#pagoDirectoId").prop("checked", false);

    contrato.EstablecimientoPropio == true ? $("#establecimientoPropioId").prop("checked", true) : contrato.EstablecimientoPropio == false ? $("#establecimientoArrendadoId").prop("checked", true) : false;
    if (contrato.TipoNegocioId == 3) {
        $("#contratoId").val(contrato.DatosFijacion.ContratoId);
        $("#datosContrato").show();
        $("#kgscontrato").text(contrato.DatosFijacion.KilosPendiente + "/" + contrato.DatosFijacion.KilosAplicados);
        $("#desdecontrato").text(contrato.DatosFijacion.FechaDesde);
        $("#hastacontrato").text(contrato.DatosFijacion.FechaHasta);
    }
    $("#contCorredorId").val(contrato.ContratoCorredor);
    $("#contVendedorId").val(contrato.ContratoVendedor);
    if (hijo != true) {
        if (contrato.Madre === true) {
            $("#madreId").prop("checked", true);
            $("#pagosDiv").show();
        }
        if (contrato.Madre === false) {
            $("#hijoId").prop("checked", true);
            $(".contratoMadreDiv").show();
            $("#contMadreId").val(contrato.ContratoMadre);
        }
    }
    contrato.SelCargoMOA === true ? $("#selCargoMOAId").prop("checked", true) : $("#selCargoMOAId").prop("checked", false);
    contrato.SelCargoVendedor === true ? $("#selCargoVendedorId").prop("checked", true) : $("#selCargoVendedorId").prop("checked", false);

    $("#tipoFasonId").data("kendoDropDownList").value(contrato.TipoFasonId);
    $("#operadorId").data("kendoDropDownList").value(contrato.OperadorId);
    $("#posicionFasonId").val(contrato.Posicion);

    var iteracionesDescuentos = viewModel.Descuentos.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.Descuentos.pop();
    }
    var iteracionesCalidades = viewModel.Calidades.length;
    for (i = 0; i < iteracionesCalidades; i++) {
        viewModel.Calidades.pop();
    }
    contrato.MercsDeposito == true ? $("#mercsDepositoId").prop("checked", true) : $("#mercsDepositoId").prop("checked", false);

    var descuentosDto = contrato.Descuentos;
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
            ContratoId: descuento.ContratoId,
            Borrar: function () {
                viewModel.Descuentos.remove(this);
            }
        };
        viewModel.Descuentos.push(descuentoKendo);
    });

    calidadesDto = contrato.Calidades;
    $.each(calidadesDto, function (key, calidad) {
        var calidadKendo = {
            Id: calidad.Id,
            CalidadEspecialId: calidad.CalidadEspecialId,
            CalidadEspecialDesc: calidad.CalidadEspecialDesc,
            Valor: calidad.Valor,
            ContratoId: calidad.ContratoId,
            PorcentajeDesde: calidad.PorcentajeDesde,
            PorcentajeHasta: calidad.PorcentajeHasta,
            StandardDeCalidadId: 2,
            Borrar: function () {
                viewModel.Calidades.remove(this);
            }
        };
        viewModel.Calidades.push(calidadKendo);
    });
    if ($("#contratoAcuerdoId").val() != "") {
        $("#precioId").data("kendoNumericTextBox").enable(false);
    }
}

