using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{

    public class CompraNetManager : ICompraNetManager
    { 
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public CompraNetManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<DatosIniCompraNet> TraerDatosInicialesAsync(string activeDirectory)
        {
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniCompraNet() {
                Proveedor = await qry.GetProveedorPorComercialComboAsync(activeDirectory),
                Comercial = await qry.GetComercialComboAsync(),
                Material = await qry.GetMaterialComboAsync(),
                Provincia = await qry.GetProvinciaComboAsync(),
                Localidad = await qry.GetLocalidadComboAsync()
            };

            return oDatosIniciales;
        }

    }
}

