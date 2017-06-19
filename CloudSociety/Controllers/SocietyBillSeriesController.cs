using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Collections.Generic;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyBillSeriesController : Controller
    {
        private SocietyBillSeriesService _service;
        const string _exceptioncontext = "SocietyBillSeries Controller";
        private static readonly IDictionary<string, string> _frequencies =
            new Dictionary<string, string>() { { "M", "Monthly" }, { "B", "Bi-Monthly" }, { "Q", "Quarterly" }, { "H", "Half-Yearly" }, { "Y", "Yearly" } };
        private static readonly IDictionary<string, string> _oSAdjustment =
             new Dictionary<string, string>() { { "A", "Tax/Int/NonChg/Chg" }, { "B", "Tax/Chg/Int/NonChg" } };
        public SocietyBillSeriesController()
        {
            _service = new SocietyBillSeriesService(this.ModelState);
        }

        // GET: /To display list of SocietyBillSeries added by Ranjit
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
            ViewBag.YearOpen = !societySubscription.Closed;
            ViewBag.LockedTillDate = societySubscription.LockedTillDate;
            var societyId = societySubscription.SocietyID;
//            var list = _service.ListWithLastBillDateBySocietyID(societyId);
            var list = _service.ListWithLastDatesBySocietyID(societyId);
            return View(list);
        }

        // GET: /To Create SocietyBillSeries added by Ranjit
        [HttpGet]
        public ActionResult Create(Guid id) // id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.TaxList = new TaxService(this.ModelState).List();
            ViewBag.BillFrequenciesList = new SelectList(_frequencies, "Key", "Value");
            ViewBag.OSAdjustmentList = new SelectList(_oSAdjustment, "Key", "Value");
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(ViewBag.SocietyID);
            return View();
        }

        // GET: /To Create SocietyBillSeries added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyBillSeries SocietyBillSeriesToCreate)
        {
            try
            {
                if (_service.Add(SocietyBillSeriesToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.TaxList = new TaxService(this.ModelState).List();
                    ViewBag.BillFrequenciesList = new SelectList(_frequencies, "Key", "Value");
                    ViewBag.OSAdjustmentList = new SelectList(_oSAdjustment, "Key", "Value");
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
                ViewBag.TaxList = new TaxService(this.ModelState).List();
                ViewBag.BillFrequenciesList = new SelectList(_frequencies, "Key", "Value");
                ViewBag.OSAdjustmentList = new SelectList(_oSAdjustment, "Key", "Value");
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(ViewBag.SocietyID);
                return View();
            }
        }

        // GET: /To Edit SocietyBillSeries added by Ranjit
        [HttpGet]
        public ActionResult Edit(string code, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.TaxList = new TaxService(this.ModelState).List();
            //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
            ViewBag.BillFrequenciesList = new SelectList(_frequencies, "Key", "Value");
            ViewBag.OSAdjustmentList = new SelectList(_oSAdjustment, "Key", "Value");
            return View(_service.GetByIdCode(societySubscription.SocietyID, code));
        }


        // POST: /To Edit SocietyBillSeries added by Ranjit

        [HttpPost]
        public ActionResult Edit(string code, Guid societySubscriptionID, SocietyBillSeries SocietyBillSeriesToUpdate)
        {
            try
            {

                if (_service.Edit(SocietyBillSeriesToUpdate))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.TaxList = new TaxService(this.ModelState).List();
                    // ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                    ViewBag.BillFrequenciesList = new SelectList(_frequencies, "Key", "Value");
                    ViewBag.OSAdjustmentList = new SelectList(_oSAdjustment, "Key", "Value");
                    return View(_service.GetByIdCode(societySubscription.SocietyID, code));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.TaxList = new TaxService(this.ModelState).List();
                //ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                ViewBag.BillFrequenciesList = new SelectList(_frequencies, "Key", "Value");
                ViewBag.OSAdjustmentList = new SelectList(_oSAdjustment, "Key", "Value");
                return View(_service.GetByIdCode(societySubscription.SocietyID, code));
            }
        }


        // GET: /To Delete SocietyBillSeries added by Ranjit
        [HttpGet]
        public ActionResult Delete(string code, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetByIdCode(societySubscription.SocietyID, code));
        }


        // POST: /To Delete SocietyBillSeries added by Ranjit

        [HttpPost]
        public ActionResult Delete(string code, Guid societySubscriptionID, SocietyBillSeries SocietyBillSeriesToDelete)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            try
            {
                if (_service.Delete(_service.GetByIdCode(societySubscription.SocietyID, code)))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(_service.GetByIdCode(societySubscription.SocietyID, code));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetByIdCode(societySubscription.SocietyID, code));
            }
        }
    }
}
