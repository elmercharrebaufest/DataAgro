var viewModel;
var datosIniCrearContrato;
var contratoEdit;
var Id;
var Siguientes;
var posicionFijacion;
var aperturaPrecio = [];
var altaTemprana;
var rol;
var precioMoa;
$(document).ready(function () {
    rol = $("#rol").val();
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

        if ($(this).val() == "" || $(this).val() == "0") {
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

    if (rol == "Corredor") {
        $(".proveedores").show();
        $("#buscadorProveedor").click(function () {
            $("#buscadorProveedor").data("kendoAutoComplete").value("");
            $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");

        });
    }

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
            ObtenerAlta(e.dataItem.Id);
            if ($("#tipoId").val() != 3 && (Id == 0 || Id == null || Id == "")) {
                $("#clasificacion").data("kendoDropDownList").value("");
                $("#consignatarioId").prop("checked", false);
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

    $("#buscadorCorredor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        select: function (e) {
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
        template: '<p class="buscar-nomb"><strong>#: data.ContratoId#</strong> - ' +
            'KG CTO: #: data.KilosContrato# ' +
            ' - KGS SIN PRECIO : #: data.ARecibirSinPrecio# - KGS SIN FIJAR : #: data.RecibidoSinFijar#' +
            ' - KILOS A FIJAR: #: data.KilosPendiente# - KG APLIC: #: data.KilosAplicados#' +
            ' - Hasta: #: data.FechaHasta# - <strong>#: data.CentroDescripcion#</strong></p>',
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
            if (e.dataItem.PagoDiferido === true) {
                MensErr("No estan habilitadas las Fijaciones de pago diferido – Contactarse con el comercial");
                $(this).data('kendoAutoComplete').value("");
            } else {
                $(".datoscontrato").show();
                $("#kgspendientescontrato").text(e.dataItem.KilosPendiente);
                $("#kgsaplicadoscontrato").text(e.dataItem.KilosAplicados);
                $("#desdecontrato").text(e.dataItem.FechaDesde);
                $("#hastacontrato").text(e.dataItem.FechaHasta);

                $("#posicionFasonId").val(e.dataItem.Posicion);
                $("#fechaDesdeId").val(e.dataItem.DesdeEntrega);
                $("#fechaHastaId").val(e.dataItem.HastaEntrega);
                $("#campanaId").data("kendoDropDownList").text(e.dataItem.Campana);
                $("#destinoId").data("kendoDropDownList").value(e.dataItem.Centro);
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
                //$("#ImporteSobrePrecio").val(e.dataItem.ImporteSobrePrecio);
                //$("#MonedaSobrePrecio").val(e.dataItem.MonedaSobrePrecio);
                //$("#PorcentajeSobrePrecio").val(e.dataItem.PorcentajeSobrePrecio);

                e.dataItem.Calidad === true ? $("#trigoEspecialFijacion").prop("checked", true) : $("#trigoEspecialFijacion").prop("checked", false);
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
    });
    $("#contratoId").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if (event.which < 48 || event.which > 57) {
            event.preventDefault();
        }
    });

    windowsResize();
    $(window).resize(windowsResize);

    $("#tipoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId"
    });

    $("#material").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId",
        change: function () {
            $("#contratoId").val("");
            $("#datosContrato").hide();
        },
        select: function (e) {
            precioMoa = MSExecuteOnServer('/CompraNet/TraerPrecioMOAPorMaterial', { materialId: e.dataItem.MaterialId });
            CargarPrecioMoa(e.dataItem.MaterialId, $("#precioMonedaId").val());
            HabilitarPizarra(e.dataItem.MaterialId);
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
        select: function (e) {
            if ($("#material").val() !== "") {
                var p = precioMoa.filter(function (f) { return f.MaterialId === Number($("#material").val()) && f.MonedaId === e.dataItem.MonedaId; })[0];
                $("#precioId").data("kendoNumericTextBox").value(p.Precio);
                $("#precioMonedaId").data("kendoDropDownList").value(p.MonedaId);
            }
        }
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
        dataValueField: "CampañaId"
        //change: function (e) {
        //    //validarFechaCampana();
        //}
    });

    $("#campanaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    //function validarFechaCampana() {
    //    var campana = $("#campanaId").data("kendoDropDownList").text();
    //    var anios = campana.split('-');
    //    var anioInicial = '20' + anios[0] + '0101';
    //    var anioFinal = '20' + anios[1] + '1231';
    //    var fechaDesde = $("#fechaDesdeId").val().split("-", 3);
    //    var fechaHasta = $("#fechaHastaId").val().split("-", 3);
    //    fechaDesde = fechaDesde[2] + fechaDesde[1] + fechaDesde[0];
    //    fechaHasta = fechaHasta[2] + fechaHasta[1] + fechaHasta[0];
    //    if (fechaDesde < anioInicial || fechaHasta > anioFinal) {
    //        $("#campanaTooltip").tooltip({ title: 'Fecha fuera del rango de Campaña' });
    //        $("#campanaTooltip").tooltip('show');
    //        $("#campanaTooltip").click(function () {
    //            $("#campanaTooltip").tooltip('destroy');
    //        });
    //    } else {
    //        $("#campanaTooltip").tooltip('destroy');
    //    }
    //}
    //$("#establecimientoDiv").hide();

    //$("#sustentableMonedaId").kendoDropDownList({
    //    optionLabel: "MONEDA...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "MonedaId"
    //});

    //$("#sustentableMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#sustentableMonedaId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#statusId").kendoDropDownList({
    //    optionLabel: "Estado de Contrato...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "EstadosContratosId"
    //});

    //$("#statusId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#statusId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#materialFiltroIndex").kendoDropDownList({
    //    optionLabel: "SELECCIONE UN MATERIAL...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "MaterialId"
    //});

    //$("#materialFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#materialFiltroIndex").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#campanaFiltroIndex").kendoDropDownList({
    //    optionLabel: "SELECCIONE UNA CAMPAÑA...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "CampañaId"
    //});

    //$("#campanaFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#campanaFiltroIndex").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#campanaModalPendienteId").kendoDropDownList({
    //    optionLabel: "SELECCIONE UNA CAMPAÑA...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "CampañaId"
    //});

    //$("#campanaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#campanaModalPendienteId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#statusFiltroIndex").kendoDropDownList({
    //    optionLabel: "Estado de Contrato...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "EstadosContratosId"
    //});

    //$("#statusFiltroIndex").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#statusFiltroIndex").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#consignatarioDiv").hide();
    //$("#clasificacion").kendoDropDownList({
    //    optionLabel: "SELECCIONE LA CLASIFICACIÓN...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id",
    //    change: function () {
    //        if (this.value() == 1) {
    //            $("#consignatarioDiv").hide();
    //            $("#consignatarioId").prop("checked", false);
    //            $("#planCanjeDiv").hide();
    //            $("#planCanjeId").prop("checked", false);
    //        } else {
    //            $("#consignatarioDiv").show();
    //            $("#planCanjeDiv").show();
    //        }
    //        ValidarAlta();
    //    }
    //});

    //$("#clasificacion").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#clasificacion").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});
    //$("#planCanjeId").click(function () {
    //    if (this.checked) {
    //        $("#consignatarioId").prop("checked", false);
    //    }
    //    ValidarAlta();
    //});
    //$("#consignatarioId").click(function () {
    //    if (this.checked) {
    //        $("#planCanjeId").prop("checked", false);
    //    }
    //    ValidarAlta();
    //});

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

    //$("#tipoFasonId").kendoDropDownList({
    //    optionLabel: "SELECCIONE TIPO FASON...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#tipoFasonId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#tipoFasonId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#tipoAgenteCompraId").kendoDropDownList({
    //    optionLabel: "SELECCIONE TIPO AGENTE...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#tipoAgenteCompraId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#tipoFasonId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#operadorId").kendoDropDownList({
    //    optionLabel: "SELECCIONE OPERADOR...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#operadorId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#operadorId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});
    //$("#destinoModalPendienteId").kendoDropDownList({
    //    optionLabel: "DESTINO...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#destinoModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#destinoModalPendienteId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#bolsaConfirmaId").kendoDropDownList({
    //    optionLabel: "SELECCIONE BOLSA...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#bolsaConfirmaId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#bolsaConfirmaId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#bolsaFisicoId").kendoDropDownList({
    //    optionLabel: "SELECCIONE BOLSA...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});
    //$("#bolsaFisicoId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#bolsaFisicoId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#bolsaCartaId").kendoDropDownList({
    //    optionLabel: "SELECCIONE BOLSA...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});
    //$("#bolsaCartaId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#bolsaCartaId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#condicionFijacionId").kendoDropDownList({
    //    optionLabel: "SELECCIONE CONDICIÓN...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#condicionFijacionId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#condicionFijacionId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#condicionFijacionIdModalPendiente").kendoDropDownList({
    //    optionLabel: "SELECCIONE CONDICIÓN...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#condicionFijacionIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#condicionFijacionIdModalPendiente").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});
    //$("#standardCalidadId").kendoDropDownList({
    //    optionLabel: "SELECCIONE STANDARD DE CALIDAD...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});

    //$("#standardCalidadId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#standardCalidadId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    //$("#calidadesEspecialesId").kendoDropDownList({
    //    optionLabel: "CALIDAD",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id",
    //    //change: CambioCalidades
    //});

    //$("#calidadesEspecialesId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#calidadesEspecialesId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});


    //$("#zonasGirasolAltoId").kendoDropDownList({
    //    optionLabel: "Zona",
    //    dataTextField: "Descripcion",
    //    dataValueField: "Id"
    //});
    //$("#zonasGirasolAltoId").closest('.k-dropdown.k-widget').keydown(function (e) {
    //    if (e.keyCode == 46) {
    //        var dropdownlist = $("#zonasGirasolAltoId").data("kendoDropDownList");
    //        dropdownlist.text("");
    //    }
    //});

    $("#cantidadId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        change: function () {
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

    //$("#cargarCantidadCamiones").change(function () {
    //    if ($("#cargarCantidadCamiones").is(':checked')) {
    //        $("#cantidadId").data("kendoNumericTextBox").trigger("change");
    //    }
    //    else {
    //        $("#cantidadCamionesId").data("kendoNumericTextBox").value('');
    //    }
    //});

    //$("#cantidadCamionesId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    spinners: false,
    //    min: 0
    //});
    $("#precioId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    //$(".number-input").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    decimals: 0,
    //    restrictDecimals: true,
    //    spinners: false,
    //    min: 0
    //});

    //$("#sustentablePrecioId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    spinners: false,
    //    min: 0
    //});

    //$("#diasDiferidoFijacionId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    spinners: false,
    //    min: 0
    //});

    //$("#contMadreId").change(function () {
    //    var sap = $("#contMadreId").val();
    //    if (sap !== "") {
    //        var datos = { sap: sap };
    //        contratoEdit = MSExecuteOnServer('/CompraNet/TraerContratoMadre', datos);
    //        if (ExistsErrorMessages(contratoEdit.Errores)) {
    //            ShowErrorMessages(contratoEdit.Errores);
    //        } else {
    //            CargarDatosEditar(contratoEdit.Contrato, true);
    //        }
    //    }
    //});
    //$("#contMadreId").on("keypress keyup blur", function (event) {
    //    $(this).val($(this).val().replace(/[^\d].+/, ""));
    //    if (event.which < 48 || event.which > 57) {
    //        event.preventDefault();
    //    }
    //});

    //$("#pesificadoDiasId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    spinners: false,
    //    min: 0
    //});

    //$("#porcentajeComision").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n2",
    //    spinners: false,
    //    decimals: 2,
    //    value: 1,
    //    min: 0,
    //    max: 100
    //});

    //$("#valorEspecialesId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n2",
    //    spinners: false,
    //    min: 0
    //});

    //$("#porcentajeDesdeId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n2",
    //    spinners: false,
    //    min: 0
    //});

    //$("#porcentajeHastaId").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n2",
    //    spinners: false,
    //    min: 0
    //});

    //var date = ObtenerFechaDesde();
    //var datehasta = ObtenerFechaHasta();

    $("#fechaDesdeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            $("#fechaHastaId").val(ObtenerFechaHasta(this.value()));
            validarFechaCampana();
        }
    });
    $("#fechaHastaId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    //$("#fechaOperacionId").kendoDatePicker({
    //    format: "dd-MM-yyyy",
    //    parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    //});

    //$("#fechaDesdeTopeId").kendoDatePicker({
    //    format: "dd-MM-yyyy",
    //    parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
    //    change: function () { $("#fechaHastaTopeId").val(ObtenerFechaHasta(this.value())); }
    //});
    //$("#fechaHastaTopeId").kendoDatePicker({
    //    format: "dd-MM-yyyy",
    //    parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    //});

    //$("#dolarizadoFechaId").kendoDatePicker({
    //    value: date,
    //    format: "dd-MM-yyyy",
    //    parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    //});

    //$("#fechaDesdeId").val(date);
    //$("#fechaHastaId").val(datehasta);

    $(".formulario-footer-guardar-contrato").click(function () {
        BlockUi('Guardando...');

        setTimeout(ObtenerDatos, 250);
    });

    $(".formulario-footer-cancelar").click(function () {
        window.location.href = window.location.origin + "/CompraNet";
    });


    $("#pizarraId").click(ClickEnPizarra);
}

function ClickEnPizarra() {
    var precioRojo = $("#precioId").hasClass("required-box-parent") ? $("#precioId") : $("#precioId").parent().parent();
    if ($("#pizarraId").is(':checked')) {
        $("#precioId").data("kendoNumericTextBox").enable(false);
        $("#precioMonedaId").data("kendoDropDownList").enable(false);
        $("#precioId").data("kendoNumericTextBox").value("");
        $("#precioId").trigger('change');
        $("#precioMonedaId").data("kendoDropDownList").value(0);
        precioRojo.removeClass("required-border");
    }
    else {
        $("#precioId").data("kendoNumericTextBox").enable(true);
        $("#precioMonedaId").data("kendoDropDownList").enable(true);
        $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        CargarPrecioMoa($("#material").val(), $("#precioMonedaId").val());
        precioRojo.addClass("required-border");
    }
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
//function CargarCalidadPorMaterial(value) {
//    var calidadGrano = MSExecuteOnServer('/CompraNet/TraerCalidadesPorMaterial', { MaterialId: value });
//    viewModel.set("EspecialesCombo", calidadGrano);

//    if ($("#calidadesEspecialesId").data("kendoDropDownList") && value === "3") {
//        $("#calidadesEspecialesId").data("kendoDropDownList").text("Fabrica");
//    } else if ($("#calidadesEspecialesId").data("kendoDropDownList") && (value === "2" || value === "1")) {
//        $("#calidadesEspecialesId").data("kendoDropDownList").text("Grado");
//    } else {
//        $("#calidadesEspecialesId").data("kendoDropDownList").text("Camara");
//    }
//    if ($("#material").val() == 2) {
//        $("#calidadesEspecialesId").data("kendoDropDownList").text("Grado 2");
//    }
//    CambioCalidades();
//}

//function LimpiarCalidades() {
//    var iteracionesCalidades = viewModel.Calidades.length;
//    for (i = 0; i < iteracionesCalidades; i++) {
//        viewModel.Calidades.pop();
//    }
//}
//function LimpiarDescuentos() {
//    var iteracionesDescuentos = viewModel.Descuentos.length;
//    for (var i = 0; i < iteracionesDescuentos; i++) {
//        viewModel.Descuentos.pop();
//    }
//}
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
        "precioNetoId": null
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
        DescuentosVisualizar: [],
        CalidadesVisualizar: [],
        ContratosPendientes: [],
        AperturaPrecio: []
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

    if ($("#tipoId").data("kendoDropDownList")) {
        $("#tipoId").data("kendoDropDownList").value("3");
        //$("#tipoId").data("kendoDropDownList").trigger("change");
        $("#tipoId").data("kendoDropDownList").readonly();
    }
    if ($("#precioMonedaId").data("kendoDropDownList")) {
        $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        //$("#pagoDolarizadoDiv").hide();
    }

    $("#proveedorLabelId").removeClass("col-sm-1");
    $("#proveedorLabelId").addClass("col-sm-2");
    if (rol == "Proveedor") {
        $("#corredorDiv").hide();
        $("#buscadorProveedor").data("kendoAutoComplete").readonly();
        $(".proveedores").hide();
    }
    if (rol == "Corredor") {
        $("#buscadorCorredor").data("kendoAutoComplete").readonly();
        $("#corredorDiv").hide();
    }


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
    obj.ContratoId = (obj.TipoNegocioId == 1 || obj.TipoNegocioId == 2) ? Id : 0;
    obj.FijacionDePrecioContratoId = obj.TipoNegocioId == 3 ? Id : 0;
    //obj.Id = (obj.TipoNegocioId == 4 || obj.TipoNegocioId == 5 || obj.TipoNegocioId == 6) ? Id : 0;
    obj.MaterialId = $("#material").val();
    obj.Cantidad = $("#cantidadId").val();
    obj.Precio = $("#precioId").val();
    obj.PrecioNeto = $("#precioId").val();
    //calcular el neto
    //var ImporteSobrePrecio = parseFloat($("#ImporteSobrePrecio").val());
    //var MonedaSobrePrecio = $("#MonedaSobrePrecio").val();
    //var PorcentajeSobrePrecio = parseFloat($("#PorcentajeSobrePrecio").val());
    //if (ImporteSobrePrecio > 0 && PorcentajeSobrePrecio > 0) {
    //    if (MonedaSobrePrecio != $("#precioMonedaId").val()) {
    //        var valorDolar = MSExecuteOnServer('/CompraNet/TraerTipoDeCambio', { });
    //        if ($.trim(MonedaSobrePrecio) == "ARP") {
    //            ImporteSobrePrecio = parseFloat(ImporteSobrePrecio) / valorDolar;
    //        } else {
    //            ImporteSobrePrecio = parseFloat(ImporteSobrePrecio) * valorDolar;
    //        }
    //    }
    //    var precioN = $("#precioId").data("kendoNumericTextBox").value();
    //    var desc = ((precioN + ImporteSobrePrecio) * PorcentajeSobrePrecio / 100);
    //    obj.PrecioNeto = precioN + ImporteSobrePrecio + desc;
    //    alert(obj.PrecioNeto);
    //} 
   
    obj.FechaEntrega = $("#fechaHastaId").val();
    obj.CampanaId = $("#campanaId").val();
    obj.FechaDesde = $("#fechaDesdeId").val();
    obj.FechaHasta = $("#fechaHastaId").val();
    obj.MonedaId = $("#precioMonedaId").val();
    if (obj.TipoNegocioId == "3" || obj.TipoNegocioId == "4") {
        obj.ComercialId = $("#comercialFijacionId").val();
        obj.ContratoSAP = $("#contratoId").val();
    }

    //obj.ComercialId = $("#comercialId").val();
    //obj.ComercialCreadorId = $("#comercialCreador").val();
    //obj.Base = $("#baseId").is(":checked") ? true : false;
    //obj.ImporteSustentable = $("#sustentablePrecioId").val();
    //obj.MonedaSustentableId = $("#sustentableMonedaId").val();
    //obj.FechaDolarizado = $("#dolarizadoFechaId").val();
    obj.PagoDiferidoContrato = $("#pesificadoId").is(":checked") ? true : false;
    obj.PagoDiferido = obj.TipoNegocioId != 3 ? $("#pesificadoId").is(":checked") ? true : false : $("#diasDiferidoId").is(":checked") ? true : false;
    //obj.Dolarizado = $("#dolarizadoId").is(":checked") ? true : false;
    //obj.Sustentable = $("#sustentableId").is(":checked") ? true : false;
    //obj.DiasPesificado = obj.TipoNegocioId != 3 ? $("#pesificadoDiasId").val() : $("#diasDiferidoFijacionId").val();
    //obj.PorcentajeComision = $("#porcentajeComision").val() != "" ? $("#porcentajeComision").val() : 0;
    //obj.NoInformaSio = $("#noInformaSioId").is(":checked") ? true : false;
    obj.EstadoId = 9;
    obj.Observacion = $("#observacionId").val();
    //obj.ClasificacionId = $("#clasificacion").val();
    //obj.CantidadCamiones = $("#cantidadCamionesId").val();
    //obj.EstablecimientoPropio = $("#establecimientoPropioId").is(":checked") ? true : $("#establecimientoArrendadoId").is(":checked") ? false : null;
    //obj.DesdeFijacion = $("#fechaDesdeTopeId").val();
    //obj.HastaFijacion = $("#fechaHastaTopeId").val();
    //obj.CondicionFijacionId = $("#condicionFijacionId").val();
    obj.DestinoId = $("#destinoId").val();
    //obj.planCanje = $("#planCanjeId").is(":checked") ? true : false;
    //obj.Consignatario = $("#consignatarioId").is(":checked") ? true : false;
    //obj.CD = $("#CDId").is(":checked") ? true : false;
    //obj.Warrant = $("#WarrantId").is(":checked") ? true : false;
    //obj.PagoDirectoVendedor = $("#pagoDirectoId").is(":checked") ? true : false;
    //obj.MercsDeposito = $("#mercsDepositoId").is(":checked") ? true : false;
    obj.Especial = $("#trigoEspecialFasonId").is(":checked") ? true : false;
    //obj.NivelTarifaId = $("#NivelTarifaId").val();
    //obj.TarifaFlete = $("#TarifaFleteId").val();

    //if ($("#boletoConfirmaId").is(':checked')) {
    //    obj.BoletoId = 1;
    //    obj.BolsaId = $("#bolsaConfirmaId").val();
    //}
    //else if ($("#boletoFisicoId").is(':checked')) {
    //    obj.BoletoId = 2;
    //    obj.BolsaId = $("#bolsaFisicoId").val();
    //}
    //else if ($("#boletoCartaId").is(':checked')) {
    //    obj.BoletoId = 4;
    //    obj.BolsaId = $("#bolsaCartaId").val();
    //}
    //else if ($("#boletoNingunoId").is(':checked')) {
    //    obj.BoletoId = 3;
    //    obj.BolsaId = 0;
    //}

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
        obj.ProveedorCreadorId = proveedorId;

    }
    if ($("#buscadorCorredor").val() != "") {
        var cuitAuxC = $("#buscadorCorredor").val().split('(');
        if (cuitAuxC[1]) {
            var cuitC = cuitAuxC[1].split(')');
            var corredorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuitC[0], corredor: true });
        }
        obj.ProveedorCreadorId = corredorId;
    }
    
    obj.ProveedorId = proveedorId;
    obj.CorredorId = corredorId;

    //if ($("#LocalidadCrearContrato").val() != "") {
    //    var localidadAux = $("#LocalidadCrearContrato").val().split('(');
    //    if (localidadAux[1]) {
    //        var provinciaAux = localidadAux[1].split(')');
    //        var Localidad = MSExecuteOnServer('/CompraNet/ObtenerLocalidadId', { localidad: localidadAux[0], provincia: provinciaAux[0] });
    //        obj.LocalidadId = Localidad.LocalidadId;
    //        obj.ProvinciaId = Localidad.ProvinciaId;
    //    } else {
    //        obj.LocalidadId = -1;
    //    }
    //}
    //obj.ContratoVendedor = $("#contVendedorId").val();
    //obj.ContratoCorredor = $("#contCorredorId").val();
    //obj.SelCargoVendedor = $("#selCargoVendedorId").is(":checked") ? true : false;
    //obj.SelCargoMOA = $("#selCargoMOAId").is(":checked") ? true : false;
    //obj.FasoneroId = proveedorId;
    obj.Posicion = $("#posicionFasonId").val();
    //obj.TipoFasonId = $("#tipoFasonId").val();
    //obj.tipoAgenteCompraId = $("#tipoAgenteCompraId").val();
    //obj.OperadorId = $("#operadorId").val();
    //if ($("#madreId").is(":checked")) {
    //    obj.Madre = true;
    //}
    //if ($("#hijoId").is(":checked")) {
    //    obj.Madre = false;
    //}
    obj.ContratoMadre = $("#contMadreId").val();
    obj.Descuentos = viewModel.Descuentos;


    obj.TrigoEspecial = viewModel.Calidades.length > 0 && obj.MaterialId == 2;

    //obj.ZonaId = obj.MaterialId == 5 ?
    //    $("#zonasGirasolAltoId").val() : null;
    //if (obj.MaterialId === "3") {
    //    obj.Calidad = viewModel.Calidades;
    //} else {
    //    if ($("#calidadesEspecialesId").data("kendoDropDownList").text() == "Especial" ||
    //        $("#calidadesEspecialesId").data("kendoDropDownList").text() == "Grado 2" ||
    //        $("#calidadesEspecialesId").data("kendoDropDownList").text() == "Grado" ||
    //        $("#calidadesEspecialesId").data("kendoDropDownList").text() == "Bonif. SECO de 7% a 10% Por punto") {
    //        var err = [];
    //        if (viewModel.Calidades.length == 0 && (obj.TipoNegocioId == 1 || obj.TipoNegocioId == 2)) {
    //            err = AgregarCalidades();
    //        } else if ($("#valorEspecialesId").val() != "") {
    //            LimpiarCalidades();
    //            err = AgregarCalidades();
    //        }
    //        if (ExistsErrorMessages(err)) {
    //            $.unblockUI();
    //            var error = true;
    //        } else {
    //            obj.Calidad = viewModel.Calidades;
    //        }
    //    }
    //}

    //if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Camara") {
    //    if (obj.MaterialId == 3) obj.StandardDeCalidadId = 4;
    //    else if (obj.MaterialId == 4 || obj.MaterialId == 5) obj.StandardDeCalidadId = 5;
    //    else if (obj.MaterialId == 1 || obj.MaterialId == 2) obj.StandardDeCalidadId = 3;
    //} else if ($("#calidadesEspecialesId").data("kendoDropDownList").text() === "Fabrica") {
    //    obj.StandardDeCalidadId = 3;
    //} else if (viewModel.Calidades.length > 0) {
    //    obj.StandardDeCalidadId = viewModel.Calidades[0].StandardDeCalidadId;
    //}

    if (obj.TipoNegocioId == 3) obj.TrigoEspecial = $("#trigoEspecialFijacion").is(":checked");
    //obj.Compensacion = $("#compensacionId").is(":checked") ? true : false;
    //obj.ContratoAcuerdoId = $("#contratoAcuerdoId").val();
    obj.Pizarra = $("#pizarraId").is(":checked") ? true : false;
    //obj.AperturaPrecio = viewModel.AperturaPrecio;
    obj.FechaOperacion = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    GrabarContrato(obj);

}

