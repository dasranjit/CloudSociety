﻿using System;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyInvestmentController : Controller
    {
        private SocietyInvestmentService _service;
        const string _exceptioncontext = "SocietyInvestment Controller";
        public SocietyInvestmentController()
        {
            _service = new SocietyInvestmentService(this.ModelState);
        }

        // GET: /To display list of SocietyInvestment added by Ranjit
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

        // GET: /To Create SocietyInvestment added by Ranjit
        public ActionResult Create(Guid id) //id = SocietySubscriptionID
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
            ViewBag.BankList = new BankService(this.ModelState).List();
            return View();
        }

        // POST: /To Create SocietyInvestment added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid id, SocietyInvestment SocietyInvestmentToCreate)
        {
            try
            {
                if (_service.Add(SocietyInvestmentToCreate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = id;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.SocietyID = societySubscriptionService.GetById(id).SocietyID;
                    ViewBag.BankList = new BankService(this.ModelState).List();
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
                ViewBag.BankList = new BankService(this.ModelState).List();
                return View();
            }
        }

        // GET: /Edit SocietyInvestment added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id=SocietyInvestmentID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.BankList = new BankService(this.ModelState).List();
            return View(_service.GetById(id));
        }

        // POST: /Edit SocietyInvestment added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyInvestment SocietyInvestmentToEdit)//id=SocietyInvestmentID
        {
            try
            {
                if (_service.Edit(SocietyInvestmentToEdit))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.BankList = new BankService(this.ModelState).List();
                return View(_service.GetById(id));
            }
        }
        // GET: /Delete SocietyInvestment added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) //id=SocietyInvestmentID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.BankList = new BankService(this.ModelState).List();
            return View(_service.GetById(id));
        }

        // POST: /Delete SocietyInvestment added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyInvestment SocietyInvestmentToDelete)//id=SocietyInvestmentID
        {
            try
            {
                if (_service.Delete(_service.GetById(id)))
                {
                    return RedirectToAction("Index", new { id = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.BankList = new BankService(this.ModelState).List();
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.BankList = new BankService(this.ModelState).List();
                return View(_service.GetById(id));
            }
        }

    }
}
