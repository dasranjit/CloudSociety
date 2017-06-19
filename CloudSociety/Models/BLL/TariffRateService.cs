using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class TariffRateService : ITariffRateRepository
    {
        private ITariffRateRepository _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "TariffRate";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public TariffRateService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new TariffRateCache();
        }

        public bool Add(TariffRate entity)
        {
            if (!_modelState.IsValid)
                return false;
            if (!_cache.Add(entity))
            {
                //_modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }

            return true;
        }

        public bool Delete(TariffRate entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(TariffRate entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public TariffRate GetById(Guid id)
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

        public IEnumerable<TariffRate> CurrentList()
        {
            try
            {
                return (_cache.CurrentList());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<TariffRate> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for "+parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool CopyTariffRatesFromPreviousTariff(Guid TariffID)
        {
            if (!_modelState.IsValid)
                return false;
            if (!_cache.CopyTariffRatesFromPreviousTariff(TariffID))
            {
                //_modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }

            return true;
        }

        public bool InsertTariffRatesFromServiceTypes(Guid TariffID)
        {
            if (!_modelState.IsValid)
                return false;
            if (!_cache.InsertTariffRatesFromServiceTypes(TariffID))
            {
                //_modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }

            return true;
        }


        public IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusForSubscription(Guid SocietySubscriptionID)
        {
            try
            {
                return (_cache.ListWithActiveStatusForSubscription(SocietySubscriptionID));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List with ActiveStatus for " + SocietySubscriptionID.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }


        public IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusMonthlyForSubscription(Guid SocietySubscriptionID)
        {
            try
            {
                return (_cache.ListWithActiveStatusMonthlyForSubscription(SocietySubscriptionID));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List with ActiveStatus - Monthly for " + SocietySubscriptionID.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}