function GrabarContrato(nuevoContrato) {
    var result;

    if (nuevoContrato.TipoNegocioId == 1 || nuevoContrato.TipoNegocioId == 2) {
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
    console.log(contrato);
    InicializarBordesRojos();
    $("#buscadorCorredor").val(contrato.Corredor);
    $("#buscadorCorredor").trigger("change");
    $("#estado").val(contrato.Estado);
    $("#buscadorProveedor").val(contrato.Proveedor);
    $("#buscadorProveedor").trigger("change");
    $("#fechaDesdeId").val(formatearFecha(contrato.FechaDesdeFormateado));
    $("#fechaHastaId").val(formatearFecha(contrato.FechaHastaFormateado));
    if (!hijo) {
        $("#fechaOperacionId").val(formatearFecha(contrato.FechaFormateado));
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
            $("#fechaDesdeTopeId").val("");
        }
        $("#condicionFijacionId").data("kendoDropDownList").value(contrato.CondicionFijacion);
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

    if (contrato.Fecha_DolarizadoFormateado !== null && contrato.Fecha_DolarizadoFormateado !== undefined && contrato.Fecha_DolarizadoFormateado !== "") {
        $("#dolarizadoId").prop("checked", true);
        $("#dolarizadoDiv").show();
        $("#dolarizadoFechaId").val(contrato.Fecha_DolarizadoFormateado);
    }

    if (contrato.PagoDiferido) {
        if (contrato.TipoNegocioId != 3) {
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
    //LimpiarBoleto();
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

    LimpiarDescuentos();
    LimpiarCalidades();
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

    if (contrato.StandardCalidadId == 2) {
        $(".calidadesEspecialesDatos").show();
    } else {
        $(".calidadesEspecialesDatos").hide();
    }
    if (contrato.Calidades !== null) {
        var descripcion = contrato.Calidades.length > 0 ? contrato.Calidades[0].CalidadEspecialDesc : contrato.StandardDeCalidadDescripcion;
        $("#calidadesEspecialesId").data("kendoDropDownList").text(descripcion);
        //CambioCalidades(contrato.Calidades);
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
            console.log(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId === 1; }));

            $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 1; }).Importe);
            $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 2; }).Importe);
            $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").max(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Importe);
            $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Importe);
            $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 3; }).Porcentaje);
            $("#aperturaPrecioImporteBonificacionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 4; }).Importe);
            $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value(contrato.AperturaPrecios.find(function (x) { return x.ConceptoAperturaPrecioId == 4; }).Porcentaje);


        }
        InicializarEditarContratoApertura();

        GuardarAperturaDePrecio();
        contrato.Compensacion === true ? $("#compensacionId").prop("checked", true) : $("#compensacionId").prop("checked", false);
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



