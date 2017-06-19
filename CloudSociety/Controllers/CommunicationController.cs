using System;
using System.Web.Mvc;
using System.Text;
using CloudSocietyEntities;
using CloudSociety.Services;
using System.Linq;
using System.IO;
using System.Threading;
using System.Web.Security;
using System.Collections.Generic;


namespace CloudSociety.Controllers
{
    [Authorize(Roles = "Member,OfficeBearer")]
    public class CommunicationController : Controller
    {
        private CommunicationService _service;

        const string _exceptioncontext = "Communication Controller";

        public CommunicationController()
        {
            _service = new CommunicationService(this.ModelState);
        }

        // GET: /Communication/
        public ActionResult Index(Guid id, Guid? communicationtypeid = null, DateTime? FromDate = null, DateTime? ToDate = null, long? ticketNo = null, string TicketStatus = null) // id = SocietySubscriptionID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            var societySubscription = societySubscriptionService.GetById(id);
            ViewBag.SocietySubscriptionID = societySubscription.SocietySubscriptionID;
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.id = id;
            var ctService = new CommunicationTypeService(this.ModelState);
            var communicationTypeList = ctService.List();
            ViewBag.communicationTypeList = communicationTypeList;
            if (FromDate == null || FromDate < societySubscription.SubscriptionStart)
                FromDate = societySubscription.SubscriptionStart;
            if (FromDate > societySubscription.SubscriptionEnd)
                FromDate = societySubscription.SubscriptionEnd;
            if (ToDate == null || ToDate > societySubscription.SubscriptionEnd)
                ToDate = societySubscription.SubscriptionEnd;
            if (ToDate < FromDate)
                ToDate = FromDate;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ticketNo = ticketNo;
            //ViewBag.ShowAllClosed = ShowAllClosed;
            List<SelectListItem> TicketStatusList = new List<SelectListItem>();
            TicketStatusList.Add(new SelectListItem { Text = "All", Value = "All", Selected = true });
            TicketStatusList.Add(new SelectListItem { Text = "Closed", Value = "Closed" });
            TicketStatusList.Add(new SelectListItem { Text = "Open", Value = "Open" });
            ViewBag.TicketStatusList = TicketStatusList;
            ViewBag.ShowMemberMenu = true;
            ViewBag.ShowFinalReports = societySubscription.Closed;
            ViewBag.ShowCommunication = new SocietyMemberService(this.ModelState).IsCommunicationEnabled(societySubscription.SocietyID);
            ViewBag.DraftMessages = _service.ListDraftMessagesByMeAndSocietyId(societySubscription.SocietyID);
            TicketStatus = string.IsNullOrWhiteSpace(TicketStatus) ? "All" : TicketStatus;
            ViewBag.PublishedMessages = _service.ListPublishedMessagesByMe((DateTime)FromDate, (DateTime)ToDate, communicationtypeid, ticketNo, TicketStatus);
            ViewBag.Message = TempData["AprovalMessage"];
            return View(_service.ListMessagesToMe((DateTime)FromDate, (DateTime)ToDate, communicationtypeid, ticketNo, TicketStatus));
        }

