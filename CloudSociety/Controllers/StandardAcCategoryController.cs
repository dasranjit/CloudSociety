using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CloudSocietyEntities;
using CloudSociety.Services;
using CloudSociety.Models;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StandardAcCategoryController : Controller
    {
        private StandardAcCategoryService _service;
        private AccountMembershipService MembershipService;
        const string _exceptioncontext = "StandardAcCategory Controller";
        private static readonly IDictionary<string, string> _DrCr = new Dictionary<string, string>()
        { { "D", "Debit" }, { "C", "Credit" }  };
        private static readonly IDictionary<string, string> _nature = new Dictionary<string, string>()
        { { "I", "Income Expenditure " }, { "B", "Balance Sheet" } };
        public StandardAcCategoryController()
        {
            _service = new StandardAcCategoryService(this.ModelState);
            MembershipService = new AccountMembershipService();
        }
        //
        // GET: /StandardAccountCategory/
      
        public ActionResult Index()
        {
             
            return View(_service.List());
        }

        //
        // GET: /StandardAccountCategory/Details/5
    
        public ActionResult Details(Guid id)
        {
            return View(_service.GetById(id));
        }

        //
        // GET: /StandardAccountCategory/Create

        public ActionResult Create()
        {
            ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View();
        }

        //
        // POST: /StandardAccountCategory/Create

        [HttpPost]
        public ActionResult Create(StandardAcCategory StandardAccountCategoryToCreate)
        {
            try
            {
                //StandardAccountCategoryToCreate.StandardAccountCategoryID = System.Guid.NewGuid();

                //StandardAccountCategoryToCreate.CreatedByID = System.Guid.NewGuid();
                //StandardAccountCategoryToCreate.CreatedOn = DateTime.Now;

                if (_service.Add(StandardAccountCategoryToCreate))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }


            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        // GET: /StandardAccountCategory/Edit/5
  
        public ActionResult Edit(Guid id)
        {
            ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
            ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardAccountCategory/Edit/5
      
        [HttpPost]
        public ActionResult Edit(Guid id, StandardAcCategory StandardAccountCategoryToUpdate)
        {
            try
            {
                // TODO: Add update logic here
                //var subscriberToUpdate = _service.GetById(id);
                //this.UpdateModel(subscriberToUpdate, collection.ToValueProvider());
                if (_service.Edit(StandardAccountCategoryToUpdate))
                    return RedirectToAction("Index");
                else
                {
                    ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                    ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.DrCrList = new SelectList(_DrCr, "Key", "Value");
                ViewBag.NatureList = new SelectList(_nature, "Key", "Value");
                return View();
            }
        }

        //
        // GET: /StandardAccountCategory/Delete/5

        public ActionResult Delete(Guid id)
        {        
            return View(_service.GetById(id));
        }

        //
        // POST: /StandardAccountCategory/Delete/5
        
        [HttpPost]
        public ActionResult Delete(Guid id,StandardAcCategory StandardAcCategoryToDelete)
        {
            try
            {
                if (_service.Delete(StandardAcCategoryToDelete))
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