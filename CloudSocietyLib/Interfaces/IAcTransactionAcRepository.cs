using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface IAcTransactionAcRepository : IGenericChildRepository<AcTransactionAc>
    {
        IEnumerable<AcTransactionAc> ListForRecoBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, String DrCr);
//        IEnumerable<AcTransactionAc> ListReconciledBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate);
        IEnumerable<AcTransactionAc> ListUnReconciledAsOnDateBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate);
        IEnumerable<AcTransactionAc> ListAllByAcTransactionID(Guid actransactionid);
        void UpdateReconciledBySocietyIDAcHeadIDDrCrForPeriod(Guid societyid, Guid acheadid, DateTime fromdate, DateTime todate, String DrCr, DateTime reconciled, Boolean onlyblank);
    }
}
