using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Mail;
using CommonLib.Financial;
using System.Web.Security;
using CloudSocietyLib.Reporting;

namespace CloudSociety.Controllers
{

    public class SubscriptionInvoiceController : Controller
    {
        private SubscriptionInvoiceService _service;
        const string _exceptioncontext = "SubscriptionInvoice Controller";
        public SubscriptionInvoiceController()
        {
            _service = new SubscriptionInvoiceService(this.ModelState);
        }

        //To return pdf MemoryStream
        private MemoryStream PDFMemoryStream(Guid id)
        {
            //SubscriptionInvoice list
            var subscriptionInvoice = _service.GetById(id);

            //data container
            String contain = "";

            //get image 
            //Image logoImage = Image.GetInstance(Server.MapPath("~/Content/Images/header.jpg"));
            //Image companyName = Image.GetInstance(Server.MapPath("~/Content/Images/companyname_300_a4size.png"));

            //mem buffer
            MemoryStream ms = new MemoryStream();
            //PageSize
            Rectangle rect = PageSize.A4;

            //the document
            Document document = new Document(rect);
            //the writer
            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            //set page width
            float pageWidth = rect.Width;

            //For give the size to image            
            //logoImage.ScaleToFit(width, height);
            //companyName.ScaleToFit(150f, 30f);
            try
            {
                PdfPTable headerTbl = new PdfPTable(1);
                PdfPCell cell;
                cell = new PdfPCell(new Paragraph("Cloud Computing TechSolutions Pvt. Ltd.", FontFactory.GetFont("Verdana", 20)));
                cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right                
                cell.Border = 0;
                headerTbl.AddCell(cell);
                cell = new PdfPCell(new Paragraph("#413, Orbit Industrial Estate, Mind Space, Malad(W), Mumbai - 400064.", FontFactory.GetFont("Verdana", 9)));
                cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right  
                cell.Border = 0;
                headerTbl.AddCell(cell);
                cell = new PdfPCell(new Paragraph(""));
                cell.BorderWidthBottom = Rectangle.NO_BORDER;
                cell.BorderWidthLeft = Rectangle.NO_BORDER;
                cell.BorderWidthRight = Rectangle.NO_BORDER;
                headerTbl.AddCell(cell);

                PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                writer.PageEvent = PageEventHandler;
                PageEventHandler.FooterRight = "Phone : 91-22-4266-6657, E-Mail : enquiry@cloudsociety.in";
                PageEventHandler.HeaderPdfTable = headerTbl;
                PageEventHandler.HeaderFont = FontFactory.GetFont("Verdana", 20);
                document.SetMargins(40, 40, 65, 35);
                
                document.Open();

                //create fonts
                BaseFont timesNormal = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font fontNormal = new Font(timesNormal, 10, Font.NORMAL);
                Font fontH1 = new Font(timesNormal, 10, Font.BOLD);

                if (subscriptionInvoice.SubscriberID != null)
                {
                    contain += subscriptionInvoice.Subscriber.Name.ToString() + Chunk.NEWLINE + subscriptionInvoice.Subscriber.Address.ToString() + Chunk.NEWLINE;
                    contain += subscriptionInvoice.Subscriber.City.ToString() + ", " + subscriptionInvoice.Subscriber.PIN.ToString() + Chunk.NEWLINE;
                    contain += subscriptionInvoice.Subscriber.State.Name.ToString() + ", " + subscriptionInvoice.Subscriber.Country.Name.ToString() + ".";
                }
                else
                {
                    contain += subscriptionInvoice.Society.Name.ToString() + Chunk.NEWLINE + subscriptionInvoice.Society.Address.ToString() + Chunk.NEWLINE;
                    contain += subscriptionInvoice.Society.City.ToString() + ", " + subscriptionInvoice.Society.PIN.ToString() + Chunk.NEWLINE;
                    contain += subscriptionInvoice.Society.State.Name.ToString() + ", " + subscriptionInvoice.Society.Country.Name.ToString() + ".";
                }
                // table1
                PdfPTable table1 = new PdfPTable(2);
                //cell1 for table1
                PdfPCell cell1 = null;
                cell1 = new PdfPCell(new Paragraph("Tax Invoice", FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.BLACK)));
                cell1.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cell1.Colspan = 2;
                cell1.BorderWidth = Rectangle.NO_BORDER;
                table1.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase("  "));
                cell1.Colspan = 2;
                cell1.BorderWidth = Rectangle.NO_BORDER;
                table1.AddCell(cell1);

