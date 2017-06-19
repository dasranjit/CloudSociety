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
    public class SocietyMemberTenantCache : IGenericChildRepository<SocietyMemberTenant>
    {
        const string CacheName = "SocietyMemberTenant";
        private IGenericChildRepository<SocietyMemberTenant> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyMemberTenantCache()
        {
            try
            {
                _repository = new SocietyMemberTenantRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyMemberTenant GetById(Guid id)
        {
            try
            {
                var SocietyMemberTenant = (SocietyMemberTenant)GenericCache.GetCacheItem(CacheName, id);
                if (SocietyMemberTenant == null)
                {
                    SocietyMemberTenant = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, SocietyMemberTenant);
                }
                return SocietyMemberTenant;
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

        public bool Add(SocietyMemberTenant entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberTenantID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyMemberTenant entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberTenantID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberTenantID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyMemberTenant entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberTenantID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyMemberTenant> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyMemberTenant>)GenericCache.GetCacheItem(CacheName, parentid);
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
