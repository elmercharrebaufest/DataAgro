using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
                GrupoDeCompras = qry.GetGrupoDeComprasCombo()
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
                Comercial = repositorio.Listar<Comercial, ComercialIni>(x => new ComercialIni()
                {
                    ComercialId = x.ComercialId,
                    Apellido = x.Apellido,
                    Nombres = x.Nombres,
                    PerDescripcion = x.Perfil.Descripcion
                }, null, 0, "Apellido")
            };
        }

        public ComercialDto TraerComercial(int intComercialId)
        {
            return repositorio.Obtener<Comercial, ComercialDto>(x => x.ComercialId == intComercialId, x =>
                new ComercialDto
                {
                    IdActiveDirectory = x.IdActiveDirectory,
                    GrupoDeComprasId = x.GrupoDeComprasId,
                    Administrador = x.Administrador,
                    Apellido = x.Apellido,
                    ComercialId = x.ComercialId,
                    EmpleadorACargoId = x.EmpleadorACargoId,
                    Nombres = x.Nombres,
                    PerfilId = x.PerfilId,
                    Cupera=x.Cupera                    
                }) ?? new ComercialDto();
        }


        public Resultado GrabarComercial(Comercial oComercial)
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
            if (oComercial.PerfilId == (int)EnumPerfil.Director && repositorio.Existe<Comercial>(x => x.PerfilId == (int)EnumPerfil.Director && x.ComercialId != oComercial.ComercialId))
            {
                oEntityErrors.Error("Perfil", "Ya existe un Director");
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

        public bool ComercialPerteneceProveedor(List<int> equipo, int proveedorId, int perfilId, List<int> corredoresComercial)
        {
            return repositorio.Existe<ProveedorComercial>(x => x.Proveedor.ProveedorId == proveedorId
                        && (equipo.Contains(x.Comercial.ComercialId) ||
                        (perfilId == (int)EnumPerfil.Administrativo && x.Proveedor.Segmentacion.Grupo == "Corredores") ||
                        (perfilId == (int)EnumPerfil.CorredoresComercial && corredoresComercial.Contains(x.Comercial.ComercialId)))
                        );
        }

        public List<ComercialDto> ListarComercial(string comercial, List<int> comerciales)
        {
            return repositorio.Listar<Comercial, ComercialDto>(x => new ComercialDto { ComercialId = x.ComercialId, Nombres = x.Nombres, Apellido = x.Apellido },
                x => comercial == "" || comerciales.Contains(x.ComercialId) && (x.Nombres.Contains(comercial) || x.Apellido.Contains(comercial)), 15);
        }

        public EnumPerfil ObtenerPerfilDeUsuario(string activeDirectoryId)
        {
            return (EnumPerfil)repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeDirectoryId, x => x.PerfilId);
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

            var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == idActiveDirectory);
            if (comercial.PerfilId == 4 || comercial.PerfilId == 5)
            {
                comercial = repositorio.Obtener<Comercial>(x => x.PerfilId == 3);
            }
            var comerciales = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, EmpleadorACargo = x.EmpleadorACargoId });

            var resultado = new EquipoDto
            {
                EquipoReal = (comercial.PerfilId != (int)EnumPerfil.CorredoresComercial) ? ListarEquipo(comercial.ComercialId, comerciales) : repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.PerfilId == (int)EnumPerfil.CorredoresComercial)
            };
            resultado.Equipo = comercial.PerfilId == (int)EnumPerfil.Mesa|| comercial.PerfilId == (int)EnumPerfil.Jefe ? comerciales.Select(x => x.ComercialId).ToList() : resultado.EquipoReal;

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
            var perfilComercial = repositorio.Obtener<Comercial, int>(x => x.ComercialId == comercialId, x => x.PerfilId);
            if (perfilComercial != 7)
            {
                listaSuperiores.Add(comercialId);
            }
            ObtenerCadenaUsuarios(comercialId, listaSuperiores);

            return listaSuperiores;
        }
        private List<int> ObtenerCadenaUsuarios(int comercialId, List<int> listaSuperiores)
        {
            var comercial = repositorio.Obtener<Comercial>(comercialId);
            if (comercial.PerfilId == 7)
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

        public List<int> ListarCorredoresComercial(int perfilId)
        {
            var resultado = (perfilId == (int)EnumPerfil.CorredoresComercial) ? repositorio.Listar<Comercial, int>(x => x.ComercialId, x => x.PerfilId == (int)EnumPerfil.CorredoresComercial) : new List<int>();
            return resultado;
        }

        public List<Comercial> ListarComercialesPorPerfil(EnumPerfil perfil)
        {
            return repositorio.Listar<Comercial>(x => x.PerfilId == (int)perfil);
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





