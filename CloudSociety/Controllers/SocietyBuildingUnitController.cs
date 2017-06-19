using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;
using System.Collections.Generic;
using System.Linq;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyBuildingUnitController : Controller
    {
        private SocietyBuildingUnitService _service;            
        const string _exceptioncontext = "Society Building Unit Controller";
        
        public SocietyBuildingUnitController()
        {           
            _service = new SocietyBuildingUnitService(this.ModelState);
        }

        // GET: /To display list of Society Building Unit added by Ranjit
        public ActionResult Index(Guid societyBuildingID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societysubscriptionservice = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societysubscriptionservice.SocietyYear(societySubscriptionID);
            var societysubscription = societysubscriptionservice.GetById(societySubscriptionID);
            var cnt = _service.GetCountBySocietySubscriptionID(societySubscriptionID) ?? 0;
            ViewBag.ShowCreate = (cnt < societysubscription.NoOfMembers);
            ViewBag.SocietyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingID);
            ViewBag.YearOpen = !societysubscription.Closed;
            return View(_service.ListByParentId(societyBuildingID));
        }

        // GET: /To display list of Society Building Unit filtered by Name added by Baji 27-Jun-16
        public ActionResult IndexWithSearch(Guid societyBuildingID, Guid societySubscriptionID, string Unit = "")
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societysubscriptionservice = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societysubscriptionservice.SocietyYear(societySubscriptionID);
            var societysubscription = societysubscriptionservice.GetById(societySubscriptionID);
            var cnt = _service.GetCountBySocietySubscriptionID(societySubscriptionID) ?? 0;
            ViewBag.ShowCreate = (cnt < societysubscription.NoOfMembers);
            ViewBag.SocietyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingID);
            ViewBag.YearOpen = !societysubscription.Closed;
            IEnumerable<SocietyBuildingUnit> SocietyBuildingUnitList = _service.ListByParentId(societyBuildingID);
            if (Unit != "")
                SocietyBuildingUnitList = SocietyBuildingUnitList.Where(a => a.Unit.ToLower().IndexOf(Unit.ToLower().Trim()) != -1);
            return View("Index", SocietyBuildingUnitList);
        }

        // GET: /To display Detail list of Society Building Unit added by Ranjit
        public ActionResult Details(Guid id, Guid societySubscriptionID) // id = SocietyBuildingUnitID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            //ViewBag.SocietyMemberName = new SocietyMemberService(this.ModelState).GetBySocietyBuildingUnitId(id).Member;
            return View(_service.GetById(id));
        }

        // GET: /To Create Society Building Unit added by Ranjit
        public ActionResult Create(Guid societyBuildingID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.societyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingID);
            ViewBag.UnitType = new UnitTypeService(this.ModelState).List();
            return View();
        }

        // POST: /To Create Society Building Unit added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid societyBuildingID, Guid societySubscriptionID, SocietyBuildingUnit SocietyBuildingUnitToCreate)
        {
            try
            {
                if (_service.Add(SocietyBuildingUnitToCreate))
                {
                    return RedirectToAction("Index", new { societyBuildingID = societyBuildingID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.societyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingID);
                    ViewBag.UnitType = new UnitTypeService(this.ModelState).List();
                    return View();
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.societyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingID);
                ViewBag.UnitType = new UnitTypeService(this.ModelState).List();
                return View();
            }
        }

        // GET: /Edit Society Building Unit added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id = SocietyBuildingUnitID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.societyBuilding = new SocietyBuildingService(this.ModelState).GetById(new SocietyBuildingUnitService(this.ModelState).GetById(id).SocietyBuildingID);
            ViewBag.UnitType = new UnitTypeService(this.ModelState).List();
            return View(_service.GetById(id));
        }

        // POST: /Edit Society Building added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyBuildingUnit SocietyBuildingUnitToEdit) //id = SocietyBuildingUnitID
        {
            var societyBuildingUnit = new CloudSociety.Services.SocietyBuildingUnitService(this.ModelState).GetById(id);
            try
            {
                if (_service.Edit(SocietyBuildingUnitToEdit))
                {
                    return RedirectToAction("Index", new { societyBuildingID = societyBuildingUnit.SocietyBuildingID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.societyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingUnit.SocietyBuildingID);
                    ViewBag.UnitType = new UnitTypeService(this.ModelState).List();
                    return View(_service.GetById(id));
                }
            }
            catch(Exception ex)
            {               
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext+" "+ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.societyBuilding = new SocietyBuildingService(this.ModelState).GetById(societyBuildingUnit.SocietyBuildingID);
                ViewBag.UnitType = new UnitTypeService(this.ModelState).List();
                return View(_service.GetById(id));
            }
        }
        // GET: /Delete Society Building Unit added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) //id = SocietyBuildingUnitID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);           
            return View(_service.GetById(id));
        }

        // POST: /Delete Society Building added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyBuildingUnit SocietyBuildingUnitToDelete) //id = SocietyBuildingUnitID
        {
            var societyBuildingUnit = _service.GetById(id);
            try
            {
                if (_service.Delete(societyBuildingUnit))
                {
                    return RedirectToAction("Index", new { societyBuildingID = societyBuildingUnit.SocietyBuildingID, societySubscriptionID = societySubscriptionID });
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
