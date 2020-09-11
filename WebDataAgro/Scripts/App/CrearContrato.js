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
$(document).ready(function () {
    $('#menuproveedor').hide();
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });
    CrearViewModel();
    InicializarElementos();
    InicializarDatos();
    AutocompleteProcedencia();

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
    $("#precioId").data("kendoNumericTextBox").value("");
    $("#precioTotalApertura").data("kendoNumericTextBox").value("");
    $("#precioMonedaId").data("kendoDropDownList").enable(false);
    $("#precioMonedaId").data("kendoDropDownList").value("");
    $("div.col-xs-12.col-sm-5.col-md-3.required-box-parent > span.k-widget.k-dropdown.k-header > span").addClass('no-border');
}

function RemoverFondosGrises() {
    $(".gris").removeClass("no-border").prop('disabled', false);
    $("#precioMonedaId").data("kendoDropDownList").enable(true);
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

function InicializarElementos() {
    kendo.culture("es-AR");
    $('[data-toggle="popover"]').popover();
    $(".datos-adicionales").hide();
    $(".datos-boleto").hide();
    $(".datos-establecimiento").hide();
    $(".datos-topesplazos").hide();
    $(".datos-calidades").hide();
    $(".datos-descuentos").hide();
    $(".ocultar").hide();
    $("#buscadorProveedor").click(function () {
        SetearComisionCorredor();
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
    });


    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#buscadorProveedor").val().split('|').length > 1) {
                $("#buscadorProveedor").val($("#buscadorProveedor").val().split('|')[1]);
            }
            $("#contratoId").val("");
            $(".datoscontrato").hide();
            $("#datosContrato").hide();
            $("#pagoCbuInput").val("");
            $("#pagoCbu").val("");
            InicializarBordesRojos();
        },
        select: function (e) {
            ObtenerAlta(e.dataItem.Id);
            ValidarFason();
            if ($("#estado").val() !== "5") {
                if ($("#tipoId").val() != 3 && (Id == 0 || Id == null || Id == "")) {
                    $("#clasificacion").data("kendoDropDownList").value("");
                    $("#consignatarioId").prop("checked", false);
                }
                var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: e.dataItem.Id });
                $("#clasificacion").data("kendoDropDownList").value(compraNet.ClasificacionCompraNetId);
                if ($("#clasificacion").val() == 2 || $("#clasificacion").val() == 3) {
                    $("#consignatarioId").prop("checked", compraNet.Consignatario);
                    $("#planCanjeId").prop("checked", compraNet.PlanCanje);
                }
                $("#clasificacion").data("kendoDropDownList").trigger("change");
                if (compraNet.LocalidadId != null) {
                    if (compraNet.LocalidadId != "" && compraNet.ProvinciaId != "") {
                        $("#LocalidadCrearContrato").val(compraNet.Localidad + " (" + compraNet.Provincia + ")");
                        HabilitarEstablecimiento();
                    } else {
                        $("#LocalidadCrearContrato").val("");
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
                    }
                }
                if ($("#tipoId").val() == 3) {
                    compraNet.ComisionPorcentaje = 0;
                }
                $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(compraNet.ComisionPorcentaje && !$("#buscadorCorredor").val() ? Number(compraNet.ComisionPorcentaje) : 0);
                InsertarAperturasViewModel(CalcularPrecioTotalApertura());
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
                    return { filtro: cuit[0], filtroProveedor: $('#buscadorProveedor').val(), corredor: 0 };
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
        SetearComisionCorredor();
        $("#buscadorCorredor").data("kendoAutoComplete").value("");
        $("#buscadorCorredor").data("kendoAutoComplete").trigger("change");
    });
    $("#buscadorCorredor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
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
                if (!$("#dolarizadoId").is(":checked") && $("#precioMonedaId").val() == "USDM " && $("#fechaCiertaId").val() == "") {
                    $("#dolarizadoExpressDiv").show();       
                    $("#dolarizadoExpressId").prop("disabled", false);              

                }
                //if (!$("#dolarizadoExpressId").is(":checked")) {
                //    $("#dolarizadoId").prop("disabled", false);           
                //    $("#dolarizadoDiv").hide();
                //    $("#dolarizadoFechaId").val("");
                //}
                if ($("#AgenteCompraId").val() == "" && !$("#chequeElectronicoInput").is(":checked")) {
                    $("#pagoDirectoDiv").hide();
                    $("#pagoCbuDiv").show();
                    if ($("#tipoId").val() == "6" || $("#tipoId").val() == "3") {
                        $("#pagoCbuId").show();
                        $("#pagoCbuDiv").hide();
                    }
                }
            }else {         
                $("#dolarizadoExpressId").prop("checked", false);  
                $("#dolarizadoExpressId").prop("disabled", true);              
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
                $('#porcentajeComisionDiv').show();
                $('#contCorredorDiv').show();
                if ($("#tipoId").val() != "3" && $("#tipoId").val() != "6") {
                    $('#pagoDirectoDiv').show();
                }
            }
            ValidarCorredor(e.dataItem.Id);
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
                    return { filtro: $('#buscadorCorredor').val(), corredor: 1 };
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
            $(".datoscontrato").show();
            $("#datosContrato").show();
            $("#kgspendientescontrato").text(e.dataItem.KilosPendiente);
            $("#kgsaplicadoscontrato").text(e.dataItem.KilosAplicados);
            $("#desdecontrato").text(e.dataItem.FechaDesde);
            $("#hastacontrato").text(e.dataItem.FechaHasta);

            $("#posicionFasonId").val(e.dataItem.Posicion);
            $("#fechaOperacionId").val(e.dataItem.FechaOperacion);
            $("#motivoOperacionAnteriorId").val(e.dataItem.MotivoOperacionAnterior);
            $("#fechaDesdeId").val(e.dataItem.DesdeEntrega);
            $("#fechaHastaId").val(e.dataItem.HastaEntrega);
            $("#campanaId").data("kendoDropDownList").text(e.dataItem.Campana);
            $("#destinoId").data("kendoDropDownList").value(e.dataItem.Centro);
            e.dataItem.Calidad === true ? $("#trigoEspecialFijacion").prop("checked", true) : $("#trigoEspecialFijacion").prop("checked", false);
            e.dataItem.PagoDiferido === true ? $("#pesificadoId").prop("checked", true) : $("#pesificadoId").prop("checked", false);

            if (e.dataItem.ImporteAPrecio == 0) {
                $("#impo-a-precio").hide();
            } else {
                $("#impo-a-precio").show();
            }
            $("#importe-a-precio").text(e.dataItem.ImporteAPrecio + " " + e.dataItem.MonedaAPrecio + " ");
            if (e.dataItem.ImporteSobrePrecio == 0) {
                $("#impo-sobre-precio").hide();
            } else {
                $("#impo-sobre-precio").show();
            }
            $("#importe-sobre-precio").text(e.dataItem.ImporteSobrePrecio + " " + e.dataItem.MonedaSobrePrecio + " ");
            if (e.dataItem.PorcentajeAPrecio == 0) {
                $("#porcenteaje-a-precio").hide();
            } else {
                $("#porcenteaje-a-precio").show();
            }
            $("#porc-a-precio").text(e.dataItem.PorcentajeAPrecio + " ");
            if (e.dataItem.PorcentajeSobrePrecio == 0) {
                $("#porcenteaje-sobre-precio").hide();
            } else {
                $("#porcenteaje-sobre-precio").show();
            }
            $("#porc-sobre-precio").text(e.dataItem.PorcentajeSobrePrecio);
            $("#cond-fijacion").text(e.dataItem.CondicionFijacionDescripcion);
            $("#cond-pago").text(e.dataItem.CondicionPagoDescripcion);

            //
            if (e.dataItem.ImporteSobrePrecio != 0 || e.dataItem.PorcentajeSobrePrecio != 0) {
                ImporteSobrePrecio = e.dataItem.ImporteSobrePrecio;
                MonedaSobrePrecio = e.dataItem.MonedaSobrePrecio;
                PorcentajeSobrePrecio = e.dataItem.PorcentajeSobrePrecio;
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
            $(".fechasAFijar").hide();
            $(".contratoAFijar").hide();
            $(".contratoAPrecio").hide();
            $("#pagosDiv").hide();
            $("#fechaCiertaDiv").hide();
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
            $("#ocultarAperturaBtn").hide();
            $("#pagoDiferidoFijacionDiv").removeClass("inline-fijacion");
            $("#diasDiferidoFijacionDiv").removeClass("inline-fijacion");
            $("#pagoDiferidoFijacionDiv").addClass("hide-fijacion");
            $("#diasDiferidoFijacionDiv").addClass("hide-fijacion");
            $("#diasDiferidoId").prop("checked", false);
            $("#ocultarAperturaMoneda").removeClass("w70");
            $("#ocultarAperturaMoneda").addClass("w100");
            $("#corredorDiv").show();
            $("#cargarCantidadPendienteFijar").hide();
            $("#chequeElectronicoDiv").hide();
            $("#chequeElectronicoId").hide();
            $("#pagoCbuId").hide();
            $("#pagoCbuDiv").hide();
            $("#dolarizadoExpressDiv").hide();
            $("#baseDiv").hide();
            $("#pizarraDiv").hide();
            ImporteSobrePrecio = 0;
            MonedaSobrePrecio = "";
            PorcentajeSobrePrecio = 0;
            if (this.value() == 3) {

                $("#fechasDiv").hide();
                $("#fechaDesdeDiv").hide();
                $("#fechaHastaDiv").hide();
                $("#fechaCiertaDiv").hide();
                $("#campanaDiv").hide();
                $("#procedenciaDiv").hide();
                $("#clasificacionDiv").hide();
                $("#destinoDiv").hide();
                $("#CantidadCamionesDiv").hide();
                $("#planCanjeConsignatarioIdDiv").hide();
                $("#DatosBoleto").hide();
                $("#DatosPago").hide();
                $("#DatosEstablecimiento").hide();
                $("#DatosDescuentos").hide();
                $("#baseDiv").hide();
                $("#DatosAdicionales").hide();
                $(".datos-adicionales").hide();
                $("#DatosCalidades").hide();
                $(".datos-calidades").hide();
                $(".datos-boleto").hide();
                $(".datos-topesplazos").hide();
                $(".datos-establecimiento").hide();
                $("#ContratoDiv").show();
                $("#ComercialDiv").show();
                $("#DatosBoleto").hide();
                $("#DatosPago").hide();
                $("#establecimientoDiv").hide();
                $("#mercsDepositoDiv").hide();
                $("#guardarBtn").empty();
                $("#guardarBtn").append("Guardar Fijacion");
                $("#chequeElectronicoId").show();
                $("#pagoCbuId").show();
                RemoverFondosGrises();
                $("#boton-ampliar").hide();
                $(".ampliar").hide();
                $(".ampliar-fijacion").show();
                $("#aperturaPrecioDiv").show();
                $("#ocultarAperturaBtn").show();
                $("#pagoDiferidoFijacionDiv").removeClass("hide-fijacion");
                $("#pagoDiferidoFijacionDiv").addClass("inline-fijacion");
                $("#ocultarAperturaMoneda").removeClass("w100");
                $("#ocultarAperturaMoneda").addClass("w70");
                $('#pagoDirectoDiv').hide();

                $("#pizarraDiv").show();
                if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");

                if (viewModel.AperturaPrecio.length > 0) {
                    viewModel.AperturaPrecio[2].Porcentaje = 0;
                    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
                    if (Number($("#precioId").val().replace(',', '.')) > 0) {
                        GuardarAperturaDePrecio();
                    }

                }
                $("#cargarCantidadPendienteFijar").show();

            }
            else if (this.value() == 4) {

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
                if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("USDM ");
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
                $("#precioMonedaId").data("kendoDropDownList").value("USDM ");
                $("#pizarraDiv").prop("checked", false);
            } else if (this.value() == 6) {
                $("#boton-ampliar").hide();
                $(".noAcuerdo").hide();
                $(".acuerdo").show();
                RemoverFondosGrises();
                $("#guardarBtn").empty();
                $("#guardarBtn").append("Guardar Acuerdo");
                $("#aperturaPrecioDiv").show();
                $("#ocultarAperturaBtn").show();
                $("#ocultarAperturaMoneda").removeClass("w100");
                $("#ocultarAperturaMoneda").addClass("w70");
                if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
                $("#pizarraDiv").prop("checked", false);

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
                if (this.value() == 1) {

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
                if (this.value() == 2) {
                    $(".ocultar").hide();
                    $("#CDId").prop("checked", false);
                    $("#WarrantId").prop("checked", false);
                    $("#pagosDiv").show();
                    if ($("#hijoId").is(':checked') == false) {
                        $("#fechaCiertaDiv").show();
                    }

                    $("#fechaDesdeTopeId").val("");
                    $("#fechaHastaTopeId").val("");
                    $("#condicionFijacionId").data("kendoDropDownList").value("");
                    RemoverFondosGrises();
                    if ($("#boton-ampliar").text() == "+ AMPLIAR") {
                        $(".contratoAPrecio").hide();
                    }
                    else {
                        $(".contratoAPrecio").show();
                    }
                    $("#pizarraDiv").show();
                    $("#aperturaPrecioDiv").show();
                    $("#ocultarAperturaBtn").show();
                    $("#ocultarAperturaMoneda").removeClass("w100");
                    $("#ocultarAperturaMoneda").addClass("w70");
                    $("#chequeElectronicoDiv").show();
                    $("#chequeElectronicoId").hide();
                    $("#pagoCbuDiv").show();
                    $("#pagoCbuId").hide();
                    if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
                }
                if ($("#boton-ampliar").text() == "+ AMPLIAR") {
                    $(".ampliar").hide();
                }
                CalcularPrecioTotalApertura();
            }
            ClickEnPizarra();
            if (this.value() == 6) {
                var precioRojo = $("#precioId").hasClass("required-box-parent") ? $("#precioId") : $("#precioId").parent().parent();
                $("#precioId").data("kendoNumericTextBox").value("");
                $("#precioId").trigger('change');
                precioRojo.removeClass("required-border");


            }
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
            $("#fechaCiertaDiv").hide();
            $("#fechaCiertaId").data("kendoDatePicker").value("");
            $("#buscadorCorredor").prop('disabled', true);
            $("#buscadorProveedor").prop('disabled', true);
            $("#material").data("kendoDropDownList").enable(false);
        }
        else {
            $(".contratoMadreDiv").hide();
            $("#contMadreId").val("");
            $("#fechaCiertaDiv").show();
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
            CargarCalidadPorMaterial($('#material').data("kendoDropDownList").value());
            if ($("#material").val() !== "") {
                CargarCampaniaPorMaterial($("#material").val());
                CargarCalidadPorMaterial($("#material").val());
                var iteraciones = viewModel.Calidades.length;
                for (var i = 0; i < iteraciones; i++) {
                    viewModel.Calidades.pop();
                }
            }
            if ($("#material").val() === "3" && ($("#tipoId").val() === "1" || $("#tipoId").val() === "2")) {
                $(".sojaSustentable").show();
            } else {
                $(".sojaSustentable").hide();
                $(".sustentableDiv").hide();
                $("#sustentablePrecioId").data('kendoNumericTextBox').value("");
                $("#sustentableId").prop('checked', false);
            }
            if ($("#material").val() === "2" && $("#tipoId").val() === "4") {
                $("#fasonEspecial").show();
            } else {
                $("#fasonEspecial").hide();
            }
            $("#contratoId").val("");
            $(".datoscontrato").hide();
            $("#datosContrato").hide();
        }
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#precioMonedaId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId",
        change: function () {
            if ($("#precioMonedaId").val() === "ARP  " && ($("#tipoId").val() === "2" || $("#tipoId").val() === "6")) {
                $("#pagoDolarizadoDiv").hide();
                $("#dolarizadoDiv").hide();
                if ($("#fechaCiertaId").val() == "") {
                    $("#pagoDiferidoDiv").show();
                }
                $("#dolarizadoId").prop("checked", false);
                $("#dolarizadoFechaId").data("kendoDatePicker").value("");
                $("#dolarizadoExpressDiv").hide();
                $("#dolarizadoExpressId").prop("checked", false);        
                if (!$("#pesificadoId").is(":checked")) {
                    if ($("#tipoId").val() === "2") {
                        $("#fechaCiertaDiv").show();
                    }                    
                    if ($("#tipoId").val() === "6") {
                        $("#fechaCiertaAcuerdoDiv").show();
                    }
                }
            } else if ($("#precioMonedaId").val() === "USDM " && ($("#tipoId").val() === "2" || $("#tipoId").val() === "6")) {
                $("#pagoDiferidoDiv").hide();
                $("#pesificadoDiv").hide();
                if ($("#fechaCiertaId").val() == "") {
                    $("#pagoDolarizadoDiv").show();
                    $("#dolarizadoDiv").hide();

                }
                $("#pesificadoId").prop("checked", false);
                $("#pesificadoDiasId").data("kendoNumericTextBox").value("");

                if ($("#clasificacion").val() == "1" && $("#fechaCiertaId").val() == "") {
                    $("#dolarizadoExpressDiv").show();
                } else {
                    $("#dolarizadoExpressDiv").hide();
                    //$("#dolarizadoDiv").hide();
                    $("#dolarizadoExpressId").prop("checked", false);                    
                   
                }
            }
            if ($("#tipoId").val() === "3") {
                CalcularPrecioTotalApertura();
            }

        },
        select: function (e) {
            $("#monedaPactadoId").data("kendoDropDownList").value(e.dataItem.MonedaId);
        }
    });

    $("#monedaAjusteComisionId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#precioMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#precioMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

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
        optionLabel: "MONEDA...",
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
                $("#consignatarioDiv").hide();
                $("#consignatarioId").prop("checked", false);
                $("#planCanjeDiv").hide();
                $("#planCanjeId").prop("checked", false);                            
                if ($("#precioMonedaId").data("kendoDropDownList").value() == "USDM " && $("#fechaCiertaId").val() == "" && $("#buscadorCorredor").val() == "") {
                        $("#dolarizadoExpressDiv").show();
                        $("#dolarizadoExpressId").attr("disabled", false);
                        $("#pagoDolarizadoDiv").show();
                        //$("#dolarizadoId").attr("disabled", false);
                } else {
                    $("#dolarizadoExpressDiv").hide();
                    $("#dolarizadoExpressId").prop("checked", false);
                    //$("#dolarizadoDiv").hide();
                    $("#dolarizadoFechaId").val("");

                }

            } else {
                if (!$("#pesificadoId").is(":checked")) {
                    $("#fechaCiertaDiv").show();
                }
                //$("#pagoDolarizadoDiv").hide();
                //$("#dolarizadoId").prop("checked", false);
                $("#consignatarioDiv").show();
                //$("#dolarizadoDiv").hide();
                $("#dolarizadoFechaId").val("");
                $("#planCanjeDiv").show();
                $("#dolarizadoExpressDiv").hide();
                $("#dolarizadoExpressId").prop("checked", false);
            }
            ValidarAlta();
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
    });
    $("#consignatarioId").click(function () {
        if (this.checked) {
            $("#planCanjeId").prop("checked", false);
        }
        ValidarAlta();
    });

    $("#destinoId").kendoDropDownList({
        optionLabel: "SELECCIONE EL DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id",
        change: function () {
            if ($("#tipoId").val() == 1 && $("#destinoId").val() != 1) {
                LimpiarDescuentos();
                $("#ImporteDescuentoId").data("kendoNumericTextBox").value("");
                $("#PorcentajeDescuentoId").val("");
            }
        }
    });

    $("#destinoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#destinoId").data("kendoDropDownList");
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
                    $("#chequeElectronicoId").show();
                    $("#pagoCbuId").show();
                }
            }
            if (this.value() == "" && ($("#tipoId").val() == "2")) {
                $("#chequeElectronicoDiv").show();
                $("#pagoCbuDiv").show();
            }
            if (this.value() >= 1) {
                $("#chequeElectronicoInput").prop("checked", false);
                $("#chequeElectronicoDiv").hide();
                $("#pagoCbuDiv").hide();
                $("#chequeElectronico").prop("checked", false);
                $("#chequeElectronicoId").hide();
                $("#pagoCbuId").hide();
                $("#pagoCbu").val("");
                $("#pagoCbuInput").val("");
            }
            if (this.value() == 1) {
                $("#boletoNingunoId").prop("checked", false);
                $("#boletoNingunoId").click();
                $("#boletoNingunoId").attr("readonly", "readonly");
                $("#boletoConfirmaId").attr("disabled", true);
                $("#boletoFisicoId").attr("disabled", true);
                $("#boletoCartaId").attr("disabled", true);
            } else {
                $("#boletoNingunoId").prop("checked", false);
                $("#boletoNingunoId").removeAttr("readonly");
                $("#boletoConfirmaId").removeAttr("disabled");
                $("#boletoFisicoId").removeAttr("disabled");
                $("#boletoCartaId").removeAttr("disabled");
            }
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
        }
    });

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
    $("#precioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0,
        change: function () {
            if ($("#tipoId").val() == "6") {
                if ($("#AgenteCompraId").val() == "") {
                    $("#chequeElectronicoId").show();
                    $("#pagoCbuId").show();
                }
                if ($("#precioId").val() != "" && $("#precioId").val() != "0") {
                    $("#fechaCiertaAcuerdoDiv").show();
                } else {
                    $("#fechaCiertaAcuerdo").data("kendoDatePicker").value("");
                    $("#fechaCiertaAcuerdoDiv").hide();
                }

            }

        }
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

    $("#pesificadoDiasId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#porcentajeComision").kendoNumericTextBox({
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
                $("#fechaOperacionMotivoDiv").show();
                if (diffDays > 1) {
                    //$("#noInformaSioId").prop("checked", true);
                    //$("#noInformaSioId").attr("disabled", true);
                } else {
                    //$("#noInformaSioId").prop("checked", false);
                    //$("#noInformaSioId").attr("disabled", false);
                }

            } else {
                $("#fechaOperacionMotivoDiv").hide();
                $("#motivoOperacionAnteriorId").val("");
                //$("#noInformaSioId").attr("disabled", false);
            }
        }
    });
    $("#fechaDesdeId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            $("#fechaHastaId").val(ObtenerFechaHasta(this.value()));
            validarFechaCampana();
        }
    });
    $("#fechaHastaId").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () { $("#fechaHastaTopeId").val(ObtenerFechaHasta(this.value())); }
    });
    $("#fechaHastaTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#dolarizadoFechaId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaCiertaId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            if ($("#fechaCiertaId").val() == "") {
                if ($("#precioMonedaId").val() == "ARP  ") {
                    $("#pagoDiferidoDiv").show();
                }
                if (!$("#dolarizadoExpressId").is(":checked") && $("#precioMonedaId").val() == "USDM ") {
                    $("#pagoDolarizadoDiv").show();
                    if ($("#clasificacion").val() == 1) {
                        $("#dolarizadoExpressDiv").show();

                    }
                }

            } else {
                if (!$("#dolarizadoExpressId").is(":checked")) {
                    $("#dolarizadoFechaId").val("");                    
                }
                if (!$("#dolarizadoId").is(":checked")) {
                    $("#dolarizadoFechaId").val("");
                }
                $("#dolarizadoExpressDiv").hide();
                $("#dolarizadoDiv").hide();  
                $("#pagoDolarizadoDiv").hide();  
                $("#pagoDiferidoDiv").hide();
            }            
        }
    });
    $("#fechaCiertaAcuerdo").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            if ($("#fechaCiertaAcuerdo").val() == "") {
                if ($("#precioMonedaId").val() == "USDM ") {
                    $("#pagoDolarizadoDiv").show();
                } else {
                    $("#pagoDiferidoDiv").show();       

                }

                
            } else {
                $("#pagoDiferidoDiv").hide();
                $("#dolarizadoId").attr("disabled", false);
                $("#pagoDolarizadoDiv").hide();
                $("#dolarizadoDiv").hide();                
                $("#dolarizadoFechaId").val("");
            }
        }
    });

    $("#fechaDesdeId").val(date);
    $("#fechaOperacionId").val(date);

    $("#fechaHastaId").val(datehasta);

    $(".formulario-footer-guardar-contrato").click(function () {
        BlockUi('Guardando...');

        setTimeout(ObtenerDatos, 250);
    });

    $(".formulario-footer-cancelar").click(function () {
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

    $("#sustentableId").click(function () {
        if ($(this).is(':checked')) {
            $(".sustentableDiv").show();
            if ($("#mercsDepositoId").is(':checked')) {
                MensInfo('Revisar la fecha desde de entrega');
            }
        }
        else {
            $(".sustentableDiv").hide();
            $("#sustentablePrecioId").data("kendoNumericTextBox").value("");
        }
    });
    $("#mercsDepositoId").click(function () {
        if ($(this).is(':checked') && $("#sustentableId").is(':checked')) {
            MensInfo('Revisar la fecha desde de entrega');
        }
    });

    $("#dolarizadoId").click(function () {
        if ($(this).is(':checked')) {
            $("#dolarizadoDiv").show();
            $("#dolarizadoExpressId").prop("checked", false);
            $("#pesificadoId").prop("checked", false);
            $("#chequeElectronicoDiv").show();
            $("#pesificadoDiv").hide();
            $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
            $("#fechaCiertaDiv").hide();
            $("#fechaCiertaId").val("");
            $("#fechaCiertaAcuerdoDiv").hide();
            $("#fechaCiertaAcuerdo").val("");
            
            
        }
        else {
            if ($("#buscadorCorredor").val() == "") {
                $("#dolarizadoExpressId").prop("disabled", false);
            }
            if (!$(this).is(':checked') && !$("#dolarizadoExpressId").is(':checked') && $("#tipoId").val() == "2") {
                $("#fechaCiertaDiv").show();
            } else {
                $("#fechaCiertaAcuerdoDiv").show();
                
            }
            $("#dolarizadoFechaId").val("");
        }
    });

    $("#dolarizadoExpressId").click(function () {
        if ($(this).is(':checked')) {
            $("#dolarizadoDiv").show();
            $("#dolarizadoId").prop("checked", false);
            $("#chequeElectronicoInput").prop("checked", false);
            $("#chequeElectronicoDiv").hide();
            $("#pagoCbu").prop("checked", false);
            $("#pagoCbuDiv").hide();
            $("#fechaCiertaDiv").hide();
            $("#fechaCiertaId").val("");
        }
        else {         
            if (!$(this).is(':checked') && !$("#dolarizadoId").is(':checked')) {
                $("#fechaCiertaDiv").show();
            }
            $("#dolarizadoFechaId").val("");
            if (!$("#compensacionId").is(":checked")) {
                $("#pagoCbuDiv").show();
                if (!$("#pagoDirectoId").is(":checked")) {
                    $("#chequeElectronicoDiv").show();
                }
            }
        }
    });

    $("#pesificadoId").click(function () {

        if ($(this).is(':checked')) {
            $("#pesificadoDiv").show();

            $("#dolarizadoId").prop("checked", false);
            $("#dolarizadoDiv").hide();
            $("#dolarizadoFechaId").val("");
            $("#fechaCiertaAcuerdoDiv").hide();
            $("#fechaCiertaAcuerdo").val("");
            $("#fechaCiertaDiv").hide();
            $("#fechaCiertaId").val("");
        }
        else {
            $("#pesificadoDiv").hide();
            $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
            $("#fechaCiertaAcuerdoDiv").show();

            if ($("#tipoId").val() == "2") {
                $("#fechaCiertaDiv").show();

            } else {
                $("#fechaCiertaDiv").hide();
            }
        }
    });

    $("#diasDiferidoId").click(function () {
        if ($(this).is(':checked')) {
            $("#diasDiferidoFijacionDiv").removeClass("hide-fijacion");
            $("#diasDiferidoFijacionDiv").addClass("inline-fijacion");

            $("#fechaCiertaAcuerdoDiv").hide();
            $("#fechaCiertaAcuerdo").val("");
            $("#fechaCiertaDiv").hide();
            $("#fechaCiertaId").val("");
        }
        else {
            $("#diasDiferidoFijacionDiv").removeClass("inline-fijacion");
            $("#diasDiferidoFijacionDiv").addClass("hide-fijacion");
            $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");

            if ($("#tipoId").val() == "2") {
                $("#fechaCiertaDiv").show();

            } else {
                $("#fechaCiertaAcuerdoDiv").show();
            }
        }
    });

    $("#boletoConfirmaId").click(function () {
        if ($(this).is(':checked')) {
            LimpiarBoleto();
            $("#boletoConfirmaId").prop("checked", true);
            $("#BolsaConfirmaDiv").show();
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
        }
    });

    $("#CDId").click(function () {
        if ($(this).is(':checked') && viewModel.AperturaPrecio.some(importeNoVacio) && $("#tipoId").val() != "6") {
            MensInfo("Debe borrar datos de apertura de precio para completar datos de flete procedencia");
        }
        $("#WarrantId").prop("checked", false);
        $("#pagoDirectoId").prop("checked", false);

        if ($(this).is(':checked') && $("#tipoId").val() == "2") {
            $(".ocultar").show();
        } else $(".ocultar").hide();
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
            if ($("#tipoId").val() == '6' || $("#tipoId").val() == '3') {
                $("#chequeElectronicoId").hide();
                $("#chequeElectronico").prop("checked", false);
            }
            $("#chequeElectronicoDiv").hide();
            $("#chequeElectronicoInput").prop("checked", false);
        } else {
            if ($("#tipoId").val() == '6' || $("#tipoId").val() == '3') {
                $("#chequeElectronicoId").show();
            } if (!$("#compensacionId").is(":checked")) {
                $("#chequeElectronicoDiv").show();
            }

        }
        $("#CDId").prop("checked", false);
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
                $(".contratoAFijar").show();
                $("#pagosDiv").hide();

                if ($("#madreId").is(':checked')) {
                    $("#pagosDiv").show();
                }
            } else if ($('#tipoId').val() == 2) {
                $(".contratoAFijar").hide();
                $(".contratoAPrecio").show();
                $("#pagosDiv").show();
            }
            //else if ($('#tipoId').val() == 6) {
            //    $(".contratoAFijar").hide();
            //    $(".contratoAPrecio").hide();
            //    $("#pagosDiv").hide();
            //    $(".noAcuerdo").hide();
            //}
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
            $("#condicionFijacionId").data("kendoDropDownList").value("7");
            $("#fechaDesdeTopeId").val(date);
            $("#fechaHastaTopeId").val(datehasta);
            LimpiarDescuentos();
            LimpiarApertura();
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
        if ($('#tipoId').val() == 6) {
            $("#condicionFijacionId").data("kendoDropDownList").value("7");
            $("#fechaDesdeTopeId").val(date);
            $("#fechaHastaTopeId").val(datehasta);
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
        if ($(this).val() == 1) {
            $(".fecha-descuento").hide();
            $(".fecha-descuento-pendiente").hide();
            $("#fechaHastaDescuentoId").val("");
            $("#fechaDesdeDescuentoId").val("");
        }
        else {
            $(".fecha-descuento").show();
            $(".fecha-descuento-pendiente").show();
            $("#fechaDesdeDescuentoId").val(date);
            $("#fechaHastaDescuentoId").val(datehasta);
        }
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

    $("#pizarraId").click(ClickEnPizarra);

    InicializarAperturaDePrecios();

    $("#ImporteDescuentoId").kendoNumericTextBox({
        culture: "es-AR",
        spinners: false,
        change: function () { ConvertirDescuentoANegativo(); }
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

    $("#monedaPactadoId").kendoDropDownList({
        optionLabel: "Moneda",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });
    $("#monedaPactadoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#monedaPactadoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#monedaPactadoId").data("kendoDropDownList").enable(false);
    $("#monedaImportePactadoId").kendoDropDownList({
        optionLabel: "Moneda",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });
    $("#monedaImportePactadoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#monedaImportePactadoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#agregarPrecioPactado").click(AgregarPrecioPactado);


    $("#pagoCbu").kendoAutoComplete({
        template: '<p class="buscar-nomb">#: data.Pago#</p>',
        dataTextField: "Pago",
        dataValueField: "Pago",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#pagoCbu").val() == "" && !$("#compensacionId").is(":checked") && !$("#dolarizadoExpressId").is(":checked")) {
                $("#chequeElectronicoDiv").show();
            } else {
                $("#chequeElectronicoDiv").hide();
                $("#chequeElectronicoInput").prop("checked", false);

            }
        },
        //select: function (e) {
        //    $("#pagoCbu").val(e.dataItem.Pago);
        //},
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerListaDeCbu"
                },
                parameterMap: function (data, type) {
                    var cuitProv = $("#buscadorProveedor").val().split('(');
                    if (cuitProv[1] != null) {
                        var cuitP = cuitProv[1].split(')');
                    }
                    else {
                        cuitP = cuitProv;
                    }
                    return { cuitProveedor: cuitP[0], filtro: $('#pagoCbu').val() };
                }
            }

        }
    });
    $('#pagoCbu').click(function (e) {
        $('#pagoCbu').val("");
        $("#pagoCbu").data("kendoAutoComplete").search("");
    });


    $("#pagoCbuInput").kendoAutoComplete({
        template: '<p class="buscar-nomb">#: data.Pago#</p>',
        type: 'text',
        dataTextField: "Pago",
        dataValueField: "Pago",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#pagoCbuInput").val() == "") {
                $("#chequeElectronicoId").show();
            } else {
                $("#chequeElectronicoId").hide();
                $("#chequeElectronico").prop("checked", false);
            }
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerListaDeCbu"
                },
                parameterMap: function (data, type) {
                    var cuitProv = $("#buscadorProveedor").val().split('(');
                    if (cuitProv[1] != null) {
                        var cuitP = cuitProv[1].split(')');
                    }
                    else {
                        cuitP = cuitProv;
                    }
                    return { cuitProveedor: cuitP[0], filtro: $('#pagoCbuInput').val() };
                }
            }

        }
    });
    $('#pagoCbuInput').click(function (e) {
        $('#pagoCbuInput').val("");
        $("#pagoCbuInput").data("kendoAutoComplete").search("");
    });

    //FIN INICIALIZARELEMENTOS
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
}

