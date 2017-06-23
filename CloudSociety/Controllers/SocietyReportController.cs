﻿using CloudSociety.Services;
using CloudSocietyEntities;
using CloudSocietyLib.Reporting;
using InstamojoAPI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser,Member,OfficeBearer")]
    public class SocietyReportController : Controller
    {
        const string _exceptioncontext = "Society Report Controller";
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
        //
        //private FileStreamResult FileStreamResult(List<PdfPTable> PDFPTableList, string fileName)
        //{
        //    MemoryStream ms = this._service.PdfMsCreator(PDFPTableList);
        //    //prepare output stream            
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        //    Response.Buffer = true;
        //    Response.Clear();
        //    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        //    Response.OutputStream.Flush();
        //    Response.End();
        //    return new FileStreamResult(Response.OutputStream, "application/pdf");
        //}
        //private FileStreamResult FileStreamResult(List<PdfPTable> PDFPTableList, PdfPTable PdfPHeaderTable, string fileName, PdfPTable PdfPHeadingTable=null, float SpacingAfterTable = 0F, string FooterRight = null, string Password = null)
        //{
        //    MemoryStream ms = this._service.PdfMsCreator(PDFPTableList, PdfPHeaderTable,PdfPHeadingTable, SpacingAfterTable, FooterRight, Password);
        //    //prepare output stream            
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        //    Response.Buffer = true;
        //    Response.Clear();
        //    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        //    Response.OutputStream.Flush();
        //    Response.End();
        //    return new FileStreamResult(Response.OutputStream, "application/pdf");
        //}
        //
        private PdfPTable FontPageTable(Society Society)
        {
            Table = new PdfPTable(1);
            Table.SpacingAfter = 200;
            Table.SpacingBefore = 200;
            Table.HorizontalAlignment = Element.ALIGN_CENTER;
            cell = new PdfPCell(_service.SocietyHeaderTable(Society));
            cell.HorizontalAlignment = 1;
            cell.Padding = 15;
            cell.PaddingBottom = 10;
            cell.FixedHeight = 80;
            Table.AddCell(cell);
            return Table;
        }
        private String GetStars()
        {
            contain = "*";
            for (int i = 0; i < 110; i++)
                contain += "*";
            return contain;
        }
        //private bool IsLastDayOfMonth(DateTime dateTime)
        //{
        //    return dateTime.Day == new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1).Day;
        //}
        private MemoryStream MSBalancesReport(Guid id, IEnumerable<MemberBalance> MemberBalancesList, bool IsDetails)
        {
            SocietySubscription SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Guid SocietyID = SocietySubscription.SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TablesList = new List<PdfPTable>();
            PdfPTable HeadingTable = IsDetails ? new PdfPTable(8) : new PdfPTable(3);
            decimal[] tempList;
            decimal Total = 0M;
            contain = "Member's Balances" + " As On " + String.Format("{0:dd-MMM-yyyy}", (SocietySubscription.PaidTillDate < DateTime.Now ? SocietySubscription.PaidTillDate : DateTime.Now));
            if (IsDetails)
            {
                tempList = new decimal[] { 0M, 0M, 0M, 0M, 0M, 0M };
                float[] widths = new float[] { 65f, 125f, 55f, 55f, 55f, 55f, 55f, 55f };
                HeadingTable.SetWidthPercentage(widths, rect);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 8;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                HeadingTable.AddCell(cell);
                //Heading       
                cell = new PdfPCell(new Phrase(new Chunk("Unit No.", FontV7Bold)));
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV7Bold)));
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("ChgBal", FontV7Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("NonChgBal", FontV7Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("IntBal", FontV7Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("TaxBal", FontV7Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Advance", FontV7Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Balance", FontV7Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);
                //
                Table = new PdfPTable(8);
                Table.SetWidthPercentage(widths, rect);
                foreach (var MemberBalance in MemberBalancesList)
                {
                    Total = (MemberBalance.ChgBal ?? 0) + (MemberBalance.NonChgBal ?? 0) + (MemberBalance.IntBal ?? 0) + (MemberBalance.TaxBal ?? 0);
                    Total = Total - (MemberBalance.Advance ?? 0);
                    cell = new PdfPCell(new Phrase(new Chunk(MemberBalance.BuildingUnit, FontV7)));
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(MemberBalance.Member, FontV7)));
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk((MemberBalance.ChgBal == 0 ? "--" : MemberBalance.ChgBal.ToString()), FontV7)));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                    tempList[0] += MemberBalance.ChgBal ?? 0;
                    cell = new PdfPCell(new Phrase(new Chunk((MemberBalance.NonChgBal == 0 ? "--" : MemberBalance.NonChgBal.ToString()), FontV7)));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                    tempList[1] += MemberBalance.NonChgBal ?? 0;
                    cell = new PdfPCell(new Phrase(new Chunk((MemberBalance.IntBal == 0 ? "--" : MemberBalance.IntBal.ToString()), FontV7)));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                    tempList[2] += MemberBalance.IntBal ?? 0;
                    cell = new PdfPCell(new Phrase(new Chunk((MemberBalance.TaxBal == 0 ? "--" : MemberBalance.TaxBal.ToString()), FontV7)));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                    tempList[3] += MemberBalance.TaxBal ?? 0;
                    cell = new PdfPCell(new Phrase(new Chunk((MemberBalance.Advance == 0 ? "--" : "(" + MemberBalance.Advance.ToString() + ")"), FontV7)));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                    tempList[4] += MemberBalance.Advance ?? 0;
                    cell = new PdfPCell(new Phrase(new Chunk((Total == 0 ? "--" : Total.ToString()), FontV7)));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                    tempList[5] += Total;
                }
                cell = new PdfPCell(new Paragraph("Total : ", FontV7Bold));
                cell.HorizontalAlignment = 2;
                cell.Colspan = 2;
                Table.AddCell(cell);
                for (int j = 0; j < tempList.Length; j++)
                {
                    cell = new PdfPCell(new Paragraph((tempList[j] == 0 ? "--" : tempList[j].ToString()), FontV7Bold));
                    cell.HorizontalAlignment = 2;
                    Table.AddCell(cell);
                }
                TablesList.Add(Table);
            }
            else
            {
                decimal grandTotal = 0;
                float[] widths = new float[] { 2f, 3f, 1f };
                //Heading  
                HeadingTable.SetWidths(widths);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Unit No.", FontV9Bold)));
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV9Bold)));
                HeadingTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Balance", FontV9Bold)));
                cell.HorizontalAlignment = 2;
                HeadingTable.AddCell(cell);

                Table = new PdfPTable(3);
                Table.SetWidths(widths);
                foreach (var MemberBalance in MemberBalancesList)
                {
                    Total = (MemberBalance.ChgBal ?? 0) + (MemberBalance.NonChgBal ?? 0) + (MemberBalance.IntBal ?? 0) + (MemberBalance.TaxBal ?? 0);
                    Total = Total - (MemberBalance.Advance ?? 0);
                    grandTotal += Total;
                    cell = new PdfPCell(new Phrase(new Chunk(MemberBalance.BuildingUnit, FontV9)));
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(MemberBalance.Member, FontV9)));
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk((Total == 0 ? "--" : Total.ToString()), FontV9)));
                    cell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right 
                    Table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph("Grand Total : ", FontV9Bold));
                cell.Colspan = 2;
                cell.HorizontalAlignment = 2;
                Table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(new Chunk(grandTotal.ToString(), FontV9Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);
                TablesList.Add(Table);
            }
            return _service.PdfMsCreator(TablesList, Society, HeadingTable);
        }
        private String GetSocietyMemberAddress(SocietyMember SocietyMember, bool MLine)
        {
            contain = "";
            if (SocietyMember != null)
            {
                if (MLine)
                {
                    contain = (SocietyMember.Address == null ? "" : SocietyMember.Address + ", ");
                    contain += "\n" + (SocietyMember.City == null ? "" : SocietyMember.City) + (SocietyMember.PIN == null ? "" : "-" + SocietyMember.PIN + ", ");
                    contain += "\n" + (SocietyMember.StateID == null ? " " : SocietyMember.State.Name + (SocietyMember.State.CountryCode == null ? "." : ", " + SocietyMember.State.Country.Name + "."));
                }
                else
                {
                    contain = (SocietyMember.Address == null ? "" : SocietyMember.Address) + (SocietyMember.City == null ? "." : ", " + SocietyMember.City) + (SocietyMember.PIN == null ? "" : "-" + SocietyMember.PIN) + (SocietyMember.StateID == null ? "." : ", " + SocietyMember.State.Name + ", " + SocietyMember.State.Country.Name + ".");
                }
                return contain;
            }
            else
                return contain;
        }
        private String GetSocietyMemberAddress(SocietyMemberNominee SocietyMemberNominee, bool MLine)
        {
            contain = "";
            if (SocietyMemberNominee != null)
            {
                if (MLine)
                {
                    contain = (SocietyMemberNominee.Address == null ? "" : SocietyMemberNominee.Address + ", ");
                    contain += "\n" + (SocietyMemberNominee.City == null ? "" : SocietyMemberNominee.City) + (SocietyMemberNominee.PIN == null ? "" : "-" + SocietyMemberNominee.PIN + ", ");
                    contain += "\n" + (SocietyMemberNominee.StateID == null ? " " : SocietyMemberNominee.State.Name + (SocietyMemberNominee.State.CountryCode == null ? "." : ", " + SocietyMemberNominee.State.Country.Name + "."));
                }
                else
                {
                    contain = (SocietyMemberNominee.Address == null ? "" : SocietyMemberNominee.Address + ", ") + (SocietyMemberNominee.City == null ? "" : SocietyMemberNominee.City) + (SocietyMemberNominee.PIN == null ? "" : "-" + SocietyMemberNominee.PIN + ", ") + (SocietyMemberNominee.StateID == null ? " " : SocietyMemberNominee.State.Name + (SocietyMemberNominee.State.CountryCode == null ? "." : ", " + SocietyMemberNominee.State.Country.Name + "."));
                }
                return contain;
            }
            else
                return contain;
        }
        private String GetSocietyMemberAddress(SocietyMemberJointHolder SocietyMemberJointHolder, bool MLine)
        {
            contain = "";
            if (SocietyMemberJointHolder != null)
            {
                if (MLine)
                {
                    contain = (SocietyMemberJointHolder.Address == null ? "" : SocietyMemberJointHolder.Address + ", ");
                    contain += "\n" + (SocietyMemberJointHolder.City == null ? "" : SocietyMemberJointHolder.City) + (SocietyMemberJointHolder.PIN == null ? "" : "-" + SocietyMemberJointHolder.PIN + ", ");
                    contain += "\n" + (SocietyMemberJointHolder.StateID == null ? " " : SocietyMemberJointHolder.State.Name + (SocietyMemberJointHolder.State.CountryCode == null ? "." : ", " + SocietyMemberJointHolder.State.Country.Name + "."));
                }
                else
                {
                    contain = (SocietyMemberJointHolder.Address == null ? "" : SocietyMemberJointHolder.Address + ", ") + (SocietyMemberJointHolder.City == null ? "" : SocietyMemberJointHolder.City) + (SocietyMemberJointHolder.PIN == null ? "" : "-" + SocietyMemberJointHolder.PIN + ", ") + (SocietyMemberJointHolder.StateID == null ? " " : SocietyMemberJointHolder.State.Name + (SocietyMemberJointHolder.State.CountryCode == null ? "." : ", " + SocietyMemberJointHolder.State.Country.Name + "."));
                }
                return contain;
            }
            else
                return contain;
        }
        public SocietyReportController()
        {
            rect = PageSize.A4;
            PageWidth = rect.Width;
            _service = new PDFService(this.ModelState);
        }
        // GET: /Generate Report For List Of Society Members By SocietySubscriptionID 
        //[HttpPost]
        public FileStreamResult MembersRegister(Guid id)
        {
            contain = "";
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDList;
            IEnumerable<SocietyMember> SocietyMembersList;
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadTable = new PdfPTable(3);
            try
            {
                //Heading for all pages                
                float[] widths = new float[] { 1f, 2f, 3f };
                HeadTable.SetWidths(widths);
                cell = new PdfPCell(_service.CaptionTable("Member's List", FontV9Bold, System.Drawing.Color.LightGray));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Unit No.", FontV9Bold)));
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV9Bold)));
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Nominee's Name", FontV9Bold)));
                HeadTable.AddCell(cell);
                //
                Table = new PdfPTable(3);
                Table.SetWidths(widths);
                string NomineeName;
                BuildingUnitWithIDList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDList)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(BuildingUnitWithID.BuildingUnit, FontV9)));
                    Table.AddCell(cell);
                    contain = "";
                    NomineeName = "";
                    SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var SocietyMember in SocietyMembersList)
                    {
                        contain += SocietyMember.Member;
                        foreach (var JointHolder in SocietyMember.SocietyMemberJointHolders)
                        {
                            contain += "\n" + JointHolder.Name + " (JH)";
                        }
                        foreach (var Nominee in SocietyMember.SocietyMemberNominees)
                        {
                            NomineeName += "\n" + Nominee.Name + " (" + Nominee.NominationPerc + " %)";
                        }
                    }
                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontV9)));
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(NomineeName, FontV9)));
                    Table.AddCell(cell);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "MemberList.pdf", HeadTable);
        }

        public FileStreamResult MembersWithoutNomineeRegister(Guid id)
        {
            contain = "";
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDList;
            IEnumerable<SocietyMember> SocietyMembersList;
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadTable = new PdfPTable(2);
            try
            {
                //Heading for all pages                
                float[] widths = new float[] { 1f, 3f };
                HeadTable.SetWidths(widths);
                cell = new PdfPCell(_service.CaptionTable("Member Register", FontV9Bold, System.Drawing.Color.LightGray));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 2;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Unit No.", FontV9Bold)));
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV9Bold)));
                HeadTable.AddCell(cell);
                //
                Table = new PdfPTable(2);
                Table.SetWidths(widths);
                BuildingUnitWithIDList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDList)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(BuildingUnitWithID.BuildingUnit, FontV9)));
                    Table.AddCell(cell);
                    contain = "";
                    SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var SocietyMember in SocietyMembersList)
                    {
                        contain += SocietyMember.Member;
                        foreach (var JointHolder in SocietyMember.SocietyMemberJointHolders)
                        {
                            contain += "\n" + JointHolder.Name + " (JH)";
                        }
                    }
                    cell = new PdfPCell(new Phrase(new Chunk(contain, FontV9)));
                    Table.AddCell(cell);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "MemberRegister.pdf", HeadTable);
        }

        public FileStreamResult CommitteeMembersList(Guid id)
        {
            //            contain = "";
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            UserDetailService udservice = new UserDetailService(this.ModelState);
            IEnumerable<SocietyMember> SocietyMembersList;
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadTable = new PdfPTable(3);
            try
            {
                //Heading for all pages                
                float[] widths = new float[] { 1f, 2f, 3f };
                HeadTable.SetWidths(widths);
                cell = new PdfPCell(_service.CaptionTable("List of Committee Members", FontV9Bold, System.Drawing.Color.LightGray));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 3;
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Sr. No.", FontV9Bold)));
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Unit No.", FontV9Bold)));
                HeadTable.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV9Bold)));
                HeadTable.AddCell(cell);
                //
                Table = new PdfPTable(3);
                Table.SetWidths(widths);
                int srno = 1;
                SocietyBuildingUnitTransfer objSocietyBuildingUnitTransfer;
                SocietyMembersList = new SocietyMemberService(this.ModelState).ListByParentId(SocietyID);
                foreach (var SocietyMember in SocietyMembersList)
                {
                    var userDetails = udservice.GetBySocietyMemberID(SocietyMember.SocietyMemberID);
                    if (userDetails != null)
                    {
                        var user = Membership.GetUser((object)userDetails.UserID);
                        if (user != null && Roles.IsUserInRole(user.UserName, "OfficeBearer"))
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(srno.ToString(), FontV9)));
                            cell.HorizontalAlignment = 2;
                            Table.AddCell(cell);
                            srno++;
                            objSocietyBuildingUnitTransfer = SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault(t => t.uEndDate == null);
                            if (null != objSocietyBuildingUnitTransfer)
                            {
                                contain = objSocietyBuildingUnitTransfer.SocietyBuildingUnit.SocietyBuilding.Building + " - " + objSocietyBuildingUnitTransfer.SocietyBuildingUnit.Unit;
                            }
                            cell = new PdfPCell(new Phrase(new Chunk(contain, FontV9)));
                            Table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(SocietyMember.Member, FontV9)));
                            Table.AddCell(cell);
                        }
                    }
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "CommitteeMemberList.pdf", HeadTable);
        }

        // GET: /Generate Report For All Balances By SocietySubscriptionID
        //public FileStreamResult PDFAllBalancesReport(Guid id,Guid? SocietyBuildingID, Boolean IsDetails)
        //{
        //    IEnumerable<MemberBalance> MemberBalanceList;
        //    if(SocietyBuildingID != null )
        //        MemberBalanceList = new SocietyBuildingUnitService(this.ModelState).ListBalanceBySocietySubscriptionID(id).Where(br=>br.SocietyBuildingID ==SocietyBuildingID);
        //    else
        //        MemberBalanceList = new SocietyBuildingUnitService(this.ModelState).ListBalanceBySocietySubscriptionID(id);
        //    return this.FileStreamResult(this.MSBalancesReport(id, MemberBalanceList, IsDetails), (IsDetails ? "MemberBalancesDetailsReport.pdf" : "MemberBalancesReport.pdf"));
        //}
        // GET: /Ask amount range
        [HttpGet]
        public ActionResult AmountBalancesReport(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societyId = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.SocietyBuildingList = new SocietyBuildingService(this.ModelState).ListByParentId(societyId);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
            return View();
        }
        // GET: /Generate Report For Balances By SocietySubscriptionID and Amount
        [HttpPost]
        public FileStreamResult AmountBalancesReport(Guid id, Guid? SocietyBuildingID, string Amount, Boolean IsDetails, string BillAbbreviation)
        {
            IEnumerable<MemberBalance> MemberBalanceList;
            decimal amount = 0;
            if (Amount == null || Amount == "")
            {
                MemberBalanceList = new SocietyBuildingUnitService(this.ModelState).ListBalanceForSocietySubscription(id, SocietyBuildingID, BillAbbreviation);
            }
            else
            {
                amount = Decimal.Parse(Amount);
                MemberBalanceList = new SocietyBuildingUnitService(this.ModelState).ListBalanceForSocietySubscription(id, amount, SocietyBuildingID, BillAbbreviation);
            }
            return this.FileStreamResult(this.MSBalancesReport(id, MemberBalanceList, IsDetails), (IsDetails ? "MemberBalancesDetailsReport.pdf" : "MemberBalancesReport.pdf"));
        }

        // GET: /Generate Report For JForm SocietySubscriptionID .
        public FileStreamResult JFormReport(Guid id)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            IEnumerable<BuildingUnitWithID> SocietyBuildingUnits = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            try
            {
                PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                writer.PageEvent = PageEventHandler;
                document.Open();
                //PageEventHandler.AddOutline(writer, "J-Form", 10f); 
                document.NewPage();
                document.Add(new Paragraph(this.GetStars()));
                Table = this.FontPageTable(Society);
                cell = new PdfPCell(new Paragraph("\"J\" FORM REGISTER", FontV9Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Table.AddCell(cell);
                document.Add(Table);
                Table = new PdfPTable(1);
                Table.TotalWidth = document.PageSize.Width;
                cell = new PdfPCell(new Paragraph(this.GetStars()));
                cell.Border = 0;
                cell.PaddingLeft = 9;
                Table.AddCell(cell);
                Table.WriteSelectedRows(0, -1, (document.LeftMargin), (document.BottomMargin + 10), writer.DirectContent);
                int count = 1;
                foreach (var SocietyBuildingUnit in SocietyBuildingUnits)
                {
                    IEnumerable<SocietyMember> SocietyMembers = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(SocietyBuildingUnit.SocietyBuildingUnitID);
                    foreach (var Member in SocietyMembers)
                    {
                        document.NewPage();
                        contain = "FORM \"J\"";
                        contain += "\nLIST OF MEMBERS";
                        contain += "\n(See Rule 33)";
                        Table = new PdfPTable(1);
                        cell = new PdfPCell(new Paragraph(contain, FontV9Bold));
                        cell.HorizontalAlignment = 1;
                        Table.AddCell(cell);
                        document.Add(Table);
                        Table = new PdfPTable(3);
                        Table.TotalWidth = document.PageSize.Width;
                        Table.SetWidths(new Single[] { 5, 25, 5 });
                        cell = new PdfPCell(new Phrase(new Chunk("Sr.No.", FontV9Bold)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV9Bold)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Class Of Member", FontV9Bold)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(count + ".", FontV9)));
                        Table.AddCell(cell);
                        contain = Member.Member + "                           " + SocietyBuildingUnit.BuildingUnit + "\n";
                        contain += this.GetSocietyMemberAddress(Member, true);
                        cell = new PdfPCell(new Phrase(new Chunk(contain, FontV9)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(Member.MemberClass.Class, FontV9)));
                        Table.AddCell(cell);
                        for (int i = 1; i <= 21; i++)
                        {
                            cell = new PdfPCell(new Paragraph(""));
                            cell.FixedHeight = 60;
                            Table.AddCell(cell);
                        }
                        count += 1;
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
            return this.FileStreamResult(ms, "J-Form.pdf");
        }
        // GET: /Generate Report For IForm SocietySubscriptionID .
        public FileStreamResult IFormReport(Guid id)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDs;
            IList<SocietyBuildingUnitTransfer> SocietyBuildingUnitTransferList;
            IEnumerable<SocietyMemberJointHolder> JointHolders;
            IEnumerable<SocietyMemberNominee> MemberNominees;
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            try
            {
                //PDFHeaderFooterService PageEventHandler = new PDFHeaderFooterService();
                //writer.PageEvent = PageEventHandler;           
                document.Open();
                document.NewPage();
                document.Add(new Paragraph(this.GetStars()));
                Table = this.FontPageTable(Society);
                cell = new PdfPCell(new Paragraph("\"I\" FORM REGISTER", FontV9Bold));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Table.AddCell(cell);
                document.Add(Table);
                Table = new PdfPTable(1);
                Table.TotalWidth = document.PageSize.Width;
                cell = new PdfPCell(new Paragraph(this.GetStars()));
                cell.Border = 0;
                cell.PaddingLeft = 9;
                Table.AddCell(cell);
                Table.WriteSelectedRows(0, -1, (document.LeftMargin), (document.BottomMargin + 9), writer.DirectContent);

                document.NewPage();
                contain = "FORM \"I\"";
                contain += "\n(See Rule 32 and 65[1])";
                contain += "\nINDEX OF MEMBERS";
                contain += "[Sec. 38(1) of " + Society.Name + " Societies' Act 1960]";
                PdfPTable HeadTable = new PdfPTable(1);
                cell = new PdfPCell(new Paragraph(contain, FontV9Bold));
                cell.HorizontalAlignment = 1;
                HeadTable.AddCell(cell);
                document.Add(HeadTable);
                Table = new PdfPTable(4);
                Table.TotalWidth = document.PageSize.Width;
                Table.SetWidths(new Single[] { 3, 15, 10, 3 });
                cell = new PdfPCell(new Phrase(new Chunk("Sr.No.", FontV9Bold)));
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Member's Name", FontV9Bold)));
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Flat", FontV9Bold)));
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(new Chunk("Page", FontV9Bold)));
                Table.AddCell(cell);
                int count = 1;
                //For Index page
                BuildingUnitWithIDs = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDs)
                {
                    IEnumerable<SocietyMember> SocietyMembers = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var Member in SocietyMembers)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(count + ".", FontV9)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(Member.Member, FontV9)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(BuildingUnitWithID.BuildingUnit, FontV9)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("", FontV9)));
                        Table.AddCell(cell);
                        count += 1;
                    }
                }
                document.Add(Table);
                int pageCount = 0;
                string[] tempList;
                PdfPTable InnerTable1;
                PdfPTable InnerTable2;
                PdfPTable tempTable;
                BuildingUnitWithIDs = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDs)
                {
                    SocietyBuildingUnit SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(BuildingUnitWithID.SocietyBuildingUnitID);
                    IEnumerable<SocietyMember> SocietyMembers = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var Member in SocietyMembers)
                    {
                        SocietyBuildingUnitTransferList = (IList<SocietyBuildingUnitTransfer>)new SocietyBuildingUnitTransferService(this.ModelState).ListByParentId(BuildingUnitWithID.SocietyBuildingUnitID);
                        JointHolders = new SocietyMemberJointHolderService(this.ModelState).ListByParentId(Member.SocietyMemberID);
                        MemberNominees = new SocietyMemberNomineeService(this.ModelState).ListByParentId(Member.SocietyMemberID);
                        document.NewPage();
                        pageCount += 1;
                        //for footer   
                        //for footer
                        //PdfPTable FooterTable = new PdfPTable(2);
                        //FooterTable.HorizontalAlignment = Element.ALIGN_CENTER;
                        //FooterTable.TotalWidth = document.PageSize.Width;
                        //cell = new PdfPCell(new Paragraph("Page " + writer.PageNumber + " of " + BuildingUnitWithIDs.Count(), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                        //cell.Border = 0;
                        //cell.PaddingLeft = 0;
                        //cell.PaddingRight = 0;
                        //FooterTable.AddCell(cell);
                        //cell = new PdfPCell(new Paragraph("Printed On " + String.Format("{0:dd-MMM-yyyy, HH:mm:ss}", DateTime.Now), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                        //cell.Border = 0;
                        //cell.PaddingLeft = 90;
                        //FooterTable.AddCell(cell);
                        //FooterTable.WriteSelectedRows(0, -1, 50, (document.BottomMargin + 9), writer.DirectContent);
                        // 
                        PdfPTable footerTbl = new PdfPTable(2);
                        footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerTbl.TotalWidth = document.PageSize.Width;
                        cell = new PdfPCell(new Paragraph("Page " + pageCount, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                        cell.Border = 0;
                        //cell.PaddingLeft = 9;
                        footerTbl.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("Printed On " + String.Format("{0:dd-MMM-yyyy, HH:mm:ss}", DateTime.Now), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                        cell.Border = 0;
                        cell.PaddingLeft = 90;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 50, (document.BottomMargin + 9), writer.DirectContent);
                        //
                        HeadTable.SetWidthPercentage(new float[] { 0.90F * PageWidth }, rect);
                        document.Add(HeadTable);
                        Table = new PdfPTable(1);
                        //Table.TotalWidth = document.PageSize.Width;
                        Table.SetWidthPercentage(new float[] { 0.90F * PageWidth }, rect);
                        cell = new PdfPCell(new Paragraph("1. SERIAL NUMBER: " + (Member.FolioNo == null ? "" : Member.FolioNo.ToString()), FontV9Bold));
                        Table.AddCell(cell);
                        Table.TotalWidth = document.PageSize.Width;
                        cell = new PdfPCell(new Paragraph("2. MEMBER(S) DETAILS:", FontV9Bold));
                        Table.AddCell(cell);
                        //Heading Inner Table1
                        InnerTable1 = new PdfPTable(4);
                        InnerTable1.TotalWidth = document.PageSize.Width;
                        InnerTable1.SetWidths(new Single[] { 8, 6, 6, 6 });
                        cell = new PdfPCell(new Phrase(new Chunk("Member's Name" + "\n-----------------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Memb. Type" + "\n------------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Occupation" + "\n-----------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Date Of Admission" + "\n---------------------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        //Heading Inner Table2
                        InnerTable2 = new PdfPTable(4);
                        InnerTable2.TotalWidth = document.PageSize.Width;
                        InnerTable2.SetWidths(new Single[] { 4, 4, 4, 5 });
                        cell = new PdfPCell(new Phrase(new Chunk("Memb. Age On Admission" + "\n------------------------------------", FontV7Bold)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Enterence Fee Paid Date" + "\n-----------------------------------", FontV7Bold)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Membership Cessation Date" + "\n----------------------------------------", FontV7Bold)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Reason of Cessation of Memb." + "\n-------------------------------------------", FontV7Bold)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        //Values InnerTable1 
                        cell = new PdfPCell(new Phrase(new Chunk(Member.Member, FontV8)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(Member.MemberClass.Class, FontV8)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk((Member.OccupationEntity == null ? "" : Member.OccupationEntity.Name) + (Member.Occupation == null ? "" : " (" + Member.Occupation + ")"), FontV8)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnitTransferList.Min(s => s.TransferDate)), FontV8)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        //Values InnerTable2
                        cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk((SocietyBuildingUnitTransferList.Count <= 1 ? "" : String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnitTransferList.Max(s => s.TransferDate))), FontV8)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                        cell.Border = 0;
                        InnerTable2.AddCell(cell);
                        foreach (var JointHolder in JointHolders)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(JointHolder.Name + "(JH)", FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(JointHolder.MemberClass.Class, FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                            cell.Border = 0;
                            InnerTable2.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", JointHolder.EnteranceFeePaidOn), FontV8)));
                            cell.Border = 0;
                            InnerTable2.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                            cell.Border = 0;
                            InnerTable2.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("", FontV8)));
                            cell.Border = 0;
                            InnerTable2.AddCell(cell);
                        }
                        //cell = new PdfPCell(InnerTable1);
                        //cell.BorderWidthBottom = 0;
                        Table.AddCell(InnerTable1);
                        //cell = new PdfPCell(InnerTable2);
                        //cell.BorderWidthTop = 0;
                        //cell.BorderWidthBottom = 0;
                        Table.AddCell(InnerTable2);
                        Table.AddCell(new PdfPCell(new Paragraph("3. ADDRESS OF MEMBER:", FontV9Bold)));
                        Table.AddCell(new PdfPCell(new Paragraph(this.GetSocietyMemberAddress(Member, false), FontV8)));
                        Table.AddCell(new PdfPCell(new Paragraph("4. NOMINATION DETAILS UNDER SEC. 30(1)", FontV9Bold)));
                        InnerTable1 = new PdfPTable(4);
                        InnerTable1.SetWidths(new Single[] { 15, 5, 5, 5 });
                        cell = new PdfPCell(new Phrase(new Chunk("Nominee's Name" + "\n-------------------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("D.O. Birth" + "\n--------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Nomin'n Date" + "\n------------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk("Relation" + "\n------------", FontV8Bold)));
                        cell.Border = 0;
                        InnerTable1.AddCell(cell);
                        MemberNominees = new SocietyMemberNomineeService(this.ModelState).ListByParentId(Member.SocietyMemberID);
                        InnerTable2 = new PdfPTable(1);
                        foreach (var MemberNominee in MemberNominees)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(MemberNominee.Name, FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk((MemberNominee.BirthDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", MemberNominee.BirthDate)), FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk((MemberNominee.NominationDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", MemberNominee.NominationDate)), FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(MemberNominee.RelationshipEntity.Name + "\n" + (MemberNominee.Relationship == null ? "" : MemberNominee.Relationship + "(Oth.)"), FontV8)));
                            cell.Border = 0;
                            InnerTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(this.GetSocietyMemberAddress(MemberNominee, false), FontV8)));
                            cell.Border = 0;
                            InnerTable2.AddCell(cell);
                        }
                        cell = new PdfPCell(InnerTable1);
                        Table.AddCell(cell);
                        Table.AddCell(new PdfPCell(new Paragraph("5. ADDRESS OF NOMINEES:", FontV9Bold)));
                        cell = new PdfPCell(InnerTable2);
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("6. REMARKS:", FontV9Bold));
                        cell.BorderColorBottom = BaseColor.WHITE;
                        Table.AddCell(cell);
                        //Remarks
                        foreach (var transfer in SocietyBuildingUnitTransferList)
                        {
                            if (transfer.uEndDate == null)
                            {
                                InnerTable1 = new PdfPTable(7);
                                InnerTable1.SetWidths(new Single[] { 3, 2, 10, 2, 3, 2, 3 });
                                tempList = new string[] { "Date", "Cash Book Folio", "PARTICLUARS OF SHARES HELD", "Total", "Amount Received", "No of Shares Held ", "Certificate and Serial Number of Shares Held" };
                                //InnerTable2 for PARTICLUARS OF SHARES HELD(Heading)
                                InnerTable2 = new PdfPTable(3);
                                InnerTable2.SetWidths(new Single[] { 4, 4, 8 });
                                cell = new PdfPCell(new Phrase(new Chunk("PARTICLUARS OF SHARES HELD", FontV7)));
                                cell.HorizontalAlignment = 1;
                                cell.Colspan = 3;
                                InnerTable2.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("Application", FontV7)));
                                InnerTable2.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("Allotment", FontV7)));
                                InnerTable2.AddCell(cell);
                                //tempTable for Amount Received On(Heading)
                                tempTable = new PdfPTable(2);
                                cell = new PdfPCell(new Phrase(new Chunk("Amount Received On", FontV7)));
                                cell.Colspan = 2;
                                tempTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("1st Call", FontV7)));
                                tempTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("2nd Call", FontV7)));
                                tempTable.AddCell(cell);
                                cell = new PdfPCell(tempTable);
                                InnerTable2.AddCell(cell);
                                for (int i = 0; i < tempList.Length; i++)
                                {
                                    if (i == 2)
                                    {
                                        cell = new PdfPCell(InnerTable2);
                                        cell.HorizontalAlignment = 1;
                                    }
                                    else
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk(tempList[i], FontV7)));
                                        cell.HorizontalAlignment = 1;
                                    }
                                    InnerTable1.AddCell(cell);
                                }
                                cell = new PdfPCell(InnerTable1);
                                Table.AddCell(cell);
                                // Values
                                InnerTable1 = new PdfPTable(7);
                                InnerTable1.SetWidths(new Single[] { 3, 2, 10, 2, 3, 2, 3 });
                                InnerTable2 = new PdfPTable(3);
                                InnerTable2.SetWidths(new Single[] { 4, 4, 8 });
                                cell = new PdfPCell(new Phrase(new Chunk(" ", FontV7)));
                                InnerTable2.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk(" ", FontV7)));
                                InnerTable2.AddCell(cell);
                                //for Amount Received On
                                tempTable = new PdfPTable(2);
                                cell = new PdfPCell(new Phrase(new Chunk(" ", FontV7)));
                                tempTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk(" ", FontV7)));
                                tempTable.AddCell(cell);
                                cell = new PdfPCell(tempTable);
                                InnerTable2.AddCell(cell);
                                tempList = new string[] { String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate), "", "P OF S H", "", (SocietyBuildingUnit.Value == null ? " " : SocietyBuildingUnit.Value.ToString()), (SocietyBuildingUnit.NoOfShares == null ? " " : SocietyBuildingUnit.NoOfShares.ToString()), (SocietyBuildingUnit.CertificateNo == null ? " " : SocietyBuildingUnit.CertificateNo.ToString() + ",   ") + (SocietyBuildingUnit.DistinctiveFrom == null ? "" : SocietyBuildingUnit.DistinctiveFrom.ToString()) + (SocietyBuildingUnit.DistinctiveTo == null ? " " : " To " + SocietyBuildingUnit.DistinctiveTo.ToString()) };
                                for (int i = 0; i < tempList.Length; i++)
                                {
                                    if (i == 2)
                                    {
                                        cell = new PdfPCell(InnerTable2);
                                        cell.HorizontalAlignment = 1;
                                    }
                                    else
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk(tempList[i], FontV7)));
                                        cell.HorizontalAlignment = 1;
                                    }
                                    InnerTable1.AddCell(cell);
                                }
                                Table.AddCell(new PdfPCell(InnerTable1));
                            }
                            else
                            {
                                //Heading
                                cell = new PdfPCell(new Paragraph("PARTICULARS OF SHARES TRANSFERRED OR SURRENDERED", FontV9Bold));
                                cell.HorizontalAlignment = 1;
                                Table.AddCell(cell);
                                tempTable = new PdfPTable(8);
                                tempTable.SetWidths(new Single[] { 3, 2, 3, 3, 3, 3, 5, 2 });
                                tempList = new string[] { "Date", "Cash Book Folio", "Date", "Cash Book Folio or Shares Transfer Reg. No.", "Number Of Share Cert." + "\n--------------------" + "\nSr. No. Of Share Cert.", "No. Of Shares Transferred Or Refunded", "b", "Amount" };
                                InnerTable1 = new PdfPTable(2);
                                cell = new PdfPCell(new Phrase(new Chunk("B A L A N C E S", FontV7)));
                                cell.HorizontalAlignment = 1;
                                cell.Colspan = 2;
                                InnerTable1.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("No. Of Shares Held", FontV7)));
                                InnerTable1.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("Serial No. Of Shares Certificate", FontV7)));
                                InnerTable1.AddCell(cell);
                                for (int i = 0; i < tempList.Length; i++)
                                {
                                    if (i == 6)
                                    {
                                        cell = new PdfPCell(InnerTable1);
                                        cell.HorizontalAlignment = 1;
                                    }
                                    else
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk(tempList[i], FontV7)));
                                        cell.HorizontalAlignment = 1;
                                    }
                                    tempTable.AddCell(cell);
                                }
                                Table.AddCell(new PdfPCell(tempTable));
                                //Values
                                tempTable = new PdfPTable(8);
                                tempTable.SetWidths(new Single[] { 3, 2, 3, 3, 3, 3, 5, 2 });
                                //tempList = new string[] { String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnitTransferList.Max(d => d.TransferDate)), "", String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate), "", "", (SocietyBuildingUnit.NoOfShares == null ? "" : SocietyBuildingUnit.NoOfShares.ToString()), "", (SocietyBuildingUnit.Value == null ? "" : SocietyBuildingUnit.Value.ToString()) };
                                tempList = new string[] { String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnitTransferList.Max(d => d.TransferDate)), "", String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate), "", "", (SocietyBuildingUnit.NoOfShares == null ? "" : SocietyBuildingUnit.NoOfShares.ToString()), "", "" };
                                InnerTable1 = new PdfPTable(2);
                                cell = new PdfPCell(new Phrase(new Chunk("", FontV7)));
                                InnerTable1.AddCell(cell);
                                cell = new PdfPCell(new Phrase(new Chunk("", FontV7)));
                                InnerTable1.AddCell(cell);
                                for (int i = 0; i < tempList.Length; i++)
                                {
                                    if (i == 6)
                                    {
                                        cell = new PdfPCell(InnerTable1);
                                        cell.HorizontalAlignment = 1;
                                    }
                                    else
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk(tempList[i], FontV7)));
                                        cell.HorizontalAlignment = 1;
                                    }
                                    tempTable.AddCell(cell);
                                }
                                Table.AddCell(new PdfPCell(tempTable));
                            }
                        }
                    }
                    document.Add(Table);
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
            return this.FileStreamResult(ms, "I-Form.pdf");
        }
        // GET: /Generate Report For Nomination
        public FileStreamResult NominationRegister(Guid id)
        {
            int count = 0;
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDList;
            IEnumerable<SocietyMember> SocietyMembersList;
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadingTable = new PdfPTable(6);
            string[] tempList = new string[] { "Sr.No." + "\n1", "Unit No." + "\n\n2", "Name Of Nominator" + "\n\n3", "Date Of Nomination" + "\n4", "Name Of Nominees" + "\n\n5", "Other Details" + "\nColumns" + "\n" + "6,7,8,9,10,11" };
            try
            {
                float[] widths = new float[] { 0.8f, 3f, 5f, 2.5f, 5f, 6.5f };
                HeadingTable.SetWidths(widths);
                cell = new PdfPCell(_service.CaptionTable("Register Of Nomination", FontV8Bold, System.Drawing.Color.LightGray));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 6;
                HeadingTable.AddCell(cell);
                for (int j = 0; j < tempList.Length; j++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(tempList[j], FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                Table = new PdfPTable(6);
                Table.SetWidths(widths);

                BuildingUnitWithIDList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDList)
                {
                    contain = "";
                    count += 1;
                    SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    cell = new PdfPCell(new Paragraph(count + ".", FontV7));
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Paragraph(BuildingUnitWithID.BuildingUnit, FontV7));
                    Table.AddCell(cell);
                    PdfPTable NomineeDetails = new PdfPTable(3);
                    widths = new float[] { 2.5f, 5f, 6.5f };
                    NomineeDetails.SetWidths(widths);
                    foreach (var SocietyMember in SocietyMembersList)
                    {
                        contain += SocietyMember.Member + "\n" + this.GetSocietyMemberAddress(SocietyMember, true) + "\n";
                        cell = new PdfPCell(new Paragraph(contain, FontV7));
                        Table.AddCell(cell);
                        string temp = "";
                        foreach (var Nominee in SocietyMember.SocietyMemberNominees)
                        {
                            cell = new PdfPCell(new Paragraph((Nominee.NominationDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", Nominee.NominationDate)), FontV7));
                            NomineeDetails.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(Nominee.Name + "\n" + this.GetSocietyMemberAddress(Nominee, true), FontV7));
                            NomineeDetails.AddCell(cell);
                            temp = "Date Of MCM     : " + (Nominee.MCMDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", Nominee.MCMDate)) + "\n";
                            temp += "Date Of Revoc'n : " + (Nominee.RevocationDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", Nominee.RevocationDate)) + "\n";
                            temp += "Relation : " + (Nominee.RelationshipID == null ? "" : Nominee.RelationshipEntity.Name) + (Nominee.Relationship == null ? "" : ", " + Nominee.Relationship + "(Oth.)") + "\n";
                            temp += "%Age Of Share   : " + Nominee.NominationPerc + " %" + "\n";
                            temp += "DOB of Nominee : " + (Nominee.BirthDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", Nominee.BirthDate));
                            cell = new PdfPCell(new Paragraph(temp, FontV7));
                            NomineeDetails.AddCell(cell);
                        }
                    }
                    cell = new PdfPCell(NomineeDetails);
                    cell.Colspan = 3;
                    Table.AddCell(cell);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "NominationReport.pdf", HeadingTable);
        }
        // GET: /Generate Report For Share Register
        public FileStreamResult ShareRegister(Guid id)
        {
            int count = 0;
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            SocietyBuildingUnit SocietyBuildingUnit;
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDList;
            IEnumerable<SocietyMember> SocietyMembersList;
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadingTable = new PdfPTable(10);
            float[] widths = new float[] { 1.7f, 3.5f, 6.5f, 3.5f, 3.5f, 3f, 3f, 2.5f, 2.5f, 2.5f };
            HeadingTable.SetWidths(widths);
            cell = new PdfPCell(_service.CaptionTable("Share Register", FontV8Bold, System.Drawing.Color.LightGray));
            cell.Colspan = 10;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            HeadingTable.AddCell(cell);
            string[] tempList = new string[] { "Sr.No" + "\n\n\n\n1", "Date Of Issue" + "\n\n\n2", "Name Of Share Holder" + "\n\n\n\n3", "Distictive Numbers Of Share Held" + "\n4", "Date Of Payment On Allotment" + "\n\n5", "Amount Paid On Allotment" + "\n\n6", "Share No. Transfered Retruned/ Forfeited" + "\n7", "Total Nominal Value Of Shares" + "\n8", "Serial No. In Share Transfer" + "\n9", "Register Number Of Member" + "\n10" };
            for (int i = 0; i < tempList.Length; i++)
            {
                cell = new PdfPCell(new Phrase(new Chunk(tempList[i], FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                HeadingTable.AddCell(cell);
            }
            try
            {
                Table = new PdfPTable(10);
                Table.SetWidths(widths);

                BuildingUnitWithIDList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDList)
                {

                    SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(BuildingUnitWithID.SocietyBuildingUnitID);
                    SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var SocietyMember in SocietyMembersList)
                    {
                        count += 1;
                        cell = new PdfPCell(new Paragraph(count + ".", FontV7));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.IssueDate == null ? " - - " : String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate)), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(SocietyMember.Member, FontV7));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.DistinctiveFrom == null ? " - " : SocietyBuildingUnit.DistinctiveFrom + "  To  ") + (SocietyBuildingUnit.DistinctiveTo == null ? " - " : SocietyBuildingUnit.DistinctiveTo.ToString()), FontV7));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.PayDate == null ? " - - " : String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.PayDate)), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.AmountAtAllotment == null ? " - - " : SocietyBuildingUnit.AmountAtAllotment.ToString()), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("", FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.Value == null ? " - - " : SocietyBuildingUnit.Value.ToString()), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("", FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyMember.FolioNo == null ? " - - " : SocietyMember.FolioNo.ToString()), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        break;      // +ed by Baji on 24/6/13 to show only latest Member Name
                    }
                }

                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "ShareRegisterReport.pdf", HeadingTable);
        }
        // GET: /Generate Report For Share Ledger
        public FileStreamResult ShareLedgerReport(Guid id)
        {
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            SocietyBuildingUnit SocietyBuildingUnit;
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
            IEnumerable<SocietyMember> SocietyMembersList;
            IList<SocietyBuildingUnitTransfer> SocietyBuildingUnitTransferList;
            PdfPTable tempTable;
            int pageCount = 0;
            document.Open();
            string[] thList;
            string[] tdList;
            try
            {
                foreach (var BuildingUnitWithID in BuildingUnitWithIDList)
                {
                    SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(BuildingUnitWithID.SocietyBuildingUnitID);
                    SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var SocietyMember in SocietyMembersList)
                    {
                        //                        SocietyBuildingUnitTransferList = (IList<SocietyBuildingUnitTransfer>)new SocietyBuildingUnitTransferService(this.ModelState).ListByParentId(BuildingUnitWithID.SocietyBuildingUnitID);
                        SocietyBuildingUnitTransferList = (IList<SocietyBuildingUnitTransfer>)new SocietyBuildingUnitTransferService(this.ModelState).ListBySocietyBuildingUnitIDSocietyMemberID(BuildingUnitWithID.SocietyBuildingUnitID, SocietyMember.SocietyMemberID);
                        pageCount = pageCount + 1;
                        document.NewPage();
                        //for footer
                        PdfPTable footerTbl = new PdfPTable(2);
                        footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                        footerTbl.TotalWidth = document.PageSize.Width;
                        cell = new PdfPCell(new Paragraph("", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.GRAY)));
                        cell.Border = 0;
                        cell.PaddingLeft = 9;
                        footerTbl.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(pageCount.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.GRAY)));
                        cell.Border = 0;
                        cell.PaddingLeft = 9;
                        footerTbl.AddCell(cell);
                        footerTbl.WriteSelectedRows(0, -1, 0, (document.BottomMargin + 9), writer.DirectContent);
                        //
                        Table = new PdfPTable(1);
                        Table.TotalWidth = document.PageSize.Width;
                        cell = new PdfPCell(_service.SocietyHeaderTable(Society));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("SHARE LEDGER", FontV8Bold));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        document.Add(Table);
                        Table = new PdfPTable(1);
                        Table.TotalWidth = document.PageSize.Width;
                        cell = new PdfPCell(new Paragraph("1. ACCOUNT NO: " + BuildingUnitWithID.BuildingUnit, FontV8Bold));
                        cell.BorderWidthBottom = 0;
                        cell.BorderWidthTop = 0;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("2. NAME OF THE SHARE HOLDER: ", FontV8Bold));
                        cell.BorderWidthBottom = 0;
                        cell.BorderWidthTop = 0;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph("    " + SocietyMember.Member, FontV8Bold));
                        cell.BorderWidthBottom = 0;
                        cell.BorderWidthTop = 0;
                        Table.AddCell(cell);
                        for (int i = 0; i < 2; i++)
                        {
                            cell = new PdfPCell(new Paragraph("  "));
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            Table.AddCell(cell);
                        }
                        foreach (var transfer in SocietyBuildingUnitTransferList)
                        {
                            //if (transfer.uEndDate == null)
                            //{
                            thList = new string[] { "DATE" + "\n\n3", "J.B.F.NO." + "\n\n4", "NO.OF SHARES" + "\n\n5", "DISTICTIVE NUMBERS" + "\n6", "VALUE Rs." + "\n\n7" };
                            //tdList = new string[] { (SocietyBuildingUnit.IssueDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate)), "", (SocietyBuildingUnit.NoOfShares == null ? "" : SocietyBuildingUnit.NoOfShares.ToString()), (SocietyBuildingUnit.DistinctiveFrom == null ? "" : SocietyBuildingUnit.DistinctiveFrom.ToString()) + (SocietyBuildingUnit.DistinctiveTo == null ? " " : " To " + SocietyBuildingUnit.DistinctiveTo.ToString()), (SocietyBuildingUnit.Value == null ? "" : SocietyBuildingUnit.Value.ToString()) };
                            tdList = new string[] { (transfer.TransferDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", transfer.TransferDate)), "", (SocietyBuildingUnit.NoOfShares == null ? "" : SocietyBuildingUnit.NoOfShares.ToString()), (SocietyBuildingUnit.DistinctiveFrom == null ? "" : SocietyBuildingUnit.DistinctiveFrom.ToString()) + (SocietyBuildingUnit.DistinctiveTo == null ? " " : " To " + SocietyBuildingUnit.DistinctiveTo.ToString()), (SocietyBuildingUnit.Value == null ? "" : SocietyBuildingUnit.Value.ToString()) };
                            tempTable = new PdfPTable(5);
                            tempTable.TotalWidth = PageWidth - 40;
                            //tempTable.SetWidthPercentage(new float[] { 8F, 8F, 8F, 8F, 8F }, rect);                               
                            cell = new PdfPCell(new Paragraph("SHARES AQUIRED", FontV8Bold));
                            cell.Colspan = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            tempTable.AddCell(cell);
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                                cell.HorizontalAlignment = 1;
                                tempTable.AddCell(cell);
                            }
                            for (int i = 0; i < tdList.Length; i++)
                            {
                                cell = new PdfPCell(new Paragraph(tdList[i], FontV8));
                                cell.HorizontalAlignment = (i == 4 ? 2 : 1);//0=Left, 1=Centre, 2=Right
                                tempTable.AddCell(cell);
                            }
                            cell = new PdfPCell(tempTable);
                            cell.HorizontalAlignment = 1;
                            cell.BorderWidthBottom = 0;
                            cell.BorderWidthTop = 0;
                            Table.AddCell(cell);
                            for (int i = 0; i < 2; i++)
                            {
                                cell = new PdfPCell(new Paragraph("  "));
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                Table.AddCell(cell);
                            }
                            //}
                            //else
                            if (transfer.uEndDate != null)
                            {
                                thList = new string[] { "DATE" + "\n\n8", "J.B.F.NO." + "\n\n9", "NO.OF SHARES" + "\n10", "DISTICTIVE NUMBERS" + "\n11", "VALUE Rs." + "\n\n12", "NO.OF SHARES " + "\n13", "VALUE Rs." + "\n\n14" };
                                //tdList = new string[] { (SocietyBuildingUnit.IssueDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate)), "", (SocietyBuildingUnit.NoOfShares == null ? "" : SocietyBuildingUnit.NoOfShares.ToString()), (SocietyBuildingUnit.DistinctiveFrom == null ? "" : SocietyBuildingUnit.DistinctiveFrom.ToString()) + (SocietyBuildingUnit.DistinctiveTo == null ? " " : " To " + SocietyBuildingUnit.DistinctiveTo.ToString()), (SocietyBuildingUnit.Value == null ? "" : SocietyBuildingUnit.Value.ToString()), "", "" };
                                tdList = new string[] { String.Format("{0:dd-MMM-yyyy}", transfer.uEndDate), "", (SocietyBuildingUnit.NoOfShares == null ? "" : SocietyBuildingUnit.NoOfShares.ToString()), (SocietyBuildingUnit.DistinctiveFrom == null ? "" : SocietyBuildingUnit.DistinctiveFrom.ToString()) + (SocietyBuildingUnit.DistinctiveTo == null ? " " : " To " + SocietyBuildingUnit.DistinctiveTo.ToString()), (SocietyBuildingUnit.Value == null ? "" : SocietyBuildingUnit.Value.ToString()), "", "" };
                                tempTable = new PdfPTable(7);
                                tempTable.TotalWidth = PageWidth - 40;
                                //tempTable.SetWidthPercentage(new float[] { 8F, 8F, 8F, 8F, 8F, 8F, 8F }, rect);
                                cell = new PdfPCell(new Paragraph("SHARE TRANSFERRED OR WITHDRAWN", FontV8Bold));
                                cell.Colspan = 5;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                tempTable.AddCell(cell);
                                cell = new PdfPCell(new Paragraph("BALANCE", FontV8Bold));
                                cell.Colspan = 2;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                tempTable.AddCell(cell);
                                for (int i = 0; i < thList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(thList[i], FontV8Bold));
                                    cell.HorizontalAlignment = 1;
                                    tempTable.AddCell(cell);
                                }
                                for (int i = 0; i < tdList.Length; i++)
                                {
                                    cell = new PdfPCell(new Paragraph(tdList[i], FontV8));
                                    cell.HorizontalAlignment = (i == 4 || i == 6 ? 2 : 1);//0=Left, 1=Centre, 2=Right
                                    tempTable.AddCell(cell);
                                }
                                cell = new PdfPCell(tempTable);
                                cell.HorizontalAlignment = 1;
                                cell.BorderWidthBottom = 0;
                                cell.BorderWidthTop = 0;
                                Table.AddCell(cell);
                            }
                        }
                        cell = new PdfPCell(new Paragraph("  "));
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        Table.AddCell(cell);

                        cell = new PdfPCell(new Paragraph("Remarks: ", FontV8Bold));
                        cell.BorderWidthTop = 0;
                        cell.BorderWidthBottom = 0;
                        Table.AddCell(cell);
                        for (int i = 0; i < 3; i++)
                        {
                            cell = new PdfPCell(new Paragraph("  "));
                            cell.BorderWidthTop = 0;
                            //cell.BorderWidthBottom = (i == 2 ? 1 : 0);
                            if (i != 2) cell.BorderWidthBottom = 0;
                            Table.AddCell(cell);
                        }
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
            return this.FileStreamResult(ms, "ShareLedgerReport.pdf");
        }
        //GET: /Generate Report For Share Transfer Report
        public FileStreamResult ShareTransferReport(Guid id)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            SocietyBuildingUnit SocietyBuildingUnit;
            IEnumerable<BuildingUnitWithID> BuildingUnitWithIDList;
            IEnumerable<SocietyMember> SocietyMembersList;
            IEnumerable<SocietyBuildingUnitTransfer> BuildingUnitTransferList;
            List<PdfPTable> TableList = new List<PdfPTable>();
            PdfPTable HeadingTable = new PdfPTable(7);
            //float[] widths = new float[] { 2.7f, 3.3f, 2.7f, 2.7f, 2.5f, 4f, 3.3f, 5f, 2.5f, 3f };
            //HeadingTable.SetWidths(widths);
            float[] widths = new float[] { 45f, 50f, 45f, 45f, 45f, 60f, 50f + 95f + 45f + 60f };
            HeadingTable.SetWidthPercentage(widths, rect);
            cell = new PdfPCell(_service.CaptionTable("Share Transfer Register", FontV8Bold, System.Drawing.Color.LightGray));
            cell.Colspan = 10;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            HeadingTable.AddCell(cell);
            string[] thList = new string[] { "Certificate No", "Issue On Date", "Distinctive" + "\nFrom", "Distinctive" + "\nTo", "No Of Shares", "Unit", "Transfered Details" };
            string[] innerthList = new string[] { "Transfered Date", "Transfered To Member", "Transfer Fee", "Payment Details" };
            PdfPTable InnerTable = new PdfPTable(4);
            InnerTable.SetWidthPercentage(new float[] { 50f, 95f, 45f, 60f }, rect);
            cell = new PdfPCell(new Phrase(new Chunk("Transfered Details", FontV7Bold)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 4;
            InnerTable.AddCell(cell);
            for (int i = 0; i < innerthList.Length; i++)
            {
                cell = new PdfPCell(new Phrase(new Chunk(innerthList[i], FontV7Bold)));
                cell.HorizontalAlignment = i == 2 ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                InnerTable.AddCell(cell);
            }
            for (int i = 0; i < thList.Length; i++)
            {
                cell = i == 6 ? new PdfPCell(InnerTable) : new PdfPCell(new Phrase(new Chunk(thList[i], FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                HeadingTable.AddCell(cell);
            }
            try
            {
                Table = new PdfPTable(7);
                Table.SetWidthPercentage(widths, rect);
                BuildingUnitWithIDList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(SocietyID);
                foreach (var BuildingUnitWithID in BuildingUnitWithIDList)
                {
                    SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(BuildingUnitWithID.SocietyBuildingUnitID);
                    SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(BuildingUnitWithID.SocietyBuildingUnitID);
                    //BuildingUnitTransferList = new SocietyBuildingUnitTransferService(this.ModelState).ListByParentId(BuildingUnitWithID.SocietyBuildingUnitID);
                    foreach (var SocietyMember in SocietyMembersList)
                    {
                        cell = new PdfPCell(new Paragraph(SocietyBuildingUnit.CertificateNo.ToString(), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.IssueDate == null ? " - - " : String.Format("{0:dd-MMM-yyyy}", SocietyBuildingUnit.IssueDate)), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.DistinctiveFrom == null ? " - - " : SocietyBuildingUnit.DistinctiveFrom.ToString()), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.DistinctiveTo == null ? " - - " : SocietyBuildingUnit.DistinctiveTo.ToString()), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph((SocietyBuildingUnit.NoOfShares == null ? " - - " : SocietyBuildingUnit.NoOfShares.ToString()), FontV7));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Paragraph(BuildingUnitWithID.BuildingUnit, FontV7));
                        Table.AddCell(cell);
                        InnerTable = new PdfPTable(4);
                        InnerTable.SetWidthPercentage(new float[] { 50f, 95f, 45f, 60f }, rect);
                        BuildingUnitTransferList = new SocietyBuildingUnitTransferService(this.ModelState).ListByParentId(BuildingUnitWithID.SocietyBuildingUnitID);
                        foreach (var BuildingUnitTransfer in BuildingUnitTransferList)
                        {
                            cell = new PdfPCell(new Paragraph(String.Format("{0:dd-MMM-yyyy}", BuildingUnitTransfer.TransferDate), FontV7));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            InnerTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(BuildingUnitTransfer.SocietyMember.Member, FontV7));
                            InnerTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph((BuildingUnitTransfer.TransferFee == null ? " - - " : BuildingUnitTransfer.TransferFee.ToString()), FontV7));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            InnerTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(BuildingUnitTransfer.PaymentDetails, FontV7));
                            InnerTable.AddCell(cell);
                        }
                        cell = new PdfPCell(InnerTable);
                        Table.AddCell(cell);
                    }
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "ShareTransferRegister.pdf", HeadingTable);
        }
        public ActionResult MemberLedgerReport(Guid id) // id = SocietySubscriptionID
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
            ViewBag.SocietyBuildingList = new SocietyBuildingService(this.ModelState).ListByParentId(societySubscriptionService.GetById(id).SocietyID);
            //code By Nityananda 11 Mar 2013 start
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societySubscription.SubscriptionStart;
            ViewBag.ToDate = societySubscription.PaidTillDate;
            //code By Nityananda 11 Mar 2013 end
            return View();
        }
        [HttpPost]
        public ActionResult MemberLedgerReport(Guid id, Guid? SocietyBuildingID, DateTime FromDate, DateTime ToDate, Boolean IsDetails) // id = SocietySubscriptionID
        {
            IEnumerable<MemberLedger> MemberLedgerList;
            if (SocietyBuildingID != null)
                //                MemberLedgerList = new SocietyBuildingUnitService(this.ModelState).ListLedgerBySocietySubscriptionID(id).Where(ml => ml.BillDate >= FromDate && ml.BillDate <= ToDate).Where(ml => ml.SocietyBuildingID == SocietyBuildingID);
                MemberLedgerList = new SocietyBuildingUnitService(this.ModelState).ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(id, (Guid)SocietyBuildingID, FromDate, ToDate);
            else
                //                MemberLedgerList = new SocietyBuildingUnitService(this.ModelState).ListLedgerBySocietySubscriptionID(id).Where(ml => ml.BillDate >= FromDate && ml.BillDate <= ToDate);
                MemberLedgerList = new SocietyBuildingUnitService(this.ModelState).ListLedgerForPeriodBySocietySubscriptionID(id, FromDate, ToDate);
            if (MemberLedgerList.Count() > 0)
            {
                if (IsDetails)
                    return PDFMemberLedgerDetailsReport(id, MemberLedgerList, FromDate, ToDate);
                else
                    return PDFMemberLedgerReport(id, MemberLedgerList, FromDate, ToDate);
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
                ViewBag.SocietyBuildingList = new SocietyBuildingService(this.ModelState).ListByParentId(societySubscriptionService.GetById(id).SocietyID);
                return View();
            }
        }
        public FileStreamResult PDFMemberLedgerDetailsReport(Guid id, IEnumerable<MemberLedger> MemberLedgerList, DateTime? FromDate, DateTime? ToDate)
        {
            SocietySubscription SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Society Society = new SocietyService(this.ModelState).GetById(SocietySubscription.SocietyID);
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            PdfPTable RecAmountTable;
            PdfPTable BillAmountTable;
            PdfPTable BalanceTable;
            PdfPTable TempTable;
            document.Open();
            decimal[] TotalBillAmoutTdList = new decimal[] { 0, 0, 0, 0, 0 };
            decimal[] TotalRecAmoutTdList = new decimal[] { 0, 0, 0, 0, 0 };
            decimal[] TotalBalanceTdList = new decimal[] { 0, 0, 0, 0, 0 };
            string[] thList;
            string[] tdList;
            decimal[] RecAmoutTdList;
            decimal[] BillAmoutTdList;
            //decimal[] BalanceTdList;
            //            bool IsFirst = true;
            int index = 0;
            bool IsBill = false;
            //            PdfPTable FooterTable;
            string BuildingUnit = "", Member = "";
            try
            {
                if (MemberLedgerList.Count() > 0)
                {
                    foreach (var MemberLedger in MemberLedgerList)
                    {
                        IsBill = MemberLedger.BillNo != "" ? MemberLedger.BillNo.Substring(0, 1) == "B" || MemberLedger.BillNo.Substring(0, 1) == "V" : false;
                        Table = new PdfPTable(1);
                        Table.SetWidthPercentage(new float[] { 665f }, rect);
                        TempTable = new PdfPTable(6);
                        TempTable.SetWidthPercentage(new float[] { 45f, 45f, 90f, 145f, 170f, 180f }, rect);
                        index += 1;
                        //                        if (BuildingUnit != MemberLedger.BuildingUnit && Member != MemberLedger.Member)
                        if (BuildingUnit != MemberLedger.BuildingUnit || Member != MemberLedger.Member)
                        {
                            // below if block added by Baji on 12-Sep-12 to print total of previous unit/memmber
                            //if (!IsFirst)
                            //{
                            //    RecAmountTable = new PdfPTable(5);
                            //    for (int i = 0; i < TotalRecAmoutTdList.Length; i++)
                            //    {
                            //        cell = new PdfPCell(new Phrase(new Chunk(TotalRecAmoutTdList[i] + "", FontV7Bold)));
                            //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //        RecAmountTable.AddCell(cell);
                            //    }
                            //    BillAmountTable = new PdfPTable(4);
                            //    for (int i = 0; i < TotalBillAmoutTdList.Length - 1; i++)
                            //    {
                            //        cell = new PdfPCell(new Phrase(new Chunk(TotalBillAmoutTdList[i] + "", FontV7Bold)));
                            //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //        BillAmountTable.AddCell(cell);
                            //    }
                            //    //for footer
                            //    FooterTable = new PdfPTable(2);
                            //    FooterTable.HorizontalAlignment = Element.ALIGN_CENTER;
                            //    FooterTable.TotalWidth = document.PageSize.Width;
                            //    cell = new PdfPCell(new Paragraph("Page " + writer.PageNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                            //    cell.Border = 0;
                            //    cell.PaddingLeft = 0;
                            //    cell.PaddingRight = 0;
                            //    FooterTable.AddCell(cell);
                            //    cell = new PdfPCell(new Paragraph("Printed On " + String.Format("{0:dd-MMM-yyyy, HH:mm:ss}", DateTime.Now), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                            //    cell.Border = 0;
                            //    cell.PaddingLeft = 90;
                            //    FooterTable.AddCell(cell);
                            //    FooterTable.WriteSelectedRows(0, -1, 50, (document.BottomMargin + 9), writer.DirectContent);
                            //    // 
                            //    cell = new PdfPCell(new Phrase(new Chunk("Member Unit Total :", FontV7Bold)));
                            //    cell.Colspan = 3;
                            //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //    TempTable.AddCell(cell);
                            //    cell = new PdfPCell(BillAmountTable);
                            //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //    TempTable.AddCell(cell);
                            //    cell = new PdfPCell(RecAmountTable);
                            //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //    TempTable.AddCell(cell);
                            //    cell = new PdfPCell(new Paragraph(""));
                            //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //    TempTable.AddCell(cell);

                            //    cell = new PdfPCell(TempTable);
                            //    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            //    Table.AddCell(cell);
                            //    document.Add(Table);
                            //}
                            //IsFirst = false;

                            index = 0;
                            TotalBillAmoutTdList = new decimal[] { 0, 0, 0, 0, 0 };
                            TotalRecAmoutTdList = new decimal[] { 0, 0, 0, 0, 0 };
                            TotalBalanceTdList = new decimal[] { 0, 0, 0, 0, 0 };
                            document.NewPage();
                            Table.AddCell(_service.SocietyHeaderTable(Society));
                            if (FromDate == null || ToDate == null)
                                contain = "MEMBER'S LEDGER AS ON " + String.Format("{0:dd-MMM-yyyy}", (SocietySubscription.PaidTillDate < DateTime.Now ? SocietySubscription.PaidTillDate : DateTime.Now));
                            else
                                contain = "MEMBER'S LEDGER FROM " + String.Format("{0:MMM d, yyyy}", FromDate) + " TO " + String.Format("{0:MMM d, yyyy}", ToDate);
                            cell = new PdfPCell(new Paragraph(contain.ToUpper(), FontV8Bold));
                            cell.PaddingBottom = 5f;
                            cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            Table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph("Unit No : " + MemberLedger.BuildingUnit + "           Member : " + MemberLedger.Member, FontV8Bold));
                            cell.BorderWidthBottom = 0;
                            cell.BorderWidthTop = 0;
                            cell.PaddingBottom = 15f;
                            cell.PaddingTop = 15f;
                            Table.AddCell(cell);
                            thList = new string[] { "Chg", "NonChg", "Int", "Tax", "Adv" };
                            RecAmountTable = new PdfPTable(5);
                            BillAmountTable = new PdfPTable(4);
                            BalanceTable = new PdfPTable(5);
                            cell = new PdfPCell(new Phrase(new Chunk("Receipt Amount", FontV7Bold)));
                            cell.Colspan = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            RecAmountTable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("Bill Amount", FontV7Bold)));
                            cell.Colspan = 4;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            BillAmountTable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk("Balance Amount", FontV7Bold)));
                            cell.Colspan = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            BalanceTable.AddCell(cell);
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV7)));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                RecAmountTable.AddCell(cell);
                                if (i < 4)
                                    BillAmountTable.AddCell(cell);
                                BalanceTable.AddCell(cell);
                            }
                            thList = new string[] { "DocNo", "DocDate", "Particulars", "Bill", "Rec", "Balance" };
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = i == 3 ? new PdfPCell(BillAmountTable) : (i == 4 ? new PdfPCell(RecAmountTable) : (i == 5 ? new PdfPCell(BalanceTable) : new PdfPCell(new Phrase(new Chunk(thList[i], FontV7)))));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                TempTable.AddCell(cell);
                            }
                        }
                        tdList = new string[] { MemberLedger.BillNo, (MemberLedger.BillDate != null ? String.Format("{0:dd-MMM-yy}", MemberLedger.BillDate) : ""), MemberLedger.Particulars, "BillAmount", "RecAmount", " Balance" };
                        BillAmoutTdList = IsBill ? new decimal[] { MemberLedger.ChgAmt ?? 0, MemberLedger.NonChgAmt ?? 0, MemberLedger.IntAmt ?? 0, MemberLedger.TaxAmt ?? 0, 0 } : new decimal[] { 0, 0, 0, 0, 0 };
                        //                        RecAmoutTdList = (!IsBill && MemberLedger.BillNo != "") ? new decimal[] { MemberLedger.ChgAmt ?? 0, MemberLedger.NonChgAmt ?? 0, MemberLedger.IntAmt ?? 0, MemberLedger.TaxAmt ?? 0, MemberLedger.Advance ?? 0 } : new decimal[] { 0, 0, 0, 0, 0 };
                        RecAmoutTdList = (!IsBill && MemberLedger.BillNo != "") ? new decimal[] { MemberLedger.ChgAmt ?? 0, MemberLedger.NonChgAmt ?? 0, MemberLedger.IntAmt ?? 0, MemberLedger.TaxAmt ?? 0, MemberLedger.Advance ?? 0 } : new decimal[] { 0, 0, 0, 0, MemberLedger.Advance ?? 0 }; // advance added here for CollectionReversal
                        //BalanceTdList = new decimal[] { MemberLedger.ChgBal ?? 0, MemberLedger.NonChgBal ?? 0, MemberLedger.IntBal ?? 0, MemberLedger.TaxBal ?? 0, MemberLedger.AdvBal ?? 0 };
                        TotalBalanceTdList[0] += MemberLedger.ChgBal ?? 0;
                        TotalBalanceTdList[1] += MemberLedger.NonChgBal ?? 0;
                        TotalBalanceTdList[2] += MemberLedger.IntBal ?? 0;
                        TotalBalanceTdList[3] += MemberLedger.TaxBal ?? 0;
                        TotalBalanceTdList[4] -= MemberLedger.AdvBal ?? 0;
                        BillAmountTable = new PdfPTable(4);
                        for (int i = 0; i < BillAmoutTdList.Length - 1; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk((BillAmoutTdList[i] != 0 ? BillAmoutTdList[i] + "" : "--"), FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            BillAmountTable.AddCell(cell);
                            TotalBillAmoutTdList[i] += BillAmoutTdList[i];
                        }
                        RecAmountTable = new PdfPTable(5);
                        for (int i = 0; i < RecAmoutTdList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk((RecAmoutTdList[i] != 0 ? RecAmoutTdList[i] + "" : "--"), FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            RecAmountTable.AddCell(cell);
                            TotalRecAmoutTdList[i] += RecAmoutTdList[i];
                        }
                        BalanceTable = new PdfPTable(5);
                        for (int i = 0; i < TotalBalanceTdList.Length; i++)
                        {
                            //decimal totalBalance = BalanceTdList[i] + BillAmoutTdList[i] + RecAmoutTdList[i];
                            //if (i < 4)
                            //    totalBalance += BillAmoutTdList[i];
                            cell = new PdfPCell(new Phrase(new Chunk(TotalBalanceTdList[i] + "", FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            BalanceTable.AddCell(cell);
                        }
                        for (int i = 0; i < tdList.Length; i++)
                        {
                            cell = i == 3 ? new PdfPCell(BillAmountTable) : (i == 4 ? new PdfPCell(RecAmountTable) : (i == 5 ? new PdfPCell(BalanceTable) : new PdfPCell(new Phrase(new Chunk(tdList[i], FontV7)))));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            TempTable.AddCell(cell);
                        }

                        BuildingUnit = MemberLedger.BuildingUnit;
                        Member = MemberLedger.Member;
                        IEnumerable<MemberLedger> MemberList = MemberLedgerList.Where(mll => mll.BuildingUnit == BuildingUnit && mll.Member == Member);
                        //if (BuildingUnit == MemberLedger.BuildingUnit && Member == MemberLedger.Member )
                        if (index == (MemberList.Count() - 1))
                        {
                            RecAmountTable = new PdfPTable(5);
                            for (int i = 0; i < TotalRecAmoutTdList.Length; i++)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(TotalRecAmoutTdList[i] + "", FontV7Bold)));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                RecAmountTable.AddCell(cell);
                            }
                            BillAmountTable = new PdfPTable(4);
                            for (int i = 0; i < TotalBillAmoutTdList.Length - 1; i++)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(TotalBillAmoutTdList[i] + "", FontV7Bold)));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                BillAmountTable.AddCell(cell);
                            }
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
                            cell = new PdfPCell(new Phrase(new Chunk("Member Total :", FontV7Bold)));
                            cell.Colspan = 3;
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(BillAmountTable);
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(RecAmountTable);
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(new Paragraph(""));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                        }

                        cell = new PdfPCell(TempTable);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        Table.AddCell(cell);
                        document.Add(Table);
                    }   // end of For Each ledger line loop
                    // Total for Last Member/Unit
                    //TempTable = new PdfPTable(6);
                    //TempTable.SetWidthPercentage(new float[] { 45f, 45f, 90f, 145f, 170f, 170f }, rect);

                    //RecAmountTable = new PdfPTable(5);
                    //for (int i = 0; i < TotalRecAmoutTdList.Length; i++)
                    //{
                    //    cell = new PdfPCell(new Phrase(new Chunk(TotalRecAmoutTdList[i] + "", FontV7Bold)));
                    //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    RecAmountTable.AddCell(cell);
                    //}
                    //BillAmountTable = new PdfPTable(4);
                    //for (int i = 0; i < TotalBillAmoutTdList.Length - 1; i++)
                    //{
                    //    cell = new PdfPCell(new Phrase(new Chunk(TotalBillAmoutTdList[i] + "", FontV7Bold)));
                    //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    BillAmountTable.AddCell(cell);
                    //}
                    ////for footer
                    //FooterTable = new PdfPTable(2);
                    //FooterTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    //FooterTable.TotalWidth = document.PageSize.Width;
                    //cell = new PdfPCell(new Paragraph("Page " + writer.PageNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                    //cell.Border = 0;
                    //cell.PaddingLeft = 0;
                    //cell.PaddingRight = 0;
                    //FooterTable.AddCell(cell);
                    //cell = new PdfPCell(new Paragraph("Printed On " + String.Format("{0:dd-MMM-yyyy, HH:mm:ss}", DateTime.Now), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.GRAY)));
                    //cell.Border = 0;
                    //cell.PaddingLeft = 90;
                    //FooterTable.AddCell(cell);
                    //FooterTable.WriteSelectedRows(0, -1, 50, (document.BottomMargin + 9), writer.DirectContent);
                    //// 
                    //cell = new PdfPCell(new Phrase(new Chunk("Member Total :", FontV7Bold)));
                    //cell.Colspan = 3;
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //TempTable.AddCell(cell);
                    //cell = new PdfPCell(BillAmountTable);
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //TempTable.AddCell(cell);
                    //cell = new PdfPCell(RecAmountTable);
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //TempTable.AddCell(cell);
                    //cell = new PdfPCell(new Paragraph(""));
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //TempTable.AddCell(cell);

                    //cell = new PdfPCell(TempTable);
                    //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //Table.AddCell(cell);
                    //document.Add(Table);

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
            return this.FileStreamResult(ms, "MemberLedgerDetailsReport.pdf");
        }

        public FileStreamResult PDFMemberLedgerReport(Guid id, IEnumerable<MemberLedger> MemberLedgerList, DateTime? FromDate, DateTime? ToDate)
        {
            SocietySubscription SocietySubscription = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id);
            Society Society = new SocietyService(this.ModelState).GetById(SocietySubscription.SocietyID);
            MemoryStream ms = new MemoryStream();
            Rectangle rect = PageSize.A4;
            Document document = new Document(rect);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            PdfPTable TempTable;
            document.Open();
            decimal TotalBillAmountTd = 0;
            decimal TotalRecAmountTd = 0;
            string[] thList;
            string[] tdList;
            decimal RecAmoutTd;
            decimal BillAmoutTd;
            decimal BalanceTd = 0;
            int index = 0;
            bool IsBill = false;
            string BuildingUnit = "", Member = "";
            try
            {
                if (MemberLedgerList.Count() > 0)
                {
                    foreach (var MemberLedger in MemberLedgerList)
                    {
                        IsBill = MemberLedger.BillNo != "" ? MemberLedger.BillNo.Substring(0, 1) == "B" || MemberLedger.BillNo.Substring(0, 1) == "V" : false;
                        Table = new PdfPTable(1);
                        Table.SetWidthPercentage(new float[] { 600f }, rect);
                        TempTable = new PdfPTable(6);
                        TempTable.SetWidthPercentage(new float[] { 85f, 50f, 230f, 70f, 85f, 80f }, rect);
                        index += 1;
                        //                        if (BuildingUnit != MemberLedger.BuildingUnit && Member != MemberLedger.Member)
                        if (BuildingUnit != MemberLedger.BuildingUnit || Member != MemberLedger.Member)
                        {
                            index = 0;
                            TotalBillAmountTd = 0;
                            TotalRecAmountTd = 0;
                            BalanceTd = 0;
                            document.NewPage();
                            Table.AddCell(_service.SocietyHeaderTable(Society));
                            if (FromDate == null || ToDate == null)
                                contain = "MEMBER'S LEDGER AS ON " + String.Format("{0:dd-MMM-yyyy}", (SocietySubscription.PaidTillDate < DateTime.Now ? SocietySubscription.PaidTillDate : DateTime.Now));
                            else
                                contain = "MEMBER'S LEDGER " + String.Format("{0:MMM d, yyyy}", FromDate) + " TO " + String.Format("{0:MMM d, yyyy}", ToDate);
                            cell = new PdfPCell(new Paragraph(contain.ToUpper(), FontV9Bold));
                            cell.PaddingBottom = 5f;
                            cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            Table.AddCell(cell);
                            cell = new PdfPCell(new Paragraph("Unit No : " + MemberLedger.BuildingUnit + "           Member : " + MemberLedger.Member, FontV9Bold));
                            cell.BorderWidthBottom = 0;
                            cell.BorderWidthTop = 0;
                            cell.PaddingBottom = 15f;
                            cell.PaddingTop = 15f;
                            Table.AddCell(cell);
                            thList = new string[] { "DocNo", "DocDate", "Particulars", "Bill Amount(Dr)", "Receipt Amount(Cr)", "Balance Amount" };
                            for (int i = 0; i < thList.Length; i++)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8)));
                                cell.HorizontalAlignment = (i == 3 || i == 4 || i == 5) ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                                TempTable.AddCell(cell);
                            }
                        }
                        BillAmoutTd = 0;
                        RecAmoutTd = 0;
                        //                        BillAmoutTd = IsBill ? ((MemberLedger.ChgAmt ?? 0) + (MemberLedger.NonChgAmt ?? 0) + (MemberLedger.IntAmt ?? 0) + (MemberLedger.TaxAmt ?? 0)) : 0;
                        BillAmoutTd = IsBill ? ((MemberLedger.ChgAmt ?? 0) + (MemberLedger.NonChgAmt ?? 0) + (MemberLedger.IntAmt ?? 0) + (MemberLedger.TaxAmt ?? 0) - (MemberLedger.Advance ?? 0)) : 0;
                        TotalBillAmountTd += BillAmoutTd;
                        RecAmoutTd = (!IsBill && MemberLedger.BillNo != "") ? ((MemberLedger.ChgAmt ?? 0) + (MemberLedger.NonChgAmt ?? 0) + (MemberLedger.IntAmt ?? 0) + (MemberLedger.TaxAmt ?? 0) + (MemberLedger.Advance ?? 0)) : 0;
                        TotalRecAmountTd += RecAmoutTd;
                        BalanceTd += ((MemberLedger.ChgBal ?? 0) + (MemberLedger.NonChgBal ?? 0) + (MemberLedger.IntBal ?? 0) + (MemberLedger.TaxBal ?? 0) - (MemberLedger.AdvBal ?? 0));
                        //tdList = new string[] { MemberLedger.BillNo, (MemberLedger.BillDate != null ? String.Format("{0:dd-MMM-yy}", MemberLedger.BillDate) : ""), MemberLedger.Particulars, (BillAmoutTd == 0 ? "-" : BillAmoutTd.ToString()), (RecAmoutTd == 0 ? "-" : RecAmoutTd.ToString()), (BalanceTd+BillAmoutTd+RecAmoutTd).ToString() };
                        tdList = new string[] { MemberLedger.BillNo, (MemberLedger.BillDate != null ? String.Format("{0:dd-MMM-yy}", MemberLedger.BillDate) : ""), MemberLedger.Particulars, (BillAmoutTd == 0 ? "-" : BillAmoutTd.ToString()), (RecAmoutTd == 0 ? "-" : RecAmoutTd.ToString()), BalanceTd < 0 ? Math.Abs(BalanceTd) + " Cr" : (BalanceTd == 0 ? BalanceTd + "" : BalanceTd + " Dr") };
                        for (int i = 0; i < tdList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(tdList[i], FontV8)));
                            cell.HorizontalAlignment = (i == 3 || i == 4 || i == 5) ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                            TempTable.AddCell(cell);
                        }

                        BuildingUnit = MemberLedger.BuildingUnit;
                        Member = MemberLedger.Member;
                        IEnumerable<MemberLedger> MemberList = MemberLedgerList.Where(mll => mll.BuildingUnit == BuildingUnit && mll.Member == Member);
                        if (index == (MemberList.Count() - 1))
                        {
                            cell = new PdfPCell(new Phrase(new Chunk("Member Total :", FontV8Bold)));
                            cell.Colspan = 3;
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(TotalBillAmountTd.ToString(), FontV8Bold)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            TempTable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk(TotalRecAmountTd.ToString(), FontV8Bold)));
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
            return this.FileStreamResult(ms, "MemberLedgerReport.pdf");
        }

        // GET: /Generate Member Own Ledger Report By Nityananda
        //public ActionResult MemberOwnLedgerReport(Guid id) // id = SocietyMemberID
        //{
        //    ViewBag.SocietyMemberID = id;
        //    ViewBag.NoRecords = false;
        //    ViewBag.ShowSocietyMemberMenu = true;
        //    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        //    var SocietySubscriptionID=new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(id).Society.SocietySubscriptions.FirstOrDefault().SocietySubscriptionID;
        //    var Societysubscription = societySubscriptionService.GetById(SocietySubscriptionID);
        //    ViewBag.SocietySubscriptionID = SocietySubscriptionID;
        //    ViewBag.StartDate = Societysubscription.SubscriptionStart;
        //    ViewBag.EndDate = Societysubscription.SubscriptionEnd;
        //    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
        //    //ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
        //   // ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
        //    //ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
        //    //ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
        //    return View();
        //}

        //// POST: /Generate Member Own Ledger Report By Nityananda
        //[HttpPost]
        //public ActionResult MemberOwnLedgerReport(Guid id, Guid SocietySubscriptionID, DateTime StartDate, DateTime EndDate, Boolean IsDetails) // id = SocietyMemberID
        //{
        //    IEnumerable<MemberLedger> MemberLedgerList;
        //    MemberLedgerList = new SocietyBuildingUnitService(this.ModelState).ListLedgerForPeriodBySocietySubscriptionID(SocietySubscriptionID, StartDate, EndDate).Where(s=>s.SocietyMemberID==id).OrderByDescending(c=>c.BillDate);
        //    if (MemberLedgerList.Count() > 0)
        //    {
        //        if (IsDetails)
        //            return PDFMemberLedgerDetailsReport(SocietySubscriptionID, MemberLedgerList, StartDate, EndDate);
        //        else
        //            return PDFMemberLedgerReport(SocietySubscriptionID, MemberLedgerList, StartDate, EndDate);
        //    }
        //    else
        //    {
        //        ViewBag.SocietyMemberID = id;
        //        ViewBag.SocietySubscriptionID = SocietySubscriptionID;
        //        ViewBag.NoRecords = true;
        //        var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        //        ViewBag.ShowSocietyMemberMenu = true;
        //        ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
        //        ViewBag.SocietySubscriptionID = SocietySubscriptionID;
        //        var Societysubscription = societySubscriptionService.GetById(SocietySubscriptionID);
        //        ViewBag.StartDate = Societysubscription.SubscriptionStart;
        //        ViewBag.EndDate = Societysubscription.SubscriptionEnd;
        //        //ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
        //        //ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
        //        //ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
        //        //ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
        //        return View();
        //    }
        //}

        public ActionResult MemberOwnLedgerReport(Guid id, bool IsToDownLoad = false) // id = SocietySubscriptionID
        {
            IEnumerable<MemberLedger> MemberLedgerList;
            MembershipUser user = Membership.GetUser();
            Guid userid = (Guid)user.ProviderUserKey;
            var societymemberid = (Guid)new Services.UserDetailService(this.ModelState).GetById(userid).SocietyMemberID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowMemberMenu = true;
            var societyMemberService = new CloudSociety.Services.SocietyMemberService(this.ModelState);
            var sm = societyMemberService.GetById(societymemberid);
            if (sm.SocietyBuildingUnitTransfers.Count > 0)
            {
                string unitNumber = sm.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + sm.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                ViewBag.MemberName = sm.Member + "(" + unitNumber + ")";
            }
            else
            {
                ViewBag.MemberName = sm.Member;
            }
            ViewBag.ShowFinalReports = societySubscription.Closed;
            ViewBag.ShowCommunication = societyMemberService.IsCommunicationEnabled(sm.SocietyID);
            MemberLedgerList = new SocietyMemberService(this.ModelState).ListLedgerBySocietySubscriptionIDSocietyMemberID(id, societymemberid);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.AsOn = String.Format("{0:dd-MMM-yyyy}", (societySubscription.PaidTillDate < DateTime.Now ? societySubscription.PaidTillDate : DateTime.Now));
            ViewBag.ShowPaymentLink = sm.Society.ShowPaymentLink;
            if (IsToDownLoad)
            {
                return PDFMemberLedgerReport(id, MemberLedgerList, null, null);
            }
            else
            {
                if (TempData["PaymentMessage"] != null)
                {
                    ViewBag.PaymentMessage = TempData["PaymentMessage"];
                }
                else
                {
                    ViewBag.PaymentMessage = "";
                }
                ViewBag.TempReceipt = new SocietyReceiptService(this.ModelState).GetOnholdReceipts(sm.SocietyID, id, sm.SocietyMemberID);
                ViewBag.SocietyBillSeriesList = (List<CloudSocietyEntities.SocietyBillSeries>)new CloudSociety.Services.SocietyBillSeriesService(this.ModelState).ListByParentId(sm.SocietyID);
                return View(MemberLedgerList);
            }

            //if (MemberLedgerList.Count() > 0)
            //{
            //    return PDFMemberLedgerReport(id, MemberLedgerList, null, null);
            //}
            //else
            //{
            //    ViewBag.NoRecords = true;
            //    return View();
            //}
        }

        //[HttpPost]
        //public ActionResult ProcessPayment(SocietyReceiptOnhold societyReceipt)
        //{
        //    if (null == societyReceipt)
        //    {
        //        return View("MemberOwnLedgerReport");
        //    }
        //    var societyMember = new SocietyMemberService(this.ModelState).GetById(societyReceipt.SocietyMemberID);
        //    if (null == societyMember)
        //    {
        //        return View("MemberOwnLedgerReport");
        //    }
        //    PaymentOrder objPaymentRequest = new PaymentOrder();
        //    //Required POST parameters
        //    objPaymentRequest.name = societyMember.Member;
        //    objPaymentRequest.email = societyMember.EmailId;
        //    objPaymentRequest.phone = societyMember.MobileNo;
        //    objPaymentRequest.amount = (double?)societyReceipt.Amount;
        //    objPaymentRequest.currency = "INR";
        //    objPaymentRequest.redirect_url = Request.Url.AbsoluteUri;
        //    var objPaymentResponse = new PaymentService().CreatePaymentOrder(objPaymentRequest);
        //    return Redirect(objPaymentResponse.payment_options.payment_url);
        //}

        [HttpPost]
        public JsonResult ProcessPayment(SocietyReceiptOnhold societyReceipt)
        {
            if (null == societyReceipt)
            {
                return Json(new { status = "false", redirectUrl = "" }, JsonRequestBehavior.AllowGet);
            }
            var societyMember = new SocietyMemberService(this.ModelState).GetById(societyReceipt.SocietyMemberID);
            if (null == societyMember)
            {
                return Json(new { status = "false", redirectUrl = "" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(societyMember.Society.PaymentGatewayLink))
            {
                return Json(new { status = "false", redirectUrl = "" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var ArrPaymentGatewayLink = societyMember.Society.PaymentGatewayLink.Split(',');
                if (ArrPaymentGatewayLink.Length != 2)
                {
                    return Json(new { status = "false", redirectUrl = "" }, JsonRequestBehavior.AllowGet);
                }
                string redirectURL;
                PaymentOrder objPaymentRequest = new PaymentOrder();
                //Required POST parameters
                objPaymentRequest.name = societyMember.Member;
                objPaymentRequest.email = System.Configuration.ConfigurationManager.AppSettings["BuyerEmailId"];
                objPaymentRequest.phone = System.Configuration.ConfigurationManager.AppSettings["BuyerPhone"];
                objPaymentRequest.amount = (double?)societyReceipt.Amount;
                objPaymentRequest.currency = "INR";
                objPaymentRequest.description = societyReceipt.BillAbbreviation.Trim() + " - Payment";
                //objPaymentRequest.send_email = false;
                //objPaymentRequest.send_sms = false;
                redirectURL = "/SocietyReport/GenerateOnholdReceipt?";
                redirectURL += "SocietySubscriptionID=" + societyReceipt.SocietySubscriptionID.ToString();
                redirectURL += "&SocietyMemberID=" + societyReceipt.SocietyMemberID.ToString();
                redirectURL += "&BillAbbreviation=" + societyReceipt.BillAbbreviation.Trim();
                redirectURL += "&Amount=" + societyReceipt.Amount.ToString();
                objPaymentRequest.redirect_url = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, redirectURL);
                var objPaymentResponse = new PaymentService(ArrPaymentGatewayLink[0], ArrPaymentGatewayLink[1]).CreatePaymentOrder(objPaymentRequest);
                Session["OrderResponse"] = objPaymentResponse.order;
                return Json(new { status = "true", redirectUrl = objPaymentResponse.payment_options.payment_url }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "false", redirectUrl = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GenerateOnholdReceipt(Guid SocietySubscriptionID, Guid SocietyMemberID, string BillAbbreviation, decimal Amount)
        {
            if (null == Session["OrderResponse"])
            {
                TempData["PaymentMessage"] = "Payment failed.";
                return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
            }
            if (null == SocietySubscriptionID || null == SocietyMemberID || string.IsNullOrWhiteSpace(BillAbbreviation) || Amount <= 0)
            {
                TempData["PaymentMessage"] = "Payment failed.";
                Session["OrderResponse"] = null;
                return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
            }
            var societyMember = new SocietyMemberService(this.ModelState).GetById(SocietyMemberID);
            if (null == societyMember)
            {
                TempData["PaymentMessage"] = "Payment failed.";
                Session["OrderResponse"] = null;
                return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
            }
            //Check order status
            var ArrPaymentGatewayLink = societyMember.Society.PaymentGatewayLink.Split(',');
            if (ArrPaymentGatewayLink.Length != 2)
            {
                TempData["PaymentMessage"] = "Payment failed.";
                Session["OrderResponse"] = null;
                return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
            }
            Order objOrder = (Order)Session["OrderResponse"];
            var objPaymentOrderDetailsResponse = new PaymentService(ArrPaymentGatewayLink[0], ArrPaymentGatewayLink[1]).GetDetailsOfPaymentOrder(objOrder.transaction_id);
            if (null == objPaymentOrderDetailsResponse)
            {
                TempData["PaymentMessage"] = "Payment failed.";
                Session["OrderResponse"] = null;
                return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
            }
            if (string.IsNullOrWhiteSpace(objPaymentOrderDetailsResponse.status))
            {
                TempData["PaymentMessage"] = "Payment failed.";
                Session["OrderResponse"] = null;
                return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
            }
            if (objPaymentOrderDetailsResponse.status.ToUpper().Equals("COMPLETED"))
            {
                var societyReceipt = new SocietyReceiptOnhold
                {
                    Amount = Amount,
                    BillAbbreviation = BillAbbreviation,
                    SocietyID = societyMember.SocietyID,
                    SocietySubscriptionID = SocietySubscriptionID,
                    SocietyMemberID = SocietyMemberID,
                    ReceiptDate = DateTime.Now,
                    TransactionId = objOrder.transaction_id
                };
                string unitNumber = "";
                if (societyMember.SocietyBuildingUnitTransfers.Count > 0)
                {
                    unitNumber = societyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + societyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                    societyReceipt.SocietyBuildingUnitID = societyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuildingUnitID;
                }
                if (new SocietyReceiptService(this.ModelState).AddTemporary(societyReceipt))
                {
                    try
                    {
                        //Sending Email
                        new SocietyReceiptService(this.ModelState).SendMail(societyMember, societyReceipt.Amount, unitNumber);
                    }
                    catch { }
                    try
                    {
                        //sending SMS
                        if (!string.IsNullOrEmpty(societyMember.MobileNo))
                        {
                            if (societyMember.MobileNo.Length == 10)
                            {
                                string smsUrl = System.Configuration.ConfigurationManager.AppSettings["SMS_URL"];
                                string messageText = "Dear Member,\n";
                                messageText += "We received Rs. " + societyReceipt.Amount + " against Unit No. " + unitNumber + " It's subject to final disbursement in our A/c\n";
                                messageText += "For " + societyMember.Society.Name;
                                var objTextMessagingService = new CloudSocietyLib.MessagingService.TextMessagingService();
                                objTextMessagingService.SendSMS(smsUrl.Replace("**MobileNo**", "91" + societyMember.MobileNo).Replace("**Message**", messageText));
                            }
                        }
                    }
                    catch { }
                    TempData["PaymentMessage"] = "Payment done successfully.";
                }
                else
                {
                    TempData["PaymentMessage"] = "Failed to create receipt.";
                }
            }
            else
            {
                TempData["PaymentMessage"] = "Payment failed.";
            }
            Session["OrderResponse"] = null;
            return RedirectToAction("MemberOwnLedgerReport", new { id = SocietySubscriptionID, IsToDownLoad = false });
        }

        [HttpGet]
        public ActionResult BillRegisterReport(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societyBillSeriesList = (List<CloudSocietyEntities.SocietyBillSeries>)new CloudSociety.Services.SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(id).SocietyID);//.Where(s=>s );
            if (societyBillSeriesList.Count == 1)
                return RedirectToAction("BillDates", new { id = id, BillAbbreviation = societyBillSeriesList.FirstOrDefault().BillAbbreviation });
            else
                ViewBag.SocietyBillSeriesList = societyBillSeriesList;
            return View();
        }

        [HttpPost]
        public ActionResult BillRegisterReport(Guid id, String BillAbbreviation) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            try
            {
                return RedirectToAction("BillDates", new { id = id, BillAbbreviation = BillAbbreviation });
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View();
            }
        }
        [HttpGet]
        public ActionResult BillDates(Guid id, String BillAbbreviation) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.BillAbbreviation = BillAbbreviation;
            ViewBag.SocietyBuildingList = new SocietyBuildingService(this.ModelState).ListByParentId(societySubscriptionService.GetById(id).SocietyID);
            ViewBag.BillDatesList = new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, BillAbbreviation);
            //code changed By Nityananda 11 Mar 2013 start
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = SocietySubscriptionService.GetById(id);
            ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
            ViewBag.StartRange = SocietySubscription.SubscriptionStart;
            //code By Nityananda 11 Mar 2013 end
            return View();
        }
        // POST: //To display list of SocietyBill added by Ranjit
        // POST: //To display list of SocietyBill Changed By Nityananda 07 Mar 2013
        [HttpPost]
        public ActionResult BillDates(Guid id, Guid? SocietyBuildingID, String BillAbbreviation, DateTime FromDate, DateTime ToDate, bool IsDetails, bool IsAllColumns) // id = SocietySubscriptionID
        {
            return PDFBillRegisterReport(id, SocietyBuildingID, BillAbbreviation, FromDate, ToDate, IsAllColumns, IsDetails);
        }
        // GET: /Generate Bill Register Report
        //public FileStreamResult PDFBillRegisterReport(Guid id, Guid? SocietyBuildingID, String BillAbbreviation, DateTime BillDate, bool IsAllColumns, bool IsDetails)
        //Function changed by Nityananda 07 mar 2013
        public FileStreamResult PDFBillRegisterReport(Guid id, Guid? SocietyBuildingID, String BillAbbreviation, DateTime BillFromDate, DateTime BillToDate, bool IsAllColumns, bool IsDetails)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            PdfPTable tempTable1;
            PdfPTable tempTable2;
            PdfPTable HeadingTable = new PdfPTable(7);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<SocietyBill> SocietyBillList;
            IEnumerable<SocietyBillChargeHeadWithHead> ChargeHeadWithHeadList;
            float[] widths;
            decimal Arears = 0;
            decimal CurBills = 0;
            decimal totalPayable = 0;
            decimal[] totalArears = new decimal[] { 0M, 0M, 0M, 0M };
            decimal[] totalCurBills = new decimal[] { 0M, 0M, 0M, 0M };
            SocietyChargeHead SocietyChargeHead;
            decimal billarears;
            try
            {
                string[] thList = new string[] { "Doc No.", "Doc Date", "Unit No.", " Member ", "Arrears", "Current Bills", "Payable" };
                string[] innerThList = new string[] { "Chg", "NonChg", "Int", "Tax" };
                //code Modified By Nityananda on 07 Mar 2013
                //contain = "BILL REGISTER OF " + String.Format("{0:dd-MMM-yyyy}", BillDate);
                contain = "BILL REGISTER FROM " + String.Format("{0:dd-MMM-yyyy}", BillFromDate) + " TO " + String.Format("{0:dd-MMM-yyyy}", BillToDate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 7;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                HeadingTable.AddCell(cell);
                Table = new PdfPTable(7);
                tempTable1 = new PdfPTable(4);
                if (IsAllColumns)
                {
                    widths = IsDetails ? new float[] { 40f, 40f, 40f, 80f, 150f, 220f, 40f } : new float[] { 70f, 50f, 50f, 100f, 150f, 150f, 40f };
                    Table.SetWidthPercentage(widths, rect);
                    HeadingTable.SetWidthPercentage(widths, rect);
                    cell = new PdfPCell(new Phrase(new Chunk("Arrears", FontV8Bold)));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tempTable1.AddCell(cell);
                    for (int i = 0; i < innerThList.Length; i++)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(innerThList[i], FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tempTable1.AddCell(cell);
                    }
                }
                else
                {
                    widths = IsDetails ? new float[] { 70f, 50f, 60f, 100f, 50f, 170f, 50f } : new float[] { 70f, 70f, 80f, 150f, 60f, 60f, 60f };
                    Table.SetWidthPercentage(widths, rect);
                    HeadingTable.SetWidthPercentage(widths, rect);
                    cell = new PdfPCell(new Phrase(new Chunk("Arrears", FontV8Bold)));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tempTable1.AddCell(cell);
                }
                if (IsDetails)
                {
                    tempTable2 = IsAllColumns ? new PdfPTable(5) : new PdfPTable(2);
                    widths = IsAllColumns ? new float[] { 4f, 2f, 2f, 2f, 2f } : new float[] { 9f, 3f };
                    tempTable2.SetWidths(widths);
                    cell = new PdfPCell(new Phrase(new Chunk("Current Bills", FontV8Bold)));
                    cell.Colspan = IsAllColumns ? 5 : 2;
                    cell.BorderWidthTop = 0;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tempTable2.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("Bill Charge Heads", FontV7)));
                    cell.Border = Rectangle.NO_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tempTable2.AddCell(cell);
                    if (IsAllColumns)
                    {
                        for (int i = 0; i < innerThList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(innerThList[i], FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Border = Rectangle.NO_BORDER;
                            tempTable2.AddCell(cell);
                        }
                    }
                    else
                    {
                        cell = new PdfPCell(new Phrase(new Chunk("Amount", FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Border = Rectangle.NO_BORDER;
                        tempTable2.AddCell(cell);
                    }
                }
                else
                {
                    tempTable2 = new PdfPTable(4);
                    widths = new float[] { 2f, 2f, 2f, 2f };
                    tempTable2.SetWidths(widths);
                    cell = new PdfPCell(new Phrase(new Chunk("Current Bills", FontV8Bold)));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = IsAllColumns ? Element.ALIGN_CENTER : Element.ALIGN_RIGHT;
                    tempTable2.AddCell(cell);
                    if (IsAllColumns)
                    {
                        for (int i = 0; i < innerThList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(innerThList[i], FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Border = Rectangle.NO_BORDER;
                            tempTable2.AddCell(cell);
                        }
                    }
                }

                for (int i = 0; i < thList.Length; i++)
                {
                    cell = i == 4 ? new PdfPCell(tempTable1) : (i == 5 ? new PdfPCell(tempTable2) : new PdfPCell(new Phrase(new Chunk(thList[i], FontV8Bold))));
                    cell.HorizontalAlignment = i == 6 ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                if (SocietyBuildingID != null)
                {
                    //Code Modified By Nityananda on )8Mar 2013
                    SocietyBillList = new SocietyBillService(this.ModelState).ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(SocietyID, BillFromDate, BillToDate, BillAbbreviation, (Guid)SocietyBuildingID);
                }
                else
                {
                    //Code Modified By Nityananda on )8Mar 2013
                    //SocietyBillList = new SocietyBillService(this.ModelState).ListBySocietyIDBillDateBillAbbreviation(SocietyID, BillDate, BillAbbreviation);
                    SocietyBillList = new SocietyBillService(this.ModelState).ListBySocietyIDBillDateRangeBillAbbreviation(SocietyID, BillFromDate, BillToDate, BillAbbreviation).ToList();
                }
                if (SocietyBillList != null)
                {
                    foreach (var SocietyBill in SocietyBillList)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyBill.BillNo, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyBill.BillDate), FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyBill.SocietyBuildingUnit.SocietyBuilding.Building + "-" + SocietyBill.SocietyBuildingUnit.Unit, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyBill.SocietyMember.Member, FontV7)));
                        Table.AddCell(cell);
                        if (IsAllColumns)
                        {
                            tempTable1 = new PdfPTable(4);
                            billarears = (SocietyBill.Arrears ?? 0) - (SocietyBill.Advance ?? 0);
                            cell = new PdfPCell(new Phrase(new Chunk(billarears == 0 ? "-" : billarears.ToString(), FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tempTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk((SocietyBill.NonChgArrears ?? 0) == 0 ? "-" : SocietyBill.NonChgArrears.ToString(), FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tempTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk((SocietyBill.InterestArrears ?? 0) == 0 ? "-" : SocietyBill.InterestArrears.ToString(), FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tempTable1.AddCell(cell);
                            cell = new PdfPCell(new Phrase(new Chunk((SocietyBill.TaxArrears ?? 0) == 0 ? "-" : SocietyBill.TaxArrears.ToString(), FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tempTable1.AddCell(cell);
                            totalArears[0] += billarears;
                            totalArears[1] += SocietyBill.NonChgArrears ?? 0;
                            totalArears[2] += SocietyBill.InterestArrears ?? 0;
                            totalArears[3] += SocietyBill.TaxArrears ?? 0;
                        }
                        else
                        {
                            tempTable1 = new PdfPTable(4);
                            billarears = (SocietyBill.Arrears ?? 0) + (SocietyBill.TaxArrears ?? 0) + (SocietyBill.InterestArrears ?? 0) + (SocietyBill.NonChgArrears ?? 0) - (SocietyBill.Advance ?? 0);
                            cell = new PdfPCell(new Phrase(new Chunk(billarears + "", FontV7)));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Colspan = 4;
                            tempTable1.AddCell(cell);
                        }

                        Arears += (SocietyBill.TaxArrears ?? 0) + (SocietyBill.InterestArrears ?? 0) + (SocietyBill.NonChgArrears ?? 0) + (SocietyBill.Arrears ?? 0) - (SocietyBill.Advance ?? 0);

                        cell = new PdfPCell(tempTable1);
                        cell.HorizontalAlignment = IsAllColumns ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        if (IsDetails)
                        {
                            tempTable2 = IsAllColumns ? new PdfPTable(5) : new PdfPTable(2);
                            widths = IsAllColumns ? new float[] { 4f, 2f, 2f, 2f, 2f } : new float[] { 9f, 3f };
                            tempTable2.SetWidths(widths);
                            ChargeHeadWithHeadList = new CloudSociety.Services.SocietyBillChargeHeadService(this.ModelState).ListByParentId(SocietyBill.SocietyBillID);

                            foreach (var ChargeHead in ChargeHeadWithHeadList)
                            {
                                SocietyChargeHead = new CloudSociety.Services.SocietyChargeHeadService(this.ModelState).GetByIds(SocietyID, ChargeHead.ChargeHeadID);
                                //{ "C", "Construction Cost Basis" }, { "A", "Per Area" }, { "L", "Late Payment Penalty" }, { "E", "Early Payment Discount" }, { "I", "Interest" }, { "T", "Tax" } }
                                cell = new PdfPCell(new Phrase(new Chunk(ChargeHead.ChargeHead, FontV7)));
                                cell.Border = Rectangle.NO_BORDER;
                                tempTable2.AddCell(cell);
                                if (IsAllColumns)
                                {
                                    if (SocietyChargeHead.Nature == "I" || SocietyChargeHead.Nature == "T")
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                        cell.Border = Rectangle.NO_BORDER;
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        tempTable2.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                        cell.Border = Rectangle.NO_BORDER;
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        tempTable2.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(SocietyChargeHead.Nature == "I" ? ChargeHead.Amount.ToString() : "-", FontV7)));
                                        cell.Border = Rectangle.NO_BORDER;
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        tempTable2.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(SocietyChargeHead.Nature == "T" ? ChargeHead.Amount.ToString() : "-", FontV7)));
                                        cell.Border = Rectangle.NO_BORDER;
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        tempTable2.AddCell(cell);
                                        totalCurBills[3] += SocietyChargeHead.Nature == "T" ? ChargeHead.Amount : 0;
                                        totalCurBills[2] += SocietyChargeHead.Nature == "I" ? ChargeHead.Amount : 0;
                                    }
                                    else
                                        if (SocietyChargeHead.ChargeInterest ?? false)
                                        {
                                            //Chargeable
                                            cell = new PdfPCell(new Phrase(new Chunk(ChargeHead.Amount.ToString(), FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            totalCurBills[0] += ChargeHead.Amount;
                                        }
                                        else
                                        {
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk(ChargeHead.Amount.ToString(), FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            totalCurBills[1] += ChargeHead.Amount;
                                        }
                                }
                                else
                                {
                                    cell = new PdfPCell(new Phrase(new Chunk(ChargeHead.Amount.ToString(), FontV7)));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.Border = Rectangle.NO_BORDER;
                                    tempTable2.AddCell(cell);
                                }
                            }
                        }
                        else
                        {
                            tempTable2 = new PdfPTable(4);
                            tempTable2.DefaultCell.Border = Rectangle.NO_BORDER;
                            widths = new float[] { 2f, 2f, 2f, 2f };
                            tempTable2.SetWidths(widths);
                            ChargeHeadWithHeadList = new CloudSociety.Services.SocietyBillChargeHeadService(this.ModelState).ListByParentId(SocietyBill.SocietyBillID);
                            foreach (var ChargeHead in ChargeHeadWithHeadList)
                            {
                                SocietyChargeHead = new CloudSociety.Services.SocietyChargeHeadService(this.ModelState).GetByIds(SocietyID, ChargeHead.ChargeHeadID);
                                //{ "C", "Construction Cost Basis" }, { "A", "Per Area" }, { "L", "Late Payment Penalty" }, { "E", "Early Payment Discount" }, { "I", "Interest" }, { "T", "Tax" } }                               
                                if (IsAllColumns)
                                {
                                    if (SocietyChargeHead.Nature == "I" || SocietyChargeHead.Nature == "T")
                                    {
                                        cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.Border = Rectangle.NO_BORDER;
                                        tempTable2.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.Border = Rectangle.NO_BORDER;
                                        tempTable2.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(SocietyChargeHead.Nature == "I" ? ChargeHead.Amount.ToString() : "-", FontV7)));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.Border = Rectangle.NO_BORDER;
                                        tempTable2.AddCell(cell);
                                        cell = new PdfPCell(new Phrase(new Chunk(SocietyChargeHead.Nature == "T" ? ChargeHead.Amount.ToString() : "-", FontV7)));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.Border = Rectangle.NO_BORDER;
                                        tempTable2.AddCell(cell);
                                        totalCurBills[3] += SocietyChargeHead.Nature == "T" ? ChargeHead.Amount : 0;
                                        totalCurBills[2] += SocietyChargeHead.Nature == "I" ? ChargeHead.Amount : 0;
                                    }
                                    else
                                        if (SocietyChargeHead.ChargeInterest ?? false)
                                        {
                                            //Chargeable
                                            cell = new PdfPCell(new Phrase(new Chunk(ChargeHead.Amount.ToString(), FontV7)));
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            cell.Border = Rectangle.NO_BORDER;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            cell.Border = Rectangle.NO_BORDER;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            cell.Border = Rectangle.NO_BORDER;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            cell.Border = Rectangle.NO_BORDER;
                                            tempTable2.AddCell(cell);
                                            totalCurBills[0] += ChargeHead.Amount;
                                        }
                                        else
                                        {
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            cell.Border = Rectangle.NO_BORDER;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk(ChargeHead.Amount.ToString(), FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            cell = new PdfPCell(new Phrase(new Chunk("-", FontV7)));
                                            cell.Border = Rectangle.NO_BORDER;
                                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                            tempTable2.AddCell(cell);
                                            totalCurBills[1] += ChargeHead.Amount;
                                        }
                                }
                            }
                            if (!IsAllColumns)
                            {
                                cell = new PdfPCell(new Phrase(new Chunk(SocietyBill.uAmount + (SocietyBill.Interest ?? 0) + (SocietyBill.TaxAmount ?? 0) + "", FontV7)));
                                cell.Colspan = 4;
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                tempTable2.AddCell(cell);
                            }
                        }
                        CurBills += (decimal)SocietyBill.uAmount + (SocietyBill.Interest ?? 0) + (SocietyBill.TaxAmount ?? 0);
                        cell = new PdfPCell(tempTable2);
                        cell.HorizontalAlignment = IsAllColumns ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
                        Table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(new Chunk(SocietyBill.Payable.ToString(), FontV7Bold)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalPayable += SocietyBill.Payable ?? 0;
                    }
                }
                if (IsAllColumns)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(IsDetails ? "Total Arrears:" : "Total :", FontV7Bold)));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    tempTable1 = new PdfPTable(4);
                    for (int i = 0; i < totalArears.Length; i++)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(totalArears[i].ToString(), FontV7Bold)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tempTable1.AddCell(cell);
                    }
                    cell = new PdfPCell(tempTable1);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase(new Chunk("Total :", FontV7Bold)));
                    cell.Colspan = 4;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    tempTable1 = new PdfPTable(4);
                    cell = new PdfPCell(new Phrase(new Chunk(Arears.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }

                if (IsAllColumns)
                {
                    tempTable2 = IsDetails ? new PdfPTable(5) : new PdfPTable(4);
                    widths = IsDetails ? new float[] { 4f, 2f, 2f, 2f, 2f } : new float[] { 2f, 2f, 2f, 2f };
                    tempTable2.SetWidths(widths);
                    if (IsDetails)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk("Total Current Bills:", FontV7Bold)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Border = Rectangle.NO_BORDER;
                        tempTable2.AddCell(cell);
                    }
                    for (int i = 0; i < totalCurBills.Length; i++)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(totalCurBills[i].ToString(), FontV7Bold)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.Border = Rectangle.NO_BORDER;
                        tempTable2.AddCell(cell);
                    }
                    cell = new PdfPCell(tempTable2);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase(new Chunk(CurBills.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }

                cell = new PdfPCell(new Phrase(new Chunk(totalPayable.ToString(), FontV7Bold)));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);

                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            //Code Modified By Nityananda on 07 Mar 2013
            //return this.FileStreamResult(TableList, Society, "BillRegister" + (IsDetails ? "Details" : "Summary") + "ReportOf" + String.Format("{0:dd-MMM-yyyy}", BillDate) + ".pdf", HeadingTable);
            return this.FileStreamResult(TableList, Society, "BillRegister" + (IsDetails ? "Details" : "Summary") + "Reportfrom " + String.Format("{0:dd-MMM-yyyy}", BillFromDate) + "to" + String.Format("{0:dd-MMM-yyyy}", BillToDate) + ".pdf", HeadingTable);
        }
        //GET : /Ask from date to date for CollectionReport added by Ranjit 
        [HttpGet]
        public ActionResult CollectionReport(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societySubscription.SubscriptionStart;
            ViewBag.ToDate = societySubscription.PaidTillDate;
            ViewBag.SocietyBuildingList = new SocietyBuildingService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            return View();
        }
        [HttpPost]
        public FileStreamResult CollectionReport(Guid id, Guid? SocietyBuildingID, DateTime FromDate, DateTime ToDate)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<SocietyReceipt> SocietyReceiptList;
            PdfPTable HeadingTable = new PdfPTable(8);
            string PayMode;
            decimal totalAmount = 0;
            try
            {
                string[] thList = new string[] { "Doc No.", "Doc Date", "Unit No.", " Member ", "PayMode", "PayRefNo", "PayRefDate", "Amount" };
                float[] widths = new float[] { 80f, 55f, 80f, 120f, 60f, 60f, 60f, 60f };
                Table = new PdfPTable(8);
                Table.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "COLLECTION REGISTER FOR THE PERIOD FROM " + String.Format("{0:dd-MMM-yyyy}", FromDate) + " TO " + String.Format("{0:dd-MMM-yyyy}", ToDate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 8;
                HeadingTable.AddCell(cell);
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8Bold)));
                    cell.HorizontalAlignment = i == 7 ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                if (SocietyBuildingID != null)
                    SocietyReceiptList = new SocietyReceiptService(this.ModelState).ListBySocietyIDStartEndDateSocietyBuildingID(SocietyID, FromDate, ToDate, (Guid)SocietyBuildingID);//ListBySocietyIDStartEndDate(SocietyID, FromDate, ToDate).Where(r => r.SocietyBuildingUnit.SocietyBuildingID == SocietyBuildingID);
                else
                    SocietyReceiptList = new SocietyReceiptService(this.ModelState).ListBySocietyIDStartEndDate(SocietyID, FromDate, ToDate);
                if (SocietyReceiptList.Count() != 0)
                {
                    foreach (var SocietyReceipt in SocietyReceiptList)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyReceipt.ReceiptNo, FontV8)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyReceipt.ReceiptDate), FontV8)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyReceipt.SocietyBuildingUnit.SocietyBuilding.Building + "-" + SocietyReceipt.SocietyBuildingUnit.Unit, FontV8)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyReceipt.SocietyMember.Member, FontV8)));
                        Table.AddCell(cell);
                        PayMode = new SocietyPayModeService(this.ModelState).ListByParentId(SocietyID).Where(r => r.PayModeCode == SocietyReceipt.PayModeCode).Select(r => r.PayMode).FirstOrDefault();
                        cell = new PdfPCell(new Phrase(new Chunk(PayMode, FontV8)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyReceipt.PayRefNo, FontV8)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyReceipt.PayRefDate), FontV8)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyReceipt.Amount.ToString(), FontV8)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalAmount += SocietyReceipt.Amount;
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Total :", FontV8Bold)));
                    cell.Colspan = 7;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalAmount.ToString(), FontV8Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase(new Chunk("No record found", FontV9Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 8;
                    Table.AddCell(cell);
                    HeadingTable = new PdfPTable(8);
                    HeadingTable.SetWidthPercentage(widths, rect);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "CollectionRegister.pdf", HeadingTable);
        }

        [HttpGet]
        public ActionResult CollectionReversalReport(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.FromDate = societySubscription.SubscriptionStart;
            ViewBag.ToDate = societySubscription.PaidTillDate;
            ViewBag.SocietyBuildingList = new SocietyBuildingService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            return View();
        }
        [HttpPost]
        public FileStreamResult CollectionReversalReport(Guid id, Guid? SocietyBuildingID, DateTime FromDate, DateTime ToDate)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<SocietyCollectionReversal> SocietyCollectionReversalList;
            PdfPTable HeadingTable = new PdfPTable(13);
            string PayMode;
            decimal totalPrinciapal = 0, totalNonChg = 0, totalInterest = 0, totalTax = 0, totalAdvance = 0;
            try
            {
                string[] thList = new string[] { "Doc No.", "Doc Date", "Unit No.", " Member", "ReceiptNo", "PayMode", "PayRefNo", "PayRefDate", "Principal", "Non.Chg", "Interest", "Tax", "Advance" };
                float[] widths = new float[] { 70f, 47f, 52f, 86f, 71f, 38f, 46f, 46f, 36f, 36f, 36f, 36f, 36f };
                Table = new PdfPTable(13);
                Table.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "COLLECTION REVERSAL REGISTER FOR THE PERIOD FROM " + String.Format("{0:dd-MMM-yyyy}", FromDate) + " TO " + String.Format("{0:dd-MMM-yyyy}", ToDate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 13;
                HeadingTable.AddCell(cell);
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV7Bold)));
                    cell.HorizontalAlignment = i > 7 ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                if (SocietyBuildingID != null)
                    SocietyCollectionReversalList = new SocietyCollectionReversalService(this.ModelState).ListBySocietyIDStartEndDateSocietyBuildingID(SocietyID, FromDate, ToDate, (Guid)SocietyBuildingID);
                else
                    SocietyCollectionReversalList = new SocietyCollectionReversalService(this.ModelState).ListBySocietyIDStartEndDate(SocietyID, FromDate, ToDate);
                if (SocietyCollectionReversalList.Count() != 0)
                {
                    foreach (var SocietyCollectionReversal in SocietyCollectionReversalList)
                    {
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.DocNo, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyCollectionReversal.ReversalDate), FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.SocietyBuildingUnit.SocietyBuilding.Building + "-" + SocietyCollectionReversal.SocietyBuildingUnit.Unit, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.SocietyMember.Member, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.SocietyReceipt.ReceiptNo, FontV7)));
                        Table.AddCell(cell);
                        PayMode = new SocietyPayModeService(this.ModelState).ListByParentId(SocietyID).Where(r => r.PayModeCode == SocietyCollectionReversal.PayModeCode).Select(r => r.PayMode).FirstOrDefault();
                        cell = new PdfPCell(new Phrase(new Chunk(PayMode, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.PayRefNo, FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(String.Format("{0:dd-MMM-yyyy}", SocietyCollectionReversal.PayRefDate), FontV7)));
                        Table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.PrincipalAdjusted.ToString(), FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalPrinciapal += SocietyCollectionReversal.PrincipalAdjusted ?? 0;
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.NonChgAdjusted.ToString(), FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalNonChg += SocietyCollectionReversal.NonChgAdjusted ?? 0;
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.InterestAdjusted.ToString(), FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalInterest += SocietyCollectionReversal.InterestAdjusted ?? 0;
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.TaxAdjusted.ToString(), FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalTax += SocietyCollectionReversal.TaxAdjusted ?? 0;
                        cell = new PdfPCell(new Phrase(new Chunk(SocietyCollectionReversal.Advance.ToString(), FontV7)));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Table.AddCell(cell);
                        totalAdvance += SocietyCollectionReversal.Advance ?? 0;
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Total :", FontV7Bold)));
                    cell.Colspan = 8;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalPrinciapal.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalNonChg.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalInterest.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalTax.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalAdvance.ToString(), FontV7Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Phrase(new Chunk("No record found", FontV9Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 13;
                    Table.AddCell(cell);
                    HeadingTable = new PdfPTable(13);
                    HeadingTable.SetWidthPercentage(widths, rect);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "CollectionReversalRegister.pdf", HeadingTable);
        }

        //GET : /Ask from date to date for SocietyInvestments report added by Ranjit 
        [HttpGet]
        public ActionResult SocietyInvestmentReport(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
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
        public FileStreamResult SocietyInvestmentReport(Guid id, bool IsPending, DateTime? FromDate, DateTime? ToDate) // id = SocietySubscriptionID
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<SocietyInvestment> SocietyInvestmentList;
            PdfPTable HeadingTable = new PdfPTable(7);
            decimal totalAmount = 0, totalMaturityAmount = 0;
            try
            {
                string[] thList = new string[] { "Doc No.", "Doc Date", "Bank", "Amount", "Interest Rate", "Maturity Date", "Maturity Amount" };
                float[] widths = new float[] { 70f, 60f, 150f, 60f, 50f, 60f, 60f };
                Table = new PdfPTable(7);
                Table.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "INVESTMENT REGISTER ";
                if (FromDate != null && ToDate != null)
                    contain += "FOR THE PERIOD FROM " + String.Format("{0:dd-MMM-yyyy}", FromDate) + " TO " + String.Format("{0:dd-MMM-yyyy}", ToDate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 7;
                HeadingTable.AddCell(cell);
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8Bold)));
                    cell.HorizontalAlignment = (i == 3 || i == 6) ? Element.ALIGN_RIGHT : Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                if (FromDate != null && ToDate != null)
                {
                    if (IsPending)
                        SocietyInvestmentList = new SocietyInvestmentService(this.ModelState).ListByParentId(SocietyID).Where(si => si.DocumentDate >= FromDate && si.DocumentDate <= ToDate && si.ClosureDate == null);
                    else
                        SocietyInvestmentList = new SocietyInvestmentService(this.ModelState).ListByParentId(SocietyID).Where(si => si.DocumentDate >= FromDate && si.DocumentDate <= ToDate);
                }
                else
                {
                    if (IsPending)
                        SocietyInvestmentList = new SocietyInvestmentService(this.ModelState).ListByParentId(SocietyID).Where(si => si.ClosureDate == null);
                    else
                        SocietyInvestmentList = new SocietyInvestmentService(this.ModelState).ListByParentId(SocietyID);
                }
                if (SocietyInvestmentList != null && SocietyInvestmentList.Count() > 0)
                {
                    foreach (var SocietyInvestment in SocietyInvestmentList)
                    {
                        thList = new string[] { SocietyInvestment.DocumentNo, String.Format("{0:dd-MMM-yyyy}", SocietyInvestment.DocumentDate), SocietyInvestment.BankEntity.Name + (SocietyInvestment.Bank == null ? "" : "\n" + SocietyInvestment.Bank + " (Oth.)"), SocietyInvestment.Amount + "", SocietyInvestment.InterestRate + " %", String.Format("{0:dd-MMM-yyyy}", SocietyInvestment.MaturityDate), SocietyInvestment.MaturityAmount + "" };
                        for (int i = 0; i < thList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8)));
                            cell.HorizontalAlignment = (i == 3 || i == 6) ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
                            Table.AddCell(cell);
                        }
                        totalAmount += SocietyInvestment.Amount;
                        totalMaturityAmount += SocietyInvestment.MaturityAmount;
                    }
                    cell = new PdfPCell(new Phrase(new Chunk("Total Amount:", FontV8Bold)));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalAmount.ToString(), FontV8Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk("Total Maturity :", FontV8Bold)));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(new Chunk(totalMaturityAmount.ToString(), FontV8Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    Table.AddCell(cell);
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "CollectionRegister.pdf", HeadingTable);
        }

        //GET : /Ask as on date for Parking Register added by Ranjit 
        [HttpGet]
        public ActionResult ParkingRegister(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            //var societySubscription = societySubscriptionService.GetById(id);
            //ViewBag.FromDate = societySubscription.SubscriptionStart;
            //ViewBag.ToDate = societySubscription.PaidTillDate;
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            return View();
        }
        [HttpPost]
        public FileStreamResult ParkingRegister(Guid id, DateTime AsOnDate)
        {
            Guid SocietyID = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            Society Society = new SocietyService(this.ModelState).GetById(SocietyID);
            List<PdfPTable> TableList = new List<PdfPTable>();
            IEnumerable<SocietyParkingWithMember> SocietyParkingWithMemberList;
            PdfPTable HeadingTable = new PdfPTable(5);
            try
            {
                string[] thList = new string[] { "Parking No.", "Parking Type", "Member", "Transferred On", "Vehicle No." };
                float[] widths = new float[] { 80f, 80f, 150f, 150f, 80f };
                Table = new PdfPTable(5);
                Table.SetWidthPercentage(widths, rect);
                HeadingTable.SetWidthPercentage(widths, rect);
                contain = "PARKING REGISTER AS ON " + String.Format("{0:dd-MMM-yyyy}", AsOnDate);
                cell = new PdfPCell(_service.CaptionTable(contain, FontV8Bold, System.Drawing.Color.LightGray));
                cell.Colspan = 5;
                HeadingTable.AddCell(cell);
                for (int i = 0; i < thList.Length; i++)
                {
                    cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8Bold)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    HeadingTable.AddCell(cell);
                }
                SocietyParkingWithMemberList = new SocietyParkingService(this.ModelState).ListWithMemberAsOnDateBySocietyID(SocietyID, AsOnDate);
                if (SocietyParkingWithMemberList != null)
                {
                    foreach (var SocietyParkingWithMember in SocietyParkingWithMemberList)
                    {
                        thList = new string[] { SocietyParkingWithMember.ParkingNo, SocietyParkingWithMember.ParkingType, SocietyParkingWithMember.Member, String.Format("{0:dd-MMM-yyyy}", SocietyParkingWithMember.TransferredOn), SocietyParkingWithMember.VehicleNumber };
                        for (int i = 0; i < thList.Length; i++)
                        {
                            cell = new PdfPCell(new Phrase(new Chunk(thList[i], FontV8)));
                            Table.AddCell(cell);
                        }
                    }
                }
                TableList.Add(Table);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
            }
            return this.FileStreamResult(TableList, Society, "ParkingRegisterAsOn-" + String.Format("{0:dd-MMM-yyyy}", AsOnDate) + ".pdf", HeadingTable);
        }

    }
}