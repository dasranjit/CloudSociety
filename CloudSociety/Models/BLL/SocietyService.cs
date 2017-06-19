using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using CloudSocietyLib.Interfaces;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyService : ISocietyRepository
    {
        private ISocietyRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "Society";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyCache();
        }

        public Society GetById(Guid id)
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

        public bool Add(Society entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(Society entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(Society entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<Society> List()
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

        public IEnumerable<Society> ListForSubscriber(Guid id)
        {
            try
            {
                return (_cache.ListForSubscriber(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Subscriber " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<Society> ListAllocated(Guid id)
        {
            try
            {
                return (_cache.ListAllocated(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Allocated for " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<Society> ListForCompany()
        {
            try
            {
                return (_cache.ListForCompany());
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Company", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public void ActivateForInvoice(Guid invoiceid)
        {
            try
            {
                _cache.ActivateForInvoice(invoiceid);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Activate", GenericExceptionHandler.ExceptionMessage());
                throw;
            }
        }

        public IEnumerable<SocietyWithUserAccess> ListWithUserAccessForSubscriber(Guid subscriberid, Guid userid)
        {
            try
            {
                return (_cache.ListWithUserAccessForSubscriber(subscriberid, userid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List with UserAccess for Subscriber " + subscriberid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public void InsertForTrialUser(Guid userid, string username)
        {
            try
            {
                _cache.InsertForTrialUser(userid, username);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Insert for Trial User", GenericExceptionHandler.ExceptionMessage());
                throw;
            }
        }

        public IEnumerable<AcFinalReport> GetIncomeExpenditureReportForPeriod(Guid societyid, DateTime from, DateTime to)
        {
            try
            {
                return (_cache.GetIncomeExpenditureReportForPeriod(societyid, from, to));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Income & Expenditure Report from " + from.ToShortDateString() + " to " + to.ToShortDateString() + " for " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureScheduleForPeriod(Guid societyid, DateTime from, DateTime to)
        {
            try
            {
                return (_cache.GetIncomeExpenditureScheduleForPeriod(societyid, from, to));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Income & Expenditure Schedule from " + from.ToShortDateString() + " to " + to.ToShortDateString() + " for " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcFinalReport> GetBalanceSheetReportAsOnDate(Guid societyid, DateTime asondate)
        {
            try
            {
                return (_cache.GetBalanceSheetReportAsOnDate(societyid, asondate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Balance Sheet As On " + asondate.ToShortDateString() + " for " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetBalanceSheetScheduleAsOnDate(Guid societyid, DateTime asondate)
        {
            try
            {
                return (_cache.GetBalanceSheetScheduleAsOnDate(societyid, asondate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Balance Sheet Schedule As On " + asondate.ToShortDateString() + " for " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public bool EditConfigurations(SocietyCommunicationSetting objSocietyCommunicationSetting)
        {
            if (!_cache.EditConfigurations(objSocietyCommunicationSetting))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }
    }
}