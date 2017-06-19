using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyCache : ISocietyRepository
    {
        const string CacheName = "Society";
        private ISocietyRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";

        //public SocietyCache() :this(new SocietyRepository()){}

        //public SocietyCache(IGenericRepository<Society> repository)
        //{
        //    _repository = repository;
        //    _cache = HttpContext.Current.Cache;
        //}

        public SocietyCache()
        {
            try
            {
                _repository = new SocietyRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Society GetById(Guid id)
        {
            try
            {
                // this object is not cached since it is changed from ActivateSocietyForInvoice
                //var society = (Society)GenericCache.GetCacheItem(CacheName, id);
                // if (society == null)
                //{
                //    society = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, society);
                //}
                //return society;
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

        public bool Add(Society entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheNameForCompany);
                if (entity.SubscriberID != null)
                    GenericCache.RemoveCacheItem(CacheName, (Guid)entity.SubscriberID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Society entity)
        {
            if (_repository.Edit(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.RemoveCacheItem(CacheNameForCompany);
                //GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                //GenericCache.AddCacheItem(CacheName, entity.SocietyID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Society entity)
        {
            if (_repository.Delete(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.RemoveCacheItem(CacheNameForCompany);
                //GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Society> List()
        {
            try
            {
                // this list is not cached since it is changed from ActivateSocietyForInvoice
                //var list = (IEnumerable<Society>)GenericCache.GetCacheItem(CacheName);
                //if (list == null)
                //{
                //    list = _repository.List();
                //    GenericCache.AddCacheItem(CacheName, list);
                //}
                //return list;
                return _repository.List();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Society> ListForSubscriber(Guid id)
        {
            try
            {
                // this list is not cached since it is changed from ActivateSocietyForInvoice
                //var list = (IEnumerable<Society>)GenericCache.GetCacheItem(CacheName, id);

                //if (list == null)
                //{
                //    list = _repository.ListForSubscriber(id);
                //    GenericCache.AddCacheItem(CacheName,id,list);
                //}
                //return list;
                return _repository.ListForSubscriber(id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List For Subscriber " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Society> ListAllocated(Guid id)
        {
            try
            {
                // this list is not cached since it is changed from ActivateSocietyForInvoice
                //var list = (IEnumerable<Society>)GenericCache.GetCacheItem(CacheName, id);

                //if (list == null)
                //{
                //    list = _repository.ListAllocated(id);
                //    GenericCache.AddCacheItem(CacheName, id, list);
                //}
                //return list;
                return _repository.ListAllocated(id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Allocated to " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Society> ListForCompany()
        {
            try
            {
                // this list is not cached since it is changed from ActivateSocietyForInvoice
                //var list = (IEnumerable<Society>)GenericCache.GetCacheItem(CacheNameForCompany);

                //if (list == null)
                //{
                //    list = _repository.ListForCompany();
                //    GenericCache.AddCacheItem(CacheNameForCompany, list);
                //}
                //return list;
                return _repository.ListForCompany();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Company");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void ActivateForInvoice(Guid invoiceid)
        {
            try
            {
                _repository.ActivateForInvoice(invoiceid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Activate for Invoice " + invoiceid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyWithUserAccess> ListWithUserAccessForSubscriber(Guid subscriberid, Guid userid)
        {
            try
            {
                return _repository.ListWithUserAccessForSubscriber(subscriberid, userid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List with User Access For Subscriber " + subscriberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void InsertForTrialUser(Guid userid, string username)
        {
            try
            {
                _repository.InsertForTrialUser(userid, username);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Insert for Trial User " + username);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public IEnumerable<AcFinalReport> GetIncomeExpenditureReportForPeriod(Guid societyid, DateTime from, DateTime to)
        {
            try
            {
                return (_repository.GetIncomeExpenditureReportForPeriod(societyid, from, to));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Income & Expenditure Report from " + from.ToShortDateString() + " to " + to.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureScheduleForPeriod(Guid societyid, DateTime from, DateTime to)
        {
            try
            {
                return (_repository.GetIncomeExpenditureScheduleForPeriod(societyid, from, to));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Income & Expenditure Schedule from " + from.ToShortDateString() + " to " + to.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReport> GetBalanceSheetReportAsOnDate(Guid societyid, DateTime asondate)
        {
            try
            {
                return (_repository.GetBalanceSheetReportAsOnDate(societyid, asondate));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Balance Sheet As On " + asondate.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetBalanceSheetScheduleAsOnDate(Guid societyid, DateTime asondate)
        {
            try
            {
                return (_repository.GetBalanceSheetScheduleAsOnDate(societyid, asondate));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Balance Sheet Schedule As On " + asondate.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool EditConfigurations(SocietyCommunicationSetting objSocietyCommunicationSetting)
        {
            return _repository.EditConfigurations(objSocietyCommunicationSetting);
        }
    }
}