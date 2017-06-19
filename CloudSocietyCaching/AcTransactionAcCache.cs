using System;
using System.Collections.Generic;
using System.Text;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class AcTransactionAcCache : IAcTransactionAcRepository
    {
        const string CacheName = "AcTransactionAc";
        private IAcTransactionAcRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";
        // Not caching since trans may be created/deleted from billing module
        public AcTransactionAcCache()
        {
            try
            {
                _repository = new AcTransactionAcRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public AcTransactionAc GetById(Guid id)
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

        public bool Add(AcTransactionAc entity)
        {
            return _repository.Add(entity);
        }

        public bool Edit(AcTransactionAc entity)
        {
            return _repository.Edit(entity);
        }

        public bool Delete(AcTransactionAc entity)
        {
            return _repository.Delete(entity);
        }

        public IEnumerable<AcTransactionAc> ListByParentId(Guid parentid)
        {
            try
            {
                return _repository.ListByParentId(parentid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate)
        //{
        //    try
        //    {
        //        return _repository.ListForRecoBySocietyIDAcHeadID(societyid, acheadid, fromdate, todate);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List for Reconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"));
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<AcTransactionAc> ListReconciledBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate)
        //{
        //    try
        //    {
        //        return _repository.ListReconciledBySocietyIDAcHeadID(societyid, acheadid, fromdate, todate);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List Reconciled by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"));
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<AcTransactionAc> ListUnReconciledAsOnDateBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate)
        {
            try
            {
                return _repository.ListUnReconciledAsOnDateBySocietyIDAcHeadID(societyid, acheadid, asondate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for UnReconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString() + " As On " + asondate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcTransactionAc> ListAllByAcTransactionID(Guid actransactionid)
        {
            try
            {
                return _repository.ListAllByAcTransactionID(actransactionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List All by " + actransactionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, string DrCr, DateTime reconciled, bool onlyblank)
        {
            try
            {
                _repository.UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(societyid,acheadid,fromdate,todate,DrCr,reconciled,onlyblank);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Update Reconciled by Bank " + acheadid.ToString()+" for DrCr "+DrCr+" from "+fromdate.ToString("dd-MMM-yyyy")+" to "+todate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, string DrCr)
        {
            try
            {
                return _repository.ListForRecoBySocietyIDAcHeadID(societyid, acheadid, fromdate, todate, DrCr);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Reconciliation by Society " + societyid.ToString() + ", Ac " + societyid.ToString()+" for DrCr "+DrCr + " from " + fromdate.ToString("dd-MMM-yy") + " to " + todate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
