var viewModel;
var datosIniCrearContrato;
var contratoEdit;
var Id;
var Siguientes;
var posicionFijacion;
var aperturaPrecio = [];
var altaTemprana;
var AperturaPrecioPorcentajeDeComision;
var ImporteSobrePrecio = 0;
var MonedaSobrePrecio = "";
var PorcentajeSobrePrecio = 0;
var esEdicion;
var boletoId;

$(document).ready(function () {
    $('#menuproveedor').hide();
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    CrearViewModel();
    InicializarElementos();
    InicializarDatos();
    AutocompleteProcedencia();
    $("#tipoId").data("kendoDropDownList").value(1);
    $("#tipoId").data("kendoDropDownList").trigger("change");
    $("#precioId").kendoNumericTextBox({});
    $("#precioTotalApertura").kendoNumericTextBox({});
    var precioRojo = $("#precioId").hasClass("required-box-parent") ? $("#precioId") : $("#precioId").parent().parent();
    precioRojo.removeClass("required-border");
    $("#porcentajeLabel").html("Total");
    $(".noAFijar").hide();
    $("#precioId").data("kendoNumericTextBox").value("");
    $("#precioTotalApertura").data("kendoNumericTextBox").value("");
    $(".precioMonedaAFijarIdDiv").show();
    OcultarCamposAgente();
});

$(document.body).delegate('[type="checkbox"][readonly="readonly"]', 'click', function (e) {
    e.preventDefault();
});

function InicializarBordesRojos() {
    $("select.required-box, input.required-box").on("change", function (e) {
        var padre = $(this).hasClass("required-box-parent") ? $(this) : $(this).parent().parent();

        if ($(this).val() == "") {
            padre.addClass("required-border");
        } else {
            padre.removeClass("required-border");
        }
    });

    $("select.required-box, input.required-box").trigger("change");
}

function InicializarFondosGrises() {
    $(".gris").addClass("no-border").prop('disabled', true);
    //$("#precioId").data("kendoNumericTextBox").value("");
    //$("#precioTotalApertura").data("kendoNumericTextBox").value("");
    //$("#precioMonedaId").data("kendoDropDownList").enable(false);
    //$("#precioMonedaId").data("kendoDropDownList").value("");
    $("div.col-xs-12.col-sm-5.col-md-3.required-box-parent > span.k-widget.k-dropdown.k-header > span").addClass('no-border');
}

function RemoverFondosGrises() {
    $(".gris").removeClass("no-border").prop('disabled', false);
    //$("#precioMonedaId").data("kendoDropDownList").enable(true);
    $("div.col-xs-12.col-sm-5.col-md-3.required-box-parent > span.k-widget.k-dropdown.k-header > span").removeClass('no-border');
    $("select.required-box, input.required-box").trigger("change");
}

