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
    public class ServiceTypeCache : IGenericRepository<ServiceType>
    {
        const string CacheName = "ServiceType";

        private IGenericRepository<ServiceType> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public ServiceTypeCache() :this(new ServiceTypeRepository()){}

        //public ServiceTypeCache(IGenericRepository<ServiceType> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public ServiceTypeCache()
        {
            try
            {
                _repository = new ServiceTypeRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public ServiceType GetById(Guid id)
        {
            try
            {
                var st = (ServiceType)GenericCache.GetCacheItem(CacheName, id);
                if (st == null)
                {
                    st = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, st);
                }
                return st;
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

        public bool Add(ServiceType entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.ServiceTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(ServiceType entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.ServiceTypeID);
                GenericCache.AddCacheItem(CacheName, entity.ServiceTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(ServiceType entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.ServiceTypeID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<ServiceType> List()
        {
            try
            {
                var list = (IEnumerable<ServiceType>)GenericCache.GetCacheItem(CacheName);
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