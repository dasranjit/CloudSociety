using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CloudSociety.Models;
using CloudSociety.Services;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Net.Mail;

namespace CloudSociety.Controllers
{
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            base.Initialize(requestContext);
        }

        private bool SendMail(string userId, string url, string emailId, string password)
        {
            SmtpClient mailClient = new SmtpClient();
            MailMessage message = new MailMessage();
            string mailBody, mailFrom, bcc;

            message.IsBodyHtml = true;
            try
            {
                mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"];
                if (string.IsNullOrEmpty(mailFrom))
                    return true;
                bcc = System.Configuration.ConfigurationManager.AppSettings["Bcc"];
                message.From = new MailAddress(mailFrom);
                message.To.Add(new MailAddress(emailId));
                message.Bcc.Add(new MailAddress(bcc));
                message.Subject = "Welcome to Cloud Society";
                mailBody = new CloudSociety.Services.AppInfoService(this.ModelState).Get().TrialMailBody;
                mailBody = mailBody.Replace("&&Url&&", url);
                mailBody = mailBody.Replace("&&UserId&&", userId);
                mailBody = mailBody.Replace("&&Password&&", password);
                message.Body = mailBody;
                if (!string.IsNullOrEmpty(mailFrom))
                    mailClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("SendMail", ex.Message + ", " + ex.InnerException.Message);
                return false;
            }
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    MembershipUser user = Membership.GetUser(model.UserName);
                    if (Roles.FindUsersInRole("TrialUser", model.UserName).ToList().Count > 0)
                    {

                        var UserDetail = new UserDetailService(this.ModelState).GetById((Guid)user.ProviderUserKey);
                        if (UserDetail.LoginExpiryDate < DateTime.Now)
                        {
                            user.IsApproved = false;
                            Membership.UpdateUser(user);
                            Response.Write("<script language='javascript'>alert('Your trial subscription has expired : ACCESS DENIED !!');window.location='../Home';</script>");
                            return View(model);
                        }
                    }
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        // return RedirectToAction("Index", "Home");
                        if (Roles.FindUsersInRole("Admin", model.UserName).ToList().Count > 0)   // if (Roles.IsUserInRole("Admin"))
                            return RedirectToAction("AdminIndex", "Home");
                        if (Roles.FindUsersInRole("CompanyAccount", model.UserName).ToList().Count > 0)
                            return RedirectToAction("Index", "SubscriptionInvoice");
                        if (Roles.RoleExists("Support"))
                            if (Roles.FindUsersInRole("Support", model.UserName).ToList().Count > 0)
                                return RedirectToAction("IndexForSupport", "Society");
                        if (Roles.FindUsersInRole("TrialUser", model.UserName).ToList().Count > 0)
                        {
                            //var user = Membership.GetUser(model.UserName);
                            Guid userid = (Guid)user.ProviderUserKey;
                            var societysubscriptionid = new Services.SocietySubscriptionService(this.ModelState).GetForCreatedByID(userid).SocietySubscriptionID;
                            return RedirectToAction("Menu", "SocietySubscription", new { id = societysubscriptionid });
                        }
                        //if (Roles.FindUsersInRole("Member", model.UserName).ToList().Count > 0)
                        //{
                        //    Guid userid = (Guid)user.ProviderUserKey;
                        //    var societymemberid = new Services.UserDetailService(this.ModelState).GetById(userid).SocietyMemberID;
                        //    return RedirectToAction("Menu", "SocietyMember", new { id = societymemberid });                           
                        //}
                        if ((Roles.FindUsersInRole("Member", model.UserName).ToList().Count > 0) || (Roles.FindUsersInRole("OfficeBearer", model.UserName).ToList().Count > 0))
                        {
                            Guid userid = (Guid)user.ProviderUserKey;
                            var societyid = new Services.UserDetailService(this.ModelState).GetById(userid).SocietyMember.Society.SocietyID;
                            return RedirectToAction("Select", "SocietySubscription", new { id = societyid });
                        }
                        else
                            return RedirectToAction("Index", "Society");
                    }
                }
                else
                {
                    if (Membership.FindUsersByName(model.UserName).Count > 0)
                    {
                        if (Roles.FindUsersInRole("TrialUser", model.UserName).ToList().Count > 0)
                        {
                            Response.Write("<script language='javascript'>alert('Your trial subscription has expired : ACCESS DENIED !!');window.location='../Home';</script>");
                        }
                        else
                        {
                            Response.Write("<script language='javascript'>alert('Unauthorized User : ACCESS DENIED !!');window.location='../Home';</script>");
                        }
                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('The user name or password provided is incorrect.');window.location='../Home';</script>");
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        //public ActionResult Register()
        //{
        //    ViewBag.PasswordLength = MembershipService.MinPasswordLength;
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Attempt to register the user
        //        MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

        //        if (createStatus == MembershipCreateStatus.Success)
        //        {
        //            FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewBag.PasswordLength = MembershipService.MinPasswordLength;
        //    return View(model);
        //}

        [Authorize(Roles = "Admin,Subscriber,SocietyAdmin,CompanyAdmin")]
        public ActionResult Register(string Role)
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
            ViewBag.Role = Role;
            return View();
        }

        [Authorize(Roles = "Admin,Subscriber,SocietyAdmin,CompanyAdmin")]
        [HttpPost]
        public ActionResult Register(RegisterModel model, string Role)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.PasswordQuestion, model.PasswordAnswer, model.Email); //(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Roles.AddUserToRole(model.UserName, Role);
                    //                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    if (Role == "SocietyAdmin" || Role == "SocietyUser")
                    {
                        var user = Membership.GetUser();
                        var userdetailservice = new UserDetailService(this.ModelState);
                        var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                        if (userdetail != null)
                        {
                            var newuser = Membership.GetUser(model.UserName);
                            var ud = new UserDetail();
                            ud.UserID = (Guid)newuser.ProviderUserKey;
                            ud.SubscriberID = userdetail.SubscriberID;
                            ud.Mobile = model.Mobile;
                            if (!userdetailservice.Add(ud))
                            {
                                this.ModelState.AddModelError("UserDetail", GenericExceptionHandler.ExceptionMessage());
                            }
                        }
                    }
                    if (Role == "CompanyAdmin" || Role == "CompanyAccount" || Role == "TrainingUser")
                        return RedirectToAction("AdminIndex", "Home");
                    else
                        return RedirectToAction("Index", "Society");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
            return View(model);
        }

        public ActionResult RegisterTrialUser(string Back)
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
            ViewBag.Role = Back;    // Role is TrialUser, so use this to get back setting
            return View("Register");
        }

        [HttpPost]
        public ActionResult RegisterTrialUser(RegisterModel model, string Role) // Role is actually Back setting
        {
            if (ModelState.IsValid)
            {
                var Back = Role;
                Role = "TrialUser";
                if (String.IsNullOrEmpty(model.Mobile))
                {
                    this.ModelState.AddModelError("", "The Mobile Can't be blank.");
                }
                else
                {
                // Attempt to register the user
                    MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.PasswordQuestion, model.PasswordAnswer, model.Email); //(model.UserName, model.Password, model.Email);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        Roles.AddUserToRole(model.UserName, Role);
                        var appInfo = new CloudSociety.Services.AppInfoService(this.ModelState).Get();
                        var userdetailservice = new UserDetailService(this.ModelState);
                        var newuser = Membership.GetUser(model.UserName);
                        var ud = new UserDetail();
                        ud.UserID = (Guid)newuser.ProviderUserKey;
                        ud.LoginExpiryDate = DateTime.Today.AddDays((double)appInfo.TrialPeriod);
                        ud.Mobile = model.Mobile;
                        if (!userdetailservice.Add(ud))
                        {
                            this.ModelState.AddModelError("UserDetail", GenericExceptionHandler.ExceptionMessage());
                            if (Back == "Home")
                                return RedirectToAction("Index", "Home");
                            else
                                return RedirectToAction("Index");
                        }
                        var societyservice = new SocietyService(this.ModelState);
                        societyservice.InsertForTrialUser(ud.UserID, model.UserName);
                        string url = System.Configuration.ConfigurationManager.AppSettings["URL"];
                        if (!SendMail(model.UserName, url, model.Email, model.Password))
                        {
                            this.ModelState.AddModelError("ProblemWithMailSending", GenericExceptionHandler.ExceptionMessage());
                        }
                        return RedirectToAction("CreateTrialUserSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
            return View("Register", model);
        }

        public ActionResult CreateTrialUserSuccess()
        {
            var appInfo = new CloudSociety.Services.AppInfoService(this.ModelState).Get();
            ViewBag.NoOfMembers = appInfo.TrainingMembersPerSociety;
            return View();
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult Index(string role)
        {
            ViewBag.Role = role;
            Guid? subscriberid = null;
            if (role != "Subscriber")
            {
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                if (userdetail != null)
                {
                    var subscriberservice = new SubscriberService(this.ModelState);
                    subscriberid = userdetail.SubscriberID;
                    var subscriber = subscriberservice.GetById((Guid)subscriberid);
                    ViewBag.Subscriber = subscriber;
                }
            }
            var userservice = new UserService();

            return View(userservice.ListForRoleAndSubscriberID(role, subscriberid));
        }

        public ActionResult UnLock(String username, String role)
        {
            Membership.Provider.UnlockUser(username);
            return RedirectToAction("Index", new { role = role });
        }

        public ActionResult Approve(Guid id, Boolean approve, String role)
        {
            var user = Membership.GetUser(id);
            user.IsApproved = approve;
            Membership.UpdateUser(user);
            return RedirectToAction("Index", new { role = role });
        }

        [HttpGet]
        public ActionResult Terms()
        {
            var appInfo = new CloudSociety.Services.AppInfoService(this.ModelState).Get();
            var terms = appInfo.TrialTerms;
            return View(appInfo);
        }

        [HttpPost]
        public ActionResult Terms(bool flag)
        {
            if (flag)
                return RedirectToAction("RegisterTrialUser", new { Back = "Home" });
            else
                return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/ResetPassword
        // **************************************

        public ActionResult AskUserName()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AskUserName(string userName)
        {
            if (Membership.Provider.GetUser(userName, false) != null)
            {
                if (Membership.Provider.GetUser(userName, false).IsLockedOut)
                {
                    Response.Write("<script language='javascript'>alert('Your Account has been locked !!');window.location='../Home';</script>");
                    return View();
                }
                else
                    return RedirectToAction("ResetPassword", new { userName = userName });
            }
            else
            {
                Response.Write("<script language='javascript'>alert('Given User Name does not exist !!');window.location='../Home';</script>");
                return View();
            }
        }

        public ActionResult ResetPassword(string userName)
        {
            ViewBag.userName = userName;
            if (MembershipService.PasswordQuestion(userName) != null)
                ViewBag.PasswordQuestion = MembershipService.PasswordQuestion(userName);
            else
                ViewBag.PasswordQuestion = "Sorry You have not set your password question !!";
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel rpm)
        {
            if (!Membership.Provider.GetUser(rpm.UserName, false).IsLockedOut)
            {
                if (ModelState.IsValid)
                {
                    if (MembershipService.ResetPassword(rpm.UserName, rpm.PasswordAnswer, rpm.NewPassword))
                    {
                        ViewBag.ResetStatus = true;
                        return View("ResetStatus");
                    }
                    else
                    {
                        ViewBag.ResetStatus = false;
                        return View("ResetStatus");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            else
                Response.Write("<script language='javascript'>alert('Your Account has been locked !!');window.location='../Home';</script>");
            return View(rpm);
        }
    }
}
