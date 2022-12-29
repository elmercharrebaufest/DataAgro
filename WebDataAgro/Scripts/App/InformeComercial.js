var ProveedorId;
var viewModel;
var datosIniInformeComercial;
var estaModificando = false;
var datosModificacion;
var activo = 0;
var variablesUltima = "";

$(document).ready(function () {
    $("#tabstrip").kendoTabStrip();

    InicializarElementos();

    CreateGridInformeComercial();

    CrearViewModel();

    //InicializarBusquedaInicial();

    HabilitarInicio();
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#butAceptar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Aceptar.png")
    });

    $("#butAceptar").click(function () {
        EmitirInforme();
    });

    $("#butVolver").click(function () {
        if (estaModificando) {
            LimpiarInputs();
            $("#butAceptar").hide();
            $("#butVolver").hide();
            $("#tab2").hide();
            $("#tab1").show();
            $("#informer").click();
        } else {
            LimpiarInputs();
            reiniciarPagina();
        }
    });

    $("#butSiguiente").click(function () {
        var algunaSeleccion = false;

        var datos = $("#gridInformeComercial").data("kendoGrid").dataSource.data();
        for (var i = 0; i < datos.length; i++) {
            if (datos[i].Seleccionado == true) {
                algunaSeleccion = true;
            }
        }

        if (algunaSeleccion) {
            $("#tab1").hide();
            $("#tab2").show();
            $("#butSiguiente").hide();
            $("#butAceptar").show();
            $("#butVolver").show();
        } else {
            MensErr("No hay ningún material seleccionado");
        }
    });

    $("input[name='depen']").change(function () {
        if ($("input[name='depen']:checked").val() == "si") {
            $("#EmplRelDepCant").prop("disabled", false);
        } else {
            $("#EmplRelDepCant").val("");
            $("#EmplRelDepCant").prop("disabled", true);
        }
    });

    $("input[name='rodado']").change(function () {
        if ($("input[name='rodado']:checked").val() == "otrosro") {
            $("#RodadosOtros").prop("disabled", false);
        } else {
            $("#RodadosOtros").val("");
            $("#RodadosOtros").prop("disabled", true);
        }
    });

    $("input[name='servicios']").change(function () {
        if ($("input[name='servicios']:checked").val() == "otros") {
            $("#ChacraOtros").prop("disabled", false);
        } else {
            $("#ChacraOtros").val("");
            $("#ChacraOtros").prop("disabled", true);
        }
    });

    $("#Inhabilitado").prop("checked", true);

    $(".proveedor-input").keyup(function (e) {
        armarBusquedaProveedorResult(e);
    });

    $(".proveedor-input").blur(function () {
        //$(".buscar-result").empty();
        //$(".buscar-result").hide();
    });
    $(".proveedor-input").focus(function (e) {
        e.stopPropagation();
        e.preventDefault();
        armarBusquedaProveedorResult();
    });

    $("#ninforme").click(function () {
        $("#nuevotab").hide();
        $("#tabactual").show();
        $("#butSiguiente").show();
    });

    $("#informer").click(function () {
        $("#nuevotab").show();
        $("#tabactual").hide();
        $("#butSiguiente").hide();
    });

    $("#limpiarinput").click(function () {
        $(".proveedor-input").val("");
        $("#gridInformeComercial").data("kendoGrid").dataSource.data([]);
        $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data([]);
        $("#ninforme .k-link").html("Informes Pendientes");
        $("#informer .k-link").html("Informes Realizados");
    });
}

function LimpiarInputs() {
    estaModificando = false;
    datosModificacion = null;
    $("#materialestemp").remove();
    $("#si").prop("checked", false);
    $("#EmplRelDepCant").val("");
    $("#EmplRelDepCant").prop("disabled", true);
    $("#no").prop("checked", false);

    $("#equipro").prop("checked", false);
    $("#equialq").prop("checked", false);
    $("#proyalq").prop("checked", false);
    $("#otrosro").prop("checked", false);
    $("#RodadosOtros").val("");
    $("#RodadosOtros").prop("disabled", true);

    $("#persprop").prop("checked", false);
    $("#perscont").prop("checked", false);
    $("#persprco").prop("checked", false);
    $("#otros").prop("checked", false);
    $("#ChacraOtros").val("");
    $("#ChacraOtros").prop("disabled", true);

    $("#AntigActividad").val("");
    $("#ActuacionProd").val("");
    $("#ClienteAnt").val("");
    $("#Domicilio").val("");
    $("#Comentarios").val("");
}

