using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using System.Web.Security;

namespace CloudSociety.Services
{
    public class CommunicationTypeService   // : ICommunicationTypeRepository
    {
        private ICommunicationTypeRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "CommunicationType";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public CommunicationTypeService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new CommunicationTypeCache();
        }

        public bool Add(CommunicationType entity)
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

        public bool Delete(CommunicationType entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(CommunicationType entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public CommunicationType GetById(Guid id)
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

        public IEnumerable<CommunicationType> List()
        {
            try
            {
                MembershipUser mUser = Membership.GetUser();
                if (Roles.IsUserInRole(mUser.UserName, "Member"))
                {
                    return (_cache.ListForMember());
                } else
                {
                    return (_cache.List());
                }
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}