                cell1 = new PdfPCell(new Phrase(new Chunk(contain, fontNormal)));
                cell1.BorderWidth = Rectangle.NO_BORDER;
                table1.AddCell(cell1);

                if (table1 != null)
                {
                    table1.SetWidthPercentage(new float[] { (float).50 * pageWidth, (float).50 * pageWidth }, rect);
                }

                contain = "";
                contain += "Invoice No    : " + subscriptionInvoice.InvoiceNo.ToString() + Chunk.NEWLINE;
                contain += "Invoice Date : " + String.Format("{0:dd-MMM-yyyy}", subscriptionInvoice.InvoiceDate).ToString() + Chunk.NEWLINE;
                contain += "Due Date      : " + String.Format("{0:dd-MMM-yyyy}", subscriptionInvoice.DueDate).ToString() + Chunk.NEWLINE;

                cell1 = new PdfPCell(new Phrase(new Chunk(contain, fontNormal)));
                cell1.BorderWidth = Rectangle.NO_BORDER;
                table1.AddCell(cell1);

                //Table2
                PdfPTable table2 = new PdfPTable(5);
                //Cell2 for Table2              
                PdfPCell cell2 = null;

                cell2 = new PdfPCell(new PdfPCell(new Phrase(new Chunk("Society Name", fontH1))));
                cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                table2.AddCell(cell2);
                cell2 = new PdfPCell(new PdfPCell(new Phrase(new Chunk("Subscription Start Date", fontH1))));
                cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                table2.AddCell(cell2);
                cell2 = new PdfPCell(new PdfPCell(new Phrase(new Chunk("Subscription End Date", fontH1))));
                cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                table2.AddCell(cell2);
                cell2 = new PdfPCell(new PdfPCell(new Phrase(new Chunk("No of Members", fontH1))));
                cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                table2.AddCell(cell2);
                cell2 = new PdfPCell(new PdfPCell(new Phrase(new Chunk("Month", fontH1))));
                cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                if (table2 != null)
                {
                    table2.SetWidthPercentage(new float[] { (float).30 * pageWidth, (float).23 * pageWidth, (float).22 * pageWidth, (float).15 * pageWidth, (float).10 * pageWidth }, rect);
                }

