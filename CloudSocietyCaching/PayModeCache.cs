using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class PayModeCache : IGenericCodeTableRepository<PayMode> //IReadOnlyCodeTableRepository<PayMode>
    {
        const string CacheName = "PayMode";
        private IGenericCodeTableRepository<PayMode> _repository;
        const string _exceptioncontext = CacheName + " Cache";

         public PayModeCache()
        {
            try
            {
                _repository = new PayModeRepository();
            //_cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public PayMode GetByCode(string code)
        {
            try
            {
                //var entities = (PayMode)GenericCache.GetCacheItem(CacheName, code);
                //if (entities == null)
                //{
                //    entities = _repository.GetByCode(code);
                //    GenericCache.AddCacheItem(CacheName, code, entities);
                //}
                //return entities;
                return _repository.GetByCode(code);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("ID: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        public IEnumerable<PayMode> List()
        {
            try
            {
                //var list = (IEnumerable<PayMode>)GenericCache.GetCacheItem(CacheName);
                //if (list == null)
                //{
                //    list = _repository.List();
                //    GenericCache.AddCacheItem(CacheName, list);
                //}
                //return list;
                return _repository.List();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(PayMode entity)
        {
            if (_repository.Add(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.AddCacheItem(CacheName, entity.PayModeCode, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(PayMode entity)
        {
            if (_repository.Edit(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.RemoveCacheItem(CacheName, entity.PayModeCode);
                //GenericCache.AddCacheItem(CacheName, entity.PayModeCode, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(PayMode entity)
        {
            if (_repository.Delete(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.RemoveCacheItem(CacheName, entity.PayModeCode);
                return true;
            }
            else
                return false;
        }
    }
}
