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
    public class SocietyMemberDocumentRepository : IGenericChildRepository<SocietyMemberDocument>
    {
        const string _entityname = "SocietyMemberDocument";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietyMemberDocument GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyMemberDocuments.FirstOrDefault(s => s.SocietyMemberDocumentID == id);
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

        public bool Add(SocietyMemberDocument entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyMemberDocumentID = Guid.NewGuid();
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entity.CreatedOn = DateTime.Now;
                entities.SocietyMemberDocuments.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Parent ID: " + entity.SocietyMemberID.ToString() + ", " + "Document Name : " + entity.DocumentName);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyMemberDocument entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietyMemberDocument = entities.SocietyMemberDocuments.FirstOrDefault(s => s.SocietyMemberDocumentID == entity.SocietyMemberDocumentID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyMemberDocuments.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("Parent ID: " + entity.SocietyMemberID.ToString() + ", " + "Document Name : " + entity.DocumentName);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyMemberDocument entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietyMemberDocument = entities.SocietyMemberDocuments.FirstOrDefault(s => s.SocietyMemberDocumentID == entity.SocietyMemberDocumentID);
                entities.SocietyMemberDocuments.DeleteObject(SocietyMemberDocument);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyMemberDocumentID.ToString() + ", " + "Document Name: " + entity.DocumentName);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyMemberDocument> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietyMemberDocuments.Where(b => b.SocietyMemberID == parentid).OrderBy(b => b.DocumentName).ToList();
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