                foreach (var subscription in subscriptionInvoice.SocietySubscriptionInvoices)
                {
                    if (subscription.SocietySubscription.SocietyID == null)
                    {
                        cell2 = new PdfPCell(new Phrase("  "));
                        cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                        cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                        cell2.BorderWidthRight = Rectangle.NO_BORDER;
                        table2.AddCell(cell2);
                    }
                    else
                    {
                        cell2 = new PdfPCell(new Phrase(new Chunk(subscription.SocietySubscription.Society.Name.ToString(), fontNormal)));
                        cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                        cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                        cell2.BorderWidthRight = Rectangle.NO_BORDER;
                        table2.AddCell(cell2);
                    }
                    cell2 = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", subscription.SocietySubscription.SubscriptionStart).ToString(), fontNormal)));
                    cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                    cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                    cell2.BorderWidthRight = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);
                    cell2 = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", subscription.SocietySubscription.SubscriptionEnd).ToString(), fontNormal)));
                    cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                    cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                    cell2.BorderWidthRight = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);
                    cell2 = new PdfPCell(new Phrase(new Chunk(subscription.NoOfMembers.ToString(), fontNormal)));
                    cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                    cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                    cell2.BorderWidthRight = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);
                    cell2 = new PdfPCell(new Phrase(new Chunk(subscription.SubscribedMonths.ToString(), fontNormal)));
                    cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                    cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                    cell2.BorderWidthRight = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(""));
                    cell2.Colspan = 2;
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new PdfPCell(new Phrase(new Chunk("Service Types", fontH1))));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(new Phrase(new Chunk("Amount", fontH1))));
                    cell2.Border = Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = 2;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    foreach (var serviceType in subscription.SocietySubscriptionInvoiceServices)
                    {
                        cell2 = new PdfPCell(new Phrase(""));
                        cell2.Colspan = 2;
                        cell2.Border = Rectangle.NO_BORDER;
                        table2.AddCell(cell2);

                        cell2 = new PdfPCell(new Phrase(new Chunk(serviceType.ServiceType.Type.ToString(), fontNormal)));
                        cell2.Border = Rectangle.NO_BORDER;
                        table2.AddCell(cell2);

                        cell2 = new PdfPCell(new Phrase(new Chunk(serviceType.Amount.ToString(), fontNormal)));
                        cell2.Border = Rectangle.NO_BORDER;
                        cell2.HorizontalAlignment = 2;
                        table2.AddCell(cell2);

                        cell2 = new PdfPCell(new Phrase(" "));
                        cell2.Border = Rectangle.NO_BORDER;
                        table2.AddCell(cell2);
                    }
                    //For Short Line
                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Colspan = 2;
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);
                    cell2 = new PdfPCell(new Phrase("  "));
                    cell2.Colspan = 2;
                    cell2.BorderWidthTop = Rectangle.NO_BORDER;
                    cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                    cell2.BorderWidthRight = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);
                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    //For Total 
                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Colspan = 2;
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(new Chunk("Total  :", fontNormal)));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(new Chunk(subscription.Amount.ToString(), fontNormal)));
                    cell2.Border = Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = 2;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                }
                //For a Long Line
                cell2 = new PdfPCell(new Phrase("  "));
                cell2.Colspan = 5;
                //cell2.BorderWidthTop = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                cell2.BorderWidthBottom = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                //For Sub-Total 
                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Colspan = 2;
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(new Chunk("Sub-Total  :", fontNormal)));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(new Chunk(subscriptionInvoice.Amount.ToString(), fontNormal)));
                cell2.Border = Rectangle.NO_BORDER;
                cell2.HorizontalAlignment = 2;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);


                //For Discount 
                if (subscriptionInvoice.Discount != 0)
                {
                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Colspan = 2;
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(new Chunk("Less Discount  :", fontNormal)));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(new Chunk(subscriptionInvoice.Discount.ToString(), fontNormal)));
                    cell2.Border = Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = 2;
                    table2.AddCell(cell2);

                    cell2 = new PdfPCell(new Phrase(" "));
                    cell2.Border = Rectangle.NO_BORDER;
                    table2.AddCell(cell2);
                }
                //For Tax  
                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Colspan = 2;
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(new Chunk(subscriptionInvoice.Tax.ToString(), fontNormal)));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(new Chunk(subscriptionInvoice.TaxAmount.ToString(), fontNormal)));
                cell2.Border = Rectangle.NO_BORDER;
                cell2.HorizontalAlignment = 2;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);


                //For Short Line
                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Colspan = 2;
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);
                cell2 = new PdfPCell(new Phrase("  "));
                cell2.Colspan = 2;
                cell2.BorderWidthTop = Rectangle.NO_BORDER;
                cell2.BorderWidthLeft = Rectangle.NO_BORDER;
                cell2.BorderWidthRight = Rectangle.NO_BORDER;
                table2.AddCell(cell2);
                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                //For Grand Total  
                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Colspan = 2;
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(new Chunk("Grand Total :", fontH1)));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(new Chunk(subscriptionInvoice.InvoiceAmount.ToString(), fontH1)));
                cell2.Border = Rectangle.NO_BORDER;
                cell2.HorizontalAlignment = 2;
                table2.AddCell(cell2);

                cell2 = new PdfPCell(new Phrase(" "));
                cell2.Border = Rectangle.NO_BORDER;
                table2.AddCell(cell2);

                document.Add(new Paragraph("\n"));
                document.Add(table1);
                document.Add(table2);
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("In words : " + ChangeCurrencyToWords.changeCurrencyToWords(subscriptionInvoice.InvoiceAmount), fontH1));
                document.Add(new Paragraph("\n*This is system generated invoice. Hence, no signature is  required.", FontFactory.GetFont("Verdana", 8)));
            }
            catch (DocumentException de)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + de.Message);
            }
            catch (IOException ioe)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ioe.Message);
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

        // GET: /SubscriptionInvoice/
        [Authorize(Roles = "CompanyAccount")]
        public ActionResult Index()
        {
            return View(_service.ListPending());
        }

        // GET: /SubscriptionInvoice/Details/5
        [Authorize(Roles = "Subscriber,CompanyAdmin,CompanyAccount")]
        public ActionResult Details(Guid id)
        {
            if (Roles.IsUserInRole("CompanyAccount"))
                ViewBag.BackToAccountIndex = true;
            else
                ViewBag.BackToAccountIndex = false;
            return View(_service.GetById(id));
        }

        // GET: Method to Print Tax Invoice Added By Ranjit
        [Authorize(Roles = "Subscriber,CompanyAdmin,CompanyAccount")]
        public ActionResult Print(Guid id)
        {
            //var SocietyService = new SocietyService(this.ModelState);
            return View(_service.GetById(id));
        }

        // Method to generate Tax Invoice PDF and return. Added By Ranjit
        public FileStreamResult CreatePdf(Guid id)
        {
            MemoryStream ms = PDFMemoryStream(id);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=SubscriptionInvoice.pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Method to show Success message. Added by Ranjit
        public ActionResult SendSuccess()
        {
            return View();
        }

        //Method to send mail. Added by Ranjit 
        public ActionResult SendMail(Guid id)
        {
            SubscriptionInvoice subscriptionInvoice = _service.GetById(id);
            if (subscriptionInvoice.SubscriberID != null)
            {
                MemoryStream ms = PDFMemoryStream(id);
                SmtpClient mailClient = new SmtpClient();
                MailMessage message = new MailMessage();
                string mailFrom, mailTo, mailBody;

                message.IsBodyHtml = true;
                var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySubscriberID((Guid)subscriptionInvoice.SubscriberID);
                mailTo = Membership.GetUser((object)ud.UserID).Email;
                try
                {
                    mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
                    message.From = new MailAddress(mailFrom);
                    message.To.Add(new MailAddress(mailTo));
                    //message.CC.Add(new MailAddress("CC@yahoo.com", "Display name CC"));
                    message.Subject = "Tax Invoice";
                    mailBody = new CloudSociety.Services.AppInfoService(this.ModelState).Get().InvoiceMailBody;
                    mailBody = mailBody.Replace("&&Subscriber&&", subscriptionInvoice.Subscriber.Name);
                    mailBody = mailBody.Replace("&&InvoiceNo&&", subscriptionInvoice.InvoiceNo);
                    mailBody = mailBody.Replace("&&InvoiceDate&&", String.Format("{0:dd-MMM-yyyy}", subscriptionInvoice.InvoiceDate));
                    mailBody = mailBody.Replace("&&DueDate&&", String.Format("{0:dd-MMM-yyyy}", subscriptionInvoice.DueDate));
                    mailBody = mailBody.Replace("&&Amount&&", "INR " + subscriptionInvoice.InvoiceAmount);
                    message.Body = mailBody;
                    Attachment a = new Attachment(ms, "TaxInvoice.pdf", "application/pdf");
                    message.Attachments.Add(a);
                    if (!string.IsNullOrEmpty(mailFrom))
                        mailClient.Send(message);
                    return RedirectToAction("SendSuccess");
                }
                catch (Exception ex)
                {
                    this.ModelState.AddModelError("SendMail", ex.Message + ", " + ex.InnerException.Message);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //GET: /Method for Payment. Added by Ranjit
        [Authorize(Roles = "CompanyAccount")]
        [HttpGet]
        public ActionResult Payment(Guid id)
        {
            ViewBag.Banks = new BankService(this.ModelState).List();
            ViewBag.PayModes = new PayModeService(this.ModelState).List();
            return View(_service.GetById(id));
        }
        // POST: /Method for Payment. Added by Ranjit
        [Authorize(Roles = "CompanyAccount")]
        [HttpPost]
        public ActionResult Payment(Guid id, SubscriptionInvoice SubscriptionInvoiceToUpdate)
        {
            try
            {
                if (_service.Edit(SubscriptionInvoiceToUpdate))
                    return RedirectToAction("Index");
                else
                {
                    ViewBag.Banks = new BankService(this.ModelState).List();
                    ViewBag.PayModes = new PayModeService(this.ModelState).List();
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.Banks = new BankService(this.ModelState).List();
                ViewBag.PayModes = new PayModeService(this.ModelState).List();
                return View(_service.GetById(id));
            }
        }

        // GET: Method to Print Challan  Added By Ranjit
        [Authorize(Roles = "Subscriber,CompanyAdmin,CompanyAccount")]
        public ActionResult PrintChallan(Guid id)
        {
            ViewBag.Bank = System.Configuration.ConfigurationManager.AppSettings["OurBank"];
            ViewBag.AcNo = System.Configuration.ConfigurationManager.AppSettings["OurAcNo"];
            ViewBag.Branch = System.Configuration.ConfigurationManager.AppSettings["OurBranch"];
            return View(_service.GetById(id));
        }

    }
}

