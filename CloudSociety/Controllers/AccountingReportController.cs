﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using CloudSocietyEntities;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CloudSociety.Services;
using System.Web.Security;
//using System.Drawing;
using CloudSocietyLib.Reporting;
using System.Net.Mail;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser,Member,OfficeBearer")]
    public class AccountingReportController : Controller
    {
        const string _exceptioncontext = "AccountingReportController";
        private PDFService _service;
        private PdfPTable Table = null;
        private PdfPCell cell = null;
        private String contain = "";
        private Font FontV9Bold = FontFactory.GetFont("Verdana", 9, Font.BOLD);
        private Font FontV8Bold = FontFactory.GetFont("Verdana", 8, Font.BOLD);
        private Font FontV7Bold = FontFactory.GetFont("Verdana", 7, Font.BOLD);
        private Font FontV9 = FontFactory.GetFont("Verdana", 9);
        private Font FontV8 = FontFactory.GetFont("Verdana", 8);
        private Font FontV7 = FontFactory.GetFont("Verdana", 7);
        private Rectangle rect;
        private float PageWidth;
        public AccountingReportController()
        {
            rect = PageSize.A4;
            PageWidth = rect.Width;
            _service = new PDFService(this.ModelState);
        }
        private String GetOfficeBearerMailIDsBySocietySubscriptionID(Guid id)
        {
            // Membership cessation not taken care off. In case of cessation, user has to manually remove OfficeBearer checkmark
            String MailIDs = "";
            try
            {
                var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
                var SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(SocietySubscription.SocietyID);
                UserDetailService UserDetailService = new CloudSociety.Services.UserDetailService(this.ModelState);
                MembershipUser user;
                Guid UserID;
                UserDetail UserDetail;
                foreach (var SocietyMember in SocietyMemberList)
                {
                    UserDetail = UserDetailService.GetBySocietyMemberID(SocietyMember.SocietyMemberID);
                    if (UserDetail != null)
                    {
                        UserID = UserDetail.UserID;
                        user = Membership.GetUser((object)UserID);
                        if (user != null)
                        {
                            if (Roles.FindUsersInRole("OfficeBearer", user.UserName).Count() > 0)
                            {
                                MailIDs += user.Email + ",";//OfficeBearerMaliID
                            }
                        }
                    }
                }
                return MailIDs;
            }
            catch
            {
                return "";
            }
        }
        private FileStreamResult FileStreamResult(MemoryStream ms, string fileName)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }
        private FileStreamResult FileStreamResult(List<PdfPTable> PDFPTableList, Society Society, string fileName, PdfPTable PdfPHeadingTable = null, float SpacingAfterTable = 8F, string FooterRight = null, string Password = null)
        {
            MemoryStream ms = this._service.PdfMsCreator(PDFPTableList, Society, PdfPHeadingTable, SpacingAfterTable, FooterRight, Password);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Return MemoryStream for Balance Sheet Changed By Baji on 29/8/12
        private MemoryStream MsBalanceSheet(Guid id, DateTime asondate)
        {
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Guid SocietyID = SocietySubscription.SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<AcFinalReport> AcFinalReportList;
            PdfPTable HeadingTable = new PdfPTable(6);
            PdfPTable AssetTable, LiabilityTable;
            decimal CYDrTotal = 0, LYDrTotal = 0, CYCrTotal = 0, LYCrTotal = 0, CYDrGTotal = 0, LYDrGTotal = 0, CYCrGTotal = 0, LYCrGTotal = 0;
            int DrIndex = 1, CrIndex = 1, DrCount = 1, CrCount = 1;
            string Category = "", CrCategory = "", DrCategory = "";
            try
            {
                //
                Table = new PdfPTable(6);
                AssetTable = new PdfPTable(6);
                LiabilityTable = new PdfPTable(6);
                //set width to tables    
                float[] widths = new float[] { 20f, 200f, 60f, 60f, 60f, 60f };
                Table.SetWidthPercentage(widths, rect);
                AssetTable.SetWidthPercentage(widths, rect);
                LiabilityTable.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                //                contain = "BALANCE SHEET as on " + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
                contain = "BALANCE SHEET as on " + String.Format("{0:dd-MMM-yyyy}", asondate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 6;
                HeadingTable.AddCell(cell);
                string[] tdList;
                string CurrentAcYear = "";
                string LastAcYear = "";
                if (SocietySubscription.SubscriptionStart.Month < 4)
                {
                    CurrentAcYear = (SocietySubscription.SubscriptionStart.Year - 1) + " - " + (SocietySubscription.SubscriptionStart.Year);
                    LastAcYear = ((SocietySubscription.SubscriptionStart.Year - 1) - 1) + " - " + ((SocietySubscription.SubscriptionStart.Year) - 1);
                }
                else
                {
                    CurrentAcYear = (SocietySubscription.SubscriptionStart.Year) + " - " + (SocietySubscription.SubscriptionStart.Year + 1);
                    LastAcYear = ((SocietySubscription.SubscriptionStart.Year) - 1) + " - " + ((SocietySubscription.SubscriptionStart.Year + 1) - 1);
                }
                string[] thList = new string[] { "SOURCES OF FUNDS", CurrentAcYear, LastAcYear };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 2;
                    HeadingTable.AddCell(cell);
                }
                for (int i = 0; i < 6; i++)
                {
                    cell = new PdfPCell(new Paragraph((" "), FontV8Bold));
                    cell.BorderWidthLeft = (i != 0 || i != 5) ? 0.025f : 0f;
                    cell.BorderWidthRight = (i != 0 || i != 5) ? 0.025f : 0f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    AssetTable.AddCell(cell);
                }
                for (int i = 0; i < 6; i++)
                {
                    cell = new PdfPCell(new Paragraph((i == 1 ? "APPLICATION OF FUNDS" : ""), FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    AssetTable.AddCell(cell);
                }
                //                AcFinalReportList = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetBalanceSheetReport(id);
                AcFinalReportList = new SocietyService(this.ModelState).GetBalanceSheetReportAsOnDate(SocietyID, asondate);
                if (AcFinalReportList != null)
                {
                    foreach (var BalanceSheet in AcFinalReportList)
                    {
                        if (BalanceSheet.DrCr == "C")
                        {
                            BalanceSheet.amt = -(BalanceSheet.amt ?? 0);
                            BalanceSheet.lyamt = -(BalanceSheet.lyamt ?? 0);
                        }
                        thList = new string[] { "", BalanceSheet.SubCategory, (BalanceSheet.amt ?? 0) != 0 ? (BalanceSheet.amt ?? 0) + "" : "", "", (BalanceSheet.lyamt ?? 0) != 0 ? (BalanceSheet.lyamt ?? 0) + "" : "", "" };
                        // 
                        if (Category != BalanceSheet.Category)
                        {
                            if (BalanceSheet.DrCr == "D")
                            {
                                if (DrCategory != "")
                                {
                                    tdList = new string[] { " ", " ", " ", CYDrTotal != 0 ? CYDrTotal + "" : "", " ", LYDrTotal != 0 ? LYDrTotal + "" : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        AssetTable.AddCell(cell);
                                    }
                                }
                            }
                            else
                            {
                                if (CrCategory != "")
                                {
                                    tdList = new string[] { " ", " ", " ", CYCrTotal != 0 ? CYCrTotal + "" : " ", " ", LYCrTotal != 0 ? LYCrTotal + " " : " " };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        LiabilityTable.AddCell(cell);
                                    }
                                }
                            }
                            if (BalanceSheet.DrCr == "D")
                            {
                                tdList = new string[] { DrIndex + ".", BalanceSheet.Category, "", "", "", "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    AssetTable.AddCell(cell);
                                }
                                DrIndex++;
                                Category = BalanceSheet.Category;
                                DrCategory = Category;
                                CYDrTotal = 0; LYDrTotal = 0;
                            }
                            else
                            {
                                tdList = new string[] { CrIndex + ".", BalanceSheet.Category, "", "", "", "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    LiabilityTable.AddCell(cell);
                                }
                                CrIndex++;
                                Category = BalanceSheet.Category;
                                CrCategory = Category;
                                CYCrTotal = 0; LYCrTotal = 0;
                            }
                        }
                        //
                        if (BalanceSheet.DrCr == "D")
                        {
                            CYDrGTotal += BalanceSheet.amt ?? 0;
                            LYDrGTotal += BalanceSheet.lyamt ?? 0;
                            CYDrTotal += BalanceSheet.amt ?? 0;
                            LYDrTotal += BalanceSheet.lyamt ?? 0;
                            DrCount++;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], (i == 3 || i == 5) ? FontV8Bold : FontV8));
                                cell.HorizontalAlignment = i < 2 ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                                AssetTable.AddCell(cell);
                            }
                        }
                        else
                        {
                            CYCrGTotal += BalanceSheet.amt ?? 0;
                            LYCrGTotal += BalanceSheet.lyamt ?? 0;
                            CYCrTotal += BalanceSheet.amt ?? 0;
                            LYCrTotal += BalanceSheet.lyamt ?? 0;
                            CrCount++;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], (i == 3 || i == 5) ? FontV8Bold : FontV8));
                                cell.HorizontalAlignment = i < 2 ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                                LiabilityTable.AddCell(cell);
                            }
                        }
                    }
                    tdList = new string[] { " ", " ", " ", CYDrTotal != 0 ? CYDrTotal + " " : " ", " ", LYDrTotal != 0 ? LYDrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        AssetTable.AddCell(cell);
                    }
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(" ", FontV8));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        AssetTable.AddCell(cell);
                    }
                    //                    tdList = new string[] { "", "Application Of Fund Total", "", CYDrGTotal != 0 ? CYDrGTotal + "" : "", "", LYDrGTotal != 0 ? LYDrGTotal + "" : "" };
                    tdList = new string[] { "", "APPLICATION OF FUNDS Total", "", CYDrGTotal != 0 ? CYDrGTotal + "" : "", "", LYDrGTotal != 0 ? LYDrGTotal + "" : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        //                        cell = new PdfPCell(new Paragraph(tdList[i], FontV9Bold));
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = i != 0 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                        AssetTable.AddCell(cell);
                    }
                    tdList = new string[] { " ", " ", "", CYCrTotal != 0 ? CYCrTotal + " " : "", " ", LYCrTotal != 0 ? LYCrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        LiabilityTable.AddCell(cell);
                    }
                    tdList = new string[] { "", "SOURCES OF FUNDS Total ", "", CYCrGTotal != 0 ? CYCrGTotal + "" : "", "", LYCrGTotal != 0 ? LYCrGTotal + "" : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        //                        cell = new PdfPCell(new Paragraph(tdList[i], FontV9Bold));
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = i != 0 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                        LiabilityTable.AddCell(cell);
                    }
                    if (CYDrGTotal != CYCrGTotal || LYDrGTotal != LYCrGTotal)
                    {
                        tdList = new string[] { "", "Difference in BalanceSheet : ", "", (CYCrGTotal - CYDrGTotal) + "", "", (LYCrGTotal - LYDrGTotal) + "" };
                        tdList[3] = tdList[3] != "0" ? tdList[3] : "";
                        tdList[5] = tdList[5] != "0" ? tdList[5] : "";
                        for (int i = 0; i < tdList.Length; i++)
                        {
                            cell = new PdfPCell(new Paragraph(tdList[i], i == 1 ? FontFactory.GetFont("Verdana", 9, Font.BOLDITALIC) : FontV9Bold));
                            cell.HorizontalAlignment = i != 0 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                            LiabilityTable.AddCell(cell);
                        }
                    }
                }
                cell = new PdfPCell(LiabilityTable);
                cell.Colspan = 6;
                Table.AddCell(cell);
                cell = new PdfPCell(AssetTable);
                cell.Colspan = 6;
                Table.AddCell(cell);
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this._service.PdfMsCreator(TableList, Society, HeadingTable, 8F);
        }

        //Retuns MemoryStream for Schedule To Balance Sheet Changed By Baji on 29/8/12
        //        private MemoryStream MsScheduleToBalanceSheet(Guid id)
        private MemoryStream MsScheduleToBalanceSheet(Guid id, DateTime asondate)
        {
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Guid SocietyID = SocietySubscription.SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<AcFinalReportSchedule> AcFinalReportScheduleList;
            PdfPTable HeadingTable = new PdfPTable(3);
            PdfPTable AssetTable, LiabilityTable;
            decimal DrTotal = 0, CrTotal = 0;
            int DrIndex = 1, CrIndex = 1;//, DrCount = 1, CrCount = 1;
            string SubCategory = "", CrSubCategory = "", DrSubCategory = "";
            try
            {
                //
                Table = new PdfPTable(3);
                AssetTable = new PdfPTable(3);
                LiabilityTable = new PdfPTable(3);
                //set width to tables    
                float[] widths = new float[] { 20f, 250f, 80f };
                Table.SetWidthPercentage(widths, rect);
                AssetTable.SetWidthPercentage(widths, rect);
                LiabilityTable.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "SCHEDULE TO BALANCE SHEET";
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 3;
                HeadingTable.AddCell(cell);
                string[] tdList;
                string[] thList = new string[] { "", "SOURCES OF FUNDS", "" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                for (int i = 0; i < 3; i++)
                {
                    cell = new PdfPCell(new Paragraph((" "), FontV8Bold));
                    cell.BorderWidthLeft = i == 1 ? 0.025f : 0f;
                    cell.BorderWidthRight = i == 1 ? 0.025f : 0f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    AssetTable.AddCell(cell);
                }
                for (int i = 0; i < 3; i++)
                {
                    cell = new PdfPCell(new Paragraph((i == 1 ? "APPLICATION OF FUNDS" : ""), FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    AssetTable.AddCell(cell);
                }

                //                AcFinalReportScheduleList = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetBalanceSheetSchedule(id);
                AcFinalReportScheduleList = new SocietyService(this.ModelState).GetBalanceSheetScheduleAsOnDate(SocietyID, asondate);
                if (AcFinalReportScheduleList != null)
                {
                    foreach (var BalanceSheet in AcFinalReportScheduleList)
                    {

                        if (BalanceSheet.DrCr == "C")
                        {
                            BalanceSheet.amt = -(BalanceSheet.amt ?? 0);
                        }
                        thList = new string[] { "", BalanceSheet.AcHead, (BalanceSheet.amt ?? 0) != 0 ? (BalanceSheet.amt ?? 0) + "" : "" };
                        //Logic For SubCategory   
                        if (SubCategory != BalanceSheet.SubCategory)
                        {
                            if (BalanceSheet.DrCr == "D")
                            {
                                if (DrSubCategory != "")
                                {
                                    tdList = new string[] { "", "Total ", DrTotal != 0 ? DrTotal + " " : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        AssetTable.AddCell(cell);
                                    }
                                }
                            }
                            else
                            {
                                if (CrSubCategory != "")
                                {
                                    tdList = new string[] { "", "Total ", CrTotal != 0 ? CrTotal + " " : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        LiabilityTable.AddCell(cell);
                                    }
                                }
                            }
                            if (BalanceSheet.DrCr == "D")
                            {
                                tdList = new string[] { DrIndex + ".", BalanceSheet.SubCategory, "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    AssetTable.AddCell(cell);
                                }
                                DrIndex++;
                                SubCategory = BalanceSheet.SubCategory;
                                DrSubCategory = SubCategory;
                                DrTotal = 0; DrTotal = 0;
                            }
                            else
                            {
                                tdList = new string[] { CrIndex + ".", BalanceSheet.SubCategory, "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    LiabilityTable.AddCell(cell);
                                }
                                CrIndex++;
                                SubCategory = BalanceSheet.SubCategory;
                                CrSubCategory = SubCategory;
                                CrTotal = 0; CrTotal = 0;
                            }
                        }
                        //
                        if (BalanceSheet.DrCr == "D")
                        {
                            DrTotal += BalanceSheet.amt ?? 0;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                                cell.HorizontalAlignment = i == 2 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                                AssetTable.AddCell(cell);
                            }
                        }
                        else
                        {
                            CrTotal += BalanceSheet.amt ?? 0;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                                cell.HorizontalAlignment = i == 2 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                                LiabilityTable.AddCell(cell);
                            }
                        }
                    }

                    tdList = new string[] { "", "Total ", DrTotal != 0 ? DrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        AssetTable.AddCell(cell);
                    }
                    tdList = new string[] { "", "Total ", CrTotal != 0 ? CrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        LiabilityTable.AddCell(cell);
                    }
                }

                cell = new PdfPCell(LiabilityTable);
                cell.Colspan = 3;
                Table.AddCell(cell);
                cell = new PdfPCell(AssetTable);
                cell.Colspan = 3;
                Table.AddCell(cell);
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            //return this.FileStreamResult(TableList, Society, "ScheduleToBalanceSheet.pdf", HeadingTable);
            return _service.PdfMsCreator(TableList, Society, HeadingTable, 8F);
        }

        //Retuns MemoryStream for Income Expenditure Statement Changed By Baji on 29/8/12
        private MemoryStream MsIncomeExpenditureStatement(Guid id, DateTime fromdate, DateTime todate)
        {
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Guid SocietyID = SocietySubscription.SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<AcFinalReport> AcFinalReportList;
            PdfPTable HeadingTable = new PdfPTable(6);
            PdfPTable IncomeTable, ExpenditureTable;
            decimal CYDrTotal = 0, LYDrTotal = 0, CYCrTotal = 0, LYCrTotal = 0, CYDrGTotal = 0, LYDrGTotal = 0, CYCrGTotal = 0, LYCrGTotal = 0;
            int DrIndex = 1, CrIndex = 1, DrCount = 1, CrCount = 1;
            string Category = "", CrCategory = "", DrCategory = "";
            try
            {
                //
                Table = new PdfPTable(6);
                IncomeTable = new PdfPTable(6);
                ExpenditureTable = new PdfPTable(6);
                //set width to tables    
                float[] widths = new float[] { 20f, 200f, 60f, 60f, 60f, 60f };
                Table.SetWidthPercentage(widths, rect);
                IncomeTable.SetWidthPercentage(widths, rect);
                ExpenditureTable.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                //                contain = "INCOME & EXPENDITURE STATEMENT from " + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionStart) + " to " + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
                contain = "INCOME & EXPENDITURE STATEMENT from " + String.Format("{0:dd-MMM-yyyy}", fromdate) + " to " + String.Format("{0:dd-MMM-yyyy}", todate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 6;
                HeadingTable.AddCell(cell);
                string[] tdList;
                string CurrentAcYear = "";
                string LastAcYear = "";
                if (SocietySubscription.SubscriptionStart.Month < 4)
                {
                    CurrentAcYear = (SocietySubscription.SubscriptionStart.Year - 1) + " - " + (SocietySubscription.SubscriptionStart.Year);
                    LastAcYear = ((SocietySubscription.SubscriptionStart.Year - 1) - 1) + " - " + ((SocietySubscription.SubscriptionStart.Year) - 1);
                }
                else
                {
                    CurrentAcYear = (SocietySubscription.SubscriptionStart.Year) + " - " + (SocietySubscription.SubscriptionStart.Year + 1);
                    LastAcYear = ((SocietySubscription.SubscriptionStart.Year) - 1) + " - " + ((SocietySubscription.SubscriptionStart.Year + 1) - 1);
                }
                string[] thList = new string[] { "I N C O M E", CurrentAcYear, LastAcYear };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 2;
                    HeadingTable.AddCell(cell);
                }
                for (int i = 0; i < 6; i++)
                {
                    cell = new PdfPCell(new Paragraph((" "), FontV8Bold));
                    cell.BorderWidthLeft = (i != 0 || i != 5) ? 0.025f : 0f;
                    cell.BorderWidthRight = (i != 0 || i != 5) ? 0.025f : 0f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    ExpenditureTable.AddCell(cell);
                }
                for (int i = 0; i < 6; i++)
                {
                    cell = new PdfPCell(new Paragraph((i == 1 ? "E X P E N D I T U R E" : ""), FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    ExpenditureTable.AddCell(cell);
                }
                //                AcFinalReportList = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetIncomeExpenditureReport(id);
                AcFinalReportList = new CloudSociety.Services.SocietyService(this.ModelState).GetIncomeExpenditureReportForPeriod(SocietyID, fromdate, todate);
                if (AcFinalReportList != null)
                {
                    foreach (var BalanceSheet in AcFinalReportList)
                    {
                        if (BalanceSheet.DrCr == "C")
                        {
                            BalanceSheet.amt = -(BalanceSheet.amt ?? 0);
                            BalanceSheet.lyamt = -(BalanceSheet.lyamt ?? 0);
                        }
                        thList = new string[] { "", BalanceSheet.SubCategory, (BalanceSheet.amt ?? 0) != 0 ? (BalanceSheet.amt ?? 0) + "" : "", "", (BalanceSheet.lyamt ?? 0) != 0 ? (BalanceSheet.lyamt ?? 0) + "" : "", "" };
                        // 
                        if (Category != BalanceSheet.Category)
                        {
                            if (BalanceSheet.DrCr == "C")
                            {
                                if (CrCategory != "")
                                {
                                    tdList = new string[] { " ", " ", " ", CYCrTotal != 0 ? CYCrTotal + " " : "", " ", LYCrTotal != 0 ? LYCrTotal + " " : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        IncomeTable.AddCell(cell);
                                    }
                                }
                            }
                            else
                            {
                                if (DrCategory != "")
                                {
                                    tdList = new string[] { " ", " ", " ", CYDrTotal != 0 ? CYDrTotal + " " : "", " ", LYDrTotal != 0 ? LYDrTotal + " " : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        ExpenditureTable.AddCell(cell);
                                    }
                                }
                            }
                            if (BalanceSheet.DrCr == "C")
                            {
                                tdList = new string[] { CrIndex + ".", BalanceSheet.Category, "", "", "", "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    IncomeTable.AddCell(cell);
                                }
                                CrIndex++;
                                Category = BalanceSheet.Category;
                                CrCategory = Category;
                                CYCrTotal = 0; LYCrTotal = 0;
                            }
                            else
                            {
                                tdList = new string[] { DrIndex + ".", BalanceSheet.Category, "", "", "", "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    ExpenditureTable.AddCell(cell);
                                }
                                DrIndex++;
                                Category = BalanceSheet.Category;
                                DrCategory = Category;
                                CYDrTotal = 0; LYDrTotal = 0;
                            }
                        }
                        //
                        if (BalanceSheet.DrCr == "C")
                        {
                            CYCrGTotal += BalanceSheet.amt ?? 0;
                            LYCrGTotal += BalanceSheet.lyamt ?? 0;
                            CYCrTotal += BalanceSheet.amt ?? 0;
                            LYCrTotal += BalanceSheet.lyamt ?? 0;
                            CrCount++;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], (i == 3 || i == 5) ? FontV8Bold : FontV8));
                                cell.HorizontalAlignment = i < 2 ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                                IncomeTable.AddCell(cell);
                            }
                        }
                        else
                        {
                            CYDrGTotal += BalanceSheet.amt ?? 0;
                            LYDrGTotal += BalanceSheet.lyamt ?? 0;
                            CYDrTotal += BalanceSheet.amt ?? 0;
                            LYDrTotal += BalanceSheet.lyamt ?? 0;
                            DrCount++;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], (i == 3 || i == 5) ? FontV8Bold : FontV8));
                                cell.HorizontalAlignment = i < 2 ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                                ExpenditureTable.AddCell(cell);
                            }
                        }
                    }
                    tdList = new string[] { " ", " ", " ", CYCrTotal != 0 ? CYCrTotal + " " : "", " ", LYCrTotal != 0 ? LYCrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        IncomeTable.AddCell(cell);
                    }
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(" ", FontV8));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        IncomeTable.AddCell(cell);
                    }
                    //                    tdList = new string[] { "", "Total Income ", " ", CYCrGTotal != 0 ? CYCrGTotal + "" : "", "", LYCrGTotal != 0 ? LYCrGTotal + "" : "" };
                    tdList = new string[] { "", "TOTAL INCOME ", " ", CYCrGTotal != 0 ? CYCrGTotal + "" : "", "", LYCrGTotal != 0 ? LYCrGTotal + "" : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        //                        cell = new PdfPCell(new Paragraph(tdList[i], FontV9Bold));
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = i != 0 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                        IncomeTable.AddCell(cell);
                    }
                    //for (int i = 0; i < tdList.Length; i++)
                    //{
                    //    cell = new PdfPCell(new Paragraph(" ", FontV8));
                    //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    IncomeTable.AddCell(cell);
                    //}
                    tdList = new string[] { " ", " ", " ", CYDrTotal != 0 ? CYDrTotal + " " : "", " ", LYDrTotal != 0 ? LYDrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ExpenditureTable.AddCell(cell);
                    }
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(" ", FontV8));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ExpenditureTable.AddCell(cell);
                    }
                    //                    tdList = new string[] { "", "Total Expenditure ", " ", CYDrGTotal != 0 ? CYDrGTotal + "" : "", "", LYDrGTotal != 0 ? LYDrGTotal + "" : "" };
                    tdList = new string[] { "", "TOTAL EXPENDITURE ", " ", CYDrGTotal != 0 ? CYDrGTotal + "" : "", "", LYDrGTotal != 0 ? LYDrGTotal + "" : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        //                        cell = new PdfPCell(new Paragraph(tdList[i], FontV9Bold));
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = i != 0 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                        ExpenditureTable.AddCell(cell);
                    }
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(" ", FontV8));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ExpenditureTable.AddCell(cell);
                    }

                    tdList = new string[] { "", "Excess of Income Over Expenditure ", "", "", "", "" };
                    tdList[3] = (CYCrGTotal - CYDrGTotal) + "";
                    tdList[3] = tdList[3] != "0" ? tdList[3] : "";
                    tdList[5] = (LYCrGTotal - LYDrGTotal) + "";
                    tdList[5] = tdList[5] != "0" ? tdList[5] : "";
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        //                        cell = new PdfPCell(new Paragraph(tdList[i], i == 1 ? FontFactory.GetFont("Verdana", 9, Font.BOLDITALIC) : FontV9Bold));
                        cell = new PdfPCell(new Paragraph(tdList[i], i == 1 ? FontFactory.GetFont("Verdana", 8, Font.BOLDITALIC) : FontV8Bold));
                        cell.HorizontalAlignment = i != 0 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                        ExpenditureTable.AddCell(cell);
                    }
                }
                cell = new PdfPCell(IncomeTable);
                cell.Colspan = 6;
                Table.AddCell(cell);
                cell = new PdfPCell(ExpenditureTable);
                cell.Colspan = 6;
                Table.AddCell(cell);
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this._service.PdfMsCreator(TableList, Society, HeadingTable, 8F);
        }

        //Returns MemoryStream for Schedule To Income Expenditure Statement changed by Baji on 29/8/12
        //        private MemoryStream MsScheduleToIncomeExpenditureStatement(Guid id)
        private MemoryStream MsScheduleToIncomeExpenditureStatement(Guid id, DateTime fromdate, DateTime todate)
        {
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Guid SocietyID = SocietySubscription.SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<AcFinalReportSchedule> AcFinalReportScheduleList;
            PdfPTable HeadingTable = new PdfPTable(3);
            PdfPTable ExpenditureTable, IncomeTable;
            decimal DrTotal = 0, CrTotal = 0;
            int DrIndex = 1, CrIndex = 1;//, DrCount = 1, CrCount = 1;
            string SubCategory = "", CrSubCategory = "", DrSubCategory = "";
            try
            {
                //
                Table = new PdfPTable(3);
                ExpenditureTable = new PdfPTable(3);
                IncomeTable = new PdfPTable(3);
                //set width to tables    
                float[] widths = new float[] { 20f, 250f, 80f };
                Table.SetWidthPercentage(widths, rect);
                ExpenditureTable.SetWidthPercentage(widths, rect);
                IncomeTable.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "SCHEDULE TO INCOME & EXPENDITURE STATEMENT from " + String.Format("{0:dd-MMM-yyyy}", fromdate) + " to " + String.Format("{0:dd-MMM-yyyy}", todate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 3;
                HeadingTable.AddCell(cell);
                string[] tdList;
                string[] thList = new string[] { "", "I N C O M E", "" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                for (int i = 0; i < 3; i++)
                {
                    cell = new PdfPCell(new Paragraph((" "), FontV8Bold));
                    cell.BorderWidthLeft = i == 1 ? 0.025f : 0f;
                    cell.BorderWidthRight = i == 1 ? 0.025f : 0f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    ExpenditureTable.AddCell(cell);
                }
                for (int i = 0; i < 3; i++)
                {
                    cell = new PdfPCell(new Paragraph((i == 1 ? "E X P E N D I T U R E" : ""), FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    ExpenditureTable.AddCell(cell);
                }

                //                AcFinalReportScheduleList = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetIncomeExpenditureSchedule(id);
                AcFinalReportScheduleList = new CloudSociety.Services.SocietyService(this.ModelState).GetIncomeExpenditureScheduleForPeriod(SocietyID, fromdate, todate);
                if (AcFinalReportScheduleList != null)
                {
                    foreach (var BalanceSheet in AcFinalReportScheduleList)
                    {

                        if (BalanceSheet.DrCr == "C")
                        {
                            BalanceSheet.amt = -(BalanceSheet.amt ?? 0);
                        }
                        thList = new string[] { "", BalanceSheet.AcHead, (BalanceSheet.amt ?? 0) != 0 ? (BalanceSheet.amt ?? 0) + "" : "" };
                        //Logic For SubCategory   
                        if (SubCategory != BalanceSheet.SubCategory)
                        {
                            if (BalanceSheet.DrCr == "D")
                            {
                                if (DrSubCategory != "")
                                {
                                    tdList = new string[] { "", "Total ", DrTotal != 0 ? DrTotal + " " : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        ExpenditureTable.AddCell(cell);
                                    }
                                }
                            }
                            else
                            {
                                if (CrSubCategory != "")
                                {
                                    tdList = new string[] { "", "Total ", CrTotal != 0 ? CrTotal + " " : "" };
                                    for (int i = 0; i < tdList.Length; i++)
                                    {
                                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        IncomeTable.AddCell(cell);
                                    }
                                }
                            }
                            if (BalanceSheet.DrCr == "D")
                            {
                                tdList = new string[] { DrIndex + ".", BalanceSheet.SubCategory, "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    ExpenditureTable.AddCell(cell);
                                }
                                DrIndex++;
                                SubCategory = BalanceSheet.SubCategory;
                                DrSubCategory = SubCategory;
                                DrTotal = 0; DrTotal = 0;
                            }
                            else
                            {
                                tdList = new string[] { CrIndex + ".", BalanceSheet.SubCategory, "" };
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                                    IncomeTable.AddCell(cell);
                                }
                                CrIndex++;
                                SubCategory = BalanceSheet.SubCategory;
                                CrSubCategory = SubCategory;
                                CrTotal = 0; CrTotal = 0;
                            }
                        }
                        //
                        if (BalanceSheet.DrCr == "D")
                        {
                            DrTotal += BalanceSheet.amt ?? 0;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                                cell.HorizontalAlignment = i == 2 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                                ExpenditureTable.AddCell(cell);
                            }
                        }
                        else
                        {
                            CrTotal += BalanceSheet.amt ?? 0;
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                                cell.HorizontalAlignment = i == 2 ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                                IncomeTable.AddCell(cell);
                            }
                        }
                    }

                    tdList = new string[] { "", "Total ", DrTotal != 0 ? DrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ExpenditureTable.AddCell(cell);
                    }
                    tdList = new string[] { "", "Total ", CrTotal != 0 ? CrTotal + " " : "" };
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(tdList[i], FontV8Bold));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        IncomeTable.AddCell(cell);
                    }
                }
                cell = new PdfPCell(IncomeTable);
                cell.Colspan = 3;
                Table.AddCell(cell);
                cell = new PdfPCell(ExpenditureTable);
                cell.Colspan = 3;
                Table.AddCell(cell);
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this._service.PdfMsCreator(TableList, Society, HeadingTable, 8F);
        }
        //Retuns MemoryStream for Trial Balance added by Ranjit  
        private MemoryStream MsTrialBalanceReport(Guid id, IEnumerable<AcBalance> AcBalanceList, DateTime FromDate, DateTime ToDate)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadingTable = new PdfPTable(7);
            float[] widths = new float[] { 6f, 2f, 2f, 2f, 2f, 2f, 2f };
            HeadingTable.SetWidths(widths);
            string[] thList = new string[] { "Account", "Opening", "Period", "Balance" };
            decimal[] tdList;
            decimal ClosingBal;
            decimal[] TotalList;
            contain = "Trial Balance For The Period From ";
            contain += String.Format("{0:dd-MMM-yyyy}", FromDate);
            contain += " To " + String.Format("{0:dd-MMM-yyyy}", ToDate);
            cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
            cell.Colspan = 7;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            HeadingTable.AddCell(cell);
            for (int i = 0; i < thList.Length; i++)
            {
                cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                if (i == 0)
                {
                    cell.BorderWidthBottom = 0;
                    cell.Colspan = 0;
                }
                else
                    cell.Colspan = 2;
                HeadingTable.AddCell(cell);
            }
            cell = new PdfPCell(new Phrase(new Chunk("", FontV7Bold)));
            cell.BorderWidthTop = 0;
            HeadingTable.AddCell(cell);
            for (int i = 0; i < 3; i++)
            {
                cell = new PdfPCell(new Phrase(new Chunk("Dr", FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Cr", FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                HeadingTable.AddCell(cell);
            }
            Table = new PdfPTable(7);
            Table.SetWidths(widths);
            try
            {
                TotalList = new decimal[] { 0, 0, 0, 0, 0, 0 };
                foreach (var AcBalance in AcBalanceList)
                {
                    ClosingBal = (AcBalance.opbal ?? 0) + (AcBalance.dramt ?? 0) - (AcBalance.cramt ?? 0);
                    //tdList = new decimal[] { AcBalance.opbal >= 0 ? (AcBalance.opbal ?? 0) : 0, AcBalance.opbal < 0 ? Math.Abs(AcBalance.opbal ?? 0) : 0, AcBalance.dramt ?? 0, AcBalance.cramt ?? 0, ClosingBal >= 0 ? ClosingBal : 0, ClosingBal < 0 ? Math.Abs(ClosingBal) : 0 };                    
                    tdList = new decimal[] { 0, 0, 0, 0, 0, 0 };
                    tdList[0] = AcBalance.opbal != null ? (AcBalance.opbal >= 0 ? AcBalance.opbal ?? 0 : 0) : 0;
                    tdList[1] = AcBalance.opbal != null ? (AcBalance.opbal < 0 ? Math.Abs(AcBalance.opbal ?? 0) : 0) : 0;
                    tdList[2] = AcBalance.dramt ?? 0;
                    tdList[3] = AcBalance.cramt ?? 0;
                    tdList[4] = ClosingBal >= 0 ? ClosingBal : 0;
                    tdList[5] = ClosingBal < 0 ? Math.Abs(ClosingBal) : 0;
                    cell = new PdfPCell(new Phrase(new Chunk(AcBalance.AcHead, FontV7)));
                    Table.AddCell(cell);
                    for (int i = 0; i < tdList.Length; i++)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk((tdList[i] == 0 ? "-" : tdList[i] + ""), FontV7)));
                        //cell = new PdfPCell(new Phrase(new Chunk(tdList[i] + "", FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        TotalList[i] += tdList[i];
                    }
                }
                cell = new PdfPCell(new Phrase(new Chunk("Total : ", FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);
                for (int i = 0; i < TotalList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(TotalList[i] + "", FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            //return this.FileStreamResult(TableList, Society, "TrialBalanceReport.pdf", HeadingTable);
            return _service.PdfMsCreator(TableList, Society, HeadingTable, 8F);
        }
        //Retuns MemoryStream for General Ledger added by Ranjit  
        private MemoryStream MsGeneralLedger(Guid id, IEnumerable<AcLedger> AcLedgerList, DateTime FromDate, DateTime ToDate)
        {
            SocietySubscription SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Society Society = new SocietyService(this.ModelState).GetById(SocietySubscription.SocietyID);
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            PdfPTable TempTable;
            document.Open();
            decimal TotalDrAmountTd = 0, TotalCrAmountTd = 0, BalanceAmount = 0;
            string[] thList;
            string[] tdList;
            int index = 0;
            string AcHead = "";
            try
            {
                if (AcLedgerList.Count() > 0)
                {
                    foreach (var AcLedger in AcLedgerList)
                    {
                        Table = new PdfPTable(1);
                        Table.SetWidthPercentage(new float[] { 600f }, rect);
                        TempTable = new PdfPTable(8);
                        TempTable.SetWidthPercentage(new float[] { 80f, 50f, 250f, 50f, 50f, 65f, 75f, 80f }, rect);
                        index += 1;
                        if (AcHead != AcLedger.AcHead)
                        {
                            index = 0;
                            TotalDrAmountTd = 0;
                            TotalCrAmountTd = 0;
                            BalanceAmount = 0;
                            document.NewPage();
                            Table.AddCell(_service.SocietyHeaderTable(Society));
                            contain = "General Ledger For The Period From ";
                            contain += String.Format("{0:dd-MMM-yyyy}", FromDate);
                            contain += " To " + String.Format("{0:dd-MMM-yyyy}", ToDate);
                            cell = new PdfPCell(new Paragraph(contain.ToUpper(), FontV9Bold));
                            cell.PaddingBottom = 5f;
                            cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            Table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph("Account Head : " + AcLedger.AcHead, FontV9Bold));
                            cell.BorderWidthBottom = 0;
                            cell.BorderWidthTop = 0;
                            cell.PaddingBottom = 15f;
                            cell.PaddingTop = 15f;
                            Table.AddCell(cell);
                            thList = new string[] { "Document No", "Doc Date", "Particulars", "Chq No", "Chq Date", "Dr Amount", "Cr Amount", "Balance Amount" };
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8Bold)));
                                cell.HorizontalAlignment = (i == 5 || i == 6 || i == 7) ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                                TempTable.AddCell(cell);
                            }
                        }
                        string Balance = "";
                        decimal DrAmount = AcLedger.DrAmt ?? 0;
                        TotalDrAmountTd += DrAmount;
                        decimal CrAmount = AcLedger.CrAmt ?? 0;
                        TotalCrAmountTd += CrAmount;
                        BalanceAmount += (DrAmount - CrAmount);
                        if (BalanceAmount > 0)
                            Balance = Math.Abs(BalanceAmount) + " Dr";
                        else if (BalanceAmount < 0)
                            Balance = Math.Abs(BalanceAmount) + " Cr";
                        else
                            Balance = 0 + "";
                        tdList = new string[] { AcLedger.DocNo, (AcLedger.DocDate != null ? String.Format("{0:dd-MMM-yy}", AcLedger.DocDate) : ""), AcLedger.Particulars, AcLedger.ChequeNo ?? "", (AcLedger.ChequeDate != null ? String.Format("{0:dd-MMM-yy}", AcLedger.ChequeDate) : ""), (AcLedger.DrAmt == 0 ? "-" : AcLedger.DrAmt.ToString()), (AcLedger.CrAmt == 0 ? "-" : AcLedger.CrAmt.ToString()), Balance };
                        for (int i = 0; i < tdList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(tdList[i], FontV7)));
                            cell.HorizontalAlignment = (i == 5 || i == 6 || i == 7) ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                            TempTable.AddCell(cell);
                        }
                        AcHead = AcLedger.AcHead;
                        IEnumerable<AcLedger> AccountList = AcLedgerList.Where(acl => acl.AcHead == AcHead);
                        if (index == (AccountList.Count() - 1))
                        {
                            cell = new PdfPCell(new Phrase(new Chunk("Total :", FontV7Bold)));
                            cell.Colspan = 5;
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(TotalDrAmountTd.ToString(), FontV7Bold)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(TotalCrAmountTd.ToString(), FontV7Bold)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(""));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);

                            //for footer
                            PdfPTable FooterTable = new PdfPTable(2);
                            FooterTable.HorizontalAlignment = Element.ALIGN_CENTER;
                            FooterTable.TotalWidth = document.PageSize.Width;
                            cell = new PdfPCell(new Paragraph("Page " + writer.PageNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                            cell.Border = 0;
                            cell.PaddingLeft = 0;
                            cell.PaddingRight = 0;
                            FooterTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph("Printed On " + String.Format("{0:dd-MMM-yyyy, HH:mm:ss}", DateTime.Now), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                            cell.Border = 0;
                            cell.PaddingLeft = 90;
                            FooterTable.AddCell(cell);
                            FooterTable.WriteSelectedRows(0, -1, 50, (document.BottomMargin + 9), writer.DirectContent);
                            //                             
                        }
                        cell = new PdfPCell(TempTable);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);

                        document.Add(Table);
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
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            finally
            {
                writer.CloseStream = false;
                document.Close();
                ms.Position = 0;
            }
            // return this.FileStreamResult(ms, "GeneralLedger.pdf");
            return ms;
        }
        //Retuns MemoryStream for Bank Reconciliation added by Ranjit  
        private MemoryStream MsBankReconciliationReport(Guid id, Guid AcHeadID, DateTime AsOnDate)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<AcTransactionAc> AcTransactionAcList = new AcTransactionAcService(this.ModelState).ListUnReconciledAsOnDateBySocietyIDAcHeadID(SocietyID, AcHeadID, AsOnDate);
            var AcHeadService = new AcHeadService(this.ModelState);
            decimal? OpBalAsPerBooks = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietyID, AcHeadID, AsOnDate, 'B');
            decimal? OpBalAsPerBank = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietyID, AcHeadID, AsOnDate, 'R');
            decimal? ClBalAsPerBooks = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietyID, AcHeadID, AsOnDate, 'B');
            decimal? ClBalAsPerBank = AcHeadService.GetBalanceAsOnBySocietyIDAcHeadID(SocietyID, AcHeadID, AsOnDate, 'R');
            decimal DrTotal = 0, CrTotal = 0;
            PdfPTable HeadTable = new PdfPTable(6);
            try
            {
                string[] thList;
                float[] widths = new float[] { 75f, 55f, 150f, 150f, 60f, 60f };
                HeadTable.SetWidthPercentage(widths, rect);
                contain = "BANK RECONCILIATION REPORT AS ON " + String.Format("{0:dd-MMM-yyyy}", AsOnDate);
                cell = new PdfPCell(_service.CaptionTable(contain.ToUpper(), FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 6;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Bank A/C :", FontV8)));
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Bank - 1 A/c", FontV8)));
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Opening Balance As Per Books :", FontV8)));
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk((OpBalAsPerBooks < 0 ? Math.Abs(OpBalAsPerBooks ?? 0) + " Cr" : (OpBalAsPerBooks > 0 ? Math.Abs(OpBalAsPerBooks ?? 0) + " Dr" : " 0")), FontV8)));
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                contain = "Opening Balance As Per Bank :";
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontV8)));
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk((OpBalAsPerBank < 0 ? Math.Abs(OpBalAsPerBank ?? 0) + " Cr" : (OpBalAsPerBank > 0 ? Math.Abs(OpBalAsPerBank ?? 0) + " Dr" : " 0")), FontV8)));
                cell.Colspan = 3;
                HeadTable.AddCell(cell);

                thList = new string[] { "\n" + "Doc No.", "\n" + "Doc Date", "\n" + "Account Head", "\n" + "Particular", "Cheques Issued But Not Presented", "Cheques Deposited But Not Cleared" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(thList[i], ((i == 4 || i == 5) ? FontV7Bold : FontV8Bold))));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    HeadTable.AddCell(cell);
                }


                Table = new PdfPTable(6);
                Table.SetWidthPercentage(widths, rect);
                foreach (var AcTransactionAc in AcTransactionAcList)
                {
                    thList = new string[] { AcTransactionAc.AcTransaction.DocNo, String.Format("{0:dd-MMM-yyyy}", AcTransactionAc.AcTransaction.DocDate), AcTransactionAc.AcHead.Name, AcTransactionAc.Particulars, (AcTransactionAc.DrCr == "C" ? AcTransactionAc.Amount + "" : "-"), (AcTransactionAc.DrCr == "D" ? AcTransactionAc.Amount + "" : "-") };
                    for (int i = 0; i < thList.Length; i++)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV7)));
                        cell.HorizontalAlignment = (i == 4 || i == 5) ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                        Table.AddCell(cell);
                    }
                    DrTotal += (AcTransactionAc.DrCr == "C" ? AcTransactionAc.Amount : 0);
                    CrTotal += (AcTransactionAc.DrCr == "D" ? AcTransactionAc.Amount : 0);
                }
                cell = new PdfPCell(new Phrase(new Chunk("Total :", FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Colspan = 4;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(DrTotal + "", FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk(CrTotal + "", FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);


                cell = new PdfPCell(new Phrase(new Chunk("Closing Balance As Per Books :", FontV8)));
                cell.Colspan = 3;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk((ClBalAsPerBooks < 0 ? Math.Abs(ClBalAsPerBooks ?? 0) + " Cr" : (ClBalAsPerBooks > 0 ? Math.Abs(ClBalAsPerBooks ?? 0) + " Dr" : " 0")), FontV8)));
                cell.Colspan = 3;
                Table.AddCell(cell);
                contain = "Opening Balance As Per Bank :";
                cell = new PdfPCell(new Phrase(new Chunk(contain, FontV8)));
                cell.Colspan = 3;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk((ClBalAsPerBank < 0 ? Math.Abs(ClBalAsPerBank ?? 0) + " Cr" : (ClBalAsPerBank > 0 ? Math.Abs(ClBalAsPerBank ?? 0) + " Dr" : " 0")), FontV8)));
                cell.Colspan = 3;
                Table.AddCell(cell);

                TableList.Add(Table);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            //return this.FileStreamResult(TableList, Society, "BankReconciliation.pdf", PdfPHeadingTable: HeadTable, SpacingAfterTable: 0);
            return _service.PdfMsCreator(TableList, Society, HeadTable);
        }
        //Retuns MemoryStream for Receipt And Payment fro given period added by Nityananda
        private MemoryStream MsReceiptAndPaymentStatement(Guid id, DateTime fromdate, DateTime todate)
        {
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Guid SocietyID = SocietySubscription.SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            //IEnumerable<AcFinalReport> AcFinalReportList;
            IEnumerable<AcClBalance> acClBalance;
            IEnumerable<AcClBalance> acOpBalance;
            IEnumerable<AcClBalance> acBalanceCr;
            IEnumerable<AcClBalance> acBalanceDr;
            PdfPTable HeadingTable = new PdfPTable(6);
            PdfPTable ReceiptTable;
            PdfPTable PaymentTable;
            PdfPTable ReceiptPaymentTable;
            decimal OpBalTotal = 0, ReceiptTotal = 0, PaymentTotal = 0, ClBalTotal = 0;     // , ReceiptGTotal = 0, PaymentGTotal = 0
            try
            {
                Table = new PdfPTable(6);
                ReceiptTable = new PdfPTable(3);
                PaymentTable = new PdfPTable(3);
                ReceiptPaymentTable = new PdfPTable(6);
                float[] widths = new float[] { 160f, 60f, 60f, 160f, 60f, 60f };
                float[] width = new float[] { 160f, 60f, 60f };
                Table.SetWidthPercentage(widths, rect);
                ReceiptTable.SetWidthPercentage(width, rect);
                PaymentTable.SetWidthPercentage(width, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                ReceiptPaymentTable.SetWidthPercentage(widths, rect);
                contain = "RECEIPT & PAYMENT STATEMENT from " + String.Format("{0:dd-MMM-yyyy}", fromdate) + " to " + String.Format("{0:dd-MMM-yyyy}", todate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 6;
                HeadingTable.AddCell(cell);
                string[] thList = new string[] { "RECEIPT", "PAYMENT" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderWidthLeft = (i != 0 || i != 5) ? 0.025f : 0f;
                    cell.BorderWidthRight = (i != 0 || i != 5) ? 0.025f : 0f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    cell.Colspan = 3;
                    HeadingTable.AddCell(cell);
                }
                acOpBalance = new AcHeadService(this.ModelState).ListBalanceBySocietyIDNatureAsOn(SocietyID, fromdate.AddDays(-1), "C,B");
                acClBalance = new AcHeadService(this.ModelState).ListBalanceBySocietyIDNatureAsOn(SocietyID, todate, "C,B");    // .AddDays(-1)
                acBalanceCr = new AcHeadService(this.ModelState).ListCashBankOppBalanceBySocietyIDDrCr(SocietyID, fromdate, todate, "Cr");
                acBalanceDr = new AcHeadService(this.ModelState).ListCashBankOppBalanceBySocietyIDDrCr(SocietyID, fromdate, todate, "Dr");

                thList = new string[] { "OPENING BALANCE", "", "" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    ReceiptTable.AddCell(cell);
                }
                foreach (var opb in acOpBalance)
                {
                    OpBalTotal += (decimal)opb.bal;
                    thList = new string[] { opb.AcHead, opb.bal.ToString(), "" };
                    for (int i = 0; i < thList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                        if (i == 0)
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        else
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ReceiptTable.AddCell(cell);
                    }
                }
                thList = new string[] { "", "Total", OpBalTotal.ToString() };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    ReceiptTable.AddCell(cell);
                }

                thList = new string[] { "RECEIPTS", "", "" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    ReceiptTable.AddCell(cell);
                }
                foreach (var rec in acBalanceCr)
                {
                    ReceiptTotal += (decimal)rec.bal;
                    thList = new string[] { rec.AcHead, rec.bal.ToString(), "" };
                    for (int i = 0; i < thList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                        if (i == 0)
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        else
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ReceiptTable.AddCell(cell);
                    }
                }

                thList = new string[] { "Total", "", ReceiptTotal.ToString() };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    ReceiptTable.AddCell(cell);
                }

                thList = new string[] { "PAYMENTS", "", "" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    PaymentTable.AddCell(cell);
                }
                foreach (var pay in acBalanceDr)
                {
                    PaymentTotal += (decimal)pay.bal;
                    thList = new string[] { pay.AcHead, pay.bal.ToString(), "" };
                    for (int i = 0; i < thList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                        if (i == 0)
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        else
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        PaymentTable.AddCell(cell);
                    }
                }

                thList = new string[] { "Total ", "", PaymentTotal.ToString() };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PaymentTable.AddCell(cell);
                }

                thList = new string[] { "CLOSING BALANCE", "", "" };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    PaymentTable.AddCell(cell);
                }
                foreach (var clb in acClBalance)
                {
                    ClBalTotal += (decimal)clb.bal;
                    thList = new string[] { clb.AcHead, clb.bal.ToString(), "" };
                    for (int i = 0; i < thList.Length; i++)
                    {
                        cell = new PdfPCell(new Paragraph(thList[i], FontV8));
                        if (i == 0)
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        else
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        PaymentTable.AddCell(cell);
                    }
                }
                thList = new string[] { "Total ", "", ClBalTotal.ToString() };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PaymentTable.AddCell(cell);
                }

                cell = new PdfPCell(ReceiptTable);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 3;
                ReceiptPaymentTable.AddCell(cell);

                cell = new PdfPCell(PaymentTable);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 3;
                ReceiptPaymentTable.AddCell(cell);

                thList = new string[] { "", "", (OpBalTotal + ReceiptTotal).ToString(), "", "", (ClBalTotal + PaymentTotal).ToString() };
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    ReceiptPaymentTable.AddCell(cell);
                }

                cell = new PdfPCell(ReceiptPaymentTable);
                cell.Colspan = 6;
                Table.AddCell(cell);
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this._service.PdfMsCreator(TableList, Society, HeadingTable, 8F);
        }

        [HttpGet]
        public ActionResult BankReconciliationReport(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = societySubscriptionService.GetById(id);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(id);
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(SocietySubscription.SocietyID, "B");
            return View();
        }
        [HttpPost]
        public ActionResult BankReconciliationReport(Guid id, Guid AcHeadID, DateTime AsOnDate)
        {
            MemoryStream ms = MsBankReconciliationReport(id, AcHeadID, AsOnDate);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=BankReconciliation.pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult TrialBalanceReport(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View();
        }
        [HttpPost]
        public ActionResult TrialBalanceReport(Guid id, DateTime FromDate, DateTime ToDate) // id = SocietySubscriptionID
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            IEnumerable<AcBalance> AcBalanceList = new AcHeadService(this.ModelState).ListBalanceBySocietyID(SocietyID, FromDate, ToDate);
            MemoryStream ms = MsTrialBalanceReport(id, AcBalanceList, FromDate, ToDate);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=TrialBalanceReport.pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult GeneralLedgerReport(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            return View();
        }
        [HttpPost]
        public ActionResult GeneralLedgerReport(Guid id, DateTime FromDate, DateTime ToDate) // id = SocietySubscriptionID
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            IEnumerable<AcLedger> AcLedgerList = new AcHeadService(this.ModelState).ListLedgerBySocietyIDAcHeadIds(SocietyID, String.Empty, FromDate, ToDate).OrderBy(a => a.Sequence);
            if (AcLedgerList != null)
            {
                MemoryStream ms = MsGeneralLedger(id, AcLedgerList, FromDate, ToDate);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=GeneralLedger.pdf");
                Response.Buffer = true;
                Response.Clear();
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                Response.OutputStream.Flush();
                Response.End();
                return new FileStreamResult(Response.OutputStream, "application/pdf");
            }
            else
            {
                ViewBag.SocietySubscriptionID = id;
                ViewBag.NoRecords = true;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.ShowSocietyMenu = true;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
                ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
                ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
                ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                return View();
            }
        }

        [HttpGet]
        public ActionResult IncomeExpenditureStatement(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societysubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societysubscription.SubscriptionStart;
            ViewBag.ToDate = societysubscription.PaidTillDate;
            return View();
        }
        [HttpPost]
        public ActionResult IncomeExpenditureStatement(Guid id, DateTime FromDate, DateTime ToDate) // id = SocietySubscriptionID   // , Boolean ShowSchedule
        {
            String FileName = "IncomeExpenditureStatementFrom" + String.Format("{0:dd-MMM-yyyy}", FromDate) + "To" + String.Format("{0:dd-MMM-yyyy}", ToDate);
            MemoryStream ms = this.MsIncomeExpenditureStatement(id, FromDate, ToDate);

            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult IncomeExpenditureSchedule(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societysubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societysubscription.SubscriptionStart;
            ViewBag.ToDate = societysubscription.PaidTillDate;
            return View();
        }
        [HttpPost]
        public ActionResult IncomeExpenditureSchedule(Guid id, DateTime FromDate, DateTime ToDate) // id = SocietySubscriptionID
        {
            String FileName = "IncomeExpenditureScheduleFrom" + String.Format("{0:dd-MMM-yyyy}", FromDate) + "To" + String.Format("{0:dd-MMM-yyyy}", ToDate);
            MemoryStream ms = this.MsScheduleToIncomeExpenditureStatement(id, FromDate, ToDate);

            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult BalanceSheet(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societysubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societysubscription.SubscriptionStart;
            ViewBag.ToDate = societysubscription.PaidTillDate;
            return View();
        }
        [HttpPost]
        public ActionResult BalanceSheet(Guid id, DateTime AsOnDate) // id = SocietySubscriptionID
        {
            String FileName = "BalanceSheetAsOn" + String.Format("{0:dd-MMM-yyyy}", AsOnDate);
            MemoryStream ms = this.MsBalanceSheet(id, AsOnDate);

            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult BalanceSheetSchedule(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societysubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societysubscription.SubscriptionStart;
            ViewBag.ToDate = societysubscription.PaidTillDate;
            return View();
        }
        [HttpPost]
        public ActionResult BalanceSheetSchedule(Guid id, DateTime AsOnDate) // id = SocietySubscriptionID
        {
            String FileName = "BalanceSheetAsOn" + String.Format("{0:dd-MMM-yyyy}", AsOnDate);
            MemoryStream ms = this.MsScheduleToBalanceSheet(id, AsOnDate);

            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        [HttpGet]
        public ActionResult TransactionReport(Guid id) // id = SocietySubscriptionID
        {
            IDictionary<string, string> DocumentTypeList = new Dictionary<string, string>() { { "CP", "Cash Payment" }, { "CR", "Cash Receipt" }, { "BP", "Bank Payment" }, { "BR", "Bank Receipt" }, { "PB", "Puchase Bill" }, { "SB", "SocietyBill" }, { "JV", "Journal Voucher" }, { "MC", "Member Collection" }, { "YC", "Year End Closing Entry" } };
            ViewBag.DocumentTypeList = new SelectList(DocumentTypeList, "Key", "Value");
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View();
        }
        [HttpPost]
        public FileStreamResult TransactionReport(Guid id, string DocType, DateTime FromDate, DateTime ToDate)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<AcTransaction> AcTransactionList;
            PdfPTable HeadingTable = new PdfPTable(6);
            decimal totalDrAmount = 0, totalCrAmount = 0;
            try
            {
                string[] thList = new string[] { "Doc No.", "Doc Date", "Account Head", "Particular", "DrAmount", "CrAmount" };
                float[] widths = new float[] { 75f, 55f, 160f, 160f, 50f, 50f };
                Table = new PdfPTable(6);
                Table.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "Transaction REGISTER For The Period From ";
                contain += String.Format("{0:dd-MMM-yyyy}", FromDate);
                contain += " To " + String.Format("{0:dd-MMM-yyyy}", ToDate);
                contain += ". Document Type :" + (DocType == "CP" ? "Cash Payment" : (DocType == "CR" ? "Cash Receipt" : (DocType == "BP" ? "Bank Payment" : (DocType == "BR" ? "Bank Receipt" : (DocType == "PB" ? "Puchase Bill" : (DocType == "EB" ? "Expense Bill" : (DocType == "SB" ? "SocietyBill" : (DocType == "JV" ? "Journal Voucher" : (DocType == "MC" ? "Member Collection" : (DocType == "OP" ? "Opening Balance" : (DocType == "YC" ? "Year End Closing Entry" : "")))))))))));
                cell = new PdfPCell(_service.CaptionTable(contain.ToUpper(), FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 6;
                HeadingTable.AddCell(cell);

                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8Bold)));
                    cell.HorizontalAlignment = (i == 4 || i == 5) ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                AcTransactionList = new AcTransactionService(this.ModelState).ListBySocietyIDDocTypePeriod(SocietyID, DocType, FromDate, ToDate); //ListBySocietySubscriptionIDDocType(id, DocType);
                if (AcTransactionList != null)
                {
                    foreach (var AcTransaction in AcTransactionList)
                    {
                        thList = new string[] { AcTransaction.DocNo, String.Format("{0:dd-MMM-yyyy}", AcTransaction.DocDate), (AcTransaction.AcHead == null ? "" : AcTransaction.AcHead.Name), AcTransaction.Particulars, AcTransaction.DrAmount + "", AcTransaction.CrAmount + "" };
                        for (int i = 0; i < thList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8)));
                            cell.HorizontalAlignment = (i == 4 || i == 5) ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                            //                            HeadingTable.AddCell(cell);
                            Table.AddCell(cell);
                        }
                        totalDrAmount += AcTransaction.DrAmount ?? 0;
                        totalCrAmount += AcTransaction.CrAmount ?? 0;
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Total :", FontV8Bold)));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalDrAmount.ToString(), FontV8Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalCrAmount.ToString(), FontV8Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "TransactionRegister.pdf", HeadingTable);
        }
        [HttpGet]
        public ActionResult FinalReports(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View();
        }

        //method Created By Nityananda 21-Mar-2013
        [HttpGet]
        public ActionResult ReceiptAndPaymentStatementReport(Guid id)// id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.NoRecords = false;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societysubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societysubscription.SubscriptionStart;
            ViewBag.ToDate = societysubscription.PaidTillDate;
            return View();
        }

        [HttpPost]
        public ActionResult ReceiptAndPaymentStatementReport(Guid id, DateTime FromDate, DateTime ToDate) // id = SocietySubscriptionID
        {
            String FileName = "ReceiptAndPaymentStatementfrom" + String.Format("{0:dd-MMM-yyyy}", FromDate) + "to" + String.Format("{0:dd-MMM-yyyy}", ToDate);
            MemoryStream ms = this.MsReceiptAndPaymentStatement(id, FromDate, ToDate);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //        public FileStreamResult DownLoadPDF(Guid id, String ReportName)
        //        {
        //            MemoryStream ms = null;
        //            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
        //            String FileName = "";
        //            if (ReportName == "ScheduleToIncomeExpenditureStatement")
        //            {
        //                FileName = ReportName;
        //                ms = this.MsScheduleToIncomeExpenditureStatement(id, SocietySubscription.SubscriptionStart, SocietySubscription.SubscriptionEnd);
        //            }
        //            if (ReportName == "IncomeExpenditureStatement")
        //            {
        //                FileName = ReportName + "From" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionStart) + "To" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
        ////                ms = this.MsIncomeExpenditureStatement(id);
        //                ms = this.MsIncomeExpenditureStatement(id,SocietySubscription.SubscriptionStart,SocietySubscription.SubscriptionEnd);
        //            }
        //            if (ReportName == "ScheduleToBalanceSheet")
        //            {
        //                FileName = ReportName;
        ////                ms = this.MsScheduleToBalanceSheet(id);
        //                ms = this.MsScheduleToBalanceSheet(id, SocietySubscription.SubscriptionEnd);
        //            }
        //            if (ReportName == "BalanceSheet")
        //            {
        //                FileName = ReportName + "AsOn" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
        //                ms = this.MsBalanceSheet(id, SocietySubscription.SubscriptionEnd);
        //            }
        //            //prepare output stream            
        //            Response.ContentType = "application/pdf";
        //            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
        //            Response.Buffer = true;
        //            Response.Clear();
        //            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        //            Response.OutputStream.Flush();
        //            Response.End();
        //            return new FileStreamResult(Response.OutputStream, "application/pdf");
        //        }
        public FileStreamResult PDFIncomeExpenditure(Guid id)
        {
            MemoryStream ms = null;
            String ReportName = "IncomeExpenditureStatement";
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            String FileName = "";

            FileName = ReportName + "From" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionStart) + "To" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
            ms = this.MsIncomeExpenditureStatement(id, SocietySubscription.SubscriptionStart, SocietySubscription.SubscriptionEnd);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult PDFScheduleToIncomeExpenditure(Guid id)
        {
            MemoryStream ms = null;
            String ReportName = "ScheduleToIncomeExpenditureStatement";
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            String FileName = "";

            FileName = ReportName + "From" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionStart) + "To" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
            ms = this.MsScheduleToIncomeExpenditureStatement(id, SocietySubscription.SubscriptionStart, SocietySubscription.SubscriptionEnd);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult PDFBalanceSheet(Guid id)
        {
            MemoryStream ms = null;
            String ReportName = "BalanceSheet";
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            String FileName = "";

            FileName = ReportName + "AsOn" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
            ms = this.MsBalanceSheet(id, SocietySubscription.SubscriptionEnd);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult PDFScheduleToBalanceSheet(Guid id)
        {
            MemoryStream ms = null;
            String ReportName = "ScheduleToBalanceSheet";
            var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            String FileName = "";

            FileName = ReportName + "AsOn" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
            ms = this.MsScheduleToBalanceSheet(id, SocietySubscription.SubscriptionEnd);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Method to Send PDF of Report in Mail To OfficeBearer, Subscriber and Currently Logged In User Added by Ranjit        
        [HttpPost]
        public ActionResult SendMail(Guid id, FormCollection fc)
        {
            try
            {
                bool Report1 = false, Report2 = false, Report3 = false, Report4 = false, Report5 = false, Report6 = false, Report7 = false;
                Boolean.TryParse(Request.Form.GetValues("Report1")[0], out Report1); //"IncomeExpenditureStatement"   
                Boolean.TryParse(Request.Form.GetValues("Report2")[0], out Report2); //"ScheduleToIncomeExpenditureStatement"  
                Boolean.TryParse(Request.Form.GetValues("Report3")[0], out Report3); //"BalanceSheet" 
                Boolean.TryParse(Request.Form.GetValues("Report4")[0], out Report4); //"ScheduleToBalanceSheet"     
                Boolean.TryParse(Request.Form.GetValues("Report5")[0], out Report5); //"GeneralLedger"       
                Boolean.TryParse(Request.Form.GetValues("Report6")[0], out Report6); //"TrialBalanceReport"      
                Boolean.TryParse(Request.Form.GetValues("Report7")[0], out Report7); //"BankReconciliationReport"     
                var SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
                SmtpClient mailClient = new SmtpClient();
                MailMessage message = new MailMessage();
                Attachment Attachment;
                string mailFrom, mailTo = "", mailBody;
                message.IsBodyHtml = true;
                MemoryStream ms = null;
                String FileName = "";
                mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
                message.From = new MailAddress(mailFrom);
                Society Society = new SocietyService(this.ModelState).GetById(SocietySubscription.SocietyID);
                if (Society.SubscriberID != null)
                {
                    var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySubscriberID((Guid)Society.SubscriberID);
                    mailTo = Membership.GetUser((object)ud.UserID).Email + ",";//SubscriberMailID
                }
                mailTo += GetOfficeBearerMailIDsBySocietySubscriptionID(id);  //OfficeBearerMailIDs                  
                mailTo = mailTo.Remove(mailTo.Length - 1, 1);
                message.To.Add(mailTo);
                message.CC.Add(new MailAddress(Membership.GetUser().Email, Membership.GetUser().UserName));
                message.Subject = "Accounting Report";
                String AcYear = "";
                if (SocietySubscription.SubscriptionStart.Month < 4)
                {
                    AcYear = (SocietySubscription.SubscriptionStart.Year - 1) + " - " + (SocietySubscription.SubscriptionStart.Year);
                }
                else
                {
                    AcYear = (SocietySubscription.SubscriptionStart.Year) + " - " + (SocietySubscription.SubscriptionStart.Year + 1);
                }
                mailBody = "<body><p>Dear Member<br /><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Please find the accounting report(s) for the Ac Year ";
                mailBody += AcYear + " in the attachment.<br /><br />Thank you and assuring you of our best services.<br /><br />Cloud Society Team.<br /></p><center><p style=\"color: #4BAF31; font-weight: bold\">WE BELIEVE IN GREEN ENVIRONMENT</p></center></body>";
                message.Body = mailBody;
                if (Report1) //"IncomeExpenditureStatement"              
                {
                    FileName = "IncomeExpenditureStatement" + "From" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionStart) + "To" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
                    //                    ms = this.MsIncomeExpenditureStatement(id);
                    ms = this.MsIncomeExpenditureStatement(id, SocietySubscription.SubscriptionStart, SocietySubscription.SubscriptionEnd);
                    Attachment = new Attachment(ms, FileName + ".pdf", "application/pdf");
                    message.Attachments.Add(Attachment);
                }
                if (Report2) //"ScheduleToIncomeExpenditureStatement"               
                {
                    //                    ms = this.MsScheduleToIncomeExpenditureStatement(id);
                    ms = this.MsScheduleToIncomeExpenditureStatement(id, SocietySubscription.SubscriptionStart, SocietySubscription.SubscriptionEnd);
                    Attachment = new Attachment(ms, "ScheduleToIncomeExpenditureStatement.pdf", "application/pdf");
                    message.Attachments.Add(Attachment);
                }
                if (Report3) //"BalanceSheet"                
                {
                    FileName = "BalanceSheet" + "AsOn" + String.Format("{0:dd-MMM-yyyy}", SocietySubscription.SubscriptionEnd);
                    ms = this.MsBalanceSheet(id, SocietySubscription.SubscriptionEnd);
                    Attachment = new Attachment(ms, FileName + ".pdf", "application/pdf");
                    message.Attachments.Add(Attachment);
                }
                if (Report4) //"ScheduleToBalanceSheet"               
                {
                    ms = this.MsScheduleToBalanceSheet(id, SocietySubscription.SubscriptionEnd);
                    Attachment = new Attachment(ms, "ScheduleToBalanceSheet.pdf", "application/pdf");
                    message.Attachments.Add(Attachment);
                }
                if (Report5) //"GeneralLedger"                
                {
                    DateTime FromDate, ToDate;
                    DateTime.TryParse(Request.Form.GetValues("FromDate")[0], out FromDate);
                    DateTime.TryParse(Request.Form.GetValues("ToDate")[0], out ToDate);
                    Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
                    IEnumerable<AcLedger> AcLedgerList = new AcHeadService(this.ModelState).ListLedgerBySocietyIDAcHeadIds(SocietyID, String.Empty, FromDate, ToDate).OrderBy(a => a.Sequence);
                    if (AcLedgerList != null)
                    {
                        ms = this.MsGeneralLedger(id, AcLedgerList, FromDate, ToDate);
                        Attachment = new Attachment(ms, "GeneralLedger.pdf", "application/pdf");
                        message.Attachments.Add(Attachment);
                    }
                }
                if (Report6) //"TrialBalanceReport"                
                {
                    DateTime FromDate, ToDate;
                    DateTime.TryParse(Request.Form.GetValues("FromDate")[0], out FromDate);
                    DateTime.TryParse(Request.Form.GetValues("ToDate")[0], out ToDate);
                    Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
                    IEnumerable<AcBalance> AcBalanceList = new AcHeadService(this.ModelState).ListBalanceBySocietyID(SocietyID, FromDate, ToDate);
                    if (AcBalanceList != null)
                    {
                        ms = MsTrialBalanceReport(id, AcBalanceList, FromDate, ToDate);
                        Attachment = new Attachment(ms, "TrialBalanceReport.pdf", "application/pdf");
                        message.Attachments.Add(Attachment);
                    }
                }
                if (Report7) //"BankReconciliationReport"                
                {
                    DateTime AsOnDate;
                    Guid AcHeadID;
                    DateTime.TryParse(Request.Form.GetValues("AsOnDate")[0], out AsOnDate);
                    Guid.TryParse(Request.Form.GetValues("AcHeadID")[0], out AcHeadID);
                    ms = MsBankReconciliationReport(id, AcHeadID, AsOnDate);
                    Attachment = new Attachment(ms, "BankReconciliation.pdf", "application/pdf");
                    message.Attachments.Add(Attachment);
                }
                if (!string.IsNullOrEmpty(mailFrom) && mailTo != null && (Report1 || Report2 || Report3 || Report4 || Report5 || Report6 || Report7))
                {
                    mailClient.Send(message);
                    return RedirectToAction("MailSendingStatus", new { id = id, mailIDs = mailTo });
                }
                else
                    return RedirectToAction("Menu", "SocietySubscription", new { id = id });
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("ErrorInSendingMail", ex.Message);
                return RedirectToAction("Menu", "SocietySubscription", new { id = id });
            }
        }
        //Method to show Success message of Mail sending. Added by Ranjit        
        public ActionResult MailSendingStatus(Guid id, string mailIDs)
        {
            ViewBag.SocietySubscriptionID = id;
            ViewBag.MailIDs = mailIDs.Replace(",", ", ");
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View();
        }
    }
}

