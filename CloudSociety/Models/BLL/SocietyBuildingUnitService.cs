using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;

namespace CloudSociety.Services
{
    public class SocietyBuildingUnitService : ISocietyBuildingUnitRepository
    {
        private ISocietyBuildingUnitRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "SocietyBuildingUnit";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public SocietyBuildingUnitService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new SocietyBuildingUnitCache();
        }

        public SocietyBuildingUnit GetById(Guid id)
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

        public bool Add(SocietyBuildingUnit entity)
        {
            if (!_cache.Add(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(SocietyBuildingUnit entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Delete(SocietyBuildingUnit entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietyBuildingUnit> ListByParentId(Guid parentid)
        {
            try
            {
                return (_cache.ListByParentId(parentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Building " + parentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<SocietyBuildingUnit> ListByParentParentId(Guid parentparentid)
        {
            try
            {
                return (_cache.ListByParentParentId(parentparentid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Society " + parentparentid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<BuildingUnitWithID> ListBuildingUnitBySocietyID(Guid societyid)
        {
            try
            {
                return (_cache.ListBuildingUnitBySocietyID(societyid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List BuildingUnit by Society " + societyid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public int? GetCountBySocietySubscriptionID(Guid societysubscriptionid)
        {
            try
            {
                return (_cache.GetCountBySocietySubscriptionID(societysubscriptionid));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - Count BuildingUnit by Society Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid)
        //{
        //    try
        //    {
        //        return (_cache.ListBalanceBySocietySubscriptionID(societysubscriptionid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid, decimal amount)
        //{
        //    try
        //    {
        //        return (_cache.ListBalanceBySocietySubscriptionID(societysubscriptionid, amount));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Amount >= " + amount.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid)
        //{
        //    try
        //    {
        //        return (_cache.ListBalanceBySocietySubscriptionIDSocietyBuildingID(societysubscriptionid, societybuildingid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        //public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionID(Guid societysubscriptionid)
        //{
        //    try
        //    {
        //        return (_cache.ListLedgerBySocietySubscriptionID(societysubscriptionid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Ledger by Society Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}
        
        //public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid)
        //{
        //    try
        //    {
        //        return (_cache.ListLedgerBySocietySubscriptionIDSocietyBuildingID(societysubscriptionid, societybuildingid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Ledger by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation)
        //{
        //    try
        //    {
        //        return (_cache.ListBalanceBySocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " & Abbr " + billabbreviation, GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation, decimal amount)
        //{
        //    try
        //    {
        //        return (_cache.ListBalanceBySocietySubscriptionIDBillAbbreviation(societysubscriptionid, billabbreviation, amount));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Amount >= " + amount.ToString() + " & Abbr " + billabbreviation, GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        //public IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviationSocietyBuildingID(Guid societysubscriptionid, string billabbreviation, Guid societybuildingid)
        //{
        //    try
        //    {
        //        return (_cache.ListBalanceBySocietySubscriptionIDBillAbbreviationSocietyBuildingID(societysubscriptionid, billabbreviation, societybuildingid));
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString() + " & Abbr " + billabbreviation, GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}
        
        public IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, Guid? societybuildingid, string billabbreviation)
        {
            try
            {
                return (_cache.ListBalanceForSocietySubscription(societysubscriptionid, societybuildingid, billabbreviation));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString() + " & Abbr " + billabbreviation, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, decimal amount, Guid? societybuildingid, string billabbreviation)
        {
            try
            {
                return (_cache.ListBalanceForSocietySubscription(societysubscriptionid, amount, societybuildingid, billabbreviation));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Balance by Society Subscription " + societysubscriptionid.ToString() + " for Amount >= " + amount.ToString() + " for Building " + societybuildingid.ToString() + " & Abbr " + billabbreviation, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                return (_cache.ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(societysubscriptionid, societybuildingid, startdate, enddate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Ledger from " + string.Format("dd-MMM-yy", startdate) + " to " + string.Format("dd-MMM-yy", enddate) + " by Society Subscription " + societysubscriptionid.ToString() + " for Building " + societybuildingid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        public IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionID(Guid societysubscriptionid, DateTime? startdate, DateTime? enddate)
        {
            try
            {
                return (_cache.ListLedgerForPeriodBySocietySubscriptionID(societysubscriptionid, startdate, enddate));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List Ledger from " + string.Format("dd-MMM-yy", startdate) + " to " + string.Format("dd-MMM-yy", enddate) + " by Society Subscription " + societysubscriptionid.ToString(), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
    }
}