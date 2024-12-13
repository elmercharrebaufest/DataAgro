using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class CentroManager : ICentroManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CentroManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public DatosIniAbmCentro TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);
            return new DatosIniAbmCentro()
            {
                Centro = qry.GetAbmCentroCombo()
            };
        }

        public ResultIniCentro TraerTodoCentro()
        {
            return new ResultIniCentro
            {
                Centro = repositorio.Listar<Centro, CentroIni>(x => new CentroIni()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    CodigoSap = x.CodigoSap,
                    ValidaRedespacho = x.ValidaRedespacho,
                    Acopio = x.Acopio,
                    Localidad = x.Localidad != null ? x.Localidad.Nombre + " (" + x.Localidad.Provincia.Nombre + ")" : "Sin asignar",
                    LocalidadId = x.LocalidadId,
                    CodigoPostal = x.CodigoPostal,
                    Direccion = x.Direccion,
                    Comision = x.Comision,
                    CargaNegocios = x.CargaNegocios,
                    CargaCupos = x.CargaCupos,
                    NoPropio = x.NoPropio,
                    Orden = x.Orden,
                    CodigoConfirma = x.CodigoConfirma,
                    CUIT = x.CUIT,
                    RazonSocial = x.RazonSocial

                }, null, 0, "Descripcion")
            };
        }

        public CentroDto TraerCentro(int id)
        {
            return repositorio.Obtener<Centro, CentroDto>(x => x.Id == id, x => new CentroDto
            {
                Id = x.Id,
                CodigoSap = x.CodigoSap,
                Descripcion = x.Descripcion,
                Acopio = x.Acopio,
                ValidaRedespacho = x.ValidaRedespacho,
                Localidad = x.Localidad != null ? x.Localidad.Nombre + " (" + x.Localidad.Provincia.Nombre + ")" : "Sin asignar",
                LocalidadId = x.LocalidadId,
                CodigoPostal = x.CodigoPostal,
                Direccion = x.Direccion,
                Comision = x.Comision,
                CargaNegocios = x.CargaNegocios,
                CargaCupos = x.CargaCupos,
                NoPropio = x.NoPropio,
                Orden = x.Orden,
                CodigoConfirma = x.CodigoConfirma,
                CUIT = x.CUIT,
                RazonSocial = x.RazonSocial
            }) ?? new CentroDto();
        }

        public Resultado GrabarCentro(Centro oCentro)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oCentro, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oCentro.Id != 0)
            {
                var oCentroSave = repositorio.Obtener<Centro>(oCentro.Id);
                oCentroSave.Descripcion = oCentro.Descripcion;
                oCentroSave.CodigoSap = oCentro.CodigoSap;
                oCentroSave.Acopio = oCentro.Acopio;
                oCentroSave.ValidaRedespacho = oCentro.ValidaRedespacho;
                oCentroSave.LocalidadId = oCentro.LocalidadId;
                oCentroSave.Direccion = oCentro.Direccion;
                oCentroSave.CodigoPostal = oCentro.CodigoPostal;
                oCentroSave.Comision = oCentro.Comision;
                oCentroSave.CargaNegocios = oCentro.CargaNegocios;
                oCentroSave.CargaCupos = oCentro.CargaCupos;
                oCentroSave.NoPropio = oCentro.NoPropio;
                oCentroSave.Orden = oCentro.Orden;
                oCentroSave.CodigoConfirma = oCentro.CodigoConfirma;
                oCentroSave.CUIT = oCentro.CUIT;
                oCentroSave.RazonSocial = oCentro.RazonSocial;
            }
            else
            {
                repositorio.Agregar(oCentro);
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

            logger.Debug("Guardando el centro:" + oCentro.Descripcion);

            return oEntityErrors;
        }

        public Resultado EliminarCentro(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Centro>(id);

            logger.Debug("Eliminando el centro:" + id);
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
        public CentroDto ObtenerCentroPorCodigoSap(string codigoSap)
        {
            return repositorio.Obtener<Centro, CentroDto>(x => x.CodigoSap == codigoSap, x => new CentroDto { Id = x.Id, CodigoSap = x.CodigoSap, Descripcion = x.Descripcion, Acopio = x.Acopio, Comision = x.Comision, ValidaRedespacho = x.ValidaRedespacho }) ?? new CentroDto();
        }
    }
}

