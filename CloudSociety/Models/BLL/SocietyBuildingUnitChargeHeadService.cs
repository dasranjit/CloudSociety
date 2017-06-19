using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyBuildingUnitChargeHeadService : ISocietyBuildingUnitChargeHeadRepository
    {
        private ISocietyBuildingUnitChargeHeadRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBuildingUnitChargeHead";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBuildingUnitChargeHeadService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBuildingUnitChargeHeadCache();
        }

        public SocietyBuildingUnitChargeHead GetById(Guid id)
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

        public bool Add(SocietyBuildingUnitChargeHead entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnitChargeHead entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnitChargeHead entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnitChargeHeadView> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}