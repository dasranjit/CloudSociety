using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TariffController : Controller
    {
        private TariffService _service;
        const string _exceptioncontext = "Tariff Controller";
        public TariffController()
        {
            _service = new TariffService(this.ModelState);
        }
        //
        // GET: /Tariff/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /Tariff/Details/5
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /Tariff/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tariff/Create

        [HttpPost]
        public ActionResult Create(Tariff TariffToCreate)
        {
            try
            {
                //TariffToCreate.TariffID = System.Guid.NewGuid();

                //TariffToCreate.CreatedByID = System.Guid.NewGuid();
                //TariffToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(TariffToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View();
            }
        }

        //
        // GET: /Tariff/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Tariff/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, Tariff TariffToUpdate)
        {
            try
            {
                if (_service.Edit(TariffToUpdate))
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

        //
        // GET: /Tariff/Delete/5
        public ActionResult Delete(Guid id)
        {
            // Temp: Remove before production
            //            _service.Delete(_service.GetById(id));

            return View(_service.GetById(id));
        }

        //
        // POST: /Tariff/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, Tariff TariffToDelete)  // FormCollection collection
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
