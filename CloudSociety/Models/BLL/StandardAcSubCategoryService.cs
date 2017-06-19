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
    public class StandardAcSubCategoryService : IGenericRepository<StandardAcSubCategory>
    {
        private IGenericRepository<StandardAcSubCategory> _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "StandardAcSubCategory";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public StandardAcSubCategoryService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new StandardAcSubCategoryCache();
        }

        public bool Add(StandardAcSubCategory entity)
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

        public bool Delete(StandardAcSubCategory entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(StandardAcSubCategory entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public StandardAcSubCategory GetById(Guid id)
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

        public IEnumerable<StandardAcSubCategory> List()
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
