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
            var datos = { id: e.dataItem.Id };
            var puedeCopiar = MSExecuteOnServer('/CompraNet/ValidarCopiarContrato', datos);
            if (puedeCopiar) {
                CargarCopiaContrato(e.dataItem.Id, "sap");
            } else {
                MensErr("No posee permisos para copiar el contrato")
            }

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


    $("#contratoAReemplazarId").click(function () {
        $("#AnulaYReemplazaContratoId").val("");
        $("#contratoAReemplazarId").data("kendoAutoComplete").value("");
        $("#contratoAReemplazarId").data("kendoAutoComplete").trigger("change");
        $("#MotivoReemplazoDiv").hide();
        $("#MotivoReemplazo").val("");
        $("#tipoId").data("kendoDropDownList").readonly(false);
        $("#motivoAnterior").data("kendoDropDownList").readonly(false);
        $("#fechaOperacionId").data("kendoDatePicker").enable(true);
        $("#fechaOperacionId").data("kendoDatePicker").value(formatearFecha(new Date()));
        $("#fechaOperacionId").data("kendoDatePicker").trigger("change");
        $("#descripcionMotivoAnterior").attr("readonly", false);
        $("#noInformaSioId").prop("checked", false);
        $("#noInformaSioId").attr('disabled', false);
        $("#boletoNingunoId").attr("disabled", false);
        $("#boletoNingunoId").prop("checked", false);
        $("#boletoConfirmaId").prop("checked", false);
        $("#boletoFisicoId").prop("checked", false);
        $("#boletoCartaId").prop("checked", false);
        $("#boletoConfirmaId").attr("disabled", false);
        $("#boletoFisicoId").attr("disabled", false);
        $("#boletoCartaId").attr("disabled", false);
        $(".mostrarConPase").hide();
    });

    $("#contratoAReemplazarId").kendoAutoComplete({
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
            CargarCopiaContrato(e.dataItem.Id, "anulayreemplaza");
            $(".mostrarConPase").show();
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
                    var valor = $("#contratoAReemplazarId").val();
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

    $("#contratoCondicional").click(function () {
        $("#contratoCondicionalId").val("");
        $("#contratoCondicional").data("kendoAutoComplete").value("");
        $("#contratoCondicional").data("kendoAutoComplete").trigger("change");
        $("#condicionalId").removeProp("disabled");
        $("#condicionalId").prop("checked", false);
        $("#aperturaPrecioConceptoBonificaciones").hide();
        $("#condicionalDiv").hide();
        $("#condicionalPrecioId").val("");
        $("#condicionalMonedaId").val("");
        $("#condicionalCantidadId").val("");
        $("#condicionalFechaId").val("");
        $("#condicionalPosicionId").val("");
        if ($("#tipoId").val() == 2) {
            $("#condicionalPrecioId").data("kendoNumericTextBox").value("");
            $("#condicionalCantidadId").data("kendoNumericTextBox").value("");
            var total = CalcularPrecioTotalApertura();
            $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(0);
            $("#precioTotalApertura").data("kendoNumericTextBox").value(total);
            $("#precioId").data("kendoNumericTextBox").enable(true);
            $("#precioMonedaId").data("kendoDropDownList").enable(true);
        }
        $("#buscadorProveedor").prop('disabled', false);
        $("#buscadorCorredor").prop('disabled', false);
        $('#material').data("kendoDropDownList").enable(true);
        $("#cantidadId").data("kendoNumericTextBox").enable(true);

    });

    $("#contratoCondicional").kendoAutoComplete({
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
            CargarCopiaContrato(e.dataItem.Id, "contratoCondicional");
        },
        dataSource: {
            serverFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerContratosCondicional"
                },
                parameterMap: function (data, type) {
                    var valor = $("#contratoCondicional").val();
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

    $("#btnConDescarga").hide();
}

function CargarCopiaContrato(contratoId, tipo) {
    var datos = { id: contratoId, tipo: tipo };
    contratoCopia = MSExecuteOnServer('/CompraNet/TraerContratoCompleto', datos, function () { $.unblockUI(); });
    if (tipo == "acuerdo") {
        contratoCopia.ContratoAcuerdoId = contratoId;
        $("#contratoAcuerdoId").val(contratoId);
    }

    if (contratoCopia.HayError) {
        MensErr(contratoCopia.Errores[0].Message);
    } else {
        contratoCopia.Estado = 1;
        if (contratoCopia.Descuentos) {
            for (var i = 0; i < contratoCopia.Descuentos.length; i++) {
                contratoCopia.Descuentos[i].Id = 0;
            }
        }
        if (contratoCopia.Calidades) {
            for (var i = 0; i < contratoCopia.Calidades.length; i++) {
                contratoCopia.Calidades[i].Id = 0;
            }
        }

        if (tipo == "sap") {
            contratoCopia.MotivoOperacionAnterior = null;
            contratoCopia.DescripcionOperacionAnterior = null;
            contratoCopia.FechaOperacionFormateado = formatearFecha(new Date());
        }

        if (tipo == "anulayreemplaza") {
            var datosStatus = { id: contratoId };
            var validaciones = MSExecuteOnServer('/CompraNet/ValidacionesAnulaYReemplaza', { contratoSap: contratoCopia.ContratoSAP }, function () { $.unblockUI(); });
            if (validaciones.ListaErrores.length > 0) {
                setTimeout(function () {
                    $("#AnulaYReemplazaContratoId").val("");
                    $("#contratoAReemplazarId").val("");
                    $("#contratoAReemplazarId").click();
                }, 300);
                MensErr(validaciones.Errores[0].Message);
                return;
            }
            var status = MSExecuteOnServer('/CompraNet/ValidarModificarFinalizado', datosStatus, function () { $.unblockUI(); });
            if (status.Status == "" && status.NumeroSio == 0) {
                setTimeout(function () {
                    $("#AnulaYReemplazaContratoId").val("");
                    $("#contratoAReemplazarId").val("");
                    $("#contratoAReemplazarId").click();
                }, 300);
                MensErr("El contrato aún no ha sido confirmado en SAP y no tiene número de SIO. Intente editarlo desde la pantalla de CompraNet.");
                return;
            } else if (status.NumeroSio > 0) {
                setTimeout(function () {
                    $("#AnulaYReemplazaContratoId").val("");
                    $("#contratoAReemplazarId").val("");
                    $("#contratoAReemplazarId").click();
                }, 300);
                MensErr("El contrato tiene número de SIO Granos. En caso de querer continuar esta anulación, por favor comunicarse con administración.");
                return;
            } else {
                contratoCopia.DescripcionOperacionAnterior = "Anula y reemplaza " + contratoCopia.ContratoSAP;
                contratoCopia.AnulaYReemplazaContratoId = contratoId;
                contratoCopia.contratoAReemplazarId = contratoCopia.ContratoSAP;
            }
        }

        if (tipo == "contratoCondicional") {
            contratoCopia.TipoNegocioId = $("#tipoId").val();
            contratoCopia.MotivoOperacionAnterior = null;
            contratoCopia.DescripcionOperacionAnterior = null;
            contratoCopia.FechaOperacionFormateado = formatearFecha(new Date());
            contratoCopia.FechaOperacion = "/Date(" + new Date().getTime() + ")/";
            contratoCopia.Cantidad = contratoCopia.CondicionalCantidad;
            if (contratoCopia.TipoNegocioId == 1) {
                contratoCopia.MonedaId = null;
                contratoCopia.Precio = 0;
                contratoCopia.PrecioNeto = null;
            }
            if (contratoCopia.TipoNegocioId == 2) {
                contratoCopia.MonedaId = contratoCopia.CondicionalMonedaId;
                contratoCopia.Precio = contratoCopia.CondicionalPrecio;
                contratoCopia.PrecioNeto = null;
            }
            objIndex = contratoCopia.AperturaPrecios.findIndex((obj => obj.ConceptoAperturaPrecioId == 4));
            contratoCopia.AperturaPrecios[objIndex].Importe = 0;
            contratoCopia.AperturaPrecios[objIndex].Porcentaje = 0;
            $("#condicionalId").prop("disabled", "disabled");
            contratoCopia.Condicional = false;
            contratoCopia.CondicionalContratoId = contratoCopia.Id;
            contratoCopia.CondicionalContratoSAP = contratoCopia.ContratoSAP;
        }
        modificarContrato(contratoCopia);
        CargarDatosEditar(contratoCopia);
        if (contratoCopia.Venta == true) {
            HayVenta();
            $(".venta").show();
            $(".venta").addClass("ampliar");
            $(".datos-venta").hide();
        }
        if (tipo == "acuerdo") {
            $("#fechaOperacionId").data("kendoDatePicker").enable(false);
            $("#chequeElectronicoId").show();
        }
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
    if (contrato.DolarExportador == true) {
        $("#dolarExportadorId").prop("checked", true);
    } else {
        $("#dolarExportadorId").prop("checked", false);
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
    else if (contrato.BoletoId == 5) {
        $("#sinBoletoId").prop("checked", true);
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
        $("#Cesion").val(contrato.Cesion);
        $("#Anticipo").text(contrato.Anticipo);
        $("#ClasificacionContrato").text(contrato.ClasificacionContrato);
        $("#ImporteAPrecioContrato").val(contrato.ImporteAPrecioContrato);
        $("#PorcentajeAPrecioContrato").val(contrato.PorcentajeAPrecioContrato);
        $("#MonedaAPrecioContrato").val(contrato.MonedaAPrecioContrato);
        $("#ImporteSobrePrecioContrato").val(contrato.ImporteSobrePrecioContrato);
        $("#PorcentajeSobrePrecioContrato").val(contrato.PorcentajeSobrePrecioContrato);
        $("#MonedaSobrePrecioContrato").val(contrato.MonedaSobrePrecioContrato);
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
    $("#posicionCBOTId").val(contrato.PosicionCBOT);
    $("#tipoPosicionCBOTId").data("kendoDropDownList").value(contrato.TipoPosicionCBOT);

    var iteracionesDescuentos = viewModel.Descuentos.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.Descuentos.pop();
    }
    var iteracionesCalidades = viewModel.Calidades.length;
    for (i = 0; i < iteracionesCalidades; i++) {
        viewModel.Calidades.pop();
    }
    contrato.MercsDeposito == true ? $("#mercsDepositoId").prop("checked", true) : $("#mercsDepositoId").prop("checked", false);
    if (contrato.MercsDeposito == true) {
        HayMercaderia();
        $(".depositoDiv").show();
        $("#cantidadDeposito").data("kendoNumericTextBox").value(contrato.CantidadDeposito);
    }
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

    if (contrato.Importe_Sustentable !== null && contrato.Importe_Sustentable !== undefined && contrato.Importe_Sustentable !== 0) {
        $("#sustentablePrecioId").data("kendoNumericTextBox").value(contrato.Importe_Sustentable);
        $("#sustentableMonedaId").data("kendoDropDownList").value(contrato.Moneda_Sustentable);
        $("#sustentableId").prop("checked", true);
        $(".sustentableDiv").show();
        if (contrato.MercsDeposito == true) {
            $(".fechaHastaSustentableDiv").show();
            $("#fechaDesdeSustentableId").data("kendoDatePicker").value(FormatearFecha((contrato.FechaDesde_SustentableFormateado)));
            $("#fechaHastaSustentableId").data("kendoDatePicker").value(FormatearFecha((contrato.FechaHasta_SustentableFormateado)));
        }
    }
}

function ObtenerDatos(error) {
    var obj = {};
    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth();
    var dia = hoy.getDate();
    var hora = hoy.getHours();
    var minuto = hoy.getMinutes();
    var maniana = new Date();
    maniana = new Date(maniana.setMonth(maniana.getMonth() + 1));

    obj.TipoNegocioId = $("#tipoId").val();
    obj.Id = Id == null || Id == undefined || Id == "" ? 0 : Id;
    obj.MaterialId = $("#material").val() || 0;
    obj.Cantidad = $("#cantidadId").val() == null || $("#cantidadId").val() == undefined || $("#cantidadId").val() == "" ? 0 : $("#cantidadId").val();
    obj.Precio = $("#precioId").val() == null || $("#precioId").val() == undefined || $("#precioId").val() == "" ? 0 : $("#precioId").val();
    obj.PrecioNeto = $("#precioTotalApertura").val();
    obj.FechaEntrega = (obj.TipoNegocioId == "1" || obj.TipoNegocioId == "2" || obj.TipoNegocioId == "6") ? $("#fechaHastaId").val() == null || $("#fechaHastaId").val() == undefined || $("#fechaHastaId").val() == "" ? formatearFecha(maniana) : FormatearFecha($("#fechaHastaId").val()) : null;
    obj.CampanaId = $("#campanaId").val();
    //if (TipoId != "3") {
    obj.FechaDesde = $("#fechaDesdeId").val() == null || $("#fechaDesdeId").val() == undefined || $("#fechaDesdeId").val() == "" ? formatearFecha(hoy) : FormatearFecha($("#fechaDesdeId").val());
    obj.FechaHasta = $("#fechaHastaId").val() == null || $("#fechaHastaId").val() == undefined || $("#fechaHastaId").val() == "" ? formatearFecha(maniana) : FormatearFecha($("#fechaHastaId").val());
    //} else {
    //    obj.FechaDesde = formatearFecha(hoy);
    //    obj.FechaHasta = formatearFecha(maniana);
    //}
    obj.Fecha = $("#fechaFijacionId").val() == null || $("#fechaFijacionId").val() == undefined || $("#fechaFijacionId").val() == "" ? formatearFecha(hoy) : FormatearFecha($("#fechaFijacionId").val());
    obj.MonedaId = $("#precioMonedaId").val();
    obj.PosicionCBOT = $("#posicionCBOTId").val();
    obj.TipoPosicionCBOTId = $("#tipoPosicionCBOTId").val();

    if (obj.TipoNegocioId == "3" || obj.TipoNegocioId == "4") {
        obj.ComercialId = $("#comercialFijacionId").val();
        obj.ContratoSAP = $("#contratoId").val();
    }
    if (obj.TipoNegocioId == "1" || obj.TipoNegocioId == "2") {
        obj.PorcentajeDePago = $("#porcentajeDePagoId").data("kendoNumericTextBox").value();
    }

    obj.ComercialId = $("#comercialId").val();
    obj.ComercialCreadorId = $("#comercialCreador").val();
    obj.Base = $("#baseId").is(":checked") ? true : false;
    obj.ImporteSustentable = $("#sustentablePrecioId").val();//== null || $("#sustentablePrecioId").val() == undefined || $("#sustentablePrecioId").val() == "" ? 0 : $("#sustentablePrecioId").val();
    obj.MonedaSustentableId = $("#sustentableMonedaId").val();
    obj.FechaDesdeSustentable = fechaValida($("#fechaDesdeSustentableId").val()) ? $("#fechaDesdeSustentableId").val() : null;
    obj.FechaHastaSustentable = fechaValida($("#fechaHastaSustentableId").val()) ? $("#fechaHastaSustentableId").val() : null;
    obj.tarifaAConvenir = $("#tarifaAConvenirId").is(":checked");
    obj.FechaDolarizado = $("#tipoId").val() == "1" ? "" : $("#dolarizadoFechaId").val();
    obj.PagoDiferidoContrato = $("#pesificadoId").is(":checked") ? true : false;
    obj.PagoDiferido = obj.TipoNegocioId != "3" ? $("#pesificadoId").is(":checked") ? true : false : $("#diasDiferidoId").is(":checked") ? true : false;
    obj.Dolarizado = $("#tipoId").val() == "1" ? false : $("#dolarizadoId").is(":checked") ? true : false;
    obj.Sustentable = $("#sustentableId").is(":checked") ? true : false;
    obj.EPA = $("#epaId").is(":checked") ? true : false;
    obj.EUDR = $("#eudrId").is(":checked") ? true : false;
    obj.SustentableTipoDBId = $("#selectSustenTipoDB").val();
    obj.DiasPesificado = obj.TipoNegocioId != "3" ? $("#pesificadoDiasId").val() : $("#diasDiferidoFijacionId").val();
    obj.PorcentajeComision = $("#porcentajeComision").val() != "" ? $("#porcentajeComision").val() : 0;
    obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.EstadoId = $("#estado").val() == "" ? $("#baseId").is(":checked") ? "3" : obj.TipoNegocioId != "4" && obj.TipoNegocioId != "5" && obj.TipoNegocioId != "6" ? "1" : "2" : $("#estado").val();
    obj.Observacion = $("#observacionId").val();
    obj.ClasificacionId = $("#clasificacion").val();
    obj.CantidadCamiones = $("#cantidadCamionesId").val() == null || $("#cantidadCamionesId").val() == undefined || $("#cantidadCamionesId").val() == "" ? 0 : $("#cantidadCamionesId").val();
    obj.EstablecimientoPropio = $("#establecimientoPropioId").is(":checked") ? true : $("#establecimientoArrendadoId").is(":checked") ? false : null;
    if (obj.TipoNegocioId == TIPO_NEGOCIO.FIJACION) {
        obj.DesdeFijacion = $("#fechaFijacionDesde").val() == null || $("#fechaFijacionDesde").val() == undefined || $("#fechaFijacionDesde").val() == "" ? formatearFecha(hoy) : $("#fechaFijacionDesde").val();
        obj.HastaFijacion = $("#fechaFijacionHasta").val() == null || $("#fechaFijacionHasta").val() == undefined || $("#fechaFijacionHasta").val() == "" ? formatearFecha(maniana) : $("#fechaFijacionHasta").val();
    } else if (obj.TipoNegocioId == TIPO_NEGOCIO.A_FIJAR) {
        obj.DesdeFijacion = $("#fechaDesdeTopeId").val() == null || $("#fechaDesdeTopeId").val() == undefined || $("#fechaDesdeTopeId").val() == "" ? formatearFecha(hoy) : $("#fechaDesdeTopeId").val();
        obj.HastaFijacion = $("#fechaHastaTopeId").val() == null || $("#fechaHastaTopeId").val() == undefined || $("#fechaHastaTopeId").val() == "" ? formatearFecha(maniana) : $("#fechaHastaTopeId").val();
    } else {
        obj.DesdeFijacion = $("#fechaDesdeTopeId").val() == null || $("#fechaDesdeTopeId").val() == undefined || $("#fechaDesdeTopeId").val() == "" ? null : $("#fechaDesdeTopeId").val();
        obj.HastaFijacion = $("#fechaHastaTopeId").val() == null || $("#fechaHastaTopeId").val() == undefined || $("#fechaHastaTopeId").val() == "" ? null : $("#fechaHastaTopeId").val();
    }
    obj.CondicionFijacionId = $("#condicionFijacionId").val();
    obj.DestinoId = $("#destinoId").val();
    obj.planCanje = $("#planCanjeId").is(":checked") ? true : false;
    obj.Consignatario = $("#consignatarioId").is(":checked") && (obj.TipoNegocioId == "1" || obj.TipoNegocioId == "2" || obj.TipoNegocioId == "6") ? true : false;
    obj.CD = $("#CDId").is(":checked") ? true : false;
    obj.Warrant = $("#WarrantId").is(":checked") ? true : false;
    obj.PagoDirectoVendedor = $("#pagoDirectoId").is(":checked") ? true : false;
    obj.MercsDeposito = $("#mercsDepositoId").is(":checked") ? true : false;
    obj.CantidadDeposito = ($("#tipoId").val() == "2" && $("#mercsDepositoId").is(":checked")) ? $("#cantidadDeposito").data("kendoNumericTextBox").value() : null;
    obj.NivelTarifaId = $("#NivelTarifaId").val();
    obj.TarifaFlete = $("#TarifaFleteId").val();
    obj.ChequeElectronico = $("#tipoId").val() == "2" ? $("#chequeElectronicoInput").is(":checked") ? true : false : $("#chequeElectronico").is(":checked") ? true : false;
    obj.DolarizadoExpress = $("#tipoId").val() == "1" ? false : $("#tipoId").val() == "3" ? $("#expressId").is(":checked") ? true : false : $("#dolarizadoExpressId").is(":checked") ? true : false;
    obj.PagoCBU = $("#tipoId").val() == "2" ? $("#pagoCbu").val() : $("#pagoCbuInput").val();

    if (obj.TipoNegocioId == "1" || obj.TipoNegocioId == "2" || obj.TipoNegocioId == "6") {
        if ($("#boletoConfirmaId").is(':checked')) {
            obj.BoletoId = 1;
            obj.BolsaId = $("#bolsaConfirmaId").val();
        }
        else if ($("#boletoFisicoId").is(':checked')) {
            obj.BoletoId = 2;
            obj.BolsaId = $("#bolsaFisicoId").val();
        }
        else if ($("#boletoCartaId").is(':checked')) {
            obj.BoletoId = 4;
            obj.BolsaId = $("#bolsaCartaId").val();
        }
        else if ($("#boletoNingunoId").is(':checked')) {
            obj.BoletoId = 3;
            obj.BolsaId = 0;
        }
        else if ($("#sinBoletoId").is(':checked')) {
            obj.BoletoId = 5;
            obj.BolsaId = 0;
        }
    }

    var proveedorId;
    var corredorId;
    var razonSocial;
    if ($("#buscadorProveedor").val() != "") {
        var cuitAux = $("#buscadorProveedor").val().split('(');
        if (cuitAux[1]) {
            var cuit = cuitAux[1].split(')');
            proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0], corredor: false });
            if (proveedorId == null || proveedorId == 0) {
                MensErr("No se pudo obtener el proveedor. Verificar la segmentación.");
                $.unblockUI();
                return;
            }
            razonSocial = cuitAux[0].trim();
        } else {
            proveedorId = -1;
        }
    }
    if ($("#buscadorCorredor").val() != "") {
        var cuitAuxC = $("#buscadorCorredor").val().split('(');
        if (cuitAuxC[1]) {
            var cuitC = cuitAuxC[1].split(')');
            corredorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuitC[0], corredor: true });
        }
    }

    if (obj.TipoNegocioId != "5") {
        obj.ProveedorId = proveedorId;
        obj.CorredorId = corredorId;
    }

    if ($("#LocalidadCrearContrato").val() != "") {
        var localidadAux = $("#LocalidadCrearContrato").val().split('(');
        if (localidadAux[1]) {
            var provinciaAux = localidadAux[1].split(')');
            var Localidad = MSExecuteOnServer('/CompraNet/ObtenerLocalidadId', { localidad: localidadAux[0], provincia: provinciaAux[0] });
            obj.LocalidadId = Localidad.LocalidadId;
            obj.ProvinciaId = Localidad.ProvinciaId;
        } else {
            obj.LocalidadId = -1;
        }
    }
    obj.ContratoVendedor = $("#contVendedorId").val();
    obj.ContratoCorredor = $("#contCorredorId").val();
    obj.SelCargoVendedor = $("#selCargoVendedorId").is(":checked") ? true : false;
    obj.SelCargoMOA = $("#selCargoMOAId").is(":checked") ? true : false;
    obj.FasoneroId = proveedorId;
    obj.Posicion = $("#posicionFasonId").val();
    obj.TipoFasonId = $("#tipoFasonId").val();
    if (obj.TipoNegocioId == 5) {
        obj.tipoAgenteCompraId = $("#tipoAgenteCompraId").val();
    }
    if (obj.TipoNegocioId == 1) {
        obj.ObligatoriedadBonificacion = $("#esBonificacion").is(":checked") == true ? true : $("#esBonificacion").is(":checked") == false ? false : null;

    }
    obj.ObligatoriedadCostoFinanciero = $("#esCostoFinanciero").is(":checked") == true ? true : $("#esCostoFinanciero").is(":checked") == false ? false : null;
    obj.OperadorId = $("#operadorId").val();
    if ($("#madreId").is(":checked")) {
        obj.Madre = true;
    }
    if ($("#hijoId").is(":checked")) {
        obj.Madre = false;
    } else {
        if (obj.TipoNegocioId == 2 || obj.TipoNegocioId == 3 || obj.TipoNegocioId == 6) {
            obj.FechaCierta = $("#fechaCiertaId").val();
        }
    } if ($("#fasonIdCheck").is(":checked")) {
        obj.EsFason = true;
    }
    obj.ContratoMadre = $("#contMadreId").val();
    obj.Descuentos = viewModel.Descuentos;
    obj.Servicios = viewModel.Servicios;

    obj.Especial = $("#trigoEspecialFasonId").is(":checked") ? true : false;
    obj.DolarExportador = $("#dolarExportadorId").is(":checked") ? true : false;
    obj.TrigoEspecial = viewModel.Calidades.length > 0 && obj.MaterialId == 2;

    obj.ZonaId = obj.MaterialId == 5 ?
        $("#zonasGirasolAltoId").val() : null;
    if (obj.MaterialId === "3") {
        obj.Calidad = viewModel.Calidades;
    } else {
        if ($("#calidadesEspecialesId").data("kendoDropDownList").text() == "Especial" ||
            $("#calidadesEspecialesId").data("kendoDropDownList").text() == "Grado 2" ||
            $("#calidadesEspecialesId").data("kendoDropDownList").text() == "Grado" ||
            $("#calidadesEspecialesId").data("kendoDropDownList").text() == "Bonif. SECO de 7% a 10% Por punto") {
            var err = [];
            if (viewModel.Calidades.length == 0 && (obj.TipoNegocioId == 1 || obj.TipoNegocioId == 2)) {
                err = AgregarCalidades();
            } else if ($("#valorEspecialesId").val() != "") {
                LimpiarCalidades();
                err = AgregarCalidades();
            }
            if (ExistsErrorMessages(err)) {
                $.unblockUI();
                error = true;
            } else {
                obj.Calidad = viewModel.Calidades;
            }
        }
    }

    if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Camara") {
        if (obj.MaterialId == 3) obj.StandardDeCalidadId = 4;
        else if (obj.MaterialId == 4 || obj.MaterialId == 5) obj.StandardDeCalidadId = 5;
        else if (obj.MaterialId == 1 || obj.MaterialId == 2 || obj.MaterialId == 6) obj.StandardDeCalidadId = 1;
    } else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Fabrica") {
        obj.StandardDeCalidadId = 3;
    } else if (viewModel.Calidades.length > 0) {
        obj.StandardDeCalidadId = viewModel.Calidades[0].StandardDeCalidadId;
    }

    if (obj.TipoNegocioId == 3 || obj.TipoNegocioId == 4) obj.TrigoEspecial = $("#trigoEspecialFijacion").is(":checked");
    obj.Compensacion = $("#compensacionId").is(":checked") ? true : false;
    obj.ContratoAcuerdoId = $("#contratoAcuerdoId").val();
    obj.Pizarra = $("#pizarraId").is(":checked") ? true : false;
    obj.AperturaPrecio = viewModel.AperturaPrecio;
    obj.PrecioPactado = viewModel.PrecioPactado;
    if (obj.TipoNegocioId == 1 || obj.TipoNegocioId == 2) {
        obj.tipoAgenteCompraId = $("#AgenteCompraId").val();
        if (obj.tipoAgenteCompraId > 0) {
            obj.CaratulaExtension = $("#caratulaExtensionId").val();
            obj.CaratulaMAT = $("#caratulaMATId").val();
            obj.PrecioAjusteComision = $("#precioAjusteComisionId").val();
            obj.MonedaAjusteComisionId = $("#monedaAjusteComisionId").val();
        }
    }
    if (obj.TipoNegocioId == 6) {
        obj.tipoAgenteCompraId = $("#AgenteCompraId").val();
    }
    if (obj.TipoNegocioId == 1 || obj.TipoNegocioId == 2 || obj.TipoNegocioId == 6 || obj.TipoNegocioId == 4) {
        obj.FechaOperacion = $("#fechaOperacionId").val() == null || $("#fechaOperacionId").val() == undefined || $("#fechaOperacionId").val() == "" ? formatearFecha(hoy) : $("#fechaOperacionId").val();
        if ($('#motivoAnterior').data("kendoDropDownList").text() == "Otro" || $('#motivoAnterior').data("kendoDropDownList").value() == "" && $('#descripcionMotivoAnterior').val() != "") {
            obj.MotivoOperacionAnterior = "Otro";
            obj.DescripcionOperacionAnterior = $("#descripcionMotivoAnterior").val();
        } else {
            $("#motivoOperacionAnteriorId").val($('#motivoAnterior').data("kendoDropDownList").text());
            obj.MotivoOperacionAnterior = $('#motivoAnterior').data("kendoDropDownList").value() != "" ? $("#motivoOperacionAnteriorId").val() : null;
            obj.DescripcionOperacionAnterior = $("#descripcionMotivoAnterior").val();
        }
    }
    if (obj.TipoNegocioId == 3) {
        obj.FechaOperacion = $("#fechaFijacionId").val() == null || $("#fechaFijacionId").val() == undefined || $("#fechaFijacionId").val() == "" ? formatearFecha(hoy) : $("#fechaFijacionId").val();

        if ($('#motivoAnterior').data("kendoDropDownList").text() == "Otro" || $('#motivoAnterior').data("kendoDropDownList").value() == "") {
            obj.MotivoOperacionAnterior = "Otro";
            obj.DescripcionOperacionAnterior = $("#descripcionMotivoAnterior").val();
        } else {
            $("#motivoOperacionAnteriorFijacion").val($('#motivoAnterior').data("kendoDropDownList").text());
            obj.MotivoOperacionAnterior = $("#motivoOperacionAnteriorFijacion").val();
            obj.DescripcionOperacionAnterior = $("#descripcionMotivoAnterior").val();
        }
    }

    if (obj.TipoNegocioId == 5) {
        obj.FechaOperacion = $("#fechaOperacionAgenteId").val() == null || $("#fechaOperacionAgenteId").val() == undefined || $("#fechaOperacionAgenteId").val() == "" ? formatearFecha(hoy) : $("#fechaOperacionAgenteId").val();
    }

    if ($("#canjeId").is(":checked") == true) {
        obj.Canje = true;
        obj.MonedaCanjeId = $("#montoMonedaId").data("kendoDropDownList").value();
        obj.Insumo = $("#insumoId").val();
        obj.Monto = $("#montoId").data("kendoNumericTextBox").value();
        ocultarSiHayCanje();
    }

    if ($("#prestamoDevolucionId").is(":checked") == true) {
        ocultarSiHayPrestamos()
        ocultarSiHayCanje();
        obj.PrestamoDevolucion = true;
        obj.PlantaDestinoId = $("#plantaDestinoId").data("kendoDropDownList").value();
    }
    if ($("#ventaId").is(":checked") == true) {
        obj.Venta = true;
    }
    if ($("#virtualId").is(":checked") == true) {
        obj.Virtual = true;
    }
    obj.Anticipo = $("#Anticipo").val();
    obj.Cesion = $("#Cesion").val();
    obj.ClasificacionContrato = $("#ClasificacionContrato").val();
    //obj.Clasificacion = $("#ClasificacionContrato").val();
    obj.ImporteAPrecioContrato = $("#ImporteAPrecioContrato").val();
    obj.PorcentajeAPrecioContrato = $("#PorcentajeAPrecioContrato").val();
    obj.MonedaAPrecioContrato = $("#MonedaAPrecioContrato").val();
    obj.ImporteSobrePrecioContrato = $("#ImporteSobrePrecioContrato").val();
    obj.PorcentajeSobrePrecioContrato = $("#PorcentajeSobrePrecioContrato").val();
    obj.MonedaSobrePrecioContrato = $("#MonedaSobrePrecioContrato").val();

    obj.AnulaYReemplazaContratoId = $("#AnulaYReemplazaContratoId").val();
    obj.MotivoReemplazo = $("#MotivoReemplazo").val();

    obj.CamaraId = $("#camaraListado").val();
    obj.ComisionAFavorId = $("#comisionAFavorListado").val();
    obj.PorcentajeComisionVenta = $("#porcentajeComisionVentaId").val();

    if ($("#LocalidadVenta").val() != "" && $("#LocalidadVenta").val() != null) {
        var localidadVentaAux = $("#LocalidadVenta").val().split('(');
        if (localidadVentaAux[1]) {
            var provinciaVentaAux = localidadVentaAux[1].split(')');
            var LocalidadVenta = MSExecuteOnServer('/CompraNet/ObtenerLocalidadId', { localidad: localidadVentaAux[0], provincia: provinciaVentaAux[0] });
            obj.ProcedenciaVentaId = LocalidadVenta.LocalidadId;
        } else {
            obj.ProcedenciaVentaId = -1;
        }
    }
    obj.CreditoDisponible = $("#creditoId").val();
    obj.FleteACargo = $("#FleteACargoListado").val();
    obj.KgBalanza = $("#KgBalanzaListado").val();
    obj.CondicionDePagoDiaPesificado = $("#cantidadDiaPesificacion").val();
    obj.CondicionDePagoTipoPesificado = $("#CondicionPesificacionListado").val();
    obj.CondicionDePagoPesificadoVentaId = $("#CondicionDePagoPesificadoVentaListado").val();
    obj.Pago = $("#PagoListado").val();
    obj.CondicionDePagoDiaFijacion = $("#cantidadDiaCondicion").val();
    obj.CondicionDePagoTipoFijacion = $("#CondicionPagoListado").val();
    obj.BoletoVentaId = $("#BoletoVenta").val();
    obj.MailVentaBoleto = $("#mailVenta").val();
    obj.CondicionDePagoFijacionVentaId = $("#CondicionDePagoFijacionVentaListado").val();

    if (obj.TipoNegocioId == 2) {
        obj.Condicional = $("#condicionalId").is(":checked");
        obj.CondicionalPrecio = $("#condicionalPrecioId").val();
        obj.CondicionalMonedaId = $("#condicionalMonedaId").val();
        obj.CondicionalCantidad = $("#condicionalCantidadId").val();
        obj.CondicionalFecha = $("#condicionalFechaId").val();
        obj.CondicionalPosicion = $("#condicionalPosicionId").val();
    }
    obj.CondicionalContratoId = $("#contratoCondicionalId").val();
    obj.CondicionalContratoSAP = $("#contratoCondicional").val();

    if (obj.TipoNegocioId == 1) {
        obj.KgMinimo = $("#minimoId").val();

        if ($("#maximaId").val() !== undefined) {
            var kgMaximo = $("#maximaId").val();
            kgMaximo = kgMaximo.replace(",", ".");
            obj.KgMaximo = Math.round(kgMaximo);
        } else {
            obj.KgMaximo = $("#maximaId").val();
        }
    }
    if ($("#ventaId").is(":checked") == true) {
        obj.Venta = true;
    }
    obj.ProveedorComisionistaId = $("#comisionistaId").val();

    if ($("#conDescargaId").is(":checked") == true) {
        obj.ConDescarga = true;
        obj.ConDescargaDias = dataTabla;
    } else {
        obj.ConDescarga = false;
        obj.ConDescargaDias = [];
    }

    return obj;
}

