var viewModel;
var datosIniCrearContrato;
var externo;
var enviarEmail;

// Función para validar el contenido de contratoDesde
function esContratoDesdeValido() {
    var contratoDesde = $("#contratoDesde").val();

    // Si contratoDesde es nulo o vacío, lo consideramos válido
    if (contratoDesde === null || contratoDesde.trim() === '') {
        return true;
    }

    // Verificar que contratoDesde contenga solo números o el símbolo ';'
    return /^[0-9;]+$/.test(contratoDesde.trim());
}

// Función para validar fechas y contratos
function validarFechasYContratos() {
    if (contratoDesde == '' && contratoHasta == '') {
        var fechaDesde = $("#FechaConfirmacionDesdeId").data("kendoDatePicker").value();
        var fechaHasta = $("#FechaConfirmacionHastaId").data("kendoDatePicker").value();
        var contratoDesde = ($("#contratoDesde").val().endsWith(';') ? $("#contratoDesde").val().slice(0, -1) : $("#contratoDesde").val()).trim();
        var contratoHasta = $("#contratoHasta").val().trim();

        if (fechaDesde && fechaHasta && fechaDesde > fechaHasta) {
            return false; // La fecha desde no puede ser mayor que la fecha hasta
        }
    } else {
        // Validar contratos solo si ambos valores están presentes
        if (contratoDesde) {
            if (contratoHasta) {
                // Verificar que contratoDesde sea menor que contratoHasta
                if (parseInt(contratoDesde) >= parseInt(contratoHasta)) {
                    return false; // El contrato desde debe ser menor que el contrato hasta
                }
            }
        }
        // Si todas las validaciones pasan
    }
    return true;
}

$(document).ready(function () {
    $('#ContratosPendientesCheck').prop('checked', true);
    kendo.culture("es-AR");
    console.log("Current Kendo culture:", kendo.culture().name);
    $('#menuproveedor').hide();
    inicializarFiltros();
    inicializarGrillaContratos();

    $('#contratoDesde').on('keypress', function (event) {
        if (event.which === 32) {
            event.preventDefault();
        }
    });

    $('#contratoHasta').on('keypress', function (event) {
        if (event.which === 32) {
            event.preventDefault();
        }
    });
});

function GenerarBoleto(negocioSAP) {
    BlockUi('Cargando...');
    setTimeout(function () {
        var data = {
            ContratoSAP: negocioSAP,
            TipoNegocioId: $("#tipoId").val(),
            Mail: $("#mailId").is(":checked")
        };
        var url = '/Boleto/GenerarBoletos';
        var result = MSExecuteOnServer(url, data);
        $.unblockUI();
        var viewmodel = {
            ObtenerBoletos: result
        }
        kendo.bind($("#ModalBoleto"), viewmodel);
        if (result.length == 0) {
            MensErr("No se generó ningún boleto.");
        } else {
            $("#ModalBoleto").modal("show");
        }
    }, 150);
}

function GenerarBoletos() {
    // Obtener las filas seleccionadas
    var grid = $("#contratos-grid").data("kendoGrid");
    var negocioSAPList = [];
    $("#contratos-grid tbody input.row-checkbox:checked").each(function () {
        var row = $(this).closest("tr");
        var dataItem = grid.dataItem(row);
        negocioSAPList.push(dataItem.NegocioSAP);
    });

    // Validar y Unir todos los codigos en un solo string
    if (negocioSAPList.length > 0) {
        var negocioSAPString = negocioSAPList.join(';');
        GenerarBoleto(negocioSAPString);
    } else {
        MensErr("No hay ningún negocio seleccionado.");
    }
}

$("#contratoDesde").bind("paste", function (e) {
    e.preventDefault();
    var clipText = e.originalEvent.clipboardData ? e.originalEvent.clipboardData.getData('text/plain') : window.clipboardData.getData('text');
    $("#contratoDesde").val(clipText.replace(/(\r\n|\n|\r|\s)/gm, ";"));
    $("#contratoDesde").val(clipText.replace(/\s+/g, ';'));

    GeneraBoletoCambioVariosContratos();
});

$("#contratoDesde").change(GeneraBoletoCambioVariosContratos);

$("body").on("click", "#filtrarBoletos", function () {
    FiltrarBoletos();
});

