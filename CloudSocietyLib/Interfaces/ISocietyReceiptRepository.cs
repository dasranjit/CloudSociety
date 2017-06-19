using System;
using System.Collections.Generic;
using CloudSocietyEntities;
using CommonLib.Interfaces;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyReceiptRepository : IGenericChildRepository<SocietyReceipt>
    {
        IEnumerable<SocietyReceipt> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, String billAbbreviation, DateTime startDate, DateTime endDate);
        IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate);
        IEnumerable<SocietyReceipt> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid);
        IEnumerable<SocietyReceipt> ListByBillAbbreviationSocietyBulidingUnitIDSocietyMemberID(String billabbreviation, Guid societybulidingunitId, Guid societymemberId);
        IEnumerable<SocietyReceipt> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid);
        IEnumerable<SocietyReceiptOnhold> GetOnholdReceipts(Guid societyId, Guid societysubScriptionId, Guid? societyMemberId);
        bool AddTemporary(SocietyReceiptOnhold entity);
        bool GenerateReceiptForOnHoldReciept(Guid SocietyReceiptOnholdID);
    }
}


