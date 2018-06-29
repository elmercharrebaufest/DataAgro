using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;


using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using System.Configuration;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business
{
    public class ComercialManager : IComercialManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicialización
        //--------------------------------------------------

        public void Inicializar(MSContext oContexto)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }


        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = oUnitOfWork;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<DatosIniAbmComercial> TraerDatosInicialesAsync()
        {

            //si es alta le madno cero, si modifico le mando el id desde la grilla
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniAbmComercial()
            {
                Comercial = await ObtenerComerciales(0),
                Perfil = await qry.GetPerfilComboAsync(),
                GrupoDeCompras = await qry.GetGrupoDeComprasComboAsync()
            };

            return oDatosIniciales;
        }


        public async Task<List<ComercialCombo>> ObtenerComerciales(int comercialId)
        {
            var list = new List<ComercialCombo>();
            var qry = new CombosQueries(mobjUnitOfWork);

            list = await qry.GetAbmComercialComboAsync();

            if (comercialId != 0)
            {
                var resultStored = mobjUnitOfWork.SelStore<JerarquiaComercial>("DataAgro_ComercialesJerarquicos_Traer", comercialId).ToList();
                list.RemoveAll(x => resultStored.Any(z => z.ComercialId == x.ComercialId));
            }

            return list;
        }


        public async Task<ResultIniComercial> TraerTodoComercialAsync()
        {
            var oResult = new ResultIniComercial();

            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable();
            var oPerfil = mobjUnitOfWork.Repository<Perfil>().Queryable();

            var query = oComercial
                        .Join(oPerfil, a => a.PerfilId, b => b.PerfilId, (a, b) => new { COM = a, PER = b })
                        .OrderBy(x => x.COM.Apellido)
                        .Select(x => new ComercialIni()
                        {
                            ComercialId = x.COM.ComercialId,
                            Apellido = x.COM.Apellido,
                            Nombres = x.COM.Nombres,
                            PerDescripcion = x.PER.Descripcion
                        });

            oResult.Comercial = await query.ToListAsync();

            return oResult;
        }


        public async Task<Comercial> TraerComercialAsync(int intComercialId)
        {
            var oComercial = new Comercial();

            oComercial = await mobjUnitOfWork.Repository<Comercial>()
                                 .Queryable()
                                 .Where(x => x.ComercialId == intComercialId)
                                 .SingleOrDefaultAsync();

            if (oComercial == null)
            {
                oComercial = new Comercial()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oComercial.ObjectState = Constants.Object_Modified;
            }

            return oComercial;
        }


        public async Task<EntityErrors> GrabarComercialAsync(Comercial oComercial)
        {
            var oEntityErrors = new EntityErrors();

            EntityValid.ValidateAll(oComercial, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }



            // Agregar el mensaje de entity error si el usuario no existe  en AD 

            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain))
                {
                    var user = UserPrincipal.FindByIdentity(ctx, oComercial.IdActiveDirectory);

                    if (user == null)
                    {
                        oEntityErrors.HayError = true;
                        oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El usuario no existe en AD " });
                        return oEntityErrors;

                    }
                }
            }
            catch (Exception ex)
            {

            }



            Comercial oComercialSave;
            var XComercial = mobjUnitOfWork.Repository<Comercial>().Queryable();
            if (oComercial.ObjectState == 0)
            {
                // Verificar que ningun otro comercial use el mismo Id de AD
                //var XComercial = mobjUnitOfWork.Repository<Comercial>().Queryable();
                if (XComercial.Where(x => x.IdActiveDirectory == oComercial.IdActiveDirectory).Count() > 0)
                {
                    oEntityErrors.HayError = true;
                    oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "El usuario de Active Directory Ya ha sido usado por otro comercial " });
                    return oEntityErrors;
                }




                //XComercial.


                oComercialSave = new Comercial()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                if (oComercial.PerfilId == (int)EnumPerfil.Director)
                {
                    var comercial = XComercial.FirstOrDefault(x => x.PerfilId == (int)EnumPerfil.Director);

                    if (comercial != null)
                    {
                        oEntityErrors.HayError = true;
                        oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "Ya existe un Director " });
                        return oEntityErrors;
                    }
                }


                //var perfiles = mobjUnitOfWork.Repository<Perfil>().Queryable();            
                //var perfilId = perfiles.FirstOrDefault(p => p.PerfilId == comercial.ComercialId ).PerfilId;
                //if (comercial.PerfilId == 3)
                //{                    
                //}

                oComercialSave = await TraerComercialAsync(oComercial.ComercialId);
            }

            oComercialSave.Apellido = oComercial.Apellido;
            oComercialSave.Nombres = oComercial.Nombres;
            oComercialSave.PerfilId = oComercial.PerfilId;
            oComercialSave.EmpleadorACargo = oComercial.EmpleadorACargo;
            oComercialSave.IdActiveDirectory = oComercial.IdActiveDirectory;
            //oComercialSave.GrupoDeCompras = oComercial.GrupoDeCompras;  
            oComercialSave.Administrador = oComercial.Administrador;

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "0")
            {
                DatoDelComercial dat = new DatoDelComercial();

                var comercial = dat.ObtenerDatosDeComercial(oComercial.IdActiveDirectory);

                if (comercial != null)
                {
                    try
                    {
                        oComercialSave.GrupoDeCompras = await VerificarGrupoComercial(comercial.EX_ZONA);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }


            if (oComercialSave.ObjectState == Constants.Object_Added)
            {
                oComercialSave.ComercialId = ((mobjUnitOfWork.Repository<Comercial>().Queryable().Max(x => (int?)x.ComercialId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Comercial>().SaveEntity(oComercialSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<int> VerificarGrupoComercial(string grupoDeCompra)
        {
            var oGrupoDeCompra = mobjUnitOfWork.Repository<GrupoDeCompras>().Queryable().AsNoTracking();

            var grCom = oGrupoDeCompra.Where(x => x.Descripcion == grupoDeCompra).FirstOrDefault();

            if (grCom != null)
                return grCom.Id;
            else
            {
                var IdGrupo = ((mobjUnitOfWork.Repository<GrupoDeCompras>().Queryable().Max(x => (int?)x.Id)) ?? 0) + 1;
                var oGrupoDeCompraSave = new GrupoDeCompras()
                {
                    Id = IdGrupo,
                    Descripcion = grupoDeCompra,
                    ObjectState = Constants.Object_Added
                };

                mobjUnitOfWork.Repository<GrupoDeCompras>().SaveEntity(oGrupoDeCompraSave);

                await mobjUnitOfWork.SaveChangesAsync();

                return IdGrupo;
            }
        }

        public async Task<EntityErrors> EliminarComercialAsync(int intComercialId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Comercial>();

            var oComercial = await oRepository
                                 .Queryable()
                                 .Where(x => x.ComercialId == intComercialId)
                                 .SingleOrDefaultAsync();

            if (oComercial != null)
            {
                oRepository.Delete(oComercial);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }
        
        public bool ComercialExiste(string ActiveDirectoryId)

        {
            bool resultado = false;

            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable();

            if (oComercial.Where(x => x.IdActiveDirectory == ActiveDirectoryId).Count() > 0)
            {
                resultado = true;
            }

            return (resultado);
        }

        public bool ComercialPerteneceProveedor(string ActiveDirectory_Id, int Proveedor_Id)
        {
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable();

            int CId = oComercial.Where(x => x.IdActiveDirectory == ActiveDirectory_Id).FirstOrDefault().ComercialId;

            var query = mobjUnitOfWork.SelStore<Validacion>("DataAgro_ValidarProveedor_PorComercial", CId, Proveedor_Id);

            var res = query.ToList();
            if (res[0].valor == 1)
                return true;
            else
                return false;
        }

        public List<Comercial> ListarComercial(string comercial)
        {
            return mobjUnitOfWork.Repository<Comercial>().Queryable().Where(x => comercial != "" && (x.Nombres.Contains(comercial) || x.Apellido.Contains(comercial))).Take(15).ToList();
        }

    }

    internal class Validacion
    {
        public int valor { get; set; }
    }

    public class JerarquiaComercial
    {
        public Nullable<int> ComercialId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public Nullable<int> PerfilId { get; set; }
        public Nullable<int> EmpleadorACargo { get; set; }
        public string IdActiveDirectory { get; set; }
        public Nullable<int> GrupoDeCompras { get; set; }
    }
}
    