function ObtenerFechaDesde(fechaBase) {
    var hoy = fechaBase != undefined ? fechaBase : new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

function ObtenerFechaHasta(fechaBase) {
    var hoy = fechaBase != undefined ? fechaBase : new Date();
    var anio = hoy.getFullYear();
    var mesPost = hoy.getMonth() + 2;
    var dia = hoy.getDate();
    var ultimoDia = new Date(anio, hoy.getMonth() + 1, 0).getDate();

    if (dia === 1) {
        dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
        mesPost = hoy.getMonth() + 1;
    }
    if (dia === ultimoDia || (mesPost === 2 && dia >= 29)) {
        dia = new Date(anio, mesPost, 0).getDate();
    }
    if (mesPost === 13) {
        mesPost = 1;
        anio += 1;
    }
    if (mesPost < 10) {
        mesPost = "0" + mesPost.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mesPost + '-' + anio;
}

function cargarContratoAFijarSeleccionado() {
    var cuitProv = $("#buscadorProveedor").val().split('(');
    if (cuitProv[1] != null) {
        var cuitP = cuitProv[1].split(')');
    }
    else {
        cuitP = cuitProv;
    }
    var cuitCorr = $("#buscadorCorredor").val().split('(');
    if (cuitCorr[1] != null) {
        var cuitC = cuitCorr[1].split(')');
    }
    else {
        cuitC = cuitCorr;
    }
    Id = Id != "" ? Id : 0;
    var datos = { cuitProveedor: cuitP[0], cuitCorredor: cuitC[0], materialId: $('#material').data("kendoDropDownList").value(), filtro: $('#contratoId').val(), fijacionId: Id };
    var afijar = MSExecuteOnServer('/CompraNet/ObtenerFijacionesAutomaticas', datos);
    cargarDatosAFijarEnFijacion(afijar[0])
}

function cargarDatosAFijarEnFijacion(afijar) {
    $(".datoscontrato").show();
    $("#datosContrato").show();
    $("#kgspendientescontrato").text(afijar.KilosPendiente);
    $("#kgsaplicadoscontrato").text(afijar.KilosAplicados);
    $("#desdecontrato").text(afijar.FechaDesde);
    $("#hastacontrato").text(afijar.FechaHasta);

    $("#posicionFasonId").val(afijar.Posicion);
    $("#posicionCBOTId").val(afijar.PosicionCBOT);
    $("#tipoPosicionCBOTId").data("kendoDropDownList").value(afijar.TipoPosicionCBOTId);
    if (afijar.PosicionCBOT != null && afijar.PosicionCBOT != "") {
        $("#posicionCBOTDiv").show();
    }
    $("#fechaOperacionId").val(afijar.FechaOperacion);
    $("#motivoOperacionAnteriorId").val(afijar.MotivoOperacionAnterior);
    $("#fechaDesdeId").val(afijar.DesdeEntrega);
    $("#fechaHastaId").val(afijar.HastaEntrega);
    $("#campanaId").data("kendoDropDownList").text(afijar.Campana);
    $("#destinoId").data("kendoDropDownList").value(afijar.Centro);
    afijar.Calidad === true ? $("#trigoEspecialFijacion").prop("checked", true) : $("#trigoEspecialFijacion").prop("checked", false);
    //afijar.PagoDiferido === true ? $("#pesificadoId").prop("checked", true) : $("#pesificadoId").prop("checked", false);

    if (afijar.ImporteAPrecio == 0) {
        $("#impo-a-precio").hide();
    } else {
        $("#impo-a-precio").show();
    }
    $("#importe-a-precio").text(afijar.ImporteAPrecio + " " + afijar.MonedaAPrecio + " ");
    if (afijar.ImporteSobrePrecio == 0) {
        $("#impo-sobre-precio").hide();
    } else {
        $("#impo-sobre-precio").show();
    }
    $("#importe-sobre-precio").text(afijar.ImporteSobrePrecio + " " + afijar.MonedaSobrePrecio + " ");
    if (afijar.PorcentajeAPrecio == 0) {
        $("#porcenteaje-a-precio").hide();
    } else {
        $("#porcenteaje-a-precio").show();
    }
    $("#porc-a-precio").text(afijar.PorcentajeAPrecio + " ");
    if (afijar.PorcentajeSobrePrecio == 0) {
        $("#porcenteaje-sobre-precio").hide();
    } else {
        $("#porcenteaje-sobre-precio").show();
    }
    $("#porc-sobre-precio").text(afijar.PorcentajeSobrePrecio);
    $("#cond-fijacion").text(afijar.CondicionFijacionDescripcion);
    $("#cond-pago").text(afijar.CondicionPagoDescripcion);

    if (afijar.ImporteSobrePrecio != 0 || afijar.PorcentajeSobrePrecio != 0) {
        ImporteSobrePrecio = afijar.ImporteSobrePrecio;
        MonedaSobrePrecio = afijar.MonedaSobrePrecio;
        PorcentajeSobrePrecio = afijar.PorcentajeSobrePrecio;
        //$("#ocultarAperturaBtn").hide();
        //$("#ocultarAperturaMoneda").removeClass("w70");
        //$("#ocultarAperturaMoneda").addClass("w100");
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").readonly();
        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").readonly();
        $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").readonly();
        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").readonly();
        $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").readonly();
        CalcularPrecioTotalApertura();
    } else {
        ImporteSobrePrecio = 0;
        MonedaSobrePrecio = "";
        PorcentajeSobrePrecio = 0;
        //$("#ocultarAperturaBtn").show();
        //$("#ocultarAperturaMoneda").removeClass("w100");
        //$("#ocultarAperturaMoneda").addClass("w70");
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").readonly(false);
        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").readonly(false);
        $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").readonly(false);
        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").readonly(false);
        $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").readonly(false);
    }
}

function InicializarElementos() {
    kendo.culture("es-AR");
    cargaFijacionAyer = ConvertirStringABool(cargaFijacionAyer);
    $('[data-toggle="popover"]').popover();
    $(".datos-adicionales").hide();
    $(".datos-boleto").hide();
    $(".datos-establecimiento").hide();
    $(".datos-topesplazos").hide();
    $(".datos-calidades").hide();
    $(".datos-descuentos").hide();
    $(".ocultar").hide();
    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
        //if ($("#tipoId").val() == "6") {
        //    $("#dolarizadoExpressDiv").hide();
        //}
    });

    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb #: data.Deshabilitar ? \'k-state-disabled\': \'\' #"  style="color:#: data.Color#">#: data.RazonSocial#(#: data.Cuit#)#if(data.Estado != null) {# ' +
            ' #: data.Estado #    #}else{# #}# </p >',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            HayCanje();
            if ($("#buscadorProveedor").val().split('|').length > 1) {
                $("#buscadorProveedor").val($("#buscadorProveedor").val().split('|')[1]);
            }
            $("#contratoId").val("");
            $(".datoscontrato").hide();
            $("#datosContrato").hide();
            $("#pagoCbuInput").val("");
            $("#pagoCbu").val("");
            //if ($("#tipoId").val() == "6") {
            //    $("#dolarizadoExpressDiv").hide();

            //}
            InicializarBordesRojos();
            DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
        },
        select: function (e) {
            if (e.dataItem.Deshabilitar) {
                $("#buscadorProveedor").val("")
                $("#mensaje").hide();
                $("#mensaje").val("");
                e.preventDefault();
            } else {
                ObtenerAlta(e.dataItem.Id);
                ValidarFason();
                var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: e.dataItem.Id });
                if ($("#estado").val() !== "5") {
                    if ($("#tipoId").val() != 3 && (Id == 0 || Id == null || Id == "")) {
                        $("#clasificacion").data("kendoDropDownList").value("");
                        $("#consignatarioId").prop("checked", false);
                    }

                    $("#proveedorId").val(compraNet.ProveedorId);
                    $("#clasificacion").data("kendoDropDownList").value(compraNet.ClasificacionCompraNetId);
                    if ($("#clasificacion").val() == 2 || $("#clasificacion").val() == 3) {
                        $("#consignatarioId").prop("checked", compraNet.Consignatario);
                        $("#planCanjeId").prop("checked", compraNet.PlanCanje);
                    }
                    $("#clasificacion").data("kendoDropDownList").trigger("change");
                    if (compraNet.LocalidadId != null) {
                        if (compraNet.LocalidadId != "" && compraNet.ProvinciaId != "") {
                            $("#ProvinciaId").val(compraNet.ProvinciaId);
                            $("#LocalidadCrearContrato").val(compraNet.Localidad + " (" + compraNet.Provincia + ")");
                            HabilitarEstablecimiento();
                        } else {
                            $("#LocalidadCrearContrato").val("");
                            $("#ProvinciaId").val("");
                            HabilitarEstablecimiento();
                        }
                    }

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
                        } else if (compraNet.BoletoCompraNetId === 5) {
                            $("#sinBoletoId").prop("checked", true);
                            ValidarSinBoleto();
                        }
                    }
                    if ($("#tipoId").val() == 3) {
                        compraNet.ComisionPorcentaje = 0;
                    }

                    SeleccionAutomaticaBolsa();
                    ValidarSinBoleto();
                }
                //if ($("#tipoId").val() == "6") {
                //    $("#dolarizadoExpressDiv").hide();

                //}
                if (ValidarComisionEnCentro()) {
                    CargarAutomaticamenteLaComision(compraNet);
                } else {
                    BorrarComisionSiEsAcopio();
                }
                EsComisionista(compraNet);
                ActivarBoletoXAgentedeCompras($("#AgenteCompraId").val());
                CompletarCantidadDisponibleDeposito();
                //$("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(compraNet.ComisionPorcentaje && !$("#buscadorCorredor").val() ? Number(compraNet.ComisionPorcentaje) : 0);
                //InsertarAperturasViewModel(CalcularPrecioTotalApertura());

            }
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarProveedoresConCorredor"
                },
                parameterMap: function (data, type) {
                    var cuitAux = $("#buscadorCorredor").val().split('(');
                    if (cuitAux[1] != null) {
                        var cuit = cuitAux[1].split(')');
                    }
                    else {
                        cuit = cuitAux;
                    }
                    return { filtro: cuit[0], filtroProveedor: $('#buscadorProveedor').val(), corredor: 0, agenteCompraId: $("#AgenteCompraId").val() };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

    $("#buscadorCorredor").click(function () {
        $("#buscadorCorredor").data("kendoAutoComplete").value("");
        $("#mensaje").hide();
        $("#mensaje").val("");
        $("#buscadorCorredor").data("kendoAutoComplete").trigger("change");
        $("#porcentajeComision").data("kendoNumericTextBox").value("");
    });
    $("#buscadorCorredor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb  #: data.Deshabilitar ? \'k-state-disabled\': \'\' #"  style="color:#: data.Color#">#: data.RazonSocial#(#: data.Cuit#) #if(data.Estado != null) {# ' +
            ' #: data.Estado #    #}else{# #}# </p > ',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#buscadorCorredor").val().split('|').length > 1) {
                $("#buscadorCorredor").val($("#buscadorCorredor").val().split('|')[1]);
                if ($("#tipoId").val() != 3 && (Id == 0 || Id == null || Id == "")) {
                    $("#clasificacion").data("kendoDropDownList").value("");
                    $("#consignatarioId").prop("checked", false);
                }
            }
            if ($("#buscadorCorredor").val() == "") {
                $("#porcentajeComisionDiv").hide();
                $("#contCorredorId").val("");
                $("#contCorredorDiv").hide();
                $("#pagoDirectoDiv").hide();
                $("#pagoDirectoId").prop("checked", false);
                //if (!$("#dolarizadoId").is(":checked") && /*$("#precioMonedaId").val() == "USDM " &&*/ /*$("#fechaCiertaId").val() == "" &&*/ $("#tipoId").val() != "6") {
                //    $("#dolarizadoExpressDiv").show();
                //    $("#dolarizadoExpressId").prop("disabled", false);

                //}
                //if (!$("#dolarizadoExpressId").is(":checked")) {
                //    $("#dolarizadoId").prop("disabled", false);           
                //    $("#dolarizadoDiv").hide();
                //    $("#dolarizadoFechaId").val("");
                //}
                if ($("#AgenteCompraId").val() == "" && !$("#chequeElectronicoInput").is(":checked")) {
                    $("#pagoDirectoDiv").hide();
                    $("#pagoDirectoId").prop("checked", false);
                    $("#pagoCbuDiv").show();
                    if ($("#tipoId").val() == "6" || $("#tipoId").val() == "3") {
                        $("#pagoCbuId").show();
                        $("#pagoCbuDiv").hide();
                    }
                }
            } else {
                //$("#dolarizadoExpressId").prop("checked", false);
                //$("#dolarizadoExpressId").prop("disabled", true);
                $("#pagoCbuDiv").hide();
                $("#pagoCbu").val("");
                $("#pagoCbuId").hide();
                $("#pagoCbuInput").val("");
            }
            $("#buscadorProveedor").val("");
            $("#contratoId").val("");
            $(".datoscontrato").hide();
            $("#datosContrato").hide();
        },
        select: function (e) {
            if (e.dataItem.Deshabilitar) {
                $("#buscadorCorredor").val("")
                e.preventDefault();
            } else {
                if ($("#estado").val() !== "5") {
                    var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: e.dataItem.Id });
                    LimpiarBoleto();

                    if (compraNet.BoletoCompraNetId !== null) {
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
                    //if (compraNet.ComisionPorcentaje != null && compraNet.ComisionPorcentaje != 0) {
                    //    $("#porcentajeComision").data("kendoNumericTextBox").value(compraNet.ComisionPorcentaje);
                    //}
                    //else {
                    $("#porcentajeComision").data("kendoNumericTextBox").value(1);
                    //}
                    $('#porcentajeComisionDiv').show();
                    $('#contCorredorDiv').show();

                    if ($("#tipoId").val() != "3" && $("#tipoId").val() != "6" && $("#boton-ampliar").text() != "+ AMPLIAR") {
                        $('#pagoDirectoDiv').show();
                    }
                }
                SeleccionAutomaticaBolsa();
                ValidarCorredor(e.dataItem.Id);
                $("#corredorId").val(e.dataItem.Id);
            }
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarCorredores"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#buscadorCorredor').val(), corredor: 1, agenteCompraId: $("#AgenteCompraId").val() };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }

    });

    $("#contratoId").kendoAutoComplete({
        template: '<p class="buscar-nomb #if(data.Calidad == true){#subrayadoVerde#}else{}#" style="color:#: data.Color#;"><strong>#: data.ContratoId#</strong> - ' +
            'KG CONTRATO: #: data.KilosContrato# ' +
            /*' - KGS SIN PRECIO : #: data.ARecibirSinPrecio#*/ ' KG APLICADOS: #: data.KilosAplicados# ' +
            ' - KG APLICADOS SIN FIJAR : #: data.RecibidoSinFijar# - KG PENDIENTES A FIJAR: #: data.KilosPendiente# ' +
            ' - Hasta: #: data.FechaHasta# - <strong>#: data.CentroDescripcion#</strong>' +
            ' #if(data.Calidad == true){ #<i style="z-index:10005 !important;" value="true" data-html="true" data-toggle="popover" data-placement="top" data-trigger="hover" data-content="<i><strong>' +
            '#for(var cal = 0; cal < data.Calidades.length; cal++){# ' +
            ' ${data.Calidades[cal].CalidadEspecialDesc} ${data.Calidades[cal].Valor} <br /> ' +
            ' #if(data.Calidades[cal].PorcentajeDesde != null && data.Calidades[cal].PorcentajeHasta != null) {' +
            '  # <li>Porc. Desde #:data.Calidades[cal].PorcentajeDesde# % Hasta #:data.Calidades[cal].PorcentajeHasta# %</li> <br /> # }' +
            ' }# </strong><i>"><i class="fa fa-exclamation-triangle" aria-hidden="true"></i></i># }else { } # </p > ',
        dataBound: function () {
            $('[data-toggle="popover"]').popover({
                container: 'body'
            });
        },
        dataTextField: "Filtro",
        dataValueField: "ContratoId",
        autoWidth: true,
        type: "number",
        filter: "contains",
        change: function () {
            $("#cantidadId").data("kendoNumericTextBox").value("");
            if ($("#contratoId").val().split('|').length > 1) {
                $("#contratoId").val($("#contratoId").val().split('|')[1]);
            }
            $('[data-toggle="popover"]').popover({
                container: 'body'
            });
        },
        open: function (e) {
            $('[data-toggle="popover"]').popover({
                container: 'body'
            });
        },
        select: function (e) {
            cargarDatosAFijarEnFijacion(e.dataItem);
            //$(".datoscontrato").show();
            //$("#datosContrato").show();
            //$("#kgspendientescontrato").text(e.dataItem.KilosPendiente);
            //$("#kgsaplicadoscontrato").text(e.dataItem.KilosAplicados);
            //$("#desdecontrato").text(e.dataItem.FechaDesde);
            //$("#hastacontrato").text(e.dataItem.FechaHasta);

            //$("#posicionFasonId").val(e.dataItem.Posicion);
            //$("#fechaOperacionId").val(e.dataItem.FechaOperacion);
            //$("#motivoOperacionAnteriorId").val(e.dataItem.MotivoOperacionAnterior);
            //$("#fechaDesdeId").val(e.dataItem.DesdeEntrega);
            //$("#fechaHastaId").val(e.dataItem.HastaEntrega);
            //$("#campanaId").data("kendoDropDownList").text(e.dataItem.Campana);
            //$("#destinoId").data("kendoDropDownList").value(e.dataItem.Centro);
            //e.dataItem.Calidad === true ? $("#trigoEspecialFijacion").prop("checked", true) : $("#trigoEspecialFijacion").prop("checked", false);
            //e.dataItem.PagoDiferido === true ? $("#pesificadoId").prop("checked", true) : $("#pesificadoId").prop("checked", false);

            //if (e.dataItem.ImporteAPrecio == 0) {
            //    $("#impo-a-precio").hide();
            //} else {
            //    $("#impo-a-precio").show();
            //}
            //$("#importe-a-precio").text(e.dataItem.ImporteAPrecio + " " + e.dataItem.MonedaAPrecio + " ");
            //if (e.dataItem.ImporteSobrePrecio == 0) {
            //    $("#impo-sobre-precio").hide();
            //} else {
            //    $("#impo-sobre-precio").show();
            //}
            //$("#importe-sobre-precio").text(e.dataItem.ImporteSobrePrecio + " " + e.dataItem.MonedaSobrePrecio + " ");
            //if (e.dataItem.PorcentajeAPrecio == 0) {
            //    $("#porcenteaje-a-precio").hide();
            //} else {
            //    $("#porcenteaje-a-precio").show();
            //}
            //$("#porc-a-precio").text(e.dataItem.PorcentajeAPrecio + " ");
            //if (e.dataItem.PorcentajeSobrePrecio == 0) {
            //    $("#porcenteaje-sobre-precio").hide();
            //} else {
            //    $("#porcenteaje-sobre-precio").show();
            //}
            //$("#porc-sobre-precio").text(e.dataItem.PorcentajeSobrePrecio);
            //$("#cond-fijacion").text(e.dataItem.CondicionFijacionDescripcion);
            //$("#cond-pago").text(e.dataItem.CondicionPagoDescripcion);

            ////
            //if (e.dataItem.ImporteSobrePrecio != 0 || e.dataItem.PorcentajeSobrePrecio != 0) {
            //    ImporteSobrePrecio = e.dataItem.ImporteSobrePrecio;
            //    MonedaSobrePrecio = e.dataItem.MonedaSobrePrecio;
            //    PorcentajeSobrePrecio = e.dataItem.PorcentajeSobrePrecio;
            //    //$("#ocultarAperturaBtn").hide();
            //    //$("#ocultarAperturaMoneda").removeClass("w70");
            //    //$("#ocultarAperturaMoneda").addClass("w100");
            //    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").readonly();
            //    $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").readonly();
            //    $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").readonly();
            //    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").readonly();
            //    $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").readonly();
            //    CalcularPrecioTotalApertura();
            //} else {
            //    ImporteSobrePrecio = 0;
            //    MonedaSobrePrecio = "";
            //    PorcentajeSobrePrecio = 0;
            //    //$("#ocultarAperturaBtn").show();
            //    //$("#ocultarAperturaMoneda").removeClass("w100");
            //    //$("#ocultarAperturaMoneda").addClass("w70");
            //    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").readonly(false);
            //    $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").readonly(false);
            //    $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").readonly(false);
            //    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").readonly(false);
            //    $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").readonly(false);
            //}
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerFijacionesAutomaticas"
                },
                parameterMap: function (data, type) {
                    var cuitProv = $("#buscadorProveedor").val().split('(');
                    if (cuitProv[1] != null) {
                        var cuitP = cuitProv[1].split(')');
                    }
                    else {
                        cuitP = cuitProv;
                    }
                    var cuitCorr = $("#buscadorCorredor").val().split('(');
                    if (cuitCorr[1] != null) {
                        var cuitC = cuitCorr[1].split(')');
                    }
                    else {
                        cuitC = cuitCorr;
                    }
                    Id = Id != "" ? Id : 0;
                    return { cuitProveedor: cuitP[0], cuitCorredor: cuitC[0], materialId: $('#material').data("kendoDropDownList").value(), filtro: $('#contratoId').val(), fijacionId: Id };
                }
            }
        }
    });
    $('#contratoId').click(function (e) {
        $('#contratoId').val("");
        $("#contratoId").data("kendoAutoComplete").search("");
        $(".datoscontrato").hide();
        $("#datosContrato").hide();
    });
    $("#contratoId").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if (event.which < 48 || event.which > 57) {
            event.preventDefault();
        }
    });

    windowsResize();
    $(window).resize(windowsResize);

    $("#comercialId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId"
    });

    $("#comercialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#comercialFijacionId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId"
    });

    $("#comercialFijacionId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialFijacionId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#comercialModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataTextField: "Comercial",
        dataValueField: "ComercialId"
    });

    $("#comercialModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $(".fechasAFijar").hide();
    $(".contratoAFijar").hide();
    $(".contratoAPrecio").hide();

    $("#NivelTarifaId").kendoDropDownList({
        optionLabel: "SELECCIONE UN NIVEL DE TARIFA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#NivelTarifaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#NivelTarifaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#tipoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId",
        change: function () {
            if (this.value() != 1) {
                var tipoId = $("#tipoId").data("kendoDropDownList").value();
                error = false;
                obj = ObtenerDatos(error);
                if (!error) {
                    BlockUi('Cargando...');
                    RedireccionarNegocio($("#crearContrato").val(), tipoId, obj);
                } else {
                    $.unblockUI();
                }
            }
            $(".fechasAFijar").hide();
            $(".contratoAFijar").hide();
            $(".contratoAPrecio").hide();
            $("#pagosDiv").hide();
            //$("#fechaCiertaDiv").hide();
            $(".contratoMadreDiv").hide();
            $("#contMadreId").val("");
            $("#madreId").prop("checked", false);
            $("#hijoId").prop("checked", false);
            $("#proveedorLabel").html("Proveedor");
            $(".fasonero").addClass("col-sm-1");
            $(".fasonero").removeClass("col-sm-2");
            $(".fason").hide();
            $(".agente").hide();
            $(".acuerdo").hide();
            $(".proveedores").show();
            $("#LabelPrecio").show();
            $("#DivPrecioMoneda").show();
            $(".espacioPrecioMoneda").hide();
            $("#pizarraDiv").hide();
            $("#aperturaPrecioDiv").hide();
            //$("#ocultarAperturaBtn").hide();
            $("#pagoDiferidoFijacionDiv").removeClass("inline-fijacion");
            $("#diasDiferidoFijacionDiv").removeClass("inline-fijacion");
            $("#pagoDiferidoFijacionDiv").addClass("hide-fijacion");
            $("#diasDiferidoFijacionDiv").addClass("hide-fijacion");
            $("#diasDiferidoId").prop("checked", false);
            $("#ocultarAperturaMoneda").removeClass("w70");
            $("#ocultarAperturaMoneda").addClass("w100");
            $("#corredorDiv").show();
            $("#cargarCantidadPendienteFijar").hide();
            //$("#chequeElectronicoDiv").hide();
            //$("#chequeElectronicoId").hide();
            //$("#pagoCbuId").hide();
            //$("#pagoCbuDiv").hide();
            //$("#dolarizadoExpressDiv").hide();
            $("#baseDiv").hide();
            $("#pizarraDiv").hide();
            $("#fechaFijacionDiv").hide();
            ImporteSobrePrecio = 0;
            MonedaSobrePrecio = "";
            PorcentajeSobrePrecio = 0;

            if (this.value() == 4) {
                $(".noFason").hide();
                $(".fason").show();
                $("#proveedorLabel").html("Fasonero");
                $(".fasonero").removeClass("col-sm-1");
                $(".fasonero").addClass("col-sm-2");
                $("#campanaDiv").show;
                $("#boton-ampliar").hide();
                RemoverFondosGrises();
                $("#guardarBtn").empty();
                $("#guardarBtn").append("Guardar Fasón");
                if ($("#material").val() === "2") {
                    $("#fasonEspecial").show();
                }
                //if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("USDM ");
                $("#pizarraDiv").prop("checked", false);
            } else if (this.value() == 5) {
                $(".noAgente").hide();
                $("#boton-ampliar").hide();
                $("#campanaDiv").show();
                $(".agente").show();
                RemoverFondosGrises();
                $("#guardarBtn").empty();
                $("#guardarBtn").append("Guardar Agente");
                //if ($("#precioMonedaId").data("kendoDropDownList"))
                //$("#precioMonedaId").data("kendoDropDownList").value("USDM ");
                $("#pizarraDiv").prop("checked", false);
                $("#fechaOperacionDiv").show();

                if ($("#material").val() === "1" || $("#material").val() === "3") { // SOJA o MAIZ
                    $("#dolarExportadorDiv").show();
                } else {
                    $("#dolarExportadorDiv").hide();
                    $("#dolarExportadorId").prop("checked", false);
                }
            } else {
                $("#fechasDiv").show();
                $("#fechaDesdeDiv").show();
                $("#fechaHastaDiv").show();
                $("#fechaHastaContratoDiv").show();
                $("#campanaDiv").show();
                $("#procedenciaDiv").show();
                $("#procedenciaDiv").show();
                if ($("#provinciaId").val() == "") {
                    obtenerLocalidadProvincia();
                }
                $("#clasificacionDiv").show();
                $("#destinoDiv").show();
                $("#CantidadCamionesDiv").show();
                $("#planCanjeConsignatarioIdDiv").show();
                $("#DatosBoleto").show();
                $("#DatosPago").show();
                $("#DatosEstablecimiento").show();
                $("#DatosCalidades").show();
                $("#DatosDescuentos").show();
                $("#baseDiv").show();
                $("#DatosAdicionales").show();
                $("#ContratoDiv").hide();
                $(".datoscontrato").hide();
                $("#ComercialDiv").hide();
                $("#guardarBtn").empty();
                $("#DatosBoleto").show();
                $("#guardarBtn").append("Guardar Negocio");
                $("#boton-ampliar").show();
                $("#mercsDepositoDiv").show();
                RemoverFondosGrises();
                $("#boton-ampliar").trigger("click");
                $("#boton-ampliar").trigger("click");

                //A FIJAR
                if (this.value() == 1) {
                    document.title = "A Fijar";
                    //if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
                    if ($("#precioMonedaAFijarId").data("kendoDropDownList")) $("#precioMonedaAFijarId").data("kendoDropDownList").value("USDM ");
                    $(".fechasAFijar").show();
                    $(".contratoAPrecio").hide();
                    $(".contratoAFijar").show();

                    $("#CDId").prop("checked", false);
                    $("#WarrantId").prop("checked", false);
                    $(".ocultar").hide();
                    $("#LabelPrecio").hide();
                    $("#DivPrecioMoneda").hide();
                    $(".espacioPrecioMoneda").show();
                    InicializarFondosGrises();
                    $("#pizarraDiv").prop("checked", false);
                }
            }
            ClickEnPizarra();
        }
    });
    $("#Id").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId",
    });
    $("#madreId").click(function () {
        $("#fasonIdCheck").prop("checked", false);
        $("#fasonIdCheck").attr("disabled", true);
        if ($(this).is(':checked')) {
            $("#pagosDiv").show();
        }
        else {
            $("#pagosDiv").hide();
            $("#CDId").prop("checked", false);
            $("#WarrantId").prop("checked", false);
            $("#fasonIdCheck").attr("disabled", false);
        }
    });
    $("#hijoId").click(function () {
        if ($(this).is(':checked')) {
            $(".contratoMadreDiv").show();
            //$("#fechaCiertaDiv").hide();
            //$("#fechaCiertaId").data("kendoDatePicker").value("");
            $("#buscadorCorredor").prop('disabled', true);
            $("#buscadorProveedor").prop('disabled', true);
            $("#material").data("kendoDropDownList").enable(false);
        }
        else {
            $(".contratoMadreDiv").hide();
            $("#contMadreId").val("");
            //$("#fechaCiertaDiv").show();
            $("#buscadorCorredor").prop('disabled', false);
            $("#buscadorProveedor").prop('disabled', false);
            $("#material").data("kendoDropDownList").enable(true);
        }
    });
    $("#fasonIdCheck").click(function () {
        if ($(this).is(':checked')) {
            $("#madreId").prop("checked", false);
            $("#madreId").attr("disabled", true);
        } else {
            $("#madreId").attr("disabled", false);
        }
    });
    $("#CheckFijacion").click(function () {
        if ($(this).is(':checked')) {
            $("#cantidadId").data("kendoNumericTextBox").value($("#kgspendientescontrato").text());
        }
    });

    $("#material").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId",
        change: function () {
            obtenerLocalidadProvincia();
            //CargarCalidadPorMaterial($('#material').data("kendoDropDownList").value());
            EsconderCalidadSiHaySojaYCalidadEspecial();
            if ($("#material").val() !== "") {
                CargarCampaniaPorMaterial($("#material").val());
                CargarCalidadPorMaterial($("#material").val());
                var iteraciones = viewModel.Calidades.length;
                for (var i = 0; i < iteraciones; i++) {
                    viewModel.Calidades.pop();
                }
                $("#material").val() === "1" || $("#material").val() === "2" ? $("#condicionFijacionId").data("kendoDropDownList").value("5") : $("#condicionFijacionId").data("kendoDropDownList").value("7");
            }
            if ($("#material").val() === "3" && ($("#tipoId").val() === "1" || $("#tipoId").val() === "2")) {
                $(".sojaSustentable").show();
                $(".sojaEpa").show();
            } else {
                $(".sojaSustentable").hide();
                $(".sojaEpa").hide();
                $(".sustentableDiv").hide();
                $(".sustenTipoDB").hide();
                $("#sustentablePrecioId").data('kendoNumericTextBox').value("");
                $("#sustentableId").prop('checked', false);
                $("#epaId").prop('checked', false);
                $("#divSustentableSinTarifa").hide();
            }
            if ($("#material").val() === "2" && $("#tipoId").val() === "4") {
                $("#fasonEspecial").show();
            } else {
                $("#fasonEspecial").hide();
            }
            $("#contratoId").val("");
            $(".datoscontrato").hide();
            $("#datosContrato").hide();
            if (($("#material").val() === "4" || $("#material").val() === "5")) {
                $("#PorcentajeDescuentoAFijarId").prop('disabled', true);
                $("#PorcentajeDescuentoAFijarId").css("background-color", "lightgray");
            } else {
                $("#PorcentajeDescuentoAFijarId").prop('disabled', false);
                $("#PorcentajeDescuentoAFijarId").css("background-color", "white");
            }

            var cuitAux = $("#buscadorProveedor").val().split('(');
            if (cuitAux[0] != "") {
                var cuit = cuitAux[1].split(')');
                var proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0], corredor: false });
                var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: proveedorId });
                CargarAutomaticamenteLaComision(compraNet);
            }
            CompletarCantidadDisponibleDeposito();
            MostrarServiciosYCalidades();
        }
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#motivoAnterior").kendoDropDownList({
        optionLabel: "SELECCIONE UN MOTIVO...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            if ($('#motivoAnterior').data("kendoDropDownList").text() == "Otro") {
                $("#motivoTexto").show();
                if (!$("#AnulaYReemplazaContratoId").val() > 0) {
                    $("#motivoOperacionAnteriorId").val("");
                    $("#descripcionMotivoAnterior").val("");
                } else {
                    $("#descripcionMotivoAnterior").val("Anula y reemplaza" + $("#AnulaYReemplazaContratoId").val());
                }
            } else {
                $("#motivoTexto").hide();
            }
        },
        select: function () {

        }
    });
    $("#precioMonedaId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#precioMonedaAFijarId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId",
        change: function () {
            if ($("#precioMonedaAFijarId").val()) {
                $(".aperturaprecioMoneda").text($("#precioMonedaAFijarId").data("kendoDropDownList").text());
            } else {
                $("#precioMonedaAFijarId").data("kendoDropDownList").value("ARP  ");
                $(".aperturaprecioMoneda").text($("#precioMonedaAFijarId").data("kendoDropDownList").text());
            }
        }
    });

    $("#monedaAjusteComisionId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    //$("#precioMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#precioMonedaId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    $("#campanaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId",
        change: function (e) {
            validarFechaCampana();
        }
    });

    $("#campanaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    function validarFechaCampana() {
        var campana = $("#campanaId").data("kendoDropDownList").text();
        var anios = campana.split('-');
        var anioInicial = '20' + anios[0] + '0101';
        var anioFinal = '20' + anios[1] + '1231';
        var fechaDesde = $("#fechaDesdeId").val().split("-", 3);
        var fechaHasta = $("#fechaHastaId").val().split("-", 3);
        fechaDesde = fechaDesde[2] + fechaDesde[1] + fechaDesde[0];
        fechaHasta = fechaHasta[2] + fechaHasta[1] + fechaHasta[0];
        if (fechaDesde < anioInicial || fechaHasta > anioFinal) {
            $("#campanaTooltip").tooltip({ title: 'Fecha fuera del rango de Campaña' });
            $("#campanaTooltip").tooltip('show');
            $("#campanaTooltip").click(function () {
                $("#campanaTooltip").tooltip('destroy');
            });
        } else {
            $("#campanaTooltip").tooltip('destroy');
        }
    }
    $("#establecimientoDiv").hide();

    $("#sustentableMonedaId").kendoDropDownList({
        optionLabel: "Moneda...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#sustentableMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#sustentableMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#statusId").kendoDropDownList({
        optionLabel: "Estado de Contrato...",
        dataTextField: "Descripcion",
        dataValueField: "EstadosContratosId"
    });

    $("#statusId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#statusId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#materialFiltroIndex").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#materialFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#materialFiltroIndex").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#campanaFiltroIndex").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });

    $("#campanaFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaFiltroIndex").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#campanaModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPAÑA...",
        dataTextField: "Descripcion",
        dataValueField: "CampañaId"
    });

    $("#campanaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#statusFiltroIndex").kendoDropDownList({
        optionLabel: "Estado de Contrato...",
        dataTextField: "Descripcion",
        dataValueField: "EstadosContratosId"
    });

    $("#statusFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#statusFiltroIndex").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#consignatarioDiv").hide();
    $("#clasificacion").kendoDropDownList({
        optionLabel: "SELECCIONE LA CLASIFICACIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            if (this.value() == 1) {
                $("#CapacidadProductivaPendienteDiv").show();
                $("#consignatarioDiv").hide();
                $("#consignatarioId").prop("checked", false);
                $("#planCanjeDiv").hide();
                $("#planCanjeId").prop("checked", false);
                if (/*$("#fechaCiertaId").val() == "" && */$("#buscadorCorredor").val() == "") {
                    //if ($("#precioMonedaId").data("kendoDropDownList").value() == "USDM " && $("#tipoId").val() != "6") {
                    //    $("#dolarizadoExpressDiv").show();
                    //    $("#dolarizadoExpressId").attr("disabled", false);
                    //    //$("#pagoDolarizadoDiv").show();
                    //    $("#dolarizadoDiv").show();

                    //}
                    //if ($("#tipoId").val() == "1" && !$("#canjeId").is(":checked")) {
                    //    $("#dolarizadoExpressDiv").show();
                    //    $("#dolarizadoExpressId").attr("disabled", false);
                    //}
                } else {
                    //$("#dolarizadoExpressDiv").hide();
                    //$("#dolarizadoExpressId").prop("checked", false);
                    //$("#dolarizadoFechaId").val("");

                }
            } else {
                $("#CapacidadProductivaPendienteDiv").hide();
                //if (!$("#pesificadoId").is(":checked")) {
                //    $("#fechaCiertaDiv").show();
                //}
                //$("#pagoDolarizadoDiv").hide();
                //$("#dolarizadoId").prop("checked", false);
                $("#consignatarioDiv").show();
                //if ($("#dolarizadoId").is(":checked")) {
                //    $("#dolarizadoDiv").show();
                //} else {
                //    $("#dolarizadoDiv").hide();
                //    $("#dolarizadoFechaId").val("");
                //}
                //$("#dolarizadoDiv").hide();

                $("#planCanjeDiv").show();
                //$("#dolarizadoExpressDiv").hide();
                //$("#dolarizadoExpressId").prop("checked", false);
            }
            ValidarAlta();
            ValidarProveedorSisa();
        }
    });

    $("#clasificacion").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacion").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#planCanjeId").click(function () {
        if (this.checked) {
            $("#consignatarioId").prop("checked", false);
        }
        ValidarAlta();
        ValidarProveedorSisa();
    });
    $("#consignatarioId").click(function () {
        if (this.checked) {
            $("#planCanjeId").prop("checked", false);
        }
        ValidarAlta();
        ValidarProveedorSisa();
    });

    $("#destinoId").kendoDropDownList({
        optionLabel: "SELECCIONE EL DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            EsconderCalidadSiHaySojaYCalidadEspecial();
            if ($("#tipoId").val() == 1 && $("#destinoId").val() != 1) {
                //LimpiarDescuentos();
                //$("#ImporteDescuentoId").data("kendoNumericTextBox").value("");
                //$("#PorcentajeDescuentoId").val("");
            }
            SeleccionAutomaticaBolsa();

            if (ValidarComisionEnCentro()) {
                if ($("#proveedorId").val() > 0) {
                    var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: $("#proveedorId").val() });
                    CargarAutomaticamenteLaComision(compraNet);
                }
                if ($("#buscadorCorredor").val() != "") {
                    $("#porcentajeComision").data("kendoNumericTextBox").value(1);
                }
            } else {
                BorrarComisionSiEsAcopio();
            }

            if ($("#destinoId").val() == 4) {
                $(".row-carta-oferta").hide();
            }
            else {
                $(".row-carta-oferta").show()
            }
            ValidarSinBoleto();
            CompletarCantidadDisponibleDeposito();
            MostrarServiciosYCalidades();
        }
    });

    $("#destinoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });


    $("#plantaDestinoId").kendoDropDownList({
        optionLabel: "SELECCIONE PLANTA DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {

        }
    });

    $("#plantaDestinoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#plantaDestinoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#tipoFasonId").kendoDropDownList({
        optionLabel: "SELECCIONE TIPO FASON...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#tipoFasonId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoFasonId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#tipoAgenteCompraId").kendoDropDownList({
        optionLabel: "SELECCIONE TIPO AGENTE...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#AgenteCompraId").kendoDropDownList({
        optionLabel: "SELECCIONE AGENTE...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            $("#porcentajeDePagoId").data("kendoNumericTextBox").value(100);
            if (this.value() == "") {
                $("#porcentajeDePagoId").data("kendoNumericTextBox").value(97.5);
                if ($("#tipoId").val() == "6") {
                    //$("#chequeElectronicoId").show();
                    $("#pagoCbuId").show();
                }
                $("#buscadorProveedor").val("");
                $("#buscadorCorredor").val("");
            }
            ActivarBoletoXAgentedeCompras(this.value());
            DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
            OcultarCamposAgente();
        }
    });

    $("#tipoAgenteCompraId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoFasonId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#operadorId").kendoDropDownList({
        optionLabel: "SELECCIONE OPERADOR...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#operadorId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#operadorId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#destinoModalPendienteId").kendoDropDownList({
        optionLabel: "DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#destinoModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinoModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#bolsaConfirmaId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#bolsaConfirmaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaConfirmaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#bolsaFisicoId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#bolsaFisicoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaFisicoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#bolsaCartaId").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#bolsaCartaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaCartaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#condicionFijacionId").kendoDropDownList({
        optionLabel: "SELECCIONE CONDICIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#condicionFijacionId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#condicionFijacionId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#condicionFijacionIdModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE CONDICIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#condicionFijacionIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#condicionFijacionIdModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#standardCalidadId").kendoDropDownList({
        optionLabel: "SELECCIONE STANDARD DE CALIDAD...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#standardCalidadId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#standardCalidadId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#calidadesEspecialesId").kendoDropDownList({
        optionLabel: "CALIDAD",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: CambioCalidades
    });

    $("#calidadesEspecialesId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#calidadesEspecialesId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#zonasGirasolAltoId").kendoDropDownList({
        optionLabel: "Zona",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#zonasGirasolAltoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#zonasGirasolAltoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#cantidadId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        change: function () {
            $("#CheckFijacion").prop("checked", false);
            if ($("#cargarCantidadCamiones").is(':checked')) {
                $("#cantidadCamionesId").data("kendoNumericTextBox").value(Math.ceil(this.value() / 30000));
            }
            if ($("#cantidadId").val() <= 30) {
                $("#cantidadTooltip").tooltip({ title: 'Cantidad inferior a 30kg' });
                $("#cantidadTooltip").tooltip('show');
                $("#cantidadTooltip").click(function () {
                    $("#cantidadTooltip").tooltip('destroy');
                });
            } else {
                $("#cantidadTooltip").tooltip('destroy');
            }
            CalcularMaximo();
            ValidarCantidad();
        }
    });

    $("#minimoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: 30000,
        spinners: false,
        change: function () {
        }
    });

    $("#maximaId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        change: function () {

        }
    });
    $("#minimoId").data("kendoNumericTextBox").enable(false);
    $("#maximaId").data("kendoNumericTextBox").enable(false);

    $("#cargarCantidadCamiones").change(function () {
        if ($("#cargarCantidadCamiones").is(':checked')) {
            if ($("#cantidadId").data("kendoNumericTextBox").value() > 0) {
                $("#cantidadId").data("kendoNumericTextBox").trigger("change");
            } else {
                $("#cantidadCamionesId").data("kendoNumericTextBox").trigger("change");
            }
        }
        else {
            $("#cantidadCamionesId").data("kendoNumericTextBox").value('');
        }
    });

    $("#cantidadCamionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        change: function () {
            if ($("#cargarCantidadCamiones").is(':checked')) {
                $("#cantidadId").data("kendoNumericTextBox").value(Math.ceil(this.value() * 30000));
                CalcularMaximo();
            }
            if ($("#cantidadId").val() <= 30) {
                $("#cantidadTooltip").tooltip({ title: 'Cantidad inferior a 30kg' });
                $("#cantidadTooltip").tooltip('show');
                $("#cantidadTooltip").click(function () {
                    $("#cantidadTooltip").tooltip('destroy');
                });
            } else {
                $("#cantidadTooltip").tooltip('destroy');
            }
        }
    });

    $("#montoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0,
        change: function () {

        }
    });

    $("#montoMonedaId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#precioAjusteComisionId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $(".number-input").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });

    $("#sustentablePrecioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#diasDiferidoFijacionId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#contMadreId").change(function () {
        var sap = $("#contMadreId").val();
        if (sap !== "") {
            var datos = { sap: sap };
            contratoEdit = MSExecuteOnServer('/CompraNet/TraerContratoMadre', datos);
            if (ExistsErrorMessages(contratoEdit.Errores)) {
                ShowErrorMessages(contratoEdit.Errores);
            } else {
                contratoEdit.Contrato.Estado = 1;
                CargarDatosEditar(contratoEdit.Contrato, true);
            }
        }
    });
    $("#contMadreId").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if (event.which < 48 || event.which > 57) {
            event.preventDefault();
        }
    });

    //$("#pesificadoDiasId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    spinners: false,
    //    min: 0
    //});

    $("#porcentajeComision").kendoNumericTextBox({
        change: function () {
            if (this.value() == 0 && $("#buscadorCorredor").val() != "") {
                MensAlerta("El porcentaje de comisión se completó con valor en 0");
            }
        },
        culture: "es-AR",
        format: "n2",
        spinners: false,
        decimals: 2,
        value: 1,
        min: 0,
        max: 100
    });

    $("#valorEspecialesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#porcentajeDesdeId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#porcentajeHastaId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#porcentajeDePagoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0,
        value: 97.5
    });
    $("#porcentajeDePagoId").data("kendoNumericTextBox").value(97.5);
    var date = ObtenerFechaDesde();
    var datehasta = ObtenerFechaHasta();
    $("#fechaOperacionId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        max: new Date(),
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            var hoy = new Date();
            var anio = hoy.getFullYear();
            var mes = hoy.getMonth();
            var dia = hoy.getDate();
            hoy = new Date(anio, mes, dia);
            const diffTime = Math.abs(hoy - this.value());
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (this.value() < hoy) {
                $(".fechaOperacionMotivoDiv").show();
                if (diffDays > 1) {
                    //$("#noInformaSioId").prop("checked", true);
                    //$("#noInformaSioId").attr("disabled", true);
                } else {
                    //$("#noInformaSioId").prop("checked", false);
                    //$("#noInformaSioId").attr("disabled", false);
                }

            } else {
                $(".fechaOperacionMotivoDiv").hide();
                $("#motivoOperacionAnteriorId").val("");
                $("#descripcionMotivoAnterior").val("");
                //$("#noInformaSioId").attr("disabled", false);
            }
            if ($("#AnulaYReemplazaContratoId").val() > 0) {
                $("#motivoAnterior").data("kendoDropDownList").text("Otro");
                $("#motivoAnterior").data("kendoDropDownList").trigger("change");
                $("#descripcionMotivoAnterior").val("Anula y reemplaza " + $("#contratoAReemplazarId").val());
            }
        }
    });
    $("#fechaDesdeId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            var maniana = new Date();
            //maniana = new Date(maniana.setDate(maniana.getDate() + 1));
            if ($("#fechaDesdeId").data("kendoDatePicker").value() >= maniana) {
                $("#fechaHastaId").val(ObtenerFechaHasta(this.value()));
                validarFechaCampana();

                $("#mercsDepositoDiv").hide();
                $("#mercsDepositoId").prop("checked", false);
            } else {
                $("#mercsDepositoDiv").show();
            }
        }
    });
    $("#fechaHastaId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaHastaOriginalId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaHastaOriginalId").data('kendoDatePicker').enable(false);

    $("#fechaDesdeTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        disableDates: function (date) {
            var day = new Date(date).getDay();
            return day === 0 || day === 6;
        },
        change: function () { $("#fechaHastaTopeId").val(ObtenerFechaHasta(this.value())); }
    });
    $("#fechaHastaTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        disableDates: function (date) {
            var day = new Date(date).getDay();
            return day === 0 || day === 6;
        }
    });

    $("#fechaDesdeSustentableId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaHastaSustentableId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeId").val(date);
    $("#fechaOperacionId").val(date);
    $("#fechaHastaId").val(datehasta);
    //$("#fechaFijacionId").val(date);


    $(".formulario-footer-guardar-contrato").click(function () {
        BlockUi('Guardando...');
        var error = false;
        var objeto = ObtenerDatos(error);
        if (objeto.Descuentos == null || objeto.Descuentos.find(function (x) { return x.TipoPeriodoDBId == 1 && x.TipoDBId == 1; }) == null) {
            objeto.AperturaPrecio = null;
        }
        if (!error) {
            setTimeout(GrabarContrato(objeto), 250);
        } else {
            $.unblockUI();
        }
    });

    $(".formulario-footer-cancelar").click(function () {
        LiberarPantalla();
        window.location.href = window.location.origin + "/CompraNet";
    });

    $("#DatosAdicionales").click(function () {
        if (document.querySelector(".datos-adicionales").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-adicionales").style.display = "block";
        } else {
            document.querySelector(".datos-adicionales").style.display = "none";
        }
    });

    $("#DatosBoleto").click(function () {
        if (document.querySelector(".datos-boleto").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-boleto").style.display = "block";
        } else {
            document.querySelector(".datos-boleto").style.display = "none";
        }
    });

    $("#DatosDescuentos").click(function () {
        if (document.querySelector(".datos-descuentos").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-descuentos").style.display = "block";
        } else {
            document.querySelector(".datos-descuentos").style.display = "none";
        }
    });

    $("#DatosCalidades").click(function () {
        if (document.querySelector(".datos-calidades").style.display == "none") {
            CerrarDatosPendientes();
            document.querySelector(".datos-calidades").style.display = "block";
        } else {
            document.querySelector(".datos-calidades").style.display = "none";
        }
    });

    //$("#sustentableId").click(function () {
    //    if ($(this).is(':checked')) {
    //        $(".sustentableDiv").show();
    //        if ($("#mercsDepositoId").is(':checked')) {
    //            //MensInfo('Revisar la fecha desde de entrega');
    //            MostrarCcPpPendientesAplicar();
    //        }
    //    }
    //    else {
    //        //$(".sustentableDiv").hide();
    //        $("#sustentablePrecioId").data("kendoNumericTextBox").value("");
    //    }
    //});
    //$("#mercsDepositoId").click(function () {
    //    if ($(this).is(':checked') && $("#sustentableId").is(':checked')) {
    //            MostrarCcPpPendientesAplicar();
    //        //MensInfo('Revisar la fecha desde de entrega');
    //    }
    //});

    //$("#dolarizadoId").click(function () {
    //    if ($(this).is(':checked')) {
    //        $("#dolarizadoDiv").show();
    //        $("#dolarizadoExpressId").prop("checked", false);
    //        $("#pesificadoId").prop("checked", false);
    //        $("#pesificadoDiv").hide();
    //        $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
    //        //$("#fechaCiertaDiv").hide();
    //        //$("#fechaCiertaId").val("");
    //        //$("#fechaCiertaAcuerdoDiv").hide();
    //        //$("#fechaCiertaAcuerdo").val("");


    //    }
    //    else {
    //        if ($("#buscadorCorredor").val() == "") {
    //            $("#dolarizadoExpressId").prop("disabled", false);
    //        }
    //        //if (!$(this).is(':checked') && !$("#dolarizadoExpressId").is(':checked') && $("#tipoId").val() == "2") {
    //        //    $("#fechaCiertaDiv").show();
    //        //} else {
    //        //    $("#fechaCiertaAcuerdoDiv").show();

    //        //}
    //        $("#dolarizadoFechaId").val("");
    //    }
    //});

    //$("#dolarizadoExpressId").click(function () {
    //    if ($(this).is(':checked')) {
    //        $("#dolarizadoDiv").show();
    //        $("#dolarizadoId").prop("checked", false);
    //        //$("#chequeElectronicoInput").prop("checked", false);
    //        //$("#chequeElectronicoDiv").hide();
    //        //$("#pagoCbu").prop("checked", false);
    //        //$("#pagoCbuDiv").hide();
    //        //$("#fechaCiertaDiv").hide();
    //        //$("#fechaCiertaId").val("");
    //    }
    //    else {
    //        //if (!$(this).is(':checked') && !$("#dolarizadoId").is(':checked')) {
    //        //    //$("#fechaCiertaDiv").show();
    //        //}
    //        $("#dolarizadoFechaId").val("");
    //        //if (!$("#compensacionId").is(":checked")) {
    //        //    $("#pagoCbuDiv").show();
    //        //    if (!$("#pagoDirectoId").is(":checked")) {
    //        //        $("#chequeElectronicoDiv").show();
    //        //    }
    //        //}
    //    }
    //});

    //$("#pesificadoId").click(function () {

    //    if ($(this).is(':checked')) {
    //        $("#pesificadoDiv").show();

    //        $("#dolarizadoExpressId").prop("checked", false);
    //        $("#dolarizadoDiv").hide();
    //        $("#dolarizadoExpressDiv").hide();
    //        $("#dolarizadoFechaId").val("");
    //        //$("#fechaCiertaAcuerdoDiv").hide();
    //        //$("#fechaCiertaAcuerdo").val("");
    //        //$("#fechaCiertaDiv").hide();
    //        //$("#fechaCiertaId").val("");
    //    }
    //    else {
    //        $("#pesificadoDiv").hide();
    //        $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
    //        if (!$("#canjeId").is(":checked") && $("#clasificacion").val() == 1) {
    //            $("#dolarizadoExpressDiv").show();
    //        }
    //        //$("#fechaCiertaAcuerdoDiv").show();

    //        //if ($("#tipoId").val() == "2") {
    //        //    //$("#fechaCiertaDiv").show();

    //        //} else {
    //        //    $("#fechaCiertaDiv").hide();
    //        //}
    //    }
    //});

    //$("#diasDiferidoId").click(function () {
    //    if ($(this).is(':checked')) {
    //        $("#diasDiferidoFijacionDiv").removeClass("hide-fijacion");
    //        $("#diasDiferidoFijacionDiv").addClass("inline-fijacion");

    //        //$("#fechaCiertaAcuerdoDiv").hide();
    //        //$("#fechaCiertaAcuerdo").val("");
    //        //$("#fechaCiertaDiv").hide();
    //        //$("#fechaCiertaId").val("");
    //    }
    //    else {
    //        $("#diasDiferidoFijacionDiv").removeClass("inline-fijacion");
    //        $("#diasDiferidoFijacionDiv").addClass("hide-fijacion");
    //        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");

    //        //if ($("#tipoId").val() == "2") {
    //        //    //$("#fechaCiertaDiv").show();

    //        //}

    //        //if ($("#tipoId").val() == "6") {

    //        //    $("#fechaCiertaAcuerdoDiv").show();
    //        //}
    //    }
    //});

    $("#boletoConfirmaId").click(function () {
        if ($(this).is(':checked')) {
            LimpiarBoleto();
            $("#boletoConfirmaId").prop("checked", true);
            $("#BolsaConfirmaDiv").show();
            SeleccionAutomaticaBolsa();
        }
        else {
            $("#BolsaConfirmaDiv").hide();
            $("#bolsaConfirmaId").data("kendoDropDownList").value("");
        }
    });

    $("#boletoFisicoId").click(function () {
        if ($(this).is(':checked')) {
            LimpiarBoleto();
            $("#boletoFisicoId").prop("checked", true);
            $("#BolsaFisicoDiv").show();
            SeleccionAutomaticaBolsa();
        }
        else {
            $("#BolsaFisicoDiv").hide();
            $("#bolsaFisicoId").data("kendoDropDownList").value("");
        }
    });
    $("#boletoCartaId").click(function () {
        if ($(this).is(':checked')) {
            LimpiarBoleto();
            $("#boletoCartaId").prop("checked", true);
            $("#BolsaCartaDiv").show();
            SeleccionAutomaticaBolsa();
        }
        else {
            $("#BolsaCartaDiv").hide();
            $("#bolsaCartaId").data("kendoDropDownList").value("");
        }
    });

    $("#boletoNingunoId").click(function () {
        if ($(this).is(':checked')) {
            LimpiarBoleto();
            $("#boletoNingunoId").prop("checked", true);
            SeleccionAutomaticaBolsa();
        }
    });
    $("#sinBoletoId").click(function () {
        if ($(this).is(':checked')) {
            LimpiarBoleto();

            $("#prestamoDevolucionId").prop("checked", false);
            $("#prestamoDevolucionId").prop("disabled", true);
            HayPrestamo();
            $("#canjeId").prop("checked", false);
            $("#canjeId").prop("disabled", true);
            HayCanje();

            //$("#posicionCBOTId").val("");
            //$("#posicionCBOTId").prop("disabled", true);
            //$("#tipoPosicionCBOTId").data("kendoDropDownList").value("");
            //var tipoPosicionCBOT = $("#tipoPosicionCBOTId").data("kendoDropDownList");
            //tipoPosicionCBOT.enable(false);
            //tipoPosicionCBOT.value("");

            $("#sinBoletoId").prop("checked", true);
            if ($("#sinBoletoId").is(':checked')) {
                MensAlerta("Se completó la tilde de mercadería en depósito automáticamente");
            }

        }
        else {

            $("#prestamoDevolucionId").prop("disabled", false);
            $("#canjeId").prop("disabled", false);
            $("#posicionCBOTId").prop("disabled", false);
            $("#tipoPosicionCBOTId").prop("disabled", false);
        }
        ValidarSinBoleto();
    });
    $("#CDId").click(function () {
        //if ($(this).is(':checked') && viewModel.AperturaPrecio.some(importeNoVacio) && $("#tipoId").val() != "6") {
        //    MensInfo("Debe borrar datos de apertura de precio para completar datos de flete procedencia");
        //}
        $("#WarrantId").prop("checked", false);
        //$("#pagoDirectoId").prop("checked", false);

        //if ($(this).is(':checked') && $("#tipoId").val() == "2") {
        //    $(".ocultar").show();
        //} else $(".ocultar").hide();
    });

    $("#WarrantId").click(function () {
        if ($(this).is(':checked') && viewModel.AperturaPrecio.some(importeNoVacio) && $("#tipoId").val() != "6") {
            MensInfo("Debe borrar datos de apertura de precio para completar datos de flete procedencia");
        }
        $("#CDId").prop("checked", false);
        $("#pagoDirectoId").prop("checked", false);
        if ($(this).is(':checked') && $("#tipoId").val() == "2") {
            $(".ocultar").show();
        } else $(".ocultar").hide();
    });

    var importeNoVacio = function (element) {
        // checks whether an element is even
        return !((element.Importe == 0 || element.Importe == undefined) && (element.Porcentaje == 0 || element.Porcentaje === undefined));
    };
    $("#pagoDirectoId").click(function () {
        if ($(this).is(':checked')) {
            //if ($("#tipoId").val() == '6' || $("#tipoId").val() == '3') {
            //    $("#chequeElectronicoId").hide();
            //    $("#chequeElectronico").prop("checked", false);
            //}
            //$("#chequeElectronicoDiv").hide();
            //$("#chequeElectronicoInput").prop("checked", false);
        }
        //else {
        //    if ($("#tipoId").val() == '6' || $("#tipoId").val() == '3') {
        //        $("#chequeElectronicoId").show();
        //    } if (!$("#compensacionId").is(":checked")) {
        //        $("#chequeElectronicoDiv").show();
        //    }

        //}
        //$("#CDId").prop("checked", false);
        $("#WarrantId").prop("checked", false);


    });

    $("#establecimientoPropioId").click(function () {
        $("#establecimientoArrendadoId").prop("checked", false);
    });
    $("#establecimientoArrendadoId").click(function () {
        $("#establecimientoPropioId").prop("checked", false);
    });

    $('#campanaId').change(function () {
        obtenerLocalidadProvincia();
    });

    $("#boton-ampliar").click(function () {
        $(".tooltip").tooltip('toggle');
        if ($("#boton-ampliar").text() == "+ AMPLIAR") {
            $("#boton-ampliar").text("- OCULTAR");
            $(".ampliar").show();
            if ($('#tipoId').val() == 1) {
                $(".contratoAPrecio").hide();
                if (!$("#prestamoDevolucionId").is(":checked")) {
                    $(".contratoAFijar").show();
                }

                $("#pagosDiv").hide();

                if ($("#madreId").is(':checked')) {
                    $("#pagosDiv").show();
                }
            } else if ($('#tipoId').val() == 2) {
                $(".contratoAFijar").hide();
                $(".contratoAPrecio").show();
                $("#pagosDiv").show();
            }
            if ($("#buscadorCorredor").val() != "") {
                $('#pagoDirectoDiv').show();
            }
            //else if ($('#tipoId').val() == 6) {
            //    $(".contratoAFijar").hide();
            //    $(".contratoAPrecio").hide();
            //    $("#pagosDiv").hide();
            //    $(".noAcuerdo").hide();
            //}
            if ($("#mercsDepositoId").is(":checked")) {
                $(".depositoDiv").show();
            } else {
                $(".depositoDiv").hide();
            }
        }
        else if ($("#boton-ampliar").text() == "- OCULTAR") {
            $("#boton-ampliar").text("+ AMPLIAR");
            $(".ampliar").hide();
            $(".ampliar-adicionales").hide();
            if ($('#tipoId').val() === 1) {
                $(".contratoAFijar").hide();
                $(".contratoAPrecio").hide();
            }
        }
    });

    $('#tipoId').change(function () {
        if ($('#tipoId').val() == 1) {
            $("#material").val() === "1" || $("#material").val() === "2" ? $("#condicionFijacionId").data("kendoDropDownList").value("5") : $("#condicionFijacionId").data("kendoDropDownList").value("7");
            $("#fechaDesdeTopeId").val(date);
            $("#fechaHastaTopeId").val(datehasta);
            //LimpiarDescuentos();
            //LimpiarApertura();
            $("#ImporteDescuentoId").data("kendoNumericTextBox").value("");
            $("#PorcentajeDescuentoId").val("");
        } else {
            $("#condicionFijacionId").data("kendoDropDownList").value("");
            $("#fechaDesdeTopeId").val("");
            $("#fechaHastaTopeId").val("");
        }
        if ($('#tipoId').val() == 2 || $('#tipoId').val() == 6) {
            $("#preciosPactadosBoton").show();
        } else {
            $("#preciosPactadosBoton").hide();
        }
    });

    $("#tipoPeriodoDBId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        dataBound: function () {
            this.select(0);
        }
    });

    $("#tipoPeriodoDBId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoPeriodoDBId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#TipoDBId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        dataBound: function () {
            this.select(0);
        },
        change: function () {
            DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
        }
    });

    $("#TipoDBId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#TipoDBId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#fechaDesdeDescuentoId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () { $("#fechaHastaDescuentoId").val(ObtenerFechaHasta(this.value())); }
    });
    $("#fechaHastaDescuentoId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#descuentoMonedaId").kendoDropDownList({
        optionLabel: "Moneda",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#descuentoMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#descuentoMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#PorcentajeDescuentoId").val("");

    $('select[id="tipoPeriodoDBId"]').change(function () {
        //$("#ocultarAperturaBtn").hide();
        $("#ImporteDescuentoId").data("kendoNumericTextBox").value("");
        if ($(this).val() == 1) {
            $(".fecha-descuento").hide();
            $(".fecha-descuento-pendiente").hide();
            $("#fechaHastaDescuentoId").val("");
            $("#fechaDesdeDescuentoId").val("");
            //if ($("#TipoDBId").val() == 1) {
            //    $("#ocultarAperturaBtn").show();
            //}
        }
        else {
            $(".fecha-descuento").show();
            $(".fecha-descuento-pendiente").show();
            $("#fechaDesdeDescuentoId").val(date);
            $("#fechaHastaDescuentoId").val(datehasta);
        }
    });
    $('select[id="TipoDBId"]').change(function () {
        //$("#ocultarAperturaBtn").hide();
        $("#ImporteDescuentoId").data("kendoNumericTextBox").value('');
        //if ($(this).val() == 1 && $("#tipoPeriodoDBId").val() == 1) {
        //    $("#ocultarAperturaBtn").show();
        //}
    });

    $("#IngresarDescuento").click(AgregarDescuentos);
    $("#IngresarCalidad").click(AgregarCalidades);
    $("#selCargoVendedorId").click(function () {
        if ($(this).is(':checked')) {
            $("#selCargoMOAId").prop("checked", false);
        }
    });
    $("#selCargoMOAId").click(function () {
        if ($(this).is(':checked')) {
            $("#selCargoVendedorId").prop("checked", false);
        }
    });
    $("#posicionFasonId").mask("00.0000", { placeholder: "MM.AAAA" });
    $("#posicionCBOTId").mask("00.0000", { placeholder: "MM.AAAA" });
    $("#posicionCBOTId").change(function () {
        //ActivarSinBoleto();
    });
    $("#pizarraId").click(ClickEnPizarra);

    InicializarAperturaDePrecios();

    $("#ImporteDescuentoId").kendoNumericTextBox({
        culture: "es-AR",
        spinners: false,
        //change: function () { ConvertirDescuentoANegativo(); }
    });

    $("#TarifaFleteId").change(function () {
        $("#precioTotalApertura").data("kendoNumericTextBox").value(CalcularPrecioTotalApertura());
        if ($(this).val() > 0) {
            $("#aperturaPrecioBtn").addClass("pointerEventDesabilitado");
        } else {
            $("#aperturaPrecioBtn").removeClass("pointerEventDesabilitado");
        }
    });
    $("#NivelTarifaId").change(function () {
        if ($(this).val() > 0) {
            $("#aperturaPrecioBtn").addClass("pointerEventDesabilitado");
        } else {
            $("#aperturaPrecioBtn").removeClass("pointerEventDesabilitado");
        }
    });

    $("#preciosPactadosBoton").click(function () {
        $("#apertura-precio").hide();
        $("#precio-pactado").show();
        $("#titulo-modal").html("PRECIO PACTADO");
    });
    $("#aperturaPrecioBoton").click(function () {
        $("#precio-pactado").hide();
        $("#apertura-precio").show();
        $("#titulo-modal").html("APERTURA DE PRECIO");
    });
    $("#fechaDesdePactado").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHastaPactado").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#precioPactado").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    $("#importePactado").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    $("#porcentajePactado").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    //$("#monedaPactadoId").kendoDropDownList({
    //    optionLabel: "Moneda",
    //    dataTextField: "Descripcion",
    //    dataValueField: "MonedaId"
    //});
    //$("#monedaPactadoId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#monedaPactadoId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});
    //$("#monedaPactadoId").data("kendoDropDownList").enable(false);
    //$("#monedaImportePactadoId").kendoDropDownList({
    //    optionLabel: "Moneda",
    //    dataTextField: "Descripcion",
    //    dataValueField: "MonedaId"
    //});
    //$("#monedaImportePactadoId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#monedaImportePactadoId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});
    //$("#agregarPrecioPactado").click(AgregarPrecioPactado);


    //$("#pagoCbu").kendoAutoComplete({
    //    template: '<p class="buscar-nomb">#: data.Pago#</p>',
    //    dataTextField: "Pago",
    //    dataValueField: "Pago",
    //    autoWidth: true,
    //    filter: "contains",
    //    change: function () {
    //        if ($("#pagoCbu").val() == "" && !$("#compensacionId").is(":checked") && !$("#dolarizadoExpressId").is(":checked")) {
    //            $("#chequeElectronicoDiv").show();
    //        } else {
    //            $("#chequeElectronicoDiv").hide();
    //            $("#chequeElectronicoInput").prop("checked", false);

    //        }
    //    },
    //    //select: function (e) {
    //    //    $("#pagoCbu").val(e.dataItem.Pago);
    //    //},
    //    dataSource: {
    //        severFiltering: true,
    //        serverPaging: true,
    //        transport: {
    //            read: {
    //                type: 'post',
    //                dataType: 'json',
    //                url: "/CompraNet/ObtenerListaDeCbu"
    //            },
    //            parameterMap: function (data, type) {
    //                var cuitProv = $("#buscadorProveedor").val().split('(');
    //                if (cuitProv[1] != null) {
    //                    var cuitP = cuitProv[1].split(')');
    //                }
    //                else {
    //                    cuitP = cuitProv;
    //                }
    //                return { cuitProveedor: cuitP[0], filtro: $('#pagoCbu').val() };
    //            }
    //        }

    //    }
    //});
    //$('#pagoCbu').click(function (e) {
    //    $('#pagoCbu').val("");
    //    $("#pagoCbu").data("kendoAutoComplete").search("");
    //});


    //$("#pagoCbuInput").kendoAutoComplete({
    //    template: '<p class="buscar-nomb">#: data.Pago#</p>',
    //    type: 'text',
    //    dataTextField: "Pago",
    //    dataValueField: "Pago",
    //    autoWidth: true,
    //    filter: "contains",
    //    change: function () {
    //        if ($("#pagoCbuInput").val() == "") {
    //            $("#chequeElectronicoId").show();
    //        } else {
    //            $("#chequeElectronicoId").hide();
    //            $("#chequeElectronico").prop("checked", false);
    //        }
    //    },
    //    dataSource: {
    //        severFiltering: true,
    //        serverPaging: true,
    //        transport: {
    //            read: {
    //                type: 'post',
    //                dataType: 'json',
    //                url: "/CompraNet/ObtenerListaDeCbu"
    //            },
    //            parameterMap: function (data, type) {
    //                var cuitProv = $("#buscadorProveedor").val().split('(');
    //                if (cuitProv[1] != null) {
    //                    var cuitP = cuitProv[1].split(')');
    //                }
    //                else {
    //                    cuitP = cuitProv;
    //                }
    //                return { cuitProveedor: cuitP[0], filtro: $('#pagoCbuInput').val() };
    //            }
    //        }

    //    }
    //});
    //$('#pagoCbuInput').click(function (e) {
    //    $('#pagoCbuInput').val("");
    //    $("#pagoCbuInput").data("kendoAutoComplete").search("");
    //});

    $("#tipoPosicionCBOTId").kendoDropDownList({
        optionLabel: "SELECCIONE TIPO...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            //ActivarSinBoleto()
            DeshabilitarConPase();
        }
    });

    $("#tipoPosicionCBOTId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoPosicionCBOTId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#CapacidadProductivaPendienteBtn").click(function () {
        AbrirModalCapacidadProductivaPendiente();
    });
    $("#cantidadDeposito").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        change: function () {
        }
    });
    $("#selectSustenTipoDB").kendoDropDownList({
        optionLabel: "Seleccionar...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            EPATipoDB();
        }
    });
    //FIN INICIALIZARELEMENTOS
}

