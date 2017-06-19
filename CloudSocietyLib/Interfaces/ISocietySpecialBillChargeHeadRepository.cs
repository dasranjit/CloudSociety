using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietySpecialBillChargeHeadRepository
    {
        SocietySpecialBillChargeHead GetById(Guid id);
        bool Add(SocietySpecialBillChargeHead entity);
        bool Edit(SocietySpecialBillChargeHead entity);
        bool Delete(SocietySpecialBillChargeHead entity);
        IEnumerable<SocietySpecialBillChargeHeadView> ListByParentId(Guid parentid);
    }
}