function reiniciarPagina() {
    $("#tab2").hide();
    $("#tab1").show();
    $("#butAceptar").hide();
    $("#butVolver").hide();
    $("#butSiguiente").show();

    $("#ninforme").click();

    $(".proveedor-input").val("");
    $("#gridInformeComercial").data("kendoGrid").dataSource.data([]);
    $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data([]);

    $("#ninforme .k-link").html("Informes Pendientes");
    $("#informer .k-link").html("Informes Realizados");
}

function CrearResultadosDataSource(datos) {
    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    ProveedorId: { type: "number", editable: false },
                    MaterialId: { type: "number", editable: false },
                    Material: { type: "string", editable: false },
                    Campaña: { type: "string", editable: false },
                    CampañaId: { type: "number", editable: false },
                    Seleccionado: { type: "boolean", editable: true },
                }
            }
        },
    });

    return ds;
}

function ModificarInformeComercial(InformeComercialId) {
    if (activo == 0) {
        activo = 1;
        LimpiarInputs();
        function funcReturn(datos) {
            if (datos.parametros) {
                datosModificacion = datos.parametros;
                $("#Domicilio").val(datos.parametros.Domicilio);

                if (datos.parametros.EmplRelDep) {
                    $("#si").prop("checked", true);
                    $("#EmplRelDepCant").prop("disabled", false);
                    $("#EmplRelDepCant").val(datos.parametros.EmplRelDepCant);
                } else {
                    $("#no").prop("checked", true);
                }

                switch (datos.parametros.Rodados) {
                    case 1:
                        $("#equipro").prop("checked", true);
                        break;
                    case 2:
                        $("#equialq").prop("checked", true);
                        break;
                    case 3:
                        $("#proyalq").prop("checked", true);
                        break;
                    case 4:
                        $("#otrosro").prop("checked", true);
                        $("#RodadosOtros").prop("disabled", false);
                        $("#RodadosOtros").val(datos.parametros.RodadosOtros);
                        break;
                }

                switch (datos.parametros.Chacra) {
                    case 1:
                        $("#persprop").prop("checked", true);
                        break;
                    case 2:
                        $("#perscont").prop("checked", true);
                        break;
                    case 3:
                        $("#persprco").prop("checked", true);
                        break;
                    case 4:
                        $("#otros").prop("checked", true);
                        $("#ChacraOtros").prop("disabled", false);
                        $("#ChacraOtros").val(datos.parametros.ChacraOtros);
                        break;
                }

                $("#AntigActividad").val(datos.parametros.AntigActividad);
                $("#ActuacionProd").val(datos.parametros.ActuacionProd);
                $("#ClienteAnt").val(datos.parametros.ClienteAnt);

                $("#Comentarios").val(datos.parametros.Comentarios);

                var formulario = $(".formulario");
                var formulariocampo = $("<div id='materialestemp'>").addClass("formulario-campo").appendTo(formulario);
                $("<div>").addClass("formulario-label").appendTo(formulariocampo).html("Materiales");
                var formularioinput = $("<div>").addClass("formulario-input").appendTo(formulariocampo);
                var select = $("<select style='width:600px;' multiple id='materiales'>").appendTo(formularioinput);
                var arrMats = [];
                for (var ii in datos.materiales) {
                    (function (i) {
                        select.append("<option value='" + datos.materiales[i].MaterialId + "'>" + datos.materiales[i].Material + "</option>");
                        if (datos.materiales[i].Seleccionado)
                            arrMats.push(datos.materiales[i].MaterialId);
                    })(ii);
                }

                select.multiselect({
                    header: false,
                    selectedList: "5",
                    noneSelectedText: "Elegir",
                });

                $("#materiales").val(arrMats);
                $("#materiales").multiselect("refresh");
                $("#materiales").multiselect("disable");

                estaModificando = true;

                $("#tab1").hide();
                $("#tab2").show();
                $("#butSiguiente").hide();
                $("#butAceptar").show();
                $("#butVolver").show();

                activo = 0;
            }
        }

        MSExecuteOnServerAsync('/InformeComercial/ModificarInformeComercial', { InformeComercialId: InformeComercialId }, funcReturn, true);
    }
}

