using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class ComercialManager : IComercialManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IDatoDelComercialAgent oDatoDelComercialAgent;

        public ComercialManager(ILogger logger, IRepositorio repositorio,IDatoDelComercialAgent oDatoDelComercialAgent)
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
            var subordinados = ListarEquipo(comercialId,lista);
            list.RemoveAll(x => subordinados.Any(z => z == x.ComercialId));
            return list;
        }


        public ResultIniComercial TraerTodoComercial()
        {
            return new ResultIniComercial
            {
                Comercial = repositorio.ObtenerConsultaEscalar(new TraerComerciales()).ToList()
            };
        }

        public ComercialDto TraerComercial(int intComercialId)
        {
            var comercial= repositorio.Obtener<Comercial, ComercialDto>(x => x.ComercialId == intComercialId, x =>
                new ComercialDto
                {
                    IdActiveDirectory = x.IdActiveDirectory,
                    GrupoDeComprasId = x.GrupoDeComprasId,
                    GrupoDeCompras= x.GrupoDeCompras.Descripcion,
                    Administrador = x.Administrador,
                    Apellido = x.Apellido,
                    ComercialId = x.ComercialId,
                    EmpleadorACargoId = x.EmpleadorACargoId,
                    Nombres = x.Nombres,
                    PerfilId = x.PerfilId ?? 0,
                    Cupera=x.Cupera,
                    RolesAsociados =   x.RolesAsociados.Select(y=> new RolBasicoDto { Descripcion = y.Descripcion, Id= y.Id}).ToList()                
                }) ?? new ComercialDto();
            return comercial;
        }


        public Resultado GrabarComercial(Comercial oComercial, List<Rol>roles)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oComercial, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain))
                {
                    var user = UserPrincipal.FindByIdentity(ctx, oComercial.IdActiveDirectory);

                    if (user == null)
                    {
                        oEntityErrors.Error("Usuario", "El usuario no existe en AD ");
                        return oEntityErrors;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            if (repositorio.Existe<Comercial>(x => x.IdActiveDirectory == oComercial.IdActiveDirectory && x.ComercialId != oComercial.ComercialId))
            {
                oEntityErrors.Error("Usuario", "El usuario de Active Directory Ya ha sido usado por otro comercial");
                return oEntityErrors;
            }
            if (roles == null)
            {
                oEntityErrors.Error("Roles", "Debe asignar algún rol");
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
                oComercialSave.Apellido = oComercial.Apellido;
                oComercialSave.Nombres = oComercial.Nombres;
                oComercialSave.Perfil = oComercial.Perfil;
                oComercialSave.EmpleadorACargoId = oComercial.EmpleadorACargoId;
                oComercialSave.IdActiveDirectory = oComercial.IdActiveDirectory;
                oComercialSave.Administrador = oComercial.Administrador;
                oComercialSave.GrupoDeCompras = oComercial.GrupoDeCompras;
                oComercialSave.PerfilId = oComercial.PerfilId;
                oComercialSave.Cupera = oComercial.Cupera;

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

            repositorio.Remover<Comercial>(intComercialId);

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
                x => comercial == "" || comerciales.Contains(x.ComercialId) && (x.Nombres.Contains(comercial) || x.Apellido.Contains(comercial)), 15);
        }

        public EnumPerfil ObtenerPerfilDeUsuario(string activeDirectoryId)
        {
            return (EnumPerfil)repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeDirectoryId, x => x.PerfilId.Value);
        }

        public bool EsAdministrador(string activeDirectoryId)
        {
            return repositorio.Obtener<Comercial, bool>(x => x.IdActiveDirectory == activeDirectoryId, x => x.Administrador ?? false);
        }
        public bool EsCupera(string activeDirectoryId)
        {
            return repositorio.Obtener<Comercial, bool>(x => x.IdActiveDirectory == activeDirectoryId, x => x.Cupera.HasValue ? x.Cupera.Value : false);
        }
        public EquipoDto ListarEquipo(string idActiveDirectory)
        {
            var comerciales = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, EmpleadorACargo = x.EmpleadorACargoId });
            var comercialId = repositorio.Obtener<Comercial,int>(x => x.IdActiveDirectory == idActiveDirectory,x=>x.ComercialId);
            var resultado = new EquipoDto
            {
                Equipo = !PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial) ? ListarEquipo(comercialId, comerciales) : repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso==PermisosDataAgro.VerCorredorComercial)))
            };
            resultado.EquipoReal = comerciales.Select(x => x.ComercialId).ToList();

            return resultado;
        }

        private static List<int> ListarEquipo(int comercialId, List<ComercialQry> comerciales)
        {
            var resultado = new List<int> { comercialId };
            foreach (var comercial in comerciales.Where(x => x.EmpleadorACargo == comercialId).ToList())
            {
                resultado.AddRange(ListarEquipo(comercial.ComercialId, comerciales));
            }
            return resultado;
        }
        public List<int> CadenaComerciales(int comercialId)
        {
            var listaSuperiores = new List<int>();
            var permiso = repositorio.Obtener<Comercial, bool>(x => x.ComercialId == comercialId, x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso == PermisosDataAgro.NotificacionesMailTodos)));
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
            var resultado = (PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial)) ? repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso == PermisosDataAgro.VerCorredorComercial))) : new List<int>();
            return resultado;
        }

        public List<Comercial> ListarComercialesCorredor()
        {
            return repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso==PermisosDataAgro.VerCorredorComercial)));
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
    }
}





