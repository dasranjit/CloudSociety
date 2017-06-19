using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyParkingRepository : IGenericChildRepository<SocietyParking>
    {
        IEnumerable<SocietyParkingWithMember> ListWithMemberAsOnDateBySocietyID(Guid societyid, DateTime asondate);
    }
}
