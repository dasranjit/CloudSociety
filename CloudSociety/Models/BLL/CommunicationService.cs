using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using CloudSocietyLib.MessagingService;
using System.Linq;

namespace CloudSociety.Services
{
    public class CommunicationService   // : ICommunicationRepository
    {
        private ICommunicationRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "Communication";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public CommunicationService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new CommunicationCache();
        }

        public bool Add(Communication entity)
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                entity.FromSocietyMemberID = (Guid)ud.SocietyMemberID;
                SocietyMemberService SocietyMemberService = new SocietyMemberService(_modelState);
                var sm = SocietyMemberService.GetById((Guid)ud.SocietyMemberID);
                entity.SocietyID = sm.SocietyID;
                if (!_modelState.IsValid)
                    return false;
                if (!_cache.Add(entity))
                {
                    _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                    return false;
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Create", GenericExceptionHandler.ExceptionMessage());
                return false;
            }
        }

        public bool Delete(Communication entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(Communication entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public Communication GetById(Guid id)
        {
            try
            {
                return (_cache.GetById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool Publish(Guid id, bool IsPublishByApprover)
        {
            try
            {
                var entity = _cache.GetById(id);
                entity.Published = true;
                entity.LastUpdate = DateTime.Now;
                if (!entity.CommunicationType.NeedToClose)
                {
                    entity.Closed = true;
                    entity.ClosedOn = DateTime.Now;
                }
                if (IsPublishByApprover)
                {
                    entity.ApprovedOn = DateTime.Now;
                    var ud = new UserDetailService(_modelState).GetById((Guid)Membership.GetUser().ProviderUserKey);
                    entity.ApprovedBySocietyMemberID = ud.SocietyMemberID.Value;
                }
                if (!_cache.Edit(entity))
                {
                    _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                    return false;
                }
                return true;
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Publish", GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public IEnumerable<Communication> ListDraftMessagesByMe()
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                return (_cache.ListDraftMessagesFromSocietyMemberID((Guid)ud.SocietyMemberID));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Draft Messages By Me", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<Communication> ListDraftMessagesByMeAndSocietyId(Guid societyId)
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                var DraftMessagesList = _cache.ListDraftMessagesFromSocietyMemberID((Guid)ud.SocietyMemberID, societyId);
                UserDetail objUserDetail = null;
                foreach (var item in DraftMessagesList)
                {
                    objUserDetail = UserDetailService.GetById(item.CreatedByID);
                    if(objUserDetail != null){
                        item.CreatedByMember = objUserDetail.SocietyMember.Member;
                    }
                }
                return (DraftMessagesList);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Draft Messages By Me", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<Communication> ListPublishedMessagesByMe(DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null)
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                var communicationList = _cache.ListPublishedMessagesFromSocietyMemberID((Guid)ud.SocietyMemberID, start, end, communicationtypeid, ticketNo, TicketStatus);
                foreach (var communication in communicationList)
                {
                    if (!communication.Closed)
                    {
                        if (communication.CommunicationType.NeedToClose)
                        {
                            var createdBy = Membership.GetUser(communication.CreatedByID);
                            if (null != createdBy)
                            {
                                if (Roles.IsUserInRole(createdBy.UserName, "OfficeBearer") || communication.CreatedByID == (Guid)createdBy.ProviderUserKey)
                                {
                                    communication.ShowClose = true;
                                }
                            }
                        }
                    }
                    if (communication.ApprovedBySocietyMemberID.HasValue)
                    {
                        communication.ApprovedByWithUnit = communication.SocietyMember1.Member + "( " + communication.SocietyMember1.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + communication.SocietyMember1.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit + ")";
                    }
                }
                return communicationList;
            }
            catch
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Published Messages By Me between " + start.ToString("dd-MMM-yy") + " and " + end.ToString("dd-MMM-yy"));
                if (communicationtypeid != null)
                {
                    sb.AppendLine(" for Type " + communicationtypeid.ToString());
                }
                if (ticketNo != null)
                {
                    sb.AppendLine(" for Ticket # " + String.Format("{0:000000}", ticketNo));
                }
                _modelState.AddModelError(sb.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<Communication> ListMessagesToMe(DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null)
        {
            try
            {
                UserDetailService UserDetailService = new UserDetailService(_modelState);
                var m = Membership.GetUser();
                var userID = (Guid)m.ProviderUserKey;
                var ud = UserDetailService.GetById(userID);
                var communicationList = _cache.ListMessagesToSocietyMemberID((Guid)ud.SocietyMemberID, start, end, communicationtypeid, ticketNo, TicketStatus);
                foreach (var communication in communicationList)
                {
                    if (!communication.Closed)
                    {
                        if (communication.CommunicationType.NeedToClose)
                        {
                            var createdBy = Membership.GetUser(communication.CreatedByID);
                            if (null != createdBy)
                            {
                                if (Roles.IsUserInRole(createdBy.UserName, "OfficeBearer") || communication.CreatedByID == (Guid)createdBy.ProviderUserKey)
                                {
                                    communication.ShowClose = true;
                                }
                            }
                        }
                    }
                }
                return communicationList;
            }
            catch
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Messages To Me between " + start.ToString("dd-MMM-yy") + " and " + end.ToString("dd-MMM-yy"));
                if (communicationtypeid != null)
                {
                    sb.AppendLine(" for Type " + communicationtypeid.ToString());
                }
                if (ticketNo != null)
                {
                    sb.AppendLine(" for Ticket # " + String.Format("{0:000000}", ticketNo));
                }
                _modelState.AddModelError(sb.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}