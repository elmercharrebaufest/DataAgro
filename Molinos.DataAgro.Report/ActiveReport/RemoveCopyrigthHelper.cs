using iTextSharp.text.pdf;
using System.IO;

namespace Molinos.DataAgro.Report.ActiveReport
{
    public static class RemoveCopyrigthHelper
    {
        public static byte[] ReplaceText(this byte[] OrigFile)
        {
            using (PdfReader reader = new PdfReader(OrigFile))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    byte[] contentBytes = reader.GetPageContent(i);
                    string contentString = PdfEncodings.ConvertToString(contentBytes, PdfObject.TEXT_PDFDOCENCODING);
                    contentString = contentString.Replace("This document was created using an EVALUATION version of ActiveReports.", "");
                    contentString = contentString.Replace("Only a licensed user may legally create reports", "");
                    contentString = contentString.Replace("for use in production.", "");
                    contentString = contentString.Replace("Please report infractions or address questions to sales@grapecity.us.com.", "");
                    contentString = contentString.Replace("Copyright © 2002-2010", "");
                    contentString = contentString.Replace("GrapeCity, inc. All rights reserved.", "");
                    contentString = contentString.Replace("Please report infractions or", "");
                    contentString = contentString.Replace("address questions to sales@grapecity.us.com.", "");

                    reader.SetPageContent(i, PdfEncodings.ConvertToBytes(contentString, PdfObject.TEXT_PDFDOCENCODING));
                }
                var resultado = new MemoryStream();
                new PdfStamper(reader, resultado).Close();
                return resultado.ToArray();
            }
        }
    }
}
