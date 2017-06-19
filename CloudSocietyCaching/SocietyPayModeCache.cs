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
    public class SocietyPayModeCache : IGenericIDCodeComboKeyRepository<SocietyPayMode>
    {
        const string CacheName = "SocietyPayMode";
        private IGenericIDCodeComboKeyRepository<SocietyPayMode> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyPayModeCache()
        {
            try
            {
                _repository = new SocietyPayModeRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(SocietyPayMode entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.PayMode, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyPayMode entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.PayMode);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.PayMode, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyPayMode entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.PayMode);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyPayMode> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyPayMode>)GenericCache.GetCacheItem(CacheName, parentid);
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

        public SocietyPayMode GetByIdCode(Guid parentid, string code)
        {
            try
            {
                var SocietyPayMode = (SocietyPayMode)GenericCache.GetCacheItem(CacheName, parentid.ToString() + code);
                if (SocietyPayMode == null)
                {
                    SocietyPayMode = _repository.GetByIdCode(parentid, code);
                    GenericCache.AddCacheItem(CacheName, parentid.ToString() + code, SocietyPayMode);
                }
                return SocietyPayMode;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Society ID: " + parentid.ToString() + ", Mode: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}