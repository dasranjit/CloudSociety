using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyBuildingUnitChargeHeadController : Controller
    {
        private SocietyBuildingUnitChargeHeadService _service;        
        const string _exceptioncontext = "Society Building Unit Charge Head Controller";
        public SocietyBuildingUnitChargeHeadController()
        {            
            _service = new SocietyBuildingUnitChargeHeadService(this.ModelState);
        }

        // GET: /To display list of SocietyBuildingUnitChargeHead added by Ranjit
        public ActionResult Index(Guid societyBuildingUnitID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);            
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);     
            return View(_service.ListByParentId(societyBuildingUnitID));
        }

        // GET: /To Create SocietyBuildingUnitChargeHead added by Ranjit
        public ActionResult Create(Guid societyBuildingUnitID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            //ViewBag.SocietyID = societySubscription.SocietyID;            
            ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            return View();
        }

        // POST: /To Create SocietyBuildingUnitChargeHead added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid societyBuildingUnitID, Guid societySubscriptionID, SocietyBuildingUnitChargeHead SocietyBuildingUnitChargeHeadToCreate)
        {
            try
            {
                if (_service.Add(SocietyBuildingUnitChargeHeadToCreate))
                {
                    return RedirectToAction("Index", new { societyBuildingUnitID = societyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    //ViewBag.SocietyID = societySubscription.SocietyID;
                    ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                    ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                    return View();
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                //ViewBag.SocietyID = societySubscription.SocietyID;
                ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                return View();
            }
        }

        // GET: /Edit SocietyBuildingUnitChargeHead added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id=SocietyBuildingUnitChargeHead
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            var societyBuildingUnitChargeHead = new CloudSociety.Services.SocietyBuildingUnitChargeHeadService(this.ModelState).GetById(id);
            ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
            ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitChargeHead.SocietyBuildingUnitID);            
            return View(_service.GetById(id));
        }
        // POST: /Edit SocietyBuildingUnitChargeHead added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyBuildingUnitChargeHead SocietyBuildingUnitChargeHeadToEdit)
        {
            var societyBuildingUnitChargeHead = _service.GetById(id);
            try
            {
                if (_service.Edit(SocietyBuildingUnitChargeHeadToEdit))
                {
                    return RedirectToAction("Index", new { societyBuildingUnitID = societyBuildingUnitChargeHead.SocietyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);                    
                    ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitChargeHead.SocietyBuildingUnitID);
                    return View(_service.GetById(id));                 
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyChargeHeadList = new SocietyChargeHeadService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitChargeHead.SocietyBuildingUnitID);
                return View(_service.GetById(id)); 
            }
        }
        // GET: /Delete SocietyBuildingUnitChargeHead added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) //id=SocietyBuildingUnitChargeHead
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID; 
            var SocietyBuildingUnitChargeHead =_service.GetById(id);
            var SocietySubscriptionService =new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = SocietySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyChargeHead = new SocietyChargeHeadService(this.ModelState).GetByIds(SocietySubscriptionService.GetById(societySubscriptionID).SocietyID, SocietyBuildingUnitChargeHead.ChargeHeadID).ChargeHead;
            return View(SocietyBuildingUnitChargeHead);
        }
        // POST: /Delete SocietyBuildingUnitChargeHead added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyBuildingUnitChargeHead SocietyBuildingUnitChargeHeadToDelete)
        {
            var societyBuildingUnitChargeHead = _service.GetById(id);
            try
            {
                if (_service.Delete(societyBuildingUnitChargeHead))
                {
                    return RedirectToAction("Index", new { societyBuildingUnitID = societyBuildingUnitChargeHead.SocietyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
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
