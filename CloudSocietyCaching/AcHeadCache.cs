using System;
using System.Collections.Generic;
using System.Text;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class AcHeadCache : IAcHeadRepository
    {
        const string CacheName = "AcHead";
        private IAcHeadRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public AcHeadCache()
        {
            try
            {
                _repository = new AcHeadRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(AcHead entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.AcHeadID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.AcHeadID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(AcHead entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.AcHeadID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.AcHeadID.ToString());
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.AcHeadID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(AcHead entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.AcHeadID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.AcHeadID.ToString());
                return true;
            }
            else
                return false;
        }

        public AcHead GetByIds(Guid parentid, Guid id)
        {
            try
            {
                //var ac = (AcHead)GenericCache.GetCacheItem(CacheName, parentid.ToString() + id.ToString());
                //if (ac == null)
                //{
                //    ac = _repository.GetByIds(parentid, id);
                //    GenericCache.AddCacheItem(CacheName, parentid.ToString() + id.ToString(), ac);
                //}
                return _repository.GetByIds(parentid, id);  //ac;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Society ID: " + parentid.ToString() + " Ac ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcHead> ListByParentId(Guid parentid)
        {
            try
            {
                //var list = (IEnumerable<AcHead>)GenericCache.GetCacheItem(CacheName, parentid);
                //if (list == null)
                //{
                //    list = _repository.ListByParentId(parentid);
                //    GenericCache.AddCacheItem(CacheName, parentid,list);
                //}
                return _repository.ListByParentId(parentid);    //list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List By Sub Category " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcHead> ListById(Guid id)
        {
            try
            {
                //var list = (IEnumerable<AcHead>)GenericCache.GetCacheItem(CacheName, id);
                //if (list == null)
                //{
                //    list = _repository.ListById(id);
                //    GenericCache.AddCacheItem(CacheName, id, list);
                //}
                return _repository.ListById(id);    //list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List By Society " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, string nature)
        //{
        //    try
        //    {
        //        return _repository.ListBySocietyIDNature(societyid, nature);
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List By Society " + societyid.ToString() + ", Natute " + nature);
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<AcBalance> ListBalanceBySocietyID(Guid societyid, DateTime fromdate, DateTime todate)
        {
            try
            {
                return _repository.ListBalanceBySocietyID(societyid, fromdate, todate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Trial Balance By Society " + societyid.ToString() + ", From " + fromdate.ToString("dd-MMM-yy") + ", To " + todate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public decimal? GetBalanceAsOnBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate, char brtype)
        {
            try
            {
                return _repository.GetBalanceAsOnBySocietyIDAcHeadID(societyid, acheadid, asondate, brtype);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get Balance By Society " + societyid.ToString() + ", Ac " + acheadid.ToString() + ", As On " + asondate.ToString("dd-MMM-yy") + " as per " + (brtype == 'B' ? "Books" : "Bank"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public IEnumerable<AcLedger> ListLedgerBySocietyIDAcHeadIds(Guid societyid, string acheadids, DateTime fromdate, DateTime todate)
        {
            try
            {
                return _repository.ListLedgerBySocietyIDAcHeadIds(societyid, acheadids, fromdate, todate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Ledger By Society " + societyid.ToString() + ", Acs " + acheadids + ", From " + fromdate.ToString("dd-MMM-yy") + ", To " + todate.ToString("dd-MMM-yy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, string nature, string exclude = "")
        {
            try
            {
                return _repository.ListBySocietyIDNature(societyid, nature, exclude);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List By Society " + societyid.ToString() + ", Nature: Include " + nature + ", Exclude " + exclude);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcClBalance> ListBalanceBySocietyIDNatureAsOn(Guid societyid, DateTime asondate, string nature)
        {
            try
            {
                return _repository.ListBalanceBySocietyIDNatureAsOn(societyid, asondate, nature);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - A/c Balance By Society " + societyid.ToString() + ", As On " + asondate.ToString("dd-MMM-yy") + ", Nature " + nature);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public IEnumerable<AcClBalance> ListCashBankOppBalanceBySocietyIDDrCr(Guid societyid, DateTime fromdate, DateTime todate, string drcr)
        {
            try
            {
                return _repository.ListCashBankOppBalanceBySocietyIDDrCr(societyid, fromdate, todate, drcr);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Cash/Bank Opp A/c Balance By Society " + societyid.ToString() + " from " + fromdate.ToString("dd-MMM-yy") + " from " + todate.ToString("dd-MMM-yy") + ", Dr/Cr " + drcr);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}