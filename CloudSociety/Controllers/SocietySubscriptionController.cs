using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser,Member,OfficeBearer")]
    public class SocietySubscriptionController : Controller
    {
        private CloudSociety.Services.SocietySubscriptionService _service;
        const string _exceptioncontext = "SocietySubscription Controller";
        // GET: /SocietySubscription/
        public SocietySubscriptionController()
        {
            _service = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
        }
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        public ActionResult PayIndex()
        {
            var appinfoservice = new AppInfoService(this.ModelState);
            var appinfo = appinfoservice.Get();
            ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
            ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";

            IEnumerable<SocietySubscriptionWithServices> list = null;
            try
            {
                bool companyadminuser = false, subscriberuser = false;
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                if (userdetail != null)
                    ViewBag.SubscriberID = userdetail.SubscriberID;

                if (Roles.IsUserInRole("Subscriber"))
                    subscriberuser = true;
                else if (Roles.IsUserInRole("CompanyAdmin"))
                    companyadminuser = true;

                if (subscriberuser)
                {
                    list = _service.ListForInvoicingForSubscriber((Guid)userdetail.SubscriberID);
                }
                else if (companyadminuser)
                {
                    list = _service.ListForInvoicingForCompany();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ModelState.AddModelError("PayIndex", "Error Generating Society List");
                View(list);
            }
            return View(list);
        }
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        [HttpPost]
        public ActionResult PayIndex(FormCollection collection)
        {
            try
            {
                Guid? SubscriberID = null;
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                if (userdetail != null)
                    SubscriberID = userdetail.SubscriberID;
                String subscriptions = System.String.Empty;
                foreach (var item in collection)
                {
                    if (collection[item.ToString()] == "on")
                        subscriptions = (subscriptions == System.String.Empty ? "" : subscriptions + ",") + collection["ID+" + item.ToString().Substring(4)];
                }
                if (subscriptions != System.String.Empty)
                {
                    Guid? SubscriptionInvoiceID = null;
                    // create invoice for selected pending subscriptions
                    var subscriptioninvoiceservice = new SubscriptionInvoiceService(this.ModelState);
                    if (SubscriberID == null)
                        SubscriptionInvoiceID = subscriptioninvoiceservice.Create(subscriptions);
                    else
                        SubscriptionInvoiceID = subscriptioninvoiceservice.Create(subscriptions, (Guid)SubscriberID);
                    if (SubscriptionInvoiceID == null)
                        return RedirectToAction("PayIndex");
                    else
                        return RedirectToAction("Details", "SubscriptionInvoice", new { id = SubscriptionInvoiceID });
                }
                return RedirectToAction("Index", "Society");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View();
            }
        }
        public ActionResult Index(Guid id)
        {
            return View(_service.ListByParentId(id));
        }
        // GET: /SocietySubscription/Details/5
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        public ActionResult Details(int id)
        {
            return View();
        }
        //
        // GET: /SocietySubscription/Create
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        public ActionResult Create()
        {
            // Get Society Name & Subscription Details (Period, No of Members, No of Months) & Service Types
            // Save Creates Society, SocietySubscription & SocietySubscriptionServices
            // Sbsequently, Pay will show list of Societies (Not Active or Subscription.Months or AddlMems > 0 or Service.Status=P
            // For selected societies - Create Invoice & Shift from Pending to Invoiced in Subscription & SubscriptionServices
            // Below invoice, should show 2 payment options - Challan, PG
            // On successfull return from PG,activate corresponding societies & close invoice
            // On Payment input against invoice,activate corresponding societies & close invoice
            // Give separate Payment option against unpaid invoices individually

            var tariffrateservice = new TariffRateService(this.ModelState);
            ViewBag.Services = tariffrateservice.CurrentList();
            ViewBag.StateList = new StateService().List();
            var appinfoservice = new AppInfoService(this.ModelState);
            var appinfo = appinfoservice.Get();
            ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
            ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";
            return View();
        }
        //
        // POST: /SocietySubscription/Create
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        [HttpPost]
        public ActionResult Create(FormCollection collection)   // SocietySubscription societysubscriptiontocreate, 
        {
            var appinfoservice = new AppInfoService(this.ModelState);
            var appinfo = appinfoservice.Get();
            try
            {
                var user = Membership.GetUser();
                var userdetailservice = new UserDetailService(this.ModelState);
                var userdetail = userdetailservice.GetById((Guid)user.ProviderUserKey);
                var society = new Society();
                society.Name = collection["Society.Name"];
                society.Address = collection["Society.Address"];
                society.City = collection["Society.City"];
                society.PIN = decimal.Parse(collection["Society.PIN"]);
                society.StateID = Guid.Parse(collection["StateID"]);
                society.ContactPerson = collection["Society.ContactPerson"];
                society.Phone = collection["Society.Phone"];
                society.Mobile = collection["Society.Mobile"];
                society.uNoOfMembers = short.Parse(collection["NoOfAdditionalMembers"]);
                society.UOMID = (Guid)appinfo.UOMID;
                if (userdetail != null)
                    society.SubscriberID = userdetail.SubscriberID;
                var societyservice = new SocietyService(this.ModelState);
                if (societyservice.Add(society))
                {
                    var societysubscription = new SocietySubscription();
                    societysubscription.SocietyID = society.SocietyID;
                    societysubscription.NoOfAdditionalMembers = society.uNoOfMembers;
                    societysubscription.SubscriptionStart = DateTime.Parse(collection["SubscriptionStart"]);
                    societysubscription.SubscriptionEnd = DateTime.Parse(collection["SubscriptionEnd"]);
                    societysubscription.SubscribedMonths = byte.Parse(collection["SubscribedMonths"]);
                    if (_service.Add(societysubscription))
                    {
                        var societysubscriptionserviceservice = new SocietySubscriptionServiceService(this.ModelState);

                        //                      Guid servicetypeid;
                        foreach (var item in collection)
                        {
                            if (collection[item.ToString()] == "on")
                            {
                                //                                servicetypeid = new Guid(collection[item.ToString().Replace("Chk","ID")]);
                                var service = new CloudSocietyEntities.SocietySubscriptionService();
                                service.SocietySubscriptionID = societysubscription.SocietySubscriptionID;
                                service.ServiceTypeID = new Guid(collection[item.ToString().Replace("Chk", "ID")]);
                                if (!societysubscriptionserviceservice.Add(service))
                                    return View();
                            }
                        }

                        //var tariffrateservice = new TariffRateService(this.ModelState);
                        //var services = tariffrateservice.CurrentList();
                        //foreach (var item in services)
                        //{
                        //    var data = collection[item.ServiceTypeID.ToString()];
                        //    if (data != null)
                        //    {
                        //        var service = new CloudSocietyEntities.SocietySubscriptionService();
                        //        service.SocietySubscriptionID = societysubscription.SocietySubscriptionID;
                        //        service.ServiceTypeID = item.ServiceTypeID;
                        //        if(!societysubscriptionserviceservice.Add(service))
                        //            return View();
                        //    }
                        //}
                        return RedirectToAction("Index", "Society");
                    }
                }
                var tariffrateservice = new TariffRateService(this.ModelState);
                ViewBag.Services = tariffrateservice.CurrentList();
                ViewBag.StateList = new StateService().List();
                ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
                ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";
                return View();
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var tariffrateservice = new TariffRateService(this.ModelState);
                ViewBag.Services = tariffrateservice.CurrentList();
                ViewBag.StateList = new StateService().List();
                ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
                ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";
                return View();
            }
        }
        //
        // GET: /SocietySubscription/Edit/5
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        public ActionResult Edit(Guid id)
        {
            var tariffrateservice = new TariffRateService(this.ModelState);
            ViewBag.Services = tariffrateservice.ListWithActiveStatusForSubscription(id);

            var appinfoservice = new AppInfoService(this.ModelState);
            var appinfo = appinfoservice.Get();
            ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
            ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";

            return View(_service.GetById(id));
        }
        //
        // POST: /SocietySubscription/Edit/5
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        [HttpPost]
        public ActionResult Edit(Guid id, FormCollection collection)
        {
            try
            {
                var societysubscription = _service.GetById(id);
                societysubscription.NoOfAdditionalMembers = short.Parse(collection["NoOfAdditionalMembers"]);
                societysubscription.SubscribedMonths = byte.Parse(collection["SubscribedMonths"]);
                if (_service.Edit(societysubscription))
                {
                    var societysubscriptionserviceservice = new SocietySubscriptionServiceService(this.ModelState);
                    societysubscriptionserviceservice.DeletePendingBySocietySubscriptionID(id);
                    foreach (var item in collection)
                    {
                        if (collection[item.ToString()] == "on")
                        {
                            var service = new CloudSocietyEntities.SocietySubscriptionService();
                            service.SocietySubscriptionID = societysubscription.SocietySubscriptionID;
                            service.ServiceTypeID = new Guid(collection[item.ToString().Replace("Chk", "ID")]);
                            if (!societysubscriptionserviceservice.Add(service))
                                return View();
                        }
                    }
                }
                return RedirectToAction("Index", new { Id = societysubscription.SocietyID });
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var tariffrateservice = new TariffRateService(this.ModelState);
                ViewBag.Services = tariffrateservice.ListWithActiveStatusForSubscription(id);

                var appinfoservice = new AppInfoService(this.ModelState);
                var appinfo = appinfoservice.Get();
                ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
                ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";

                return View(_service.GetById(id));
            }
        }
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        public ActionResult Renew(Guid id)
        {
            // Last Subscription ID for the society is passed

            var lastsubscription = _service.GetById(id);
            var newsubscription = new SocietySubscription();
            newsubscription.SocietyID = lastsubscription.SocietyID;
            newsubscription.SubscriptionStart = lastsubscription.SubscriptionEnd.AddDays(1);
            newsubscription.SubscriptionEnd = newsubscription.SubscriptionStart.AddYears(1).AddDays(-1);
            newsubscription.NoOfMembers = (short)(lastsubscription.NoOfMembers);
            newsubscription.NoOfAdditionalMembers = (short)(lastsubscription.NoOfAdditionalMembers);

            ViewBag.SocietyName = lastsubscription.Society.Name;

            var tariffrateservice = new TariffRateService(this.ModelState);
            ViewBag.Services = tariffrateservice.ListWithActiveStatusMonthlyForSubscription(id);

            var appinfoservice = new AppInfoService(this.ModelState);
            var appinfo = appinfoservice.Get();
            ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
            ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";
            return View(newsubscription);
        }
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        [HttpPost]
        public ActionResult Renew(Guid id, FormCollection collection)
        {
            try
            {
                var societysubscription = new SocietySubscription();
                societysubscription.SocietyID = Guid.Parse(collection["SocietyID"]);
                societysubscription.SubscriptionStart = DateTime.Parse(collection["SubscriptionStart"]);
                societysubscription.SubscriptionEnd = DateTime.Parse(collection["SubscriptionEnd"]);
                societysubscription.NoOfMembers = short.Parse(collection["NoOfMembers"]);
                societysubscription.NoOfAdditionalMembers = short.Parse(collection["NoOfAdditionalMembers"]);
                societysubscription.SubscribedMonths = byte.Parse(collection["SubscribedMonths"]);
                if (_service.Add(societysubscription))
                {
                    var societysubscriptionserviceservice = new SocietySubscriptionServiceService(this.ModelState);
                    foreach (var item in collection)
                    {
                        if (collection[item.ToString()] == "on")
                        {
                            var service = new CloudSocietyEntities.SocietySubscriptionService();
                            service.SocietySubscriptionID = societysubscription.SocietySubscriptionID;
                            service.ServiceTypeID = new Guid(collection[item.ToString().Replace("Chk", "ID")]);
                            if (!societysubscriptionserviceservice.Add(service))
                            {
                                var tariffrateservice = new TariffRateService(this.ModelState);
                                ViewBag.Services = tariffrateservice.ListWithActiveStatusMonthlyForSubscription(id);

                                var appinfoservice = new AppInfoService(this.ModelState);
                                var appinfo = appinfoservice.Get();
                                ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
                                ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";
                                return View(societysubscription);
                            }
                        }
                    }
                }
                return RedirectToAction("Index", new { Id = societysubscription.SocietyID });
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var lastsubscription = _service.GetById(id);
                var newsubscription = new SocietySubscription();
                newsubscription.SubscriptionStart = lastsubscription.SubscriptionEnd.AddDays(1);
                newsubscription.SubscriptionEnd = newsubscription.SubscriptionStart.AddYears(1).AddDays(-1);
                newsubscription.NoOfAdditionalMembers = (short)(lastsubscription.NoOfAdditionalMembers + lastsubscription.NoOfInvoicedMembers + lastsubscription.NoOfMembers);

                var tariffrateservice = new TariffRateService(this.ModelState);
                ViewBag.Services = tariffrateservice.ListWithActiveStatusMonthlyForSubscription(id);

                var appinfoservice = new AppInfoService(this.ModelState);
                var appinfo = appinfoservice.Get();
                ViewBag.TaxPrompt = appinfo.Tax.Head;   // "Service Tax @10.3%";
                ViewBag.TaxPerc = appinfo.Tax.TaxPerc;  // "10.3";
                return View(newsubscription);
            }
        }
        public ActionResult Select(Guid id)     // id = societyid
        {
            // Since new society was selected, clear society-related session variables
            ClearSession();
            //ViewBag.SocietyName = new SocietyService(this.ModelState).GetById(id).Name;
            var list = _service.ListByParentId(id);
            var lst = (IList<SocietySubscription>)list;
            if (lst.Count == 1)
            {
                var societysubscriptionid = lst[0].SocietySubscriptionID;
                return RedirectToAction("Menu", new { id = societysubscriptionid });
            }
            else
                return View(list);
        }

        private void ClearSession()
        {
            const string _receiptfromdate = "ReceiptFromDate";
            const string _receipttodate = "ReceiptToDate";

            Session[_receiptfromdate] = null;
            Session[_receipttodate] = null;
        }

        public ActionResult Menu(Guid id)
        {
            if (Roles.IsUserInRole("Member") || Roles.IsUserInRole("OfficeBearer"))
                return RedirectToAction("Menu", "SocietyMember", new { id = id });
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            return View();
        }
        public ActionResult ChangeYear(Guid id)//id=societySubscriptionId
        {
            var societyid = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).GetById(id).SocietyID;
            var list = _service.ListByParentId(societyid);
            var lst = (IList<SocietySubscription>)list;
            return View("Select", list);
        }
        [HttpGet]
        public ActionResult ReportMessage(Guid societySubscriptionID, String ControllerName, String ActionName)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.ActionName = ActionName;
            ViewBag.ControllerName = ControllerName;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(societySubscriptionID);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(societySubscriptionID);
            return View();
        }
        // GET: /Delete Society Subscription added by Ranjit
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        [HttpGet]
        public ActionResult Delete(Guid id) // id= SocietySubscriptionID
        {
            //ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /Delete Society Subscription added by Ranjit
        [Authorize(Roles = "Subscriber,CompanyAdmin")]
        [HttpPost]
        public ActionResult Delete(Guid id, SocietySubscription SocietySubscriptionToDelete) //id = SocietySubscriptionID
        {
            SocietySubscription SocietySubscription = _service.GetById(id);
            try
            {
                if (_service.Delete(SocietySubscription))
                {
                    return RedirectToAction("Index", new { id = SocietySubscription.SocietyID });
                }
                else
                {
                    // ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View(SocietySubscription);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                //ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View(_service.GetById(id));
            }
        }

        [Authorize(Roles = "Subscriber,SocietyAdmin,CompanyAdmin")]
        public ActionResult Lock(Guid id)
        {
            // Subscription ID for the society is passed

            var subscription = _service.GetById(id);
            ViewBag.EndRange = (subscription.PaidTillDate == null ? subscription.SubscriptionEnd : subscription.PaidTillDate);
            ViewBag.StartRange = subscription.SubscriptionStart;
            //            ViewBag.SocietyName = subscription.Society.Name;

            return View(subscription);
        }

        [Authorize(Roles = "Subscriber,SocietyAdmin,CompanyAdmin")]
        [HttpPost]
        public ActionResult Lock(Guid id, DateTime? LockedTillDate)
        {
            // Subscription ID for the society is passed
            var subscription = _service.GetById(id);
            subscription.LockedTillDate = LockedTillDate;
            _service.Edit(subscription);

            return RedirectToAction("Menu", new { id = id });
        }
    }
}
