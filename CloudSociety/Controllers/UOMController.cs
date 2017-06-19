using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UOMController : Controller
    {
        private UOMService _service;
        const string _exceptioncontext = " Controller";
        public UOMController()
        {
            _service = new UOMService(this.ModelState);
           
        }
        //
        // GET: /UOM/
       
        public ActionResult Index()
        {

            return View(_service.List());
        }

        //
        // GET: /UOM/Details/5
        
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /UOM/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /UOM/Create

        [HttpPost]
        public ActionResult Create(UOM UOMToCreate)
        {
            try
            {
                //UOMToCreate.BankID = System.Guid.NewGuid();

                //UOMToCreate.CreatedByID = System.Guid.NewGuid();
                //UOMToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(UOMToCreate))
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

        // GET: /UOM/Edit/5
     
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /UOM/Edit/5
      
        [HttpPost]
        public ActionResult Edit(Guid id, UOM UOMToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(UOMToUpdate))
                    return RedirectToAction("Index");
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
        // GET: /UOM/Delete/5
     
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Bank/Delete/5
      
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            try
            {
             if(_service.Delete(_service.GetById(id)))
                return RedirectToAction("Index");
                else
                 return View(_service.GetById(id));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetById(id));
            }
        }
    }
}
