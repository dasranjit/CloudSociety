using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CommonLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyMemberNomineeCache : IGenericChildRepository<SocietyMemberNominee>
    {
        const string CacheName = "SocietyMemberNominee";
        private IGenericChildRepository<SocietyMemberNominee> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyMemberNomineeCache()
        {
            try
            {
                _repository = new SocietyMemberNomineeRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyMemberNominee GetById(Guid id)
        {
            try
            {
                //var SocietyMemberNominee = (SocietyMemberNominee)GenericCache.GetCacheItem(CacheName, id);
                //if (SocietyMemberNominee == null)
                //{
                //    SocietyMemberNominee = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, SocietyMemberNominee);
                //}
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

        public bool Add(SocietyMemberNominee entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberNomineeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyMemberNominee entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberNomineeID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberNomineeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyMemberNominee entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberNomineeID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyMemberNominee> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyMemberNominee>)GenericCache.GetCacheItem(CacheName, parentid);
                if (list == null)
                {
                    list = _repository.ListByParentId(parentid);
                    GenericCache.AddCacheItem(CacheName, parentid, list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}
