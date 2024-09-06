var viewModel;
var datosIniCrearContrato;
var externo;
var enviarEmail;

$(document).ready(function () {
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
    let fechaDesde = $("#FechaConfirmacionDesdeId").data("kendoDatePicker").value();
    let fechaHasta = $("#FechaConfirmacionHastaId").data("kendoDatePicker").value();
    let claseNegocio = $("#tipoId").val();
    let contratoDesde = $("#contratoDesde").val();
    let contratoHasta = $("#contratoHasta").val();

    BlockUi("Consultando...");

    var filters = {
        logic: "and",
        filters: []
    };

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
    if (claseNegocio) {
        filters.filters.push({ field: "ClaseNegocio", operator: "eq", value: parseInt(claseNegocio) });
    }

    var data = {
        Filter: filters,
        Take: 100000,
        Skip: 0
    };

    var result = MSExecuteOnServer('/Boleto/BuscaDatosTabla', data);

    $.unblockUI();

    if (result && result.Data) {
        if (result.Data.length == 0) {
            MensAlerta("Sin Resultados");
            $("#contratos-grid").data("kendoGrid").dataSource.data([]);
        } else {
            // Actualizar la grilla Kendo UI con los resultados
            var grid = $("#contratos-grid").data("kendoGrid");
            grid.dataSource.data(result.Data.map(function (item) {
                return {
                    ...item,
                    FechaOperacion: parseDate(item.FechaOperacion),
                    FechaConfirmacion: parseDate(item.FechaConfirmacion)
                };
            }));
            $("#contratos-grid").show();
        }
    } else {
        MensErr(result.Mensaje || "Ocurrió un error al intentar obtener los Negocios.");
    }
}

function GeneraBoletoCambioVariosContratos() {
    var lista = $("#contratoDesde").val().split(';');
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
                alert("No hay filas seleccionadas para exportar.");
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
                { field: "ContratoSAP", title: "Contrato SAP", width: 150 },
                { field: "FijacionSAP", title: "Fijación SAP", width: 150 },
                { field: "Version_Proxima", title: "Versión Próxima", width: 150 },
                { field: "TipoBoleto", title: "Tipo Boleto", width: 150 },
                { field: "Bolsa", title: "Bolsa", width: 150 },
                { field: "FechaOperacion", title: "Fecha Operación", width: 150 },
                { field: "FechaConfirmacion", title: "Fecha Confirmación", width: 150 },
                { field: "Corredor", title: "Corredor", width: 150 },
                { field: "Vendedor", title: "Vendedor", width: 150 },
                { field: "Precio", title: "Precio", width: 150 },
                { field: "Moneda", title: "Moneda", width: 150 },
                { field: "TipoNegocio", title: "Tipo de Contrato", width: 150 },
                { field: "Canje", title: "Canje", width: 150 }
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
            console.log(e.workbook);
        },
        dataSource: {
            transport: {
                read: {
                    url: "/Boleto/BuscaDatosTabla",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: function () {
                        // Construir el objeto de filtros y parámetros
                        var filters = {
                            logic: "and",
                            filters: []
                        };

                        var contratoDesde = $("#contratoDesde").val();
                        var contratoHasta = $("#contratoHasta").val();
                        var fechaDesde = $("#FechaConfirmacionDesdeId").val();
                        var fechaHasta = $("#FechaConfirmacionHastaId").val();
                        var claseNegocio = $("#tipoId").val();

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
                        if (claseNegocio) {
                            filters.filters.push({ field: "ClaseNegocio", operator: "eq", value: claseNegocio });
                        }

                        return JSON.stringify({
                            filtro: filters,
                            Take: 10,
                            Skip: 0
                        });
                    }
                }
            },
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    fields: {
                        ContratoSAP: { type: "string" },
                        FijacionSAP: { type: "string" },
                        Version_Proxima: { type: "number" },
                        TipoBoleto: { type: "string" },
                        Bolsa: { type: "string" },
                        FechaOperacion: { type: "date" },
                        FechaConfirmacion: { type: "date" },
                        Corredor: { type: "string" },
                        Vendedor: { type: "string" },
                        Precio: { type: "number" },
                        Moneda: { type: "string" },
                        TipoNegocio: { type: "string" },
                        Canje: { type: "string" },
                        NegocioSAP: { type: "string" }
                    }
                }
            },
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
                width: 100
            },
            { field: "ContratoSAP", title: "Contrato SAP", width: 150 },
            { field: "FijacionSAP", title: "Fijación SAP", width: 150 },
            { field: "Version_Proxima", title: "Versión Próxima", width: 150, format: "{0:N0}" },
            { field: "TipoBoleto", title: "Tipo Boleto", width: 150 },
            { field: "Bolsa", title: "Bolsa", width: "100px" },
            { field: "FechaOperacion", title: "Fecha Operación", width: 150, format: "{0:dd/MM/yyyy}" },
            { field: "FechaConfirmacion", title: "Fecha Confirmación", width: 150, format: "{0:dd/MM/yyyy}" },
            { field: "Corredor", title: "Corredor", width: 150 },
            { field: "Vendedor", title: "Vendedor", width: 150 },
            { field: "Precio", title: "Precio", width: 150 },
            { field: "Moneda", title: "Moneda", width: 150 },
            { field: "TipoNegocio", title: "Tipo de Contrato", width: 150 },
            { field: "Canje", title: "Canje", width: 150 }
        ]
    });
}

function inicializarFiltros() {
    $("#FechaConfirmacionDesdeId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy"
    });

    $("#FechaConfirmacionHastaId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy"
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

    if (response && response.Mensaje == '') {
        // Redireccionar si el negocio es válido
        var redirectUrl = `/Boleto/GestionarClausulas?numeroSap=${encodeURIComponent(negocioSAP)}&tipoNegocio=${tipoNegocioId}`;
        window.location.href = redirectUrl;
    } else {
        // Mostrar mensaje de error si el negocio no es válido
        MensErr(response.Mensaje || "Ocurrió un error al intentar validar el Negocio.");
    }
}