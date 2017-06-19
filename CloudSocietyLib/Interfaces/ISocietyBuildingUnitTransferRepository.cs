using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBuildingUnitTransferRepository : IGenericChildRepository<SocietyBuildingUnitTransfer>
    {
        IEnumerable<SocietyBuildingUnitTransfer> ListBySocietyBuildingUnitIDSocietyMemberID(Guid societybuildingunitid, Guid societymemberid);
    }
}
