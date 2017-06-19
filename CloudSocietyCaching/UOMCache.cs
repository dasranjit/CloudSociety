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
    public class UOMCache : IGenericRepository<UOM>
    {
        const string CacheName = "UOM";
        private IGenericRepository<UOM> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public UOMCache() :this(new UOMRepository()){}

        //public UOMCache(IGenericRepository<UOM> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public UOMCache()
        {
            try
            {
                _repository = new UOMRepository();
                //_cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public UOM GetById(Guid id)
        {
            try
            {
                var uom = (UOM)GenericCache.GetCacheItem(CacheName, id);
                if (uom == null)
                {
                    uom = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, uom);
                }
                return uom;
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

        public bool Add(UOM entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.UOMID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(UOM entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.UOMID);
                GenericCache.AddCacheItem(CacheName, entity.UOMID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(UOM entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.UOMID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<UOM> List()
        {
            try
            {
                var list = (IEnumerable<UOM>)GenericCache.GetCacheItem(CacheName);
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