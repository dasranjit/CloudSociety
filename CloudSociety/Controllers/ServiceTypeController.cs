using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceTypeController : Controller
    {
        private ServiceTypeService _service;

        const string _exceptioncontext = "ServiceType Controller";
        private static readonly IDictionary<string, string> _chargeability = new Dictionary<string, string>() { { "O", "One-time" }, { "M", "Monthly" } }; // ,{"Q","Quarterly"}
        private static readonly IDictionary<string, string> _basis = new Dictionary<string, string>() { { "M", "per Member" }, { "S", "per Society" } };
        private static readonly IDictionary<string, string> _nature = new Dictionary<string, string>() { { "A", "Accounts" }, { "B", "Billing" }, { "S", "SMS" } };


        public ServiceTypeController()
        {

            _service = new ServiceTypeService(this.ModelState);
        }
        //
        // GET: /ServiceTypes/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /ServiceTypes/Details/5
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /ServiceTypes/Create

        public ActionResult Create()
        {
            ViewBag.ChargeabilityList = new SelectList(_chargeability, "Key", "Value");
            ViewBag.BasisList = new SelectList(_basis, "Key", "Value");
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View();
        }

        //
        // POST: /ServiceTypes/Create

        [HttpPost]
        public ActionResult Create(ServiceType ServiceTypeToCreate)
        {
            try
            {
                //                ServiceTypeToCreate.ChargeabilityBasis = ServiceTypeToCreate.Chargeability + ServiceTypeToCreate.Basis;
                if (_service.Add(ServiceTypeToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ChargeabilityList = new SelectList(_chargeability, "Key", "Value");
                    ViewBag.BasisList = new SelectList(_basis, "Key", "Value");
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.ChargeabilityList = new SelectList(_chargeability, "Key", "Value");
                ViewBag.BasisList = new SelectList(_basis, "Key", "Value");
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        // GET: /ServiceTypes/Edit/5
        public ActionResult Edit(Guid id)
        {
            ViewBag.ChargeabilityList = new SelectList(_chargeability, "Key", "Value");
            ViewBag.BasisList = new SelectList(_basis, "Key", "Value");
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View(_service.GetById(id));
        }

        //
        // POST: /ServiceTypes/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, ServiceType ServiceTypeToUpdate)
        {
            try
            {
                if (_service.Edit(ServiceTypeToUpdate))
                    return RedirectToAction("Index");
                else
                {
                    ViewBag.ChargeabilityList = new SelectList(_chargeability, "Key", "Value");
                    ViewBag.BasisList = new SelectList(_basis, "Key", "Value");
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.ChargeabilityList = new SelectList(_chargeability, "Key", "Value");
                ViewBag.BasisList = new SelectList(_basis, "Key", "Value");
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        //
        // GET: /ServiceTypes/Delete/5
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /ServiceTypes/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            try
            {
                if (_service.Delete(_service.GetById(id)))
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
