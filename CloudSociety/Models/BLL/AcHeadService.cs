using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class AcHeadService : IAcHeadRepository
    {
        private IAcHeadRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "AcHead";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public AcHeadService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new AcHeadCache();
        }

        public bool Add(AcHead entity)
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

        public bool Delete(AcHead entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(AcHead entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public AcHead GetByIds(Guid parentid, Guid id)   // SocietyID, AcHeadID
        {
            try
            {
                return (_cache.GetByIds(parentid, id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get", GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcHead> ListByParentId(Guid parentid)   // AcSubCategoryID
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Sub Category " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcHead> ListById(Guid id)   // SocietyID
        {
            try
            {
                return (_cache.ListById(id));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + id.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        //Return List of AcHead by SocietyID and DocType added by Ranjit
        //public IEnumerable<AcHead> AcHeadListBy(Guid SocietyID, string DocType)
        //{
        //    //{ "C", "Cash Payment" }, { "R", "Cash Receipt" }, { "P", "Bank Payment" }, { "B", "Bank Receipt" }, { "U", "Purchase" }, { "E", "Expense" }, { "S", "Bill" }, { "J", "Journal Voucher" } };
        //    //{ "C", "Cash" }, { "B", "Bank" }, { "S", "Creditor" }, { "D", "Debtor" }, { "T", "TDS" } 
        //    switch (DocType)
        //    {
        //        case "C":
        //            return this.ListById(SocietyID).Where(a => a.Nature == "C");
        //        case "R":
        //            goto case "C";
        //        case "P":
        //            return this.ListById(SocietyID).Where(a => a.Nature == "B");
        //        case "B":
        //            goto case "P";
        //        case "U":
        //            return this.ListById(SocietyID).Where(a => a.Nature == "S");
        //        case "E":
        //            goto case "U";
        //        default:
        //            return this.ListById(SocietyID);
        //    }
        //}

        //public IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, string nature)
        //{
        //    try
        //    {
        //        return (_cache.ListBySocietyIDNature(societyid, nature));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List for Society " + societyid.ToString() + ", Natute " + nature, GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public IEnumerable<AcBalance> ListBalanceBySocietyID(Guid societyid, DateTime fromdate, DateTime todate)
        {
            try
            {
                return (_cache.ListBalanceBySocietyID(societyid, fromdate, todate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Trial Balance for Society " + societyid.ToString() + ", From " + fromdate.ToString("dd-MMM-yy") + ", To " + todate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public decimal? GetBalanceAsOnBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate, char brtype)
        {
            try
            {
                return (_cache.GetBalanceAsOnBySocietyIDAcHeadID(societyid, acheadid, asondate, brtype));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get Balance By Society " + societyid.ToString() + ", Ac " + acheadid.ToString() + ", As On " + asondate.ToString("dd-MMM-yy") + " as per " + (brtype == 'B' ? "Books" : "Bank"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcLedger> ListLedgerBySocietyIDAcHeadIds(Guid societyid, string acheadids, DateTime fromdate, DateTime todate)
        {
            try
            {
                return (_cache.ListLedgerBySocietyIDAcHeadIds(societyid, acheadids, fromdate, todate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Ledger for Society " + societyid.ToString() + ", Acs " + acheadids + ", From " + fromdate.ToString("dd-MMM-yy") + ", To " + todate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, string nature, string exclude = "")
        {
            try
            {
                return (_cache.ListBySocietyIDNature(societyid, nature, exclude));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + societyid.ToString() + ", Nature: Include " + nature + ", Exclude " + exclude, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcClBalance> ListBalanceBySocietyIDNatureAsOn(Guid societyid, DateTime asondate, string nature)
        {
            try
            {
                return (_cache.ListBalanceBySocietyIDNatureAsOn(societyid, asondate, nature));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - A/c Balance By Society " + societyid.ToString() + ", As On " + asondate.ToString("dd-MMM-yy") + ", Nature " + nature, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<AcClBalance> ListCashBankOppBalanceBySocietyIDDrCr(Guid societyid, DateTime fromdate, DateTime todate, string drcr)
        {
            try
            {
                return (_cache.ListCashBankOppBalanceBySocietyIDDrCr(societyid, fromdate, todate, drcr));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Cash/Bank Opp A/c Balance By Society " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " from " + todate.ToString("dd-MMM-yy") + ", Dr/Cr " + drcr, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}