function GuardarAperturaDePrecio() {
    var total = CalcularPrecioTotalApertura();
    if (total <= 0 && ($("#tipoId").val() == 2 || $("#tipoId").val() == 3) && !$("#pizarraId").is(':checked')) {
        MensErr("El Precio Total no puede ser menor o igual a 0");
    } else {
        InsertarAperturasViewModel(total);
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


//function InicializarAperturaDePrecios() {
//    $("#precioTotalApertura").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false,
//        min: 0
//    });
//    $("#precioTotalApertura").data("kendoNumericTextBox").readonly();
//    $("#precioTotalApertura").data("kendoNumericTextBox").enable(false);

//    $("#aperturaPrecioBtn").click(function () {
//        AbrirModalAperturaDePrecio();
//    });

//    $(".aperturaPrecioInput").change(function () {
//        CalcularPrecioTotalApertura();
//    });

//    $("#guardarAperturaPrecio").click(function () {
//        GuardarAperturaDePrecio();
//    });

//    $("#precioId").change(function () {
//        var total = CalcularPrecioTotalApertura();
//        $("#precioTotalApertura").data("kendoNumericTextBox").value(total);
//        SetearValoresMaximosApertura();
//    });

//    $("#aperturaPrecioImporteFinancieroId").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false
//    });


//    $("#aperturaPrecioImporteRedespachoId").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false
//    });
//    $("#aperturaPrecioImporteComisionesId").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false,
//        min: 0
//    });

//    $("#aperturaPrecioImporteFinancieroId").change(function () { PonerEnCeroSiEsNulo("aperturaPrecioImporteFinancieroId"); });
//    $("#aperturaPrecioImporteComisionesId").change(ModificarComisionesPorImporte);
//    $("#aperturaPrecioPorcentajeComisionesId").change(ModificarComisionesPorcentaje);
//    $("#aperturaPrecioImporteRedespachoId").change(ConvertirRedespachoANegativo);

//    $("#aperturaPrecioPorcentajeComisionesId").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false,
//        min: 0,
//        max: 100
//    });


//    $("#aperturaPrecioImporteBonificacionesId").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false,
//        min: 0
//    });
//    $("#aperturaPrecioPorcentajeBonificacionesId").kendoNumericTextBox({
//        culture: "es-AR",
//        format: "n2",
//        spinners: false,
//        min: 0
//    });

//    $("#botonAperturaDePrecioFijacion").hide();
//}

//function AbrirModalAperturaDePrecio() {
//    if (viewModel.AperturaPrecio.length > 0) {
//        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[0].Importe);
//        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[1].Importe);
//        $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[2].Porcentaje);
//        $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox").value(viewModel.AperturaPrecio[2].Importe);
//    } else {
//        $("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").value(0);
//        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(0);
//        //$("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").value(0);
//        $("#aperturaPrecioPorcentajeBonificacionesId").data("kendoNumericTextBox").value(0);
//    }

//    CalcularMaximoComision();
//    var moneda = "";
//    if ($("#precioMonedaId").val()) {
//        $(".aperturaprecioMoneda").text($("#precioMonedaId").data("kendoDropDownList").text());
//        moneda = $("#precioMonedaId").data("kendoDropDownList").text();
//    } else {
//        $(".aperturaprecioMoneda").text("");
//    }
//    $("#precioAperturaOriginal").text(kendo.toString($("#precioId").val().replace(',', '.') ? Number($("#precioId").val().replace(',', '.')) : Number(0), "n2") + " " + moneda);
//    CalcularPrecioTotalApertura();
//    $("#modalAperturaPrecio").modal("show");
//}

//function CalcularPrecioTotalApertura() {
//    var bonificacion = Number($("#precioId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
//    var precioOriginal = Number($("#precioId").val().replace(',', '.'));
//    var porcentajeComision = Math.min(Number($("#aperturaPrecioPorcentajeComisionesId").val().replace(',', '.')), $("#aperturaPrecioPorcentajeComisionesId").data("kendoNumericTextBox").max());
//    var precioTarifaFlete = Number($("#TarifaFleteId").val().replace(',', '.'));
//    precioOriginal += Math.min(Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')), Number($("#aperturaPrecioImporteFinancieroId").data("kendoNumericTextBox").max()));
//    precioOriginal += Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
//    CalcularMaximoComision();
//    precioOriginal += Number($("#aperturaPrecioImporteBonificacionesId").val().replace(',', '.'));

//    precioOriginal += Number($("#aperturaPrecioPorcentajeBonificacionesId").val().replace(',', '.')) * Number($("#precioId").val().replace(',', '.')) / 100;

//    porcentajeComision = porcentajeComision / 100;
//    precioOriginal += (precioOriginal * porcentajeComision) - precioTarifaFlete;
//    precioOriginal += Number($("#aperturaPrecioImporteComisionesId").val().replace(',', '.'));

//    $("#totalApertura").text(kendo.toString(precioOriginal, "n2") + " " + ($("#precioMonedaId").val() ? $("#precioMonedaId").data("kendoDropDownList").text() : ""));

//    return precioOriginal;
//}

//function CalcularMaximoComision() {
//    var maximoBonificacion = (Number($("#precioId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteFinancieroId").val().replace(',', '.')) + Number($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'))) * 0.01;
//    var comisionesTextBox = $("#aperturaPrecioImporteComisionesId").data("kendoNumericTextBox");
//    if (comisionesTextBox) {
//        comisionesTextBox.max(maximoBonificacion);
//        if (Number($("#aperturaPrecioImporteComisionesId").val().replace(',', '.')) > comisionesTextBox.max()) {
//            comisionesTextBox.value(comisionesTextBox.max());
//        }
//    }
////}

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
//function ConvertirDescuentoANegativo() {
//    if ($("#tipoId").val() == 1 && $("#destinoId").val() != 1) {
//        var valorAbsoluto = Math.abs($("#ImporteDescuentoId").val().replace(',', '.'));
//        $("#ImporteDescuentoId").data("kendoNumericTextBox").value(-1 * valorAbsoluto);
//    }
//}
//function ConvertirRedespachoANegativo() {
//    var valorAbsoluto = Math.abs($("#aperturaPrecioImporteRedespachoId").val().replace(',', '.'));
//    if (valorAbsoluto > Number($("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").max())) {
//        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(-1 * $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").max());
//    } else {
//        $("#aperturaPrecioImporteRedespachoId").data("kendoNumericTextBox").value(-1 * valorAbsoluto);
//    }
//    CalcularPrecioTotalApertura();
//}
//function PonerEnCeroSiEsNulo(elemento) {
//    if (!$("#" + elemento).val()) {
//        $("#" + elemento).val(0);
//    }
//}

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
        MensInfo("No está habilitado en Ruca. COntactese con el Comercial");
    }
}

