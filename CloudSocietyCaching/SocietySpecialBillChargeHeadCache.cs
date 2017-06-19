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
    public class SocietySpecialBillChargeHeadCache : ISocietySpecialBillChargeHeadRepository
    {
        const string CacheName = "SocietySpecialBillChargeHead";
        private ISocietySpecialBillChargeHeadRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietySpecialBillChargeHeadCache()
        {
            try
            {
                _repository = new SocietySpecialBillChargeHeadRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietySpecialBillChargeHead GetById(Guid id)
        {
            try
            {
                var societySpecialBillChargeHead = (SocietySpecialBillChargeHead)GenericCache.GetCacheItem(CacheName, id);
                if (societySpecialBillChargeHead == null)
                {
                    societySpecialBillChargeHead = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, societySpecialBillChargeHead);
                }
                return societySpecialBillChargeHead;
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

        public bool Add(SocietySpecialBillChargeHead entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySpecialBillChargeHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietySpecialBillChargeHead entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillChargeHeadID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySpecialBillChargeHeadID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietySpecialBillChargeHead entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySpecialBillChargeHeadID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietySpecialBillChargeHeadView> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietySpecialBillChargeHeadView>)GenericCache.GetCacheItem(CacheName, parentid);
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