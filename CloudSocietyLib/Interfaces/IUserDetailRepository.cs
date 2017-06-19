using System;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface IUserDetailRepository : IGenericRepository<UserDetail>
    {
        UserDetail GetBySocietyMemberID(Guid societymemberid);
        UserDetail GetBySubscriberID(Guid subscriberid);
    }
}