function RedireccionarNegocio(url, tipoId, obj) {
    if (tipoId != '') {
        var form = document.createElement("form");
        var element1 = document.createElement("input");
        var element2 = document.createElement("input");
        var siguiente = document.createElement("input");
        var negocioId = document.createElement("input");
        form.method = "POST";
        form.action = url;
        element1.value = tipoId;
        element1.name = "tipoId";
        element2.value = JSON.stringify(obj);
        element2.name = "obj";
        siguiente.value = Siguientes.replace(/(&quot\;)/g, "\"");
        siguiente.name = "siguientes";
        negocioId.value = Id;
        negocioId.name = "Id";
        form.appendChild(element1);
        form.appendChild(element2);
        form.appendChild(siguiente);
        form.appendChild(negocioId);
        document.body.appendChild(form);
        form.submit();
    } else {
        $.unblockUI();
    }
}

function InicializarDatos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniCrearContrato = data;
            datosIniCrearContrato.Datos.Destino = datosIniCrearContrato.Datos.Destino.filter(function (x) { return (x => x.Id != 10 && x.Id != 9) });
            //6	Prest Dev.Buenos Aires
            //7	Prest Dev.Santa Fe 
            //9	LE
            //10	SAN LORENZO SUSTENTABLE / CALIDAD
            AsignarDatos();
        }
        if (Id != "" && esEdicion == "False") {
            switch (TipoId) {
                case "1":
                    setTimeout(InicializarContratoEdit, 300);
                    break;
                case "2":
                    setTimeout(InicializarContratoEdit, 300);
                    break;
                case "3":
                    setTimeout(InicializarFijacionEdit, 300);
                    break;
                case "4":
                    setTimeout(InicializarFasonEdit, 300);
                    break;
                case "5":
                    setTimeout(InicializarAgenteEdit, 300);
                    break;
                case "6":
                    setTimeout(InicializarAcuerdoEdit, 300);
                    break;
                default:
                    $.unblockUI();
                    InicializarBordesRojos();
            }
        } else {
            $.unblockUI();
            InicializarBordesRojos();
        }
    };

    ExecuteURLOnServer('/CompraNet/InicializarContrato', funcReturn, '');
}

