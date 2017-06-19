using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StandardChargeHeadController : Controller
    {
        private StandardChargeHeadService _service;
        const string _exceptioncontext = "StandardChargeHead Controller";
        private static readonly IDictionary<string, string> _nature = new Dictionary<string, string>() { { "C", "Construction Cost Basis " }, { "A", "Per Area" }, { "L", "Late Payment Penalty" }, { "E", "Early Payment Discount" }, { "I", "Interest" }, { "T", "Tax" } };

        public StandardChargeHeadController()
        {
            _service = new StandardChargeHeadService(this.ModelState);
        }
        //
        // GET: /StandardChargeHead/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /StandardChargeHead/Details/5
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /StandardChargeHead/Create

        public ActionResult Create()
        {
            var _StdAcHdService = new StandardAcHeadService(this.ModelState);
            var list = _StdAcHdService.List();
            ViewBag.StandardCh = list;
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View();
        }

        //
        // POST: /StandardChargeHead/Create

        [HttpPost]
        public ActionResult Create(StandardChargeHead StandardChargeHeadToCreate)
        {
            try
            {
                if (_service.Add(StandardChargeHeadToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                    var list = _StdAcHdService.List();
                    ViewBag.StandardCh = list;
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }

            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                var list = _StdAcHdService.List();
                ViewBag.StandardCh = list;
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        //
        // GET: /StandardChargeHead/Edit/5
        public ActionResult Edit(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl; 
            var _StdAcHdService = new StandardAcHeadService(this.ModelState);
            var list = _StdAcHdService.List();
            ViewBag.StandardCh = list;
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardChargeHead/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, string backUrl, StandardChargeHead StandardChargeHeadToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(StandardChargeHeadToUpdate))
                    return RedirectPermanent(backUrl);
                else
                {
                    var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                    var list = _StdAcHdService.List();
                    ViewBag.StandardCh = list;
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                var list = _StdAcHdService.List();
                ViewBag.StandardCh = list;
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        //
        // GET: /StandardChargeHead/Delete/5
        public ActionResult Delete(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardChargeHead/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, string backUrl, StandardChargeHead StandardChargeHeadToDelete)
        {
            try
            {
                if (_service.Delete(StandardChargeHeadToDelete))
                    return Redirect(backUrl);
                else
                {
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View();
            }
        }
    }
}
