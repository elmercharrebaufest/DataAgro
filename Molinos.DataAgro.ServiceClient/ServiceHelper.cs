using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Molinos.DataAgro.ServiceClient.DataAgroServicesProxy;

namespace Molinos.DataAgro.ServiceClient
{
    public class ServiceHelper
    {
        //------------------------------------------------
        // Metodos Publicos
        //------------------------------------------------

        #region Servicios del ABM de Destinatarios

        public WcfResultIniDestinatario TraerTodoDestinatario()
        {
            var oCliente = new DataAgroServicesClient();

            var oResult = new WcfResultIniDestinatario();

            try
            {
                oResult = oCliente.TraerTodoDestinatario();
                
                oCliente.Close(); 
            }
            catch (Exception ex)
            {
                oCliente.Abort(); 

                var listaErrores = new List<ErrorMessage>();

                listaErrores.Add(new ErrorMessage() { Message = ex.Message });

                oResult.EntityErrors.ListaErrores = listaErrores.ToArray(); 
            }

            return oResult;
        }
        

        public WcfDestinatario TraerDestinatario(int intDestinatarioId)
        {
            var oCliente = new DataAgroServicesClient();

            var oResult = new WcfDestinatario();

            try
            {
                oResult = oCliente.TraerDestinatario(intDestinatarioId); 

                oCliente.Close();
            }
            catch (Exception ex)
            {
                oCliente.Abort();

                var listaErrores = new List<ErrorMessage>();

                listaErrores.Add(new ErrorMessage() { Message = ex.Message });

                oResult.EntityErrors.ListaErrores = listaErrores.ToArray();
            }

            return oResult;
        }


        public EntityErrors GrabarDestinatario(Destinatario oDestinatario)
        {
            var oCliente = new DataAgroServicesClient();

            var oEntityErrors = new EntityErrors();

            try
            {
                oEntityErrors = oCliente.GrabarDestinatario(oDestinatario);

                oCliente.Close();
            }
            catch (Exception ex)
            {
                oCliente.Abort();

                var listaErrores = new List<ErrorMessage>();

                listaErrores.Add(new ErrorMessage() { Message = ex.Message });

                oEntityErrors.ListaErrores = listaErrores.ToArray();
            }

            return oEntityErrors;
        }


        public EntityErrors EliminarDestinatario(int intDestinatarioId)
        {
            var oCliente = new DataAgroServicesClient();

            var oEntityErrors = new EntityErrors();

            try
            {
                oEntityErrors = oCliente.EliminarDestinatario(intDestinatarioId);

                oCliente.Close();
            }
            catch (Exception ex)
            {
                oCliente.Abort();

                var listaErrores = new List<ErrorMessage>();

                listaErrores.Add(new ErrorMessage() { Message = ex.Message });

                oEntityErrors.ListaErrores = listaErrores.ToArray();
            }

            return oEntityErrors;
        }
        
        #endregion

    }
}
