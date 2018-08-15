var viewModel;
var datosIniCrearContrato;

$(document).ready(function () {
    kendo.culture("es-AR");

    CreateGridInformeCompraNet();

    InicializarElementosModalPendiente();

    AutoRecargar();
    CrearViewModel()

    //mobile();

    //+ datos modal//

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
        if ($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial") {
            window.location.href = window.location.origin + "/CompraNet/CrearContrato";
        } else {
            MensInfo("No posee permisos para la carga de contratos");
        }
    });
});

function htmlEncode(value) {
    return $('<div/>').text(value.replace(/(\r\n|\n|\r)/gm, "")).html();
};

function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
};

function botonPendiente(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Editar" onclick="editarContrato(' +        
        "'" + dataItem.ContratoId + "'" +
        ')"><i class="fa ' + icono + '"></i></button>';
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
        "'" + dataItem.FijacionDePrecioContratoId + "'" +
        ')"><i class="fa ' + icono + ' conf"></i></button>';
}

function botonVisualizar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Visualizar" onclick="ModalVisualizar(' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.Proveedor + "'" + ',' +
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
        "'" + dataItem.TrigoEspecial + "'" + ',' +
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
        "'" + dataItem.StandardDeCalidad + "'" + ',' +
        "'" + dataItem.CalidadEspecial + "'" + ',' +
        "'" + dataItem.ValorCalidadEspecial + "'" + ',' +
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
        "'" + formatearFecha(dataItem.HastaFijacion) + "'" +
        ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
}

function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalBorrar(' +
        "'" + dataItem.Proveedor + "'" + ',' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" + ',' +
        "'" + dataItem.FijacionDePrecioContratoId + "'" +
        ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
}

function AutoRecargar() {
    setInterval(function () {
        if (document.getElementById('checkRecarga').checked == true) {
            recargarGrilla();
        }
    }, 60000);
};

function recargarGrilla() {
    $('#gridInformeCompraNet').data('kendoGrid').dataSource.read();
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
                url: '/CompraNet/BuscaDatosTabla',
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
            if (($("#perfil").val() !== "Mesa") && ($("#TieneEmpleadosACargo").val() !== "True"))
            {
                $("#gridInformeCompraNet").data("kendoGrid").hideColumn("Comercial");
            }
        },
        columns: [
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
                    }
                },
                filterable: { ui: createMultiSelectProveedor }
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
                        TipoNegocio: "A FIJAR",
                    }, {
                        TipoNegocio: "A PRECIO",
                    }, {
                        TipoNegocio: "FIJACION",
                    }]
                }, title: "Tipo", width: 70, attributes: {
                    "class": "mobile-sm"
                }
            },
            {
                field: "Material", type: "string", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz Duro Dentado",
                    }, {
                        Material: "Trigo Pan",
                    }, {
                        Material: "Semilla de Soja",
                    }]
                }, width: 95, attributes: {
                    "class": "mobile-xs"
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Material|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Material#'/></label></span>"
                }, template: "#=Material#"
            },
            {
                field: "Cantidad", type: "number", width: 70, format: "{0:n0}", attributes: {
                    "class": "mobile-xs"
                }
            },
            {
                field: "Ampliaciones", type: "number", width: 60, attributes: {
                    "class": "mobile-sm"
                }, template: function (dataItem) {
                    if (($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial") && dataItem.Estado == 2) {
                        return '' + dataItem.Ampliaciones + '<button data-toggle="tooltip" title="Ampliar"onclick="ModalAmpliaciones(' +
                            "'" + dataItem.ContratoId + "'" + ',' + "'" + dataItem.Ampliacion + "'" + ',' + "'" + dataItem.TipoNegocioId + "'" + "," + "'" + dataItem.FijacionDePrecioContratoId + "'" + ')"><i class="fa fa-plus aria-hidden="true"></i></button>';
                    } else if (dataItem.Estado == 1 || dataItem.Estado == 3) {
                        return dataItem.Ampliaciones;
                    } else {
                        return '';
                    }
                }
            },
            { field: "Precio", type: "number", width: 70, format: "{0:n2}", attributes: { "class": "mobile-xs" } },
            { field: "Campania", type: "string", title: "Campa&ntilde;a", width: 70, attributes: { "class": "mobile-md" } },
            { field: "ContratoSAP", type: "number", title: "N&deg; SAP", width: 70, attributes: { "class": "mobile-md" } },
            { field: "Fecha", type: "date", title: "Carga", width: 20, format: _DefaultDateTemplate, attributes: { "class": "mobile-xs" } },
            {
                field: "Comercial", type: "string", title: "Comercial", width: 70, filterable: { ui: createMultiSelectComercial }, attributes: { "class": "mobile-md" }
            },
            {
                field: "Estado_Contrato", sortable: false, title: "Estado", filterable: {
                    multi: true,
                    dataSource: [{
                        Estado_Contrato: "Pendiente",
                    }, {
                        Estado_Contrato: "Confirmado",
                    }, {
                        Estado_Contrato: "Con Error",
                    }, {
                        Estado_Contrato: "Oferta",
                    }, {
                        Estado_Contrato: "Finalizado",
                    }, {
                        Estado_Contrato: "Rechazado",
                    },]
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
                            botonVisualizar(dataItem, 'fa-eye pend');
                    }

                    if (dataItem.Estado == 2 && ($("#perfil").val() == "Mesa")) { //confirmado
                        return '<div class="status confirmado">Confirmado</div>' +
                            botonPendiente(dataItem, 'fa-pencil conf') +
                            botonFinalizado(dataItem, 'fa-flag-checkered conf') +
                            botonVisualizar(dataItem, 'fa-eye conf') +
                            botonBorrar(dataItem, 'fa-trash conf');
                    } else if (dataItem.Estado == 2) {
                        return '<div class="status confirmado">Confirmado</div>' +
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
                            botonVisualizar(dataItem, 'fa-eye ofe');
                    }

                    if (dataItem.Estado == 4 && ($("#perfil").val() == "Mesa")) { //error
                        return '<div class="status error">Con Error</div>' +
                            botonPendiente(dataItem, 'fa-pencil err') +
                            botonFinalizado(dataItem, 'fa-flag-checkered err') +
                            botonVisualizar(dataItem, 'fa-eye err') +
                            botonBorrar(dataItem, 'fa-trash err');
                    } else if (dataItem.Estado == 4) {
                        return '<div class="status error">Con Error</div>' +
                            botonFinalizado(dataItem, 'fa-flag-checkered err') +
                            botonVisualizar(dataItem, 'fa-eye err');
                    }

                    if (dataItem.Estado == 5) { //Finalizado
                        return '<div class="status finalizado">Finalizado</div>' +
                            botonVisualizar(dataItem, 'fa-eye fin');
                    }

                    if (dataItem.Estado == 6) { //Rechazado
                        return '<div class="status borrado">Rechazado</div>' +
                            botonVisualizar(dataItem, 'fa-eye bor');
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
        selectable: "row",

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
    };

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "ProveedorId", "/CompraNet/ListarProveedor");
    };

    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial");
    };
}

