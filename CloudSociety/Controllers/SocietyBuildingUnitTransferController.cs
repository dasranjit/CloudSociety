using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyBuildingUnitTransferController : Controller
    {
        private SocietyBuildingUnitTransferService _service;
        const string _exceptioncontext = "Society Building Unit Transfer Controller";
        public SocietyBuildingUnitTransferController()
        {
            _service = new SocietyBuildingUnitTransferService(this.ModelState);
        }

        //
        // GET: /SocietyBuildingUnitTransfer/

        public ActionResult Index(Guid societyBuildingUnitID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            return View(_service.ListByParentId(societyBuildingUnitID));
        }

        //
        // GET: /SocietyBuildingUnitTransfer/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SocietyBuildingUnitTransfer/Create

        public ActionResult Create(Guid societyBuildingUnitID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
            return View();
        }

        //
        // POST: /SocietyBuildingUnitTransfer/Create

        [HttpPost]
        public ActionResult Create(Guid societyBuildingUnitID, Guid societySubscriptionID, SocietyBuildingUnitTransfer SocietyBuildingUnitTransferToCreate)
        {
            try
            {
                if (_service.Add(SocietyBuildingUnitTransferToCreate))
                {
                    return RedirectToAction("Index", new { societyBuildingUnitID = societyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                    ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                ViewBag.SocietyBuildingUnit = new SocietyBuildingUnitService(this.ModelState).GetById(societyBuildingUnitID);
                return View();
            }
        }

        //
        // GET: /SocietyBuildingUnitTransfer/Edit/5
        public ActionResult Edit(Guid id, Guid societySubscriptionID) //id=SocietyBuildingUnitTransfer
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListByParentId(societySubscription.SocietyID);
            return View(_service.GetById(id));
        }

        //
        // POST: /SocietyBuildingUnitTransfer/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyBuildingUnitTransfer SocietyBuildingUnitTransferToEdit)
        {
            try
            {
                if (_service.Edit(SocietyBuildingUnitTransferToEdit))
                {
                    return RedirectToAction("Index", new { societyBuildingUnitID = SocietyBuildingUnitTransferToEdit.SocietyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var societySubscription = societySubscriptionService.GetById(societySubscriptionID);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyMemberList = new SocietyMemberService(this.ModelState).ListByParentId(societySubscription.SocietyID);
                return View(_service.GetById(id));
            }
        }

        //
        // GET: /SocietyBuildingUnitTransfer/Delete/5

        public ActionResult Delete(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            return View(_service.GetById(id));
        }

        //
        // POST: /SocietyBuildingUnitTransfer/Delete/5

        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyBuildingUnitTransfer SocietyBuildingUnitTransferToDelete)
        {
            var SocietyBuildingUnitTransfer = _service.GetById(id);
            try
            {
                if (_service.Delete(SocietyBuildingUnitTransfer))
                    return RedirectToAction("Index", new { societyBuildingUnitID = SocietyBuildingUnitTransfer.SocietyBuildingUnitID, societySubscriptionID = societySubscriptionID });
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    return View(SocietyBuildingUnitTransfer);
                }
            }
            catch
            {
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                return View(SocietyBuildingUnitTransfer);
            }
        }
    }
}
