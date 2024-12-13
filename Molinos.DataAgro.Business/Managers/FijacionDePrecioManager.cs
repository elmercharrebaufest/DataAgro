using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business
{
    public class FijacionDePrecioManager : IFijacionDePrecioManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public FijacionDePrecioManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DatosIniAbmFijacionDePrecio TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);

            return new DatosIniAbmFijacionDePrecio()
            {
                Material = qry.GetMaterialCombo()
            };
        }

        public ResultIniFijacionDePrecio TraerTodoFijacionDePrecio()
        {
            var oResult = new ResultIniFijacionDePrecio();

            oResult.FijacionDePrecio = repositorio.Listar<FijacionDePrecio, FijacionDePrecioIni>(x => new FijacionDePrecioIni()
            {
                FijacionId = x.FijacionId,
                MatDescripcion = x.Material.Descripcion,
                Precio = x.Precio,
                Fecha = x.Fecha,
                ProveedorId = x.Proveedor.ProveedorId
            });

            return oResult;
        }

        public FijacionDePrecioDto TraerFijacionDePrecio(int intFijacionId)
        {
            return repositorio.Obtener<FijacionDePrecio, FijacionDePrecioDto>(x => x.FijacionId == intFijacionId,
                x => new FijacionDePrecioDto
                {
                    Fecha = x.Fecha,
                    FijacionId = x.FijacionId,
                    MaterialId = x.MaterialId,
                    Precio = x.Precio,
                    ProveedorId = x.ProveedorId
                }) ?? new FijacionDePrecioDto();
        }


        public Resultado GrabarFijacionDePrecio(FijacionDePrecio oFijacionDePrecio, string idActiveDirectory)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oFijacionDePrecio, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oFijacionDePrecio.FijacionId != 0)
            {
                var oFijacionDePrecioSave = repositorio.Obtener<FijacionDePrecio>(oFijacionDePrecio.FijacionId);
                oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
                oFijacionDePrecioSave.Fecha = oFijacionDePrecio.Fecha;
                oFijacionDePrecioSave.MaterialId = oFijacionDePrecio.MaterialId;
                oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;
            }
            else
            {
                repositorio.Agregar(oFijacionDePrecio);
            }

            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }


        public Resultado EliminarFijacionDePrecio(int intFijacionId)
        {
            var oEntityErrors = new Resultado();
            repositorio.Remover<FijacionDePrecio>(intFijacionId);
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }
    }
}