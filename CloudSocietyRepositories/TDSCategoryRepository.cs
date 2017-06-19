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
    public class TDSCategoryRepository : IGenericRepository<TDSCategory>
    {
        //private CloudSocietyModels.CloudSocietyEntities entities = new CloudSocietyModels.CloudSocietyEntities();
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "TDSCategory";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        //public TDSCategoryRepository()
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

        public TDSCategory GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TDSCategories.FirstOrDefault(s => s.TDSCategoryID == id);
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

        public bool Add(TDSCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.TDSCategoryID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.TDSCategories.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.TDSCategoryID.ToString() + ", " + "Name: " + entity.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(TDSCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalTDSCategory = entities.TDSCategories.FirstOrDefault(s => s.TDSCategoryID == entity.TDSCategoryID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.TDSCategories.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.TDSCategoryID.ToString() + ", " + "Name: " + entity.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(TDSCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var TDSCategory = entities.TDSCategories.FirstOrDefault(s => s.TDSCategoryID == entity.TDSCategoryID);
                entities.TDSCategories.DeleteObject(TDSCategory);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.TDSCategoryID.ToString() + ", " + "Name: " + entity.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<TDSCategory> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TDSCategories.ToList();
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