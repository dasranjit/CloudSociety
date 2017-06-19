using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using CloudSocietyLib.Interfaces;
using System.Data.Objects;

namespace CloudSociety.Repositories
{
    public class TariffRateRepository : ITariffRateRepository
    {
        //private CloudSocietyModels.CloudSocietyEntities entities = new CloudSocietyModels.CloudSocietyEntities();
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "TariffRate";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        //public TariffRateRepository()
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

        public TariffRate GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TariffRates.FirstOrDefault(s => s.TariffRateID == id);
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

        public bool Add(TariffRate entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.TariffRateID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.TariffRates.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.TariffRateID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(TariffRate entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalTariffRate = entities.TariffRates.FirstOrDefault(s => s.TariffRateID == entity.TariffRateID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.TariffRates.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.TariffRateID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(TariffRate entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var TariffRate = entities.TariffRates.FirstOrDefault(s => s.TariffRateID == entity.TariffRateID);
                entities.TariffRates.DeleteObject(TariffRate);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.TariffRateID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<TariffRate> CurrentList()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TariffRates.Where(r => r.Tariff.uEndDate==null).OrderBy(t => t.ServiceType.Sequence).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<TariffRate> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.TariffRates.Where(r => r.TariffID == parentid).OrderBy(t => t.ServiceType.Sequence).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool CopyTariffRatesFromPreviousTariff(Guid TariffID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = 
                    { new ObjectParameter("TariffID", TariffID), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey) };
                entities.ExecuteFunction("CopyTariffRatesFromPreviousTariff", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Copy from Previous Tariff");
                sb.AppendLine("Tariff ID: " + TariffID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool InsertTariffRatesFromServiceTypes(Guid TariffID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = 
                    { new ObjectParameter("TariffID", TariffID), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey) };
                entities.ExecuteFunction("InsertTariffRatesFromServiceTypes", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Copy from Service Types");
                sb.AppendLine("Tariff ID: " + TariffID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }


        public IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusForSubscription(Guid SocietySubscriptionID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListTariffRatesWithActiveStatusForSubscription(SocietySubscriptionID);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with ActiveStatus for " + SocietySubscriptionID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusMonthlyForSubscription(Guid SocietySubscriptionID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListTariffRatesWithActiveStatusMonthlyForSubscription(SocietySubscriptionID);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with ActiveStatus - Monthly for " + SocietySubscriptionID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}