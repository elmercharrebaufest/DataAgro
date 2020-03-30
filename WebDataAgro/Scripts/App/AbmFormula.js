
var viewModel;
var data = new Array();



$(document).ready(function () {


    $('[data-toggle="popover"]').popover();
    traerDatosIniciales();
    CrearViewModel();
    crearArbol();
    
    crearPopupAgregarHijo();
    iniciarCamposInicioCantDias();


});

function traerDatosIniciales() {

    var funcionRetornada = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);

        }
        else {
            viewModel.set("DatosDeInicio", data.Datos);


            var iniciotextbox = $("#inicio").data("kendoNumericTextBox");
            iniciotextbox.value(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.Inicio);

            var cantdiastextbox = $("#cantdias").data("kendoNumericTextBox");
            cantdiastextbox.value(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.CantDias);
        }
    };

    MSExecuteURLOnServerAsync('/Formula/Inicializar', funcionRetornada, '');
}

function CrearViewModel() {

    viewModel = kendo.observable({

        DatosDeInicio: new Object()
    });
}

function crearArbol() {

    $("#treelist").kendoTreeList({

        dataSource: {
            requestEnd: function (e) {
                var type = e.type;
                if (type != "read") {
                    $('#treelist').data('kendoTreeList').dataSource.read();
                }
            },

            transport: {
                read: {
                    url: MSGetUrl('/Formula/Buscar'),
                    cache: false
                },
                update: {
                    url: MSGetUrl('/Formula/update'),
                },
                destroy: {
                    url: MSGetUrl('/Formula/eliminar'),
                },
                create: {
                    url: MSGetUrl('/Formula/update'),
                },
            },

            schema: {
                model: {
                    id: "Id",
                    parentId: "PadreId",
                    fields: {
                        PadreId: { field: "PadreId", type: "number", nullable: true, editable: true },
                        Id: { field: "Id", type: "number" },
                        DisplayName: { type: "string", editable: false, nullable: false }

                    },
                    expanded: true
                }
            },
        },



        dataBound: function (e) {

            var datos = e.sender.dataSource.data();

            for (var i = 0; i < datos.length; i++) {

                var dataItem = datos[i];
                var idCriterioRaiz = $("#treelist").data("kendoTreeList").dataSource.data().filter(function (x) { return x.Descripcion == 'CriterioRaiz' })[0].Id;

                if (dataItem.Concreta) {
                    $("#treelist").find("[data-uid='" + dataItem.uid + "']").find(".k-grid-agregarhijo").hide();
                }


                if (dataItem.PadreId != idCriterioRaiz && dataItem.PadreId !=null ) {
                    
                    var padreDeEsteItem = $("#treelist").data("kendoTreeList").dataSource.data().filter(function (x) { return x.Id == dataItem.PadreId })[0];
                    $("#treelist").find("[data-uid='" + padreDeEsteItem.uid + "']").find(".k-grid-delete").hide();
                }

                if (dataItem.Descripcion == "CriterioRaiz") {
                    $("#treelist").find("[data-uid='" + dataItem.uid + "']").find(".k-grid-edit").hide();
                    $("#treelist").find("[data-uid='" + dataItem.uid + "']").find(".k-grid-delete").hide();
                }
            }
        },

        edit: function (e) {
            //e.sender.dataSource.options.transport.update.data = datosDias();
        },
        remove: function (e) {
            //console.log(e);
            //e.sender.dataSource.options.transport.destroy.data = datosDias();
            location.reload();

        },

        save: function (e) {
            validarPrioridadIngresadaEnEditar(e);
        },


        autoSync:false,
        editable: {
            mode: "popup",
            window: {
                title: "Editar Criterio",
            }
        },



        columns: [

            { field: "DisplayName", title: "Descripción", width: 280 },
            { field: "Prioridad", title: "Prioridad (Puntos)", width: 280 },
            {
                command: [

                    { name: "edit", iconClass: "k-icon k-i-copy"},
                    { name: "destroy" },
                    { name: "qw", text: "Agregar Criterio", click: abrirVentanaAgregarHijo, className: "k-grid-agregarhijo" }], title: " ", width: "480px"
            }
        ],


        messages: {
            commands: {
                edit: "Editar",
                update: "Guardar",
                canceledit: "Cancelar",
                destroy: "Eliminar",

            }
        }
    });

}

function crearPopupAgregarHijo() {

    wnd = $("#popUpAgregarCriterio")
        .kendoWindow({
            title: "Agregar Criterio",
            modal: true,
            visible: false,
            resizable: false,
            width: 400,

        }).data("kendoWindow");



    $("#prioridad").kendoNumericTextBox({
        format: "0",
        decimals: 0,
        min: 0,
    });
}

