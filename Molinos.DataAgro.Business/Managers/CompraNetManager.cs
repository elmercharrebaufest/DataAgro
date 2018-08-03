using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business
{

    public class CompraNetManager : ICompraNetManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public CompraNetManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DatosIniCompraNet TraerDatosIniciales(List<int> equipo)
        {
            var qry = new CombosQueries(logger, repositorio);

            var oDatosIniciales = new DatosIniCompraNet()
            {
                Proveedor = qry.GetProveedorPorComercialCombo(equipo),
                Comercial = qry.GetComercialCombo(),
                Material = qry.GetMaterialCombo(),
                Provincia =  qry.GetProvinciaCombo(),
                Localidad = qry.GetLocalidadCombo()
            };

            return oDatosIniciales;
        }
    }
}

