using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
//using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class SocietySubscriptionServiceService : ISocietySubscriptionServiceRepository
    {
        private ISocietySubscriptionServiceRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietySubscriptionService";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietySubscriptionServiceService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietySubscriptionServiceCache(); // SocietySubscriptionServiceCache();
        }

        public bool Add(CloudSocietyEntities.SocietySubscriptionService entity)
        {
            if (!_modelState.IsValid)
                return false;
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(CloudSocietyEntities.SocietySubscriptionService entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(CloudSocietyEntities.SocietySubscriptionService entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public CloudSocietyEntities.SocietySubscriptionService GetById(Guid id)
        {
            try
            {
                return (_cache.GetById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<CloudSocietyEntities.SocietySubscriptionService> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public void DeletePendingBySocietySubscriptionID(Guid id)
        {
            try
            {
                _cache.DeletePendingBySocietySubscriptionID(id);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Delete Pending for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                throw;
            }
        }
    }
}