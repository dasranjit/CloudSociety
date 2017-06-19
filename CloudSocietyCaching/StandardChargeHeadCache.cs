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
    public class StandardChargeHeadCache : IGenericRepository<StandardChargeHead>
    {
        const string CacheName = "StandardChargeHead";
        private IGenericRepository<StandardChargeHead> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public StandardChargeHeadCache() :this(new StandardChargeHeadRepository()){}

        //public StandardChargeHeadCache(IGenericRepository<StandardChargeHead> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public StandardChargeHeadCache()
        {
            try
            {
                _repository = new StandardChargeHeadRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public StandardChargeHead GetById(Guid id)
        {
            try
            {
                var hd = (StandardChargeHead)GenericCache.GetCacheItem(CacheName, id);
                if (hd == null)
                {
                    hd = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, hd);
                }
                return hd;
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

        public bool Add(StandardChargeHead entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.ChargeHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(StandardChargeHead entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.ChargeHeadID);
                GenericCache.AddCacheItem(CacheName, entity.ChargeHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(StandardChargeHead entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.ChargeHeadID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<StandardChargeHead> List()
        {
            try
            {
                var list = (IEnumerable<StandardChargeHead>)GenericCache.GetCacheItem(CacheName);
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