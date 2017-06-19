using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietySubscriptionRepository : IGenericChildRepository<SocietySubscription>
    {
        IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForSubscriber(Guid id);
        IEnumerable<SocietySubscriptionWithServices> ListForInvoicingForCompany();
        SocietySubscriptionWithServices GetInvoicedById(Guid id);
        bool AccountingEnabled(Guid id);
        bool BillingEnabled(Guid id);
        bool SMSEnabled(Guid id);
        string SocietyYear(Guid id);
        SocietySubscription GetForCreatedByID(Guid createdbyid);
        bool PrevYearAccountingEnabled(Guid id);
        bool CreateAcYearClosingEntry(Guid societysubscriptionid);
        bool DeleteAcYearClosingEntry(Guid societysubscriptionid);
        IEnumerable<AcFinalReport> GetIncomeExpenditureReport(Guid societysubscriptionid);
        IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureSchedule(Guid societysubscriptionid);
        IEnumerable<AcFinalReport> GetBalanceSheetReport(Guid societysubscriptionid);
        IEnumerable<AcFinalReportSchedule> GetBalanceSheetSchedule(Guid societysubscriptionid);
    }
}
