using System;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TDSCategoryRateController : Controller
    {
        private TDSCategoryRateService _service;
        const string _exceptioncontext = "TDSCategoryRate Controller";
        public TDSCategoryRateController()
        {
            _service = new TDSCategoryRateService(this.ModelState);
        }
        
        // GET: /TDSCategortyRate/

        public ActionResult Index(Guid parentid)
        {
            var TDSCategoryservice = new TDSCategoryService(this.ModelState);
            ViewBag.TDSCategory = TDSCategoryservice.GetById(parentid);
            return View(_service.ListByParentId(parentid));
        }

      
        // GET: /TDSCategortyRate/Details/5

        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

   
        // GET: /TDSCategortyRate/Create

        public ActionResult Create(Guid TDSCategoryID)
        {
            ViewBag.TDSCategoryID = TDSCategoryID;
            return View();
        } 
        
        // POST: /TDSCategortyRate/Create

        [HttpPost]
        public ActionResult Create(Guid TDSCategoryID,TDSCategoryRate TDSCategoryRateToCreate)
        {
            try
            {
                if (_service.Add(TDSCategoryRateToCreate))
                {
                    //return RedirectToAction("Index");
                    return RedirectToAction("Index", "TDSCategoryRate", new { parentid = TDSCategoryRateToCreate.TDSCategoryID });
                }
                else
                {
                    ViewBag.TDSCategoryID = TDSCategoryID;
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.TDSCategoryID = TDSCategoryID;
                return View();
            }
        }

        // GET: /TDSCategortyRate/Edit/5

        public ActionResult Edit(Guid id)
        {
            return View(_service.GetById(id));
        }

       
        // POST: /TDSCategortyRate/Edit/5

        [HttpPost]
        public ActionResult Edit(TDSCategoryRate TDSCategoryRateToUpdate)
        {
            try
            {
                if (_service.Edit(TDSCategoryRateToUpdate))
                   
                {
                    return RedirectToAction("Index", "TDSCategoryRate", new { parentid = TDSCategoryRateToUpdate.TDSCategoryID });
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
        // GET: /TDSCategortyRate/Delete/5

        public ActionResult Delete(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // POST: /TDSCategortyRate/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, TDSCategoryRate TDSCategoryRateToDelete)
        {
            try
            {
                if (_service.Delete(TDSCategoryRateToDelete))
                    return RedirectToAction("Index", new { parentid = TDSCategoryRateToDelete.TDSCategoryID });
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
