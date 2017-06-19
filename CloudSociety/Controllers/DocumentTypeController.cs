using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DocumentTypeController : Controller
    {
        private DocumentTypeService _service;       
        const string _exceptioncontext = "DocumentType Controller";

        public DocumentTypeController()
        {
            _service = new DocumentTypeService(this.ModelState);
            
        }
        //
        // GET: /DocumentType/
 
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /Subscriber/Details/5
  
         public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /DocumentType/Create

        public ActionResult Create()
        {
           return View();
        }

        //
        // POST: /DocumentType/Create

        [HttpPost]
        public ActionResult Create(DocumentType DocumentTypeToCreate)
        {
            try
            {
                //DocumentTypeToCreate.DocumentTypeID = System.Guid.NewGuid();

                //DocumentTypeToCreate.CreatedByID = System.Guid.NewGuid();
                //DocumentTypeToCreate.CreatedOn = DateTime.Now;
               
                if (_service.Add(DocumentTypeToCreate))
                {
                    return RedirectToAction("Index");
                }
                  else
                    {
                        return View();
                }
            }
               
         
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View();
            }
        }
        //
        // GET: /DocumentType/Edit/5

        public ActionResult Edit(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl; 
            return View(_service.GetById(id));
        }

        //
        // POST: /DocumentType/Edit/5
 
        [HttpPost]
        public ActionResult Edit(Guid id, string backUrl, DocumentType DocumentTypeToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(DocumentTypeToUpdate))
                    return RedirectPermanent(backUrl);
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);   
                return View();
            }
        }

        //
        // GET: /DocumentType/Delete/5

        public ActionResult Delete(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View(_service.GetById(id));
        }

        //
        // POST: /DocumentType/Delete/5
        
        [HttpPost]
        public ActionResult Delete(Guid id,string backUrl, FormCollection collection)
        {
            try
            {
                if (_service.Delete(_service.GetById(id)))
                    return Redirect(backUrl);
                else
                {
                    ViewBag.BackUrl = backUrl;
                    return View(_service.GetById(id));
                }
            }
            catch(Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetById(id));
            }
        }
    }
}
