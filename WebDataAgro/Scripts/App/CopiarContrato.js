

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
        
        modificarContrato(contratoCopia);
        CargarDatosEditar(contratoCopia);
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
    obj.MaterialId = $("#material").val() == null || $("#material").val() == undefined || $("#material").val() == "" ? 0 : $("#material").val();
    obj.Cantidad = $("#cantidadId").val() == null || $("#cantidadId").val() == undefined || $("#cantidadId").val() == "" ? 0 : $("#cantidadId").val(); 
    obj.Precio = $("#precioId").val() == null || $("#precioId").val() == undefined || $("#precioId").val() == "" ? 0 : $("#precioId").val();
    obj.PrecioNeto = $("#precioTotalApertura").val();
    obj.FechaEntrega = $("#fechaHastaId").val() == null || $("#fechaHastaId").val() == undefined || $("#fechaHastaId").val() == "" ? formatearFecha(maniana) : FormatearFecha($("#fechaHastaId").val());
    obj.CampanaId = $("#campanaId").val();
    if (TipoId != "3") {
        obj.FechaDesde = $("#fechaDesdeId").val() == null || $("#fechaDesdeId").val() == undefined || $("#fechaDesdeId").val() == "" ? formatearFecha(hoy) : FormatearFecha($("#fechaDesdeId").val());
        obj.FechaHasta = $("#fechaHastaId").val() == null || $("#fechaHastaId").val() == undefined || $("#fechaHastaId").val() == "" ? formatearFecha(maniana) : FormatearFecha($("#fechaHastaId").val());
    } else {
        obj.FechaDesde = formatearFecha(hoy);
        obj.FechaHasta = formatearFecha(maniana);
    }
    obj.Fecha = $("#fechaFijacionId").val() == null || $("#fechaFijacionId").val() == undefined || $("#fechaFijacionId").val() == "" ? formatearFecha(hoy) : FormatearFecha($("#fechaFijacionId").val());
    obj.MonedaId = $("#precioMonedaId").val();
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
    obj.FechaDolarizado = $("#tipoId").val() == "1" ? "" :$("#dolarizadoFechaId").val();
    obj.PagoDiferidoContrato = $("#pesificadoId").is(":checked") ? true : false;
    obj.PagoDiferido = obj.TipoNegocioId != 3 ? $("#pesificadoId").is(":checked") ? true : false : $("#diasDiferidoId").is(":checked") ? true : false;
    obj.Dolarizado = $("#tipoId").val() == "1" ? false: $("#dolarizadoId").is(":checked") ? true : false;
    obj.Sustentable = $("#sustentableId").is(":checked") ? true : false;
    obj.DiasPesificado = obj.TipoNegocioId != 3 ? $("#pesificadoDiasId").val() : $("#diasDiferidoFijacionId").val();
    obj.PorcentajeComision = $("#porcentajeComision").val() != "" ? $("#porcentajeComision").val() : 0;
    obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.EstadoId = $("#estado").val() == "" ? $("#baseId").is(":checked") ? "3" : obj.TipoNegocioId != 4 && obj.TipoNegocioId != 5 && obj.TipoNegocioId != 6 ? "1" : "2" : $("#estado").val();
    obj.Observacion = $("#observacionId").val();
    obj.ClasificacionId = $("#clasificacion").val() == null || $("#clasificacion").val() == undefined || $("#clasificacion").val() == "" ? 0 : $("#clasificacion").val();
    obj.CantidadCamiones = $("#cantidadCamionesId").val() == null || $("#cantidadCamionesId").val() == undefined || $("#cantidadCamionesId").val() == "" ? 0 : $("#cantidadCamionesId").val();
    obj.EstablecimientoPropio = $("#establecimientoPropioId").is(":checked") ? true : $("#establecimientoArrendadoId").is(":checked") ? false : null;
    obj.DesdeFijacion = $("#fechaDesdeTopeId").val() == null || $("#fechaDesdeTopeId").val() == undefined || $("#fechaDesdeTopeId").val() == "" ? formatearFecha(hoy) : $("#fechaDesdeTopeId").val(); 
    obj.HastaFijacion = $("#fechaHastaTopeId").val() == null || $("#fechaHastaTopeId").val() == undefined || $("#fechaHastaTopeId").val() == "" ? formatearFecha(maniana) : $("#fechaHastaTopeId").val();
    obj.CondicionFijacionId = $("#condicionFijacionId").val();
    obj.DestinoId = $("#destinoId").val();
    obj.planCanje = $("#planCanjeId").is(":checked") ? true : false;
    obj.Consignatario = $("#consignatarioId").is(":checked") ? true : false;
    obj.CD = $("#CDId").is(":checked") ? true : false;
    obj.Warrant = $("#WarrantId").is(":checked") ? true : false;
    obj.PagoDirectoVendedor = $("#pagoDirectoId").is(":checked") ? true : false;
    obj.MercsDeposito = $("#mercsDepositoId").is(":checked") ? true : false;
    obj.NivelTarifaId = $("#NivelTarifaId").val();
    obj.TarifaFlete = $("#TarifaFleteId").val();
    obj.ChequeElectronico = $("#tipoId").val() == "2" ? $("#chequeElectronicoInput").is(":checked") ? true : false : $("#chequeElectronico").is(":checked") ? true : false;
    obj.DolarizadoExpress = $("#tipoId").val() == "1" ? false :$("#tipoId").val() == "3" ? $("#expressId").is(":checked") ? true : false : $("#dolarizadoExpressId").is(":checked") ? true : false;
    obj.PagoCBU = $("#tipoId").val() == "2" ? $("#pagoCbu").val() : $("#pagoCbuInput").val();

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
    var proveedorId;
    var corredorId;
    if ($("#buscadorProveedor").val() != "") {
        var cuitAux = $("#buscadorProveedor").val().split('(');      
        if (cuitAux[1]) {
            var cuit = cuitAux[1].split(')');
            proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0], corredor: false });
            if (proveedorId == null || proveedorId == 0) {
                MensErr("No se pudo obtener el proveedor, verificar la segmentación.");
                $.unblockUI();
                return;
            }
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

    obj.ProveedorId = proveedorId;
    obj.CorredorId = corredorId;

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

    obj.OperadorId = $("#operadorId").val();
    if ($("#madreId").is(":checked")) {
        obj.Madre = true;
    }
    if ($("#hijoId").is(":checked")) {
        obj.Madre = false;
    } else {
        if (obj.TipoNegocioId == 2) {
            obj.FechaCierta = $("#fechaCiertaId").val();
        }
        if (obj.TipoNegocioId == 6) {
            obj.FechaCierta = $("#fechaCiertaAcuerdo").val();

        }
    } if ($("#fasonIdCheck").is(":checked")) {
        obj.EsFason = true;
    }
    obj.ContratoMadre = $("#contMadreId").val();
    obj.Descuentos = viewModel.Descuentos;

    obj.Especial = $("#trigoEspecialFasonId").is(":checked") ? true : false;
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
        else if (obj.MaterialId == 1 || obj.MaterialId == 2) obj.StandardDeCalidadId = 1;
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
    if (obj.TipoNegocioId == 1 || obj.TipoNegocioId == 2) {
        obj.FechaOperacion = $("#fechaOperacionId").val() == null || $("#fechaOperacionId").val() == undefined || $("#fechaOperacionId").val() == "" ? formatearFecha(hoy) : $("#fechaOperacionId").val();
        if ($('#motivoAnterior').data("kendoDropDownList").text() == "Otro" || $('#motivoAnterior').data("kendoDropDownList").value() == "") {
            obj.MotivoOperacionAnterior = $("#motivoOperacionAnteriorId").val();
        } else {
            $("#motivoOperacionAnteriorId").val($('#motivoAnterior').data("kendoDropDownList").text());
            obj.MotivoOperacionAnterior = $("#motivoOperacionAnteriorId").val();
        }
      
    }
    if (obj.TipoNegocioId == 3) {
        obj.FechaOperacion = $("#fechaFijacionId").val() == null || $("#fechaFijacionId").val() == undefined || $("#fechaFijacionId").val() == "" ? formatearFecha(hoy) : $("#fechaFijacionId").val(); 

        if ($('#motivoAnterior').data("kendoDropDownList").text() == "Otro" || $('#motivoAnterior').data("kendoDropDownList").value() == "") {
            obj.MotivoOperacionAnterior = $("#motivoOperacionAnteriorFijacion").val();
        } else {
            $("#motivoOperacionAnteriorFijacion").val($('#motivoAnterior').data("kendoDropDownList").text());
            obj.MotivoOperacionAnterior = $("#motivoOperacionAnteriorFijacion").val();
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

    obj.Anticipo = $("#Anticipo").val();
    obj.Cesion = $("#Cesion").val();
    obj.ClasificacionContrato = $("#ClasificacionContrato").val();
    obj.Clasificacion = $("#ClasificacionContrato").val();
    obj.ImporteAPrecioContrato = $("#ImporteAPrecioContrato").val();
    obj.PorcentajeAPrecioContrato = $("#PorcentajeAPrecioContrato").val();
    obj.MonedaAPrecioContrato = $("#MonedaAPrecioContrato").val();
    obj.ImporteSobrePrecioContrato = $("#ImporteSobrePrecioContrato").val();
    obj.PorcentajeSobrePrecioContrato = $("#PorcentajeSobrePrecioContrato").val();
    obj.MonedaSobrePrecioContrato = $("#MonedaSobrePrecioContrato").val();
    if ($("#ventaId").is(":checked") == true) {
        obj.Venta = true;
    }
    return obj;
}

function RedireccionarNegocio(url, tipoId, obj) {
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
}

function InicializarDatos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniCrearContrato = data;
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
    if ($("#buscadorProveedor").val() != "") {
        var id = $("#proveedorId").val() != "" ? $("#proveedorId").val() : 0;
        var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: id });
        if (compraNet.BoletoCompraNetId !== null) {
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
        }
    }
}


function MostrarCcPpPendientesAplicar() {
    if ($("#mercsDepositoId").is(":checked") && $("#sustentableId").is(":checked")) {
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
        } else {
            $("#fechaDesdeSustentableId").val(lista[0].FechaIngreso);
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
    if ($("#AgenteCompraId").data("kendoDropDownList").value() == "" && ($("#tipoId").data("kendoDropDownList").value() == "2") || $("#tipoId").data("kendoDropDownList").value() == "1") {
        $(".ocultarAgenteDiv").hide();
        $("#caratulaExtensionId").val("");
        $("#caratulaMATId").val("");
        $("#precioAjusteComisionId").data("kendoNumericTextBox").value("");
        $("#monedaAjusteComisionId").data("kendoDropDownList").value("");
    } else {
        $(".ocultarAgenteDiv").show();
    }
}