function EliminarInformeComercial(InformeComercialId) {
    Confirma("¿Desea eliminar el registro?", function (dialogItself) {
        function funcReturn(datos) {
            var grilla = $("#gridInformeComercialNuevo").data("kendoGrid");
            var elemento = grilla.dataItem(grilla.select());
            grilla.dataSource.remove(elemento);
            llenarInput(variablesUltima);
        }

        MSExecuteOnServerAsync('/InformeComercial/EliminarInformeComercial', { InformeComercialId: InformeComercialId }, funcReturn, false);
    });
}

function descargarPdf(InformeComercialId) {
    function funcReturn(datos) {
        if (datos.DownloadKey.length > 0) {
            var url = MSGetUrl('/DownLoad/Reporte?key=' + datos.DownloadKey);
            window.open(window.location.origin + "/" + url, '_blank');
        }
    }

    MSExecuteOnServerAsync('/InformeComercial/ReImprimirPDF', { InformeComercialId: InformeComercialId }, funcReturn, false);
}

function CreateGridInformeComercial() {
    $("#gridInformeComercial").kendoGrid({
        columns: [
            { template: '<input type="checkbox" #= Seleccionado ? \'checked="checked"\' : "" # class="chkbx" />', width: 110 },
            { field: "Material", title: "Material", width: 250 },
            { field: "Campaña", title: "Campaña", width: 250 },
            { field: "CampañaId", hidden: true },
            { field: "MaterialId", hidden: true },
            { field: "ProveedorId", hidden: true }
        ],
        sortable: true,
        selectable: "row"
    });

    $("#gridInformeComercial").on("change", "input.chkbx", function (e) {
        var grid = $("#gridInformeComercial").data("kendoGrid"),
            dataItem = grid.dataItem($(e.target).closest("tr"));

        dataItem.set("Seleccionado", this.checked);
    });

    $("#gridInformeComercialNuevo").kendoGrid({
        columns: [
            { field: "Cuit", width: 140 },
            { field: "RazonSocial", title: "Razón Social", width: 150 },
            { field: "InformeComercialId", hidden: true },
            { field: "EstadoId", hidden: true },
            { field: "Comercial" },
            { field: "Estado", hidden: true },
            { field: "Campaña" },
            { field: "Material", template: "#=Material#" },
            { field: "FechaAlta", type: "date", title: "Fecha de Generación", template: "#= kendo.toString(kendo.parseDate(FechaAlta), 'dd/MM/yyyy hh:mm') #" },
            {
                field: "Modificar", title: "", width: 40, template: function (dataItem) {
                    if (dataItem.EstadoId == 1) {
                        return "<span class='imprimir' title='Modificar' onClick='ModificarInformeComercial(" + dataItem.InformeComercialId + ")'><img src='../content/images/modificar.png' style='width:16px;height:16px;cursor:pointer;' /> </span>";
                    } else {
                        return "<span></span>";
                    }
                }
            },
            {
                field: "Eliminar", title: "", width: 40, template: function (dataItem) {
                    if (dataItem.EstadoId == 1) {
                        return "<span class='imprimir' title='Eliminar' onClick='EliminarInformeComercial(" + dataItem.InformeComercialId + ")'><img src='../content/images/delete.png' style='width:16px;height:16px;cursor:pointer;' /> </span>";
                    } else {
                        return "<span></span>";
                    }
                }
            },
            {
                field: "Descargar", title: "", width: 40, template: function (dataItem) {
                    if (dataItem.InformeComercialId > 0) {
                        return "<span class='imprimir' title='ReImprimir' onClick='descargarPdf(" + dataItem.InformeComercialId + ")'><img src='../content/images/lupa.png' style='width:16px;height:16px;cursor:pointer;' /> </span>";
                    } else {
                        return "<span> " + dataItem.Cuit + "</span> ";
                    }
                }
            },

        ],
        sortable: true,
        selectable: "row"
    });

    $("#gridInformeComercialNuevo thead [data-field=Eliminar] .k-link").html("");
    $("#gridInformeComercialNuevo thead [data-field=Modificar] .k-link").html("");
    $("#gridInformeComercialNuevo thead [data-field=Descargar] .k-link").html("");
}

