using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using CloudSocietyLib.Interfaces;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyUserService : ISocietyUserRepository
    {
        private ISocietyUserRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyUser";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyUserService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyUserCache();
        }

        public bool Add(SocietyUser entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        //public bool Edit(SocietyUser entity)
        //{
        //    if (!_cache.Edit(entity))
        //    {
        //        _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Delete(SocietyUser entity)
        //{
        //    throw new NotImplementedException();
        //}

        public void DeleteBySocietyID(Guid societyid)
        {
            try
            {
                _cache.DeleteBySocietyID(societyid);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Delete for Society " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                throw;
            }
        }

        public void DeleteByUserID(Guid userid)
        {
            try
            {
                _cache.DeleteByUserID(userid);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Delete for User " + userid.ToString(), GenericExceptionHandler.ExceptionMessage());
                throw;
            }
        }

        public IEnumerable<SocietyUser> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society "+parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}