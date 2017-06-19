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
    public class TDSCategoryRateRepository : IGenericChildRepository<TDSCategoryRate>
    {
        const string _entityname = "TDSCategoryRate";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public TDSCategoryRate GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TDSCategoryRates.FirstOrDefault(s => s.TDSCategoryRateID == id);
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

        public bool Add(TDSCategoryRate entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.TDSCategoryRateID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.TDSCategoryRates.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.TDSCategoryRateID.ToString() + ", " + "Name: " + entity.TDSCategory.Category + ", " + "Date: " + entity.FromDate);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(TDSCategoryRate entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalTDSCategoryRate = entities.TDSCategoryRates.FirstOrDefault(s => s.TDSCategoryRateID == entity.TDSCategoryRateID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.TDSCategoryRates.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.TDSCategoryRateID.ToString() + ", " + "Name: " + entity.TDSCategory.Category + ", " + "Date: " + entity.FromDate);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(TDSCategoryRate entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var TDSCategoryRate = entities.TDSCategoryRates.FirstOrDefault(s => s.TDSCategoryRateID == entity.TDSCategoryRateID);
                entities.TDSCategoryRates.DeleteObject(TDSCategoryRate);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.TDSCategoryRateID.ToString() + ", " + "Name: " + entity.TDSCategory.Category + ", " + "Date: " + entity.FromDate);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<TDSCategoryRate> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TDSCategoryRates.Where(r => r.TDSCategoryID == parentid).OrderBy(t => t.FromDate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by "+parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}