        // GET: /Communication/Create
        public ActionResult Create(Guid id) // id = SocietySubscriptionID
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.id = id;
            var societySubscription = societySubscriptionService.GetById(id);
            var objSocietyCommunicationSetting = societySubscription.Society.SocietyCommunicationSettings.FirstOrDefault();
            ViewBag.MaxFileSize = null != objSocietyCommunicationSetting ? objSocietyCommunicationSetting.MaxFileSizeInMB : 1;
            var ctService = new CommunicationTypeService(this.ModelState);
            var communicationTypeList = ctService.List();
            if(!objSocietyCommunicationSetting.IsGroupDiscussionActive){
                communicationTypeList = communicationTypeList.Where(i => !i.CommunicationType1.Contains("Group Discussion"));
            }
            ViewBag.communicationTypeList = communicationTypeList;
            return View();
        }

        // POST: /Communication/Create
        [HttpPost]
        public ActionResult Create(Guid id, Communication CommunicationToCreate)
        {
            try
            {
                if (_service.Add(CommunicationToCreate))
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            //this.ModelState.AddModelError(_exceptioncontext + " - Create", _exceptioncontext + " " + ex.Message);
                            string fileRepositoryPath = System.Configuration.ConfigurationManager.AppSettings["FileRepositoryPath"];
                            var fileName = Path.GetFileName(file.FileName);
                            fileRepositoryPath = Server.MapPath(fileRepositoryPath + CommunicationToCreate.CommunicationID.ToString());
                            DirectoryInfo di = new DirectoryInfo(fileRepositoryPath);
                            if (!di.Exists)
                            {
                                di = Directory.CreateDirectory(fileRepositoryPath);
                            }
                            if (di.EnumerateFiles().Any())
                            {
                                Array.ForEach(Directory.GetFiles(fileRepositoryPath), delegate(string tempPath) { System.IO.File.Delete(tempPath); });
                            }
                            file.SaveAs(Path.Combine(fileRepositoryPath, fileName));
                        }
                    }
                    var communicationType = new CommunicationTypeService(this.ModelState).GetById(CommunicationToCreate.CommunicationTypeID);
                    if (null != communicationType)
                    {
                        if (communicationType.IsApprovalNeeded)
                        {
                            TempData["AprovalMessage"] = "The created message will go under approval process.";
                        }
                    }
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.id = id;
                    var ctService = new CommunicationTypeService(this.ModelState);
                    ViewBag.communicationTypeList = ctService.List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Create", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.id = id;
                var ctService = new CommunicationTypeService(this.ModelState);
                ViewBag.communicationTypeList = ctService.List();
                return View();
            }
        }

        // GET: /Communication/Edit/5
        public ActionResult Edit(Guid id, Guid communicationId)
        {
            string fileRepositoryPath = System.Configuration.ConfigurationManager.AppSettings["FileRepositoryPath"];
            string filePath = fileRepositoryPath + communicationId.ToString();
            fileRepositoryPath = Server.MapPath(filePath);
            ViewBag.FilePath = "";
            ViewBag.FileName = "";
            if (Directory.Exists(fileRepositoryPath))
            {
                DirectoryInfo di = new DirectoryInfo(fileRepositoryPath);
                var tempFile = di.GetFiles().FirstOrDefault();
                if (null != tempFile)
                {
                    ViewBag.FilePath = Request.Url.GetLeftPart(System.UriPartial.Authority) + filePath + "/" + tempFile.Name;
                    ViewBag.FileName = tempFile.Name;
                }
            }
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.id = id;
            var societySubscription = societySubscriptionService.GetById(id);
            var objSocietyCommunicationSetting = societySubscription.Society.SocietyCommunicationSettings.FirstOrDefault();
            ViewBag.MaxFileSize = null != objSocietyCommunicationSetting ? objSocietyCommunicationSetting.MaxFileSizeInMB : 1;
            var ctService = new CommunicationTypeService(this.ModelState);
            var communicationTypeList = ctService.List();
            if (!objSocietyCommunicationSetting.IsGroupDiscussionActive)
            {
                communicationTypeList = communicationTypeList.Where(i => !i.CommunicationType1.Contains("Group Discussion"));
            }
            ViewBag.communicationTypeList = communicationTypeList;
            return View(_service.GetById(communicationId));
        }

        // POST: /Communication/Edit/5
        [HttpPost]
        public ActionResult Edit(Guid id, Communication CommunicationToUpdate)
        {
            try
            {
                if (_service.Edit(CommunicationToUpdate))
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileRepositoryPath = System.Configuration.ConfigurationManager.AppSettings["FileRepositoryPath"];
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            fileRepositoryPath = Server.MapPath(fileRepositoryPath + CommunicationToUpdate.CommunicationID.ToString());
                            DirectoryInfo di = new DirectoryInfo(fileRepositoryPath);
                            if (!di.Exists)
                            {
                                di = Directory.CreateDirectory(fileRepositoryPath);
                            }
                            if (di.EnumerateFiles().Any())
                            {
                                Array.ForEach(Directory.GetFiles(fileRepositoryPath), delegate(string tempPath) { System.IO.File.Delete(tempPath); });
                            }
                            file.SaveAs(Path.Combine(fileRepositoryPath, fileName));
                        }
                    }
                    var communicationType = new CommunicationTypeService(this.ModelState).GetById(CommunicationToUpdate.CommunicationTypeID);
                    if (null != communicationType)
                    {
                        if (communicationType.IsApprovalNeeded)
                        {
                            TempData["AprovalMessage"] = "The updated message will go under approval process.";
                        }
                    }
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.id = id;
                    var ctService = new CommunicationTypeService(this.ModelState);
                    ViewBag.communicationTypeList = ctService.List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.id = id;
                var ctService = new CommunicationTypeService(this.ModelState);
                ViewBag.communicationTypeList = ctService.List();
                return View();
            }
        }

        // GET: /Communication/Delete/5
        public ActionResult Delete(Guid id, Guid communicationId)
        {
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.id = id;
            var entity = _service.GetById(communicationId);
            var ctService = new CommunicationTypeService(this.ModelState);
            var ctEntity = ctService.GetById(entity.CommunicationTypeID);
            ViewBag.ct = ctEntity.CommunicationType1;
            return View(entity);
        }

        // POST: /Communication/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id, Guid CommunicationID, FormCollection collection)
        {
            try
            {
                if (_service.Delete(_service.GetById(CommunicationID)))
                    return RedirectToAction("Index", new { id = id });
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.id = id;
                    var entity = _service.GetById(CommunicationID);
                    var ctService = new CommunicationTypeService(this.ModelState);
                    var ctEntity = ctService.GetById(entity.CommunicationTypeID);
                    ViewBag.ct = ctEntity.CommunicationType1;
                    return View(entity);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Get", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.id = id;
                var entity = _service.GetById(CommunicationID);
                var ctService = new CommunicationTypeService(this.ModelState);
                var ctEntity = ctService.GetById(entity.CommunicationTypeID);
                ViewBag.ct = ctEntity.CommunicationType1;
                return View(entity);
            }
        }

        // GET: /Communication/Detail
        public ActionResult Detail(Guid id, Guid communicationId, bool myMessage)
        {
            string fileRepositoryPath = System.Configuration.ConfigurationManager.AppSettings["FileRepositoryPath"];
            string filePath = fileRepositoryPath + communicationId.ToString();
            fileRepositoryPath = Server.MapPath(filePath);
            ViewBag.FilePath = "";
            ViewBag.FileName = "";
            if (Directory.Exists(fileRepositoryPath))
            {
                DirectoryInfo di = new DirectoryInfo(fileRepositoryPath);
                var tempFile = di.GetFiles().FirstOrDefault();
                if (null != tempFile)
                {
                    ViewBag.FilePath = Request.Url.GetLeftPart(System.UriPartial.Authority) + filePath + "/" + tempFile.Name;
                    ViewBag.FileName = tempFile.Name;
                }
            }
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.id = id;
            var entity = _service.GetById(communicationId);

            if (entity.ApprovedBySocietyMemberID.HasValue)
            {
                entity.ApprovedByWithUnit = entity.SocietyMember1.Member + "( " + entity.SocietyMember1.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + entity.SocietyMember1.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit + ")";
            }

            ViewBag.MyMessage = myMessage;
            var ctEntity = new CommunicationTypeService(this.ModelState).GetById(entity.CommunicationTypeID);
            ViewBag.CommunicationType = ctEntity;
            var recipientService = new CommunicationRecipientService(this.ModelState);
            var recipientList = recipientService.ListByParentId(communicationId);
            var sb = new StringBuilder();
            sb.Append("<table style=\"width:100%\">");
            int i = 0;
            foreach (var item in recipientList)
            {
                if (i == 4)
                {
                    i = 0;
                }
                if (i == 0)
                {
                    sb.Append("<tr class=\"TableRow\">");
                }
                sb.Append("<td style=\"width:25%\"  border=\"0\" cellpadding=\"2\" cellspacing=\"0\" class=\"table\">");
                if (item.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                {
                    sb.Append(item.SocietyMember.Member + " (" + item.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + item.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit + ")");
                }
                else
                {
                    sb.Append(item.SocietyMember.Member + " ( NA )");
                }
                sb.Append("</td>");
                if (i == 4)
                {
                    sb.Append("</tr>");
                }
                i++;
            }
            sb.Append("</table>");
            ViewBag.recipients = sb.ToString();
            var replyService = new CommunicationReplyService(this.ModelState);
            var replies = replyService.ListByParentId(communicationId);
            ViewBag.replies = replies;
            if (!entity.Closed)
            {
                if (entity.CommunicationType.NeedToClose)
                {
                    var createdBy = Membership.GetUser(entity.CreatedByID);
                    if (null != createdBy)
                    {
                        if (Roles.IsUserInRole(createdBy.UserName, "OfficeBearer") || entity.CreatedByID == (Guid)createdBy.ProviderUserKey)
                        {
                            entity.ShowClose = true;
                        }
                    }
                }
            }
            ViewBag.ShowEmailAndSms = IsToShowSmsAndEmail(entity, ctEntity.CommunicationType1);

            return View(entity);
        }

        // POST: /Communication/Detail
        [HttpPost]
        public ActionResult Detail(Guid id, Guid CommunicationID, string Reply, bool myMessage, bool SendSMS, bool SendEmail, bool IsToClose)
        {
            try
            {
                bool IsToSendSocietyContact = false;
                var recipientService = new CommunicationRecipientService(this.ModelState);
                var replyService = new CommunicationReplyService(this.ModelState);
                if (replyService.Add(CommunicationID, Reply))
                {
                    if (IsToClose)
                    {
                        var CommunicationToUpdate = _service.GetById(CommunicationID);
                        var ud = new UserDetailService(this.ModelState).GetById((Guid)Membership.GetUser().ProviderUserKey);
                        CommunicationToUpdate.ClosedBySocietyMemberID = ud.SocietyMemberID.Value;
                        CommunicationToUpdate.ClosedOn = DateTime.Now;
                        CommunicationToUpdate.Closed = true;
                        _service.Edit(CommunicationToUpdate);
                    }

                    var communication = new CommunicationService(this.ModelState).GetById(CommunicationID);
                    if (null != communication)
                    {
                        var communicationSetting = communication.Society.SocietyCommunicationSettings.FirstOrDefault();
                        if (Roles.IsUserInRole(Membership.GetUser().UserName, "Member"))
                        {
                            IsToSendSocietyContact = true;
                            if (communication.CommunicationType.CommunicationType1.Trim().ToUpper() != "GROUP DISCUSSION")
                            {
                                SendEmail = SendSMS = true;
                            }
                            else
                            {
                                SendEmail = communicationSetting.AllowToSendSmsAndEmailForGD;
                                SendSMS = SendEmail;
                            }
                        }
                        //Send Communications
                        if (communicationSetting.IsThirdPartySmsAndEmail)
                        {
                            //TO DO : Send communication through customer's communication media
                        }
                        else
                        {
                            //TO DO : Send communication through cloud society's communication media
                            if (communicationSetting.SmsAndEmailEndsOn >= DateTime.Now && !communicationSetting.IsThirdPartySmsAndEmail)
                            {
                                //TO DO : Send SMS to All office Bearers 
                                var ud = new UserDetailService(this.ModelState).GetById((Guid)Membership.GetUser().ProviderUserKey);
                                if (SendSMS)
                                {
                                    if (communication.FromSocietyMemberID == (Guid)ud.SocietyMemberID)
                                    {
                                        recipientService.SendSMS(CommunicationID, IsToSendSocietyContact, communicationSetting.IsToReplyAll);
                                    }
                                    else
                                    {
                                        recipientService.SendSMS(CommunicationID, IsToSendSocietyContact, communicationSetting.IsToReplyAll, communication.SocietyMember.MobileNo);
                                    }
                                }
                                if (SendEmail)
                                {
                                    if (communication.FromSocietyMemberID == (Guid)ud.SocietyMemberID)
                                    {
                                        recipientService.SendEmail(CommunicationID, IsToSendSocietyContact, communicationSetting.IsToReplyAll);
                                    }
                                    else
                                    {
                                        recipientService.SendEmail(CommunicationID, IsToSendSocietyContact, communicationSetting.IsToReplyAll, Reply);
                                    }
                                }
                            }
                        }
                    }
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.id = id;
                    var entity = _service.GetById(CommunicationID);
                    ViewBag.MyMessage = myMessage;
                    var ctService = new CommunicationTypeService(this.ModelState);
                    var ctEntity = ctService.GetById(entity.CommunicationTypeID);
                    ViewBag.ct = ctEntity.CommunicationType1;
                    var sb = new StringBuilder();
                    sb.Append("<table style=\"width:100%\">");
                    int i = 0;
                    var recipientList = recipientService.ListByParentId(CommunicationID);
                    foreach (var item in recipientList)
                    {
                        if (i == 4)
                        {
                            i = 0;
                        }
                        if (i == 0)
                        {
                            sb.Append("<tr class=\"TableRow\">");
                        }
                        sb.Append("<td style=\"width:25%\"  border=\"0\" cellpadding=\"2\" cellspacing=\"0\" class=\"table\">");
                        if (item.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                        {
                            sb.Append(item.SocietyMember.Member + " (" + item.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + item.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit + ")");
                        }
                        else
                        {
                            sb.Append(item.SocietyMember.Member + " ( NA )");
                        }
                        sb.Append("</td>");
                        if (i == 4)
                        {
                            sb.Append("</tr>");
                        }
                        i++;
                    }
                    sb.Append("</table>");
                    ViewBag.recipients = sb.ToString();
                    //return View(replyService.ListByParentId(communicationId));
                    var replies = replyService.ListByParentId(CommunicationID);
                    ViewBag.replies = replies;
                    return View(entity);
                }
            }
            catch (Exception ex)
            {
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.id = id;
                var entity = _service.GetById(CommunicationID);
                //                ViewBag.Messgae = entity;
                ViewBag.MyMessage = myMessage;
                var ctService = new CommunicationTypeService(this.ModelState);
                var ctEntity = ctService.GetById(entity.CommunicationTypeID);
                ViewBag.ct = ctEntity.CommunicationType1;
                var recipientService = new CommunicationRecipientService(this.ModelState);
                var recipientList = recipientService.ListByParentId(CommunicationID);
                var sb = new StringBuilder();
                bool first = true;
                foreach (var item in recipientList)
                {
                    if (!first) sb.Append("; ");
                    sb.Append(item.SocietyMember.Member);
                    first = false;
                }
                ViewBag.recipients = sb.ToString();
                //return View(replyService.ListByParentId(communicationId));
                var replyService = new CommunicationReplyService(this.ModelState);
                var replies = replyService.ListByParentId(CommunicationID);
                ViewBag.replies = replies;
                return View(entity);
            }
        }

        // GET: /Communication/Publish
        public ActionResult Publish(Guid id, Guid communicationId)
        {
            string fileRepositoryPath = System.Configuration.ConfigurationManager.AppSettings["FileRepositoryPath"];
            string filePath = fileRepositoryPath + communicationId.ToString();
            fileRepositoryPath = Server.MapPath(filePath);
            var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
            ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
            ViewBag.id = id;
            var entity = _service.GetById(communicationId);
            var ctService = new CommunicationTypeService(this.ModelState);
            var ctEntity = ctService.GetById(entity.CommunicationTypeID);
            ViewBag.ShowEmailAndSms = IsToShowSmsAndEmail(entity, ctEntity.CommunicationType1);
            ViewBag.ct = ctEntity.CommunicationType1;
            ViewBag.FilePath = "";
            ViewBag.FileName = "";
            if (Directory.Exists(fileRepositoryPath))
            {
                DirectoryInfo di = new DirectoryInfo(fileRepositoryPath);
                var tempFile = di.GetFiles().FirstOrDefault();
                if (null != tempFile)
                {
                    ViewBag.FilePath = Request.Url.GetLeftPart(System.UriPartial.Authority) + filePath + "/" + tempFile.Name;
                    ViewBag.FileName = tempFile.Name;
                }
            }
            if (entity.CommunicationType.IsToSendSelective)
            {
                var memberService = new SocietyMemberService(this.ModelState);
                var societyMemberList = memberService.ListByParentId(entity.SocietyID).ToList();
                var OfficebearerList = memberService.ListOfficeBearerBySocietyID(entity.SocietyID).ToList();
                var MemberList = memberService.ListMemberBySocietyID(entity.SocietyID).ToList();
                SocietyMember sm = null;
                for (int i = 0; i < OfficebearerList.Count(); i++)
                {
                    entity.OfficeBearersList += i == 0 ? OfficebearerList[i].SocietyMemberID.ToString() : "," + OfficebearerList[i].SocietyMemberID.ToString();
                    sm = societyMemberList.FirstOrDefault(m => m.SocietyMemberID == OfficebearerList[i].SocietyMemberID);
                    if (null != sm)
                    {
                        if (sm.SocietyBuildingUnitTransfers.Count > 0)
                        {
                            OfficebearerList[i].Member += " (" + sm.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + sm.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit + ")";
                        }
                    }
                }
                for (int i = 0; i < MemberList.Count(); i++)
                {
                    sm = societyMemberList.FirstOrDefault(m => m.SocietyMemberID == MemberList[i].SocietyMemberID);
                    if (null != sm)
                    {
                        if (sm.SocietyBuildingUnitTransfers.Count > 0)
                        {
                            MemberList[i].Member += " (" + sm.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + sm.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit + ")";
                        }
                    }
                }
                //OfficebearerList.ForEach(x => x.Member = x.Member + (memberService.GetById(x.SocietyMemberID).SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + memberService.GetById(x.SocietyMemberID).SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit));
                //MemberList.ForEach(x => x.Member = x.Member + " (O - 20)");
                ViewBag.OfficebearerList = OfficebearerList;
                ViewBag.MemberList = MemberList;
            }
            ViewBag.IsPublishByApprover = entity.CommunicationType.IsApprovalNeeded && (Guid)Membership.GetUser().ProviderUserKey != entity.CreatedByID;
            return View(entity);
        }

        // POST: /Communication/View
        [HttpPost]
        public ActionResult Publish(Guid id, Guid CommunicationID, bool OfficeBearers, bool Members, string OfficeBearersList, string MemberList, bool SendSMS, bool SendEmail, bool IsPublishByApprover)
        {
            try
            {
                bool IsToPublish = false;
                bool IsToSendSocietyContact = false;
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                var subs = societySubscriptionService.GetById(id);
                var recipientService = new CommunicationRecipientService(this.ModelState);
                var communicationService = new CommunicationService(this.ModelState);
                var communication = communicationService.GetById(CommunicationID);
                if (null == communication)
                {
                    return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
                }
                var communicationSetting = communication.Society.SocietyCommunicationSettings.FirstOrDefault();
                if (null == communicationSetting)
                {
                    return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
                }
                if (Roles.IsUserInRole(Membership.GetUser().UserName, "Member"))
                {
                    IsToSendSocietyContact = true;
                    OfficeBearers = true;
                    OfficeBearersList = "";
                    Members = false;
                    MemberList = "";
                    if (communication.CommunicationType.CommunicationType1.Trim().ToUpper() != "GROUP DISCUSSION")
                    {
                        SendEmail = SendSMS = true;
                    }
                    else
                    {
                        SendEmail = communicationSetting.AllowToSendSmsAndEmailForGD;
                        SendSMS = SendEmail;
                    }
                }
                if (OfficeBearers)
                {
                    if (!recipientService.AddOfficeBearers(CommunicationID, subs.SocietyID))
                    {
                        return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
                    }
                    else
                    {
                        IsToPublish = true;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(OfficeBearersList))
                {
                    if (!recipientService.AddOfficeBearersList(CommunicationID, subs.SocietyID, OfficeBearersList))
                    {
                        return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
                    }
                    else
                    {
                        IsToPublish = true;
                    }
                }
                if (Members)
                {
                    if (!recipientService.AddMembers(CommunicationID, subs.SocietyID))
                    {
                        return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
                    }
                    else
                    {
                        IsToPublish = true;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(MemberList))
                {
                    if (!recipientService.AddMembersList(CommunicationID, subs.SocietyID, MemberList))
                    {
                        return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
                    }
                    else
                    {
                        IsToPublish = true;
                    }
                }
                if (IsToPublish)
                {
                    if (_service.Publish(CommunicationID, IsPublishByApprover))    // Publish only if some recipient is set
                    {
                        //Send Communications
                        if (communicationSetting.IsThirdPartySmsAndEmail)
                        {
                            //TO DO : Send communication through customer's communication media
                        }
                        else
                        {
                            //TO DO : Send communication through cloud society's communication media
                            if (communicationSetting.SmsAndEmailEndsOn >= DateTime.Now && !communicationSetting.IsThirdPartySmsAndEmail)
                            {
                                //var officebearerList = new SocietyMemberService(this.ModelState).ListOfficeBearerBySocietyID(subs.SocietyID);
                                //TO DO : Send SMS to All office Bearers  
                                if (SendSMS)
                                {
                                    recipientService.SendSMS(CommunicationID, IsToSendSocietyContact, communicationSetting.IsToReplyAll);
                                }
                                if (SendEmail)
                                {
                                    recipientService.SendEmail(CommunicationID, IsToSendSocietyContact, communicationSetting.IsToReplyAll);
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("Index", new { id = id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Publish", new { id = id, CommunicationID = CommunicationID });
            }
        }

        // GET: /Communication/Close/5
        public ActionResult Close(Guid id, Guid communicationId)
        {
            try
            {
                var CommunicationToUpdate = _service.GetById(communicationId);
                var ud = new UserDetailService(this.ModelState).GetById((Guid)Membership.GetUser().ProviderUserKey);
                CommunicationToUpdate.ClosedBySocietyMemberID = ud.SocietyMemberID.Value;
                CommunicationToUpdate.ClosedOn = DateTime.Now;
                CommunicationToUpdate.Closed = true;
                if (_service.Edit(CommunicationToUpdate))
                {
                    return RedirectToAction("Index", new { id = id });
                }
                else
                {
                    var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                    ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                    ViewBag.id = id;
                    var ctService = new CommunicationTypeService(this.ModelState);
                    ViewBag.communicationTypeList = ctService.List();
                    return View();
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(_exceptioncontext + " - Close", _exceptioncontext + " " + ex.Message);
                var societySubscriptionService = new CloudSociety.Services.SocietySubscriptionService(this.ModelState);
                ViewBag.SocietyHead = societySubscriptionService.SocietyYear(id);
                ViewBag.id = id;
                var ctService = new CommunicationTypeService(this.ModelState);
                ViewBag.communicationTypeList = ctService.List();
                return View();
            }
        }

        private bool IsToShowSmsAndEmail(Communication entity, string CommunicationType)
        {
            bool showEmailAndSms = false;
            if (!Roles.IsUserInRole(Membership.GetUser().UserName, "OfficeBearer"))
            {
                return showEmailAndSms;
            }
            var objSocietyCommunicationSettings = entity.Society.SocietyCommunicationSettings.FirstOrDefault();

            if (null != objSocietyCommunicationSettings)
            {

                showEmailAndSms = objSocietyCommunicationSettings.SmsAndEmailEndsOn >= DateTime.Now;
                if (objSocietyCommunicationSettings.IsThirdPartySmsAndEmail)
                {
                    showEmailAndSms = false;
                }
                //else
                //{
                //    showEmailAndSms = objSocietyCommunicationSettings.SmsAndEmailEndsOn >= DateTime.Now;
                //    if (CommunicationType.Trim().ToUpper() == "GROUP DISCUSSION" && showEmailAndSms)
                //    {
                //        showEmailAndSms = objSocietyCommunicationSettings.AllowToSendSmsAndEmailForGD;
                //    }
                //}
            }
            return showEmailAndSms;
        }
    }
}
