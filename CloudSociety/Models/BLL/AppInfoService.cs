using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Security;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class AppInfoService : IGenericAppInfoRepository<AppInfo>
    {
        private IGenericAppInfoRepository<AppInfo> _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "AppInfo";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public AppInfoService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new AppInfoCache();
        }

        public AppInfo Get()
        {
            try
            {
                return (_cache.Get());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool Edit(AppInfo entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
    }
}