function editarContrato(contratoId) {
    window.location.href = window.location.origin + "/CompraNet/CrearContrato?ContratoId=" + contratoId;
}

function InicializarElementosModalPendiente() {
    kendo.culture("es-AR");

    $("#proveedorModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#proveedorModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#tipoModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UN TIPO DE NEGOCIO...",
        dataTextField: "Descripcion",
        dataValueField: "TipoNegocioId"
    });

    $("#tipoModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#tipoModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#materialModalPendiente").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#materialModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#materialModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#precioMonedaModalPendienteId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#precioMonedaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#precioMonedaModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#campanaModalPendienteId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA CAMPA&Ntilde;A...",
        dataTextField: "Descripcion",
        dataValueField: "CampanaId"
    });

    $("#campanaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#campanaModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#provinciaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA PROVINCIA...",
        dataTextField: "Nombre",
        dataValueField: "Provinciaid",
    });

    $("#provinciaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#provinciaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#LocalidadId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA LOCALIDAD...",
        dataTextField: "Nombre",
        dataValueField: "LocalidadId"
    });

    $("#LocalidadId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#LocalidadId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#sustentableMonedaModalPendienteId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "Descripcion",
        dataValueField: "MonedaId"
    });

    $("#sustentableMonedaModalPendienteId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#sustentableMonedaModalPendienteId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#standardCalidadIdModalPendiente").kendoDropDownList({
        optionLabel: "STANDARD",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#standardCalidadIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#standardCalidadIdModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    $("#calidadesEspecialesIdModalPendiente").kendoDropDownList({
        optionLabel: "CALIDAD",
        dataTextField: "Descripcion",
        dataValueField: "Id"
    });

    $("#calidadesEspecialesIdModalPendiente").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#calidadesEspecialesIdModalPendiente").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth();
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }

    var mesPost = hoy.getMonth() + 1;
    if (mesPost < 10) {
        mesPost = "0" + mesPost.toString();
    }

    if (dia < 10) {
        dia = "0" + dia.toString();
    }

    var date = new Date(anio, mes, dia);
    var dateHasta = new Date(anio, mesPost, dia);

    $("#fechaDesdeModalPendienteId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHastaModalPendienteId").kendoDatePicker({
        value: dateHasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaEntregaModalPendienteId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHoyId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#dolarizadoFechaModalPendienteId").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#fechaDesdeTopeIdModalPendiente").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHastaTopeIdModalPendiente").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    $("#modificarContratoPendiente").click(function () {
        ObtenerDatosModalPendiente();
    });

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

    $("#sustentableModalPendienteId").click(function () {
        if ($(this).is(':checked')) {
            $("#sustentableDivModalPendiente").show();
        }
        else {
            $("#sustentableDivModalPendiente").hide();
            $("#sustentablePrecioModalPendienteId").val("");
            $("#sustentableMonedaModalPendienteId").val("");
        }
    });

    $("#dolarizadoModalPendienteId").click(function () {
        if ($(this).is(':checked')) {
            $("#dolarizadoDivModalPendiente").show();
        }
        else {
            $("#dolarizadoDivModalPendiente").hide();
            $("#dolarizadoFechaModalPendienteId").val("");
        }
    });

    $("#pesificadoModalPendienteId").click(function () {
        if ($(this).is(':checked')) {
            $("#pesificadoDivModalPendiente").show();
        }
        else {
            $("#pesificadoDivModalPendiente").hide();
            $("#pesificadoDiasModalPendienteId").val("");
        }
    });

    $('select[id="materialModalPendiente"]').change(function () {
        cargarPorMaterial($(this).val());
    });

    $("#boletoConfirmaIdModalPendiente").click(function () {
        if ($(this).is(':checked')) {
            $("#BolsaConfirmaDivModalPendiente").show();
            $("#BolsaFisicoDivModalPendiente").hide();
            $("#boletoFisicoIdModalPendiente").prop("checked", false);
            $("#boletoNingunoIdModalPendiente").prop("checked", false);
            $("#bolsaFisicoIdModalPendiente").data("kendoDropDownList").value("");
        }
        else {
            $("#BolsaConfirmaDivModalPendiente").hide();
            $("#bolsaConfirmaIdModalPendiente").data("kendoDropDownList").value("");
        }
    });

    $("#boletoFisicoIdModalPendiente").click(function () {
        if ($(this).is(':checked')) {
            $("#BolsaFisicoDivModalPendiente").show();
            $("#BolsaConfirmaDivModalPendiente").hide();
            $("#boletoConfirmaIdModalPendiente").prop("checked", false);
            $("#boletoNingunoIdModalPendiente").prop("checked", false);
            $("#bolsaConfirmaIdModalPendiente").data("kendoDropDownList").value("");
        }
        else {
            $("#BolsaFisicoDivModalPendiente").hide();
            $("#bolsaFisicoIdModalPendiente").data("kendoDropDownList").value("");
        }
    });
    $("#standardCalidadIdModalPendiente").change(function () {
        if ($("#standardCalidadIdModalPendiente").data("kendoDropDownList").value() == 2) {
            $("#especialesIdModalPendiente").show();
        } else {
            $("#especialesIdModalPendiente").hide();
            $("#calidadesEspecialesIdModalPendiente").data("kendoDropDownList").value("");
            $("#valorEspecialesIdModalPendiente").val("");
        }
    })

    $("#boletoNingunoIdModalPendiente").click(function () {
        if ($(this).is(':checked')) {
            $("#BolsaConfirmaDivModalPendiente").hide();
            $("#BolsaFisicoDivModalPendiente").hide();
            $("#boletoFisicoIdModalPendiente").prop("checked", false);
            $("#boletoConfirmaIdModalPendiente").prop("checked", false);
            $("#bolsaFisicoIdModalPendiente").data("kendoDropDownList").value("");
            $("#bolsaConfirmaIdModalPendiente").data("kendoDropDownList").value("");
        }
    });
    $("#CDIdModalPendiente").click(function () {
        $("#WarrantIdModalPendiente").prop("checked", false);
        $("#pagoDirectoIdModalPendiente").prop("checked", false);
    });

    $("#WarrantIdModalPendiente").click(function () {
        $("#CDIdModalPendiente").prop("checked", false);
        $("#pagoDirectoIdModalPendiente").prop("checked", false);
    });

    $("#pagoDirectoIdModalPendiente").click(function () {
        $("#CDIdModalPendiente").prop("checked", false);
        $("#WarrantIdModalPendiente").prop("checked", false);
    });

    $("#establecimientoPropioIdModalPendiente").click(function () {
        $("#establecimientoArrendadoIdModalPendiente").prop("checked", false);
    });
    $("#establecimientoArrendadoIdModalPendiente").click(function () {
        $("#establecimientoPropioIdModalPendiente").prop("checked", false);
    });
}

