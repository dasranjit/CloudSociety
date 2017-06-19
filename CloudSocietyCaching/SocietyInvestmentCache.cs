using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CommonLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyInvestmentCache : IGenericChildRepository<SocietyInvestment>
    {
        const string CacheName = "SocietyInvestment";
        private IGenericChildRepository<SocietyInvestment> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyInvestmentCache()
        {
            try
            {
                _repository = new SocietyInvestmentRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyInvestment GetById(Guid id)
        {
            try
            {
                var societyInvestment = (SocietyInvestment)GenericCache.GetCacheItem(CacheName, id);
                if (societyInvestment == null)
                {
                    societyInvestment = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, societyInvestment);
                }
                return societyInvestment;
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

        public bool Add(SocietyInvestment entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyInvestmentID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyInvestment entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyInvestmentID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyInvestmentID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyInvestment entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyInvestmentID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyInvestment> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyInvestment>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}