using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MemberClassController : Controller
    {
        private MemberClassService _service;        
        const string _exceptioncontext = "Member Class Controller";

        public MemberClassController()
        {
            _service = new MemberClassService(this.ModelState);
          
        }
        //
        // GET: /MemberClass/
        public ActionResult Index()
        {

            return View(_service.List());
        }

        //
        // GET: /Memberclass/Details/5
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /Memberclass/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Memberclass/Create

        [HttpPost]
        public ActionResult Create(MemberClass MemberClassToCreate)
        {
            try
            {
                if (_service.Add(MemberClassToCreate))
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

        // GET: /Memberclass/Edit/5
  
        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Memeberclass/Edit/5
      
        [HttpPost]
        public ActionResult Edit(Guid id, MemberClass MemberclassToUpdate)
        {
            try

            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(MemberclassToUpdate))
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
        // GET: /Bank/Delete/5
      
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /Memberclasss/Delete/5
      
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
