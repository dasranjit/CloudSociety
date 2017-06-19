using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class TariffRateCache : ITariffRateRepository
    {
        const string CacheName = "TariffRate";
        private ITariffRateRepository _repository;
        //        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public TariffRateCache() :this(new TariffRateRepository()){}

        //public TariffRateCache(IGenericRepository<TariffRate> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public TariffRateCache()
        {
            try
            {
                _repository = new TariffRateRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public TariffRate GetById(Guid id)
        {
            try
            {
                var tariffrate = (TariffRate)GenericCache.GetCacheItem(CacheName, id);
                if (tariffrate == null)
                {
                    tariffrate = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, tariffrate);
                }
                return tariffrate;
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

        public bool Add(TariffRate entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TariffID);
                GenericCache.AddCacheItem(CacheName, entity.TariffRateID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(TariffRate entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TariffRateID);
                GenericCache.RemoveCacheItem(CacheName, entity.TariffID);
                GenericCache.AddCacheItem(CacheName, entity.TariffRateID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(TariffRate entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TariffRateID);
                GenericCache.RemoveCacheItem(CacheName, entity.TariffID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<TariffRate> CurrentList()
        {
            try
            {
                var list = (IEnumerable<TariffRate>)GenericCache.GetCacheItem(CacheName);
                if (list == null)
                {
                    list = _repository.CurrentList();
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

        public IEnumerable<TariffRate> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<TariffRate>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List for "+parentid);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool CopyTariffRatesFromPreviousTariff(Guid TariffID)
        {
            if (_repository.CopyTariffRatesFromPreviousTariff(TariffID))
            {
                GenericCache.RemoveCacheItem(CacheName);
                return true;
            }
            else
                return false;
        }

        public bool InsertTariffRatesFromServiceTypes(Guid TariffID)
        {
            if (_repository.InsertTariffRatesFromServiceTypes(TariffID))
            {
                GenericCache.RemoveCacheItem(CacheName);
                return true;
            }
            else
                return false;
        }


        public IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusForSubscription(Guid SocietySubscriptionID)
        {
            try
            {
                // Not cached
                return _repository.ListWithActiveStatusForSubscription(SocietySubscriptionID);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List with ActiveStatus for " + SocietySubscriptionID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusMonthlyForSubscription(Guid SocietySubscriptionID)
        {
            try
            {
                // Not cached
                return _repository.ListWithActiveStatusMonthlyForSubscription(SocietySubscriptionID);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List with ActiveStatus - Monthly for " + SocietySubscriptionID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}