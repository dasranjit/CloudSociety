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
    public class OccupationCache : IGenericRepository<Occupation>
    {
        const string CacheName = "Occupation";
        private IGenericRepository<Occupation> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public OccupationCache() :this(new OccupationRepository()){}

        //public OccupationCache(IGenericRepository<Occupation> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public OccupationCache()
        {
            try
            {
                _repository = new OccupationRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Occupation GetById(Guid id)
        {
            try
            {
                var occupation = (Occupation)GenericCache.GetCacheItem(CacheName, id);
                if (occupation == null)
                {
                    occupation = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, occupation);
                }
                return occupation;
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

        public bool Add(Occupation entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.OccupationID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Occupation entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.OccupationID);
                GenericCache.AddCacheItem(CacheName, entity.OccupationID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Occupation entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.OccupationID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Occupation> List()
        {
            try
            {
                var list = (IEnumerable<Occupation>)GenericCache.GetCacheItem(CacheName);
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