function AbrirModalCapacidadProductivaPendiente() {
    if ($("#proveedorId").val() > 0) {
        var data = { proveedorId: $("#proveedorId").val() };
        var datos = MSExecuteOnServer('/CompraNet/ObtenerCapacidadProductivaPendiente', data);

        var viewmodel = {
            ObtenerCapacidadProductivaPendiente: datos
        }
        kendo.bind($("#modalCapacidadProductivaPendiente"), viewmodel);
        $("#modalCapacidadProductivaPendiente").modal("show");
    } else {
        MensErr("Seleccione el proveedor");
    }
}

function CambioCalidades(calidades) {
    if ($("#calidadesEspecialesId").data("kendoDropDownList").text() !== "Camara"
        && $("#calidadesEspecialesId").data("kendoDropDownList").text() !== "Fabrica"
        && $("#calidadesEspecialesId").data("kendoDropDownList").text() !== "Bonif. SECO de 7% a 10% Por punto"
        && $("#calidadesEspecialesId").data("kendoDropDownList").text() !== "Grado 2"
        && $("#calidadesEspecialesId").val() !== "") {
        $(".calidadesEspecialesDatos").show();
        if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado 2" ||
            $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado" ||
            $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Especial") {
            $(".calidad-no-grado").hide();
            LimpiarCalidades();
        } else {
            $(".calidad-no-grado").show();
        }
    } else {
        $(".calidadesEspecialesDatos").hide();
        LimpiarCalidades();
    }

    if (calidades !== undefined && calidades.length == 1) {
        $("#valorEspecialesId").data("kendoNumericTextBox").value(calidades[0].Valor);
    } else {
        if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado") {
            $("#valorEspecialesId").data("kendoNumericTextBox").value(2);
        } else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado 2") {
            $("#valorEspecialesId").data("kendoNumericTextBox").value(2);
            $(".calidadesEspecialesDatos").hide();
            $(".calidad-no-grado").hide();
        } else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Especial") {
            $("#valorEspecialesId").data("kendoNumericTextBox").value("");
            $(".calidad-no-grado").hide();
        }
        else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Bonif. SECO de 7% a 10% Por punto") {
            $("#valorEspecialesId").data("kendoNumericTextBox").value(1);
        }
        else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Granos verdes") {
            $("#valorEspecialesId").data("kendoNumericTextBox").value('0,20');
        }
        else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Dañados") {
            $("#valorEspecialesId").data("kendoNumericTextBox").value('0,50');
        } else {
            $("#valorEspecialesId").data("kendoNumericTextBox").value("");
        }
    } if ($("#material").val() == 5 && $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Bonif. SECO de 7% a 10% Por punto") {
        $(".no-girasol-alto").hide();
        $(".girasol-alto").hide();
        $("#valorEspecialesId").data("kendoNumericTextBox").value("");
    } else {
        $(".girasol-alto").hide();
        $("#zonasGirasolAltoId").data("kendoDropDownList").value("");
    }
    MostrarServiciosYCalidades();
}

