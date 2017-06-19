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
    public class PayModeRepository : IGenericCodeTableRepository<PayMode>   //    IReadOnlyCodeTableRepository<PayMode>
    {
        const string _entityname = "PayMode";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public PayMode GetByCode(string code)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.PayModes.FirstOrDefault(s => s.PayModeCode == code);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("PayModeCode: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<PayMode> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.PayModes.Where(p => p.Active==true).OrderBy(p => p.Mode).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public bool Add(PayMode entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entities.PayModes.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("Mode: " + entity.Mode);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(PayMode entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalPayMode = entities.PayModes.FirstOrDefault(s => s.PayModeCode == entity.PayModeCode);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.PayModes.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("Mode: " + entity.Mode);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(PayMode entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var PayMode = entities.PayModes.FirstOrDefault(s => s.PayModeCode == entity.PayModeCode);
                entities.PayModes.DeleteObject(PayMode);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("Mode: " + entity.Mode);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }
    }
}
