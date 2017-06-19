using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class AcTransactionAcService : IAcTransactionAcRepository
    {
        private IAcTransactionAcRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "AcTransactionAc";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public AcTransactionAcService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new AcTransactionAcCache();
        }

        public AcTransactionAc GetById(Guid id)
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

        public bool Add(AcTransactionAc entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(AcTransactionAc entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(AcTransactionAc entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<AcTransactionAc> ListByParentId(Guid parentid)
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

        //public IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate)
        //{
        //    try
        //    {
        //        return (_cache.ListForRecoBySocietyIDAcHeadID(societyid, acheadid, fromdate, todate));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List for Reconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public IEnumerable<AcTransactionAc> ListUnReconciledAsOnDateBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate)
        {
            try
            {
                return (_cache.ListUnReconciledAsOnDateBySocietyIDAcHeadID(societyid, acheadid, asondate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for UnReconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " As On " + asondate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        public IEnumerable<AcTransactionAc> ListAllByAcTransactionID(Guid actransactionid)
        {
            try
            {
                return (_cache.ListAllByAcTransactionID(actransactionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List All for " + actransactionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public void UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, string DrCr, DateTime reconciled, bool onlyblank)
        {
            try
            {
                _cache.UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(societyid, acheadid, fromdate, todate, DrCr, reconciled, onlyblank);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Update Reconciled by Bank " + acheadid.ToString() + " for DrCr " + DrCr + " from " + fromdate.ToString("dd-MMM-yyyy") + " to " + todate.ToString("dd-MMM-yyyy"), GenericExceptionHandler.ExceptionMessage());
            }
        }

        public IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, string DrCr)
        {
            try
            {
                return (_cache.ListForRecoBySocietyIDAcHeadID(societyid, acheadid, fromdate, todate,DrCr));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Reconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString()+" for DrCr "+DrCr + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}