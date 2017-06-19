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
    public class TariffCache : IGenericWithCountRepository<Tariff>
    {
        const string CacheName = "Tariff";
        private IGenericWithCountRepository<Tariff> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public TariffCache() :this(new TariffRepository()){}

        //public TariffCache(IGenericRepository<Tariff> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public TariffCache()
        {
            try
            {
                _repository = new TariffRepository();
                //_cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Tariff GetById(Guid id)
        {
            try
            {
                var tariff = (Tariff)GenericCache.GetCacheItem(CacheName, id);
                if (tariff == null)
                {
                    tariff = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, tariff);
                }
                return tariff;
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

        public bool Add(Tariff entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName + "-Count");
                GenericCache.AddCacheItem(CacheName, entity.TariffID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Tariff entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.TariffID);
                GenericCache.AddCacheItem(CacheName, entity.TariffID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Tariff entity)
        {
            // Temp: Remove before production
            GenericCache.RemoveCacheItem(CacheName);

            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName + "-Count");
                GenericCache.RemoveCacheItem(CacheName, entity.TariffID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Tariff> List()
        {
            try
            {
                var list = (IEnumerable<Tariff>)GenericCache.GetCacheItem(CacheName);
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


        public int Count()
        {
            try
            {
                int count;
                Object o = GenericCache.GetCacheItem(CacheName + "-Count");
                if (o == null)
                {
                    count = _repository.Count();
                    GenericCache.AddCacheItem(CacheName + "-Count", count);
                }
                else
                    count = (int)o;
                return count;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Count");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}