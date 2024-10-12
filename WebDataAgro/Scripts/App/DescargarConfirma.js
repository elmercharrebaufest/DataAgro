var viewModel;
var datosIniCrearContrato;
var externo;
var enviarEmail;

$(document).ready(function () {
    $("#grid").kendoGrid({
        dataSource: {
            data: [],
        },
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            var view = grid.dataSource.view();
            //grid.thead.find("[data-title='Kgs Aplicados']").html("Kgs Aplicados");

            grid.tbody.find("tr").dblclick(function (e) {
                var data = grid.dataItem(this);
                $("#Name").val(data.Name);
                $("#Name").data("kendoAutoComplete").trigger("change");
            });
        },
        height: 400,
        pageable: false,
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
        var value = $(this).val().toLowerCase();
        $("table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
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

                // Establecer el nombre del archivo a descargar
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

        // Enviar los datos en formato JSON
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.send(JSON.stringify(data));
    }
}

function SeleccionarElementos() {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        obj.push(selectedItem);
    });
    return obj;
}