using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyBuildingUnitCache : ISocietyBuildingUnitRepository
    {
        const string CacheName = "SocietyBuildingUnitCache";
        private ISocietyBuildingUnitRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyBuildingUnitCache()
        {
            try
            {
                _repository = new SocietyBuildingUnitRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyBuildingUnit GetById(Guid id)
        {
            try
            {
                //var SocietyBuildingUnit = (SocietyBuildingUnit)GenericCache.GetCacheItem(CacheName, id);
                //if (SocietyBuildingUnit == null)
                //{
                //    SocietyBuildingUnit = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, SocietyBuildingUnit);
                //}
                //return SocietyBuildingUnit;
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

        public bool Add(SocietyBuildingUnit entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingID);
//                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuilding.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingUnitID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyBuildingUnit entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
//                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuilding.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingUnitID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyBuildingUnit entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
//                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuilding.SocietyID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyBuildingUnit> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyBuildingUnit>)GenericCache.GetCacheItem(CacheName, parentid);
                if (list == null)
                {
                    list = _repository.ListByParentId(parentid);
                    GenericCache.AddCacheItem(CacheName, parentid, list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Building " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyBuildingUnit> ListByParentParentId(Guid parentparentid)
        {
            // not caching
            try
            {
                //var list = (IEnumerable<SocietyBuildingUnit>)GenericCache.GetCacheItem(CacheName, parentparentid);
                //if (list == null)
                //{
                //    list = _repository.ListByParentParentId(parentparentid);
                //    GenericCache.AddCacheItem(CacheName, parentparentid, list);
                //}
                //return list;
                return _repository.ListByParentParentId(parentparentid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Society " + parentparentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<BuildingUnitWithID> ListBuildingUnitBySocietyID(Guid societyid)
        {
            // Not cached as dependent on Building
            try
            {
                return _repository.ListBuildingUnitBySocietyID(societyid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List BuildingUnit by Society " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public int? GetCountBySocietySubscriptionID(Guid societysubscriptionid)
        {
            // Not cached
            try
            {
                return _repository.GetCountBySocietySubscriptionID(societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Count BuildingUnit by Society Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListBalanceBySocietySubscriptionID(societysubscriptionid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid, decimal amount)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListBalanceBySocietySubscriptionID(societysubscriptionid, amount);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString()+" for Amount >= "+amount.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListBalanceBySocietySubscriptionIDSocietyBuildingID(societysubscriptionid, societybuildingid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionID(Guid societysubscriptionid)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListLedgerBySocietySubscriptionID(societysubscriptionid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Ledger by Society Subscription " + societysubscriptionid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListLedgerBySocietySubscriptionIDSocietyBuildingID(societysubscriptionid, societybuildingid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Ledger by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListBalanceBySocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " & Abbr " + billabbreviation);
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation, decimal amount)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListBalanceBySocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation, amount);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString(), billabbreviation + " for Amount >= " + amount.ToString() + " & Abbr " + billabbreviation);
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviationSocietyBuildingID(Guid societysubscriptionid, string billabbreviation, Guid societybuildingid)
        //{
        //    // Not cached
        //    try
        //    {
        //        return _repository.ListBalanceBySocietySubscriptionIDBillAbbreviationSocietyBuildingID(societysubscriptionid, billabbreviation, societybuildingid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString() + " & Abbr " + billabbreviation);
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, Guid? societybuildingid, string billabbreviation)
        {
            // Not cached
            try
            {
                return _repository.ListBalanceForSocietySubscription(societysubscriptionid, societybuildingid, billabbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString() + " for Abbr " + billabbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, decimal amount, Guid? societybuildingid, string billabbreviation)
        {
            // Not cached
            try
            {
                return _repository.ListBalanceForSocietySubscription(societysubscriptionid, amount, societybuildingid, billabbreviation);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Amount >= " + amount.ToString() + " for Building " + societybuildingid.ToString() + " for Abbr " + billabbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid, DateTime? startdate, DateTime? enddate)
        {
            // Not cached
            try
            {
                return _repository.ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(societysubscriptionid, societybuildingid,startdate, enddate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Ledger from " + string.Format("dd-MMM-yy", startdate) + " to " + string.Format("dd-MMM-yy", enddate) + " by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionID(Guid societysubscriptionid, DateTime? startdate, DateTime? enddate)
        {
            // Not cached
            try
            {
                return _repository.ListLedgerForPeriodBySocietySubscriptionID(societysubscriptionid, startdate, enddate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Ledger from " + string.Format("dd-MMM-yy", startdate) + " to " + string.Format("dd-MMM-yy", enddate) + " by Society Subscription " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