function CrearViewModel() {
    var ResultadosDataSource = CrearResultadosDataSource([]);

    viewModel = kendo.observable({
        Resultados: ResultadosDataSource,

        isReadOnly: true,
        isFilterDisabled: true,
        isModifyDisabled: false,
        isControlDisabled: true,
        isDeleteDisabled: true,

        CanalOperacion: null,
    });

    kendo.bind($("#InformeComercial"), viewModel);
}

function llenarInput(variables) {
    variablesUltima = variables;
    var razon = variables.split("|||")[1];
    var cuit = variables.split("|||")[0];
    var id = variables.split("|||")[2];
    $(".proveedor-input").val(razon + " (" + cuit + ")");
    //armarBusquedaProveedorResult(cuit, true);

    MSExecuteOnServerAsync('/InformeComercial/ListarMateriales', { filtro: id }, funcReturnListaMateriales, false);

    function funcReturnListaMateriales(result) {
        $("#ninforme .k-link").html("Informes Pendientes (" + result.materiales.length + ")")
        $("#informer .k-link").html("Informes Realizados (" + result.InformeGenerado.length + ")")
        if (result.materiales.length == 0 && result.InformeGenerado.length == 0) {
            $("#gridInformeComercial").data("kendoGrid").dataSource.data([]);
            $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data([]);
            MensInfo("No se encontraron Resultados");
        } else {
            if (result.materiales.length > 0) {
                $("#gridInformeComercial").data("kendoGrid").dataSource.data([]);
                var grid = $("#gridInformeComercial").data("kendoGrid");
                var ds = [];
                for (var ii in result.materiales) {
                    (function (i) {
                        var obj = {};
                        obj.ProveedorId = result.materiales[i].ProveedorId;
                        obj.MaterialId = result.materiales[i].MaterialId;
                        obj.Material = result.materiales[i].Material;
                        obj.Campaña = result.materiales[i].Campaña;
                        obj.CampañaId = result.materiales[i].CampañaId;
                        obj.Seleccionado = false;
                        ds.push(obj);
                    })(ii);
                }
                $("#gridInformeComercial").data("kendoGrid").dataSource.data(ds);
            } else {
                var ds = [];
                $("#gridInformeComercial").data("kendoGrid").dataSource.data(ds);
                $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data(ds);
            }

            if (result.InformeGenerado.length > 0) {
                $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data([]);
                var grid = $("#gridInformeComercialNuevo").data("kendoGrid");
                var ds = [];
                for (var ii in result.InformeGenerado) {
                    (function (i) {
                        var obj = {};
                        obj.Cuit = result.InformeGenerado[i].Cuit;
                        obj.RazonSocial = result.InformeGenerado[i].RazonSocial;
                        obj.Comercial = result.InformeGenerado[i].Comercial;
                        obj.Estado = result.InformeGenerado[i].EstadoInforme;
                        obj.Material = result.InformeGenerado[i].Materiales;
                        obj.Campaña = result.InformeGenerado[i].Campaña;
                        obj.FechaAlta = result.InformeGenerado[i].FechaAlta;
                        obj.Modificar = "";
                        obj.Eliminar = "";
                        obj.Descargar = "";
                        obj.EstadoId = result.InformeGenerado[i].EstadoId;
                        obj.InformeComercialId = result.InformeGenerado[i].InformeComercialId;
                        ds.push(obj);
                    })(ii);
                }
                $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data(ds);
            } else {
                var ds = [];
                $("#gridInformeComercialNuevo").data("kendoGrid").dataSource.data(ds);
            }
        }
    }
}

