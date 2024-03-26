var idModalBorrar = 0;
var eliminarRegistro;

$(document).ready(function () {
    kendo.culture("es-AR");
    eliminarRegistro = document.getElementById('permisos').getAttribute('data-eliminar');
    
    inicializarTodosKendoDate($(".filtroFecha"));
    //$("#fechaCargaId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
    CreateGrid();
    InicializarElementos();
});

function deseleccionarRadioButtonEliminado() {
    $('[name=Eliminado]:checked').prop('checked', false);
}

function CreateGrid() {
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
                url: '/ResearchReporte/BuscaDatosTabla',

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
                    IdPowerApp: { type: "number" },
                    FechaAlta: { type: "date" },
                    FechaModificacion: { type: "date" },
                    MaterialId: { type: "number" },
                    Material: { type: "string" },
                    MaterialIdAntecesor: { type: "number" },
                    MaterialAntecesor: { type: "string" },
                    Author: { type: "string" },
                    Editor: { type: "string" },
                    ComerdialId: { type: "string" },
                    Comercial: { type: "string" },
                    EstadoConectividad: { type: "string" },
                    Sincronizado: { type: "boolean" },
                    TipoCargaId: { type: "number" },
                    TipoCarga: { type: "string" },
                    CampañaId: { type: "number" },
                    Campaña: { type: "string" },
                    Latitud: { type: "number" },
                    Longitud: { type: "number" },
                    LocalidadId: { type: "number" },
                    PartidoId: { type: "number" },
                    ProvinciaId: { type: "number" },
                    Localidad: { type: "string" },
                    Partido: { type: "string" },
                    Provincia: { type: "string" },
                    Comentarios: { type: "string" },
                    EstadioId: { type: "number" },
                    Estadio: { type: "string" },
                    CondicionId: { type: "number" },
                    Condicion: { type: "string" },
                    Coeficiente: { type: "number" },
                    Rendimiento: { type: "number" },
                    //RendimientoCalculado: { type: "number" },
                    HumedadSueloId: { type: "number" },
                    HumedadSuelo: { type: "string" },
                    CapitulosGirasol: { type: "number" },
                    DistanciaHileras: { type: "number" },

                    TipoMuestraIdUno: { type: "number" },
                    TipoMuestraIdDos: { type: "number" },
                    TipoMuestraIdTres: { type: "number" },
                    TipoMuestraUno: { type: "string" },
                    TipoMuestraDos: { type: "string" },
                    TipoMuestraTres: { type: "string" },
                    PromedioMuestraUno: { type: "number" },
                    PromedioMuestraDos: { type: "number" },
                    PromedioMuestraTres: { type: "number" },
                    MedidasUno: { type: "string" },
                    MedidasDos: { type: "string" },
                    MedidasTres: { type: "string" },
                    Adjuntos: { type: "object" },
                    Attachments: { type: "boolean" },
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        sort: [
            { field: "FechaAlta", dir: "desc" }
        ],
        pageSize: 20,
    };

    var grid = $("#grid").kendoGrid({
        //toolbar: kendo.template($("#templateToolbar").html()),
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Research.xlsx",
            allPages: true,
        },
        dataSource: ds,
        columns: [
            { field: "Id", title: "ID", type: "number", format: "{0:n0}", width: 45 },
            { field: "FechaAlta", type: "date", title: "Fecha<br>de Alta", format: _DefaultDateTemplate, width: 80 },
            { field: "Material", title: "Material", width: 80 },
            { field: "Comercial", title: "Comercial", width: 80, template: "<span title='#= Comercial #'>#= Comercial #</span>" },
            { field: "Campaña", title: "Camp.", width: 60 },
            { field: "Provincia", title: "Provincia", width: 80 },
            { field: "Partido", title: "Partido", width: 80 },
            { field: "Localidad", title: "Localidad", width: 80 },
            { field: "Estadio", title: "Estadio", width: 80 },
            { field: "Condicion", title: "Condición", width: 80 },
            { field: "Coeficiente", title: "Coeficiente", width: 80 },
            { field: "HumedadSuelo", title: "Humedad<br>del Suelo", width: 80 },
            { field: "TipoMuestraUno", title: "Muestra<br>Uno", width: 80 },
            { field: "MedidasUno", title: "Medidas", width: 80 },
            { field: "PromedioMuestraUno", title: "Promedio<br>muestra 1", width: 70 },
            { field: "TipoMuestraDos", title: "Muestra<br>Dos", width: 80 },
            { field: "MedidasDos", title: "Medidas", width: 80 },
            { field: "PromedioMuestraDos", title: "Promedio<br>muestra 2", width: 70 },
            { field: "TipoMuestraTres", title: "Muestra<br>Tres", width: 80 },
            { field: "MedidasTres", title: "Medidas", width: 80 },
            { field: "PromedioMuestraTres", title: "Promedio<br>muestra 3", width: 70 },
            { field: "PromedioGranosVaina", title: "Prom. G.<br>por Vaina", width: 70 },
            { field: "Rendimiento", title: "Rendimiento", width: 80 },
            //{ field: "RendimientoCalculado", title: "RendimientoCalculado", width: 80 },
            { field: "CapitulosGirasol", title: "Capítulos<br>Girasol", width: 70 },
            { field: "DistanciaHileras", title: "Distancia<br>Hileras", width: 70 },
            { field: "Latitud", title: "Latitud", width: 70 },
            { field: "Longitud", title: "Longitud", width: 70 },
            { field: "Comentarios", title: "Comentarios", width: 70 },
            { field: "MaterialAntecesor", title: "Material<br>Antecesor", width: 70 },
            { field: "TipoCarga", title: "Tipo de<br>Carga", width: 70 },
            { field: "Eliminado", type: "string", title: "Fue<br>Eliminado", template: function (dataItem) { return dataItem.Eliminado ? "Si" : "No"; }, width: 80 },
            { field: "Adjuntos", template: function (dataItem) { return listaAdjuntos(dataItem); }, title: "Adjuntos", width: 120 },
            {
                template: function (dataItem) {
                    if (eliminarRegistro == 'True') return botonBorrar(dataItem, 'fa-trash err');
                    else return '';
                },
                width: 40, title: "Elim."
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
        scrollable: true,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        selectable: "row",
        height: 550,
        filterable: false,
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var newRows = [];
            for (var i = 0; i < sheet.rows[0].cells.length; i++) {
                sheet.rows[0].cells[i].value = sheet.rows[0].cells[i].value.replace(/<br>/g, " ");
            }

            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];
                row.cells[30].value = row.cells[30].value ? "Si" : "No";
            }

            // Proceso para adjuntos:
            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];
                var dataItem = { Adjuntos: row.cells[31].value };
                
                if (dataItem.Adjuntos.constructor === window.init) {
                    dataItem.Adjuntos = Array.from(dataItem.Adjuntos); // Convertir init a array
                }
                if (dataItem.Adjuntos.length === 0) {
                    row.cells[31].value = "No contiene";
                } else {
                    var imagenData = dataItem.Adjuntos.map(function (imagen) {
                        return {
                            url: imagen.Path,
                            nombre: imagen.Nombre,
                        };
                    });
                    if (imagenData.length === 1) {
                        var hyperlink = `=HYPERLINK("${imagenData[0].url}", "${imagenData[0].nombre}")`;
                        row.cells[31].formula = hyperlink;
                    } else {
                        var firstHyperlink = `=HYPERLINK("${imagenData[0].url}", "${imagenData[0].nombre}")`;
                        row.cells[31].formula = firstHyperlink;

                        // Insertar las demás filas con adjuntos
                        for (var l = 1; l < imagenData.length; l++) {
                            var newRow = { cells: [] };
                            for (var k = 0; k < row.cells.length; k++) {
                                newRow.cells.push({ value: "" }); // Insertar celdas vacías
                            }
                            newRow.cells[31].formula = `=HYPERLINK("${imagenData[l].url}", "${imagenData[l].nombre}")`;
                            newRows.push(newRow);
                        }
                    }
                }
                sheet.rows.splice(i + 1, 0, ...newRows);
                i += newRows.length;
                newRows = [];
            }
        }
    }).data("kendoGrid");

    var exportFlag = false;
    var columnasHidden = [];// ["Latitud", "Longitud", "Comentarios", "MaterialAntecesor", "TipoCarga", "MedidasUno", "MedidasDos", "MedidasTres", "Attachments", "Eliminado"];
    for (var i in columnasHidden) {
        grid.hideColumn(columnasHidden[i]);
    }

    $("#grid").data("kendoGrid").bind("excelExport", function (e) {
        if (!exportFlag) {
            for (var i in columnasHidden) {
                e.sender.showColumn(columnasHidden[i]);
            }
            e.preventDefault();
            exportFlag = true;
            setTimeout(function () {
                e.sender.saveAsExcel();
            });
        } else {
            for (var i in columnasHidden) {
                e.sender.hideColumn(columnasHidden[i]);
            }
            exportFlag = false;
        }
    });
}