function fechaValida(fecha) {
    return fecha != null && fecha != undefined && fecha != "";
}

function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd-MM-yyyy");
    return FormatearFecha(fechaFormateada);
}

function FormatearFecha(fecha) {
    if (fecha != null && fecha != undefined && fecha != "") {
        var dias = fecha.split('-');
        var dia = dias[0];
        var mes = dias[1];
        var anio = dias[2];
        if (dia.length < 2) {
            dia = "0" + dia;
        }
        if (mes.length < 2) {
            mes = "0" + mes;
        }
        return dia + '-' + mes + '-' + anio;
    }
}

function DatosProveedor() {
    if ($("#buscadorProveedor").val() != "" && !$("#sinBoletoId").is(":checked")) {
        var id = $("#proveedorId").val() != "" ? $("#proveedorId").val() : 0;
        var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: id });
        if ($("#estado").val() !== "5" && compraNet.BoletoCompraNetId !== null) {
            LimpiarBoleto();
            if (compraNet.BoletoCompraNetId === 1) {
                $("#boletoConfirmaId").prop("checked", true);
                $("#BolsaConfirmaDiv").show();
                $("#bolsaConfirmaId").data("kendoDropDownList").value(compraNet.BolsaCompraNetId);
                $("#bolsaConfirmaId").data("kendoDropDownList").trigger("change");
            } else if (compraNet.BoletoCompraNetId === 2) {
                $("#boletoFisicoId").prop("checked", true);
                $("#BolsaFisicoDiv").show();
                $("#bolsaFisicoId").data("kendoDropDownList").value(compraNet.BolsaCompraNetId);
                $("#bolsaFisicoId").data("kendoDropDownList").trigger("change");
            } else if (compraNet.BoletoCompraNetId === 4) {
                $("#boletoCartaId").prop("checked", true);
                $("#BolsaCartaDiv").show();
                $("#bolsaCartaId").data("kendoDropDownList").value(compraNet.BolsaCompraNetId);
                $("#bolsaCartaId").data("kendoDropDownList").trigger("change");
            } else if (compraNet.BoletoCompraNetId === 3) {
                $("#boletoNingunoId").prop("checked", true);
            }
            else if (compraNet.BoletoCompraNetId === 5) {
                $("#sinBoletoId").prop("checked", true);
            }
        }
    }
}

