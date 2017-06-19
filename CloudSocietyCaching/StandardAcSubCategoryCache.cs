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
    public class StandardAcSubCategoryCache : IGenericRepository<StandardAcSubCategory>
    {
        const string CacheName = "StandardAcSubCategory";
        private IGenericRepository<StandardAcSubCategory> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public StandardAcSubCategoryCache() :this(new StandardAcSubCategoryRepository()){}

        //public StandardAcSubCategoryCache(IGenericRepository<StandardAcSubCategory> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public StandardAcSubCategoryCache()
        {
            try
            {
                _repository = new StandardAcSubCategoryRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public StandardAcSubCategory GetById(Guid id)
        {
            try
            {
                var subctg = (StandardAcSubCategory)GenericCache.GetCacheItem(CacheName, id);
                if (subctg == null)
                {
                    subctg = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, subctg);
                }
                return subctg;
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

        public bool Add(StandardAcSubCategory entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.SubCategoryID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(StandardAcSubCategory entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.SubCategoryID);
                GenericCache.AddCacheItem(CacheName, entity.SubCategoryID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(StandardAcSubCategory entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.SubCategoryID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<StandardAcSubCategory> List()
        {
            try
            {
                var list = (IEnumerable<StandardAcSubCategory>)GenericCache.GetCacheItem(CacheName);
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