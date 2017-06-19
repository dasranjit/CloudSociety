using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using System.Web.Security;
using CloudSocietyLib.Interfaces;
using System.Data.Objects;

namespace CloudSociety.Repositories
{
    public class SocietySubscriptionRepository : ISocietySubscriptionRepository
    {
        const string _entityname = "SocietySubscription";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public SocietySubscription GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(SocietySubscription entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietySubscriptionID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietySubscriptions.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.SocietySubscriptionID.ToString() + ", " + "Name: " + entity.Society.Name + ", " + "Start Date: " + entity.SubscriptionStart);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(SocietySubscription entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSocietySubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == entity.SocietySubscriptionID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietySubscriptions.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietySubscriptionID.ToString() + ", " + "Name: " + entity.Society.Name + ", " + "Start Date: " + entity.SubscriptionStart);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(SocietySubscription entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var SocietySubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == entity.SocietySubscriptionID);
                entities.SocietySubscriptions.DeleteObject(SocietySubscription);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietySubscriptionID.ToString() + ", " + "Name: " + entity.Society.Name + ", " + "Start Date: " + entity.SubscriptionStart);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<SocietySubscription> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySubscriptions.Where(r => r.SocietyID == parentid).OrderBy(t => t.SubscriptionStart).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForSubscriber(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietySubscriptionsForInvoicingForSubscriber(id).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Invoicing for Subscriber " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForCompany()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietySubscriptionsForInvoicingForCompany().ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Invoicing for Company");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public SocietySubscriptionWithServices GetInvoicedById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetSocietySubscriptionWithServicesInvoiced(id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " With Services Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
        
        public bool AccountingEnabled(Guid id)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var retval = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == id).SocietySubscriptionServices.Any(v => v.ServiceType.Nature=="A" && v.ActiveStatus == "A");
            return retval;
        }

        public bool BillingEnabled(Guid id)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            return entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == id).SocietySubscriptionServices.Any(v => v.ServiceType.Nature == "B" && v.ActiveStatus == "A");
        }

        public bool SMSEnabled(Guid id)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            return entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == id).SocietySubscriptionServices.Any(v => v.ServiceType.Nature == "S" && v.ActiveStatus == "A");
        }


        public string SocietyYear(Guid id)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var societysubscription = entities.SocietySubscriptions.FirstOrDefault(s => s.SocietySubscriptionID == id);
            return societysubscription.Society.Name + " [" + String.Format("{0:yy}", societysubscription.SubscriptionStart) + " - " + String.Format("{0:yy}", societysubscription.SubscriptionEnd) + "]";
        }
        
        public SocietySubscription GetForCreatedByID(Guid createdbyid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.SocietySubscriptions.FirstOrDefault(s => s.CreatedByID == createdbyid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("Created By ID: " + createdbyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool PrevYearAccountingEnabled(Guid id)
        {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            //var subscriptionstart = entities.SocietySubscriptions.FirstOrDefault(y => y.SocietySubscriptionID == id).SubscriptionStart;
            //var previd = entities.SocietySubscriptions.Where(s => s.SubscriptionStart < subscriptionstart).OrderByDescending(s => s.SubscriptionStart).First().SocietySubscriptionID;
            //if (previd == null)
            //    return false;
            //else
            //    return AccountingEnabled(previd);
            var cnt = (int) entities.GetAccountingEnabledCountForPreviousYear(id).FirstOrDefault();
            return (cnt > 0);
        }
        
        public bool CreateAcYearClosingEntry(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = { new ObjectParameter("SocietySubscriptionID", societysubscriptionid), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey) };
                entities.ExecuteFunction("CreateAcYearClosingEntry", qparams);
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Create A/c Year Closing Entry");
                sb.AppendLine("Year: " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
        }

        public bool DeleteAcYearClosingEntry(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                ObjectParameter[] qparams = { new ObjectParameter("SocietySubscriptionID", societysubscriptionid) };
                entities.ExecuteFunction("DeleteAcYearClosingEntry", qparams);
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete A/c Year Closing Entry");
                sb.AppendLine("Year: " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
        }

        public IEnumerable<AcFinalReport> GetIncomeExpenditureReport(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetIncomeExpenditureReportBySocietySubscriptionID(societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Income & Expenditure Report for Year "+societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureSchedule(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetIncomeExpenditureScheduleBySocietySubscriptionID(societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Income & Expenditure Schedule for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReport> GetBalanceSheetReport(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetBalanceSheetReportBySocietySubscriptionID(societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Balance Sheet for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetBalanceSheetSchedule(Guid societysubscriptionid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetBalanceSheetScheduleBySocietySubscriptionID(societysubscriptionid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Balance Sheet Schedule for Year " + societysubscriptionid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}