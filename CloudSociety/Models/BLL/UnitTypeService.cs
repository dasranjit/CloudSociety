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
    public class UnitTypeService : IGenericRepository<UnitType>
    {
        private IGenericRepository<UnitType> _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "UnitType";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public UnitTypeService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new UnitTypeCache();
        }

        public bool Add(UnitType entity)
        {
            if (!_modelState.IsValid)
                return false;
            // TO DO: wrap transaction around next statements
            if (!_cache.Add(entity))
            {
                //_modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }

            return true;


        }

        public bool Delete(UnitType entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(UnitType entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public UnitType GetById(Guid id)
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

        public IEnumerable<UnitType> List()
        {
            try
            {
                return (_cache.List());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}