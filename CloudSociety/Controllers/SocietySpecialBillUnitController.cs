using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietySpecialBillUnitController : Controller
    {
        private SocietySpecialBillUnitService _service;
        const string _exceptioncontext = "Society Special Bill Charge Head Controller";
        public SocietySpecialBillUnitController()
        {
            _service = new SocietySpecialBillUnitService(this.ModelState);
        }

        public ActionResult Index(Guid societySpecialBillID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
            return View(_service.ListByParentId(societySpecialBillID));
        }

        public ActionResult Create(Guid societySpecialBillID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
            ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            return View();
        }

        [HttpPost]
        public ActionResult Create(Guid societySpecialBillID, Guid societySubscriptionID, SocietySpecialBillUnit SocietySpecialBillUnitToCreate)
        {
            try
            {
                if (_service.Add(SocietySpecialBillUnitToCreate))
                {
                    return RedirectToAction("Index", new { societySpecialBillID = societySpecialBillID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
                    ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
                ViewBag.SocietyBuildingUnitList = new SocietyBuildingUnitService(this.ModelState).ListBuildingUnitBySocietyID(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                return View();
            }
        }

        //public ActionResult Edit(Guid societySpecialBillID, Guid societyBuildingUnitID, Guid societySubscriptionID) //id=SocietySpecialBillID
        //{
        //    ViewBag.SocietySubscriptionID = societySubscriptionID;
        //    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
        //    return View(_service.GetByIds(societySpecialBillID, societyBuildingUnitID));
        //}

        

        public ActionResult Delete(Guid societySpecialBillID, Guid societyBuildingUnitID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetByIds(societySpecialBillID, societyBuildingUnitID));
        }

        [HttpPost]
        public ActionResult Delete(Guid societySpecialBillID, Guid societyBuildingUnitID, Guid societySubscriptionID, SocietySpecialBillUnit SocietySpecialBillUnitToDelete)   // (int id, FormCollection collection)
        {
            try
            {
                if (_service.Delete(SocietySpecialBillUnitToDelete))
                    return RedirectToAction("Index", new { societySpecialBillID = societySpecialBillID, societySubscriptionID = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View(_service.GetByIds(societySpecialBillID, societyBuildingUnitID));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View(_service.GetByIds(societySpecialBillID, societyBuildingUnitID));
            }
        }

    }
}
