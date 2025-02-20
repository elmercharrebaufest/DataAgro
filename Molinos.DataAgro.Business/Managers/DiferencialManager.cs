using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class DiferencialManager : IDiferencialManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IMailManager mailManager;
        private readonly IHedgeManager hedgeManager;

        public DiferencialManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IHedgeManager hedgeManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.hedgeManager = hedgeManager;
        }

        public DiferencialDto TraerDiferencial()
        {
            var diferencial = repositorio.ObtenerMayor<Diferencial, int, DiferencialDto>(x => true, x => x.Id, x =>
                                          new DiferencialDto
                                          {
                                              Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                                              DiferencialDefault = x.DiferencialDefault,
                                              Id = x.Id,
                                              Fecha = x.Fecha,
                                              TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                                              TipoNegocioId = (int)x.TipoNegocioId
                                          });
            if (diferencial != null)
            {
                diferencial.historialDiferencial = TraerHistorial();
            }

            return diferencial;
        }

        public DiferencialDto TraerDiferencial(int tipoNegocioId)
        {
            var diferencial = repositorio.ObtenerMayor<Diferencial, int, DiferencialDto>(x => x.TipoNegocioId == tipoNegocioId, x => x.Id, x =>
                                          new DiferencialDto
                                          {
                                              Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                                              DiferencialDefault = x.DiferencialDefault,
                                              Id = x.Id,
                                              Fecha = x.Fecha,
                                              TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                                              TipoNegocioId = (int)x.TipoNegocioId
                                          });
            if (diferencial != null)
            {
                diferencial.historialDiferencial = TraerHistorial();
            }

            return diferencial;
        }

        public Resultado GrabarDiferencial(Diferencial diferencial)
        {
            var resultado = new Resultado();
            if (diferencial.DiferencialDefault == 0)
            {
                resultado.Errores.Add(new ErrorMessage(400, "El diferencial no puede ser cero"));
                return resultado;
            }
            var anterior = repositorio.ObtenerMayor<Diferencial, int>(x => x.TipoNegocioId == diferencial.TipoNegocioId, x => x.Id);
            if (anterior != null)
            {
                if (diferencial.DiferencialDefault == anterior.DiferencialDefault)
                {
                    resultado.Errores.Add(new ErrorMessage(400, "El diferencial no puede ser igual al activo"));
                    return resultado;
                }
            }
            repositorio.Agregar<Diferencial>(diferencial);
            repositorio.GuardarCambios();
            return resultado;
        }

        public Resultado EliminarDiferencial(int id)
        {
            var resultado = new Resultado();
            var diferencial = repositorio.Obtener<Diferencial>(id);
            repositorio.Remover(diferencial);
            repositorio.GuardarCambios();
            return resultado;
        }

        private List<DiferencialDto> TraerHistorial(int ultimosN = 20)
        {
            return repositorio.Listar<Diferencial, DiferencialDto>(x => new DiferencialDto
            {
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                DiferencialDefault = x.DiferencialDefault,
                Id = x.Id,
                Fecha = x.Fecha,
                TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId
            }, null, ultimosN, "Fecha", Entities.Helpers.DirOrden.Desc);
        }

        public Resultado ValidarComprasDiferencial(int comercialId)
        {
            try
            {
                if (hedgeManager.Dia() != null)
                {
                    logger.Debug("hedgeManager.Dia() " + hedgeManager.Dia().ToJson());
                    var cantidadAFijar = repositorio.Listar<Contrato>(
                        x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(DateTime.Now)
                        && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.TipoNegocioId == 1
                        && x.FinDelDia == null).ToList().Sum(x => x.Cantidad);

                    var cantidadAPrecio = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(DateTime.Now)
                        && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.TipoNegocioId == 2
                        && x.FinDelDia == null).ToList().Sum(x => x.Cantidad);

                    logger.Debug("cantidadAFijar " + cantidadAFijar);
                    logger.Debug("cantidadAPrecio " + cantidadAPrecio);
                    if (cantidadAFijar > (this.TraerDiferencial(1).DiferencialDefault * 1000))
                    {
                        logger.Debug("envia mail a fijar");
                        var cuerpo = hedgeManager.GenerarCuerpoMail("");
                        logger.Debug("GenerarCuerpoMail ");

                        mailManager.ReenviarMailCierreDia("Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            "Actualización Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            cuerpo);
                        logger.Debug("ya envio ");
                    }
                    else if (cantidadAPrecio > (this.TraerDiferencial(2).DiferencialDefault * 1000))
                    {
                        logger.Debug("envia mail a precio");
                        var cuerpo = hedgeManager.GenerarCuerpoMail("");
                        logger.Debug("GenerarCuerpoMail ");

                        mailManager.ReenviarMailCierreDia("Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            "Actualización Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            cuerpo);
                        logger.Debug("ya envio ");
                    }
                }
                return new Resultado();
            }
            catch (Exception e)
            {
                logger.Error("Error en validar compras diferencial", e);
                var resultado = new Resultado();
                resultado.Errores.Add(new ErrorMessage(400, "Error en validar compras diferencial"));
                return resultado;

            }
        }
    }
}