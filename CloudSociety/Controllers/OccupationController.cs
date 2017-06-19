using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OccupationController : Controller
    {
        private OccupationService _service;
        const string _exceptioncontext = "Occupation Controller";
        public OccupationController()
        {
        
            _service = new OccupationService(this.ModelState);
           
        }
        //
        // GET: /Occupation/
      
        public ActionResult Index()
        {

            return View(_service.List());
        }

        //
        // GET: /Occupation/Details/5
       
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /Occupation/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /Bank/Create

        [HttpPost]
        public ActionResult Create(Occupation OccupationToCreate)
        {
            try
            {
                //BankToCreate.BankID = System.Guid.NewGuid();

                //BankToCreate.CreatedByID = System.Guid.NewGuid();
                //BankToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(OccupationToCreate))
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

        // GET: /Occupation/Edit/5
      
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Occupation/Edit/5
    
        [HttpPost]
        public ActionResult Edit(Guid id, Occupation OccupationToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(OccupationToUpdate))
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
        // GET: /Occupation/Delete/5
      
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Occupation/Delete/5
       
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