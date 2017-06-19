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
    public class PasswordQuestionCache : IGenericRepository<PasswordQuestion>
    {
        const string CacheName = "PasswordQuestion";
        private IGenericRepository<PasswordQuestion> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public PasswordQuestionCache() :this(new PasswordQuestionRepository()){}

        //public PasswordQuestionCache(IGenericRepository<PasswordQuestion> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public PasswordQuestionCache()
        {
            try
            {
                _repository = new PasswordQuestionRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public PasswordQuestion GetById(Guid id)
        {
            try
            {
                var bank = (PasswordQuestion)GenericCache.GetCacheItem(CacheName, id);
                if (bank == null)
                {
                    bank = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, bank);
                }
                return bank;
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

        public bool Add(PasswordQuestion entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.PasswordQuestionID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(PasswordQuestion entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.PasswordQuestionID);
                GenericCache.AddCacheItem(CacheName, entity.PasswordQuestionID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(PasswordQuestion entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.PasswordQuestionID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<PasswordQuestion> List()
        {
            try
            {
                var list = (IEnumerable<PasswordQuestion>)GenericCache.GetCacheItem(CacheName);
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
