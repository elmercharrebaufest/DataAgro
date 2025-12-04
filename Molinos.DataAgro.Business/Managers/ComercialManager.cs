using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class ComercialManager : IComercialManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IDatoDelComercialAgent oDatoDelComercialAgent;

        public ComercialManager(ILogger logger, IRepositorio repositorio, IDatoDelComercialAgent oDatoDelComercialAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oDatoDelComercialAgent = oDatoDelComercialAgent;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public DatosIniAbmComercial TraerDatosIniciales()
        {
            //si es alta le mandoo cero, si modifico le mando el id desde la grilla
            var qry = new CombosQueries(logger, repositorio);

            return new DatosIniAbmComercial()
            {
                Comercial = ObtenerComerciales(new List<int>(), 0),
                Perfil = qry.GetPerfilCombo(),
                GrupoDeCompras = qry.GetGrupoDeComprasCombo(),
                Rol = qry.GetRolCombo()
            };
        }

        public List<ComercialCombo> ObtenerComerciales(List<int> equipo, int comercialId)
        {
            var qry = new CombosQueries(logger, repositorio);
            var empleadorId = repositorio.Obtener<Comercial, int?>(x => x.ComercialId == comercialId, x => x.EmpleadorACargoId) ?? 0;
            var list = qry.GetAbmComercialCombo();
            var lista = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, EmpleadorACargo = x.EmpleadorACargoId });
            var subordinados = ListarEquipo(comercialId, lista);
            list.RemoveAll(x => subordinados.Any(z => z == x.ComercialId));
            return list;
        }

        public ResultIniComercial TraerTodoComercial()
        {
            var result = repositorio.ObtenerConsultaEscalar(new TraerComerciales()).ToList();
            foreach (var item in result)
            {
                var equipo = ListarEquipo(item.IdActiveDirectory).Equipo;
                item.Equipo = repositorio.Listar<Comercial, ComercialCombo>(a => new ComercialCombo { ComercialId = a.ComercialId, Apellido = a.Apellido + " " + a.Nombres }, a => equipo.Contains(a.ComercialId)).ToList();
            }
            return new ResultIniComercial
            {
                Comercial = result
            };
        }
        public ComercialDto TraerComercial(string email)
        {
            var comercial = repositorio.Obtener<Comercial, int>(x => x.Email == email, x => x.ComercialId);
            var comercialDto = TraerComercial(comercial);
            return comercialDto;
        }
        public ComercialDto TraerComercial(int intComercialId)
        {
            var comercial = repositorio.Obtener<Comercial, ComercialDto>(x => x.ComercialId == intComercialId, x =>
                 new ComercialDto
                 {
                     IdActiveDirectory = x.IdActiveDirectory,
                     IdUsuarioSAP = x.IdUsuarioSAP,
                     GrupoDeComprasId = x.GrupoDeComprasId,
                     GrupoDeCompras = x.GrupoDeCompras.Descripcion,
                     Administrador = x.Administrador,
                     Apellido = x.Apellido,
                     ComercialId = x.ComercialId,
                     EmpleadorACargoId = x.EmpleadorACargoId,
                     Nombres = x.Nombres,
                     PerfilId = x.PerfilId ?? 0,
                     Deshabilitado = x.Deshabilitado ?? false,
                     FechaDeshabilitado = x.FechaDeshabilitado,
                     Cupera = x.Cupera,
                     RolesAsociados = x.RolesAsociados.Select(y => new RolBasicoDto { Descripcion = y.Descripcion, Id = y.Id }).ToList(),
                     AsignarNegocios = x.AsignarNegocios,
                     Email = x.Email,
                     ComercialSuplenteId = x.ComercialSuplenteId
                 }) ?? new ComercialDto();
            return comercial;
        }

        public Resultado GrabarComercial(Comercial oComercial, List<Rol> roles)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oComercial, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            try
            {
                if (oComercial.Deshabilitado != true)
                {
                    using (var ctx = new PrincipalContext(ContextType.Domain))
                    {
                        var user = UserPrincipal.FindByIdentity(ctx, oComercial.IdActiveDirectory);

                        if (user == null)
                        {
                            oEntityErrors.Error("Usuario", "El usuario no existe en Active Directory.");
                            return oEntityErrors;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            if (repositorio.Existe<Comercial>(x => x.IdActiveDirectory == oComercial.IdActiveDirectory && x.ComercialId != oComercial.ComercialId))
            {
                oEntityErrors.Error("Usuario", "El usuario de Active Directory ya ha sido usado por otro comercial.");
                return oEntityErrors;
            }
            if (roles == null)
            {
                oEntityErrors.Error("Roles", "Debe asignar al menos un rol.");
                return oEntityErrors;
            }

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "0")
            {
                var comercial = oDatoDelComercialAgent.ObtenerDatosDeComercial(oComercial.IdActiveDirectory);

                if (comercial != null)
                {
                    try
                    {
                        oComercial.GrupoDeCompras = VerificarGrupoComercial(comercial.EX_ZONA);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        throw;
                    }

                }
            }
            var listaRoles = roles.Select(y => y.Id).ToList();
            oComercial.RolesAsociados = repositorio.Listar<Rol>(x => listaRoles.Any(y => y == x.Id));

            if (oComercial.ComercialId != 0)
            {
                var oComercialSave = repositorio.Obtener<Comercial>(oComercial.ComercialId);
                if (oComercial.Deshabilitado != oComercialSave.Deshabilitado)
                {
                    if (oComercial.Deshabilitado == true)
                    {
                        oComercialSave.FechaDeshabilitado = DateTime.Now;
                    }
                    else
                    {
                        oComercialSave.FechaDeshabilitado = null;
                    }
                }
                oComercialSave.Apellido = oComercial.Apellido;
                oComercialSave.Nombres = oComercial.Nombres;
                oComercialSave.Perfil = oComercial.Perfil;
                oComercialSave.EmpleadorACargoId = oComercial.EmpleadorACargoId;
                oComercialSave.IdActiveDirectory = oComercial.IdActiveDirectory;
                oComercialSave.IdUsuarioSAP = oComercial.IdUsuarioSAP;
                oComercialSave.Administrador = oComercial.Administrador;
                oComercialSave.GrupoDeComprasId = oComercial.GrupoDeComprasId;
                oComercialSave.Deshabilitado = oComercial.Deshabilitado;
                oComercialSave.PerfilId = oComercial.PerfilId;
                oComercialSave.Cupera = oComercial.Cupera;
                oComercialSave.AsignarNegocios = oComercial.AsignarNegocios;
                oComercialSave.Email = oComercial.Email;
                oComercialSave.ComercialSuplenteId = oComercial.ComercialSuplenteId;
                if (oComercialSave.RolesAsociados != null)
                {
                    oComercialSave.RolesAsociados.Clear();
                }
                else
                {
                    oComercialSave.RolesAsociados = new List<Rol>();
                }
                foreach (var rol in oComercial.RolesAsociados)
                {
                    oComercialSave.RolesAsociados.Add(rol);
                }
            }
            else
            {
                if (oComercial.Deshabilitado == true)
                {
                    oComercial.FechaDeshabilitado = DateTime.Now;
                }
                repositorio.Agregar(oComercial);
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

        private GrupoDeCompras VerificarGrupoComercial(string grupoDeCompra)
        {
            return repositorio.Obtener<GrupoDeCompras>(x => x.Descripcion == grupoDeCompra) ?? repositorio.Agregar(new GrupoDeCompras()
            {
                Descripcion = grupoDeCompra,
            });
        }

        public Resultado EliminarComercial(int intComercialId)
        {
            var oEntityErrors = new Resultado();

            var comercial = repositorio.Obtener<Comercial>(intComercialId);
            comercial.Deshabilitado = true;
            comercial.FechaDeshabilitado = DateTime.Now;
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

        public bool ComercialExiste(string ActiveDirectoryId)

        {
            return repositorio.Existe<Comercial>(x => x.IdActiveDirectory == ActiveDirectoryId);
        }

        public bool ComercialPerteneceProveedor(List<int> equipo, int proveedorId, List<int> corredoresComercial)
        {
            var corredor = PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial);
            return repositorio.Existe<ProveedorComercial>(x => x.Proveedor.ProveedorId == proveedorId
                        && (equipo.Contains(x.Comercial.ComercialId) ||
                        (corredor && x.Proveedor.Segmentacion.Grupo == "Corredores")) ||
                        (corredor && corredoresComercial.Contains(x.Comercial.ComercialId)));
        }

        public List<ComercialDto> ListarComercial(string comercial, List<int> comerciales)
        {
            return repositorio.Listar<Comercial, ComercialDto>(x => new ComercialDto { ComercialId = x.ComercialId, Nombres = x.Nombres, Apellido = x.Apellido },
                x => comercial == "" || comerciales.Contains(x.ComercialId) || (x.Nombres.Contains(comercial) || x.Apellido.Contains(comercial)));
        }

        public EnumPerfil ObtenerPerfilDeUsuario(string activeDirectoryId)
        {
            return (EnumPerfil)repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeDirectoryId, x => x.PerfilId ?? 0);
        }

        public bool EsAdministrador(string activeDirectoryId)
        {
            return repositorio.Obtener<Comercial, bool>(x => x.IdActiveDirectory == activeDirectoryId, x => x.Administrador ?? false);
        }
        public bool EsCupera(string activeDirectoryId)
        {
            return repositorio.Obtener<Comercial, bool>(x => x.IdActiveDirectory == activeDirectoryId, x => x.Cupera ?? false);
        }
        public EquipoDto ListarEquipo(string idActiveDirectory)
        {
            var comerciales = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, EmpleadorACargo = x.EmpleadorACargoId });
            var comercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == idActiveDirectory, x => x.ComercialId);
            var resultado = new EquipoDto
            {
                Equipo = !PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial) ?

                ListarEquipo(comercialId, comerciales) : repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)))
            };
            resultado.EquipoReal = comerciales.Select(x => x.ComercialId).ToList();

            return resultado;
        }

        private static List<int> ListarEquipo(int comercialId, List<ComercialQry> comerciales)
        {
            var resultado = new List<int> { comercialId };
            if (!PermisosHelper.Is(PermisosDataAgro.VerJerarquia) && !PermisosHelper.Is(PermisosDataAgro.VerTodos))
            {
                return resultado;
            }
            foreach (var comercial in comerciales.Where(x => x.EmpleadorACargo == comercialId).ToList())
            {
                resultado.AddRange(ListarEquipo(comercial.ComercialId, comerciales));
            }
            return resultado;
        }

        public List<int> CadenaComerciales(int comercialId)
        {
            var listaSuperiores = new List<int>();
            var permiso = repositorio.Obtener<Comercial, bool>(x => x.ComercialId == comercialId,
                x => x.Deshabilitado != true &&
                     x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.NotificacionesMailTodos)));
            if (permiso)
            {
                listaSuperiores.Add(comercialId);
            }
            ObtenerCadenaUsuarios(comercialId, listaSuperiores);

            return listaSuperiores;
        }

        private List<int> ObtenerCadenaUsuarios(int comercialId, List<int> listaSuperiores)
        {
            var comercial = repositorio.Obtener<Comercial>(comercialId);
            if (!listaSuperiores.Contains(comercial.ComercialId) &&
                comercial.RolesAsociados.Any(x => x.PermisosAsociados
                .Any(y => y.Permiso == PermisosDataAgro.NotificacionesMailTodos || y.Permiso == PermisosDataAgro.NotificacionesMailJerarquia)))
            {
                listaSuperiores.Add(comercial.ComercialId);
            }

            if (comercial.EmpleadorACargo != null)
            {
                ObtenerCadenaUsuarios(comercial.EmpleadorACargo.ComercialId, listaSuperiores);
            }
            return listaSuperiores;
        }
        public int ObtenerComercialId(string idActiveDirectory)
        {
            return repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == idActiveDirectory, x => x.ComercialId);
        }

        public List<int> ListarCorredoresComercial()
        {
            var resultado = (PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial)) ? repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.Deshabilitado != true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial))) : new List<int>();
            return resultado;
        }

        public List<Comercial> ListarComercialesCorredor()
        {
            return repositorio.Listar<Comercial>(x => x.Deshabilitado != true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)));
        }

        public List<Comercial> ListarComercialesSinRecibirMail()
        {
            return repositorio.Listar<Comercial>(x => x.Deshabilitado != true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.NoRecibirMail)));
        }

        public List<Comercial> ListarComercialesOyTNorte()
        {
            return repositorio.Listar<Comercial>(x => x.Deshabilitado != true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.OyT_Norte)));
        }

        public List<GrupoDeCompras> ListarGrupoDeCompras(string filtro)
        {
            if (filtro != null)
            {
                return repositorio.Listar<GrupoDeCompras>(x => x.Descripcion.Contains(filtro));
            }
            else
            {
                return repositorio.Listar<GrupoDeCompras>();
            }
        }
        public int ComercialAsociado(int proveedorId)
        {
            var comercial = repositorio.Obtener<ProveedorComercial, int>(x => x.ProveedorId == proveedorId, x => x.ComercialId);
            return comercial;
        }

        public List<ComercialDto> TraerComercialesProveedor(int proveedorId)
        {
            var comercial = repositorio.Listar<ProveedorComercial, ComercialDto>(
                x => new ComercialDto { ComercialId = x.ComercialId, Nombres = x.Comercial.Nombres, Apellido = x.Comercial.Apellido },
                x => x.ProveedorId == proveedorId);
            return comercial;
        }

        public int TraerZonaDelComercialAsociado()
        {
            var cuit = PermisosHelper.ObtenerCuit();
            var comercial = repositorio.Obtener<ProveedorComercial, string>(x => x.Proveedor.CUIT == cuit, x => x.Comercial.GrupoDeCompras.Descripcion);
            return repositorio.Obtener<ZonaCupo, int>(x => x.Descripcion == comercial, x => x.Id);
        }

        public List<ComercialQry> ListarComercialesAsignanNegocios()
        {
            return repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry()
            {
                ComercialId = x.ComercialId,
                Comercial = x.Nombres + " " + x.Apellido,
                IdActiveDirectory = x.IdActiveDirectory
            },
              x => x.Deshabilitado != true && ((x.AsignarNegocios == true &&
              x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ListaComercialCompraNet)))
              || x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.AltaCupos))), 0, "Comercial");
            //se listan también los usuarios con rol Cupos
        }

        public List<Comercial> ListarComercialesRecibirSugerenciaFAQ()
        {
            return repositorio.Listar<Comercial>(x => x.Deshabilitado != true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.Recibir_Mail_SugerenciaFAQ)));
        }

        public DataSourceResult TraerComercialesProveedorReporte(int? proveedorId, DataSourceRequest request)
        {
            var comerciales = repositorio.ObtenerConsultaEscalar(new BusquedaContactosComercialReporte(request, proveedorId));

            return comerciales;
        }

        public List<string> ObtenerPermisosPorEmail(string email)
        {
            email = email.ToLower();
            var usuario = repositorio.ObtenerNoTracking<Comercial>(u => u.Email == email);
            if (usuario != null)
            {
                var permisos = usuario.RolesAsociados
                    .SelectMany(rol => rol.PermisosAsociados)
                    .Distinct()
                    .ToList();
                return permisos.Select(a => a.Permiso.ToString()).Distinct().ToList();
            }
            return new List<string>();
        }
    }
}