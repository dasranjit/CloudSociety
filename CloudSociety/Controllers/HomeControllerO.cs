using System.Web.Mvc;
using System;
using System.Web.Security;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           return View();
        }
        public ActionResult Index1()
        {            

            return View();
        }

        public ActionResult AdminIndex()
        {
            return View();
        }

        //public ActionResult About()
        //{
        //    return View();
        //}
        public ActionResult Sitemap()
        {
            return View();
        }
        public ActionResult Productfeature()
        {
            return View();
        }
        public ActionResult Productbenefits()
        {
            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult CloudComputing()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult Mission()
        {
            return View();
        }
        public ActionResult Vision()
        {
            return View();
        }
        public ActionResult Clientele()
        {
            return View();
        }
        public ActionResult Team()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult MessageToViewer(Guid id)
        {
            ViewBag.SocietySubscriptionID = id;
            return View();
        }
        public ActionResult Upcoming(Guid id,String module)
        {
            ViewBag.SocietySubscriptionID = id;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.ShowSocietyMenu = true;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.ShowBillingMenu = societySubscriptionService.BillingEnabled(id);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(id);
            ViewBag.TrialUser = (Roles.IsUserInRole("TrialUser"));
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(id);
            ViewBag.Module = module;
            return View();
        }
        public ActionResult MemberUpcoming(Guid id,String module)
        {
            var societySubscriptionService = new SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = id;
            ViewBag.Module = module;
            ViewBag.ShowFinalReports = societySubscriptionService.GetById(id).Closed;
            ViewBag.ShowMemberMenu = true;
            return View("Upcoming");
        }
    }
}
