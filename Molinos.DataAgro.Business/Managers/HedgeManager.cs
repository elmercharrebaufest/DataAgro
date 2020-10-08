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
table {
   
    text-align: center;
    border-bottom: 2px solid black;
    border-spacing: 0px !important;
  }

    table tr {
        border-top: 1px solid black;
        border-bottom: 1px solid black;
    }

.titulos {
    border-top: 2px solid black;
    border-bottom: 2px solid black;
    background: #017940;
    color: white;
}
.row {
    background: white;
}

#MonedaKilo th {
    background: #017940;
    color: white;
}

#MonedaKilo td {
    background: white;
    color: black;
}


#Soja .titulosPosicion {
    background: rgb(153, 204, 0);
}
#Maiz .titulosPosicion {
    background: rgb(255, 204, 153);
}
#MaizTable {
    z-index: 175;
}
#TrigoCámara .titulosPosicion {
    background: rgb(153, 153, 255);
}
#TrigoCámaraTable {
    z-index: 150;
}
#TrigoCalidad .titulosPosicion {
    background: rgb(153, 204, 255);
}
#TrigoCalidadTable {
    z-index: 125;
}
#TrigoGrado2 .titulosPosicion {
    background: rgb(106, 230, 190);
}
#TrigoGrado2Table {
    z-index: 100;
}
#Girasol .titulosPosicion {
    background: rgb(211, 96, 212);
}
#GirasolTable {
    z-index: 75;
}
#GirasolAltoOleico .titulosPosicion {
    background: rgb(244, 193, 247);
}

.trPar {
    background-color: #eaeaea;
}

.titulos.hedgemat {
    background: darkcyan;
}

.titulos.hedgeobj {
    background: plum;
}

.titulos.agente {
    background: gold;
    color: black;
}

.Mat {  
    background: gold !important;
    color: black;
}


.titulos.pricing {
    background: mediumvioletred;
}

#grillaAgente > .k-grid-header, #grillaAgente .k-grid-header .k-header {
    background-color: yellow;
    font-weight: bold;
}

#grilla > .k-grid-header, #grilla .k-grid-header .k-header {
    background-color: yellowgreen;
    font-weight: bold;
}



.espacio {
    background-color: white;
    border-top: 2px solid white;
    border-bottom: 2px solid white;
    border-right: 2px solid black;
    border-left:  1px solid  black;
}

.negrita{
font-weight: bold !important;
}

 td{
margin
}
 
