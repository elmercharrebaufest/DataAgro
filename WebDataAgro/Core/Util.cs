using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using System.Text;
using System.Xml;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using System.Security.Principal;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Mapping.Context;
/*
using System.DirectoryServices.AccountManagement;
*/
namespace WebDataAgro.Core
{
    public static class Util
    {
        //--------------------------------------------------
        //  Constantes Privadas
        //--------------------------------------------------

        private const string DefaultCNPrefix = "DataAgro";

        //------------------------------------------------------------------------------------------------
        //  ObjectState
        //------------------------------------------------------------------------------------------------

        public const int Object_Added = 0;
        public const int Object_Deleted = 1;
        public const int Object_Modified = 2;
        public const int Object_Unchanged = 3;


        //------------------------------------------------------------------------------------------------
        //  Metodos Publicos
        //------------------------------------------------------------------------------------------------

        public static string GetIdActiveDirectory()
        {
            return GetUsuario();
        }

        public static string GetNameUser()
        {
            var nameUser = HttpContext.Current.User.Identity.Name;
            return nameUser;
        }

        public static MSContext GetMSContext()
        {

            IIdentity WinId = HttpContext.Current.User.Identity;

            WindowsIdentity wi = (WindowsIdentity)WinId;
            
            var oMSContext = new MSContext();

            var strIdentity = HttpContext.Current.User.Identity.Name;

            oMSContext.CN = "";
            oMSContext.CNPrefix = DefaultCNPrefix;
            oMSContext.DBProvider = "SQLServer";
            oMSContext.EmpresaId = 1;
            oMSContext.UsuarioId = 1;
            oMSContext.Usuario =(strIdentity.Trim().Length > 0? GetUsuario() : String.Empty);
            oMSContext.PerfilId = 0;

            return oMSContext;
        }



        public static string GetUsuario()
        {
            var strIdentity = HttpContext.Current.User.Identity.Name;

            return strIdentity.ToString().Split('\\')[1];

        }

        public static bool IsAdmin()
        {            
            Molinos.DataAgro.Business.ComercialManager cm = new Molinos.DataAgro.Business.ComercialManager();
            cm.Inicializar(GetMSContext());
            return  cm.EsAdmin(System.Environment.UserName);
        }
        

        public static bool EsPerfilAdministrativo(string activeDirectoryId)
        {
            bool resultado = false;

            var unitOfWork = new UnitOfWork(GetMSContext(), new DataAgroContext(GetMSContext()));
            var oComercial = unitOfWork.Repository<Comercial>().Queryable();
                 
            if (oComercial.Where(h => h.PerfilId == (int)EnumPerfil.Administrativo && h.IdActiveDirectory == activeDirectoryId).Count() > 0)
            {
                resultado = true;
            }

            return (resultado);
        }


        public static bool EsPerfilVisualizador(string activeDirectoryId)
        {
            bool resultado = false;

            var unitOfWork = new UnitOfWork(GetMSContext(), new DataAgroContext(GetMSContext()));
            var oComercial = unitOfWork.Repository<Comercial>().Queryable();

            if (oComercial.Where(x => x.PerfilId == (int)EnumPerfil.Visualizador && x.IdActiveDirectory == activeDirectoryId).Count() > 0)
            {
                resultado = true;
            }

            return (resultado);
        }

        public static int ObtenerPerfilDeUsuario(string activeDirectoryId)
        {
            int resultado = 0;

            var unitOfWork = new UnitOfWork(GetMSContext(), new DataAgroContext(GetMSContext()));
            var oComercial = unitOfWork.Repository<Comercial>().Queryable();

            resultado = oComercial.Where(x => x.IdActiveDirectory == activeDirectoryId).FirstOrDefault().PerfilId;

            return resultado;
        }


