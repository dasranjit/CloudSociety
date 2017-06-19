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
    public class SubscriptionInvoiceCache : ISubscriptionInvoiceRepository
    {
        const string CacheName = "SubscriptionInvoice";
        private ISubscriptionInvoiceRepository _repository;
        //        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public SubscriptionInvoiceCache() :this(new SubscriptionInvoiceRepository()){}

        //public SubscriptionInvoiceCache(IGenericRepository<SubscriptionInvoice> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public SubscriptionInvoiceCache()
        {
            try
            {
                _repository = new SubscriptionInvoiceRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SubscriptionInvoice GetById(Guid id)
        {
            try
            {
                //var hd = (SubscriptionInvoice)GenericCache.GetCacheItem(CacheName, id);
                //if (hd == null)
                //{
                //    hd = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, hd);
                //}
                //return hd;
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

        public bool Edit(SubscriptionInvoice entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName+"-Pending");
                GenericCache.RemoveCacheItem(CacheName, entity.SubscriptionInvoiceID);
                GenericCache.AddCacheItem(CacheName, entity.SubscriptionInvoiceID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SubscriptionInvoice entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName + "-Pending");
                GenericCache.RemoveCacheItem(CacheName, entity.SubscriptionInvoiceID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SubscriptionInvoice> List()
        {
            try
            {
                var list = (IEnumerable<SubscriptionInvoice>)GenericCache.GetCacheItem(CacheName);
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

        public Guid? Create(string Subscriptions, Guid SubscriberID)
        {
            Guid? SubscriptionInvoiceID = _repository.Create(Subscriptions, SubscriberID) ;
            if (SubscriptionInvoiceID != null)
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName + "-Pending");
            }
            return SubscriptionInvoiceID;
        }


        public IEnumerable<SubscriptionInvoice> ListPending()
        {
            try
            {
                var list = (IEnumerable<SubscriptionInvoice>)GenericCache.GetCacheItem(CacheName + "-Pending");
                if (list == null)
                {
                    list = _repository.ListPending();
                    GenericCache.AddCacheItem(CacheName + "-Pending", list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Pending List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public Guid? Create(string Subscriptions)
        {
            Guid? SubscriptionInvoiceID = _repository.Create(Subscriptions);
            if (SubscriptionInvoiceID != null)
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName + "-Pending");
            }
            return SubscriptionInvoiceID;
        }
    }
}