function cargarPorMaterial(material) {
    var resultGrano = MSExecuteOnServer('/CompraNet/TraerCampanaPorMaterial', { MaterialId: material });
    viewModel.set("CampanaComboModalPendiente", resultGrano);

    var calidadGrano = MSExecuteOnServer('/CompraNet/TraerCalidadesPorMaterial', { MaterialId: material });
    viewModel.set("EspecialesComboModalPendiente", calidadGrano);
}

function modalPendiente(observacion, estado, contratoId, proveedor, fechaDesde, fechaHasta, fecha, tipoId, MaterialId, cantidad, ampliaciones, precio, MonedaId, campana, provincia, localidad, comercial, nroSAP, base, sustentablePrecio, sustentableMoneda, dolarizadoFecha, pesificadoDias, noInformaSIO, trigoEspecial, fijacionId, clasificacion, destino, planCanje, consignatario, cantidadCamiones, boleto, bolsa, desdeFijacion, hastaFijacion, condicion, cd, warrant, pagoDirectoVendedor, standardDeCalidad, calidadEspecial, valorCalidadEspecial, establecimientoPropio) {
    LimpiarPendiente();

    if (tipoId === "3") {
        $("#modalPendiente .noFijacion").hide();
    } else {
        $("#modalPendiente .noFijacion").show();
    }

    $(".modal-title-pendiente").empty();
    $(".modal-title-pendiente").append("Contrato Nro Sap: " + nroSAP);

    if (estado == "2") {
        $(".confirmado-modal").show();
        $(".pendiente-modal").hide();
        $(".conerror-modal").hide();
        $(".oferta-modal").hide();
    }
    else if (estado == "3") {
        $(".oferta-modal").show();
        $(".pendiente-modal").hide();
        $(".conerror-modal").hide();
        $(".confirmado-modal").hide();
    }
    else if (estado == "4") {
        $(".conerror-modal").show();
        $(".pendiente-modal").hide();
        $(".oferta-modal").hide();
        $(".confirmado-modal").hide();
    }
    else {
        $(".pendiente-modal").show();
        $(".oferta-modal").hide();
        $(".conerror-modal").hide();
        $(".confirmado-modal").hide();
    }

    $("#contratoIdModal").val(contratoId);
    $("#fijacionIdModal").val(fijacionId);

    var oProveedor = MSExecuteOnServer('/CompraNet/BuscarCuitPorId', { ProveedorId: proveedor });
    $("#buscadorProveedorModalPendiente").val(oProveedor.RazonSocial + ' ' + '(' + oProveedor.CUIT + ')');
    $("#fechaDesdeModalPendienteId").val(fechaDesde);
    $("#fechaHastaModalPendienteId").val(fechaHasta);
    $("#fechaHoyId").val(fecha);
    $("#tipoModalPendienteId").data("kendoDropDownList").value(tipoId);
    $("#materialModalPendiente").data("kendoDropDownList").value(MaterialId);
    $("#observacionModalPendienteId").val(observacion);
    $("#cantidadModalPendienteId").val(cantidad);
    $("#precioModalPendienteId").val(precio);
    $("#precioMonedaModalPendienteId").data("kendoDropDownList").value(MonedaId);
    $("#provinciaId").data("kendoDropDownList").value(provincia);
    $("#clasificacionModalPendienteId").data("kendoDropDownList").value(clasificacion);
    $("#destinoModalPendienteId").data("kendoDropDownList").value(destino);
    if (cantidadCamiones !== "null" && cantidadCamiones !== "undefined" && cantidadCamiones !== 0) {
        $("#cantidadCamionesModalPendienteId").val(cantidadCamiones);
    }
    (planCanje == "true") ? $("#planCanjeModalPendienteId").prop("checked", true) : $("#planCanjeModalPendienteId").prop("checked", false);
    (consignatario == "true") ? $("#consignatarioModalPendienteId").prop("checked", true) : $("#consignatarioModalPendienteId").prop("checked", false);
    CargarLocalidadPorProvincia(provincia);
    if (!(localidad == "null" || localidad == "undefined")) $("#LocalidadId").data("kendoDropDownList").value(localidad);
    if (!(comercial == "null" || comercial == "undefined")) $("#comercialModalPendienteId").data("kendoDropDownList").value(comercial);

    cargarPorMaterial(MaterialId);
    $("#campanaModalPendienteId").data("kendoDropDownList").value(campana);

    if (base == "true") {
        $("#baseModalPendienteId").prop("checked", true);
    } else {
        $("#baseModalPendienteId").prop("checked", false);
    }

    if (!(sustentablePrecio == "null" || sustentablePrecio == "undefined" || sustentablePrecio == 0)) {
        $("#sustentablePrecioModalPendienteId").val(sustentablePrecio);
        $("#sustentableMonedaModalPendienteId").data("kendoDropDownList").value(sustentableMoneda);
        $("#sustentableModalPendienteId").prop("checked", true);
        $("#sustentableDivModalPendiente").show();
    }

    if (!(dolarizadoFecha == "null" || dolarizadoFecha == "undefined" || dolarizadoFecha == "")) {
        $("#dolarizadoModalPendienteId").prop("checked", true);
        $("#dolarizadoDivModalPendiente").show();
        $("#dolarizadoFechaModalPendienteId").val(dolarizadoFecha);
    }

    if (!(pesificadoDias == "null" || pesificadoDias == "undefined" || pesificadoDias == "")) {
        $("#pesificadoModalPendienteId").prop("checked", true);
        $("#pesificadoDivModalPendiente").show();
        $("#pesificadoDiasModalPendienteId").val(pesificadoDias);
    }

    if (noInformaSIO == "true") {
        $("#noInformaSioModalPendienteId").prop("checked", true);
    } else {
        $("#noInformaSioModalPendienteId").prop("checked", false);
    }

    if (trigoEspecial == "true") {
        $("#trigoEspecialModalPendienteId").prop("checked", true);
    } else {
        $("#trigoEspecialModalPendienteId").prop("checked", false);
    }
    if (boleto == 1) {
        $("#boletoConfirmaIdModalPendiente").prop("checked", true);
        $("#BolsaConfirmaDivModalPendiente").show();
        $("#bolsaConfirmaIdModalPendiente").data("kendoDropDownList").value(bolsa);
    } else if (boleto == 2) {
        $("#boletoFisicoIdModalPendiente").prop("checked", true);
        $("#BolsaFisicoDivModalPendiente").show();
        $("#bolsaFisicoIdModalPendiente").data("kendoDropDownList").value(bolsa);
    } else if (boleto == 3) {
        $("#boletoNingunoIdModalPendiente").prop("checked", true);
    }

    if (!(desdeFijacion == "null" || desdeFijacion == "undefined" || desdeFijacion == "")) {
        $("#fechaDesdeTopeIdModalPendiente").val(desdeFijacion);
    } else {
        $("#fechaDesdeTopeIdModalPendiente").val("");
    }
    if (!(hastaFijacion == "null" || hastaFijacion == "undefined" || hastaFijacion == "")) {
        $("#fechaHastaTopeIdModalPendiente").val(desdeFijacion);
    } else {
        $("#fechaDesdeTopeIdModalPendiente").val("");
    }
    $("#condicionFijacionIdModalPendiente").data("kendoDropDownList").value(condicion);
    $("#modalPendiente").modal('show');
    cd == "true" ? $("#CDIdModalPendiente").prop("checked", true) : $("#CDIdModalPendiente").prop("checked", false);
    warrant == "true" ? $("#WarrantIdModalPendiente").prop("checked", true) : $("#WarrantIdModalPendiente").prop("checked", false);
    pagoDirectoVendedor == "true" ? $("#pagoDirectoIdModalPendiente").prop("checked", true) : $("#pagoDirectoIdModalPendiente").prop("checked", false);

    $("#standardCalidadIdModalPendiente").data("kendoDropDownList").value(standardDeCalidad);
    if (standardDeCalidad == 2) {
        $("#especialesIdModalPendiente").show();
        $("#calidadesEspecialesIdModalPendiente").data("kendoDropDownList").value(calidadEspecial);
        $("#valorEspecialesIdModalPendiente").val(valorCalidadEspecial);
    }
    establecimientoPropio == "true" ? $("#establecimientoPropioIdModalPendiente").prop("checked", true) : establecimientoPropio == "false" ? $("#establecimientoArrendadoIdModalPendiente").prop("checked", true) : false;

    var iteraciones = viewModel.Descuentos.length;
    for (var i = 0; i < iteraciones; i++) {
        viewModel.Descuentos.pop();
    }

    var descuentosDto = MSExecuteOnServer('/CompraNet/TraerDescuentosPorContrato', { contratoId: contratoId });

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
}

