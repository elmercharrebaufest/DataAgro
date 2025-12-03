using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business
{
    public class CubProveedoresManager : ICubProveedoresManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CubProveedoresManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DatosIniCubProveedores TraerDatosIniciales(List<int> equipo)
        {
            var qry = new CombosQueries(logger, repositorio);

            var oDatosIniciales = new DatosIniCubProveedores()
            {
                Proveedor = qry.GetProveedorPorComercialCombo(equipo),
                Provincia = qry.GetProvinciaCombo(),
                Localidad = qry.GetLocalidadCombo(),
                Estado = qry.GetEstadoCombo(),
                Segmentacion = qry.GetSegmentacionCombo(),
                Material = qry.GetMaterialCombo(),
                Comercial = qry.GetComercialCombo()
            };

            return oDatosIniciales;
        }


        public ParamCubProveedores TraerParam()
        {
            return new ParamCubProveedores();
        }


        public ResultCubProveedores TraerDatos(ParamCubProveedores oParam)
        {
            return new ResultCubProveedores
            {
                Proveedores = repositorio.SelStore<ProveedoresCub>("DataAgro_Proveedores_Cubo", 0,
                                                                     oParam.proveedorId,
                                                                     oParam.provinciaId,
                                                                     oParam.LocalidadId,
                                                                     oParam.EstadoId,
                                                                     oParam.SegmentacionId,
                                                                     oParam.material,
                                                                     oParam.ComercialId)
            };
        }
    }
}


