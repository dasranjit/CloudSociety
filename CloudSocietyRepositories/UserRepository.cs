using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class UserRepository : IUserRepository
    {
        const string _entityname = "User";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public IEnumerable<User> ListForRoleAndSubscriberID(string Role, Guid? SubscriberID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
//                return entities.ListUsersForRoleAndSubscriberID(Role,SubscriberID).OrderBy(s => s.UserName).ToList();
                return entities.ListUsersForRoleAndSubscriberID(Role, SubscriberID).OrderByDescending(s => s.CreateDate).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }


        public IEnumerable<UserWithSocietyAccess> ListWithSocietyAccessForRoleAndSubscriberID(string Role, Guid? SubscriberID, Guid SocietyID)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListUsersWithSocietyAccessForRoleAndSubscriber(Role, SubscriberID, SocietyID).OrderBy(s => s.UserName).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List with Access setting for Society "+SocietyID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}