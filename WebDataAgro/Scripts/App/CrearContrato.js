var viewModel;
var datosIniCrearContrato;
var contratoEdit;
var contratoId;
var fijacionId;
var fasonId;

var posicionFijacion;

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
    var ultimoDia = new Date(anio, hoy.getMonth()+1, 0).getDate();

    if (dia == 1){
        dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
        mesPost = hoy.getMonth() + 1;
    }
    if (dia == ultimoDia) {
        dia = new Date(anio, mesPost, 0).getDate();        
    }
    if (mesPost == 13) {
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

    $(".datos-adicionales").hide();
    $(".datos-boleto").hide();
    $(".datos-establecimiento").hide();
    $(".datos-topesplazos").hide();
    $(".datos-calidades").hide();
    $(".datos-descuentos").hide();

    $("#buscadorProveedor").click(function () {
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
            $("#datosContrato").hide();
            InicializarBordesRojos();
        },
        select: function (e) {
            if ($("#tipoId").val() != 3 && (contratoId == 0 || contratoId == null || contratoId == "")) {
                $("#clasificacion").data("kendoDropDownList").value("");
                $("#consignatarioId").prop("checked", false);
            }
            var compraNet = MSExecuteOnServer('/CompraNet/ObtenerDatosCompraNet', { id: e.dataItem.Id });
            $("#clasificacion").data("kendoDropDownList").value(compraNet.ClasificacionCompraNetId);
            $("#clasificacion").data("kendoDropDownList").trigger("change");
            if ($("#clasificacion").val() == 2) {
                $("#consignatarioId").prop("checked", compraNet.Consignatario);
            }
            if (compraNet.LocalidadId != null) {
                if (compraNet.LocalidadId != "" && compraNet.ProvinciaId != "") {
                    $("#LocalidadCrearContrato").val(compraNet.Localidad + " (" + compraNet.Provincia + ")");
                    HabilitarEstablecimiento();
                } else {
                    $("#LocalidadCrearContrato").val("");
                    HabilitarEstablecimiento();
                }
            }
            if ($("#buscadorCorredor").val() === "") {
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
                    return { filtro: cuit[0], filtroProveedor: $('#buscadorProveedor').val(), corredor: false };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });
    

    $("#buscadorCorredor").click(function() {
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
                if ($("#tipoId").val() != 3 && (contratoId == 0 || contratoId == null || contratoId == "")) {
                    $("#clasificacion").data("kendoDropDownList").value("");
                    $("#consignatarioId").prop("checked", false);
                }
            }
            if ($("#buscadorCorredor").val() == "") {
                $("#porcentajeComisionDiv").hide();
                $("#contCorredorId").val("");
                $("#contCorredorDiv").hide();
                $("#pagoDirectoDiv").hide();
            }
            $("#buscadorProveedor").val("");
            $("#contratoId").val("");
            $("#datosContrato").hide();
        },
        select: function (e) {
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
            $('#pagoDirectoDiv').show();
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
                    return { filtro: $('#buscadorCorredor').val(), corredor: true};
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
        template: '<p class="buscar-nomb"><strong>#: data.ContratoId#</strong> - ' +
            ' Kgs Pendientes: #: data.KilosPendiente# - Kgs Aplicados: #: data.KilosAplicados#' +
            ' - Desde: #: data.FechaDesde# - Hasta: #: data.FechaHasta# </p> ',
        dataTextField: "Filtro",
        dataValueField: "ContratoId",
        autoWidth: true,
        type: "number",
        filter: "contains",
        change: function () {
            if ($("#contratoId").val().split('|').length > 1) {
                $("#contratoId").val($("#contratoId").val().split('|')[1]);
            }
        },
        select: function (e) {
            $("#datosContrato").show();
            $("#kgspendientescontrato").text(e.dataItem.KilosPendiente);
            $("#kgsaplicadoscontrato").text(e.dataItem.KilosAplicados);
            $("#desdecontrato").text(e.dataItem.FechaDesde);
            $("#hastacontrato").text(e.dataItem.FechaHasta);

            $("#posicionFasonId").val(e.dataItem.Posicion);
            $("#fechaDesdeId").val(e.dataItem.DesdeEntrega);
            $("#fechaHastaId").val(e.dataItem.HastaEntrega);
            $("#campanaId").data("kendoDropDownList").text(e.dataItem.Campana);
            e.dataItem.Calidad === true ? $("#trigoEspecialId").prop("checked", true) : $("#trigoEspecialId").prop("checked", false);
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
                    return { CuitProveedor: cuitP[0], CuitCorredor: cuitC[0], materialId: $('#material').data("kendoDropDownList").value(), filtro: $('#contratoId').val()};
                }
            }

        }        
    });
    $('#contratoId').click(function (e) {
        $('#contratoId').val("");
        $("#contratoId").data("kendoAutoComplete").search("");  
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

    $(".contratoAFijar").hide();
    $(".contratoAPrecio").hide();

    $("#tipoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId",
        change: function () {
            $(".contratoAFijar").hide();
            $(".contratoAPrecio").hide();
            $("#pagosDiv").hide();
            $(".contratoMadreDiv").hide();
            $("#contMadreId").val("");
            $("#madreId").prop("checked", false);
            $("#hijoId").prop("checked", false);
            $("#proveedorLabel").html("Proveedor");
            $(".fasonero").addClass("col-sm-1");
            $(".fasonero").removeClass("col-sm-2");
            $(".fason").hide();
            if (this.value() == 3) {
                $("#fechasDiv").hide();
                $("#fechaDesdeDiv").hide();
                $("#fechaHastaDiv").hide();
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
                RemoverFondosGrises();
                $("#boton-ampliar").hide();
                $("#corredorDiv").show();
            } else if (this.value() == 4) {
                $(".noFason").hide();
                $(".fason").show();
                $("#proveedorLabel").html("Fasonero");
                $(".fasonero").removeClass("col-sm-1");
                $(".fasonero").addClass("col-sm-2");
                $("#campanaDiv").show;
                $("#boton-ampliar").hide();
                $("#corredorDiv").hide();
                $("#guardarBtn").empty();
                $("#guardarBtn").append("Guardar Fasón");
                if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("USDM ");
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
                $("#ComercialDiv").hide();
                $("#guardarBtn").empty();
                $("#DatosBoleto").show();
                $("#guardarBtn").append("Guardar Negocio");
                $("#boton-ampliar").show();
                $("#mercsDepositoDiv").show();
                $("#corredorDiv").show();
                RemoverFondosGrises();
                $("#boton-ampliar").trigger("click");
                $("#boton-ampliar").trigger("click");
                if (this.value() == 1) {
                    if ($("#boton-ampliar").text() == "+ AMPLIAR") {
                        $(".contratoAFijar").hide();
                    }
                    else {
                        $(".contratoAFijar").show();
                    }
                    $(".contratoAPrecio").hide();
                    $("#CDId").prop("checked", false);
                    $("#WarrantId").prop("checked", false);
                    InicializarFondosGrises();
                }
                if (this.value() == 2) {
                    $("#CDId").prop("checked", false);
                    $("#WarrantId").prop("checked", false);
                    $("#pagosDiv").show();
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
                }
                if ($("#boton-ampliar").text() == "+ AMPLIAR") {
                    $(".ampliar").hide();
                }
            }
        }
    });
    $("#madreId").click(function () {
        if ($(this).is(':checked')) {
            $("#pagosDiv").show();
        }
        else {
            $("#pagosDiv").hide();
            $("#CDId").prop("checked", false);
            $("#WarrantId").prop("checked", false);
        }
    });
    $("#hijoId").click(function () {
        if ($(this).is(':checked')) {
            $(".contratoMadreDiv").show();
        }
        else {
            $(".contratoMadreDiv").hide();
            $("#contMadreId").val("");
        }
    });
    $("#tipoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoId").data("kendoDropDownList");
            dropdownlist.text("");
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
            $("#contratoId").val("");
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
    function validarFechaCampana(){
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
            if (this.value() == 2) {
                $("#consignatarioDiv").show();
            } else {
                $("#consignatarioDiv").hide();
                $("#consignatarioId").prop("checked", false);
            }
            if (this.value() != 2 && this.value() != 3) {
                $("#planCanjeDiv").hide();
                $("#planCanjeId").prop("checked", false);
            } else {
                $("#planCanjeDiv").show();
                
            }

            $("#clasificacion").trigger('change');
        }
    });

    $("#clasificacion").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacion").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#clasificacionModalPendienteId").kendoDropDownList({
        optionLabel: "CLASIFICACIÓN...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#clasificacionModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#clasificacionModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#destinoId").kendoDropDownList({
        optionLabel: "SELECCIONE EL DESTINO...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
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

    var bolsaConfirma = $("#bolsaConfirmaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaConfirmaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#bolsaConfirmaIdModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#bolsaConfirmaIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaConfirmaIdModalPendiente").data("kendoDropDownList");
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
    $("#bolsaFisicoIdModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE BOLSA...",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });
    $("#bolsaFisicoIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#bolsaFisicoIdModalPendiente").data("kendoDropDownList");
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
        dataValueField: "Id",
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
        dataValueField: "Id"
    });

    $("#calidadesEspecialesId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#calidadesEspecialesId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#cantidadId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        change: function () {
            if ($("#cargarCantidadCamiones").is(':checked')) {
                $("#cantidadCamionesId").data("kendoNumericTextBox").value(Math.ceil(this.value() / 30000));
            }
        }
    });

    $("#cargarCantidadCamiones").change(function () {
        if($("#cargarCantidadCamiones").is(':checked')) {
            $("#cantidadId").data("kendoNumericTextBox").trigger("change");
        }
        else {
            $("#cantidadCamionesId").data("kendoNumericTextBox").value('');
        }
    });

    $("#cantidadCamionesId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });
    $("#precioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#sustentablePrecioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#contMadreId").change(function () {
        var sap = $("#contMadreId").val();
        var datos = { sap };
        contratoEdit = MSExecuteOnServer('/CompraNet/TraerContratoMadre', datos);
        if (ExistsErrorMessages(contratoEdit.Errores)) {
            ShowErrorMessages(contratoEdit.Errores);
        } else {
            CargarDatosEditar(contratoEdit.Contrato, true);
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
        value:1,
        min: 0,
        max:100
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
        
    var date = ObtenerFechaDesde();
    var datehasta = ObtenerFechaHasta();

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

    $("#fechaOperacionId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () { $("#fechaHastaTopeId").val(ObtenerFechaHasta(this.value()));}
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

    $("#fechaDesdeId").val(date);
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
        }
        else {
            $(".sustentableDiv").hide();
            $("#sustentablePrecioId").data("kendoNumericTextBox").value("");
        }
    });

    $("#dolarizadoId").click(function () {
        if ($(this).is(':checked')) {
            $("#dolarizadoDiv").show();
        }
        else {
            $("#dolarizadoDiv").hide();
            $("#dolarizadoFechaId").val("");
        }
    });

    $("#pesificadoId").click(function () {
        if ($(this).is(':checked')) {
            $("#pesificadoDiv").show();
        }
        else {
            $("#pesificadoDiv").hide();
            $("#pesificadoDiasId").data("kendoNumericTextBox").value("");
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
        $("#WarrantId").prop("checked", false);
        $("#pagoDirectoId").prop("checked", false);
    });

    $("#WarrantId").click(function () {
        $("#CDId").prop("checked", false);
        $("#pagoDirectoId").prop("checked", false);
    });

    $("#pagoDirectoId").click(function () {
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
        if ($("#boton-ampliar").text() == "+ AMPLIAR") {
            $("#boton-ampliar").text("- OCULTAR");
            $(".ampliar").show();
            if ($('#tipoId').val() == 1) {
                $(".contratoAFijar").show();
                $(".contratoAPrecio").hide();
                $("#pagosDiv").hide();
                if ($("#madreId").is(':checked')) {
                    $("#pagosDiv").show();
                }
            } else if ($('#tipoId').val() == 2) {
                $(".contratoAFijar").hide();
                $(".contratoAPrecio").show();
                $("#pagosDiv").show();
            } 
        }
        else if ($("#boton-ampliar").text() == "- OCULTAR") {
            $("#boton-ampliar").text("+ AMPLIAR");
            $(".ampliar").hide();
            $(".ampliar-adicionales").hide();
            if ($('#tipoId').val() == 1) {
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
        } else {
            $("#condicionFijacionId").data("kendoDropDownList").value("");
            $("#fechaDesdeTopeId").val("");
            $("#fechaHastaTopeId").val("");
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
        change: function () { $("#fechaHastaDescuentoId").val(ObtenerFechaHasta(this.value())) }
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
    $("#trigoEspecialId").click(function () {
        if ($(this).is(':checked')) {
            $("#especialesId").show();
            $("#agregarCalidad").show();
        }
        else {
            $("#especialesId").hide();
            $("#agregarCalidad").hide();
            $("#calidadesEspecialesId").data("kendoDropDownList").value("");
            $("#porcentajeDesdeDiv").hide();
            $("#porcentajeHastaDiv").hide();            
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
}
function CargarCalidadPorMaterial(value) {
    var calidadGrano = MSExecuteOnServer('/CompraNet/TraerCalidadesPorMaterial', { MaterialId: value });
    viewModel.set("EspecialesCombo", calidadGrano); 

    $('#calidadesEspecialesId').change(function () {     
        $("#porcentajeDesdeDiv").hide();
        $("#porcentajeHastaDiv").hide();
        for (var i in calidadGrano) {
            if (calidadGrano[i].Id == $('#calidadesEspecialesId').val() && calidadGrano[i].PermiteRango == true) {
                $("#porcentajeDesdeDiv").show();
                $("#porcentajeHastaDiv").show();
                break;
            }  
        }
    });
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
        "planCanjeId": null,
        "consignatarioId": null,
        "cdId": null,
        "warrantId": null,
        "pagoDirectoId": null,
        "standardCalidadId": null,
        "calidadesEspecialesId": null,
        "tipoPeriodoDBId": null,
        "mercsDepositoId":null,

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
        "ContratosPendientes":null
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
        TipoPeriodoDBCombo: [],
        TipoDBCombo: [],

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
        StandardComboModalPendiente: [],
        EspecialesComboModalPendiente: [],
        isControlDisabled: true,
        Descuentos: [],
        Calidades:[],
        DescuentosVisualizar: [],
        CalidadesVisualizar: [],
        ContratosPendientes: []
    });

    kendo.bind($("#CrearContrato"), viewModel);
    kendo.bind($("#CompraNet"), viewModel);
    kendo.bind($("#modalPendienteDiv"), viewModel);
    kendo.bind($("#tabla-descuentos"), viewModel);
    kendo.bind($("#tabla-descuentos-visualizar"), viewModel);
    kendo.bind($("#tabla-calidades"), viewModel);
    kendo.bind($("#tabla-calidades-visualizar"), viewModel);
    kendo.bind($("#tabla-pendientes"), viewModel);
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

        if (contratoId != undefined && contratoId) {
            setTimeout(InicializarContratoEdit, 300);
        } else if (fijacionId != undefined && fijacionId) {
            setTimeout(InicializarFijacionEdit, 300);
        } else if (fasonId != undefined && fasonId) {
            setTimeout(InicializarFasonEdit, 300);
        } else {
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
    viewModel.set("CondicionFijacionCombo", datosIniCrearContrato.Datos.Condicion);
    viewModel.set("StandardCombo", datosIniCrearContrato.Datos.Standard);
    viewModel.set("tipoFasonCombo", datosIniCrearContrato.Datos.TipoFason);

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
    if ($("#precioMonedaId").data("kendoDropDownList")) $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
    if ($("#comercialId").data("kendoDropDownList")) $("#comercialId").data("kendoDropDownList").value(comercialId);
    if ($("#comercialFijacionId").data("kendoDropDownList")) $("#comercialFijacionId").data("kendoDropDownList").value(comercialId);
    if ($("#material").data("kendoDropDownList")) $("#material").data("kendoDropDownList").value("3");
    if ($("#sustentableMonedaId").data("kendoDropDownList")) $("#sustentableMonedaId").data("kendoDropDownList").value("USDM ");
    if ($("#campanaId").data("kendoDropDownList")) CargarCampaniaPorMaterial("3");
    if ($("#destinoId").data("kendoDropDownList")) $("#destinoId").data("kendoDropDownList").value("1");
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
}

function ObtenerDatos() {
    var obj = {};

    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth();
    var dia = hoy.getDate();
    var hora = hoy.getHours();
    var minuto = hoy.getMinutes();

    obj.ContratoId = contratoId == undefined ? 0 : contratoId;
    obj.FijacionDePrecioContratoId = fijacionId == undefined ? 0 : fijacionId;
    obj.Id = fasonId == undefined ? 0 : fasonId;
    obj.MaterialId = $("#material").val();
    obj.TipoNegocioId = $("#tipoId").val();
    obj.Cantidad = $("#cantidadId").val();
    obj.Precio = $("#precioId").val();
    obj.FechaEntrega = $("#fechaHastaId").val();
    obj.CampanaId = $("#campanaId").val();
    obj.FechaDesde = $("#fechaDesdeId").val();
    obj.FechaHasta = $("#fechaHastaId").val();
    obj.MonedaId = $("#precioMonedaId").val();
    obj.Fecha = $("#fechaOperacionId").val() == "" ? hoy : $("#fechaOperacionId").val();
    if (obj.TipoNegocioId == "3") {
        obj.ComercialId = $("#comercialFijacionId").val();
        obj.ContratoSAP = $("#contratoId").val();
    }

    obj.ComercialId = $("#comercialId").val();
    obj.ComercialCreadorId = $("#comercialCreador").val();
    obj.Base = $("#baseId").is(":checked") ? true : false;
    obj.ImporteSustentable = $("#sustentablePrecioId").val();
    obj.MonedaSustentableId = $("#sustentableMonedaId").val();
    obj.FechaDolarizado = $("#dolarizadoFechaId").val();
    obj.DiasPesificado = $("#pesificadoDiasId").val();
    obj.PorcentajeComision = $("#porcentajeComision").val() != "" ? $("#porcentajeComision").val():0;
    obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.TrigoEspecial = $("#trigoEspecialId").is(":checked") ? true : false;
    obj.EstadoId = $("#baseId").is(":checked") ? "3" : obj.TipoNegocioId != 4 ? "1": "2";
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
    obj.MercsDeposito =  $("#mercsDepositoId").is(":checked") ? true : false;
    
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
        var cuit = cuitAux[1].split(')');
        var proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0], corredor:false });
    }
    if ($("#buscadorCorredor").val() != "") {
        var cuitAuxC = $("#buscadorCorredor").val().split('(');
        var cuitC = cuitAuxC[1].split(')');
        var corredorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuitC[0], corredor:true });
    }
    obj.ProveedorId = proveedorId;
    obj.CorredorId = corredorId;

    if ($("#LocalidadCrearContrato").val() != "") {
        var localidadAux = $("#LocalidadCrearContrato").val().split('(');
        var provinciaAux = localidadAux[1].split(')');
        var Localidad = MSExecuteOnServer('/CompraNet/ObtenerLocalidadId', { localidad: localidadAux[0], provincia: provinciaAux[0] });
        obj.LocalidadId = Localidad.LocalidadId;
        obj.ProvinciaId = Localidad.ProvinciaId;
    }
    obj.ContratoVendedor = $("#contVendedorId").val();
    obj.ContratoCorredor = $("#contCorredorId").val();
    obj.SelCargoVendedor = $("#selCargoVendedorId").is(":checked") ? true : false;
    obj.SelCargoMOA = $("#selCargoMOAId").is(":checked") ? true : false;
    obj.FasoneroId = proveedorId;
    obj.Posicion = $("#posicionFasonId").val();
    obj.TipoFasonId = $("#tipoFasonId").val();

    if ($("#madreId").is(":checked")) {
        obj.Madre = true;
    }
    if ($("#hijoId").is(":checked")) {
        obj.Madre = false;
    }
    obj.ContratoMadre = $("#contMadreId").val();
    obj.Descuentos = viewModel.Descuentos;
    obj.Calidad = viewModel.Calidades;   

    GrabarContrato(obj);
}

function GrabarContrato(nuevoContrato) {
    var result;

    if (nuevoContrato.TipoNegocioId != 3 && nuevoContrato.TipoNegocioId != 4) {
        result = MSExecuteOnServer('/CompraNet/GrabarContrato', nuevoContrato);
    } else if (nuevoContrato.TipoNegocioId == 4) {
        result = MSExecuteOnServer('/CompraNet/GrabarFason', nuevoContrato);
    }
    else {
        var kilosPendientes = $("#kgspendientescontrato").text().replace('.','');
        if (nuevoContrato.Cantidad > parseInt(kilosPendientes)) {
            MensErr("La cantidad excede los kilos pendientes de fijar");
            $.unblockUI();
        } else {
            result = MSExecuteOnServer('/CompraNet/GrabarFijacion', nuevoContrato);
        }
    }

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
            $.unblockUI();
        }
        else {
            window.location.href = window.location.origin + "/CompraNet";
        }
    }
    $.unblockUI();
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
        Importe: $("#ImporteDescuentoId").val() !== null && $("#ImporteDescuentoId").val() !== "" ? $("#ImporteDescuentoId").val(): 0,
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
        $("#ImporteDescuentoId").val("");
        $("#descuentoMonedaId").data("kendoDropDownList").value("");
        $("#PorcentajeDescuentoId").val("");
    }
}

