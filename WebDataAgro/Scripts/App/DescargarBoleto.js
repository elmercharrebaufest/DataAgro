$(document).ready(function () {
    $("#grid").kendoGrid({
        dataSource: {
            data: [],
        },
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            var view = grid.dataSource.view();
            grid.thead.find("[data-title='Kgs Aplicados']").html("Kgs Aplicados");
            //grid.tbody.find("tr[data-uid='" + view[i].uid + "'] td:eq(0)");
            //grid.tbody.find("tr").dblclick(function (e) {
            //    var data = grid.dataItem(this);

            //});
            grid.tbody.find("tr").dblclick(function (e) {
                var data = grid.dataItem(this);
                $("#Name").val(data.Name);
                $("#Name").data("kendoAutoComplete").trigger("change");
                
            });


        },
        height: 400,
        //resizable: true,
        toolbar: [{ template: kendo.template($("#template").html()) }],
        pageable: false,
        columns: [
            {

                field: "Name", title: "Nombre", template: function (dataItem) {
                    return "<label  style=' color: black'> <strong>" + dataItem.Nombre + "</strong></label>"
                }
            },
            {
                field: "LastWriteTime", title: "Fecha Modificación", template: function (dataItem) {
                    let fecha = moment(dataItem.FechaUltimaEscritura, 'YYYY/MM/DD HH:mm').format("DD/MM/YYYY HH:mm");
                    return "<label  style=' color: black'> <strong>" + fecha + "</strong></label>"
                }
            },
            {
                field: "Download", title: "Descargar", template: function (dataItem) {
                    return '<a onclick="DescargarPDF(\'' + dataItem.Nombre +'\')"> Descargar </a>'
                    //return '<a href="Url.Action("DescargarArchivoBoleto","Boleto", new { log = \''+ dataItem.Nombre +'\' })">Descargar</a>'
                }
            }
        ],
    }).data("kendoGrid");

    IniciarListaBoletos();
});
function IniciarListaBoletos() {
    BlockUi('Consultando...');
    setTimeout(function () { ArmarGrillaBoletosDescargados(); }, 1000);

    setTimeout(function () { $.unblockUI() }, 1000);
}
function ArmarGrillaBoletosDescargados() {


    var grid = $("#grid").data("kendoGrid");
    
    var boletos = MSExecuteOnServer("/Boleto/ListarBoletos", {});
    //consultarBonificacionAfijar(contratos);

    var data = new kendo.data.DataSource({
        data: boletos
    });
    grid.setDataSource(data);
    setTimeout(function () {
        grid.setOptions({
            height: 400
        })
    }, 200);

}

function DescargarPDF(nombreArchivo) {
    var funcReturn = function (data) {
        if (data != null) {
            if (data.length > 0) {
                var url = MSGetUrl('/Boleto/DescargarArchivoBoleto?nombre=' + nombreArchivo);
                window.location = url;
            }
        }
    }
    MSExecuteOnServerAsync('/Boleto/ObtenerDownloadKey', null, funcReturn, true);
}
