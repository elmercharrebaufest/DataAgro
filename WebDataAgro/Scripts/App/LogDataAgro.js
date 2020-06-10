$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    CrearViewModel();
    InicializarCuposIndex();



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
            fileName: "Reporte - LogsDataAgro.xlsx",
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {

            var grid = $("#grid").data("kendoGrid");
            grid.hideColumn("Id");

        },
        columns: [
            { field: "Id", type: "string" },
            { field: "Fecha", title: "Fecha de Modificacion", type: "date", /*width: 150,*/ format: "{0:dd/MM/yyyy HH:mm }" },
            { field: "Usuario", title: "Usuario", type: "string" },
            { field: "AccionRealizada", title: "Accion Realizada", type: "string" },
            { field: "Clase", type: "string" },
            {
                field: "Estado_Contrato", sortable: false, title: " ", template: function (dataItem) {

                    return botonVisualizar(dataItem);
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
        excelExport: function (e) {


            let sheet = e.workbook.sheets[0];
            sheet.rows[0].cells[4].value = "Campos Modificados";


            for (var i = 1; i < sheet.rows.length; i++) {
                let row = sheet.rows[i];
                let idDelLogSeleccionado = e.data[i - 1].Id;

                datos = MSExecuteOnServer('/logdataagro/TraerDatosModificados', { idLogDataAgro: idDelLogSeleccionado });

                let datosCambiadosParaMostrar = "";
                datos.CamposCambiados.forEach(function (entry) {

                    if (!entry.includes("Id")) {
                        datosCambiadosParaMostrar += (entry + ", ");
                    }


                });
                row.cells[4].value = datosCambiadosParaMostrar;
            }
        },
    });

}




function botonVisualizar(dataItem) {

    return '<a role="button" class="k-button k-button-icontext k-grid-vermas k-grid-qw" onclick="traerDatosModificados(' + dataItem.id + ',' +
        "'" + dataItem.Clase.trim() + "'" + ')">Ver Más</a>';
}

