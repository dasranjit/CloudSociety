using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class TariffService : IGenericWithCountRepository<Tariff>
    {
        private IGenericWithCountRepository<Tariff> _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "Tariff";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public TariffService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new TariffCache();
        }

        public bool Add(Tariff entity)
        {
            bool firsttariff = (Count() < 1);
            if (!_modelState.IsValid)
                return false;
            // TO DO: wrap transaction around next statements
            if (!_cache.Add(entity))
            {
                //_modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            var tariffrateservice = new TariffRateService(_modelState);
            try
            {
                if (firsttariff)
                    tariffrateservice.InsertTariffRatesFromServiceTypes(entity.TariffID);
                else
                    tariffrateservice.CopyTariffRatesFromPreviousTariff(entity.TariffID);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Insert Rates", GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(Tariff entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(Tariff entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public Tariff GetById(Guid id)
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

        public IEnumerable<Tariff> List()
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


        public int Count()
        {
            try
            {
                return (_cache.Count());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Count", GenericExceptionHandler.ExceptionMessage());
                return (-1);
            }
        }
    }
}