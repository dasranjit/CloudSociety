using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaxController : Controller
    {
        private TaxService _service;
        const string _exceptioncontext = "Tax Controller";

        public TaxController()
        {
            _service = new TaxService(this.ModelState);
           
        }
        //
        // GET: /Taxes/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /Taxes/Details/5
        public ActionResult Details(Guid id)
        {

            Tax tax;
            tax = _service.GetById(id);
            return View(tax);
        }

        //
        // GET: /Taxes/Create

        public ActionResult Create()
        {
            var _StandardAcHeadService = new StandardAcHeadService(this.ModelState);
            var list = _StandardAcHeadService.List();
            ViewBag.StandardAcHead = list;
            return View();
        }

        //
        // POST: /Taxes/Create

        [HttpPost]
        public ActionResult Create(Tax TaxToCreate)
        {
            try
            {
                if (_service.Add(TaxToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var _StandardAcHeadService = new StandardAcHeadService(this.ModelState);
                    var list = _StandardAcHeadService.List();
                    ViewBag.StandardAcHead = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _StandardAcHeadService = new StandardAcHeadService(this.ModelState);
                var list = _StandardAcHeadService.List();
                ViewBag.StandardAcHead = list;
                return View();
            }
        }

        //
        // GET: /Taxes/Edit/5
        public ActionResult Edit(Guid id)
        {
            var _StandardAcHeadService = new StandardAcHeadService(this.ModelState);
            var list = _StandardAcHeadService.List();
            ViewBag.StandardAcHead = list;
            return View(_service.GetById(id));
        }

        //
        // POST: /Taxes/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, Tax TaxToUpdate)
        {
            try
            {
                if (_service.Edit(TaxToUpdate))
                    return RedirectToAction("Index");
                else
                {
                    var _StandardAcHeadService = new StandardAcHeadService(this.ModelState);
                    var list = _StandardAcHeadService.List();
                    ViewBag.StandardAcHead = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _StandardAcHeadService = new StandardAcHeadService(this.ModelState);
                var list = _StandardAcHeadService.List();
                ViewBag.StandardAcHead = list;
                return View();
            }
        }

        //
        // GET: /Taxes/Delete/5
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Taxes/Delete/5       
        [HttpPost]
        public ActionResult Delete(Guid id, Tax TaxToDelete)
        {
            try
            {
                if (_service.Delete(TaxToDelete))
                    return RedirectToAction("Index");
                else
                {
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetById(id));
            }
        }
    }
}
