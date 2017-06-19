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
    public class SocietySpecialBillUnitCache : ISocietySpecialBillUnitRepository
    {
        const string CacheName = "SocietySpecialBillUnit";
        private ISocietySpecialBillUnitRepository _repository;

        const string _exceptioncontext = CacheName + " Cache";

        public SocietySpecialBillUnitCache()
        {
            try
            {
                _repository = new SocietySpecialBillUnitRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(SocietySpecialBillUnit entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySpecialBillID.ToString()+entity.SocietyBuildingUnitID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietySpecialBillUnit entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID.ToString() + entity.SocietyBuildingUnitID.ToString());
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietySpecialBillUnit> ListByParentId(Guid parentid)
        {
            try
            {
                // not cached
                return _repository.ListByParentId(parentid); ;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Society " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietySpecialBillUnit GetByIds(Guid parentid, Guid id)
        {
            try
            {
                //var SocietySpecialBillUnit = (SocietySpecialBillUnit)GenericCache.GetCacheItem(CacheName, parentid.ToString() + id.ToString());
                //if (SocietySpecialBillUnit == null)
                //{
                //    SocietySpecialBillUnit = _repository.GetByIds(parentid, id);
                //    GenericCache.AddCacheItem(CacheName, parentid.ToString() + id.ToString(), SocietySpecialBillUnit);
                //}
                return _repository.GetByIds(parentid, id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Special Bill ID: " + parentid.ToString() + ", Unit ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void DeleteByParentId(Guid parentid)
        {
            try
            {
                GenericCache.RemoveCacheItem(CacheName, parentid);
                _repository.DeleteByParentId(parentid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Delete for Special Bill " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}