function MostrarCcPpPendientesAplicar() {
    if ($("#mercsDepositoId").is(":checked") && ($("#sustentableId").is(":checked") || $("#epaId").is(":checked") || $("#eudrId").is(":checked")) && $("#selectSustenTipoDB").val() == 2) {
        $(".fechaHastaSustentableDiv").show();
        var cuitP = "";
        var cuitC = "";

        var cuitProv = $("#buscadorProveedor").val().split('(');
        if (cuitProv[1] != null) {
            cuitP = cuitProv[1].split(')')[0];
        }
        else {
            cuitP = cuitProv[0];
        }
        var cuitCorr = $("#buscadorCorredor").val().split('(');
        if (cuitCorr[1] != null) {
            cuitC = cuitCorr[1].split(')')[0];
        }
        else {
            cuitC = cuitCorr[0];
        }

        var lista = MSExecuteOnServer('/CompraNet/ListarCartasDePortePendienteAplicar',
            {
                AgenteCompra: $("#AgenteCompraId").val(),
                Proveedor: cuitP,
                Corredor: cuitC,
                Centro: $("#destinoId").val(),
                Material: $('#material').data("kendoDropDownList").value()
            });
        if (lista == null || lista.length == 0) {
            $("#mercsDepositoId").prop("checked", false);
            $(".fechaHastaSustentableDiv").hide();
            $("#fechaDesdeSustentableId").data("kendoDatePicker").value("");
            $("#fechaHastaSustentableId").data("kendoDatePicker").value("");
            MensInfo("No hay cartas de porte pendientes de aplicar");
        } else if ($("#fechaDesdeSustentableId").val() == '') {
            $("#fechaDesdeSustentableId").val(lista[0].FechaIngresoString);
            $("#fechaHastaSustentableId").val($("#fechaHastaId").val());
        }
        //var viewmodel = {
        //    CcPpPendientes: lista
        //}
        //kendo.bind($("#modalCcPpPendienteAplicar"), viewmodel);
        //$("#modalCcPpPendienteAplicar").modal("show");
    } else {
        $(".fechaHastaSustentableDiv").hide();
        $("#fechaDesdeSustentableId").data("kendoDatePicker").value("");
        $("#fechaHastaSustentableId").data("kendoDatePicker").value("");
    }
}

