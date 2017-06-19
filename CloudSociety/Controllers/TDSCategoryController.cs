using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TDSCategoryController : Controller
    {
        private TDSCategoryService _service;
        const string _exceptioncontext = "TDSCategory Controller";

        public TDSCategoryController()
        {
            _service = new TDSCategoryService(this.ModelState);
        }
        //
        // GET: /TDSCategory/
        public ActionResult Index()
        {
            return View(_service.List());
        }

        //
        // GET: /TDSCategory/Details/5
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /TDSCategory/Create

        public ActionResult Create()
        {
            var _StdAcHdService = new StandardAcHeadService(this.ModelState);
            var list = _StdAcHdService.List();
            ViewBag.Standardtds = list;
            return View();        }

        //
        // POST: /TDSCategory/Create

        [HttpPost]
        public ActionResult Create(TDSCategory TDSCategoryToCreate)
        {
            try
            {
                //TDSCategoryToCreate.TDSCategoryID = System.Guid.NewGuid();

                //TDSCategoryToCreate.CreatedByID = System.Guid.NewGuid();
                //TDSCategoryToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(TDSCategoryToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                    var list = _StdAcHdService.List();
                    ViewBag.Standardtds = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                var list = _StdAcHdService.List();
                ViewBag.Standardtds = list;
                return View();
            }
        }

        // GET: /TDSCategory/Edit/5
        public ActionResult Edit(Guid id)
        {
            var _StdAcHdService = new StandardAcHeadService(this.ModelState);
            var list = _StdAcHdService.List();
            ViewBag.Standardtds = list;
            return View(_service.GetById(id));
        }


        //
        // POST: /TDSCategory/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, TDSCategory TDSCategoryToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(TDSCategoryToUpdate))
                    return RedirectToAction("Index");
                else
                {
                    var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                    var list = _StdAcHdService.List();
                    ViewBag.Standardtds = list;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var _StdAcHdService = new StandardAcHeadService(this.ModelState);
                var list = _StdAcHdService.List();
                ViewBag.Standardtds = list;
                return View();
            }
        }

        //
        // GET: /TDSCategory/Delete/5
        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /TDSCategory/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, TDSCategory TDSCategoryToDelete)
        {
            try
            {
                //TDSCategoryRateService TDSCategoryRateService = new TDSCategoryRateService(this.ModelState);
                //var TDSCategoryRatelist = TDSCategoryRateService.ListByParentId(TDSCategoryToDelete.TDSCategoryID); 
                //foreach(var TDSCategoryRate in TDSCategoryRatelist)
                //{
                //    if (!TDSCategoryRateService.Delete(TDSCategoryRate))
                //    {
                //        this.ModelState.AddModelError(_exceptioncontext + " - Delete ", _exceptioncontext + " Errors occurred while delete TDSCategoryRate");
                //        return View(_service.GetById(id));
                //    }
                //}  
                if (_service.Delete(TDSCategoryToDelete))
                    return RedirectToAction("Index");
                else
                {
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                return View(_service.GetById(id));
            }
        }
    }
}