function ClickEnPizarra() {
    var precioRojo = $("#precioId").hasClass("required-box-parent") ? $("#precioId") : $("#precioId").parent().parent();
    if ($("#pizarraId").is(':checked')) {
        LimpiarApertura();
        //$("#precioId").data("kendoNumericTextBox").enable(false);
        //$("#precioMonedaId").data("kendoDropDownList").enable(false);
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").enable(false);
        //$("#pesificadoDiasId").data("kendoNumericTextBox").enable(false);
        $("#aperturaPrecioBtn").addClass("pointerEventDesabilitado");
        $("#precioId").data("kendoNumericTextBox").value("");
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");
        //$("#pesificadoDiasId").data("kendoNumericTextBox").value("");
        $("#precioId").trigger('change');
        $("#precioTotalApertura").data("kendoNumericTextBox").value("");
        //$("#precioMonedaId").data("kendoDropDownList").value(0);

        $("#pagoDiferidoFijacionDiv").removeClass("inline-fijacion");
        $("#pagoDiferidoFijacionDiv").addClass("hide-fijacion");
        $("#pagoDiferidoDiv").hide();
        //$("#pesificadoDiv").hide();
        $("#diasDiferidoFijacionDiv").removeClass("inline-fijacion");
        $("#diasDiferidoFijacionDiv").addClass("hide-fijacion");
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");
        precioRojo.removeClass("required-border");
    }
    else {
        //$("#precioId").data("kendoNumericTextBox").enable(true);
        //$("#precioMonedaId").data("kendoDropDownList").enable(true);
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").enable(true);
        //$("#pesificadoDiasId").data("kendoNumericTextBox").enable(true);
        $("#diasDiferidoId").prop("checked", false);
        //$("#pesificadoId").prop("checked", false);
        $("#aperturaPrecioBtn").removeClass("pointerEventDesabilitado");
        //$("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        if ($("#tipoId").val() == 3) {
            $("#pagoDiferidoFijacionDiv").addClass("inline-fijacion");
            $("#pagoDiferidoFijacionDiv").removeClass("hide-fijacion");
        } else {
            if ($("#tipoId").val() != 6) {
                $("#pagoDiferidoDiv").show();
            }

        }
        //if ($("#tipoId").val() == 5) {
        //    $("#precioMonedaId").data("kendoDropDownList").value("USDM ");
        //}
        //precioRojo.addClass("required-border");
    }

}

function LimpiarBoleto() {
    $("#boletoFisicoId").prop("checked", false);
    $("#boletoConfirmaId").prop("checked", false);
    $("#boletoCartaId").prop("checked", false);
    $("#boletoNingunoId").prop("checked", false);
    $("#sinBoletoId").prop("checked", false);
    $("#bolsaFisicoId").data("kendoDropDownList").value("");
    $("#bolsaConfirmaId").data("kendoDropDownList").value("");
    $("#bolsaCartaId").data("kendoDropDownList").value("");
    $("#BolsaConfirmaDiv").hide();
    $("#BolsaFisicoDiv").hide();
    $("#BolsaCartaDiv").hide();
    ValidarSinBoleto();
}

function CargarCampaniaPorMaterial(value) {
    var resultGrano = MSExecuteOnServer('/CompraNet/TraerCampanaPorMaterial', { MaterialId: value });

    var campanaActualId = MSExecuteOnServer('/CompraNet/TraerCampanaActualMaterial', { MaterialId: value });

    viewModel.set("CampanaCombo", resultGrano);

    $("#campanaId").data("kendoDropDownList").value(campanaActualId);
}

function CerrarDatosPendientes() {
    document.querySelector(".datos-adicionales").style.display = "none";
    document.querySelector(".datos-boleto").style.display = "none";
    document.querySelector(".datos-descuentos").style.display = "none";
    document.querySelector(".datos-calidades").style.display = "none";
}
function windowsResize() {
    if ($(window).width() <= 400) {
        $("#buscadorProveedor").data("kendoAutoComplete").list.width(300);
        $("#buscadorCorredor").data("kendoAutoComplete").list.width(300);
        $("#contratoId").data("kendoAutoComplete").list.width(300);
    } else {
        $("#buscadorProveedor").data("kendoAutoComplete").list.width("auto");
        $("#buscadorCorredor").data("kendoAutoComplete").list.width("auto");
        $("#contratoId").data("kendoAutoComplete").list.width("auto");
    }

    if ($(window).width() >= 751 && $(window).width() <= 991) {
        $("#proveedorLabelId").addClass("noLeftPadding");
    } else {
        $("#proveedorLabelId").removeClass("noLeftPadding");
    }
}
function CargarCalidadPorMaterial(value) {
    var calidadGrano = MSExecuteOnServer('/CompraNet/TraerCalidadesPorMaterial', { MaterialId: value });
    if ((($("#destinoId").data("kendoDropDownList").value() == "13" || $("#destinoId").data("kendoDropDownList").value() == "6" ||
        $("#destinoId").data("kendoDropDownList").value() == "7") && $('#material').data("kendoDropDownList").value() == "3") || $("#canjeId").is(":checked")) {
        for (var i = 0; i < calidadGrano.length; i++) {
            if (calidadGrano[i].Descripcion != "Camara" && calidadGrano[i].Descripcion != "Fabrica") {
                calidadGrano.splice(i);
            }
        }
    }
    viewModel.set("EspecialesCombo", calidadGrano);

    if ($("#calidadesEspecialesId").data("kendoDropDownList") && value === "3") {
        $("#calidadesEspecialesId").data("kendoDropDownList").text("Fabrica");
    } else if ($("#calidadesEspecialesId").data("kendoDropDownList") && (value === "2" || value === "1")) {
        $("#calidadesEspecialesId").data("kendoDropDownList").text("Grado");
    } else {
        $("#calidadesEspecialesId").data("kendoDropDownList").text("Camara");
    }
    if ($("#material").val() == 2) {
        $("#calidadesEspecialesId").data("kendoDropDownList").text("Grado 2");
    }
    CambioCalidades();
}

function LimpiarCalidades() {
    var iteracionesCalidades = viewModel.Calidades.length;
    for (i = 0; i < iteracionesCalidades; i++) {
        viewModel.Calidades.pop();
    }
}
function LimpiarDescuentos() {
    var iteracionesDescuentos = viewModel.Descuentos.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.Descuentos.pop();
    }
}
function CrearViewModel() {
    var param = {
        "proveedorId": null,
        "material": null,
        "materialDesc": null,
        "cantidadId": null,
        "precioId": null,
        "campanaId": null,
        "campanaDesc": null,
        "provinciaId": null,
        "provinciaIdDesc": null,
        "LocalidadCrearContrato": null,
        "LocalidadCrearContratoDesc": null,
        "baseId": null,
        "sustentableId": null,
        "sustentablePrecioId": null,
        "dolarizadoId": null,
        "dolarizadoFechaId": null,
        "pesificadoId": null,
        "pesificadoDiasId": null,
        "noInformaSioId": null,
        "trigoEspecialId": null,
        "statusId": null,
        "observacionId": null,
        "clasificacionId": null,
        "cantidadCamionesId": null,
        "condicionFijacionId": null,
        "destinoId": null,
        "tipoFasonId": null,
        "operadorId": null,
        "tipoAgenteCompraId": null,
        "planCanjeId": null,
        "consignatarioId": null,
        "cdId": null,
        "warrantId": null,
        "pagoDirectoId": null,
        "standardCalidadId": null,
        "calidadesEspecialesId": null,
        "tipoPeriodoDBId": null,
        "mercsDepositoId": null,

        "proveedorIdModal": null,
        "comercialIdModal": null,
        "comercialDescModal": null,
        "materialModal": null,
        "materialDescModal": null,
        "cantidadIdModal": null,
        "tipoIdModal": null,
        "tipoDescModal": null,
        "precioIdModal": null,
        "precioMonedaIdModal": null,
        "precioMonedaDescModal": null,
        "campanaIdModal": null,
        "campanaDescModal": null,
        "fechaDesdeIdModal": null,
        "fechaHastaIdModal": null,
        "fechaEntregaIdModal": null,
        "provinciaModalPendienteId": null,
        "provinciaIdDescModal": null,
        "LocalidadCrearContratoModal": null,
        "LocalidadCrearContratoDescModal": null,
        "baseIdModal": null,
        "sustentableIdModal": null,
        "sustentablePrecioIdModal": null,
        "sustentableMonedaIdModal": null,
        "sustentableMonedaDescModal": null,
        "dolarizadoIdModal": null,
        "dolarizadoFechaIdModal": null,
        "pesificadoIdModal": null,
        "pesificadoDiasIdModal": null,
        "noInformaSioIdModal": null,
        "trigoEspecialIdModal": null,
        "observacionIdModal": null,
        "clasificacionIdModal": null,
        "cantidadCamionesIdModal": null,
        "condicionFijacionIdModal": null,
        "destinoIdModal": null,
        "planCanjeIdModal": null,
        "consignatarioIdModal": null,
        "cDIdModal": null,
        "warrantIdModal": null,
        "pagoDirectoIdModal": null,
        "standardCalidadIdModal": null,
        "calidadesEspecialesIdModal": null,
        "tipoPeriodoDBIdModal": null,
        "Descuentos": null,
        "ContratosPendientes": null,
        "pizarraId": null,
        "precioNetoId": null,
        "fechaCiertaId": null,
        "porcentajeDePagoId": null,
        "fechaOperacionId": null,
        "tipoPosicionCBOTId": null,
        "conDescargaId": null,
        "ConDescarga": null,
    };
    viewModel = kendo.observable({
        Parametros: param,

        ComercialCombo: [],
        MaterialCombo: [],
        TipoCombo: [],
        PrecioMonedaCombo: [],
        CampanaCombo: [],
        ProvinciaCombo: [],
        LocalidadCombo: [],
        SustentableMonedaCombo: [],
        EstadoCombo: [],
        CondicionFijacionCombo: [],
        StandardCombo: [],
        EspecialesCombo: [],
        ZonasCombo: [],
        TipoPeriodoDBCombo: [],
        TipoDBCombo: [],
        NivelTarifa: [],
        ComercialComboModalPendiente: [],
        MaterialComboModalPendiente: [],
        TipoComboModalPendiente: [],
        PrecioMonedaComboModalPendiente: [],
        CampanaComboModalPendiente: [],
        ProvinciaComboModalPendiente: [],
        LocalidadComboModalPendiente: [],
        SustentableMonedaComboModalPendiente: [],
        Clasificacion: [],
        CondicionFijacionComboModalPendiente: [],
        Destino: [],
        TipoFason: [],
        TipoAgenteCompra: [],
        Operador: [],
        StandardComboModalPendiente: [],
        EspecialesComboModalPendiente: [],
        isControlDisabled: true,
        Descuentos: [],
        Calidades: [],
        PrecioPactado: [],
        DescuentosVisualizar: [],
        CalidadesVisualizar: [],
        ContratosPendientes: [],
        AperturaPrecio: [],
        MonedaPactadoCombo: [],
        TipoPosicionCBOT: [],
        ObtenerCapacidadProductivaPendiente: [],
        Servicios: [],
        ConDescarga: [],
    });

    kendo.bind($("#CrearContrato"), viewModel);
    kendo.bind($("#CompraNet"), viewModel);
    kendo.bind($("#modalPendienteDiv"), viewModel);
    kendo.bind($("#tabla-descuentos"), viewModel);
    kendo.bind($("#tabla-descuentos-visualizar"), viewModel);
    kendo.bind($("#tabla-calidades"), viewModel);
    kendo.bind($("#tabla-calidades-visualizar"), viewModel);
    kendo.bind($("#tabla-pendientes"), viewModel);
    kendo.bind($("#modalAperturaPrecio"), viewModel);
}

//function InicializarDatos() {
//    var funcReturn = function (data) {
//        if (ExistsErrorMessages(data.Errores)) {
//            ShowErrorMessages(data.Errores);
//        }
//        else {
//            datosIniCrearContrato = data;
//            AsignarDatos();
//        }
//        if (Id != "") {
//            switch (TipoId) {
//                case "1":
//                    setTimeout(InicializarContratoEdit, 300);
//                    break;
//                case "2":
//                    setTimeout(InicializarContratoEdit, 300);
//                    break;
//                case "3":
//                    setTimeout(InicializarFijacionEdit, 300);
//                    break;
//                case "4":
//                    setTimeout(InicializarFasonEdit, 300);
//                    break;
//                case "5":
//                    setTimeout(InicializarAgenteEdit, 300);
//                    break;
//                case "6":
//                    setTimeout(InicializarAcuerdoEdit, 300);
//                    break;
//                default:
//                    $.unblockUI();
//                    InicializarBordesRojos();
//            }
//        }else {
//            $.unblockUI();
//            InicializarBordesRojos();
//        }
//    };

//    MSExecuteURLOnServerAsync('/CompraNet/InicializarContrato', funcReturn, '');
//}

function AsignarDatos() {
    viewModel.set("ComercialCombo", datosIniCrearContrato.Datos.comercial);
    viewModel.set("MaterialCombo", datosIniCrearContrato.Datos.material);
    viewModel.set("MotivoAnteriorCombo", datosIniCrearContrato.Datos.MotivoAnterior);
    viewModel.set("TipoCombo", datosIniCrearContrato.Datos.tiponegocio);
    viewModel.set("PrecioMonedaCombo", datosIniCrearContrato.Datos.moneda);
    viewModel.set("ProvinciaCombo", datosIniCrearContrato.Datos.prov);
    viewModel.set("LocalidadCombo", datosIniCrearContrato.Datos.loc);
    viewModel.set("SustentableMonedaCombo", datosIniCrearContrato.Datos.monedaSustentable);
    viewModel.set("EstadoCombo", datosIniCrearContrato.Datos.estadoContrato);
    viewModel.set("CampanaCombo", datosIniCrearContrato.Datos.campaña);
    viewModel.set("ClasificacionCombo", datosIniCrearContrato.Datos.Clasificacion);
    viewModel.set("DestinoCombo", datosIniCrearContrato.Datos.Destino);
    viewModel.set("BolsaCombo", datosIniCrearContrato.Datos.Bolsa);
    var bolsaFisico = [];
    for (i = 0; i < datosIniCrearContrato.Datos.Bolsa.length; i++) {
        if (datosIniCrearContrato.Datos.Bolsa[i].Descripcion == "Buenos Aires")
            bolsaFisico.push(datosIniCrearContrato.Datos.Bolsa[i]);
    }
    viewModel.set("BolsaFisicoCombo", bolsaFisico);
    var bolsaCarta = [];
    for (i = 0; i < datosIniCrearContrato.Datos.Bolsa.length; i++) {
        if (datosIniCrearContrato.Datos.Bolsa[i].Descripcion == "Buenos Aires")
            bolsaCarta.push(datosIniCrearContrato.Datos.Bolsa[i]);
    }
    viewModel.set("BolsaCartaCombo", bolsaCarta);
    viewModel.set("CondicionFijacionCombo", datosIniCrearContrato.Datos.Condicion);
    viewModel.set("StandardCombo", datosIniCrearContrato.Datos.Standard);
    viewModel.set("tipoFasonCombo", datosIniCrearContrato.Datos.TipoFason);
    viewModel.set("operadorCombo", datosIniCrearContrato.Datos.Operador);
    viewModel.set("tipoAgenteCompraCombo", datosIniCrearContrato.Datos.TipoAgenteCompra);
    viewModel.set("NivelTarifaCombo", datosIniCrearContrato.Datos.NivelTarifa);
    viewModel.set("ZonasCombo", datosIniCrearContrato.Datos.Zona);

    viewModel.set("TipoPeriodoDBCombo", [datosIniCrearContrato.Datos.TipoPeriodoDB[0], datosIniCrearContrato.Datos.TipoPeriodoDB[2]]);
    viewModel.set("TipoDBCombo", datosIniCrearContrato.Datos.TipoDB);
    viewModel.set("DescuentoMonedaCombo", datosIniCrearContrato.Datos.MonedaDescuento);

    viewModel.set("ComercialComboModalPendiente", datosIniCrearContrato.Datos.comercial);
    viewModel.set("MaterialComboModalPendiente", datosIniCrearContrato.Datos.material);
    viewModel.set("TipoComboModalPendiente", datosIniCrearContrato.Datos.tiponegocio);
    viewModel.set("PrecioMonedaComboModalPendiente", datosIniCrearContrato.Datos.moneda);
    viewModel.set("ProvinciaComboModalPendiente", datosIniCrearContrato.Datos.prov);
    viewModel.set("LocalidadComboModalPendiente", datosIniCrearContrato.Datos.loc);
    viewModel.set("SustentableMonedaComboModalPendiente", datosIniCrearContrato.Datos.monedaSustentable);
    viewModel.set("CampanaComboModalPendiente", datosIniCrearContrato.Datos.campaña);
    viewModel.set("ClasificacionComboModalPendiente", datosIniCrearContrato.Datos.Clasificacion);
    viewModel.set("DestinoComboModalPendiente", datosIniCrearContrato.Datos.Destino);
    viewModel.set("BolsaComboModalPendiente", datosIniCrearContrato.Datos.Bolsa);
    viewModel.set("CondicionFijacionComboModalPendiente", datosIniCrearContrato.Datos.Condicion);
    viewModel.set("StandardComboModalPendiente", datosIniCrearContrato.Datos.Standard);
    viewModel.set("MotivoComboModalPendiente", datosIniCrearContrato.Datos.MotivoAnterior);
    viewModel.set("TipoPosicionCBOT", datosIniCrearContrato.Datos.TipoPosicionCBOT);
    viewModel.set("isControlDisabled", false);

    if ($("#tipoId").data("kendoDropDownList")) $("#tipoId").data("kendoDropDownList").value("1");
    //if ($("#precioMonedaId").data("kendoDropDownList")) {
    //    $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
    //    $("#pagoDolarizadoDiv").hide();
    //}
    //if ($("#monedaPactadoId").data("kendoDropDownList")) $("#monedaPactadoId").data("kendoDropDownList").value("ARP  ");

    if ($("#comercialId").data("kendoDropDownList")) $("#comercialId").data("kendoDropDownList").value(comercialId);
    if ($("#comercialFijacionId").data("kendoDropDownList")) $("#comercialFijacionId").data("kendoDropDownList").value(comercialId);
    if ($("#material").data("kendoDropDownList")) $("#material").data("kendoDropDownList").value("3");
    if ($("#sustentableMonedaId").data("kendoDropDownList")) $("#sustentableMonedaId").data("kendoDropDownList").value("USDM ");
    if ($("#campanaId").data("kendoDropDownList")) CargarCampaniaPorMaterial("3");
    if ($("#destinoId").data("kendoDropDownList")) $("#destinoId").data("kendoDropDownList").value("1");
    if ($("#tipoAgenteCompraId").data("kendoDropDownList")) $("#tipoAgenteCompraId").data("kendoDropDownList").value("1");
    if ($("#descuentoMonedaId").data("kendoDropDownList")) $("#descuentoMonedaId").data("kendoDropDownList").value("1");
    var materialId = $('select[id="material"]').val();

    CargarCalidadPorMaterial(materialId);
    MostrarServiciosYCalidades();
}

function LimpiarValidaciones() {
    $("#errproveedorId").css("display", "none");
    $("#errcomercialId").css("display", "none");
    $("#errmaterial").css("display", "none");
    $("#errcantidadId").css("display", "none");
    $("#errtipoId").css("display", "none");
    $("#errprecioId").css("display", "none");
    $("#errprecioMonedaId").css("display", "none");
    $("#errcampanaId").css("display", "none");
    $("#errfechaDesdeId").css("display", "none");
    $("#errfechaHastaId").css("display", "none");
    $("#errfechaEntregaId").css("display", "none");
    $("#errprovinciaId").css("display", "none");
    $("#errLocalidadId").css("display", "none");
    $("#errBaseId").css("display", "none");
    $("#errsustentableId").css("display", "none");
    $("#errsustentablePrecioId").css("display", "none");
    $("#errsustentableMonedaId").css("display", "none");
    //$("#errdolarizadoId").css("display", "none");
    //$("#errdolarizadoFechaId").css("display", "none");
    //$("#errpesificadoId").css("display", "none");
    //$("#errpesificadoDiasId").css("display", "none");
    $("#errnoInformaSioId").css("display", "none");
    $("#errtrigoEspecialId").css("display", "none");
    $("#errobservacionId").css("display", "none");
    $("#errclasificacion").css("display", "none");
    $("#errdestinoId").css("display", "none");
    $("#errTipoFasonId").css("display", "none");
    $("#errBoletoConfirmaId").css("display", "none");
    $("#errBolsaConfirmaId").css("display", "none");
    $("#errBoletoFisicoId").css("display", "none");
    $("#errbolsaFisicoId").css("display", "none");
    $("#errBoletoCartaId").css("display", "none");
    $("#errbolsaCartaId").css("display", "none");
    $("#errBoletoNingunoId").css("display", "none");
    $("#errEstablecimientoPropioId").css("display", "none");
    $("#errcantidadCamionesId").css("display", "none");
    $("#errfechaDesdeTopeId").css("display", "none");
    $("#errfechaHastaTopeId").css("display", "none");
    $("#errcondicionFijacionId").css("display", "none");
    $("#errplanCanjeId").css("display", "none");
    $("#errConsignatarioId").css("display", "none");
    $("#errCDId").css("display", "none");
    $("#errWarrantId").css("display", "none");
    $("#errpagoDirectoId").css("display", "none");
    $("#errstandardCalidadId").css("display", "none");
    $("#errcalidadesEspecialesId").css("display", "none");
    $("#errvalorEspecialesId").css("display", "none");
    $("#errmercsDepositoId").css("display", "none");

    $("#errtipoPeriodoDBId").css("display", "none");
    $("#errTipoDBId").css("display", "none");
    $("#errfechaDesdeDescuentoId").css("display", "none");
    $("#errfechaHastaDescuentoId").css("display", "none");
    $("#errImporteDescuentoId").css("display", "none");
    $("#errdescuentoMonedaId").css("display", "none");
    $("#errPorcentajeDescuentoId").css("display", "none");
    $("errAgenteCompraId").css("display", "none");
}

function GrabarContrato(nuevoContrato) {
    var result;

    if (nuevoContrato.TipoNegocioId == 1 || nuevoContrato.TipoNegocioId == 2) {
        var cantidadCamiones = $("#cantidadCamionesId").data("kendoNumericTextBox").value();
        var cantidad = $("#cantidadId").data("kendoNumericTextBox").value();
        if (cantidadCamiones > 0) {
            var cantidadCamionesNecesarios = Math.ceil(cantidad / 30000);
            if (cantidadCamiones > cantidadCamionesNecesarios) {
                MensErr("La cantidad de camiones ingresados es mayor a la necesaria");
                $.unblockUI();
                return;
            }
            if (cantidadCamiones < cantidadCamionesNecesarios) {
                MensErr("La cantidad de camiones ingresados es menor a la necesaria");
                $.unblockUI();
                return;
            }
        }
        if (nuevoContrato.TipoNegocioId == 2 && $("#hijoId").is(':checked') && $("#contMadreId").val() == "") {
            MensErr("El Contrato Madre es Obligatorio al Fijar el Convenio");
            $.unblockUI();
        } else {
            if ($("#aperturaPrecioImporteBonificacionesId").val() == "0" && $("#tipoPosicionCBOTId").data("kendoDropDownList").value() == "3" && $("#estado").val() != "5" /*&& $("#esCostoFinanciero").is(':checked') != true*/) {
                $("#ModalConfirmarBonificacion").modal('show');
            } else {

                var objeto = {
                    oParam: nuevoContrato,
                    listCupoConDescargaFechas: nuevoContrato.ConDescargaDias,
                }

                result = MSExecuteOnServer('/CompraNet/GrabarContrato', objeto);
            }
        }

        detenerIntervalo();
        LiberarPantalla();
        dataTabla = [];

    } else if (nuevoContrato.TipoNegocioId == 4) {
        result = MSExecuteOnServer('/CompraNet/GrabarFason', nuevoContrato);
    } else if (nuevoContrato.TipoNegocioId == 5) {
        result = MSExecuteOnServer('/CompraNet/GrabarAgente', nuevoContrato);
    } else if (nuevoContrato.TipoNegocioId == 6) {
        result = MSExecuteOnServer('/CompraNet/GrabarAcuerdo', nuevoContrato);
    } else {
        result = MSExecuteOnServer('/CompraNet/GrabarFijacion', nuevoContrato);
    }

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
            $.unblockUI();
        }
        else {
            if (Siguientes != undefined && Siguientes != null && Siguientes != "" && Siguientes != "[]") {
                var siguientesObj = JSON.parse(Siguientes.replace(/(&quot\;)/g, "\""));
                var primero = siguientesObj.shift();
                editarContrato(primero.Id, primero.TipoNegocioId, siguientesObj);
            } else {
                if (result.ListaCupos.length > 0) {
                    mostrarResultados(result);
                    $.unblockUI();

                    $("#resultadoCupo").on('hidden.bs.modal', function () {
                        window.location.href = window.location.origin + "/CompraNet";
                    });
                } else {
                    window.location.href = window.location.origin + "/CompraNet";
                }
            }
        }
    }
    $.unblockUI();
}

function editarContrato(id, tipoId, siguientes) {
    window.location.href = window.location.origin + "/CompraNet/CrearContrato?id=" + id + '&tipoId=' + tipoId + (siguientes != undefined ? "&siguientes=" + JSON.stringify(siguientes) : "");
}

function obtenerLocalidadProvincia() {
    if ($("#buscadorProveedor").val() != "" && !$("#sinBoletoId").is(":checked")) {
        let cuitProvAux = $("#buscadorProveedor").val().split('(');
        let cuitProv = cuitProvAux[1].split(')');
        if (cuitProv[0] != null && $("#material").val() != "" && $("#campanaId").val() != "") {
            let localidadProvincia = MSExecuteOnServer('/CompraNet/ObtenerProvinciaLocalidadProv', {
                CUIT: cuitProv[0],
                MaterialId: $("#material").val(),
                CampanaId: $("#campanaId").val(),
                Consignatario: $("#consignatarioId").val()
            });
            if (localidadProvincia != null) {
                if (localidadProvincia.LocalidadId != "" && localidadProvincia.CUIT != "") {
                    $("#LocalidadCrearContrato").val(localidadProvincia.Localidad + "(" + localidadProvincia.Provincia + ")");
                    HabilitarEstablecimiento();
                } else {
                    $("#LocalidadCrearContrato").val("");
                    HabilitarEstablecimiento();
                }
            }
        }
    }
}

