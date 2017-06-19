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
    public class SocietySubscriptionCache : ISocietySubscriptionRepository
    {
        const string CacheName = "SocietySubscription";
        private ISocietySubscriptionRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public SocietySubscriptionCache()
        {
            try
            {
                _repository = new SocietySubscriptionRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietySubscription GetById(Guid id)
        {
            try
            {
                // this object is not cached since it is changed from ActivateSocietyForInvoice
                //var item = (SocietySubscription)GenericCache.GetCacheItem(CacheName, id);
                //if (item == null)
                //{
                //    item = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, item);
                //}
                //return item;
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

        public bool Add(SocietySubscription entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName,entity.SocietySubscriptionID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySubscriptionID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietySubscription entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                GenericCache.AddCacheItem(CacheName, entity.SocietySubscriptionID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietySubscription entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietySubscriptionID);
                return true;
            }
            else
                return false;
        }
            
        public IEnumerable<SocietySubscription> ListByParentId(Guid parentid)
        {
            try
            {
                // this list is not cached since it is changed from ActivateSocietyForInvoice
                //var list = (IEnumerable<SocietySubscription>)GenericCache.GetCacheItem(CacheName, parentid);
                //if (list == null)
                //{
                //    list = _repository.ListByParentId(parentid);
                //    GenericCache.AddCacheItem(CacheName, parentid, list);
                //}
                //return list;
                return _repository.ListByParentId(parentid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for " + parentid);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForSubscriber(Guid id)
        {
            // this list is not cached
            try
            {
                return (_repository.ListForInvoicingForSubscriber(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Invoicing for Subscriber " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForCompany()
        {
            // this list is not cached
            try
            {
                return (_repository.ListForInvoicingForCompany());
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Invoicing for Company");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietySubscriptionWithServices GetInvoicedById(Guid id)
        {
            // not cached
            try
            {
                return (_repository.GetInvoicedById(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get Invoiced for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool AccountingEnabled(Guid id)
        {
            // not cached
            try
            {
                return (_repository.AccountingEnabled(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Accounting Enabled for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool BillingEnabled(Guid id)
        {
            // not cached
            try
            {
                return (_repository.BillingEnabled(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Billing Enabled for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool SMSEnabled(Guid id)
        {
            // not cached
            try
            {
                return (_repository.SMSEnabled(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - SMS Enabled for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public string SocietyYear(Guid id)
        {
            // not cached
            try
            {
                return (_repository.SocietyYear(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Society-Year for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietySubscription GetForCreatedByID(Guid createdbyid)
        {
            try
            {
                var item = (SocietySubscription)GenericCache.GetCacheItem(CacheName, createdbyid);
                if (item == null)
                {
                    item = _repository.GetForCreatedByID(createdbyid);
                    GenericCache.AddCacheItem(CacheName, createdbyid, item);
                }
                return item;
                //                return _repository.GetForCreatedByID(createdbyid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Created By ID: " + createdbyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool PrevYearAccountingEnabled(Guid id)
        {
            // not cached
            try
            {
                return (_repository.PrevYearAccountingEnabled(id));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Previous Year Accounting Enabled for " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool CreateAcYearClosingEntry(Guid societysubscriptionid)
        {
            if (_repository.CreateAcYearClosingEntry(societysubscriptionid))
            {
                return true;
            }
            else
                return false;
        }

        public bool DeleteAcYearClosingEntry(Guid societysubscriptionid)
        {
            if (_repository.DeleteAcYearClosingEntry(societysubscriptionid))
            {
                return true;
            }
            else
                return false;
        }

        public IEnumerable<AcFinalReport> GetIncomeExpenditureReport(Guid societysubscriptionid)
        {
            try
            {
                return (_repository.GetIncomeExpenditureReport(societysubscriptionid));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Income & Expenditure Report for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureSchedule(Guid societysubscriptionid)
        {
            try
            {
                return (_repository.GetIncomeExpenditureSchedule(societysubscriptionid));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Income & Expenditure Schedule for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReport> GetBalanceSheetReport(Guid societysubscriptionid)
        {
            try
            {
                return (_repository.GetBalanceSheetReport(societysubscriptionid));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Balance Sheet Report for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetBalanceSheetSchedule(Guid societysubscriptionid)
        {
            try
            {
                return (_repository.GetBalanceSheetSchedule(societysubscriptionid));
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Balance Sheet Schedule for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}