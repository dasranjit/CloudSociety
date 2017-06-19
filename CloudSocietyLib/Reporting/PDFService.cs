using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CloudSocietyEntities;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CloudSocietyLib.Reporting
{
    public class PDFService
    {
        private ModelStateDictionary _modelState;       
        const string _exceptioncontext = "PDFServices";

        public PDFService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public MemoryStream PdfMsCreator(List<PdfPTable> TablesList)
        {
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            try
            {
                PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                writer.PageEvent = PageEventHandler;

                document.Open();
                foreach (var Table in TablesList)
                {
                    Table.TotalWidth = document.PageSize.Width;
                    document.Add(Table);
                }
            }
            catch (DocumentException ex)
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            catch (IOException ioe)
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ioe.Message);
            }
            finally
            {
                writer.CloseStream = false;
                //close the document
                document.Close();
                ms.Position = 0;
            }
            return ms;
        }

        public MemoryStream PdfMsCreator(List<PdfPTable> TablesList, Society Society, PdfPTable PdfPHeadingTable = null, float SpacingAfterTable = 0F, string FooterRight = null, string Password = null)
        {
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            if (Password != null)
                writer.SetEncryption(PdfWriter.STRENGTH40BITS, Password, Password, PdfWriter.ALLOW_PRINTING);
            try
            {
                PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                writer.PageEvent = PageEventHandler;
                PageEventHandler.FooterRight = FooterRight;
                PageEventHandler.Society = Society;
                PageEventHandler.HeadingPdfTable = PdfPHeadingTable;
                document.SetMargins(20, 20, 75, 35);
                document.Open();
                foreach (var Table in TablesList)
                {
                    Table.TotalWidth = rect.Width - 40;
                    Table.SpacingAfter = SpacingAfterTable;
                    document.Add(Table);
                }
            }
            catch (DocumentException ex)
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            catch (IOException ioe)
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ioe.Message);
            }
            finally
            {
                writer.CloseStream = false;
                //close the document
                document.Close();
                ms.Position = 0;
            }
            return ms;
        }

        public MemoryStream PdfMsCreator(List<PdfPTable> TablesList, PdfPTable PdfPHeaderTable, PdfPTable PdfPHeadingTable = null, float SpacingAfterTable = 0F, string FooterRight = null, string Password = null)
        {
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            if (Password != null)
            {
                writer.SetEncryption(PdfWriter.STRENGTH40BITS, Password, Password, PdfWriter.ALLOW_PRINTING);
            }
            try
            {
                PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                writer.PageEvent = PageEventHandler;
                PageEventHandler.FooterRight = FooterRight;
                PageEventHandler.HeaderPdfTable = PdfPHeaderTable;
                PageEventHandler.HeadingPdfTable = PdfPHeadingTable;
                document.SetMargins(20, 20, 75, 35);
                document.Open();
                foreach (var Table in TablesList)
                {
                    Table.TotalWidth = rect.Width - 40;
                    Table.SpacingAfter = SpacingAfterTable;
                    document.Add(Table);
                }
            }
            catch (DocumentException ex)
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            catch (IOException ioe)
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ioe.Message);
            }
            finally
            {
                writer.CloseStream = false;
                //close the document
                document.Close();
                ms.Position = 0;
            }
            return ms;
        }

        public PdfPTable SocietyHeaderTable(Society Society)
        {
            // also check PDFHeaderFooterService OnStartPage event
            string contain = "";
            PdfPTable HeaderTbl = new PdfPTable(1);
            PdfPCell cell = null;
            cell = new PdfPCell(new Paragraph(Society.Name + "\n", FontFactory.GetFont("Verdana", 16)));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right                
            cell.Border = 0;
            HeaderTbl.AddCell(cell);
            contain = "Regd. No. : " + Society.RegistrationNo + " Dated : " + String.Format("{0:dd-MMM-yyyy}", Society.RegistrationDate);
            if (Society.TaxRegistrationNo != null)
            {
                contain = contain + ", Services Tax No : " + Society.TaxRegistrationNo;
            }
            cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 7)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            HeaderTbl.AddCell(cell);
            contain = Society.Address + ", " + Society.City + " - " + Society.PIN;
            cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 9)));
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right                
            cell.Border = 0;
            HeaderTbl.AddCell(cell);
            HeaderTbl.SpacingAfter = 10f;
            return HeaderTbl;
        }

        public PdfPTable CaptionTable(string Caption, Font CFont, System.Drawing.Color BgColor, int Padding = 5, int PaddingBottom = 8)
        {
            PdfPTable Table = new PdfPTable(1);
            PdfPCell cell;
            cell = new PdfPCell(new Paragraph(Caption, CFont));
            cell.BackgroundColor = new BaseColor(BgColor);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = Padding;
            cell.PaddingBottom = PaddingBottom;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            Table.AddCell(cell);
            return Table;
        }       
    }
}