function OcultarCamposAgente() {
    if ($("#AgenteCompraId").data("kendoDropDownList").value() == "" && ($("#tipoId").data("kendoDropDownList").value() == "2" || $("#tipoId").data("kendoDropDownList").value() == "1")) {
        $(".ocultarAgenteDiv").hide();
        $("#caratulaExtensionId").val("");
        $("#caratulaMATId").val("");
        $("#precioAjusteComisionId").data("kendoNumericTextBox").value("");
        $("#monedaAjusteComisionId").data("kendoDropDownList").value("");
    } else {
        $(".ocultarAgenteDiv").show();
    }
}

var intervalActivo = false;
var intervalId;
var textoMensajeFlotante = '';
$("#mensaje-flotante").hide();
var dataTabla = [];
var nombrePantalla = "CuposConDescarga";

function comenzarCarga() {
    if (intervalActivo) return;

    console.log("minutos configurados: " + datosIniCrearContrato.Datos.MinutosCronometroConDescarga);
    const startingMinutes = datosIniCrearContrato.Datos.MinutosCronometroConDescarga;
    let time = startingMinutes * 60;
    const countdownEl = document.getElementById('countdown');

    intervalId = setInterval(updateCountdown, 1000);
    intervalActivo = true;
    $("#mensaje-flotante").show();

    function updateCountdown() {
        var minutes = Math.floor(time / 60);
        let seconds = time % 60;

        if (minutes == 0 && seconds == 0) {
            detenerIntervalo();
            LiberarPantalla();
            dataTabla = [];
            $("#modalCargarCuposConDescarga").modal("hide");
            MensErr("El tiempo ha terminado. Debe configurar nuevamente los Cupos con Descarga.\n\n");
        }

        seconds = seconds < 10 ? '0' + seconds : seconds;
        minutes = minutes < 10 ? '0' + minutes : minutes;

        $("#mensaje-flotante").empty();
        textoMensajeFlotante = `<p>${minutes}:${seconds}</p>`;
        $("#mensaje-flotante").append(textoMensajeFlotante);

        countdownEl.innerHTML = `${minutes}:${seconds}`;
        time--;
    }
}

