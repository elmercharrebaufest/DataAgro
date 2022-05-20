$(document).ready(function () {

    kendo.culture("es-AR");

    inicializarTodosKendoDate($(".filtroFecha"));
    $("#fechaCargaId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
    CreateGridInformeCompraNet();
    InicializarElementos();


});


function CreateGridInformeCompraNet() {
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });

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
                url: '/Contrato/BuscaDatosTabla',

                data: function () {

                    let filtroCompleto = TraerFiltrosConValores();

                    return filtroCompleto;
                }
            }

        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Cuit: { type: "number" },
                    Negocio: { type: "number" },
                    Acuerdo: { type: "number" },
                    Fecha: { type: "date" },
                    FechaCierta: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    FechaEntrega: { type: "date" },
                    Fecha_Dolarizado: { type: "date" },
                    FechaConfirmacion: { type: "date" },
                    UsuarioConfirmador: { type: "string" },
                    Cantidad: { type: "number" },
                    CantidadAmpliado: { type: "number" },
                    Precio: { type: "number", format: "n2" },
                    Pizarra: { type: "boolean" },
                    Dias_Pesificado: { type: "number" },
                    Pesificado: { type: "boolean" },
                    Sustentable: { type: "boolean" },
                    Dolarizado: { type: "boolean" },
                    NoInformaSIO: { type: "boolean" },
                    TrigoEspecial: { type: "boolean" },
                    DesdeFijacion: { type: "date" },
                    FechaOperacion: { type: "date" },
                    FechaDesde_Sustentable: { type: "date" },
                    FechaHasta_Sustentable: { type: "date" },
                    Pesificado: { type: "boolean" },
                    AnulaYReemplazaContratoSAP: { type: "number" },
                    Precio: { type: "number", format: "n2" },
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],

        pageSize: 20,
    };

    $("#grid").kendoGrid({
        toolbar: kendo.template($("#templateToolbar").html()),
        //toolbar: ["excel"],
        excel: {
            fileName: "Reporte Contratos.xlsx",
            allPages: true,
        },
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
            $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
            $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
            $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
            $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
            $("td:has(div.statusborrado)").css('border-bottom', '5px solid #848484');
            $("td:has(div.statuspreaprobacion)").css('border-bottom', '5px solid #15deca');
            $("td:has(div.statuspreanulado)").css('border-bottom', 'border-grey');
            $("td:has(div.statusreconfirmarfinalizado)").css('border-bottom', '5px solid #ac67ca');
        },
        columns: [
            {
                field: "Cuit", title: "CUIT", width: 90, template: function (dataItem) {
                    if (dataItem.Estado === 1) {
                        return '<div class="statuspendiente "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 2) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 3) {
                        return '<div class="statusoferta "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 4) {
                        return '<div class="statuserror "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 5) {
                        return '<div class="statusfinalizado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 6) {
                        return '<div class="statusborrado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 9) {
                        return '<div class="statuspreaprobacion "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 10) {
                        return '<div class="statuspreanulado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 11) {
                        return '<div class="statusreconfirmarfinalizado "></div>' + dataItem.Proveedor;
                    }
                }
            },

            {
                field: "Proveedor", type: "string", width: 300,
            }, {
                field: "Corredor", type: "string", width: 300,
            },
            {
                field: "Negocio", width: 90
            },
            {
                field: "Acuerdo", width: 90
            },
            { field: "Fecha", title: "Carga", width: 80, format: _DefaultDateTemplate },
            {
                field: "Hora", value: "Hora", title: "Hora", width: 50, template:
                    function (dataItem) {
                        var numeros = dataItem.Hora.split(":");
                        if (numeros[0] < 10) {
                            numeros[0] = "0" + numeros[0];
                        }
                        if (numeros[1] < 10) {
                            numeros[1] = "0" + numeros[1];
                        }
                        return numeros[0] + ":" + numeros[1];
                    }
            },
            {
                field: "TipoNegocio", title: "Tipo", width: 100
            },
            {
                field: "Material", width: 60, template: "#=Material#"
            },

            {
                field: "Cantidad", format: "{0:n0}", width: 80
            },
            {
                field: "CantidadAmpliado", title: "Ampliado", format: "{0:n0}", width: 80
            },
            {
                field: "Precio", type: "number", format: "{0:n2}", width: 80
            },
            {
                field: "Moneda", title: "Moneda", width: 70
            },
            { field: "PrecioNeto", title: "Precio Neto", type: "number", format: "{0:n2}", width: 80 },
            { field: "DestinoDescripcion", title: "Centro", width: 80 },
            { field: "Pizarra", title: "Pizarra", template: function (dataItem) { return dataItem.Pizarra ? "Si" : "No"; }, width: 70 },
            { field: "ImporteFinanciero", title: "Importe <br>Financiero", type: "number", format: "{0:n2}", width: 80 },
            { field: "MonedaFinanciero", title: "Moneda", type: "string", width: 70 },
            { field: "ImporteRedespacho", title: "Importe<br>Redespacho", type: "number", format: "{0:n2}", width: 80 },
            { field: "MonedaRedespacho", title: "Moneda", type: "string", width: 80 },
            { field: "PorcentajeComision", title: "Porcentaje<br> Comision", type: "number", format: "{0:n2}", width: 80 },
            { field: "ImporteComision", title: "Importe<br> Comision", type: "number", format: "{0:n2}", width: 80 },
            { field: "MonedaComision", title: "Moneda", type: "string", width: 80 },
            { field: "ImporteBonificacion", title: "Importe<br> Bonificacion", type: "number", format: "{0:n2}", width: 80 },
            { field: "PorcentajeBonificacion", title: "Porcentaje<br> Bonificacion", type: "number", format: "{0:n2}", width: 80 },
            { field: "MonedaBonificacion", title: "Moneda", type: "string", width: 80 },
            { field: "ImporteBasis", title: "Importe<br> Basis", type: "number", format: "{0:n2}", width: 80 },
            { field: "MonedaBasis", title: "Moneda", type: "string", width: 80 },

            { field: "Provincia", width: 80 },
            { field: "Localidad", width: 80 },
            { field: "Campania", value: "Campania", title: "Campaña", width: 80 },//30
            {
                title: "Fecha", columns: [
                    { field: "FechaDesde", type: "date", title: "Desde", format: _DefaultDateTemplate, width: 80 },
                    { field: "FechaHasta", type: "date", title: "Hasta", format: _DefaultDateTemplate, width: 80 }//col num 32
                ]
            },

            { field: "Comercial", title: "Comercial", width: 180 },
            { field: "ComercialCreador", title: "Registro <br>Comercial", width: 180 },
            {
                field: "Sustentable", columns: [
                    {
                        field: "Sustentable", title: "Sust.", template: function (dataItem) {
                            //console.log(dataItem.TarifaAConvenir);
                            return dataItem.Sustentable ? "Si" : (dataItem.TarifaAConvenir ? "Si" : "No");
                        }, width: 80
                    },
                    { field: "Importe_Sustentable", title: "Importe", filterable: false, width: 80 },
                    { field: "Moneda_Sustentable", title: "Moneda", filterable: false, width: 80 },
                    {
                        field: "TarifaAConvenir", title: "Tarifa a Convenir", width: 120, template: function (dataItem) {
                            return dataItem.TarifaAConvenir ? "Si" : "No";
                        }
                    },
                    {
                        field: "FechaDesde_Sustentable", title: "Desde", filterable: false, width: 80, format: _DefaultDateTemplate, template: function (dataItem) {
                            return dataItem.FechaDesde_Sustentable ? kendo.toString(kendo.parseDate(dataItem.FechaDesde_Sustentable, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    },
                    {
                        field: "FechaHasta_Sustentable", title: "Hasta", filterable: false, width: 80, format: _DefaultDateTemplate, template: function (dataItem) {
                            return dataItem.FechaHasta_Sustentable ? kendo.toString(kendo.parseDate(dataItem.FechaHasta_Sustentable, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    }

                ]
            },
            {
                field: "Dolarizado", columns: [
                    { field: "Dolarizado", title: "Dolar.", template: function (dataItem) { return dataItem.Dolarizado ? "Si" : "No"; }, width: 80 },
                    {
                        field: "Fecha_Dolarizado", title: "Fecha", filterable: false, width: 80, format: _DefaultDateTemplate, template: function (dataItem) {
                            return dataItem.Dolarizado ? kendo.toString(kendo.parseDate(dataItem.Fecha_Dolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    }
                ]
            },
            {
                field: "Dolarizado <br>Express", columns: [//48
                    { field: "DolarizadoExpress", title: "Dolar. Express", template: function (dataItem) { return dataItem.DolarizadoExpress ? "Si" : "No"; }, width: 80 },
                    {
                        field: "Fecha_Dolarizado", title: "Fecha", format: _DefaultDateTemplate, filterable: false, width: 80, template: function (dataItem) {
                            return dataItem.DolarizadoExpress ? kendo.toString(kendo.parseDate(dataItem.Fecha_Dolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    }
                ]
            },
            {
                field: "Dolarizado <br>Corredor", columns: [
                    { field: "DolarizadoCorredor", title: "Dolar. Corredor", template: function (dataItem) { return dataItem.DolarizadoCorredor ? "Si" : "No"; }, width: 80 },
                    {
                        field: "Fecha_Dolarizado", title: "Fecha", format: _DefaultDateTemplate, filterable: false, width: 80, template: function (dataItem) {
                            return dataItem.DolarizadoCorredor ? kendo.toString(kendo.parseDate(dataItem.Fecha_Dolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    }
                ]
            },
            {
                field: "Pago Diferido<br> en pesos", columns: [
                    { field: "Pesificado", title: "Pago Dif.", template: function (dataItem) { return dataItem.Pesificado ? "Si" : "No"; }, width: 80 },
                    { field: "Dias_Pesificado", title: "Dias", filterable: false, width: 80 }
                ]
            },
            { field: "NoInformaSIO", title: "No informa SIO", width: 80, headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.NoInformaSIO ? "Si" : "No"; } },
            { field: "TrigoEspecial", title: "Trigo<br> Especial", width: 60, headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.TrigoEspecial ? "Si" : "No"; } },
            {
                field: "Estado_Contrato", title: "Estado", width: 90, sortable: false
            },
            { field: "Observacion", type: "string", filterable: false, attributes: { "class": "ColumnaObservacion" }, width: 80 },
            { field: "FechaCierta", type: "date", title: "Fecha<br> Cierta", format: _DefaultDateTemplate, width: 80 },
            { field: "Rechazo", type: "string", title: "Motivo<br> Rechazo", width: 80 },
            { field: "ClasificacionDescripcion", type: "string", title: "Clasificación", width: 80 },
            { field: "FechaOperacion", type: "date", title: "Fecha <br>Operacion", format: _DefaultDateTemplate, width: 80 },
            { field: "MotivoOperacionAnterior", type: "string", title: "Motivo <br>Operación<br> Anterior", width: 80 },
            { field: "DescripcionOperacionAnterior", type: "string", title: "Descripcion <br>Operación<br> Anterior", width: 80 },
            { field: "UsuarioConfirmador", type: "string", title: "Usuario <br>Confirmador", width: 80 },
            {
                field: "FechaConfirmacion", type: "date", title: "Fecha <br>Confirmación", format: _DefaultDateTemplate, width: 120,

                template: function (dataItem) {
                    if (dataItem.FechaConfirmacion != null) {
                        return '<div class="statusexterno "></div>' + kendo.toString(kendo.parseDate(dataItem.FechaConfirmacion, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },

            { field: "ChequeElectronicoValor", type: "string", title: "Cheque <br>Electrónico", width: 80 },
            { field: "PagoCBU", type: "string", title: "Pago Cbu", width: 470 },
            { field: "ObligatoriedadCostoFinancieroDesc", title: "Obligatoriedad <br>Costo <br>Financiero", width: 80 },
            { field: "PosicionCBOT", type: "string", title: "Posicion", width: 80 },
            { field: "TipoPosicionCBOT", type: "string", title: "Tipo Posicion", width: 80 },
            { field: "UsuarioTercero", type: "string", title: "Usuario <br>Tercero", width: 150 },
            { field: "AnulaYReemplazaContratoSAP", type: "string", title: "Anula y <br>reemplaza", width: 180 },
            { field: "MotivoReemplazo", type: "string", title: "Motivo", width: 180 },
            { field: "Cesion", type: "string", title: "Cesion", template: function (dataItem) { return dataItem.Cesion ? "Si" : "No"; }, width: 80 },
            { field: "ObligatoriedadBonificacionDesc", title: "Obligatoriedad<br> bonificacion", width: 80},
            { field: "PrecioPonderado", title: "Precio<br> Ponderado", type: "number", format: "{0:n2}", width: 100 },

            { field: "Condicional", title: "Condicional", type: "string", template: function (dataItem) { return dataItem.Condicional ? "Si" : "No"; }, width: 80 },
            { field: "CondicionalPrecio", title: "Precio<br> Strike", type: "number", format: "{0:n2}", width: 80 },
            { field: "CondicionalMonedaId", title: "Moneda", width: 80 },
            { field: "CondicionalCantidad", title: "Cantidad", type: "number", format: "{0:n0}", width: 80 },
            { field: "CondicionalFechaFormateado", title: "Fecha", type: "date", format: _DefaultDateTemplate, width: 80 },
            { field: "CondicionalPosicion", title: "Posicion", type: "string", width: 80 },
            { field: "CondicionalContratoSAP", title: "Condicional<br> Contrato", type: "string", width: 80 },

            { field: "RazonSocialProveedorComisionista", title: "Comisionista", type: "string", width: 300 },
            { field: "BoletoDescripcion", title: "Boleto", type: "string", width: 80 },
            { field: "MercaderiaDescripcion", title: "Mercaderia en depósito", type: "string", width: 300 },
            {
                field: "CantidadDeposito", title: "Cantidad en depósito", type: "number", width: 80, minResizableWidth: 80, format: "{0:n0}", attributes: {
                    "class": "mobile-xs"
                }
            },
            
        ],
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var i = 0; i < sheet.rows[0].cells.length; i++) {
                sheet.rows[0].cells[i].value = sheet.rows[0].cells[i].value.replace("<br>", "").replace("<br>", "").replace("<br>", "");
            }
            for (var i = 0; i < sheet.rows[1].cells.length; i++) {
                sheet.rows[1].cells[i].value = sheet.rows[1].cells[i].value.replace("<br>", "").replace("<br>", "").replace("<br>", "");
            }
            var templateHora = kendo.template(this.columns[6].template);
            var templatePizarra = kendo.template(this.columns[15].template);
            var templateSustentable = kendo.template(this.columns[34].columns[0].template);
            //var templateTarifaAConvenir = kendo.template(this.columns[34].columns[3].template);
            var templateDolarizado = kendo.template(this.columns[35].columns[0].template);
            var templateDolarizadoExpress = kendo.template(this.columns[36].columns[0].template);
            var templateDolarizadoCorredor = kendo.template(this.columns[37].columns[0].template);
            var templatePesificado = kendo.template(this.columns[38].columns[0].template);
            var templateSIO = kendo.template(this.columns[39].template);
            var templateTrigoEsp = kendo.template(this.columns[40].template);
            var templateCesion = kendo.template(this.columns[59].template);
            var templateCondicional = kendo.template(this.columns[62].template);

            for (var i = 2; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataItem = {
                    Hora: row.cells[6].value,
                    Pizarra: row.cells[15].value,
                    Sustentable: row.cells[35].value,
                    TarifaAConvenir: row.cells[38].value,
                    Dolarizado: row.cells[41].value,
                    DolarizadoExpress: row.cells[43].value,
                    DolarizadoCorredor: row.cells[45].value,
                    Pesificado: row.cells[47].value,
                    NoInformaSIO: row.cells[49].value,
                    TrigoEspecial: row.cells[50].value,
                    Cesion: row.cells[69].value,
                    Condicional: row.cells[72].value,
                };

                var operacionFecha = row.cells[5].value;
                operacionFecha.setHours(operacionFecha.getHours() + 1);
                row.cells[5].value = operacionFecha;

                var fechaDesde = row.cells[31].value;
                var fechaHasta = row.cells[32].value;

                if (fechaDesde != null) {
                    fechaDesde.setHours(fechaDesde.getHours() + 1);
                    row.cells[31].value = fechaDesde;
                }

                if (fechaHasta != null) {

                    fechaHasta.setHours(fechaHasta.getHours() + 1);
                    row.cells[32].value = fechaHasta;
                }


                row.cells[6].value = templateHora(dataItem);
                row.cells[15].value = templatePizarra(dataItem);
                row.cells[35].value = templateSustentable(dataItem);
                row.cells[38].value = row.cells[38].value == true ? "Si" : "No";
                row.cells[41].value = templateDolarizado(dataItem);
                row.cells[42].value = row.cells[41].value == "Si" ? row.cells[42].value : "";
                row.cells[43].value = templateDolarizadoExpress(dataItem);
                row.cells[44].value = row.cells[43].value == "Si" ? row.cells[44].value : "";
                row.cells[45].value = templateDolarizadoCorredor(dataItem);
                row.cells[46].value = row.cells[45].value == "Si" ? row.cells[46].value : "";
                row.cells[47].value = templatePesificado(dataItem);
                row.cells[49].value = templateSIO(dataItem);
                row.cells[50].value = templateTrigoEsp(dataItem);
                row.cells[69].value = templateCesion(dataItem);
                row.cells[72].value = templateCondicional(dataItem);

            }
        },
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
        scrollable: true,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        selectable: "row",
        height: 550,
        filterable: false,
        filterMenuInit: function (e) {
            if (e.field == "Proveedor" || e.field == "Comercial" || e.field == "Provincia" || e.field == "Localidad") {
                $(e.container).css("width", "300px");
            } else {
                $(e.container).css("width", "150px");
            }
        }
    });


    //Con definir un método de estos para cada columna multiselect estamos,
    //function createMultiSelectComercial(element) {
    //    return createMultiSelect(element, "Comercial", "ComercialId", "/Contrato/ListarComercial");
    //}

    //function createMultiSelectProvincia(element) {
    //    return createMultiSelect(element, "Provincia", "ProvinciaId", "/Contrato/ListarProvincia");
    //}

    //function createMultiSelectLocalidad(element) {
    //    return createMultiSelect(element, "Localidad", "LocalidadId", "/Contrato/ListarLocalidad");
    //}

    //function createMultiSelectProveedor(element) {
    //    return createMultiSelect(element, "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    //}
}

function ObtenerFecha() {
    var hoy = new Date();
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

function ObtenerFechaMas30() {
    var hoy = new Date();
    hoy.setDate(hoy.getDate() + 30);
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

function InicializarElementos() {
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    kendo.culture("es-AR");

    $("#nuevoNumContrato").kendoNumericTextBox({
        culture: "es-AR",
        format: "######################",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Corredor", "CorredorId", "/Contrato/ListarCorredor");
    CrearMultiSelectFiltro("#ClasificacionId", "Clasificacion", "ClasificacionId", "/Contrato/ListarClasificacion");


    $("#SustentableId").click(function () {
        if ($("#SustentableId").is(':checked')) {
            $("#Importe_SustentableId").prop('disabled', true);
            $("#Importe_SustentableId").data("kendoNumericTextBox").value("");
        } else {
            $("#Importe_SustentableId").prop('disabled', false);
        }
    });
    $("#PesificadoId").click(function () {
        if ($("#PesificadoId").is(':checked')) {
            $("#Dias_PesificadoId").prop('disabled', true);
            $("#Dias_PesificadoId").data("kendoNumericTextBox").value("");
        } else {
            $("#Dias_PesificadoId").prop('disabled', false);
        }
    });

    inicializarPopUpSap("Contratos");
}

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}

$("#DolarizadoId").click(function () {

    if ($("#DolarizadoId").is(':checked')) {
        $("#fechalimiteId").data("kendoDatePicker").enable(false);
        $("#fechalimiteId").data("kendoDatePicker").value("");
    } else {
        $("#fechalimiteId").data("kendoDatePicker").enable();
    }
});

//function filtroContratoSap(nombreDelFiltro) {

//    let filtrosContratosSap = [];
//    $("#contratos-table td").each(function (e) {

//        let valorBuscado = $("#contratos-table td")[e].innerText;

//        (valorBuscado != "" &&
//            $(valorBuscado != null)) ? filtrosContratosSap.push(new FiltroHijo(nombreDelFiltro, valorBuscado, "eq")) : null;
//    });
//    return (filtrosContratosSap.length > 0) ? new FiltroPadre("or", filtrosContratosSap) : null;
//}


function customExport() {
    //TraerFiltrosConValores();
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    };
    MSExecuteOnServerAsync('/Contrato/Export', TraerFiltrosConValores(), funcReturn, true);
}

