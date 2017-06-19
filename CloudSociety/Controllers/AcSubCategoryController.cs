using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class AcSubCategoryController : Controller
    {
        private AcSubCategoryService _service;
        const string _exceptioncontext = "AcSubCategory Controller";
        public AcSubCategoryController()
        {           
            _service = new AcSubCategoryService(this.ModelState);            
        }

        // GET: /To display list of AcSubCategory added by Ranjit
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

        // GET: /To Create AcSubCategory  added by Ranjit
        public ActionResult Create(Guid id)  //id=SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);            
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.AcCategoryList = new AcCategoryService(this.ModelState).ListById(ViewBag.SocietyID);
            return View();
        }

        // POST: To Create AcSubCategory added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, AcSubCategory AcSubCategoryToCreate) //id=SocietySubscriptionID
        {
            try
            {
                if (_service.Add(AcSubCategoryToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.AcCategoryList = new AcCategoryService(this.ModelState).ListById(ViewBag.SocietyID);
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
                ViewBag.AcCategoryList = new AcCategoryService(this.ModelState).ListById(ViewBag.SocietyID);
                return View();
            }
        }

        // GET: /Edit Society AcSubCategory added by Ranjit
        public ActionResult Edit(Guid id, string backUrl, Guid societySubscriptionID) 
        {
            ViewBag.BackUrl = backUrl; 
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.AcCategoryList = new AcCategoryService(this.ModelState).ListById(societySubscription.SocietyID);
            return View(_service.GetByIds(societySubscription.SocietyID,id));
        }

        // POST: /Edit Society AcSubCategory added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, string backUrl, AcSubCategory AcSubCategoryToUpdate)
        {
            try
            {
                if (_service.Edit(AcSubCategoryToUpdate))
                    return RedirectPermanent(backUrl);
                else
                {

                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.AcCategoryList = new AcCategoryService(this.ModelState).ListById(societySubscription.SocietyID);
                    return View(_service.GetByIds(societySubscription.SocietyID, id));
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.AcCategoryList = new AcCategoryService(this.ModelState).ListById(societySubscription.SocietyID);
                return View(_service.GetByIds(societySubscription.SocietyID, id));
            }
        }
        // GET: /Delete Society AcSubCategory added by Ranjit
        public ActionResult Delete(Guid id, string backUrl, Guid societySubscriptionID)
        {
            ViewBag.BackUrl = backUrl;
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            return View(_service.GetByIds(societySubscription.SocietyID, id));
        }

        // POST: /Delete Society AcSubCategory added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, string backUrl, AcSubCategory AcSubCategoryToDelete)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);            
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            try
            {
                if (_service.Delete(_service.GetByIds(societySubscription.SocietyID, id)))
                    return RedirectPermanent(backUrl);
                else
                {

                    ViewBag.BackUrl = backUrl;
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    return View(_service.GetByIds(societySubscription.SocietyID, id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.BackUrl = backUrl;
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                return View(_service.GetByIds(societySubscription.SocietyID, id));
            }
        }
    }
}
