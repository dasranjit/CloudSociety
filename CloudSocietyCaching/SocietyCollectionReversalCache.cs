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
    public class SocietyCollectionReversalCache : ISocietyCollectionReversalRepository
    {
        const string CacheName = "SocietyCollectionReversal";
        private ISocietyCollectionReversalRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyCollectionReversalCache()
        {
            try
            {
                _repository = new SocietyCollectionReversalRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyCollectionReversal GetById(Guid id)
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

        public bool Add(SocietyCollectionReversal entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyCollectionReversalID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyCollectionReversal entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyCollectionReversalID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyCollectionReversalID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyCollectionReversal entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyCollectionReversalID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyCollectionReversal> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyCollectionReversal>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate)
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
        
        public IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid)
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

        public IEnumerable<SocietyCollectionReversal> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, string billAbbreviation, DateTime startDate, DateTime endDate)
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
    }
}