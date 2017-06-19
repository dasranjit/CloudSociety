using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyBuildingController : Controller
    {
        private SocietyBuildingService _service;
        const string _exceptioncontext = "Society Building Controller";
        public SocietyBuildingController()
        {
            _service = new SocietyBuildingService(this.ModelState);
        }

        // GET: /To display list of Society Building added by Ranjit
        public ActionResult Index(Guid id) // id= SocietySubscriptionID
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

        // GET: /To Create Society Building added by Ranjit
        public ActionResult Create(Guid id) // id= SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            return View();
        }

        // POST: /To Create Society Building added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyBuilding societyBuildingToCreate) // id= SocietySubscriptionID
        {
            try
            {
                if (_service.Add(societyBuildingToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
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
                return View();
            }
        }

        // GET: /Edit Society Building added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) // id= SocietyBuildingID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /Edit Society Building added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyBuilding societyBuildingToEdit) //id = SocietyBuildingID
        {
            try
            {
                if (_service.Edit(societyBuildingToEdit))
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
        // GET: /Delete Society Building added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) // id= SocietyBuildingID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /Delete Society Building added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyBuilding societyBuildingToDelete) //id = SocietyBuildingID
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
