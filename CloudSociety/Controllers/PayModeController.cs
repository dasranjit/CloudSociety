using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PayModeController : Controller
    {
        private PayModeService _service;
        const string _exceptioncontext = "PayMode Controller";

        public PayModeController()
        {
            _service = new PayModeService(this.ModelState);

        }
        //
        // GET: /PayMode/

        public ActionResult Index()
        {
            return View(_service.List());
        }


        //GET: /PayMode/Create
        public ActionResult Create()
        {
            ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
            return View();
        }

        //
        // POST: /PayMode/Create
        [HttpPost]
        public ActionResult Create(PayMode PayModeToCreate)
        {
            try
            {
                if(_service.Add(PayModeToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
                return View();
            }
        }

        //GET: /PayMode/Edit/5
        public ActionResult Edit(string code)
        {
            ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
            return View(_service.GetByCode(code));
        }

        //POST: /PayMode/Edit/5
        [HttpPost]
        public ActionResult Edit(string code, PayMode PayModeToUpdate)
        {
            try
            {                
                if(_service.Edit(PayModeToUpdate))
                    return RedirectToAction("Index");
                else
                {
                    ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
                return View();
            }
        }

        //
        // GET: /PayMode/Delete/5

        public ActionResult Delete(string code)
        {
            return View(_service.GetByCode(code));
        }

        //
        // POST: /PayMode/Delete/5
        [HttpPost]
        public ActionResult Delete(string code, PayMode PayModeToDelete)
        {
            try
            {
                if (_service.Delete(_service.GetByCode(code)))
                    return RedirectToAction("Index");
                else
                {
                    ViewBag.StandardAcHeadsList = new StandardAcHeadService(this.ModelState).List();
                    return View(_service.GetByCode(code));
                }

            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetByCode(code));
            }
        }
    }
}