function detenerIntervalo() {
    clearInterval(intervalId);
    intervalId = null;
    intervalActivo = false;
    $("#mensaje-flotante").empty();
    $("#mensaje-flotante").hide();
}

//-------------------------------------------------------
function AbrirConDescarga() {

    if (!ControlesAccesoConDescarga()) {
        $("#conDescargaId").prop("checked", false);
        $("#btnConDescarga").hide();
        return;
    }
    $("#btnConDescarga").show();

    var objeto = {
        fechaDesdeNegocio: $("#fechaDesdeId").val(),
        fechaHastaNegocio: $("#fechaHastaId").val(),
        materialId: parseInt($("#material").val()),
        centroId: parseInt($("#destinoId").val()),
        comercialId: $("#comercialId").val(),
    }
    var listDiasCuposConDescarga = MSExecuteOnServer('/CompraNet/CantidadDiasCuposConDescarga', objeto);

    $("#cuerpo-carga-cupos").empty();
    var fila = '';
    for (var i = 0; i < listDiasCuposConDescarga.length; i++) {

        var fechaCupo = new Date(parseInt(listDiasCuposConDescarga[i].Fecha.substr(6)));
        fechaCupo = FormatearFecha(formatearFecha(fechaCupo))

        var item = dataTabla.find(x => x.Fecha == fechaCupo);

        if (item != undefined) {
            fila = '<tr>'
                + '<input name="Dias[' + i + '].Fecha" value="' + kendo.toString(fechaCupo, "dd/MM/yyyy") + '" type="hidden"/>'
                + '<td>' + kendo.toString(fechaCupo, "dd/MM/yyyy") + '</td>'
                + '<td><input id="cupo' + i + '" name="Dias[' + i + '].CantidadCupo" min="0"  class="cantidad-masiva" value="' + parseInt(item.CantidadCupo) + '"/> </td>'
                + '<td><input id="flete' + i + '" name="Dias[' + i + '].CantidadFlete" min="0" class="cantidad-masiva" value="' + parseInt(item.CantidadFlete) + '"/> </td>'
                + '<input name="Dias[' + i + '].CuposDisponibles" value="' + listDiasCuposConDescarga[i].CuposDisponibles + '" type="hidden"/>'
                + '<td> Cupos máx.: ' + listDiasCuposConDescarga[i].CuposDisponibles + '</td>'
                + '</tr>';
        } else {
            fila = '<tr>'
                + '<input name="Dias[' + i + '].Fecha" value="' + kendo.toString(fechaCupo, "dd/MM/yyyy") + '" type="hidden"/>'
                + '<td>' + kendo.toString(fechaCupo, "dd/MM/yyyy") + '</td>'
                + '<td><input id="cupo' + i + '" name="Dias[' + i + '].CantidadCupo" min="0"  class="cantidad-masiva" value="' + 0 + '"/> </td>'
                + '<td><input id="flete' + i + '" name="Dias[' + i + '].CantidadFlete" min="0" class="cantidad-masiva" value="' + 0 + '"/> </td>'
                + '<input name="Dias[' + i + '].CuposDisponibles" value="' + listDiasCuposConDescarga[i].CuposDisponibles + '" type="hidden"/>'
                + '<td> Cupos máx.: ' + listDiasCuposConDescarga[i].CuposDisponibles + '</td>'
                + '</tr>';
        }

        $("#cuerpo-carga-cupos").append(fila);
    }

    $(".cantidad-masiva").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        step: 0
    });

    $("#comenzarCarga").click();
    $("#modalCargarCuposConDescarga").modal('show');
}

