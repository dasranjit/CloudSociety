using System.Linq;
using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using System.Net.Mail;
using System.Threading;
using System.Web.Security;

namespace CloudSociety.Services
{
    public class CommunicationRecipientService  // : IGenericChildRepository<CommunicationRecipient>
    {
        private IGenericChildRepository<CommunicationRecipient> _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "CommunicationRecipient";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;
        public enum RecipientType { Member = 1, OfficeBearer = 2, Tenant = 3 };
        public CommunicationRecipientService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new CommunicationRecipientCache();
        }

        //public CommunicationRecipient GetById(Guid id)
        //{
        //    try
        //    {
        //        return (_cache.GetById(id));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public bool AddOfficeBearers(Guid communicationId, Guid societyId)
        {
            try
            {
                CommunicationRecipient entity;
                var memberService = new SocietyMemberService(_modelState);
                var officebearerList = memberService.ListOfficeBearerBySocietyID(societyId);
                foreach (var item in officebearerList)
                {
                    entity = new CommunicationRecipient();
                    entity.CommunicationID = communicationId;
                    entity.SocietyMemberID = item.SocietyMemberID;
                    if (!_cache.Add(entity))
                    {
                        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Add Office Bearers", GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public bool AddMembers(Guid communicationId, Guid societyId)
        {
            try
            {
                CommunicationRecipient entity;
                var memberService = new SocietyMemberService(_modelState);
                var memberList = memberService.ListMemberBySocietyID(societyId);
                foreach (var item in memberList)
                {
                    entity = new CommunicationRecipient();
                    entity.CommunicationID = communicationId;
                    entity.SocietyMemberID = item.SocietyMemberID;
                    if (!_cache.Add(entity))
                    {
                        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Add Members", GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        //public bool Edit(CommunicationRecipient entity)
        //{
        //    if (!_cache.Edit(entity))
        //    {
        //        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Delete(CommunicationRecipient entity)
        //{
        //    if (!_cache.Delete(entity))
        //    {
        //        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
        //        return false;
        //    }
        //    return true;
        //}

        public IEnumerable<CommunicationRecipient> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool AddOfficeBearersList(Guid communicationID, Guid societyId, string OfficeBearersList)
        {
            try
            {
                CommunicationRecipient entity;
                var memberService = new SocietyMemberService(_modelState);
                var officebearerList = memberService.ListOfficeBearerBySocietyID(societyId);
                var arrOfficeBearersIds = OfficeBearersList.Split(',');
                foreach (var item in officebearerList)
                {
                    if (OfficeBearersList.Contains(item.SocietyMemberID.ToString()))
                    {
                        entity = new CommunicationRecipient();
                        entity.CommunicationID = communicationID;
                        entity.SocietyMemberID = item.SocietyMemberID;
                        if (!_cache.Add(entity))
                        {
                            _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Add Office Bearers", GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public bool AddMembersList(Guid communicationId, Guid societyId, string MemberIdList)
        {
            try
            {
                CommunicationRecipient entity;
                var memberService = new SocietyMemberService(_modelState);
                var memberList = memberService.ListMemberBySocietyID(societyId);
                foreach (var item in memberList)
                {
                    if (MemberIdList.Contains(item.SocietyMemberID.ToString()))
                    {
                        entity = new CommunicationRecipient();
                        entity.CommunicationID = communicationId;
                        entity.SocietyMemberID = item.SocietyMemberID;
                        if (!_cache.Add(entity))
                        {
                            _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Add Members", GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public void SendSMS(Guid communicationId, bool IsToSendSocietyContact, bool isToReplyAll, string ReplyMobileNo = "")
        {
            if (null == communicationId)
            {
                return;
            }
            var objCommunication = new CommunicationService(this._modelState).GetById(communicationId);

            if (null == objCommunication.CommunicationRecipients)
            {
                return;
            }
            if (objCommunication.CommunicationRecipients.Count() == 0)
            {
                return;
            }
            string unitNumber = "";
            string smsUrl = System.Configuration.ConfigurationManager.AppSettings["SMS_URL"];
            string messageText = "Dear Member,\n";
            messageText += "A new " + objCommunication.CommunicationType.CommunicationType1 + " message has been sent to you. Please log on www.cloudsociety.in to view more detail.\n";
            messageText += "For " + objCommunication.Society.Name;
            List<String> actulaSmsURLList = new List<String>();
            if (!string.IsNullOrWhiteSpace(ReplyMobileNo))
            {
                messageText = "Dear Member,\n";
                messageText += "You have received reply for ticket no. " + string.Format("{0:000000}", objCommunication.TicketNumber) + ". Please log on www.cloudsociety.in to view more detail.\n";
                messageText += "For " + objCommunication.Society.Name;
                if (ReplyMobileNo.Length == 10)
                {
                    actulaSmsURLList.Add(smsUrl.Replace("**MobileNo**", "91" + ReplyMobileNo).Replace("**Message**", messageText));
                }
                if (isToReplyAll)
                {
                    foreach (var objCommunicationRecipient in objCommunication.CommunicationRecipients)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(objCommunicationRecipient.SocietyMember.MobileNo))
                            {
                                if (objCommunicationRecipient.SocietyMember.MobileNo.Length == 10)
                                {
                                    actulaSmsURLList.Add(smsUrl.Replace("**MobileNo**", "91" + objCommunicationRecipient.SocietyMember.MobileNo).Replace("**Message**", messageText));
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            else if (IsToSendSocietyContact)
            {
                if (!string.IsNullOrWhiteSpace(objCommunication.Society.Mobile))
                {
                    if (objCommunication.Society.Mobile.Length == 10)
                    {
                        if (objCommunication.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                        {
                            unitNumber = objCommunication.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + objCommunication.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                        }
                        messageText = "New " + objCommunication.CommunicationType.CommunicationType1 + " has been received wide ticket no. ";
                        messageText += string.Format("{0:000000}", objCommunication.TicketNumber);
                        messageText += " from " + objCommunication.SocietyMember.Member + (string.IsNullOrWhiteSpace(unitNumber) ? "" : " (" + unitNumber + ")");
                        messageText += ". Log on www.cloudsociety.in to view detail.";
                        actulaSmsURLList.Add(smsUrl.Replace("**MobileNo**", "91" + objCommunication.Society.Mobile).Replace("**Message**", messageText));
                    }
                }
            }
            else
            {
                foreach (var objCommunicationRecipient in objCommunication.CommunicationRecipients)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(objCommunicationRecipient.SocietyMember.MobileNo))
                        {
                            if (objCommunicationRecipient.SocietyMember.MobileNo.Length == 10)
                            {
                                actulaSmsURLList.Add(smsUrl.Replace("**MobileNo**", "91" + objCommunicationRecipient.SocietyMember.MobileNo).Replace("**Message**", messageText));
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            if (actulaSmsURLList.Count > 0)
            {
                Thread thread = new Thread(() => SendSMS(actulaSmsURLList));
                thread.Name = "SendSMS";
                thread.Start();
            }
        }

        public void SendEmail(Guid communicationId, bool IsToSendSocietyContact, bool isToReplyAll, string Reply = "")
        {
            if (null == communicationId)
            {
                return;
            }
            string mailFrom = System.Configuration.ConfigurationManager.AppSettings["MailFrom"], mailBody = "", unitNumber = "";
            List<MailMessage> messageList = new List<MailMessage>(); ;
            MailMessage message;
            var appInfo = new AppInfoService(this._modelState).Get();
            var communication = new CommunicationService(this._modelState).GetById(communicationId);
            string strMessage = string.IsNullOrWhiteSpace(Reply) ? communication.Details : Reply;

            if (!string.IsNullOrWhiteSpace(Reply))
            {
                #region Send reply to creator
                if (communication.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                {
                    unitNumber = communication.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + communication.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                }
                //var ud = new UserDetailService(_modelState).GetBySocietyMemberID(communication.FromSocietyMemberID);
                var ud = Membership.GetUser();
                if (ud != null)
                {
                    if (Roles.IsUserInRole(ud.UserName, "Member"))
                    {
                        mailBody = appInfo.CommunMemberMailBody;
                    }
                    else if (Roles.IsUserInRole(ud.UserName, "OfficeBearer"))
                    {
                        mailBody = appInfo.CommunOfficeBearerMailBody;
                    }
                }
                if (!string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(communication.SocietyMember.EmailId) && !string.IsNullOrEmpty(mailBody))
                {
                    try
                    {
                        mailBody = mailBody.Replace("&&TicketNumber&&", string.Format("{0:000000}", communication.TicketNumber.Value));
                        mailBody = mailBody.Replace("&&PublishedOn&&", DateTime.Now.ToString("dd-MMM-yyyy"));
                        mailBody = mailBody.Replace("&&CommunicationType&&", communication.CommunicationType.CommunicationType1);
                        mailBody = mailBody.Replace("&&Message&&", strMessage);
                        mailBody = mailBody.Replace("&&UnitNumber&&", unitNumber);
                        mailBody = mailBody.Replace("&&MemberName&&", communication.SocietyMember.Member);
                        mailBody = mailBody.Replace("&&FromMemberName&&", communication.SocietyMember.Member);

                        mailBody = mailBody.Replace("&&SocietyName&&", communication.Society.Name);
                        mailBody = mailBody.Replace("&&RegistrationNo&&", communication.Society.TaxRegistrationNo);
                        mailBody = mailBody.Replace("&&RegistrationDate&&", string.Format("{0:dd-MMM-yyyy}", communication.Society.RegistrationDate));
                        mailBody = mailBody.Replace("&&TaxRegistrationNoWithLavel&&", string.IsNullOrWhiteSpace(communication.Society.TaxRegistrationNo) ? " " : ", Services Tax No :" + communication.Society.TaxRegistrationNo);
                        mailBody = mailBody.Replace("&&Address&&", communication.Society.Address + ", " + communication.Society.City + " - " + communication.Society.PIN);
                        mailBody = mailBody.Replace("&&SocietyContactPerson&&", communication.Society.ContactPerson);
                        mailBody = mailBody.Replace("&&SocietyContactNumber&&", communication.Society.Mobile);
                        mailBody = mailBody.Replace("&&SocietyContactEmailId&&", communication.Society.EMailId);

                        message = new MailMessage();
                        message.From = new MailAddress(mailFrom);
                        message.To.Add(new MailAddress(communication.SocietyMember.EmailId));
                        message.Subject = communication.Subject;
                        message.Body = mailBody;
                        message.IsBodyHtml = true;
                        messageList.Add(message);
                        //mailClient = new SmtpClient();
                        //mailClient.Send(message);
                    }
                    catch
                    {
                    }
                }
                #endregion
                if (!string.IsNullOrWhiteSpace(Reply) && isToReplyAll)
                {
                    #region Send to All recipient
                    var memberService = new SocietyMemberService(_modelState);
                    var recipientList = ListByParentId(communicationId);
                    var memberRecipientList = memberService.ListMemberBySocietyID(communication.SocietyID);
                    var officeBearerRecipientList = memberService.ListOfficeBearerBySocietyID(communication.SocietyID);
                    if (null != recipientList)
                    {
                        foreach (var recipient in recipientList)
                        {
                            mailBody = "";
                            if (null != recipient.SocietyMember)
                            {
                                if (recipient.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                                {
                                    unitNumber = recipient.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + recipient.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                                }
                                if (null != memberRecipientList)
                                {
                                    if (memberRecipientList.Any(mr => mr.SocietyMemberID == recipient.SocietyMemberID))
                                    {
                                        mailBody = appInfo.CommunMemberMailBody;
                                    }
                                }
                                if (null != officeBearerRecipientList)
                                {
                                    if (officeBearerRecipientList.Any(obr => obr.SocietyMemberID == recipient.SocietyMemberID))
                                    {
                                        mailBody = appInfo.CommunOfficeBearerMailBody;
                                    }
                                }
                                if (!string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(recipient.SocietyMember.EmailId) && !string.IsNullOrEmpty(mailBody))
                                {
                                    try
                                    {
                                        mailBody = mailBody.Replace("&&TicketNumber&&", string.Format("{0:000000}", communication.TicketNumber.Value));
                                        mailBody = mailBody.Replace("&&PublishedOn&&", DateTime.Now.ToString("dd-MMM-yyyy"));
                                        mailBody = mailBody.Replace("&&CommunicationType&&", communication.CommunicationType.CommunicationType1);
                                        mailBody = mailBody.Replace("&&Message&&", strMessage);
                                        mailBody = mailBody.Replace("&&UnitNumber&&", unitNumber);
                                        mailBody = mailBody.Replace("&&MemberName&&", recipient.SocietyMember.Member);
                                        mailBody = mailBody.Replace("&&FromMemberName&&", communication.SocietyMember.Member);

                                        mailBody = mailBody.Replace("&&SocietyName&&", communication.Society.Name);
                                        mailBody = mailBody.Replace("&&RegistrationNo&&", communication.Society.TaxRegistrationNo);
                                        mailBody = mailBody.Replace("&&RegistrationDate&&", string.Format("{0:dd-MMM-yyyy}", communication.Society.RegistrationDate));
                                        mailBody = mailBody.Replace("&&TaxRegistrationNoWithLavel&&", string.IsNullOrWhiteSpace(communication.Society.TaxRegistrationNo) ? " " : ", Services Tax No :" + communication.Society.TaxRegistrationNo);
                                        mailBody = mailBody.Replace("&&Address&&", communication.Society.Address + ", " + communication.Society.City + " - " + communication.Society.PIN);
                                        mailBody = mailBody.Replace("&&SocietyContactPerson&&", communication.Society.ContactPerson);
                                        mailBody = mailBody.Replace("&&SocietyContactNumber&&", communication.Society.Mobile);
                                        mailBody = mailBody.Replace("&&SocietyContactEmailId&&", communication.Society.EMailId);

                                        message = new MailMessage();
                                        message.From = new MailAddress(mailFrom);
                                        message.To.Add(new MailAddress(recipient.SocietyMember.EmailId));
                                        message.Subject = communication.Subject;
                                        message.Body = mailBody;
                                        message.IsBodyHtml = true;
                                        messageList.Add(message);
                                        //mailClient = new SmtpClient();
                                        //mailClient.Send(message);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            else if (IsToSendSocietyContact)
            {
                #region Send to Society Contact person
                mailBody = appInfo.CommunOfficeBearerMailBody;
                if (!string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(communication.Society.EMailId) && !string.IsNullOrEmpty(mailBody))
                {
                    try
                    {
                        if (communication.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                        {
                            unitNumber = communication.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + communication.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                        }
                        mailBody = mailBody.Replace("&&TicketNumber&&", string.Format("{0:000000}", communication.TicketNumber.Value));
                        mailBody = mailBody.Replace("&&PublishedOn&&", DateTime.Now.ToString("dd-MMM-yyyy"));
                        mailBody = mailBody.Replace("&&CommunicationType&&", communication.CommunicationType.CommunicationType1);
                        mailBody = mailBody.Replace("&&Message&&", strMessage);
                        mailBody = mailBody.Replace("&&UnitNumber&&", unitNumber);
                        mailBody = mailBody.Replace("&&MemberName&&", communication.SocietyMember.Member);
                        mailBody = mailBody.Replace("&&SocietyName&&", communication.Society.Name);
                        mailBody = mailBody.Replace("&&FromMemberName&&", communication.SocietyMember.Member);

                        message = new MailMessage();
                        message.From = new MailAddress(mailFrom);
                        message.To.Add(new MailAddress(communication.Society.EMailId));
                        message.Subject = communication.Subject;
                        message.Body = mailBody;
                        message.IsBodyHtml = true;
                        messageList.Add(message);
                    }
                    catch
                    {
                    }
                }
                #endregion
            }
            else
            {
                #region Send to All recipient
                var memberService = new SocietyMemberService(_modelState);
                var recipientList = ListByParentId(communicationId);
                var memberRecipientList = memberService.ListMemberBySocietyID(communication.SocietyID);
                var officeBearerRecipientList = memberService.ListOfficeBearerBySocietyID(communication.SocietyID);
                if (null != recipientList)
                {
                    foreach (var recipient in recipientList)
                    {
                        mailBody = "";
                        if (null != recipient.SocietyMember)
                        {
                            if (recipient.SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                            {
                                unitNumber = recipient.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + recipient.SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                            }
                            if (null != memberRecipientList)
                            {
                                if (memberRecipientList.Any(mr => mr.SocietyMemberID == recipient.SocietyMemberID))
                                {
                                    mailBody = appInfo.CommunMemberMailBody;
                                }
                            }
                            if (null != officeBearerRecipientList)
                            {
                                if (officeBearerRecipientList.Any(obr => obr.SocietyMemberID == recipient.SocietyMemberID))
                                {
                                    mailBody = appInfo.CommunOfficeBearerMailBody;
                                }
                            }
                            if (!communication.CommunicationType.NeedToClose)
                            {
                                mailBody = appInfo.CommunMemberMailBody;
                            }
                            if (!string.IsNullOrEmpty(mailFrom) && !string.IsNullOrEmpty(recipient.SocietyMember.EmailId) && !string.IsNullOrEmpty(mailBody))
                            {
                                try
                                {
                                    mailBody = mailBody.Replace("&&TicketNumber&&", string.Format("{0:000000}", communication.TicketNumber.Value));
                                    mailBody = mailBody.Replace("&&PublishedOn&&", DateTime.Now.ToString("dd-MMM-yyyy"));
                                    mailBody = mailBody.Replace("&&CommunicationType&&", communication.CommunicationType.CommunicationType1);
                                    mailBody = mailBody.Replace("&&Message&&", strMessage);
                                    mailBody = mailBody.Replace("&&UnitNumber&&", unitNumber);
                                    mailBody = mailBody.Replace("&&MemberName&&", recipient.SocietyMember.Member);
                                    mailBody = mailBody.Replace("&&FromMemberName&&", communication.SocietyMember.Member);

                                    mailBody = mailBody.Replace("&&SocietyName&&", communication.Society.Name);
                                    mailBody = mailBody.Replace("&&RegistrationNo&&", communication.Society.TaxRegistrationNo);
                                    mailBody = mailBody.Replace("&&RegistrationDate&&", string.Format("{0:dd-MMM-yyyy}", communication.Society.RegistrationDate));
                                    mailBody = mailBody.Replace("&&TaxRegistrationNoWithLavel&&", string.IsNullOrWhiteSpace(communication.Society.TaxRegistrationNo) ? " " : ", Services Tax No :" + communication.Society.TaxRegistrationNo);
                                    mailBody = mailBody.Replace("&&Address&&", communication.Society.Address + ", " + communication.Society.City + " - " + communication.Society.PIN);
                                    mailBody = mailBody.Replace("&&SocietyContactPerson&&", communication.Society.ContactPerson);
                                    mailBody = mailBody.Replace("&&SocietyContactNumber&&", communication.Society.Mobile);
                                    mailBody = mailBody.Replace("&&SocietyContactEmailId&&", communication.Society.EMailId);

                                    message = new MailMessage();
                                    message.From = new MailAddress(mailFrom);
                                    message.To.Add(new MailAddress(recipient.SocietyMember.EmailId));
                                    message.Subject = communication.Subject;
                                    message.Body = mailBody;
                                    message.IsBodyHtml = true;
                                    messageList.Add(message);
                                    //mailClient = new SmtpClient();
                                    //mailClient.Send(message);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            if (messageList.Count > 0)
            {
                Thread thread = new Thread(() => SendEmail(messageList));
                thread.Name = "SendEmail";
                thread.Start();
            }
        }

        private void SendEmail(List<MailMessage> messageList)
        {
            foreach (var message in messageList)
            {
                try
                {
                    new SmtpClient().Send(message);
                }
                catch
                {
                    continue;
                }
            }
        }

        private void SendSMS(List<String> actulaSmsURLList)
        {
            var objTextMessagingService = new CloudSocietyLib.MessagingService.TextMessagingService();
            foreach (var actulaSmsURL in actulaSmsURLList)
            {
                try
                {
                    objTextMessagingService.SendSMS(actulaSmsURL);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}