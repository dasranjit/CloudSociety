using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using CloudSocietyLib.Interfaces;
using System.Web.Security;
using System.Data.Objects;

namespace CloudSociety.Repositories
{
    public class SocietySubscriptionServiceRepository : ISocietySubscriptionServiceRepository
    {
        const string _entityname = "SocietySubscriptionService";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietySubscriptionService GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySubscriptionServices.FirstOrDefault(s => s.SocietySubscriptionServiceID == id);
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

        public bool Add(SocietySubscriptionService entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietySubscriptionServiceID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietySubscriptionServices.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.SocietySubscriptionServiceID.ToString() + ", " + "Name: " + entity.ServiceType.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietySubscriptionService entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietySubscriptionService = entities.SocietySubscriptionServices.FirstOrDefault(s => s.SocietySubscriptionServiceID == entity.SocietySubscriptionServiceID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietySubscriptionServices.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietySubscriptionServiceID.ToString() + ", " + "Name: " + entity.ServiceType.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietySubscriptionService entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietySubscriptionService = entities.SocietySubscriptionServices.FirstOrDefault(s => s.SocietySubscriptionServiceID == entity.SocietySubscriptionServiceID);
                entities.SocietySubscriptionServices.DeleteObject(SocietySubscriptionService);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietySubscriptionServiceID.ToString() + ", " + "Name: " + entity.ServiceType.Type);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietySubscriptionService> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySubscriptionServices.Where(r => r.SocietySubscriptionID == parentid).OrderBy(t => t.ServiceType.Sequence).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void DeletePendingBySocietySubscriptionID(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                //var services = entities.SocietySubscriptionServices.Where(r => r.SocietySubscriptionID == id && r.ActiveStatus=="P");
                //entities.DeleteObject(services);
                //entities.SaveChanges();
                ObjectParameter[] qparams = 
                    { new ObjectParameter("SocietySubscriptionID", id) };
                entities.ExecuteFunction("DeleteSocietySubscriptionServicesPendingBySocietySubscriptionID", qparams);

            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete Pending");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}