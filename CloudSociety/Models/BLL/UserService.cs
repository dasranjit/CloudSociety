using System;
using System.Collections.Generic;
using CloudSocietyEntities;
using CloudSociety.Caching;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Services
{
    public class UserService : IUserRepository
    {
        private IUserRepository _cache;
        const string _entityname = "User";
        const string _layername = "Service";
        const string _exceptioncontext = _entityname + " " + _layername;
        const string _subscriberrole = _entityname;

        public UserService()   // ModelStateDictionary modelState
        {
            _cache = new UserCache();
        }

        public IEnumerable<User> ListForRoleAndSubscriberID(string Role, Guid? SubscriberID)
        {
            try
            {
                return _cache.ListForRoleAndSubscriberID(Role, SubscriberID);
            }
            catch
            {
                throw;
            }
        }


        public IEnumerable<UserWithSocietyAccess> ListWithSocietyAccessForRoleAndSubscriberID(string Role, Guid? SubscriberID, Guid SocietyID)
        {
            try
            {
                return _cache.ListWithSocietyAccessForRoleAndSubscriberID(Role, SubscriberID, SocietyID);
            }
            catch
            {
                throw;
            }
        }
    }
}