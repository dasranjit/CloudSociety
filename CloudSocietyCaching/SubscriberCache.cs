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
    public class SubscriberCache : IGenericRepository<Subscriber>
    {
        const string CacheName = "Subscriber";
        private IGenericRepository<Subscriber> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName+" Cache";

        //public SubscriberCache() :this(new SubscriberRepository()){}

        //public SubscriberCache(IGenericRepository<Subscriber> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public SubscriberCache()
        {
            try 
	        {
                _repository = new SubscriberRepository();
                //_cache = HttpContext.Current.Cache;
	        }
	        catch (Exception ex)
	        {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext," - ","Repository Creation"));
                throw;
	        }
        }

        public Subscriber GetById(Guid id)
        {
            try
            {
                var subscriber = (Subscriber)GenericCache.GetCacheItem(CacheName, id);
                if (subscriber == null)
                {
                    subscriber = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, subscriber);
                }
                return subscriber;
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

        public bool Add(Subscriber entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.SubscriberID, entity);
                return true;
            } else
                return false;
        }

        public bool Edit(Subscriber entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.SubscriberID);
                GenericCache.AddCacheItem(CacheName, entity.SubscriberID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Subscriber entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.SubscriberID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Subscriber> List()
        {
            try
            {
                var list = (IEnumerable<Subscriber>)GenericCache.GetCacheItem(CacheName);
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