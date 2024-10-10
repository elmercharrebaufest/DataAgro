//Inicializar
$(document).ready(function () {
    inicializarFiltros();
    inicializarGrillaContratos();
});

function inicializarFiltros() {
    $("#FechaConfirmacionDesde").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy"
    });

    $("#FechaConfirmacionHasta").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy"
    });
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
                { field: "Version_Proxima", title: "V. Prox.", width: 150 },
                { field: "Estado_Version", title: "Estado V.", width: 150},
                { field: "FechaGeneracion", title: "Fecha Generación", width: 150 },
                { field: "FechaOperacion", title: "Fecha Operación", width: 150 },
                { field: "FechaConfirmacion", title: "Fecha Confirmación", width: 150 },
                { field: "FechaAnulacion", title: "Fecha Anulación", width: 150 },
                { field: "ContratoVendedor", title: "Contrato Vendedor", width: 150 },
                { field: "ContratoCorredor", title: "Contrato Corredor", width: 150 },
                { field: "Corredor", title: "Corredor", width: 150 },
                { field: "Vendedor", title: "Vendedor", width: 150 },
                { field: "Comercial", title: "Comercial", width: 150 },
                { field: "Precio", title: "Precio", width: 150 },
                { field: "Moneda", title: "Moneda", width: 150 },
                { field: "TipoNegocio", title: "Tipo de Contrato", width: 150 },
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
                field: "Select",
                title: "<input type='checkbox' id='select-all'>",
                template: "<input type='checkbox' class='row-checkbox'/>",
                width: 25,
                sortable: false
            },
            {
                title: "",
                template: `
                    <div style="display: flex; flex-direction: column;">
                        <a onclick="GenerarConfirma('#= NegocioSAP #')" title="Generar Confirma">
                            <img style="width:16px;height:16px;" src="/Content/Images/agregar-tel-mail.png" hidden>
                        </a>
                        <a onclick="GestionarClausulas('#= NegocioSAP #')" title="Gestionar Clausulas">
                            <img style="width:16px;height:16px;" src="/Content/Images/pencil_line.png">
                        </a>
                    </div>
                `,
                width: 30
            },
            { field: "NegocioSAP", title: "Contrato", width: 80, },
            { field: "Material", title: "Material", width: 50 },
            { field: "TipoBoleto", title: "Tipo Boleto", width: 60, headerAttributes: { "title": "Tipo Boleto" } },
            { field: "Bolsa", title: "Bolsa", width: 50 },
            { field: "Version_Proxima", title: "Vers. P.", width: 38, headerAttributes: { "title": "Versión Próxima" } },
            { field: "Estado_Version", title: "Estado V.", width: 65, headerAttributes: { "title": "Estado Versión" } },
            { field: "FechaGeneracion", title: "F. Generación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Generación" } },
            { field: "FechaOperacion", title: "F. Operación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Operación" } },
            { field: "FechaConfirmacion", title: "F. Confirmación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Confirmación" } },
            { field: "FechaAnulacion", title: "F. Anulación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Anulación" } },
            { field: "ContratoVendedor", title: "C. Vendedor", width: 80, headerAttributes: { "title": "Contrato Vendedor" } },
            { field: "ContratoCorredor", title: "C. Corredor", width: 80, headerAttributes: { "title": "Contrato Corredor" } },
            { field: "Vendedor", title: "Vendedor", width: 150 },
            { field: "Corredor", title: "Corredor", width: 150 },
            { field: "Comercial", title: "Comercial", width: 150 },
            { field: "Precio", title: "Precio", type: "number", width: 70, format: "{0:#,##0.00}" },
            { field: "Moneda", title: "Moneda", width: 35, headerAttributes: { "title": "Moneda" } },
            { field: "TipoNegocio", title: "Tipo Contrato", width: 55, headerAttributes: { "title": "Tipo Contrato" } },
            { field: "UsuarioAnulacion", title: "U. Anulación", width: 35, headerAttributes: { "title": "Usuario Anulación" } },
        ]
    });
}

//Eventos
$("body").on("click", "#filtrarConfirmas", function () {
    FiltrarNegocios();
});

$("body").on("change", "#ContratoDesde", function () {
    let lista = trimEnd($("#ContratoDesde").val()).split(';');
    if (lista.length > 1) {
        $("#ContratoHasta").attr('disabled', 'disabled');
        $("#ContratoHasta").val("");
    } else {
        $("#ContratoHasta").removeAttr('disabled');
    }
});

$("#ContratoDesde").bind("paste", function (e) {//En caso de Pegar Codigos
    e.preventDefault();
    if (e.originalEvent.clipboardData !== undefined) {
        clipText = e.originalEvent.clipboardData.getData('text/plain');
    } else {
        clipText = window.clipboardData.getData('text');
    }
    $("#ContratoDesde").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));
});