function traerDatosModificados(idLogDataAgro, tipoDeClase) {

    console.log(idLogDataAgro);
    datos = null;
    datos = MSExecuteOnServer('/logdataagro/TraerDatosModificados', { idLogDataAgro: idLogDataAgro });

    var obj = JSON.parse(datos.LogActual.DatoModificado);



    if (tipoDeClase.toString().includes("Negocio") || tipoDeClase.toString().includes("Contrato")) {

        var desdeHasta = formatearFecha(new Date(obj.fechaDesde)) + " - " + formatearFecha(new Date(obj.fechaHasta));
        //var desdeHasta = formatearFecha(obj.fechaDesde) + " - " + formatearFecha(obj.FechaHasta);

        ModalVisualizar(obj.id, obj.proveedor, obj.corredor, desdeHasta, formatearFecha(new Date(obj.fecha)), obj.tipoNegocio, obj.material, obj.cantidad, obj.precio, obj.comercial, obj.monedaId, obj.precioMoneda,
            obj.campania, obj.provincia, obj.localidad, obj.nro_SAP, obj.sustentablePrecio, obj.sustentableMonedaId, obj.dolarizadoFecha, obj.pesificadoDias, obj.informaSIO,
            obj.trigoEspecial, obj.status, obj.Observacion, obj.moneda, obj.sustentableMoneda, obj.destino, obj.destinoDescripcion, obj.cantidadCamiones, obj.consignatario, obj.planCanje, obj.condicionFijacionId,
            obj.cd, obj.warrant, obj.pagoDirectoVendedor, obj.establecimientoPropio, obj.boletoId, obj.bolsaId, obj.boletoDescripcion, obj.bolsaDescripcion, obj.desdeHastaFijacion, obj.condicionFijacionDescripcion,
            obj.clasificacionId, obj.clasificacionDescripcion, obj.standardDeCalidadDescripcion, obj.calidadEspecialDescripcion, obj.desdeFijacion, obj.hastaFijacion, obj.mercsFijacion,
            obj.contratoCorredor, obj.contratoVendedor, obj.selCargoMOA, obj.selCargoVendedor, obj.tipoFason, obj.posicion, obj.operador, obj.precioNeto, obj.id, obj.pizarra, obj.zona, obj.nivelTarifa, obj.tarifaFlete,
            obj.compensacion, obj.rechazo, obj.fechaCierta, obj.porcentajeDePago);

        //ModalVisualizar(obj.Contrato, obj.Proveedor, obj.Corredor, (obj.Fecha != null) ? formatearFecha(new Date(obj.Fecha)) : "", DesdeHasta, obj.TipoNegocio, obj.Material, obj.Cantidad, obj.Precio, obj.Comercial, obj.MonedaId, obj.PrecioMoneda, obj.Campania,
        //    obj.Provincia, obj.Localidad, obj.Nro_SAP, obj.SustentablePrecio, obj.SustentableMonedaId, (obj.DolarizadoFecha != null) ? formatearFecha(new Date(obj.DolarizadoFecha)) : "", obj.PesificadoDias, obj.InformaSIO,
        //    obj.TrigoEspecial, obj.Status, obj.Observacion, obj.Moneda, obj.SustentableMoneda, obj.Destino, obj.DestinoDescripcion, obj.CantidadCamiones, obj.Consignatario, obj.PlanCanje, obj.CondicionFijacionId,
        //    obj.Cd, obj.warrant, obj.PagoDirectoVendedor, obj.EstablecimientoPropio, obj.BoletoId, obj.BolsaId, obj.BoletoDescripcion, obj.BolsaDescripcion, obj.DesdeHastaFijacion, obj.CondicionFijacionDescripcion,
        //    obj.ClasificacionId, obj.ClasificacionDescripcion, obj.StandardDeCalidadDescripcion, obj.CalidadEspecialDescripcion, obj.DesdeFijacion, obj.HastaFijacion, obj.MercsFijacion,
        //    obj.ContratoCorredor, obj.ContratoVendedor, obj.SelCargoMOA, obj.SelCargoVendedor, obj.TipoFason, obj.Posicion, obj.Operador, obj.PrecioNeto, obj.Id, obj.Pizarra, obj.Zona, obj.NivelTarifa, obj.TarifaFlete,
        //    obj.Compensacion, obj.Rechazo, (obj.FechaCierta != null) ? formatearFecha(new Date(obj.FechaCierta)) : "", obj.PorcentajeDePago);

    }
    if (tipoDeClase.includes("Cupo")) {
        ModalVisualizarCupo(obj.calidad, obj.cupoSap, obj.destinatario, (obj.fechaGeneracion != null) ? formatearFecha(new Date(obj.fechaGeneracion)) : "", (obj.fechaIngreso) ? formatearFecha(new Date(obj.fechaIngreso)) : "",
            obj.fleteProcedencia, obj.observaciones, obj.centro, obj.estadoCupo, obj.zonaCupo,
            obj.comercial, obj.material, obj.proveedor);
    }
    if (tipoDeClase.includes("Proveedor")) {

        var proveedorObj = obj.basicoProveedorTraerPorProveedores[0];

        ModalVisualizarProveedor(proveedorObj.apellido, proveedorObj.areaInfluencia, proveedorObj.cuit, proveedorObj.canalOperacion, proveedorObj.clasificacionCompraNet,
            proveedorObj.codigoPostal, proveedorObj.condicion, proveedorObj.destinatario, proveedorObj.direccion, proveedorObj.email1, proveedorObj.estado, proveedorObj.grupoDeCompras,
            proveedorObj.grupoSegmentacion, proveedorObj.localidad, proveedorObj.nombreReferente, proveedorObj.nombres, proveedorObj.observaciones, proveedorObj.provincia,
            proveedorObj.razonSocial, proveedorObj.segmentacion, proveedorObj.telefono1);
    }
    eliminarMarcarCamposModificados(datos.CamposCambiados);
    marcarCamposModificados(datos.CamposCambiados);
    ocultarNulos();
}