$("#modalPendiente #tipoModalPendienteId").on("change", function () {
    if ($("#modalPendiente #tipoModalPendienteId").val() == 3) {
        $("#modalPendiente .noFijacion").hide();
    } else {
        $("#modalPendiente .noFijacion").show();
    }
});

function ObtenerDatosModalPendiente() {
    var objPendiente = {};

    if ($("#buscadorProveedorModalPendiente").val() != "") {
        var cuitAux = $("#buscadorProveedorModalPendiente").val().split('(');
        var cuit = cuitAux[1].split(')');

        objPendiente.ProveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0] });
    }

    objPendiente.fijacionDePrecioContratoId = $("#fijacionIdModal").val();
    objPendiente.contratoId = $("#contratoIdModal").val();
    objPendiente.MaterialId = $("#materialModalPendiente").val();
    objPendiente.TipoNegocioId = $("#tipoModalPendienteId").val();
    objPendiente.Cantidad = $("#cantidadModalPendienteId").val();
    objPendiente.Observacion = $("#observacionModalPendienteId").val();
    objPendiente.Precio = $("#precioModalPendienteId").val();
    objPendiente.FechaEntrega = $("#fechaHastaModalPendienteId").val();
    objPendiente.CampanaId = $("#campanaModalPendienteId").val();
    objPendiente.FechaDesde = $("#fechaDesdeModalPendienteId").val();
    objPendiente.FechaHasta = $("#fechaHastaModalPendienteId").val();
    objPendiente.MonedaId = $("#precioMonedaModalPendienteId").val();
    objPendiente.Fecha = $("#fechaHoyId").val();
    objPendiente.ComercialId = $("#comercialModalPendienteId").val();
    objPendiente.ProvinciaId = $("#provinciaId").val();
    objPendiente.LocalidadId = $("#LocalidadId").val();
    objPendiente.Base = $("#baseModalPendienteId").is(":checked") ? true : false;
    objPendiente.ImporteSustentable = $("#sustentablePrecioModalPendienteId").val();
    objPendiente.MonedaSustentableId = $("#sustentableMonedaModalPendienteId").val();
    objPendiente.FechaDolarizado = $("#dolarizadoFechaModalPendienteId").val();
    objPendiente.DiasPesificado = $("#pesificadoDiasModalPendienteId").val();
    objPendiente.NoInformaSio = $("#noInformaSioModalPendienteId").is(":checked") ? true : false;
    objPendiente.TrigoEspecial = $("#trigoEspecialModalPendienteId").is(":checked") ? true : false;
    objPendiente.EstadoId = $("#baseId").is(":checked") ? "3" : "1";
    objPendiente.ClasificacionId = $("#clasificacionModalPendienteId").val();
    objPendiente.DestinoId = $("#destinoModalPendienteId").val();
    objPendiente.CantidadCamiones = $("#cantidadCamionesModalPendienteId").val();
    objPendiente.PlanCanje = $("#planCanjeModalPendienteId").is(":checked") ? true : false;
    objPendiente.Consignatario = $("#consignatarioModalPendienteId").is(":checked") ? true : false;
    objPendiente.EstablecimientoPropio = ($("#establecimientoPropioIdModalPendiente").is(":checked")) ? true : ($("#establecimientoArrendadoIdModalPendiente").is(":checked")) ? false : null;
    objPendiente.DesdeFijacion = $("#fechaDesdeTopeIdModalPendiente").val();
    objPendiente.HastaFijacion = $("#fechaHastaTopeIdModalPendiente").val();
    objPendiente.CondicionFijacionId = $("#condicionFijacionIdModalPendiente").val();
    objPendiente.DestinoId = $("#destinoModalPendienteId").val();
    objPendiente.CD = $("#CDIdModalPendiente").is(":checked") ? true : false;
    objPendiente.Warrant = $("#WarrantIdModalPendiente").is(":checked") ? true : false;
    objPendiente.PagoDirectoVendedor = $("#pagoDirectoIdModalPendiente").is(":checked") ? true : false;
    objPendiente.StandardDeCalidadId = $("#standardCalidadIdModalPendiente").val();
    objPendiente.CalidadEspecialId = $("#calidadesEspecialesIdModalPendiente").val();
    objPendiente.ValorCalidadEspecial = $("#valorEspecialesIdModalPendiente").val();
    if ($("#boletoConfirmaIdModalPendiente").is(':checked')) {
        objPendiente.BoletoId = 1;
        objPendiente.BolsaId = $("#bolsaConfirmaIdModalPendiente").val();
    }
    else if ($("#boletoFisicoIdModalPendiente").is(':checked')) {
        objPendiente.BoletoId = 2;
        objPendiente.BolsaId = $("#bolsaFisicoIdModalPendiente").val();
    }
    else {
        objPendiente.BoletoId = 3;
        objPendiente.BolsaId = 0;
    }
    objPendiente.Descuentos = viewModel.Descuentos

    ModificarContrato(objPendiente);
}

