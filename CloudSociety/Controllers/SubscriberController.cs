using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;
using CloudSociety.Models;
using System.Web.Security;

namespace CloudSociety.Controllers
{
    public class SubscriberController : Controller
    {
        private SubscriberService _service;
        private AccountMembershipService MembershipService;
        const string _exceptioncontext = "Subscriber Controller";
        public SubscriberController()
        {
            _service = new SubscriberService(this.ModelState);
            MembershipService = new AccountMembershipService();
        }
        //
        // GET: /Subscriber/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /Subscriber/Details/5

        [Authorize(Roles = "Admin")]
        public ActionResult Details(Guid id)
        {
            Subscriber subscriber;
            subscriber = _service.GetById(id);
            return View(subscriber);
        }

        //
        // GET: /Subscriber/Create

        public ActionResult Create()
        {
            var _stateservice = new StateService();
            var list = _stateservice.List();
            ViewBag.States = list;
            ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        //
        // POST: /Subscriber/Create

        [HttpPost]
        public ActionResult Create(Subscriber subscriberToCreate)   // [Bind(Exclude = "SubscriberID,CreatedOn")]
        {
            try
            {
                if (_service.Add(subscriberToCreate))
                    return RedirectToAction("CreateSuccess");
                else
                {
                    var _stateservice = new StateService();
                    ViewBag.States = _stateservice.List();
                    ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
                    ViewBag.PasswordLength = MembershipService.MinPasswordLength;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _stateservice = new StateService();
                ViewBag.States = _stateservice.List();
                ViewBag.PasswordQuestions = new CloudSociety.Services.PasswordQuestionService(this.ModelState).List();
                ViewBag.PasswordLength = MembershipService.MinPasswordLength;
                return View();
            }
        }

        public ActionResult CreateSuccess()
        {
            return View();
        }
        // GET: /Subscriber/Edit/5

        [Authorize(Roles = "Subscriber")]
        public ActionResult Edit(Guid id)
        {
            var _stateservice = new StateService();
            var list = _stateservice.List();
            ViewBag.States = list;
            var a = Membership.GetUser().Email;
            ViewBag.SubscriberEmailId = a;
            return View(_service.GetById(id));
        }

        //
        // POST: /Subscriber/Edit/5

        [Authorize(Roles = "Subscriber")]
        [HttpPost]
        public ActionResult Edit(Guid id, Subscriber subscriberToUpdate, string Email)    // FormCollection collection
        {
            try
            {
                if (_service.Edit(subscriberToUpdate))
                {
                    //code by Nityanada start 07 Mar 2013
                    MembershipUser user = Membership.GetUser();
                    user.Email = Email;
                    Membership.UpdateUser(user);
                    //code by Nityanada end
                    return RedirectToAction("Index", "Society");
                }
                else
                {
                    var _stateservice = new StateService();
                    ViewBag.States = _stateservice.List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _stateservice = new StateService();
                ViewBag.States = _stateservice.List();
                return View();
            }
        }

        //
        // GET: /Subscriber/Delete/5

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Subscriber/Delete/5

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            Subscriber Subscriber = _service.GetById(id);
            UserDetail UserDetail = new UserDetailService(this.ModelState).GetBySubscriberID(Subscriber.SubscriberID);
            try
            {
                if (_service.Delete(_service.GetById(id)))
                {
                    if (new UserDetailService(this.ModelState).Delete(UserDetail))
                    {
                        if (Membership.DeleteUser(Subscriber.UserName, true))
                            return RedirectToAction("Index");
                        else
                            return View(_service.GetById(id));
                    }
                    else
                        return View(_service.GetById(id));
                }
                else
                    return View(_service.GetById(id));

            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetById(id));
            }
        }

        [HttpGet]
        public ActionResult Terms()
        {
            ViewBag.AdminUser = Membership.GetUser();
            //            ViewBag.Terms = new CloudSociety.Services.AppInfoService(this.ModelState).Get().SubscriberTerms;
            var appinfo = new CloudSociety.Services.AppInfoService(this.ModelState).Get();
            return View(appinfo);
        }

        [HttpPost]
        public ActionResult Terms(bool flag)
        {
            if (flag)
                return RedirectToAction("Create");
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
