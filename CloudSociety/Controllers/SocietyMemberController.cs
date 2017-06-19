using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser,Member,OfficeBearer")]
    public class SocietyMemberController : Controller
    {
        private SocietyMemberService _service;
        const string _exceptioncontext = "SocietyMember Controller";
        public SocietyMemberController()
        {
            _service = new SocietyMemberService(this.ModelState);
        }
        // GET: /To display list of Society Member added by Ranjit
        public ActionResult Index(Guid id) // id = SocietySubscriptionID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View(_service.ListByParentId(societySubscriptionService.GetById(id).SocietyID));
        }
        // GET: /To display list of Society Member filtered by Name added by Baji 24-Jun-16
        public ActionResult IndexWithSearch(Guid id, string Name = "") // id = SocietySubscriptionID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            IEnumerable<SocietyMember> SocietyMemberList = _service.ListByParentId(societySubscriptionService.GetById(id).SocietyID);
            if (Name != "")
                SocietyMemberList = SocietyMemberList.Where(a => a.Member.ToLower().IndexOf(Name.ToLower().Trim()) != -1);
            return View("Index",SocietyMemberList);
        }

        // GET: /To display Detail list of Society Member added by Ranjit
        public ActionResult Details(Guid id, Guid societySubscriptionID) // id = SocietyMemberID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            SocietyMember SocietyMember = _service.GetById(id);
            var userDetails = new UserDetailService(this.ModelState).GetBySocietyMemberID(id);
            if (userDetails != null)
            {
                var user = Membership.GetUser((object)userDetails.UserID);
                if (user != null)
                {
                    //SocietyMember.Email = user.Email;
                    SocietyMember.PasswordAnswer = "******";
                    SocietyMember.Password = "******";
                    SocietyMember.PasswordQuestion = user.PasswordQuestion;
                    SocietyMember.OfficeBearer = Roles.IsUserInRole(user.UserName, "OfficeBearer");
                }
            }
            return View(SocietyMember);
        }
        // GET: /To Create Society Member added by Ranjit
        public ActionResult Create(Guid id) //  id = SocietySubscriptionID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.MemberClassList = new MemberClassService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            ViewBag.OccupationList = new OccupationService(this.ModelState).List();
            return View();
        }
        // POST: /To Create Society Member added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyMember SocietyMemberToCreate)
        {
            try
            {
                if (_service.Add(SocietyMemberToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietySubscriptionID = id;
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.MemberClassList = new MemberClassService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    ViewBag.OccupationList = new OccupationService(this.ModelState).List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietySubscriptionID = id;
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                ViewBag.MemberClassList = new MemberClassService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                ViewBag.OccupationList = new OccupationService(this.ModelState).List();
                return View();
            }
        }
        // GET: /Edit Society Member added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) // id = SocietyMemberID 
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.MemberClassList = new MemberClassService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            ViewBag.OccupationList = new OccupationService(this.ModelState).List();
            return View(_service.GetById(id));
        }
        // POST: /Edit Society Member added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyMember SocietyMemberToEdit) // id = SocietyMemberID
        {
            try
            {
                if (_service.Edit(SocietyMemberToEdit))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.MemberClassList = new MemberClassService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    ViewBag.OccupationList = new OccupationService(this.ModelState).List();
                    return View(SocietyMemberToEdit);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.MemberClassList = new MemberClassService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                ViewBag.OccupationList = new OccupationService(this.ModelState).List();
                return View(SocietyMemberToEdit);
            }
        }
        // GET: /Delete Society Member added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) // id = SocietyMemberID 
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            SocietyMember SocietyMember = _service.GetById(id);
            var userDetails = new UserDetailService(this.ModelState).GetBySocietyMemberID(id);
            if (userDetails != null)
            {
                var user = Membership.GetUser((object)userDetails.UserID);
                if (user != null)
                {
                    //SocietyMember.Email = user.Email;
                    SocietyMember.OfficeBearer = Roles.IsUserInRole(user.UserName, "OfficeBearer");
                }
            }
            return View(SocietyMember);
        }
        // POST: /Delete Society Member added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyMember SocietyMemberToDelete) // id = SocietyMemberID
        {
            SocietyMember SocietyMember = _service.GetById(id);
            SocietyMember.EmailId = SocietyMemberToDelete.EmailId;
            SocietyMember.OfficeBearer = SocietyMemberToDelete.OfficeBearer;
            try
            {
                if (SocietyMember.EmailId != null ? _service.DeleteWithMemberShipUser(SocietyMember) : _service.Delete(SocietyMember))
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View(SocietyMember);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View(SocietyMember);
            }
        }
        // GET: /Manage LogIn add by Ranjit
        [HttpGet]
        public ActionResult ManageLogIn(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
            ViewBag.OfficeBearer = false;
            var userDetails = new UserDetailService(this.ModelState).GetBySocietyMemberID(id);
            if (userDetails != null)
            {
                MembershipUser mUser = Membership.GetUser((object)userDetails.UserID);
                Models.RegisterModel RegisterModel = new Models.RegisterModel();
                RegisterModel.UserName = mUser.UserName;
                RegisterModel.Email = mUser.Email;
                RegisterModel.PasswordQuestion = mUser.PasswordQuestion;
                RegisterModel.PasswordAnswer = "";
                RegisterModel.Password = "";
                RegisterModel.ConfirmPassword = "";
                ViewBag.OfficeBearer = Roles.IsUserInRole(mUser.UserName, "OfficeBearer");
                return View(RegisterModel);
            }
            return View();
        }
        // POST: /Manage LogIn add by Ranjit
        [HttpPost]
        public ActionResult ManageLogIn(Guid id, Guid societySubscriptionID, bool OfficeBearer, Models.RegisterModel RegisterModel)
        {
            try
            {
                if (_service.ManageLogIn(id, OfficeBearer, RegisterModel))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
                    ViewBag.OfficeBearer = OfficeBearer;
                    return View(RegisterModel);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
                ViewBag.OfficeBearer = OfficeBearer;
                return View(RegisterModel);
            }
        }
        // GET: /Manage OfficeBearer add by Ranjit
        [HttpGet]
        public ActionResult ManageRole(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.OfficeBearer = false;
            var userDetails = new UserDetailService(this.ModelState).GetBySocietyMemberID(id);
            if (userDetails != null)
            {
                MembershipUser mUser = Membership.GetUser((object)userDetails.UserID);
                if (mUser != null)
                    ViewBag.OfficeBearer = Roles.IsUserInRole(mUser.UserName, "OfficeBearer");
            }
            return View();
        }
        // POST: /Manage OfficeBearer add by Ranjit
        [HttpPost]
        public ActionResult ManageRole(Guid id, Guid societySubscriptionID, bool OfficeBearer)
        {
            try
            {
                if (_service.ManageRole(id, OfficeBearer))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
                    ViewBag.OfficeBearer = OfficeBearer;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
                ViewBag.OfficeBearer = OfficeBearer;
                return View();
            }
        }

        public JsonResult ListForBuildingUnit(Guid societybuildingunitid) 
        {
            var SocietyMemberList = new SelectList(_service.ListBySocietyBuildUnitID(societybuildingunitid) as System.Collections.IEnumerable, "SocietyMemberID", "Member");
            return Json(SocietyMemberList, JsonRequestBehavior.AllowGet); 
        }

        //Show menu added by Nityananda
        //public ActionResult Menu(Guid id)
        //{
        //    ViewBag.SocietyMemberID = id;
        //    var societyMemberService = new CloudSociety.Services.SocietyMemberService(this.ModelState);
        //    ViewBag.ShowSocietyMemberMenu = true;
        //    //ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
        //    //ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
        //    //ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societyMemberService.GetById(id).Society.CreatedByID);
        //    //ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
        //    //ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
        //    return View();
        //}
        public ActionResult Menu(Guid id)   // societysubscriptionid
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowMemberMenu = true;
            MembershipUser user = Membership.GetUser();
            Guid userid = (Guid)user.ProviderUserKey;
            var societymemberid = (Guid)new Services.UserDetailService(this.ModelState).GetById(userid).SocietyMemberID;
            var societyMemberService = new CloudSociety.Services.SocietyMemberService(this.ModelState);
            ViewBag.MemberName = societyMemberService.GetById(societymemberid).Member;
            var societySubscription =  societySubscriptionService.GetById(id);
            ViewBag.ShowFinalReports = societySubscription.Closed;
            //Session["ShowCommunication"] = societyMemberService.IsCommunicationEnabled(societySubscription.SocietyID);
            ViewBag.ShowCommunication = societyMemberService.IsCommunicationEnabled(societySubscription.SocietyID);
            return View();
        }

        //MemberRegisterReport added by Nityananda
        //[HttpGet]
        //public ActionResult ReportMessage(Guid societyMemberID, String ControllerName, String ActionName)
        //{
        //    ViewBag.SocietyMemberID = societyMemberID;
        //    ViewBag.ActionName = ActionName;
        //    ViewBag.ControllerName = ControllerName;
        //    var societyMemberService = new CloudSociety.Services.SocietyMemberService(this.ModelState);
        //    var societyMember = societyMemberService.GetById(societyMemberID);
        //    ViewBag.ShowSocietyMemberMenu = true;
        //    //ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(societySubscriptionID);
        //    //ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(societySubscriptionID);
        //    ViewBag.SocietyHead = societyMember.Society.Name;
        //    //ViewBag.Header =
        //    //ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societyMember.Society.SocietySubscriptions);
        //    //ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
        //    // ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(societySubscriptionID);
        //    return View();
        //}
    }
}
