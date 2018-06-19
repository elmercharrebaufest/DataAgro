var viewModel;
var datosIniCrearFijacion;

$(document).ready(function () {

    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    InicializarDatos();
 
    CrearViewModel();
});

function InicializarElementos() {

    kendo.culture("es-AR");

    $("#proveedorId").kendoDropDownList({
        optionLabel: "SELECCIONE UN PROVEEDOR...",
        dataTextField: "Descripcion",
        dataValueField: "ProveedorId"
    });

    $("#proveedorId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#proveedorId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").val("");
    });

    $("#buscadorProveedor").keyup(function (e) {
        armarBusquedaResultProveedor(e);
    });

    $("#buscadorProveedor").focus(function (e) {
        e.stopPropagation();
        e.preventDefault();
        // armarBusquedaResultProveedor();
    });


    $("#comercialId").kendoDropDownList({
        optionLabel: "SELECCIONE UN COMERCIAL...",
        dataValueField: "ComercialId",
        dataTextField: "Comercial"
    });

    $("#comercialId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#comercialId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });
    


    $("#material").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#precioMonedaId").kendoDropDownList({
        optionLabel: "MONEDA...",
        dataTextField: "MonedaId",
        dataValueField: "Descripcion"
    });

    $("#precioMonedaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#precioMonedaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $(".formulario-footer-guardar-fijacion").click(function () {
        //if (comprobarInputs()) {
        //if (validar()) {
        ObtenerDatos();
        //}
        //}
    });

    $(".formulario-footer-cancelar").click(function () {
        window.location.href = window.location.origin + "/CompraNet";
    });

}

function CrearViewModel() {

    var param = {
        "proveedorId": null,
        "comercialId": null,
        "comercialDesc": null,
        "material": null,
        "materialDesc": null,
        "cantidadId": null,
        "precioId": null,
        "precioMonedaId": null,
        "precioMonedaDesc": null

    };

    viewModel = kendo.observable({

        Parametros: param,

        ProveedorCombo: [],
        ComercialCombo: [],
        MaterialCombo: [],
        PrecioMonedaCombo: [],

        isControlDisabled: true,

    });

    kendo.bind($("#CrearFijacion"), viewModel);
}

function InicializarDatos() {

    var funcReturn = function (data) {

        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniCrearFijacion = data;
            AsignarDatos();
            RefrescarWidgets();
        }
    }

    MSExecuteURLOnServerAsync('/CompraNet/InicializarFijacion', funcReturn, '');
}

function AsignarDatos() {

    /*
    viewModel.set("Parametros", datosIniCrearFijacion.Param);
    viewModel.set("Parametros", datosIniCrearFijacion.Param);
    */
    viewModel.set("ProveedorCombo", datosIniCrearFijacion.Datos.proveedor);
    viewModel.set("ComercialCombo", datosIniCrearFijacion.Datos.comercial);
    viewModel.set("MaterialCombo", datosIniCrearFijacion.Datos.material);
    viewModel.set("PrecioMonedaCombo", datosIniCrearFijacion.Datos.moneda);
    var comercialId = MSExecuteOnServer('/CompraNet/ObtenerComercialId');
    $("#comercialId").data("kendoDropDownList").value(comercialId.Result);

    viewModel.set("isControlDisabled", false);
}

function RefrescarWidgets() {

    //viewModel.Parametros.proveedorId = $("#proveedorId").data("kendoDropDownList").dataItem();
    //viewModel.Parametros.comercialId = $("#comercialId").data("kendoDropDownList").dataItem();
    //viewModel.Parametros.material = $("#material").data("kendoDropDownList").dataItem();
    //viewModel.Parametros.precioMonedaId = $("#precioMonedaId").data("kendoDropDownList").dataItem();
}

function LimpiarValidaciones() {

    $("#errproveedorId").css("display", "none");
    $("#errcomercialId").css("display", "none");
    $("#errmaterial").css("display", "none");
    $("#errcantidadId").css("display", "none");
    $("#errprecioId").css("display", "none");
    $("#errprecioMonedaId").css("display", "none");
}

function ObtenerDatos() {

    var obj = {};
    var fecha = new Date();
    var fechaHoy = new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDate());

    cuitAux = $("#buscadorProveedor").val().split('(');
    cuit = cuitAux[1].split(')');

    var proveedorId = MSExecuteOnServer('/CompraNet/ObtenerProveedorId', { Cuit: cuit[0] });
    obj.Fecha = fechaHoy;
    obj.proveedorId = proveedorId;
    obj.MaterialId = $("#material").val();
    obj.Cantidad = $("#cantidadId").val();
    obj.Precio = $("#precioId").val();
    obj.MonedaId = $("#precioMonedaId").val();
    obj.comercialId = $("#comercialId").val();
    GrabarFijacion(obj);
}

function GrabarFijacion(nuevaFijacion) {

    
    //if (ProveedorId)
      //  nuevoProveedor.ProveedorId = ProveedorId;
    
    var result = MSExecuteOnServer('/CompraNet/GrabarFijacion', nuevaFijacion);
    if (result != null) {

        if (ExistsErrorMessages(result.errores.ListaErrores)) {
            MensErr(result.errores.ListaErrores[0].Message);
        }
        else {
            //MensInfo("Se ha realizado la operacion con exito");
            //window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + result.ProveedorId;
        }
    }
}


function armarBusquedaResultProveedor() {
    if ($("#buscadorProveedor").val().length >= 3) {
        $("#buscadorResult").empty();

        //aca tiene que ir a buscar
        var txt = $("#buscadorProveedor").val().toUpperCase();


        var result = MSExecuteOnServer('/Home/BusquedaHome', { filtro: txt });


        var html = "";
        for (var i = 0; i < result.length; i++) {

            var valor = "";


            valor = result[i].RazonSocial + ' (' + result[i].Cuit + ')';

            valor = valor.toUpperCase().split(txt).join("<strong>" + txt + "</strong>");

            var url = MSGetUrl("/Content/Images/usuario-busqueda.png");

            html += '<div class="buscar-result-linea" onclick="seleccionarProveedor(this)" >'
                  + '<img class="buscar-cont" src="..' + url + '" /> '
                  + '<p class="buscar-nomb">' + valor + '</p>'
                  + '</div>';
        }

        if (!result.length) {
            html += '<div class="buscar-result-linea">'
                  + '<p class="buscar-nomb">No se encontraron resultados</p>'
                  + '</div>';
        }

        $("#buscadorResult").append(html);
        console.log($("#buscadorProveedor").is(":focus"));
        $("#buscadorResult").show();
    } else {
        $("#buscadorResult").empty();
        $("#buscadorResult").hide();
    }
}

function seleccionarProveedor(opciones) {
    $("#buscadorProveedor").val(opciones.innerText);
    $("#buscadorResult").hide();


}