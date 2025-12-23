using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class HabilitacionBoletoManager : IHabilitacionBoletoManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public HabilitacionBoletoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<TipoNegocioDetalleDto> ListarTipoNegociosDetalle()
        {
            return repositorio.Listar<TipoNegocioDetalle, TipoNegocioDetalleDto>(x => new TipoNegocioDetalleDto()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                TipoNegocioId = x.TipoNegocioId,
                TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                CartaOferta = x.CartaOferta,
                Confirma = x.Confirma,
                BoletoFisico = x.BoletoFisico,
            });
        }

        public List<BoletoCompraNetProvinciaDto> ListarBoletoCompraNetProvincia()
        {
            return repositorio.Listar<BoletoCompraNetProvincia, BoletoCompraNetProvinciaDto>(x => new BoletoCompraNetProvinciaDto()
            {
                Id = x.Id,
                BoletoCompraNetId = x.BoletoCompraNetId,
                ProvinciaId = x.ProvinciaId,
                BoletoDescripcion = x.BoletoCompraNet.Descripcion,
                ProvinciaNombre = x.Provincia.Nombre
            });
        }

        public List<BoletoCompraNetDto> ListarBoletoCompraNet()
        {
            return repositorio.Listar<BoletoCompraNet, BoletoCompraNetDto>(x => new BoletoCompraNetDto()
            {
                Id = x.Id,
                Descripcion = x.Descripcion
            });
        }

        public void ActualizarTipoNegocioDetalle(List<TipoNegocioDetalleDto> tipoDetalle)
        {
            var resultado = new Result();
            var tipos = repositorio.Listar<TipoNegocioDetalle>();
            foreach (var item in tipos)
            {
                item.BoletoFisico = tipoDetalle.Where(x => x.Id == item.Id).FirstOrDefault().BoletoFisico;
                item.Confirma = tipoDetalle.Where(x => x.Id == item.Id).FirstOrDefault().Confirma;
                item.CartaOferta = tipoDetalle.Where(x => x.Id == item.Id).FirstOrDefault().CartaOferta;
            }
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al GuardarCambios en ActualizarTipoNegocioDetalle.");
            }
        }

        public Resultado AgregarTipoNegocioDetalle(TipoNegocioDetalleDto tipoDetalle)
        {
            var resultado = ValidarTipoNegocioDetalle(tipoDetalle);
            if (!resultado.HayError)
            {
                repositorio.Agregar(new TipoNegocioDetalle()
                {
                    TipoNegocioId = tipoDetalle.TipoNegocioId,
                    Descripcion = tipoDetalle.Descripcion,
                });

                try
                {
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "AgregarTipoNegocioDetalle ");
                    resultado.Error("AgregarTipoNegocioDetalle", ex.Message);
                }
            }

            return resultado;
        }

        private Resultado ValidarTipoNegocioDetalle(TipoNegocioDetalleDto tipoDetalle)
        {
            var error = new Resultado();
            if (tipoDetalle.TipoNegocioId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Tipo de Negocio no debe estar vacío"));
            }
            if (string.IsNullOrEmpty(tipoDetalle.Descripcion))
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Descripcion no debe estar vacío"));
            }

            return error;
        }

        public Resultado AgregarBoletoCompraNetProvincia(BoletoCompraNetProvinciaDto boletoProvincia)
        {
            var resultado = new Resultado();
            if (boletoProvincia.BoletoCompraNetId == 0)
            {
                resultado.Errores.Add(new ErrorMessage("Se debe elegir un tipo de boleto."));
            }
            else
            {
                repositorio.Agregar(new BoletoCompraNetProvincia
                {
                    BoletoCompraNetId = boletoProvincia.BoletoCompraNetId,
                    ProvinciaId = boletoProvincia.ProvinciaId
                });
                try
                {
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    resultado.Error("AgregarBoletoCompraNetProvincia", ex.Message);
                }
            }
            return resultado;
        }

        public Resultado EliminarBoletoCompraNetProvincia(int id)
        {
            Resultado resultado = new Resultado();
            try
            {
                var boleto = repositorio.Obtener<BoletoCompraNetProvincia>(id);
                if (boleto != null)
                {
                    repositorio.Remover(boleto);
                    repositorio.GuardarCambios();
                }
                else
                {
                    resultado.Errores.Add(new ErrorMessage(400, "No existe un registro con ID " + id));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error en EliminarBoletoCompraNetProvincia ");
                resultado.Error("400", ex.Message);
            }
            return resultado;
        }

    }
}

