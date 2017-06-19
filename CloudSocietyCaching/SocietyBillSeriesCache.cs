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
    public class SocietyBillSeriesCache : ISocietyBillSeriesRepository
    {
        const string CacheName = "SocietyBillSeries";
        private ISocietyBillSeriesRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyBillSeriesCache()
        {
            try
            {
                _repository = new SocietyBillSeriesRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(SocietyBillSeries entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.BillAbbreviation, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyBillSeries entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.BillAbbreviation);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.BillAbbreviation, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyBillSeries entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.BillAbbreviation);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyBillSeries> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyBillSeries>)GenericCache.GetCacheItem(CacheName, parentid);
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

        public SocietyBillSeries GetByIdCode(Guid parentid, string code)
        {
            try
            {
                var SocietyBillSeries = (SocietyBillSeries)GenericCache.GetCacheItem(CacheName, parentid.ToString() + code);
                if (SocietyBillSeries == null)
                {
                    SocietyBillSeries = _repository.GetByIdCode(parentid, code);
                    GenericCache.AddCacheItem(CacheName, parentid.ToString() + code, SocietyBillSeries);
                }
                return SocietyBillSeries;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Society ID: " + parentid.ToString() + ", Abbr: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<SocietyBillSeriesWithLastBillDate> ListWithLastBillDateBySocietyID(Guid societyid)
        //{
        //    try
        //    {
        //        // Not cached
        //        return _repository.ListWithLastBillDateBySocietyID(societyid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List with Last Bill Date by " + societyid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<SocietyBillSeriesWithLastDates> ListWithLastDatesBySocietyID(Guid societyid)
        {
            try
            {
                // Not cached
                return _repository.ListWithLastDatesBySocietyID(societyid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List with Last Dates by " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}