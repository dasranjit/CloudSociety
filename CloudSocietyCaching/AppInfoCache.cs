using System;
using System.Text;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class AppInfoCache : IGenericAppInfoRepository<AppInfo>
    {
        const string CacheName = "AppInfo";
        private IGenericAppInfoRepository<AppInfo> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        public AppInfoCache()
        {
            try
            {
                _repository = new AppInfoRepository();
//                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public AppInfo Get()
        {
            try
            {
                var info = (AppInfo)GenericCache.GetCacheItem(CacheName);
                if (info == null)
                {
                    info = _repository.Get();
                    GenericCache.AddCacheItem(CacheName, info);
                }
                return info;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Edit(AppInfo entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                return true;
            }
            else
                return false;
        }
    }
}