using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyLib.Interfaces;
using CloudSocietyEntities;
using CommonLib.Exceptions;

namespace CloudSociety.Repositories
{
    public class UserDetailRepository : IUserDetailRepository   // IGenericRepository<UserDetail>
    {
        const string _entityname = "UserDetail";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public UserDetail GetById(Guid id)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.UserDetails.FirstOrDefault(s => s.UserID == id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(UserDetail entity)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                entities.UserDetails.AddObject(entity);
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname+" Creation");
                sb.AppendLine("ID: " + entity.SubscriberID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
            return true;
        }

        public bool Edit(UserDetail entity)
        {
            try
            {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var OriginalUserDetail = entities.UserDetails.FirstOrDefault(s => s.UserID == entity.UserID);
            entities.UserDetails.ApplyCurrentValues(entity);
            entities.SaveChanges();
            return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Edit");
                sb.AppendLine("ID: " + entity.SubscriberID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
        }

        public bool Delete(UserDetail entity)
        {
            try
            {
            var entities = new CloudSocietyModels.CloudSocietyEntities();
            var UserDetail = entities.UserDetails.FirstOrDefault(s => s.UserID == entity.UserID);
            entities.UserDetails.DeleteObject(UserDetail);
            entities.SaveChanges();
            return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Delete");
                sb.AppendLine("ID: " + entity.SubscriberID.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                return false;
            }
        }

        public IEnumerable<UserDetail> List()
        {
             try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.UserDetails.ToList();
            }
             catch (Exception ex)
             {
                 var sb = new StringBuilder();
                 sb.AppendLine(_exceptioncontext + " - " + _entityname + " List");
                 GenericExceptionHandler.HandleException(ex, sb.ToString());
                 throw;
             }
        }

        public UserDetail GetBySocietyMemberID(Guid societymemberid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.UserDetails.FirstOrDefault(s => s.SocietyMemberID == societymemberid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get By Society Member");
                sb.AppendLine("ID: " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public UserDetail GetBySubscriberID(Guid subscriberid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.UserDetails.FirstOrDefault(s => s.SubscriberID == subscriberid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " Get By Subscriber ID");
                sb.AppendLine("ID: " + subscriberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
