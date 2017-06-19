using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBuildingUnitChargeHeadRepository
    {
        SocietyBuildingUnitChargeHead GetById(Guid id);
        bool Add(SocietyBuildingUnitChargeHead entity);
        bool Edit(SocietyBuildingUnitChargeHead entity);
        bool Delete(SocietyBuildingUnitChargeHead entity);
        IEnumerable<SocietyBuildingUnitChargeHeadView> ListByParentId(Guid parentid);
    }
}