function ModificarContrato(modificarContrato) {
    if (modificarContrato.TipoNegocioId == 3) {
        var result = MSExecuteOnServer('/CompraNet/GrabarFijacion', modificarContrato);

        if (result != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr(result.Errores[0].Message);
            }
            else {
                MensInfo("Se ha modificado la fijacion con exito");
                recargarGrilla();
            }
        }
    }
    else {
        var result = MSExecuteOnServer('/CompraNet/GrabarContrato', modificarContrato);

        if (result != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr(result.Errores[0].Message);
            }
            else {
                MensInfo("Se ha modificado el Contrato con exito");
                recargarGrilla();
            }
        }
    }
}

function LimpiarPendiente() {
    $("#buscadorProveedorModalPendiente").val("");
    $("#baseModalPendienteId").prop('checked', false);
    $("#sustentablePrecioModalPendienteId").val("");
    $("#sustentableMonedaModalPendienteId").val("");
    $("#dolarizadoFechaModalPendienteId").val("");
    $("#pesificadoDiasModalPendienteId").val("");
    $("#sustentableModalPendienteId").prop('checked', false);
    $("#dolarizadoModalPendienteId").prop('checked', false);
    $("#pesificadoModalPendienteId").prop('checked', false);
    $("#noInformaSioModalPendienteId").prop('checked', false);
    $("#trigoEspecialModalPendienteId").prop('checked', false);
    $("#sustentableDivModalPendiente").hide();
    $("#dolarizadoDivModalPendiente").hide();
    $("#pesificadoDivModalPendiente").hide();
    $('.datosEditarAdicionalesPendiente').hide();
    $('.masDatosPendiente').show();
    $('.datosEditarPendiente').show();
    $("#boletoConfirmaIdModalPendiente").prop('checked', false);
    $("#bolsaConfirmaIdModalPendiente").val("")
    $("#boletoFisicoIdModalPendiente").prop('checked', false);
    $("#bolsaFisicoIdModalPendiente").val("")
    $("#boletoNingunoIdModalPendiente").prop('checked', false);
    $("#BolsaFisicoDivModalPendiente").hide();
    $("BolsaConfirmaDivModalPendiente").hide();
    $("#fechaDesdeTopeIdModalPendiente").val("");
    $("#fechaHastaTopeIdModalPendiente").val("");
    $("#cantidadCamionesModalPendienteId").val("");
}

