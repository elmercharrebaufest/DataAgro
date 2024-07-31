
var viewModel;
var data = new Array();

$(document).ready(function () {

    kendo.culture("es-AR");
    $('[data-toggle="popover"]').popover();
    traerDatosIniciales();
    CrearViewModel();
    crearArbol();

    crearPopupAgregarHijo();
    iniciarCampos();
    cargarTiposNegociosExcluidos();


});

function traerDatosIniciales(MaterialId) {

    var funcionRetornada = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);

        }
        else {
            viewModel.set("DatosDeInicio", data.Datos);


            var cuposDesde = $("#cuposDesde").data("kendoDatePicker");
            cuposDesde.value(new Date(parseInt(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.CuposDesde.substr(6))));

            var cuposHasta = $("#cuposHasta").data("kendoDatePicker");
            cuposHasta.value(new Date(parseInt(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.CuposHasta.substr(6))));

            var negociosDesde = $("#negociosDesde").data("kendoDatePicker");
            negociosDesde.value(new Date(parseInt(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.NegociosDesde.substr(6))));

            var negociosHasta = $("#negociosHasta").data("kendoDatePicker");
            negociosHasta.value(new Date(parseInt(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.NegociosHasta.substr(6))));


            $("#deshabilitar").prop("checked", viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.Cierre);

            $("#MaterialId").data("kendoDropDownList").dataSource.data(data.Datos.materiales);
            $("#MaterialId").data("kendoDropDownList").value(viewModel.DatosDeInicio.ultimaFormulaTraida.Formula.MaterialId);


        }
    };
    var url = '/Formula/Inicializar';
    if (MaterialId > 0) {
        url = url + "?MaterialId=" + MaterialId;
    }
    MSExecuteURLOnServerAsync(url, funcionRetornada, '');
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
                    cache: false,
                    data: function () {
                        $("#treelist").data("kendoTreeList").dataSource.data([]);
                        return {
                            MaterialId: $("#MaterialId").val()
                        };
                    }
                },
                update: {
                    url: MSGetUrl('/Formula/update'),
                    type: "POST"
                },
                destroy: {
                    url: MSGetUrl('/Formula/eliminar'),
                    type: "POST"
                },
                create: {
                    url: MSGetUrl('/Formula/update'),
                    type: "POST"
                }
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


                if (dataItem.PadreId != idCriterioRaiz && dataItem.PadreId != null) {

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
            //recargarPantalla();

        },

        save: function (e) {
            validarPrioridadIngresadaEnEditar(e);
            cargarTiposNegociosExcluidos();
        },


        autoSync: false,
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

                    { name: "edit", iconClass: "k-icon k-i-copy" },
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

function iniciarCampos() {

    $("#cuposDesde").kendoDatePicker({
        format: "dd-MM-yyyy",
        change: function () {
            actualizarDias();
            actualizarTiposNegociosExcluidos();
        }
    });

    $("#cuposHasta").kendoDatePicker({
        format: "dd-MM-yyyy",
        change: function () {
            actualizarDias();
            actualizarTiposNegociosExcluidos();
        }
    });

    $("#negociosDesde").kendoDatePicker({
        format: "dd-MM-yyyy",
        change: function () {
            actualizarDias();
            actualizarTiposNegociosExcluidos();
        }
    });

    $("#negociosHasta").kendoDatePicker({
        format: "dd-MM-yyyy",
        change: function () {
            actualizarDias();
            actualizarTiposNegociosExcluidos();
        }
    });
    $("#MaterialId").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "MaterialId",
        change: function () {
            recargarPantalla();
            cargarTiposNegociosExcluidos();
        }
    });

    $("#TipoNegocioId").kendoMultiSelect({
        autoClose: false,
        change: function () {
            actualizarTiposNegociosExcluidos();
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
        if (!ExistsErrorMessages(result.Errores)) {
            recargarPantalla();
        }
    }
}

function actualizarCierre() {
    var cambioDeLosDias = datosDias();
    var result = MSExecuteOnServer('/Formula/ActualizarCierre', cambioDeLosDias);
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
    }
}

function agregar(e) {

    recargarPantalla();
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
            recargarPantalla();
            cargarTiposNegociosExcluidos();
        }
    }    


    $("#treelist").data("kendoTreeList").dataSource.read();
    $("#popUpAgregarCriterio").data("kendoWindow").close();
}

