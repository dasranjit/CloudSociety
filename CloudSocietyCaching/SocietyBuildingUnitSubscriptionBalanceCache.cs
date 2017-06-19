using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyBuildingUnitSubscriptionBalanceCache : ISocietyBuildingUnitSubscriptionBalanceRepository
    {
        const string CacheName = "SocietyBuildingUnitSubscriptionBalance";
        private ISocietyBuildingUnitSubscriptionBalanceRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";
// Cannot cache since will be updated from transactions
        public SocietyBuildingUnitSubscriptionBalanceCache()
        {
            try
            {
                _repository = new SocietyBuildingUnitSubscriptionBalanceRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(SocietyBuildingUnitSubscriptionBalance entity)
        {
            if (_repository.Add(entity))
            {
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyBuildingUnitSubscriptionBalance entity)
        {
            if (_repository.Edit(entity))
            {
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyBuildingUnitSubscriptionBalance entity)
        {
            if (_repository.Delete(entity))
            {
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListByParentId(Guid parentid)
        {
            try
            {
                return _repository.ListByParentId(parentid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Society " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListBySocietyBuildingUnitID(Guid societybuildingunitid)
        {
            try
            {
                return _repository.ListBySocietyBuildingUnitID(societybuildingunitid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Unit " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietyBuildingUnitSubscriptionBalance GetById(Guid id)
        {
            try
            {
                return _repository.GetById(id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public IEnumerable<SocietyBuildingUnitBalanceWithBillReceiptExistCheck> ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(Guid societybuildingunitid)
        {
            try
            {
                return _repository.ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(societybuildingunitid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Opening Balance with Reference Check by Unit " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}