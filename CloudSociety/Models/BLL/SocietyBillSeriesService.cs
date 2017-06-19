using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class SocietyBillSeriesService : ISocietyBillSeriesRepository
    {
        private ISocietyBillSeriesRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBillSeries";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBillSeriesService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBillSeriesCache();
        }

        public bool Add(SocietyBillSeries entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBillSeries entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBillSeries entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBillSeries> ListByParentId(Guid parentid)
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

        public SocietyBillSeries GetByIdCode(Guid parentid, string code)
        {
            try
            {
                return (_cache.GetByIdCode(parentid, code));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        //public IEnumerable<SocietyBillSeriesWithLastBillDate> ListWithLastBillDateBySocietyID(Guid societyid)
        //{
        //    try
        //    {
        //        return (_cache.ListWithLastBillDateBySocietyID(societyid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List with Last Bill Date for " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public IEnumerable<SocietyBillSeriesWithLastDates> ListWithLastDatesBySocietyID(Guid societyid)
        {
            try
            {
                return (_cache.ListWithLastDatesBySocietyID(societyid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List with Last Dates for " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}