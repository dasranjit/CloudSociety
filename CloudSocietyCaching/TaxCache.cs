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
    public class TaxCache : IGenericRepository<Tax>
    {
        const string CacheName = "Tax";
        private IGenericRepository<Tax> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public TaxCache() :this(new TaxRepository()){}

        //public TaxCache(IGenericRepository<Tax> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public TaxCache()
        {
            try
            {
                _repository = new TaxRepository();
                //_cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Tax GetById(Guid id)
        {
            try
            {
                var tax = (Tax)GenericCache.GetCacheItem(CacheName, id);
                if (tax == null)
                {
                    tax = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, tax);
                }
                return tax;
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

        public bool Add(Tax entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.TaxID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Tax entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TaxID);
                GenericCache.AddCacheItem(CacheName, entity.TaxID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Tax entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TaxID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Tax> List()
        {
            try
            {
                var list = (IEnumerable<Tax>)GenericCache.GetCacheItem(CacheName);
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