function ModalFinalizado(contratoId, tipoId, fijacionDePrecioContratoId) {
    $(".modal-title-finalizado").empty();

    if (tipoId === "3") {
        $("#contratoModalFinalizado").val(fijacionDePrecioContratoId);
        $(".modal-title-finalizado").append("Fijaci&oacute;n Nro: " + fijacionDePrecioContratoId);
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
    } else {
        objFinalizado.contratoId = $("#contratoModalFinalizado").val();
    }

    Finalizar(objFinalizado);
}

function Finalizar(finalizarContratoFijacion) {
    if ($("#tipoNegocioModalFinalizado").val() === "3") {
        var result = MSExecuteOnServer('/CompraNet/FinalizarFijacion', finalizarContratoFijacion);
    } else {
        var result = MSExecuteOnServer('/CompraNet/FinalizarContrato', finalizarContratoFijacion);
    }

    if (result != null) {
        if (result.Errores != null) {
            if (ExistsErrorMessages(result.Errores)) {
                MensErr("No se ha podido finalizar el contrato correctamente: " + result.Errores[0].Message);
            }
        }

        else {
            MensInfo("Se ha finalizado el Contrato con exito");
        }
    }
    recargarGrilla();
}

function ReenviarMails(reenviarMail) {
    var result = MSExecuteOnServer('/CompraNet/ReenviarMails', reenviarMail);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            MensInfo("Se ha reenviado el Mail con exito");
        }
    }
}

