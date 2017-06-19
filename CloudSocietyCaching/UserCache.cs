using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSocietyLib.Interfaces;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class UserCache : IUserRepository
    {
        const string CacheName = "User";
        private IUserRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public UserCache()
        {
            try
            {
                _repository = new UserRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public IEnumerable<User> ListForRoleAndSubscriberID(string Role, Guid? SubscriberID)
        {
            // Cache is not used
            try
            {
                return _repository.ListForRoleAndSubscriberID(Role, SubscriberID);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public IEnumerable<UserWithSocietyAccess> ListWithSocietyAccessForRoleAndSubscriberID(string Role, Guid? SubscriberID, Guid SocietyID)
        {
            // Cache is not used
            try
            {
                return _repository.ListWithSocietyAccessForRoleAndSubscriberID(Role, SubscriberID, SocietyID);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List with Access setting for Society " + SocietyID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}