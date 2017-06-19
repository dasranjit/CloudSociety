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
    public class DocumentTypeRepository : IGenericRepository<DocumentType>
    {
        const string _entityname = "DocumentType";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public DocumentType GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.DocumentTypes.FirstOrDefault(s => s.DocumentTypeID == id);
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

        public bool Add(DocumentType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.DocumentTypeID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.DocumentTypes.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.DocumentTypeID.ToString() + ", " + "Document Type: " + entity.DocumentType1);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(DocumentType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalDocumentType = entities.DocumentTypes.FirstOrDefault(s => s.DocumentTypeID == entity.DocumentTypeID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.DocumentTypes.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.DocumentTypeID.ToString() + ", " + "DocumentType: " + entity.DocumentType1);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(DocumentType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var DocumentType = entities.DocumentTypes.FirstOrDefault(s => s.DocumentTypeID == entity.DocumentTypeID);
                entities.DocumentTypes.DeleteObject(DocumentType);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.DocumentTypeID.ToString() + ", " + "DocumentType: " + entity.DocumentType1);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<DocumentType> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.DocumentTypes.OrderBy(b => b.DocumentType1).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}