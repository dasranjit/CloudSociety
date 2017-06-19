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
    public class SocietySpecialBillCache : IGenericChildRepository<SocietySpecialBill>
    {
        const string CacheName = "SocietySpecialBill";
        private IGenericChildRepository<SocietySpecialBill> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietySpecialBillCache()
        {
            try
            {
                _repository = new SocietySpecialBillRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietySpecialBill GetById(Guid id)
        {
            try
            {
                //var societySpecialBill = (SocietySpecialBill)GenericCache.GetCacheItem(CacheName, id);
                //if (societySpecialBill == null)
                //{
                //    societySpecialBill = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, societySpecialBill);
                //}
                return _repository.GetById(id);
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

        public bool Add(SocietySpecialBill entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySpecialBillID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietySpecialBill entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySpecialBillID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietySpecialBill entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietySpecialBill> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietySpecialBill>)GenericCache.GetCacheItem(CacheName, parentid);
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