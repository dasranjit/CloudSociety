using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyRepository : IGenericRepository<Society>
    {
        IEnumerable<Society> ListForSubscriber(Guid id);
        IEnumerable<Society> ListAllocated(Guid id);
        IEnumerable<Society> ListForCompany();
        void ActivateForInvoice(Guid invoiceid);
        IEnumerable<SocietyWithUserAccess> ListWithUserAccessForSubscriber(Guid subscriberid, Guid userid);
        void InsertForTrialUser(Guid userid, string username);
        IEnumerable<AcFinalReport> GetIncomeExpenditureReportForPeriod(Guid societyid, DateTime from, DateTime to);
        IEnumerable<AcFinalReportSchedule> GetIncomeExpenditureScheduleForPeriod(Guid societyid, DateTime from, DateTime to);
        IEnumerable<AcFinalReport> GetBalanceSheetReportAsOnDate(Guid societyid, DateTime asondate);
        IEnumerable<AcFinalReportSchedule> GetBalanceSheetScheduleAsOnDate(Guid societyid, DateTime asondate);

        bool EditConfigurations(SocietyCommunicationSetting objSocietyCommunicationSetting);
    }
}
