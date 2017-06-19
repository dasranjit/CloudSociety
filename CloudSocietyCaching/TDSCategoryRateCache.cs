using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class TDSCategoryRateCache : IGenericChildRepository<TDSCategoryRate>
    {
        const string CacheName = "TDSCategoryRate";
        private IGenericChildRepository<TDSCategoryRate> _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public TDSCategoryRateCache()
        {
            try
            {
                _repository = new TDSCategoryRateRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public TDSCategoryRate GetById(Guid id)
        {
            try
            {
                var ctgrate = (TDSCategoryRate)GenericCache.GetCacheItem(CacheName, id);
                if (ctgrate == null)
                {
                    ctgrate = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, ctgrate);
                }
                return ctgrate;
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

        public bool Add(TDSCategoryRate entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName,entity.TDSCategoryID);
                GenericCache.AddCacheItem(CacheName, entity.TDSCategoryRateID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(TDSCategoryRate entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.TDSCategoryID);
                GenericCache.RemoveCacheItem(CacheName, entity.TDSCategoryRateID);
                GenericCache.AddCacheItem(CacheName, entity.TDSCategoryRateID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(TDSCategoryRate entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.TDSCategoryID);
                GenericCache.RemoveCacheItem(CacheName, entity.TDSCategoryRateID);
                return true;
            }
            else
                return false;
        }
            
        public IEnumerable<TDSCategoryRate> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<TDSCategoryRate>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List for " + parentid);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}