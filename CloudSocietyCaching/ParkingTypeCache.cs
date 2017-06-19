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
    public class ParkingTypeCache : IGenericRepository<ParkingType>
    {
        const string CacheName = "ParkingType";
        private IGenericRepository<ParkingType> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public ParkingTypeCache() :this(new ParkingTypeRepository()){}

        //public ParkingTypeCache(IGenericRepository<ParkingType> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public ParkingTypeCache()
        {
            try
            {
                _repository = new ParkingTypeRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public ParkingType GetById(Guid id)
        {
            try
            {
                var pt = (ParkingType)GenericCache.GetCacheItem(CacheName, id);
                if (pt == null)
                {
                    pt = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, pt);
                }
                return pt;
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

        public bool Add(ParkingType entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.ParkingTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(ParkingType entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.ParkingTypeID);
                GenericCache.AddCacheItem(CacheName, entity.ParkingTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(ParkingType entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.ParkingTypeID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<ParkingType> List()
        {
            try
            {
                var list = (IEnumerable<ParkingType>)GenericCache.GetCacheItem(CacheName);
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