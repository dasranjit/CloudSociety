using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyUserCache : ISocietyUserRepository
    {
        const string CacheName = "SocietyUser";
        private ISocietyUserRepository _repository;

        const string _exceptioncontext = CacheName + " Cache";

        public SocietyUserCache()
        {
            try
            {
                _repository = new SocietyUserRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(SocietyUser entity)
        {
            if (_repository.Add(entity))
            {
                // Not Cached
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.AddCacheItem(CacheName, entity.SocietyID, entity);
                return true;
            }
            else
                return false;
        }

        //public bool Edit(SocietyUser entity)
        //{
        //    if (_repository.Edit(entity))
        //    {
        //        GenericCache.RemoveCacheItem(CacheName);
        //        GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
        //        GenericCache.AddCacheItem(CacheName, entity.SocietyID, entity);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //public bool Delete(SocietyUser entity)
        //{
        //    if (_repository.Delete(entity))
        //    {
        //        GenericCache.RemoveCacheItem(CacheName);
        //        GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public void DeleteBySocietyID(Guid societyid)
        {
            try
            {
                _repository.DeleteBySocietyID(societyid); ;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Society " + societyid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public void DeleteByUserID(Guid userid)
        {
            try
            {
                _repository.DeleteByUserID(userid); ;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for User " + userid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyUser> ListByParentId(Guid parentid)
        {
            try
            {
                // not cached
                return _repository.ListByParentId(parentid); ;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for Society "+parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}