function EsConDescarga() {
    if ($("#conDescargaId").is(":checked") == false) {
        $("#btnConDescarga").hide();
        detenerIntervalo();
        LiberarPantalla();
        if (dataTabla.find(x => x.CantidadFlete > 0 || x.CantidadCupo > 0)) {
            dataTabla = [];
            MensAlerta("La configuración de Cupos con Descarga se ha reestablecido.\n\n");
        }
        return;
    }

    if ($('#buscadorProveedor').val() == "") {
        $("#conDescargaId").prop("checked", false);
        $("#btnConDescarga").hide();
        MensErr("Es obligatorio elegir un proveedor para configurar cupos con descarga.\n\n");
        return;
    }
    if ($('#fechaDesdeId').val() == "" || $('#fechaHastaId').val() == "") {
        $("#conDescargaId").prop("checked", false);
        $("#btnConDescarga").hide();
        MensErr("Es obligatorio ingresar las fechas desde y hasta para configurar cupos con descarga.\n\n");
        return;
    }
    if ($('#cantidadId').val() == 0 || $('#cantidadId').val() == "") {
        $("#conDescargaId").prop("checked", false);
        $("#btnConDescarga").hide();
        MensErr("Es obligatorio ingresar una cantidad de kilos para configurar cupos con descarga.\n\n");
        return;
    }
    if ($('#comercialId').val() == null) {
        $("#conDescargaId").prop("checked", false);
        $("#btnConDescarga").hide();
        MensErr("Es obligatorio elegir un comercial para configurar cupos con descarga.\n\n");
        return;
    }

    if ($("#conDescargaId").is(":checked") == true) {
        AbrirConDescarga();
    } else {
        $("#btnConDescarga").hide();
    }
}

function ControlesAccesoConDescarga() {
    var todoOK = false;
    var error = false;
    var objeto = ObtenerDatos(error);

    result = MSExecuteOnServer('/CompraNet/ControlesAccesoConDescarga', objeto);
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
            return false;
        }
        else todoOK = true;
    }

    result = MSExecuteOnServer('/CompraNet/ValidarPantallaEnUso', { NombrePantalla: nombrePantalla, usar: true });
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
            return false;
        }
        else todoOK = true;
    }

    return todoOK;
}

