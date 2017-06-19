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
    public class SubscriberService : IGenericRepository<Subscriber>
    {
        private IGenericRepository<Subscriber> _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "Subscriber";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SubscriberService(ModelStateDictionary modelState)
        {
            _modelState=modelState;
            _cache = new SubscriberCache();
        }

        public Subscriber GetById(Guid id)
        {
            try
            {
                return(_cache.GetById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext+" - Get", GenericExceptionHandler.ExceptionMessage());
                return(null);
            }
        }

        public bool Add(Subscriber entity)
        {
            var user = Membership.FindUsersByName(entity.UserName);
            if (user.Count > 0)
                _modelState.AddModelError("User", "User with user name '" + entity.UserName + "' already exists.");
            user = Membership.FindUsersByEmail(entity.Email);
            if (user.Count > 0)
                _modelState.AddModelError("User", "User with e-mail id '" + entity.Email + "' already exists.");
            if (!_modelState.IsValid)
                return false;
            // TO DO: wrap transaction around next statements
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            else
            {
//                Membership.CreateUser(entity.UserName, entity.Password, entity.Email);
                MembershipCreateStatus Status;
                MembershipUser membershipUser = Membership.CreateUser(entity.UserName, entity.Password, entity.Email, entity.PasswordQuestion, entity.PasswordAnswer, true, null, out Status);
                Roles.AddUserToRole(entity.UserName, _subscriberrole);
                var newuser = Membership.GetUser(entity.UserName);
                var userdetail = new UserDetail();
                userdetail.UserID = (Guid)newuser.ProviderUserKey;
                userdetail.SubscriberID = entity.SubscriberID;
                var userdetailservice = new UserDetailService(_modelState);
                if (!userdetailservice.Add(userdetail))
                {
                    _modelState.AddModelError("UserDetail", GenericExceptionHandler.ExceptionMessage());
                    return false;
                }
            }
            return true;
        }

        public bool Edit(Subscriber entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(Subscriber entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<Subscriber> List()
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