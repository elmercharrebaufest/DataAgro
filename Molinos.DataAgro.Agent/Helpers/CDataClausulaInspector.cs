using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Molinos.DataAgro.Agent.Helpers
{
    /// <summary>
    /// Interceptor WCF que inyecta secciones CDATA en los elementos &lt;Clausula&gt;
    /// del mensaje SOAP saliente, equivalente al AgregarElementoCData de ConfirmaLoteBorradorAgent.
    /// </summary>
    internal sealed class CDataClausulaInspector : IClientMessageInspector, IEndpointBehavior
    {
        // ── IClientMessageInspector ──────────────────────────────────────────

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var sb = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
                request.WriteMessage(xmlWriter);

            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            foreach (XmlNode node in doc.SelectNodes("//*[local-name()='Clausula']"))
            {
                if (!(node is XmlElement el)) continue;

                string texto = el.InnerText;
                el.InnerXml = string.Empty;
                if (!string.IsNullOrEmpty(texto))
                    el.AppendChild(doc.CreateCDataSection(texto));
            }

            using (var reader = XmlReader.Create(new StringReader(doc.OuterXml)))
                request = Message.CreateMessage(reader, int.MaxValue, request.Version);

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        // ── IEndpointBehavior ────────────────────────────────────────────────

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            => clientRuntime.ClientMessageInspectors.Add(this);

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint endpoint) { }
    }
}
