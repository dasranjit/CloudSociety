using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyBuildingUnitTransferService : ISocietyBuildingUnitTransferRepository
    {
        private ISocietyBuildingUnitTransferRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBuildingUnitTransfer";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBuildingUnitTransferService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBuildingUnitTransferCache();
        }

        public SocietyBuildingUnitTransfer GetById(Guid id)
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

        public bool Add(SocietyBuildingUnitTransfer entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnitTransfer entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnitTransfer entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnitTransfer> ListByParentId(Guid parentid)
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

        public IEnumerable<SocietyBuildingUnitTransfer> ListBySocietyBuildingUnitIDSocietyMemberID(Guid societybuildingunitid, Guid societymemberid)
        {
            try
            {
                return (_cache.ListBySocietyBuildingUnitIDSocietyMemberID(societybuildingunitid, societymemberid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Unit " + societybuildingunitid.ToString() + ", Member " + societymemberid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}