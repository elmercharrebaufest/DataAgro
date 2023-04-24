using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;

namespace Molinos.DataAgro.Business
{
    public class HedgeManager : IHedgeManager
    {
        private readonly ILogger logger;
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
        public List<HedgeMargenMoliendaDto> TraerTodosHedgeMargenMolienda()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<HedgeMargenMolienda, HedgeMargenMoliendaDto>(x => new HedgeMargenMoliendaDto
            {
                Id = x.Id,
                MargenMolienda = x.MargenMolienda,
                Fecha = x.Fecha,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => DbFunctions.TruncateTime(x.Fecha) == hoy).OrderByDescending(a => a.Fecha).ToList();
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
            //var modificado = false;
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
                    //modificado = true;
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
        public Resultado GrabarHedgeMargenMolienda(HedgeMargenMolienda hedgeMargen, int comercialId)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = ValidarFinDelDia(oEntityErrors);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            if (hedgeMargen.MargenMolienda == 0)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(400, "El margen de molienda no se puede guardar en cero."));
                return oEntityErrors;
            }
            else if (hedgeMargen.MargenMolienda > 99 || hedgeMargen.MargenMolienda < -99)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(400, "El margen de molienda debe estar entre -99 y 99."));
                return oEntityErrors;
            }
            try
            {
                hedgeMargen.ComercialId = comercialId;
                hedgeMargen.Fecha = DateTime.Now;
                repositorio.Agregar(hedgeMargen);
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
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
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
                    EnviarMail(comercialId, hoy, cuerpoMail, archivo);
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
        public void EnviarMail(int comercialId, DateTime hoy, string cuerpoMail, byte[] archivo)
        {
            var asunto = "Cierre del día " + hoy.Day + "/" + hoy.Month;
            List<string> to = new List<string>();
            var comerciales = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailHedge)));
            foreach (var comercial in comerciales)
            {
                string mail = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
                if (!string.IsNullOrEmpty(mail))
                    to.Add(mail);
            }
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                var externos = repositorio.Listar<CierreDelDiaMailExternos, string>(x => x.Mail);
                to.AddRange(externos);
            }
            else
            {
                asunto = "Mail Prueba - " + asunto;
                to.Add("baufestdataagro@outlook.com");
            }

            mailManager.EnviarMail(to, asunto, string.Empty, null,
                               AlternateView.CreateAlternateViewFromString(cuerpoMail, null, "text/html"),
                               archivo,
                               "Cierre del dia " + hoy.Day + "-" + hoy.Month + ".xls");
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
                    oEntityErrors.Errores.Add(new ErrorMessage(400, "El día ya se encuentra abierto."));
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
                mensaje.Errores.Add(new ErrorMessage(1, "Se cerrará el día, enviándose un mail a Gerentes y Directivos."));
            }
            else if (finDia.Diferencial.HasValue)
            {
                var cantidad = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Cantidad) +
                repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Cantidad) +
                repositorio.Listar<Fason>(x => DbFunctions.TruncateTime(x.Fecha) == hoy && x.FinDelDiaId == null && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Cantidad);
                if (finDia.Diferencial.Value < cantidad)
                {
                    mensaje.Errores.Add(new ErrorMessage(1, "Se cerrará el día, enviándose un mail a Gerentes y Directivos."));
                }
                else
                {
                    mensaje.Errores.Add(new ErrorMessage(2, "Se cerrará el día. ¿Aceptar?"));
                }
            }
            else
            {
                mensaje.Errores.Add(new ErrorMessage(2, "Se cerrará el día. ¿Aceptar?"));
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
        private ReporteCompraNetModel ObtenerDatosReporte(DateTime fechaDesde, DateTime fechaHasta, string centroId)
        {
            int idCentro = int.Parse(centroId);
            bool filtrarAcopio = idCentro == 0 || idCentro == 1;
            var agentes = filtrarAcopio ? reportesManager.TraerAgenteDeCompra(fechaDesde, fechaHasta, null) : new List<AgenteCompraDto>();
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            agentes.ForEach(x => x.Operador.ForEach(y => y.Cantidad = y.Cantidad));

            var objetivos = reportesManager.TraerHedgeObjetivo(fechaDesde, fechaHasta, null);
            objetivos.PricingCumplido = objetivos.PricingCumplido;
            objetivos.PricingObjetivo = objetivos.PricingObjetivo;
            objetivos.RemitirCumplido = objetivos.RemitirCumplido;
            objetivos.RemitirObjetivo = objetivos.RemitirObjetivo;
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = reportesManager.TraerToneladasGranoTipo(fechaDesde, fechaHasta, null, idCentro),
                SojaSustentable = reportesManager.TraerToneladasSojaSust(fechaDesde, fechaHasta, idCentro),
                PosicionCompras = reportesManager.TraerPosicionCompras(fechaDesde, fechaHasta, null, idCentro),
                PricingCampania = reportesManager.TraerPricingCampania(fechaDesde, fechaHasta, null, idCentro),
                PrecioCantidad = reportesManager.TraerMonedaCantidad(fechaDesde, fechaHasta, null, idCentro),
                HedgeMaterial = TransformarAModelHedge(reportesManager.TraerTodosHedgeMaterial(fechaDesde, fechaHasta, null)),
                HedgeObjetivo = objetivos,
                TCPromedioDto = reportesManager.TraerTcPromedio(fechaDesde, fechaHasta, null),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
                SojaEPA = reportesManager.TraerToneladasSojaEPA(fechaDesde, fechaHasta, idCentro),
            };
        }
        private List<HedgeMaterialModel> TransformarAModelHedge(List<HedgeMaterialDto> hedgeMat)
        {
            var lista = new List<HedgeMaterialModel>()
            {
                new HedgeMaterialModel {MaterialId = 1, MaterialDescripcion ="Hedge Maíz",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad)},
                new HedgeMaterialModel {MaterialId = 3, MaterialDescripcion ="Hedge Soja",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad) }
            };
            return lista;
        }
        public ReporteCompraNetModel ObtenerDatosReporte()
        {
            var hoy = DateTime.Now.Date;
            var agentes = reportesManager.TraerAgenteDeCompra(hoy, hoy, null);
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = reportesManager.TraerToneladasGranoTipo(hoy, hoy, null),
                SojaSustentable = reportesManager.TraerToneladasSojaSust(hoy, hoy),
                PosicionCompras = reportesManager.TraerPosicionCompras(hoy, hoy, null),
                PrecioCantidad = reportesManager.TraerMonedaCantidad(hoy, hoy, null),
                HedgeMaterial = TransformarAModelHedge(reportesManager.TraerTodosHedgeMaterial(hoy, hoy, null)),
                HedgeObjetivo = reportesManager.TraerHedgeObjetivo(hoy, hoy, null),
                TCPromedioDto = reportesManager.TraerTcPromedio(hoy, hoy, null),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
                SojaEPA = reportesManager.TraerToneladasSojaEPA(hoy, hoy),
            };
        }
        public string GenerarCuerpoMail(string observaciones)
        {
            var hoy = DateTime.Now.Date;
            var Model = this.ObtenerDatosReporte(hoy, hoy, "0");
            var dispAFijar = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.DispAFijar > 0));
            var dispAPrecio = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.DispAPrecio > 0));
            var dispFijacion = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.DispFijac > 0));
            var dispAgente = Model.ToneladasGranoTipo.Any(x => x.DispAgente != 0);
            var disp = 4 - (dispAFijar ? 0 : 1) - (dispAPrecio ? 0 : 1) - (dispFijacion ? 0 : 1) - (dispAgente ? 0 : 1);
            var forwAFijar = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.FrwAFijar > 0));
            var forwAPrecio = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.FrwAPrecio > 0));
            var forwFijacion = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.FrwFijac > 0));
            var forwAgente = Model.ToneladasGranoTipo.Any(x => x.FrwAgente != 0);
            var forw = 4 - (forwAFijar ? 0 : 1) - (forwAPrecio ? 0 : 1) - (forwFijacion ? 0 : 1) - (forwAgente ? 0 : 1);
            var newcAFijar = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.NewAFijar > 0));
            var newcAPrecio = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.NewAPrecio > 0));
            var newcFijacion = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.NewFijac > 0));
            var newcAgente = Model.ToneladasGranoTipo.Any(x => x.NewAgente != 0);
            var newc = 4 - (newcAFijar ? 0 : 1) - (newcAPrecio ? 0 : 1) - (newcFijacion ? 0 : 1) - (newcAgente ? 0 : 1);
            var hedgeMat = Model.HedgeMaterial.Any(x => x.Disponible != 0 || x.Forward != 0 || x.NewCrop != 0);
            var hedgeObj = Model.HedgeObjetivo.RemitirObjetivo != 0 && Model.HedgeObjetivo.PricingObjetivo != 0 ? 1 : 0;
            var pricing = Model.PricingCampania.Count != 0;
            var margenMolienda = TraerTodosHedgeMargenMolienda().FirstOrDefault();

            var htmlBody = "";

            htmlBody += "Estimados,";
            htmlBody += "<br></br>";
            htmlBody += "A continuación se detallan las compras correspondientes al cierre del día.";
            htmlBody += "<br></br>";
            htmlBody += "Margen de Molienda: " + (margenMolienda == null ? "No especificado." : margenMolienda.MargenMolienda.ToString());
            htmlBody += "<br />";
            htmlBody += "Observaciones: " + (String.IsNullOrEmpty(observaciones) ? "Sin observaciones." : observaciones);
            htmlBody += "<br></br>";

            if (pricing)
            {
                var p = new List<PricingCampaniaDto>();
                foreach (var item in Model.PricingCampania)
                {
                    switch (item.Material)
                    {
                        case "Soja": item.Orden = 1; break;
                        case "Maiz": item.Orden = 2; break;
                        case "Trigo": item.Orden = 3; break;
                        case "Girasol": item.Orden = 4; break;
                        case "Girasol AO": item.Orden = 5; break;
                    }
                    p.Add(item);
                }
                htmlBody += @"<table class='pricing' style='font-family: Arial, Helvetica, sans-serif; border-bottom: 2px solid #070707;border-top: 2px solid #070707; border-left: 0px ; border-right: none !important; background-color: #FFFFFF;width: 350px;height: 200px;text-align: center;border-collapse: collapse; width:700px;'>
                    <thead style='background: #C71585;border-bottom: 1px solid #C71585;'>
                    <tr style= 'font-size: 12px;font-weight: bold;color: #FFFFFF;text-align: center; border-left: 0px;border-right: 0px !important; border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>
                        <th style= 'border-left: 0px ;border-right: 0px !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>PRICING</th>
                        <th style= 'border-left: 0px;border-right: 0px !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>CAMPAÑA</th>";
                htmlBody += "<th style= 'border-left: 0px; border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>SL</th>";
                htmlBody += "<th style= 'border-left: 0px;border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>Acopios</th>";
                htmlBody += "<th style= 'border-left: 0px;border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>BB</th>";
                htmlBody += "<th style= 'border-left: 0px;border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'> TOTALES </th> ";
                htmlBody += "</tr>";
                htmlBody += "</thead>";
                htmlBody += "<tbody>";
                var par = 0;
                foreach (var datos in p.OrderBy(x => x.Orden).ToList())
                {
                    par += 1;
                    string sumaPricing;
                    switch (datos.Id)
                    {
                        case 11:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 1).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Maiz").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 12:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 1).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Maiz").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 21:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Trigo" /*x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad"*/).Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 22:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Trigo" /*x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad"*/).Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 31:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 3).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Soja").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 32:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 3).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Soja").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 41:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 4).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 42:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 4).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 51:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 5).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol Alto Oleico").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 52:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 5).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol Alto Oleico").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        default:
                            sumaPricing = "0";
                            break;
                    }
                    var estilo = "style='font-size: 15px;border-left: none !important; border-right: 0px !important; background: #DDDDDD;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'";
                    if ((par % 2) == 0)
                    {
                        estilo = "style='font-size: 15px;border-left: none !important; border-right: 0px !important; background: #FFFFF;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'";
                    }
                    htmlBody += @"<tr " + estilo + @">
                            <td style= 'font-size: 15px; border-left: none !important; border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;' >" + datos.Material + @"</td>
                            <td style= 'font-size: 15px; border-left: none !important; border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>" + datos.Campania + @"</td>";

                    htmlBody += $"<td style= 'border-left: none !important;border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'>" + (datos.SanLorenzo == 0 ? "" : datos.SanLorenzo.ToString("N0")) + "</td>";
                    htmlBody += $"<td style= 'border-left: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;border-right: none !important;'>" + (datos.Acopio == 0 ? "" : datos.Acopio.ToString("N0")) + "</td>";
                    htmlBody += $"<td style= 'border-left: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;border-right: none !important;'>" + (datos.BahiaBlanca == 0 ? "" : datos.BahiaBlanca.ToString("N0")) + "</td>";
                    htmlBody += $"<td style='font-size: 15px;border-left: none !important;border-right: none !important;border-top: 1px solid #000000;border-bottom: 1px solid #000000;padding: 4px 4px;'> " + sumaPricing + "</td>";
                    htmlBody += "</tr>";

                }
                htmlBody += "</tbody></table> <br><br>";
            }
            var estiloTdTrVerde = "border: 1px solid #000000; padding: 4px 10px; border-top: solid black 1.0pt !important; border-left: solid black 1.0pt !important; border-right: solid black 1.0pt !important;";
            var head = "font-size: 12px; font-weight: bold;color: #FFFFFF;text-align: left; background: #017940;border-bottom: 0px solid #444444; text-align:center;  border-left: solid black 1.0pt !important; border-right: solid black 1.0pt !important; border-top: solid black 1.0pt !important;";
            var tabla = "font-family: Arial, Helvetica, sans-serif;border: 2px solid #000000;background-color: #FFFFFF;height: 200px;text-align: center;border-collapse: collapse; width: 100%; ";
            if (disp + forw + newc != 0)
            {
                htmlBody += $@" <table class='verde' style='{tabla}'><thead>
                    <tr style='{estiloTdTrVerde} {head}'><td style='{estiloTdTrVerde} {head}'></td>";
                htmlBody += (disp > 0) ? $"<td style='" + estiloTdTrVerde + head + " text-aligne:center;' id='disponible'  colspan='" + disp + "'>DISPONIBLE</td>" +
                    $"<td style='{estiloTdTrVerde} {head}' id='disponibleTotal'  rowspan='2' colspan=''>TOTAL DISPONIBLE</td>" : "";
                htmlBody += (forw > 0) ? $"<td style='{estiloTdTrVerde} {head}' id='forward' colspan='" + forw + "'>FORWARD</td>" +
                         $"<td style='{estiloTdTrVerde} {head}' id='forwardTotal' rowspan='2' colspan=''>TOTAL FORWARD</td>" : "";
                htmlBody += (newc > 0) ? $"<td style='style='{estiloTdTrVerde} {head}'text-aligne: center;' colspan='" + newc + "'>NEW CROP</td>" +
                         $"<td style='{estiloTdTrVerde} {head}' id='newCropTotal' rowspan='2'>TOTAL NEW CROP</td>" : "";
                htmlBody += "</tr>";

                htmlBody += $"<tr style='{estiloTdTrVerde} {head}' class='titulos'>";
                htmlBody += $"<td style='{estiloTdTrVerde} {head}'>PRODUCTO</td>";
                htmlBody += dispAFijar ? $"<td style='{estiloTdTrVerde} {head}'class='valores disp'>A Fijar</td>" : "";
                htmlBody += dispAPrecio ? $"<td style='{estiloTdTrVerde} {head}' class='valores disp'>A Precio</td>" : "";
                htmlBody += dispFijacion ? $"<td style='{estiloTdTrVerde} {head}' class='valores disp'>Fijación</td>" : "";
                htmlBody += dispAgente ? $"<td style='{estiloTdTrVerde} {head}' class='valores disp'>MAT</td>" : "";
                //htmlBody += disp > 0 ?  $"<td class=''></td>" : "";
                htmlBody += forwAFijar ? $"<td style='{estiloTdTrVerde} {head}' class='valores forw'>A Fijar</td>" : "";
                htmlBody += forwAPrecio ? $"<td style='{estiloTdTrVerde} {head}' class='valores forw'>A Precio</td>" : "";
                htmlBody += forwFijacion ? $"<td style='{estiloTdTrVerde} {head}'class='valores forw'>Fijación</td>" : "";
                htmlBody += forwAgente ? $"<td style='{estiloTdTrVerde} {head}' class='valores forw'>MAT</td>" : "";
                //htmlBody += forw > 0 ?  $"<td class=''> </td>" : "";
                htmlBody += newcAFijar ? $"<td style='{estiloTdTrVerde} {head}'class='valores newc'>A Fijar</td>" : "";
                htmlBody += newcAPrecio ? $"<td style='{estiloTdTrVerde} {head}'class='valores newc'>A Precio</td>" : "";
                htmlBody += newcFijacion ? $"<td style='{estiloTdTrVerde} {head}' class='valores newc'>Fijación</td>" : "";
                htmlBody += newcAgente ? $"<td style='{estiloTdTrVerde} {head}' class='valores newc'>MAT</td>" : "";

                htmlBody += "</tr> </thead>";
                htmlBody += "<tbody>";
                var toneladas = new List<ToneladasGranoTipoDto>();
                foreach (var item in Model.ToneladasGranoTipo)
                {
                    switch (item.Material)
                    {
                        case "Soja": item.Orden = 1; break;
                        case "Maiz": item.Orden = 2; break;
                        case "Trigo": item.Orden = 3; break;
                        case "Trigo Grado 2": item.Orden = 4; break;
                        case "Trigo Calidad": item.Orden = 5; break;
                        case "Girasol": item.Orden = 6; break;
                        case "Girasol Alto Oleico": item.Orden = 7; break;
                    }
                    toneladas.Add(item);
                }
                foreach (var toneladaPrecio in toneladas.OrderBy(x => x.Orden).ToList())
                {
                    var posicion = Model.PosicionCompras.Where(x => x.Material.ToLower() == toneladaPrecio.Material.ToLower());
                    var totalDisp = posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac + y.DispAFijar + y.DispAPrecio)).Sum() + toneladaPrecio.DispAgente;
                    var totalForw = posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar + y.FrwAPrecio + y.FrwFijac)).Sum() + toneladaPrecio.FrwAgente;
                    var totalNewC = posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar + y.NewAPrecio + y.NewFijac)).Sum() + toneladaPrecio.NewAgente;

                    if (totalDisp != 0 || totalForw != 0 || totalNewC != 0)
                    {
                        htmlBody += $@"<tr  style='{estiloTdTrVerde} '>";
                        htmlBody += $"<td style='{estiloTdTrVerde} '>" + toneladaPrecio.Material + "</td>";
                        htmlBody += dispAFijar ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispAPrecio ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispFijacion ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac)).Sum().ToString("N0")) + " </td>" : "";
                        htmlBody += dispAgente ? $"<td style='{estiloTdTrVerde} ' class=''>" + toneladaPrecio.DispAgente.ToString("N0") + " </td>" : "";
                        htmlBody += disp > 0 ? $"<td style='{estiloTdTrVerde} '>" + totalDisp.ToString("N0") + " </td>" : "";

                        //htmlBody += disp > 0 ?  $"<td style='{estiloTdTrVerde} ' class=''></td>" : "";
                        htmlBody += forwAFijar ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAPrecio ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwFijacion ? $"<td style='{estiloTdTrVerde} 'class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAgente ? $"<td style='{estiloTdTrVerde} ' class=''>" + toneladaPrecio.FrwAgente.ToString("N0") + "</td>" : "";
                        htmlBody += forw > 0 ? $"<td style='{estiloTdTrVerde} '>" + totalForw.ToString("N0") + "</td>" : "";

                        //htmlBody += forw > 0 ?  $"<td style='{estiloTdTrVerde} ' class=' '> </td>" : "";
                        htmlBody += newcAFijar ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAPrecio ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcFijacion ? $"<td style='{estiloTdTrVerde} ' class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAgente ? $"<td style='{estiloTdTrVerde} ' class=''>" + toneladaPrecio.NewAgente.ToString("N0") + " </td>" : "";
                        htmlBody += newc > 0 ? $"<td style='{estiloTdTrVerde} '>" + totalNewC.ToString("N0") + " </td>" : "";
                        htmlBody += "</tr>";
                    }
                }
                htmlBody += " </tbody>";
                htmlBody += "</table><br><br>";
            }

            var tablaMaterial = "";
            var td = "";
            var bodyTdM = "";
            var bodyTdFoot = "";
            var theadTr = "";
            var color = "";

            foreach (var material in Model.PosicionCompras)
            {

                dispAFijar = material.PosicionKilos.Any(x => x.DispAFijar > 0);
                dispAPrecio = material.PosicionKilos.Any(x => x.DispAPrecio > 0);
                dispFijacion = material.PosicionKilos.Any(x => x.DispFijac > 0);
                forwAFijar = material.PosicionKilos.Any(x => x.FrwAFijar > 0);
                forwAPrecio = material.PosicionKilos.Any(x => x.FrwAPrecio > 0);
                forwFijacion = material.PosicionKilos.Any(x => x.FrwFijac > 0);
                newcAFijar = material.PosicionKilos.Any(x => x.NewAFijar > 0);
                newcAPrecio = material.PosicionKilos.Any(x => x.NewAPrecio > 0);
                newcFijacion = material.PosicionKilos.Any(x => x.NewFijac > 0);
                var totalPesos = material.PosicionKilos.Sum(y => y.KilosPesos);
                var totalDolares = material.PosicionKilos.Sum(y => y.KilosDolares);
                var fix = 3 - (dispFijacion ? 0 : 1) - (forwFijacion ? 0 : 1) - (newcFijacion ? 0 : 1);
                var aFijar = 3 - (dispAFijar ? 0 : 1) - (forwAFijar ? 0 : 1) - (newcAFijar ? 0 : 1);
                var aPrecio = 3 - (dispAPrecio ? 0 : 1) - (forwAPrecio ? 0 : 1) - (newcAPrecio ? 0 : 1);
                var suma = fix + aFijar + aPrecio + (totalPesos != 0 ? 1 : 0) + (totalDolares != 0 ? 1 : 0);
                var pondPesos = material.PosicionKilos.Any(x => x.PrecioPonderadoPesos > 0) ? 1 : 0;
                var pondDolares = material.PosicionKilos.Any(x => x.PrecioPonderadoDolares > 0) ? 1 : 0;
                var pond = pondPesos + pondDolares;


                var materialDesc = material.Material.Replace(" ", "").ToLower();
                switch (materialDesc)
                {
                    case "soja": color = "background: #99CC00"; break;
                    case "maiz": color = "background: #ffcc99"; break;
                    case "trigocalidad": color = "background: #99ccff"; break;
                    case "trigogrado2": color = "background: #6ae6be"; break;
                    case "trigocámara": color = "background: #9999FF"; break;
                    case "girasol": color = "background: #d360d4"; break;
                    case "girasolaltooleico": color = "background: #f4c1f7"; break;

                }

                tablaMaterial = "font-family: Arial, Helvetica, sans-serif; width: 100%; text-align: center; border-collapse: collapse;";
                td = "border: 1px solid #000000; padding: 4px 4px;";
                bodyTdM = "font-size: 15px; color: #000000;background: #FFF; border: 1px solid #000000;";
                bodyTdFoot = $"font-size: 15px; color: #000000; {color} ;font-weight: bold; border: 1px solid #000000;";
                theadTr = $"font-size: 15px;font-weight: bold;color: #050505;text-align: center;border-left: 0px; {color} ;border-bottom: 1px solid #000000;";

                if (suma > 0)
                {

                    htmlBody += $@"<table style='{tablaMaterial}' id='" + material.Material.Replace(" ", "") + @"' class='" + material.Material.Replace(" ", "").ToLower() + $@"'>
                     <thead>
                        <tr style='{theadTr}'><td style='{td}' colspan = '" + ((suma + pond) + 1) + "'> " + material.Material.ToUpper() + $@" </td></tr>
                        <tr style='{theadTr}'>
                            <td style='{td}' rowspan = '2'> Posición </td>";
                    htmlBody += fix > 0 ? $"<td style='{td}' colspan='" + fix + "'>Fix</td>" : "";
                    htmlBody += aFijar > 0 ? $"<td style='{td}' colspan = '" + aFijar + "'>A Fijar</td>" : "";
                    htmlBody += aPrecio > 0 ? $"<td style='{td}' colspan = '" + aPrecio + "'>A Precio</td>" : "";
                    htmlBody += totalPesos != 0 ? $"<td style='{td}' rowspan = '2'> Ton. $</td>" : "";
                    htmlBody += pondPesos > 0 ? $"<td style='{td}' rowspan = '2'>Precio $</td>" : "";
                    htmlBody += totalDolares != 0 ? $"<td style='{td}' rowspan = '2' class=''>Ton.USD</td>" : "";
                    htmlBody += pondDolares > 0 ? $"<td style='{td}' rowspan = '2'>Precio USD</td>" : "";

                    htmlBody += "</tr>";
                    htmlBody += $"<tr style='{theadTr}' class='titulosPosicion'>";
                    htmlBody += dispFijacion ? $"<td style='{td}'>Disponible</td>" : "";
                    htmlBody += forwFijacion ? $"<td style='{td}'>Forward</td>" : "";
                    htmlBody += newcFijacion ? $"<td style='{td}'>New Crop</td>" : "";
                    htmlBody += dispAFijar ? $"<td style='{td}'>Disponible</td>" : "";
                    htmlBody += forwAFijar ? $"<td style='{td}'>Forward</td>" : "";
                    htmlBody += newcAFijar ? $"<td style='{td}'>New Crop</td>" : "";
                    htmlBody += dispAPrecio ? $"<td style='{td}'>Disponible</td>" : "";
                    htmlBody += forwAPrecio ? $"<td style='{td}'>Forward</td>" : "";
                    htmlBody += newcAPrecio ? $"<td style='{td}'>New Crop</td>" : "";
                    htmlBody += "</tr></thead><tbody>";

                    foreach (var mes in material.PosicionKilos.OrderBy(x => x.Anio).ThenBy(x => x.Mes))
                    {
                        int mesActual = (int)((EnumMeses)Enum.Parse(typeof(EnumMeses), mes.Mes.ToString()));
                        htmlBody += $@"<tr>
                        <td style='{bodyTdM}'> " + mes.Mes + " - " + mes.Anio + "</td>";
                        htmlBody += dispFijacion ? $"<td style='{bodyTdM}'>" + mes.DispFijac.ToString("N0") + "</td>" : "";
                        htmlBody += forwFijacion ? $"<td  style='{bodyTdM}'>" + mes.FrwFijac.ToString("N0") + "</td>" : "";
                        htmlBody += newcFijacion ? $"<td  style='{bodyTdM}'>" + mes.NewFijac.ToString("N0") + "</td>" : "";
                        htmlBody += dispAFijar ? $"<td  style='{bodyTdM}'>" + mes.DispAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += forwAFijar ? $"<td  style='{bodyTdM}'>" + mes.FrwAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += newcAFijar ? $"<td  style='{bodyTdM}'>" + mes.NewAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += dispAPrecio ? $"<td  style='{bodyTdM}'>" + mes.DispAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += forwAPrecio ? $"<td  style='{bodyTdM}'>" + mes.FrwAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += newcAPrecio ? $"<td  style='{bodyTdM}'>" + mes.NewAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += totalPesos != 0 ? $"<td  style='{bodyTdM}'> " + (mes.KilosPesos.ToString("N0")) + "</td>" : "";
                        htmlBody += pondPesos > 0 ? $"<td  style='{bodyTdM}'> " + (mes.PrecioPonderadoPesos.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += totalDolares != 0 ? $"<td  style='{bodyTdM}'> " + (mes.KilosDolares.ToString("N0")) + "</td>" : "";
                        htmlBody += pondDolares > 0 ? $"<td  style='{bodyTdM}'> " + (mes.PrecioPonderadoDolares.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += "</tr>";
                    }
                    htmlBody += @"<tr class='m" + material.Material.Replace(" ", "").ToLower() + $@"'>
                        <td style='{bodyTdFoot}'>Total</td>";
                    htmlBody += dispFijacion ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.DispFijac).ToString("N0") + "</td>" : "";
                    htmlBody += forwFijacion ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.FrwFijac).ToString("N0") + "</td>" : "";
                    htmlBody += newcFijacion ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.NewFijac).ToString("N0") + "</td>" : "";
                    htmlBody += dispAFijar ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.DispAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += forwAFijar ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.FrwAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += newcAFijar ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.NewAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += dispAPrecio ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.DispAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += forwAPrecio ? $"<td  style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.FrwAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += newcAPrecio ? $"<td style='{bodyTdFoot}'>" + material.PosicionKilos.Sum(y => y.NewAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += totalPesos != 0 ? $"<td  style='{bodyTdFoot}'>" + totalPesos.ToString("N0") + "</td>" : "";
                    htmlBody += pondPesos > 0 ? $"<td style='{bodyTdFoot}'></td>" : "";
                    htmlBody += totalDolares != 0 ? $"<td  style='{bodyTdFoot}'>" + totalDolares.ToString("N0") + "</td>" : "";
                    htmlBody += totalDolares != 0 ? $"<td style='{bodyTdFoot}'></td>" : "";

                    htmlBody += @"</tr>";
                    htmlBody += @"</tbody>                       
                </table><br><br>";

                }
            }

            var tableAgente = "font-family: Arial, Helvetica, sans-serif;border-bottom: 2px solid #070707;border-top: 2px solid #070707;border-left: 0px ;border-right: 0px ;background-color: #FFFFFF;width: 100%;height: 200px;text-align: center;border-collapse: collapse;";
            var tableTdTrAgente = "border-left: 0px !important;border-right: 0px !important; border-top: 1px solid #000000;border-bottom: 1px solid #000000; padding: 4px 4px;";
            var headTrAgente = "font-size: 12px; font-weight: bold;color: #000000;text-align: center; background: #FFD700; border-left: none !important;border-right: 0px !important;";
            var bodyTdAgente = "font-size: 15px;border-left: 0px !important;  border-right: 0px !important;";

            if (Model.AgenteCompras.ListaAgenteCompras.Count() > 0)
            {
                htmlBody += $@"<table style='{tableAgente}' cellpadding='10' class='agente'>" +
                    "<thead>" +
                    $"<tr style='{headTrAgente} {tableTdTrAgente}' >" +
                    $"<td style='{headTrAgente} {tableTdTrAgente}' colspan = " + (Model.AgenteCompras.ListaOperadores.Count() + 4) + " > AGENTE DE COMPRA MAT </th>" +
                    $"</tr>" +
                    $"<tr  style='{headTrAgente} {tableTdTrAgente}' class='borde'>" +
                    $"<td  style='{headTrAgente} {tableTdTrAgente}'>Producto</td>" +
                    $"<td  style='{headTrAgente} {tableTdTrAgente}'>Posición</td>" +
                    $"<td  style='{headTrAgente} {tableTdTrAgente}'>Total</td>" +
                    $"<td  style='{headTrAgente} {tableTdTrAgente}'>USD Pond.</td>";
                foreach (var op in Model.AgenteCompras.ListaOperadores)
                {
                    htmlBody += $"<td style='{headTrAgente} {tableTdTrAgente}'>" + op.OperadorDesc + "</td>";
                }
                htmlBody += "</tr></thead><tbody>";
                foreach (var agente in Model.AgenteCompras.ListaAgenteCompras)
                {
                    var pos = agente.Posicion.Split('.');

                    htmlBody += $@"<tr style='{tableTdTrAgente}'>" +
                                $"<td style='{bodyTdAgente} {tableTdTrAgente}'>" + agente.MaterialDesc + "</td>" +
                                $"<td style='{bodyTdAgente} {tableTdTrAgente}'>" + ((EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1]) + "</td>" +
                                $"<td style='{bodyTdAgente} {tableTdTrAgente}'>" + agente.Operador.Sum(x => x.Cantidad).ToString("N0") + "</td>" +
                                $"<td style='{bodyTdAgente} {tableTdTrAgente}'>" + agente.PrecioPonderado.ToString("N2") + "</td>";


                    foreach (var op in Model.AgenteCompras.ListaOperadores)
                    {
                        var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
                        htmlBody += "<td>" + (cantidad != null ? cantidad : "0") + "</td>";
                    }

                    htmlBody += "</tr>";
                }
                htmlBody += $@"</tbody><tfoot><tr> 
                 <td  style='{tableTdTrAgente}'></td> 
                 <td  style=' {tableTdTrAgente}'> Total </td>
                 <td  style='{tableTdTrAgente}'>" + Model.AgenteCompras.ListaAgenteCompras.Sum(x => x.Operador.Sum(y => y.Cantidad)).ToString("N0") + $"</td> <td style='{tableTdTrAgente}'></td>";
                foreach (var op in Model.AgenteCompras.ListaOperadores)
                {
                    var cantidad = Model.AgenteCompras.ListaAgenteCompras.Sum(x => x.Operador.Where(y => y.OperadorId == op.OperadorId).Sum(y => y.Cantidad));
                    htmlBody += $"<td  style='{tableTdTrAgente}'>" + cantidad + "</td>";
                }
                htmlBody += "</tr></tfoot>";
                htmlBody += @"</table><br><br>";
            }
            var tableSoja = "font-family: Arial, Helvetica, sans-serif;border-bottom: 2px solid #070707;border-top: 2px solid #070707;border-left: 0px ;border-right: 0px ;background-color: #FFFFFF;width: 350px;height: 200px;text-align: center;border-collapse: collapse; width:700px;";
            var tableTdTrSoja = "border-left: 0px !important;border-right: 0px !important; border-top: 1px solid #000000;border-bottom: 1px solid #000000; padding: 4px 4px;";
            var headTrSoja = "font-size: 12px; font-weight: bold;color: #FFFFFF;text-align: center; background: #017940; border-left: none !important;border-right: 0px !important;";
            var bodyTdSoja = "font-size: 15px;border-left: 0px !important;  border-right: 0px !important;";

            if (Model.SojaSustentable.Total > 0)
            {
                htmlBody += $"<table style='{tableSoja}'  class='sojasustentable'>";
                htmlBody += "<thead>";
                htmlBody += $"<tr style='{headTrSoja} {tableTdTrSoja}' >";
                htmlBody += $"<th style='{headTrSoja} {tableTdTrSoja}'  colspan='3'>SOJA SUSTENTABLE</th>";
                htmlBody += "</tr>";
                htmlBody += $"<tr style='{headTrSoja} {tableTdTrSoja}' class='borde'>";
                htmlBody += $"<th style='{headTrSoja} {tableTdTrSoja}'>A Precio</th>";
                htmlBody += $"<th style='{headTrSoja} {tableTdTrSoja}'>A Fijar</th>";
                htmlBody += $"<th style='{headTrSoja} {tableTdTrSoja}'>Total</th>";
                htmlBody += "</tr></thead><tbody>";
                htmlBody += "<tr>";
                htmlBody += $"<td style='{bodyTdSoja} {tableTdTrSoja}'>" + Model.SojaSustentable.Precio.ToString("N0") + "</td>";
                htmlBody += $"<td style='{bodyTdSoja} {tableTdTrSoja}'>" + Model.SojaSustentable.Fijar.ToString("N0") + "</td>";
                htmlBody += $"<td style='{bodyTdSoja} {tableTdTrSoja}'>" + Model.SojaSustentable.Total.ToString("N0") + "</td>";
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table><br><br>";
            }

            // =========================
            var tableSojaEPA = "font-family: Arial, Helvetica, sans-serif;border-bottom: 2px solid #070707;border-top: 2px solid #070707;border-left: 0px ;border-right: 0px ;background-color: #FFFFFF;width: 350px;height: 200px;text-align: center;border-collapse: collapse; width:700px;";
            var tableTdTrSojaEPA = "border-left: 0px !important;border-right: 0px !important; border-top: 1px solid #000000;border-bottom: 1px solid #000000; padding: 4px 4px;";
            var headTrSojaEPA = "font-size: 12px; font-weight: bold;color: #FFFFFF;text-align: center; background: #017940; border-left: none !important;border-right: 0px !important;";
            var bodyTdSojaEPA = "font-size: 15px;border-left: 0px !important;  border-right: 0px !important;";

            if (Model.SojaEPA.Total > 0)
            {
                htmlBody += $"<table style='{tableSojaEPA}'  class='sojaEPA'>";
                htmlBody += "<thead>";
                htmlBody += $"<tr style='{headTrSojaEPA} {tableTdTrSojaEPA}' >";
                htmlBody += $"<th style='{headTrSojaEPA} {tableTdTrSojaEPA}'  colspan='3'>SOJA EPA</th>";
                htmlBody += "</tr>";
                htmlBody += $"<tr style='{headTrSojaEPA} {tableTdTrSojaEPA}' class='borde'>";
                htmlBody += $"<th style='{headTrSojaEPA} {tableTdTrSojaEPA}'>A Precio</th>";
                htmlBody += $"<th style='{headTrSojaEPA} {tableTdTrSojaEPA}'>A Fijar</th>";
                htmlBody += $"<th style='{headTrSojaEPA} {tableTdTrSojaEPA}'>Total</th>";
                htmlBody += "</tr></thead><tbody>";
                htmlBody += "<tr>";
                htmlBody += $"<td style='{bodyTdSojaEPA} {tableTdTrSojaEPA}'>" + Model.SojaEPA.Precio.ToString("N0") + "</td>";
                htmlBody += $"<td style='{bodyTdSojaEPA} {tableTdTrSojaEPA}'>" + Model.SojaEPA.Fijar.ToString("N0") + "</td>";
                htmlBody += $"<td style='{bodyTdSojaEPA} {tableTdTrSojaEPA}'>" + Model.SojaEPA.Total.ToString("N0") + "</td>";
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table><br><br>";
            }
            // =========================

            var tablePrecio = "font-family: Arial, Helvetica, sans-serif; border-bottom: 2px solid #070707;border-top: 1px solid #070707;border-left: 0px; border-right: 0px; height: 200px; text-align: center; border-collapse: collapse; width:700px;";
            var precioTdTr = "background: #017940; border-left: 0px !important; border-right: 0px !important; border-top: solid #070707 1.0pt;border-bottom: solid #070707 1.0pt; padding: 4px 4px; color: #FFFFFF !important; ";
            var precioTbodyTh = "background: #FFFFFF !important; border-top: solid #070707 1.0pt !important; border-bottom: 1px solid #000000;color: #000000 !important; ";
            var precioBodyTr = "font-size: 15px;border-left: 0px ; border-right: 0px; border-top: solid #070707 1.0pt !important;";
            htmlBody += $@"<table style='{tablePrecio}' class='precio'>
                <tbody>";
            foreach (var moneda in Model.PrecioCantidad)
            {
                if (moneda.Cantidad > 0)
                {
                    htmlBody += $"<tr style='{precioBodyTr}'>";
                    htmlBody += $"<td  style='{precioTdTr}'>" + moneda.Moneda + "</td>";
                    htmlBody += $"<td style='{precioTbodyTh}' class='negrita'> " + moneda.Cantidad.Value.ToString("N2") + " </th>";
                    htmlBody += $"</tr>";
                }

            }

            htmlBody += @"</tbody>
            </table><br><br>";

            var tableHedge = "font-family: Arial, Helvetica, sans-serif;border-bottom: 2px solid #070707;border-top: 2px solid #070707;border-left: 0px ;border-right: 0px ;background-color: #FFFFFF;width: 350px;height: 200px;text-align: center;border-collapse: collapse; width:700px;";
            var tableTdTrHedge = "border-left: 0px !important;border-right: 0px !important; border-top: 1px solid #000000;border-bottom: 1px solid #000000; padding: 4px 4px;";
            var headTr = "font-size: 12px; font-weight: bold;color: #FFFFFF;text-align: center; background: #008B8B; border-left: none !important;border-right: 0px !important;";
            var bodyTd = "font-size: 15px;border-left: 0px !important;  border-right: 0px !important;";
            if (hedgeMat)
            {
                htmlBody += $@"<table class='hedge' style='{tableHedge}'>
                     <thead style='background: #008B8B; border-bottom: 1px solid #008B8B;'>
                    <tr style='{tableTdTrHedge} {headTr}'>
                        <th style='{tableTdTrHedge}' colspan='4'>HEDGE</th>
                    </tr>
                    <tr style='{tableTdTrHedge} {headTr}' class='borde'>
                        <th style='{tableTdTrHedge}' width='150'>Producto</th>
                        <th style='{tableTdTrHedge}'>Disponible</th>
                        <th style='{tableTdTrHedge}'>Forward</th>
                        <th style='{tableTdTrHedge}' width='100'>New Crop</th>
                    </tr>
                    </thead>";

                htmlBody += "<tbody>";
                foreach (var mat in Model.HedgeMaterial)
                {
                    if (mat.Disponible != 0 || mat.Forward != 0 || mat.NewCrop != 0)
                    {
                        htmlBody += $@"<tr style='{tableTdTrHedge}'>" +
                                 $"<td style='{tableTdTrHedge} {bodyTd}'>" + mat.MaterialDescripcion + "</td>" +
                                 $"<td style='{tableTdTrHedge}  {bodyTd}'>" + mat.Disponible.ToString("N0") + "</td>" +
                                 $"<td style='{tableTdTrHedge}  {bodyTd}'>" + mat.Forward.ToString("N0") + "</td>" +
                                 $"<td style='{tableTdTrHedge}  {bodyTd}'>" + mat.NewCrop.ToString("N0") + "</td>" +
                            "</tr>";
                    }
                }
                htmlBody += @"</tbody>
                    </table><br><br>";
            }

            htmlBody += "<br></br>";

            htmlBody += "<br></br>";

            return htmlBody;
        }
        public void JobCerrarDia(int comercialId, string idActiveDirectory, byte[] archivo, int diferencial)
        {
            var dia = Dia();
            var diaDeLaSemana = DateTime.Today.DayOfWeek;
            var feriado = repositorio.Existe<FechaFeriado>(x => x.Feriado == DateTime.Today);
            comercialId = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == "DATAAGRO").ComercialId;

            if (diaDeLaSemana == DayOfWeek.Saturday || diaDeLaSemana == DayOfWeek.Sunday || feriado) return;

            if (dia == null)
            {
                CerrarDia(comercialId, archivo, idActiveDirectory, true, GenerarCuerpoMail(""), diferencial);
            }
            else if (dia.Cerrado == false)
            {
                CerrarDia(comercialId, archivo, idActiveDirectory, true, GenerarCuerpoMail(""), diferencial);
            }
        }
    }
}