        public static bool EsAdministrador(string activeDirectoryId)
        {
            bool resultado = false;

            var unitOfWork = new UnitOfWork(GetMSContext(), new DataAgroContext(GetMSContext()));
            var oComercial = unitOfWork.Repository<Comercial>().Queryable();

            if (oComercial.Where(x => x.Administrador == true  && x.IdActiveDirectory == activeDirectoryId).Count() > 0)
            {
                resultado = true;
            }

            return (resultado);
        }

        /*
        public static string GetNombre()
        {
            UserPrincipal userPrincipal = UserPrincipal.Current;
            return userPrincipal.DisplayName;
        }
        */

        public static List<MSErrorMessage> EntityErrorsToMSErrorMessage(EntityErrors entityErrors)
        {
            var result = new List<MSErrorMessage>();

            var hayDetailError = false;

            foreach (ErrorMessage oError in entityErrors.ListaErrores)
            {
                if (oError.Source.IndexOf(".") > 0)
                {
                    hayDetailError = true;
                }
                else
                {
                    result.Add(new MSErrorMessage()
                    {
                        Message = oError.Message,
                        Source = oError.Source
                    });
                }
            }

            if (hayDetailError || result.Count > 0)
            {
                if (result.Where(x => String.IsNullOrEmpty(x.Source)).Count() == 0)
                {
                    result.Add(new MSErrorMessage()
                    {
                        Message = "Datos incorrectos, verifique el mensaje de error en cada campo",
                        Source = ""
                    });
                }
            }

            return result;
        }


        public static IEnumerable<MSErrorMessage> EntityErrorsToMSErrorMessage(EntityErrors entityErrors, string entityName)
        {
            var result = new List<MSErrorMessage>();

            foreach (ErrorMessage oError in entityErrors.ListaErrores)
            {
                if (oError.Source.StartsWith(entityName + "."))
                {
                    result.Add(new MSErrorMessage()
                    {
                        Message = oError.Message,
                        Source = oError.Source.Replace(".", "")
                    });
                }
            }

            return result;
        }


        public static string EntityErrorsToItemError(EntityErrors entityErrors, string entityName, int indice)
        {
            string msg = null;

            foreach (ErrorMessage oError in entityErrors.ListaErrores)
            {
                if (oError.Source.StartsWith(entityName + ".") && oError.Item == indice)
                {
                    if (msg == null)
                    {
                        msg = oError.Message;
                    }
                    else
                    {
                        msg += "<br />" + oError.Message;
                    }
                }
            }

            return msg;
        }

        public static int? ToNullInt(int nValor)
        {
            if (nValor == 0)
            {
                return null;
            }
            else
            {
                return nValor;
            }
        }


        public static int ToInt(object nValor)
        {
            int intValor = 0;

            if ((nValor != null))
            {
                if (!(nValor is DBNull))
                {
                    intValor = (int)nValor;
                }

            }

            return intValor;
        }


        public static string ToStr(object cValor)
        {
            string strCadena = "";

            if ((cValor != null))
            {
                if (!(cValor is DBNull))
                {
                    strCadena = (string)cValor;
                }

            }

            return strCadena;
        }


        private static byte[] GetPasswordBytes()
        {
            var key = "sadhgj6123hhdajdkqjnzqfjlka7Z23";

            var ba = Encoding.UTF8.GetBytes(key);

            return System.Security.Cryptography.SHA256.Create().ComputeHash(ba);
        }


        public static string EncryptData(string text)
        {
            return AES.Encrypt(text, GetPasswordBytes());
        }


        public static string DecryptString(string text)
        {
            return AES.Decrypt(text, GetPasswordBytes());
        }


        public static string GetDownloadKey(string key)
        {
            var enc = EncryptData(key);

            var res = System.Web.HttpUtility.UrlEncode(enc);

            return res;
        }


        public static string GetPathFiles()
        {
            return ConfigurationManager.AppSettings["PathFiles"];
        }


        public static string GetFullPathPDF(string strFile)
        {
            return Path.Combine(GetPathFiles(), strFile);
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }


    }
}