function ClickEnPizarra() {
    var precioRojo = $("#precioId").hasClass("required-box-parent") ? $("#precioId") : $("#precioId").parent().parent();
    if ($("#pizarraId").is(':checked')) {
        LimpiarApertura();
        $("#precioId").data("kendoNumericTextBox").enable(false);
        $("#precioMonedaId").data("kendoDropDownList").enable(false);
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").enable(false);
        $("#pesificadoDiasId").data("kendoNumericTextBox").enable(false);
        $("#aperturaPrecioBtn").addClass("pointerEventDesabilitado");
        $("#precioId").data("kendoNumericTextBox").value("");
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");
        $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
        $("#precioId").trigger('change');
        $("#precioTotalApertura").data("kendoNumericTextBox").value("");
        $("#precioMonedaId").data("kendoDropDownList").value(0);

        $("#pagoDiferidoFijacionDiv").removeClass("inline-fijacion");
        $("#pagoDiferidoFijacionDiv").addClass("hide-fijacion");
        $("#pagoDiferidoDiv").hide();
        $("#pesificadoDiv").hide();
        $("#diasDiferidoFijacionDiv").removeClass("inline-fijacion");
        $("#diasDiferidoFijacionDiv").addClass("hide-fijacion");
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");
        precioRojo.removeClass("required-border");
    }
    else {
        $("#precioId").data("kendoNumericTextBox").enable(true);
        $("#precioMonedaId").data("kendoDropDownList").enable(true);
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").enable(true);
        $("#pesificadoDiasId").data("kendoNumericTextBox").enable(true);
        $("#diasDiferidoId").prop("checked", false);
        $("#pesificadoId").prop("checked", false);
        $("#aperturaPrecioBtn").removeClass("pointerEventDesabilitado");
        $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        if ($("#tipoId").val() == 3) {
            $("#pagoDiferidoFijacionDiv").addClass("inline-fijacion");
            $("#pagoDiferidoFijacionDiv").removeClass("hide-fijacion");
        } else {
            if ($("#tipoId").val() != 6) {
                $("#pagoDiferidoDiv").show();
            }

        }
        if ($("#tipoId").val() == 5) {
            $("#precioMonedaId").data("kendoDropDownList").value("USDM ");
        }
        precioRojo.addClass("required-border");
    }

}

