using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class AcSubCategoryService : IGenericComboKeyRepository<AcSubCategory>
    {
        private IGenericComboKeyRepository<AcSubCategory> _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "AccountCategories";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public AcSubCategoryService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new AcSubCategoryCache();
        }

        public bool Add(AcSubCategory entity)
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

        public bool Delete(AcSubCategory entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(AcSubCategory entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public AcSubCategory GetByIds(Guid parentid, Guid id)   // SocietyID, AcSubCategoryID
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

        public IEnumerable<AcSubCategory> ListByParentId(Guid parentid)   // AcCategoryID
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Category " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcSubCategory> ListById(Guid id)   // SocietyID
        {
            try
            {
                return (_cache.ListById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}
