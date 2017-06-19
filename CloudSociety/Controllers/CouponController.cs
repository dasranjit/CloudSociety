using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private CouponService _service;        
        const string _exceptioncontext = "Coupon Controller";
        public CouponController()
        {
            _service = new CouponService(this.ModelState);
        }
        //
        // GET: /Coupons/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /Subscriber/Details/5
         public ActionResult Details(string code)
        {
            return View(_service.GetByCode(code));
        }

        //
        // GET: /Coupon/Create

        public ActionResult Create()
        {
            var subscriberservice = new SubscriberService(this.ModelState);
            ViewBag.subscriberlist = subscriberservice.List();
            var societyservice = new SocietyService(this.ModelState);
            ViewBag.societylist = societyservice.ListForCompany();
            return View();
        }

        //
        // POST: /Coupon/Create
        [HttpPost]
        public ActionResult Create(Coupon CouponToCreate)
        {
            try
            {
                if (_service.Add(CouponToCreate))
                {
                    return RedirectToAction("Index");
                }
                  else
                    {
                        var subscriberservice = new SubscriberService(this.ModelState);
                        ViewBag.subscriberlist = subscriberservice.List();
                        var societyservice = new SocietyService(this.ModelState);
                        ViewBag.societylist = societyservice.ListForCompany();
                        return View();
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var subscriberservice = new SubscriberService(this.ModelState);
                ViewBag.subscriberlist = subscriberservice.List();
                var societyservice = new SocietyService(this.ModelState);
                ViewBag.societylist = societyservice.ListForCompany();
                return View();
            }
        }

        //
        // GET: /Coupon/Edit/5
        public ActionResult Edit(string code)
        {
            var subscriberservice = new SubscriberService(this.ModelState);
            ViewBag.subscriberlist = subscriberservice.List();
            var societyservice = new SocietyService(this.ModelState);
            ViewBag.societylist = societyservice.ListForCompany();
            return View(_service.GetByCode(code));
        }

        //
        // POST: /Coupon/Edit/5
        [HttpPost]
        public ActionResult Edit(string code, Coupon CouponToUpdate)
        {
            try
            {
                if (_service.Edit(CouponToUpdate))
                    return RedirectToAction("Index");
                else
                {                    
                    var subscriberservice = new SubscriberService(this.ModelState);
                    ViewBag.subscriberlist = subscriberservice.List();
                    var societyservice = new SocietyService(this.ModelState);
                    ViewBag.societylist = societyservice.ListForCompany();
                    return View();
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var subscriberservice = new SubscriberService(this.ModelState);
                ViewBag.subscriberlist = subscriberservice.List();
                var societyservice = new SocietyService(this.ModelState);
                ViewBag.societylist = societyservice.ListForCompany();
                return View();
            }
        }

        //
        // GET: /Coupon/Delete/5
        public ActionResult Delete(string code)
        {
            return View(_service.GetByCode(code));
        }

        //
        // POST: /Coupon/Delete/5
        [HttpPost]
        public ActionResult Delete(string code, Coupon couponToDelete)
        {
            try
            {
                if (_service.Delete(_service.GetByCode(code)))
                
                    return RedirectToAction("Index");
                else
                {
                    return View(_service.GetByCode(code));
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetByCode(code));
            }
        }
    }
}
