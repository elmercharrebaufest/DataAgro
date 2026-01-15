//Inicializar
$(document).ready(function () {
    $('#ContratosPendientesCheck').prop('checked', true);
    inicializarFiltros();
    inicializarGrillaContratos();
});

function inicializarFiltros() {
    var hoy = new Date();
    var ayer = new Date(hoy);
    ayer.setDate(hoy.getDate() - 1);

    $("#FechaConfirmadoSAPDesde").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
        value: ayer
    });

    $("#FechaConfirmadoSAPHasta").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
        value: hoy
    });

    $("#fechaCargaId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
    });

    $("#fechaCargaHastaId").kendoDatePicker({
        weekNumber: true,
        format: "dd/MM/yyyy",
    });
    inicializarPopUpSap("Contratos");
}

function inicializarGrillaContratos() {
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
                url: '/Confirma/BuscaDatosTabla',

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
                    Select: { type: "string" },
                    NegocioSAP: { type: "string" },
                    Material: { type: "string" },
                    TipoBoleto: { type: "string" },
                    Bolsa: { type: "string" },
                    Version: { type: "string" },
                    Estado_Version: { type: "string" },
                    FechaGeneracion: { type: "date" },
                    FechaOperacion: { type: "date" },
                    FechaConfirmadoSAP: { type: "date" },
                    FechaAnulacion: { type: "date" },
                    ContratoVendedor: { type: "string" },
                    ContratoCorredor: { type: "string" },
                    Vendedor: { type: "string" },
                    Corredor: { type: "string" },
                    Comercial: { type: "string" },
                    Precio: { type: "number" },
                    Moneda: { type: "string" },
                    TipoNegocio: { type: "string" }
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        pageSize: 15,
        requestEnd: function (e) {
            if (e.type === "read" && e.response && e.response.Data) {
                var checkPendiente = $('#ContratosPendientesCheck').is(':checked');
                if (checkPendiente) {
                    let datosFiltrados = e.response.Data.filter(x => x.Estado_Version == 'Pendiente');
                    // Reemplazar los datos originales por los filtrados
                    var grid = $("#contratos-grid").data("kendoGrid");
                    grid.dataSource.data(datosFiltrados);
                }
            }
        }
    };

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
            console.log(e.workbook);
        },
        dataSource: ds,
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
                sortable: false,
                filterable: false
            },
            {
                title: "",
                template: `
                    <div style="display: flex; flex-direction: column;">
                        <a onclick="GenerarConfirma('#= NegocioSAP #')" title="Generar Confirma">
                            <img style="width:16px;height:16px;" src="/Content/Images/agregar-tel-mail.png" hidden>
                        </a>
                        <a onclick="GestionarClausulas('#= NegocioSAP #')" title="Gestionar Clausulas">
                            <img style="width:16px;height:16px;" src="/Content/Images/contacto-edit.png">
                        </a>
                    </div>
                `,
                width: 30,
                sortable: false,
                filterable: false
            },
            { field: "NegocioSAP", title: "Contrato", width: 80, type: "string", headerAttributes: { "title": "Contrato" } },
            {
                field: "Material", title: "Material", width: 50, editable: false, type: "string", headerAttributes: { "title": "Material" },
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
            { field: "TipoBoleto", title: "Tipo Boleto", width: 60, headerAttributes: { "title": "Tipo Boleto" }, type: "string" },
            {
                field: "Bolsa", title: "Bolsa", width: 50, editable: false, type: "string",
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
            { field: "Version", title: "Vers.", width: 38, headerAttributes: { "title": "Versión" }, type: "number" },
            {
                field: "Estado_Version", title: "Estado V.", width: 65, headerAttributes: { "title": "Estado Versión" }, editable: false, type: "string",
                filterable: {
                    multi: true,
                    dataSource: [
                        { Estado_Version: "Pendiente" },
                        { Estado_Version: "Vigente" },
                        { Estado_Version: "Anulado" }
                    ],
                }
            },
            { field: "FechaGeneracion", title: "F. Generación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Generación" }, type: "date" },
            { field: "FechaOperacion", title: "F. Operación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Operación" }, type: "date" },
            { field: "FechaConfirmadoSAP", title: "F. Confirmación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Confirmación" }, type: "date" },
            { field: "FechaAnulacion", title: "F. Anulación", width: 70, format: "{0:dd/MM/yyyy}", headerAttributes: { "title": "Fecha Anulación" }, type: "date" },
            { field: "ContratoVendedor", title: "C. Vendedor", width: 80, headerAttributes: { "title": "Contrato Vendedor" }, type: "string" },
            { field: "ContratoCorredor", title: "C. Corredor", width: 80, headerAttributes: { "title": "Contrato Corredor" }, type: "string" },
            {
                field: "Vendedor",
                title: "Vendedor",
                width: 150,
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
        ],
    });
}

