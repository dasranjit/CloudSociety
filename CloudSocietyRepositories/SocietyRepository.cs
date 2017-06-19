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
    public class SocietyRepository : ISocietyRepository
    {
        const string _entityname = "Society";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public Society GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Societies.FirstOrDefault(s => s.SocietyID == id);
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

        public bool Add(Society entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entity.SocietyID = Guid.NewGuid();
                entity.CreatedOn = DateTime.Now;
                var curUser = Membership.GetUser();
                entity.CreatedByID = (Guid)curUser.ProviderUserKey;
                entities.Societies.AddObject(entity);
                entities.SaveChanges();
                ObjectParameter[] qparams = { new ObjectParameter("SocietyId", entity.SocietyID), new ObjectParameter("CreatedByID", (Guid)curUser.ProviderUserKey) };
                entities.ExecuteFunction("SetDefaultSettingOnSocietyCreation", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Creation");
                sb.AppendLine("ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(Society entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSociety = entities.Societies.FirstOrDefault(s => s.SocietyID == entity.SocietyID);
                var curUser = Membership.GetUser();
                entity.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.Societies.ApplyCurrentValues(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Delete(Society entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var Society = entities.Societies.FirstOrDefault(s => s.SocietyID == entity.SocietyID);
                entities.Societies.DeleteObject(Society);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SocietyID.ToString() + ", " + "Name: " + entity.Name);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public IEnumerable<Society> List()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Societies.ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Society> ListForSubscriber(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Societies.Where(s => s.SubscriberID == id).OrderBy(s => s.Name).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Subscriber " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Society> ListAllocated(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Societies.Where(s => s.SocietyUsers.Any(u => u.UserID == id)).OrderBy(s => s.Name).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for User " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Society> ListForCompany()
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.Societies.Where(s => s.SubscriberID == null).OrderBy(s => s.Name).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List for Company");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void ActivateForInvoice(Guid invoiceid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var curUser = Membership.GetUser();
                ObjectParameter[] qparams = { new ObjectParameter("SubscriptionInvoiceID", invoiceid), new ObjectParameter("UserID", (Guid)curUser.ProviderUserKey) };
                entities.ExecuteFunction("ActivateForInvoice", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Activate");
                sb.AppendLine("Invoice ID: " + invoiceid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyWithUserAccess> ListWithUserAccessForSubscriber(Guid subscriberid, Guid userid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietiesWithUserAccessBySubscriberID(subscriberid, userid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with User Access for Subscriber " + subscriberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void InsertForTrialUser(Guid userid, string username)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                ObjectParameter[] qparams = { new ObjectParameter("CreatedByID", userid), new ObjectParameter("UserName", username) };
                entities.ExecuteFunction("InsertSocietyForTrialUser", qparams);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Insert for Trial User");
                sb.AppendLine("User: " + username);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReport> GetIncomeExpenditureReportForPeriod(Guid societyid, DateTime from, DateTime to)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetIncomeExpenditureReportForPeriodBySocietyID(societyid, from, to);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Income & Expenditure Report from " + from.ToShortDateString() + " to " + to.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureScheduleForPeriod(Guid societyid, DateTime from, DateTime to)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetIncomeExpenditureScheduleForPeriodBySocietyID(societyid, from, to);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Income & Expenditure Schedule from " + from.ToShortDateString() + " to " + to.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReport> GetBalanceSheetReportAsOnDate(Guid societyid, DateTime asondate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetBalanceSheetReportAsOnDateBySocietyID(societyid, asondate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Balance Sheet As On " + asondate.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcFinalReportSchedule> GetBalanceSheetScheduleAsOnDate(Guid societyid, DateTime asondate)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.GetBalanceSheetScheduleAsOnDateBySocietyID(societyid, asondate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Balance Sheet Schedule As On " + asondate.ToShortDateString() + " for " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool EditConfigurations(SocietyCommunicationSetting objSocietyCommunicationSetting)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                var OriginalSociety = entities.Societies.FirstOrDefault(s => s.SocietyID == objSocietyCommunicationSetting.SocietyID);
                var OriginalSocietyCommunicationSetting = entities.SocietyCommunicationSettings.FirstOrDefault(c => c.SocietyCommunicationSettingID == objSocietyCommunicationSetting.SocietyCommunicationSettingID);
                if (null == OriginalSociety || null == OriginalSocietyCommunicationSetting)
                {
                    return false;
                }
                var curUser = Membership.GetUser();
                OriginalSociety.UpdatedByID = (Guid)curUser.ProviderUserKey;
                OriginalSociety.SMS = objSocietyCommunicationSetting.IsBillingSMSActive;
                OriginalSociety.ShowPaymentLink = objSocietyCommunicationSetting.ShowPaymentLink;
                OriginalSociety.PaymentGatewayLink = objSocietyCommunicationSetting.PaymentGatewayLink;
                OriginalSociety.TransDelayHour = objSocietyCommunicationSetting.TransDelayHour;
                entities.Societies.ApplyCurrentValues(OriginalSociety);
                objSocietyCommunicationSetting.UpdatedByID = (Guid)curUser.ProviderUserKey;
                entities.SocietyCommunicationSettings.ApplyCurrentValues(objSocietyCommunicationSetting);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " EditConfigurations");
                sb.AppendLine("ID: " + objSocietyCommunicationSetting.SocietyID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }
    }
}