function iniciarCamposInicioCantDias() {

    $("#inicio").kendoNumericTextBox({
        format: "0",
        decimals: 0,
        min: 0,
        max: 30,
        change: function () {
            actualizarDias();
        }
    });

    $("#cantdias").kendoNumericTextBox({
        format: "0",
        decimals: 0,
        min: 0,
        max: 30,
        change: function () {
            actualizarDias();
        }
    });
}

function iniciarDatosComboAgregarCriterios() {

    data = viewModel.DatosDeInicio.Criterios.Criterios;
    cargarCombo();
}

function actualizarDias() {
    var cambioDeLosDias = datosDias();
    var result = MSExecuteOnServer('/Formula/ActualizarDiasFormula', cambioDeLosDias);
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
    }
}

function agregar(e) {

    var id = $("#padreid").val();
    var maximoParaEsteCriterio = parseInt($("#maximo").val());


    var prioridad = parseInt($("#prioridad").val());
    //var prioridadMaxima = $("#prioridad").data("kendoNumericTextBox").max();

    var dropdownlist = $("#dropdown").data("kendoDropDownList");
    var dataItem = dropdownlist.dataItem();

    var result = null;


    var datos = {
        "Descripcion": dataItem.Descripcion,
        "PadreId": id,
        "Prioridad": prioridad
    };




    if (maximoParaEsteCriterio < prioridad) {

        if (maximoParaEsteCriterio == 0) {
            PopUpError("No se pueden Agregar mas Criterios");
        } else {
            PopUpError("La Prioridad debe ser Menor o Igual a " + maximoParaEsteCriterio);
        }
        $("#popUpAgregarCriterio").data("kendoWindow").close();
    } else {
        result = MSExecuteOnServer('/Formula/update', datos);
    }


    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        if (!ExistsErrorMessages(result.Errores)) {
            location.reload();
        }
    }


    $("#treelist").data("kendoTreeList").dataSource.read();
    $("#popUpAgregarCriterio").data("kendoWindow").close();
}

function datosDias() {

    var inicioFormula = $("#inicio").val();
    var cantdiasFormula = $("#cantdias").val();

    formulaDias = {
        "Inicio": inicioFormula,
        "CantDias": cantdiasFormula
    };


    return formulaDias;
}

function abrirVentanaAgregarHijo(e) {

    iniciarDatosComboAgregarCriterios();

    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $("#padreid").val(dataItem.Id);

    var numerictextbox = $("#prioridad").data("kendoNumericTextBox");

    var hijosDeEsteCriterio = $("#treelist").data("kendoTreeList").dataSource.data().filter(function (criterio) { return criterio.PadreId == dataItem.Id });

    let sumaPrioridadHijos = 0;
    hijosDeEsteCriterio.forEach(function (x) { return sumaPrioridadHijos = sumaPrioridadHijos + x.Prioridad });

    //$("#maximo").val(dataItem.Prioridad - sumaPrioridadHijos);
    $("#maximo").val(100 - sumaPrioridadHijos);

    wnd.center().open();
}

function validarPrioridadIngresadaEnEditar(e) {


    let prioridadIngresada = parseInt(e.model.Prioridad);
    let criteriosHermanos = $("#treelist").data("kendoTreeList").dataSource.data().filter(function (x) { return x.PadreId == e.model.PadreId && x.Id != e.model.Id });
    let criterioPadre = $("#treelist").data("kendoTreeList").dataSource.data().filter(function (x) { return x.Id == e.model.PadreId })[0];

    let sumaPrioridadHermanos = 0;
    criteriosHermanos.forEach(function (x) { return sumaPrioridadHermanos = sumaPrioridadHermanos + x.Prioridad });

    //let PrioridadMaximaDisponible = criterioPadre.Prioridad - sumaPrioridadHermanos;
    let PrioridadMaximaDisponible = 100 - sumaPrioridadHermanos;


    if (PrioridadMaximaDisponible - prioridadIngresada < 0) {

        PopUpError("La Prioridad debe ser Menor o Igual a " + PrioridadMaximaDisponible);


        //var treeList = $("#treelist").data("kendoTreeList");
        //treeList.cancelChanges();
    }

}

function cargarCombo() {

    var criteriosParaAgregar = data;

    var criteriosAgregados = $("#treelist").data("kendoTreeList").dataSource.data();

    criteriosAgregados.forEach(function(criAgregado) {

      return  criteriosParaAgregar = criteriosParaAgregar.filter(function (criterio) { return criterio.Descripcion != criAgregado.Descripcion });
    });


    $("#dropdown").kendoDropDownList({
        dataTextField: "DisplayName",
        dataValueField: "Descripcion",
        dataSource: { data: criteriosParaAgregar },
        optionLabel: "Seleccione uno..."
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

$("#agregarCriterio").kendoButton();
