using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UnitTypeController : Controller
    {
        private UnitTypeService _service;
        const string _exceptioncontext = "UnitType Controller";
        //private AccountMembershipService MembershipService;

        public UnitTypeController()
        {
            _service = new UnitTypeService(this.ModelState);
            //MembershipService = new AccountMembershipService();
        }
        //
        // GET: /UnitType/
       
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
        // GET: /UnitType/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /UnitType/Create

        [HttpPost]
        public ActionResult Create(UnitType UnitTypeToCreate)
        {
            try
            {
                

                if (_service.Add(UnitTypeToCreate))
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

        // GET: /UnitType/Edit/5
       
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }        

        //
        // POST: /Unit Type/Edit/5
      
        [HttpPost]
        public ActionResult Edit(Guid id, UnitType UnitTypeToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(UnitTypeToUpdate))
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
        // GET: /UnitType/Delete/5
       
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /UnitType/Delete/5
      
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

