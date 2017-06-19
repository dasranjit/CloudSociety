using System;
using CloudSocietyEntities;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace CloudSocietyLib.Reporting
{
    public class PDFHeaderFooterService : PdfPageEventHelper
    {
        PdfContentByte cb;

        PdfTemplate template;

        BaseFont bf = null;

        String PrintTime = "";

        #region Properties

        private Society _Society;
        public Society Society
        {
            get { return _Society; }
            set { _Society = value; }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _Header;
        public string Header
        {
            get { return _Header; }
            set { _Header = value; }
        }

        private PdfPTable _HeaderPdfTable;
        public PdfPTable HeaderPdfTable
        {
            get { return _HeaderPdfTable; }
            set { _HeaderPdfTable = value; }
        }
        private PdfPTable _HeadingPdfTable;
        public PdfPTable HeadingPdfTable
        {
            get { return _HeadingPdfTable; }
            set { _HeadingPdfTable = value; }
        }

        private Font _HeaderFont;
        public Font HeaderFont
        {
            get { return _HeaderFont; }
            set { _HeaderFont = value; }
        }

        private string _FooterRight;
        public string FooterRight
        {
            get { return _FooterRight; }
            set { _FooterRight = value; }
        }

        //private Font _FooterFont;
        //public Font FooterFont
        //{
        //    get { return _FooterFont; }
        //    set { _FooterFont = value; }
        //}
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = String.Format("{0:dd-MMM-yyyy, HH:mm:ss}",DateTime.Now);
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
                throw de;
            }
            catch (System.IO.IOException ioe)
            {
                throw ioe;
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            Rectangle pageSize = document.PageSize;

            if (Title != string.Empty && Title != null)
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.SetRGBColorFill(50, 50, 200);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(15));
                cb.ShowText(Title);
                cb.EndText();
            }

            if (Society != null)
            {
                string contain = "";
                PdfPTable HeaderTable = new PdfPTable(1);
                HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                HeaderTable.TotalWidth = pageSize.Width - 80;
                HeaderTable.SetWidthPercentage(new float[] { 90 }, pageSize);
                PdfPCell cell = null;
                cell = new PdfPCell(new Paragraph(Society.Name + "\n", HeaderFont != null ? HeaderFont : FontFactory.GetFont("Verdana", 16)));
                cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right                
                cell.Border = 0;
                HeaderTable.AddCell(cell);
                contain = "Regd. No. : " + Society.RegistrationNo + " Dated : " + String.Format("{0:dd-MMM-yyyy}", Society.RegistrationDate);
                if (Society.TaxRegistrationNo != null)
                {
                    contain = contain + ", Services Tax No : " + Society.TaxRegistrationNo;
                }
                cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 7)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                HeaderTable.AddCell(cell);
                contain = Society.Address + ", " + Society.City + " - " + Society.PIN;
                cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 9)));
                cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right                
                cell.Border = 0;
                cell.BorderWidthBottom = 1;
                HeaderTable.AddCell(cell);

                cb.SetRGBColorFill(0, 0, 0);
                HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(20), cb);
            }

            if (HeaderPdfTable != null)
            {
                HeaderPdfTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                HeaderPdfTable.TotalWidth = pageSize.Width - 80;
                HeaderPdfTable.SetWidthPercentage(new float[] { 90 }, pageSize);
                cb.SetRGBColorFill(0, 0, 0);
                HeaderPdfTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(20), cb);
            }
            if (HeadingPdfTable != null)
            {                
                document.Add(HeadingPdfTable); 
            }

            if (Header != string.Empty && Header != null)
            {
                PdfPTable HeaderTable = new PdfPTable(1);
                HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                HeaderTable.TotalWidth = pageSize.Width - 80;
                HeaderTable.SetWidthPercentage(new float[] { 90 }, pageSize);

                PdfPCell Cell = new PdfPCell(new Phrase(8, Header, HeaderFont));
                Cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Cell.Border = 0;
                Cell.BorderWidthBottom = 1;
                HeaderTable.AddCell(Cell);

                cb.SetRGBColorFill(0, 0, 0);
                HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(20), cb);
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {

            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(20));
            cb.ShowText(text);
            cb.EndText();

            text = (FooterRight != string.Empty && FooterRight != null ? FooterRight : "Printed On " + PrintTime.ToString());
            cb.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(20));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, text, pageSize.GetRight(40), pageSize.GetBottom(20), 0);
            cb.EndText();

        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }

        public void AddOutline(PdfWriter writer, string Title, float Position)
        {
            PdfDestination destination = new PdfDestination(PdfDestination.FITH, Position);
            PdfOutline outline = new PdfOutline(writer.DirectContent.RootOutline, destination, Title);
            writer.DirectContent.AddOutline(outline, "Name = " + Title);
        }
    }
}