function armarBusquedaProveedorResult(value, cambio) {
    if ($(".proveedor-input").val().length >= 3) {
        $(".buscar-result-proveedor").empty();

        //aca tiene que ir a buscar
        var txt = $(".proveedor-input").val().toUpperCase();

        if (cambio)
            txt = value;

        var result = MSExecuteOnServer('/Home/BusquedaHome', { filtro: txt });

        var html = "";
        for (var i = 0; i < result.length; i++) {
            var valor = "";

            valor = result[i].RazonSocial + ' (' + result[i].Cuit + ')';

            valor = valor.toUpperCase().split(txt).join("<strong>" + txt + "</strong>");

            var url = MSGetUrl("/Content/Images/usuario-busqueda.png");

            var variables = "'" + result[i].Cuit + "|||" + result[i].RazonSocial + "|||" + result[i].Id + "'";

            html += '<div onclick="llenarInput(' + variables + ')" class="buscar-result-linea">'
                + '<img class="buscar-cont" src="..' + url + '" /> '
                + '<p class="buscar-nomb">' + valor + '</p>'
                + '</div>';
        }

        if (!result.length) {
            html += '<div class="buscar-result-linea">'
                + '<p class="buscar-nomb">No se encontraron resultados</p>'
                + '</div>';
        }

        $(".buscar-result-proveedor").append(html);

        $(".buscar-result-proveedor").show();
    } else {
        $(".buscar-result-proveedor").empty();
        $(".buscar-result-proveedor").hide();
    }
}

$(window).click(function (e) {
    if ($(".buscar-result-proveedor").is(":visible")) {
        if (!$(".proveedor-input").is(":focus")) {
            $(".buscar-result-proveedor").empty();
            $(".buscar-result-proveedor").hide();
        }
    }
})

function Grabar() {
    var grid = $("#gridInformeComercial").data("kendoGrid");

    var row = grid.select();

    var data = grid.dataItem(row);

    var objectstate = 0;

    if (data != null) {
        objectstate = 2;
    }

    LimpiarValidaciones();

    var datos = {
        "ObjectState": objectstate,
        "CanalOperacionId": viewModel.get("CanalOperacion.CanalOperacionId"),
        "Descripcion": viewModel.get("CanalOperacion.Descripcion"),
        "Inhabilitado": viewModel.get("CanalOperacion.Inhabilitado"),
    };

    var result = MSExecuteOnServer('/CanalOperacion/Grabar', datos);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            UpdateViewModel(result);
            InicializarBusquedaInicial();
            MensInfo("Grabación Realizada Correctamente");
            HabilitarInicio();
        }
    }
}

function EmitirInformeModificado() {
    var campañaid = datosModificacion.CampañaId;
    var campaña = datosModificacion.Campaña;
    var ArrayMaterial = $("#materiales").val();
    var proveedorId = datosModificacion.ProveedorId;
    var arrMat = [];
    if (ArrayMaterial != null && ArrayMaterial.length > 0) {
        for (var i = 0; i < ArrayMaterial.length; i++) {
            arrMat.push({
                MaterialId: ArrayMaterial[i],
                Toneladas: null
            });
        }
    }

    var datos = $("#gridInformeComercial").data("kendoGrid").dataSource.data();

    //var informeComercialId = datos[0].InformeComercialId;
    var informeComercialId = datosModificacion.InformeComercialId;
    var rod = 0;
    switch ($("input[name='rodado']:checked").val()) {
        case "equipro":
            rod = 1;
            break;
        case "equialq":
            rod = 2;
            break;
        case "proyalq":
            rod = 3;
            break;
        case "otrosro":
            rod = 4;
            break;
    }

    var ser = 0;
    switch ($("input[name='servicios']:checked").val()) {
        case "persprop":
            ser = 1;
            break;
        case "perscont":
            ser = 2;
            break;
        case "persprco":
            ser = 3;
            break;
        case "otros":
            ser = 4;
            break;
    }

    var obj = {
        ProveedorId: proveedorId,
        Materiales: arrMat,
        CampañaId: campañaid,
        Campaña: campaña,
        EmplRelDep: ($("input[name='depen']:checked").val() == "si" ? true : false),
        EmplRelDepCant: $("#EmplRelDepCant").val(),
        Rodados: rod,
        RodadosOtros: $("#RodadosOtros").val(),
        Chacra: ser,
        ChacraOtros: $("#ChacraOtros").val(),
        AntigActividad: $("#AntigActividad").val(),
        ActuacionProd: $("#ActuacionProd").val(),
        ClienteAnt: $("#ClienteAnt").val(),
        Comentarios: $("#Comentarios").val(),
        Domicilio: $("#Domicilio").val(),
        InformeComercialId: informeComercialId
    };

    function funcReturn(datos) {
        if (datos.DownloadKey.length > 0) {
            LimpiarInputs();
            reiniciarPagina();
            var url = MSGetUrl('/DownLoad/Reporte?key=' + datos.DownloadKey);

            window.open(window.location.origin + "/" + url, '_blank');
        }
    }

    MSExecuteOnServerAsync('/InformeComercial/Listar', obj, funcReturn, true)
}