function listaAdjuntos(dataItem) {
    var div = "<div>";
    if (dataItem.Adjuntos != null) {
        for (var i = 0; i < dataItem.Adjuntos.length; i++) {
            var objetoActual = dataItem.Adjuntos[i];
            if (objetoActual != undefined && objetoActual.Path != undefined && objetoActual.Path != null && objetoActual.Path != "") {
                var enlace = '<a href="' + objetoActual.Path + '" target="_blank">' + objetoActual.Nombre + '</a>';
                div += enlace;
                div += "<br>";
            }
        }
    }
    div += "</div>";
    return div;
}

function botonBorrar(dataItem, icono) {
    if (dataItem.Eliminado != true) {
        return '<button data-toggle="tooltip" title="Eliminar registro" ' +
            'onclick="ModalBorrar(' +
            "'" + dataItem.Id + "'" + ',' +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return '<div></div>';
    }
}

function ModalBorrar(id) {
    $("#idModalBorrar").html(id);
    idModalBorrar = id;
    $("#modalBorrar").modal('show');
}

$("#confirmarBorrar").click(function () {
    result = MSExecuteOnServer('/ResearchReporte/BorrarRegistro', { id: idModalBorrar });
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
});

function recargarGrilla() {
    $('#grid').data('kendoGrid').dataSource.read();
}

