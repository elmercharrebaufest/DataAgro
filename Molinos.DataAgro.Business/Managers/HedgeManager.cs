using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class HedgeManager : IHedgeManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public HedgeManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public bool Dia()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Existe<FinDelDia>(x => DbFunctions.TruncateTime(x.Dia) == hoy && x.Cerrado);
        }
        public List<HedgeMaterialDto> TraerTodosHedgeMaterial()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeMaterial, HedgeMaterialDto>(x => new HedgeMaterialDto
            {
                MaterialId = x.MaterialId,
                TipoHedgeMaterialId = x.TipoHedgeMaterialId,
                Cantidad = x.Cantidad
            }, x=> DbFunctions.TruncateTime(x.Fecha) == hoy);
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
            foreach (var hM in hedgeMat)
            {
                if (hM.Cantidad > 0)
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
                        }
                    }
                    else
                    {
                        hM.ComercialId = comercialId;
                        hM.Fecha = DateTime.Now;
                        repositorio.Agregar(hM);
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
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se Guardo Correctamente"));
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
            var hoy = DateTime.Now.Date;
            foreach (var hM in hedgeMat)
            {
                if (hM.Cantidad > 0)
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
                        }
                    }
                    else
                    {
                        hM.ComercialId = comercialId;
                        hM.Fecha = DateTime.Now;
                        repositorio.Agregar(hM);
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
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se Guardo Correctamente"));
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
                oEntityErrors.Errores.Add(new ErrorMessage(400, "El TC o $ no debe ser 0"));
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
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se Guardo Correctamente"));
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
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se Guardo Correctamente"));
            }
            return oEntityErrors;
        }
        public Resultado CerrarDia(int comercialId)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var dia = new FinDelDia { Dia = DateTime.Now, Cerrado = true,ComercialId = comercialId };
            try
            {
                repositorio.Agregar(dia);
                repositorio.GuardarCambios();
            } 
            catch(Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source,ex.Message);
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