function ModalConfirmado(contratoId, proveedor, fechaDesdeHasta, tipo, material, cantidad, precio, campana, provincia, localidad, nroSAP, sustentablePrecio, sustentableMoneda, dolarizadoFecha, pesificadoDias, informaSIO, trigoEspecial) {
    $(".modal-title-confirmado").empty();
    $(".modal-title-confirmado").append("Contrato DataAgro: " + contratoId);
    $("#contratoModalConfirmado").val(contratoId);
    $("#modalConfirmado").modal('show');
}

function ObtenerDatosModalConfirmado() {
    var objConfirmado = {};

    if ($("#tipoNegocioModalConTilde").val() === '3') {
        objConfirmado.FijacionDePrecioContratoId = $("#contratoModalConTilde").val();
    } else {
        objConfirmado.contratoId = $("#contratoModalConTilde").val();
    }
    Confirmar(objConfirmado);
}

function Confirmar(confirmarContratoFijacion) {
    if ($("#tipoNegocioModalConTilde").val() === '3') {
        var result = MSExecuteOnServer('/CompraNet/ConfirmarFijacion', confirmarContratoFijacion);
    } else {
        var result = MSExecuteOnServer('/CompraNet/ConfirmarContrato', confirmarContratoFijacion);
    }

    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
        MensInfo("Se ha confirmado el Contrato con exito");
    }
}

