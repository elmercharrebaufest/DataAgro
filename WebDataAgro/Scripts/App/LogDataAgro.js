$(document).ready(function () {
    //$('#menuproveedor').hide();
    kendo.culture("es-AR");
    InicializarCuposIndex();
    crearPopupAgregarHijo();


});


function InicializarCuposIndex() {

    var ds = {
        transport: {
            parameterMap: function (options, operation) {

                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            },
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/LogDataAgro/BuscarDatosLogDataAgro',
                data: function () {

                }
            }
        },
        schema: {
            data: "Data",
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Fecha: { type: "date" },
                    Usuario: { type: "string" },
                    AccionRealizada: { type: "string" },
                    Clase: { type: "string" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        sort: [
        ],
        pageSize: 20,
        serverFiltering: true
    };


    $("#grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Cupos.xlsx",
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            var view = grid.dataSource.view();

        },
        columns: [
            { field: "Fecha", title: "Fecha de Modificacion", type: "date", width: 150, format: "{0:dd/MM/yyyy HH:mm }" },
            { field: "Usuario", title: "Usuario", type: "string", width: 150 },
            { field: "AccionRealizada", title: "Accion Realizada", type: "string", width: 150 },
            { field: "Clase", type: "string", width: 150 },
            {
                command: [
                    { name: "qw", text: "Ver Más", click: abrirVentanaAgregarHijo, className: "k-grid-agregarhijo", width: 350 }], title: " ", width: "15%"
            }
            ,
            {
                field: "Estado_Contrato", sortable: false, title: "Estado", template: function (dataItem) {
                    var obj = JSON.parse(dataItem.DatoModificado);

                    if (dataItem.Clase.includes("Negocio")) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>' +

                            botonVisualizar(obj, 'fa-eye pend');
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
        //excelExport: function (e) {
        //    var sheet = e.workbook.sheets[0];
        //    var templateFlete = kendo.template(this.columns[8].template);

        //    for (var i = 1; i < sheet.rows.length; i++) {
        //        var row = sheet.rows[i];

        //        var dataItem = { FleteProcedencia: row.cells[8].value };
        //        row.cells[8].value = templateFlete(dataItem);


        //        //la Fecha en Chrome aparece corrida un dia, solucion:
        //        var fecha = row.cells[0].value;

        //        if (fecha != null) {
        //            fecha = fecha.setHours(fecha.getHours() + 1);
        //            row.cells[0].value = new Date(fecha);
        //        }
        //    }
        //},
    });
}



function abrirVentanaAgregarHijo(e) {

    e.preventDefault();
    wnd.center().open();
}


function crearPopupAgregarHijo() {

    wnd = $("#popUpAgregarCriterio")
        .kendoWindow({
            title: "Agregar Criterio",
            modal: true,
            visible: false,
            resizable: false,
            width: 400,

        }).data("kendoWindow");



    $("#prioridad").kendoNumericTextBox({
        format: "0",
        decimals: 0,
        min: 0,
    });
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

        "'" + dataItem.ZonaDescripcion + "'" + ',' +
        "'" + dataItem.NivelTarifa + "'" + ',' +
        "'" + dataItem.TarifaFlete + "'" + ',' +
        "'" + dataItem.Compensacion + "'" + ',' +
        "'" + dataItem.Rechazo + "'" + ',' +
        "'" + formatearFecha(dataItem.FechaCierta) + "'" + ',' +
        "'" + dataItem.PorcentajeDePago + "'" +
        ')"><i class="fa ' + icono + ' aria-hidden="true"></i></button>';
}


function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
}

function htmlEncode(value) {
    if (value != null) {
        return $('<div/>').text(value.replace(/(\r\n|\n|\r)/gm, "")).html();
    }

}

function ModalVisualizar(contrato, proveedor, corredor, fecha, desdeHasta, tipo, material, cantidad, precio, comercial, monedaId, precioMoneda,
    campana, provincia, localidad, nro_SAP, sustentablePrecio, sustentableMonedaId, dolarizadoFecha, pesificadoDias, informaSIO,
    trigoEspecial, status, Observacion, moneda, sustentableMoneda, destino, destinoDescripcion, cantidadCamiones, consignatario, planCanje, condicionFijacionId,
    cd, warrant, pagoDirectoVendedor, establecimientoPropio, boletoId, bolsaId, boletoDescripcion, bolsaDescripcion, desdeHastaFijacion, condicionFijacionDescripcion,
    clasificacionId, clasificacionDescripcion, standardDeCalidadDescripcion, calidadEspecialDescripcion, desdeFijacion, hastaFijacion, mercsFijacion,
    contratoCorredor, contratoVendedor, selCargoMOA, selCargoVendedor, tipoFason, posicion, operador, precioNeto, id, pizarra, zona, nivelTarifa, tarifaFlete,
    compensacion, rechazo, fechaCierta, porcentajeDePago) {
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
    if (tipo === "CONTRATO ACUERDO") {
        $("#pactadosDivVisualizar").hide();
    } else {
        $("#pactadosDivVisualizar").show();
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
        case "11": //reconfirmarFinalizado
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.reconfirmar-modal").show();
            break;
        case "10": //preanular
            $("#modalVisualizar .modal-header > div.status").hide();
            $("#modalVisualizar .modal-header > div.status.borrado-modal").show();
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
    precio != 0 ? $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2") + " " + moneda) : pizarra ? $("#visualizar_precio").text("Pizarra") : $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2"));
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

    if (fechaCierta == "null") {
        $("#FechaCiertaVisualizar").hide();
    } else {
        $("#visualizar_FechaCierta").text(fechaCierta);
        $("#FechaCiertaVisualizar").show();
    }
    if (porcentajeDePago == "null") {
        $("#porcentajeDePagoDivVisualizar").hide();
    } else {
        $("#visualizar_porcentajeDePago").text(porcentajeDePago + " %");
        $("#porcentajeDePagoDivVisualizar").show();
    }
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


    var datos;
    console.log(tipo, contrato, id);
    if (tipo == "CONTRATO ACUERDO" || tipo == "AGENTE DE COMPRAS" || tipo == "FASON" || tipo == "FIJACION") {
        datos = MSExecuteOnServer('/CompraNet/TraerCalidadesPorContrato', { contratoId: contrato, acuerdoId: id });
    } else {
        datos = MSExecuteOnServer('/CompraNet/TraerDatosDeContrato', { contratoId: id });
    }
    var iteracionesDescuentos = viewModel.DescuentosVisualizar.length;
    for (var i = 0; i < iteracionesDescuentos; i++) {
        viewModel.DescuentosVisualizar.pop();
    }

    var descuentosDto = datos.DescuentosBonificaciones;
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

    var calidadesDto = datos.Calidades;
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
    var iteracionesPrecio = viewModel.PreciosVisualizar.length;
    for (var k = 0; k < iteracionesPrecio; k++) {
        viewModel.PreciosVisualizar.pop();
    }
    var preciosDto = datos.Precios;
    if (preciosDto != null && preciosDto.length > 0)
        $("#pactadosDivVisualizar").show();
    else
        $("#pactadosDivVisualizar").hide();

    $.each(preciosDto, function (key, precio) {
        var precioKendo = {
            Id: precio.Id,
            FechaDesde: precio.FechaDesde,
            FechaHasta: precio.FechaHasta,
            Precio: kendo.toString(precio.Precio ? Number(precio.Precio) : "", "n2"),
            MonedaPactadoId: precio.MonedaPactadoId,
            MonedaPactadoDesc: precio.MonedaPactadoDesc,
            ImportePactado: kendo.toString(precio.ImportePactado ? Number(precio.ImportePactado) : "", "n2"),
            MonedaImportePactadoId: precio.MonedaImportePactadoId != null ? precio.MonedaImportePactadoId : "",
            MonedaImportePactadoDesc: precio.MonedaImportePactadoDesc != null ? precio.MonedaImportePactadoDesc : "",
            Porcentaje: precio.Porcentaje != null ? precio.Porcentaje : ""
        };
        viewModel.PreciosVisualizar.push(precioKendo);
    });

    if (standardDeCalidadDescripcion == "null") $("#tipoCalidadDiv").hide();
    if (standardDeCalidadDescripcion == "Grado 2" || standardDeCalidadDescripcion == "Bonif. SECO de 7% a 10% Por punto") $("#calidadesDivVisualizar").hide();

    if (zona !== "undefined" && zona !== "") {
        $("#visualizar-zona-girasol").text(zona);
    } else {
        $("#visualizar-zona-girasol").text("");
        $("#visualizar_aperturaFinancieroPrecioNeto").text(kendo.toString(parseFloat(precioNeto), "n2") + " " + moneda);
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
    if ((nivelTarifa == "" || nivelTarifa == "null") && tarifaFlete == "null") {
        $("#fleteDivVisualizar").hide();
    } else {
        $("#fleteDivVisualizar").show();
        $("#visualizar_nivelFlete").text(nivelTarifa);
        $("#visualizar_tarifaFlete").text(tarifaFlete);
        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
    }
    compensacion === "true" || compensacion === true ? $("#compensacionVisualizar").show() : $("#compensacionVisualizar").hide();
    if (rechazo != "null") {
        $(".rechazo").show();
        $("#visualizar_rechazo").text(rechazo);
    } else {
        $(".rechazo").hide();
        $("#visualizar_rechazo").text("");
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


//function inicializarElementos() {

//    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/cupo/ListarProveedor");

//    $("#Destinatario").bind("paste", function (e) {
//        e.preventDefault();
//        if (e.originalEvent.clipboardData !== undefined) {
//            clipText = e.originalEvent.clipboardData.getData('text/plain');
//        } else {
//            clipText = window.clipboardData.getData('text');
//        }
//        $("#Destinatario").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));
//        CambioVariosContratos();
//    });

//    inicializarPopUpSap("Cupos");

//}



//function mostrarocultar(element) {
//    if ($(element).text() == "Mostrar") {
//        $(element).text("Ocultar");
//    } else {
//        $(element).text("Mostrar");
//    }
//}
//function deseleccionarRadioButton() {
//    $('[name=FleteProcedencia]:checked').prop('checked', false);
//}

//function ConvertirFechaRegistroAString(filtros) {
//    arr = [];

//    filtros.forEach(function (x) {
//        (x.field == "FechaRegistro") ? arr.push(x) : null;
//    });

//    if (arr != null) {
//        //se usa asi porque usando toLocaleDateString no trae valores igual al dia de hoy
//        arr.forEach(function (x) { x.value = deFechaAString(x.value) });
//    }
//}








