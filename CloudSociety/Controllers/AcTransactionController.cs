using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Collections.Generic;
//using System.Linq;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class AcTransactionController : Controller
    {
        private AcTransactionService _service;
        const string _exceptioncontext = "AcTransaction Controller";

        public AcTransactionController()
        {
            _service = new AcTransactionService(this.ModelState);
        }

        // GET: /To display list of AcTransaction added by Ranjit
        public ActionResult Index(Guid societySubscriptionID, string docType)
        {
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = SocietySubscriptionService.GetById(societySubscriptionID);
            IEnumerable<AcTransaction> AcTransactionIEList = _service.ListBySocietySubscriptionIDDocType(societySubscriptionID, docType);
            bool flag = false;
            if (docType == "OP")
            {
                List<AcTransaction> AcTransactionList = (List<AcTransaction>)AcTransactionIEList;
                if (AcTransactionList.Count == 0)
                {
                    AcTransaction AcTransaction = new AcTransaction();
                    AcTransaction.SocietyID = SocietySubscription.SocietyID;
                    AcTransaction.SocietySubscriptionID = societySubscriptionID;
                    AcTransaction.DocType = docType;
                    AcTransaction.DocDate = SocietySubscription.SubscriptionStart.AddDays(-1);
                    AcTransaction.Particulars = "Opening Balance";
                    //AcTransaction.AcHeadID = ?  
                    try
                    {
                        if (_service.Add(AcTransaction))
                        {
                            AcTransactionList = (List<AcTransaction>)_service.ListBySocietySubscriptionIDDocType(societySubscriptionID, docType);
                            if (AcTransactionList.Count == 1)//(AcTransactionIEList != null)
                            {
                                foreach (var AcTrans in AcTransactionList)
                                {
                                    return RedirectToAction("Index", "AcTransactionAc", new { AcTrans.AcTransactionID });
                                }
                            }
                            else
                                flag = true;
                        }
                        else
                            flag = true;
                    }
                    catch (Exception ex)
                    {
                        this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                        flag = true;
                    }
                }
                else
                {
                    foreach (var AcTrans in AcTransactionList)
                    {
                        return RedirectToAction("Index", "AcTransactionAc", new { AcTrans.AcTransactionID });
                    }
                }
            }
            else if (docType == "YC")
            {
                List<AcTransaction> AcTransactionList = (List<AcTransaction>)AcTransactionIEList;
                if (AcTransactionList.Count > 0)
                {
                    foreach (var AcTrans in AcTransactionList)
                    {
                        return RedirectToAction("Index", "AcTransactionAc", new { AcTrans.AcTransactionID });
                    }
                }
            }
            if (flag)
            {
                return RedirectToAction("Menu", "SocietySubscription", new { id = societySubscriptionID });
            }
            else
            {
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.DocType = docType;
                ViewBag.ShowSocietyMenu = true;
                ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.ShowBillingMenu = SocietySubscriptionService.BillingEnabled(societySubscriptionID);
                ViewBag.ShowAccountingMenu = SocietySubscriptionService.AccountingEnabled(societySubscriptionID);
                ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                ViewBag.IsPrevYearAccountingEnabled = SocietySubscriptionService.PrevYearAccountingEnabled(societySubscriptionID);
                ViewBag.YearClosed = SocietySubscription.Closed;
                ViewBag.LockedTillDate = SocietySubscription.LockedTillDate;
                return View(AcTransactionIEList);
            }
        }

        // GET: /To Create AcTransaction  added by Ranjit
        public ActionResult Create(Guid societySubscriptionID, string docType)
        {
            ViewBag.DocType = docType;
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = SocietySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietySubscription = SocietySubscription;
            ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(SocietySubscription.SocietyID);
            ViewBag.BankList = new BankService(this.ModelState).List();
            ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
            //ViewBag.StartRange = SocietySubscription.SubscriptionStart;
            ViewBag.StartRange = (SocietySubscription.LockedTillDate == null ? SocietySubscription.SubscriptionStart : (DateTime)SocietySubscription.LockedTillDate.Value.AddDays(1));
            if (docType != "JV")
            {
                var AcHeadService = new AcHeadService(this.ModelState);
                ViewBag.AcHeadList = AcHeadService.ListBySocietyIDNature(SocietySubscription.SocietyID, _service.GetAcNatureByDocType(docType));
            }
            return View();
        }
        // POST: To Create AcTransaction added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid societySubscriptionID, string docType, AcTransaction AcTransactionToCreate)
        {
            if (AcTransactionToCreate.DocType == "BP")
                AcTransactionToCreate.PayRefDate = AcTransactionToCreate.DocDate;
            try
            {
                if (_service.Add(AcTransactionToCreate))
                {
                    return RedirectToAction("Index", new { societySubscriptionID = societySubscriptionID, docType = docType });
                }
                else
                {
                    ViewBag.DocType = docType;
                    var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var SocietySubscription = SocietySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietySubscription = SocietySubscription;
                    ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(SocietySubscription.SocietyID);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
                    //ViewBag.StartRange = SocietySubscription.SubscriptionStart;
                    ViewBag.StartRange = (SocietySubscription.LockedTillDate == null ? SocietySubscription.SubscriptionStart : (DateTime)SocietySubscription.LockedTillDate.Value.AddDays(1));
                    if (docType != "JV")
                    {
                        var AcHeadService = new AcHeadService(this.ModelState);
                        ViewBag.AcHeadList = AcHeadService.ListBySocietyIDNature(SocietySubscription.SocietyID, _service.GetAcNatureByDocType(docType));
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.DocType = docType;
                var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var SocietySubscription = SocietySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietySubscription = SocietySubscription;
                ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(SocietySubscription.SocietyID);
                ViewBag.BankList = new BankService(this.ModelState).List();
                ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
                //ViewBag.StartRange = SocietySubscription.SubscriptionStart;
                ViewBag.StartRange = (SocietySubscription.LockedTillDate == null ? SocietySubscription.SubscriptionStart : (DateTime)SocietySubscription.LockedTillDate.Value.AddDays(1));
                if (docType != "JV")
                {
                    var AcHeadService = new AcHeadService(this.ModelState);
                    ViewBag.AcHeadList = AcHeadService.ListBySocietyIDNature(SocietySubscription.SocietyID, _service.GetAcNatureByDocType(docType));
                }
                return View();
            }
        }

        // GET: /Edit Society AcTransaction added by Ranjit
        public ActionResult Edit(Guid id)
        {
            var AcTransaction = new CloudSociety.Services.AcTransactionService(this.ModelState).GetById(id);
            var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var SocietySubscription = SocietySubscriptionService.GetById(AcTransaction.SocietySubscriptionID);
            ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
            ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(AcTransaction.SocietyID);
            ViewBag.BankList = new BankService(this.ModelState).List();
            ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
            //ViewBag.StartRange = SocietySubscription.SubscriptionStart;
            ViewBag.StartRange = (SocietySubscription.LockedTillDate == null ? SocietySubscription.SubscriptionStart : (DateTime)SocietySubscription.LockedTillDate.Value.AddDays(1));
            if (AcTransaction.DocType != "JV")
            {
                var AcHeadService = new AcHeadService(this.ModelState);
                ViewBag.AcHeadList = AcHeadService.ListBySocietyIDNature(AcTransaction.SocietyID, _service.GetAcNatureByDocType(AcTransaction.DocType));
            }
            return View(AcTransaction);
        }
        // POST: /Edit Society AcTransaction added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, AcTransaction AcTransactionToUpdate)
        {
            if (AcTransactionToUpdate.DocType == "BP")
                AcTransactionToUpdate.PayRefDate = AcTransactionToUpdate.DocDate;

            try
            {
                if (_service.Edit(AcTransactionToUpdate))
                    return RedirectToAction("Index", new { societySubscriptionID = AcTransactionToUpdate.SocietySubscriptionID, docType = AcTransactionToUpdate.DocType });
                else
                {
                    var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var SocietySubscription = SocietySubscriptionService.GetById(AcTransactionToUpdate.SocietySubscriptionID);
                    ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransactionToUpdate.SocietySubscriptionID);
                    ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(AcTransactionToUpdate.SocietyID);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
                    //ViewBag.StartRange = SocietySubscription.SubscriptionStart;
                    ViewBag.StartRange = (SocietySubscription.LockedTillDate == null ? SocietySubscription.SubscriptionStart : (DateTime)SocietySubscription.LockedTillDate.Value.AddDays(1));
                    if (AcTransactionToUpdate.DocType != "JV")
                    {
                        var AcHeadService = new AcHeadService(this.ModelState);
                        ViewBag.AcHeadList = AcHeadService.ListBySocietyIDNature(AcTransactionToUpdate.SocietyID, _service.GetAcNatureByDocType(AcTransactionToUpdate.DocType));
                    }
                    return View(AcTransactionToUpdate);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var SocietySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var SocietySubscription = SocietySubscriptionService.GetById(AcTransactionToUpdate.SocietySubscriptionID);
                ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(AcTransactionToUpdate.SocietySubscriptionID);
                ViewBag.PayModeList = new SocietyPayModeService(this.ModelState).ListByParentId(AcTransactionToUpdate.SocietyID);
                ViewBag.BankList = new BankService(this.ModelState).List();
                ViewBag.EndRange = (SocietySubscription.PaidTillDate == null ? SocietySubscription.SubscriptionEnd : SocietySubscription.PaidTillDate);
                //ViewBag.StartRange = SocietySubscription.SubscriptionStart;
                ViewBag.StartRange = (SocietySubscription.LockedTillDate == null ? SocietySubscription.SubscriptionStart : (DateTime)SocietySubscription.LockedTillDate.Value.AddDays(1));
                if (AcTransactionToUpdate.DocType != "JV")
                {
                    var AcHeadService = new AcHeadService(this.ModelState);
                    ViewBag.AcHeadList = AcHeadService.ListBySocietyIDNature(AcTransactionToUpdate.SocietyID, _service.GetAcNatureByDocType(AcTransactionToUpdate.DocType));
                }
                return View(AcTransactionToUpdate);
            }
        }

        // GET: /To display Details of AcTransaction added by Ranjit
        public ActionResult Details(Guid id)
        {
            AcTransaction AcTransaction = new CloudSociety.Services.AcTransactionService(this.ModelState).GetById(id);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(AcTransaction.SocietySubscriptionID);
            return View(AcTransaction);
        }

        // GET: CreateAcYearClosingEntry  added by Ranjit
        public ActionResult CreateAcYearClosingEntry(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.StatusMessage = "";
            return View();
        }
        [HttpPost]
        public ActionResult CreateAcYearClosingEntry(Guid id, FormCollection fc)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.ShowMessage = true;
            try
            {
                if (new CloudSociety.Services.SocietySubscriptionService(this.ModelState).CreateAcYearClosingEntry(id))
                {
                    ViewBag.StatusMessage = "Year Closing Entry has been created successfully.";
                    return View();
                }
                else
                {
                    ViewBag.StatusMessage = "Error : Year Closing Entry was not created !!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.StatusMessage = "Error : Year Closing Entry was not created !!";
                return View();
            }
        }

        // GET: DeleteAcYearClosingEntry  added by Ranjit
        public ActionResult DeleteAcYearClosingEntry(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.StatusMessage = "";
            return View();
        }
        [HttpPost]
        public ActionResult DeleteAcYearClosingEntry(Guid id, FormCollection fc)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.ShowMessage = true;
            try
            {
                if (new CloudSociety.Services.SocietySubscriptionService(this.ModelState).DeleteAcYearClosingEntry(id))
                {
                    ViewBag.StatusMessage = "Year Closing Entry has been deleted successfully.";
                    return View();
                }
                else
                {
                    ViewBag.StatusMessage = "Error : Year Closing Entry was not deleted !!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.StatusMessage = "Error : Year Closing Entry was not created !!";
                return View();
            }
        }

        // GET: Delete AcTransaction added by Ranjit
        public ActionResult Delete(Guid id)
        {
            AcTransaction AcTransaction = _service.GetById((Guid)id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
            IEnumerable<CloudSocietyEntities.AcTransactionAc> AcTransactionAcList = new AcTransactionAcService(this.ModelState).ListByParentId(id);
            ViewBag.AcTransactionAcList = AcTransactionAcList;
            return View(AcTransaction);
        }
        public ActionResult Print(Guid id)
        {
            return View(_service.GetById(id));
        }

        // POST: Delete AcTransaction added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, AcTransaction AcTransactionToDelete)
        {
            var AcTransaction = new AcTransactionService(this.ModelState).GetById(id); 
            try
            {
                if (_service.Delete(AcTransaction))
                    return RedirectToAction("Index", new { societySubscriptionID = AcTransaction.SocietySubscriptionID, docType = AcTransaction.DocType });
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
                    IEnumerable<CloudSocietyEntities.AcTransactionAc> AcTransactionAcList = new AcTransactionAcService(this.ModelState).ListByParentId(id);
                    ViewBag.AcTransactionAcList = AcTransactionAcList;
                    return View(AcTransaction);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(AcTransaction.SocietySubscriptionID);
                IEnumerable<CloudSocietyEntities.AcTransactionAc> AcTransactionAcList = new AcTransactionAcService(this.ModelState).ListByParentId(id);
                ViewBag.AcTransactionAcList = AcTransactionAcList;
                return View(AcTransaction);
            }
        }

    }
}