function LimpiarBoleto() {
    $("#boletoFisicoId").prop("checked", false);
    $("#boletoConfirmaId").prop("checked", false);
    $("#boletoCartaId").prop("checked", false);
    $("#boletoNingunoId").prop("checked", false);
    $("#bolsaFisicoId").data("kendoDropDownList").value("");
    $("#bolsaConfirmaId").data("kendoDropDownList").value("");
    $("#bolsaCartaId").data("kendoDropDownList").value("");
    $("#BolsaConfirmaDiv").hide();
    $("#BolsaFisicoDiv").hide();
    $("#BolsaCartaDiv").hide();
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
        MonedaPactadoCombo: []
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

function InicializarDatos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniCrearContrato = data;
            AsignarDatos();
        }

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
    };

    MSExecuteURLOnServerAsync('/CompraNet/InicializarContrato', funcReturn, '');
}

function AsignarDatos() {
    viewModel.set("ComercialCombo", datosIniCrearContrato.Datos.comercial);
    viewModel.set("MaterialCombo", datosIniCrearContrato.Datos.material);
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
        if (datosIniCrearContrato.Datos.Bolsa[i].Descripcion == "Bs As" || datosIniCrearContrato.Datos.Bolsa[i].Descripcion == "Rosario")
            bolsaFisico.push(datosIniCrearContrato.Datos.Bolsa[i]);
    }
    viewModel.set("BolsaFisicoCombo", bolsaFisico);
    var bolsaCarta = [];
    for (i = 0; i < datosIniCrearContrato.Datos.Bolsa.length; i++) {
        if (datosIniCrearContrato.Datos.Bolsa[i].Descripcion == "Bs As")
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

    viewModel.set("TipoPeriodoDBCombo", datosIniCrearContrato.Datos.TipoPeriodoDB);
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

    viewModel.set("isControlDisabled", false);

    if ($("#tipoId").data("kendoDropDownList")) $("#tipoId").data("kendoDropDownList").value("2");
    if ($("#precioMonedaId").data("kendoDropDownList")) {
        $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        $("#pagoDolarizadoDiv").hide();
    }
    if ($("#monedaPactadoId").data("kendoDropDownList")) $("#monedaPactadoId").data("kendoDropDownList").value("ARP  ");

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
    $("#errdolarizadoId").css("display", "none");
    $("#errdolarizadoFechaId").css("display", "none");
    $("#errpesificadoId").css("display", "none");
    $("#errpesificadoDiasId").css("display", "none");
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

function ObtenerDatos() {
    var obj = {};

    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth();
    var dia = hoy.getDate();
    var hora = hoy.getHours();
    var minuto = hoy.getMinutes();

    obj.TipoNegocioId = $("#tipoId").val();
    obj.Id = Id;
    obj.MaterialId = $("#material").val();
    obj.Cantidad = $("#cantidadId").val();
    obj.Precio = $("#precioId").val();
    obj.PrecioNeto = $("#precioTotalApertura").val();
    obj.FechaEntrega = $("#fechaHastaId").val();
    obj.CampanaId = $("#campanaId").val();
    obj.FechaDesde = $("#fechaDesdeId").val();
    obj.FechaHasta = $("#fechaHastaId").val();
    obj.MonedaId = $("#precioMonedaId").val();
    if (obj.TipoNegocioId == "3" || obj.TipoNegocioId == "4") {
        obj.ComercialId = $("#comercialFijacionId").val();
        obj.ContratoSAP = $("#contratoId").val();
    }
    if (obj.TipoNegocioId == "1" || obj.TipoNegocioId == "2") {
        obj.PorcentajeDePago = $("#porcentajeDePagoId").val();
    }

    obj.ComercialId = $("#comercialId").val();
    obj.ComercialCreadorId = $("#comercialCreador").val();
    obj.Base = $("#baseId").is(":checked") ? true : false;
    obj.ImporteSustentable = $("#sustentablePrecioId").val();
    obj.MonedaSustentableId = $("#sustentableMonedaId").val();
    obj.FechaDolarizado = $("#dolarizadoFechaId").val();
    obj.PagoDiferidoContrato = $("#pesificadoId").is(":checked") ? true : false;
    obj.PagoDiferido = obj.TipoNegocioId != 3 ? $("#pesificadoId").is(":checked") ? true : false : $("#diasDiferidoId").is(":checked") ? true : false;
    obj.Dolarizado = $("#dolarizadoId").is(":checked") ? true : false;
    obj.Sustentable = $("#sustentableId").is(":checked") ? true : false;
    obj.DiasPesificado = obj.TipoNegocioId != 3 ? $("#pesificadoDiasId").val() : $("#diasDiferidoFijacionId").val();
    obj.PorcentajeComision = $("#porcentajeComision").val() != "" ? $("#porcentajeComision").val() : 0;
    obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.EstadoId = $("#estado").val() == "" ? $("#baseId").is(":checked") ? "3" : obj.TipoNegocioId != 4 && obj.TipoNegocioId != 5 && obj.TipoNegocioId != 6 ? "1" : "2" : $("#estado").val();
    obj.Observacion = $("#observacionId").val();
    obj.ClasificacionId = $("#clasificacion").val();
    obj.CantidadCamiones = $("#cantidadCamionesId").val();
    obj.EstablecimientoPropio = $("#establecimientoPropioId").is(":checked") ? true : $("#establecimientoArrendadoId").is(":checked") ? false : null;
    obj.DesdeFijacion = $("#fechaDesdeTopeId").val();
    obj.HastaFijacion = $("#fechaHastaTopeId").val();
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
    obj.DolarizadoExpress = $("#dolarizadoExpressId").is(":checked") ? true : false;
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

    if ($("#buscadorProveedor").val() != "") {
        var cuitAux = $("#buscadorProveedor").val().split('(');
        var proveedorId;
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
            var corredorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuitC[0], corredor: true });
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
                var error = true;
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
        obj.FechaOperacion = $("#fechaOperacionId").val();
        obj.MotivoOperacionAnterior = $("#motivoOperacionAnteriorId").val();
    }
    if (!error) {
        GrabarContrato(obj);
    } else {
        $.unblockUI();
    }
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
            result = MSExecuteOnServer('/CompraNet/GrabarContrato', nuevoContrato);
        }
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
                window.location.href = window.location.origin + "/CompraNet";
            }
        }
    }
    $.unblockUI();
}

