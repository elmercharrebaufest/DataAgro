function armarDetalleCorredor(datos){
    $(".noCorredor").hide();
    $("#proveedoresCorredor").show();
    CargarProveedores(datos);
}

function CargarProveedores(datos) {

    var proveedores = MSExecuteOnServer('/Proveedor/TraerProveedoresCorredor', datos);

    if (proveedores !== null) {
        for (var i = 0; i < proveedores.length; i++) {
            var obj = proveedores[i];
            var htmlProveedores = "";
            htmlProveedores += '<div class="contenedor-proveedorescorredor-comercial" id="proveedor' + i + '">' +
                '<div class="contenedor-proveedor-corredor-titulo">' +
                '<img class="img-contacto-comercial" src="../Content/Images/contprinc-cont4.png" /> ' +
                '<span class="span-contacto-comercial"> ' +
                obj.RazonSocial + ' (' + obj.CUIT + ')</span>' +                
                '</div>' +
                '<div class="row">'+
                '<div class="contenedor-contacto-comercial-posicion col-md-5">' +
                '<span class="contenedor-contacto-comercial-posicion-izq">' +
                (obj.Direccion ? obj.Direccion + ' (' + obj.CodigoPostal + ')' : "No se especifica dirección") +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-posicion col-md-6">' +
                '<span class="contenedor-contacto-comercial-posicion-izq">' +
                (obj.Localidad ? obj.Localidad + ', ' + obj.Provincia : "No se especifica procedencia") +
                '</span>' +
                '</div>' +
                '</div>' +
                '<div class="row">' +
                '<div class="contenedor-contacto-comercial-posicion col-lg-3">' +
                '<span class="contenedor-contacto-comercial-posicion-izq">' +
                'Procedencia Compranet:' +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-posicion col-lg-6">' +
                '<span class="contenedor-contacto-comercial-posicion-der contacto-proveedor-corredor">' +
                (obj.LocalidadCompraNetId !== null ? obj.LocalidadCompraNet + " (" + obj.ProvinciaCompraNet + ")" : "no especifica") +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-posicion col-lg-3">' +
                '<span class="contenedor-contacto-comercial-posicion-izq">' +
                'Clasificacion:' +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-posicion col-lg-3">' +
                '<span class="contenedor-contacto-comercial-posicion-der contacto-proveedor-corredor">' +
                (obj.ClasificacionDescripcion ? obj.ClasificacionDescripcion : "no especifica") +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-posicion col-lg-3 clearLeft">' +
                '<span class="contenedor-contacto-comercial-posicion-izq">' +
                'Consignatario:' +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-posicion col-lg-3">' +
                '<span class="contenedor-contacto-comercial-posicion-der contacto-proveedor-corredor">' +
                (obj.Consignatario ? "SI" : "NO") +
                '</span>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>';
            $(".proveedores-corredor").append(htmlProveedores);
        }
    }
}