function ModalVisualizarProveedor(apellido, areainfluencia, cuit, canaloperacion, clasificacioncompranet,
    codigopostal, condicion, destinatario, direccion, email1, estado, grupodecompras,
    gruposegmentacion, localidad, nombrereferente, nombres, observaciones, provincia,
    razonsocial, segmentacion, telefono1) {
    $("#modalVisualizarProveedor").modal('show');

    if (apellido != null || apellido != undefined || apellido != "") {
        $("#apellidoDivVisualizar").attr("hidden", false);
        $("#visualizar_apellido").text(apellido);
    }
    if (areainfluencia != null || areainfluencia != undefined || areainfluencia != "") {
        $("#areainfluenciaDivVisualizar").attr("hidden", false);
        $("#visualizar_areainfluencia").text(areainfluencia);
    }
    if (cuit != null || cuit != undefined || cuit != "") {
        $("#cuitDivVisualizar").attr("hidden", false);
        $("#visualizar_cuit").text(cuit);
    }
    if (canaloperacion != null || canaloperacion != undefined || canaloperacion != "") {
        $("#canaloperacionDivVisualizar").attr("hidden", false);
        $("#visualizar_canaloperacion").text(canaloperacion);
    }
    if (clasificacioncompranet != null || clasificacioncompranet != undefined || clasificacioncompranet != "") {
        $("#clasificacioncompranetDivVisualizar").attr("hidden", false);
        $("#visualizar_clasificacioncompranet").text(clasificacioncompranet);
    }
    if (codigopostal != null || codigopostal != undefined || codigopostal != "") {
        $("#codigopostalDivVisualizar").attr("hidden", false);
        $("#visualizar_codigopostal").text(codigopostal);
    }
    if (condicion != null || condicion != undefined || condicion != "") {
        $("#condicionDivVisualizar").attr("hidden", false);
        $("#visualizar_condicion").text(condicion);
    }
    if (destinatario != null || destinatario != undefined || destinatario != "") {
        $("#destinatarioDivVisualizar").attr("hidden", false);
        $("#visualizar_destinatario").text(destinatario);
    }
    if (direccion != null || direccion != undefined || direccion != "") {
        $("#direccionDivVisualizar").attr("hidden", false);
        $("#visualizar_direccion").text(direccion);
    }
    if (email1 != null || email1 != undefined || email1 != "") {
        $("#email1DivVisualizar").attr("hidden", false);
        $("#visualizar_email1").text(email1);
    }
    if (estado != null || estado != undefined || estado != "") {
        $("#estadoDivVisualizar").attr("hidden", false);
        $("#visualizar_estado").text(estado);
    }
    if (grupodecompras != null || grupodecompras != undefined || grupodecompras != "") {
        $("#grupodecomprasDivVisualizar").attr("hidden", false);
        $("#visualizar_grupodecompras").text(grupodecompras);
    }
    if (gruposegmentacion != null || gruposegmentacion != undefined || gruposegmentacion != "") {
        $("#gruposegmentacionDivVisualizar").attr("hidden", false);
        $("#visualizar_gruposegmentacion").text(gruposegmentacion);
    }
    if (localidad != null || localidad != undefined || localidad != "") {
        $("#localidadDivVisualizar").attr("hidden", false);
        $("#visualizar_localidad").text(localidad);
    }
    if (nombrereferente != null || nombrereferente != undefined || nombrereferente != "") {
        $("#nombrereferenteDivVisualizar").attr("hidden", false);
        $("#visualizar_nombrereferente").text(nombrereferente);
    }
    if (nombres != null || nombres != undefined || nombres != "") {
        $("#nombresDivVisualizar").attr("hidden", false);
        $("#visualizar_nombres").text(nombres);
    }
    if (observaciones != null || observaciones != undefined || observaciones != "") {
        $("#observacionesDivVisualizar").attr("hidden", false);
        $("#visualizar_observacionesproveedor").text(observaciones);
    }
    if (provincia != null || provincia != undefined || provincia != "") {
        $("#provinciaDivVisualizar").attr("hidden", false);
        $("#visualizar_provincia").text(provincia);
    }
    if (razonsocial != null || razonsocial != undefined || razonsocial != "") {
        $("#razonsocialDivVisualizar").attr("hidden", false);
        $("#visualizar_razonsocial").text(razonsocial);
    }
    if (segmentacion != null || segmentacion != undefined || segmentacion != "") {
        $("#segmentacionDivVisualizar").attr("hidden", false);
        $("#visualizar_segmentacion").text(segmentacion);
    }
    if (telefono1 != null || telefono1 != undefined || telefono1 != "") {
        $("#telefono1DivVisualizar").attr("hidden", false);
        $("#visualizar_telefono1").text(telefono1);
    }
}

