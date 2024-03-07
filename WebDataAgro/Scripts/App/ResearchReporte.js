var idModalBorrar = 0;
$(document).ready(function () {
    kendo.culture("es-AR");

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
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],

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
            { field: "Id", title: "#", type: "number", format: "{0:n0}", width: 40 },
            { field: "FechaAlta", type: "date", title: "Fecha Alta", format: _DefaultDateTemplate, width: 80 },
            { field: "Material", title: "Material", width: 80 },
            { field: "Comercial", title: "Comercial", width: 80 },
            { field: "Campaña", title: "Camp.", width: 60 },
            { field: "Provincia", title: "Provincia", width: 80 },
            { field: "Partido", title: "Partido", width: 80 },
            { field: "Localidad", title: "Localidad", width: 80 },
            { field: "Estadio", title: "Estadio", width: 80 },
            { field: "Condicion", title: "Condicion", width: 80 },
            { field: "Coeficiente", title: "Coeficiente", width: 80 },
            { field: "HumedadSuelo", title: "Humedad<br>Suelo", width: 80 },
            { field: "TipoMuestraUno", title: "Muestra<br>Uno", width: 80 },
            { field: "MedidasUno", title: "Medidas", width: 80 },
            { field: "PromedioMuestraUno", title: "Promedio", width: 70 },
            { field: "TipoMuestraDos", title: "Muestra<br>Dos", width: 80 },
            { field: "MedidasDos", title: "Medidas", width: 80 },
            { field: "PromedioMuestraDos", title: "Promedio", width: 70 },
            { field: "TipoMuestraTres", title: "Muestra<br>Tres", width: 80 },
            { field: "MedidasTres", title: "Medidas", width: 80 },
            { field: "PromedioMuestraTres", title: "Promedio", width: 70 },
            { field: "Rendimiento", title: "Rendimiento", width: 80 },
            //{ field: "RendimientoCalculado", title: "RendimientoCalculado", width: 80 },
            { field: "CapitulosGirasol", title: "Capitulos<br>Girasol", width: 70 },
            { field: "DistanciaHileras", title: "Distancia<br>Hileras", width: 70 },
            { field: "Latitud", title: "Latitud", width: 70 },
            { field: "Longitud", title: "Longitud", width: 70 },
            { field: "Comentarios", title: "Comentarios", width: 70 },
            { field: "MaterialAntecesor", title: "Material<br>Antecesor", width: 70 },
            { field: "TipoCarga", title: "Tipo<br>Carga", width: 70 },
            //{ field: "Attachments", title: "Attachments", type: "string", title: "Tiene Adjuntos", template: function (dataItem) { return dataItem.Attachments ? "Si" : "No"; }, width: 80 },
            { field: "Eliminado", type: "string", title: "Eliminado", template: function (dataItem) { return dataItem.Eliminado ? "Si" : "No"; }, width: 80 },
            { field: "Adjuntos", template: function (dataItem) { return listaAdjuntos(dataItem); }, title: "Adjuntos", width: 120 },
            { template: function (dataItem) { return botonBorrar(dataItem, 'fa-trash err'); }, width: 40, title: "Elim." },
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
            for (var i = 0; i < sheet.rows[0].cells.length; i++) {
                sheet.rows[0].cells[i].value = sheet.rows[0].cells[i].value.replace("<br>", " ").replace("<br>", " ").replace("<br>", " ");
            }

            var templateEliminado = kendo.template(this.columns[30].template);
            var templateAdjuntos = kendo.template(this.columns[31].template);

            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataItem = {
                    Eliminado: row.cells[30].value,
                    Adjuntos: row.cells[31].value,
                };

                var asdv = dataItem.value;
                var asd = templateAdjuntos(dataItem);
                row.cells[30].value = templateEliminado(dataItem);
                row.cells[31].value = dataItem.Adjuntos.length == 0 ? "No" : "Si";
            }
        },
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
        return '<button data-toggle="tooltip" title="Rechazar" ' +
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
    $(".multiselect").kendoMultiSelect({});

    $("#PartidoId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "Id",
        dataSource: [],
        optionLabel: "Selecione una",
        filter: "contains",
    });
}

function getProvinciaId() {
    var provinciaId = $("#ProvinciaId").val();
    console.log(provinciaId);
    provinciaId = 1;
    return {
        provinciaId: provinciaId
    }
};

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}

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

document.getElementById("ProvinciaId").addEventListener("change", function () {
    var provId = document.getElementById("ProvinciaId").value;
    //$("#PartidoId").data("kendoDropDownList").val("");
    if (provId >= 0) {
        var result = MSExecuteOnServer("/ResearchReporte/TraerPartidosPorProvincia", { provinciaId: provId });
        var dataSource = new kendo.data.DataSource({
            data: result,
            sort: { field: "Descripcion", dir: "asc" }
        });
        $("#PartidoId").data("kendoDropDownList").setDataSource(dataSource);
    } else {

    }
});