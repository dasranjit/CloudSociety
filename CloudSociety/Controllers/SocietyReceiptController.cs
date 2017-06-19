using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;
using System.Web.Security;
using System.IO;
using System.Net.Mail;
using CommonLib.Financial;
using CloudSocietyLib.Reporting;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser,Member,OfficeBearer")]
    public class SocietyReceiptController : Controller
    {
        private SocietyReceiptService _service;
        const string _receiptfromdate = "ReceiptFromDate";
        const string _receipttodate = "ReceiptToDate";
        const string _exceptioncontext = "SocietyReceipt Controller";
        public SocietyReceiptController()
        {
            _service = new SocietyReceiptService(this.ModelState);
        }

        // Method to generate society receipt and return. Added By Ranjit
        public FileStreamResult CreatePdf(Guid id)
        {
            MemoryStream ms = _service.PdfMsReceipt(id);
            string receiptDate = String.Format("{0:dd-MMM-yyyy}", _service.GetById(id).ReceiptDate);
            //prepare output stream            
            Response.ContentType = "application/pdf";
            string arrg = "attachment;filename=ReceiptOf" + String.Format("{0:dd-MMM-yyyy}", receiptDate) + "." + "pdf";
            Response.AddHeader("content-disposition", arrg);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        // GET: /To display list of SocietyReceipt added by Ranjit
        public ActionResult Index(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            DateTime? FromDate = null, ToDate = null;
            if (Session[_receiptfromdate] != null)
                FromDate = (DateTime)Session[_receiptfromdate];
            if (Session[_receipttodate] != null)
                ToDate = (DateTime)Session[_receipttodate];
            bool showlist = (FromDate != null || ToDate != null);
            var cntReceipt = _service.ListBySocietyIDStartEndDate(societySubscription.SocietyID, societySubscription.SubscriptionStart, (DateTime)societySubscription.PaidTillDate).Count();
            if (cntReceipt > 0)
            {
                if (FromDate == null || FromDate < societySubscription.SubscriptionStart || FromDate > societySubscription.PaidTillDate)
                {
                    /// Following statement causes architectural violation, uses LINQ
                    var lastDate = _service.ListBySocietyIDStartEndDate(societySubscription.SocietyID, societySubscription.SubscriptionStart, (DateTime)societySubscription.PaidTillDate).Last().ReceiptDate;
                    FromDate = lastDate.AddDays((lastDate.Day * -1) + 1);
                    Session[_receiptfromdate] = FromDate;
                }
                if (ToDate == null || ToDate < societySubscription.SubscriptionStart || ToDate > societySubscription.PaidTillDate)
                {
                    ToDate = (DateTime)societySubscription.PaidTillDate;
                    Session[_receipttodate] = ToDate;
                }
            }
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.ReadOnly = (societySubscription.PaidTillDate == null ? true : false) || societySubscription.Closed;
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.LockedTillDate = societySubscription.LockedTillDate;
            if (cntReceipt > 0) // showlist
            {
                ViewBag.SendMessage = TempData["SendMessage"];
                return View(_service.ListBySocietyIDStartEndDate(societySubscription.SocietyID, (DateTime)FromDate, (DateTime)ToDate));
            }
            else
                return View();  // _service.ListByParentId(societySubscriptionService.GetById(id).SocietyID)
        }

        [HttpPost]
        public ActionResult Index(Guid id, DateTime FromDate, DateTime ToDate) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            if (FromDate == null || FromDate < societySubscription.SubscriptionStart || FromDate > societySubscription.PaidTillDate)
            {
                var lastDate = _service.ListBySocietyIDStartEndDate(societySubscription.SocietyID, societySubscription.SubscriptionStart, (DateTime)societySubscription.PaidTillDate).Last().ReceiptDate;
                FromDate = lastDate.AddDays((lastDate.Day * -1) + 1);
            }
            if (ToDate == null || ToDate < societySubscription.SubscriptionStart || ToDate > societySubscription.PaidTillDate)
            {
                ToDate = (DateTime)societySubscription.PaidTillDate;
            }
            Session[_receiptfromdate] = FromDate;
            Session[_receipttodate] = ToDate;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.ReadOnly = societySubscription.PaidTillDate == null || societySubscription.Closed;
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.LockedTillDate = societySubscription.LockedTillDate;
            return View(_service.ListBySocietyIDStartEndDate(societySubscription.SocietyID, FromDate, ToDate));
        }

        // GET: /To display Detail list of Society Receipt added by Ranjit
        public ActionResult Details(Guid id) // id = SocietyReceiptID
        {
            var SocietyReceipt = _service.GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
            ViewBag.YearOpen = !societySubscriptionService.GetById(SocietyReceipt.SocietySubscriptionID).Closed;
            ViewBag.IsMember = (Roles.IsUserInRole("Member") || Roles.IsUserInRole("OfficeBearer"));
            return View(SocietyReceipt);
        }

        public ActionResult MemberReceiptIndex(Guid id) // id = SocietySubscriptionID
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

        // GET: /To display list of SocietyReceipt added by Nityananda
        //public ActionResult IndexForMember(Guid id) //id = SocietyMemberID
        //{

        //    ViewBag.SocietyMemberID = id;
        //    var SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(id);
        //    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        //    var SocietySubscriptionID = societySubscriptionService.ListByParentId(SocietyMember.SocietyID).FirstOrDefault().SocietySubscriptionID;
        //    ViewBag.SocietySubscriptionID = SocietySubscriptionID;
        //    //var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        //    ViewBag.ShowSocietyMemberMenu = true;
        //    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietySubscriptionID);
        //    //ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
        //   // ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
        //    //ViewBag.ReadOnly = (societySubscriptionService.GetById(id).PaidTillDate == null ? true : false);
        //    //ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
        //    //ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
        //    return View(_service.ListByParentId(SocietyMember.SocietyID).Where(c=>c.SocietyMemberID==id));
        //}

        // GET: /To display  of SocietyMemberReceipt added by Nityananda
        //public ActionResult DetailsForMember(Guid id) // id = SocietyReceiptID
        //{
        //    var SocietyReceipt = _service.GetById(id);
        //    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(SocietyReceipt.SocietySubscriptionID);
        //    return View(SocietyReceipt);
        //}
        //public ActionResult DetailsForMember(Guid id) //id = SocietyMemberID
        //{
        //    ViewBag.SocietyMemberID = id;
        //    var societyMemberService = new CloudSociety.Services.SocietyMemberService(this.ModelState);
        //    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        //    ViewBag.ShowSocietyMemberMenu = true;
        //    var societyMember = societyMemberService.GetById(id);
        //    var SocietySubscriptionID = societySubscriptionService.ListByParentId(societyMember.SocietyID).FirstOrDefault().SocietySubscriptionID;
        //    var Societyreceiptservice = new CloudSociety.Services.SocietyReceiptService(this.ModelState);
        //    var SocietyreceiptList = Societyreceiptservice.ListByParentId(societyMember.SocietyID);
        //    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(SocietySubscriptionID);
        //    var SocietyReceipt=SocietyreceiptList.OrderBy(s=>s.ReceiptNo).FirstOrDefault(s=>s.SocietyMemberID==id);
        //    if(SocietyReceipt==null)
        //        return View(SocietyReceipt);
        //    else
        //    SocietyReceipt = _service.GetById(SocietyReceipt.SocietyReceiptID);
        //    return View(SocietyReceipt);
        //}

        // GET: /To display list of BillAbbr added by Ranjit
        [HttpGet]
        public ActionResult ListOfBuildingUnitBillAbbreviation(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societyId = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyId);
            return View();
        }

        [HttpPost]
        public ActionResult ListOfBuildingUnitBillAbbreviation(Guid id, SocietyReceipt SocietyReceipt) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            return RedirectToAction("Create", new { id = id, societyBuildingUnitID = SocietyReceipt.SocietyBuildingUnitID, billAbbreviation = SocietyReceipt.BillAbbreviation });
        }

        // GET: /To Create SocietyReceipt added by Ranjit
        public ActionResult Create(Guid id, Guid societyBuildingUnitID, String billAbbreviation) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            var societyID = societySubscription.SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societyID;
            ViewBag.SocietyBuildingUnit = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            ViewBag.BillAbbreviation = billAbbreviation;
            ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(societyBuildingUnitID);
            ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyID).Where(r => r.Active == true);
            ViewBag.BankList = new BankService(this.ModelState).List();
            //            List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
            ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
            //            ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
            DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
            DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
            ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
            return View();
        }

        // POST: /To Create SocietyReceipt added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, Guid societyBuildingUnitID, String billAbbreviation, SocietyReceipt SocietyReceiptToCreate)
        {
            try
            {
                if (_service.Add(SocietyReceiptToCreate))
                {
                    if (Session[_receipttodate] == null)
                    {
                        var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                        Session[_receipttodate] = (DateTime)societySubscriptionService.GetById(id).PaidTillDate;
                    }
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(id);
                    var societyID = societySubscription.SocietyID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societyID;
                    ViewBag.SocietyBuildingUnit = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                    ViewBag.BillAbbreviation = billAbbreviation;
                    ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(societyBuildingUnitID);
                    ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyID).Where(r => r.Active == true);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    //                    List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                    ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                    //                    ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                    //                    ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                    DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                    DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                    ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(id);
                var societyID = societySubscription.SocietyID;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyID = societyID;
                ViewBag.SocietyBuildingUnit = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                ViewBag.BillAbbreviation = billAbbreviation;
                ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(societyBuildingUnitID);
                ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societyID).Where(r => r.Active == true);
                ViewBag.BankList = new BankService(this.ModelState).List();
                //                List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, billAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                //                ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                //                ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, billAbbreviation);
                ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                return View();
            }
        }

        // GET: /Edit SocietyReceipt added by Ranjit
        public ActionResult Edit(Guid id) //id=SocietyReceiptID
        {
            var SocietyReceipt = _service.GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(SocietyReceipt.SocietySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
            ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societySubscription.SocietyID).Where(r => r.Active == true);
            ViewBag.BankList = new BankService(this.ModelState).List();
            //            List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
            ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
            //            ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
            //            ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation);
            DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
            DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, SocietyReceipt.BillAbbreviation);
            ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
            return View(SocietyReceipt);
        }

        // POST: /Edit SocietyReceipt added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, SocietyReceipt SocietyReceiptToEdit)//id=SocietyReceiptID
        {
            var SocietyReceipt = _service.GetById(id);
            try
            {
                if (_service.Edit(SocietyReceiptToEdit))
                {
                    return RedirectToAction("Index", new { id = SocietyReceipt.SocietySubscriptionID });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(SocietyReceipt.SocietySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
                    ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societySubscription.SocietyID).Where(r => r.Active == true);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    //                    List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                    ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                    //                    ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                    //                    ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation);
                    DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                    DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, SocietyReceipt.BillAbbreviation);
                    ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                    return View(SocietyReceipt);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(SocietyReceipt.SocietySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
                ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(societySubscription.SocietyID).Where(r => r.Active == true);
                ViewBag.BankList = new BankService(this.ModelState).List();
                //                List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                //                ViewBag.StartRange = (billDateList.Count == 0 ? societySubscription.SubscriptionStart : billDateList.First());
                //                ViewBag.StartRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(societySubscription.SocietySubscriptionID, SocietyReceipt.BillAbbreviation);
                DateTime startDate = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                DateTime? startRange = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(id, SocietyReceipt.BillAbbreviation);
                ViewBag.StartRange = (startRange == null || startRange < startDate ? startDate : startRange);
                return View(SocietyReceipt);
            }
        }

        // GET: Delete SocietyReceipt added by Ranjit
        public ActionResult Delete(Guid id)
        {
            SocietyReceipt SocietyReceipt = _service.GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
            //            List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(SocietyReceipt.SocietySubscriptionID, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
            var lastbilldate = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(SocietyReceipt.SocietySubscriptionID, SocietyReceipt.BillAbbreviation);
            //            ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (billDateList.Count == 0 ? SocietyReceipt.SocietySubscription.SubscriptionStart : billDateList.First()) ? true : false);
            //            ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (lastbilldate == null ? SocietyReceipt.SocietySubscription.SubscriptionStart : lastbilldate));
            // changed to below to allow delete of receipts on last bill date 21-Jun-16
            ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate >= (lastbilldate == null ? SocietyReceipt.SocietySubscription.SubscriptionStart : lastbilldate));
            return View(SocietyReceipt);
        }

        //Method to send receipt mail. Added by Ranjit 
        public ActionResult SendMail(Guid id)
        {
            SocietyReceipt societyReceipt = _service.GetById(id);
            //            ViewBag.SocietySubscriptionID = societyReceipt.SocietySubscriptionID;
            //Guid societyMemberID = new CloudSociety.Services.SocietyReceiptService(this.ModelState).GetById(id).SocietyMemberID;
            MemoryStream ms = _service.PdfMsReceipt(id);
            SmtpClient mailClient = new SmtpClient();
            MailMessage message = new MailMessage();
            string mailFrom, mailTo, mailBody, receiptDate, fileName;

            receiptDate = String.Format("{0:dd-MMM-yyyy}", societyReceipt.ReceiptDate);
            fileName = "ReceiptOf" + String.Format("{0:dd-MMM-yyyy}", receiptDate) + "." + "pdf";
            message.IsBodyHtml = true;
            mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
            mailTo = societyReceipt.SocietyMember.EmailId;
            //var ud = new CloudSociety.Services.UserDetailService(this.ModelState).GetBySocietyMemberID(societyMemberID);
            //if (ud != null)
            //{ 
            //    var usr = Membership.GetUser((object)ud.UserID);
            //    if(usr != null)
            //        mailTo = usr.Email;
            //}
            if (!string.IsNullOrEmpty(mailFrom) && !String.IsNullOrEmpty(mailTo) && !String.IsNullOrWhiteSpace(mailTo))
            {
                try
                {
                    message.From = new MailAddress(mailFrom);
                    message.To.Add(new MailAddress(mailTo));
                    message.Subject = "Society Receipt for " + societyReceipt.Society.Name;
                    //Following need to add Admin Parameter
                    mailBody = new CloudSociety.Services.AppInfoService(this.ModelState).Get().ReceiptMailBody;
                    mailBody = mailBody.Replace("&&Member&&", societyReceipt.SocietyMember.Member);
                    mailBody = mailBody.Replace("&&ReceiptNo&&", societyReceipt.ReceiptNo);
                    mailBody = mailBody.Replace("&&ReceiptDate&&", receiptDate);
                    mailBody = mailBody.Replace("&&Unit&&", societyReceipt.SocietyBuildingUnit.SocietyBuilding.Building + "-" + societyReceipt.SocietyBuildingUnit.Unit);
                    mailBody = mailBody.Replace("&&Amount&&", societyReceipt.Amount.ToString("###0.00"));
                    mailBody = mailBody.Replace("&&SocietyName&&", societyReceipt.Society.Name);
                    mailBody = mailBody.Replace("&&SocietyContactPerson&&", societyReceipt.Society.ContactPerson);
                    mailBody = mailBody.Replace("&&SocietyContactNumber&&", societyReceipt.Society.Mobile);
                    mailBody = mailBody.Replace("&&SocietyContactEmailId&&", societyReceipt.Society.EMailId);
                    mailBody = mailBody.Replace("&&ToDayDate&&", string.Format("{0:dd-MMM-yyyy}", DateTime.Now));
                    message.Body = mailBody;
                    Attachment a = new Attachment(ms, fileName, "application/pdf");
                    message.Attachments.Add(a);
                    //                if (!string.IsNullOrEmpty(mailFrom) && mailTo != null)
                    mailClient.Send(message);
                    //                this.ModelState.AddModelError("SendMessage", "Receipt No. "+societyReceipt.ReceiptNo + " sent to member");
                    TempData["SendMessage"] = "Receipt No. " + societyReceipt.ReceiptNo + " emailed to member";
                    return RedirectToAction("Index", new { id = societyReceipt.SocietySubscriptionID });
                    //                return RedirectToAction("SendSuccess", new { id = societyReceipt.SocietySubscriptionID });
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

        ////Method to show Success message. Added by Ranjit
        //public ActionResult SendSuccess(Guid id)
        //{
        //    ViewBag.SocietySubscriptionID = id;
        //    return View();
        //}

        // POST: Delete SocietyReceipt added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, SocietyReceipt SocietyReceiptToDelete)
        {
            SocietyReceipt SocietyReceipt = _service.GetById(id);
            try
            {
                if (_service.Delete(SocietyReceipt))
                    return RedirectToAction("Index", new { id = SocietyReceipt.SocietySubscriptionID });
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
                    //                    List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                    var lastbilldate = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(SocietyReceipt.SocietySubscriptionID, SocietyReceipt.BillAbbreviation);
                    //            ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (billDateList.Count == 0 ? SocietyReceipt.SocietySubscription.SubscriptionStart : billDateList.First()) ? true : false);
                    ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (lastbilldate == null ? SocietyReceipt.SocietySubscription.SubscriptionStart : lastbilldate));
                    return View(SocietyReceipt);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(SocietyReceipt.SocietySubscriptionID);
                //                List<DateTime> billDateList = (List<DateTime>)new SocietyBillService(this.ModelState).ListBillDatesBySocietySubscriptionID(id, SocietyReceipt.BillAbbreviation).OrderByDescending(billDate => billDate.Date).ToList();
                var lastbilldate = new SocietyBillService(this.ModelState).GetLastBillDateBySocietySubscriptionIDBillAbbreviation(SocietyReceipt.SocietySubscriptionID, SocietyReceipt.BillAbbreviation);
                //            ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (billDateList.Count == 0 ? SocietyReceipt.SocietySubscription.SubscriptionStart : billDateList.First()) ? true : false);
                ViewBag.AllowToDelete = (SocietyReceipt.ReceiptDate > (lastbilldate == null ? SocietyReceipt.SocietySubscription.SubscriptionStart : lastbilldate));
                return View(SocietyReceipt);
            }
        }

        // GET: Method to Print Receipt added By Ranjit        
        public ActionResult Print(Guid id) //id=SocietyReceiptID
        {
            return View(_service.GetById(id));
        }

        public JsonResult ListForMember(String billabbreviation, Guid societybuildingunitId, Guid societymemberId)
        {
            var SocietyReceiptList = new SelectList(_service.ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(billabbreviation, societybuildingunitId, societymemberId) as System.Collections.IEnumerable, "SocietyReceiptID", "SocietyReceiptDetails");
            return Json(SocietyReceiptList, JsonRequestBehavior.AllowGet);
        }

        class receiptamounts
        {
            public Nullable<decimal> PrincipalAdjusted;
            public Nullable<decimal> InterestAdjusted;
            public Nullable<decimal> NonChgAdjusted;
            public Nullable<decimal> TaxAdjusted;
            public Nullable<decimal> Advance;
            public string PayModeCode;
            public string PayRefNo;
            public string PayRefDate;
            public Nullable<System.Guid> BankID;
            public string Branch;
        }

        public JsonResult Get(Guid id)  // SocietyReceiptID
        {
            var societyreceipt = _service.GetById(id);
            var receiptamt = new receiptamounts();
            receiptamt.PrincipalAdjusted = societyreceipt.PrincipalAdjusted;
            receiptamt.InterestAdjusted = societyreceipt.InterestAdjusted;
            receiptamt.NonChgAdjusted = societyreceipt.NonChgAdjusted;
            receiptamt.TaxAdjusted = societyreceipt.TaxAdjusted;
            receiptamt.Advance = societyreceipt.Advance;
            receiptamt.PayModeCode = societyreceipt.PayModeCode;
            receiptamt.PayRefNo = societyreceipt.PayRefNo;
            receiptamt.PayRefDate = String.Format("{0:dd-MMM-yyyy}", societyreceipt.PayRefDate);
            receiptamt.BankID = societyreceipt.BankID;
            receiptamt.Branch = societyreceipt.Branch;
            return Json(receiptamt, JsonRequestBehavior.AllowGet);
        }

        // GET : All Hold receipt transactions
        [HttpGet]
        public ActionResult HoldReceipts(Guid id)
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
            ViewBag.CreateReceiptStatusMessage = TempData["CreateReceiptStatusMessage"];
            return View(_service.GetOnholdReceipts(societySubscription.SocietyID, id));
        }
        [HttpPost]
        public ActionResult CreateReceipt(SocietyReceiptOnhold objSocietyReceiptOnhold)
        {
            if (null == objSocietyReceiptOnhold)
            {
                TempData["CreateReceiptStatusMessage"] = "Failed to generated Receipt.";
                return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
            }
            if (null == objSocietyReceiptOnhold.SocietyID || null == objSocietyReceiptOnhold.SocietySubscriptionID)
            {
                TempData["CreateReceiptStatusMessage"] = "Failed to generated Receipt.";
                return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
            }
            bool result = true;
            try
            {
                if (objSocietyReceiptOnhold.SocietyReceiptOnholdID.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                {
                    var OnholdReceipts = _service.GetOnholdReceipts(objSocietyReceiptOnhold.SocietyID, objSocietyReceiptOnhold.SocietySubscriptionID);
                    foreach (var OnholdReceipt in OnholdReceipts)
                    {
                        try
                        {
                            _service.GenerateReceiptForOnHoldReciept(OnholdReceipt.SocietyReceiptOnholdID);
                        }
                        catch (Exception)
                        {
                            result = false;
                            continue;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = _service.GenerateReceiptForOnHoldReciept(objSocietyReceiptOnhold.SocietyReceiptOnholdID);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }
                //Create Actual Receipt
                if (result)
                {
                    TempData["CreateReceiptStatusMessage"] = "Receipt generated successfully.";
                    return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["CreateReceiptStatusMessage"] = "Failed to generated Receipt.";
                    return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                TempData["CreateReceiptStatusMessage"] = "Failed to generated Receipt.";
                return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
