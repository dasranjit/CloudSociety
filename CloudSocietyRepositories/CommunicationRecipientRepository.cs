using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;

namespace CloudSociety.Repositories
{
    public class CommunicationRecipientRepository : IGenericChildRepository<CommunicationRecipient>
    {
        const string _entityname = "CommunicationRecipient";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public CommunicationRecipient GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.CommunicationRecipients.FirstOrDefault(s => s.CommunicationRecipientID == id);
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

        public bool Add(CommunicationRecipient entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CommunicationRecipientID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.CommunicationRecipients.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.CommunicationID.ToString() + ", " + "Recipient ID: " + entity.SocietyMemberID);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(CommunicationRecipient entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalCommunicationRecipient = entities.CommunicationRecipients.FirstOrDefault(s => s.CommunicationRecipientID == entity.CommunicationRecipientID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entity.UpdatedOn = DateTime.Now;
                entities.CommunicationRecipients.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.CommunicationRecipientID.ToString() + ", " + "Communication ID: " + entity.CommunicationID);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(CommunicationRecipient entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var CommunicationRecipient = entities.CommunicationRecipients.FirstOrDefault(s => s.CommunicationRecipientID == entity.CommunicationRecipientID);
                entities.CommunicationRecipients.DeleteObject(CommunicationRecipient);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.CommunicationRecipientID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<CommunicationRecipient> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.CommunicationRecipients.Where(b => b.CommunicationID == parentid).OrderBy(b => b.SocietyMember.Member).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}