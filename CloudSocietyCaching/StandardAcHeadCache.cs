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
    public class StandardAcHeadCache : IGenericRepository<StandardAcHead>
    {
        const string CacheName = "StandardAcHead";
        private IGenericRepository<StandardAcHead> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public StandardAcHeadCache() :this(new StandardAcHeadRepository()){}

        //public StandardAcHeadCache(IGenericRepository<StandardAcHead> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public StandardAcHeadCache()
        {
            try
            {
                _repository = new StandardAcHeadRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public StandardAcHead GetById(Guid id)
        {
            try
            {
                var ac = (StandardAcHead)GenericCache.GetCacheItem(CacheName, id);
                if (ac == null)
                {
                    ac = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, ac);
                }
                return ac;
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

        public bool Add(StandardAcHead entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.AcHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(StandardAcHead entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.AcHeadID);
                GenericCache.AddCacheItem(CacheName, entity.AcHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(StandardAcHead entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.AcHeadID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<StandardAcHead> List()
        {
            try
            {
                var list = (IEnumerable<StandardAcHead>)GenericCache.GetCacheItem(CacheName);
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