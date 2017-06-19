using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Collections.Generic;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyChargeHeadController : Controller
    {
        private SocietyChargeHeadService _service;
        const string _exceptioncontext = "Society ChargeHead Controller";
        private static readonly IDictionary<string, string> _Nature =
            new Dictionary<string, string>() { { "C", "Construction Cost Basis" }, { "A", "Per Area" }, { "L", "Late Payment Penalty" }, { "E", "Early Payment Discount" }, { "I", "Interest" }, { "T", "Tax" } };
        public SocietyChargeHeadController()
        {
            _service = new SocietyChargeHeadService(this.ModelState);
        }

        // GET: /To display list of SocietyChargeHead added by Ranjit
        public ActionResult Index(Guid id) // id = SocietySubscriptionID
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

        // GET: /To Create SocietyChargeHead added by Ranjit
        public ActionResult Create(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscription.SocietyID;
            ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
            ViewBag.SocietyBillSeriesList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            return View();
        }

        // GET: /To Create SocietyChargeHead added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyChargeHead SocietyChargeHeadToCreate) // id = SocietySubscriptionID
        {
            try
            {
                if (_service.Add(SocietyChargeHeadToCreate))
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
                    ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                    ViewBag.SocietyBillSeriesList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                    return View();
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(id);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyID = societySubscription.SocietyID;
                ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                ViewBag.SocietyBillSeriesList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                return View();
            }
        }

        // GET: /Edit SocietyChargeHead added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID, string backUrl) // id = ChargeHeadID 
        {
            ViewBag.BackUrl = backUrl; 
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
            ViewBag.SocietyBillSeriesList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            return View(_service.GetByIds(societySubscription.SocietyID, id));
        }

        // POST: /Edit Society Member added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, string backUrl, SocietyChargeHead SocietyChargeHeadToEdit) // id = ChargeHeadID 
        {
            try
            {
                if (_service.Edit(SocietyChargeHeadToEdit))
                {
                    return RedirectPermanent(backUrl);
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.BackUrl = backUrl; 
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                    ViewBag.SocietyBillSeriesList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                    return View(_service.GetByIds(societySubscription.SocietyID, id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.BackUrl = backUrl; 
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                ViewBag.SocietyBillSeriesList = new SocietyBillSeriesService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                return View(_service.GetByIds(societySubscription.SocietyID, id));
            }
        }
        // GET: /Delete SocietyChargeHead added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID, string backUrl) // id = ChargeHeadID 
        {
            ViewBag.BackUrl = backUrl;
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);           
            return View(_service.GetByIds(societySubscription.SocietyID, id));
        }

        // POST: /Delete Society Member added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, string backUrl, SocietyChargeHead SocietyChargeHeadToDelete) // id = ChargeHeadID 
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            try
            {
                if (_service.Delete(_service.GetByIds(societySubscription.SocietyID, id)))
                {
                    return RedirectPermanent(backUrl);
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.BackUrl = backUrl;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);                  
                    return View(_service.GetByIds(societySubscription.SocietyID, id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.BackUrl = backUrl;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetByIds(societySubscription.SocietyID, id));
            }
        }
    }
}
