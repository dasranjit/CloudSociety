using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyParkingTransferController : Controller
    {
        private SocietyParkingTransferService _service;
        const string _exceptioncontext = "SocietyParkingTransfer Controller";
        public SocietyParkingTransferController()
        {
            _service = new SocietyParkingTransferService(this.ModelState);
        }

        // GET: /To display list of Society Parking Transfer added by Ranjit
        public ActionResult Index(Guid societyParkingID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingID);            
            return View(_service.ListByParentId(societyParkingID));
        }

        // GET: /To Create Society Parking Transfer added by Ranjit
        public ActionResult Create(Guid societyParkingID, Guid societySubscriptionID)
        {            
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);          
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);            
            ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingID);
            ViewBag.SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);           
            return View();
        }

        // POST: /To Create Society Parking Transfer by Ranjit
        [HttpPost]
        public ActionResult Create(Guid societyParkingID, Guid societySubscriptionID, SocietyParkingTransfer societyParkingTransferToCreate)//id = SocietyParkingID
        {
            try
            {
                if (_service.Add(societyParkingTransferToCreate))
                {
                    return RedirectToAction("Index", new { societyParkingID = societyParkingID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingID);
                    ViewBag.SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingID);
                ViewBag.SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                return View();
            }
        }

        // GET: /Edit Society Parking Transfer added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) 
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societyParkingTransfer = new CloudSociety.Services.SocietyParkingTransferService(this.ModelState).GetById(id);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingTransfer.SocietyParkingID);
            ViewBag.SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);           
            return View(_service.GetById(id));
        }

        // POST: /Edit SocietyParkingTransfer added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyParkingTransfer SocietyParkingTransferToEdit)
        {
            var societyParkingTransfer = new CloudSociety.Services.SocietyParkingTransferService(this.ModelState).GetById(id);            
            try
            {
                if (_service.Edit(SocietyParkingTransferToEdit))
                {
                    return RedirectToAction("Index", new { societyParkingID = societyParkingTransfer.SocietyParkingID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;                   
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                    ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingTransfer.SocietyParkingID);
                    ViewBag.SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
                ViewBag.SocietyParking = new CloudSociety.Services.SocietyParkingService(this.ModelState).GetById(societyParkingTransfer.SocietyParkingID);
                ViewBag.SocietyMemberList = new CloudSociety.Services.SocietyMemberService(this.ModelState).ListByParentId(societySubscriptionService.GetById(societySubscriptionID).SocietyID);
                return View(_service.GetById(id));              
            }
        }

        // GET: /Delete Society Parking Transfer added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /Delete SocietyParkingTransfer added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyParkingTransfer SocietyParkingTransferToDelete)
        {
            SocietyParkingTransfer SocietyParkingTransfer = _service.GetById(id);
            try
            {
                if (_service.Delete(SocietyParkingTransfer))
                {
                    return RedirectToAction("Index", new { societyParkingID = SocietyParkingTransfer.SocietyParkingID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View(_service.GetById(id));
            }
        }

    }
}
