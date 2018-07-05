using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Domain;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class InformeComercialManager : IInformeComercialManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public InformeComercialManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<List<InformeComercialMaterialDisponible>> TraerInformeComercialAsync(int ProveedorId)
        {
            var query = await mobjUnitOfWork.SelStoreAsync<InformeComercialMaterialDisponible>("DataAgro_InformeComercial_Traer", ProveedorId).ToListAsync();

            return query;

        }

        public async Task<List<InformeGeneradoList>> TraerInformeComercialGeneradoAsync(int ProveedorId)
        {
            var query = await mobjUnitOfWork.SelStoreAsync<InformeGeneradoList>("DataAgro_InformeComercial_TraerInformesGenerado", ProveedorId).ToListAsync();
            query.ForEach(x => x.Materiales = x.Materiales.Replace("|", @"<br>"));
            return query;
        }

        public async Task<InformeResult> GrabarInformeComercial(ParamInformeComercial informe, int IdActiveDirectory  )
        {
            var oEntityErrors = new InformeResult();
            
            var oInformesComercial = mobjUnitOfWork.Repository<InformeComercial>().Queryable();
            var oInformesComercialProduccion = mobjUnitOfWork.Repository<InformeComercialProduccion>().Queryable();
            var oInformesComercialAlmacenamiento = mobjUnitOfWork.Repository<InformeComercialAlmacenamiento>().Queryable();

            var oCampo = mobjUnitOfWork.Repository<Campo>().Queryable().AsNoTracking();
            var oCampoMaterial= mobjUnitOfWork.Repository<CampoMaterial>().Queryable().AsNoTracking();
            var oAcopio = mobjUnitOfWork.Repository<Acopio>().Queryable().AsNoTracking();
            var oAcopioCampaña = mobjUnitOfWork.Repository<AcopioCampaña>().Queryable().AsNoTracking();

            oEntityErrors.errores = new EntityErrors();

            InformeComercial inf = new InformeComercial();
            int Id = 0;

            if (informe.InformeComercialId > 0)
            {
                var produccion = await oInformesComercialProduccion.Where(x => x.InformeComercialId == informe.InformeComercialId).ToListAsync();
                produccion.ForEach(x => mobjUnitOfWork.Repository<InformeComercialProduccion>().Delete(x));
                if (produccion.Count > 0) await mobjUnitOfWork.SaveChangesAsync();

                var almacenamiento = oInformesComercialAlmacenamiento.Where(x => x.InformeComercialId == informe.InformeComercialId).ToList();
                almacenamiento.ForEach(x => mobjUnitOfWork.Repository<InformeComercialAlmacenamiento>().Delete(x));
                if (almacenamiento.Count > 0) await mobjUnitOfWork.SaveChangesAsync();
            }


            // 1 - Tiene que grabar en informe Comercial
            if (informe.InformeComercialId == 0)
            {
                Id = oInformesComercial.AsNoTracking().Select(x => x.InformeComercialId)
                             .DefaultIfEmpty(0)
                             .Max() + 1;
                
                inf.InformeComercialId = Id;
                inf.ObjectState = Constants.Object_Added;
            }
            else
            {
                inf = await oInformesComercial.Where(x => x.InformeComercialId == informe.InformeComercialId).FirstOrDefaultAsync();
                inf.ObjectState = Constants.Object_Modified;
                Id = inf.InformeComercialId;
            }

            #region Informe Comercial
            oEntityErrors.InformeId = Id;
            inf.ProveedorId = informe.ProveedorId;
            inf.EstadoId = (int)EnumEstadoInforme.Generado;
            inf.CampañaId = (informe.InformeComercialId == 0? informe.CampañaId : inf.CampañaId) ;
            informe.CampañaId = (int)inf.CampañaId;
            inf.FechaAlta = DateTime.Now;
            inf.ComercialId = IdActiveDirectory;
            inf.EmplRelDep = informe.EmplRelDep;

            inf.EmplRelDepCant = informe.EmplRelDepCant;
            inf.Rodados = informe.Rodados;
            inf.RodadosOtros = informe.RodadosOtros;
            inf.Chacra = informe.Chacra;
            inf.AntigActividad = informe.AntigActividad;
            inf.ChacraOtros = informe.ChacraOtros;

            inf.ActuacionProd = informe.ActuacionProd;
            inf.ClienteAnt = informe.ClienteAnt;
            inf.Comentarios = informe.Comentarios;
            inf.DomicilioReal = informe.Domicilio;

            

            mobjUnitOfWork.Repository<InformeComercial>().SaveEntity(inf);
            #endregion

            // 2 - Tiene que grabar en informe Comercial Produccion
            
            int IdProduccion = oInformesComercialProduccion.AsNoTracking().Select(x => x.InformeComerciaProduccionId)
                             .DefaultIfEmpty(0)
                             .Max() + 1;

            foreach (var mat in informe.Materiales)
            { 
                var Produccion = oCampo
                        .Join(oCampoMaterial, a => a.CampoId, b => b.CampoId, (a, b) => new { C = a, CM = b })
                        .Where(x => x.C.ProveedorId == informe.ProveedorId  && x.CM.CampañaId == informe.CampañaId && x.CM.MaterialId == mat.MaterialId)
                        .GroupBy(z=> new { z.C.ArrendaPropia,z.C.LocalidadId,z.CM.MaterialId })
                        .Select(v => new InformeProduccionFac()
                            {
                                InformeComercialId = Id,
                                LocalidadId = v.Key.LocalidadId,
                                MaterialId = v.Key.MaterialId,
                                Propio = (v.Key.ArrendaPropia == 1 ? true : false),
                                Hectareas = v.Sum(q=> q.CM.Hectareas),
                                Toneladas = v.Sum(b=> b.CM.Toneladas)
                            }
                        ).ToList();

                
                foreach (var produ in Produccion)
                {

                    InformeComercialProduccion prod = new InformeComercialProduccion();
                    prod.InformeComerciaProduccionId = IdProduccion;
                    prod.InformeComercialId = produ.InformeComercialId;
                    prod.LocalidadId = produ.LocalidadId;
                    prod.MaterialId = produ.MaterialId;
                    prod.Hectareas = produ.Hectareas;
                    prod.Toneladas = produ.Toneladas;
                    prod.Propio = ((bool)produ.Propio ? true : false);
                    prod.Alquilado = ((bool)produ.Propio ? false : true);
                    prod.ObjectState = Constants.Object_Added;

                    mobjUnitOfWork.Repository<InformeComercialProduccion>().SaveEntity(prod);

                    IdProduccion += 1;

                }
                
            }
            // 3 - Tiene que grabar en informe Comercial Almacenamiento

            int IdAlmacenamiento = oInformesComercialAlmacenamiento.AsNoTracking().Select(x => x.InformeComercialAlmacenamientoId)
                             .DefaultIfEmpty(0)
                             .Max() + 1;

            var Almacemiento = await mobjUnitOfWork.SelStoreAsync<InformeAlmacenamientoFac>("DataAgro_InformeComercial_TraerAlmacenamiento", informe.ProveedorId, informe.CampañaId, Id).ToListAsync();
            
            foreach (var produ in Almacemiento)
            {

                InformeComercialAlmacenamiento prod = new InformeComercialAlmacenamiento();
                prod.InformeComercialAlmacenamientoId = IdAlmacenamiento;
                prod.InformeComercialId = produ.InformeComercialId;
                prod.LocalidadId = produ.LocalidadId;
                prod.Toneladas = produ.Toneladas;
                prod.Propia = ((bool)produ.Propio ? true : false);
                prod.Alquilada = ((bool)produ.Propio ? false : true);
                prod.ObjectState = Constants.Object_Added;

                mobjUnitOfWork.Repository<InformeComercialAlmacenamiento>().SaveEntity(prod);

                IdAlmacenamiento = +1;

            }

            try
            {
                await mobjUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }


            return oEntityErrors;
        }

        public async Task<RptInformeComercialInfo> GenerarInformeComercial(ParamInformeComercial informe, int InformeId)
        {
            var datos = new RptInformeComercialInfo();

            var oProveedorComercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
            var oSegmentacion = mobjUnitOfWork.Repository<Segmentacion>().Queryable().AsNoTracking();


            var oProveedor = mobjUnitOfWork.Repository<Proveedor>()
                                .Queryable()
                                .AsNoTracking()
                                .Where(x=> x.ProveedorId == informe.ProveedorId ).FirstOrDefault();

            datos.RazonSocial = oProveedor.RazonSocial;
            datos.CUIT = oProveedor.CUIT;
            datos.DomLegal = oProveedor.Direccion;
            datos.Telefono = oProveedor.Telefono1;
            datos.Email = oProveedor.Email1;

            var Comercial = oProveedorComercial
                                     .Where(x => x.ProveedorId == informe.ProveedorId)
                                     .Join(oComercial, a => a.ComercialId, b => b.ComercialId, (a, b) => b).FirstOrDefault();

            switch (Comercial.GrupoDeCompras)
            {
                case (int)AreasDeCompras.CORREDORBSAS:
                    datos.CorrBsAs = "X";
                    break;
                case (int)AreasDeCompras.CORREDORROSARIO:
                    datos.CorrRos = "X";
                    break;
                case (int)AreasDeCompras.ORIGINTERIORCENTRO:
                    datos.OrIntCentro = "X";
                    break;
                case (int)AreasDeCompras.ORIGINTERIORNORTE:
                    datos.OrIntNorte = "X";
                    break;
                case (int)AreasDeCompras.ORIGINTERIORSUR:
                    datos.OrIntSur = "X";
                    break;
            }

            var Contactos = await mobjUnitOfWork.Repository<ContactoComercial>()
                    .Queryable()
                    .AsNoTracking()
                    .Where(x => x.ProveedorId == informe.ProveedorId)
                    .Take(2)
                    .OrderByDescending(x=> x.EsPrincipal)
                    .Select(x=> new RptContactosInfo() {Nombres = x.Nombres + " " + x.Apellido,Cargo = x.Puesto,Telefono1 = x.Telefono1,Email1 = x.Email1 })
                    .ToListAsync();

            if (Contactos.Count >= 1)
            {
                datos.ContactoNombre1 = Contactos[0].Nombres + " " + Contactos[0].Apellido;
                datos.ContactoCargo1 = Contactos[0].Cargo;
                datos.ContactoTelefono1 = Contactos[0].Telefono1;
                datos.ContactoMail1 = Contactos[0].Email1;
            }

            if (Contactos.Count == 2)
            {
                datos.ContactoNombre2 = Contactos[1].Nombres + " " + Contactos[1].Apellido;
                datos.ContactoCargo2 = Contactos[1].Cargo;
                datos.ContactoTelefono2 = Contactos[1].Telefono1;
                datos.ContactoMail2 = Contactos[1].Email1;
            }

            var seg = await oSegmentacion.Where(x => x.SegmentacionId == oProveedor.SegmentacionId).FirstOrDefaultAsync();

            switch (seg.Grupo)
            {
                case "Acopiadores":
                    datos.Acopiadores = "X";
                    break;
                case "Canjeadores":
                    datos.Canjeadores = "X";
                    break;
                case "Corredores":
                    datos.Corredores = "X";
                    break;
                case "Exportadores":
                    datos.Exportadores = "X";
                    break;
                case "Grandes Cuentas":
                    datos.GrandesCuentas = "X";
                    break;
                case "Productores":
                    datos.Productores = "X";
                    break;
            }

            datos.Campaña = informe.Campaña;


            //Datos de Produccion 
            var oInformesComercial = mobjUnitOfWork.Repository<InformeComercial>().Queryable().AsNoTracking();
            var oInformesComercialProduccion = mobjUnitOfWork.Repository<InformeComercialProduccion>().Queryable().AsNoTracking();
            var oInformesComercialAlmacenamiento = mobjUnitOfWork.Repository<InformeComercialAlmacenamiento>().Queryable().AsNoTracking();
            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable().AsNoTracking();
            var oProvincia = mobjUnitOfWork.Repository<Provincia>().Queryable().AsNoTracking();
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking();


            var ProduccionInfo = new InformeComercialAcopiadores();
            var AlmacenamientoInfo = new InformeComercialAcopiadores();
            var oRptProduccionInfo = new List<InformeComercialAcopiadores>();
            var oRptAlmacenamientoInfo = new List<InformeComercialAcopiadores>();

            var ListProduccion = oInformesComercialProduccion
                .Join(oLocalidad, a => a.LocalidadId, b => b.LocalidadId, (a, b) => new { P = a, L = b })
                .Join(oProvincia, a => a.L.ProvinciaId, b => b.ProvinciaId, (a, b) => new { a.L, a.P, PR = b })
                .Join(oMaterial, a => a.P.MaterialId, b => b.MaterialId, (a, b) => new { a.L, a.P, a.PR, M = b })
                .Where(x => x.P.InformeComercialId == InformeId)
                .Select(x => new InformeComercialAcopiadores()
                    {
                        Grano=x.M.Descripcion,
                        Localidad=x.L.Nombre,
                        Provincia = x.PR.Nombre,
                        Toneladas= (int)x.P.Toneladas,
                        Superficie = (int)x.P.Hectareas,
                        Alquilado = (bool)x.P.Alquilado ? "X": String.Empty,
                        Propio = (bool)x.P.Propio ? "X" : String.Empty
                    }
                ).ToList();


            oRptProduccionInfo.AddRange(ListProduccion);
            
            //// Maiz
            var TonMaiz = oInformesComercialProduccion
                            .Where(x => x.InformeComercialId == InformeId && x.MaterialId == 1)
                            .GroupBy(z => z.MaterialId)
                            .Select(x => (float)x.Sum(a => a.Toneladas))
                            .DefaultIfEmpty(0)
                            .First();

            //// Soja
            var TonSoja = oInformesComercialProduccion
                            .Where(x => x.InformeComercialId == InformeId && x.MaterialId == 3)
                            .GroupBy(z => z.MaterialId)
                            .Select(x => (float)x.Sum(a => a.Toneladas))
                            .DefaultIfEmpty(0)
                            .First();

            //// Trigo
            var TonTrigo = oInformesComercialProduccion
                            .Where(x => x.InformeComercialId == InformeId && x.MaterialId == 2)
                            .GroupBy(z => z.MaterialId)
                            .Select(x => (float)x.Sum(a => a.Toneladas))
                            .DefaultIfEmpty(0)
                            .First();

            datos.ToneladasTodo = "Maiz " + TonMaiz.ToString("N2") + " Tn. / Soja " + TonSoja.ToString("N2") + " Tn. / Trigo " + TonTrigo.ToString("N2") + " Tn. ";


            var ListAlmacenamiento = oInformesComercialAlmacenamiento
                .Join(oLocalidad, a => a.LocalidadId, b => b.LocalidadId, (a, b) => new { P = a, L = b })
                .Join(oProvincia, a => a.L.ProvinciaId, b => b.ProvinciaId, (a, b) => new { a.L, a.P, PR = b })
                .Where(x => x.P.InformeComercialId == InformeId)
                .Select(x => new InformeComercialAcopiadores()
                {
                    Localidad = x.L.Nombre,
                    Provincia = x.PR.Nombre,
                    Toneladas = (int)x.P.Toneladas,
                    Alquilado = (bool)x.P.Alquilada ? "X" : String.Empty,
                    Propio = (bool)x.P.Propia ? "X" : String.Empty
                }
                ).ToList();

            oRptAlmacenamientoInfo.AddRange(ListAlmacenamiento);

            datos.Antiguedad = informe.AntigActividad;

            if (informe.EmplRelDep)
            {
                datos.RelacionDependenciaSi = "X";
                datos.RelacionDependenciaCant = informe.EmplRelDepCant;
            }
            else
                datos.RelacionDependenciaNo = "X";


            switch (informe.Rodados)
            {
                case (int)EnumRodados.EquipamientoPropio:
                    datos.RodadosEquipamientoPropio = "X";
                    break;
                case (int)EnumRodados.Alquilado:
                    datos.RodadosAlquilado = "X";
                    break;
                case (int)EnumRodados.PropioAlquilado:
                    datos.RodadosPropioYAlquilado = "X";
                    break;
                case (int)EnumRodados.Otros:
                    datos.RodadosOtros = "X";
                    datos.RodadosOtrosTexto = informe.RodadosOtros;
                    break;
            }

            switch (informe.Chacra)
            {
                case (int)EnumChacra.PersonalPropio:
                    datos.ServicioPersonalPropio = "X";
                    break;
                case (int)EnumChacra.PersonalContratado:
                    datos.ServicioPersonalContratado = "X";
                    break;
                case (int)EnumChacra.PersonalPropioContratado:
                    datos.ServicioPropioYContratado = "X";
                    break;
                case (int)EnumChacra.Otros:
                    datos.ServicioOtros = "X";
                    datos.ServicioOtrosTexto = informe.ChacraOtros;
                    break;
            }

            datos.Antiguedad = informe.AntigActividad;

            datos.ActuacionProduccion = informe.ActuacionProd;

            datos.ClientesAnteriores = informe.ClienteAnt;

            datos.DomReal = informe.Domicilio ;

            datos.Comentarios = informe.Comentarios;

            datos.CapProduccion = oRptProduccionInfo;

            datos.CapAlmacenaje = oRptAlmacenamientoInfo;

            return datos;
        }

        public async Task<ParamInformeComercial> ReimprimirInformeComercial(int InformeId)
        {

            
            var oParanInforme = new ParamInformeComercial();

            try
            {

                var oInformeComercial = await mobjUnitOfWork.Repository<InformeComercial>().Queryable().AsNoTracking().Where(x => x.InformeComercialId == InformeId).FirstOrDefaultAsync();

                var oCampaña = await mobjUnitOfWork.Repository<Campaña>().Queryable().AsNoTracking().Where(x => x.CampañaId == oInformeComercial.CampañaId).FirstOrDefaultAsync();
                oParanInforme.InformeComercialId = InformeId;
                oParanInforme.CampañaId = (int)oInformeComercial.CampañaId;
                oParanInforme.ProveedorId = oInformeComercial.ProveedorId;
                oParanInforme.Campaña = oCampaña.Descripcion;
                oParanInforme.AntigActividad = oInformeComercial.AntigActividad;
                oParanInforme.ActuacionProd = oInformeComercial.ActuacionProd;
                oParanInforme.ClienteAnt = oInformeComercial.ClienteAnt;
                oParanInforme.Domicilio = oInformeComercial.DomicilioReal;
                oParanInforme.Comentarios = oInformeComercial.Comentarios;

                if (oInformeComercial.Chacra != null)
                {
                    oParanInforme.Chacra = oInformeComercial.Chacra;
                    oParanInforme.ChacraOtros = oInformeComercial.ChacraOtros;
                }

                if (oInformeComercial.Rodados != null)
                {
                    oParanInforme.Rodados = oInformeComercial.Rodados;
                    oParanInforme.RodadosOtros = oInformeComercial.RodadosOtros;
                }

                if ((bool)oInformeComercial.EmplRelDep)
                {
                    oParanInforme.EmplRelDep = true;
                    oParanInforme.EmplRelDepCant = oInformeComercial.EmplRelDepCant;
                }
                else
                {
                    oParanInforme.EmplRelDep = false;
                }
            }
            catch (Exception ex)
            {

            }

            return oParanInforme;

        }

        public async Task<List<MaterialesModificacionInforme>> TraerInformeMaterialesAsync(int InformeId)
        {
            var list = await mobjUnitOfWork.SelStoreAsync<MaterialesModificacionInforme>("DataAgro_InformeComercial_TraerMaterialesAModificar", InformeId).ToListAsync();
            return list;
        }

        public async Task<List<ReportesList>> ListarReportes(ParamReportesIC oParam)
        {

            var query = await mobjUnitOfWork.SelStoreAsync<ReportesList>("DataAgro_InformeComercial_Reporte", oParam.Cuit, oParam.ComercialID, oParam.ComercialIDGenerador, oParam.MaterialID, oParam.EstadoId).ToListAsync();

            return query;

        }


  
        public async Task<List<InformeList>> TraerInformesGeneradosAsync()
        {

            var list = await mobjUnitOfWork.SelStoreAsync<InformeList>("DataAgro_InformeComercial_TraerExcelGeneracion").ToListAsync();
            list.ForEach(x => x.Materiales= x.Materiales.Replace("|", @"<br>"));
            return list;
        }

        public async Task<List<ResultCapacidadProductiva>> TraerCapacidadProductivaAsync(string informes)
        {

            var list = await mobjUnitOfWork.SelStoreAsync<ResultCapacidadProductiva>("DataAgro_InformeComercial_TraerCapacidadProductiva", informes).ToListAsync();

            return list;
        }

        public async Task<int> GrabarCapacidadProductivaAsync(string informes)
        {
            var val= await mobjUnitOfWork.SelStoreAsync<Result>("DataAgro_InformeComercial_GrabarCapacidadProductiva", informes).FirstOrDefaultAsync();
            return val.Res;
        }

        public async Task<EntityErrors> RespuestaDeSapCapacidadProductivaAsync(string cuit, string Material, string Respuesta)
        {
            EntityErrors error = new EntityErrors();

            try
            {
                
                var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
                var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking();
                var oInformeComercial = mobjUnitOfWork.Repository<InformeComercial>().Queryable().AsNoTracking();
                var oInformeComercialProduccion = mobjUnitOfWork.Repository<InformeComercialProduccion>().Queryable().AsNoTracking();

                var proveedorId = oProveedor.Where(x => x.CUIT == cuit).Select(x =>  x.ProveedorId).FirstOrDefault();

                var Mat = await oMaterial.Where(x => x.Codigo == Material).Select(z => new { MateriaId = z.MaterialId, CampañaId = z.CampañaId }).FirstOrDefaultAsync();

                var ListProduccion = await  oInformeComercialProduccion
                   .Join(oInformeComercial, a => a.InformeComercialId, b => b.InformeComercialId, (a, b) => new { P = a, I = b })
                   .Where(x => x.I.EstadoId == (int)EnumEstadoInforme.Enviado && x.I.ProveedorId== proveedorId
                                && x.I.CampañaId == Mat.CampañaId && x.P.MaterialId == Mat.MateriaId)
                   .Select(x => new
                       {
                           InformeComercialId = x.I.InformeComercialId,
                           InformeComercialProduccionId = x.P.InformeComerciaProduccionId
                       }
                   ).FirstOrDefaultAsync();

                var oInformeComercialProduccionSave = await mobjUnitOfWork.Repository<InformeComercialProduccion>().Queryable()
                        .Where(x=> x.InformeComerciaProduccionId == ListProduccion.InformeComercialProduccionId).FirstOrDefaultAsync();

                if (Respuesta.Length > 0)
                {
                    oInformeComercialProduccionSave.RtaOkSap = false;
                    oInformeComercialProduccionSave.MensajeSap = Respuesta;
                }
                else
                {
                    oInformeComercialProduccionSave.RtaOkSap = true;
                    oInformeComercialProduccionSave.MensajeSap = string.Empty;
                }

                oInformeComercialProduccionSave.ObjectState = Constants.Object_Modified;

                mobjUnitOfWork.Repository<InformeComercialProduccion>().SaveEntity(oInformeComercialProduccionSave);

                mobjUnitOfWork.SaveChanges();


                var list = oInformeComercialProduccion.Where(x => x.InformeComercialId == ListProduccion.InformeComercialId && (x.RtaOkSap == null || x.RtaOkSap == false) ).Select(x => x.InformeComerciaProduccionId).DefaultIfEmpty(0).FirstOrDefaultAsync().Result;

                if (list == 0 )
                {
                    var oInformeComercialSave = await mobjUnitOfWork.Repository<InformeComercial>().Queryable()
                            .Where(x => x.InformeComercialId == ListProduccion.InformeComercialId).FirstOrDefaultAsync();

                    oInformeComercialSave.EstadoId = (int)EnumEstadoInforme.Confirmado;

                    oInformeComercialSave.ObjectState = Constants.Object_Modified;

                    mobjUnitOfWork.Repository<InformeComercialProduccion>().SaveEntity(oInformeComercialProduccionSave);

                    mobjUnitOfWork.SaveChanges();
                }
            
            }
            catch (Exception ex)
            {
                var a = 1;
            }

            return error;
        }

        public async Task<EntityErrors> EliminarInformes(int InformeId)
        {
            EntityErrors error = new EntityErrors();

            var oInformeComercial = mobjUnitOfWork.Repository<InformeComercial>();
            var oInformeComercialProduccion = mobjUnitOfWork.Repository<InformeComercialProduccion>();
            var oInformeComercialAlmacenamiento = mobjUnitOfWork.Repository<InformeComercialAlmacenamiento>();

            var produccion = await oInformeComercialProduccion.Queryable().Where(x => x.InformeComercialId == InformeId).ToListAsync();
            produccion.ForEach(x=> oInformeComercialProduccion.Delete(x));
            if(produccion.Count> 0) await mobjUnitOfWork.SaveChangesAsync();

            var almacenamiento = oInformeComercialAlmacenamiento.Queryable().Where(x => x.InformeComercialId == InformeId).ToList();
            almacenamiento.ForEach(x => oInformeComercialAlmacenamiento.Delete(x));
            if (almacenamiento.Count > 0) await mobjUnitOfWork.SaveChangesAsync();

            var informe = await oInformeComercial.Queryable().Where(x => x.InformeComercialId == InformeId).SingleOrDefaultAsync();
            if (informe != null)
            {
                oInformeComercial.Delete(informe);
                await mobjUnitOfWork.SaveChangesAsync();
            }

            return error;
        }
    }

    public class InformeAlmacenamientoFac
    {
        public int InformeComercialId { get; set; }
        public Nullable<Decimal> Toneladas { get; set; }
        public Nullable<int> LocalidadId { get; set; }
        public Nullable<bool> Propio { get; set; }
    }

    public class InformeProduccionFac
    {
        public int InformeComercialId { get; set; }
        public Nullable<int> MaterialId { get; set; }
        public Nullable<int> Hectareas { get; set; }
        public Nullable<decimal> Toneladas { get; set; }
        public Nullable<int> LocalidadId { get; set; }
        public Nullable<bool> Propio { get; set; }

    }

    public class Result
    {
        public int Res { get; set; }
    }
}
