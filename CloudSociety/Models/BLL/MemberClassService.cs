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
    public class MemberClassService : IGenericRepository<MemberClass>

   {
        private IGenericRepository<MemberClass> _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "MemberClasses";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public MemberClassService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new MemberClassCache();
        }
         public bool Add(MemberClass entity)
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

         public bool Delete(MemberClass entity)
         {
             if (!_cache.Delete(entity))
             {
                 _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                 return false;
             }
             return true;
         }

         public bool Edit(MemberClass entity)
         {
             if (!_cache.Edit(entity))
             {
                 _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                 return false;
             }
             return true;
         }

         public MemberClass GetById(Guid id)
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

         public IEnumerable<MemberClass> List()
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