function AgregarDescuentos() {

    var descuento = {
        Id: 0,
        TipoPeriodoDBDesc: $("#tipoPeriodoDBId").data("kendoDropDownList").text(),
        TipoPeriodoDBId: $("#tipoPeriodoDBId").data("kendoDropDownList").value(),
        TipoDBDesc: $("#TipoDBId").data("kendoDropDownList").text(),
        TipoDBId: $("#TipoDBId").data("kendoDropDownList").value(),
        FechaDesde: $("#fechaDesdeDescuentoId").val(),
        FechaHasta: $("#fechaHastaDescuentoId").val(),
        Importe: $("#ImporteDescuentoId").val() !== null && $("#ImporteDescuentoId").val() !== "" ? $("#ImporteDescuentoId").val() : 0,
        MonedaId: $("#descuentoMonedaId").data("kendoDropDownList").value(),
        Porcentaje: $("#PorcentajeDescuentoId").val() !== null && $("#PorcentajeDescuentoId").val() !== "" ? $("#PorcentajeDescuentoId").val() : 0,
        Borrar: function () {
            eliminarDescuento(this);
        }
    };

    if (descuento.TipoDBId == 1 && descuento.TipoPeriodoDBId == 1) {
        MensErr("Los descuentos generales sobre precio se deben agregar desde la apertura de precio.");
        //AbrirModalAperturaDePrecio();
        return;
    }
    var err = validarDescuento(descuento);
    if (ExistsErrorMessages(err)) {
        MensErr(err[0]);
    }
    else {
        viewModel.Descuentos.push(descuento);
        $("#ImporteDescuentoId").data("kendoNumericTextBox").value("");
        $("#descuentoMonedaId").data("kendoDropDownList").value("");
        $("#PorcentajeDescuentoId").val("");
    }
}

function validarDescuento(descuento) {
    var errores = [];
    var descuentos = viewModel.Descuentos;
    var sonIguales = false;

    if (descuento.TipoPeriodoDBId == 0 || descuento.TipoPeriodoDBId == "" || descuento.TipoPeriodoDBId == null) {
        errores.push("El campo Descuento no puede estar vacíos");
    }
    if (descuento.TipoDBId == 0 || descuento.TipoDBId == null || descuento.TipoDBId == "") {
        errores.push("El campo Tipo no puede estar vacíos");
    }
    //if (descuento.TipoPeriodoDBId != 1 && (descuento.FechaDesde == "" || descuento.FechaDesde == undefined || descuento.FechaHasta == "" || descuento.FechaHasta == undefined)) {
    //    errores.push("La fecha no puede estar vacía");
    //}
    var fechaD = kendo.parseDate(descuento.FechaDesde, "dd-MM-yyyy");
    var fechaH = kendo.parseDate(descuento.FechaHasta, "dd-MM-yyyy");
    if (descuento.TipoPeriodoDBId != 1 && (!fechaD || !fechaH)) {
        errores.push("La fecha no es válida");
    }
    if (descuento.Importe == 0 && descuento.Porcentaje == 0) {
        errores.push("El campo Importe y Porcentaje no pueden estar vacíos");
    }
    if (descuento.Importe !== 0 && (descuento.MonedaId == "Moneda" || descuento.MonedaId == null || descuento.MonedaId == undefined || descuento.MonedaId == "")) {
        errores.push("El campo Moneda no puede estar vacío");
    }

    if (descuento.Porcentaje > 5 /*&& $("#canjeId").is(":checked")*/) {
        errores.push("El campo Porcentaje no puede ser mayor al 5%");
    }
    var topeD = kendo.parseDate($("#fechaDesdeTopeId").val(), "dd-MM-yyyyy");
    var topeH = kendo.parseDate($("#fechaHastaTopeId").val(), "dd-MM-yyyyy");

    if (descuento.TipoPeriodoDBId != 1 && (fechaD < topeD || fechaH > topeH)) {
        errores.push("El descuento o bonificación que intenta agregar esta fuera del rango de la fijacion.");
    }
    if (descuento.TipoDBId == "2") {
        var porcentajeNum = parseFloat(descuento.Porcentaje.toString().replace(',', '.'));
        if (($("#material").val() == "4" || $("#material").val() == "5") && (porcentajeNum > 1 || porcentajeNum < 0)) {
            errores.push("El porcentaje debe estar entre 0% y 1%");
        }
    }
    if (descuentos != undefined && descuentos != null && descuentos.length > 0) {
        for (var i = 0; i < descuentos.length; i++) {
            console.log(descuento.FechaDesde);
            if (descuentos[i].TipoPeriodoDBId == descuento.TipoPeriodoDBId && descuentos[i].TipoDBId == descuento.TipoDBId
                && descuentos[i].TipoPeriodoDBId == 1
            ) {
                sonIguales = true;
                break;
            }

            if (descuentos[i].TipoPeriodoDBId == descuento.TipoPeriodoDBId && descuentos[i].TipoDBId == descuento.TipoDBId
                && descuentos[i].TipoPeriodoDBId != 1 && descuento.TipoPeriodoDBId != 1) {
                var descD = kendo.parseDate(descuentos[i].FechaDesde, "dd-MM-yyyyy");
                var descH = kendo.parseDate(descuentos[i].FechaHasta, "dd-MM-yyyyy");
                if ((descD >= fechaD && descD < fechaH) || (descH >= fechaD && descH < fechaH)) {
                    sonIguales = true;
                    break;
                }
            }
        }
    }
    if (sonIguales) {
        errores.push("El descuento o bonificación que intenta agregar ya existe, debe anular el dto/bonif existente");
    }
    return errores;
}

function AgregarCalidades() {
    if ($("#material").val() == 3 &&
        $("#valorEspecialesId").val() === ""
        && $("#porcentajeDesdeId").val() === ""
        && $("#porcentajeHastaId").val() === "") {
        var cal = ValorDeCalidad($("#calidadesEspecialesId").data("kendoDropDownList").value());
        if ($("#calidadesEspecialesId").data("kendoDropDownList").value() == 2 && (cal == undefined || cal.CalidadEspecialId != "2")) {
            var cal1 = {
                Id: 0,
                CalidadEspecialDesc: "Granos verdes",
                CalidadEspecialId: 2,
                Valor: "0",
                PorcentajeDesde: "0",
                PorcentajeHasta: "20",
                StandardDeCalidadId: 2,
                Borrar: function () {
                    viewModel.Calidades.remove(this);
                }
            };
            viewModel.Calidades.push(cal1);
            var cal2 = {
                Id: 0,
                CalidadEspecialDesc: "Granos verdes",
                CalidadEspecialId: 2,
                Valor: "0,2",
                PorcentajeDesde: "20,1",
                PorcentajeHasta: "100",
                StandardDeCalidadId: 2,
                Borrar: function () {
                    viewModel.Calidades.remove(this);
                }
            };
            viewModel.Calidades.push(cal2);
        } else if ($("#calidadesEspecialesId").data("kendoDropDownList").value() == 1 && (cal == undefined || cal.CalidadEspecialId != "1")) {
            var cal3 = {
                Id: 0,
                CalidadEspecialDesc: "Dañados",
                CalidadEspecialId: 1,
                Valor: "0",
                PorcentajeDesde: "0",
                PorcentajeHasta: "5",
                StandardDeCalidadId: 2,
                Borrar: function () {
                    viewModel.Calidades.remove(this);
                }
            };
            viewModel.Calidades.push(cal3);

            var cal4 = {
                Id: 0,
                CalidadEspecialDesc: "Dañados",
                CalidadEspecialId: 1,
                Valor: "1",
                PorcentajeDesde: "5,1",
                PorcentajeHasta: "40",
                StandardDeCalidadId: 2,
                Borrar: function () {
                    viewModel.Calidades.remove(this);
                }
            };
            viewModel.Calidades.push(cal4);
        }
    } else {
        var calidades = {
            Id: 0,
            CalidadEspecialDesc: $("#calidadesEspecialesId").data("kendoDropDownList").text(),
            CalidadEspecialId: $("#calidadesEspecialesId").data("kendoDropDownList").value(),
            Valor: $("#valorEspecialesId").val(),
            PorcentajeDesde: $("#porcentajeDesdeId").val() != "" ? $("#porcentajeDesdeId").val() : null,
            PorcentajeHasta: $("#porcentajeHastaId").val() != "" ? $("#porcentajeHastaId").val() : null,
            StandardDeCalidadId:
                $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado 2" ? 7 :
                    $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Bonif. SECO de 7% a 10% Por punto" ? 6 :
                        $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Fabrica" ? 3 :
                            $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Especial" || $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado" ||
                                $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Dañados" || $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Granos verdes" ? 2 : 0,

            Borrar: function () {
                viewModel.Calidades.remove(this);
            }
        };

        var err = validarCalidad(calidades);
        if (ExistsErrorMessages(err)) {
            MensErr(err[0]);
        }
        else {
            viewModel.Calidades.push(calidades);
            $("#valorEspecialesId").data("kendoNumericTextBox").value("");
            $("#porcentajeDesdeId").data("kendoNumericTextBox").value("");
            $("#porcentajeHastaId").data("kendoNumericTextBox").value("");

            if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Granos verdes") {
                $("#valorEspecialesId").data("kendoNumericTextBox").value('0,20');
            }
            else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Dañados") {
                $("#valorEspecialesId").data("kendoNumericTextBox").value('0,50');
            }
        }
        return err;
    }
}

function validarCalidad(calidad) {
    var errores = [];
    var porcDesde = parseFloat(calidad.PorcentajeDesde);
    var porcHasta = parseFloat(calidad.PorcentajeHasta);
    var ultimaCalidad = ValorDeCalidad(calidad.CalidadEspecialId);

    if (calidad.CalidadEspecialId === 0 || calidad.CalidadEspecialId === "" || calidad.CalidadEspecialId === null) {
        errores.push("El campo Calidades Especiales no puede estar vacio");
    }
    if (calidad.Valor === "" || calidad.Valor === null || calidad.Valor === "undefined") {
        errores.push("El campo Valor no puede estar vacio");
    }
    if (porcDesde > porcHasta) {
        errores.push("El Porcentaje Desde no puede ser mayor que el Porcentaje Hasta");
    }
    if (ultimaCalidad == undefined && porcDesde > 0) {
        errores.push("El Porcentaje Desde no puede ser mayor a 0");
    }
    if ((calidad.CalidadEspecialId == 1 || calidad.CalidadEspecialId == 2) &&
        (calidad.PorcentajeHasta == null || calidad.PorcentajeDesde == null)) {
        errores.push('El Porcentaje es obligatorio');
    }
    if (calidad.PorcentajeHasta > 50 && calidad.CalidadEspecialId == 1) {
        errores.push('El Porcentaje Hasta no debe ser mayor a 50% para "Dañados"');
    }
    if (calidad.PorcentajeHasta > 100 && calidad.CalidadEspecialId == 2) {
        errores.push('El Porcentaje Hasta no debe ser mayor a 100% para "Granos verdes"');
    }
    if (calidad.PorcentajeHasta - Math.floor(calidad.PorcentajeHasta) != 0) {
        errores.push('El Porcentaje Hasta debe ser Entero');
    }
    if (ultimaCalidad !== undefined && ultimaCalidad.PorcentajeHasta + ",1" != calidad.PorcentajeDesde) {
        errores.push("El Porcentaje Desde debe ser el último Porcentaje Hasta más 0,10");
    }


    return errores;
}

function ValorDeCalidad(calidadId) {
    var calidadGrano = viewModel.Calidades;
    var calidad;
    for (var i in calidadGrano) {
        if (calidadGrano[i].CalidadEspecialId == calidadId) {
            calidad = calidadGrano[i];
        }
    }
    return calidad;
}

function InicializarContratoEdit() {
    var datos = { id: Id };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerContratoCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
}

function InicializarFijacionEdit() {
    var datos = { id: Id };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerFijacionCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
    InicializarBordesRojos();
    cargarContratoAFijarSeleccionado();
}
function InicializarFasonEdit() {
    var datos = { id: Id };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerFasonCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
    InicializarBordesRojos();
}
function InicializarAgenteEdit() {
    var datos = { id: Id };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerAgenteCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
    InicializarBordesRojos();
}
function InicializarAcuerdoEdit() {
    var datos = { id: Id };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerAcuerdoCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
    InicializarBordesRojos();
}

