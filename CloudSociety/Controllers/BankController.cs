using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BankController : Controller
    {
        private BankService _service;       
        const string _exceptioncontext = "Bank Controller";

        public BankController()
        {
            _service = new BankService(this.ModelState);
            
        }
        //
        // GET: /Bank/
 
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
        // GET: /Bank/Create

        public ActionResult Create()
        {
           return View();
        }

        //
        // POST: /Bank/Create

        [HttpPost]
        public ActionResult Create(Bank bankToCreate)
        {
            try
            {
                //BankToCreate.BankID = System.Guid.NewGuid();

                //BankToCreate.CreatedByID = System.Guid.NewGuid();
                //BankToCreate.CreatedOn = DateTime.Now;
               
                if (_service.Add(bankToCreate))
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
        // GET: /Bank/Edit/5

        public ActionResult Edit(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl; 
            return View(_service.GetById(id));
        }

        //
        // POST: /Bank/Edit/5
 
        [HttpPost]
        public ActionResult Edit(Guid id, string backUrl, Bank bankToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(bankToUpdate))
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
        // GET: /Bank/Delete/5

        public ActionResult Delete(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View(_service.GetById(id));
        }

        //
        // POST: /Bank/Delete/5
        
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
