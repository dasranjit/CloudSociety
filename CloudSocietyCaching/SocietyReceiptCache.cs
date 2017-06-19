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
    public class SocietyReceiptCache : ISocietyReceiptRepository
    {
        const string CacheName = "SocietyReceipt";
        private ISocietyReceiptRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyReceiptCache()
        {
            try
            {
                _repository = new SocietyReceiptRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyReceipt GetById(Guid id)
        {
            try
            {
                //var societyReceipt = (SocietyReceipt)GenericCache.GetCacheItem(CacheName, id);
                //if (societyReceipt == null)
                //{
                //    societyReceipt = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, societyReceipt);
                //}
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

        public bool Add(SocietyReceipt entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyReceiptID, entity);
                return true;
            }
            else
                return false;
        }

        public bool AddTemporary(SocietyReceiptOnhold entity)
        {
            if (_repository.AddTemporary(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyReceiptOnholdID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyReceipt entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyReceiptID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyReceiptID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyReceipt entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyReceiptID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyReceipt> ListByParentId(Guid parentid)
        {
            try
            {
                // Caching removed because reversal has to be linked
                //var list = (IEnumerable<SocietyReceipt>)GenericCache.GetCacheItem(CacheName, parentid);
                //if (list == null)
                //{
                //    list = _repository.ListByParentId(parentid);
                //    GenericCache.AddCacheItem(CacheName, parentid, list);
                //}
                //return list;
                return _repository.ListByParentId(parentid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _repository.ListBySocietyIDStartEndDate(societyId, startDate, endDate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid)
        {
            try
            {
                return _repository.ListBySocietyIDStartEndDateSocietyBuildingID(societyId, startDate, endDate, societybuildingid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy") + " for Building " + societybuildingid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, string billAbbreviation, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _repository.ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBulidingUnitId, billAbbreviation, startDate, endDate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by " + societyBulidingUnitId.ToString() + " for " + billAbbreviation + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(string billabbreviation, Guid societybulidingunitId, Guid societymemberId)
        {
            try
            {
                return _repository.ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(billabbreviation, societybulidingunitId, societymemberId);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Unit " + societybulidingunitId.ToString() + " & for Member " + societymemberId.ToString() + " for " + billabbreviation);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceipt> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid)
        {
            try
            {
                return _repository.ListBySocietyMemberIDSocietySubscriptionID(societymemberid, societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Member " + societymemberid.ToString() + " & year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyReceiptOnhold> GetOnholdReceipts(Guid societyId, Guid societySubscriptionId, Guid? societyMemberId)
        {
            try
            {
                return _repository.GetOnholdReceipts(societyId, societySubscriptionId, societyMemberId);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for societyId " + societyId.ToString() + " & year " + societySubscriptionId.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool GenerateReceiptForOnHoldReciept(Guid SocietyReceiptOnholdID)
        {
            if (_repository.GenerateReceiptForOnHoldReciept(SocietyReceiptOnholdID))
            {
                return true;
            }
            else
                return false;
        }
    }
}