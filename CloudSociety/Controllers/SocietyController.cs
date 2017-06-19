using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;
using System.Linq;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin,Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyController : Controller
    {
        private SocietyService _service;
        const string _exceptioncontext = "Society Controller";
        private static readonly IDictionary<string, string> _accountPosting =
            new Dictionary<string, string>() { { "S", "Summary" }, { "D", "Details" }, { "N", "No" } };//Summary, Details, No

        // GET: /Society/
        public SocietyController()
        {
            _service = new SocietyService(this.ModelState);
        }
        [HttpGet]
        public ActionResult IndexForSupport(string Name = "", string RegistrationNo = "", string Subscriber = "")
        {
            IEnumerable<Society> SocietyList = _service.List();
            if (Name != "")
                SocietyList = SocietyList.Where(s => s.Name.ToLower().IndexOf(Name.ToLower().Trim()) != -1);
            if (RegistrationNo != "")
                SocietyList = SocietyList.Where(s => (s.RegistrationNo != null ? s.RegistrationNo.ToLower().IndexOf(RegistrationNo.ToLower().Trim()) != -1 : false));
            if (Subscriber != "")
                SocietyList = SocietyList.Where(s => (s.Subscriber != null ? s.Subscriber.Name.ToLower().IndexOf(Subscriber.ToLower().Trim()) != -1 : false));
            return View(SocietyList);
        }

        public ActionResult Index()
        {
            IEnumerable<Society> list = null;
            try
            {
                ViewBag.ShowChangeProfile = false;
                ViewBag.ShowSubscription = false;
                ViewBag.ShowCreateAdminUser = false;
                ViewBag.ShowCreateSocietyUser = false;
                ViewBag.ShowCreateCompanyUser = false;

                bool companyadminuser = false, subscriberuser = false, societyadminuser = false;
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                if (userdetail != null)
                    ViewBag.SubscriberID = userdetail.SubscriberID;

                if (Roles.IsUserInRole("Subscriber"))
                    subscriberuser = true;
                else if (Roles.IsUserInRole("CompanyAdmin"))
                    companyadminuser = true;
                else if (Roles.IsUserInRole("SocietyAdmin"))
                    societyadminuser = true;

                if (subscriberuser)
                {
                    ViewBag.ShowSubscriberMenu = true;
                    ViewBag.ShowChangeProfile = true;
                    ViewBag.ShowSubscription = true;
                    ViewBag.ShowCreateAdminUser = true;
                    ViewBag.ShowCreateSocietyUser = true;
                    list = _service.ListForSubscriber((Guid)userdetail.SubscriberID);
                }
                else if (societyadminuser)
                {
                    ViewBag.ShowSubscriberMenu = true;
                    ViewBag.ShowCreateSocietyUser = true;
                    list = _service.ListForSubscriber((Guid)userdetail.SubscriberID);
                }
                else if (companyadminuser)
                {
                    ViewBag.ShowSubscriberMenu = true;
                    ViewBag.ShowSubscription = true;
                    ViewBag.ShowCreateCompanyUser = true;
                    list = _service.ListForCompany();
                }
                else
                    list = _service.ListAllocated((Guid)user.ProviderUserKey);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Index", "Error Generating Society List " + ex.Message + ", " + ex.InnerException.Message);
                //View(list);
            }
            return View(list);
        }

        //public ActionResult Operate(Guid id)        // Is this required?
        //{
        //    return View();
        //}

        //GET : /Method to update Society Details Added by Ranjit
        public ActionResult Edit(Guid id)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.States = new StateService().List();
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View(_service.GetById(societySubscriptionService.GetById(id).SocietyID));
        }

        // POST: /Method to update Society Details Added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Society SocietyToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                if (_service.Edit(SocietyToUpdate))
                    return RedirectToAction("Menu", "SocietySubscription", new { id = id });
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietySubscriptionID = id;
                    ViewBag.ShowSocietyMenu = true;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
                    ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
                    ViewBag.States = new StateService().List();
                    ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                    ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
                    return View(_service.GetById(societySubscriptionService.GetById(id).SocietyID));
                }

            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietySubscriptionID = id;
                ViewBag.ShowSocietyMenu = true;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
                ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
                ViewBag.States = new StateService().List();
                ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
                return View(_service.GetById(societySubscriptionService.GetById(id).SocietyID));
            }
        }
        
        //GET : /Method to update Society BillingParam Added by Ranjit
        public ActionResult Param(Guid id)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListBySocietyIDNature(societySubscription.SocietyID, "D");// ListById(societySubscription.SocietyID);
            ViewBag.AccountPostingList = new SelectList(_accountPosting, "Key", "Value");
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View(_service.GetById(societySubscription.SocietyID));
        }

        // POST: /Method to update Society BillingParam Added by Ranjit
        [HttpPost]
        public ActionResult Param(Guid id, Society SocietyToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                if (_service.Edit(SocietyToUpdate))
                    return RedirectToAction("Menu", "SocietySubscription", new { id = id });
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(id);
                    ViewBag.SocietySubscriptionID = id;
                    ViewBag.ShowSocietyMenu = true;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
                    ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
                    ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                    ViewBag.AccountPostingList = new SelectList(_accountPosting, "Key", "Value");
                    ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                    ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
                    return View(_service.GetById(societySubscription.SocietyID));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(id);
                ViewBag.SocietySubscriptionID = id;
                ViewBag.ShowSocietyMenu = true;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
                ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
                ViewBag.AcHeadList = new AcHeadService(this.ModelState).ListById(societySubscription.SocietyID);
                ViewBag.AccountPostingList = new SelectList(_accountPosting, "Key", "Value");
                ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
                ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
                return View(_service.GetById(societySubscription.SocietyID));
            }
        }

        [HttpGet]
        public ActionResult Configurations(string SocietyName = "")
        {
            var societyList = _service.List();
            if (null != societyList)
            {
                //ViewBag.SocietyNameList = societyList.Where(i=>i.Active).Select(i => i.Name).ToArray();

                if (!string.IsNullOrWhiteSpace(SocietyName))
                {
                    societyList = societyList.Where(i => i.Name.ToLower().Contains(SocietyName.ToLower()) && i.Active).OrderBy(i=>i.Name);
                }
                else
                {
                    societyList = societyList.Where(i => i.Active).OrderBy(i => i.Name);
                }
            }

            return View(societyList);
        }

        [HttpGet]
        public ActionResult EditConfigurations(Guid SocietyID)
        {
            var society = _service.GetById(SocietyID);
            SocietyCommunicationSetting societyCommunicationSetting = null;
            if (null != society)
            {
                if (null != society.SocietyCommunicationSettings)
                {
                    societyCommunicationSetting = society.SocietyCommunicationSettings.FirstOrDefault();
                }
                if (null == societyCommunicationSetting)
                {
                    societyCommunicationSetting = new SocietyCommunicationSetting { SocietyID = society.SocietyID };
                }
                societyCommunicationSetting.IsBillingSMSActive = society.SMS;
                societyCommunicationSetting.ShowPaymentLink = society.ShowPaymentLink;
                societyCommunicationSetting.PaymentGatewayLink = society.PaymentGatewayLink;
                societyCommunicationSetting.TransDelayHour = society.TransDelayHour;
            }
            return View(societyCommunicationSetting);
        }

        [HttpPost]
        public ActionResult EditConfigurations(Guid SocietyID, SocietyCommunicationSetting objSocietyCommunicationSetting)
        {
            objSocietyCommunicationSetting.SocietyID = SocietyID;
            if (_service.EditConfigurations(objSocietyCommunicationSetting))
            {
                return RedirectToAction("Configurations");
            }
            else
            {
                return View(objSocietyCommunicationSetting);
            }
        }
    }
}