function CargarDatosEditar(contrato, hijo) {
    InicializarBordesRojos();
    $("#precioId").data("kendoNumericTextBox").value("");
    $("#precioTotalApertura").data("kendoNumericTextBox").value("");
    $("#precioMonedaId").val("");
    $("#buscadorCorredor").val(contrato.Corredor);
    $("#buscadorCorredor").trigger("change");
    if (contrato.Corredor != null && contrato.Corredor != "") {
        $('#contCorredorDiv').show();
        $('#porcentajeComisionDiv').show();
    }
    $("#proveedorId").val(contrato.ProveedorId);
    $("#estado").val(contrato.Estado);
    $("#buscadorProveedor").val(contrato.Proveedor);
    $("#buscadorProveedor").trigger("change");

    if (contrato.FechaOperacionFormateado != null) {
        $("#fechaOperacionId").val(contrato.FechaOperacionFormateado);
        //$("#fechaFijacionId").val(contrato.FechaOperacionFormateado);

    } else {
        $("#fechaOperacionId").val("");
        //$("#fechaFijacionId").val("");

    }
    if (contrato.ProveedorCreador != null) {
        $(".esconderTercero").removeClass("ampliar");
    } else {
        $(".esconderTercero").addClass("ampliar");
    }
    if (contrato.Precio != null && contrato.Precio > 0 && contrato.TipoNegocioId != 1) {
        $("#precioId").data("kendoNumericTextBox").value(contrato.Precio);
        $("#precioMonedaId").val(contrato.MonedaId);
    }
    if (contrato.PrecioNeto != null && contrato.TipoNegocioId != 1) {
        $("#precioTotalApertura").data("kendoNumericTextBox").value(contrato.PrecioNeto);
    }

    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth();
    var dia = hoy.getDate();
    hoy = new Date(anio, mes, dia);

    var fechaop = new Date(parseInt(contrato.FechaOperacion.substr(6)));
    if (fechaop < hoy) {
        if (contrato.TipoNegocioId != 3) {
            var listMotivos = $("#motivoAnterior").data("kendoDropDownList").dataSource.data().filter(function (x) { return x.Descripcion == contrato.MotivoOperacionAnterior });
            if (listMotivos != null && listMotivos.length > 0) {
                $("#motivoOperacionAnteriorId").val(contrato.MotivoOperacionAnterior);
                $("#motivoAnterior").data("kendoDropDownList").value(listMotivos[0].Id);
                $("#motivoAnterior").data("kendoDropDownList").trigger("change");
                $("#descripcionMotivoAnterior").val(contrato.DescripcionOperacionAnterior);

            } else {
                $("#motivoAnterior").data("kendoDropDownList").text("Otro");
                $("#motivoAnterior").data("kendoDropDownList").trigger("change");
                $("#motivoOperacionAnteriorId").val(contrato.MotivoOperacionAnterior);
                $("#descripcionMotivoAnterior").val(contrato.DescripcionOperacionAnterior);
            }
            $(".fechaOperacionMotivoDiv").show();
            var hoy = new Date();
            var anio = hoy.getFullYear();
            var mes = hoy.getMonth();
            var dia = hoy.getDate();
            hoy = new Date(anio, mes, dia);
            var fechaop = new Date(parseInt(contrato.FechaOperacion.substr(6)));
            const diffTime = Math.abs(hoy - fechaop);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (fechaop < hoy) {
                if (Id > 0 && diffDays > 1) {
                    //$("#noInformaSioId").attr("disabled", true);
                }
            }
        }

    } else {
        $("#motivoOperacionAnteriorId").val("");
        $("#descripcionMotivoAnterior").val("");
        $(".fechaOperacionMotivoDiv").hide();
        $("#motivoOperacionAnteriorFijacion").val("");
        $("#fechaFijacionMotivoDiv").hide();
    }

    $("#fechaDesdeId").val(contrato.FechaDesdeFormateado);
    $("#fechaHastaId").val(contrato.FechaHastaFormateado);
    //$("#fechaCiertaId").val(contrato.FechaCiertaFormateado);
    //$("#fechaCiertaAcuerdo").val(contrato.FechaCiertaFormateado);
    $("#porcentajeDePagoId").data("kendoNumericTextBox").value(contrato.PorcentajeDePago == null ? 97.5 : contrato.PorcentajeDePago);


    $("#material").data("kendoDropDownList").value(contrato.MaterialId);
    $("#material").data("kendoDropDownList").trigger("change");

    $("#NivelTarifaId").data("kendoDropDownList").value(contrato.NivelTarifaId);
    $("#NivelTarifaId").data("kendoDropDownList").trigger("change");

    $("#TarifaFleteId").data('kendoNumericTextBox').value(contrato.TarifaFlete);

    $("#observacionId").val(contrato.Observacion);
    $("#cantidadId").data("kendoNumericTextBox").value(contrato.Cantidad);
    if (contrato.Cantidad > 0) {
        CalcularMaximo();
    }
    $("#cantidadId").trigger("change");

    //$("#precioId").data("kendoNumericTextBox").value(contrato.Precio);
    //$("#precioId").trigger('change');

    //$("#precioTotalApertura").data("kendoNumericTextBox").value(contrato.PrecioNeto);
    //$("#precioMonedaId").data("kendoDropDownList").value(contrato.MonedaId);
    //$("#precioMonedaId").data("kendoDropDownList").trigger("change");

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

    $("#campanaId").data("kendoDropDownList").value(contrato.CampanaId);

    if (contrato.Base == true) {
        $("#baseId").prop("checked", true);
    } else {
        $("#baseId").prop("checked", false);
    }



    //if (contrato.Dolarizado) {
    //    $("#dolarizadoId").prop("checked", true);
    //    $("#dolarizadoDiv").show();
    //    $("#dolarizadoFechaId").val(contrato.Fecha_DolarizadoFormateado);
    //}

    //if (contrato.PagoDiferido === true && contrato.TipoNegocioId != 3) {
    //    $("#pesificadoId").prop("checked", true);
    //    $("#pesificadoDiv").show();
    //    $("#pesificadoDiasId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    //} else {
    //    $("#pesificadoId").prop("checked", true);
    //    $("#diasDiferidoId").prop("checked", true);
    //    $("#diasDiferidoFijacionDiv").addClass("inline-fijacion");
    //    $("#diasDiferidoFijacionDiv").removeClass("hide-fijacion");
    //    $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    //}
    //if (contrato.PagoDiferido === true) {
    //    $("#pesificadoId").prop("checked", true);
    //    $("#pesificadoDiv").show();
    //    $("#pesificadoDiasId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    //} else {
    //    $("#pesificadoId").prop("checked", false);
    //    $("#diasDiferidoId").prop("checked", false);
    //    $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");
    //}

    if (contrato.PorcentajeComision !== null && contrato.PorcentajeComision !== undefined && contrato.PorcentajeComision !== "") {
        $("#porcentajeComision").data("kendoNumericTextBox").value(contrato.PorcentajeComision);
        $("#porcentajeComision").data("kendoNumericTextBox").trigger("change");
    }
    if (contrato.NoInformaSIO == true) {
        $("#noInformaSioId").prop("checked", true);
    } else {
        $("#noInformaSioId").prop("checked", false);
    }

    if (contrato.TrigoEspecial == true) {
        $("#trigoEspecialFasonId").prop("checked", true);
    }

    if (contrato.CD) {
        $("#CDId").prop("checked", true);
        $(".ocultar").show();
    } else {
        $("#CDId").prop("checked", false);
    }
    if (contrato.Warrant) {
        $("#WarrantId").prop("checked", true);
        $(".ocultar").show();
    } else {
        $("#WarrantId").prop("checked", false);
    }
    if (contrato.ChequeElectronico == true) {
        $("#chequeElectronico").prop("checked", true);
        $("#chequeElectronicoInput").prop("checked", true);
        $("#pagoCbuDiv").hide();
        $("#pagoCbuId").hide();
    } else {
        $("#chequeElectronico").prop("checked", false);
        $("#chequeElectronicoInput").prop("checked", false);
    }
    //if (contrato.DolarizadoExpress == true) {
    //    $("#dolarizadoExpressDiv").show();
    //    $("#dolarizadoDiv").show();
    //    $("#dolarizadoFechaId").val(contrato.Fecha_DolarizadoFormateado);
    //    $("#dolarizadoExpressId").prop("checked", true);
    //    //$("#chequeElectronicoDiv").hide();
    //    //$("#pagoCbuDiv").hide();


    //} else {
    //    $("#dolarizadoExpressId").prop("checked", false);
    //    //$("#dolarizadoDiv").hide();
    //}

    //if (contrato.PagoCBU != "" && contrato.PagoCBU != null) {
    //    $("#pagoCbu").data("kendoAutoComplete").value(contrato.PagoCBU);
    //    $("#pagoCbuInput").data("kendoAutoComplete").value(contrato.PagoCBU);
    //    $("#chequeElectronicoId").hide();
    //    $("#chequeElectronicoDiv").hide();
    //}
    LimpiarServicios();
    if (contrato.Servicios != null && contrato.Servicios.length > 0) {
        ArmarDescripcionServicio(contrato.Servicios);
        viewModel.set("Servicios", contrato.Servicios);
        kendo.bind($("#ModalServicio"), viewModel);
        InicializarServicios();
        $("#servicioBtn").show();
    } else {
        MostrarServiciosYCalidades();
    }

    contrato.PagoDirectoVendedor == true ? $("#pagoDirectoId").prop("checked", true) : $("#pagoDirectoId").prop("checked", false);
    contrato.EstablecimientoPropio == true ? $("#establecimientoPropioId").prop("checked", true) : contrato.EstablecimientoPropio == false ? $("#establecimientoArrendadoId").prop("checked", true) : false;

    if (contrato.PagoDirectoVendedor == true) {
        $("#pagoDirectoDiv").show();
        $("#pagoDirectoId").prop("checked", true);
    }

    if (contrato.Corredor != "") {
        $("#pagoDirectoDiv").addClass("ampliar");
    } else {
        $("#pagoDirectoDiv").removeClass("ampliar");
    }



    $("#contCorredorId").val(contrato.ContratoCorredor);
    $("#contVendedorId").val(contrato.ContratoVendedor);
    if (hijo != true) {
        if (contrato.Madre === true) {
            $("#fasonIdCheck").attr("disabled", true);
            $("#madreId").prop("checked", true);
            $("#pagosDiv").show();
        }
        if (contrato.Madre === false) {
            $("#fasonIdCheck").attr("disabled", false);
            $("#hijoId").prop("checked", true);
            $(".contratoMadreDiv").show();
            $("#contMadreId").val(contrato.ContratoMadre);
        }
    }
    if (contrato.EsFason === true) {
        $("#madreId").attr("disabled", true);
        $("#fasonIdCheck").prop("checked", true);
        $("#pagosDiv").hide();
    } else {
        $("#madreId").attr("disabled", false);
    }

    contrato.SelCargoMOA === true ? $("#selCargoMOAId").prop("checked", true) : $("#selCargoMOAId").prop("checked", false);
    contrato.SelCargoVendedor === true ? $("#selCargoVendedorId").prop("checked", true) : $("#selCargoVendedorId").prop("checked", false);

    $("#tipoFasonId").data("kendoDropDownList").value(contrato.TipoFasonId);
    $("#operadorId").data("kendoDropDownList").value(contrato.OperadorId);
    $("#posicionFasonId").val(contrato.Posicion);


    if (contrato.Canje == true) {
        ocultarSiHayCanje();
        $("#mostrarCanje").show();
        $("#prestamoDevolucionDiv").hide();
        $("#prestamoDevolucionId").prop("checked", false);
        $("#plantaDestinoId").data("kendoDropDownList").value("");
        $("#plantaDestinoId").data("kendoDropDownList").trigger("change");
        $("#canjeId").prop("checked", true);
        $("#montoId").data("kendoNumericTextBox").value(contrato.Monto);
        $("#montoMonedaId").data("kendoDropDownList").value(contrato.MonedaCanjeId);
        $("#montoMonedaId").data("kendoDropDownList").trigger("change");
        $("#insumoId").val(contrato.Insumo);
        $("#plantaDestinoDiv").hide();
        $("#plantaDestinoId").data("kendoDropDownList").value("");
        $("#plantaDestinoId").data("kendoDropDownList").trigger("change");
        $("#boletoCartaId").prop("checked", false);
        $("#boletoNingunoId").prop("checked", false);
        $("#boletoCartaId").prop("disabled", true);
        $("#boletoNingunoId").prop("disabled", true);
        CargarCalidadPorMaterial(contrato.MaterialId);
    }
    LimpiarDescuentos();
    LimpiarCalidades();
    contrato.MercsDeposito == true ? $("#mercsDepositoId").prop("checked", true) : $("#mercsDepositoId").prop("checked", false);
    if (contrato.MercsDeposito == true) {
        HayMercaderia();
        if ($("#boton-ampliar").text() != "+ AMPLIAR") {
            $(".depositoDiv").show();
        } else {
            $(".depositoDiv").hide();
        }
        $("#cantidadDeposito").data("kendoNumericTextBox").value(contrato.CantidadDeposito);
    }
    var descuentosDto = contrato.Descuentos;
    $.each(descuentosDto, function (key, descuento) {
        var descuentoKendo = {
            Id: hijo ? 0 : descuento.Id,
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
                eliminarDescuento(this);
            }
        };
        if (descuentoKendo.TipoDBId == 1 && descuentoKendo.TipoPeriodoDBId == 1 && descuentoKendo.Porcentaje > 0) {
            $("#PorcentajeDescuentoAFijarId").val(descuentoKendo.Porcentaje);
        }
        viewModel.Descuentos.push(descuentoKendo);
    });

    calidadesDto = contrato.Calidades;
    $.each(calidadesDto, function (key, calidad) {
        var calidadKendo = {
            Id: hijo ? 0 : calidad.Id,
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

    if (contrato.AperturaPrecios != null && contrato.AperturaPrecios.length != 0 && contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 1; }).MonedaId != null) {

        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 1; }).Importe);
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 2; }).Importe);
        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").max(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Importe);
        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Importe);
        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Porcentaje);
        $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 4; }).Importe);
        $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 4; }).Porcentaje);
        if (contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 5; }) != null) {
            $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 5; }).Importe);
        }

        $("#precioMonedaAFijarId").data("kendoDropDownList").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 1; }).MonedaId);
        $("#precioMonedaAFijarId").data("kendoDropDownList").trigger("change");
        InsertarAperturasViewModel();
    }

    //MostrarTablaPrecioPactado();
    if (contrato.StandardCalidadId == 2) {
        $(".calidadesEspecialesDatos").show();
    } else {
        $(".calidadesEspecialesDatos").hide();
    }
    if (contrato.Calidades !== null) {
        var descripcion = contrato.Calidades.length > 0 ? contrato.Calidades[0].CalidadEspecialDesc : contrato.StandardDeCalidadDescripcion;
        if (descripcion != null) $("#calidadesEspecialesId").data("kendoDropDownList").text(descripcion);
        CambioCalidades(contrato.Calidades);
        $("#zonasGirasolAltoId").data("kendoDropDownList").value(contrato.ZonaId);
    }
    if (contrato.Pizarra === true) {
        $("#pizarraId").prop("checked", true);
        ClickEnPizarra();
    } else {
        $("#pizarraId").prop("checked", false);
    }

    if (contrato.Estado == 5) {
        $("#tipoId").data("kendoDropDownList").enable(false);
        $("#material").data("kendoDropDownList").enable(false);
        $("#fechaOperacionId").data("kendoDatePicker").enable(false);
        $(".copia-contratos").hide();
        if (!contrato.TipoNegocioId == 3) {
            $("#contrato-modificado").html("<h3>CONTRATO " + contrato.ContratoSAP.replace('000', '') + "</h3>");
        }

    }
    //if (contrato.Estado == 5 && contrato.TipoNegocioId == 3) {
    //    $("#buscadorCorredor").data("kendoAutoComplete").enable(false);
    //    $("#buscadorProveedor").data("kendoAutoComplete").enable(false);
    //    $("#contratoId").data("kendoAutoComplete").enable(false);
    //    $("#cantidadId").data("kendoNumericTextBox").enable(false);
    //    //$("#precioId").data("kendoNumericTextBox").enable(false);
    //    //$("#precioMonedaId").data("kendoDropDownList").enable(false);
    //    $("#aperturaPrecioBtn").attr("disabled", true);
    //    $("#diasDiferidoId").attr("disabled", true);
    //    $("#diasDiferidoFijacionId").data("kendoNumericTextBox").enable(false);
    //    $("#comercialFijacionId").data("kendoDropDownList").enable(false);
    //    $("#motivoOperacionAnteriorFijacion").attr("disabled", true);
    //    $("#fechaFijacionId").data("kendoDatePicker").enable(false);
    //    //$("#pagoCbuInput").data("kendoAutoComplete").enable(false);

    //}

    if (contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 2) {

        $("#AgenteCompraId").data("kendoDropDownList").value(contrato.TipoAgenteCompraId);
        if (contrato.TipoAgenteCompraId > 0) {
            $("#caratulaExtensionId").val(contrato.CaratulaExtension);
            $("#caratulaMATId").val(contrato.CaratulaMAT);
            $("#precioAjusteComisionId").data("kendoNumericTextBox").value(contrato.PrecioAjusteComision);
            $("#monedaAjusteComisionId").data("kendoDropDownList").value(contrato.MonedaAjusteComisionId);
            $("#montoMonedaId").data("kendoDropDownList").value(contrato.MonedaCanjeId);
            $("#boletoNingunoId").prop("checked", false);
            $("#boletoNingunoId").click();
            $("#boletoNingunoId").attr("readonly", "readonly");
            $("#boletoConfirmaId").attr("disabled", true);
            $("#boletoFisicoId").attr("disabled", true);
            $("#boletoCartaId").attr("disabled", true);
            $(".ocultarAgenteDiv").show();
        } else {
            OcultarCamposAgente();
        }
        if (contrato.ContratoAcuerdoId != null) {
            $("#contratoAcuerdoId").data("kendoAutoComplete").value(contrato.ContratoAcuerdoId);
        }
        if (contrato.Acuerdo != null) {
            $("#contratoAcuerdoId").data("kendoAutoComplete").value(contrato.Acuerdo);
        }
    }

    if (contrato.ProveedorCreador != null) {
        $("#datosCargaTercero").show();
        var p = "";
        if (contrato.ObservacionTercero != null) {
            p = contrato.ObservacionTercero.split("|");
            $("#visualizar_observacionTercero").text(p[0]);
        } else {

            $(".observacionTercero").hide();
        }

        var datosTercero = "";
        if (contrato.CalidadTercero == true) {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Calidad:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Calidad: </strong><span> Si ' + n + '</span><br>';
        }
        if (contrato.DolarizadoTercero == true) {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Dolarizado:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Dolarizado: </strong><span> Si ' + n + '</span><br>';
        }
        if (contrato.SustentableTercero == true) {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Sustentable:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Sustentable: </strong><span> Si ' + n + '</span><br>';
        }
        if (contrato.PagoDiferidoTercero == true) {
            var n = "";
            var f = p.filter(function (e) { return e.includes("Pago Diferido:") });
            if (f) {
                n = " - " + f[0].split(":")[1].trim();
            }
            datosTercero = datosTercero + '<strong style="float:left">Pago Diferido: </strong><span> Si ' + n + '</span><br>';
        }

        $("#visualizar_datosTercero").html(datosTercero);
    } else {
        $("#datosCargaTercero").hide();
        $(".observacionTercero").hide();
    }



    if (contrato.Estado == 5) {
        $("#prestamoDevolucionId").prop('disabled', true);
        $("#plantaDestinoId").data("kendoDropDownList").enable(false);
    }

    if (!hijo) {
        //$("#fechaOperacionId").val(contrato.FechaFormateado);
        $("#tipoId").data("kendoDropDownList").value(contrato.TipoNegocioId);
        //$("#tipoId").data("kendoDropDownList").trigger("change");
        if (contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 6) {
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
    }

    if (contrato.PrestamoDevolucion == true) {
        $("#prestamoDevolucionId").prop("checked", true);
        ocultarSiHayCanje();
        ocultarSiHayPrestamos();
        $("#plantaDestinoId").data("kendoDropDownList").value(contrato.PlantaDestinoId);
        $("#plantaDestinoId").data("kendoDropDownList").trigger("change");
    }

    if (contrato.Sustentable == true || contrato.EPA == true) {
        $("#sustentablePrecioId").data("kendoNumericTextBox").value(contrato.Importe_Sustentable);
        $("#sustentableMonedaId").data("kendoDropDownList").value(contrato.Moneda_Sustentable);
        if (contrato.Sustentable) {
            $("#sustentableId").prop("checked", true);
            $(".sojaEpa").hide();
            $(".sustenTipoDB").show();
        } else {
            $("#epaId").prop("checked", true);
            $(".sojaSustentable").hide();
            $(".sustenTipoDB").show();
        }
        $(".sustentableDiv").show();
        if (contrato.MercsDeposito == true) {
            $(".fechaHastaSustentableDiv").show();
            $("#fechaDesdeSustentableId").val(contrato.FechaDesde_SustentableFormateado);
            $("#fechaHastaSustentableId").val(contrato.FechaHasta_SustentableFormateado);
        }
        if (contrato.TarifaAConvenir == true) {
            $("#tarifaAConvenirId").prop("checked", true);
            $("#sustentablePrecioId").addClass("disabled").prop("disabled", true);
            $("#sustentableMonedaId").addClass("disabled").prop("disabled", true);
        }
        if (contrato.SustentableTipoDBId != null) {
            $("#selectSustenTipoDB").data("kendoDropDownList").value(contrato.SustentableTipoDBId);
            $("#selectSustenTipoDB").data("kendoDropDownList").trigger("change");
        }
    }

    if (contrato.AnulaYReemplazaContratoId) {
        $("#AnulaYReemplazaContratoId").val(contrato.AnulaYReemplazaContratoId);
        $("#contratoAReemplazarId").val(contrato.AnulaYReemplazaContratoSAP);
        $("#MotivoReemplazo").val(contrato.MotivoReemplazo);
        $(".mostrarConPase").show();
        $("#tipoId").data("kendoDropDownList").readonly(true);
        $("#motivoAnterior").data("kendoDropDownList").readonly(true);

        var hoy = new Date();
        var fechaContrato = contrato.FechaOperacionFormateado;
        $("#fechaOperacionId").data("kendoDatePicker").setOptions({
            month: {
                content: '# if((data.date.getFullYear() == ' + hoy.getFullYear()
                    + '&& data.date.getMonth() == ' + hoy.getMonth()
                    + '&& data.date.getDate() == ' + hoy.getDate() + ')'
                    + '|| (data.date.getFullYear() == ' + fechaop.getFullYear()
                    + '&& data.date.getMonth() == ' + fechaop.getMonth()
                    + '&& data.date.getDate() == ' + fechaop.getDate() + ')' +
                    ') { #' +
                    '#= data.value #' +
                    '# } else { #' +
                    '<div class="disabledDay">#= data.value #</div>' +
                    '# } #'
            }
        });
        var dateView = $("#fechaOperacionId").data("kendoDatePicker").dateView;
        dateView._calendar();
        var calendar = dateView.calendar;
        calendar.bind("navigate", function () {
            $(".disabledDay").parent().removeClass("k-link")
            $(".disabledDay").parent().removeAttr("href")
        });

        $("#fechaOperacionId").val(fechaContrato);
        $("#fechaOperacionId").data("kendoDatePicker").enable(true);
        $("#descripcionMotivoAnterior").attr("readonly", true);

        $("#noInformaSioId").prop("checked", true);
        $("#noInformaSioId").attr('disabled', true);

        $("#boletoNingunoId").click();
        $("#boletoNingunoId").attr("disabled", true);
        $("#boletoNingunoId").prop("checked", true);

        $("#boletoConfirmaId").prop("checked", false);
        $("#boletoFisicoId").prop("checked", false);
        $("#boletoCartaId").prop("checked", false);
        $("#boletoConfirmaId").attr("disabled", true);
        $("#boletoFisicoId").attr("disabled", true);
        $("#boletoCartaId").attr("disabled", true);
    }
    DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
    if (contrato.TipoNegocioId == 2 && contrato.Condicional == true) {
        $("#condicionalId").prop("checked", true);
        $("#condicionalPrecioId").val(contrato.CondicionalPrecio);
        $("#condicionalMonedaId").val(contrato.CondicionalMonedaId);
        $("#condicionalCantidadId").val(contrato.CondicionalCantidad);
        $("#condicionalFechaId").val(contrato.CondicionalFecha);
        $("#condicionalPosicionId").val(contrato.CondicionalPosicion);
    }
    if (contrato.TipoNegocioId != 1) {
        $("#tipoId").data("kendoDropDownList").trigger("change");
    }


    if (contrato.ObligatoriedadBonificacion == true) {
        $("#esBonificacion").prop("checked", true);
    } else if (contrato.ObligatoriedadBonificacion == false) {
        $("#esBonificacion").prop("checked", false);
    }

    if (contrato.PosicionCBOT != null && contrato.PosicionCBOT != "") {
        if (contrato.TipoPosicionCBOTId == 3) {
            var asociados = MSExecuteOnServer('/CompraNet/DevolverSiTieneAsociados', { contratoId: contrato.Id });
            if (asociados == true) {
                $("#tipoPosicionCBOTId").data("kendoDropDownList").enable(false);
                $("#tipoId").data("kendoDropDownList").enable(false);
                $('#material').data("kendoDropDownList").enable(false);
                $("#posicionCBOTId").prop("disabled", true);
            }

        }
        $("#posicionCBOTId").val(contrato.PosicionCBOT);
        $("#tipoPosicionCBOTId").data("kendoDropDownList").value(contrato.TipoPosicionCBOTId);
        $("#posicionCBOTDiv").show();
        DeshabilitarConPase();
    }

    LimpiarBoleto();
    boletoId = contrato.BoletoId;
    if (hijo || contrato.BoletoId == 3) {
        $("#boletoNingunoId").prop("checked", true);
    }
    else if (contrato.BoletoId == 1) {
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
    } else if (contrato.BoletoId == 5) {
        $("#sinBoletoId").prop("checked", true);
        $("#mercsDepositoId").prop("checked", true)
        $("#mercsDepositoId").attr("disabled", true);
        $("#fechaDesdeId").data('kendoDatePicker').enable(false);
        $("#fechaHastaId").data('kendoDatePicker').enable(false);
        HayMercaderia();
        if (contrato.Estado == 5 || contrato.Estado == 7) {
            DeshabilitarCampoSiEsBoleto();
        }
    }



    if (contrato.Id > 0) {
        //$("#maximaId").data("kendoNumericTextBox").value(contrato.KgMaximo);
        CalcularMaximo();
        $("#minimoId").data("kendoNumericTextBox").value(contrato.KgMinimo);
    }
    //else {
    //    CalcularMaximo();
    //}


    if (contrato.CondicionalContratoId != null && contrato.CondicionalContratoId > 0) {
        $("#buscadorProveedor").prop('disabled', true);
        $("#buscadorCorredor").prop('disabled', true);
        $('#material').data("kendoDropDownList").enable(false);
        $("#cantidadId").data("kendoNumericTextBox").enable(false);
        $("#contratoCondicionalId").val(contrato.CondicionalContratoId);
        $("#contratoCondicional").val(contrato.CondicionalContratoSAP);
    } else {
        $("#precioId").data("kendoNumericTextBox").enable(true);
        //$("#precioMonedaId").data("kendoDropDownList").enable(true);
        $("#buscadorProveedor").prop('disabled', false);
        $("#buscadorCorredor").prop('disabled', false);
        if (contrato.Estado != 5) {
            $('#material').data("kendoDropDownList").enable(true);
        }
        $("#cantidadId").data("kendoNumericTextBox").enable(true);
        $("#contratoCondicionalId").val("");
        $("#contratoCondicional").val("");
        //$("#condicionalId").removeAttr("disabled");

    }

    if ((contrato.Id == 0 || contrato.Id == null) && contrato.ProveedorId > 0) {
        var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: contrato.ProveedorId });
        CargarAutomaticamenteLaComision(compraNet);
    }

    if (contrato.ProveedorComisionistaId != null) {
        $("#comisionistaCheckId").prop("checked", true);
        EsComisionista(null);
    } else {
        $("#comisionistaCheckId").prop("checked", false);
    }

    if (contrato.FechaHastaOriginalFormateado != null && contrato.FechaHastaOriginalFormateado != "") {
        $(".fechahastaOriginalDiv").show();
        $("#fechaHastaOriginalId").data("kendoDatePicker").value(contrato.FechaHastaOriginalFormateado);
    }

    if (contrato.ConDescarga == true) {
        $("#conDescargaId").prop("checked", true);
        $("#btnConDescarga").hide();
        if (contrato.Id > 0) {
            $("#conDescargaId").prop("disabled", true);
        }
    }
    //Fin cargar datos editar
}

function LimpiarApertura() {
    var iteraciones = viewModel.AperturaPrecio.length;
    for (i = 0; i < iteraciones; i++) {
        viewModel.AperturaPrecio.pop();
    }
    $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value("");
    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value("");
    $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value("");
    $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value("");
    $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value("");

}

function AutocompleteProcedencia() {
    $("#LocalidadCrearContrato").click(function () {
        $("#LocalidadCrearContrato").data("kendoAutoComplete").value("");
        $("#establecimientoDiv").hide();
        $("#LocalidadCrearContrato").trigger("change");
        $("#establecimientoPropioId").prop("checked", false);
        $("#establecimientoArrendadoId").prop("checked", false);
        DatosProveedor();
    });

    $("#LocalidadCrearContrato").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function () {
            if ($("#LocalidadCrearContrato").val().split('|').length > 1) {
                $("#LocalidadCrearContrato").val($("#LocalidadCrearContrato").val().split('|')[1]);
                HabilitarEstablecimiento();
            }
            ValidarSinBoleto();
        },
        select: function (e) {
            $("#ProvinciaId").val(e.dataItem.ProvinciaId);
            DatosProveedor();
            SeleccionAutomaticaBolsa();
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#LocalidadCrearContrato').val() };
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

function HabilitarEstablecimiento() {
    if ($("#LocalidadCrearContrato").val() != "") {
        let provAux = $("#LocalidadCrearContrato").val().split('(');
        let prov = provAux[1].split(')');
        //if (prov[0] === "BUENOS AIRES") {
        //    $("#establecimientoDiv").show();
        //} else {
        //    $("#establecimientoDiv").hide();
        //}
    }
}

function validarGuardarApertura(precioPactado) {
    if (precioPactado.Precio === 0 && (precioPactado.FechaDesde === "" || precioPactado.FechaDesde === undefined)
        && (precioPactado.FechaHasta === "" || precioPactado.FechaHasta === undefined) && precioPactado.ImportePactado == 0
        && (precioPactado.MonedaImportePactadoId === "" || precioPactado.MonedaImportePactadoId === undefined)
        && (precioPactado.FechaHasta === "" || precioPactado.FechaHasta === undefined)) {

        return true;
    }
    return false;
}

function GuardarAperturaDePrecio() {
    if (AperturaPrecioPorcentajeDeComision != null && AperturaPrecioPorcentajeDeComision != '') {
        var num = Number(AperturaPrecioPorcentajeDeComision.replace(',', '.'));
        if ($("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value() > num) {
            MensErr("El Porcentaje de Comision no puede ser mayor a " + AperturaPrecioPorcentajeDeComision);
            return false;
        }
        if (descuento.Porcentaje > 1) {
            errores.push("El campo Porcentaje de Comision no puede ser mayor al 1%.");
            return false;
        }
    }
    var total = CalcularPrecioTotalApertura();

    InsertarAperturasViewModel(total);

    if (viewModel.Descuentos.filter(el => el.TipoDBId == 1 && el.TipoPeriodoDBId == 1).length == 1) {
        var descuentos = viewModel.Descuentos.filter(el => el.TipoDBId == 1 && el.TipoPeriodoDBId == 1)[0];
        viewModel.Descuentos.remove(descuentos);
    }
    if ($("#PorcentajeDescuentoAFijarId").val() > 0 || total != 0) {
        var newdescuento = {
            Id: 0,
            TipoPeriodoDBDesc: "Generales",
            TipoPeriodoDBId: 1,
            TipoDBDesc: "Sobre el precio",
            TipoDBId: 1,
            FechaDesde: null,
            FechaHasta: null,
            Importe: total,
            MonedaId: $("#precioMonedaAFijarId").data("kendoDropDownList").value(),
            Porcentaje: $("#PorcentajeDescuentoAFijarId").val(),
            Borrar: function () {
                eliminarDescuento(this);
            }
        };

        viewModel.Descuentos.push(newdescuento);
    }
    DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
    if ($("#epaId").is(":checked") == true && $("#selectSustenTipoDB").data("kendoDropDownList").value() == 1) {
        $("#selectSustenTipoDB").data("kendoDropDownList").trigger("change");
    }
}

function InsertarAperturasViewModel(total) {
    var Financiero = {
        Id: 0,
        ConceptoAperturaPrecioId: 1,
        Importe: $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val()
    };
    var Redespacho = {
        Id: 0,
        ConceptoAperturaPrecioId: 2,
        Importe: $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val()
    };
    var Comisiones = {
        Id: 0,
        ConceptoAperturaPrecioId: 3,
        Importe: $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val(),
        //Porcentaje: Number($("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value())
    };
    var Bonificaciones = {
        Id: 0,
        ConceptoAperturaPrecioId: 4,
        Importe: $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val(),
        //Porcentaje: Number($("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value())
    };

    var Basis = {
        Id: 0,
        ConceptoAperturaPrecioId: 5,
        Importe: $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val(),
        //Porcentaje: Number($("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value())
    };

    viewModel.AperturaPrecio = [];
    viewModel.AperturaPrecio.push(Financiero);
    viewModel.AperturaPrecio.push(Redespacho);
    viewModel.AperturaPrecio.push(Comisiones);
    viewModel.AperturaPrecio.push(Bonificaciones);
    viewModel.AperturaPrecio.push(Basis);
    $('#modalAperturaPrecio').modal('hide');
    //var descuento = {
    //    Id: 0,
    //    TipoPeriodoDBDesc: $("#tipoPeriodoDBId").data("kendoDropDownList").text(),
    //    TipoPeriodoDBId: $("#tipoPeriodoDBId").data("kendoDropDownList").value(),
    //    TipoDBDesc: $("#TipoDBId").data("kendoDropDownList").text(),
    //    TipoDBId: $("#TipoDBId").data("kendoDropDownList").value(),
    //    FechaDesde: $("#fechaDesdeDescuentoId").val(),
    //    FechaHasta: $("#fechaHastaDescuentoId").val(),
    //    Importe: total,
    //    MonedaId: $("#descuentoMonedaId").data("kendoDropDownList").value(),
    //    Porcentaje: $("#PorcentajeDescuentoId").val(),
    //    Borrar: function () {
    //        eliminarDescuento(this);
    //    }
    //};
    //if (total) {
    //    viewModel.Descuentos = viewModel.Descuentos.filter(el => el.TipoDBId != 1 && el.TipoPeriodoDBId != 1);
    //    viewModel.Descuentos.push(descuento);
    //}
    //if (viewModel.AperturaPrecio.some(x => x.ConceptoAperturaPrecioId == 5 && x.Importe)) {
    //    $("#posicionCBOTDiv").show();
    //} else {
    //    $("#posicionCBOTDiv").hide();
    //    $("#posicionCBOTId").val("");
    //    $("#tipoPosicionCBOTId").data("kendoDropDownList").value("");
    //}
    //$("#ImporteDescuentoId").data("kendoNumericTextBox").value(total);
}
function eliminarDescuento(descuento) {
    if (descuento.TipoDBId == 1 && descuento.TipoPeriodoDBId == 1) {
        //LimpiarApertura();
        //$("#ImporteDescuentoId").data("kendoNumericTextBox").value("");
        //$("#posicionCBOTDiv").hide();
        //$("#posicionCBOTId").val("");
        //$("#tipoPosicionCBOTId").data("kendoDropDownList").value("");
        //viewModel.Descuentos.remove(descuento);
    } else {
        viewModel.Descuentos.remove(descuento);
    }

}

function InicializarAperturaDePrecios() {

    $("#aperturaPrecioBtn").click(function () {
        AbrirModalAperturaDePrecio();
        DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
    });

    $("#guardarAperturaPrecio").click(function () {
        GuardarAperturaDePrecio();
    });

    $(".aperturaPrecioInput").change(function () {
        CalcularPrecioTotalApertura();
    });

    $("#aperturaPrecioImporteBasisId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false
    });

    $("#aperturaPrecioImporteFinancieroId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false
    });

    $("#aperturaPrecioImporteRedespachoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false
    });
    $("#aperturaPrecioImporteComisionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#aperturaPrecioPorcentajeComisionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0,
        max: 100
    });

    $("#aperturaPrecioImporteBonificacionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#aperturaPrecioPorcentajeBonificacionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#botonAperturaDePrecioFijacion").hide();
    $(".aperturaPrecioPrecio").hide();
    $("#aperturaPrecioConceptoBonificaciones").show();
    $("#aperturaPrecioConceptoBasis").show();
    $("#aperturaPrecioConceptoComisiones").hide();
    $("#aperturaPrecioConceptoFinanciero").hide();

    $("#aperturaPrecioImporteRedespachoId").change(ConvertirRedespachoANegativo);
    $("#aperturaPrecioImporteBasisId").change(ConvertirBasisANegativo);

    $("#aperturaPrecioTdPrecioNeto").html("Total");

}

function AbrirModalAperturaDePrecio() {
    var moneda = "";

    if (viewModel.AperturaPrecio.length > 0) {
        moneda = viewModel.AperturaPrecio[0].MonedaId;
        $("#precioMonedaAFijarId").data("kendoDropDownList").value(moneda);
        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[0].Importe);
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[1].Importe);
        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[2].Porcentaje);
        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[2].Importe);
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[4].Importe);
    } else {
        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(0);
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(0);
        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
        $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value(0);
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(0);

        if ($("#precioMonedaAFijarId").val()) {
            moneda = $("#precioMonedaAFijarId").data("kendoDropDownList").text();
        } else {
            $("#precioMonedaAFijarId").data("kendoDropDownList").value("ARP  ");
            moneda = $("#precioMonedaAFijarId").data("kendoDropDownList").text();
        }
    }
    $(".aperturaprecioMoneda").text($("#precioMonedaAFijarId").data("kendoDropDownList").text());

    //CalcularMaximoComision();

    $("#precioAperturaOriginal").text(kendo.toString(Number(0), "n2") + " " + moneda);
    DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
    CalcularPrecioTotalApertura();
    $("#modalAperturaPrecio").modal("show");
}

