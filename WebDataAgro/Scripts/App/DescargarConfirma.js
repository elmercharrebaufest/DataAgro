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
