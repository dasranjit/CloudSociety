using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyBuildingUnitSubscriptionBalanceService : ISocietyBuildingUnitSubscriptionBalanceRepository
    {
        private ISocietyBuildingUnitSubscriptionBalanceRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBuildingUnitSubscriptionBalance";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBuildingUnitSubscriptionBalanceService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBuildingUnitSubscriptionBalanceCache();
        }

        public bool Add(SocietyBuildingUnitSubscriptionBalance entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnitSubscriptionBalance entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnitSubscriptionBalance entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListByParentId(Guid parentid)    // SocietySubscriptionID
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Year " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListBySocietyBuildingUnitID(Guid societybuildingunitid)
        {
            try
            {
                return (_cache.ListBySocietyBuildingUnitID(societybuildingunitid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Unit " + societybuildingunitid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public SocietyBuildingUnitSubscriptionBalance GetById(Guid id)
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
        
        public IEnumerable<SocietyBuildingUnitBalanceWithBillReceiptExistCheck> ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(Guid societybuildingunitid)
        {
            try
            {
                return (_cache.ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(societybuildingunitid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Opening Balance with Reference Check for Unit " + societybuildingunitid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}