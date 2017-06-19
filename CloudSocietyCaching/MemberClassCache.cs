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
    public class MemberClassCache : IGenericRepository<MemberClass>
    {
        const string CacheName = "MemberClass";
        private IGenericRepository<MemberClass> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public MemberClassCache() :this(new MemberClassRepository()){}

        //public MemberClassCache(IGenericRepository<MemberClass> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public MemberClassCache()
        {
            try
            {
                _repository = new MemberClassRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public MemberClass GetById(Guid id)
        {
            try
            {
                var mc = (MemberClass)GenericCache.GetCacheItem(CacheName, id);
                if (mc == null)
                {
                    mc = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, mc);
                }
                return mc;
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

        public bool Add(MemberClass entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.MemberClassID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(MemberClass entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.MemberClassID);
                GenericCache.AddCacheItem(CacheName, entity.MemberClassID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(MemberClass entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.MemberClassID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<MemberClass> List()
        {
            try
            {
                var list = (IEnumerable<MemberClass>)GenericCache.GetCacheItem(CacheName);
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