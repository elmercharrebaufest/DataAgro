using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;

namespace Molinos.DataAgro.Business
{

    public class HedgeManager : IHedgeManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IMailManager mailManager;
        private readonly IReportesManager reportesManager;

        public HedgeManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IReportesManager reportesManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.reportesManager = reportesManager;
        }

        public FinDelDiaDto Dia()
        {
            var hoy = DateTime.Now.Date;
            var dia = repositorio.ObtenerMayor<FinDelDia, DateTime, FinDelDiaDto>(x => DbFunctions.TruncateTime(x.Dia) == hoy, x => x.Dia, x => new FinDelDiaDto
            {
                Cerrado = x.Cerrado,
                Diferencial = x.Diferencial
            });

            return dia;
        }

        public List<HedgeMaterialDto> TraerTodosHedgeMaterial()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeMaterial, HedgeMaterialDto>(x => new HedgeMaterialDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                MaterialDesc = x.MaterialId == 1 ? "Hedge Maíz" : "Hedge Soja",
                TipoHedgeMaterialId = x.TipoHedgeMaterialId,
                TipoHedgeMaterialDesc = x.TipoHedgeMaterial.Descripcion,
                Cantidad = x.Cantidad,
                Fecha = x.Fecha,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => DbFunctions.TruncateTime(x.Fecha) == hoy, 0, "Fecha");
        }
        public List<HedgeObjetivoDto> TraerTodosHedgeObjetivo()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeObjetivo, HedgeObjetivoDto>(x => new HedgeObjetivoDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                MaterialDesc = x.Material.Descripcion,
                TipoObjetivoId = x.TipoObjetivoId,
                TipoHedgeMaterialDesc = x.TipoObjetivo.Descripcion,
                Cantidad = x.Cantidad,
                Fecha = x.Fecha,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => DbFunctions.TruncateTime(x.Fecha) == hoy);
        }
        public List<HedgeTCDto> TraerTodosHedgeTC()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeTC, HedgeTCDto>(x => new HedgeTCDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                TC = x.TipoCambio,
                HedgePesos = x.HedgePesos,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => DbFunctions.TruncateTime(x.Fecha) == hoy);
        }
        public Resultado GrabarHedgeMaterial(List<HedgeMaterial> hedgeMat, int comercialId)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var hoy = DateTime.Now.Date;
            var modificado = false;
            foreach (var hM in hedgeMat)
            {
                EntityValid.ValidateAll(hM, oEntityErrors);
                if (oEntityErrors.HayErrores)
                {
                    return oEntityErrors;
                }

                if (hM.Cantidad != 0)
                {
                    hM.ComercialId = comercialId;
                    hM.Fecha = DateTime.Now;
                    repositorio.Agregar(hM);
                    modificado = true;
                }
            }
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!modificado)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(400, "No hay datos para guardar"));
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }
        public Resultado GrabarHedgeObjetivo(List<HedgeObjetivo> hedgeMat, int comercialId)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var modificado = false;
            var hoy = DateTime.Now.Date;
            foreach (var hM in hedgeMat)
            {
                EntityValid.ValidateAll(hM, oEntityErrors);
                if (oEntityErrors.HayErrores)
                {
                    return oEntityErrors;
                }
                if (hM.Cantidad != 0)
                {
                    hM.ComercialId = comercialId;
                    hM.Fecha = DateTime.Now;
                    repositorio.Agregar(hM);
                    modificado = true;
                }

            }
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            //if (!modificado)
            //{
            //    oEntityErrors.Errores.Add(new ErrorMessage(400, "No hay datos para guardar"));
            //}
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }
        public Resultado GrabarHedgeTC(HedgeTC hedgeTC, int comercialId)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            if (hedgeTC.HedgePesos == 0 || hedgeTC.TipoCambio == 0)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(400, "El TC o Hedge $ no debe ser 0"));
                return oEntityErrors;
            }
            try
            {
                hedgeTC.ComercialId = comercialId;
                hedgeTC.Fecha = DateTime.Now;
                repositorio.Agregar(hedgeTC);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }
        public Resultado EliminarHedgeTC(int hedgeTCId)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var tc = repositorio.Obtener<HedgeTC>(x => x.Id == hedgeTCId);
            try
            {
                repositorio.Remover(tc);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se Eliminó Correctamente"));
            }
            return oEntityErrors;
        }
        public Resultado CerrarDia(int comercialId, byte[] archivo, string idActivedirectory, bool mail, string cuerpoMail, int diferencial)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var dia = new FinDelDia { Dia = DateTime.Now, Cerrado = true, ComercialId = comercialId, ReabrioComercialId = null, Diferencial = diferencial };
            try
            {
                repositorio.Agregar(dia);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            try
            {
                var hoy = DateTime.Now.Date;
                var finDia = repositorio.Obtener<FinDelDia>(x => x.Cerrado && DbFunctions.TruncateTime(x.Dia) == hoy);
                var contratos = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var fason = repositorio.Listar<Fason>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));

                contratos.ForEach(x => x.FinDelDiaId = finDia.Id);
                fijaciones.ForEach(x => x.FinDelDiaId = finDia.Id);
                fason.ForEach(x => x.FinDelDiaId = finDia.Id);

                if (mail)
                {
                    mailManager.EnviarMail(repositorio.Obtener<Comercial>(x => x.ComercialId == comercialId),
                                            repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso == PermisosDataAgro.MailHedge))),
                                                "Cierre del dia " + hoy.Day + "/" + hoy.Month,
                                                    string.Empty,
                                                        null,
                                                            AlternateView.CreateAlternateViewFromString(cuerpoMail, null, "text/html"),
                                                                archivo,
                                                                    "Cierre del dia.xls");
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }

            return oEntityErrors;
        }
        public Resultado ReabrirDia(int comercialId, double? diferencial)
        {
            var oEntityErrors = new Resultado();
            var dia = repositorio.Listar<FinDelDia>().OrderBy(x => x.Id).LastOrDefault();
            try
            {
                if (dia != null)
                {
                    dia.ReabrioComercialId = comercialId;
                    dia.Diferencial = diferencial;
                    dia.Cerrado = false;
                    repositorio.GuardarCambios();
                }
                else
                {
                    oEntityErrors.Errores.Add(new ErrorMessage(400, "El día ya se encuentra abierto"));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }

            return oEntityErrors;
        }
        public Resultado Diferencial()
        {
            var mensaje = new Resultado();
            var hoy = DateTime.Now.Date;
            var finDia = repositorio.Listar<FinDelDia>(x => DbFunctions.TruncateTime(x.Dia) == hoy, 0, "Id").LastOrDefault();

            if (finDia == null)
            {
                mensaje.Errores.Add(new ErrorMessage(1, "Se cerrara el día, enviándose un mail a Gerentes y Directivos."));
            }
            else if (finDia.Diferencial.HasValue)
            {
                var cantidad = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Cantidad) +
                repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Cantidad) +
                repositorio.Listar<Fason>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Cantidad);
                if (finDia.Diferencial.Value < cantidad)
                {
                    mensaje.Errores.Add(new ErrorMessage(1, "Se cerrara el día, enviándose un mail a Gerentes y Directivos. ¿Aceptar?"));
                }
                else
                {
                    mensaje.Errores.Add(new ErrorMessage(2, "Se cerrara el día. ¿Aceptar?"));
                }
            }
            else
            {
                mensaje.Errores.Add(new ErrorMessage(2, "Se cerrara el día. ¿Aceptar?"));
            }
            return mensaje;
        }
        private Resultado ValidarFinDelDia(Resultado res)
        {
            var dia = Dia();
            if (dia != null && dia.Cerrado.Value)
            {
                res.Errores.Add(new ErrorMessage(400, "El día se encuentra cerrado"));
            }
            return res;
        }
    }
}

