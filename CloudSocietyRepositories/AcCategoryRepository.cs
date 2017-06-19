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
    public class AcCategoryRepository : IGenericComboKeyRepository<AcCategory>
    {
        const string _entityname = "AcCategory";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public bool Add(AcCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                
                entity.CategoryID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcCategories.AddObject(entity);
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

        public bool Edit(AcCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalAcCategory = entities.AcCategories.FirstOrDefault(s => s.SocietyID==entity.SocietyID && s.CategoryID == entity.CategoryID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.AcCategories.ApplyCurrentValues(entity);
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

        public bool Delete(AcCategory entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var AcCategory = entities.AcCategories.FirstOrDefault(s => s.SocietyID == entity.SocietyID && s.CategoryID == entity.CategoryID);
                entities.AcCategories.DeleteObject(AcCategory);
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

        public IEnumerable<AcCategory> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcCategories.Where(c => c.SocietyID==parentid).OrderBy(c => c.Sequence).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List By Society "+parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public AcCategory GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcCategories.FirstOrDefault(s => s.SocietyID == parentid && s.CategoryID == id);
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

        public IEnumerable<AcCategory> ListById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.AcCategories.Where(c => c.CategoryID == id).OrderBy(c => c.Society.Name).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List By Category " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}