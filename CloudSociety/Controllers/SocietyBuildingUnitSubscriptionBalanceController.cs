using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Collections.Generic;
//using System.Linq;

namespace CloudSociety.Controllers
{

    public class SocietyBuildingUnitSubscriptionBalanceController : Controller
    {
        private SocietyBuildingUnitSubscriptionBalanceService _service;
        const string _exceptioncontext = "SocietyBuildingUnitSubscriptionBalance Controller";
        public SocietyBuildingUnitSubscriptionBalanceController()
        {
            _service = new SocietyBuildingUnitSubscriptionBalanceService(this.ModelState);
        }

        // GET: /To display list of SocietyBuildingUnitSubscriptionBalance from Menu
        public ActionResult Index1(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View(_service.ListByParentId(id));
        }
        // GET: /To display list of SocietyBuildingUnit Op Balance
        public ActionResult Index2(Guid societyBuildingUnitID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            ViewBag.SocietyBuildingUnit = societyBuildingUnit;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(societyBuildingUnitID);
            return View(_service.ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(societyBuildingUnitID));
        }

        [HttpGet]
        public ActionResult AskBuildingUnit(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societyId = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societyId);
            return View();
        }
        [HttpPost]
        public ActionResult AskBuildingUnit(Guid id, Guid SocietyBuildingUnitID)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societyId = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            return RedirectToAction("Create1", new { id = id, societyBuildingUnitID = SocietyBuildingUnitID });
        }

        // GET: /To Create SocietyBuildingUnitSubscriptionBalance added by Ranjit
        public ActionResult Create1(Guid id, Guid societyBuildingUnitID)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societyId = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
            ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(societyBuildingUnitID);
            return View();
        }
        // Post: /To Create SocietyBuildingUnitSubscriptionBalance added by Ranjit
        [HttpPost]
        public ActionResult Create1(Guid id, SocietyBuildingUnitSubscriptionBalance SocietyBuildingUnitSubscriptionBalanceToCreate) // id = SocietySubscriptionID
        {
            try
            {
                if (_service.Add(SocietyBuildingUnitSubscriptionBalanceToCreate))
                {
                    return RedirectToAction("Index1", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societyId = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(SocietyBuildingUnitSubscriptionBalanceToCreate.SocietyBuildingUnitID);
                    ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
                    ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(SocietyBuildingUnitSubscriptionBalanceToCreate.SocietyBuildingUnitID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societyId = societySubscriptionService.GetById(id).SocietyID;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(SocietyBuildingUnitSubscriptionBalanceToCreate.SocietyBuildingUnitID);
                ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societyId);
                ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(SocietyBuildingUnitSubscriptionBalanceToCreate.SocietyBuildingUnitID);
                return View();
            }
        }
        
        // GET: /To Create SocietyBuildingUnitSubscriptionBalance added by Ranjit
        public ActionResult Create2(Guid societyBuildingUnitID, Guid societySubscriptionID) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            ViewBag.SocietyBuildingUnit = SocietyBuildingUnit;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(societyBuildingUnitID); //ListBySocietyBuildUnitID(societyBuildingUnitID);
            return View();
        }
        // Post: /To Create SocietyBuildingUnitSubscriptionBalance added by Ranjit
        [HttpPost]
        public ActionResult Create2(Guid societyBuildingUnitID, Guid societySubscriptionID, SocietyBuildingUnitSubscriptionBalance SocietyBuildingUnitSubscriptionBalanceToCreate) // id = SocietySubscriptionID
        {
            try
            {
                SocietyBuildingUnitSubscriptionBalanceToCreate.SocietySubscriptionID = null;
                if (_service.Add(SocietyBuildingUnitSubscriptionBalanceToCreate))
                {
                    return RedirectToAction("Index2", new { societyBuildingUnitID = societyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(societyBuildingUnitID); //ListBySocietyBuildUnitID(societyBuildingUnitID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(societyBuildingUnitID); //ListBySocietyBuildUnitID(societyBuildingUnitID);
                return View();
            }
        }

        // GET: /Edit SocietyBuildingUnitSubscriptionBalance added by Ranjit
        public ActionResult Edit1(Guid id, Guid societySubscriptionID)
        {
            var societyBuildingUnitSubscriptionBalance = _service.GetById(id);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            //ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(societyBuildingUnitSubscriptionBalance.SocietyBuildingUnitID); //ListBySocietyBuildUnitID(societyBuildingUnitSubscriptionBalance.SocietyBuildingUnitID);
            return View(societyBuildingUnitSubscriptionBalance);
        }
        // POST: /Edit SocietyBuildingUnitSubscriptionBalance added by Ranjit
        [HttpPost]
        public ActionResult Edit1(Guid id, Guid societySubscriptionID, SocietyBuildingUnitSubscriptionBalance SocietyBuildingUnitSubscriptionBalanceToEdit)
        {
            try
            {
                if (_service.Edit(SocietyBuildingUnitSubscriptionBalanceToEdit))
                {
                    return RedirectToAction("Index1", new { id = societySubscriptionID });
                }
                else
                {
                    var sbusb = _service.GetById(id);
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    //ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(sbusb.SocietyBuildingUnitID);//ListBySocietyBuildUnitID(sbusb.SocietyBuildingUnitID);
                    return View(sbusb);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var sbusb = _service.GetById(id);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                //ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitIDForNoOpeningBalance(sbusb.SocietyBuildingUnitID);
                return View(sbusb);
            }
        }

        // GET: /Edit SocietyBuildingUnitSubscriptionBalance added by Ranjit
        public ActionResult Edit2(Guid id, Guid societySubscriptionID)
        {
            var sbusb = _service.GetById(id);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            //ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(sbusb.SocietyBuildingUnitID);
            return View(sbusb);
        }
        // POST: /Edit SocietyBuildingUnitSubscriptionBalance added by Ranjit
        [HttpPost]
        public ActionResult Edit2(Guid id, Guid societySubscriptionID, SocietyBuildingUnitSubscriptionBalance SocietyBuildingUnitSubscriptionBalanceToEdit)
        {
            SocietyBuildingUnitSubscriptionBalanceToEdit.SocietySubscriptionID = null;
            try
            {
                if (_service.Edit(SocietyBuildingUnitSubscriptionBalanceToEdit))
                {
                    return RedirectToAction("Index2", new { societyBuildingUnitID = SocietyBuildingUnitSubscriptionBalanceToEdit.SocietyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    var sbusb = _service.GetById(id);
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    //ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(sbusb.SocietyBuildingUnitID);
                    return View(sbusb);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var sbusb = _service.GetById(id);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.BillAbbreviationList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                //ViewBag.SocietyMembersList = new SocietyMemberService(this.ModelState).ListBySocietyBuildUnitID(sbusb.SocietyBuildingUnitID);
                return View(sbusb);
            }
        }
        
        // GET: Delete SocietyBuildingUnitSubscriptionBalance added by Ranjit
        public ActionResult Delete1(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }
        // POST: Delete SocietyBuildingUnitSubscriptionBalance added by Ranjit
        [HttpPost]
        public ActionResult Delete1(Guid id, Guid societySubscriptionID, SocietyBuildingUnitSubscriptionBalance SocietyBuildingUnitSubscriptionBalanceToDelete)
        {
            try
            {
                if (_service.Delete(_service.GetById(id)))
                    return RedirectToAction("Index1", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetById(id));
            }
        }

        // GET: Delete SocietyBuildingUnitSubscriptionBalance added by Ranjit
        public ActionResult Delete2(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }
        // POST: Delete SocietyBuildingUnitSubscriptionBalance added by Ranjit
        [HttpPost]
        public ActionResult Delete2(Guid id, Guid societySubscriptionID, SocietyBuildingUnitSubscriptionBalance SocietyBuildingUnitSubscriptionBalanceToDelete)
        {
            //SocietyBuildingUnitSubscriptionBalanceToDelete.SocietySubscriptionID = null;

            try
            {
                if (_service.Delete(_service.GetById(id)))
                    return RedirectToAction("Index2", new { societyBuildingUnitID = SocietyBuildingUnitSubscriptionBalanceToDelete.SocietyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetById(id));
            }
        }
    }
}
