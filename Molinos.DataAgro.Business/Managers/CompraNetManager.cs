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

namespace Molinos.DataAgro.Business {

    public class CompraNetManager : ICompraNetManager { 
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicialización
        //--------------------------------------------------

        public void Inicializar(MSContext oContexto) {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }


        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork) {
            mobjContexto = oContexto;

            mobjUnitOfWork = oUnitOfWork;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
                
        public async Task<DatosIniCompraNet> TraerDatosInicialesAsync(string ActiveDirectory) {
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniCompraNet() {
                Proveedor = await qry.GetProveedorPorComercialComboAsync(ActiveDirectory),
                Comercial = await qry.GetComercialComboAsync(),
                Material = await qry.GetMaterialComboAsync(),
                Provincia = await qry.GetProvinciaComboAsync(),
                Localidad = await qry.GetLocalidadComboAsync()
                //moneda
                //campaña
            };

            return oDatosIniciales;
        }

        /*

        public ParamCubProveedores TraerParam() {
            return new ParamCubProveedores();
        }
        

        public EntityErrors ValidarContrato(ParamCubProveedores oParam) {
            var oEntityErrors = new EntityErrors();

            oParam.Validate(oEntityErrors.ListaErrores);

            return oEntityErrors;
        }


        public EntityErrors ValidarFijacion(ParamCubProveedores oParam) {
            var oEntityErrors = new EntityErrors();

            oParam.Validate(oEntityErrors.ListaErrores);

            return oEntityErrors;
        }
        
        public async Task<ResultCompraNet> TraerDatosAsyncContrato(ParamCompraNet oParam) {
            var oResult = new ResultCompraNet();

            var oEntityErrors = ValidarContrato(oParam);

            if (oEntityErrors.ListaErrores.Count > 0) {
                oResult.ListaErrores = oEntityErrors.ListaErrores;

                return oResult;
            }

            ActualizarProveedoresCubo(oParam.ComercialId);

            var query = mobjUnitOfWork.SelStoreAsync<CompraNet>("DataAgro_Proveedores_Cubo",
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


        public async Task<ResultCompraNet> TraerDatosAsyncFijacion(ParamCompraNet oParam) {
            var oResult = new ResultCompraNet();

            var oEntityErrors = ValidarFijacion(oParam);

            if (oEntityErrors.ListaErrores.Count > 0) {
                oResult.ListaErrores = oEntityErrors.ListaErrores;

                return oResult;
            }

            ActualizarProveedoresCubo(oParam.ComercialId);

            var query = mobjUnitOfWork.SelStoreAsync<CompraNet>("DataAgro_Proveedores_Cubo",
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
        */

        /*
        public void ActualizarProveedoresCubo(int? ComercialId) {
            var query = mobjUnitOfWork.SelStoreAsync<Datos>("DataAgro_Proveedores_TraerPorComercial", ComercialId).ToList();

            if (query.Count > 0) {
                var compras = new ComprasManager();
                compras.Inicializar(mobjContexto);
                compras.ActualizarComprasProveedor(query);
            }

        }
        */
    }
}

