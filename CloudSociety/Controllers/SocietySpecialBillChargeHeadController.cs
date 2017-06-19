using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietySpecialBillChargeHeadController : Controller
    {
        private SocietySpecialBillChargeHeadService _service;
        const string _exceptioncontext = "Society Special Bill Charge Head Controller";
        public SocietySpecialBillChargeHeadController()
        {
            _service = new SocietySpecialBillChargeHeadService(this.ModelState);
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
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
            return View();
        }

        [HttpPost]
        public ActionResult Create(Guid societySpecialBillID, Guid societySubscriptionID, SocietySpecialBillChargeHead SocietySpecialBillChargeHeadToCreate)
        {
            try
            {
                if (_service.Add(SocietySpecialBillChargeHeadToCreate))
                {
                    return RedirectToAction("Index", new { societySpecialBillID = societySpecialBillID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                    ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillID);
                return View();
            }
        }

        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id=SocietySpecialBillChargeHead
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            var societySpecialBillChargeHead = _service.GetById(id);
            ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillChargeHead.SocietySpecialBillID);
            return View(_service.GetById(id));
        }

        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietySpecialBillChargeHead SocietySpecialBillChargeHeadToEdit)
        {
            var societySpecialBillChargeHead = _service.GetById(id);
            try
            {
                if (_service.Edit(SocietySpecialBillChargeHeadToEdit))
                {
                    return RedirectToAction("Index", new { societySpecialBillID = societySpecialBillChargeHead.SocietySpecialBillID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillChargeHead.SocietySpecialBillID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                ViewBag.SocietySpecialBill = new SocietySpecialBillService(this.ModelState).GetById(societySpecialBillChargeHead.SocietySpecialBillID);
                return View(_service.GetById(id));
            }
        }
        public ActionResult Delete(Guid id, Guid societySubscriptionID) //id=SocietySpecialBillChargeHeadID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            var SocietySpecialBillChargeHead = _service.GetById(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyChargeHead = new SocietyChargeHeadService(this.ModelState).GetByIds(societySubscription.SocietyID, SocietySpecialBillChargeHead.ChargeHeadID).AcHead.Name;
            return View(SocietySpecialBillChargeHead);
        }

        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietySpecialBillChargeHead SocietySpecialBillChargeHeadToDelete)
        {
            var SocietySpecialBillChargeHead = _service.GetById(id);
            try
            {
                if (_service.Delete(SocietySpecialBillChargeHead))
                {
                    return RedirectToAction("Index", new { societySpecialBillID = SocietySpecialBillChargeHead.SocietySpecialBillID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyChargeHead = new SocietyChargeHeadService(this.ModelState).GetByIds(societySubscription.SocietyID, SocietySpecialBillChargeHead.ChargeHeadID).AcHead.Name;
                    return View(SocietySpecialBillChargeHead);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyChargeHead = new SocietyChargeHeadService(this.ModelState).GetByIds(societySubscription.SocietyID, SocietySpecialBillChargeHead.ChargeHeadID).AcHead.Name;
                return View(SocietySpecialBillChargeHead);
            }
        }
    }
}
