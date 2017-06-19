using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using CloudSocietyLib.Interfaces;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietySpecialBillUnitService : ISocietySpecialBillUnitRepository
    {
        private ISocietySpecialBillUnitRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietySpecialBillUnit";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietySpecialBillUnitService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietySpecialBillUnitCache();
        }

        public bool Add(SocietySpecialBillUnit entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietySpecialBillUnit> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Special Bill " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public SocietySpecialBillUnit GetByIds(Guid parentid, Guid id)
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

        public bool Delete(SocietySpecialBillUnit entity)
        {
            try
            {
                _cache.Delete(entity);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Delete for Special Bill " + entity.SocietySpecialBillID.ToString() + " , Unit "+entity.SocietyBuildingUnitID, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public void DeleteByParentId(Guid parentid)
        {
            try
            {
                _cache.DeleteByParentId(parentid);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Delete for Special Bill " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                throw;
            }
        }
    }
}