//Eventos
$("body").on("click", "#filtrarConfirmas", function () {
    FiltrarNegocios();
});

$("body").on("change", "#NegocioSAP-desde", function () {
    let lista = trimEnd($("#NegocioSAP-desde").val()).split(';');
    if (lista.length > 1) {
        $("#NegocioSAP-hasta").attr('disabled', 'disabled');
        $("#NegocioSAP-hasta").val("");
    } else {
        $("#NegocioSAP-hasta").removeAttr('disabled');
    }
});

$("#NegocioSAP-desde").bind("paste", function (e) {//En caso de Pegar Codigos
    e.preventDefault();
    if (e.originalEvent.clipboardData !== undefined) {
        clipText = e.originalEvent.clipboardData.getData('text/plain');
    } else {
        clipText = window.clipboardData.getData('text');
    }
    $("#NegocioSAP-desde").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));
});

function CargarTablaModal(contratos) {
    $("#modal-respuestas-confirma").empty();
    let descargaHabilitada = $("#checkDescargar").is(":checked");
    var tabla = '';
    for (var i = 0; i < contratos.length; i++) {
        tabla += '<tr><td>'
            + contratos[i].NegocioSAP
            + '</td><td '
            + (contratos[i].TieneFechaGeneracionUltimoConfirma ? 'title="Último Confirma generado"' : '') + '>'
            + (contratos[i].FechaGeneracionFormateada == '1/1/0001' ? '' : contratos[i].FechaGeneracionFormateada)
            + '</td><td>'
            + (contratos[i].Generado ? '<i class="fa fa-check generado" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
            + '</td><td>'
            + (contratos[i].IsWebService ? '<i class="fa fa-check web" aria-hidden="true" style="color:green; text-align: center"></i>' : '<i class="fa fa-times generado" aria-hidden="true" style="color:red; text-align: center"></i>')
            + (contratos[i].Generado && descargaHabilitada ? ('<a href="/Confirma/DescargarArchivoConfirma?nombreArchivo=' + contratos[i].Archivo + '" class="k-button k-button-icontext" style="height: 34px;text-align: center;margin-left: 1rem;"><i class="fa fa-download generado" style="text-align: center"></i></a>') : "")
            + '</td><td>'
            + contratos[i].Mensaje
            + '</td></tr>';
    }
    $("#modal-respuestas-confirma").append(tabla);
}

function FiltrarNegocios() {
    BlockUi('Consultando...');
    setTimeout(function () {
        try {
            if (esContratoDesdeValido() && validarFechasYContratos()) {

                $('#contratos-grid').data('kendoGrid').dataSource.read();

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
    var contratoDesde = $("#NegocioSAP-desde").val();

    // Si contratoDesde es nulo o vacío, lo consideramos válido
    if (contratoDesde === null || contratoDesde.trim() === '') {
        return true;
    }

    // Verificar que contratoDesde contenga solo números o el símbolo ';'
    return /^[0-9;]+$/.test(contratoDesde.trim());
}

// Función para validar fechas y contratos
function validarFechasYContratos() {
    var contratoDesde = ($("#NegocioSAP-desde").val().endsWith(';') ? $("#NegocioSAP-desde").val().slice(0, -1) : $("#NegocioSAP-desde").val()).trim();
    var contratoHasta = $("#NegocioSAP-hasta").val().trim();

    if (contratoDesde == '' && contratoHasta == '') {
        var fechaDesde = $("#FechaConfirmadoSAPDesde").data("kendoDatePicker").value();
        var fechaHasta = $("#FechaConfirmadoSAPHasta").data("kendoDatePicker").value();
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