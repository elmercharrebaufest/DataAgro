var viewModel;
var datosIniCrearContrato;

$(document).ready(function () {

    kendo.culture("es-AR");

    CreateGridInformeCompraNet();

    InicializarElementosModalPendiente();

    AutoRecargar();

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
    return $('<div/>').text(value.replace(/(\r\n|\n|\r)/gm, "") ).html();
};

function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
};

function botonPendiente(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Editar" onclick="modalPendiente(' +
        "'" + htmlEncode(dataItem.Observacion) + "'" + ',' +
        "'" + dataItem.Estado + "'" + ',' +
        "'" + dataItem.ContratoId + "'" + ',' +
        "'" + dataItem.ProveedorId + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaDesde) + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaHasta) + "'" + ',' +
        "'" + formatearFecha(dataItem.Fecha) + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaEntrega) + "'" + ',' +
        "'" + dataItem.TipoNegocioId + "'" + ',' +
        "'" + dataItem.MaterialId + "'" + ',' +
        "'" + dataItem.Cantidad + "'" + ',' +
        "'" + dataItem.Ampliaciones + "'" + ',' +
        "'" + dataItem.Precio + "'" + ',' +
        "'" + dataItem.MonedaId + "'" + ',' +
        "'" + dataItem.CampanaId + "'" + ',' +
        "'" + dataItem.ProvinciaId + "'" + ',' +
        "'" + dataItem.LocalidadId + "'" + ',' +
        "'" + dataItem.ComercialId + "'" + ',' +
        "'" + dataItem.ContratoSAP + "'" + ',' +
        "'" + dataItem.Base + "'" + ',' +
        "'" + dataItem.Importe_Sustentable + "'" + ',' +
        "'" + dataItem.MonedaId_Sustentable + "'" + ',' +
        "'" + dataItem.Fecha_Dolarizado + "'" + ',' +
        "'" + dataItem.PesificadoDias + "'" + ',' +
        "'" + dataItem.NoInformaSIO + "'" + ',' +
        "'" + dataItem.TrigoEspecial + "'" + ',' +
        "'" + dataItem.FijacionDePrecioContratoId + "'" +
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
    "'" + dataItem.Moneda_Sustentable + "'" +
    ')"><i class="fa ' + icono +' aria-hidden="true"></i></button>' ;
}

function botonBorrar(dataItem, icono) {
    return '<button data-toggle="tooltip" title="Borrar" onclick="ModalBorrar(' +
            "'" + dataItem.Proveedor + "'" + ',' +
            ')"><i class="fa  '+ icono + '" aria-hidden="true"></i></button>';
}

function AutoRecargar() {
    setInterval(function () {
        if (document.getElementById('checkRecarga').checked == true) {
            recargarGrilla();
        }
    }, 30000);
};

function recargarGrilla() {
$('#gridInformeCompraNet').data('kendoGrid').dataSource.read();
}