function FiltrarBoletos() {
    BlockUi('Consultando...');
    setTimeout(function () {
        try {
            if (esContratoDesdeValido() && validarFechasYContratos()) {
                let fechaDesde = $("#FechaConfirmacionDesdeId").data("kendoDatePicker").value();
                let fechaHasta = $("#FechaConfirmacionHastaId").data("kendoDatePicker").value();
                let fechaCargaDesde = $("#FechaCargaId").data("kendoDatePicker").value();
                let fechaCargaHasta = $("#FechaCargaHastaId").data("kendoDatePicker").value();
                let claseNegocio = $("#tipoId").val();
                let contratoDesde = $("#contratoDesde").val().endsWith(';') ? $("#contratoDesde").val().slice(0, -1) : $("#contratoDesde").val();
                let contratoHasta = $("#contratoHasta").val();
                let material = $("#materialId").val();

                var filters = {
                    logic: "and",
                    filters: []
                };

                if (claseNegocio) {
                    filters.filters.push({ field: "ClaseNegocio", operator: "eq", value: parseInt(claseNegocio) });
                }
                if (contratoDesde) {
                    filters.filters.push({ field: calcularTextoNegocio(claseNegocio), operator: "gte", value: contratoDesde });
                }
                if (contratoHasta) {
                    filters.filters.push({ field: calcularTextoNegocio(claseNegocio), operator: "lte", value: contratoHasta });
                }
                if (fechaDesde) {
                    filters.filters.push({ field: "FechaConfirmacion", operator: "gte", value: fechaDesde });
                }
                if (fechaHasta) {
                    filters.filters.push({ field: "FechaConfirmacion", operator: "lte", value: fechaHasta });
                }
                if (fechaCargaDesde) {
                    filters.filters.push({ field: "FechaCarga", operator: "gte", value: fechaCargaDesde });
                }
                if (fechaCargaHasta) {
                    filters.filters.push({ field: "FechaCarga", operator: "lte", value: fechaCargaHasta });
                }
                if (material) {
                    filters.filters.push({ field: "MaterialId", operator: "eq", value: parseInt(material) });
                }
                
                var data = {
                    Filter: filters,
                    Take: 100000,
                    Skip: 0
                };

                var result = MSExecuteOnServer('/Boleto/BuscaDatosTabla', data);

                if (result && result.Data) {
                    if (result.Data.length == 0) {
                        MensAlerta("Sin Resultados");
                        $("#contratos-grid").data("kendoGrid").dataSource.data([]);
                    } else {
                        // Actualizar la grilla Kendo UI con los resultados                         
                        var checkPendiente = $('#ContratosPendientesCheck').is(':checked');
                        if (checkPendiente) {
                            result.Data = result.Data.filter(x => x.Estado_Version == 'Pendiente');
                        }
                        var grid = $("#contratos-grid").data("kendoGrid");
                        grid.dataSource.data(result.Data.map(function (item) {
                            return {
                                ...item,
                                FechaOperacion: parseDate(item.FechaOperacion),
                                FechaConfirmacion: parseDate(item.FechaConfirmacion),
                                FechaConfirmadoSAP: parseDate(item.FechaConfirmadoSAP),
                                FechaGeneracion: parseDate(item.FechaGeneracion),
                                FechaAnulacion: parseDate(item.FechaAnulacion),
                                FechaCarga: parseDate(item.FechaCarga)
                            };
                        }));
                        $("#contratos-grid").show();
                    }
                } else {
                    MensErr(result.Mensaje || "Ocurrió un error al intentar obtener los Negocios.");
                }
            } else {
                if (!esContratoDesdeValido()) MensErr("Solo se admiten números y el ';' en el campo Contrato.");
                else MensErr("El Rango de Contratos o Fechas no es válido. El campo Desde debe tener un valor menor al campo Hasta.");
            }
        } catch (e) {
            console.error("Error al filtrar negocios: ", e);
            MensErr("Ocurrió un error inesperado. Inténtelo nuevamente.");
        } finally {
            $.unblockUI();
        }
    }, 200);
}

function GeneraBoletoCambioVariosContratos() {
    let contratos = $("#contratoDesde").val().endsWith(';') ? $("#contratoDesde").val().slice(0, -1) : $("#contratoDesde").val();
    var lista = contratos.split(';');
    if (lista.length > 1) {
        $("#contratoHasta").attr('disabled', 'disabled');
        $("#contratoHasta").val("");
    } else {
        $("#contratoHasta").removeAttr('disabled');
    }
}

