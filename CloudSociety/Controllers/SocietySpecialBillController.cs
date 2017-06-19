using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietySpecialBillController : Controller
    {
        private SocietySpecialBillService _service;
        const string _exceptioncontext = "SocietySpecialBill Controller";
        public SocietySpecialBillController()
        {
            _service = new SocietySpecialBillService(this.ModelState);
        }

        // GET: /To display list of SocietySpecialBill added by Ranjit
        public ActionResult Index(Guid id) // id = SocietySubscriptionID
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
            ViewBag.LockedTillDate = societySubscription.LockedTillDate;
            return View(_service.ListByParentId(societySubscription.SocietyID));
        }

        // GET: /To Create SocietySpecialBill added by Ranjit
        public ActionResult Create(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscription.SocietyID;
            ViewBag.UnitTypeList = new UnitTypeService(this.ModelState).List();
//            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListByParentParentId(societySubscription.SocietyID);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscription.SocietyID);
            ViewBag.StartRange = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
            ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
            return View();
        }

        // POST: /To Create SocietySpecialBill added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietySpecialBill SocietySpecialBillToCreate)
        {
            try
            {

                if (_service.Add(SocietySpecialBillToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(id);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscription.SocietyID;
                    ViewBag.UnitTypeList = new UnitTypeService(this.ModelState).List();
//                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListByParentParentId(societySubscription.SocietyID);
                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscription.SocietyID);
                    ViewBag.StartRange = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                    ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(id);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyID = societySubscription.SocietyID;
                ViewBag.UnitTypeList = new UnitTypeService(this.ModelState).List();
                //ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListByParentParentId(societySubscription.SocietyID);
                ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscription.SocietyID);
                ViewBag.StartRange = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                return View();
            }
        }

        // GET: /To Edit SocietySpecialBill added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id = item.SocietySpecialBillID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.UnitTypeList = new UnitTypeService(this.ModelState).List();
//            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListByParentParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            ViewBag.StartRange = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
            ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
            return View(_service.GetById(id));
        }

        // POST: /To Edit SocietySpecialBill added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietySpecialBill SocietySpecialBillToUpdate) //id = item.SocietySpecialBillID
        {
            try
            {

                if (_service.Edit(SocietySpecialBillToUpdate))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.UnitTypeList = new UnitTypeService(this.ModelState).List();
//                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListByParentParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    ViewBag.StartRange = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                    ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.UnitTypeList = new UnitTypeService(this.ModelState).List();
//                ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListByParentParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                ViewBag.StartRange = (societySubscription.LockedTillDate == null ? societySubscription.SubscriptionStart : (DateTime)societySubscription.LockedTillDate.Value.AddDays(1));
                ViewBag.EndRange = (societySubscription.PaidTillDate == null ? societySubscription.SubscriptionEnd : societySubscription.PaidTillDate);
                return View(_service.GetById(id));
            }
        }
        // GET: /To Delete SocietySpecialBill added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) //id = item.SocietySpecialBillID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /To Delete SocietySpecialBill added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietySpecialBill SocietySpecialBillToDelete) //id = item.SocietySpecialBillID
        {
            try
            {

                if (_service.Delete(_service.GetById(id)))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View(_service.GetById(id));
            }
        }
    }
}
