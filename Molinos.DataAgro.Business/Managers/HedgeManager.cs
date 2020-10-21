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
            mailManager.EnviarMail(repositorio.Obtener<Comercial>(x => x.ComercialId == comercialId),
                                            repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailHedge))),
                                                "Cierre del dia " + hoy.Day + "/" + hoy.Month,
                                                    string.Empty,
                                                        null,
                                                            AlternateView.CreateAlternateViewFromString(cuerpoMail, null, "text/html"),
                                                                archivo,
                                                                    "Cierre del dia.xls");
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
        public List<HedgeMaterialModel> TransformarAModel(List<HedgeMaterialDto> hedgeMat)
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
                HedgeMaterial = TransformarAModel(reportesManager.TraerTodosHedgeMaterial(fechaDesde, fechaHasta, null)),
                HedgeObjetivo = objetivos,
                TCPromedioDto = reportesManager.TraerTcPromedio(fechaDesde, fechaHasta, null),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
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

            var htmlBody = "";
            htmlBody += @"<style>

             table.verde {
               font-family: Arial, Helvetica, sans-serif;
               border: 2px solid #000000;
               background-color: #FFFFFF;               
               height: 200px;
               text-align: center;
               border-collapse: collapse;
               width: 700px;               
             }
             table.verde td, table.verde tr {
               border: 1px solid #000000;
               padding: 4px 10px;
             }
             table.verde tbody td {
               font-size: 15px;
               color: #000000;
             }
             table.verde thead tr td {
               font-size: 12px;
               font-weight: bold;
               color: #FFFFFF;
               text-align: left;
               border-left: 0px solid #D0E4F5;
               background: #017940;
               border-bottom: 0px solid #444444;
              text-align:center;
             }
             table.verde thead tr:first-child {
               border-left: none;
             }
             
             table.verde tfoot td {
               font-size: 19px;
             }

            table.hedge {
               font-family: Arial, Helvetica, sans-serif;
               border-bottom: 2px solid #070707;
               border-top: 2px solid #070707;
               border-left: 0px ;
               border-right: 0px ;
               background-color: #FFFFFF;
               width: 350px;
               height: 200px;
               text-align: center;
               border-collapse: collapse;
               width:700px;
             }
             table.hedge td, table.hedge tr {
               border-left: 0px solid #000000;  
               border-right: 0px solid #000000;
               border-top: 1px solid #000000;
               border-bottom: 1px solid #000000;
             
               padding: 4px 4px;
             }
             table.hedge tbody td {
               font-size: 15px;
               border-left: 0px ;
               border-right: 0px ;
             }
             table.hedge tr:nth-child(even) {
               background: #DDDDDD;
             }
             table.hedge thead {
               background: #008B8B;
               border-bottom: 1px solid #008B8B;
             }
             table.hedge thead tr {
               font-size: 12px;
               font-weight: bold;
               color: #FFFFFF;
               text-align: center;
               background: #008B8B;
             }
             table.hedge tfoot td {
               font-size: 14px;
             }


             table.pricing {
               font-family: Arial, Helvetica, sans-serif;
               border-bottom: 2px solid #070707;
               border-top: 2px solid #070707;
               border-left: 0px ;
               border-right: 0px ;
               background-color: #FFFFFF;
               width: 350px;
               height: 200px;
               text-align: center;
               border-collapse: collapse;
               width:700px;
             }
             table.pricing td, table.pricing tr {
               border-left: 0px solid #000000;  
               border-right: 0px solid #000000;
               border-top: 1px solid #000000;
               border-bottom: 1px solid #000000;
             
               padding: 4px 4px;
             }
             table.pricing tbody td {
               font-size: 15px;
               border-left: 0px ;
               border-right: 0px ;
             }
             table.pricing tr:nth-child(even) {
               background: #DDDDDD;
             }
             table.pricing thead {
               background: #C71585;
               border-bottom: 1px solid #C71585;
             }
             table.pricing thead tr {
               font-size: 12px;
               font-weight: bold;
               color: #FFFFFF;
               text-align: center;
             }
             table.pricing tfoot td {
               font-size: 14px;
             }

             table.soja {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.soja td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.soja tbody td {
             font-size: 15px;
             color: #000000;
             background: #FFF;
           }
          table.soja tbody tr{
            background: #FFF;
            }
        table.soja tbody tr td{
            background: #FFF;
            }
           table.soja thead tr  {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #99CC00;
             border-bottom: 2px solid #000000;
           }
           table.soja thead td:first-child {
             border-left: none;
           }
           
           table.soja tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #99CC00;
             
           
           }
           table.soja tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
            background: #99CC00;
           }
           
           table.maiz {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.maiz td, table.maiz td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.maiz tbody td {
             font-size: 15px;
             color: #000000;
             background: #FFF;
           }
          table.maiz tbody tr{
            background: #FFF;
            }
           table.maiz thead td {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #ffcc99;
             border-bottom: 2px solid #000000;
           }
           table.maiz thead td:first-child {
             border-left: none;
           }
           
           table.maiz tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #ffcc99;
             
           
           }
           table.maiz tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
            background: #ffcc99;
           }
           
           
           table.trigocalidad {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.trigocalidad td, table.trigocalidad td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.trigocalidad tbody td {
             background: #FFF;
             font-size: 15px;
             color: #000000;
           }
          table.trigocalidad tbody tr{
            background: #FFF;
            }
           table.trigocalidad thead td {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #99ccff;
             border-bottom: 2px solid #000000;
           }
           table.trigocalidad thead td:first-child {
             border-left: none;
           }
           
           table.trigocalidad tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #99ccff;
             
           
           }
           table.trigocalidad tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
            background: #99ccff;
           }
           
           
           table.trigogrado2 {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.trigogrado2 td, table.trigogrado2 td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.trigogrado2 tbody td {
             font-size: 15px;
             color: #000000;
             background: #FFF;
           }
          table.trigogrado2 tbody tr{
            background: #FFF;
            }
           table.trigogrado2 thead td {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #6ae6be;
             border-bottom: 2px solid #000000;
           }
           table.trigogrado2 thead td:first-child {
             border-left: none;
           }
           
           table.trigogrado2 tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #6ae6be;
             
           
           }
           table.trigogrado2 tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
             background: #6ae6be;
           }
           
           
           
           
           table.girasol {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.girasol td, table.girasol td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.girasol tbody td {
             font-size: 15px;
             color: #000000;
             background: #FFF;
           }

          table.girasol tbody tr{
            background: #FFF;
            }
           table.girasol thead td {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #d360d4;
             border-bottom: 2px solid #000000;
           }
           table.girasol thead td:first-child {
             border-left: none;
           }
           
           table.girasol tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #d360d4;  
           
           }
           table.girasol tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
           }
               table.agente {
               font-family: Arial, Helvetica, sans-serif;
               border-bottom: 2px solid #070707;
               border-top: 2px solid #070707;
               border-left: 0px ;
               border-right: 0px ;
               background-color: #FFFFFF;              
               height: 200px;
               text-align: center;
               border-collapse: collapse;
               width:100%;
             }
             table.agente td, table.agente tr {
               border-left: 0px solid #000000;  
               border-right: 0px solid #000000;
               border-top: 1px solid #000000;
               border-bottom: 1px solid #000000;             
               padding: 4px 4px;
             }
             table.agente tbody td {
               font-size: 15px;
               border-left: 0px ;
               border-right: 0px ;
             }
             table.agente thead {
               background: #FFD700;
               border-bottom: 1px solid #FFD700;
             }
             table.agente thead tr {
               font-size: 12px;
               font-weight: bold;
               color: #000000;
               text-align: center;
               background: #FFD700;
             }
              table.agente tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
           
           }
           table.agente tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
           }       
           
           
           table.trigocámara {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.trigocámara td, table.trigocámara td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.trigocámara tbody td {
             font-size: 15px;
             color: #000000;
             background: #FFF;
           }

          table.trigocámara tbody tr{
            background: #FFF;
            }
           table.trigocámara thead td {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #9999FF;
             border-bottom: 2px solid #000000;
           }
           table.trigocámara thead td:first-child {
             border-left: none;
           }
           
           table.trigocámara tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #9999FF;
             
           
           }
           table.trigocámara tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
             background: #9999FF;
           }          
           
           
           table.girasolaltooleico {
             font-family: Arial, Helvetica, sans-serif;
             border: 2px solid #000000;
             width: 100%;
             text-align: center;
             border-collapse: collapse;
           }
           table.girasolaltooleico td, table.girasolaltooleico td {
             border: 1px solid #000000;
             padding: 4px 4px;
           }
           table.girasolaltooleico tbody td {
             font-size: 15px;
             color: #000000;
            
           }
          table.girasolaltooleico tbody tr{
            background: #FFF;
            }
           table.girasolaltooleico thead td {
             font-size: 12px;
             font-weight: bold;
             color: #050505;
             text-align: center;
             border-left: 0px solid #000000;
             background: #f4c1f7;
             border-bottom: 2px solid #000000;
           }
           table.girasolaltooleico thead td:first-child {
             border-left: none;
           }
           
           table.girasolaltooleico tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #000000;
             background: #f4c1f7;  
           
           }
           table.girasolaltooleico tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
             background: #f4c1f7;  
           }
         table.sojasustentable {
               font-family: Arial, Helvetica, sans-serif;
               border-bottom: 2px solid #070707;
               border-top: 2px solid #070707;
               border-left: 0px ;
               border-right: 0px ;
               background-color: #FFFFFF;              
               height: 200px;
               text-align: center;
               border-collapse: collapse;
               width:700px;
             }
             table.sojasustentable td, table.sojasustentable tr {
               border-left: 0px solid #000000;  
               border-right: 0px solid #000000;
               border-top: 1px solid #000000;
               border-bottom: 1px solid #000000;             
               padding: 4px 4px;
             }
             table.sojasustentable tbody td {
               font-size: 15px;
               border-left: 0px ;
               border-right: 0px ;
             }
             table.sojasustentable thead {
               background: #017940;
               border-bottom: 1px solid #017940;
             }
             table.sojasustentable thead tr {
               font-size: 12px;
               font-weight: bold;
               color: #FFF;
               text-align: center;
               background: #017940;
             }
              table.sojasustentable tfoot {
             font-size: 12px;
             font-weight: bold;
             color: #FFF;
           
           }
           table.sojasustentable tfoot td {
             font-size: 12px;
             border-left: 0px;  
             border-right: 0px;
           }     

           tr.msoja td, tr.mgirasolaltooleico, .mmaiz, .mgirasol, .mtrigocámara,.mtrigogrado2,.mtrigocalidad{           
             font-weight: bold;
            }
            tr.msoja td{
                 background: #99CC00 !important;
            }
            tr.mmaiz td {
             background: #ffcc99 !important;
            }
            tr.mtrigocalidad td {
             background: #99ccff !important;
            }
            tr.mtrigogrado2 td {
             background: #6ae6be !important;
            }
            tr.mgirasol td {
             background: #d360d4 !important;
            }
            tr.trigocámara td {
             background: #9999FF !important;
            }
            tr.girasolaltooleico td {
             background: #f4c1f7 !important;
            }
            tr.borde th{
            border-top: solid #070707 1.4pt !important;
            }   

    table.precio {
               font-family: Arial, Helvetica, sans-serif;
               border-bottom: 2px solid #070707;
               border-top: 2px solid #070707;
               border-left: 0px ;
               border-right: 0px ;
               background-color: #FFFFFF;              
               height: 200px;
               text-align: center;
               border-collapse: collapse;
               width:700px;
             }
             table.precio td, table.precio tr {
               border-left: 0px solid #000000;  
               border-right: 0px solid #000000;
               border-top: 1px solid #000000;
               border-bottom: 1px solid #000000;             
               padding: 4px 4px;

             }
             table.precio tbody td {
               font-size: 15px;
               border-left: 0px ;
               border-right: 0px ;
               background: #017940;
  border-top: solid #070707 1.2pt !important;
             }
             table.precio thead tr {
               font-size: 12px;
               font-weight: bold;
               color: #FFF;
               text-align: center;
               background: #017940;
             }
                table.precio tbody th{
              border-top: solid #070707 1.2pt !important;
                }
        </style>";//style

            htmlBody += "Estimados,";
            htmlBody += "<br></br>";
            htmlBody += "A continuación se detallan las compras correspondientes al cierre del día.";
            htmlBody += "<br></br>";
            htmlBody += "Observaciones: " + (String.IsNullOrEmpty(observaciones) ? "Sin observaciones." : observaciones);
            htmlBody += "<br></br>";          
           

            if (pricing)
            {
                htmlBody += @"<table class='pricing'>
                    <thead>
                    <tr>
                        <th>PRICING</th>
                        <th>CAMPAÑA</th>";               
                htmlBody += Model.PricingCampania.Any(x => x.SanLorenzo > 0) ? "<th>SL</th>" : "";
                htmlBody += Model.PricingCampania.Any(x => x.Acopio > 0) ? "<th>Acopios</th>" : "";
                htmlBody += "<th> TOTALES </th> ";
                htmlBody += "</tr>";
                htmlBody += "</thead>";
                htmlBody += "<tbody>";
                var par = 0;
                foreach (var datos in Model.PricingCampania)
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
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 22:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad").Sum(x => x.NewAgente)).ToString("N0");
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
                    var estilo = "style='background: #DDDDDD'";
                    if ((par % 2) == 0)
                    {
                        estilo = "style='background: #FFFFF'";
                    }
                    htmlBody += @"<tr " + estilo + @">
                            <td >" + datos.Material + @"</td>
                            <td>" + datos.Campania + @"</td>";                           

                    htmlBody +=  (Model.PricingCampania.Any(x => x.SanLorenzo > 0 && x.Id == datos.Id) ? "<td>" + datos.SanLorenzo.ToString("N0") + "</td>" : "");
                    htmlBody +=  (Model.PricingCampania.Any(x => x.Acopio > 0 && x.Id == datos.Id) ? "<td>" + datos.Acopio.ToString("N0") + "</td>" : "");
                    htmlBody += "<td> " + sumaPricing + "</td>";
                    htmlBody += "</tr>";

                }
                htmlBody += "</tbody></table> <br><br>";
            }

            if (disp + forw + newc != 0)
            {
                htmlBody += @" <table class='verde'>
                <thead>
                    <tr>
                        <td></td>";
                htmlBody += (disp > 0) ? "<td style='text-aligne:center;' id='disponible'  colspan='" + disp + "'>DISPONIBLE</td>" +
                    "<td id='disponibleTotal'  rowspan='2' colspan='2'>TOTAL DISPONIBLE</td>" : "";
                htmlBody += (forw > 0) ? "<td id='forward' colspan='" + forw + "'>FORWARD</td>" +
                        "<td id='forwardTotal' class=' rowspan='2' colspan='2'>TOTAL FORWARD</td>" : "";
                htmlBody += (newc > 0) ? "<td style='text-aligne: center;' colspan='" + newc + "'>NEW CROP</td>" +
                        "<td id='newCropTotal' rowspan='2'>TOTAL NEW CROP</td>" : "";
                htmlBody += "</tr>";

                htmlBody += "<tr class='titulos'>";
                htmlBody += "<td>PRODUCTO</td>";
                htmlBody += dispAFijar ? "<td class='valores'>A Fijar</td>" : "";
                htmlBody += dispAPrecio ? "<td class='valores'>A Precio</td>" : "";
                htmlBody += dispFijacion ? "<td class='valores'>Fijación</td>" : "";
                htmlBody += dispAgente ? "<td class='valores'>MAT</td>" : "";
                //htmlBody += disp > 0 ? "<td class=''></td>" : "";
                htmlBody += forwAPrecio ? "<td class='valores'>A Precio</td>" : "";
                htmlBody += forwFijacion ? "<td class='valores'>Fijación</td>" : "";
                htmlBody += forwAgente ? "td class='valores'>MAT</td>" : "";
                //htmlBody += forw > 0 ? "<td class=''> </td>" : "";
                htmlBody += newcAFijar ? "<td class='valores'>A Fijar</td>" : "";
                htmlBody += newcAPrecio ? "<td class='valores'>A Precio</td>" : "";
                htmlBody += newcFijacion ? "<td class='valores'>Fijación</td>" : "";
                htmlBody += newcAgente ? "<td class='valores'>MAT</td>" : "";

                htmlBody += "</tr> </thead>";
                htmlBody += "<tbody>";
                foreach (var toneladaPrecio in Model.ToneladasGranoTipo)
                {
                    var posicion = Model.PosicionCompras.Where(x => x.Material.ToLower() == toneladaPrecio.Material.ToLower());
                    var totalDisp = posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac + y.DispAFijar + y.DispAPrecio)).Sum() + toneladaPrecio.DispAgente;
                    var totalForw = posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar + y.FrwAPrecio + y.FrwFijac)).Sum() + toneladaPrecio.FrwAgente;
                    var totalNewC = posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar + y.NewAPrecio + y.NewFijac)).Sum() + toneladaPrecio.NewAgente;

                    if (totalDisp != 0 || totalForw != 0 || totalNewC != 0)
                    {
                        htmlBody += @"<tr>";
                        htmlBody += "<td>" + toneladaPrecio.Material + "</td>";
                        htmlBody += dispAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac)).Sum().ToString("N0")) + " </td>" : "";
                        htmlBody += dispAgente ? "<td class=''>" + toneladaPrecio.DispAgente.ToString("N0") + " </td>" : "";
                        htmlBody += disp > 0 ? "<td>" + totalDisp.ToString("N0") + " </td>" : "";

                        htmlBody += disp > 0 ? "<td class=''></td>" : "";
                        htmlBody += forwAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAgente ? "<td class=''>" + toneladaPrecio.FrwAgente.ToString("N0") + "</td>" : "";
                        htmlBody += forw > 0 ? "<td>" + totalForw.ToString("N0") + "</td>" : "";

                        htmlBody += forw > 0 ? "<td class=' '> </td>" : "";
                        htmlBody += newcAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAgente ? "<td class=''>" + toneladaPrecio.NewAgente.ToString("N0") + " </td>" : "";
                        htmlBody += newc > 0 ? "<td>" + totalNewC.ToString("N0") + " </td>" : "";
                        htmlBody += "</tr>";
                    }
                }
                htmlBody += " </tbody>";
                htmlBody += "</table><br><br>";
            }


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
                if (suma > 0)
                {

                    htmlBody += @"<table id='" + material.Material.Replace(" ", "") + @"' class='" + material.Material.Replace(" ", "").ToLower() + @"'>
                     <thead>
                        <tr><td colspan = '" + ((suma + pond) + 1) + "'> " + material.Material.ToUpper() + @" </td></tr>
                        <tr>
                            <td rowspan = '2'> Posición </td>";
                    htmlBody += fix > 0 ? "<td colspan='" + fix + "'>Fix</td>" : "";
                    htmlBody += aFijar > 0 ? "<td colspan = '" + aFijar + "'>A Fijar</td>" : "";
                    htmlBody += aPrecio > 0 ? "<td colspan = '" + aPrecio + "'>A Precio</td>" : "";
                    htmlBody += totalPesos != 0 ? "<td rowspan = '2'> Ton. $</td>" : "";
                    htmlBody += pondPesos > 0 ? "<td rowspan = '2'>Precio $</td>" : "";
                    htmlBody += totalDolares != 0 ? "<td rowspan = '2' class=''>Ton.USD</td>" : "";
                    htmlBody += pondDolares > 0 ? "<td rowspan = '2'>Precio USD</td>" : "";

                    htmlBody += "</tr>";
                    htmlBody += "<tr class='titulosPosicion'>";
                    htmlBody += dispFijacion ? "<td>Disponible</td>" : "";
                    htmlBody += forwFijacion ? "<td>Forward</td>" : "";
                    htmlBody += newcFijacion ? "<td>New Crop</td>" : "";
                    htmlBody += dispAFijar ? "<td>Disponible</td>" : "";
                    htmlBody += forwAFijar ? "<td>Forward</td>" : "";
                    htmlBody += newcAFijar ? "<td>New Crop</td>" : "";
                    htmlBody += dispAPrecio ? "<td>Disponible</td>" : "";
                    htmlBody += forwAPrecio ? "<td>Forward</td>" : "";
                    htmlBody += newcAPrecio ? "<td>New Crop</td>" : "";
                    htmlBody += "</tr></thead><tbody>";

                    foreach (var mes in material.PosicionKilos.OrderBy(x => x.Anio).ThenBy(x => x.Mes))
                    {
                        int mesActual = (int)((EnumMeses)Enum.Parse(typeof(EnumMeses), mes.Mes.ToString()));
                        htmlBody += @"<tr>
                        <td> " + mes.Mes + " - " + mes.Anio + "</td>";
                        htmlBody += dispFijacion ? "<td>" + mes.DispFijac.ToString("N0") + "</td>" : "";
                        htmlBody += forwFijacion ? "<td>" + mes.FrwFijac.ToString("N0") + "</td>" : "";
                        htmlBody += newcFijacion ? "<td>" + mes.NewFijac.ToString("N0") + "</td>" : "";
                        htmlBody += dispAFijar ? "<td>" + mes.DispAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += forwAFijar ? "<td>" + mes.FrwAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += newcAFijar ? "<td>" + mes.NewAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += dispAPrecio ? "<td>" + mes.DispAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += forwAPrecio ? "<td>" + mes.FrwAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += newcAPrecio ? "<td>" + mes.NewAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += totalPesos != 0 ? "<td> " + (mes.KilosPesos.ToString("N0")) + "</td>" : "";
                        htmlBody += pondPesos > 0 ? "<td> " + (mes.PrecioPonderadoPesos.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += totalDolares != 0 ? "<td> " + (mes.KilosDolares.ToString("N0")) + "</td>" : "";
                        htmlBody += pondDolares > 0 ? "<td> " + (mes.PrecioPonderadoDolares.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += "</tr>";
                    }
                    htmlBody += @"<tr class='m" + material.Material.Replace(" ", "").ToLower() + @"'>
                        <td>Total</td>";
                    htmlBody += dispFijacion ? "<td>" + material.PosicionKilos.Sum(y => y.DispFijac).ToString("N0") + "</td>" : "";
                    htmlBody += forwFijacion ? "<td>" + material.PosicionKilos.Sum(y => y.FrwFijac).ToString("N0") + "</td>" : "";
                    htmlBody += newcFijacion ? "<td>" + material.PosicionKilos.Sum(y => y.NewFijac).ToString("N0") + "</td>" : "";
                    htmlBody += dispAFijar ? "<td>" + material.PosicionKilos.Sum(y => y.DispAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += forwAFijar ? "<td>" + material.PosicionKilos.Sum(y => y.FrwAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += newcAFijar ? "<td>" + material.PosicionKilos.Sum(y => y.NewAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += dispAPrecio ? "<td>" + material.PosicionKilos.Sum(y => y.DispAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += forwAPrecio ? "<td>" + material.PosicionKilos.Sum(y => y.FrwAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += newcAPrecio ? "<td>" + material.PosicionKilos.Sum(y => y.NewAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += totalPesos != 0 ? "<td >" + totalPesos.ToString("N0") + "</td>" : "";
                    htmlBody += pondPesos > 0 ? "<td></td>" : "";
                    htmlBody += totalDolares != 0 ? "<td >" + totalDolares.ToString("N0") + "</td>" : "";
                    htmlBody += totalDolares != 0 ? "<td></td>" : "";

                    htmlBody += @"</tr>";
                    htmlBody += @"</tbody>                       
                </table><br><br>";

                }
            }


            if (Model.AgenteCompras.ListaAgenteCompras.Count() > 0)
            {
                htmlBody += @"<table cellpadding='10' class='agente'>" +
                    "<thead>" +
                    "<tr>" +
                    "<th colspan = " + (Model.AgenteCompras.ListaOperadores.Count() + 4) + " > AGENTE DE COMPRA MAT </th>" +                   
                    "</tr>" +
                    "<tr class='borde'>" +
                    "<th>Producto</th>" +
                    "<th>Posición</th>" +
                    "<th>Total</th>" +
                    "<th>USD Pond.</th>";
                foreach (var op in Model.AgenteCompras.ListaOperadores)
                {
                    htmlBody += "<th>" + op.OperadorDesc + "</th>";
                }
                htmlBody += "</tr></thead><tbody>";
                foreach (var agente in Model.AgenteCompras.ListaAgenteCompras)
                {
                    var pos = agente.Posicion.Split('.');

                    htmlBody += @"<tr>" +
                                "<td>" + agente.MaterialDesc + "</td>" +
                                "<td>" + ((EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1]) + "</td>" +
                                "<td>" + agente.Operador.Sum(x => x.Cantidad).ToString("N0") + "</td>" +
                                "<td>" + agente.PrecioPonderado.ToString("N2") + "</td>";


                    foreach (var op in Model.AgenteCompras.ListaOperadores)
                    {
                        var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
                        htmlBody += "<td>" + (cantidad != null ? cantidad : "0") + "</td>";
                    }

                    htmlBody += "</tr>";
                }
                htmlBody += @"</tbody><tfoot><tr> 
                 <td></td> 
                 <td> Total </td>
                 <td>" + Model.AgenteCompras.ListaAgenteCompras.Sum(x => x.Operador.Sum(y => y.Cantidad)).ToString("N0") + "</td> <td></td>";
                foreach (var op in Model.AgenteCompras.ListaOperadores)
                {
                    var cantidad = Model.AgenteCompras.ListaAgenteCompras.Sum(x => x.Operador.Where(y => y.OperadorId == op.OperadorId).Sum(y => y.Cantidad));
                    htmlBody += "<td>" + cantidad + "</td>";
                }
               htmlBody += "</tr></tfoot>";
               htmlBody += @"</table><br><br>";
            }
            


            if (Model.SojaSustentable.Total > 0)
            {
                htmlBody += "<table  class='sojasustentable'>";
                htmlBody += "    <thead>";
                htmlBody += "        <tr>";
                htmlBody += "            <th colspan='3'>SOJA SUSTENTABLE</th>";
                htmlBody += "        </tr>";
                htmlBody += "        <tr class='borde'>";
                htmlBody +=  "<th>A Precio</th>";
                htmlBody +=  "<th>A Fijar</th>";
                htmlBody += "            <th>Total</th>";
                htmlBody += "        </tr></thead><tbody>";
                htmlBody += "        <tr>";
                htmlBody +="<td>" + Model.SojaSustentable.Precio.ToString("N0") + "</td>";
                htmlBody += "<td>" + Model.SojaSustentable.Fijar.ToString("N0") + "</td>";
                htmlBody += "            <td>" + Model.SojaSustentable.Total.ToString("N0") + "</td>";
                htmlBody += "        </tr>";
                htmlBody += "    </tbody>";
                htmlBody += "</table><br><br>";
            }

            htmlBody += @"<table class='precio'>
                <tbody>";
            foreach (var moneda in Model.PrecioCantidad)
            {
                if (moneda.Cantidad > 0)
                {
                    htmlBody += "<tr>";
                    htmlBody += "<td>" + moneda.Moneda + "</td>";
                    htmlBody += "<th class='negrita'> " + moneda.Cantidad.Value.ToString("N2") + " </th>";
                    htmlBody += "</tr>";
                }

            }
            htmlBody += @"</tbody>
            </table><br><br>";

            if (hedgeMat)
            {
                htmlBody += @"<table class='hedge'>
                     <thead>
                    <tr>
                        <th colspan='4'>HEDGE</th>
                    </tr>
                    <tr class='borde'>
                        <th width='150'>Producto</th>
                        <th>Disponible</th>
                        <th>Forward</th>
                        <th width='100'>New Crop</th>
                    </tr>
                    </thead>";

                htmlBody += "<tbody>";
                foreach (var mat in Model.HedgeMaterial)
                {
                    if (mat.Disponible != 0 || mat.Forward != 0 || mat.NewCrop != 0)
                    {
                        htmlBody += @"<tr>" +
                                "<td>" + mat.MaterialDescripcion + "</td>" +
                                "<td>" + mat.Disponible.ToString("N0") + "</td>" +
                                "<td>" + mat.Forward.ToString("N0") + "</td>" +
                                "<td>" + mat.NewCrop.ToString("N0") + "</td>" +
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


    }
}

