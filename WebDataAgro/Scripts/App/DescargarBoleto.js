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
        //toolbar: [{ template: kendo.template($("#template").html()) }],
        pageable: false,
        columns: [
            { selectable: true, width: "35px" },
            {
                field: "Name", title: "Nombre", template: function (dataItem) {
                    return "<label  style=' color: black'> <strong>" + dataItem.Nombre + "</strong></label>"
                }
            },
            {
                field: "LastWriteTime", title: "Fecha de Modificación", template: function (dataItem) {
                    let fecha = moment(dataItem.FechaUltimaEscritura, 'YYYY/MM/DD HH:mm').format("DD/MM/YYYY HH:mm");
                    return "<label  style=' color: black'> <strong>" + fecha + "</strong></label>"
                }
            },
            {
                field: "Download", title: "Descargar", template: function (dataItem) {
                    return '<a onclick="DescargarPDF(\'' + dataItem.Nombre + '\')"> Descargar </a>'
                    //return '<a href="Url.Action("DescargarArchivoBoleto","Boleto", new { log = \''+ dataItem.Nombre +'\' })">Descargar</a>'
                }
            }
        ],
    }).data("kendoGrid");

    IniciarListaBoletos();

    $("#filtrogrilla").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#cerrarBoletosReenvio").click(function () {
        $(".modal").modal('hide');
    });
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


function ModalReenviarBoletos() {
    $("#reenviarBoletos").show();
    $("#cancelarBoletosReenvio").show();
    $("#cerrarBoletosReenvio").hide();

    $("#boletoEmailReenviado-modal").html('');

    var boletosSeleccionados = SeleccionarElementos();
    var numeroContratos = [];
    if (boletosSeleccionados.length > 0) {
        for (var i in boletosSeleccionados) {
            var loader = '<div class="col-xs-1"><div id="estado' + i + '" class="loader" hidden></div></div><div id="error' + i + '" class="col-xs-8"> </div>';

            numeroContratos.push(boletosSeleccionados[i].Nombre.split("_")[0])
            $("#boletoEmailReenviado-modal").append('<div class="row"><div class="col-xs-6">Contrato SAP: ' + boletosSeleccionados[i].Nombre.split("_")[0] + '</div>' + loader + '</div>');
        }

    } else {
        $("#boletoEmailReenviado-modal").append('<div style="text-align:center">Se deben seleccionar boletos para reenviar.</div>');
        $("#reenviarBoletos").hide();

    }
    $("#ModalReenviarBoletos").modal('show');
}

function ReenviarBoletosMails() {
    BlockUi('Enviando...');
    setTimeout(function () {
        $("#reenviarBoletos").hide();
        $("#cancelarBoletosReenvio").hide();
        $("#cerrarBoletosReenvio").show();

        var boletosSeleccionados = SeleccionarElementos();
        var data = {};
        var contratosBoletos = [];
        var nombresArchivos = [];
        for (var i in boletosSeleccionados) {
            contratosBoletos.push(boletosSeleccionados[i].Nombre.split("_")[0])
            nombresArchivos.push(boletosSeleccionados[i].Nombre)
        }
        data.contratosBoletos = contratosBoletos;
        data.nombresArchivos = nombresArchivos;
        $(".loader").show();
        result = MSExecuteOnServer('/Boleto/ReenviarEmailBoletos', data);
        $.unblockUI();
        if (result == "Ok") {
            $('#alert-msj').addClass('alert-success');
            $('#alert-msj').removeClass('display-none');
            $("#alert-msj").append('<div style="text-align:center">Boleto(s) reenviado(s).</div>');

        }
    }, 200);
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