function editarContrato(id, tipoId, siguientes) {
    window.location.href = window.location.origin + "/CompraNet/CrearContrato?id=" + id + '&tipoId=' + tipoId + (siguientes != undefined ? "&siguientes=" + JSON.stringify(siguientes) : "");
}

function obtenerLocalidadProvincia() {
    if ($("#buscadorProveedor").val() != "") {
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
            viewModel.Descuentos.remove(this);
        }
    };

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
    //var fechaD = kendo.parseDate(descuento.FechaDesde, "dd-MM-yyyy");
    //var fechaH = kendo.parseDate(descuento.FechaHasta, "dd-MM-yyyy");
    //if (descuento.TipoPeriodoDBId != 1 && (!fechaD || !fechaH)) {
    //    errores.push("La fecha no es válida");
    //}
    if (descuento.Importe == 0 && descuento.Porcentaje == 0) {
        errores.push("El campo Importe y Porcentaje no pueden estar vacíos");
    }
    if (descuento.Importe !== 0 && (descuento.MonedaId == "Moneda" || descuento.MonedaId == null || descuento.MonedaId == undefined || descuento.MonedaId == "")) {
        errores.push("El campo Moneda no puede estar vacío");
    }

    if (descuentos != undefined && descuentos != null && descuentos.length > 0) {
        for (var i = 0; i < descuentos.length; i++) {

            if (descuentos[i].TipoPeriodoDBId == descuento.TipoPeriodoDBId &&
                descuentos[i].TipoDBId == descuento.TipoDBId &&
                descuentos[i].Importe == descuento.Importe &&
                descuentos[i].Porcentaje == descuento.Porcentaje &&
                descuentos[i].MonedaId == descuento.MonedaId || descuentos[i].TipoDBDesc == descuento.TipoDBDesc) {

                sonIguales = true;
                break;
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
            $("#calidadesEspecialesId").val("");
            $("#valorEspecialesId").data("kendoNumericTextBox").value("");
            $("#porcentajeDesdeId").data("kendoNumericTextBox").value("");
            $("#porcentajeHastaId").data("kendoNumericTextBox").value("");
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
    if (calidad.PorcentajeHasta > 40 && calidad.CalidadEspecialId == 1) {
        errores.push('El Porcentaje Hasta no debe ser mayor a 40% para "Dañados"');
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
function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
}

function CargarDatosEditar(contrato, hijo) {
    InicializarBordesRojos();
    $("#buscadorCorredor").val(contrato.Corredor);
    $("#buscadorCorredor").trigger("change");
    if (contrato.Corredor != null && contrato.Corredor != "") {
        $('#contCorredorDiv').show();
    }
    $("#estado").val(contrato.Estado);
    $("#buscadorProveedor").val(contrato.Proveedor);
    $("#buscadorProveedor").trigger("change");

    if (contrato.FechaOperacionFormateado != null) {
        $("#fechaOperacionId").val(FormatearFecha(formatearFecha(contrato.FechaOperacionFormateado)));
    } else {
        $("#fechaOperacionId").val("");

    }

    if (contrato.MotivoOperacionAnterior != null && contrato.MotivoOperacionAnterior != "") {
        $("#motivoOperacionAnteriorId").val(contrato.MotivoOperacionAnterior);
        $("#fechaOperacionMotivoDiv").show();
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

    } else {
        $("#motivoOperacionAnteriorId").val("");
        $("#fechaOperacionMotivoDiv").hide();
    }

    $("#fechaDesdeId").val(FormatearFecha(formatearFecha(contrato.FechaDesdeFormateado)));
    $("#fechaHastaId").val(FormatearFecha(formatearFecha(contrato.FechaHastaFormateado)));
    $("#fechaCiertaId").val(FormatearFecha(formatearFecha(contrato.FechaCiertaFormateado)));
    $("#fechaCiertaAcuerdo").val(FormatearFecha(formatearFecha(contrato.FechaCiertaFormateado)));
    $("#porcentajeDePagoId").data("kendoNumericTextBox").value(contrato.PorcentajeDePago == null ? 97.5 : contrato.PorcentajeDePago);

    if (!hijo) {
        //$("#fechaOperacionId").val(FormatearFecha(formatearFecha(contrato.FechaFormateado)));
        $("#tipoId").data("kendoDropDownList").value(contrato.TipoNegocioId);
        $("#tipoId").data("kendoDropDownList").trigger("change");
        if (contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 6) {
            if (!(contrato.DesdeFijacionFormateado == null && contrato.DesdeFijacionFormateado == undefined && contrato.DesdeFijacionFormateado == "")) {
                $("#fechaDesdeTopeId").val(FormatearFecha(formatearFecha(contrato.DesdeFijacionFormateado)));
            } else {
                $("#fechaDesdeTopeId").val("");
            }
            if (!(contrato.HastaFijacionFormateado == "null" && contrato.HastaFijacionFormateado == undefined && contrato.HastaFijacionFormateado == "")) {
                $("#fechaHastaTopeId").val(FormatearFecha(formatearFecha(contrato.HastaFijacionFormateado)));
            } else {
                $("#fechaHastaTopeId").val("");
            }
            $("#condicionFijacionId").data("kendoDropDownList").value(contrato.CondicionFijacion);
        }
    }
    $("#material").data("kendoDropDownList").value(contrato.MaterialId);
    $("#material").data("kendoDropDownList").trigger("change");

    $("#NivelTarifaId").data("kendoDropDownList").value(contrato.NivelTarifaId);
    $("#NivelTarifaId").data("kendoDropDownList").trigger("change");

    $("#TarifaFleteId").data('kendoNumericTextBox').value(contrato.TarifaFlete);

    $("#observacionId").val(contrato.Observacion);
    $("#cantidadId").data("kendoNumericTextBox").value(contrato.Cantidad);
    $("#cantidadId").trigger("change");

    $("#precioId").data("kendoNumericTextBox").value(contrato.Precio);
    $("#precioId").trigger('change');

    $("#precioTotalApertura").data("kendoNumericTextBox").value(contrato.PrecioNeto);
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

    $("#campanaId").data("kendoDropDownList").value(contrato.CampanaId);

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

    if (contrato.Dolarizado) {
        $("#dolarizadoId").prop("checked", true);
        $("#dolarizadoDiv").show();
        $("#dolarizadoFechaId").val(FormatearFecha(contrato.Fecha_DolarizadoFormateado));
    }

    if (contrato.PagoDiferido === true && contrato.TipoNegocioId != 3) {
        $("#pesificadoId").prop("checked", true);
        $("#pesificadoDiv").show();
        $("#pesificadoDiasId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    } else {
        $("#pesificadoId").prop("checked", true);
        $("#diasDiferidoId").prop("checked", true);
        $("#diasDiferidoFijacionDiv").addClass("inline-fijacion");
        $("#diasDiferidoFijacionDiv").removeClass("hide-fijacion");
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    }
    if (contrato.PagoDiferido === true) {
        $("#pesificadoId").prop("checked", true);
        $("#pesificadoDiv").show();
        $("#pesificadoDiasId").data("kendoNumericTextBox").value(contrato.Dias_Pesificado);
    } else {
        $("#pesificadoId").prop("checked", false);
        $("#diasDiferidoId").prop("checked", false);
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").value("");
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
        $("#trigoEspecialFasonId").prop("checked", true);
    }
    LimpiarBoleto();
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

    } else {
        $("#chequeElectronico").prop("checked", false);
        $("#chequeElectronicoInput").prop("checked", false);
    }
    if (contrato.DolarizadoExpress == true) {
        $("#dolarizadoDiv").show();
        $("#dolarizadoFechaId").val(FormatearFecha(contrato.Fecha_DolarizadoFormateado));
        $("#dolarizadoExpressId").prop("checked", true);
        $("#chequeElectronicoDiv").hide();
        $("#pagoCbuDiv").hide();


    } else {
        $("#dolarizadoExpressId").prop("checked", false);
    }

    $("#pagoCbu").val(contrato.PagoCBU);


    $("#pagoCbuInput").val(contrato.PagoCBU);



    contrato.PagoDirectoVendedor == true ? $("#pagoDirectoId").prop("checked", true) : $("#pagoDirectoId").prop("checked", false);


    contrato.EstablecimientoPropio == true ? $("#establecimientoPropioId").prop("checked", true) : contrato.EstablecimientoPropio == false ? $("#establecimientoArrendadoId").prop("checked", true) : false;
    if (contrato.TipoNegocioId == 3) {
        $("#contratoId").val(contrato.DatosFijacion.ContratoId);
        $("#datosContrato").show();
        $("#kgscontrato").text(contrato.DatosFijacion.KilosPendiente + "/" + contrato.DatosFijacion.KilosAplicados);
        $("#desdecontrato").text(contrato.DatosFijacion.FechaDesde);
        $("#hastacontrato").text(contrato.DatosFijacion.FechaHasta);
    } else {
        if ($("#buscadorCorredor").val() != "") {
            $("#pagoDirectoDiv").show();
        }
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
    if (contrato.esFason === true) {
        $("#madreId").attr("disabled", true);
        $("#pagosDiv").hide();
    } else {
        $("#madreId").attr("disabled", false);
    }

    contrato.SelCargoMOA === true ? $("#selCargoMOAId").prop("checked", true) : $("#selCargoMOAId").prop("checked", false);
    contrato.SelCargoVendedor === true ? $("#selCargoVendedorId").prop("checked", true) : $("#selCargoVendedorId").prop("checked", false);

    $("#tipoFasonId").data("kendoDropDownList").value(contrato.TipoFasonId);
    $("#operadorId").data("kendoDropDownList").value(contrato.OperadorId);
    $("#posicionFasonId").val(contrato.Posicion);

    LimpiarDescuentos();
    LimpiarCalidades();
    contrato.MercsDeposito == true ? $("#mercsDepositoId").prop("checked", true) : $("#mercsDepositoId").prop("checked", false);

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
                viewModel.Descuentos.remove(this);
            }
        };
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
    var precioPactadoDto = contrato.PreciosPactados;
    $.each(precioPactadoDto, function (key, precio) {
        var precioKendo = {
            Id: 0,
            FechaDesde: FormatearFecha(precio.FechaDesde),
            FechaHasta: FormatearFecha(precio.FechaHasta),
            PrecioVisualizar: kendo.toString(precio.Precio ? Number(precio.Precio) : "", "n2"),
            Precio: precio.Precio,
            MonedaPactadoId: precio.MonedaPactadoId,
            MonedaPactadoDesc: precio.MonedaPactadoDesc,
            ImportePactadoVisualizar: kendo.toString(precio.ImportePactado ? Number(precio.ImportePactado) : "", "n2"),
            ImportePactado: precio.ImportePactado !== null ? precio.ImportePactado : "",
            MonedaImportePactadoId: precio.MonedaImportePactadoId !== null ? precio.MonedaImportePactadoId : "",
            MonedaImportePactadoDesc: precio.MonedaImportePactadoDesc !== null ? precio.MonedaImportePactadoDesc : "",
            Porcentaje: precio.Porcentaje !== null ? precio.Porcentaje : "",
            Borrar: function () {
                viewModel.PrecioPactado.remove(this);
                MostrarTablaPrecioPactado();
            }
        };
        viewModel.PrecioPactado.push(precioKendo);
    });
    MostrarTablaPrecioPactado();
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
    if (!hijo) {

        if (contrato.AperturaPrecios != null && contrato.AperturaPrecios.length != 0) {

            $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 1; }).Importe);
            $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 2; }).Importe);
            $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").max(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Importe);
            $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Importe);
            $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Porcentaje);
            $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 4; }).Importe);
            $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 4; }).Porcentaje);


        }
        InicializarEditarContratoApertura();
        if (Number($("#precioId").val().replace(',', '.')) > 0) {
            GuardarAperturaDePrecio();
        }

        contrato.Compensacion === true ? $("#compensacionId").prop("checked", true) : $("#compensacionId").prop("checked", false);
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
    if (contrato.Estado == 5 && contrato.TipoNegocioId == 3) {
        $("#buscadorCorredor").data("kendoAutoComplete").enable(false);
        $("#buscadorProveedor").data("kendoAutoComplete").enable(false);
        $("#contratoId").data("kendoAutoComplete").enable(false);
        $("#cantidadId").data("kendoNumericTextBox").enable(false);
        $("#precioId").data("kendoNumericTextBox").enable(false);
        $("#precioMonedaId").data("kendoDropDownList").enable(false);
        $("#aperturaPrecioBtn").attr("disabled", true);
        $("#diasDiferidoId").attr("disabled", true);
        $("#diasDiferidoFijacionId").data("kendoNumericTextBox").enable(false);
        $("#comercialFijacionId").data("kendoDropDownList").enable(false);
        //$("#pagoCbuInput").data("kendoAutoComplete").enable(false);

    }

    if (contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 2) {

        $("#AgenteCompraId").data("kendoDropDownList").value(contrato.TipoAgenteCompraId);
        if (contrato.TipoAgenteCompraId > 0) {
            $("#caratulaExtensionId").val(contrato.CaratulaExtension);
            $("#caratulaMATId").val(contrato.CaratulaMAT);
            $("#precioAjusteComisionId").data("kendoNumericTextBox").value(contrato.PrecioAjusteComision);
            $("#monedaAjusteComisionId").data("kendoDropDownList").value(contrato.MonedaAjusteComisionId);
            $("#boletoNingunoId").prop("checked", false);
            $("#boletoNingunoId").click();
            $("#boletoNingunoId").attr("readonly", "readonly");
            $("#boletoConfirmaId").attr("disabled", true);
            $("#boletoFisicoId").attr("disabled", true);
            $("#boletoCartaId").attr("disabled", true);
        }
        if (contrato.ContratoAcuerdoId != null) {
            $("#contratoAcuerdoId").data("kendoAutoComplete").value(contrato.ContratoAcuerdoId);
        }
    }
    if (contrato.TipoNegocioId == 6) {
        $("#AgenteCompraId").data("kendoDropDownList").value(contrato.TipoAgenteCompraId);
        if (contrato.Precio != null) {
            $("#fechaCiertaAcuerdoDiv").show();

        }
    }

}

