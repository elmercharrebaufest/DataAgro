using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerComprasHome : IConsulta<MaterialCampaña>
    {
        private readonly int idComercial;
        private readonly List<int> equipo;
        private readonly Campaña campañaAñoFiscal;

        public TraerComprasHome(int idComercial, List<int> equipo, Campaña campañaAñoFiscal)
        {
            this.idComercial = idComercial;
            this.equipo = equipo;
            this.campañaAñoFiscal = campañaAñoFiscal;
        }

        private static List<MaterialCampaña> Query(DbContext contexto, int comercialId, List<int> equipo, Campaña campañaAñoFiscal)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from x in contexto.Set<CampanaMaterialDetallePorMes>()
                where equipo.Contains(x.Comercial.ComercialId) && x.Material.CampañaId == x.CampanaId && campañaAñoFiscal.CampañaId == x.CampanaId
                group x by new { Material = x.Material.Descripcion, Campania = x.Campana.Descripcion } into g
                select new MaterialCampaña
                {
                    Nombre = g.Key.Material,
                    Toneladas = g.Sum(t => t.ToneladaAplicada),
                    Campaña = g.Key.Campania
                };
            //var resultado =
            //    from x in contexto.Set<CampanaMaterialDetallePorMes>()
            //    where equipo.Contains(x.Comercial.ComercialId) //&& x.Campana.Descripcion == x.CampanaDesc
            //    //&& x.ComercialId == comercialId
            //    group x by new { Material = x.Material.Descripcion, Campania = x.Campana.Descripcion } into g
            //    select new MaterialCampaña
            //    {
            //        Nombre = g.Key.Material,
            //        Toneladas = g.Sum(t => t.ToneladaAplicada),
            //        Campaña = g.Key.Campania
            //    };

            return resultado.OrderByDescending(x => x.Toneladas).ToList();
        }

        public virtual List<MaterialCampaña> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, idComercial, equipo, campañaAñoFiscal);
            }
        }
    }
}