</style>";//style

            htmlBody += "Estimados,";
            htmlBody += "<br></br>";
            htmlBody += "A continuación, se detallan las compras correspondientes al cierre del día.";
            htmlBody += "<br></br>";
            htmlBody += "Observaciones: " + (String.IsNullOrEmpty(observaciones) ? "Sin observaciones." : observaciones);
            htmlBody += "<br></br>";
            htmlBody += "<br></br>";
            if (hedgeMat)
            {
                htmlBody += @"<table cellpadding='10' style='' id='HedgeMaterial'>
                <tbody>
                    <tr class='titulos hedgemat'>
                        <th colspan='4'>HEDGE</th>
                    </tr>
                    <tr class='titulos hedgemat'>
                        <th>Producto</th>
                        <th>Disponible</th>
                        <th>Forward</th>
                        <th>New Crop</th>
                    </tr>";
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
                htmlBody += @"</tbody></table><br><br>";
            }

            if (pricing)
            {
                htmlBody += @"<table cellpadding='10' style='' id='Pricing'>
                    <tr class='titulos pricing' style='cellspacing:0; background: mediumvioletred;border: 2px solid mediumvioletred;  color: white;'>
                        <th>PRICING</th>
                        <th>CAMPAÑA</th>";               
                htmlBody += Model.PricingCampania.Any(x => x.SanLorenzo > 0) ? "<th style='width:50px'>SL</th>" : "";
                htmlBody += Model.PricingCampania.Any(x => x.Acopio > 0) ? "<th>Acopios</th>" : "";
                htmlBody += "<th> TOTALES </th> ";
                htmlBody += "</tr>";
                foreach (var datos in Model.PricingCampania)
                {
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
                    htmlBody += @"<tr>
                            <td>" + datos.Material + @"</td>
                            <td>" + datos.Campania + @"</td>";                           

                    htmlBody +=  (Model.PricingCampania.Any(x => x.SanLorenzo > 0 && x.Id == datos.Id) ? "<td>" + datos.SanLorenzo.ToString("N0") + "</td>" : "");
                    htmlBody +=  (Model.PricingCampania.Any(x => x.Acopio > 0 && x.Id == datos.Id) ? "<td>" + datos.Acopio.ToString("N0") + "</td>" : "");
                    htmlBody += "<td> " + sumaPricing + "</td>";
                    htmlBody += "</tr>";
                }
                htmlBody += "</table> <br><br>";
            }

            if (disp + forw + newc != 0)
            {
                htmlBody += @" <table cellpadding='10' class='table-condensed noSideMargin' id='ComprasToneladasTipo'>
                <tbody>
                    <tr class='titulos'>
                        <td id='' class='borde-izquierdo'></td>";

                htmlBody += (disp > 0) ? "<th id='disponible' class='borde-derecho' colspan='" + disp + "'>DISPONIBLE</th>" +
                    "<th id='disponibleTotal' class='borde-derecho-oscuro' rowspan='2' colspan='2'>TOTAL DISPONIBLE</th>" : "";
                htmlBody += (forw > 0) ? "<th id='forward' class='borde-derecho' colspan='" + forw + "'>FORWARD</th>" +
                        "<th id='forwardTotal' class=' borde-derecho-oscuro' rowspan='2' colspan='2'>TOTAL FORWARD</th>" : "";
                htmlBody += (newc > 0) ? "<th id='newCrop' class='borde-derecho' colspan='" + newc + "'>NEW CROP</th>" +
                        "<th id='newCropTotal' class='borde-derecho-oscuro' rowspan='2'>TOTAL NEW CROP</th>" : "";

                htmlBody += "</tr>";
                htmlBody += "<tr class='titulos'>";
                htmlBody += "<th class='producto borde-izquierdo'>PRODUCTO</th>";
                htmlBody += dispAFijar ? "<th class='valores'>A Fijar</th>" : "";
                htmlBody += dispAPrecio ? "<th class='valores'>A Precio</th>" : "";
                htmlBody += dispFijacion ? "<th class='valores'>Fijación</th>" : "";
                htmlBody += dispAgente ? "<th class='valores'>MAT</th>" : "";
                //htmlBody += disp > 0 ? "<td class=''></td>" : "";
                htmlBody += forwAPrecio ? "<th class='valores'>A Precio</th>" : "";
                htmlBody += forwFijacion ? "<th class='valores'>Fijación</th>" : "";
                htmlBody += forwAgente ? "th class='valores'>MAT</th>" : "";
                //htmlBody += forw > 0 ? "<td class=''> </td>" : "";
                htmlBody += newcAFijar ? "<th class='valores'>A Fijar</th>" : "";
                htmlBody += newcAPrecio ? "<th class='valores'>A Precio</th>" : "";
                htmlBody += newcFijacion ? "<th class='valores'>Fijación</th>" : "";
                htmlBody += newcAgente ? "<th class='valores'>MAT</th>" : "";

                htmlBody += "</tr>";
                foreach (var toneladaPrecio in Model.ToneladasGranoTipo)
                {
                    var posicion = Model.PosicionCompras.Where(x => x.Material.ToLower() == toneladaPrecio.Material.ToLower());
                    var totalDisp = posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac + y.DispAFijar + y.DispAPrecio)).Sum() + toneladaPrecio.DispAgente;
                    var totalForw = posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar + y.FrwAPrecio + y.FrwFijac)).Sum() + toneladaPrecio.FrwAgente;
                    var totalNewC = posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar + y.NewAPrecio + y.NewFijac)).Sum() + toneladaPrecio.NewAgente;

                    if (totalDisp != 0 || totalForw != 0 || totalNewC != 0)
                    {
                        htmlBody += @"<tr>";
                        htmlBody += " <th class='borde-izquierdo'>" + toneladaPrecio.Material + "</th>";
                        htmlBody += dispAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac)).Sum().ToString("N0")) + " </td>" : "";
                        htmlBody += dispAgente ? "<td class=''>" + toneladaPrecio.DispAgente.ToString("N0") + " </td>" : "";
                        htmlBody += disp > 0 ? "<td class='borde-derecho-oscuro'>" + totalDisp.ToString("N0") + " </td>" : "";

                        htmlBody += disp > 0 ? "<td class=''></td>" : "";
                        htmlBody += forwAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAgente ? "<td class=''>" + toneladaPrecio.FrwAgente.ToString("N0") + "</td>" : "";
                        htmlBody += forw > 0 ? "<td class='borde-derecho-oscuro'>" + totalForw.ToString("N0") + "</td>" : "";

                        htmlBody += forw > 0 ? "<td class=' '> </td>" : "";
                        htmlBody += newcAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAgente ? "<td class=''>" + toneladaPrecio.NewAgente.ToString("N0") + " </td>" : "";
                        htmlBody += newc > 0 ? "<td class='borde-derecho-oscuro'>" + totalNewC.ToString("N0") + " </td>" : "";
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
                    htmlBody += @" <div id='" + material.Material.Replace(" ", "") + "Table' style='width:" + (suma <= 5 ? "50%" : "100%") + "'>" +
                    @"<table cellpadding='10' id='" + material.Material.Replace(" ", "") + @"' class='table table-condensed'>
                    <tbody>
                        <tr class='titulosPosicion'><th colspan = '" + (suma + pond + 3 + 1) + "'> " + material.Material.ToUpper() + @" </ th ></ tr >
                        <tr class='titulosPosicion'>
                            <th rowspan = '2'> Posición </th>";
                    htmlBody += fix > 0 ? "<th colspan='" + fix + "' class='borde-izq-der'>Fix</th>" : "";
                    htmlBody += aFijar > 0 ? "<th colspan = '" + aFijar + "' class='borde-izq-der'>A Fijar</th>" : "";
                    htmlBody += aPrecio > 0 ? "<th colspan = '" + aPrecio + "' class='borde-izq-der'>A Precio</th>" : "";
                    htmlBody += totalPesos != 0 ? "<th rowspan = '2' colspan='2' class=''>Ton. $</th>" : "";
                    htmlBody += pondPesos > 0 ? "<th rowspan = '2'  colspan='2' class=''>Precio $</th>" : "";
                    htmlBody += totalDolares != 0 ? "<th rowspan = '2'  colspan='2' class=''>Ton.USD</th>" : "";
                    htmlBody += pondDolares > 0 ? "<th rowspan = '2'   class=''>Precio USD</th>" : "";

                    htmlBody += "</tr>";
                    htmlBody += "<tr class='titulosPosicion'>";
                    htmlBody += dispFijacion ? "<th class='borde-izq-der'>Disponible</th>" : "";
                    htmlBody += forwFijacion ? "<th class='borde-izq-der'>Forward</th>" : "";
                    htmlBody += newcFijacion ? "<th class='borde-izq-der'>New Crop</th>" : "";
                    htmlBody += dispAFijar ? "<th class='borde-izq-der'>Disponible</th>" : "";
                    htmlBody += forwAFijar ? "<th class='borde-izq-der'>Forward</th>" : "";
                    htmlBody += newcAFijar ? "<th class='borde-izq-der'>New Crop</th>" : "";
                    htmlBody += dispAPrecio ? "<th class='borde-izq-der'>Disponible</th>" : "";
                    htmlBody += forwAPrecio ? "<th class='borde-izq-der'>Forward</th>" : "";
                    htmlBody += newcAPrecio ? "<th class='borde-izq-der'>New Crop</th>" : "";
                    htmlBody += "</tr>";

                    foreach (var mes in material.PosicionKilos.OrderBy(x => x.Anio).ThenBy(x => x.Mes))
                    {
                        int mesActual = (int)((EnumMeses)Enum.Parse(typeof(EnumMeses), mes.Mes.ToString()));
                        htmlBody += @"  <tr>
                        <td> " + mes.Mes + " - " + mes.Anio + "</td>";
                        htmlBody += dispFijacion ? "<td class='borde-izq-der'>" + mes.DispFijac.ToString("N0") + "</td>" : "";
                        htmlBody += forwFijacion ? "<td class='borde-izq-der'>" + mes.FrwFijac.ToString("N0") + "</td>" : "";
                        htmlBody += newcFijacion ? "<td class='borde-izq-der'>" + mes.NewFijac.ToString("N0") + "</td>" : "";
                        htmlBody += dispAFijar ? "<td class='borde-izq-der'>" + mes.DispAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += forwAFijar ? "<td class='borde-izq-der'>" + mes.FrwAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += newcAFijar ? "<td class='borde-izq-der'>" + mes.NewAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += dispAPrecio ? "<td class='borde-izq-der'>" + mes.DispAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += forwAPrecio ? "<td class='borde-izq-der'>" + mes.FrwAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += newcAPrecio ? "<td class='borde-izq-der'>" + mes.NewAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += totalPesos != 0 ? "<td class='' colspan='2'> " + (mes.KilosPesos.ToString("N0")) + "</td>" : "";
                        htmlBody += pondPesos > 0 ? "<td class='' colspan='2'> " + (mes.PrecioPonderadoPesos.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += totalDolares != 0 ? "<td class='' colspan='2'> " + (mes.KilosDolares.ToString("N0")) + "</td>" : "";
                        htmlBody += pondDolares > 0 ? "<td class='' colspan='2'> " + (mes.PrecioPonderadoDolares.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += "</tr>";
                    }
                    htmlBody += @"   <tr class='titulosPosicion total'>
                        <th>Total</th>";
                    htmlBody += dispFijacion ? "<td class=''>" + material.PosicionKilos.Sum(y => y.DispFijac).ToString("N0") + "</td>" : "";
                    htmlBody += forwFijacion ? "<td class=''>" + material.PosicionKilos.Sum(y => y.FrwFijac).ToString("N0") + "</td>" : "";
                    htmlBody += newcFijacion ? "<td class=''>" + material.PosicionKilos.Sum(y => y.NewFijac).ToString("N0") + "</td>" : "";
                    htmlBody += dispAFijar ? "<td class=''>" + material.PosicionKilos.Sum(y => y.DispAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += forwAFijar ? "<td class=''>" + material.PosicionKilos.Sum(y => y.FrwAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += newcAFijar ? "<td class=''>" + material.PosicionKilos.Sum(y => y.NewAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += dispAPrecio ? "<td class=''>" + material.PosicionKilos.Sum(y => y.DispAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += forwAPrecio ? "<td class=''>" + material.PosicionKilos.Sum(y => y.FrwAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += newcAPrecio ? "<td class=''>" + material.PosicionKilos.Sum(y => y.NewAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += totalPesos != 0 ? "<th class='negrita' colspan='2'>" + totalPesos.ToString("N0") + "</th>" : "";
                    htmlBody += pondPesos > 0 ? "<td class='' colspan='2'></td>" : "";
                    htmlBody += totalDolares != 0 ? "<th class='negrita' colspan='2'>" + totalDolares.ToString("N0") + "</th>" : "";
                    htmlBody += totalDolares != 0 ? "<td class='negrita' colspan='2'></td>" : "";
                    //htmlBody += "<td></td>  <td></td>";
                    htmlBody += @"</tr>
                    </tbody>
                </table><br><br>
            </div>";


                }
            }


            if (Model.AgenteCompras.ListaAgenteCompras.Count() > 0)
            {
                htmlBody += @"<table cellpadding='10' style=''>" +
                    "<tr class='Mat'>" +
                    "<th colspan = " + (Model.AgenteCompras.ListaOperadores.Count() + 4) + " > AGENTE DE COMPRA MAT </ th >" +
                    "<tbody>" +
                    "</tr>" +
                    "<tr>" +
                    "<th class='Mat'>Producto</th>" +
                    "<th class='Mat'>Posición</th>" +
                    "<th class='Mat'>Total</th>" +
                    "<th class='Mat'>USD Pond.</th>";
                foreach (var op in Model.AgenteCompras.ListaOperadores)
                {
                    htmlBody += "<th class='Mat'>" + op.OperadorDesc + "</th>";
                }
                htmlBody += "</tr>";
                foreach (var agente in Model.AgenteCompras.ListaAgenteCompras)
                {
                    var pos = agente.Posicion.Split('.');

                    htmlBody += @"<tr>" +
                                "<td>" + agente.MaterialDesc + "</td>" +
                                "<td>" + ((EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1]) + "</td>" +
                                "<td class='negrita'>" + agente.Operador.Sum(x => x.Cantidad).ToString("N0") + "</td>" +
                                "<td>" + agente.PrecioPonderado.ToString("N2") + "</td>";


                    foreach (var op in Model.AgenteCompras.ListaOperadores)
                    {
                        var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
                        htmlBody += "<td>" + (cantidad != null ? cantidad : "0") + "</td>";
                    }

                    htmlBody += "</tr>";
                }
                htmlBody += @"<tr> 
                 <th></th> 
                 <th> Total </th>
                 <th>" + Model.AgenteCompras.ListaAgenteCompras.Sum(x => x.Operador.Sum(y => y.Cantidad)).ToString("N0") + "</th> <th></th>";
                foreach (var op in Model.AgenteCompras.ListaOperadores)
                {
                    var cantidad = Model.AgenteCompras.ListaAgenteCompras.Sum(x => x.Operador.Where(y => y.OperadorId == op.OperadorId).Sum(y => y.Cantidad));
                    htmlBody += "<th class='negrita'>" + cantidad + "</th>";
                }
               htmlBody += "</tr>";
               htmlBody += @"</tbody></table><br><br>";
            }
            


            if (Model.SojaSustentable.Total > 0)
            {
                htmlBody += "<table  cellpadding='10' class='table-condensed ' id='SojaSustentable' style='width:50%;'>";
                htmlBody += "    <tbody>";
                htmlBody += "        <tr>";
                htmlBody += "            <th class='titulos' colspan='3'>SOJA SUSTENTABLE</th>";
                htmlBody += "        </tr>";
                htmlBody += "        <tr>";
                htmlBody += Model.SojaSustentable.Precio > 0 ? "<th class=''>A Precio</th>" : "";
                htmlBody += Model.SojaSustentable.Fijar > 0 ? "<th class=''>A Fijar</th>" : "";
                htmlBody += "            <th>Total</th>";
                htmlBody += "        </tr>";
                htmlBody += "        <tr>";
                htmlBody += Model.SojaSustentable.Precio > 0 ? "<td class=''>" + Model.SojaSustentable.Precio.ToString("N0") + "</td>" : "";
                htmlBody += Model.SojaSustentable.Fijar > 0 ? "<td class=''>" + Model.SojaSustentable.Fijar.ToString("N0") + "</td>  " : "";
                htmlBody += "            <th>" + Model.SojaSustentable.Total.ToString("N0") + "</th>";
                htmlBody += "        </tr>";
                htmlBody += "    </tbody>";
                htmlBody += "</table><br><br>";
            }

            htmlBody += @"<table cellpadding='10' class='table-condensed noSideMargin' id='MonedaKilo' style='width:50%;'>
                <tbody>";
            foreach (var moneda in Model.PrecioCantidad)
            {
                if (moneda.Cantidad > 0)
                {
                    htmlBody += "<tr>";
                    htmlBody += "<th>" + moneda.Moneda + "</th>";
                    htmlBody += "<td class='negrita'> " + moneda.Cantidad.Value.ToString("N2") + " </td>";
                    htmlBody += "</tr>";
                }

            }
            htmlBody += @"</tbody>
            </table><br><br>";



            htmlBody += "<br></br>";

            htmlBody += "<br></br>";


            return htmlBody;
        }


    }
}

