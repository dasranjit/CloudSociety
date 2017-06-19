using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class AcCategoryController : Controller
    {
        private AcCategoryService _service;        
        const string _exceptioncontext = "AcCategory Controller";
        private static readonly IDictionary<string, string> _DrCr  =
           new Dictionary<string, string>() { { "D", "Debit" }, { "C", "Credit" } };
        private static readonly IDictionary<string, string> _Nature =
            new Dictionary<string, string>() { { "I", "Income Expenditure" }, { "B", "Balance Sheet" } };
        
        public AcCategoryController()
        {           
            _service = new AcCategoryService(this.ModelState);           
        }

        // GET: /To display list of AcCategory added by Ranjit
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
            return View(_service.ListByParentId(societySubscription.SocietyID));
        }

        // GET: /To Create AcCategory  added by Ranjit
        public ActionResult Create(Guid id)  //id=SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
            ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
            return View();
        }

        // POST: To Create AcCategory added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, AcCategory AcCategoryToCreate) //id=SocietySubscriptionID
        {
            try
            {
                if (_service.Add(AcCategoryToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                    ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                    return View();                  
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = id;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                return View();
            }
        }

        // GET: /Edit  AcCategory added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) 
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
            ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
            return View(_service.GetByIds(societySubscription.SocietyID,id));
        }


        // POST: /Edit AcCategory added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, AcCategory AcCategoryToUpdate)
        {
            try
            {
                if (_service.Edit(AcCategoryToUpdate))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                    ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                    return View(_service.GetByIds(societySubscription.SocietyID, id));
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                ViewBag.NatureList = new SelectList(_Nature, "Key", "Value");
                return View(_service.GetByIds(societySubscription.SocietyID, id));
            }
        }
        // GET: /Delete  AcCategory added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            return View(_service.GetByIds(societySubscription.SocietyID, id));
        }

        // POST: /Delete AcCategory added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, AcCategory AcCategoryToDelete)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            try
            {
                if (_service.Delete(_service.GetByIds(societySubscription.SocietyID, id)))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;                    
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(_service.GetByIds(societySubscription.SocietyID, id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;               
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetByIds(societySubscription.SocietyID, id));
            }
        }
    }
}
