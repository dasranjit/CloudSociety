using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyPayModeController : Controller
    {
        private SocietyPayModeService _service;
        const string _exceptioncontext = "SocietyPayMode Controller";
        public SocietyPayModeController()
        {
            _service = new SocietyPayModeService(this.ModelState);
        }

        // GET: /To display list of SocietyPayMode added by Ranjit
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

        // GET: /To Create SocietyPayMode added by Ranjit
        public ActionResult Create(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(ViewBag.SocietyID);
            return View();
        }

        // POST: /To Create SocietyPayMode added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyPayMode SocietyPayModeToCreate)
        {
            try
            {
                if (_service.Add(SocietyPayModeToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(ViewBag.SocietyID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(ViewBag.SocietyID);
                return View();
            }
        }

        // GET: /Edit SocietyPayMode added by Ranjit
        public ActionResult Edit(string code, Guid societySubscriptionID) //code=PayModeCode
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
            return View(_service.GetByIdCode(societySubscription.SocietyID, code));
        }

        // POST: /Edit Society Building added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid societySubscriptionID, SocietyPayMode SocietyPayModeToEdit)
        {
            try
            {
                if (_service.Edit(SocietyPayModeToEdit))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                    return View(_service.GetByIdCode(societySubscription.SocietyID, SocietyPayModeToEdit.PayModeCode));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                return View(_service.GetByIdCode(societySubscription.SocietyID, SocietyPayModeToEdit.PayModeCode));
            }
        }
        // GET: /Delete SocietyPayMode added by Ranjit
        public ActionResult Delete(string code, Guid societySubscriptionID) //code=PayModeCode
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetByIdCode(societySubscription.SocietyID, code));
        }

        // POST: /Delete SocietyPayMode added by Ranjit
        [HttpPost]
        public ActionResult Delete(string code, Guid societySubscriptionID, SocietyPayMode SocietyPayModeToDelete)
        {            
            try
            {
                if (_service.Delete(SocietyPayModeToDelete))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(SocietyPayModeToDelete);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(SocietyPayModeToDelete);
            }
        }
    }
}