function parseDate(jsonDate) {
    if (jsonDate) {
        // Extraer el timestamp de la cadena JSON
        var timestamp = parseInt(jsonDate.replace('/Date(', '').replace(')/', ''));
        // Crear una fecha a partir del timestamp
        return new Date(timestamp);
    }
    return null;
}

function inicializarGrillaContratos() {
    $("#contratos-grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Seleccionado.xlsx",
            allPages: false
        },
        excelExport: function (e) {
            var grid = $("#contratos-grid").data("kendoGrid");
            var selectedRows = [];
            $("#contratos-grid tbody input.row-checkbox:checked").each(function () {
                var row = $(this).closest("tr");
                var dataItem = grid.dataItem(row);
                selectedRows.push(dataItem);
            });

            if (selectedRows.length === 0) {
                MensAlerta("No hay filas seleccionadas para exportar.");
                e.preventDefault();
                return;
            }

            // Usa el workbook existente en e.workbook
            var workbook = e.workbook;

            var sheet = workbook.sheets[0];
            if (!sheet) {
                sheet = {
                    name: "Datos Seleccionados",
                    rows: [],
                    columns: []
                };
                workbook.sheets.push(sheet);
            } else {
                // Vacíar las filas existentes del sheet
                sheet.rows = [];
                sheet.columns = [];
                sheet.name = "Datos Seleccionados";
            }

            var columns = [
                { field: "NegocioSAP", title: "Contrato", width: 150 },
                { field: "Material", title: "Material", width: 150 },
                { field: "TipoBoleto", title: "Tipo Boleto", width: 150 },
                { field: "Bolsa", title: "Bolsa", width: 150 },
                { field: "Version", title: "Vers.", width: 150 },
                { field: "Estado_Version", title: "Estado V.", width: 150 },
                { field: "FechaGeneracion", title: "Fecha Generación", width: 150 },
                { field: "FechaOperacion", title: "Fecha Operación", width: 150 },
                { field: "FechaConfirmadoSAP", title: "Fecha Confirmación", width: 150 },
                { field: "FechaAnulacion", title: "Fecha Anulación", width: 150 },
                { field: "ContratoVendedor", title: "Contrato Vendedor", width: 150 },
                { field: "ContratoCorredor", title: "Contrato Corredor", width: 150 },
                { field: "Corredor", title: "Corredor", width: 150 },
                { field: "Vendedor", title: "Vendedor", width: 150 },
                { field: "Comercial", title: "Comercial", width: 150 },
                { field: "Precio", title: "Precio", width: 150 },
                { field: "Moneda", title: "Moneda", width: 150 },
                { field: "TipoNegocio", title: "Tipo de Contrato", width: 150 },
                { field: "UsuarioAnulacion", title: "Usuario Anulación", width: 35 },
            ];

            var header = columns.map(function (column) {
                return { value: column.title, background: "#f4f4f4", bold: true };
            });

            sheet.rows.push({ cells: header });

            // Agregar filas seleccionadas
            selectedRows.forEach(function (dataItem) {
                var row = columns.map(function (column) {
                    var value = dataItem[column.field];
                    if (value instanceof Date) {
                        // Formatear fechas
                        value = kendo.toString(value, "dd/MM/yyyy");
                    }
                    return { value: value };
                });
                sheet.rows.push({ cells: row });
            });

            // Ajustar el ancho de todas las columnas a 150
            columns.forEach(function (column, index) {
                sheet.columns[index] = { width: 150 };
            });

            // Actualizar el workbook en e.data
            e.workbook = workbook;
        },
        dataSource: {

            pageSize: 10
        },
        pageable: {
            refresh: true,
            pageSizes: [10, 20, 50, 100, 1000, "all"],
            buttonCount: 5,
            messages: {
                display: "{0} - {1} de {2} elementos",
                empty: "No hay elementos para mostrar",
                page: "Página",
                of: "de {0}",
                itemsPerPage: "elementos por página",
                first: "Primero",
                last: "Último",
                next: "Siguiente",
                previous: "Anterior"
            }
        },
        sortable: true,
        filterable: false,
        scrollable: true,
        columns: [
            {
                field: "NegocioSAP",
                title: "Negocio SAP",
                hidden: true
            },
            {
                field: "Select",
                title: "<input type='checkbox' id='select-all'>",
                template: "<input type='checkbox' class='row-checkbox'/>",
                width: 50,
                sortable: false
            },
            {
                title: "",
                template: `
                    <div style="display: flex; flex-direction: column;">
                        <a onclick="GenerarBoleto('#= NegocioSAP #')" style="margin-bottom: 5px;">
                            <img style="width:16px;height:16px;" src="/Content/Images/agregar-tel-mail.png">
                            <span class="editar-contacto editar-contacto-dc" style="text-decoration: none;"> Generar Boleto</span>
                        </a>
                        <a onclick="GestionarClausulas('#= NegocioSAP #')">
                            <img style="width:16px;height:16px;" src="/Content/Images/contacto-edit.png">
                            <span class="editar-contacto editar-contacto-dc" style="text-decoration: none;"> Editar Clausulas</span>
                        </a>
                    </div>
                `,
            },
            { field: "NegocioSAP", title: "Contrato", width: "auto", type: "string", headerAttributes: { "title": "Contrato" } },
            {
                field: "Material", title: "Material", width: "auto", editable: false, type: "string", headerAttributes: { "title": "Material" },
                filterable: {
                    multi: true,
                    dataSource: [
                        { Material: "Maiz" },
                        { Material: "Trigo" },
                        { Material: "Soja" },
                        { Material: "Girasol" }
                    ],
                }
            },
            { field: "TipoBoleto", title: "Tipo Boleto", width: "auto", headerAttributes: { "title": "Tipo Boleto" }, type: "string" },
            {
                field: "Bolsa", title: "Bolsa", width: "auto", editable: false, type: "string",
                filterable: {
                    multi: true,
                    dataSource: [
                        { Bolsa: "Buenos Aires" },
                        { Bolsa: "Rosario" },
                        { Bolsa: "Bahía Blanca" },
                        { Bolsa: "Santa Fe" },
                        { Bolsa: "Cordoba" },
                        { Bolsa: "Entre Ríos" },
                        { Bolsa: "Chaco" }
                    ],
                }
            },
            { field: "Version", title: "Vers.", width: "auto", headerAttributes: { "title": "Versión" }, type: "number" },
            {
                field: "Estado_Version", title: "Estado V.", width: "auto", headerAttributes: { "title": "Estado Versión" }, editable: false, type: "string",
                filterable: {
                    multi: true,
                    dataSource: [
                        { Estado_Version: "Pendiente" },
                        { Estado_Version: "Vigente" },
                        { Estado_Version: "Anulado" }
                    ],
                }
            },
            { field: "FechaGeneracion", title: "F. Generación", width: "auto", format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Generación" }, type: "date" },
            { field: "FechaOperacion", title: "F. Operación", width: "auto", format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Operación" }, type: "date" },
            { field: "FechaConfirmadoSAP", title: "F. Confirmación", width: "auto", format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Confirmación" }, type: "date" },
            { field: "FechaAnulacion", title: "F. Anulación", width: "auto", format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Anulación" }, type: "date" },
            { field: "ContratoVendedor", title: "C. Vendedor", width: "auto", headerAttributes: { "title": "Contrato Vendedor" }, type: "string" },
            { field: "ContratoCorredor", title: "C. Corredor", width: "auto", headerAttributes: { "title": "Contrato Corredor" }, type: "string" },
            {
                field: "Vendedor",
                title: "Vendedor",
                width: "auto",
                headerAttributes: { "title": "Vendedor" },
                filterable: {
                    multi: true,
                    ui: function (element) {
                        element.kendoMultiSelect({
                            placeholder: "Seleccione vendedores...",
                            dataTextField: "Vendedor",
                            dataValueField: "Vendedor",
                            dataSource: [], // Inicialmente vacío
                            change: function () {
                                var values = this.value();
                                var grid = $("#contratos-grid").data("kendoGrid");
                                grid.dataSource.filter({
                                    logic: "or",
                                    filters: values.map(function (value) {
                                        return { field: "Vendedor", operator: "eq", value: value };
                                    })
                                });
                            }
                        });
                    }
                }
            },
            {
                field: "Corredor",
                title: "Corredor",
                width: 150,
                headerAttributes: { "title": "Corredor" },
                filterable: {
                    multi: true,
                    ui: function (element) {
                        element.kendoMultiSelect({
                            placeholder: "Seleccione corredores...",
                            dataTextField: "Corredor",
                            dataValueField: "Corredor",
                            dataSource: [], // Inicialmente vacío
                            change: function () {
                                var values = this.value();
                                var grid = $("#contratos-grid").data("kendoGrid");
                                grid.dataSource.filter({
                                    logic: "or",
                                    filters: values.map(function (value) {
                                        return { field: "Corredor", operator: "eq", value: value };
                                    })
                                });
                            }
                        });
                    }
                }
            },
            {
                field: "Comercial",
                title: "Comercial",
                width: 150,
                headerAttributes: { "title": "Comercial" },
                filterable: {
                    multi: true, // Permitir selección múltiple
                    ui: function (element) {
                        element.kendoMultiSelect({
                            placeholder: "Seleccione comerciales...",
                            dataTextField: "Comercial",
                            dataValueField: "Comercial",
                            dataSource: [], // Inicialmente vacío
                            change: function () {
                                var values = this.value();
                                var grid = $("#contratos-grid").data("kendoGrid");
                                grid.dataSource.filter({
                                    logic: "or",
                                    filters: values.map(function (value) {
                                        return { field: "Comercial", operator: "eq", value: value };
                                    })
                                });
                            }
                        });
                    }
                }
            },
            { field: "Precio", title: "Precio", type: "number", width: 70, format: "{0:#,##0.00}", headerAttributes: { "title": "Precio" } },
            {
                field: "Moneda", title: "Moneda", width: 35, headerAttributes: { "title": "Moneda" }, editable: false, type: "string",
                filterable: {
                    multi: true,
                    dataSource: [
                        { Moneda: "USD" },
                        { Moneda: "ARP" }
                    ],
                }
            },
            {
                field: "TipoNegocio", title: "Tipo Contrato", width: 55, headerAttributes: { "title": "Tipo Contrato" }, editable: false, type: "string",
                filterable: {
                    multi: true,
                    dataSource: [
                        { TipoNegocio: "CONVENIO" },
                        { TipoNegocio: "FIJ. CONVENIO" },
                        { TipoNegocio: "FASON MP" },
                        { TipoNegocio: "AGENTE DE COMPRAS MP" },
                        { TipoNegocio: "ACUERDO AGENTE" },
                        { TipoNegocio: "CANJE" },
                        { TipoNegocio: "PRESTAMO DEVOLUCION" },
                        { TipoNegocio: "VENTA" },
                        { TipoNegocio: "A FIJAR PASE" },
                        { TipoNegocio: "FIJACION VIRTUAL" },
                        { TipoNegocio: "FIJACION CANJE" },
                        { TipoNegocio: "A FIJAR" },
                        { TipoNegocio: "A PRECIO" },
                        { TipoNegocio: "FIJACION" },
                        { TipoNegocio: "FASON" },
                        { TipoNegocio: "AGENTE DE COMPRAS" },
                        { TipoNegocio: "CONTRATO ACUERDO" },
                        { TipoNegocio: "ESPACIO DINAMICO" }
                    ],
                }
            }
        ]
    });

    $('#contratos-grid').contents().wrap('<div class="grid-scroll-container"></div>');

}

function inicializarFiltros() {
    var hoy = new Date(); //Hoy
    var ayer = new Date(hoy);
    ayer.setDate(hoy.getDate() - 1); //Ayer

    $("#FechaConfirmacionDesdeId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
        value: ayer
    });

    $("#FechaConfirmacionHastaId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
        value: hoy
    });
    $("#FechaCargaId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
        value: hoy
    });
    $("#FechaCargaHastaId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
        value: hoy
    });
}

function calcularTextoNegocio(claseNegocio) { return claseNegocio == '1' ? "ContratoSAP" : "FijacionSAP" };

// Seleccionar todas las filas
$("#contratos-grid").on("change", "#select-all", function () {
    var isChecked = $(this).is(":checked");
    $("#contratos-grid").find("input.row-checkbox").prop("checked", isChecked);
});

function GestionarClausulas(negocioSAP) {
    var tipoNegocioId = $("#tipoId").val();
    // Validar el negocio
    var url = '/Boleto/ValidarNegocio';
    var data = {
        NegocioSAP: negocioSAP
    };
    var response = MSExecuteOnServer(url, data);

    if (response && (response.Mensaje == '' || response.Mensaje == null)) {
        // Redireccionar si el negocio es válido
        var redirectUrl = `/Boleto/GestionarClausulas?numeroSap=${encodeURIComponent(negocioSAP)}&tipoNegocio=${tipoNegocioId}`;
        window.location.href = redirectUrl;
    } else {
        // Mostrar mensaje de error si el negocio no es válido
        MensErr(response.Mensaje || "Ocurrió un error al intentar validar el Negocio.");
    }
}


