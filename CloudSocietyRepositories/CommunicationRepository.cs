using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class CommunicationRepository : ICommunicationRepository
    {
        const string _entityname = "Communication";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public Communication GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Communications.FirstOrDefault(s => s.CommunicationID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(Communication entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CommunicationID = Guid.NewGuid();
                entity.LastUpdate = DateTime.Now;
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.Communications.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.CommunicationID.ToString() + ", " + "Subject: " + entity.Subject);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(Communication entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.Communications.FirstOrDefault(s => s.CommunicationID == entity.CommunicationID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entity.UpdatedOn = DateTime.Now;
                entities.Communications.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.CommunicationID.ToString() + ", " + "Subject: " + entity.Subject);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(Communication entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.Communications.FirstOrDefault(s => s.CommunicationID == entity.CommunicationID);
                entities.Communications.DeleteObject(OriginalEntity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.CommunicationID.ToString() + ", " + "Subject: " + entity.Subject);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<Communication> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Communications.ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<Communication> ListMessagesFromSocietyMemberID(Guid societymemberid, bool published)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.Communications.Where(s => s.FromSocietyMemberID == societymemberid && s.Published == published && !s.Closed).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List From");
        //        sb.AppendLine("ID: " + societymemberid.ToString() + ", " + "Published? " + (published ? "Yes" : "No"));
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<Communication> ListMessagesToSocietyMemberID(Guid societymemberid)
        //{
        //    try
        //    {
        //        var entities = new CloudSocietyModels.CloudSocietyEntities();
        //        return entities.Communications.Where(s => s.CommunicationRecipients.Any(r => r.SocietyMemberID == societymemberid) && !s.Closed).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - " + _entityname + " List To");
        //        sb.AppendLine("ID: " + societymemberid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<Communication> ListDraftMessagesFromSocietyMemberID(Guid societymemberid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Communications.Where(s => s.FromSocietyMemberID == societymemberid && !s.Published && !s.Closed).OrderByDescending(s => s.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Draft Messages From");
                sb.AppendLine("ID: " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Communication> ListDraftMessagesFromSocietyMemberID(Guid societymemberid, Guid societyId)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                bool IsOfficeBearer = Roles.IsUserInRole(curUser.UserName, "OfficeBearer");
                return entities.Communications.Where(s => (s.FromSocietyMemberID == societymemberid || (IsOfficeBearer && s.SocietyID == societyId && s.CommunicationType.IsApprovalNeeded)) && !s.Published && !s.Closed).OrderByDescending(s => s.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Draft Messages From");
                sb.AppendLine("ID: " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Communication> ListPublishedMessagesFromSocietyMemberID(Guid societymemberid, DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                end = end.AddDays(1);
                if (TicketStatus == "Open")
                {
                    return entities.Communications.Where(s => s.FromSocietyMemberID == societymemberid && s.Published && !s.Closed && s.CreatedOn >= start && s.CreatedOn < end && (communicationtypeid == null || s.CommunicationTypeID == communicationtypeid) && (ticketNo == null || s.TicketNumber == ticketNo)).OrderByDescending(s => s.CreatedOn).ToList();
                }
                else if (TicketStatus == "Closed")
                {
                    return entities.Communications.Where(s => s.FromSocietyMemberID == societymemberid && s.Published && s.Closed && s.CreatedOn >= start && s.CreatedOn < end && (communicationtypeid == null || s.CommunicationTypeID == communicationtypeid) && (ticketNo == null || s.TicketNumber == ticketNo)).OrderByDescending(s => s.CreatedOn).ToList();
                }
                else
                {
                    return entities.Communications.Where(s => s.FromSocietyMemberID == societymemberid && s.Published && s.CreatedOn >= start && s.CreatedOn < end && (communicationtypeid == null || s.CommunicationTypeID == communicationtypeid) && (ticketNo == null || s.TicketNumber == ticketNo)).OrderByDescending(s => s.CreatedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List Published Messages From");
                sb.AppendLine("ID: " + societymemberid.ToString() + " between " + start.ToString("dd-MMM-yy") + " and " + end.ToString("dd-MMM-yy"));
                if (communicationtypeid != null)
                {
                    sb.AppendLine(" for Type " + communicationtypeid.ToString());
                }
                if (ticketNo != null)
                {
                    sb.AppendLine(" for Ticket # " + String.Format("{0:000000}", ticketNo));
                }
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Communication> ListMessagesToSocietyMemberID(Guid societymemberid, DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                end = end.AddDays(1);
                if (TicketStatus == "Open")
                {
                    return entities.Communications.Where(s => s.CommunicationRecipients.Any(r => r.SocietyMemberID == societymemberid) && !s.Closed && s.CreatedOn >= start && s.CreatedOn < end && (communicationtypeid == null || s.CommunicationTypeID == communicationtypeid) && (ticketNo == null || s.TicketNumber == ticketNo)).OrderByDescending(s => s.CreatedOn).ToList();
                }
                else if (TicketStatus == "Closed")
                {
                    return entities.Communications.Where(s => s.CommunicationRecipients.Any(r => r.SocietyMemberID == societymemberid) && s.Closed && s.CreatedOn >= start && s.CreatedOn < end && (communicationtypeid == null || s.CommunicationTypeID == communicationtypeid) && (ticketNo == null || s.TicketNumber == ticketNo)).OrderByDescending(s => s.CreatedOn).ToList();
                }
                else
                {
                    return entities.Communications.Where(s => s.CommunicationRecipients.Any(r => r.SocietyMemberID == societymemberid) && s.CreatedOn >= start && s.CreatedOn < end && (communicationtypeid == null || s.CommunicationTypeID == communicationtypeid) && (ticketNo == null || s.TicketNumber == ticketNo)).OrderByDescending(s => s.CreatedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List To");
                sb.AppendLine("ID: " + societymemberid.ToString() + " between " + start.ToString("dd-MMM-yy") + " and " + end.ToString("dd-MMM-yy"));
                if (communicationtypeid != null)
                {
                    sb.AppendLine(" for Type " + communicationtypeid.ToString());
                }
                if (ticketNo != null)
                {
                    sb.AppendLine(" for Ticket # " + String.Format("{0:000000}", ticketNo));
                }
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

    }
}