function CalcularPrecioTotalApertura() {
    var bonificacion = Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
    var precioOriginal = 0;

    var porcentajeComision = 0;//Math.min(Number($("#aperturaPrecioPorcentajeComisionesId").val().replace(',', '.')), $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").max());
    var precioTarifaFlete = Number($("#TarifaFleteId").val().replace(',', '.'));
    precioOriginal += Math.min(Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')), Number($("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max()));
    precioOriginal += Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
    precioOriginal += Number($("#aperturaPrecioImporteBasisId").val().replace(',', '.'));
    //CalcularMaximoComision();
    precioOriginal += Number($("#aperturaPrecioImporteBonificacionesId").val().replace(',', '.'));

    porcentajeComision = porcentajeComision / 100;
    precioOriginal += (precioOriginal * porcentajeComision) - precioTarifaFlete;
    precioOriginal += Number($("#aperturaPrecioImporteComisionesId").val().replace(',', '.'));

    $("#totalApertura").text(kendo.toString(precioOriginal, "n2") + " " + ($("#totalApertura").val() ? $("#totalApertura").data("kendoDropDownList").text() : ""));

    return precioOriginal;
}

//function CalcularMaximoComision() {
//    var maximoBonificacion = (Number($("#precioId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'))) * 0.01;
//    var comisionesTextBox = $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox");
//    if (comisionesTextBox) {
//        comisionesTextBox.max(maximoBonificacion);
//        if (Number($("#aperturaPrecioImporteComisionesId").val().replace(',', '.')) > comisionesTextBox.max()) {
//            comisionesTextBox.value(comisionesTextBox.max());
//        }
//    }
//}

//function ModificarComisionesPorImporte() {
//    PonerEnCeroSiEsNulo("aperturaPrecioImporteComisionesId");
//    $("#aperturaPrecioPorcentajeComisionesId").val(0);
//    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
//    CalcularPrecioTotalApertura();
//}
//function ModificarComisionesPorcentaje() {
//    PonerEnCeroSiEsNulo("aperturaPrecioPorcentajeComisionesId");
//    $("#aperturaPrecioImporteComisionesId").val(0);
//    $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(0);
//    CalcularPrecioTotalApertura();
//}
function ConvertirDescuentoANegativo() {
    if ($("#tipoId").val() == 1 && $("#destinoId").val() != 1) {
        var valorAbsoluto = Math.abs($("#ImporteDescuentoId").val().replace(',', '.'));
        $("#ImporteDescuentoId").data("kendoNumericTextBox").value(-1 * valorAbsoluto);
    }
}
function ConvertirRedespachoANegativo() {
    var valor = $("#aperturaPrecioImporteRedespachoId").val().replace(',', '.');
    if (valor > 0) {
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(-1 * valor);
    }
    CalcularPrecioTotalApertura();
}
function ConvertirBasisANegativo() {
    var valor = $("#aperturaPrecioImporteBasisId").val().replace(',', '.');
    if (valor > 0) {
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(-1 * valor);
    }
    CalcularPrecioTotalApertura();
}
function PonerEnCeroSiEsNulo(elemento) {
    if (!$("#" + elemento).val()) {
        $("#" + elemento).val(0);
    }
}

//function SetearValoresMaximosApertura() {
//    $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max(Number($("#precioId").val().replace(',', '.')));
//    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").max(Math.abs(Number($("#precioId").val().replace(',', '.'))));
//    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").min(-1 * Math.abs(Number($("#precioId").val().replace(',', '.'))));
//}

//function InicializarEditarContratoApertura() {
//    SetearValoresMaximosApertura();
//    CalcularPrecioTotalApertura();
//}

//function SetearComisionCorredor() {
//    $("#aperturaPrecioPorcentajeComisionesId").val(0);
//    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
//    InsertarAperturasViewModel(CalcularPrecioTotalApertura());
//}

function ObtenerAlta(proveedorId) {
    altaTemprana = MSExecuteOnServer('/CompraNet/ValidarProveedor', { proveedorId: proveedorId });
}
function ValidarCorredor(Id) {
    var corredor = MSExecuteOnServer('/CompraNet/ValidarProveedor', { proveedorId: Id });
    if (corredor.Ruca.Corredor == "NO") {
        MensInfo("El corredor no está habilitado en Ruca");
    }
}
function ValidarAlta() {
    if (altaTemprana) {

        if ($("#clasificacion").val() == 2 &&
            (($("#planCanjeId").is(':checked') && altaTemprana.Ruca.Acopiador.PlanCanje == "NO") ||
                ($("#consignatarioId").is(':checked') && altaTemprana.Ruca.Acopiador.Consignatario == "NO") ||
                ($("#planCanjeId").is(':not(:checked)') && $("#consignatarioId").is(':not(:checked)') && altaTemprana.Ruca.Acopiador.Directo == "NO"))) {
            MensInfo("No está habilitado en Ruca");
            return;
        }
        if ($("#clasificacion").val() == 3 &&
            (($("#planCanjeId").is(':checked') && altaTemprana.Ruca.Otros.PlanCanje == "NO") ||
                ($("#consignatarioId").is(':checked') && altaTemprana.Ruca.Otros.Consignatario == "NO") ||
                ($("#planCanjeId").is(':not(:checked)') && $("#consignatarioId").is(':not(:checked)') && altaTemprana.Ruca.Otros.Directo == "NO"))) {
            MensInfo("No está habilitado en Ruca");
            return;
        }
        if (altaTemprana.FechaActualizacion == "NO") {
            MensInfo("Falta fecha de actualización de legajo");
            return;
        }
    }
}

function ValidarFason() {
    if (altaTemprana) {
        if ($("#fasonIdCheck").is(':checked') && altaTemprana.Ruca.Fason == "NO") {
            MensInfo("No está habilitado en Ruca");
            return;
        }
    }
}

//function AgregarPrecioPactado() {
//    var precioPactado = {
//        Id: 0,
//        FechaDesde: $("#fechaDesdePactado").val(),
//        FechaHasta: $("#fechaHastaPactado").val(),
//        Precio: Number($("#precioPactado").val().replace(',', '.')),
//        PrecioVisualizar: kendo.toString($("#precioPactado").val().replace(',', '.') ? Number($("#precioPactado").val().replace(',', '.')) : "", "n2"),
//        MonedaPactadoId: $("#monedaPactadoId").data("kendoDropDownList").value(),
//        MonedaPactadoDesc: $("#monedaPactadoId").data("kendoDropDownList").text(),
//        ImportePactado: Number($("#importePactado").val().replace(',', '.')),
//        ImportePactadoVisualizar: kendo.toString($("#importePactado").val().replace(',', '.') ? Number($("#importePactado").val().replace(',', '.')) : "", "n2"),
//        MonedaImportePactadoId: $("#monedaImportePactadoId").data("kendoDropDownList").value(),
//        MonedaImportePactadoDesc: $("#monedaImportePactadoId").data("kendoDropDownList").value() != "" ? $("#monedaImportePactadoId").data("kendoDropDownList").text() : "",
//        Porcentaje: $("#porcentajePactado").val(),
//        Borrar: function () {
//            viewModel.PrecioPactado.remove(this);
//            MostrarTablaPrecioPactado();
//        }
//    };

//    var err = ValidarPrecioPactado(precioPactado);
//    if (ExistsErrorMessages(err)) {
//        MensErr(err[0]);
//    }
//    else {
//        viewModel.PrecioPactado.push(precioPactado);
//        $("#fechaDesdePactado").data("kendoDatePicker").value("");
//        $("#fechaHastaPactado").data("kendoDatePicker").value("");
//        $("#precioPactado").data("kendoNumericTextBox").value("");
//        $("#importePactado").data("kendoNumericTextBox").value("");
//        $("#monedaImportePactadoId").data("kendoDropDownList").value("");
//        $("#porcentajePactado").data("kendoNumericTextBox").value("");
//        MostrarTablaPrecioPactado();
//    }

//    function ValidarPrecioPactado(precioPactado) {
//        var errores = [];
//        if (precioPactado.Precio === "" || precioPactado.Precio === 0) {
//            errores.push("El Precio no debe ser vacio o 0");
//        }
//        if (precioPactado.FechaDesde === "" || precioPactado.FechaDesde === undefined) {
//            errores.push("La Fecha Desde no debe ser vacia");
//        }
//        if (precioPactado.FechaHasta === "" || precioPactado.FechaHasta === undefined) {
//            errores.push("La Fecha Hasta no debe ser vacia");
//        }
//        if (precioPactado.ImportePactado > 0 && (precioPactado.MonedaImportePactadoId === "" || precioPactado.MonedaImportePactadoId === undefined)) {
//            errores.push("La Moneda no debe ser vacia cuando hay Importe");
//        }
//        if ((precioPactado.ImportePactado == 0 || precioPactado.ImportePactado == "" || precioPactado.ImportePactado === undefined) && (precioPactado.MonedaImportePactadoId != "")) {
//            errores.push("El Importe no debe ser vacia cuando seleciono Moneda");
//        }
//        if (kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") > kendo.parseDate(precioPactado.FechaHasta, "dd-MM-yyyy")) {
//            errores.push("La Fecha Desde no debe ser mayor a Fecha Hasta");
//        }
//        if (kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") < kendo.parseDate($("#fechaDesdeId").val(), "dd-MM-yyyy")
//            || kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") > kendo.parseDate($("#fechaHastaId").val(), "dd-MM-yyyy")) {
//            errores.push("La Fecha Desde debe estar dentro de los rangos de entrega");
//        }
//        if (kendo.parseDate(precioPactado.FechaHasta, "dd-MM-yyyy") < kendo.parseDate($("#fechaDesdeId").val(), "dd-MM-yyyy")
//            || kendo.parseDate(precioPactado.FechaHasta, "dd-MM-yyyy") > kendo.parseDate($("#fechaHastaId").val(), "dd-MM-yyyy")) {
//            errores.push("La Fecha Hasta debe estar dentro de los rangos de entrega");
//        }
//        var i = viewModel.PrecioPactado.length - 1;
//        if (i >= 0 && kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") <= kendo.parseDate(viewModel.PrecioPactado[i].FechaHasta, "dd-MM-yyyy")) {
//            errores.push("La Fecha Desde no debe ser menor a la Fecha Hasta anterior");
//        }

//        var rangos = MSExecuteOnServer('/CompraNet/ObtenerRangoDePrecios', { materialId: $("#material").val(), monedaId: $("#precioMonedaId").val() });
//        if (Number($("#precioPactado").val().replace(',', '.')) < rangos.PrecioMinimo) {
//            errores.push("Precio por fuera del rango de Precio Mínimo");
//        }
//        if (Number($("#precioPactado").val().replace(',', '.')) > rangos.PrecioMaximo) {
//            errores.push("Precio por fuera del rango de Precio Máximo");
//        }
//        return errores;
//    }
//}
//function MostrarTablaPrecioPactado() {
//    if (viewModel.PrecioPactado.length > 0) {
//        $(".tabla-pactados").show();
//    } else {
//        $(".tabla-pactados").hide();
//    }
//}

//function FormatearFecha(fecha) {
//    if (fecha != null && fecha != undefined && fecha != "") {
//        var dias = fecha.split('-');
//        var dia = dias[0];
//        var mes = dias[1];
//        var anio = dias[2];
//        if (dia.length < 2) {
//            dia = "0" + dia;
//        }
//        if (mes.length < 2) {
//            mes = "0" + mes;
//        }
//        return dia + '-' + mes + '-' + anio;
//    }
//}

//function CalcularNetoFijacionConDescuentos() {
//    var ImporteSobrePrecioMonedaIgual = ImporteSobrePrecio;
//    if (ImporteSobrePrecio != 0 || PorcentajeSobrePrecio != 0) {
//        if ($.trim(MonedaSobrePrecio) != $.trim($("#precioMonedaId").val())) {
//            var valorDolar = MSExecuteOnServer('/CompraNet/TraerTipoDeCambio', {});
//            console.log("valorDolar", valorDolar);
//            if ($.trim(MonedaSobrePrecio) == "ARP") {
//                ImporteSobrePrecioMonedaIgual = ImporteSobrePrecio / valorDolar;
//            } else {
//                ImporteSobrePrecioMonedaIgual = ImporteSobrePrecio * valorDolar;
//            }
//        }
//        var costoFinanciero = Math.min(Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')), Number($("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max()));
//        var precioN = Number($("#precioId").val().replace(',', '.'));
//        console.log("precio base:", precioN);
//        console.log("costoFinanciero", costoFinanciero);
//        console.log("ImporteSobrePrecio", ImporteSobrePrecio);
//        console.log("ImporteSobrePrecioMonedaIgual", ImporteSobrePrecioMonedaIgual);
//        console.log("PorcentajeSobrePrecio", PorcentajeSobrePrecio);
//        var desc = ((precioN + ImporteSobrePrecioMonedaIgual + costoFinanciero) * PorcentajeSobrePrecio / 100);
//        console.log("descuento", desc);
//        var totalNeto = precioN + ImporteSobrePrecioMonedaIgual + costoFinanciero + desc;
//        console.log("totalNeto", totalNeto);
//        $("#precioTotalApertura").data("kendoNumericTextBox").value(totalNeto);
//        return totalNeto;
//    }
//}
function HayCompensacion() {
    //if (!$("#compensacionId").is(":checked")) {
    //    //if (!$("#pagoDirectoId").is(":checked")) {
    //    //    $("#chequeElectronicoDiv").show();
    //    //}
    //    if ($("#buscadorCorredor").val() == "" && $("#AgenteCompraId").val() == "" && !$("#chequeElectronicoInput").is(":checked")) {
    //        $("#pagoCbuDiv").show();
    //    }

    //}
    //else {
    //    $("#chequeElectronicoInput").prop("checked", false);
    //    $("#chequeElectronicoDiv").hide();
    //    $("#pagoCbuDiv").hide();
    //}
}

function HayChequeElectronicoAmpliar() {
    if (!$("#chequeElectronicoInput").is(":checked") && $("#buscadorCorredor").val() == "" && $("#AgenteCompraId").val() == "") {
        $("#pagoCbuDiv").show();
    } else {
        $("#pagoCbuDiv").hide();
        $("#pagoCbu").val("");
    }
}

function HayChequeElectronicoOtros() {

    if (!$("#chequeElectronico").is(":checked") && $("#buscadorCorredor").val() == "" && $("#AgenteCompraId").val() == "") {
        $("#pagoCbuId").show();
    } else {
        $("#pagoCbuId").hide();
        $("#pagoCbuInput").val("");
    }
}
function FechaFeriado() {
    var fechaFeriado = MSExecuteOnServer('/CompraNet/FechaFeriados');
    var fechas = [];

    for (var i = 0; i < fechaFeriado.length; i++) {
        var src = fechaFeriado[i];
        src = src.replace(/[^0-9 +]/g, '');
        fechas.push(kendo.toString(new Date(parseInt(src)), "dd-MM-yyyy"));
    }
    return fechas;
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function HayCanje() {

    if ($("#canjeId").is(":checked")) {
        ocultarSiHayCanje();
        CargarCalidadPorMaterial($('#material').data("kendoDropDownList").value());
        $("#prestamoDevolucionDiv").hide();
        $("#plantaDestinoId").data("kendoDropDownList").value("");
        $("#plantaDestinoId").data("kendoDropDownList").trigger("change");
        $("#mostrarCanje").show();
        $("#fasonIdCheck").prop("disabled", false);
        $("#madreId").prop("disabled", false);
        $("#TipoDBId").data("kendoDropDownList").value(1);
        $("#TipoDBId").data("kendoDropDownList").enable(false);
        $("#boletoCartaId").prop("checked", false);
        $("#boletoNingunoId").prop("checked", false);
        $("#boletoCartaId").prop("disabled", true);
        $("#boletoNingunoId").prop("disabled", true);
        $("#BolsaCartaDiv").hide();
        $("#bolsaCartaId").data("kendoDropDownList").value("");
        $("#bolsaCartaId").data("kendoDropDownList").trigger("change");
        $("#boletoCartaId").prop("checked", false);
        $("#boletoNingunoId").prop("checked", false);
        $("#boletoCartaId").prop("disabled", true);
        $("#boletoNingunoId").prop("disabled", true);
        //$("#calidadesEspecialesId").data("kendoDropDownList").value(0)
        //$("#calidadesEspecialesId").data("kendoDropDownList").trigger("change");
        //LimpiarCalidades();

    } else {
        CargarCalidadPorMaterial($('#material').data("kendoDropDownList").value());
        $("#mostrarCanje").hide();
        $("#montoId").data("kendoNumericTextBox").value("");
        $("#montoMonedaId").data("kendoDropDownList").value("");
        $("#insumoId").val("");
        $("#pagoDiferidoDiv").show();
        $(".madreDiv").show();
        //if ($("#clasificacion").val() == 1) {
        //    $("#dolarizadoExpressDiv").show();
        //}
        $("#divSojaSustentable").show();
        $("#compensacionDiv").show();
        $("#ordenarRow").hide();
        $("#prestamoDevolucionDiv").show();
        if ($("#AgenteCompraId").val() != 1) {
            $("#boletoCartaId").prop("disabled", false);
        }
        $("#boletoNingunoId").prop("disabled", false);
        $("#TipoDBId").data("kendoDropDownList").enable(true);
        //$("#DatosCalidades").show();
        //$("#calidadesEspecialesId").data("kendoDropDownList").value(0)
        //$("#calidadesEspecialesId").data("kendoDropDownList").trigger("change");
        //LimpiarCalidades();
    }
    ActivarSinBoleto();
}

function ocultarSiHayCanje() {
    //$("#DatosCalidades").hide();
    //$(".datos-calidades").hide();
    $("#pagoDiferidoDiv").hide();
    $("#sustentableId").prop("checked", false);
    $("#epaId").prop("checked", false);
    $("#compensacionDiv").hide();
    $("#compensacionId").prop("checked", false);
    $("#sustentablePrecioId").data("kendoNumericTextBox").value("");
    $("#sustentableMonedaId").data("kendoDropDownList").value("");
    $(".sustentableDiv").hide();
    $(".madreDiv").hide();
    $("#madreId").prop("checked", false);
    $("#fasonIdCheck").prop("checked", false);
    $("#pagosDiv").hide();
    $("#CDId").prop("checked", false);
    $("#WarrantId").prop("checked", false);
    $("#TipoDBId").data("kendoDropDownList").value(1);
    $("#TipoDBId").data("kendoDropDownList").enable(false);
    $("#BolsaCartaDiv").hide();
    $("#bolsaCartaId").data("kendoDropDownList").value("");
    $("#bolsaCartaId").data("kendoDropDownList").trigger("change");
}
function ocultarSiHayPrestamos() {
    $("#mostrarCanje").hide();
    $("#DatosDescuentos").hide();
    $(".datos-descuentos").hide();
    $("#DatosCalidades").hide();
    $(".datos-calidades").hide();
    $("#DatosBoleto").hide();
    $(".datos-boleto").hide();
    $("#selCargoVendedorDiv").hide();
    $("#selCargoVendedorId").prop("checked", false);
    $("#selCargoMOADiv").hide();
    $("#selCargoMOAId").prop("checked", false);
    $("#canjeDiv").hide();
    $("#canjeId").prop("checked", false);
    //$("#noInformarSioDiv").hide();
    //$("#noInformaSioId").prop("checked", false);
    $("#montoId").data("kendoNumericTextBox").value("");
    $("#montoMonedaId").data("kendoDropDownList").value("");
    $("#insumoId").val("");
    $("#plantaDestinoDiv").show();
    $("#porcentajePagoDiv").hide();
    $("#porcentajeDePagoId").data("kendoNumericTextBox").value(97.5);
    LimpiarBoleto();
    $("#boletoNingunoId").prop("checked", true);
    $("#calidadesEspecialesId").data("kendoDropDownList").value(0)
    $("#calidadesEspecialesId").data("kendoDropDownList").trigger("change");
    LimpiarCalidades();
    LimpiarDescuentos();
    $(".contratoAFijar").hide();
    $("#fechaDesdeTopeId").val("");
    $("#fechaHastaTopeId").val("");

    $("#condicion").hide();
    $("#condicionFijacionId").data("kendoDropDownList").value("");
    $("#calidadesEspecialesId").data("kendoDropDownList").trigger("change");

}
function HayPrestamo() {
    if ($("#prestamoDevolucionId").is(":checked")) {
        ocultarSiHayCanje();
        ocultarSiHayPrestamos();


    } else {
        $("#DatosDescuentos").show();
        $("#DatosCalidades").show();
        $("#DatosBoleto").show();
        $("#selCargoVendedorDiv").show();
        $("#selCargoMOADiv").show();
        $("#canjeDiv").show();
        $("#noInformarSioDiv").show();
        $("#pagoDiferidoDiv").show();
        //if ($("#clasificacion").val() == 1) {
        //    $("#dolarizadoExpressDiv").show();
        //}
        $("#divSojaSustentable").show();
        $("#compensacionDiv").show();
        $("#ordenarRow").hide();
        $("#plantaDestinoDiv").hide();
        $("#plantaDestinoId").data("kendoDropDownList").value("");
        $("#plantaDestinoId").data("kendoDropDownList").trigger("change");
        $("#porcentajePagoDiv").hide();
        $("#boletoNingunoId").prop("checked", false);
        $(".madreDiv").show();
        $(".contratoAFijar").show();
        var hoy = new Date();
        var maniana = new Date();
        maniana = new Date(maniana.setMonth(maniana.getMonth() + 1));
        $("#fechaDesdeTopeId").val(formatearFecha(hoy));
        $("#fechaHastaTopeId").val(formatearFecha(maniana));
        $("#condicion").show();
        $("#condicionFijacionId").data("kendoDropDownList").value(7);
        $("#calidadesEspecialesId").data("kendoDropDownList").trigger("change");
    }
    ActivarSinBoleto();
}

function SeleccionAutomaticaBolsa() {
    //$("#LocalidadCrearContrato").trigger("change");
    if ($("#tipoId").val() == "1") {
        var destino = $("#destinoId").val();
        var provincia = $("#ProvinciaId").val();
        var localidadInput = $("#LocalidadCrearContrato").val();
        var bolsa = 0;
        if (destino != "" && provincia != "" && localidadInput != "") {
            bolsa = MSExecuteOnServer('/ConfiguracionBolsa/TraerConfiguracionBolsaConDestinoYProcedencia', { destinoId: destino, provinciaId: provincia });
        }

        if (bolsa != 0 && $("#boletoConfirmaId").is(':checked') && $("#bolsaConfirmaId").val() != bolsa.BolsaId) {
            $("#idBolsa").val(bolsa.BolsaId);
            if ($("#buscadorCorredor").val() != "") {
                $("#nombreBolsa").text(bolsa.Bolsa.Descripcion);
                $("#modalConfirmarBolsa").modal("show");
            } else {
                $("#boletoConfirmaId").prop("checked", true);
                $("#bolsaConfirmaId").data("kendoDropDownList").value($("#idBolsa").val());
                $("#bolsaConfirmaId").data("kendoDropDownList").trigger("change");
            }
        }
    }
}

function ConfirmarBolsaModal() {

    $("#boletoConfirmaId").prop("checked", true);
    $("#BolsaConfirmaDiv").show();
    $("#bolsaConfirmaId").data("kendoDropDownList").value($("#idBolsa").val());
    $("#bolsaConfirmaId").data("kendoDropDownList").trigger("change");
    MensInfo('Se cambio la bolsa a ' + $("#bolsaConfirmaId").data("kendoDropDownList").text());
}

function HayMercaderia() {
    CompletarCantidadDisponibleDeposito();
    if ($("#mercsDepositoId").is(":checked")) {
        MostrarCcPpPendientesAplicar();
        $(".depositoDiv").show();
        $("#cantidadDeposito").data("kendoNumericTextBox").enable(false);
    } else {
        $(".fechaHastaSustentableDiv").hide();
        $("#fechaDesdeSustentableId").data("kendoDatePicker").value("");
        $("#fechaHastaSustentableId").data("kendoDatePicker").value("");
        $(".depositoDiv").hide();
        $("#cantidadDeposito").data("kendoNumericTextBox").value("");
    }
}
function HaySustentable() {
    if ($("#sustentableId").is(":checked")) {
        $(".sojaEpa").hide();
        $(".sustentableDiv").show();
        $(".sustenTipoDB").show();
    } else if ($("#epaId").is(":checked")) {
        $(".sojaSustentable").hide();
        $(".sustentableDiv").show();
        $(".sustenTipoDB").show();
    } else {
        $(".sustentableDiv").hide();
        $("#sustentablePrecioId").data("kendoNumericTextBox").value("");
        $("#sustentablePrecioId").addClass("disabled").prop("disabled", false);
        $("#sustentableMonedaId").data("kendoDropDownList").value("");
        $("#sustentableMonedaId").data("kendoDropDownList").enable(true);
        $(".fechaHastaSustentableDiv").hide();
        $("#fechaDesdeSustentableId").data("kendoDatePicker").value("");
        $("#fechaHastaSustentableId").data("kendoDatePicker").value("");
        $(".sojaSustentable").show();
        $(".sojaEpa").show();
        $(".sustenTipoDB").hide();
        $("#selectSustenTipoDB").data("kendoDropDownList").value("");
        $("#tarifaAConvenirId").prop("checked", false);
        $("#divSustentableSinTarifa").hide();
    }

    CompletarCantidadDisponibleDeposito();
    //var maniana = new Date();
    //if ($("#fechaDesdeId").data("kendoDatePicker").value() != null && $("#fechaDesdeId").data("kendoDatePicker").value() >= maniana || $("#sustentableId").is(":checked")) {
    //    $("#mercsDepositoDiv").hide();
    //} else {
    //    $("#mercsDepositoDiv").show();
    //}
}
function HayTarifaAConvenir() {
    if ($("#tarifaAConvenirId").is(":checked")) {
        $("#sustentablePrecioId").data('kendoNumericTextBox').value("");
        $("#sustentableMonedaId").data("kendoDropDownList").value("USDM ");
        $("#sustentablePrecioId").addClass("disabled").prop("disabled", true);
        $("#sustentableMonedaId").addClass("disabled").prop("disabled", true);

        $(".tarifaAConvenirDiv").addClass("disabled").prop("disabled", true);

        MensAlerta("Se está creando un contrato con tarifa a convenir fuera de precio.");
    } else {
        $(".tarifaAConvenirDiv").removeClass("disabled").prop("disabled", false);
        $("#sustentablePrecioId").removeClass("disabled").prop("disabled", false);
        $("#sustentableMonedaId").removeClass("disabled").prop("disabled", false);
    }
}
function EsconderCalidadSiHaySojaYCalidadEspecial() {
    if (($("#destinoId").data("kendoDropDownList").value() == "13" || $("#destinoId").data("kendoDropDownList").value() == "6" ||
        $("#destinoId").data("kendoDropDownList").value() == "7") && $('#material').data("kendoDropDownList").value() == "3") {
        CargarCalidadPorMaterial($('#material').data("kendoDropDownList").value());
    } else {
        CargarCalidadPorMaterial($('#material').data("kendoDropDownList").value());
    }

}

function DeshabilitarDescuentoSobrePrecioCuandoTieneAgente() {
    if ($("#TipoDBId").data("kendoDropDownList").value() == 1 && $("#AgenteCompraId").val() != "") {
        $("#ImporteDescuentoId").prop('disabled', true);
        $("#PorcentajeDescuentoId").prop('disabled', true);
        $("#descuentoMonedaId").data("kendoDropDownList").enable(false);
        LimpiarDescuentosConAgenteDeCompra();
    } else {
        $("#ImporteDescuentoId").prop('disabled', false);
        $("#PorcentajeDescuentoId").prop('disabled', false);
        $("#descuentoMonedaId").data("kendoDropDownList").enable(true);
    }

    if ($("#AgenteCompraId").val() != "") {
        $("#PorcentajeDescuentoAFijarId").val(0);
        $("#PorcentajeDescuentoAFijarId").prop('disabled', true);
        $("#PorcentajeDescuentoAFijarId").css("background-color", "lightgray");
        LimpiarDescuentosConAgenteDeCompra();
        ActualizarAperturas();
    } else {
        $("#PorcentajeDescuentoAFijarId").css("background-color", "white");
        $("#PorcentajeDescuentoAFijarId").prop('disabled', false);
    }
    if (($("#material").val() === "4" || $("#material").val() === "5")) {
        $("#PorcentajeDescuentoAFijarId").val(0);
        $("#PorcentajeDescuentoAFijarId").prop('disabled', true);
        $("#PorcentajeDescuentoAFijarId").css("background-color", "lightgray");
    } else {
        $("#PorcentajeDescuentoAFijarId").prop('disabled', false);
        $("#PorcentajeDescuentoAFijarId").css("background-color", "white");
    }

}

function LimpiarDescuentosConAgenteDeCompra() {
    var iteracionesDescuentos = viewModel.Descuentos.length;
    if (iteracionesDescuentos > 0) {
        for (var i = 0; i < iteracionesDescuentos; i++) {
            if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 &&
                viewModel.Descuentos[i].TipoDBId == 1) {
                viewModel.Descuentos.remove(viewModel.Descuentos[i]);
            }
        }
    }
}
function LimpiarDatos() {
    DeshabilitarDescuentoSobrePrecioCuandoTieneAgente();
}

function DeshabilitarConPase() {
    if ($("#tipoPosicionCBOTId").data("kendoDropDownList").value() == '3') {
        if ($("#AgenteCompraId").data("kendoDropDownList").value() > 0) {
            MensAlerta("El campo Agente de Compra se deshabilitó automaticamente. Agente de Compra es excluyente con A Fijar Pase");
        }
        $("#AgenteCompraId").data("kendoDropDownList").enable(false);
        $("#AgenteCompraId").data("kendoDropDownList").value('');
        $("#AgenteCompraId").data("kendoDropDownList").trigger("change");
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(0)
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").wrapper.find("input").css("background-color", "lightgray");
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").readonly(true);
        ActualizarAperturas();
        if ($("#estado").val() == "5" || $("#estado").val() == "11") {
            $("#fechaDesdeTopeId").data("kendoDatePicker").enable(false);
            $("#fechaHastaTopeId").data("kendoDatePicker").enable(false);
            $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").wrapper.find("input").css("background-color", "lightgray");
            $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").readonly(true);
            $("#eliminar-descuento").hide();
            $(".descuentos").hide();
        }
    } else {
        $("#AgenteCompraId").data("kendoDropDownList").enable(true);
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").wrapper.find("input").css("background-color", "white");
        $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").readonly(false);
    }
}

function ActualizarAperturas() {
    var Financiero = {
        Id: 0,
        ConceptoAperturaPrecioId: 1,
        Importe: $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val()
    };
    var Redespacho = {
        Id: 0,
        ConceptoAperturaPrecioId: 2,
        Importe: $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val()
    };
    var Comisiones = {
        Id: 0,
        ConceptoAperturaPrecioId: 3,
        Importe: $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val(),
        //Porcentaje: Number($("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value())
    };
    var Bonificaciones = {
        Id: 0,
        ConceptoAperturaPrecioId: 4,
        Importe: $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val(),
        //Porcentaje: Number($("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value())
    };

    var Basis = {
        Id: 0,
        ConceptoAperturaPrecioId: 5,
        Importe: $("#aperturaPrecioImporteBasisId").data("kendoNumericTextBox").value(),
        MonedaId: $("#precioMonedaAFijarId").val(),
        //Porcentaje: Number($("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value())
    };

    viewModel.AperturaPrecio = [];
    viewModel.AperturaPrecio.push(Financiero);
    viewModel.AperturaPrecio.push(Redespacho);
    viewModel.AperturaPrecio.push(Comisiones);
    viewModel.AperturaPrecio.push(Bonificaciones);
    viewModel.AperturaPrecio.push(Basis);
}

function ConfirmarBonificacion() {
    $("#esBonificacion").prop("checked", true);
}

function RechazarBonificacion() {
    $("#esBonificacion").prop("checked", false);
    var error = false;
    BlockUi('Guardando...');
    var objeto = ObtenerDatos(error);
    if (!error) {
        var result = MSExecuteOnServer('/CompraNet/GrabarContrato', objeto);
        if (result != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr(result.Errores[0].Message);
                $.unblockUI();
            }
            else {
                if (Siguientes != undefined && Siguientes != null && Siguientes != "" && Siguientes != "[]") {
                    var siguientesObj = JSON.parse(Siguientes.replace(/(&quot\;)/g, "\""));
                    var primero = siguientesObj.shift();
                    editarContrato(primero.Id, primero.TipoNegocioId, siguientesObj);
                } else {
                    window.location.href = window.location.origin + "/CompraNet";
                }
            }
        }
        $.unblockUI();
    } else {
        $.unblockUI();
    }
}

