//Filtros para enviar como Kendo.DataSourceRequest


function TraerFiltrosConValores() {

    let listaFiltros = [];

    filtrosBusqSelectMultiple(listaFiltros);
    filtrosBusqFecha(listaFiltros);
    filtrosBusqTexto(listaFiltros);
    filtrosBusqIdUnico(listaFiltros);
    filtrosBusqBooleano(listaFiltros);
    filtroTextToArrayCsv(listaFiltros);
    filtroAgregarValorContratoCorredorSap(listaFiltros);
    filtroAgregarValorPopUp(listaFiltros);

    let filtroPrincipal = (listaFiltros.length == 0) ? null : new FiltroPadre("and", listaFiltros);
    let filtroCompleto = new FiltroCompleto(20, 0, filtroPrincipal);

    return filtroCompleto;
}

function filtroAgregarValorPopUp(listaFiltros) {
    let filtrosParaContratoSAP;
    //solo se esta pudiendo agregar 1 filtro de estos por pantalla
    if ($(".filtroAgregarValorPopUp")[0] != undefined) {
        filtrosParaContratoSAP = filtroPopUpAddValor($(".filtroAgregarValorPopUp")[0].name);
    }

    if (filtrosParaContratoSAP != null) {
        for (var i = 0; i < filtrosParaContratoSAP.filters.length; i++) {
            filtrosParaContratoSAP.filters[i].value = filtrosParaContratoSAP.filters[i].valuepadStart(10, '0');  
        }
        listaFiltros.push(filtrosParaContratoSAP);
    }
}

function filtroAgregarValorContratoCorredorSap(listaFiltros) {
    let filtrosParaContratoSAP;
    //solo se esta pudiendo agregar 1 filtro de estos por pantalla
    if ($(".filtroAgregarContrato")[0] != undefined) {
        filtrosParaContratoSAP = filtroPopUpAddValor($(".filtroAgregarContrato")[0].name);

    }

    if (filtrosParaContratoSAP != null) {
        filtrosParaContratoSAP.filters[0].value = filtrosParaContratoSAP.filters[0].value;
        listaFiltros.push(filtrosParaContratoSAP);
    }
}


function filtroTextToArrayCsv(listaDeFiltros) {

    let listaTodosLosValores = [];

    let todosTextosSinSeparar = $(".filtroTextToArrayCsv");

    todosTextosSinSeparar.each(function (e) {

        let arrayDeValores = $("#" + todosTextosSinSeparar[e].id).val().split(";");


        arrayDeValores.forEach(function (valorBuscado) {
            (valorBuscado != "" && valorBuscado != null) ? listaTodosLosValores.push(new FiltroHijo(todosTextosSinSeparar[e].name, valorBuscado, "eq")) : null;
        });


        if (listaTodosLosValores.length > 0) {

            listaDeFiltros.push(new FiltroPadre("or", listaTodosLosValores));

        }


    });
}

function filtrosBusqIdUnico(listaDeFiltros) {

    let filtroBusquedaPorId = $(".filtroBusquedaPorId");
    filtroBusquedaPorId.each(function (e) {
        if (filtroBusquedaPorId[e].id != "") {
            ($("#" + filtroBusquedaPorId[e].id).val() == "") ? null : listaDeFiltros.push(new FiltroHijo(filtroBusquedaPorId[e].name, JSON.parse($("#" + filtroBusquedaPorId[e].id).val()), "eq"));
        }
    });
}



function filtrosBusqTexto(listaDeFiltros) {

    let filtroBusquedaTextBox = $(".filtroBusquedaTextBox");
    filtroBusquedaTextBox.each(function (e) {
        if (filtroBusquedaTextBox[e].id != "") {
            ($("#" + filtroBusquedaTextBox[e].id).val() == "") ? null : listaDeFiltros.push(new FiltroHijo(filtroBusquedaTextBox[e].name, $("#" + filtroBusquedaTextBox[e].id).val(), "eq"));
        }
    });
}

function filtrosBusqBooleano(listaDeFiltros) {

    let filtrosBoleanos = $(".filtroBoleano");

    filtrosBoleanos.each(function (e) {

        if (filtrosBoleanos[e].id != "") {

            ($('[name=' + filtrosBoleanos[e].name + ']:checked').val() == null) ? null : listaDeFiltros.push(new FiltroHijo(filtrosBoleanos[e].name, JSON.parse($('[name=' + filtrosBoleanos[e].name + ']:checked').val().toLowerCase()), "eq"));
        }
    });
}


