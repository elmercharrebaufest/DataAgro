$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarDate();

    $("#gridLog").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Log/ListaLogGrilla",
                    data: { fechaString: getSelectedDate() },
                    dataType: "json",
                    type: "GET"
                }
            },
            schema: {
                model: {
                    fields: {
                        Fecha: { type: "date" },
                        Xml: { type: "string" }
                    }
                }
            },
        },
        sortable: true,
        pageable: false,
        toolbar: [{ template: kendo.template($("#templateLog").html()) }],
        columns: [
            { field: "Fecha", title: "Fecha", format: "{0:dd/MM/yyyy HH:mm}", width: "135px" },
            {
                field: "Xml", title: "XML", width: "auto", template: function (dataItem) {
                    return '<span title="' + kendo.htmlEncode(dataItem.Xml) + '">' + kendo.htmlEncode(dataItem.Xml) + '</span>';
                }
            },
            {
                title: "Copiar XML",
                template: '<button class="k-button" onclick="copyXml(this)">Copiar</button>', width: "100px"
            },
        ],
        rowHeight: 50,
    });
});

function getSelectedDate() {
    return $("#fecha").val();
}
function Buscar() {
    $("#gridLog").data("kendoGrid").dataSource.read({ fechaString: getSelectedDate() });
}
function copyXml(button) {
    var dataItem = $("#gridLog").data("kendoGrid").dataItem($(button).closest("tr"));
    var xml = dataItem.Xml;

    // Create a temporary textarea element to copy the XML to the clipboard
    var textarea = document.createElement("textarea");
    textarea.value = xml;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand("copy");

    // Remove the temporary textarea element
    document.body.removeChild(textarea);
}
function copia_portapapeles(data) {
    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', data);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', data);
        }
    };
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
}

function Refrescar() {
    $("#buscar").click();
}

function InicializarDate() {
    kendo.culture("es-AR");
    var date = ObtenerFecha();
    $("#fecha").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });
    fechaString = $("#fecha").val();
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