function ValidarProveedorSisa() {
    var mensaje = MSExecuteOnServer("/Compranet/ValidarProveedorSISA", {
        proveedorId: $("#proveedorId").val(),
        clasificacion: $("#clasificacion").val() != "" ? $("#clasificacion").val() : "0",
        planCanje: $("#ventaId").is(":checked") != true ? $("#planCanjeId").is(':checked') : false,
        consignatario: $("#ventaId").is(":checked") != true ? $("#consignatarioId").is(':checked') : false
    });
    if (mensaje != "" && mensaje != null) {
        MensErr(mensaje);
        $("#mensaje").show();
        $("#mensaje").text(mensaje);
    } else {
        $("#mensaje").hide();
        $("#mensaje").val("");
    }
}

function ValidarComisionEnCentro() {
    var centro = MSExecuteOnServer("/Compranet/ValidarComisionEnCentro", {
        Id: $("#destinoId").val()
    });

    if (centro && centro.Comision && centro.Comision == 1) {
        return true;
    }
    return false;
}

function BorrarComisionSiEsAcopio() {
    $("#PorcentajeDescuentoAFijarId").val(0);
    //$("#porcentajeComision").data("kendoNumericTextBox").value(0);
    ActualizarAperturas();
    var total = CalcularPrecioTotalApertura();
    InsertarAperturasViewModel(total);
    if ($("#material").val() === "4" || $("#material").val() === "5") {
        for (var i = 0; i < viewModel.Descuentos.length; i++) {
            if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 && viewModel.Descuentos[i].TipoDBId == 2) {
                viewModel.Descuentos.remove(viewModel.Descuentos[i]);
            }
        }
    } else {

        for (var i = 0; i < viewModel.Descuentos.length; i++) {
            if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 && viewModel.Descuentos[i].TipoDBId == 1) {
                viewModel.Descuentos.remove(viewModel.Descuentos[i]);
            }
        }
    }
}

function CargarAutomaticamenteLaComision(compraNet) {
    if (($("#material").val() === "4" || $("#material").val() === "5")) {
        if (compraNet.ComisionPorcentaje != null && compraNet.ComisionPorcentaje > 0 && !$("#buscadorCorredor").val()) {
            $("#tipoPeriodoDBId").data("kendoDropDownList").value("1");
            $("#TipoDBId").data("kendoDropDownList").value("2");
            $("#PorcentajeDescuentoId").val(compraNet.ComisionPorcentaje);
            AgregarDescuentos();
        } else {
            for (var i = 0; i < viewModel.Descuentos.length; i++) {
                if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 && viewModel.Descuentos[i].TipoDBId == 2) {
                    viewModel.Descuentos.remove(viewModel.Descuentos[i]);
                }
            }
        }
        $("#PorcentajeDescuentoAFijarId").val(0);
        ActualizarAperturas();
        var total = CalcularPrecioTotalApertura();
        InsertarAperturasViewModel(total);
        for (var i = 0; i < viewModel.Descuentos.length; i++) {
            if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 && viewModel.Descuentos[i].TipoDBId == 1) {
                viewModel.Descuentos.remove(viewModel.Descuentos[i]);
            }
        }
        if (total != 0) {
            var newdescuento = {
                Id: 0,
                TipoPeriodoDBDesc: "Generales",
                TipoPeriodoDBId: 1,
                TipoDBDesc: "Sobre el precio",
                TipoDBId: 1,
                FechaDesde: null,
                FechaHasta: null,
                Importe: total,
                MonedaId: $("#precioMonedaAFijarId").data("kendoDropDownList").value(),
                Porcentaje: $("#PorcentajeDescuentoAFijarId").val(),
                Borrar: function () {
                    eliminarDescuento(this);
                }
            };
            viewModel.Descuentos.push(newdescuento);
        }
    } else {
        $("#PorcentajeDescuentoAFijarId").val(compraNet.ComisionPorcentaje && !$("#buscadorCorredor").val() ? Number(compraNet.ComisionPorcentaje) : 0);
        ActualizarAperturas();
        var total = CalcularPrecioTotalApertura();
        InsertarAperturasViewModel(total);
        for (var i = 0; i < viewModel.Descuentos.length; i++) {
            if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 && viewModel.Descuentos[i].TipoDBId == 1) {
                viewModel.Descuentos.remove(viewModel.Descuentos[i]);
            }
        }
        if (compraNet.ComisionPorcentaje != null && compraNet.ComisionPorcentaje > 0 && !$("#buscadorCorredor").val()) {
            var newdescuento = {
                Id: 0,
                TipoPeriodoDBDesc: "Generales",
                TipoPeriodoDBId: 1,
                TipoDBDesc: "Sobre el precio",
                TipoDBId: 1,
                FechaDesde: null,
                FechaHasta: null,
                Importe: total,
                MonedaId: $("#precioMonedaAFijarId").data("kendoDropDownList").value(),
                Porcentaje: $("#PorcentajeDescuentoAFijarId").val(),
                Borrar: function () {
                    eliminarDescuento(this);
                }
            };
            viewModel.Descuentos.push(newdescuento);
        } else {
            if (total != 0) {
                var newdescuento = {
                    Id: 0,
                    TipoPeriodoDBDesc: "Generales",
                    TipoPeriodoDBId: 1,
                    TipoDBDesc: "Sobre el precio",
                    TipoDBId: 1,
                    FechaDesde: null,
                    FechaHasta: null,
                    Importe: total,
                    MonedaId: $("#precioMonedaAFijarId").data("kendoDropDownList").value(),
                    Porcentaje: $("#PorcentajeDescuentoAFijarId").val(),
                    Borrar: function () {
                        eliminarDescuento(this);
                    }
                };
                viewModel.Descuentos.push(newdescuento);
            }
        }

        for (var i = 0; i < viewModel.Descuentos.length; i++) {
            if (viewModel.Descuentos[i].TipoPeriodoDBId == 1 && viewModel.Descuentos[i].TipoDBId == 2) {
                viewModel.Descuentos.remove(viewModel.Descuentos[i]);
            }
        }
    }
    if (!ValidarComisionEnCentro()) {
        BorrarComisionSiEsAcopio();
    }
}

function CalcularMaximo() {
    var cantidad = $("#cantidadId").data("kendoNumericTextBox").value();
    if (cantidad > 0) {
        var cantidadMaxima = 30 * cantidad / 100;
        var cantidadMinima = 30000;
        if (cantidad < 30000) {
            cantidadMaxima = cantidad;
            cantidadMinima = cantidad;
        } else if (cantidadMaxima < 30000) {
            cantidadMaxima = 30000;
        }
        $("#maximaId").data("kendoNumericTextBox").value(cantidadMaxima);
        $("#minimoId").data("kendoNumericTextBox").value(cantidadMinima);
    } else {
        $("#maximaId").val("");
    }
}

function ObtenerDatosProveedor() {
    var cuitAux = $("#buscadorProveedor").val().split('(');
    if (cuitAux[0] != "") {
        var cuit = cuitAux[1].split(')');
        var proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0], corredor: false });
        return compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: proveedorId });
    }
    return null;
}

function EsComisionista(compranet) {
    var datos = compranet != null ? compranet : ObtenerDatosProveedor();
    if (datos != null && $("#comisionistaCheckId").is(":checked") && datos.RazonSocialComisionista != "") {
        $("#ocultarComisionista").show();
        $("#comisionistaId").val(datos.ComisionistaId);
        $("#razonSocialComisionista").val(datos.RazonSocialComisionista);
    } else {
        $("#ocultarComisionista").hide();
        $("#comisionistaCheckId").prop("checked", false);
        $("#comisionistaId").val("");
    }
}

function ValidarSinBoleto() {
    DeshabilitarCampoSiEsBoleto();
    if ($("#sinBoletoId").is(':checked')) {
        $("#mercsDepositoId").prop("checked", true)
        $("#mercsDepositoId").attr("disabled", true);
        $("#mercsDepositoDiv").show();
        if (Id == 0 || Id == null || Id == "") {
            $("#fechaDesdeId").val(ObtenerFechaDesde());
            $("#fechaHastaId").val(ObtenerFechaDesde());
        }
        if (boletoId != 5 && $("#sinBoletoId").is(':checked')) {
            $("#fechaDesdeId").val(ObtenerFechaDesde());
            $("#fechaHastaId").val(ObtenerFechaDesde());
        }
        $("#fechaDesdeId").data('kendoDatePicker').enable(false);
        $("#fechaHastaId").data('kendoDatePicker').enable(false);
    } else {
        $("#mercsDepositoId").attr("disabled", false);
        $("#fechaDesdeId").data('kendoDatePicker').enable(true);
        $("#fechaHastaId").data('kendoDatePicker').enable(true);
        DesbloquearCamposExpluyentesSinBoleto();
    }
    HayMercaderia();
}

function DeshabilitarCampoSiEsBoleto() {
    if ($("#sinBoletoId").is(':checked')) {
        if ($("#estado").val() == '5' || $("#estado").val() == '7') {
            $("#fechaDesdeId").data('kendoDatePicker').enable(false);
            $("#fechaHastaId").data('kendoDatePicker').enable(false);
            $("#mercsDepositoId").attr("disabled", true);
            $("#cantidadId").data("kendoNumericTextBox").enable(false);
            $("#destinoId").data("kendoDropDownList").enable(false);
            $("#clasificacion").data("kendoDropDownList").enable(false);
            $("#LocalidadCrearContrato").data("kendoAutoComplete").enable(false);
            $("#fechaHastaId").data('kendoDatePicker').enable(false);
            if (boletoId == 5) {
                $("#sinBoletoId").attr("disabled", true);
            }
            $("#boletoConfirmaId").attr("disabled", true);
            $("#boletoFisicoId").attr("disabled", true);
            $("#boletoCartaId").attr("disabled", true);
            $("#boletoNingunoId").attr("disabled", true);
        }
    } else {
        $("#fechaDesdeId").data('kendoDatePicker').enable(true);
        $("#fechaHastaId").data('kendoDatePicker').enable(true);
        $("#mercsDepositoId").attr("disabled", false);
        $("#cantidadId").data("kendoNumericTextBox").enable(true);
        $("#destinoId").data("kendoDropDownList").enable(true);
        $("#clasificacion").data("kendoDropDownList").enable(true);
        $("#LocalidadCrearContrato").data("kendoAutoComplete").enable(true);
        $("#fechaHastaId").data('kendoDatePicker').enable(true);
    }
}

function LimpiarServicios() {
    var iteracionesServicios = viewModel.Servicios.length;
    for (var i = 0; i < iteracionesServicios; i++) {
        viewModel.Servicios.pop();
    }
}

function TraerServicio() {

    var servicio = {
        MaterialId: $("#material").val(),
        CentroId: $("#destinoId").val(),
    };
    var url = '/Compranet/TraerServicios';
    var data = servicio;
    var result = MSExecuteOnServer(url, data);
    $.unblockUI();
    ArmarDescripcionServicio(result);
    viewModel.set("Servicios", result);
    kendo.bind($("#ModalServicio"), viewModel);
    InicializarServicios();
}

function InicializarServicios() {
    for (var i = 0; i < viewModel.Servicios.length; i++) {
        $("#" + viewModel.Servicios[i].ServicioValorId).kendoNumericTextBox({
            culture: "es-AR",
            format: "n2",
            spinners: false,
            min: 0
        });
    }
}

function ArmarDescripcionServicio(servicio) {
    for (var i = 0; i < servicio.length; i++) {
        //servicio[i].Importe = kendo.toString(servicio[i].Importe, "n2");
        servicio[i].DescripcionServicio = servicio[i].Descripcion +
            ((servicio[i].Desde >= 0 && servicio[i].Hasta > 0) ? (" DE " + kendo.toString(servicio[i].Desde, "n2") + " A " + kendo.toString(servicio[i].Hasta, "n2") + "%") :
                (servicio[i].Desde > 0 && servicio[i].Hasta == 0) ? (" MAS DE " + kendo.toString(servicio[i].Desde, "n2") + "%") :
                    (servicio[i].Desde == 0 && servicio[i].Hasta == 0) ? "" : "");
    }
}
function MostrarServicios() {
    $("#ModalServicio").modal("show");
}

function GuardarServicio() {
    BlockUi('Guardando...');
    for (var i = 0; i < viewModel.Servicios.length; i++) {
        viewModel.Servicios[i].Importe = $("#" + viewModel.Servicios[i].ServicioValorId).data("kendoNumericTextBox").value();
    }
    $("#ModalServicio").modal("hide");
    $.unblockUI();
}

function CancelarServicio() {
    kendo.bind($("#ModalServicio"), viewModel);
}

function ActivarBoletoXAgentedeCompras(agenteCompraId) {
    if (agenteCompraId == 1) {
        $("#boletoNingunoId").prop("checked", false);
        $("#boletoNingunoId").click();
        $("#boletoNingunoId").prop("checked", true);
        //$("#boletoNingunoId").attr("readonly", "readonly");
        $("#boletoConfirmaId").attr("disabled", true);
        $("#boletoFisicoId").attr("disabled", true);
        $("#boletoCartaId").attr("disabled", true);
    } else {
        DatosProveedor();
        //$("#boletoNingunoId").prop("checked", false);
        $("#boletoNingunoId").removeAttr("readonly");
        $("#boletoConfirmaId").removeAttr("disabled");
        $("#boletoFisicoId").removeAttr("disabled");
        $("#boletoCartaId").removeAttr("disabled");
    }
}

function ActivarSinBoleto() {
    var activar = true;
    //if ($("#tipoPosicionCBOTId").data("kendoDropDownList").value() == '3') { //&& $("#posicionCBOTId").val() != '') {
    //    activar = false;
    //}
    if ($("#canjeId").is(":checked")) {
        activar = false;
    }
    if ($("#prestamoDevolucionId").is(":checked")) {
        activar = false;
    }
    if (activar) {
        $("#sinBoletoId").prop("disabled", false);
    } else {
        $("#sinBoletoId").prop("checked", false);
        $("#sinBoletoId").prop("disabled", true);
    }
}

function DesbloquearCamposExpluyentesSinBoleto() {
    $("#canjeId").prop("disabled", false);
    $("#prestamoDevolucionId").prop("disabled", false);
    //$("#posicionCBOTId").prop("disabled", false);
    //var tipoPosicionCBOT = $("#tipoPosicionCBOTId").data("kendoDropDownList");
    //tipoPosicionCBOT.enable(true);
}

function MostrarServiciosYCalidades() {
    var validar = $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado 2" ||
        $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Grado" ||
        $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Especial" ||
        $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Materia Extraña" ||
        $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Granos verdes" ||
        $("#calidadesEspecialesId").data("kendoDropDownList").text() === "Dañados";
    //($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Camara" && ($('#material').val() == 4 || $('#material').val() == 5));

    if (validar == false) {
        $("#servicioBtn").hide();
        LimpiarServicios();
    } else {
        $("#servicioBtn").show();
        LimpiarServicios();
        TraerServicio();
    }
    return validar;
}

function CompletarCantidadDisponibleDeposito() {
    if ($("#mercsDepositoId").is(":checked")) {
        var cuitProv = $("#buscadorProveedor").val().split('(');
        if (cuitProv[1] != null) {
            var cuitP = cuitProv[1].split(')');
        }
        else {
            cuitP = cuitProv;
        }
        var cuitCorr = $("#buscadorCorredor").val().split('(');
        if (cuitCorr[1] != null) {
            var cuitC = cuitCorr[1].split(')');
        }
        else {
            cuitC = cuitCorr;
        }
        Id = Id != "" ? Id : 0;
        var datos = { materialId: $('#material').data("kendoDropDownList").value(), id: Id, centro: $("#destinoId").data("kendoDropDownList").value(), corredorId: $("#corredorId").val(), proveedorId: $("#proveedorId").val(), tieneSustentable: $("#sustentableId").is(":checked") || $("#epaId").is(":checked"), tieneBoleto: $("#sinBoletoId").is(':checked') };
        var disponible = MSExecuteOnServer('/CompraNet/ObtenerDatosMercaderiaEnDeposito', datos);
        if (disponible != null && disponible.CantidadDisponible != null) {
            $("#cantidadDeposito").data("kendoNumericTextBox").value(disponible.CantidadDisponible);
            $("#cantidadTotal").text(disponible.CantidadDisponible.toLocaleString("es-AR", { minimumFractionDigits: 0 }) + " (Kg)");
            $("#cantidadDepositoOriginal").val(disponible.CantidadDisponible);
            ValidarCantidad();
        }
    }
}

function ValidarCantidad() {
    var cantidad = Number($("#cantidadId").val());
    var cantidadOriginal = Number($("#cantidadDepositoOriginal").val());
    if (cantidadOriginal > 0) {
        if (cantidad > 0 && cantidad < $("#cantidadDepositoOriginal").val()) {
            $("#cantidadDeposito").data("kendoNumericTextBox").value($("#cantidadId").val())
        }
        if (cantidad > cantidadOriginal) {
            $("#cantidadDeposito").data("kendoNumericTextBox").value($("#cantidadDepositoOriginal").val())
        }
        if (cantidad <= 0) {
            $("#cantidadDeposito").data("kendoNumericTextBox").value($("#cantidadDepositoOriginal").val());
        }
    }
}

function EPATipoDB() {
    if ($("#selectSustenTipoDB").val() == 1) { //sobre precio
        var monedaContrato = $("#precioMonedaAFijarId").val();
        if (monedaContrato != $("#sustentableMonedaId").data("kendoDropDownList").value()) {
            $("#sustentablePrecioId").data('kendoNumericTextBox').value("");
        }
        $("#sustentableMonedaId").data("kendoDropDownList").enable(false);
        $("#sustentableMonedaId").data("kendoDropDownList").value(monedaContrato);
        $("#divSustentableSinTarifa").hide();
        $("#tarifaAConvenirId").prop("checked", false);
        MostrarCcPpPendientesAplicar();
    } else if ($("#selectSustenTipoDB").val() == 2) { //fuera de precio
        $("#sustentableMonedaId").data("kendoDropDownList").enable(false);
        $("#sustentableMonedaId").data("kendoDropDownList").value("USDM ");
        $("#divSustentableSinTarifa").show();
        MostrarCcPpPendientesAplicar();
    } else {
        $("#sustentableMonedaId").data("kendoDropDownList").enable(true);
        $("#divSustentableSinTarifa").hide();
        $("#tarifaAConvenirId").prop("checked", false);
        MostrarCcPpPendientesAplicar();
    }
}