function ObtenerDatosModalBorrado() {
    var objConfirmado = {};

    if ($("#tipoNegocioModalBorrar").val() === '3') {
        objConfirmado.FijacionDePrecioContratoId = $("#contratoModalBorrar").val();
        var result = MSExecuteOnServer('/CompraNet/BorrarFijacion', objConfirmado);
    } else {
        objConfirmado.contratoId = $("#contratoModalBorrar").val();
        var result = MSExecuteOnServer('/CompraNet/BorrarContrato', objConfirmado);
    }

    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
        MensInfo("Se ha borrado el Contrato con exito");
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

function ModalAmpliaciones(contrato, ampliacion, tipoNegocio, FijacionDePrecioContratoId) {
    $(".modal-title-ampliaciones").empty();
    $(".modal-title-ampliaciones").append("Contrato DataAgro: " + contrato);

    if (tipoNegocio === "3") {
        $("#contratoIdAmpliaciones").val(FijacionDePrecioContratoId);
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
    }

    GuardarAmpliacion(objAmpliaciones);
}

function GuardarAmpliacion(ampliacion) {
    if ($("#tipoNegocioIdAmpliaciones").val() === "3") {
        var result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionFijacion', ampliacion);
    } else {
        var result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionContrato', ampliacion);
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

function ModalVisualizar(contrato, proveedor, fecha, desdeHasta, tipo, material, cantidad, precio, comercial, monedaId, precioMoneda, campana, provincia, localidad, nro_SAP, sustentablePrecio, sustentableMonedaId, dolarizadoFecha, pesificadoDias, informaSIO, trigoEspecial, status, Observacion, moneda, sustentableMoneda, destino, destinoDescripcion, cantidadCamiones, consignatario, planCanje, condicionFijacionId, cd, warrant, pagoDirectoVendedor, standardDeCalidadId, calidadEspecialId, valorCalidadEspecial, establecimientoPropio, boletoId, bolsaId, boletoDescripcion, bolsaDescripcion, desdeHastaFijacion, condicionFijacionDescripcion, clasificacionId, clasificacionDescripcion, standardDeCalidadDescripcion, calidadEspecialDescripcion, desdeFijacion, hastaFijacion) {
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
    }

    $(".modal-title-visualizar").empty();
    $(".modal-title-visualizar").append("Contrato N&deg; SAP: " + (nro_SAP != "null" ? nro_SAP : ""));
    $("#visualizar_proveedor").text(proveedor);
    $("#fechacontrato").text(desdeHasta);
    $("#visualizar_desdeHasta").text(fecha);
    $("#visualizar_tipo").text(tipo);
    $("#visualizar_comercial").text(comercial);
    $("#visualizar_material").text(material);
    $("#visualizar_cantidad").text(isNaN(parseInt(cantidad)) ? "" : kendo.toString(parseInt(cantidad), "n0"));
    $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2") + " " + moneda);
    $("#visualizar_campana").text(campana);

    var procedencia = "";
    var provinciaDat = provincia != "undefined" && provincia != "null" ? provincia : "";
    var localidadDat = localidad != "undefined" && localidad != "null" ? localidad : "";
    if (provinciaDat == "" || localidadDat == "") {
        procedencia = provinciaDat + localidadDat;
    }
    else if (provinciaDat != "" && localidadDat != "") {
        procedencia = localidadDat + ", " + provinciaDat;
    }

    $("#visualizar_procedencia").text(procedencia);
    $("#visualizar_nro_SAP").text(nro_SAP != "undefined" && nro_SAP != "null" ? nro_SAP : "");
    $("#visualizar_observacion").text(Observacion != "undefined" && Observacion ? Observacion : "");
    $("#visualizar_destino").text(destinoDescripcion);
    $("#visualizar_condicionFijacion").text(condicionFijacionId);
    $("#visualizar_clasificacion").text(clasificacionDescripcion);
    (sustentablePrecio !== "null" && sustentableMonedaId !== "null") ? $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + sustentableMonedaId) : $("#visualizar_sustentablePrecio").text("null");
    $("#visualizar_dolarizadoFecha").text(dolarizadoFecha);
    $("#visualizar_pesificadoDias").text(pesificadoDias);
    (informaSIO === "true") ? $("#visualizar_informaSIO").text("Si") : $("#visualizar_informaSIO").text("null");
    (trigoEspecial === "true") ? $("#visualizar_trigoEspecial").text("Si") : $("#visualizar_trigoEspecial").text("null");
    (cd === "true") ? $("#visualizar_pago").text("CD") : (warrant === "true") ? $("#visualizar_pago").text("Warrant") : (pagoDirectoVendedor === "true") ? $("#visualizar_pago").text("Pago Directo Vendedor") : $("#visualizar_pago").text("null");
    (boletoDescripcion === "Ninguno" || boletoDescripcion === "null") ? ($("#visualizar_boleto").text("null") && $("#visualizar_bolsa").text("null")) : ($("#visualizar_boleto").text(boletoDescripcion) && $("#visualizar_bolsa").text(bolsaDescripcion));
    $("#visualizar_standard").text(standardDeCalidadDescripcion);

    if (condicionFijacionDescripcion === "undefined" || condicionFijacionDescripcion === "null" || condicionFijacionDescripcion === "false" || condicionFijacionDescripcion === "") {
        $("#desdeHastaFijacionDivVisualizar").hide();
        $("#condicionFijacionDivVisualisar").hide();
    }

    (planCanje === "true") ? $("#visualizar_planCanje").text("Si") : $("#visualizar_planCanje").text("null");
    (consignatario === "true") ? $("#visualizar_consignatario").text("Si") : $("#visualizar_consignatario").text("null");
    (cantidadCamiones !== 0 && cantidadCamiones !== "null") ? $("#visualizar_cantidadDeCamiones").text(cantidadCamiones) : $("#visualizar_cantidadDeCamiones").text("N/A");
    (establecimientoPropio === "true") ? $("#visualizar_establecimiento").text("Propio") : (establecimientoPropio === "false") ? $("#visualizar_establecimiento").text("Arrendado") : $("#visualizar_establecimiento").text("N/A");   
    (calidadEspecialId !== "null") ? $("#visualizar_calidades").text(calidadEspecialDescripcion + "    " + valorCalidadEspecial) : $("#visualizar_calidades").text("null");

    visualizacionRowDoble("trigoEspecialDivVisualizar", "visualizar_trigoEspecial", "pagoDivVisualizar", "visualizar_pago");
    visualizacionRowDoble("boletoDivVisualizar", "visualizar_boleto", "bolsaDivVisualizar", "visualizar_bolsa");
    visualizacionRowDoble("pesificadoDiasDivVisualizar", "visualizar_pesificadoDias", "informaSIODivVisualizar", "visualizar_informaSIO");
    visualizacionRowDoble("sustentableDivVisualizar", "visualizar_sustentablePrecio", "dolarizadoFechaDivVisualizar", "visualizar_dolarizadoFecha");
    visualizacionRowDoble("standardDivVisualizar", "visualizar_standard", "calidadesDivVisualizar", "visualizar_calidades");
    visualizacionRowDoble("planCanjeDivVisualizar", "visualizar_planCanje", "consignatarioDivVisualizar", "visualizar_consignatario");

    visualizacionRowSimple(condicionFijacionDescripcion, "condicionFijacionDivVisualisar", "visualizar_condicionFijacion");

    if (desdeFijacion !== "undefined" && desdeFijacion !== "null") {
        visualizacionRowSimple(desdeHastaFijacion, "desdeHastaFijacionDivVisualizar", "visualizar_desdeHastaFijacion");
    } else {
        visualizacionRowSimple("null", "desdeHastaFijacionDivVisualizar", "visualizar_desdeHastaFijacion");
    }

    if (!sustentablePrecio === "undefined" || !sustentablePrecio === "null" || !sustentablePrecio === "false") {
        $("#visualizar_sustentablePrecio").text(sustentablePrecio + " " + (sustentableMoneda != "undefined" && sustentableMoneda != "null" ? sustentableMoneda : ""));
    }

    $("#modalVisualizar").modal('show');
    var iteraciones = viewModel.DescuentosVisualizar.length;
    for (var i = 0; i < iteraciones; i++) {
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
            ContratoId: descuento.contratoId,
        };
        viewModel.DescuentosVisualizar.push(descuentoKendo);
    });
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

function ModalBorrar(proveedor, id, tipoNegocio, fijacionDePrecioContratoId) {
    $("#proveedor_a_borrar").text(proveedor);

    if (tipoNegocio === "3") {
        $("#contratoModalBorrar").val(fijacionDePrecioContratoId);
    } else {
        $("#contratoModalBorrar").val(id);
    }

    $("#tipoNegocioModalBorrar").val(tipoNegocio);

    $("#modalBorrar").modal('show');
}