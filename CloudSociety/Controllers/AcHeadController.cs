using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;
using System.Linq;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class AcHeadController : Controller
    {
        private AcHeadService _service;
        const string _exceptioncontext = "AcHead Controller";
        private static readonly IDictionary<string, string> _nature = new Dictionary<string, string>() { { "C", "Cash" }, { "B", "Bank" }, { "S", "Creditor" }, { "D", "Debtor" }, { "T", "TDS" }, { "F", "Year End Closing Transfer" } };

        public AcHeadController()
        {
            _service = new AcHeadService(this.ModelState);
        }

        // GET: /To display list of AcHead added by Ranjit
        public ActionResult Index(Guid id)  //id=SocietySubscriptionID
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
            return View(_service.ListById(societySubscription.SocietyID));
        }

        public ActionResult IndexWithSearch(Guid id, string Name = "", string SubCategory = "")  //id=SocietySubscriptionID
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
            IEnumerable<AcHead> AcHeadList = _service.ListById(societySubscription.SocietyID);
            if (Name != "")
                AcHeadList = AcHeadList.Where(a => a.Name.ToLower().IndexOf(Name.ToLower().Trim()) != -1);
            if (SubCategory != "")
                AcHeadList = AcHeadList.Where(a => a.AcSubCategory.SubCategory.ToLower().IndexOf(SubCategory.ToLower().Trim()) != -1);
            return View(AcHeadList);
        }

        // GET: /To display Detail list of Society Member added by Ranjit
        public ActionResult Details(Guid id, Guid societySubscriptionID, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetByIds(societySubscription.SocietyID, id));
        }

        // GET: /To Create AcHead  added by Ranjit
        public ActionResult Create(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.AcSubCategoryList = new AcSubCategoryService(this.ModelState).ListById(ViewBag.SocietyID);
            ViewBag.TDSCategoryList = new TDSCategoryService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            //ViewBag.Countries = new CountryService().List();                    
            return View();
        }


        // POST: /To Create AcHead  added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, AcHead acHeadToCreat)
        {
            try
            {
                if (_service.Add(acHeadToCreat))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.AcSubCategoryList = new AcSubCategoryService(this.ModelState).ListById(ViewBag.SocietyID);
                    ViewBag.TDSCategoryList = new TDSCategoryService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    //ViewBag.Countries = new CountryService().List();                    
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
                ViewBag.AcSubCategoryList = new AcSubCategoryService(this.ModelState).ListById(ViewBag.SocietyID);
                ViewBag.TDSCategoryList = new TDSCategoryService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                //ViewBag.Countries = new CountryService().List();                    
                return View();
            }
        }

        // POST: /Edit AcHead added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.AcSubCategoryList = new AcSubCategoryService(this.ModelState).ListById(societySubscription.SocietyID);
            ViewBag.TDSCategoryList = new TDSCategoryService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            //ViewBag.Countries = new CountryService().List();
            return View(_service.GetByIds(societySubscription.SocietyID, id));
        }


        // POST: /Edit AcHead added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, string backUrl, AcHead acHeadToUpdate)
        {
            try
            {
                if (_service.Edit(acHeadToUpdate))
                {
                    return RedirectPermanent(backUrl);
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.AcSubCategoryList = new AcSubCategoryService(this.ModelState).ListById(societySubscription.SocietyID);
                    ViewBag.TDSCategoryList = new TDSCategoryService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    //ViewBag.Countries = new CountryService().List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.AcSubCategoryList = new AcSubCategoryService(this.ModelState).ListById(societySubscription.SocietyID);
                ViewBag.TDSCategoryList = new TDSCategoryService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                //ViewBag.Countries = new CountryService().List();
                return View();
            }
        }

        // POST: /Delete AcHead added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetByIds(societySubscriptionService.GetById(societySubscriptionID).SocietyID, id));
        }


        // POST: /Delete AcHead added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, string backUrl, AcHead AcHeadToDelete)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            try
            {
                if (_service.Delete(_service.GetByIds(societySubscriptionService.GetById(societySubscriptionID).SocietyID, id)))
                {
                    return RedirectPermanent(backUrl);
                }
                else
                {
                    ViewBag.BackUrl = backUrl;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(_service.GetByIds(societySubscriptionService.GetById(societySubscriptionID).SocietyID, id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.BackUrl = backUrl;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetByIds(societySubscriptionService.GetById(societySubscriptionID).SocietyID, id));
            }
        }


    }
}
