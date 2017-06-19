using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBillRepository
    {
        SocietyBill GetById(Guid id);
        bool Generate(Guid societyid, String billAbbreviation);
        bool Delete(Guid societyid, String billAbbreviation);
        IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviation(Guid societyid, DateTime billdate, String billAbbreviation);
        IEnumerable<DateTime> ListBillDatesBySocietySubscriptionID(Guid societysubscriptionid, String billAbbreviation);
        IEnumerable<SocietyBill> ListBySocietyMemberIDSocietySubscriptionID(Guid societymemberid, Guid societysubscriptionid);
        DateTime? GetPrevBillDateBySocietyBuildingUnitID(Guid SocietyBuildingUnitID, DateTime BillDate);
        //IEnumerable<SocietyBill> ListBySocietyBuildingUnitID(Guid societybuildingunitid);   // where is this used?
//        IEnumerable<SocietyBill> ListBySocietyIDBillDateBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdate, String billAbbreviation, Guid societybuildingid);   // is this required, since range is used?
        DateTime? GetLastBillDateBySocietySubscriptionIDBillAbbreviation(Guid societysubscriptionid, String billAbbreviation);
        IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviation(Guid societyid, DateTime billdatestart, DateTime billdateend, String billAbbreviation);   // used in Society Reports -> Bill Register
        IEnumerable<SocietyBill> ListBySocietyIDBillDateRangeBillAbbreviationSocietyBuildingID(Guid societyid, DateTime billdatestart, DateTime billdateend, String billAbbreviation, Guid societybuildingid);
        bool ReCreateAcTransactionAcs(Guid societysubscriptionid);
    }
}
