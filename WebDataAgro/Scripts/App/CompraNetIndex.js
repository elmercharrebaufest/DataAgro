var viewModel;
var datosIniCrearContrato;
var filasSeleccionadas = {};
var comercialId;

$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    CrearViewModel();
    InicializarBuscador();
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

    $("#finalizarBorrar").click(function () {
        ObtenerDatosModalBorrado();
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
        if ($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial" || $("#perfil").val() == "CorredoresComercial") {
            window.location.href = window.location.origin + "/CompraNet/CrearContrato";
        } else {
            MensInfo("No posee permisos para la carga de contratos");
        }
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
});

$("#confirmarVariosDiv").ready(function () {
    if ($("#perfil").val() == "Mesa") {
        $("#confirmarVariosDiv").show();
    } else {
        $("#confirmarVariosDiv").hide();
    }
});

$("#buscadorZonaDiv").ready(function () {
    if ($("#perfil").val() == "Mesa") {
        $("#buscadorZonaDiv").show();
    } else {
        $("#buscadorZonaDiv").hide();
    }
}
);

function htmlEncode(value) {
    return $('<div/>').text(value.replace(/(\r\n|\n|\r)/gm, "")).html();
}

function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
}
function FormatearString(string, moneda) {
    var numero = (parseFloat(string)).toLocaleString('es-AR', { minimumFractionDigits: 2 }) + ' ' + moneda;
    return numero;
}
function botonPendiente(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Editar" onclick="editarContrato(' +
        "'" + dataItem.Id + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" + ')"><i class="fa ' + icono + '"></i></button>';
}

function botonConfirmadoTilde(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Confirmar" onclick="ModalConfirmadoTilde(' +
        "'" + dataItem.Estado + "'" + ',' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.ContratoSAP + "'" + ',' +
        "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" +
        ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
}

function botonFinalizado(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Finalizar" onclick="ModalFinalizado(' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" + ',' +
        "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
        "'" + dataItem.FasonId + "'" + ',' +
        "'" + dataItem.AgenteId + "'" + ',' +
        "'" + dataItem.AcuerdoId + "'" +
        ')"><i class="fa ' + icono + ' conf"></i></button>';
}

function botonVisualizar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Visualizar" onclick="ModalVisualizar(' +
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
        "'" + dataItem.Importe_Sustentable + "'" + ',' +
        "'" + dataItem.MonedaId_Sustentable + "'" + ',' +
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

        "'" + dataItem.ZonaDescripcion + "'" +

        ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
}

function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalBorrar(' +
        "'" + dataItem.Proveedor + "'" + ',' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" + ',' +
        "'" + dataItem.FijacionDePrecioContratoId + "'" + ',' +
        "'" + dataItem.FasonId + "'" + ',' +
        "'" + dataItem.AgenteId + "'" + ',' +
        "'" + dataItem.Estado + "'" +
        ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
}

function AutoRecargar() {
    setInterval(function () {
        if (document.getElementById('checkRecarga').checked == true) {
            filasSeleccionadas = SeleccionarElementos();
            recargarGrilla();
        }
    }, 30000);

}

function recargarGrilla() {
    $('#gridInformeCompraNet').data('kendoGrid').dataSource.read();
    AvisoContratosPendientes();
}

function filtrarZona() {
    //FILTRO MANUAL
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    if ($("#buscadorFiltroZona").val() != null) {
        addOrRemoveFilter(grilla, "GrupoCompraDescripcion", "eq", $("#buscadorFiltroZona").val());
    } else {
        addOrRemoveFilter(grilla, "GrupoCompraDescripcion", "eq", "");
    }
    recargarGrilla();
}
function filtrarMesa() {
    //FILTRO MANUAL
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    if (!$("#negociosPropiosDiv").hasClass("selected")) {
        addOrRemoveFilter(grilla, "ComercialId", "eq", comercialId);
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

    if (value && value.length > 0) {
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
                url: '/CompraNet/BuscaDatosTabla'
            },
            parameterMap: function (options, operation) {
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
                    HastaFijacion: { type: "date" }
                }
            },
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Fecha", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20,
        filter: defaultFilter,
    };

    $("#gridInformeCompraNet").kendoGrid({
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").attr('id', 'border-orange');
            $("td:has(div.statusconfirmado)").attr('id', 'border-green');
            $("td:has(div.statusoferta)").attr('id', 'border-blue');
            $("td:has(div.statuserror)").attr('id', 'border-red');
            $("td:has(div.statusfinalizado)").attr('id', 'border-black');
            $("td:has(div.statusborrado)").attr('id', 'border-grey');
            $("td:has(div.statusreconfirmar)").attr('id', 'border-purple');
            $("td:has(div.statuseliminado)").attr('id', 'border-grey');
            if (($("#perfil").val() !== "Mesa")) {
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
                if (view[i].DestinoDescripcion != "San Lorenzo" && view[i].DestinoDescripcion != "") {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("otroDestino");
                }
            }
            filasSeleccionadas = {};

        },

        columns: [
            { selectable: true, width: "50px" },
            {
                field: "Proveedor", type: "string", width: 150, attributes: {
                    "id": "line"
                },
                template: function (dataItem) {
                    if (dataItem.Estado == 1) {
                        return '<div class="statuspendiente "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 2) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 3) {
                        return '<div class="statusoferta "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 4) {
                        return '<div class="statuserror "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 5) {
                        return '<div class="statusfinalizado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 6) {
                        return '<div class="statusborrado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 7) {
                        return '<div class="statusreconfirmar "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 8) {
                        return '<div class="statuseliminado "></div>' + dataItem.Proveedor;
                    }
                },
                filterable: { ui: createMultiSelectProveedor }
            },
            {
                field: "Corredor", type: "string", width: 150, filterable: { ui: createMultiSelectCorredor }
            },
            {
                field: "FechaDesde", type: "date", title: "Desde", format: _DefaultDateTemplate, width: 45, attributes: {
                    "class": "mobile-sm"
                }
            },
            {
                field: "FechaHasta", type: "date", title: "Hasta", format: _DefaultDateTemplate, width: 45, attributes: {
                    "class": "mobile-sm"
                }
            },
            {
                field: "TipoNegocio", type: "string", filterable: {
                    multi: true, dataSource: [{
                        TipoNegocio: "A FIJAR"
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
                        TipoNegocio: "AGENTE COMPRAS"
                    }]
                }, title: "Tipo", width: 70, attributes: {
                    "class": "mobile-sm"
                }
            },
            {
                field: "Material", type: "string", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz Duro Dentado"
                    }, {
                        Material: "Trigo Pan"
                        }, {
                            Material: "Semilla de Soja"
                        }, {
                            Material: "Girasol"
                        }, {
                            Material: "Girasol Alto Oleico"
                        }]
                }, width: 95, attributes: {
                    "class": "mobile-xs"
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Material|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Material#'/></label></span>";
                }, template: "#=Material#"
            },
            {
                field: "Cantidad", type: "number", width: 70, format: "{0:n0}", attributes: {
                    "class": "mobile-xs"
                }
            },
            {
                field: "Ampliaciones", type: "number", width: 60, title: "Ampl.", attributes: {
                    "class": "mobile-sm"
                }, template: function (dataItem) {
                    if (($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial" || $("#perfil").val() == "CorredoresComercial") && dataItem.Estado == 2) {
                        return '' + dataItem.Ampliaciones + '<button data-toggle="tooltip" title="Ampliar"onclick="ModalAmpliaciones(' +
                            "'" + dataItem.ContratoId + "'" + ',' + "'" + dataItem.Ampliacion + "'" + ',' + "'" + dataItem.TipoNegocioId + "'" + "," + "'" + dataItem.FijacionDePrecioContratoId + "'" + "," + "'" + dataItem.FasonId + "'" + "," + "'" + dataItem.AgenteId + "'" + ')"><i class="fa fa-plus aria-hidden="true"></i></button>';
                    } else if (dataItem.Estado == 1 || dataItem.Estado == 3 || dataItem.Estado == 7) {
                        return dataItem.Ampliaciones;
                    } else {
                        return '';
                    }
                }
            },
            {
                field: "PrecioPlazo", type: "string", title: "Precio/Plazo", width: 70, filterable: false, sortable: false, template: function (dataItem) {
                    if (dataItem.TipoNegocioId !== 1) {
                        return FormatearString(dataItem.PrecioPlazo, dataItem.Moneda);
                    } else {
                        return dataItem.PrecioPlazo;
                    }
                }
            },
            { field: "Campania", type: "string", title: "Cos", width: 60, attributes: { "class": "mobile-md" } /*title: "Campa&ntilde;a"*/ },
            {
                field: "Negocio", type: "number", title: "N&deg; Negocio", width: 70, attributes: { "class": "mobile-md" },
                template: function (dataItem) {
                    if (dataItem.Negocio !== "" && dataItem.Negocio !== null) {
                        return kendo.parseInt(dataItem.Negocio);
                    } else {
                        return "";
                    }
                }
            },
            {
                field: "Fecha", type: "date", title: "Carga", width: 1, format: _DefaultDateTemplate, attributes: { "class": "mobile-xs" }
            },
            {
                field: "GrupoCompraDescripcion", type: "string", title: "Zona", width: 70, attributes: { "class": "mobile-xs mobile-md" }
            },
            {
                field: "Comercial", type: "string", title: "Comercial", width: 70, filterable: { ui: createMultiSelectComercial }, attributes: { "class": "mobile-xs" }
            },
            {
                field: "ComercialCreador", type: "string", title: "Creador", width: 70, filterable: { ui: createMultiSelectComercial }, attributes: { "class": "mobile-xs" }
            },
            {
                field: "DestinoDescripcion", type: "string", title: "Destino", attributes: { "class": "mobile-xs mobile-md" }
            },
            {
                field: "Estado_Contrato", sortable: false, title: "Estado", filterable: {
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
                        Estado_Contrato: "Reconfirmar"
                    }, {
                        Estado_Contrato: "Eliminado"
                    }]
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Estado_Contrato|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Estado_Contrato#'/></label></span>"
                }, template: function (dataItem) {
                    if (dataItem.Estado == 1 && ($("#perfil").val() == "Mesa")) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonPendiente(dataItem, 'fa-pencil pend') +
                            botonConfirmadoTilde(dataItem, 'fa-check pend') +
                            botonVisualizar(dataItem, 'fa-eye pend') +
                            botonBorrar(dataItem, 'fa-trash pend');
                    } else if (dataItem.Estado == 1) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonPendiente(dataItem, 'fa-pencil pend') +
                            botonVisualizar(dataItem, 'fa-eye pend') +
                            botonBorrar(dataItem, 'fa-trash pend');
                    }

                    if (dataItem.Estado == 2 && ($("#perfil").val() == "Mesa")) { //confirmado
                        return '<div class="status confirmado">Confirmado</div>' +
                            botonPendiente(dataItem, 'fa-pencil conf') +
                            botonFinalizado(dataItem, 'fa-flag-checkered conf') +
                            botonVisualizar(dataItem, 'fa-eye conf') +
                            botonBorrar(dataItem, 'fa-trash conf');
                    } else if (dataItem.Estado == 2) {
                        return '<div class="status confirmado">Confirmado</div>' +
                            botonPendiente(dataItem, 'fa-pencil conf') +
                            botonFinalizado(dataItem, 'fa-flag-checkered conf') +
                            botonVisualizar(dataItem, 'fa-eye conf');
                    }
                    if (dataItem.Estado == 3 && ($("#perfil").val() == "Mesa")) { //Oferta
                        return '<div class="status oferta">Oferta</div>' +
                            botonPendiente(dataItem, 'fa-pencil ofe') +
                            botonConfirmadoTilde(dataItem, 'fa-check ofe') +
                            botonVisualizar(dataItem, 'fa-eye ofe') +
                            botonBorrar(dataItem, 'fa-trash ofe');
                    } else if (dataItem.Estado == 3) {
                        return '<div class="status oferta">Oferta</div>' +
                            botonPendiente(dataItem, 'fa-pencil ofe') +
                            botonVisualizar(dataItem, 'fa-eye ofe') +
                            botonBorrar(dataItem, 'fa-trash ofe');
                    }

                    if (dataItem.Estado == 4 && ($("#perfil").val() == "Mesa")) { //error
                        return '<div class="status error">Con Error</div>' +
                            botonPendiente(dataItem, 'fa-pencil err') +
                            botonFinalizado(dataItem, 'fa-flag-checkered err') +
                            botonVisualizar(dataItem, 'fa-eye err') +
                            botonBorrar(dataItem, 'fa-trash err');
                    } else if (dataItem.Estado == 4) {
                        return '<div class="status error">Con Error</div>' +
                            botonPendiente(dataItem, 'fa-pencil err') +
                            botonFinalizado(dataItem, 'fa-flag-checkered err') +
                            botonVisualizar(dataItem, 'fa-eye err');
                    }

                    if (dataItem.Estado == 5) { //Finalizado
                        if ($("#perfil").val() == "Mesa" && (dataItem.ContratoId || dataItem.FasonId || dataItem.AgenteId) && !dataItem.FijacionDePrecioContratoId) {
                            return '<div class="status finalizado">Finalizado</div>' +
                                botonVisualizar(dataItem, 'fa-eye fin') +
                                botonBorrar(dataItem, 'fa-trash fin');
                        } else {
                            return '<div class="status finalizado">Finalizado</div>' +
                                botonVisualizar(dataItem, 'fa-eye fin');
                        }
                    }
                    if (dataItem.Estado == 6) { //Rechazado
                        return '<div class="status borrado">Rechazado</div>' +
                            botonVisualizar(dataItem, 'fa-eye bor');
                    }
                    if (dataItem.Estado == 7 && ($("#perfil").val() == "Mesa")) { //reconfirmar
                        return '<div class="status reconfirmar">Reconfirmar</div>' +
                            botonPendiente(dataItem, 'fa-pencil reconf') +
                            botonConfirmadoTilde(dataItem, 'fa-check reconf') +
                            botonVisualizar(dataItem, 'fa-eye reconf') +
                            botonBorrar(dataItem, 'fa-trash reconf');
                    } else if (dataItem.Estado == 7) { //reconfirmar
                        return '<div class="status reconfirmar">Reconfirmar</div>' +
                            botonPendiente(dataItem, 'fa-pencil reconf') +
                            botonVisualizar(dataItem, 'fa-eye reconf') +
                            botonBorrar(dataItem, 'fa-trash reconf');
                    }
                    if (dataItem.Estado == 8) { //Finalizado
                        return '<div class="status anulado">Anulado</div>' +
                            botonVisualizar(dataItem, 'fa-eye anu');
                    }
                }
            }
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
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        ////selectable: "multiple, row",


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
            if (e.field == "Proveedor" || e.field == "Comercial") {
                $(e.container).css("width", "300px")
            }
        }


    });
    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };
    function createMultiSelect(element, textField, valueField, url) {
        element.removeAttr("data-bind");

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

                var filter = { logic: "or", filters: [] };
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        filter.filters.push({ field: valueField, operator: "eq", value: v });
                    }
                });
                if (values.length === 0) {
                    $("#gridInformeCompraNet").data("kendoGrid").dataSource.filter(defaultFilter);
                } else {
                    $("#gridInformeCompraNet").data("kendoGrid").dataSource.filter(filter);
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "ProveedorId", "/CompraNet/ListarProveedor");
    }
    function createMultiSelectCorredor(element) {
        return createMultiSelect(element, "Corredor", "CorredorId", "/CompraNet/ListarCorredor");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial");
    }
    AvisoContratosPendientes();
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
    window.location.href = window.location.origin + "/CompraNet/CrearContrato?id=" + id + '&tipoId=' + tipoId + (siguientes != undefined ? "&siguientes=" + JSON.stringify(siguientes) : "");
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
    } else if($("#tipoNegocioModalFinalizado").val() === "6") {
        result = MSExecuteOnServer('/CompraNet/FinalizarAcuerdo', finalizarContratoFijacion);
    } else {
        result = MSExecuteOnServer('/CompraNet/FinalizarContrato', finalizarContratoFijacion);
    }

    if (result != null) {
        if (result.Errores != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr("No se ha podido finalizar el contrato correctamente: " + result.Errores[0].Message);
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
    if (negocios.length > 0) {
        for (var i in negocios) {
            var loader = '<div class="col-xs-1"><div id="estado' + i + '" class="loader" hidden></div></div><div id="error' + i + '" class="col-xs-8"> </div>';
            if (negocios[i].Estado === 1 || negocios[i].Estado === 3 || negocios[i].Estado === 7) {
                if (negocios[i].TipoNegocioId === 3) {
                    $("#negocioConfirmado-modal").append('<div class="row"><div class="col-xs-3">Fijaci&oacute;n: ' + negocios[i].FijacionDePrecioContratoId + '</div>' + loader + '</div>');
                } else {
                    $("#negocioConfirmado-modal").append('<div class="row"><div class="col-xs-3">Contrato: ' + negocios[i].ContratoId + '</div>' + loader + '</div>');
                }
            }
        }
    } else {
        $("#negocioConfirmado-modal").append('<div style="text-align:center"> Se debe seleccionar negocios</div>');
        $("#confirmarVarios").hide();
        $("#cancelarVarios").hide();
        $("#cerrarVarios").show();
    }
    $("#ModalConfirmarVarios").modal('show');
}

function ConfirmarVariosContratos() {
    $("#confirmarVarios").hide();
    $("#cancelarVarios").hide();
    $("#cerrarVarios").show();

    var negocios = SeleccionarElementos();
    $(".loader").show();
    for (var i in negocios) {
        if (negocios[i].Estado === 1 || negocios[i].Estado === 3 || negocios[i].Estado === 7) {
            var objConfirmado = {};
            $("#estado" + i).empty();
            var result = null;
            if (negocios[i].TipoNegocioId === 3) {
                objConfirmado.fijacionDePrecioContratoId = negocios[i].FijacionDePrecioContratoId;
                result = MSExecuteOnServer('/CompraNet/ConfirmarFijacion', objConfirmado);
            } else if (negocios[i].TipoNegocioId === 2 || negocios[i].TipoNegocioId === 1){
                objConfirmado.contratoId = negocios[i].ContratoId;
                result = MSExecuteOnServer('/CompraNet/ConfirmarContrato', objConfirmado);
            }
            finalizacionCallBack(i, result);
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
                } else if (negocios[i].TipoNegocioId === 6) {
                    $("#negocioFinalizado-modal").append('<div class="row"><div class="col-xs-4">Agente de Compras: ' + negocios[i].AcuerdoId + '</div>' + loader + '</div>');
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
            }
        }
    } else {
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
    } else {
        objConfirmado.contratoId = $("#contratoModalConTilde").val();
    }
    Confirmar(objConfirmado);
}

