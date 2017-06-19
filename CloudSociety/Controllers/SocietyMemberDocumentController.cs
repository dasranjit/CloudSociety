using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyMemberDocumentController : Controller
    {
        private SocietyMemberDocumentService _service;
        const string _exceptioncontext = "SocietyMemberDocument Controller";
        public SocietyMemberDocumentController()
        {
            _service = new SocietyMemberDocumentService(this.ModelState);
        }

        // GET: /To display list of Society Member Nominee added by Ranjit
        public ActionResult Index(Guid societyMemberID, Guid societySubscriptionID)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
            return View(_service.ListByParentId(societyMemberID));
        }

        // GET: /To display Detail list of Society Member Nominee added by Ranjit
        public ActionResult Details(Guid id, Guid societySubscriptionID) // id = SocietyMemberDocumentID,
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(new CloudSociety.Services.SocietyMemberDocumentService(this.ModelState).GetById(id).SocietyMemberID);
            return View(_service.GetById(id));
        }

        // GET: /To Create Society Member Nominee added by Ranjit
        public ActionResult Create(Guid societyMemberID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
            ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            return View();
        }

        // POST: //To Create Society Member Nominee added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid societyMemberID, Guid societySubscriptionID, SocietyMemberDocument SocietyMemberDocumentToCreate)
        {
            try
            {
                if (_service.Add(SocietyMemberDocumentToCreate))
                {
                    return RedirectToAction("Index", new { societyMemberID = societyMemberID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
                    ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    return View();
                }
            }

            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
                ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                return View();
            }
        }

        // GET: /Edit Society Member Nominee added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) // id = SocietyMemberDocumentID
        {

            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var SocietyMemberDocument = new CloudSociety.Services.SocietyMemberDocumentService(this.ModelState).GetById(id);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(SocietyMemberDocument.SocietyMemberID);
            ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            return View(_service.GetById(id));
        }

        // POST: /Edit Society Member Nominee added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyMemberDocument SocietyMemberDocumentToEdit) // id = SocietyMemberDocumentID 
        {
            var SocietyMemberDocument = _service.GetById(id);
            try
            {
                if (_service.Edit(SocietyMemberDocumentToEdit))
                {
                    return RedirectToAction("Index", new { societyMemberID = SocietyMemberDocument.SocietyMemberID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(SocietyMemberDocument.SocietyMemberID);
                    ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(SocietyMemberDocument.SocietyMemberID);
                ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                return View(_service.GetById(id));
            }
        }
        // GET: /Delete Society Member Nominee added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) // id = SocietyMemberDocumentID
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // POST: /Edit Society Member Nominee added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyMemberDocument SocietyMemberDocumentToDelete) // id = SocietyMemberDocumentID 
        {
            var SocietyMemberDocument = new CloudSociety.Services.SocietyMemberDocumentService(this.ModelState).GetById(id);
            try
            {
                if (_service.Delete(SocietyMemberDocument))
                {
                    return RedirectToAction("Index", new { societyMemberID = SocietyMemberDocument.SocietyMemberID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    return View(SocietyMemberDocument);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                return View(SocietyMemberDocument);
            }
        }
    }
}
