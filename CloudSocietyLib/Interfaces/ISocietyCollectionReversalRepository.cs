using System;
using System.Collections.Generic;
using CloudSocietyEntities;
using CommonLib.Interfaces;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyCollectionReversalRepository : IGenericChildRepository<SocietyCollectionReversal>
    {
        IEnumerable<SocietyCollectionReversal> ListBySocietyBulidingUnitIDBillAbbreviationStartEndDate(Guid societyBulidingUnitId, String billAbbreviation, DateTime startDate, DateTime endDate);
        IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDate(Guid societyId, DateTime startDate, DateTime endDate);
        IEnumerable<SocietyCollectionReversal> ListBySocietyIDStartEndDateSocietyBuildingID(Guid societyId, DateTime startDate, DateTime endDate, Guid societybuildingid);
    }
}


