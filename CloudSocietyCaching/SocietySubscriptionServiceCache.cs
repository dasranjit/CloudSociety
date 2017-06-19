using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietySubscriptionServiceCache : ISocietySubscriptionServiceRepository
    {
        const string CacheName = "SocietySubscriptionService";
        private ISocietySubscriptionServiceRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public SocietySubscriptionServiceCache()
        {
            try
            {
                _repository = new SocietySubscriptionServiceRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietySubscriptionService GetById(Guid id)
        {
            try
            {
                // do not cache, may be bulk-deleted by DeletePendingBySocietySubscriptionID
                //var item = (SocietySubscriptionService)GenericCache.GetCacheItem(CacheName, id);
                //if (item == null)
                //{
                //    item = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, item);
                //}
                //return item;
                return _repository.GetById(id);
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

        public bool Add(SocietySubscriptionService entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                //GenericCache.AddCacheItem(CacheName, entity.SocietySubscriptionServiceID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietySubscriptionService entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                //GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionServiceID);
                //GenericCache.AddCacheItem(CacheName, entity.SocietySubscriptionServiceID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietySubscriptionService entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                //GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionServiceID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietySubscriptionService> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietySubscriptionService>)GenericCache.GetCacheItem(CacheName, parentid);
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

        public void DeletePendingBySocietySubscriptionID(Guid id)
        {
            try
            {
                _repository.DeletePendingBySocietySubscriptionID(id);
                GenericCache.RemoveCacheItem(CacheName, id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Delete Pending for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}