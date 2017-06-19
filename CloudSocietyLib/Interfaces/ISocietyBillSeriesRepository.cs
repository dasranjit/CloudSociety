using System;
using CommonLib.Interfaces;
using CloudSocietyEntities;
using System.Collections.Generic;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBillSeriesRepository : IGenericIDCodeComboKeyRepository<SocietyBillSeries>
    {
        IEnumerable<SocietyBillSeriesWithLastDates> ListWithLastDatesBySocietyID(Guid societyid);
    }
}