function CargarTablaModal(contratos) {
    $("#tabla-cap-pendientes").empty();
    var tabla = '';
    for (var i = 0; i < contratos.length; i++) {
        tabla += '<tr><td>'
            + contratos[i].NegocioSAP
            + '</td><td>'
            + contratos[i].FechaGeneracionFormateada
            + '</td><td>'
            + (contratos[i].Generado ? '<i class="fa fa-check generado" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
            + '</td><td>'
            + (contratos[i].IsWebService ? '<i class="fa fa-check web" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
            + (contratos[i].Generado ? ('<a href="/Confirma/DescargarArchivoConfirma?nombreArchivo=' + contratos[i].Archivo + '" class="k-button k-button-icontext" style="height: 34px;text-align: center;margin-left: 1rem;"><i class="fa fa-download generado" style="text-align: center"></i></a>') : "")
            + '</td><td>'
            + contratos[i].Mensaje
            + '</td></tr>';
    }
    $("#tabla-cap-pendientes").append(tabla);
}

function FiltrarNegocios() {
    BlockUi('Consultando...');
    setTimeout(function () {
        try {
            if (esContratoDesdeValido() && validarFechasYContratos()) {
                let fechaDesde = $("#FechaConfirmacionDesde").data("kendoDatePicker").value();
                let fechaHasta = $("#FechaConfirmacionHasta").data("kendoDatePicker").value();
                let claseNegocio = $("#ClaseNegocio").val();
                let contratoDesde = $("#ContratoDesde").val().endsWith(';') ? $("#ContratoDesde").val().slice(0, -1) : $("#ContratoDesde").val();
                let contratoHasta = $("#ContratoHasta").val();

                var filters = {
                    logic: "and",
                    filters: []
                };

                if (claseNegocio) {
                    filters.filters.push({ field: "ClaseNegocio", operator: "eq", value: parseInt(claseNegocio) });
                }
                if (contratoDesde) {
                    filters.filters.push({ field: "NegocioSAP", operator: "gte", value: contratoDesde });
                }
                if (contratoHasta) {
                    filters.filters.push({ field: "NegocioSAP", operator: "lte", value: contratoHasta });
                }
                if (fechaDesde) {
                    filters.filters.push({ field: "FechaConfirmacion", operator: "gte", value: fechaDesde });
                }
                if (fechaHasta) {
                    filters.filters.push({ field: "FechaConfirmacion", operator: "lte", value: fechaHasta });
                }

                var data = {
                    Filter: filters,
                    Take: 100000,
                    Skip: 0
                };

                var result = MSExecuteOnServer('/Confirma/BuscaDatosTabla', data);

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
                                FechaConfirmacion: parseDate(item.FechaConfirmacion),
                                FechaGeneracion: parseDate(item.FechaGeneracion)
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

function ValidarNegocio(codigoSAP) {
    let claseNegocio = $("#ClaseNegocio").val();
    BlockUi("Consultando...");
    var data = { codigoSAP: codigoSAP, claseNegocio: claseNegocio };
    var result = MSExecuteOnServer('/Confirma/ValidarNegocio', data);
    $.unblockUI();
    return result;
}

//Funciones Utiles
function isNullOrWhitespace(input) {
    return !input || !input.trim();
}

function trimEnd(cadena) {
    return cadena.at(cadena.length - 1) == ';' ? cadena.slice(0, cadena.length - 1) : cadena;
}

// Función para validar el contenido de contratoDesde
function esContratoDesdeValido() {
    var contratoDesde = $("#ContratoDesde").val();

    // Si contratoDesde es nulo o vacío, lo consideramos válido
    if (contratoDesde === null || contratoDesde.trim() === '') {
        return true;
    }

    // Verificar que contratoDesde contenga solo números o el símbolo ';'
    return /^[0-9;]+$/.test(contratoDesde.trim());
}

// Función para validar fechas y contratos
function validarFechasYContratos() {
    var fechaDesde = $("#FechaConfirmacionDesde").data("kendoDatePicker").value();
    var fechaHasta = $("#FechaConfirmacionHasta").data("kendoDatePicker").value();
    var contratoDesde = ($("#ContratoDesde").val().endsWith(';') ? $("#ContratoDesde").val().slice(0, -1) : $("#ContratoDesde").val()).trim();
    var contratoHasta = $("#ContratoHasta").val().trim();

    if (fechaDesde && fechaHasta && fechaDesde > fechaHasta) {
        return false; // La fecha desde no puede ser mayor que la fecha hasta
    }

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
    return true;
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

function GenerarConfirmas() {
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
        GenerarConfirma(negocioSAPString);
    } else {
        MensErr("No hay ningún negocio seleccionado.");
    }
}

//Funcion Generar Confirma
function GenerarConfirma(negocioSAP) {
    BlockUi('Cargando...');
    setTimeout(function () {
        try {
            var data = {
                ContratoSAP: negocioSAP,
                ClaseNegocioId: $("#ClaseNegocio").val(),
                IsWebService: $("#servicioConfirmaId").is(":checked")
            };

            var url = '/Confirma/GenerarConfirma';
            var result = MSExecuteOnServer(url, data);

            if (result.confirmasGenerados == undefined || result.confirmasGenerados.length == 0) {
                if (result.HayError) {
                    result.ListaErrores.forEach(err => MensErr(err.Message));
                } else {
                    MensErr("No se generó ningún Confirma.");
                }
            } else {
                CargarTablaModal(result.confirmasGenerados);
                $("#ModalConfirma").modal("show");
            }
        } catch (e) {
            console.error("Error al Generar Confirma: ", e);
            MensErr("Ocurrió un error inesperado. Inténtelo nuevamente.");
        } finally {
            $.unblockUI();
        }
    }, 200);
}

$("#contratos-grid").on("change", "#select-all", function () {
    var isChecked = $(this).is(":checked");
    $("#contratos-grid").find("input.row-checkbox").prop("checked", isChecked);
});

function GestionarClausulas(negocioSAP) {
    var tipoNegocioId = $("#ClaseNegocio").val();
    // Validar el negocio
    var url = '/Confirma/ValidarNegocio';
    var data = {
        NegocioSAP: negocioSAP
    };
    var response = MSExecuteOnServer(url, data);

    if (response && response.Mensaje == '') {
        // Redireccionar si el negocio es válido
        var redirectUrl = `/Confirma/GestionarClausulas?numeroSap=${encodeURIComponent(negocioSAP)}&tipoNegocio=${tipoNegocioId}`;
        window.location.href = redirectUrl;
    } else {
        // Mostrar mensaje de error si el negocio no es válido
        MensErr(response.Mensaje || "Ocurrió un error al intentar validar el Negocio.");
    }
}