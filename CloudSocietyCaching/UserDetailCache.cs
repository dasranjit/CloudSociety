using System;
using System.Collections.Generic;
using System.Text;
using CloudSocietyLib.Interfaces;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class UserDetailCache : IUserDetailRepository   // IGenericRepository<UserDetail>
    {
        const string CacheName = "UserDetail";
        private IUserDetailRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public UserDetailCache()
        {
            try
            {
                _repository = new UserDetailRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public UserDetail GetById(Guid id)
        {
            try
            {
                return _repository.GetById(id);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool Add(UserDetail entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.UserID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(UserDetail entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.UserID);
                GenericCache.AddCacheItem(CacheName, entity.UserID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(UserDetail entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.UserID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<UserDetail> List()
        {
            try
            {
                var list = (IEnumerable<UserDetail>)GenericCache.GetCacheItem(CacheName);
                if (list == null)
                {
                    list = _repository.List();
                    GenericCache.AddCacheItem(CacheName, list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public UserDetail GetBySocietyMemberID(Guid societymemberid)
        {
            try
            {
                return _repository.GetBySocietyMemberID(societymemberid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get By Society Member ");
                sb.AppendLine("ID: " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public UserDetail GetBySubscriberID(Guid subscriberid)
        {
            try
            {
                return _repository.GetBySubscriberID(subscriberid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get By Subscriber ");
                sb.AppendLine("ID: " + subscriberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}