function datosDias() {

    var cuposDesde = $("#cuposDesde").val();
    var cuposHasta = $("#cuposHasta").val();
    var negociosDesde = $("#negociosDesde").val();
    var negociosHasta = $("#negociosHasta").val();
    var MaterialId = $("#MaterialId").val();
    var cierre = $("#deshabilitar").is(":checked");

    formulaDias = {
        "CuposDesde": cuposDesde,
        "CuposHasta": cuposHasta,
        "NegociosDesde": negociosDesde,
        "NegociosHasta": negociosHasta,
        "Cierre": cierre,
        MaterialId: MaterialId
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

    criteriosAgregados.forEach(function (criAgregado) {

        return criteriosParaAgregar = criteriosParaAgregar.filter(function (criterio) { return criterio.Descripcion != criAgregado.Descripcion });
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

function Deshabilitar() {
    actualizarDias();
}

function recargarPantalla() {
    $("#treelist").data("kendoTreeList").dataSource.data([]);
    traerDatosIniciales($("#MaterialId").val());
    $("#treelist").data("kendoTreeList").dataSource.read();
}
$(document)
    .ajaxStart(function () {
        BlockUi('Cargando...');
    })
    .ajaxStop(function () {
        $.unblockUI();
    });

//function EjecutarAlgoritmo() {

//    var mensaje = MSExecuteOnServer('/Formula/EjecutarFormula', { materialId: $("#MaterialId").val() });
//    if (mensaje != null) {
//        MensInfoReload(mensaje);
//    } else {
//        MensErr("Error al ejecutar el algoritmo")
//    }
//}

function cargarTiposNegociosExcluidos() {
    var material = $("#MaterialId").val() == "" ? null : parseInt($("#MaterialId").val());
    datos = {
        MaterialId: material
    }
    var result = MSExecuteOnServer('/Formula/BuscarTiposNegociosExcluidos', datos);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        if (!ExistsErrorMessages(result.Errores)) {
            // Obtener la instancia del MultiSelect
            var multiSelect = $("#TipoNegocioId").data("kendoMultiSelect");
            // Obtener los datos seleccionados actualmente
            var datosSeleccionados = [];
            // Agregar los nuevos elementos preseleccionados a los datos existentes
            result.Datos.forEach(function (elemento) {
                datosSeleccionados.push(elemento.TipoNegocioId);
            });
            // Establecer los datos seleccionados en el MultiSelect
            multiSelect.value(datosSeleccionados);
        }
    }
}

function actualizarTiposNegociosExcluidos() {
    var formulaDias = datosDias();
    var tiposNegociosExcluidos = [];
    // Obtener una referencia al widget kendoMultiSelect
    var multiSelect = $("#TipoNegocioId").data("kendoMultiSelect");
    // Obtener los elementos seleccionados como objetos de datos
    var selectedItems = multiSelect.dataItems();

    for (var i = 0; i < selectedItems.length; i++) {
        var selectedItem = selectedItems[i];
        // Acceder a las propiedades del objeto de datos
        var TipoNegocioId = parseInt(selectedItem.value);
        var Descripcion = selectedItem.text;

        tiposNegociosExcluidos.push({ TipoNegocioId: TipoNegocioId, Descripcion: Descripcion })
    }

    var datos = {
        materialId: parseInt($("#MaterialId").val()),
        tiposNegociosExcluidos: tiposNegociosExcluidos,
        formulaDias: formulaDias,
    }
    var result = MSExecuteOnServer('/Formula/ActualizarTiposNegociosExcluidos', datos);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
            cargarTiposNegociosExcluidos();
        }
    }

    
}

$("body").on("change", '#vincularSolicitudExtraordinaria', function () {
    let valor = $("#vincularSolicitudExtraordinaria").prop("checked");
    $("#vincularSolicitudExtraordinaria").prop("checked", !valor);

    var datos = {
        valor: valor,
    }
    var result = MSExecuteOnServer('/Formula/VincularSolicitudExtraordinaria', datos);
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowErrorMessages(result.Errores);
        } else {
            $("#vincularSolicitudExtraordinaria").prop("checked", valor);
            MensInfo("Actualización exitosa.");
        }
    }
});