using System;
using System.Web.Mvc;
using CloudSociety.Services;
using CloudSocietyEntities;

namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Support,Subscriber,SocietyAdmin,SocietyUser,CompanyAdmin,CompanyUser,TrainingUser,TrialUser")]
    public class SocietyMemberJointHolderController : Controller
    {
        private SocietyMemberJointHolderService _service;
        const string _exceptioncontext = "SocietyMemberJointHolder Controller";
        public SocietyMemberJointHolderController()
        {
            _service = new SocietyMemberJointHolderService(this.ModelState);
        }

        // GET: /To display list of Society Member JointHolder added by Ranjit
        public ActionResult Index(Guid societyMemberID, Guid societySubscriptionID)   
        {            
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(societySubscriptionID);
            return View(_service.ListByParentId(societyMemberID));
        }

        // GET: /To display Detail list of Society Member JointHolder added by Ranjit
        public ActionResult Details(Guid id, Guid societySubscriptionID)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(societySubscriptionID);
            ViewBag.ShowAccountingMenu = societySubscriptionService.AccountingEnabled(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(_service.GetById(id).SocietyMemberID);
            ViewBag.IsPrevYearAccountingEnabled = societySubscriptionService.PrevYearAccountingEnabled(societySubscriptionID);
            return View(_service.GetById(id));
        }

        // GET: /To Create Society Member JointHolder added by Ranjit
        public ActionResult Create(Guid societyMemberID, Guid societySubscriptionID)
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;          
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);           
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
            ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
            ViewBag.MemberClassList = new CloudSociety.Services.MemberClassService(this.ModelState).List();
            //ViewBag.CessationReasonList = new CloudSociety.Services.CessationReasonService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();            
            return View();
        }

        // POST: //To Create Society Member JointHolder added by Ranjit
        [HttpPost]
        public ActionResult Create(Guid societyMemberID, Guid societySubscriptionID, SocietyMemberJointHolder SocietyMemberJointHolderToCreate)//  id = SocietyMemberID
        {
            try
            {               
                if (_service.Add(SocietyMemberJointHolderToCreate))
                {
                    return RedirectToAction("Index", new { societyMemberID = societyMemberID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberID);
                    ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                    ViewBag.MemberClassList = new CloudSociety.Services.MemberClassService(this.ModelState).List();
                    //ViewBag.CessationReasonList = new CloudSociety.Services.CessationReasonService(this.ModelState).List();
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
                ViewBag.MemberClassList = new CloudSociety.Services.MemberClassService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                return View();
            }
        }

        // GET: /Edit Society Member Joint Holder added by Ranjit
        public ActionResult Edit(Guid id, Guid societySubscriptionID) // id = SocietyMemberJointHolderID 
        {

            ViewBag.SocietySubscriptionID = societySubscriptionID;
            var societyMemberJointHolder = _service.GetById(id);
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
            ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberJointHolder.SocietyMemberID);
            ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
            ViewBag.MemberClassList = new CloudSociety.Services.MemberClassService(this.ModelState).List();
            //ViewBag.CessationReasonList = new CloudSociety.Services.CessationReasonService(this.ModelState).List();
            ViewBag.StateList = new StateService().List();
            return View(_service.GetById(id));
        }

        // POST: /Edit Society Member Joint Holder added by Ranjit
        [HttpPost]
        public ActionResult Edit(Guid id, Guid societySubscriptionID, SocietyMemberJointHolder SocietyMemberJointHolderToEdit) // id = SocietyMemberJointHolderID 
        {
            var societyMemberJointHolder = _service.GetById(id);
            try
            {                
                if (_service.Edit(SocietyMemberJointHolderToEdit))
                {
                    return RedirectToAction("Index", new { societyMemberID = societyMemberJointHolder.SocietyMemberID, societySubscriptionID = societySubscriptionID });
                }
                else
                {
                    ViewBag.SocietySubscriptionID = societySubscriptionID;                    
                    ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                    ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberJointHolder.SocietyMemberID);
                    ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                    ViewBag.MemberClassList = new CloudSociety.Services.MemberClassService(this.ModelState).List();
                    //ViewBag.CessationReasonList = new CloudSociety.Services.CessationReasonService(this.ModelState).List();
                    ViewBag.StateList = new StateService().List();
                    return View(_service.GetById(id));
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                ViewBag.SocietySubscriptionID = societySubscriptionID;
                ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);
                ViewBag.SocietyMember = new CloudSociety.Services.SocietyMemberService(this.ModelState).GetById(societyMemberJointHolder.SocietyMemberID);
                ViewBag.RelationshipList = new CloudSociety.Services.RelationshipService(this.ModelState).List();
                ViewBag.MemberClassList = new CloudSociety.Services.MemberClassService(this.ModelState).List();
                //ViewBag.CessationReasonList = new CloudSociety.Services.CessationReasonService(this.ModelState).List();
                ViewBag.StateList = new StateService().List();
                return View(_service.GetById(id));
            }
        }
        // GET: /Delete Society Member Joint Holder added by Ranjit
        public ActionResult Delete(Guid id, Guid societySubscriptionID) // id = SocietyMemberJointHolderID 
        {
            ViewBag.SocietySubscriptionID = societySubscriptionID;
            ViewBag.SocietyHead = new CloudSociety.Services.SocietySubscriptionService(this.ModelState).SocietyYear(societySubscriptionID);            
            return View(_service.GetById(id));
        }

        // POST: /Delete Society Member Joint Holder added by Ranjit
        [HttpPost]
        public ActionResult Delete(Guid id, Guid societySubscriptionID, SocietyMemberJointHolder SocietyMemberJointHolderToDelete) // id = SocietyMemberJointHolderID 
        {
            var societyMemberJointHolder = _service.GetById(id);
            try
            {
                if (_service.Delete(societyMemberJointHolder))
                {
                    return RedirectToAction("Index", new { societyMemberID = societyMemberJointHolder.SocietyMemberID, societySubscriptionID = societySubscriptionID });
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