function LimpiarApertura() {
    var iteraciones = viewModel.AperturaPrecio.length;
    for (i = 0; i < iteraciones; i++) {
        viewModel.AperturaPrecio.pop();
    }
}

function AutocompleteProcedencia() {
    $("#LocalidadCrearContrato").click(function () {
        $("#LocalidadCrearContrato").data("kendoAutoComplete").value("");
        $("#establecimientoDiv").hide();
        $("#LocalidadCrearContrato").trigger("change");
        $("#establecimientoPropioId").prop("checked", false);
        $("#establecimientoArrendadoId").prop("checked", false);
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
        if (prov[0] === "BUENOS AIRES") {
            $("#establecimientoDiv").show();
        } else {
            $("#establecimientoDiv").hide();
        }
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
    var precioPactado = {
        Id: 0,
        FechaDesde: $("#fechaDesdePactado").val(),
        FechaHasta: $("#fechaHastaPactado").val(),
        Precio: Number($("#precioPactado").val().replace(',', '.')),
        PrecioVisualizar: kendo.toString($("#precioPactado").val().replace(',', '.') ? Number($("#precioPactado").val().replace(',', '.')) : "", "n2"),
        MonedaPactadoId: $("#monedaPactadoId").data("kendoDropDownList").value(),
        MonedaPactadoDesc: $("#monedaPactadoId").data("kendoDropDownList").text(),
        ImportePactado: Number($("#importePactado").val().replace(',', '.')),
        ImportePactadoVisualizar: kendo.toString($("#importePactado").val().replace(',', '.') ? Number($("#importePactado").val().replace(',', '.')) : "", "n2"),
        MonedaImportePactadoId: $("#monedaImportePactadoId").data("kendoDropDownList").value(),
        MonedaImportePactadoDesc: $("#monedaImportePactadoId").data("kendoDropDownList").value() != "" ? $("#monedaImportePactadoId").data("kendoDropDownList").text() : "",
        Porcentaje: $("#porcentajePactado").val(),
        Borrar: function () {
            viewModel.PrecioPactado.remove(this);
            MostrarTablaPrecioPactado();
        }
    };

    if (validarGuardarApertura(precioPactado)) {
        if (AperturaPrecioPorcentajeDeComision != null && AperturaPrecioPorcentajeDeComision != '') {
            var num = Number(AperturaPrecioPorcentajeDeComision.replace(',', '.'));
            if ($("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value() > num) {
                MensErr("El Porcentaje de Comision no puede ser mayor a " + AperturaPrecioPorcentajeDeComision);
                return false;
            }
        }
        var total = CalcularPrecioTotalApertura();
        if (total <= 0 && ($("#tipoId").val() == 2 || $("#tipoId").val() == 3 || $("#tipoId").val() == 6) && !$("#pizarraId").is(':checked')) {
            MensErr("El Precio Total no puede ser menor o igual a 0");
        } else {
            InsertarAperturasViewModel(total);
        }
    } else {
        AgregarPrecioPactado();
    }
}

function InsertarAperturasViewModel(total) {
    var Financiero = {
        Id: 0,
        ConceptoAperturaPrecioId: 1,
        Importe: $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value()
    };
    var Redespacho = {
        Id: 0,
        ConceptoAperturaPrecioId: 2,
        Importe: $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value()
    };
    var Comisiones = {
        Id: 0,
        ConceptoAperturaPrecioId: 3,
        Importe: $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(),
        Porcentaje: Number($("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value())
    };
    var Bonificaciones = {
        Id: 0,
        ConceptoAperturaPrecioId: 4,
        Importe: $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(),
        Porcentaje: Number($("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value())
    };

    viewModel.AperturaPrecio = [];
    viewModel.AperturaPrecio.push(Financiero);
    viewModel.AperturaPrecio.push(Redespacho);
    viewModel.AperturaPrecio.push(Comisiones);
    viewModel.AperturaPrecio.push(Bonificaciones);
    $('#modalAperturaPrecio').modal('hide');

    $("#precioTotalApertura").data("kendoNumericTextBox").value(total);
    $("#precioTotalApertura").trigger("change");

    if (Financiero.Importe != 0 || Redespacho.Importe != 0 || Financiero.Importe != 0 || Comisiones.Importe != 0 || Bonificaciones.Importe != 0 || Comisiones.Porcentaje != 0 || Bonificaciones.Porcentaje != 0) {
        $("#NivelTarifaId").data("kendoDropDownList").enable(false);
        $("#TarifaFleteId").data("kendoNumericTextBox").enable(false);
    } else {
        $("#NivelTarifaId").data("kendoDropDownList").enable(true);
        $("#TarifaFleteId").data("kendoNumericTextBox").enable(true);
    }
}


function InicializarAperturaDePrecios() {
    $("#precioTotalApertura").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    $("#precioTotalApertura").data("kendoNumericTextBox").readonly();
    $("#precioTotalApertura").data("kendoNumericTextBox").enable(false);

    $("#aperturaPrecioBtn").click(function () {
        AbrirModalAperturaDePrecio();
    });

    $(".aperturaPrecioInput").change(function () {
        CalcularPrecioTotalApertura();
    });

    $("#guardarAperturaPrecio").click(function () {
        GuardarAperturaDePrecio();
    });

    $("#precioId").change(function () {
        var total = CalcularPrecioTotalApertura();
        $("#precioTotalApertura").data("kendoNumericTextBox").value(total);

        SetearValoresMaximosApertura();
        if ($("#tipoId").data("kendoDropDownList").value() == "6") {
            if ($("#precioId").val() == "" || $("#precioId").val() == "0") {

                $("#pesificadoId").prop("checked", false);
                $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
                $("#CDId").prop("checked", false);
                $("#WarrantId").prop("checked", false);
                $(".contratoAFijar").show();
                $("#pagoDiferidoDiv").hide();
                $("#pagosDiv").hide();
                $("#pesificadoDiv").hide();
                $(".madreDiv").hide();
                $("#FijacionDesdeHastaDiv").show();
                $("#condicionFijacionDiv").show();
                $(".acuerdoSinPrecio").show();

                $("#pagoDolarizadoDiv").hide();
                $("#dolarizadoDiv").hide();
                $("#dolarizadoId").prop("checked", false);
                $("#dolarizadoFechaId").data("kendoDatePicker").value("");
                $("#ocultarAperturaBtn").hide();
                $("#ocultarAperturaMoneda").removeClass("w70");
                $("#ocultarAperturaMoneda").addClass("w100");
                //$("#fechaCiertaAcuerdo").data("kendoDatePicker").value("");
            } else {
                $("#ocultarAperturaBtn").show();
                $("#ocultarAperturaMoneda").removeClass("w100");
                $("#ocultarAperturaMoneda").addClass("w70");
                $("#pagosDiv").show();
                $(".acuerdoSinPrecio").hide();
                $("#precioMonedaId").data("kendoDropDownList").trigger("change");
            }
        }

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

    $("#aperturaPrecioImporteFinancieroId").change(function () { PonerEnCeroSiEsNulo("aperturaPrecioImporteFinancieroId"); });
    $("#aperturaPrecioImporteComisionesId").change(ModificarComisionesPorImporte);
    $("#aperturaPrecioPorcentajeComisionesId").change(ModificarComisionesPorcentaje);
    $("#aperturaPrecioImporteRedespachoId").change(ConvertirRedespachoANegativo);

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
}

function AbrirModalAperturaDePrecio() {
    if (viewModel.AperturaPrecio.length > 0) {
        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[0].Importe);
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[1].Importe);
        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[2].Porcentaje);
        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[2].Importe);
    } else {
        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(0);
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(0);
        //$("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
        $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value(0);
    }

    CalcularMaximoComision();
    var moneda = "";
    if ($("#precioMonedaId").val()) {
        $(".aperturaprecioMoneda").text($("#precioMonedaId").data("kendoDropDownList").text());
        moneda = $("#precioMonedaId").data("kendoDropDownList").text();
    } else {
        $(".aperturaprecioMoneda").text("");
    }
    $("#precioAperturaOriginal").text(kendo.toString($("#precioId").val().replace(',', '.') ? Number($("#precioId").val().replace(',', '.')) : Number(0), "n2") + " " + moneda);
    CalcularPrecioTotalApertura();
    $("#modalAperturaPrecio").modal("show");
}

function CalcularPrecioTotalApertura() {
    var bonificacion = Number($("#precioId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
    var precioOriginal = Number($("#precioId").val().replace(',', '.'));

    if ((ImporteSobrePrecio != 0 || PorcentajeSobrePrecio > 0) && Number($("#precioId").val().replace(',', '.')) > 0) {
        precioOriginal = CalcularNetoFijacionConDescuentos();
    } else {
        var porcentajeComision = Math.min(Number($("#aperturaPrecioPorcentajeComisionesId").val().replace(',', '.')), $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").max());
        var precioTarifaFlete = Number($("#TarifaFleteId").val().replace(',', '.'));
        precioOriginal += Math.min(Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')), Number($("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max()));
        precioOriginal += Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
        CalcularMaximoComision();
        precioOriginal += Number($("#aperturaPrecioImporteBonificacionesId").val().replace(',', '.'));

        precioOriginal += Number($("#aperturaPrecioPorcentajeBonificacionesId").val().replace(',', '.')) * Number($("#precioId").val().replace(',', '.')) / 100;

        porcentajeComision = porcentajeComision / 100;
        precioOriginal += (precioOriginal * porcentajeComision) - precioTarifaFlete;
        precioOriginal += Number($("#aperturaPrecioImporteComisionesId").val().replace(',', '.'));

        $("#totalApertura").text(kendo.toString(precioOriginal, "n2") + " " + ($("#precioMonedaId").val() ? $("#precioMonedaId").data("kendoDropDownList").text() : ""));
    }


    return precioOriginal;
}

function CalcularMaximoComision() {
    var maximoBonificacion = (Number($("#precioId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'))) * 0.01;
    var comisionesTextBox = $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox");
    if (comisionesTextBox) {
        comisionesTextBox.max(maximoBonificacion);
        if (Number($("#aperturaPrecioImporteComisionesId").val().replace(',', '.')) > comisionesTextBox.max()) {
            comisionesTextBox.value(comisionesTextBox.max());
        }
    }
}

function ModificarComisionesPorImporte() {
    PonerEnCeroSiEsNulo("aperturaPrecioImporteComisionesId");
    $("#aperturaPrecioPorcentajeComisionesId").val(0);
    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
    CalcularPrecioTotalApertura();
}
function ModificarComisionesPorcentaje() {
    PonerEnCeroSiEsNulo("aperturaPrecioPorcentajeComisionesId");
    $("#aperturaPrecioImporteComisionesId").val(0);
    $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(0);
    CalcularPrecioTotalApertura();
}
function ConvertirDescuentoANegativo() {
    if ($("#tipoId").val() == 1 && $("#destinoId").val() != 1) {
        var valorAbsoluto = Math.abs($("#ImporteDescuentoId").val().replace(',', '.'));
        $("#ImporteDescuentoId").data("kendoNumericTextBox").value(-1 * valorAbsoluto);
    }
}
function ConvertirRedespachoANegativo() {
    var valorAbsoluto = Math.abs($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
    if (valorAbsoluto > Number($("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").max())) {
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(-1 * $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").max());
    } else {
        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(-1 * valorAbsoluto);
    }
    CalcularPrecioTotalApertura();
}
function PonerEnCeroSiEsNulo(elemento) {
    if (!$("#" + elemento).val()) {
        $("#" + elemento).val(0);
    }
}

function SetearValoresMaximosApertura() {
    $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max(Number($("#precioId").val().replace(',', '.')));
    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").max(Math.abs(Number($("#precioId").val().replace(',', '.'))));
    $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").min(-1 * Math.abs(Number($("#precioId").val().replace(',', '.'))));
}

function InicializarEditarContratoApertura() {
    SetearValoresMaximosApertura();
    CalcularPrecioTotalApertura();
}

function SetearComisionCorredor() {
    $("#aperturaPrecioPorcentajeComisionesId").val(0);
    $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
    InsertarAperturasViewModel(CalcularPrecioTotalApertura());
}

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

function AgregarPrecioPactado() {
    var precioPactado = {
        Id: 0,
        FechaDesde: $("#fechaDesdePactado").val(),
        FechaHasta: $("#fechaHastaPactado").val(),
        Precio: Number($("#precioPactado").val().replace(',', '.')),
        PrecioVisualizar: kendo.toString($("#precioPactado").val().replace(',', '.') ? Number($("#precioPactado").val().replace(',', '.')) : "", "n2"),
        MonedaPactadoId: $("#monedaPactadoId").data("kendoDropDownList").value(),
        MonedaPactadoDesc: $("#monedaPactadoId").data("kendoDropDownList").text(),
        ImportePactado: Number($("#importePactado").val().replace(',', '.')),
        ImportePactadoVisualizar: kendo.toString($("#importePactado").val().replace(',', '.') ? Number($("#importePactado").val().replace(',', '.')) : "", "n2"),
        MonedaImportePactadoId: $("#monedaImportePactadoId").data("kendoDropDownList").value(),
        MonedaImportePactadoDesc: $("#monedaImportePactadoId").data("kendoDropDownList").value() != "" ? $("#monedaImportePactadoId").data("kendoDropDownList").text() : "",
        Porcentaje: $("#porcentajePactado").val(),
        Borrar: function () {
            viewModel.PrecioPactado.remove(this);
            MostrarTablaPrecioPactado();
        }
    };

    var err = ValidarPrecioPactado(precioPactado);
    if (ExistsErrorMessages(err)) {
        MensErr(err[0]);
    }
    else {
        viewModel.PrecioPactado.push(precioPactado);
        $("#fechaDesdePactado").data("kendoDatePicker").value("");
        $("#fechaHastaPactado").data("kendoDatePicker").value("");
        $("#precioPactado").data("kendoNumericTextBox").value("");
        $("#importePactado").data("kendoNumericTextBox").value("");
        $("#monedaImportePactadoId").data("kendoDropDownList").value("");
        $("#porcentajePactado").data("kendoNumericTextBox").value("");
        MostrarTablaPrecioPactado();
    }

    function ValidarPrecioPactado(precioPactado) {
        var errores = [];
        if (precioPactado.Precio === "" || precioPactado.Precio === 0) {
            errores.push("El Precio no debe ser vacio o 0");
        }
        if (precioPactado.FechaDesde === "" || precioPactado.FechaDesde === undefined) {
            errores.push("La Fecha Desde no debe ser vacia");
        }
        if (precioPactado.FechaHasta === "" || precioPactado.FechaHasta === undefined) {
            errores.push("La Fecha Hasta no debe ser vacia");
        }
        if (precioPactado.ImportePactado > 0 && (precioPactado.MonedaImportePactadoId === "" || precioPactado.MonedaImportePactadoId === undefined)) {
            errores.push("La Moneda no debe ser vacia cuando hay Importe");
        }
        if ((precioPactado.ImportePactado == 0 || precioPactado.ImportePactado == "" || precioPactado.ImportePactado === undefined) && (precioPactado.MonedaImportePactadoId != "")) {
            errores.push("El Importe no debe ser vacia cuando seleciono Moneda");
        }
        if (kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") > kendo.parseDate(precioPactado.FechaHasta, "dd-MM-yyyy")) {
            errores.push("La Fecha Desde no debe ser mayor a Fecha Hasta");
        }
        if (kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") < kendo.parseDate($("#fechaDesdeId").val(), "dd-MM-yyyy")
            || kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") > kendo.parseDate($("#fechaHastaId").val(), "dd-MM-yyyy")) {
            errores.push("La Fecha Desde debe estar dentro de los rangos de entrega");
        }
        if (kendo.parseDate(precioPactado.FechaHasta, "dd-MM-yyyy") < kendo.parseDate($("#fechaDesdeId").val(), "dd-MM-yyyy")
            || kendo.parseDate(precioPactado.FechaHasta, "dd-MM-yyyy") > kendo.parseDate($("#fechaHastaId").val(), "dd-MM-yyyy")) {
            errores.push("La Fecha Hasta debe estar dentro de los rangos de entrega");
        }
        var i = viewModel.PrecioPactado.length - 1;
        if (i >= 0 && kendo.parseDate(precioPactado.FechaDesde, "dd-MM-yyyy") <= kendo.parseDate(viewModel.PrecioPactado[i].FechaHasta, "dd-MM-yyyy")) {
            errores.push("La Fecha Desde no debe ser menor a la Fecha Hasta anterior");
        }

        var rangos = MSExecuteOnServer('/CompraNet/ObtenerRangoDePrecios', { materialId: $("#material").val(), monedaId: $("#precioMonedaId").val() });
        if (Number($("#precioPactado").val().replace(',', '.')) < rangos.PrecioMinimo) {
            errores.push("Precio por fuera del rango de Precio Mínimo");
        }
        if (Number($("#precioPactado").val().replace(',', '.')) > rangos.PrecioMaximo) {
            errores.push("Precio por fuera del rango de Precio Máximo");
        }
        return errores;
    }
}
function MostrarTablaPrecioPactado() {
    if (viewModel.PrecioPactado.length > 0) {
        $(".tabla-pactados").show();
    } else {
        $(".tabla-pactados").hide();
    }
}

function FormatearFecha(fecha) {
    if (fecha != null && fecha != undefined && fecha != "") {
        var dias = fecha.split('-');
        var dia = dias[0];
        var mes = dias[1];
        var anio = dias[2];
        if (Number(dia) < 10) {
            dia = "0" + dia;
        }
        if (Number(mes) < 10) {
            mes = "0" + mes;
        }
        return dia + '-' + mes + '-' + anio;
    }
}

function CalcularNetoFijacionConDescuentos() {
    var ImporteSobrePrecioMonedaIgual = ImporteSobrePrecio;
    if (ImporteSobrePrecio != 0 || PorcentajeSobrePrecio != 0) {
        if ($.trim(MonedaSobrePrecio) != $.trim($("#precioMonedaId").val())) {
            var valorDolar = MSExecuteOnServer('/CompraNet/TraerTipoDeCambio', {});
            console.log("valorDolar", valorDolar);
            if ($.trim(MonedaSobrePrecio) == "ARP") {
                ImporteSobrePrecioMonedaIgual = ImporteSobrePrecio / valorDolar;
            } else {
                ImporteSobrePrecioMonedaIgual = ImporteSobrePrecio * valorDolar;
            }
        }
        var costoFinanciero = Math.min(Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')), Number($("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max()));
        var precioN = Number($("#precioId").val().replace(',', '.'));
        console.log("precio base:", precioN);
        console.log("costoFinanciero", costoFinanciero);
        console.log("ImporteSobrePrecio", ImporteSobrePrecio);
        console.log("ImporteSobrePrecioMonedaIgual", ImporteSobrePrecioMonedaIgual);
        console.log("PorcentajeSobrePrecio", PorcentajeSobrePrecio);
        var desc = ((precioN + ImporteSobrePrecioMonedaIgual + costoFinanciero) * PorcentajeSobrePrecio / 100);
        console.log("descuento", desc);
        var totalNeto = precioN + ImporteSobrePrecioMonedaIgual + costoFinanciero + desc;
        console.log("totalNeto", totalNeto);
        $("#precioTotalApertura").data("kendoNumericTextBox").value(totalNeto);
        return totalNeto;
    }
}
function HayCompensacion() {
    if (!$("#compensacionId").is(":checked")) {
        if (!$("#pagoDirectoId").is(":checked")) {
            $("#chequeElectronicoDiv").show();
        }
        if ($("#buscadorCorredor").val() == "" && $("#AgenteCompraId").val() == "" && !$("#chequeElectronicoInput").is(":checked")) {
            $("#pagoCbuDiv").show();
        }

    }
    else {
        $("#chequeElectronicoInput").prop("checked", false);
        $("#chequeElectronicoDiv").hide();
        $("#pagoCbuDiv").hide();
    }
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