function filtrosBusqFecha(listaDeFiltros) {

    let filtroFecha = $(".filtroFecha");
    filtroFecha.each(function (e) {
        if (filtroFecha[e].id != "") {

            let configs = filtroFecha[e].name.split("-");

            let nombreDelFiltro = configs[0];
            let operadorDeComparacion = "gte";

            if (configs.length > 1) {

                switch (configs[1]) {
                    case "desde": operadorDeComparacion = "gte"; break;
                    case "hasta": operadorDeComparacion = "lte"; break;
                    case "igual": operadorDeComparacion = "eq"; break;
                }
            }
            ($("#" + filtroFecha[e].id).val() == "") ? null : listaDeFiltros.push(new FiltroHijo(nombreDelFiltro, crearDate($("#" + filtroFecha[e].id).val()), operadorDeComparacion));
        }
    });

}

function filtrosBusqSelectMultiple(listaDeFiltros) {

    //Solo Para Multiselect de Kendo
    let filtroMultiselect = $(".multiselect input");
    filtroMultiselect.each(function (e) {

        let listaSeleccionados = [];
        if (filtroMultiselect[e].id == "" || filtroMultiselect[e].id == null) {
            return;
        }
        ($("#" + filtroMultiselect[e].id).data("kendoMultiSelect").value().length == 1) ? null : listaSeleccionados = $("#" + filtroMultiselect[e].id).data("kendoMultiSelect").value().filter(function (x) { if (x == "") { return; } else { return x } });

        let numberArray = $("#" + filtroMultiselect[e].id).data("kendoMultiSelect").value().map(Number)
        if (numberArray.length > 0) {

            let filtrosPorCadaValorSeleccionado = [];
            numberArray.forEach(function (x) {
                if (x > 0) {
                    filtrosPorCadaValorSeleccionado.push(new FiltroHijo(filtroMultiselect[e].name, JSON.parse(x), "eq"));
                }
            });
            if (filtrosPorCadaValorSeleccionado.length>0) {
                let FiltroDeMultiselect = new FiltroPadre("or", filtrosPorCadaValorSeleccionado);
                listaDeFiltros.push(FiltroDeMultiselect);
            }
            
        }
    });
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

function deFechaAString(fecha) {

    let arrfecha = JSON.stringify(fecha).slice(1, 11).split("-");
    return arrfecha[2] + "/" + arrfecha[1] + "/" + arrfecha[0];
}

function FiltroCompleto(Take, Skip, Filter) {
    this.take = Take;
    this.skip = Skip;
    this.filter = Filter;
}

function FiltroPadre(Logic, Filters) {
    this.field = null;
    this.value = {};
    this.logic = Logic;
    this.operator = null;
    this.filters = Filters;
}

function FiltroHijo(Field, Value, Operator) {

    this.field = Field;
    this.value = Value;
    this.logic = null;
    this.operator = Operator;
    this.filters = {};

}

function Ordenar(dir, field) {
    this.dir = dir;
    this.field = field;
}

function crearDate(fechaFormatoKendo) {

    let expr = /\d+/g;
    let fecha = fechaFormatoKendo.match(expr);

    return new Date(fecha[2], parseInt(fecha[1]) - 1, fecha[0]);
}

function inicializarTodosKendoDate(filtrosConFechas) {

    filtrosConFechas.each(function (e) {

        if (filtrosConFechas[e].id != "") {
            $("#" + filtrosConFechas[e].id).kendoDatePicker({
                format: "dd-MM-yyyy",
                parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
            });

            $("#" + filtrosConFechas[e].id).click(function () {
                $("#" + filtrosConFechas[e].id).val("");
            });
        }
    });
}

function CrearMultiSelectFiltro(element, text, value, url) {

    $(element).kendoMultiSelect({
        itemTemplate: " #:data." + text + " #",
        dataBound: function () {
            setTimeout(function () {
            }, 500);
        },
        open: function (e) {
            $(element).data("kendoMultiSelect").value('');
        },
        dataTextField: text,
        dataValueField: value,
        autoClose: false,
        autoBind: false,
        dataSource: {
            serverFiltering: true,
            filter: [],
            transport: {
                read: {
                    url: url,
                    data: function () {
                        return {
                            text: $(element).data("kendoMultiSelect").input.val()
                        };
                    },
                    prefix: ""
                }
            }
        }
    });
}

function PopUpError(mensaje) {
    ShowTooltipMessages("err",

        [{
            Item: 0,
            ErrorCode: 0,
            LogId: 0,
            Message: mensaje,
            Source: "",
            LargeDescription: "",
            Translate: false,
            Format: ""
        }]
    );
}

//===============PopUpSeleccionAgregar================

function filtroPopUpAddValor(nombreDelFiltro) {

    let filtrosContratosSap = [];
    $("#contratos-table td").each(function (e) {

        let valorBuscado = $("#contratos-table td")[e].innerText;

        (valorBuscado != "" &&
            $(valorBuscado != null)) ? filtrosContratosSap.push(new FiltroHijo(nombreDelFiltro, valorBuscado, "eq")) : null;
    });
    return (filtrosContratosSap.length > 0) ? new FiltroPadre("or", filtrosContratosSap) : null;
}


//Agregarlo en document.ready de kendo si se usa
function inicializarPopUpSap(nombrePopUp) {

    crearPopUp(nombrePopUp);

    ModalContrato();


    $("#ContratoSAPId").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#ContratoSAPId").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));

        CambioVariosContratos();
    });



    $("#ContratoSAPId").change(CambioVariosContratos);
    $("#ContratoSAPHastaId").change(CambioVariosContratos);

    $(document).on("click", ".agregarContrato", function () {
        var num = $("#nuevoNumContrato").val();
        //if ($.isNumeric(num)) {
        if (num.trim() != "" && num.trim() != null) {
            $("#contratos-table").append('<tr><td>' + num + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" style="height: 34px;float: right" type="button"></button></td></tr></td></tr>');
            $(".nuevoNumContrato").val('');
            $(".nuevoNumContrato").focus();
            GenerarContratoSAPDesde();
        }

        //}
    });

    $(document).on("click", ".borrarContrato", function () {
        $(this).parent().parent().remove();
        GenerarContratoSAPDesde();
    });
}

