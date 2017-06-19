using System;
using System.Collections.Generic;
using CloudSocietyEntities;
using CommonLib.Interfaces;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyBillChargeHeadsRepository 
    {       
        IEnumerable<SocietyBillChargeHeadWithHead> ListByParentId(Guid parentid);
    }
}
