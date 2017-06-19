using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class SocietyCollectionReversalService : ISocietyCollectionReversalRepository
    {
        private ISocietyCollectionReversalRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyCollectionReversal";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyCollectionReversalService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyCollectionReversalCache();
        }

        public SocietyCollectionReversal GetById(Guid id)
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

        public bool Add(SocietyCollectionReversal entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyCollectionReversal entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyCollectionReversal entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyCollectionReversal> ListByParentId(Guid parentid)
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
        
        public IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return (_cache.ListBySocietyIDStartEndDate(societyId, startDate, endDate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        public IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid)
        {
            try
            {
                return (_cache.ListBySocietyIDStartEndDateSocietyBuildingID(societyId, startDate, endDate, societybuildingid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Society " + societyId.ToString() + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy") + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyCollectionReversal> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, string billAbbreviation, DateTime startDate, DateTime endDate)
        {
            try
            {
                return (_cache.ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(societyBulidingUnitId, billAbbreviation, startDate, endDate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for " + societyBulidingUnitId.ToString() + " for " + billAbbreviation + " from " + startDate.ToString("dd-MMM-yyyy") + " to " + endDate.ToString("dd-MMM-yyyy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}