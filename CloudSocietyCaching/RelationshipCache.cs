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
    public class RelationshipCache : IGenericRepository<Relationship>
    {
        const string CacheName = "Relationship";
        private IGenericRepository<Relationship> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public RelationshipCache() :this(new RelationshipRepository()){}

        //public RelationshipCache(IGenericRepository<Relationship> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public RelationshipCache()
        {
            try
            {
                _repository = new RelationshipRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Relationship GetById(Guid id)
        {
            try
            {
                var relation = (Relationship)GenericCache.GetCacheItem(CacheName, id);
                if (relation == null)
                {
                    relation = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, relation);
                }
                return relation;
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

        public bool Add(Relationship entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.RelationshipID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Relationship entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.RelationshipID);
                GenericCache.AddCacheItem(CacheName, entity.RelationshipID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Relationship entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.RelationshipID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Relationship> List()
        {
            try
            {
                var list = (IEnumerable<Relationship>)GenericCache.GetCacheItem(CacheName);
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