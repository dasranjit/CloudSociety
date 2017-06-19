using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> ListForRoleAndSubscriberID(String Role, Guid? SubscriberID);
        IEnumerable<UserWithSocietyAccess> ListWithSocietyAccessForRoleAndSubscriberID(String Role, Guid? SubscriberID, Guid SocietyID);
    }
}
