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
    public class CouponCache : IGenericCodeTableRepository<Coupon>
    {
        const string CacheName = "Coupon";
        private IGenericCodeTableRepository<Coupon> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        public CouponCache()
        {
            try
            {
                _repository = new CouponRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Coupon GetByCode(String code)
        {
            try
            {
                var coupon = (Coupon)GenericCache.GetCacheItem(CacheName, code);
                if (coupon == null)
                {
                    coupon = _repository.GetByCode(code);
                    GenericCache.AddCacheItem(CacheName, code, coupon);
                }
                return coupon;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Code: " + code);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(Coupon entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.CouponNo, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Coupon entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.CouponNo);
                GenericCache.AddCacheItem(CacheName, entity.CouponNo, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Coupon entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.CouponNo);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Coupon> List()
        {
            try
            {
                var list = (IEnumerable<Coupon>)GenericCache.GetCacheItem(CacheName);
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