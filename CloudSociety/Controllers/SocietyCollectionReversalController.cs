using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Mail;
using CommonLib.Financial;
using System.Collections.Generic;
using System.Web.Security;
using System.Linq;
using CloudSocietyLib.Reporting;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyCollectionReversalController : Controller
    {
        private PDFService _PDFService;
        private SocietyCollectionReversalService _service;
        const string _exceptioncontext = "SocietyCollectionReversal Controller";

        public SocietyCollectionReversalController()
        {
            _service = new SocietyCollectionReversalService(this.ModelState);
            _PDFService = new PDFService(this.ModelState);
        }

        //Method to return memory stream of receipt reversal added by Keval
        private MemoryStream PdfMsReceipt(Guid id)
        {
            SocietyCollectionReversal SocietyCollectionReversal = _service.GetById(id);
            Society Society = new SocietyService(this.ModelState).GetById(SocietyCollectionReversal.SocietyID);
            SocietyReceipt societyReceipt = new SocietyReceiptService(this.ModelState).GetById((System.Guid)SocietyCollectionReversal.SocietyReceiptID);

            decimal totalAmount = 0;
            String contain = "";
            PdfPCell cell;
            List<PdfPTable> PdfPTableList = new List<PdfPTable>();
            try
            {
                PdfPTable receiptTable = new PdfPTable(6);
                cell = new PdfPCell(new Phrase(new Chunk("C O L L E C T I O N   R E V E R S A L", FontFactory.GetFont("Verdana", 11, Font.BOLD))));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 1.2f;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Paragraph(SocietyCollectionReversal.Society.Name + "\n", FontFactory.GetFont("Verdana", 15)));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                contain = "Regd. No. : " + SocietyCollectionReversal.Society.RegistrationNo + " Dated : " + String.Format("{0:dd-MMM-yyyy}", SocietyCollectionReversal.Society.RegistrationDate);
                if (SocietyCollectionReversal.Society.TaxRegistrationNo != null)
                {
                    contain = contain + ", Services Tax No : " + SocietyCollectionReversal.Society.TaxRegistrationNo;
                }
                cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 7)));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                contain = SocietyCollectionReversal.Society.Address + ", " + SocietyCollectionReversal.Society.City + " - " + SocietyCollectionReversal.Society.PIN;
                cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 9)));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Receipt No.", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Receipt Date", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                //cell.Width = 15;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Cheque No", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Dated", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Bank Name", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Amount", FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                //cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.ReceiptNo, FontFactory.GetFont("Verdana", 7))));
                cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.ReceiptNo, FontFactory.GetFont("Verdana", 7))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", societyReceipt.ReceiptDate), FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);


                if (SocietyCollectionReversal.SocietyPayMode != null)
                {
                    if (SocietyCollectionReversal.SocietyPayMode.AskDetails)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.PayRefNo, FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyCollectionReversal.PayRefDate), FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        contain = "";
                        if (SocietyCollectionReversal.Bank != null)
                            contain = SocietyCollectionReversal.Bank.Name;
                        cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                    }
                    else
                    {

                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.SocietyPayMode.PayMode, FontFactory.GetFont("Verdana", 9))));
                        cell.Colspan = 3;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        //cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                        //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        //cell.BorderWidthLeft = 0.5f;
                        //cell.BorderWidthRight = 1.2f;
                        //cell.BorderWidthTop = 0.5f;
                        //cell.BorderWidthBottom = 0;
                        //receiptTable.AddCell(cell);
                        //totalAmount += societyReceipt.Amount;

                        //cell = new PdfPCell(new Phrase(new Chunk("Details not required", FontFactory.GetFont("Verdana", 9))));
                        //cell.Colspan = 3;
                        //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //cell.BorderWidthLeft = 0.5f;
                        //cell.BorderWidthRight = 0;
                        //cell.BorderWidthTop = 0.5f;
                        //cell.BorderWidthBottom = 0;
                        //receiptTable.AddCell(cell);

                    }
                }
                else
                {

                    cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 0.5f;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0;
                    receiptTable.AddCell(cell);
                    //cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //cell.BorderWidthLeft = 0.5f;
                    //cell.BorderWidthRight = 1.2f;
                    //cell.BorderWidthTop = 0.5f;
                    //cell.BorderWidthBottom = 0;
                    //receiptTable.AddCell(cell);
                    //totalAmount += societyReceipt.Amount;


                    //cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                    //cell.Colspan = 3;
                    //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.BorderWidthLeft = 0.5f;
                    //cell.BorderWidthRight = 0;
                    //cell.BorderWidthTop = 0.5f;
                    ///cell.BorderWidthBottom = 0;
                    //receiptTable.AddCell(cell);
                }

                cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0.5f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                totalAmount += societyReceipt.Amount;

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                contain = "Above mentioned document received from " + SocietyCollectionReversal.SocietyMember.Member + ", Flat No. : " + SocietyCollectionReversal.SocietyBuildingUnit.SocietyBuilding.Building + "-" + SocietyCollectionReversal.SocietyBuildingUnit.Unit + " the sum of Rs. " + totalAmount + " now reversed.";

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                contain = "Reversal Reason : "+ SocietyCollectionReversal.Particulars;
                                
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : " + ChangeCurrencyToWords.changeCurrencyToWords(totalAmount).ToUpper(), FontFactory.GetFont("Verdana", 8, Font.BOLD))));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("For " + SocietyCollectionReversal.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Checked By :", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                receiptTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                receiptTable.AddCell(cell);
                PdfPTableList.Add(receiptTable);
            }
            catch (DocumentException ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return _PDFService.PdfMsCreator(PdfPTableList, Society);
        }


        // Method to generate society receipt and return. Added By Keval
        public FileStreamResult CreatePdf(Guid id)
        {
            MemoryStream ms = PdfMsReceipt(id);
            string receiptDate = String.Format("{0:dd-MMM-yyyy}", _service.GetById(id).ReversalDate);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            string arrg = "attachment;filename=ReceiptReversalOf" + String.Format("{0:dd-MMM-yyyy}", receiptDate) + "." + "pdf";
            Response.AddHeader("content-disposition", arrg);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Method to send receipt mail. Added by Keval
        public ActionResult SendMail(Guid id)
        {
            SocietyCollectionReversal societyCollectionReversal = _service.GetById(id);
//            SocietyReceipt societyReceipt = societyCollectionReversal.SocietyReceipt;
            SocietyReceipt societyReceipt = new SocietyReceiptService(this.ModelState).GetById((System.Guid)societyCollectionReversal.SocietyReceiptID);
            
//            ViewBag.SocietySubscriptionID = societyCollectionReversal.SocietySubscriptionID;
            Guid societyMemberID = societyReceipt.SocietyMemberID;
            MemoryStream ms = PdfMsReceipt(id);
            SmtpClient mailClient = new SmtpClient();
            MailMessage message = new MailMessage();
            string mailFrom, mailTo, mailBody, receiptDate, fileName;

            receiptDate = String.Format("{0:dd-MMM-yyyy}", societyReceipt.ReceiptDate);
            fileName = "ReceiptReversalOf" + String.Format("{0:dd-MMM-yyyy}", receiptDate) + "." + "pdf";
            message.IsBodyHtml = true;
            mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
            var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySocietyMemberID(societyMemberID);
            mailTo = "";
            if (ud != null)
            { 
                var usr = Membership.GetUser((object)ud.UserID);
                if(usr != null)
                    mailTo = usr.Email;
            }
            if (!string.IsNullOrEmpty(mailFrom) && !String.IsNullOrEmpty(mailTo) && !String.IsNullOrWhiteSpace(mailTo))
            {
                try
                {
                    message.From = new MailAddress(mailFrom);
                    message.To.Add(new MailAddress(mailTo));
                    message.Subject = "Society Receipt Reversal";
                    //Following need to add Admin Parameter
                    mailBody = new CloudSociety.Services.AppInfoService(this.ModelState).Get().ReceiptReversalBody;
                    mailBody = mailBody.Replace("&&Member&&", societyReceipt.SocietyMember.Member);
                    mailBody = mailBody.Replace("&&ReceiptNo&&", societyReceipt.ReceiptNo);
                    mailBody = mailBody.Replace("&&ReceiptDate&&", receiptDate);
                    mailBody = mailBody.Replace("&&Unit&&", societyReceipt.SocietyBuildingUnit.Unit);
                    mailBody = mailBody.Replace("&&Amount&&", societyReceipt.Amount.ToString("###0.00"));
                    message.Body = mailBody;
                    Attachment a = new Attachment(ms, fileName, "application/pdf");
                    message.Attachments.Add(a);
                    //                    if (!string.IsNullOrEmpty(mailFrom) && mailTo != null)
                    mailClient.Send(message);
                    //                    return RedirectToAction("SendSuccess", new { id = societyReceipt.SocietySubscriptionID });
                    TempData["SendMessage"] = "Reversal of Receipt No. " + societyReceipt.ReceiptNo + " emailed to member";
                    return RedirectToAction("Index", new { id = societyReceipt.SocietySubscriptionID });
                }
                catch (Exception ex)
                {
                    this.ModelState.AddModelError("SendMail", ex.Message + ", " + ex.InnerException.Message);
                    return RedirectToAction("Index", new { id = societyReceipt.SocietySubscriptionID });
                }
            }
            else
            {
                TempData["SendMessage"] = "Member " + societyReceipt.SocietyMember.Member + " does not have email id. Mail not sent.";
                return RedirectToAction("Index", new { id = societyReceipt.SocietySubscriptionID });
            }
        }

        //Method to show Success message. Added by Keval
        //public ActionResult SendSuccess(Guid id)
        //{
        //    ViewBag.SocietySubscriptionID = id;
        //    return View();
        //}

        public ActionResult Index(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.ReadOnly = (societySubscription.PaidTillDate == null ? true : false) || societySubscription.Closed;
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.LockedTillDate = societySubscription.LockedTillDate;
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(id).SocietyID);
            ViewBag.SendMessage = TempData["SendMessage"];
            return View(_service.ListByParentId(societySubscriptionService.GetById(id).SocietyID));
        }
        
        public ActionResult Details(Guid id)
        {
            var SocietyCollectonReverse = _service.GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectonReverse.SocietySubscriptionID);
            ViewBag.YearOpen = !societySubscriptionService.GetById(SocietyCollectonReverse.SocietySubscriptionID).Closed;
            return View(SocietyCollectonReverse);
        }

        //
        // GET: /SocietyCollectionReverse/Create

        public ActionResult Create(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            var societyId = societySubscription.SocietyID;
            ViewBag.SocietyID = societyId;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            var billAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
            ViewBag.BillAbbreviation = String.Empty;
            DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
            if (billAbbreviationList.Count() > 1)
            {
                ViewBag.BillAbbreviationList = billAbbreviationList;
                ViewBag.StartRange = startDate;  // societySubscription.SubscriptionStart;
            }
            else
            {
                var billAbbreviation = billAbbreviationList.FirstOrDefault().BillAbbreviation;
                ViewBag.BillAbbreviation = billAbbreviation;
                //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation);
                //ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.Last());
//                ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                //var startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                //ViewBag.StartRange = (startRange ?? societySubscription.SubscriptionStart);
            }
            ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);

            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyId);

            ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyId).Where(r => r.Active == true);
            ViewBag.BankList = new BankService(this.ModelState).List();
            return View();
        }


        //
        // POST: /SocietyCollectionReverse/Create

        [HttpPost]
        public ActionResult Create(Guid id, SocietyCollectionReversal SocietyCollectionReversalToCreate)
        {
            try
            {
                if (_service.Add(SocietyCollectionReversalToCreate))
                  //string s=  SocietyCollectionReversToCreate.BillAbbreviation
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                  //  ViewBag.SocietySubscriptionID = id;
                  //  var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                  //  var societySubscription = societySubscriptionService.GetById(id);
                  //  var societyID = societySubscription.SocietyID;
                  //  ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                  //  ViewBag.SocietyID = societyID;
                  //  ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyID); ;
                  //  ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyID);
                  //  //ViewBag.BillAbbreviation = billAbbreviation;
                  //  ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(societyBuildingUnitID);
                  //  ViewBag.ReverseDate = SocietyCollectionReversalToCreate.ReversalDate;
                  //  ViewBag.SocietyReceiptDetails = new SocietyReceiptService(this.ModelState).ListByParentId(societyID);
                  //  ViewBag.Particulars = SocietyCollectionReversalToCreate.Particulars;
                  //  ViewBag.PrincipalAdjusted = SocietyCollectionReversalToCreate.PrincipalAdjusted;
                  //  ViewBag.InterestAdjusted = SocietyCollectionReversalToCreate.InterestAdjusted;
                  //  ViewBag.NonChgAdjusted = SocietyCollectionReversalToCreate.NonChgAdjusted;
                  //  //ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyID).Where(r => r.Active == true);
                  //  //ViewBag.BankList = new BankService(this.ModelState).List();
                  // // List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                  // // ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                  ////  ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(id);
                    var societyId = societySubscription.SocietyID;
                    ViewBag.SocietyID = societyId;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    var billAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
                    ViewBag.BillAbbreviation = String.Empty;
                    DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                    if (billAbbreviationList.Count() > 1)
                    {
                        ViewBag.BillAbbreviationList = billAbbreviationList;
                        ViewBag.StartRange = startDate;  // societySubscription.SubscriptionStart;
                    }
                    else
                    {
                        var billAbbreviation = billAbbreviationList.FirstOrDefault().BillAbbreviation;
                        ViewBag.BillAbbreviation = billAbbreviation;
                        //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation);
                        //ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.Last());
//                        ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                        DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                        ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                    }
                    ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);

                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyId);

                    ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyId).Where(r => r.Active == true);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                //ViewBag.SocietySubscriptionID = id;
                //var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                //var societySubscription = societySubscriptionService.GetById(id);
                //var societyID = societySubscription.SocietyID;
                //ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                //ViewBag.SocietyID = societyID;
                //ViewBag.SocietyBuildingUnit = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                //ViewBag.BillAbbreviation = billAbbreviation;
                //ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(societyBuildingUnitID);
                //ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyID).Where(r => r.Active == true);
                //ViewBag.BankList = new BankService(this.ModelState).List();
                //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                //ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                //ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(id);
                var societyId = societySubscription.SocietyID;
                ViewBag.SocietyID = societyId;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                var billAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
                ViewBag.BillAbbreviation = String.Empty;
                DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                if (billAbbreviationList.Count() > 1)
                {
                    ViewBag.BillAbbreviationList = billAbbreviationList;
                    ViewBag.StartRange = startDate;  // societySubscription.SubscriptionStart;
                }
                else
                {
                    var billAbbreviation = billAbbreviationList.FirstOrDefault().BillAbbreviation;
                    ViewBag.BillAbbreviation = billAbbreviation;
                    //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation);
                    //ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.Last());
//                    ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                    DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                    ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                }
                ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);

                ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyId);

                ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyId).Where(r => r.Active == true);
                ViewBag.BankList = new BankService(this.ModelState).List();
                return View();
            }
        }

        // EDIT is NOT USED
        public ActionResult Edit(Guid id) //id=SocietyCollectionReverseID
        {
            var SocietyCollectionReversal = _service.GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(SocietyCollectionReversal.SocietySubscriptionID);
            var societyId = SocietyCollectionReversal.SocietyID;
            ViewBag.SocietyID = societyId;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectionReversal.SocietySubscriptionID);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyId);
            ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(SocietyCollectionReversal.SocietyBuildingUnitID);
            IEnumerable<SocietyReceipt> SocietyReceiptList = new SocietyReceiptService(this.ModelState).ListByParentId(societyId).Where(s => s.PayModeCode == "CH");
            ViewBag.SocietyReceiptDetails = SocietyReceiptList;
            ViewBag.SocietySubscriptionID = SocietyCollectionReversal.SocietySubscriptionID;
            ViewBag.PayModeCode = "CH";
            ViewBag.AcYear = SocietyCollectionReversal.AcYear;
            ViewBag.Serial = SocietyCollectionReversal.Serial;
            ViewBag.DocNo = SocietyCollectionReversal.DocNo;
            ViewBag.Advance = SocietyCollectionReversal.Advance;
            ViewBag.PayRefNo = SocietyCollectionReversal.PayRefNo;
            ViewBag.PayRefDate = SocietyCollectionReversal.PayRefDate;
            ViewBag.BankID = SocietyCollectionReversal.BankID;
            ViewBag.Branch = SocietyCollectionReversal.Branch;
            ViewBag.AcTransactionID = SocietyCollectionReversal.AcTransactionID;
            return View(SocietyCollectionReversal);
        }

        //
        // POST: /SocietyCollectionReverse/Edit/5

        [HttpPost]
        public ActionResult Edit(Guid id, SocietyCollectionReversal SocietyCollectionReversalToEdit) //id=SocietyCollectionReverseID
        {
            var SocietyCollectionReversal = _service.GetById(id);
            try
            {
                if (_service.Edit(SocietyCollectionReversalToEdit))
                {
                    return RedirectToAction("Index", new { id = SocietyCollectionReversal.SocietySubscriptionID });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(SocietyCollectionReversal.SocietySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectionReversal.SocietySubscriptionID);
                    ViewBag.SocietySubscriptionID = SocietyCollectionReversal.SocietySubscriptionID;
                    //var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    // var societySubscription = societySubscriptionService.GetById(id);
                    var societyID = societySubscription.SocietyID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectionReversal.SocietySubscriptionID);
                    ViewBag.SocietyID = societyID;
                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyID); ;
                    ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyID);
                    ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(SocietyCollectionReversal.SocietyBuildingUnitID);
                    ViewBag.ReverseDate = SocietyCollectionReversal.ReversalDate;
                    ViewBag.SocietyReceiptDetails = new SocietyReceiptService(this.ModelState).ListByParentId(societyID);
                    ViewBag.Particulars = SocietyCollectionReversal.Particulars;
                    ViewBag.PrincipalAdjusted = SocietyCollectionReversal.PrincipalAdjusted;
                    ViewBag.InterestAdjusted = SocietyCollectionReversal.InterestAdjusted;
                    ViewBag.NonChgAdjusted = SocietyCollectionReversal.NonChgAdjusted;
                    // ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societySubscription.SocietyID).Where(r => r.Active == true);
                    // ViewBag.BankList = new BankService(this.ModelState).List();
                    // List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                    // ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                    //  ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                    return View(SocietyCollectionReversal);
                }
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SocietyCollectionReverse/Delete/5

        public ActionResult Delete(Guid id)
        {
            SocietyCollectionReversal SocietyCollectionRevers = _service.GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectionRevers.SocietySubscriptionID);
            //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(SocietyReceipt.SocietySubscriptionID, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
            //ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (billDateList.Count == 0 ? SocietyReceipt.SocietySubscription.SubscriptionStart : billDateList.First()) ? true : false);
            ViewBag.AllowToDelete = true;
            return View(SocietyCollectionRevers);
        }

        //
        // POST: /SocietyCollectionReverse/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, SocietyCollectionReversal SocietyCollectionReversalToDelete)
        {
            SocietyCollectionReversal SocietyCollectionReversal = _service.GetById(id);
            try
            {
                if (_service.Delete(SocietyCollectionReversal))
                    return RedirectToAction("Index", new { id = SocietyCollectionReversal.SocietySubscriptionID });
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectionReversal.SocietySubscriptionID);
                    //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                    //ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (billDateList.Count == 0 ? SocietyReceipt.SocietySubscription.SubscriptionStart : billDateList.First()) ? true : false);
                    ViewBag.AllowToDelete = true;
                    return View(SocietyCollectionReversal);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyCollectionReversal.SocietySubscriptionID);
                //List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                //ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (billDateList.Count == 0 ? SocietyReceipt.SocietySubscription.SubscriptionStart : billDateList.First()) ? true : false);
                ViewBag.AllowToDelete = true;
                return View(SocietyCollectionReversal);
            }
        }
    }
}
