using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface IAcHeadRepository : IGenericComboKeyRepository<AcHead>
    {
        IEnumerable<AcHead> ListBySocietyIDNature(Guid societyid, String nature, String exclude = "");
        IEnumerable<AcBalance> ListBalanceBySocietyID(Guid societyid, DateTime fromdate, DateTime todate);
        Nullable<decimal> GetBalanceAsOnBySocietyIDAcHeadID(Guid societyid, Guid acheadid, DateTime asondate, char brtype);
        IEnumerable<AcLedger> ListLedgerBySocietyIDAcHeadIds(Guid societyid, String acheadids, DateTime fromdate, DateTime todate);
        IEnumerable<AcClBalance> ListBalanceBySocietyIDNatureAsOn(Guid societyid, DateTime asondate, string nature);
        IEnumerable<AcClBalance> ListCashBankOppBalanceBySocietyIDDrCr(Guid societyid, DateTime fromdate, DateTime todate, string drcr);
    }
}
