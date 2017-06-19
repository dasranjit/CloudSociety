using System;
using System.Collections.Generic;
using System.Text;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class AcTransactionCache : IAcTransactionRepository
    {
        const string CacheName = "AcTransaction";
        private IAcTransactionRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";
// Not caching since trans may be created/deleted from billing module
        public AcTransactionCache()
        {
            try
            {
                _repository = new AcTransactionRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public AcTransaction GetById(Guid id)
        {
            try
            {
                return _repository.GetById(id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(AcTransaction entity)
        {
            return _repository.Add(entity);
        }

        public bool Edit(AcTransaction entity)
        {
            return _repository.Edit(entity);
        }

        public bool Delete(AcTransaction entity)
        {
            return _repository.Delete(entity);
        }

        //public IEnumerable<AcTransaction> ListByParentId(Guid parentid)
        //{
        //    try
        //    {
        //        return _repository.ListByParentId(parentid);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List by " + parentid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<AcTransaction> ListBySocietyIDDocTypePeriod(Guid societyid, string doctype, DateTime startdate, DateTime enddate)
        {
            try
            {
                return _repository.ListBySocietyIDDocTypePeriod(societyid, doctype, startdate, enddate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Society " + societyid.ToString()+ " Doc Type "+doctype + " from " + startdate.ToString("dd-MMM-yy") + " to " + enddate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcTransaction> ListBySocietySubscriptionIDDocType(Guid societysubscriptionid, string doctype)
        {
            try
            {
                return _repository.ListBySocietySubscriptionIDDocType(societysubscriptionid, doctype);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " -  List by Year " + societysubscriptionid.ToString() + ", Doc Type " + doctype);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
