using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommunicationTypeController : Controller
    {
        private CommunicationTypeService _service;

        const string _exceptioncontext = "Communication Type Controller";

        public CommunicationTypeController()
        {
            _service = new CommunicationTypeService(this.ModelState);

        }

        // GET: /CommunicationType/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        // GET: /CommunicationType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /CommunicationType/Create
        [HttpPost]
        public ActionResult Create(CommunicationType CommunicationTypeToCreate)
        {
            try
            {
                if (_service.Add(CommunicationTypeToCreate))
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

        // GET: /CommunicationType/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /CommunicationType/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, CommunicationType CommunicationTypeToUpdate)
        {
            try
            {
                if (_service.Edit(CommunicationTypeToUpdate))
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

        // GET: /CommunicationType/Delete/5
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        // POST: /CommunicationType/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid CommunicationTypeID, FormCollection collection)
        {
            try
            {
                if (_service.Delete(_service.GetById(CommunicationTypeID)))
                    return RedirectToAction("Index");
                else
                    return View(_service.GetById(CommunicationTypeID));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetById(CommunicationTypeID));
            }
        }
    }
}