function ModalVisualizarCupo(calidad, cupoSap, destinatario, fechaGeneracion, fechaIngreso,
    fleteProcedencia, observaciones, centro, estadoCupo, zonaCupo, comercial, material, proveedor) {
    $("#modalVisualizarCupo").modal('show');

    if (calidad != null || calidad != "undefined" || calidad != "") {
        $("#calidadDivVisualizarCupo").attr("hidden", false);
        $("#visualizar_calidadcupo").text(calidad);
    }
    if (cupoSap != null || cupoSap != "undefined" || cupoSap != "") {
        $("#cupoSapDivVisualizar").attr("hidden", false);
        $("#visualizar_cuposap").text(cupoSap);
    }
    if (destinatario != null || destinatario != "undefined" || destinatario != "") {
        $("#destinatarioDivVisualizar").attr("hidden", false);
        $("#visualizar_destinatario").text(destinatario);
    }
    if (fechaGeneracion != null || fechaGeneracion != "undefined" || fechaGeneracion != "") {
        $("#fechaGeneracionDivVisualizar").attr("hidden", false);
        $("#visualizar_fechageneracion").text(fechaGeneracion);
    }
    if (fechaIngreso != null || fechaIngreso != "undefined" || fechaIngreso != "") {
        $("#fechaIngresoDivVisualizar").attr("hidden", false);
        $("#visualizar_fechaingreso").text(fechaIngreso);
    }
    if (observaciones != null || observaciones != "undefined" || observaciones != "") {
        $("#observacionesDivVisualizar").attr("hidden", false);
        $("#visualizar_observaciones").text(observaciones);
    }
    if (centro != null || centro != "undefined" || centro != "") {
        $("#centroDivVisualizar").attr("hidden", false);
        $("#visualizar_centro").text(centro);
    }
    if (estadoCupo != null || estadoCupo != "undefined" || estadoCupo != "") {
        $("#estadoCupoDivVisualizar").attr("hidden", false);
        $("#visualizar_estadocupo").text(estadoCupo);
    }
    if (zonaCupo != null || zonaCupo != "undefined" || zonaCupo != "") {
        $("#zonaCupoDivVisualizar").attr("hidden", false);
        $("#visualizar_zonacupo").text(zonaCupo);
    }
    if (comercial != null || comercial != "undefined" || comercial != "") {
        $("#comercialDivVisualizar").attr("hidden", false);
        $("#visualizar_comercial").text(comercial);
    }
    if (material != null || material != "undefined" || material != "") {
        $("#materialDivVisualizarCupo").attr("hidden", false);
        $("#visualizar_materialcupo").text(material);
    }
    if (proveedor != null || proveedor != "undefined" || proveedor != "") {
        $("#proveedorDivVisualizarCupo").attr("hidden", false);
        $("#visualizar_proveedorcupo").text(proveedor);
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



    $(".modal-title-visualizar").empty();
    //$(".modal-title-visualizar").append("Contrato N&deg; SAP: " + (nro_SAP != "null" ? nro_SAP : ""));
    //$(".modal-title-visualizar").append("Contrato N&deg; SAP: " + (nro_SAP != "null" ? nro_SAP : ""));
    $("#visualizar_proveedor").text(proveedor);
    $("#fechacontrato").text(desdeHasta);
    $("#visualizar_desdehasta").text(fecha);
    $("#visualizar_tipo").text(tipo);
    $("#visualizar_comercial").text(comercial);
    $("#visualizar_comercial_afijar").text(comercial);
    $("#visualizar_material").text(material);
    $("#visualizar_cantidad").text(isNaN(parseInt(cantidad)) ? "" : kendo.toString(parseInt(cantidad), "n0"));
    precio != 0 ? $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2") + " " + moneda) : pizarra ? $("#visualizar_precio").text("Pizarra") : $("#visualizar_precio").text(kendo.toString(parseFloat(precio), "n2"));
    campana !== "" ? $("#visualizar_campanacupo").text(campana) : $("#visualizar_campana").text("null");

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

    contratoCorredor != null && contratoCorredor != "" ? $("#visualizar_contratocorredor").text(contratoCorredor) : $("#visualizar_contratocorredor").text("null");
    contratoVendedor != null && contratoVendedor != "" ? $("#visualizar_contratoVendedor").text(contratoVendedor) : $("#visualizar_contratoVendedor").text("null");
    visualizacionRowDoble("contratoCorredorVisualizar", "visualizar_contratocorredor", "contratovendedorvisualizar", "visualizar_contratovendedor");

    selCargoMOA === "true" ? $("#visualizar_selladoacargo").text("MOA") : selCargoVendedor === "true" ? $("#visualizar_selladoacargo").text("Vendedor") : $("#selladoacargovisualizar").hide();

    if (fechaCierta == "null") {
        $("#FechaCiertaVisualizar").hide();
    } else {
        $("#visualizar_fechacierta").text(fechaCierta);
        $("#FechaCiertaVisualizar").show();
    }
    if (porcentajeDePago == "null") {
        $("#porcentajeDePagoDivVisualizar").hide();
    } else {
        $("#visualizar_porcentajedepago").text(porcentajeDePago + " %");
        $("#porcentajeDePagoDivVisualizar").show();
    }
    $("#visualizar_procedencia").text(procedencia);
    $("#visualizar_nro_sap").text(nro_SAP != "undefined" && nro_SAP != "null" ? nro_SAP : "");
    $("#visualizar_observacion").text(Observacion != "undefined" && Observacion ? Observacion : "");

    $("#visualizar_condicionfijacion").text(condicionFijacionId);
    $("#visualizar_clasificacion").text(clasificacionDescripcion);
    sustentablePrecio !== "null" && sustentableMonedaId !== "null" ? $("#visualizar_sustentableprecio").text(sustentablePrecio + " " + sustentableMonedaId) : $("#visualizar_sustentablePrecio").text("null");
    $("#visualizar_dolarizadofecha").text(dolarizadoFecha);
    $("#visualizar_pesificadodias").text(pesificadoDias);
    informaSIO === "true" ? $("#visualizar_informasio").text("Si") : $("#visualizar_informasio").text("null");
    mercsFijacion == "true" ? $("#visualizar_mercsdeposito").text("Si") : $("#visualizar_mercsdeposito").text("null");
    cd === "true" ? $("#visualizar_pago").text("CD") : (warrant === "true") ? $("#visualizar_pago").text("Warrant") : (pagoDirectoVendedor === "true") ? $("#visualizar_pago").text("Pago Directo Vendedor") : $("#visualizar_pago").text("null");
    boletoDescripcion === "Ninguno" || boletoDescripcion === null || boletoDescripcion === "" || boletoDescripcion === "undefined" ? ($("#visualizar_boleto").text("null") && $("#visualizar_bolsa").text("null")) : ($("#visualizar_boleto").text(boletoDescripcion) && $("#visualizar_bolsa").text(bolsaDescripcion));

    if (condicionFijacionDescripcion === "undefined" || condicionFijacionDescripcion === "null" || condicionFijacionDescripcion === "false" || condicionFijacionDescripcion === "") {
        $("#desdeHastaFijacionDivVisualizar").hide();
        $("#condicionFijacionDivVisualisar").hide();
    } else {
        $("#visualizar_desdehastafijacion").text(desdeHastaFijacion);
        $("#visualizar_condicionfijacion").text(condicionFijacionDescripcion);
    }

    planCanje === "true" ? $("#visualizar_plancanje").text("Si") : $("#visualizar_plancanje").text("null");
    consignatario === "true" ? $("#visualizar_consignatario").text("Consignatario") : $("#visualizar_consignatario").text("");
    cantidadCamiones !== 0 && cantidadCamiones !== "null" ? $("#visualizar_cantidaddecamiones").text(cantidadCamiones) : $("#visualizar_cantidaddecamiones").text("null");
    establecimientoPropio === "true" ? $("#visualizar_establecimiento").text("Propio") : establecimientoPropio === "false" ? $("#visualizar_establecimiento").text("Arrendado") : $("#visualizar_establecimiento").text("null");
    destinoDescripcion != "" ? $("#visualizar_destino").text(destinoDescripcion) : $("#visualizar_destino").text("null");
    visualizacionRowDoble("cantidadDivVisualizar", "visualizar_cantidad", "cantidadDeCamionesDivVisualizar", "visualizar_cantidaddecamiones");
    visualizacionRowDoble("tipoDivVisualizar", "visualizar_tipo", "destinoDivVisualizar", "visualizar_destino");
    visualizacionRowDoble("mercsDepositoDivVisualizar", "visualizar_mercsdeposito", "pagoDivVisualizar", "visualizar_pago");
    visualizacionRowDoble("boletoDivVisualizar", "visualizar_boleto", "bolsaDivVisualizar", "visualizar_bolsa");
    visualizacionRowDoble("pesificadoDiasDivVisualizar", "visualizar_pesificadoDias", "informaSIODivVisualizar", "visualizar_informaSIO");
    visualizacionRowDoble("sustentableDivVisualizar", "visualizar_sustentableprecio", "dolarizadoFechaDivVisualizar", "visualizar_dolarizadofecha");
    visualizacionRowDoble("planCanjeDivVisualizar", "visualizar_planCanje", "establecimientoDivVisualizar", "visualizar_establecimiento");

    if (!sustentablePrecio === "undefined" || !sustentablePrecio === "null" || !sustentablePrecio === "false") {
        $("#visualizar_sustentableprecio").text(sustentablePrecio + " " + (sustentableMoneda != "undefined" && sustentableMoneda != "null" ? sustentableMoneda : ""));
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
        $("#visualizar_aperturafinancieroprecioneto").text(kendo.toString(parseFloat(precioNeto), "n2") + " " + moneda);
    }
    $("#visualizar_aperturafinanciero").text(null);
    $("#visualizar_aperturaredespacho").text(null);
    $("#visualizar_aperturacomisiones").text(null);
    $("#visualizar_aperturabonificaciones").text(null);
    $("#aperturaFinancieroDivVisualizar").hide();
    $("#aperturaRedespachoDivVisualizar").hide();
    $("#aperturaComisionesDivVisualizar").hide();
    $("#aperturaBonificacionesDivVisualizar").hide();
    $("#aperturaDePrecioVisualizarDiv").hide();
    $("#aperturaDePrecioVisualizarDivPrecioNeto").hide();

    if (precioNeto != 0) {
        $("#visualizar_aperturafinancieroprecioneto").text(kendo.toString(parseFloat(precioNeto), "n2") + " " + moneda);
        var aperturaPrecio = MSExecuteOnServer('/CompraNet/TraerAperturaPrecioPorContrato', { contratoId: id, tipo: tipo });
        $.each(aperturaPrecio, function (key, concepto) {
            switch (concepto.ConceptoAperturaPrecioId) {
                case 1:
                    if (concepto.Importe) {
                        $("#aperturaFinancieroDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        $("#visualizar_aperturafinanciero").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                    }
                    break;
                case 2:
                    if (concepto.Importe) {
                        $("#aperturaRedespachoDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();
                        $("#visualizar_aperturaredespacho").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda);
                    }
                    break;
                case 3:
                    if (concepto.Importe || concepto.Porcentaje) {
                        $("#aperturaComisionesDivVisualizar").show();
                        $("#aperturaDePrecioVisualizarDiv").show();
                        $("#aperturaDePrecioVisualizarDivPrecioNeto").show();

                        $("#visualizar_aperturacomisiones").text(concepto.Importe ? kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda : concepto.Porcentaje + "%");
                    }
                    break;
                case 4:
                    if (concepto.Importe || concepto.Porcentaje) {
                        $("#visualizar_aperturabonificaciones").text(kendo.toString(parseFloat(concepto.Importe), "n2") + " " + moneda + " - " + concepto.Porcentaje + "%");
                    }

                    break;
            }

            visualizacionRowDoble("aperturaFinancieroDivVisualizar", "visualizar_aperturafinanciero", "aperturaRedespachoDivVisualizar", "visualizar_aperturaredespacho");
            visualizacionRowDoble("aperturaComisionesDivVisualizar", "visualizar_aperturacomisiones", "aperturaBonificacionesDivVisualizar", "visualizar_aperturabonificaciones");
        });
    }
    if ((nivelTarifa == "" || nivelTarifa == "null") && tarifaFlete == "null") {
        $("#fleteDivVisualizar").hide();
    } else {
        $("#fleteDivVisualizar").show();
        $("#visualizar_nivelflete").text(nivelTarifa);
        $("#visualizar_tarifaflete").text(tarifaFlete);
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
function marcarCamposModificados(camposModificados) {
    camposModificados.forEach(x => {
        if (x.toLowerCase().includes("$id")) {
            return;
        }
        $("#visualizar_" + x.toLowerCase()).css("background-color", "yellow");
        $("#visualizar_" + x.toLowerCase() + "cupo").css("background-color", "yellow");
        $("#visualizar_" + x.toLowerCase() + "proveedor").css("background-color", "yellow");
    });
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


function formatearFecha(fecha) {
    var fechaFormateada = kendo.toString(fecha, "dd/MM/yyyy");
    return fechaFormateada;
}

function htmlEncode(value) {
    if (value != null) {
        return $('<div/>').text(value.replace(/(\r\n|\n|\r)/gm, "")).html();
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


function CrearViewModel() {
    var param = {
        "Descuentos": null,
        "ContratosPendientes": null
    };
    viewModel = kendo.observable({
        Parametros: param,
        DescuentosVisualizar: [],
        CalidadesVisualizar: [],
        ContratosPendientes: [],
        PreciosVisualizar: []
    });

    kendo.bind($("#tabla-descuentos-visualizar"), viewModel);
    kendo.bind($("#tabla-calidades-visualizar"), viewModel);
    kendo.bind($("#tabla-precios-pactados"), viewModel);
    kendo.bind($("#tabla-pendientes"), viewModel);
}


function eliminarMarcarCamposModificados(camposModificados) {

    $('#modalVisualizarCupo').on('hidden.bs.modal', function () {
        camposModificados.forEach(x => { $("#visualizar_" + x.toLowerCase()).css("background-color", ""); });
    });

    $('#modalVisualizar').on('hidden.bs.modal', function () {
        camposModificados.forEach(x => { $("#visualizar_" + x.toLowerCase()).css("background-color", ""); });
    });
}

function ocultarNulos() {

    let todos = null;
    todos = $(".campo");


    todos.each(x => {

        console.log(todos[x].id);
        let valorTexto = (todos[x].innerText != undefined) ? (todos[x].innerText.split(":")[1] != undefined) ? todos[x].innerText.split(":")[1].trim() : "" : "";


        $("#" + todos[x].id).hide();
        $("#comercialDivVisualizar").show();

        if (!valorTexto.includes("null") && !valorTexto.includes("undefined") && valorTexto != "") {

            if (todos[x].id != null && todos[x].id.trim() != "" && !todos[x].id.includes("undefined")) {
                if ($("#" + todos[x].id + "> span").text().trim() != "") {
                    console.log(todos[x].id + ": ");
                    console.log($("#" + todos[x].id + "> span").text().trim());
                    $("#" + todos[x].id).show();
                }
            }

        }
    });
}

