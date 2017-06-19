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
    public class StateService : IReadOnlyRepository<State>
    {
        private IReadOnlyRepository<State> _cache;
//        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _entityname = "State";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public StateService()   // ModelStateDictionary modelState
        {
//            _modelState=modelState;
            _cache = new StateCache();
        }

        public State GetById(Guid id)
        {
            try
            {
                return (_cache.GetById(id));
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<State> List()
        {
            try
            {
                return _cache.List();
            }
            catch
            {
                throw;
            }
        }
    }
}