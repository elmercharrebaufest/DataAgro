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
    $("#" + key.id).val($("#" + key.id).is(':checked') ? 'true' : 'false');
    BlockUi('Guardando...');
    setTimeout(function () {
        $("#actualizarListado").submit();
        $.unblockUI();
    }, 250);
}

function AbrirModal() {
    $("#ModalCrearNegocio").modal("show");
}
function AbrirModalBoletoProvincia() {
    $("#ModalCrearBoletoProvincia").modal("show");
}

function GrabarNegocio() {
    $(function () {
        BlockUi('Guardando...');
        setTimeout(function () {
            $.ajax({
                url: $('#grabarNegocio').attr('action'),
                type: "POST",
                data: $('#grabarNegocio').serialize(),
                success: function (result) {
                    $("#ModalCrearNegocio").modal("hide");
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                    if (result.success) {
                        $("#listado").html(MSExecuteURLOnServer('/HabilitacionBoleto/RecargarTipoNegocioDetalle'));
                    } else {
                        alert('Hubo un error al guardar: ' + result.message);
                    }
                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                    alert('Error al guardar el tipo de negocio.');
                }
            });
        }, 250);
    });
}

function GrabarBoletoProvincia() {
    $(function () {
        BlockUi('Guardando...');
        $.ajax({
            url: '/HabilitacionBoleto/AgregarBoletoCompraNetProvincia',
            type: 'POST',
            data: $('#grabarBoletoProvincia').serialize(),
            success: function (result) {
                $("#ModalCrearBoletoProvincia").modal("hide");
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
                if (result.success) {
                    setTimeout(function () {
                        $("#actualizarBoletoProvincia").submit();
                    }, 250);
                } else {
                    alert('Hubo un error al guardar: ' + result.message);
                }
                $.unblockUI();
            },
            error: function (result) {
                $.unblockUI();
                $('#ModalCrearBoletoProvincia').modal("hide");
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
                alert('Hubo un error al guardar: ' + result.message);
            }
        });
    });
}

$(document).off('click', '#TipoNegocioBoleto').on('click', '#TipoNegocioBoleto', function () {
    $("#tipoNegocioDetalle").toggleClass("hidden");
});

$(document).off('click', '#BoletoProvincia').on('click', '#BoletoProvincia', function () {
    $("#boletoCompraNetProvincia").toggleClass("hidden");
});


$(document).off('click', '#eliminarBoletoProvincia').on('click', '#eliminarBoletoProvincia', function (e) {
    e.preventDefault();

    var boletoId = $(this).data('id');

    if (confirm('¿Confirma la eliminación de este boleto habilitado?\n')) {
        BlockUi('Eliminando...');
        $.ajax({
            url: '/HabilitacionBoleto/EliminarBoletoCompraNetProvincia',
            type: 'POST',
            data: { id: boletoId },
            success: function (result) {
                if (result.success) {
                    setTimeout(function () {
                        $("#actualizarBoletoProvincia").submit();
                    }, 250);
                } else {
                    alert('Hubo un error al eliminar: ' + result.message);
                }
                $.unblockUI();
            },
            error: function () {
                $.unblockUI();
                alert('Error en la solicitud de eliminación.');
            }
        });
    }
});
