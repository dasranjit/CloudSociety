using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyBuildingUnitChargeHeadCache : ISocietyBuildingUnitChargeHeadRepository
    {
        const string CacheName = "SocietyBuildingUnitChargeHeadCache";
        private ISocietyBuildingUnitChargeHeadRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyBuildingUnitChargeHeadCache()
        {
            try
            {
                _repository = new SocietyBuildingUnitChargeHeadRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyBuildingUnitChargeHead GetById(Guid id)
        {
            try
            {
                var societyBuildingUnitChargeHead = (SocietyBuildingUnitChargeHead)GenericCache.GetCacheItem(CacheName, id);
                if (societyBuildingUnitChargeHead == null)
                {
                    societyBuildingUnitChargeHead = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, societyBuildingUnitChargeHead);
                }
                return societyBuildingUnitChargeHead;
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

        public bool Add(SocietyBuildingUnitChargeHead entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, (Guid)entity.SocietyBuildingUnitID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingUnitChargeHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyBuildingUnitChargeHead entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingUnitChargeHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyBuildingUnitChargeHead entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitChargeHeadID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyBuildingUnitChargeHeadView> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyBuildingUnitChargeHeadView>)GenericCache.GetCacheItem(CacheName, parentid);
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
