using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class ServiceTypeRepository : IGenericRepository<ServiceType>
    {
//        private CloudSocietyModels.CloudSocietyEntities entities = new CloudSocietyModels.CloudSocietyEntities();
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "ServiceType";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        //public ServiceTypeRepository()
        //{
        //    try 
        //    {
        //        using (entities = new CloudSocietyModels.CloudSocietyEntities()) 
        //        {
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext," - ","Entity Context Creation"));
        //        throw;
        //    }
        //}

        public ServiceType GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ServiceTypes.FirstOrDefault(s => s.ServiceTypeID == id);
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

        public bool Add(ServiceType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.ServiceTypeID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.ServiceTypes.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.ServiceTypeID.ToString() + ", " + "Name: " + entity.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(ServiceType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.ServiceTypes.FirstOrDefault(s => s.ServiceTypeID == entity.ServiceTypeID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.ServiceTypes.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.ServiceTypeID.ToString() + ", " + "Name: " + entity.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(ServiceType entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalEntity = entities.ServiceTypes.FirstOrDefault(s => s.ServiceTypeID == entity.ServiceTypeID);
                entities.ServiceTypes.DeleteObject(OriginalEntity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.ServiceTypeID.ToString() + ", " + "Name: " + entity.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<ServiceType> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ServiceTypes.OrderBy(s => s.Sequence).ToList();
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