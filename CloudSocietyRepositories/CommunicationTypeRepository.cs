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
    public class CommunicationTypeRepository : ICommunicationTypeRepository
    {
        const string _entityname = "CommunicationType";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public CommunicationType GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.CommunicationTypes.FirstOrDefault(s => s.CommunicationTypeID == id);
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

        public bool Add(CommunicationType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CommunicationTypeID = Guid.NewGuid();
                entity.Active = true;
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.CommunicationTypes.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.CommunicationTypeID.ToString() + ", " + "Name: " + entity.CommunicationType1);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(CommunicationType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.CommunicationTypes.FirstOrDefault(s => s.CommunicationTypeID == entity.CommunicationTypeID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entity.UpdatedOn = DateTime.Now;
                entities.CommunicationTypes.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.CommunicationTypeID.ToString() + ", " + "Name: " + entity.CommunicationType1);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(CommunicationType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.CommunicationTypes.FirstOrDefault(s => s.CommunicationTypeID == entity.CommunicationTypeID);
                entities.CommunicationTypes.DeleteObject(OriginalEntity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.CommunicationTypeID.ToString() + ", " + "Name: " + entity.CommunicationType1);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<CommunicationType> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.CommunicationTypes.OrderBy(t => t.CommunicationType1).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<CommunicationType> ListForMember()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.CommunicationTypes.Where(t => !t.OnlyForOfficeBearer).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Member");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}