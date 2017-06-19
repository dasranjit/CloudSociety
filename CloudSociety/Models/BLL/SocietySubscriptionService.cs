using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using CloudSocietyLib.Interfaces;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietySubscriptionService : ISocietySubscriptionRepository
    {
        private ISocietySubscriptionRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietySubscription";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietySubscriptionService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietySubscriptionCache();
        }

        public bool Add(SocietySubscription entity)
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

        public bool Delete(SocietySubscription entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietySubscription entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public SocietySubscription GetById(Guid id)
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

        public IEnumerable<SocietySubscription> ListByParentId(Guid parentid)
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

        public IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForSubscriber(Guid id)
        {
            try
            {
                return (_cache.ListForInvoicingForSubscriber(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Invoicing for Subscriber " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForCompany()
        {
            try
            {
                return (_cache.ListForInvoicingForCompany());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Invoicing for Company", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public SocietySubscriptionWithServices GetInvoicedById(Guid id)
        {
            try
            {
                return (_cache.GetInvoicedById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get Invoiced for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool AccountingEnabled(Guid id)
        {
            try
            {
                return (_cache.AccountingEnabled(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Accounting Enabled for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public bool BillingEnabled(Guid id)
        {
            try
            {
                return (_cache.BillingEnabled(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Billing Enabled for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public bool SMSEnabled(Guid id)
        {
            try
            {
                return (_cache.SMSEnabled(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - SMS Enabled for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public string SocietyYear(Guid id)
        {
            try
            {
                return (_cache.SocietyYear(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Society-Year for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (string.Empty);
            }
        }

        public SocietySubscription GetForCreatedByID(Guid createdbyid)
        {
            try
            {
                return (_cache.GetForCreatedByID(createdbyid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get for Created By "+createdbyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool PrevYearAccountingEnabled(Guid id)
        {
            try
            {
                return (_cache.PrevYearAccountingEnabled(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Previous Year Accounting Enabled for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public bool CreateAcYearClosingEntry(Guid societysubscriptionid)
        {
            if (!_cache.CreateAcYearClosingEntry(societysubscriptionid))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool DeleteAcYearClosingEntry(Guid societysubscriptionid)
        {
            if (!_cache.DeleteAcYearClosingEntry(societysubscriptionid))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<AcFinalReport> GetIncomeExpenditureReport(Guid societysubscriptionid)
        {
            try
            {
                return (_cache.GetIncomeExpenditureReport(societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Income & Expenditure Report for Year " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureSchedule(Guid societysubscriptionid)
        {
            try
            {
                return (_cache.GetIncomeExpenditureSchedule(societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Income & Expenditure Schedule for Year " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcFinalReport> GetBalanceSheetReport(Guid societysubscriptionid)
        {
            try
            {
                return (_cache.GetBalanceSheetReport(societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Balance Sheet Report for Year " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetBalanceSheetSchedule(Guid societysubscriptionid)
        {
            try
            {
                return (_cache.GetBalanceSheetSchedule(societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Balance Sheet Schedule for Year " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}