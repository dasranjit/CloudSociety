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
    public class StandardAcCategoryCache : IGenericRepository<StandardAcCategory>
    {
        const string CacheName = "StandardAcCategory";
        private IGenericRepository<StandardAcCategory> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public StandardAcCategoryCache() :this(new StandardAcCategoryRepository()){}

        //public StandardAcCategoryCache(IGenericRepository<StandardAcCategory> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public StandardAcCategoryCache()
        {
            try
            {
                _repository = new StandardAcCategoryRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public StandardAcCategory GetById(Guid id)
        {
            try
            {
                var ctg = (StandardAcCategory)GenericCache.GetCacheItem(CacheName, id);
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

        public bool Add(StandardAcCategory entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.CategoryID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(StandardAcCategory entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.CategoryID);
                GenericCache.AddCacheItem(CacheName, entity.CategoryID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(StandardAcCategory entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.CategoryID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<StandardAcCategory> List()
        {
            try
            {
                var list = (IEnumerable<StandardAcCategory>)GenericCache.GetCacheItem(CacheName);
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