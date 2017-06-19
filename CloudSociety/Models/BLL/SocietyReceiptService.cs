using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;
using System.IO;
using System.Net.Mail;
using iTextSharp.text.pdf;
using iTextSharp.text;
using CommonLib.Financial;
using CloudSocietyLib.Reporting;

namespace CloudSociety.Services
{
    public class SocietyReceiptService : ISocietyReceiptRepository
    {
        private ISocietyReceiptRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyReceipt";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyReceiptService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyReceiptCache();
        }

        public SocietyReceipt GetById(Guid id)
        {
            try
            {
                return (_cache.GetById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool Add(SocietyReceipt entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool AddTemporary(SocietyReceiptOnhold entity)
        {
            if (!_cache.AddTemporary(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyReceipt entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyReceipt entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyReceipt> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return (_cache.ListBySocietyIDStartEndDate(societyId, startDate, endDate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid)
        {
            try
            {
                return (_cache.ListBySocietyIDStartEndDateSocietyBuildingID(societyId, startDate, endDate, societybuildingid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy") + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, string billAbbreviation, DateTime startDate, DateTime endDate)
        {
            try
            {
                return (_cache.ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBulidingUnitId, billAbbreviation, startDate, endDate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + societyBulidingUnitId.ToString() + " for " + billAbbreviation + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyReceipt> ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(string billabbreviation, Guid societybulidingunitId, Guid societymemberId)
        {
            try
            {
                return (_cache.ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(billabbreviation, societybulidingunitId, societymemberId));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Unit " + societybulidingunitId.ToString() + " & for Member " + societymemberId.ToString() + " for " + billabbreviation, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid)
        {
            try
            {
                return (_cache.ListBySocietyMemberIDSocietySubscriptionID(societymemberid, societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Member " + societymemberid.ToString() + " & Year " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyReceiptOnhold> GetOnholdReceipts(Guid SocietyId, Guid societySubscriptionId, Guid? societyMemberId = null)
        {
            try
            {
                return (_cache.GetOnholdReceipts(SocietyId, societySubscriptionId, societyMemberId));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for SocietyId " + SocietyId.ToString() + " & Year " + societySubscriptionId.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool SendMail(SocietyMember societyMember, decimal Amount, string Unit)
        {
            bool result = false;
            //SocietyReceipt societyReceipt = GetById(SocietyReceiptID);
            //MemoryStream ms = PdfMsReceipt(SocietyReceiptID);
            SmtpClient mailClient = new SmtpClient();
            MailMessage message = new MailMessage();
            string mailFrom, mailTo, mailBody, receiptDate;

            receiptDate = String.Format("{0:dd-MMM-yyyy}", DateTime.Now);
            //fileName = "ReceiptOf" + String.Format("{0:dd-MMM-yyyy}", receiptDate) + "." + "pdf";
            message.IsBodyHtml = true;
            mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
            mailTo = societyMember.EmailId;

            if (!string.IsNullOrEmpty(mailFrom) && !String.IsNullOrEmpty(mailTo) && !String.IsNullOrWhiteSpace(mailTo))
            {
                try
                {
                    message.From = new MailAddress(mailFrom);
                    message.To.Add(new MailAddress(mailTo));
                    message.Subject = "Society Receipt for " + societyMember.Society.Name;
                    //Following need to add Admin Parameter
                    mailBody = new AppInfoService(_modelState).Get().TempReceiptMailBody;
                    mailBody = mailBody.Replace("&&Member&&", societyMember.Member);
                    //mailBody = mailBody.Replace("&&ReceiptNo&&","NA");
                    mailBody = mailBody.Replace("&&ReceiptDate&&", receiptDate);
                    mailBody = mailBody.Replace("&&Unit&&", Unit);
                    mailBody = mailBody.Replace("&&Amount&&", Amount.ToString("###0.00"));
                    mailBody = mailBody.Replace("&&SocietyName&&", societyMember.Society.Name);
                    mailBody = mailBody.Replace("&&SocietyContactPerson&&", societyMember.Society.ContactPerson);
                    mailBody = mailBody.Replace("&&SocietyContactNumber&&", societyMember.Society.Mobile);
                    mailBody = mailBody.Replace("&&SocietyContactEmailId&&", societyMember.Society.EMailId);
                    mailBody = mailBody.Replace("&&ToDayDate&&", string.Format("{0:dd-MMM-yyyy}", DateTime.Now));
                    message.Body = mailBody;
                    //Attachment a = new Attachment(ms, fileName, "application/pdf");
                    //message.Attachments.Add(a);
                    mailClient.Send(message);
                    result = true;
                }
                catch (Exception ex)
                {
                    _modelState.AddModelError(_exceptioncontext + " - Send mail ", GenericExceptionHandler.ExceptionMessage());
                }
            }
            else
            {
                _modelState.AddModelError("Member " + societyMember.Member + " does not have email id. Mail not sent.", GenericExceptionHandler.ExceptionMessage());
            }
            return result;
        }

        //Method to return memory stream of receipt added by Ranjit
        public MemoryStream PdfMsReceipt(Guid id)
        {
            SocietyReceipt societyReceipt = GetById(id);
            Society Society = new SocietyService(_modelState).GetById(societyReceipt.SocietyID);
            decimal totalAmount = 0;
            String contain = "";
            PdfPCell cell;
            List<PdfPTable> PdfPTableList = new List<PdfPTable>();
            try
            {
                PdfPTable receiptTable = new PdfPTable(6);
                cell = new PdfPCell(new Phrase(new Chunk("R E C E I P T", FontFactory.GetFont("Verdana", 11, Font.BOLD))));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 1.2f;
                receiptTable.AddCell(cell);
                cell = new PdfPCell(new Paragraph(societyReceipt.Society.Name + "\n", FontFactory.GetFont("Verdana", 15)));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                contain = "Regd. No. : " + societyReceipt.Society.RegistrationNo + " Dated : " + String.Format("{0:dd-MMM-yyyy}", societyReceipt.Society.RegistrationDate);
                if (societyReceipt.Society.TaxRegistrationNo != null)
                {
                    contain = contain + ", Services Tax No : " + societyReceipt.Society.TaxRegistrationNo;
                }
                cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 7)));
                cell.Colspan = 6;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);

                contain = societyReceipt.Society.Address + ", " + societyReceipt.Society.City + " - " + societyReceipt.Society.PIN;
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
                if (societyReceipt.SocietyPayMode != null)
                {
                    if (societyReceipt.SocietyPayMode.AskDetails)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.PayRefNo, FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", societyReceipt.PayRefDate), FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        contain = "";
                        if (societyReceipt.Bank != null)
                            contain = societyReceipt.Bank.Name;
                        cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.SocietyPayMode.PayMode, FontFactory.GetFont("Verdana", 9))));
                        cell.Colspan = 3;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);

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
                contain = "1. We received with thanks from " + societyReceipt.SocietyMember.Member + ", Flat No. : " + societyReceipt.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyReceipt.SocietyBuildingUnit.Unit + " the sum of Rs. " + totalAmount;
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                contain = "2. Cheques are subject to realization.";
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 6;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                receiptTable.AddCell(cell);
                contain = "3. This is system generated receipt. Hence, signature not require.";
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

                cell = new PdfPCell(new Phrase(new Chunk("For " + societyReceipt.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
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

                cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
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
                _modelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            //return _PDFService.PdfMsCreator(PdfPTableList, Society);
            return new PDFService(_modelState).PdfMsCreator(PdfPTableList);
        }

        public bool GenerateReceiptForOnHoldReciept(Guid SocietyReceiptOnholdID)
        {
            if (!_cache.GenerateReceiptForOnHoldReciept(SocietyReceiptOnholdID))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
    }
}