function CreateGridInformeCompraNet() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/CompraNet/BuscaDatosTabla',
            },
            parameterMap: function (options) {
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
                    Fecha_Dolarizado: { type: "date" }
                }
            },

        },
        requestStart: function () {
            kendo.ui.progress($("#loading"), false);
        },
        requestEnd: function () {
            kendo.ui.progress($("#loading"), false);
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Estado_Order", dir: "asc" }],
        serverFiltering: true,
        pageSize: 20,
        filter: { field:"Fecha", operator:"eq", value: new Date },
    };

    $("#gridInformeCompraNet").kendoGrid({
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
            $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
            $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
            $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
            $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
        },
        columns: [
            { field: "Proveedor", type: "string", width: 140, template: function (dataItem) {
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
                    }
                } },
            { field: "FechaDesde", type: "date", title: "Desde", format: _DefaultDateTemplate, width: 45 },
            { field: "FechaHasta", type: "date", title: "Hasta", format: _DefaultDateTemplate, width: 45 },
            { field: "TipoNegocio", type: "string", title: "Tipo", width: 70 },
            { field: "Material", type: "string", filterable: { multi: true }, width: 95, template: "#=Material#" },
            { field: "Cantidad", type: "number", width: 70, format: "{0:n0}" },
            { field: "Ampliaciones", type: "number", width: 60, template: function (dataItem) {
                    if (($("#perfil").val() == "Jefe" || $("#perfil").val() == "Mesa" || $("#perfil").val() == "Comercial") && dataItem.Estado == 2) {
                        return '' + dataItem.Ampliaciones + '<button data-toggle="tooltip" title="Ampliar"onclick="ModalAmpliaciones(' +
                            "'" + dataItem.ContratoId + "'" + ',' + "'" + dataItem.Ampliacion + "'" + ',' + "'" + dataItem.TipoNegocio + "'" + "," + "'" + dataItem.FijacionDePrecioContratoId + "'" + ')"><i class="fa fa-plus aria-hidden="true"></i></button>';
                    } else if (dataItem.Estado == 1 || dataItem.Estado == 3) {
                        return dataItem.Ampliaciones;
                    } else {
                        return '';
                    }
                } },
            { field: "Precio", type: "number", width: 70, format: "{0:n2}" },
            { field: "Campania", type: "string", title: "Campa&ntilde;a", width: 70 },
            { field: "ContratoSAP", type: "number", title: "N&deg; SAP", width: 70 },
            { field: "Fecha", type: "date", title: "Carga", width: 20, format: _DefaultDateTemplate },
            { field: "Comercial", type: "string", title: "Comercial", width: 70, template: function (dataItem) {
                    if (($("#perfil").val() != "Analista")) {
                        return dataItem.Comercial;
                    } else {
                        return '';
                    }
                } },
            { field: "Estado_Contrato", width: 150, filterable: { multi: true }, template: function (dataItem) {
                    if (dataItem.Estado == 1 && $("#perfil").val() == "Mesa") { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonPendiente(dataItem, 'fa-pencil pend') +
                            botonConfirmadoTilde(dataItem, 'fa-check pend') +
                            botonVisualizar(dataItem,'fa-eye pend') +
                            botonBorrar(dataItem ,'fa-trash pend');
                            

                    } else if (dataItem.Estado == 1) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +
                            botonPendiente(dataItem, 'fa-pencil pend') +
                            botonVisualizar(dataItem, 'fa-eye pend');
                    }

                    if (dataItem.Estado == 2 && $("#perfil").val() == "Mesa") { //confirmado
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
                    if (dataItem.Estado == 3 && $("#perfil").val() == "Mesa") { //Oferta
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

                    if (dataItem.Estado == 4 && $("#perfil").val() == "Mesa") { //error
                        return '<div class="status error">Con Error</div>' +                            
                            botonPendiente(dataItem, 'fa-pencil err') +
                            botonFinalizado(dataItem, 'fa-flag-checkered err') +
                            botonVisualizar(dataItem, 'fa-eye err')+
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
                } }
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
        sortable: true,
        selectable: "row",
        
        filterable: {
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
        }   
        
    });
}

function InicializarElementosModalPendiente() {

    kendo.culture("es-AR");

    //$("#proveedorModalPendienteId").kendoDropDownList({
    //    optionLabel: "SELECCIONE UN PROVEEDOR...",
    //    dataTextField: "Descripcion",
    //    dataValueField: "ProveedorId"
    //});

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
        dataValueField: "MonedaId"    });

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

    $("#finalizarConfirmado").click(function () {
        ObtenerDatosModalConfirmado();
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

        var resultGrano = MSExecuteOnServer('/CompraNet/TraerCampanaPorMaterial', { MaterialId: $(this).val() });

        viewModel.set("CampanaComboModalPendiente", resultGrano);
    });

}


