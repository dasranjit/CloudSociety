using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyUserRepository
    {
        bool Add(SocietyUser entity);
        void DeleteBySocietyID(Guid societyid);
        void DeleteByUserID(Guid userid);
        IEnumerable<SocietyUser> ListByParentId(Guid parentid);
    }
}