function CargarPrecioMoa(material) {
    lista = [];
    for (var i = 0; i < precioMoa.length; i++) {
        if (precioMoa[i].Precio > 0) {
            lista.push(precioMoa[i]);
        }
    }
    if (lista.length > 0) {
        if ($("#precioMonedaId").val() === "") {
            $("#precioMonedaId").data("kendoDropDownList").value("ARP  ");
        }
        var moneda = $("#precioMonedaId").val();
        $("#precioId").data("kendoNumericTextBox").enable(true);
        $("#precioMonedaId").data("kendoDropDownList").enable(true);
        if (lista.length == 1) {
            $("#precioId").data("kendoNumericTextBox").value(lista[0].Precio);
            $("#precioMonedaId").data("kendoDropDownList").value(lista[0].MonedaId);
        } else if (lista.length > 1) {
            var p = precioMoa.filter(function (e) { return e.MaterialId === Number(material) && e.MonedaId === moneda; })[0];
            $("#precioId").data("kendoNumericTextBox").value(p.Precio);
            $("#precioMonedaId").data("kendoDropDownList").value(p.MonedaId);
        }
    } else {
        $("#precioId").data("kendoNumericTextBox").enable(false);
        $("#precioMonedaId").data("kendoDropDownList").enable(false);
        $("#precioId").data("kendoNumericTextBox").value("");
        $("#precioMonedaId").data("kendoDropDownList").value("");
    }
    InicializarBordesRojos();
}

function HabilitarPizarra(material) {
    var pizarra = MSExecuteOnServer('/CompraNet/HabilitarPizarra', { material: material });
    if (pizarra != true) {
        $("#pizarraId").prop("checked", false);
        $("#pizarraId").attr('disabled', 'disabled');
    } else {
        if (!precioMoa) {
            $("#pizarraId").prop("checked", true);
            $("#pizarraId").attr('disabled', 'disabled');
        } else {
            $("#pizarraId").prop("checked", false);
            $("#pizarraId").removeAttr('disabled');
        }
    }
}