function modalPendiente(observacion, estado, contratoId, proveedor, fechaDesde, fechaHasta, fecha, fechaEntrega, tipoId, MaterialId, cantidad, ampliaciones, precio, MonedaId, campana, provincia, localidad, comercial, nroSAP, base, sustentablePrecio, sustentableMoneda, dolarizadoFecha, pesificadoDias, noInformaSIO, trigoEspecial, fijacionId) {
      
    if (tipoId === "3") {
        $("#modalPendiente .noFijacion").hide();
    } else {
        $("#modalPendiente .noFijacion").show();
    }

    $("#cantidadModalPendienteId").kendoNumericTextBox({
        value: cantidad ,
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#precioModalPendienteId").kendoNumericTextBox({
        value: precio ,
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });
       
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
    $("#fechaEntregaModalPendienteId").val(fechaEntrega);
    $("#fechaHoyId").val(fecha);
    $("#tipoModalPendienteId").data("kendoDropDownList").value(tipoId);
    $("#materialModalPendiente").data("kendoDropDownList").value(MaterialId);
    $("#observacionModalPendienteId").val(observacion);
    $("#cantidadModalPendienteId").val(cantidad);
    $("#precioModalPendienteId").val(precio);
    $("#precioMonedaModalPendienteId").data("kendoDropDownList").value(MonedaId);
    $("#campanaModalPendienteId").data("kendoDropDownList").value(campana);
    $("#provinciaId").data("kendoDropDownList").value(provincia);
    CargarLocalidadPorProvincia(provincia);
    if (!(localidad == "null" || localidad == "undefined")) $("#LocalidadId").data("kendoDropDownList").value(localidad);
    if (!(comercial == "null" || comercial == "undefined")) $("#comercialModalPendienteId").data("kendoDropDownList").value(comercial);

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
    
    if (!(dolarizadoFecha == "null" || dolarizadoFecha == "undefined" || dolarizadoFecha == "" )) {
        $("#dolarizadoModalPendienteId").prop("checked", true);
        $("#dolarizadoDivModalPendiente").show();
        $("#dolarizadoFechaModalPendienteId").val(dolarizadoFecha);
    }

    if (!(pesificadoDias == "null" || pesificadoDias == "undefined" || pesificadoDias == "" )) {
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

    $("#modalPendiente").modal('show');
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
    var fecha = new Date();
    var fechaHoy = new Date(
        fecha.getFullYear(),
        fecha.getMonth(),
        fecha.getDate(),
        fecha.getHours(),
        fecha.getMinutes(),
        fecha.getSeconds()
        );

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
    objPendiente.FechaEntrega = $("#fechaEntregaModalPendienteId").val();
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
    objPendiente.MonedaIdSustentable = $("#sustentableMonedaModalPendienteId").val();
    objPendiente.FechaDolarizado = $("#dolarizadoFechaModalPendienteId").val();
    objPendiente.DiasPesificado = $("#pesificadoDiasModalPendienteId").val();
    objPendiente.NoInformaSio = $("#noInformaSioModalPendienteId").is(":checked") ? true : false;
    objPendiente.TrigoEspecial = $("#trigoEspecialModalPendienteId").is(":checked") ? true : false;
    objPendiente.Estado = $("#baseId").is(":checked") ? "3" : "1";

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

    LimpiarPendiente();
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
}


function ModalFinalizado(contratoId,tipoId, fijacionDePrecioContratoId) {

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
                MensErr(result.Errores[0].Message);
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

function ModalConfirmado(contratoId, proveedor, fechaDesdeHasta, tipo, material, cantidad, precio, campana, provincia, localidad, nroSAP , sustentablePrecio, sustentableMoneda, dolarizadoFecha, pesificadoDias, informaSIO, trigoEspecial) {

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

    if (tipoId === '2') {
        $("#contratoModalConTilde").val(fijacionDePrecioContratoId);
        $(".modal-title-confirmadoTilde").append("Contrato DataAgro: " + contratoId);
    } else if (tipoId === '3') {
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

    if ($("#tipoNegocioIdAmpliaciones").val() == '1' || $("#tipoNegocioIdAmpliaciones").val() == '2') {
        var result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionContrato', ampliacion);
    } else {
        var result = MSExecuteOnServer('/CompraNet/GrabarAmpliacionFijacion', ampliacion);
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

function ModalVisualizar(contrato, proveedor,fecha, desdeHasta, tipo, material, cantidad, precio,comercial, monedaId, precioMoneda, campana, provincia, localidad, nro_SAP, sustentablePrecio, sustentableMonedaId, dolarizadoFecha, pesificadoDias, informaSIO, trigoEspecial, status, Observacion, moneda, sustentableMoneda) {


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
    $("#visualizar_precio").text(kendo.toString(parseInt(precio), "n0") + " " + moneda);
    $("#visualizar_campana").text(campana);
   

    var procedencia = "";
    var provinciaDat = provincia != "undefined" && provincia != "null" ? provincia : "";
    var localidadDat = localidad != "undefined" && localidad != "null" ? localidad : "";
    if (provinciaDat == "" || localidadDat == "") {
        procedencia = provinciaDat + localidadDat;
    } else if (provinciaDat != "" && localidadDat != "") {
        procedencia = localidadDat + ", " + provinciaDat;
    }

    $("#visualizar_procedencia").text(procedencia);
    $("#visualizar_nro_SAP").text(nro_SAP != "undefined" && nro_SAP != "null" ? nro_SAP : "");
    $("#visualizar_sustentablePrecio").text(sustentablePrecio != "undefined" && sustentablePrecio != "null" && sustentablePrecio  != 0 ? sustentablePrecio + " " + (sustentableMoneda != "undefined" && sustentableMoneda != "null" ? sustentableMoneda : "") : "");
    $("#visualizar_dolarizadoFecha").text(dolarizadoFecha != "undefined" && dolarizadoFecha != "null" ? dolarizadoFecha : "");
    $("#visualizar_pesificadoDias").text(pesificadoDias != "undefined" && pesificadoDias != "null" ? pesificadoDias : "");
    $("#visualizar_informaSIO").text(informaSIO != "undefined" && informaSIO != "null" && informaSIO != "false" ? "Si" : "");
    $("#visualizar_trigoEspecial").text(trigoEspecial != "undefined" && trigoEspecial != "null" && trigoEspecial != "false" ? "Si" : "");
    $("#visualizar_observacion").text(Observacion != "undefined" && Observacion ? Observacion : "");
    $("#modalVisualizar").modal('show');
}


function ModalBorrar(proveedor) {
    $("#proveedor_a_borrar").text(proveedor);

    $("#modalBorrar").modal('show');
}