function LiberarPantalla() {
    var todoOK = false;

    result = MSExecuteOnServer('/CompraNet/ValidarPantallaEnUso', { NombrePantalla: nombrePantalla, usar: false });
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
            return false;
        }
        else todoOK = true;
    }

    return todoOK;
}

function guardarCuposConDescarga() {
    var dataTablaTemp = [];

    var table = document.getElementById("cuerpo-carga-cupos");
    var superaCuposMaximoDia = false;
    var cantidadTotalCuposFletes = 0;

    for (let i = 0, n = table.rows.length; i <= (n - 1); i++) {
        if ($('[name="Dias[' + i + '].CantidadCupo"]').val() > 0 || $('[name="Dias[' + i + '].CantidadFlete"]').val() > 0) {

            var cuposDisponibles = $('[name="Dias[' + i + '].CuposDisponibles"]').val() == "" ? 0 : parseInt($('[name="Dias[' + i + '].CuposDisponibles"]').val());
            var cantidadCupo = $('[name="Dias[' + i + '].CantidadCupo"]').val() == "" ? 0 : parseInt($('[name="Dias[' + i + '].CantidadCupo"]').val());
            var cantidadFlete = $('[name="Dias[' + i + '].CantidadFlete"]').val() == "" ? 0 : parseInt($('[name="Dias[' + i + '].CantidadFlete"]').val());

            var cantidadCupoFleteDia = cuposDisponibles - cantidadCupo - cantidadFlete;
            if (cantidadCupoFleteDia < 0) superaCuposMaximoDia = true;

            cantidadTotalCuposFletes += cantidadCupo + cantidadFlete;

            dataTablaTemp.push(
                {
                    Fecha: $('[name="Dias[' + i + '].Fecha"]').val(),
                    CantidadCupo: $('[name="Dias[' + i + '].CantidadCupo"]').val(),
                    CantidadFlete: $('[name="Dias[' + i + '].CantidadFlete"]').val()
                });
        }
    }

    if (superaCuposMaximoDia) {
        MensErr("La cantidad de cupos para uno de los días supera la cantidad disponible en su zona.\n\n");
        return;
    }
    var cantidadCuposFletesPermitidos = Math.ceil($("#cantidadId").val() / 30000);
    if (cantidadTotalCuposFletes > cantidadCuposFletesPermitidos) {
        MensErr("La cantidad de cupos ingresados se excede con respecto a los kilos del negocio.\n\n");
        return;
    }

    dataTabla = dataTablaTemp;
    $("#modalCargarCuposConDescarga").modal("hide");
}

// PARA VISUALIZAR STOCK SI ES EPA, SUSTENTABLE O EUDR

function mostrarResultados(result) {
    var error = new Array();
    var cupos = new Array();

    if (result.HayError) {
        error = error.concat(result.ListaErrores);
    }
    if (result.ListaCupos != null && result.ListaCupos.length > 0) {
        cupos = cupos.concat(result.ListaCupos);
    }

    if (cupos.length > 0) {
        var table = "";
        if ($("#sustentableId").is(":checked") || $("#epaId").is(":checked") || $("#eudrId").is(":checked")) {
            var result2 = MSExecuteOnServer('/Cupo/TraerEstablecimientos', { cuitProveedor: $("#proveedorId").val(), esEPAoEUDR: $("#epaId").is(":checked") || $("#eudrId").is(":checked") });
            if (result2 != null && result2.length > 0) {
                table = "<tr>";
                table += '<th colspan = "3">Cosecha ' + result2[0].Cosecha + '</th>';
                table += "</tr>";
                table += "<tr>";
                table += "<th> Establecimiento</th>"
                table += "<th> Cantidad (Kg)</th>"
                table += "<th> Localidad (Provincia) </th>"
                table += "</tr>";
                for (var i = 0; i < result2.length; i++) {
                    table += "<tr>";
                    table += '<td>' + result2[i].Establecimiento + '</td>';
                    table += '<td>' + kendo.toString(result2[i].Cantidad, "n0") + '</td>';
                    table += '<td>' + result2[i].Localidad + ' (' + result2[i].Provincia + ')' + '</td>';
                    table += "</tr>";
                }
            }
        }

        cuposCreados(cupos, table);
    }
    if (error.length > 0) {
        ShowErrorMessages(error);
    }
}

function cuposCreados(lista, table) {
    $("#cupos-generados-modal").html(lista.join("</br>"));

    if (table != "") {
        $("#cargarDatosEstablecimiento2").html(table);
    }

    $('#resultadoCupo').modal('toggle');
}

function copiarGenerados(idDiv) {
    var listaCupos = $("#" + idDiv).html().replace(/<br>/g, "\n");
    listaCupos = listaCupos.split('<strong>').join("");
    listaCupos = listaCupos.split('</strong>').join("");

    var copiarEstablecimientos = document.getElementById("cargarDatosEstablecimiento2").innerText;

    listaCupos += "\n\n" + copiarEstablecimientos;

    //como un replaceall 
    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', listaCupos);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', listaCupos);
        }
    };
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
}

function cancelarCuposConDescarga() {
    var table = document.getElementById("cuerpo-carga-cupos");

    for (let i = 0, n = table.rows.length; i <= (n - 1); i++) {

        var fecha = $('[name="Dias[' + i + '].Fecha"]').val();
        var item = dataTabla.find(x => x.Fecha == fecha);

        if (item != undefined) {
            if (parseInt(item.CantidadCupo) > 0 || parseInt(item.CantidadFlete) > 0) {
                $('[name="Dias[' + i + '].CantidadCupo"]').data("kendoNumericTextBox").value(item.CantidadCupo);
                $('[name="Dias[' + i + '].CantidadFlete"]').data("kendoNumericTextBox").value(item.CantidadFlete);
            }
        }
    }

    $("#modalCargarCuposConDescarga").modal("hide");
}

function ValidarCapacidadProductiva() {
    var cuitAux = $("#buscadorProveedor").val().split('(');
    var negocio = {
        MaterialId: $("#material").val(),
        Material: { Descripcion: $('#material').data("kendoDropDownList").dataItem().Descripcion },
        CampanaId: $("#campanaId").val(),
        Campana: { Descripcion: $('#campanaId').data("kendoDropDownList").dataItem().Descripcion },
        ProveedorId: $("#proveedorId").val(),
        Proveedor: { RazonSocial: cuitAux[0].trim() }
    };
    var result = MSExecuteOnServer('/CompraNet/ValidarCapacidadProductiva', negocio);
    if (result.HayError)
        AlertaCapProd(result.Errores[0].Message)
}

function AlertaCapProd(mensaje) {
    BootstrapDialog.show({
        title: 'Alerta de Capacidad Productiva',
        message: "\n" + mensaje,
        draggable: true,
        type: BootstrapDialog.TYPE_WARNING,
        buttons: [
            {
                label: 'Ir a Cargar Informe',
                cssClass: 'k-button',
                action: function () {
                    sessionStorage.setItem('pantallaActiva', 'produccion');
                    window.location.href = window.location.origin + "/Proveedor/Agregar?ProveedorId=" + $("#proveedorId").val();
                }
            },
            {
                label: 'Volver a CompraNet',
                cssClass: 'k-button',
                action: function () {
                    window.location.href = window.location.origin + "/CompraNet";
                }
            }
        ]
    });
}