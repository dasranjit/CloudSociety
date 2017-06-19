﻿using System;
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
using System.ComponentModel.DataAnnotations;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser,Member,OfficeBearer")]
    public class SocietyBillController : Controller
    {
        private SocietyBillService _service;
        private PDFService _PDFService;
        const string _exceptioncontext = "SocietyBill Controller";

        public SocietyBillController()
        {
            _service = new SocietyBillService(this.ModelState);
            _PDFService = new PDFService(this.ModelState);
        }

        //Retuns MemoryStream of bill
        private MemoryStream PDFMsBill(Guid id) //id=SocietyBillID
        {
            SocietyBill societyBill = _service.GetById(id);
            Society Society = new SocietyService(this.ModelState).GetById(societyBill.SocietyID);
            decimal total;
            String contain = "";
            IEnumerable<SocietyBillChargeHeadWithHead> societyBillChargeHeadList = new SocietyBillChargeHeadService(this.ModelState).ListByParentId(societyBill.SocietyBillID);
            List<PdfPTable> PdfPTableList = new List<PdfPTable>();
            PdfPTable billTable;
            PdfPCell cell;
            try
            {
                //PdfPTableList.Add(_PDFService.CaptionTable(" B I L L ", FontFactory.GetFont("Verdana", 9, Font.BOLD), System.Drawing.Color.LightGray));
                billTable = new PdfPTable(3);
                cell = new PdfPCell(new Phrase(new Chunk(societyBill.SocietyBillSery.Note + " B I L L ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 0;
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                billTable.AddCell(cell);
                contain = "Unit No : " + societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit + Chunk.NEWLINE + "Name : " + societyBill.SocietyMember.Member;
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                contain = societyBill.Particulars + Chunk.NEWLINE;
                if (societyBill.SocietyBillSery.PrintArea != null)
                    if ((bool)societyBill.SocietyBillSery.PrintArea && societyBill.SocietyBuildingUnit.ChargeableArea != null)
                        contain += "Area : " + societyBill.SocietyBuildingUnit.ChargeableArea.ToString();
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                contain = "Bill No     : " + societyBill.BillNo.ToString() + Chunk.NEWLINE;
                contain += "Bill Date  : " + String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate) + Chunk.NEWLINE;
                if (societyBill.DueDate != null)
                {
                    contain += "Due Date  : " + String.Format("{0:dd-MMM-yyyy}", societyBill.DueDate);
                }

                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Nature Of Charges" + Chunk.NEWLINE, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Amount " + Chunk.NEWLINE, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);
                foreach (var chargeHead in societyBillChargeHeadList)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(chargeHead.ChargeHead, FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(chargeHead.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(new Chunk("Gross Amount ", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                billTable.AddCell(cell);
                total = (decimal)societyBill.uAmount + (societyBill.Interest ?? 0) + (societyBill.TaxAmount ?? 0);
                cell = new PdfPCell(new Phrase(new Chunk(total.ToString(), FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                billTable.AddCell(cell);

                contain = "";
                if (societyBill.Arrears != null && societyBill.Arrears != 0)
                {
                    contain = "Add : Arrears" + societyBill.Arrears;
                }
                if (societyBill.InterestArrears != null && societyBill.InterestArrears != 0)
                {
                    contain = contain + ", Add : Interest Arrears" + societyBill.InterestArrears;
                }
                if (societyBill.NonChgArrears != null && societyBill.NonChgArrears != 0)
                {
                    contain = contain + ", Add : NonChgArrears" + societyBill.NonChgArrears;
                }
                if (societyBill.TaxArrears != null && societyBill.TaxArrears != 0)
                {
                    contain = contain + ", Add : Tax Arrears" + societyBill.TaxArrears;
                }

                if (societyBill.Advance != null && societyBill.Advance != 0)
                {
                    contain = string.IsNullOrWhiteSpace(contain) ? "Less : Advance" + societyBill.Advance + "." : contain + ", Less : Advance" + societyBill.Advance + ".";
                }
                if (contain != "")
                {
                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9, Font.NORMAL))));
                    cell.Colspan = 2;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("")));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(new Chunk("Net Amount Payable ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(societyBill.Payable.ToString(), FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : " + ChangeCurrencyToWords.changeCurrencyToWords(societyBill.Payable.ToString()).ToUpper(), FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(societyBill.SocietyBillSery.Terms, FontFactory.GetFont("Verdana", 8))));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("FOR " + societyBill.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Checked By : ", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(societyBill.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("E. & O. E.", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                billTable.AddCell(cell);

                PdfPTableList.Add(billTable);

            }
            catch (DocumentException ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return _PDFService.PdfMsCreator(PdfPTableList, Society, SpacingAfterTable: 8f);
        }

        //Retuns MemoryStream of bill and receipt
        private MemoryStream PDFMsBillReceipt(Guid societyBillId)
        {
            SocietyBill societyBill = _service.GetById(societyBillId);
            Society Society = new SocietyService(this.ModelState).GetById(societyBill.SocietyID);
            decimal total;
            String contain = "";
            IEnumerable<SocietyBillChargeHeadWithHead> societyBillChargeHeadList;
            List<PdfPTable> PdfPTableList = new List<PdfPTable>();
            PdfPTable billTable;
            PdfPCell cell;
            try
            {
                societyBillChargeHeadList = new SocietyBillChargeHeadService(this.ModelState).ListByParentId(societyBill.SocietyBillID);
                //PdfPTableList.Add(_PDFService.CaptionTable(" B I L L ", FontFactory.GetFont("Verdana", 9, Font.BOLD), System.Drawing.Color.LightGray));
                billTable = new PdfPTable(3);

                cell = new PdfPCell(new Phrase(new Chunk(" B I L L ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 1.2f;
                cell.BorderWidthBottom = 1.2f;
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                billTable.AddCell(cell);

                contain = "Unit No : " + societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit + Chunk.NEWLINE + "Name : " + societyBill.SocietyMember.Member;
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                contain = societyBill.Particulars + Chunk.NEWLINE;
                if ((bool)societyBill.SocietyBillSery.PrintArea && societyBill.SocietyBuildingUnit.ChargeableArea != null)
                    contain += "Area : " + societyBill.SocietyBuildingUnit.ChargeableArea.ToString();
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                contain = "Bill No     : " + societyBill.BillNo.ToString() + Chunk.NEWLINE;
                contain += "Bill Date  : " + String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate).ToString() + Chunk.NEWLINE;
                if (societyBill.DueDate != null)
                    contain += "Due Date  : " + String.Format("{0:dd-MMM-yyyy}", societyBill.DueDate);

                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Nature Of Charges" + Chunk.NEWLINE, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Amount " + Chunk.NEWLINE, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);
                foreach (var chargeHead in societyBillChargeHeadList)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(chargeHead.ChargeHead, FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(chargeHead.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(new Chunk("Gross Amount ", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                billTable.AddCell(cell);
                total = (decimal)societyBill.uAmount + (societyBill.Interest ?? 0) + (societyBill.TaxAmount ?? 0);
                cell = new PdfPCell(new Phrase(new Chunk(total.ToString(), FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                billTable.AddCell(cell);
                contain = "";
                if (societyBill.Arrears != null && societyBill.Arrears != 0)
                {
                    contain = "Add : Arrears" + societyBill.Arrears;
                }
                if (societyBill.InterestArrears != null && societyBill.InterestArrears != 0)
                {
                    contain = contain + ", Add : Interest Arrears" + societyBill.InterestArrears;
                }
                if (societyBill.NonChgArrears != null && societyBill.NonChgArrears != 0)
                {
                    contain = contain + ", Add : NonChgArrears" + societyBill.NonChgArrears;
                }
                if (societyBill.TaxArrears != null && societyBill.TaxArrears != 0)
                {
                    contain = contain + ", Add : Tax Arrears" + societyBill.TaxArrears;
                }

                if (societyBill.Advance != null && societyBill.Advance != 0)
                {
                    contain = string.IsNullOrWhiteSpace(contain) ? "Less : Advance" + societyBill.Advance + "." : contain + ", Less : Advance" + societyBill.Advance + ".";
                }
                if (contain != "")
                {
                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                }
                /*
                 * *
                 if (societyBill.Tax != null && societyBill.TaxAmount != null && societyBill.TaxAmount != 0)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(societyBill.Tax, FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(societyBill.TaxAmount.ToString(), FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase(new Chunk("Add : Previous Dues", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("ToDo", FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Add : Previous Penalty", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("ToDo", FontFactory.GetFont("Verdana", 9))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                 */

                cell = new PdfPCell(new Phrase(new Chunk("Net Amount Payable ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(societyBill.Payable.ToString(), FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                if (societyBill.Payable == 0)
                    cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                else
                    cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : " + ChangeCurrencyToWords.changeCurrencyToWords(societyBill.Payable.ToString()).ToUpper(), FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(societyBill.SocietyBillSery.Terms, FontFactory.GetFont("Verdana", 8))));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("FOR " + societyBill.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(" ")));
                cell.Colspan = 3;
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Checked By : ", FontFactory.GetFont("Verdana", 9))));
                cell.BorderWidthLeft = 1.2f;
                cell.BorderWidthRight = 0;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(societyBill.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.BorderWidthLeft = 0;
                cell.BorderWidthRight = 1.2f;
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 1.2f;
                billTable.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk(" E. & O. E.", FontFactory.GetFont("Verdana", 9))));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                billTable.AddCell(cell);
                //Bill table added here 
                PdfPTableList.Add(billTable);

                DateTime ssd = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(societyBill.SocietySubscriptionID).SubscriptionStart;
                DateTime? smd = _service.GetPrevBillDateBySocietyBuildingUnitID(societyBill.SocietyBuildingUnitID, societyBill.BillDate);
                DateTime sd = smd == null ? ssd : (DateTime)smd;
                DateTime ed = societyBill.BillDate;
                IList<SocietyReceipt> societyReceiptList = (IList<SocietyReceipt>)new SocietyReceiptService(this.ModelState).ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBill.SocietyBuildingUnitID, societyBill.BillAbbreviation, sd, ed);
                IList<SocietyCollectionReversal> societyCollectionReversalList = (IList<SocietyCollectionReversal>)new SocietyCollectionReversalService(this.ModelState).ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBill.SocietyBuildingUnitID, societyBill.BillAbbreviation, sd, ed);
                if (societyReceiptList.Count > 0 || societyCollectionReversalList.Count > 0)
                {
                    decimal totalAmount = 0;
                    PdfPTable receiptTable = new PdfPTable(6);
                    cell = new PdfPCell(new Phrase(new Chunk("R E C E I P T ", FontFactory.GetFont("Verdana", 11, Font.BOLD))));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 1.2f;
                    cell.BorderWidthBottom = 1.2f;
                    receiptTable.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(societyBill.Society.Name + "\n", FontFactory.GetFont("Verdana", 15)));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    receiptTable.AddCell(cell);

                    contain = "Regd. No. : " + societyBill.Society.RegistrationNo + " Dated : " + String.Format("{0:dd-MMM-yyyy}", societyBill.Society.RegistrationDate);
                    if (societyBill.Society.TaxRegistrationNo != null)
                    {
                        contain = contain + ", Services Tax No : " + societyBill.Society.TaxRegistrationNo;
                    }
                    cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 7)));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    receiptTable.AddCell(cell);

                    contain = societyBill.Society.Address + ", " + societyBill.Society.City + " - " + societyBill.Society.PIN;
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
                    foreach (var societyReceipt in societyReceiptList)
                    {
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
                                if (societyReceipt.Bank == null)
                                    contain = "";
                                else
                                    contain = societyReceipt.Bank.Name;
                                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 1.2f;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                totalAmount += societyReceipt.Amount;
                            }
                            else
                            {
                                //                                cell = new PdfPCell(new Phrase(new Chunk("Details not required", FontFactory.GetFont("Verdana", 9))));
                                cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                                cell.Colspan = 3;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 1.2f;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                totalAmount += societyReceipt.Amount;
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
                            cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.BorderWidthLeft = 0.5f;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 0.5f;
                            cell.BorderWidthBottom = 0;
                            receiptTable.AddCell(cell);
                            totalAmount += societyReceipt.Amount;
                        }
                    }
                    // Collection Reversal
                    foreach (var societyCollectionReversal in societyCollectionReversalList)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(societyCollectionReversal.DocNo, FontFactory.GetFont("Verdana", 7))));
                        cell.BorderWidthLeft = 1.2f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", societyCollectionReversal.ReversalDate), FontFactory.GetFont("Verdana", 9))));
                        cell.BorderWidthLeft = 0.5f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        receiptTable.AddCell(cell);
                        if (societyCollectionReversal.SocietyPayMode != null)
                        {
                            if (societyCollectionReversal.SocietyPayMode.AskDetails)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(societyCollectionReversal.PayRefNo, FontFactory.GetFont("Verdana", 9))));
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", societyCollectionReversal.PayRefDate), FontFactory.GetFont("Verdana", 9))));
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                if (societyCollectionReversal.Bank == null)
                                    contain = "";
                                else
                                    contain = societyCollectionReversal.Bank.Name;
                                cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk((societyCollectionReversal.SocietyReceipt.Amount * -1).ToString(), FontFactory.GetFont("Verdana", 9))));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 1.2f;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                totalAmount -= societyCollectionReversal.SocietyReceipt.Amount;
                            }
                            else
                            {
                                //                                cell = new PdfPCell(new Phrase(new Chunk("Details not required", FontFactory.GetFont("Verdana", 9))));
                                cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                                cell.Colspan = 3;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk((societyCollectionReversal.SocietyReceipt.Amount * -1).ToString(), FontFactory.GetFont("Verdana", 9))));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 1.2f;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                totalAmount -= societyCollectionReversal.SocietyReceipt.Amount;
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
                            cell = new PdfPCell(new Phrase(new Chunk((societyCollectionReversal.SocietyReceipt.Amount * -1).ToString(), FontFactory.GetFont("Verdana", 9))));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.BorderWidthLeft = 0.5f;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 0.5f;
                            cell.BorderWidthBottom = 0;
                            receiptTable.AddCell(cell);
                            totalAmount -= societyCollectionReversal.SocietyReceipt.Amount;
                        }
                    }
                    cell = new PdfPCell(new Phrase(new Chunk(" ")));
                    cell.Colspan = 6;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0;
                    receiptTable.AddCell(cell);
                    contain = (totalAmount >= 0 ? "1. We received with thanks from " : "1. There was collection reversal from ") + societyBill.SocietyMember.Member + ", Flat No. : " + societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit + " the sum of Rs. " + totalAmount;
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
                    cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : " + (totalAmount < 0 ? "MINUS " : "") + ChangeCurrencyToWords.changeCurrencyToWords(Math.Abs(totalAmount).ToString()).ToUpper(), FontFactory.GetFont("Verdana", 8, Font.BOLD))));
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

                    cell = new PdfPCell(new Phrase(new Chunk("For " + societyBill.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
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

                    cell = new PdfPCell(new Phrase(new Chunk(societyBill.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    receiptTable.AddCell(cell);

                    PdfPTableList.Add(receiptTable);
                }
            }
            catch (DocumentException ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }

            return _PDFService.PdfMsCreator(PdfPTableList, Society, SpacingAfterTable: 8f);  //(PdfPTableList, 8f, Society, "");
        }

        //Retuns MemoryStream of all bills and receipts
        private MemoryStream PDFMsBillsReceipts(IEnumerable<SocietyBill> societyBillList)
        {
            decimal total;
            //data container
            String contain = "";
            IEnumerable<SocietyBillChargeHeadWithHead> societyBillChargeHeadList;
            //mem buffer
            MemoryStream ms = new MemoryStream();
            //PageSize
            Rectangle rect = PageSize.A4;
            //the document
            Document document = new Document(rect);
            //the writer
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            PdfPTable billTable;
            PdfPCell cell;
            Society Society = new SocietyService(this.ModelState).GetById(societyBillList.FirstOrDefault().SocietyID);
            try
            {
                PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                writer.PageEvent = PageEventHandler;
                PageEventHandler.FooterRight = "";
                //PageEventHandler.Society = new SocietyService(this.ModelState).GetById(societyBillList.FirstOrDefault().SocietyID);
                document.SetMargins(20, 20, 75, 35);
                document.Open();
                foreach (var societyBill in societyBillList)
                {
                    societyBillChargeHeadList = new SocietyBillChargeHeadService(this.ModelState).ListByParentId(societyBill.SocietyBillID);
                    document.NewPage();

                    PdfPTable headTable = _PDFService.SocietyHeaderTable(Society);
                    headTable.TotalWidth = rect.Width - 80;
                    cell = new PdfPCell();
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 1.2f;
                    cell.BorderWidthBottom = 0;
                    headTable.SpacingAfter = 5f;
                    headTable.AddCell(cell);
                    headTable.WriteSelectedRows(0, -1, rect.GetLeft(40), rect.GetTop(20), writer.DirectContent);

                    billTable = new PdfPTable(3);
                    billTable.TotalWidth = rect.Width - 40;

                    cell = new PdfPCell(new Phrase(new Chunk(" B I L L ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 1.2f;
                    cell.BorderWidthBottom = 1.2f;
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    billTable.AddCell(cell);

                    contain = "Unit No : " + societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit + Chunk.NEWLINE + "Name : " + societyBill.SocietyMember.Member;
                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);

                    contain = societyBill.Particulars + Chunk.NEWLINE;
                    if (societyBill.SocietyBillSery.PrintArea != null)
                        if ((bool)societyBill.SocietyBillSery.PrintArea && societyBill.SocietyBuildingUnit.ChargeableArea != null)
                            contain += "Area : " + societyBill.SocietyBuildingUnit.ChargeableArea.ToString();
                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);

                    contain = "Bill No     : " + societyBill.BillNo.ToString() + Chunk.NEWLINE;
                    contain += "Bill Date  : " + String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate).ToString() + Chunk.NEWLINE;
                    if (societyBill.DueDate != null)
                        contain += "Due Date  : " + String.Format("{0:dd-MMM-yyyy}", societyBill.DueDate);

                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk("Nature Of Charges" + Chunk.NEWLINE, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("Amount " + Chunk.NEWLINE, FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);
                    foreach (var chargeHead in societyBillChargeHeadList)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(chargeHead.ChargeHead, FontFactory.GetFont("Verdana", 9))));
                        cell.Colspan = 2;
                        //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BorderWidthLeft = 1.2f;
                        cell.BorderWidthRight = 0.5f;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        billTable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(chargeHead.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderWidthLeft = 0;
                        cell.BorderWidthRight = 1.2f;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        billTable.AddCell(cell);
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Gross Amount ", FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    total = (decimal)societyBill.uAmount + (societyBill.Interest ?? 0) + (societyBill.TaxAmount ?? 0);
                    cell = new PdfPCell(new Phrase(new Chunk(total.ToString(), FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    contain = "";
                    if (societyBill.Arrears != null && societyBill.Arrears != 0)
                    {
                        contain = "Add : Arrears " + societyBill.Arrears;
                    }
                    if (societyBill.InterestArrears != null && societyBill.InterestArrears != 0)
                    {
                        contain = contain + ", Add : Interest Arrears " + societyBill.InterestArrears;
                    }
                    if (societyBill.NonChgArrears != null && societyBill.NonChgArrears != 0)
                    {
                        contain = contain + ", Add : NonChgArrears " + societyBill.NonChgArrears;
                    }
                    if (societyBill.TaxArrears != null && societyBill.TaxArrears != 0)
                    {
                        contain = contain + ", Add : Tax Arrears " + societyBill.TaxArrears;
                    }

                    if (societyBill.Advance != null && societyBill.Advance != 0)
                    {
                        contain = string.IsNullOrWhiteSpace(contain) ? "Less : Advance " + societyBill.Advance + "." : contain + ", Less : Advance " + societyBill.Advance + ".";
                    }

                    if (contain != "")
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                        cell.Colspan = 2;
                        //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderWidthLeft = 1.2f;
                        cell.BorderWidthRight = 0;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        billTable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderWidthLeft = 0;
                        cell.BorderWidthRight = 1.2f;
                        cell.BorderWidthTop = 0.5f;
                        cell.BorderWidthBottom = 0;
                        billTable.AddCell(cell);
                    }
                    /*
                     * *
                     if (societyBill.Tax != null && societyBill.TaxAmount != null && societyBill.TaxAmount != 0)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(societyBill.Tax, FontFactory.GetFont("Verdana", 9))));
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderWidthLeft = 1.2f;
                        cell.BorderWidthRight = 0.5f;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        billTable.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(societyBill.TaxAmount.ToString(), FontFactory.GetFont("Verdana", 9))));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.BorderWidthLeft = 0;
                        cell.BorderWidthRight = 1.2f;
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        billTable.AddCell(cell);
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Add : Previous Dues", FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("ToDo", FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk("Add : Previous Penalty", FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("ToDo", FontFactory.GetFont("Verdana", 9))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                     */

                    cell = new PdfPCell(new Phrase(new Chunk("Net Amount Payable ", FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0.5f;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(societyBill.Payable.ToString(), FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : " + ChangeCurrencyToWords.changeCurrencyToWords(societyBill.Payable.ToString()).ToUpper(), FontFactory.GetFont("Verdana", 9, Font.BOLD))));
                    cell.Colspan = 3;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0.5f;
                    cell.BorderWidthBottom = 0.5f;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk(societyBill.SocietyBillSery.Terms, FontFactory.GetFont("Verdana", 8))));
                    cell.Colspan = 3;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk(" ")));
                    cell.Colspan = 3;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk("FOR " + societyBill.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk(" ")));
                    cell.Colspan = 3;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(" ")));
                    cell.Colspan = 3;
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 0;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk("Checked By : ", FontFactory.GetFont("Verdana", 9))));
                    cell.BorderWidthLeft = 1.2f;
                    cell.BorderWidthRight = 0;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk(societyBill.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 1.2f;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthBottom = 1.2f;
                    billTable.AddCell(cell);

                    cell = new PdfPCell(new Phrase(new Chunk(" E. & O. E.", FontFactory.GetFont("Verdana", 9))));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Border = 0;
                    billTable.AddCell(cell);
                    //Bill table added here 
                    document.Add(billTable);
                    document.Add(new Paragraph("\n"));

                    DateTime ssd = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(societyBill.SocietySubscriptionID).SubscriptionStart;
                    DateTime? smd = _service.GetPrevBillDateBySocietyBuildingUnitID(societyBill.SocietyBuildingUnitID, societyBill.BillDate);
                    DateTime sd = smd == null ? ssd : (DateTime)smd;
                    DateTime ed = societyBill.BillDate;
                    IList<SocietyReceipt> societyReceiptList = (IList<SocietyReceipt>)new SocietyReceiptService(this.ModelState).ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBill.SocietyBuildingUnitID, societyBill.BillAbbreviation, sd, ed);
                    IList<SocietyCollectionReversal> societyCollectionReversalList = (IList<SocietyCollectionReversal>)new SocietyCollectionReversalService(this.ModelState).ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBill.SocietyBuildingUnitID, societyBill.BillAbbreviation, sd, ed);
                    if (null != societyReceiptList && null != societyCollectionReversalList)
                    {
                        if (societyReceiptList.Count > 0 || societyCollectionReversalList.Count > 0)
                        {
                            decimal totalAmount = 0;
                            PdfPTable receiptTable = new PdfPTable(6);
                            cell = new PdfPCell(new Phrase(new Chunk("R E C E I P T ", FontFactory.GetFont("Verdana", 11, Font.BOLD))));
                            cell.Colspan = 6;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.BorderWidthLeft = 1.2f;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 1.2f;
                            cell.BorderWidthBottom = 1.2f;
                            receiptTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(societyBill.Society.Name + "\n", FontFactory.GetFont("Verdana", 15)));
                            cell.Colspan = 6;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.BorderWidthLeft = 1.2f;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            receiptTable.AddCell(cell);

                            contain = "Regd. No. : " + societyBill.Society.RegistrationNo + " Dated : " + String.Format("{0:dd-MMM-yyyy}", societyBill.Society.RegistrationDate);
                            if (societyBill.Society.TaxRegistrationNo != null)
                            {
                                contain = contain + ", Services Tax No : " + societyBill.Society.TaxRegistrationNo;
                            }
                            cell = new PdfPCell(new Paragraph(contain, FontFactory.GetFont("Verdana", 7)));
                            cell.Colspan = 6;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.BorderWidthLeft = 1.2f;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            receiptTable.AddCell(cell);

                            contain = societyBill.Society.Address + ", " + societyBill.Society.City + " - " + societyBill.Society.PIN;
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
                            foreach (var societyReceipt in societyReceiptList)
                            {
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
                                        if (societyReceipt.Bank == null)
                                            contain = "";
                                        else
                                            contain = societyReceipt.Bank.Name;
                                        cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 0;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 1.2f;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        totalAmount += societyReceipt.Amount;
                                    }
                                    else
                                    {
                                        //                                cell = new PdfPCell(new Phrase(new Chunk("Details not required", FontFactory.GetFont("Verdana", 9))));
                                        cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                                        cell.Colspan = 3;
                                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 0;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 1.2f;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        totalAmount += societyReceipt.Amount;
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
                                    cell = new PdfPCell(new Phrase(new Chunk(societyReceipt.Amount.ToString(), FontFactory.GetFont("Verdana", 9))));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.BorderWidthLeft = 0.5f;
                                    cell.BorderWidthRight = 1.2f;
                                    cell.BorderWidthTop = 0.5f;
                                    cell.BorderWidthBottom = 0;
                                    receiptTable.AddCell(cell);
                                    totalAmount += societyReceipt.Amount;
                                }
                            }
                            // Collection Reversal
                            foreach (var societyCollectionReversal in societyCollectionReversalList)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(societyCollectionReversal.DocNo, FontFactory.GetFont("Verdana", 7))));
                                cell.BorderWidthLeft = 1.2f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", societyCollectionReversal.ReversalDate), FontFactory.GetFont("Verdana", 9))));
                                cell.BorderWidthLeft = 0.5f;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                receiptTable.AddCell(cell);
                                if (societyCollectionReversal.SocietyPayMode != null)
                                {
                                    if (societyCollectionReversal.SocietyPayMode.AskDetails)
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk(societyCollectionReversal.PayRefNo, FontFactory.GetFont("Verdana", 9))));
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 0;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", societyCollectionReversal.PayRefDate), FontFactory.GetFont("Verdana", 9))));
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 0;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        if (societyCollectionReversal.Bank == null)
                                            contain = "";
                                        else
                                            contain = societyCollectionReversal.Bank.Name;
                                        cell = new PdfPCell(new Phrase(new Chunk(contain, FontFactory.GetFont("Verdana", 9))));
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 0;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk((societyCollectionReversal.SocietyReceipt.Amount * -1).ToString(), FontFactory.GetFont("Verdana", 9))));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 1.2f;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        totalAmount -= societyCollectionReversal.SocietyReceipt.Amount;
                                    }
                                    else
                                    {
                                        //                                cell = new PdfPCell(new Phrase(new Chunk("Details not required", FontFactory.GetFont("Verdana", 9))));
                                        cell = new PdfPCell(new Phrase(new Chunk("", FontFactory.GetFont("Verdana", 9))));
                                        cell.Colspan = 3;
                                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 0;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk((societyCollectionReversal.SocietyReceipt.Amount * -1).ToString(), FontFactory.GetFont("Verdana", 9))));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.BorderWidthLeft = 0.5f;
                                        cell.BorderWidthRight = 1.2f;
                                        cell.BorderWidthTop = 0.5f;
                                        cell.BorderWidthBottom = 0;
                                        receiptTable.AddCell(cell);
                                        totalAmount -= societyCollectionReversal.SocietyReceipt.Amount;
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
                                    cell = new PdfPCell(new Phrase(new Chunk((societyCollectionReversal.SocietyReceipt.Amount * -1).ToString(), FontFactory.GetFont("Verdana", 9))));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.BorderWidthLeft = 0.5f;
                                    cell.BorderWidthRight = 1.2f;
                                    cell.BorderWidthTop = 0.5f;
                                    cell.BorderWidthBottom = 0;
                                    receiptTable.AddCell(cell);
                                    totalAmount -= societyCollectionReversal.SocietyReceipt.Amount;
                                }
                            }
                            cell = new PdfPCell(new Phrase(new Chunk(" ")));
                            cell.Colspan = 6;
                            cell.BorderWidthLeft = 1.2f;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 0.5f;
                            cell.BorderWidthBottom = 0;
                            receiptTable.AddCell(cell);
                            contain = (totalAmount >= 0 ? "1. We received with thanks from " : "1. There was collection reversal from ") + societyBill.SocietyMember.Member + ", Flat No. : " + societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit + " the sum of Rs. " + totalAmount;
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
                            //cell = new PdfPCell(new Phrase(new Chunk(" ")));
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
                            cell = new PdfPCell(new Phrase(new Chunk("IN WORDS : " + (totalAmount < 0 ? "MINUS " : "") + ChangeCurrencyToWords.changeCurrencyToWords(Math.Abs(totalAmount).ToString()).ToUpper(), FontFactory.GetFont("Verdana", 8, Font.BOLD))));
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

                            cell = new PdfPCell(new Phrase(new Chunk("For " + societyBill.Society.Name.ToUpper(), FontFactory.GetFont("Verdana", 9))));
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

                            cell = new PdfPCell(new Phrase(new Chunk(societyBill.Society.Signatory, FontFactory.GetFont("Verdana", 9))));
                            cell.Colspan = 3;
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 1.2f;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 1.2f;
                            receiptTable.AddCell(cell);

                            document.Add(receiptTable);
                        }
                    }
                }
            }
            catch (DocumentException ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
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

        // GET: /To display list of SocietyBill added by Ranjit
        [HttpGet]
        public ActionResult BillAbbreviation(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societyBillSeriesList = (List<CloudSocietyEntities.SocietyBillSeries>)new CloudSociety.Services.SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(id).SocietyID);
            if (societyBillSeriesList.Count == 1)
                return RedirectToAction("Index", new { id = id, billAbbreviation = societyBillSeriesList.FirstOrDefault().BillAbbreviation });
            else
                ViewBag.SocietyBillSeriesList = societyBillSeriesList;
            return View();
        }

        [HttpPost]
        public ActionResult BillAbbreviation(Guid id, SocietyBill societyBill) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            return RedirectToAction("Index", new { id = id, billAbbreviation = societyBill.BillAbbreviation });
        }

        // GET: /To display list of SocietyBill added by Ranjit
        [HttpGet]
        public ActionResult Index(Guid id, String billAbbreviation) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.BillAbbreviation = billAbbreviation;
            ViewBag.BillDatesList = _service.ListBillDatesBySocietySubscriptionID(id, billAbbreviation);
            return View();
        }

        // POST: //To display list of SocietyBill added by Ranjit
        [HttpPost]
        public ActionResult Index(Guid id, String billAbbreviation, SocietyBill societyBill) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societyId = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyBillDate = societyBill.BillDate;
            ViewBag.BillAbbreviation = billAbbreviation;
            var billList = _service.ListBySocietyIDBillDateBillAbbreviation(societyId, societyBill.BillDate, billAbbreviation);
            DateTime latestBillDate = _service.ListBillDatesBySocietySubscriptionID(id, billAbbreviation).Max();
            ViewBag.IsToShowSMSLink = latestBillDate == societyBill.BillDate && billList.FirstOrDefault().Society.SMS;
            return View("BillList", billList);
        }

        public ActionResult MemberBillIndex(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowMemberMenu = true;
            MembershipUser user = Membership.GetUser();
            Guid userid = (Guid)user.ProviderUserKey;
            var societymemberid = (Guid)new Services.UserDetailService(this.ModelState).GetById(userid).SocietyMemberID;
            var societyMemberService = new CloudSociety.Services.SocietyMemberService(this.ModelState);
            var sm = societyMemberService.GetById(societymemberid);
            ViewBag.MemberName = sm.Member;
            ViewBag.ShowFinalReports = societySubscriptionService.GetById(id).Closed;
            ViewBag.ShowCommunication = societyMemberService.IsCommunicationEnabled(sm.SocietyID);
            return View(_service.ListBySocietyMemberIDSocietySubscriptionID(societymemberid, id));
        }

        // GET: /Details of SocietyBill added by Ranjit
        public ActionResult Details(Guid id, Guid SocietySubscriptionID, bool sms)
        {
            ViewBag.SocietySubscriptionID = SocietySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
            ViewBag.IsMember = (Roles.IsUserInRole("Member") || Roles.IsUserInRole("OfficeBearer"));
            ViewBag.SocietyBillChargeHeadList = new CloudSociety.Services.SocietyBillChargeHeadService(this.ModelState).ListByParentId(id);
            ViewBag.SMS = sms;
            var societyBill = _service.GetById(id);
            return View(societyBill);
        }

        // GET: Method to Print bill added By Ranjit        
        public ActionResult Print(Guid id)
        {
            ViewBag.SocietyBillChargeHeadList = new CloudSociety.Services.SocietyBillChargeHeadService(this.ModelState).ListByParentId(id);
            return View(_service.GetById(id));
        }

        // Method to generate society bill and return. Added By Ranjit
        public FileStreamResult CreatePdf(Guid id)
        {
            MemoryStream ms = PDFMsBill(id);
            string billDate = String.Format("{0:dd-MMM-yyyy}", _service.GetById(id).BillDate);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            string arrg = "attachment;filename=BillOf" + String.Format("{0:dd-MMM-yyyy}", billDate) + "." + "pdf";
            Response.AddHeader("content-disposition", arrg);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Method to show Success message. Added by Ranjit
        public ActionResult SendSuccess(Guid id, string billAbbreviation)
        {
            ViewBag.BillAbbreviation = billAbbreviation;
            ViewBag.SocietySubscriptionID = id;
            return View();
        }

        //Method to send mail. Added by Ranjit 
        public ActionResult SendMail(Guid id)
        {
            SocietyBill societyBill = _service.GetById(id);
            ViewBag.Result = "Faild to send mail.";
            ViewBag.BillAbbreviation = societyBill.BillAbbreviation;
            ViewBag.SocietySubscriptionID = societyBill.SocietySubscriptionID;
            //Guid societyMemberID = new CloudSociety.Services.SocietyBillService(this.ModelState).GetById(id).SocietyMemberID;
            MemoryStream ms = PDFMsBill(id);
            SmtpClient mailClient = new SmtpClient();
            MailMessage message = new MailMessage();
            string mailFrom, mailTo, mailBody, billDate, fileName;

            billDate = String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate);
            fileName = "BillOf" + String.Format("{0:dd-MMM-yyyy}", billDate) + "." + "pdf";
            message.IsBodyHtml = true;
            mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
            mailTo = societyBill.SocietyMember.EmailId;
            //var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySocietyMemberID(societyBill.SocietyMemberID);
            //if (ud != null)
            //{
            //    var usr = Membership.GetUser((object)ud.UserID);
            //    if (usr != null)
            //        mailTo = usr.Email;
            //}
            //var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySocietyMemberID(societyMemberID);
            //mailTo = Membership.GetUser((object)ud.UserID).Email;
            if (!string.IsNullOrEmpty(mailFrom) && !String.IsNullOrEmpty(mailTo) && !String.IsNullOrWhiteSpace(mailTo))
            {
                try
                {
                    message.From = new MailAddress(mailFrom);
                    message.To.Add(new MailAddress(mailTo));
                    //message.CC.Add(new MailAddress("CC@yahoo.com", "Display name CC")); 
                    message.Subject = "Society Bill for " + societyBill.Society.Name;
                    mailBody = new CloudSociety.Services.AppInfoService(this.ModelState).Get().BillMailBody;
                    mailBody = mailBody.Replace("&&Member&&", societyBill.SocietyMember.Member);
                    mailBody = mailBody.Replace("&&BillNo&&", societyBill.BillNo);
                    mailBody = mailBody.Replace("&&BillDate&&", billDate);
                    mailBody = mailBody.Replace("&&Unit&&", societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit);
                    mailBody = mailBody.Replace("&&Particulars&&", societyBill.Particulars.Replace("Period : ", ""));
                    mailBody = mailBody.Replace("&&SocietyName&&", societyBill.Society.Name);
                    mailBody = mailBody.Replace("&&Amount&&", societyBill.Payable.ToString());
                    mailBody = mailBody.Replace("&&DueDate&&", string.Format("{0:dd-MMM-yyyy}", societyBill.DueDate));
                    mailBody = mailBody.Replace("&&SocietyContactPerson&&", societyBill.Society.ContactPerson);
                    mailBody = mailBody.Replace("&&SocietyContactNumber&&", societyBill.Society.Mobile);
                    mailBody = mailBody.Replace("&&SocietyContactEmailId&&", societyBill.Society.EMailId);
                    mailBody = mailBody.Replace("&&ToDayDate&&", string.Format("{0:dd-MMM-yyyy}", DateTime.Now));
                    message.Body = mailBody;
                    Attachment a = new Attachment(ms, fileName, "application/pdf");
                    message.Attachments.Add(a);
                    //if (!string.IsNullOrEmpty(mailFrom) && mailTo != null)
                    mailClient.Send(message);
                    ViewBag.Result = "Your Mail has been send successfully.";
                }
                catch (Exception ex)
                {
                    this.ModelState.AddModelError("SendMail", ex.Message + ", " + ex.InnerException.Message);
                }
            }
            //return RedirectToAction("SendSuccess", new { id = societyBill.SocietySubscriptionID, billAbbreviation = societyBill.BillAbbreviation });
            return View("SendSuccess", new { id = societyBill.SocietySubscriptionID, billAbbreviation = societyBill.BillAbbreviation });
        }

        //Method to send SMS. Added by Ranjit 
        public ActionResult SendSMS(Guid id)
        {
            SocietyBill societyBill = _service.GetById(id);
            ViewBag.Result = "Failed to send SMS.";
            ViewBag.BillAbbreviation = societyBill.BillAbbreviation;
            ViewBag.SocietySubscriptionID = societyBill.SocietySubscriptionID;
            Guid societyMemberID = new CloudSociety.Services.SocietyBillService(this.ModelState).GetById(id).SocietyMemberID;
            string messageText, smsUrl;

            if (!string.IsNullOrEmpty(societyBill.SocietyMember.MobileNo))
            {
                if (societyBill.SocietyMember.MobileNo.Length == 10)
                {
                    societyBill.SocietyMember.MobileNo = "91" + societyBill.SocietyMember.MobileNo;
                    try
                    {
                        messageText = "Dear Member,\nYour Bill No. " + societyBill.BillNo;
                        messageText += " Dt. " + String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate);
                        messageText += " Amt. Rs. " + societyBill.Payable + "/- is due on " + String.Format("{0:dd-MMM-yyyy}", societyBill.DueDate);
                        messageText += "\nIf paid please ignore.\nFor " + societyBill.Society.Name;
                        smsUrl = System.Configuration.ConfigurationManager.AppSettings["SMS_URL"];
                        smsUrl = smsUrl.Replace("**MobileNo**", societyBill.SocietyMember.MobileNo);
                        smsUrl = smsUrl.Replace("**Message**", messageText);
                        if (new CloudSocietyLib.MessagingService.TextMessagingService().SendSMS(smsUrl))
                        {
                            ViewBag.Result = "SMS has been send successfully.";
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ModelState.AddModelError("SendSMS", ex.Message + ", " + ex.InnerException.Message);
                    }
                }
            }
            return View("SendSuccess");
        }

        // Method To Create pdf file of All Society Bills With Society Receipts added by Ranjit
        public FileStreamResult CreateBillsReceiptsPdf(Guid societySubscriptionID, DateTime billDate, String billAbbreviation)
        {
            var societySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(societySubscriptionID);
            var societyBillList = _service.ListBySocietyIDBillDateBillAbbreviation(societySubscription.SocietyID, billDate, billAbbreviation);
            MemoryStream ms = PDFMsBillsReceipts(societyBillList);
            Response.ContentType = "application/pdf";
            string arrg = "attachment;filename=Bills&ReceiptsOf" + String.Format("{0:dd-MMM-yyyy}", billDate) + "." + "pdf";
            Response.AddHeader("content-disposition", arrg);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Method to show Success message of Mail sending. Added by Ranjit
        public ActionResult SendSuccessToAll(Guid id, DateTime billDate, String billAbbreviation)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            IList<SocietyBill> societyBillList = (IList<SocietyBill>)_service.ListBySocietyIDBillDateBillAbbreviation(societySubscription.SocietyID, billDate, billAbbreviation);
            ViewBag.SocietyBillList = societyBillList;
            ViewBag.BillAbbreviation = billAbbreviation;
            return View();
        }

        //Method to send mail To All listed members Added by Ranjit 
        public ActionResult SendMailToAll(Guid societySubscriptionID, DateTime billDate, String billAbbreviation)
        {
            var societySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(societySubscriptionID);
            IList<SocietyBill> societyBillList = (IList<SocietyBill>)_service.ListBySocietyIDBillDateBillAbbreviation(societySubscription.SocietyID, billDate, billAbbreviation);
            try
            {
                foreach (var societyBill in societyBillList)
                {
                    string mailFrom, mailTo, mailBody, formatedbillDate, fileName;

                    mailTo = societyBill.SocietyMember.EmailId;
                    //var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySocietyMemberID(societyBill.SocietyMemberID);
                    //if (ud != null)
                    //{
                    //    var usr = Membership.GetUser((object)ud.UserID);
                    //    if (usr != null)
                    //        mailTo = usr.Email;
                    //}
                    if (!String.IsNullOrEmpty(mailTo) && !String.IsNullOrWhiteSpace(mailTo))
                    {
                        try
                        {

                            MemoryStream ms = PDFMsBillReceipt(societyBill.SocietyBillID);
                            SmtpClient mailClient = new SmtpClient();
                            MailMessage message = new MailMessage();

                            message.IsBodyHtml = true;
                            formatedbillDate = String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate);
                            fileName = "Bill&ReceiptOf" + formatedbillDate + "." + "pdf";
                            mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
                            message.From = new MailAddress(mailFrom);
                            message.To.Add(new MailAddress(mailTo));
                            //message.CC.Add(new MailAddress("CC@yahoo.com", "Display name CC"));                    
                            //message.Subject = "Society Bill";
                            message.Subject = "Society Bill for " + societyBill.Society.Name;
                            mailBody = new CloudSociety.Services.AppInfoService(this.ModelState).Get().BillMailBody;
                            mailBody = mailBody.Replace("&&Member&&", societyBill.SocietyMember.Member);
                            mailBody = mailBody.Replace("&&BillNo&&", societyBill.BillNo);
                            mailBody = mailBody.Replace("&&BillDate&&", formatedbillDate);
                            mailBody = mailBody.Replace("&&Unit&&", societyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyBill.SocietyBuildingUnit.Unit);
                            mailBody = mailBody.Replace("&&Particulars&&", societyBill.Particulars.Replace("Period : ", ""));
                            mailBody = mailBody.Replace("&&SocietyName&&", societyBill.Society.Name);
                            mailBody = mailBody.Replace("&&Amount&&", societyBill.Payable.ToString());
                            mailBody = mailBody.Replace("&&DueDate&&", string.Format("{0:dd-MMM-yyyy}", societyBill.DueDate));
                            mailBody = mailBody.Replace("&&SocietyContactPerson&&", societyBill.Society.ContactPerson);
                            mailBody = mailBody.Replace("&&SocietyContactNumber&&", societyBill.Society.Mobile);
                            mailBody = mailBody.Replace("&&SocietyContactEmailId&&", societyBill.Society.EMailId);
                            mailBody = mailBody.Replace("&&ToDayDate&&", string.Format("{0:dd-MMM-yyyy}", DateTime.Now));

                            message.Body = mailBody;
                            Attachment a = new Attachment(ms, fileName, "application/pdf");
                            message.Attachments.Add(a);
                            if (!string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(mailTo))
                                mailClient.Send(message);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                ViewBag.SMS = false;
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyBillList = societyBillList;
                ViewBag.BillAbbreviation = billAbbreviation;
                return View("SendSuccessToAll", new { id = societySubscriptionID, billDate = billDate, billAbbreviation = billAbbreviation });
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("SendSuccessToAll", ex.Message);
                return RedirectToAction("Index", new { id = societySubscriptionID, billAbbreviation = billAbbreviation });
            }
        }

        //Method to send SMS To All listed members Added by Ranjit 
        public ActionResult SendSMSToAll(Guid societySubscriptionID, DateTime billDate, String billAbbreviation)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(societySubscriptionID);
            IList<SocietyBill> societyBillList = (IList<SocietyBill>)_service.ListBySocietyIDBillDateBillAbbreviation(societySubscription.SocietyID, billDate, billAbbreviation);
            ViewBag.SocietyBillList = societyBillList;
            ViewBag.BillAbbreviation = billAbbreviation;
            string messageText, smsUrl;
            try
            {
                foreach (var societyBill in societyBillList)
                {
                    if (!string.IsNullOrEmpty(societyBill.SocietyMember.MobileNo))
                    {
                        if (societyBill.SocietyMember.MobileNo.Length == 10)
                        {
                            societyBill.SocietyMember.MobileNo = "91" + societyBill.SocietyMember.MobileNo;
                            try
                            {
                                messageText = "Dear Member,\nYour Bill No. " + societyBill.BillNo;
                                messageText += " Dated " + String.Format("{0:dd-MMM-yyyy}", societyBill.BillDate);
                                messageText += " Amount Rs. " + societyBill.Payable + " is due on " + String.Format("{0:dd-MMM-yyyy}", societyBill.DueDate);
                                messageText += "\nIf paid please ignore.\n-For " + societyBill.Society.Name;
                                smsUrl = System.Configuration.ConfigurationManager.AppSettings["SMS_URL"];
                                smsUrl = smsUrl.Replace("**MobileNo**", societyBill.SocietyMember.MobileNo);
                                smsUrl = smsUrl.Replace("**Message**", messageText);
                                if (new CloudSocietyLib.MessagingService.TextMessagingService().SendSMS(smsUrl))
                                {
                                    ViewBag.Result = "SMS has been send successfully.";
                                }
                            }
                            catch (Exception)
                            {
                                //this.ModelState.AddModelError("SendSMS", ex.Message + ", " + ex.InnerException.Message);
                            }
                        }
                    }
                }
                ViewBag.SMS = true;
                ViewBag.BillAbbreviation = billAbbreviation;
                //return RedirectToAction("SendSuccessToAll", new { id = societySubscriptionID, billDate = billDate, billAbbreviation = billAbbreviation });
                return View("SendSuccessToAll");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("SendSuccessToAll", ex.Message);
                return RedirectToAction("Index", new { id = societySubscriptionID, billAbbreviation = billAbbreviation });
            }
        }

        //GET To Generate SocietyBill added by Ranjit 
        [HttpGet]
        public ActionResult Create(Guid id, string billAbbreviation)
        {
            ViewBag.SocietySubscriptionID = id;
            var societysubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            ViewBag.BillAbbreviation = billAbbreviation;
            ViewBag.NoOfCurrentMembers = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetCountBySocietySubscriptionID(id) ?? 0;
            ViewBag.NoOfAllowMembers = societysubscription.NoOfMembers;
            ViewBag.GenerateBill = (ViewBag.NoOfCurrentMembers <= ViewBag.NoOfAllowMembers);
            return View();

        }

        //POST To Generate SocietyBill added by Ranjit 
        [HttpPost]
        public ActionResult Create(Guid id, string billAbbreviation, FormCollection fc)
        {
            var societyId = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;

            try
            {

                if (_service.Generate(societyId, billAbbreviation))
                {
                    return RedirectToAction("Index", "SocietyBillSeries", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societysubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
                    ViewBag.BillAbbreviation = billAbbreviation;
                    ViewBag.NoOfCurrentMembers = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetCountBySocietySubscriptionID(id) ?? 0;
                    ViewBag.NoOfAllowMembers = societysubscription.NoOfMembers;
                    ViewBag.GenerateBill = (ViewBag.NoOfCurrentMembers <= ViewBag.NoOfAllowMembers);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societysubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
                ViewBag.BillAbbreviation = billAbbreviation;
                ViewBag.NoOfCurrentMembers = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetCountBySocietySubscriptionID(id) ?? 0;
                ViewBag.NoOfAllowMembers = societysubscription.NoOfMembers;
                ViewBag.GenerateBill = (ViewBag.NoOfCurrentMembers <= ViewBag.NoOfAllowMembers);
                return View();
            }
        }

        [Authorize(Roles = "Support")]
        public ActionResult Recreate(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societysubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            return View();
        }

        //POST To Recreate AcTransactionAcs from SocietyBill added by Baji on 2-Apr-13
        [HttpPost]
        [Authorize(Roles = "Support")]
        public ActionResult Recreate(Guid id, FormCollection fc)
        {
            try
            {

                if (_service.ReCreateAcTransactionAcs(id))
                {
                    return RedirectToAction("Menu", "SocietySubscription", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societysubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societysubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
                return View();
            }
        }

        // GET: /Method to Delete SocietyBill added by Ranjit
        [HttpGet]
        public ActionResult Delete(Guid societySubscriptionID, DateTime lastBillDate, String billAbbreviation)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.BillAbbreviation = billAbbreviation;
            ViewBag.LastBillDate = lastBillDate;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View();
        }

        [HttpPost]
        public ActionResult Delete(Guid societySubscriptionID, DateTime lastBillDate, String billAbbreviation, SocietyBill SocietyBillToDelete)
        {
            var societyId = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(societySubscriptionID).SocietyID;
            try
            {
                if (_service.Delete(societyId, billAbbreviation))
                {
                    return RedirectToAction("Index", "SocietyBillSeries", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.BillAbbreviation = billAbbreviation;
                    ViewBag.LastBillDate = lastBillDate;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.BillAbbreviation = billAbbreviation;
                ViewBag.LastBillDate = lastBillDate;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View();
            }
        }

        // Called by Ajax from CollectionReversal Create, Edit
        [HttpGet]
        public String LastBillDate(Guid societySubscriptionID, String billAbbreviation)
        {
            var lastbilldate = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(societySubscriptionID, billAbbreviation);
            return String.Format("{0:dd-MMM-yyyy}", lastbilldate);
        }
    }
}