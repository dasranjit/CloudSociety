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
    public class BankCache : IGenericRepository<Bank>
    {
        const string CacheName = "Bank";
        private IGenericRepository<Bank> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        //public BankCache() :this(new BankRepository()){}

        //public BankCache(IGenericRepository<Bank> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public BankCache()
        {
            try
            {
                _repository = new BankRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Bank GetById(Guid id)
        {
            try
            {
                var bank = (Bank)GenericCache.GetCacheItem(CacheName, id);
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

        public bool Add(Bank entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.BankID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Bank entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.BankID);
                GenericCache.AddCacheItem(CacheName, entity.BankID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Bank entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.BankID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Bank> List()
        {
            try
            {
                var list = (IEnumerable<Bank>)GenericCache.GetCacheItem(CacheName);
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