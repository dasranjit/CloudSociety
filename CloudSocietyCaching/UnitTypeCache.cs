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
    public class UnitTypeCache : IGenericRepository<UnitType>
    {
        const string CacheName = "UnitType";
        private IGenericRepository<UnitType> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public UnitTypeCache() :this(new UnitTypeRepository()){}

        //public UnitTypeCache(IGenericRepository<UnitType> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public UnitTypeCache()
        {
            try
            {
                _repository = new UnitTypeRepository();
                //_cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public UnitType GetById(Guid id)
        {
            try
            {
                var ut = (UnitType)GenericCache.GetCacheItem(CacheName, id);
                if (ut == null)
                {
                    ut = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, ut);
                }
                return ut;
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

        public bool Add(UnitType entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.UnitTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(UnitType entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.UnitTypeID);
                GenericCache.AddCacheItem(CacheName, entity.UnitTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(UnitType entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.UnitTypeID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<UnitType> List()
        {
            try
            {
                var list = (IEnumerable<UnitType>)GenericCache.GetCacheItem(CacheName);
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