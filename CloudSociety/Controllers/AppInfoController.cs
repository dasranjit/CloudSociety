using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Collections.Generic;
using System.Web.Security;
using System.Configuration;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppInfoController : Controller
    {
        private AppInfoService _service;
        const string _exceptioncontext = "AppInfo Controller";

        public AppInfoController()
        {
            _service = new AppInfoService(this.ModelState);
        }

        // GET: /AppInfo/Edit/5
        public ActionResult Edit()
        {
            ViewBag.EditStatus = "";
            AppInfo appInfo = _service.Get();
            Dictionary<string, string> mailBody = new Dictionary<string, string>();
            ViewBag.UOMList = new CloudSociety.Services.UOMService(this.ModelState).List();
            ViewBag.SubscriptionTaxList = new CloudSociety.Services.TaxService(this.ModelState).List();
            string tempMailBody = "";
            //encode the mail body
            tempMailBody = Server.HtmlEncode(appInfo.BillMailBody);
            mailBody.Add("BillMailBody", tempMailBody);
            tempMailBody = Server.HtmlEncode(appInfo.InvoiceMailBody);
            mailBody.Add("InvoiceMailBody", tempMailBody);
            tempMailBody = Server.HtmlEncode(appInfo.TrialMailBody);
            mailBody.Add("TrialMailBody", tempMailBody);
            tempMailBody = Server.HtmlEncode(appInfo.ReceiptMailBody);
            mailBody.Add("ReceiptMailBody", tempMailBody);
            tempMailBody = Server.HtmlEncode(appInfo.ReceiptReversalBody);
            mailBody.Add("ReceiptReversalBody", tempMailBody);
            ViewBag.MailBody = mailBody;
            return View(appInfo);
        }

        // POST: /AppInfo/Edit/5
        [HttpPost]
        public ActionResult Edit(AppInfo appInfToUpdate)
        {
            try
            {
                Dictionary<string, string> mailBody = new Dictionary<string, string>();
                mailBody.Add("TrialMailBody", appInfToUpdate.TrialMailBody);
                mailBody.Add("InvoiceMailBody", appInfToUpdate.InvoiceMailBody);
                mailBody.Add("BillMailBody", appInfToUpdate.BillMailBody);
                mailBody.Add("ReceiptMailBody", appInfToUpdate.ReceiptMailBody);
                mailBody.Add("ReceiptReversalBody", appInfToUpdate.ReceiptReversalBody);
                //as SubscriberTerms is same with TrialTerms
                appInfToUpdate.SubscriberTerms = appInfToUpdate.TrialTerms;
                //decode the mail bodies
                appInfToUpdate.TrialMailBody = Server.HtmlDecode(appInfToUpdate.TrialMailBody);
                appInfToUpdate.BillMailBody = Server.HtmlDecode(appInfToUpdate.BillMailBody);
                appInfToUpdate.InvoiceMailBody = Server.HtmlDecode(appInfToUpdate.InvoiceMailBody);
                appInfToUpdate.ReceiptMailBody = Server.HtmlDecode(appInfToUpdate.ReceiptMailBody);
                appInfToUpdate.ReceiptReversalBody = Server.HtmlDecode(appInfToUpdate.ReceiptReversalBody);
                //set viewbag 
                ViewBag.UOMList = new CloudSociety.Services.UOMService(this.ModelState).List();
                ViewBag.SubscriptionTaxList = new CloudSociety.Services.TaxService(this.ModelState).List();
                ViewBag.MailBody = mailBody;
                if (_service.Edit(appInfToUpdate))
                {
                    ViewBag.EditStatus = "Saved Successfully.";
                    return View(appInfToUpdate);
                }
                else
                {
                    ViewBag.EditStatus = "Not Saved.";
                    return View(appInfToUpdate);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(appInfToUpdate);
            }
        }

        [HttpGet]
        public ActionResult IsFullAccessToSupport()
        {
            ViewBag.IsFullAccessToSupport = ConfigurationManager.AppSettings["IsFullAccessToSupport"].ToLower()=="yes";
            return View();
        }
        [HttpPost]
        public ActionResult IsFullAccessToSupport(bool IsFullAccessToSupport)
        {           
            //Configuration Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            ConfigurationManager.AppSettings.Set("IsFullAccessToSupport", IsFullAccessToSupport ? "yes" : "no");
            //Configuration.Save(ConfigurationSaveMode.Full, true);  
            ViewBag.IsFullAccessToSupport = (ConfigurationManager.AppSettings["IsFullAccessToSupport"].ToLower() == "yes");
            return View();          
        }

        public bool IsFullAccess()
        {
            if (Roles.IsUserInRole("Support"))

                return ConfigurationManager.AppSettings["IsFullAccessToSupport"].ToLower() == "yes";
            else
                return true;

        }
    }

}
