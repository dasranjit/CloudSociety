using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietySpecialBillUnitRepository
    {
        SocietySpecialBillUnit GetByIds(Guid parentid, Guid id);
        bool Add(SocietySpecialBillUnit entity);
        bool Delete(SocietySpecialBillUnit entity);
        void DeleteByParentId(Guid parentid);
        IEnumerable<SocietySpecialBillUnit> ListByParentId(Guid parentid);
    }
}
