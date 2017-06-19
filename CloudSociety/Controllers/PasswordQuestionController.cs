using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PasswordQuestionController : Controller
    {
        private PasswordQuestionService _service;
        const string _exceptioncontext = "PasswordQuestion Controller";

        public PasswordQuestionController()
        {
            _service = new PasswordQuestionService(this.ModelState);

        }
        //
        // GET: /PasswordQuestion/

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
        // GET: /PasswordQuestion/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PasswordQuestion/Create

        [HttpPost]
        public ActionResult Create(PasswordQuestion bankToCreate)
        {
            try
            {
                //PasswordQuestionToCreate.PasswordQuestionID = System.Guid.NewGuid();

                //PasswordQuestionToCreate.CreatedByID = System.Guid.NewGuid();
                //PasswordQuestionToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(bankToCreate))
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
        // GET: /PasswordQuestion/Edit/5

        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /PasswordQuestion/Edit/5

        [HttpPost]
        public ActionResult Edit(Guid id, PasswordQuestion bankToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(bankToUpdate))
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
        // GET: /PasswordQuestion/Delete/5

        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /PasswordQuestion/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            try
            {
                if (_service.Delete(_service.GetById(id)))
                    return RedirectToAction("Index");
                else
                    return View(_service.GetById(id));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View();
            }
        }
    }
}
