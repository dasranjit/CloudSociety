using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TariffRateController : Controller
    {
        private TariffRateService _service;
        const string _exceptioncontext = "TariffRate Controller";
        public TariffRateController()
        {
            _service = new TariffRateService(this.ModelState);
        }
        //
        // GET: /TariffRate/

        public ActionResult Index(Guid parentid)
        {
            var tariffservice = new TariffService(this.ModelState);
            ViewBag.Tariff = tariffservice.GetById(parentid);
            return View(_service.ListByParentId(parentid));
        }

        //
        // GET: /TariffRate/Details/5

        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /TariffRate/Create

        public ActionResult Create(Guid TariffID)
        {
            var TariffService = new TariffService(this.ModelState);
            ViewBag.Tariff = TariffService.GetById(TariffID);
            var _servicetypeservice = new ServiceTypeService(this.ModelState);
            var list = _servicetypeservice.List();
            ViewBag.ServiceTypes = list;
            return View();
        } 

        //
        // POST: /TariffRate/Create

        [HttpPost]
        public ActionResult Create(TariffRate TariffRateToCreate)
        {
            try
            {
                if (_service.Add(TariffRateToCreate))
                {
                    return RedirectToAction("Index", "TariffRate", new { parentid = TariffRateToCreate.TariffID });
                }
                else
                {
                    var TariffService = new TariffService(this.ModelState);
                    ViewBag.Tariff = TariffService.GetById(TariffRateToCreate.TariffID);
                    var _servicetypeservice = new ServiceTypeService(this.ModelState);
                    var list = _servicetypeservice.List();
                    ViewBag.ServiceTypes = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var TariffService = new TariffService(this.ModelState);
                ViewBag.Tariff = TariffService.GetById(TariffRateToCreate.TariffID);
                var _servicetypeservice = new ServiceTypeService(this.ModelState);
                var list = _servicetypeservice.List();
                ViewBag.ServiceTypes = list;
                return View();
            }
        }
        
        //
        // GET: /TariffRate/Edit/5

        public ActionResult Edit(Guid id)
        {
            var _servicetypeservice = new ServiceTypeService(this.ModelState);
            var list = _servicetypeservice.List();
            ViewBag.ServiceTypes = list;
            var tariffrate = _service.GetById(id);
            var TariffService = new TariffService(this.ModelState);
            ViewBag.Tariff = TariffService.GetById(tariffrate.TariffID);
            return View(tariffrate);
        }

        //
        // POST: /TariffRate/Edit/5

        [HttpPost]
        public ActionResult Edit(TariffRate TariffRateToUpdate)
        {
            try
            {
                if (_service.Edit(TariffRateToUpdate))
                {
                    return RedirectToAction("Index", "TariffRate", new { parentid = TariffRateToUpdate.TariffID });
                }
                else
                {
                    var TariffService = new TariffService(this.ModelState);
                    ViewBag.Tariff = TariffService.GetById(TariffRateToUpdate.TariffID);
                    var _servicetypeservice = new ServiceTypeService(this.ModelState);
                    var list = _servicetypeservice.List();
                    ViewBag.ServiceTypes = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var TariffService = new TariffService(this.ModelState);
                ViewBag.Tariff = TariffService.GetById(TariffRateToUpdate.TariffID);
                var _servicetypeservice = new ServiceTypeService(this.ModelState);
                var list = _servicetypeservice.List();
                ViewBag.ServiceTypes = list;
                return View();
            }
        }

        //
        // GET: /TariffRate/Delete/5
 
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /TariffRate/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, TariffRate TariffRateToDelete)
        {            
            try
            {
                if (_service.Delete( _service.GetById(id)))
                    return RedirectToAction("Index", "TariffRate", new { parentid = TariffRateToDelete.TariffID });
                else
                {
                    return View(_service.GetById(id));
                }
            }
            catch
            {
                return View(_service.GetById(id));
            }
        }
    }
}
