
using Mastersoft.Framework.DataRepository;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Molinos.DataAgro.Mapping.Context
{
    public partial class DataAgroContext : DataContext
    {
        static DataAgroContext()
        {
            Database.SetInitializer<DataAgroContext>(null);
        }

        public DataAgroContext(MSContext oContexto)
            : base(GetMSConectionString(oContexto))
        {
            //this.Database.Log = Console.WriteLine;  
        }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            MapearAssemblyDe<Contrato>(modelBuilder, x => x.Namespace == typeof(Contrato).Namespace,
                excluir: null);

            modelBuilder.Configurations.Add(new ErroresMap());
            modelBuilder.Configurations.Add(new ReportesMap());
            modelBuilder.Configurations.Add(new AreaInfluenciaMap());
            modelBuilder.Configurations.Add(new CanalOperacionMap());
            modelBuilder.Configurations.Add(new CondicionMap());
            modelBuilder.Configurations.Add(new DestinatarioMap());
            modelBuilder.Configurations.Add(new MaterialMap());
            modelBuilder.Configurations.Add(new ProvinciaMap());
            modelBuilder.Configurations.Add(new LocalidadMap());
            //modelBuilder.Configurations.Add(new ProveedorMap());
            modelBuilder.Configurations.Add(new ComercialMap());
            modelBuilder.Configurations.Add(new ProveedorComercialMap());
            modelBuilder.Configurations.Add(new AcopioMap());
            modelBuilder.Configurations.Add(new AcopioMaterialMap());
            modelBuilder.Configurations.Add(new AcopioCampañaMap());
            modelBuilder.Configurations.Add(new ActividadMap());
            modelBuilder.Configurations.Add(new CampañaMap());
            modelBuilder.Configurations.Add(new CampañaMaterialMap());
            modelBuilder.Configurations.Add(new CampoMap());
            modelBuilder.Configurations.Add(new CampoMaterialMap());
            modelBuilder.Configurations.Add(new ComercialZonaMap());
            modelBuilder.Configurations.Add(new ContactoComercialMap());
            modelBuilder.Configurations.Add(new ContactoComercialInteresMap());
            modelBuilder.Configurations.Add(new ProveedorCanalOperacionMap());
            modelBuilder.Configurations.Add(new ProveedorCondicionMap());
            modelBuilder.Configurations.Add(new ProveedorDestinatarioMap());
            modelBuilder.Configurations.Add(new SegmentacionMap());
            modelBuilder.Configurations.Add(new TipoActividadMap());
            modelBuilder.Configurations.Add(new TipoTelefonoMap());
            modelBuilder.Configurations.Add(new ZonaMap());
            modelBuilder.Configurations.Add(new RG2300Map());
            modelBuilder.Configurations.Add(new EstadoMap());
            modelBuilder.Configurations.Add(new InteresMap());
            modelBuilder.Configurations.Add(new FACACOPMap());
            modelBuilder.Configurations.Add(new CampañaMaterialPorMesMap());
            modelBuilder.Configurations.Add(new PerfilMap());
            modelBuilder.Configurations.Add(new GrupoDeComprasMap());
            modelBuilder.Configurations.Add(new CampañaMaterialHistoricoMap());
            modelBuilder.Configurations.Add(new ObjetivoMap());
            modelBuilder.Configurations.Add(new ProveedorEstadoMap());
            modelBuilder.Configurations.Add(new PostItMap());
            modelBuilder.Configurations.Add(new InformeComercialMap());
            modelBuilder.Configurations.Add(new InformeComercialProduccionMap());
            modelBuilder.Configurations.Add(new InformeComercialAlmacenamientoMap());
            modelBuilder.Configurations.Add(new FijacionDePrecioMap());
            modelBuilder.Configurations.Add(new InformeComercialEstadoMap());
            modelBuilder.Configurations.Add(new MonedaMap());
            modelBuilder.Configurations.Add(new TipoNegocioMap());
            modelBuilder.Configurations.Add(new ContratoMap());
            modelBuilder.Configurations.Add(new FijacionDePrecioContratoMap());
            modelBuilder.Configurations.Add(new EstadoContratoMap());
            modelBuilder.Configurations.Add(new ClasificacionCompraNetMap());
            modelBuilder.Configurations.Add(new BoletoCompraNetMap());
            modelBuilder.Configurations.Add(new BolsaCompraNetMap());
            modelBuilder.Configurations.Add(new CentroMap());
            modelBuilder.Configurations.Add(new CondicionFijacionMap());
            modelBuilder.Configurations.Add(new CalidadesEspecialesMap());
            modelBuilder.Configurations.Add(new StandardDeCalidadMap());
            modelBuilder.Configurations.Add(new DescuentosBonificacionesMap());
            modelBuilder.Configurations.Add(new TipoDBMap());
            modelBuilder.Configurations.Add(new TipoPeriodoDBMap());
        }

        private void MapearAssemblyDe<TEntidad>(DbModelBuilder modelBuilder, Predicate<Type> incluir, Predicate<Type> excluir)
        {
            var tiposEntidades = typeof(TEntidad).Assembly.GetTypes()
                .Where(x => incluir(x));
            if (excluir != null)
            {
                tiposEntidades = tiposEntidades.Where(x => !excluir(x));
            }

            tiposEntidades = tiposEntidades.Where(x => x.Name == "Proveedor");
           
            var metodo = modelBuilder.GetType().GetMethod("Entity");
            foreach (var tipoEntidad in tiposEntidades)
            {
                metodo.MakeGenericMethod(tipoEntidad).Invoke(modelBuilder, null);
            }
        }
    }
}































