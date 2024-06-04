using iTextSharp.text;
using iTextSharp.text.pdf;
using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BoletoHeaderFooter : PdfPageEventHelper
    {
        private readonly BasicoContrato basico;
        private PdfContentByte cb;
        private readonly List<PdfTemplate> templates;
        int pagenumber = 1;

        public BoletoHeaderFooter(BasicoContrato basico)
        {
            templates = new List<PdfTemplate>();
            this.basico = basico;
        }

        //public override void OnOpenDocument(PdfWriter writer, Document document)
        //{
        //    cb = writer.DirectContent;
        //    template = cb.CreateTemplate(50, 50);
        //}

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            cb = writer.DirectContentUnder;
            PdfTemplate templateM = cb.CreateTemplate(595, 50);
            templates.Add(templateM);

            int pageN = writer.CurrentPageNumber;
            string pageText = "";
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            float len = bf.GetWidthPoint(pageText, 10);
            cb.BeginText();
            cb.SetFontAndSize(bf, 9);
            cb.SetTextMatrix(document.PageSize.Width - document.RightMargin - 20, 10); // colocamos el texto en la posicion que queremos
            cb.ShowText(pageText);
            cb.EndText();
            cb.AddTemplate(templateM, 10, 10); // posicion donde se agrega el template
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont bfBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            foreach (PdfTemplate item in templates)
            {
                if (basico.BoletoContratoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)//Si no es Carta oferta
                {
                    item.BeginText();
                    item.SetFontAndSize(bf, 9);
                    item.SetTextMatrix(document.PageSize.Width - document.RightMargin - 30, 0);
                    item.ShowText(pagenumber++ + " / " + (writer.PageNumber));
                    item.EndText();
                    if (pagenumber - 1 != writer.PageNumber)
                    {
                        var text = "_______________";
                        item.BeginText();
                        item.SetFontAndSize(bf, 9);
                        item.SetTextMatrix(80, 30);
                        item.ShowText(text);
                        item.EndText();
                        text = "Firma Comprador";
                        item.BeginText();
                        item.SetFontAndSize(bfBold, 9);
                        item.SetTextMatrix(80, 20);
                        item.ShowText(text);
                        item.EndText();

                        if (basico.CorredorId > 0)
                        {
                            text = "______________";
                            item.BeginText();
                            item.SetFontAndSize(bf, 9);
                            item.SetTextMatrix(230, 30);
                            item.ShowText(text);
                            item.EndText();
                            text = "Firma Corredor";
                            item.BeginText();
                            item.SetFontAndSize(bfBold, 9);
                            item.SetTextMatrix(230, 20);
                            item.ShowText(text);
                            item.EndText();
                        }

                        text = "______________";
                        item.BeginText();
                        item.SetFontAndSize(bf, 9);
                        item.SetTextMatrix(430, 30);
                        item.ShowText(text);
                        item.EndText();
                        text = "Firma Vendedor";
                        item.BeginText();
                        item.SetFontAndSize(bfBold, 9);
                        item.SetTextMatrix(430, 20);
                        item.ShowText(text);
                        item.EndText();
                    }
                }
            }
        }
    }
}