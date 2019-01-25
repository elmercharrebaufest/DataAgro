using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Molinos.DataAgro.Business
{

    public class HedgeManager : IHedgeManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IMailManager mailManager;

        public HedgeManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
        }

        public bool Dia()
        {
            var hoy = DateTime.Now.Date;
            var dia = repositorio.Listar<FinDelDia>(x => DbFunctions.TruncateTime(x.Dia) == hoy && x.Cerrado, 0, "Id").LastOrDefault();
            var cerrado = false;
            if (dia != null)
            {
                cerrado = dia.Cerrado;
            }
            return cerrado;
        }

        public List<HedgeMaterialDto> TraerTodosHedgeMaterial()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeMaterial, HedgeMaterialDto>(x => new HedgeMaterialDto
            {
                MaterialId = x.MaterialId,
                TipoHedgeMaterialId = x.TipoHedgeMaterialId,
                Cantidad = x.Cantidad
            }, x => DbFunctions.TruncateTime(x.Fecha) == hoy);
        }
        public List<HedgeObjetivoDto> TraerTodosHedgeObjetivo()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeObjetivo, HedgeObjetivoDto>(x => new HedgeObjetivoDto
            {
                MaterialId = x.MaterialId,
                TipoObjetivoId = x.TipoObjetivoId,
                Cantidad = x.Cantidad
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
                HedgePesos = x.HedgePesos
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

                var hedgeMatSave = repositorio.Obtener<HedgeMaterial>(x => x.MaterialId == hM.MaterialId && x.TipoHedgeMaterialId == hM.TipoHedgeMaterialId && DbFunctions.TruncateTime(x.Fecha) == hoy);
                if (hedgeMatSave != null)
                {
                    if (hedgeMatSave.Cantidad != hM.Cantidad)
                    {
                        hedgeMatSave.MaterialId = hM.MaterialId;
                        hedgeMatSave.TipoHedgeMaterialId = hM.TipoHedgeMaterialId;
                        hedgeMatSave.Cantidad = hM.Cantidad;
                        hedgeMatSave.ComercialId = comercialId;
                        hedgeMatSave.Fecha = DateTime.Now;
                        modificado = true;
                    }
                }
                else
                {
                    if (hM.Cantidad > 0)
                    {
                        hM.ComercialId = comercialId;
                        hM.Fecha = DateTime.Now;
                        repositorio.Agregar(hM);
                        modificado = true;
                    }
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

                var hedgeMatSave = repositorio.Obtener<HedgeObjetivo>(x => x.MaterialId == hM.MaterialId && x.TipoObjetivoId == hM.TipoObjetivoId && DbFunctions.TruncateTime(x.Fecha) == hoy);
                if (hedgeMatSave != null)
                {
                    if (hedgeMatSave.Cantidad != hM.Cantidad)
                    {
                        hedgeMatSave.MaterialId = hM.MaterialId;
                        hedgeMatSave.TipoObjetivoId = hM.TipoObjetivoId;
                        hedgeMatSave.Cantidad = hM.Cantidad;
                        hedgeMatSave.ComercialId = comercialId;
                        hedgeMatSave.Fecha = DateTime.Now;
                        modificado = true;
                    }
                }
                else
                {
                    if (hM.Cantidad > 0)
                    {
                        hM.ComercialId = comercialId;
                        hM.Fecha = DateTime.Now;
                        repositorio.Agregar(hM);
                        modificado = true;
                    }
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
        public Resultado CerrarDia(int comercialId, byte[] archivo, string idActivedirectory)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var dia = new FinDelDia { Dia = DateTime.Now, Cerrado = true, ComercialId = comercialId, ReabrioComercialId = null };
            try
            {
                
                repositorio.Agregar(dia);
                repositorio.GuardarCambios();
                mailManager.EnviarMail(repositorio.Obtener<Comercial>(x => x.ComercialId == comercialId), new List<string>(), "Cierre del dia", string.Empty, null, null, archivo, "Cierre del dia.xls");
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
                var contratos = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null &&(x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var fason = repositorio.Listar<Fason>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));

                contratos.ForEach(x => x.FinDelDiaId = finDia.Id);
                fijaciones.ForEach(x => x.FinDelDiaId = finDia.Id);
                fason.ForEach(x => x.FinDelDiaId = finDia.Id);

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
        public Resultado ReabrirDia(int comercialId)
        {
            var oEntityErrors = new Resultado();
            var dia = repositorio.Listar<FinDelDia>().OrderBy(x => x.Id).LastOrDefault();
            try
            {
                if (dia != null && dia.Cerrado)
                {
                    dia.ReabrioComercialId = comercialId;
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
        private Resultado ValidarFinDelDia(Resultado res)
        {
            if (Dia())
            {
                res.Errores.Add(new ErrorMessage(400, "El día ya ha finalizado"));
            }
            return res;
        }

    }
}

