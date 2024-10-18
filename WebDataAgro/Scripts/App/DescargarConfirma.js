var viewModel;
var datosIniCrearContrato;
var externo;
var enviarEmail;
var isFiltered = false;

$(document).ready(function () {
    isFiltered = false;

    $("#grid").kendoGrid({
        dataSource: {
            data: [],
        },
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");

            // Limpiar selecciones anteriores cuando el filtro está activo
            if (isFiltered) {
                grid.clearSelection();
                var view = grid.dataSource.view();
                // Seleccionar solo las filas visibles después del filtrado
                grid.tbody.find("tr:visible").each(function () {
                    var dataItem = grid.dataItem(this);
                    if (dataItem) {
                        $(this).addClass("k-state-selected");
                    }
                });
            }

            grid.tbody.find("tr").dblclick(function (e) {
                var data = grid.dataItem(this);
                $("#Name").val(data.Name);
                $("#Name").data("kendoAutoComplete").trigger("change");
            });
        },
        height: 400,
        pageable: false,
        selectable: "multiple row",
        columns: [
            { selectable: true, width: "35px" },
            {
                field: "Name", title: "Nombre", template: function (dataItem) {
                    return "<label  style=' color: black'> <strong>" + dataItem.Nombre + "</strong></label>"
                }
            },
            {
                field: "LastWriteTime", title: "Fecha de Generación", template: function (dataItem) {
                    return "<label  style=' color: black'> <strong>" + dataItem.FechaGeneracion + "</strong></label>"
                }
            },
            {
                field: "Download", title: "Descargar", template: function (dataItem) {
                    return '<a onclick="DesacargaConfirma(\'' + dataItem.Nombre + '\')"> Descargar </a>'
                }
            }
        ],
    }).data("kendoGrid");

    IniciarListaConfirmas();

    $("#filtrogrilla").on("keyup", function () {
        var grid = $("#grid").data("kendoGrid");
        isFiltered = true;
        grid.clearSelection();
        var value = $(this).val().toLowerCase();
        $("table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // Sobrescribimos el evento de "Select All" para manejar solo lo visible
    $(".k-header .k-checkbox").on("change", function () {
        var grid = $("#grid").data("kendoGrid");
        var isChecked = $(this).is(":checked");

        if (isFiltered) {
            grid.clearSelection();

            // Solo seleccionamos las filas visibles
            grid.tbody.find("tr:visible").each(function () {
                if (isChecked) {
                    $(this).addClass("k-state-selected");
                } else {
                    $(this).removeClass("k-state-selected");
                }
            });
        } else {
            // Si no está filtrado, el comportamiento del "Select All" es estándar
            grid.tbody.find("tr").each(function () {
                if (isChecked) {
                    $(this).addClass("k-state-selected");
                } else {
                    $(this).removeClass("k-state-selected");
                }
            });
        }
    });
});

function IniciarListaConfirmas() {
    BlockUi('Consultando...');
    setTimeout(function () { ArmarGrillaConfirmasDesacargas(); }, 1000);
    setTimeout(function () { $.unblockUI() }, 1000);
}

function ArmarGrillaConfirmasDesacargas() {

    var grid = $("#grid").data("kendoGrid");

    var confirmas = MSExecuteOnServer("/Confirma/ListarConfirmas");

    var data = new kendo.data.DataSource({
        data: confirmas
    });
    grid.setDataSource(data);
    setTimeout(function () {
        grid.setOptions({
            height: 400
        })
    }, 200);

}

function DesacargaConfirma(nombreArchivo) {
    var funcReturn = function (data) {
        if (data != null) {
            if (data.length > 0) {
                var url = MSGetUrl('/Confirma/DescargarArchivoConfirma?nombreArchivo=' + nombreArchivo);
                window.location = url;
            }
        }
    }
    MSExecuteOnServerAsync('/Confirma/ObtenerDownloadKey', null, funcReturn, true);
}

function DescargarZipConfirmas() {
    var boletosSeleccionados = SeleccionarElementos();
    var nombresArchivos = [];
    var data = {};

    if (boletosSeleccionados.length > 0) {
        for (var i in boletosSeleccionados) {
            if (boletosSeleccionados[i].Nombre != null)
                nombresArchivos.push(boletosSeleccionados[i].Nombre);
        }

        data.nombresArchivos = nombresArchivos;

        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Confirma/DescargarZipConfirmas', true);
        xhr.responseType = 'blob';  // Para manejar la respuesta como un archivo binario

        // Definir qué sucede cuando la respuesta está lista
        xhr.onload = function () {
            if (xhr.status === 200) {
                // Crear un Blob a partir de la respuesta
                var blob = xhr.response;

                var filename = "confirmas.zip";

                // Crear un enlace para descargar el archivo
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            } else {
                alert('Ocurrió un error al descargar el archivo ZIP.');
            }
        };

        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.send(JSON.stringify(data));
    }
}

function SeleccionarElementos() {
    var grid = $("#grid").data("kendoGrid");
    var obj = [];

    if (isFiltered) {
        var visibleRows = grid.tbody.find("tr:visible");
        visibleRows.each(function () {
            if ($(this).hasClass("k-state-selected")) {
                var dataItem = grid.dataItem(this);
                obj.push(dataItem);
            }
        });
    } else {
        var selectedRows = grid.select();
        selectedRows.each(function (index, row) {
            var selectedItem = grid.dataItem(row);
            obj.push(selectedItem);
        });
    }

    return obj;
}