function InicializarElementos() {
    $(".multiselect").kendoMultiSelect({});

    var provinciasData = JSON.parse(document.getElementById("ProvinciaId").getAttribute("data-provincias"));
    $("#ProvinciaId").kendoDropDownList({
        dataTextField: "Nombre",
        dataValueField: "ProvinciaId",
        dataSource: provinciasData,
        optionLabel: "Seleccione una provincia",
        filter: "contains",
        change: function () {
            if (this.value() > 0) {
                var result = MSExecuteOnServer("/ResearchReporte/TraerPartidosPorProvincia", { provinciaId: this.value() });
                var dataSource = new kendo.data.DataSource({
                    data: result,
                    sort: { field: "Descripcion", dir: "asc" }
                });
                $("#PartidoId").data("kendoDropDownList").setDataSource(dataSource);
            } else {
                $("#PartidoId").data("kendoDropDownList").setDataSource([]);
            }
        }
    });

    $("#PartidoId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        dataSource: [],
        optionLabel: "Selecione un partido",
        filter: "contains",
    });
}

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}

function SincronizarResearch() {
    BlockUi('Sincronizando...');
    $.ajax({
        url: '/TareasProgramadas/SincronizarResearch',
        type: 'POST',
        success: function (response) {
            if (response == "ok") {
                window.location = '/ResearchReporte/Index';
                $.unblockUI();
            } else {
                $.unblockUI();
                MensErr("Ocurrió un error al ejecutar la sincronización de los registros.");
            }
        }
    });
}