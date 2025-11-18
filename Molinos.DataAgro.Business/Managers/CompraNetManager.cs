using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class CompraNetManager : ICompraNetManager
    {
        private readonly ILogger logger;
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
                Provincia = qry.GetProvinciaCombo(),
                Localidad = qry.GetLocalidadCombo(),
                MotivoAnterior = qry.GetAbmMotivoCombo()
            };

            return oDatosIniciales;
        }

        public GrabarSuscripcionResult GrabarSuscripcion(string key, int comercialId)
        {
            var oEntityErrors = new GrabarSuscripcionResult();
            if (!string.IsNullOrEmpty(key))
            {
                if (!repositorio.Listar<SuscripcionComercial>(x => x.Key == key).Any())
                {
                    SuscripcionComercial suscripcion = new SuscripcionComercial()
                    {
                        Key = key,
                        ComercialId = comercialId
                    };
                    repositorio.Agregar(suscripcion);
                }
            }
            else
            {
                var suscripcion = repositorio.Listar<SuscripcionComercial>(x => x.ComercialId == comercialId);
                foreach (var susc in suscripcion)
                {
                    repositorio.Remover(susc);
                }
            }
            repositorio.GuardarCambios();
            return oEntityErrors;
        }

        public bool UsuarioSuscripto(int comercialId)
        {
            return repositorio.Listar<SuscripcionComercial>(x => x.ComercialId == comercialId).Any();
        }
    }
}

