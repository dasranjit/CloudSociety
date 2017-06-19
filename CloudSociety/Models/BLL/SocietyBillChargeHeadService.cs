using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class SocietyBillChargeHeadService : ISocietyBillChargeHeadsRepository
    {
        private ISocietyBillChargeHeadsRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBillChargeHead";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBillChargeHeadService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBillChargeHeadCache();
        }

        //IEnumerable<SocietyBillChargeHeadWithHead> ISocietyBillChargeHeadsRepository.ListByParentId(Guid parentid)
        //{
        //    try
        //    {
        //        return (_cache.ListByParentId(parentid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List for " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public IEnumerable<SocietyBillChargeHeadWithHead> ListByParentId(Guid parentid)
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
    }
}