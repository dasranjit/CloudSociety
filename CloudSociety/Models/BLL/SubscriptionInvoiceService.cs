using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class SubscriptionInvoiceService : ISubscriptionInvoiceRepository
    {
        private ISubscriptionInvoiceRepository _cache;
        private ModelStateDictionary _modelState;
        //private GenericExceptionHandler GenericExceptionHandler; // = new GenericExceptionHandler()
        const string _entityname = "SubscriptionInvoice";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SubscriptionInvoiceService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SubscriptionInvoiceCache();
        }

        public bool Delete(SubscriptionInvoice entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SubscriptionInvoice entity)
        {
            var oriinvoice = GetById(entity.SubscriptionInvoiceID);
            var oripaid = (oriinvoice.PaidAmount >= oriinvoice.InvoiceAmount);
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            if (!oripaid && (entity.PaidAmount >= entity.InvoiceAmount))
            {
                var societyservice = new SocietyService(_modelState);
                societyservice.ActivateForInvoice(entity.SubscriptionInvoiceID);
            }
            return true;
        }

        public SubscriptionInvoice GetById(Guid id)
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

        public Guid? Create(string Subscriptions, Guid SubscriberID)
        {
            Guid? SubscriptionInvoiceID = _cache.Create(Subscriptions, SubscriberID);
            if (SubscriptionInvoiceID == null)
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
            }
            return SubscriptionInvoiceID;
        }

        public IEnumerable<SubscriptionInvoice> List()
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


        public IEnumerable<SubscriptionInvoice> ListPending()
        {
            try
            {
                return (_cache.ListPending());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Pending List", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }


        public Guid? Create(string Subscriptions)
        {
            Guid? SubscriptionInvoiceID = _cache.Create(Subscriptions);
            if (SubscriptionInvoiceID == null)
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
            }
            return SubscriptionInvoiceID;
        }
    }
}
