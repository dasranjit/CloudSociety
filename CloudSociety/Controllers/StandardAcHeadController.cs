using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Linq;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StandardAcHeadController : Controller
    {
        private StandardAcHeadService _service;
        const string _exceptioncontext = "StandardAcHead Controller";
        private static readonly IDictionary<string, string> _nature = new Dictionary<string, string>() { { "C", "Cash" }, { "B", "Bank" }, { "S", "Creditor" }, { "D", "Debtor" }, { "T", "TDS" }, { "F", "Year End Closing Transaction" } };

        public StandardAcHeadController()
        {
            _service = new StandardAcHeadService(this.ModelState);
        }
        //
        // GET: /StandardAcHeads/

        public ActionResult Index()
        {
            return View(_service.List());
        }

        public ActionResult IndexWithSearch(string AcHead = "", string SubCategory = "")  //id=SocietySubscriptionID
        {
            IEnumerable<StandardAcHead> StandardAcHead = _service.List();
            if (AcHead != "")
                StandardAcHead = StandardAcHead.Where(a => a.AcHead.ToLower().IndexOf(AcHead.ToLower().Trim()) != -1);
            if (SubCategory != "")
                StandardAcHead = StandardAcHead.Where(a => a.StandardAcSubCategory.SubCategory.ToLower().IndexOf(SubCategory.ToLower().Trim()) != -1);
            return View(StandardAcHead);
        }

        //
        // GET: /StandardAcHeads/Details/5

        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /StandardAcHeads/Create

        public ActionResult Create()
        {
            ViewBag.StandardAcSubCategory = new StandardAcSubCategoryService(this.ModelState).List();
            ViewBag.TDSCategory = new TDSCategoryService(this.ModelState).List();
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View();
        }

        //
        // POST: /StandardAcHeads/Create

        [HttpPost]
        public ActionResult Create(StandardAcHead StandardAcHeadsToCreate)
        {
            try
            {
                if (_service.Add(StandardAcHeadsToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.StandardAcSubCategory = new StandardAcSubCategoryService(this.ModelState).List();
                    ViewBag.TDSCategory = new TDSCategoryService(this.ModelState).List();
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.StandardAcSubCategory = new StandardAcSubCategoryService(this.ModelState).List();
                ViewBag.TDSCategory = new TDSCategoryService(this.ModelState).List();
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        //
        // GET: /StandardAcHeads/Edit/5

        public ActionResult Edit(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            ViewBag.StandardAcSubCategory = new StandardAcSubCategoryService(this.ModelState).List();
            ViewBag.TDSCategory = new TDSCategoryService(this.ModelState).List();
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardAcHeads/Edit/5

        [HttpPost]
        public ActionResult Edit(Guid id, string backUrl, StandardAcHead StandardAcHeadsToUpdate)
        {
            try
            {
                if (_service.Edit(StandardAcHeadsToUpdate))
                    return RedirectPermanent(backUrl);
                else
                {
                    ViewBag.BackUrl = backUrl;
                    ViewBag.StandardAcSubCategory = new StandardAcSubCategoryService(this.ModelState).List();
                    ViewBag.TDSCategory = new TDSCategoryService(this.ModelState).List();
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.BackUrl = backUrl;
                ViewBag.StandardAcSubCategory = new StandardAcSubCategoryService(this.ModelState).List();
                ViewBag.TDSCategory = new TDSCategoryService(this.ModelState).List();
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View(_service.GetById(id));
            }
        }

        //
        // GET: /StandardAcHeads/Delete/5

        public ActionResult Delete(Guid id, string backUrl)
        {
            ViewBag.BackUrl = backUrl;
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardAcHeads/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, string backUrl, StandardAcHead StandardAcHeadToDelete)
        {
            try
            {
                if (_service.Delete(StandardAcHeadToDelete))
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
                ViewBag.BackUrl = backUrl;
                return View(_service.GetById(id));
            }
        }
    }
}