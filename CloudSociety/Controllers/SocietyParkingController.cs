using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyParkingController : Controller
    {
        private SocietyParkingService _service;
        const string _exceptioncontext = "SocietyParking Controller";
        public SocietyParkingController()
        {
            _service = new SocietyParkingService(this.ModelState);
        }

        // GET: /To display list of Society Parking added by Ranjit
        public ActionResult Index(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View(_service.ListByParentId(societySubscriptionService.GetById(id).SocietyID));
        }

        // GET: /To Create Society Parking added by Ranjit
        public ActionResult Create(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ParkingTypeList = new ParkingTypeService(this.ModelState).List();
            return View();
        }

        // POST: /To Create Society Parking added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyParking SocietyParkingToCreate)
        {
            try
            {
                if (_service.Add(SocietyParkingToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.ParkingTypeList = new ParkingTypeService(this.ModelState).List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.ParkingTypeList = new ParkingTypeService(this.ModelState).List();
                return View();
            }
        }

        // GET: /Edit Society Parking added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id=SocietyParkingID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyID = societySubscriptionService.GetById(societySubscriptionID).SocietyID;
            ViewBag.ParkingTypeList = new ParkingTypeService(this.ModelState).List();
            return View(_service.GetById(id));
        }

        // POST: /Edit Society Parking added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyParking SocietyParkingToEdit)
        {
            try
            {
                if (_service.Edit(SocietyParkingToEdit))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyID = societySubscriptionService.GetById(societySubscriptionID).SocietyID;
                    ViewBag.ParkingTypeList = new ParkingTypeService(this.ModelState).List();
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyID = societySubscriptionService.GetById(societySubscriptionID).SocietyID;
                ViewBag.ParkingTypeList = new ParkingTypeService(this.ModelState).List();
                return View(_service.GetById(id));
            }
        }

        // GET: /Delete Society Parking added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) //id=SocietyParkingID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /Delete Society Parking added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyParking SocietyParkingToDelete)
        {
            try
            {
                if (_service.Delete(_service.GetById(id)))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
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
