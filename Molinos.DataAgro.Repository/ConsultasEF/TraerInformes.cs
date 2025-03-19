using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerInformes : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        public TraerInformes(DataSourceRequest filtro)
        {
            request = filtro;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest filtro)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from x in contexto.Set<InformeComercialProduccion>()
                select new InformeProduccionList
                {
                    InformeComerciaProduccionId = x.InformeComerciaProduccionId,
                    InformeComercialId = x.InformeComercialId,
                    Cuit = x.InformeComercial.Proveedor.CUIT,
                    RazonSocial = x.InformeComercial.Proveedor.RazonSocial,
                    Campaña = x.InformeComercial.Campaña.Descripcion,
                    Material = x.Material.Descripcion,
                    MaterialId = (int)x.MaterialId,
                    MaterialSAP = x.Material.Codigo,
                    Toneladas = x.Toneladas,
                    Comercial = x.InformeComercial.Comercial.Nombres + " " + x.InformeComercial.Comercial.Apellido,
                    Seleccionado = false,
                    ProveedorId = x.InformeComercial.ProveedorId,
                    CampanaId = x.InformeComercial.CampañaId,
                    ComercialId = x.InformeComercial.ComercialId,
                    FechaAlta = DbFunctions.TruncateTime(x.InformeComercial.FechaAlta),
                    FechaAltaConHora = x.InformeComercial.FechaAlta,
                    FechaDescarga = x.FechaDescarga != null? true: false,
                    FechaDescargaConHora = x.FechaDescarga,
                    OrigenDA = x.InformeComercial.OrigenDA == true ? true: false,
                    UsuarioSAP = x.InformeComercial.Comercial.IdUsuarioSAP,
                    OrigenDAVal = x.InformeComercial.OrigenDA == true ? "Si" : "No",
                    FechaDescargaVal = DbFunctions.TruncateTime(x.FechaDescarga)
                };
            resultado = resultado.GroupBy(a => new { a.InformeComercialId, a.MaterialId }).Select(x =>
               new InformeProduccionList
               {
                   InformeComerciaProduccionId = x.FirstOrDefault().InformeComerciaProduccionId,
                   InformeComercialId = x.FirstOrDefault().InformeComercialId,
                   Cuit = x.FirstOrDefault().Cuit,
                   RazonSocial = x.FirstOrDefault().RazonSocial,
                   Campaña = x.FirstOrDefault().Campaña,
                   Material = x.FirstOrDefault().Material,
                   MaterialId = x.FirstOrDefault().MaterialId,
                   MaterialSAP = x.FirstOrDefault().MaterialSAP,
                   Toneladas = x.Sum(a => a.Toneladas),
                   Comercial = x.FirstOrDefault().Comercial,
                   Seleccionado = false,
                   ProveedorId = x.FirstOrDefault().ProveedorId,
                   CampanaId = x.FirstOrDefault().CampanaId,
                   ComercialId = x.FirstOrDefault().ComercialId,
                   FechaAlta = x.FirstOrDefault().FechaAlta,
                   FechaAltaConHora = x.FirstOrDefault().FechaAltaConHora,
                   FechaDescarga = x.FirstOrDefault().FechaDescarga,
                   FechaDescargaConHora = x.FirstOrDefault().FechaDescargaConHora,
                   OrigenDA = x.FirstOrDefault().OrigenDA,
                   UsuarioSAP = x.FirstOrDefault().UsuarioSAP,
                   OrigenDAVal = x.FirstOrDefault().OrigenDAVal,
                   FechaDescargaVal = x.FirstOrDefault().FechaDescargaVal
               }
            );
            return resultado.ToDataSourceResult(filtro);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
