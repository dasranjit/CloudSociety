using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class AcCategoryService : IGenericComboKeyRepository<AcCategory>
    {
        private IGenericComboKeyRepository<AcCategory> _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "AccountCategories";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public AcCategoryService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new AcCategoryCache();
        }

        public bool Add(AcCategory entity)
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

        public bool Delete(AcCategory entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(AcCategory entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public AcCategory GetByIds(Guid parentid, Guid id)  // SocietyID, CategoryID
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

        public IEnumerable<AcCategory> ListByParentId(Guid parentid)    // SocietyID
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

        public IEnumerable<AcCategory> ListById(Guid id)    // CategoryID
        {
            try
            {
                return (_cache.ListById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Category " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}
