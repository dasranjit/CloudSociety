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
    public class CommunicationTypeCache : ICommunicationTypeRepository
    {
        const string CacheName = "CommunicationType";
        private ICommunicationTypeRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public CommunicationTypeCache()
        {
            try
            {
                _repository = new CommunicationTypeRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public IEnumerable<CommunicationType> ListForMember()
        {
            try
            {
                var list = _repository.ListForMember();
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Member");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public CommunicationType GetById(Guid id)
        {
            try
            {
                var ct = (CommunicationType)GenericCache.GetCacheItem(CacheName, id);
                if (ct == null)
                {
                    ct = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, ct);
                }
                return ct;
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

        public bool Add(CommunicationType entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.CommunicationTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(CommunicationType entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.CommunicationTypeID);
                GenericCache.AddCacheItem(CacheName, entity.CommunicationTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(CommunicationType entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.CommunicationTypeID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<CommunicationType> List()
        {
            try
            {
                var list = (IEnumerable<CommunicationType>)GenericCache.GetCacheItem(CacheName);
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


