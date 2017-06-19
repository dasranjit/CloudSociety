using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RelationshipController : Controller
    {
        private RelationshipService _service;       
        const string _exceptioncontext = "Relationship Controller";
        public RelationshipController()
        {
            _service = new RelationshipService(this.ModelState);
            
        }
        //
        // GET: /Relationship/
       
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
        // GET: /Relationship/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /Relationship/Create

        [HttpPost]
        public ActionResult Create(Relationship RelationshipToCreate)
        {
            try
            {
                //RelationshipToCreate.BankID = System.Guid.NewGuid();

                //RelationshipToCreate.CreatedByID = System.Guid.NewGuid();
                //RelationshipToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(RelationshipToCreate))
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

        // GET: /Relationship/Edit/5
      
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /RelationshipToCreate/Edit/5
       
        [HttpPost]
        public ActionResult Edit(Guid id, Relationship RelationshipToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(RelationshipToUpdate))
                    return RedirectToAction("Index");
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
        // GET: /Relationship/Delete/5
       
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Relationship/Delete/5
       
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            try
            {
                if(_service.Delete(_service.GetById(id)))
                return RedirectToAction("Index");
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}