function Confirmar(confirmarContratoFijacion) {
    var result = null;
    if ($("#tipoNegocioModalConTilde").val() === '3') {
        result = MSExecuteOnServer('/CompraNet/ConfirmarFijacion', confirmarContratoFijacion);
    } else {
        result = MSExecuteOnServer('/CompraNet/ConfirmarContrato', confirmarContratoFijacion);
    }

    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}

function ObtenerDatosModalBorrado() {
    var objConfirmado = {};
    var result;
    if ($("#tipoNegocioModalBorrar").val() === '3') {
        objConfirmado.FijacionDePrecioContratoId = $("#contratoModalBorrar").val();
        result = MSExecuteOnServer('/CompraNet/BorrarFijacion', objConfirmado);
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
        objConfirmado.contratoId = $("#contratoModalBorrar").val();
        if ($("#estadoModalBorrar").val() == '5') {
            result = MSExecuteOnServer('/CompraNet/AnularContrato', objConfirmado);
        } else {
            result = MSExecuteOnServer('/CompraNet/BorrarContrato', objConfirmado);
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

function ObtenerDatosModalConError() {
    Finalizar(objFinalizado);
}

function ModalConfirmadoTilde(estado, contratoId, nroSAP, fijacionDePrecioContratoId, tipoId) {
    $(".modal-title-confirmadoTilde").empty();

    if (tipoId === '3') {
        $("#contratoModalConTilde").val(fijacionDePrecioContratoId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + contratoId);
    } else {
        $("#contratoModalConTilde").val(contratoId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + contratoId);
    }

    $("#tipoNegocioModalConTilde").val(tipoId);

    $("#modalConfirmadoTilde").modal('show');
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
    if ($("#tipoNegocioIdAmpliaciones").val() == 3) {
        objAmpliaciones.FijacionDePrecioContratoId = $("#contratoIdAmpliaciones").val();
    } else if ($("#tipoNegocioIdAmpliaciones").val() == 4 || $("#tipoNegocioIdAmpliaciones").val() == 5) {
        objAmpliaciones.Id = $("#contratoIdAmpliaciones").val();
    }

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

function ModalVisualizar(contrato, proveedor, corredor, fecha, desdeHasta, tipo, material, cantidad, precio, comercial, monedaId, precioMoneda,
    campana, provincia, localidad, nro_SAP, sustentablePrecio, sustentableMonedaId, dolarizadoFecha, pesificadoDias, informaSIO,
    trigoEspecial, status, Observacion, moneda, sustentableMoneda, destino, destinoDescripcion, cantidadCamiones, consignatario, planCanje, condicionFijacionId,
    cd, warrant, pagoDirectoVendedor, establecimientoPropio, boletoId, bolsaId, boletoDescripcion, bolsaDescripcion, desdeHastaFijacion, condicionFijacionDescripcion,
    clasificacionId, clasificacionDescripcion, standardDeCalidadDescripcion, calidadEspecialDescripcion, desdeFijacion, hastaFijacion, mercsFijacion,
    contratoCorredor, contratoVendedor, selCargoMOA, selCargoVendedor, tipoFason, posicion, operador, precioNeto, id, pizarra, zona) {
    $("#modalVisualizar").modal('show');
    visualizacionRowDoblePrecioCero("precioDivVisualizar", "comercialDivVisualizar", false);
    if (tipo === "FIJACION") {
        $(".noFason").show();
        $(".fason").hide();
        $(".noFijacion").hide();
    } else if (tipo === "FASON") {
        $(".noFason").hide();
        $(".fason").show();
        $("#visualizar_tipofason").text(tipoFason);
        $("#visualizar_posicion").text(posicion);
    } else if (tipo === "AGENTE COMPRAS") {
        $(".noAgente").hide();
        $(".agente").show();
        $("#visualizar_operador").text(operador);
        $("#visualizar_posicion").text(posicion);
    } else if (tipo === "A PRECIO") {
        $(".noFason").show();
        $(".noAgente").show();
        $(".noFijacion").show();
        $("#fechaDivVisualizar").show();
        $("fechaDesdeHastaDivVisualizar").show();
        $(".fason").hide();
    } else {
        visualizacionRowDoblePrecioCero("precioDivVisualizar", "comercialDivVisualizar", true);
        $(".noFason").show();
        $(".noAgente").show();
        $(".noFijacion").show();
        $("#fechaDivVisualizar").show();
        $("fechaDesdeHastaDivVisualizar").show();
        $(".fason").hide();
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
    }

    $(".modal-title-visualizar").empty();
    $(".modal-title-visualizar").append("Contrato N&deg; SAP: " + (nro_SAP != "null" ? nro_SAP : ""));
    $("#visualizar_proveedor").text(proveedor);
    $("#fechacontrato").text(desdeHasta);
    $("#visualizar_desdeHasta").text(fecha);
    $("#visualizar_tipo").text(tipo);
    $("#visualizar_comercial").text(comercial);
    $("#visualizar_comercial_AFijar").text(comercial);
    $("#visualizar_material").text(material);
    $("#visualizar_cantidad").text(isNaN(parseInt(cantidad)) ? "" : kendo.toString(parseInt(cantidad), "n0"));
    (precio != 0) ? $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2") + " " + moneda) : pizarra ? $("#visualizar_precio").text("Pizarra") : $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2"));
    campana !== "" ? $("#visualizar_campana").text(campana) : $("#visualizar_campana").text("null");

    visualizacionRowDoble("materialDivVisualizar", "visualizar_material", "campanaDivVisualizar", "visualizar_campana");


    var procedencia = "";
    var provinciaDat = provincia != "undefined" && provincia != "null" ? provincia : "";
    var localidadDat = localidad != "undefined" && localidad != "null" ? localidad : "";
    if (provinciaDat == "" || localidadDat == "") {
        procedencia = provinciaDat + localidadDat;
    }
    else if (provinciaDat != "" && localidadDat != "") {
        procedencia = localidadDat + ", " + provinciaDat;
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

    $("#visualizar_procedencia").text(procedencia);
    $("#visualizar_nro_SAP").text(nro_SAP != "undefined" && nro_SAP != "null" ? nro_SAP : "");
    $("#visualizar_observacion").text(Observacion != "undefined" && Observacion ? Observacion : "");

    $("#visualizar_condicionFijacion").text(condicionFijacionId);
    $("#visualizar_clasificacion").text(clasificacionDescripcion);
    sustentablePrecio !== "null" && sustentableMonedaId !== "null" ? $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + sustentableMonedaId) : $("#visualizar_sustentablePrecio").text("null");
    $("#visualizar_dolarizadoFecha").text(dolarizadoFecha);
    $("#visualizar_pesificadoDias").text(pesificadoDias);
    informaSIO === "true" ? $("#visualizar_informaSIO").text("Si") : $("#visualizar_informaSIO").text("null");
    mercsFijacion == "true" ? $("#visualizar_mercsDeposito").text("Si") : $("#visualizar_mercsDeposito").text("null");
    cd === "true" ? $("#visualizar_pago").text("CD") : (warrant === "true") ? $("#visualizar_pago").text("Warrant") : (pagoDirectoVendedor === "true") ? $("#visualizar_pago").text("Pago Directo Vendedor") : $("#visualizar_pago").text("null");
    boletoDescripcion === "Ninguno" || boletoDescripcion === null || boletoDescripcion === "" || boletoDescripcion === "undefined" ? ($("#visualizar_boleto").text("null") && $("#visualizar_bolsa").text("null")) : ($("#visualizar_boleto").text(boletoDescripcion) && $("#visualizar_bolsa").text(bolsaDescripcion));

    if (condicionFijacionDescripcion === "undefined" || condicionFijacionDescripcion === "null" || condicionFijacionDescripcion === "false" || condicionFijacionDescripcion === "") {
        $("#desdeHastaFijacionDivVisualizar").hide();
        $("#condicionFijacionDivVisualisar").hide();
    } else {
        $("#visualizar_desdeHastaFijacion").text(desdeHastaFijacion);
        $("#visualizar_condicionFijacion").text(condicionFijacionDescripcion);
    }

    planCanje === "true" ? $("#visualizar_planCanje").text("Si") : $("#visualizar_planCanje").text("null");
    consignatario === "true" ? $("#visualizar_consignatario").text("Consignatario") : $("#visualizar_consignatario").text("");
    cantidadCamiones !== 0 && cantidadCamiones !== "null" ? $("#visualizar_cantidadDeCamiones").text(cantidadCamiones) : $("#visualizar_cantidadDeCamiones").text("null");
    establecimientoPropio === "true" ? $("#visualizar_establecimiento").text("Propio") : establecimientoPropio === "false" ? $("#visualizar_establecimiento").text("Arrendado") : $("#visualizar_establecimiento").text("null");
    destinoDescripcion != "" ? $("#visualizar_destino").text(destinoDescripcion) : $("#visualizar_destino").text("null");
    visualizacionRowDoble("cantidadDivVisualizar", "visualizar_cantidad", "cantidadDeCamionesDivVisualizar", "visualizar_cantidadDeCamiones");
    visualizacionRowDoble("tipoDivVisualizar", "visualizar_tipo", "destinoDivVisualizar", "visualizar_destino");
    visualizacionRowDoble("mercsDepositoDivVisualizar", "visualizar_mercsDeposito", "pagoDivVisualizar", "visualizar_pago");
    visualizacionRowDoble("boletoDivVisualizar", "visualizar_boleto", "bolsaDivVisualizar", "visualizar_bolsa");
    visualizacionRowDoble("pesificadoDiasDivVisualizar", "visualizar_pesificadoDias", "informaSIODivVisualizar", "visualizar_informaSIO");
    visualizacionRowDoble("sustentableDivVisualizar", "visualizar_sustentablePrecio", "dolarizadoFechaDivVisualizar", "visualizar_dolarizadoFecha");
    visualizacionRowDoble("planCanjeDivVisualizar", "visualizar_planCanje", "establecimientoDivVisualizar", "visualizar_establecimiento");

    if (!sustentablePrecio === "undefined" || !sustentablePrecio === "null" || !sustentablePrecio === "false") {
        $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + (sustentableMoneda != "undefined" && sustentableMoneda != "null" ? sustentableMoneda : ""));
    }

    var iteracionesDescuentos = viewModel.DescuentosVisualizar.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.DescuentosVisualizar.pop();
    }

    var descuentosDto = MSExecuteOnServer('/CompraNet/TraerDescuentosPorContrato', { contratoId: contrato });

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

    var calidadesDto = MSExecuteOnServer('/CompraNet/TraerCalidadesPorContrato', { contratoId: contrato });

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

    if (zona !== "undefined" && zona !=="") {
        $("#visualizar-zona-girasol").text(zona);
    } else {
        $("#visualizar-zona-girasol").text("");
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
    $("#aperturaDePrecioVisualizarDiv").hide();
    $("#aperturaDePrecioVisualizarDivPrecioNeto").hide();

    if (precioNeto != 0) {
    $("#visualizar_aperturaFinancieroPrecioNeto").text(kendo.toString(parseFloat(precioNeto), "n2") + " " + moneda);
    var aperturaPrecio = MSExecuteOnServer('/CompraNet/TraerAperturaPrecioPorContrato', { contratoId: id, tipo: tipo });
        $.each(aperturaPrecio, function (key, concepto) {
            switch (concepto.ConceptoAperturaPrecioId) {
                case 1:
                    if (concepto.Importe) {
                        $("#aperturaFinancieroDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        $("#visualizar_aperturaFinanciero").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                    }
                    break;
                case 2:
                    if (concepto.Importe) {
                        $("#aperturaRedespachoDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        $("#visualizar_aperturaRedespacho").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                    }
                    break;
                case 3:
                    if (concepto.Importe || concepto.Porcentaje) {
                        $("#aperturaComisionesDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();

                        $("#visualizar_aperturaComisiones").text(concepto.Importe ? kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda : concepto.Porcentaje + "%");
                    }
                    break;
                case 4:
                    if (concepto.Importe || concepto.Porcentaje) {
                        $("#visualizar_aperturaBonificaciones").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda + " - " + concepto.Porcentaje + "%");
                    }

                    break;
            }

            visualizacionRowDoble("aperturaFinancieroDivVisualizar", "visualizar_aperturaFinanciero", "aperturaRedespachoDivVisualizar", "visualizar_aperturaRedespacho");
            visualizacionRowDoble("aperturaComisionesDivVisualizar", "visualizar_aperturaComisiones", "aperturaBonificacionesDivVisualizar", "visualizar_aperturaBonificaciones");
        });
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

function ModalBorrar(proveedor, id, tipoNegocio, fijacionDePrecioContratoId, fasonId, agenteId, estado) {
    $("#proveedor_a_borrar").text(proveedor);
    $("#proveedorBorrarDivVisualizar").show();
    $("#agenteBorrarDivVisualizar").hide();
    if (tipoNegocio === "3") {
        $("#contratoModalBorrar").val(fijacionDePrecioContratoId);
    } else if (tipoNegocio === "4") {
        $("#contratoModalBorrar").val(fasonId);
    } else if (tipoNegocio === "5") {
        $("#proveedorBorrarDivVisualizar").hide();
        $("#agenteBorrarDivVisualizar").show();
        $("#contratoModalBorrar").val(agenteId);
        $("#agente_a_borrar").text(agenteId);
    } else {
        $("#contratoModalBorrar").val(id);
        $("#estadoModalBorrar").val(estado);
    }

    $("#tipoNegocioModalBorrar").val(tipoNegocio);

    $("#modalBorrar").modal('show');
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
        ContratosPendientes: []
    });

    kendo.bind($("#tabla-descuentos-visualizar"), viewModel);
    kendo.bind($("#tabla-calidades-visualizar"), viewModel);
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
    if ($("#perfil").val() != "Mesa") {
        $(".comercial-contrato-pendiente").hide();
    }
}


function InicializarBuscador() {
    $("#buscadorFiltroZona").kendoDropDownList({
        autoWidth: true,
        optionLabel: "SELECCIONE UNA ZONA...",
        dataTextField: "Descripcion",
        dataValueField: "Descripcion",
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/BuscarGrupoDeCompras"
                }
            }

        },
        change: function () {

        }
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
}
