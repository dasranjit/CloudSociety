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
    public class StandardAcCategoryRepository : IGenericRepository<StandardAcCategory>
    {
        //private CloudSocietyModels.CloudSocietyEntities entities = new CloudSocietyModels.CloudSocietyEntities();
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "StandardAcCategory";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        //public StandardAcCategoryRepository()
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

        public StandardAcCategory GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.StandardAcCategories.FirstOrDefault(s => s.CategoryID == id);
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

        public bool Add(StandardAcCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.CategoryID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.StandardAcCategories.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.CategoryID.ToString() + ", " + "Name: " + entity.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(StandardAcCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalStandardAcCategory = entities.StandardAcCategories.FirstOrDefault(s => s.CategoryID == entity.CategoryID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.StandardAcCategories.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.CategoryID.ToString() + ", " + "Name: " + entity.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(StandardAcCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var StandardAcCategory = entities.StandardAcCategories.FirstOrDefault(s => s.CategoryID == entity.CategoryID);
                entities.StandardAcCategories.DeleteObject(StandardAcCategory);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.CategoryID.ToString() + ", " + "Name: " + entity.Category);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<StandardAcCategory> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.StandardAcCategories.OrderBy(b => b.Sequence).ToList();
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