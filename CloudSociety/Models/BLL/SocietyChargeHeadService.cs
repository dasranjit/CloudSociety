using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyChargeHeadService : IGenericComboKeyRepository<SocietyChargeHead>
    {
        private IGenericComboKeyRepository<SocietyChargeHead> _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyChargeHead";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyChargeHeadService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyChargeHeadCache();
        }

        public bool Add(SocietyChargeHead entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyChargeHead entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyChargeHead entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyChargeHead> ListByParentId(Guid parentid)   // SocietyID
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public SocietyChargeHead GetByIds(Guid parentid, Guid id)   // SocietyID, ChargeHeadID
        {
            try
            {
                return (_cache.GetByIds(parentid, id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyChargeHead> ListById(Guid id) // ChargeHeadID
        {
            try
            {
                return (_cache.ListById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Head " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}