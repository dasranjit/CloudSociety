using System;
using System.Collections.Generic;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Caching;
using System.Web.Mvc;
using System.Linq;

namespace CloudSociety.Services
{
    public class AcTransactionService : IAcTransactionService   // Repository
    {
        private IAcTransactionRepository _cache;
        private ModelStateDictionary _modelState;
        const string _entityname = "AcTransaction";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public AcTransactionService(ModelStateDictionary modelState)
        {
            _modelState = modelState;
            _cache = new AcTransactionCache();
        }

        public bool Add(AcTransaction entity)
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

        public bool Delete(AcTransaction entity)
        {
            if (!_cache.Delete(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public bool Edit(AcTransaction entity)
        {
            if (!_cache.Edit(entity))
            {
                _modelState.AddModelError(_entityname, GenericExceptionHandler.ExceptionMessage());
                return false;
            }
            return true;
        }

        public AcTransaction GetById(Guid id)
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

        //public IEnumerable<AcTransaction> ListByParentId(Guid parentid)
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

        public IEnumerable<AcTransaction> ListBySocietyIDDocTypePeriod(Guid societyid, string doctype, DateTime startdate, DateTime enddate)
        {
            try
            {
                return _cache.ListBySocietyIDDocTypePeriod(societyid, doctype,startdate, enddate);
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List by Society " + societyid.ToString() + " Doc Type "+doctype + " from " + startdate.ToString("dd-MMM-yy") + " to " + enddate.ToString("dd-MMM-yy"), GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }

        //public IEnumerable<AcTransaction> ListBySocietyIDDocType(Guid societyid, string doctype)
        //{
        //    try
        //    {
        //        return this.ListByParentId(societyid).Where(a => a.DocType == doctype);
        //    }
        //    catch
        //    {
        //        _modelState.AddModelError(_exceptioncontext + " - List by Society " + societyid.ToString() + " Doc Type " + doctype , GenericExceptionHandler.ExceptionMessage());
        //        return (null);
        //    }
        //}

        public string GetAcNatureByDocType(string doctype)
        {
            string nature;
            switch (doctype)
            {
                case "CP":
                case "CR":
                    nature = "C";
                    break;
                case "BP":
                case "BR":
                    nature = "B";
                    break;
                case "PB":
                case "EB":
                    nature = "S";
                    break;
                case "SB":
                    nature = "D";
                    break;
                case "JV":
                    nature = "A";
                    break;
                default:
                    nature = "";
                    break;
            }
            return (nature);
        }

        public IEnumerable<AcTransaction> ListBySocietySubscriptionIDDocType(Guid societysubscriptionid, string doctype)
        {
            try
            {
                return (_cache.ListBySocietySubscriptionIDDocType(societysubscriptionid, doctype));
            }
            catch
            {
                _modelState.AddModelError(_exceptioncontext + " - List for Year " + societysubscriptionid.ToString()+", DocType "+doctype, GenericExceptionHandler.ExceptionMessage());
                return (null);
            }
        }
        //public Guid? GetIDBySocietySubscriptionIDDocType(Guid societysubscriptionid, string doctype)
        //{
        //    Guid? AcTransactionID = new Guid();
        //    AcTransactionID = null;
        //    foreach (var Act in this.ListBySocietySubscriptionIDDocType(societysubscriptionid, doctype))
        //    {
        //        AcTransactionID = Act.AcTransactionID; 
        //    }
        //    return AcTransactionID;
        //}
    }
}
