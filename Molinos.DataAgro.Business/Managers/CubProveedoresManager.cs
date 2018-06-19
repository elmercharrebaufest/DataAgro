using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Diagnostics;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business
{
    public class CubProveedoresManager : ICubProveedoresManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicialización
        //--------------------------------------------------

        public void Inicializar(MSContext oContexto)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }


        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = oUnitOfWork;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<DatosIniCubProveedores> TraerDatosInicialesAsync(string ActiveDirectory)
        {
            var qry = new CombosQueries(mobjUnitOfWork);
                     
            var oDatosIniciales = new DatosIniCubProveedores()
            {
                Proveedor = await qry.GetProveedorPorComercialComboAsync(ActiveDirectory),
                Provincia = await qry.GetProvinciaComboAsync(),
                Localidad = await qry.GetLocalidadComboAsync(),
                Estado = await qry.GetEstadoComboAsync(),
                Segmentacion = await qry.GetSegmentacionComboAsync(),
                Material = await qry.GetMaterialComboAsync(),
                Comercial = await qry.GetComercialComboAsync()
            };
            
            return oDatosIniciales;
        }


        public ParamCubProveedores TraerParam()
        {
            return new ParamCubProveedores();
        }


        public EntityErrors Validar(ParamCubProveedores oParam)
        {
            var oEntityErrors = new EntityErrors();

            oParam.Validate(oEntityErrors.ListaErrores);

            return oEntityErrors;
        }


        public async Task<ResultCubProveedores> TraerDatosAsync(ParamCubProveedores oParam)
        {
            var oResult = new ResultCubProveedores();

            var oEntityErrors = Validar(oParam);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                oResult.ListaErrores = oEntityErrors.ListaErrores;

                return oResult; 
            }
            
            ActualizarProveedoresCubo(oParam.ComercialId);

            var query = mobjUnitOfWork.SelStoreAsync<ProveedoresCub>("DataAgro_Proveedores_Cubo",
                                                                     oParam.proveedorId,
                                                                     oParam.provinciaId, 
                                                                     oParam.LocalidadId, 
                                                                     oParam.EstadoId, 
                                                                     oParam.SegmentacionId, 
                                                                     oParam.material, 
                                                                     oParam.ComercialId);

            oResult.Proveedores = await query.ToListAsync(); 
            
            return oResult;
        }

        public void ActualizarProveedoresCubo(int? ComercialId)
        {
            var query = mobjUnitOfWork.SelStoreAsync<Datos>("DataAgro_Proveedores_TraerPorComercial", ComercialId).ToList();

            if (query.Count > 0)
            {
                var compras = new ComprasManager();
                compras.Inicializar(mobjContexto);
                compras.ActualizarComprasProveedor(query);
            }

        }
    }
}