function EmitirInforme() {
    if (estaModificando) {
        EmitirInformeModificado();
        return;
    }

    var datos = $("#gridInformeComercial").data("kendoGrid").dataSource.data();

    var campañaid = 0;
    var ArrayMaterial = [];
    var Param = [];
    var proveedorId = 0;

    var rod = 0;
    switch ($("input[name='rodado']:checked").val()) {
        case "equipro":
            rod = 1;
            break;
        case "equialq":
            rod = 2;
            break;
        case "proyalq":
            rod = 3;
            break;
        case "otrosro":
            rod = 4;
            break;
    }

    var ser = 0;
    switch ($("input[name='servicios']:checked").val()) {
        case "persprop":
            ser = 1;
            break;
        case "perscont":
            ser = 2;
            break;
        case "persprco":
            ser = 3;
            break;
        case "otros":
            ser = 4;
            break;
    }

    for (var i = 0; i < datos.length; i++) {
        if (datos[i].Seleccionado == true) {
            if (proveedorId == 0)
                proveedorId = datos[i].ProveedorId;

            if ($.inArray(datos[i].CampañaId, Param) === -1) {
                Param.push(datos[i].CampañaId);
            }
        }
    }

    for (var j = 0; j < Param.length; j++) {
        var arrMat = [];
        var objCam = null;
        for (var i = 0; i < datos.length; i++) {
            if (datos[i].Seleccionado == true) {
                if (datos[i].CampañaId == Param[j]) {
                    arrMat.push({
                        MaterialId: datos[i].MaterialId,
                        Toneladas: null
                    });
                    if (!objCam) {
                        objCam = {
                            CampañaId: datos[i].CampañaId,
                            Campaña: datos[i].Campaña
                        }
                    }
                }
            }
        }

        var obj = {
            ProveedorId: proveedorId,
            Materiales: arrMat,
            CampañaId: objCam.CampañaId,
            Campaña: objCam.Campaña,
            EmplRelDep: ($("input[name='depen']:checked").val() == "si" ? true : false),
            EmplRelDepCant: $("#EmplRelDepCant").val(),
            Rodados: rod,
            RodadosOtros: $("#RodadosOtros").val(),
            Chacra: ser,
            ChacraOtros: $("#ChacraOtros").val(),
            AntigActividad: $("#AntigActividad").val(),
            ActuacionProd: $("#ActuacionProd").val(),
            ClienteAnt: $("#ClienteAnt").val(),
            Comentarios: $("#Comentarios").val(),
            Domicilio: $("#Domicilio").val(),
            InformeComercialId: 0
        };

        function funcReturn(datos) {
            if (datos.DownloadKey.length > 0) {
                LimpiarInputs();
                reiniciarPagina();
                var url = MSGetUrl('/DownLoad/Reporte?key=' + datos.DownloadKey);

                window.open(window.location.origin + "/" + url, '_blank');
            }
        }

        MSExecuteOnServerAsync('/InformeComercial/Listar', obj, funcReturn, true)
    }
}

function HabilitarInicio() {
    //$('#rootwizard').bootstrapWizard('show', 'tab1');
}