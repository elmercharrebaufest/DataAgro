using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Molinos.DataAgro.Agent.ConfirmaQALoteDocumentos
{
    /// <summary>
    /// Extiende Clausula para serializar el texto libre como sección CDATA,
    /// evitando el escape de caracteres especiales (comillas, ampersand, etc.).
    /// No modifica Reference.cs.
    /// </summary>
    public partial class Clausula : IXmlSerializable
    {
        public XmlSchema GetSchema() => null;

        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Orden))
                writer.WriteAttributeString("Orden", Orden);
            if (Value != null)
                writer.WriteCData(Value);
        }

        public void ReadXml(XmlReader reader)
        {
            Orden = reader.GetAttribute("Orden");
            bool isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmpty)
            {
                Value = reader.ReadString();
                reader.ReadEndElement();
            }
        }
    }

    /// <summary>
    /// Extiende OrigenLocalidadOrigen para serializar la localidad como CDATA.
    /// </summary>
    public partial class OrigenLocalidadOrigen : IXmlSerializable
    {
        public XmlSchema GetSchema() => null;

        public void WriteXml(XmlWriter writer)
        {
            if (Caption != null)
                writer.WriteAttributeString("Caption", Caption);
            if (LocalidadText != null)
                writer.WriteAttributeString("LocalidadText", LocalidadText);
            if (Value != null)
                writer.WriteCData(Value);
        }

        public void ReadXml(XmlReader reader)
        {
            Caption = reader.GetAttribute("Caption");
            LocalidadText = reader.GetAttribute("LocalidadText");
            bool isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmpty)
            {
                Value = reader.ReadString();
                reader.ReadEndElement();
            }
        }
    }
}
