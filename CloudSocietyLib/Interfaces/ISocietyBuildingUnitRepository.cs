using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBuildingUnitRepository : IGenericChildChildRepository<SocietyBuildingUnit>
    {
        IEnumerable<BuildingUnitWithID> ListBuildingUnitBySocietyID(Guid societyid);
        int? GetCountBySocietySubscriptionID(Guid societysubscriptionid);
        //IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid);
        //IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionID(Guid societysubscriptionid, decimal amount);
        //IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid);
        //IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation);
        //IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, string billabbreviation, decimal amount);
        //IEnumerable<MemberBalance> ListBalanceBySocietySubscriptionIDBillAbbreviationSocietyBuildingID(Guid societysubscriptionid, string billabbreviation, Guid societybuildingid);
        IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, Guid? societybuildingid, string billabbreviation);
        IEnumerable<MemberBalance> ListBalanceForSocietySubscription(Guid societysubscriptionid, decimal amount, Guid? societybuildingid, string billabbreviation);
//        IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionID(Guid societysubscriptionid);
        IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionIDSocietyBuildingID(Guid societysubscriptionid, Guid societybuildingid, DateTime? startdate, DateTime? enddate);
        IEnumerable<MemberLedger> ListLedgerForPeriodBySocietySubscriptionID(Guid societysubscriptionid, DateTime? startdate, DateTime? enddate);
    }
}
