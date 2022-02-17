var habilitados;
$(document).ready(function () {
    $('#menuproveedor').hide();
    var listaPrecargada = $(".check")
    for (var i = 0; i < listaPrecargada.length; i++) {
        $(listaPrecargada[i]).val($(listaPrecargada[i]).is(':checked') ? 'true' : 'false');
    }

    $('#ModalCrearNegocio').on('shown.bs.modal', function () {
        $.unblockUI();
        $("#Descripcion").val("");        
    })
});

function actualizarListado(key) {
    //$('[Id]').change(function () {
    $("#" + key.id).val($("#" + key.id).is(':checked') ? 'true' : 'false');
    BlockUi('Guardando...');
    setTimeout(function () {
        $("#actualizarListado").submit();
        $.unblockUI();
    }, 250);
    //});
}

function AbrirModal() {
    $("#ModalCrearNegocio").modal("show");
}

function GrabarNegocio() {

    $(function () {
        BlockUi('Guardando...');
        setTimeout(function () {
            //$('#grabarNegocio').delegate('form', 'submit', function () {
                $.ajax({
                    url: $('#grabarNegocio').attr('action'),
                    type: "POST",
                    data: $('#grabarNegocio').serialize(),
                    success: function (result) {
                        if (result.success) {
                            $.ajax({
                                url: 'HabilitacionBoleto/RecargarPantalla',
                                type: "POST",
                                data: $('#actualizarListado').serialize(),
                                success: function (result) {
                                    $("#ModalCrearNegocio").modal("hide");
                                    $("#listado").html(result);
                                    $.unblockUI();
                                }
                            });
                           
                        } else {                           
                            $("#modal-agregar").html(result);
                            $('#ModalCrearNegocio').modal('hide');
                            $('body').removeClass('modal-open');
                            $('.modal-backdrop').remove();
                            $("#ModalCrearNegocio").modal("show");
                            $.unblockUI();
                        }
                    }
                });
            //    return false;
            //});
            $.unblockUI();
        }, 250);
    });
}

