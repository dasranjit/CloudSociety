using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class UserDetailService : IUserDetailRepository
    {
        private IUserDetailRepository _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "UserDetail";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;

        public UserDetailService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new UserDetailCache();
        }

        public UserDetail GetById(Guid id)
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

        public bool Add(UserDetail entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(UserDetail entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(UserDetail entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<UserDetail> List()
        {
            throw new NotImplementedException();
        }

        public UserDetail GetBySocietyMemberID(Guid societymemberid)
        {
            try
            {
                return (_cache.GetBySocietyMemberID(societymemberid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get By Society Member ID " + societymemberid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public UserDetail GetBySubscriberID(Guid subscriberid)
        {
            try
            {
                return (_cache.GetBySubscriberID(subscriberid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get By Subscriber " + subscriberid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}