function validarDescuento(descuento) {
    var errores = [];

    if (descuento.TipoPeriodoDBId == 0 || descuento.TipoPeriodoDBId == "" || descuento.TipoPeriodoDBId == null) {
        errores.push("El campo Descuento no puede estar vacíos");
    }
    if (descuento.TipoDBId == 0 || descuento.TipoDBId == null || descuento.TipoDBId == "") {
        errores.push("El campo Tipo no puede estar vacíos");
    }
    if (descuento.TipoPeriodoDBId != 1 && (descuento.FechaDesde == "" || descuento.FechaDesde == undefined || descuento.FechaHasta == "" || descuento.FechaHasta == undefined)) {
        errores.push("La fecha no puede estar vacía");
    }
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
    return errores;
}

function AgregarCalidades() {
    var calidades = {
        Id: 0,
        CalidadEspecialDesc: $("#calidadesEspecialesId").data("kendoDropDownList").text(),
        CalidadEspecialId: $("#calidadesEspecialesId").data("kendoDropDownList").value(),
        Valor: $("#valorEspecialesId").val(),
        PorcentajeDesde: $("#porcentajeDesdeId").val() != "" ? $("#porcentajeDesdeId").val() : null,
        PorcentajeHasta: $("#porcentajeHastaId").val() != "" ? $("#porcentajeHastaId").val() : null,
        StandardDeCalidadId: 2,
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
}

function validarCalidad(calidad) {
    var errores = [];
    var cantidadCalidades = viewModel.Calidades.length;
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
    if (calidad.PorcentajeHasta > 51 && calidad.CalidadEspecialId == 1) {
        errores.push('El Porcentaje Hasta no debe ser mayor a 51% para "Dañados"');
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
    var datos = { id: contratoId };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerContratoCompleto', datos, function () { $.unblockUI(); });
    CargarDatosEditar(contratoEdit);
}

function InicializarFijacionEdit() {
    var datos = { id: fijacionId };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerFijacionCompleto', datos, function () { $.unblockUI();  });
    CargarDatosEditar(contratoEdit);
    InicializarBordesRojos();
}
function InicializarFasonEdit() {
    var datos = { id: fasonId };
    contratoEdit = MSExecuteOnServer('/CompraNet/TraerFasonCompleto', datos, function () { $.unblockUI(); });
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

    $("#buscadorProveedor").val(contrato.Proveedor);
    $("#buscadorProveedor").trigger("change");
    $("#fechaDesdeId").val(formatearFecha(contrato.FechaDesdeFormateado));
    $("#fechaHastaId").val(formatearFecha(contrato.FechaHastaFormateado));
    if (!hijo) {
        $("#fechaOperacionId").val(formatearFecha(contrato.FechaFormateado));
        $("#tipoId").data("kendoDropDownList").value(contrato.TipoNegocioId);
        $("#tipoId").data("kendoDropDownList").trigger("change");
    }
    $("#material").data("kendoDropDownList").value(contrato.MaterialId);
    $("#material").data("kendoDropDownList").trigger("change");

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

    (contrato.PlanCanje == true) ? $("#planCanjeId").prop("checked", true) : $("#planCanjeId").prop("checked", false);
    (contrato.Consignatario == true) ? $("#consignatarioId").prop("checked", true) : $("#consignatarioId").prop("checked", false);
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
    }else if (contrato.BoletoId == 3) {
        $("#boletoNingunoId").prop("checked", true);
    }

    if (!(contrato.DesdeFijacionFormateado == null && contrato.DesdeFijacionFormateado == undefined && contrato.DesdeFijacionFormateado == "")) {
        $("#fechaDesdeTopeId").val(contrato.DesdeFijacionFormateado);
    } else {
        $("#fechaDesdeTopeId").val("");
    }
    if (!(contrato.HastaFijacionFormateado == "null" && contrato.HastaFijacionFormateado == undefined && contrato.HastaFijacionFormateado == "")) {
        $("#fechaHastaTopeId").val(contrato.HastaFijacionFormateado);
    } else {
        $("#fechaDesdeTopeId").val("");
    }
    $("#condicionFijacionId").data("kendoDropDownList").value(contrato.CondicionFijacion);
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
    $("#posicionFasonId").val(contrato.Posicion);

    var iteracionesDescuentos = viewModel.Descuentos.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.Descuentos.pop();
    }
    var iteracionesCalidades = viewModel.Calidades.length;
    for (var i = 0; i < iteracionesCalidades; i++) {
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
