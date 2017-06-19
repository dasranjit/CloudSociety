using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StandardAcSubCategoryController : Controller
    {
        private StandardAcSubCategoryService _service;
        const string _exceptioncontext = "StandardAcSubCategory Controller";
        public StandardAcSubCategoryController()
        {
            _service = new StandardAcSubCategoryService(this.ModelState);
        }
        //
        // GET: /StandardAcSubCategory/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /StandardAcSubCategory/Details/5
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /StandardAcSubCategory/Create

        public ActionResult Create()
        {

            var _AcCtgService = new StandardAcCategoryService(this.ModelState);
            var list = _AcCtgService.List();
            ViewBag.StandardAC = list;
            return View();
        }

        //
        // POST: /StandardAcSubCategory/Create

        [HttpPost]
        public ActionResult Create(StandardAcSubCategory StandardAcSubCategoryToCreate)
        {
            try
            {
                if (_service.Add(StandardAcSubCategoryToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var _AcCtgService = new StandardAcCategoryService(this.ModelState);
                    var list = _AcCtgService.List();
                    ViewBag.StandardAC = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _AcCtgService = new StandardAcCategoryService(this.ModelState);
                var list = _AcCtgService.List();
                ViewBag.StandardAC = list;
                return View();
            }
        }

        //
        // GET: /StandardAcSubCategory/Edit/5
        public ActionResult Edit(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl; 
            var _AcCtgService = new StandardAcCategoryService(this.ModelState);
            var list = _AcCtgService.List();
            ViewBag.StandardAC = list;
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardAcSubCategory/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, string backUrl, StandardAcSubCategory StandardAcSubCategoryToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(StandardAcSubCategoryToUpdate))
                    return RedirectPermanent(backUrl);
                else
                {
                    var _AcCtgService = new StandardAcCategoryService(this.ModelState);
                    var list = _AcCtgService.List();
                    ViewBag.StandardAC = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _AcCtgService = new StandardAcCategoryService(this.ModelState);
                var list = _AcCtgService.List();
                ViewBag.StandardAC = list;
                return View();
            }
        }

        //
        // GET: /StandardAcSubCategory/Delete/5
        public ActionResult Delete(Guid id,string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardAcSubCategory/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, string backUrl, StandardAcSubCategory StandardAcSubCategoryToDelete)
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
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.backUrl = backUrl;
                return View(_service.GetById(id));
            }
        }
    }
}

