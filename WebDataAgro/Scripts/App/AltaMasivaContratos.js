var dataFile;

$(document).ready(function () {
    $("#menuproveedor").hide();
    $("#contratoAcuerdoId").click(function () {
        $("#contratoAcuerdoId").data("kendoAutoComplete").value("");
        $("#contratoAcuerdoId").data("kendoAutoComplete").trigger("change");
    });
    $("#contratoAcuerdoId").kendoAutoComplete({
        template: function (data) {
            if ($(window).width() > 768) {
                return '<p class="buscar-nomb">' + data.Id + ' - ' + data.Material + ' - ' + data.RazonSocial + ' - ' + data.Fecha + ' - ' + data.Cantidad + ' Kg. </p>';
            } else {
                return '<p class="buscar-nomb letra650">' + data.Id + ' - ' + data.Material + ' - ' + data.RazonSocial + ' - ' + data.Fecha + ' - ' + data.Cantidad + ' Kg. </p>'
            }
        },
        minLength: 1,
        enforceMinLength: true,
        dataTextField: "Id",
        dataValueField: "Id",
        autoWidth: true,
        change: function () {

        },
        select: function (e) {
            //CargarCopiaContrato(e.dataItem.Id, "acuerdo");
            $("#contratoId").val(e.dataItem.Id);
            $("#divAcuerdoData").html('');
            $("#divAcuerdoData").append(e.dataItem.Filtro);
        },
        dataSource: {
            serverFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/ObtenerContratosAcuerdoAlDia"
                },
                parameterMap: function (data, type) {
                    var valor = $("#contratoAcuerdoId").val();
                    return { filtro: valor };
                }
            }
        },
        filtering: function (e) {
            $("#divAcuerdoData").html('');
            if (!e.filter.value) {
                e.preventDefault();
            } else {
                //$("#contratoId").val(e.dataItem.Id);
            }
        }
    });

    $("#tipoAlta").change(function () {
        var divContrato = document.getElementById("divContratoAcuerdo");
        var divDescargarPlantillaVacio = document.getElementById("divDescargarPlantillaVacio");
        var divDescargarPlantillaAcuerdo = document.getElementById("divDescargarPlantillaAcuerdo");
        var divDescargarPlantillaConvenio = document.getElementById("divDescargarPlantillaConvenio");
        var divDescargarPlantillaMATBA = document.getElementById("divDescargarPlantillaMATBA");
        if (this.value == "3") {
            divContrato.style.display = 'none';
            divDescargarPlantillaVacio.style.display = 'none';
            divDescargarPlantillaAcuerdo.style.display = 'none';
            divDescargarPlantillaConvenio.style.display = 'none';
            divDescargarPlantillaMATBA.style.display = 'block';
            divAcuerdoData.style.display = 'none'
            $('#divAcuerdoData').text("");
            $("#contratoAcuerdoId").data("kendoAutoComplete").value("");
        }
        if (this.value == "2") {
            divContrato.style.display = 'none';
            divDescargarPlantillaVacio.style.display = 'none';
            divDescargarPlantillaAcuerdo.style.display = 'none';
            divDescargarPlantillaMATBA.style.display = 'none';
            divDescargarPlantillaConvenio.style.display = 'block';         
            divAcuerdoData.style.display = 'none'
            $('#divAcuerdoData').text("");
            $("#contratoAcuerdoId").data("kendoAutoComplete").value("");
        } else if (this.value == "1") {
            divContrato.style.display = 'block';
            divDescargarPlantillaVacio.style.display = 'none';
            divDescargarPlantillaAcuerdo.style.display = 'block';
            divDescargarPlantillaMATBA.style.display = 'none';
            divDescargarPlantillaConvenio.style.display = 'none';
            $('#divAcuerdoData').show();
        } else if (this.value == "0") {
            divContrato.style.display = 'none';
            divDescargarPlantillaVacio.style.display = 'block';
            divDescargarPlantillaAcuerdo.style.display = 'none';
            divDescargarPlantillaConvenio.style.display = 'none';
            divDescargarPlantillaMATBA.style.display = 'none';
            divAcuerdoData.style.display = 'none'
            $('#divAcuerdoData').text("");
            $("#contratoAcuerdoId").data("kendoAutoComplete").value("");
        }
    });
});


function cargarContratos() {
    BlockUi('Cargando...');
    setTimeout(function () {
        if (ValidarCarga()) {
            if (window.FormData !== undefined) {
                var fileUpload = $("#file").get(0);
                var files = fileUpload.files;
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                $.ajax({
                    url: '/Compranet/AltaMasivaContratosExcel?tipoAlta=' + $("#tipoAlta").val() +'&contratoacuerdo=' + $("#contratoId").val(),
                    type: "POST",
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: data,
                    success: function (result) {
                        var tablaRespuesta = '<table style="border-radius:15px" id="tableRespuesta"><tr><th>Fila</th><th>Resultado</th></tr>';
                        if (result.Resultado) {
                            for (var i = 0; i < result.Resume.length; i++) {
                                var erroresHtml = '';
                                if (result.Resume[i].HasError) {
                                    for (var j = 0; j < result.Resume[i].Errors.length; j++) {
                                        erroresHtml += '<tr style="color:red"><td>' + String(result.Resume[i].Row + 2) + '</td><td>' + result.Resume[i].Errors[j] + '</td></tr>'
                                    }
                                } else {
                                    erroresHtml += '<tr><td>' + String(result.Resume[i].Row + 2) + '</td><td>Contrato creado correctamente.</td></tr>'
                                }
                                tablaRespuesta += erroresHtml;
                            }
                        }
                        else {
                            var erroresHtml = "";
                            for (var i = 0; i < result.Resume.length; i++) {

                                erroresHtml = '<tr style="color:red">';
                                //for (var j = 0; j < result.Resume[i].Errors.length; j++) {
                                erroresHtml += '<td>0</td><td>' + result.Resume[i] + '</td>'
                                //}
                                erroresHtml += '</tr>';
                            }
                            tablaRespuesta += erroresHtml;
                        }
                        tablaRespuesta += '</table>';
                        tablaRespuesta += '<div class="col-md-4" style="padding-left: 67px;"><input id="volverBtn" onclick="volverFormulario()" class="k-button k-button-icontext" value="Volver" /></div>'
                        $.unblockUI();
                        $('#divFormulario').hide();
                        $("#divRespuesta").html('');
                        $("#divRespuesta").html(tablaRespuesta);
                        $('#divRespuesta').show();
                    },
                    error: function (err) {
                        MensErr(err);
                        $.unblockUI();
                    }
                });
            }
        }
        else {
            $.unblockUI();
        }
    }, 1000);

}

function ValidarCarga() {
    if ($("#tipoAlta").val() == "0") {
        MensErr("Debe seleccionar un tipo de alta");
        return false;
    }
    if ($("#contratoId").val() == "" && $("#tipoAlta").val() == "1") {
        MensErr("Debe seleccionar el contrato acuerdo");
        return false;
    }
    return true;
}


function setFileName() {
    var fullPath = document.getElementById("file").value;
    if (fullPath) {
        var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
        var filename = fullPath.substring(startIndex);
        if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
            filename = filename.substring(1);
        }
        $("#fileLb").html('');
        $("#fileLb").append(filename);
    }
}

function volverFormulario() {

    $("#fileLb").html('');

    document.getElementById("file").value = '';
    $('#divRespuesta').hide();
    $('#divFormulario').show();
}