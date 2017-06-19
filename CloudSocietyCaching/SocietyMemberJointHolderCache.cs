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
    public class SocietyMemberJointHolderCache : IGenericChildRepository<SocietyMemberJointHolder>
    {
        const string CacheName = "SocietyMemberJointHolder";
        private IGenericChildRepository<SocietyMemberJointHolder> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";
        
        public SocietyMemberJointHolderCache()
        {
            try
            {
                _repository = new SocietyMemberJointHolderRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyMemberJointHolder GetById(Guid id)
        {
            try
            {
                //var SocietyMemberJointHolder = (SocietyMemberJointHolder)GenericCache.GetCacheItem(CacheName, id);
                //if (SocietyMemberJointHolder == null)
                //{
                //    SocietyMemberJointHolder = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, SocietyMemberJointHolder);
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

        public bool Add(SocietyMemberJointHolder entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberJointHolderID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyMemberJointHolder entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberJointHolderID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberJointHolderID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyMemberJointHolder entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberJointHolderID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyMemberJointHolder> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyMemberJointHolder>)GenericCache.GetCacheItem(CacheName, parentid);
                if (list == null)
                {
                    list = _repository.ListByParentId(parentid);
                    GenericCache.AddCacheItem(CacheName, parentid,list);
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
