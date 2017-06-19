using System;
using System.Collections.Generic;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using CloudSocietyLib.Interfaces;
using System.Web.Mvc;
using System.Text;

namespace CloudSociety.Services
{
    public class SocietyBillService : ISocietyBillRepository
    {
        private ISocietyBillRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBill";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBillService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBillCache();
        }

        public SocietyBill GetById(Guid id)
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

        public bool Generate(Guid societyid, string billAbbreviation)
        {
            try
            {
                return _cache.Generate(societyid, billAbbreviation);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Generate", GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }

        public bool Delete(Guid societyid,String billAbbreviation)
        {
            try
            {
                return _cache.Delete(societyid, billAbbreviation);
            }
            catch (Exception ex)
            {
                //_modelState.AddModelError(_exceptioncontext + " - Delete", GenericExceptionHandler.ExceptionMessage());
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Delete for Society " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return (false);
                throw;                
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviation(Guid societyid, DateTime billdate, String billAbbreviation)
        {
            try
            {
                return (_cache.ListBySocietyIDBillDateBillAbbreviation(societyid, billdate, billAbbreviation));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + societyid.ToString() + " and " + billdate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<DateTime> ListBillDatesBySocietySubscriptionID(Guid societysubscriptionid, String billAbbreviation)
        {
            try
            {
                return (_cache.ListBillDatesBySocietySubscriptionID(societysubscriptionid,billAbbreviation));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Bill Date for Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyBill> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid)
        {
            try
            {
                return (_cache.ListBySocietyMemberIDSocietySubscriptionID(societymemberid, societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Member " + societymemberid.ToString() + " and Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        public DateTime? GetPrevBillDateBySocietyBuildingUnitID(Guid SocietyBuildingUnitID, DateTime BillDate)
        {
            try
            {
                return (_cache.GetPrevBillDateBySocietyBuildingUnitID(SocietyBuildingUnitID, BillDate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Get Prev Bill Date For Unit " + SocietyBuildingUnitID.ToString() + " and Bill Date " + BillDate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        //public IEnumerable<SocietyBill> ListBySocietyBuildingUnitID(Guid societybuildingunitid)
        //{
        //    try
        //    {
        //        return (_cache.ListBySocietyBuildingUnitID(societybuildingunitid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List By SocietyBuildingUnit ID" + societybuildingunitid.ToString() , GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}
        
        //public IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdate, string billAbbreviation, Guid societybuildingid)
        //{
        //    try
        //    {
        //        return (_cache.ListBySocietyIDBillDateBillAbbreviationSocietyBuildingID(societyid, billdate, billAbbreviation, societybuildingid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List for Society " + societyid.ToString() + " and " + billdate.ToString("dd-MMM-yy") + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public DateTime? GetLastBillDateBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billAbbreviation)
        {
            try
            {
                return (_cache.GetLastBillDateBySocietySubscriptionIDBillAbbreviation(societysubscriptionid, billAbbreviation));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Last Bill Date for Abbreviation " + billAbbreviation + " & Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        public IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviation(Guid societyid, DateTime billdatestart, DateTime billdateend, string billAbbreviation)
        {
            try
            {
                return (_cache.ListBySocietyIDBillDateRangeBillAbbreviation(societyid, billdatestart, billdateend, billAbbreviation));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + societyid.ToString() + " and " + billdatestart.ToString("dd-MMM-yy") + " to " + billdateend.ToString("dd-MMM-yy") + " for Abbr " + billAbbreviation, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        public IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdatestart, DateTime billdateend, string billAbbreviation, Guid societybuildingid)
        {
            try
            {
                return (_cache.ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(societyid, billdatestart, billdateend, billAbbreviation, societybuildingid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Society " + societyid.ToString() + " from " + billdatestart.ToString("dd-MMM-yy") + " to " + billdateend.ToString("dd-MMM-yy") + " for Abbr " + billAbbreviation + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        
        public bool ReCreateAcTransactionAcs(Guid societysubscriptionid)
        {
            try
            {
                return _cache.ReCreateAcTransactionAcs(societysubscriptionid);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Recreate A/c Transactions for Year " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (false);
            }
        }
    }
}