function crearPopUp(nombrePopUp) {

    if ($("#popUpCargarValores").children().length == 0) {

        $("#popUpCargarValores").prepend('<div class="modal-dialog" id="1" role="document"></div>');
        $("#1").prepend('<div id="2" class="modal-content">');

        $("#2").prepend('<div id="4" class="modal-body"></div>');
        $("#2").prepend('<div id="3" class="modal-header"></div>');

        //tabla
        $("#4").prepend('<div id="7" class="table-responsive">');
        $("#7").prepend('<table id="contratos-table" class="table table-striped"></table>');

        //body
        $("#7").prepend('<div id="8" class="row">');
        $("#8").prepend('<div id="10" class="col-xs-2">');
        $("#8").prepend('<div id="9" class="col-xs-4"></div>');

        $("#10").prepend('<button class="k-button k-button-icontext fa fa-plus agregarContrato" style="height: 34px;" type="button"></button>');
        $("#9").prepend('<input type="text" id="nuevoNumContrato" class="nuevoNumContrato">');

        //header
        $("#5").prepend('<span aria-hidden="true">&times;</span>');
        $("#3").prepend('<div id="6" class="status confirmado-modal"></div>');
        $("#6").prepend('<div>Seleccione ' + nombrePopUp + '</div>');
        $("#3").prepend('<button id="5" type="button" class="cerrar close" data-dismiss="modal" aria-label="Close"></button>');
    }
}

function GenerarContratoSAPDesde() {
    var arr = "";
    $("#contratos-table tr").each(function () {
        if (arr != "") {
            arr += ";";
        }
        arr += $(this).find("td:first").text(); //put elements into array
    });
    $("#ContratoSAPId").val(arr);
    $("#ContratoSAPHastaId").attr('disabled', 'disabled');
    $("#ContratoSAPHastaId").val('');
}

function CambioVariosContratos() {

    if
        (parseInt($("#ContratoSAPHastaId").val()) - parseInt($("#ContratoSAPId").val()) > 1000) {

        PopUpError("Seleccione un rango de valores menor a 1000");

        return;
    }

    var lista = [];
    lista = $("#ContratoSAPId").val().split(';');
    if (lista.length > 1) {

        $("#ContratoSAPHastaId").attr('disabled', 'disabled');
        $("#ContratoSAPHastaId").val("")
        ArmarTabla(lista);
    } else if ($.isNumeric($("#ContratoSAPId").val()) && $.isNumeric($("#ContratoSAPHastaId").val())) {

        $("#ContratoSAPHastaId").removeAttr('disabled');

        lista = [];
        for (var i = parseInt($("#ContratoSAPId").val()); i <= parseInt($("#ContratoSAPHastaId").val()); i++) {
            lista.push(i);
        }
        ArmarTabla(lista);
    }
    else {
        $("#ContratoSAPHastaId").removeAttr('disabled');
        ArmarTabla(lista);
    }
}

function ModalContrato() {
    $("#abrirPopUpCargarValores").click(function () {
        $("#popUpCargarValores").modal('toggle');
    });
}

function ArmarTabla(contratos) {
    $("#contratos-table").empty();
    var tabla = '<tr><th>Seleccionados:</th></tr>';
    if (contratos) {
        for (var i = 0; i < contratos.length; i++) {
            tabla += '<tr><td>' + contratos[i] + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" style="height: 34px;float: right" type="button"></button></td></tr>';
        }
    }
    $("#contratos-table").append(tabla);
}


