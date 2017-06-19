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
    public class TDSCategoryCache : IGenericRepository<TDSCategory>
    {
        const string CacheName = "TDSCategory";
        private IGenericRepository<TDSCategory> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public TDSCategoryCache() :this(new TDSCategoryRepository()){}

        //public TDSCategoryCache(IGenericRepository<TDSCategory> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public TDSCategoryCache()
        {
            try
            {
                _repository = new TDSCategoryRepository();
                //_cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public TDSCategory GetById(Guid id)
        {
            try
            {
                var ctg = (TDSCategory)GenericCache.GetCacheItem(CacheName, id);
                if (ctg == null)
                {
                    ctg = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, ctg);
                }
                return ctg;
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

        public bool Add(TDSCategory entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.TDSCategoryID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(TDSCategory entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TDSCategoryID);
                GenericCache.AddCacheItem(CacheName, entity.TDSCategoryID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(TDSCategory entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TDSCategoryID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<TDSCategory> List()
        {
            try
            {
                var list = (IEnumerable<TDSCategory>)GenericCache.GetCacheItem(CacheName);
                if (list == null)
                {
                    list = _repository.List();
                    GenericCache.AddCacheItem(CacheName, list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}