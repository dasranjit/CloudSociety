using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyParkingService : ISocietyParkingRepository
    {
        private ISocietyParkingRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyParking";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyParkingService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyParkingCache();
        }

        public SocietyParking GetById(Guid id)
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

        public bool Add(SocietyParking entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyParking entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyParking entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
        public IEnumerable<SocietyParking> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyParkingWithMember> ListWithMemberAsOnDateBySocietyID(Guid societyid, DateTime asondate)
        {
            try
            {
                return (_cache.ListWithMemberAsOnDateBySocietyID(societyid,asondate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List with Member by Society " + societyid.ToString() + " As On " + asondate.ToString("dd-MMM-yyyy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}