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
    public class SocietyBuildingCache : IGenericChildRepository<SocietyBuilding>
    {
        const string CacheName = "SocietyBuilding";
        private IGenericChildRepository<SocietyBuilding> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";
        
        public SocietyBuildingCache()
        {
            try
            {
                _repository = new SocietyBuildingRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyBuilding GetById(Guid id)
        {
            try
            {
                var societyBuilding = (SocietyBuilding)GenericCache.GetCacheItem(CacheName, id);
                if (societyBuilding == null)
                {
                    societyBuilding = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, societyBuilding);
                }
                return societyBuilding;
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

        public bool Add(SocietyBuilding entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyBuilding entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyBuilding entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyBuilding> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyBuilding>)GenericCache.GetCacheItem(CacheName, parentid);
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