using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBuildingUnitSubscriptionBalanceRepository : IGenericChildRepository<SocietyBuildingUnitSubscriptionBalance>
    {
        IEnumerable<SocietyBuildingUnitSubscriptionBalance> ListBySocietyBuildingUnitID(Guid societybuildingunitid);
        IEnumerable<SocietyBuildingUnitBalanceWithBillReceiptExistCheck> ListOpeningBalanceBySocietyBuildingUnitIDWithBillReceiptExistCheck(Guid societybuildingunitid);
    }
}
