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
    public class CommunicationReplyRepository : IGenericChildRepository<CommunicationReply>
    {
        const string _entityname = "CommunicationReply";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public CommunicationReply GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.CommunicationReplies.FirstOrDefault(s => s.CommunicationReplyID == id);
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

        public bool Add(CommunicationReply entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CommunicationReplyID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.CommunicationReplies.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.CommunicationID.ToString() + ", " + "From ID: " + entity.BySocietyMemberID);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(CommunicationReply entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalCommunicationReply = entities.CommunicationReplies.FirstOrDefault(s => s.CommunicationReplyID == entity.CommunicationReplyID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entity.UpdatedOn = DateTime.Now;
                entities.CommunicationReplies.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.CommunicationReplyID.ToString() + ", " + "Communication ID: " + entity.CommunicationID);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(CommunicationReply entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var CommunicationReply = entities.CommunicationReplies.FirstOrDefault(s => s.CommunicationReplyID == entity.CommunicationReplyID);
                entities.CommunicationReplies.DeleteObject(CommunicationReply);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.CommunicationReplyID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<CommunicationReply> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                //return entities.CommunicationReplies.Where(b => b.CommunicationID == parentid).OrderBy(b => b.SocietyMember.Member).ToList();
                return entities.CommunicationReplies.Where(b => b.CommunicationID == parentid).OrderBy(b => b.CreatedOn).ToList();
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