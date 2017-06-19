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
    public class SocietyMemberDocumentCache : IGenericChildRepository<SocietyMemberDocument>
    {
        const string CacheName = "SocietyMemberDocument";
        private IGenericChildRepository<SocietyMemberDocument> _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyMemberDocumentCache()
        {
            try
            {
                _repository = new SocietyMemberDocumentRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyMemberDocument GetById(Guid id)
        {
            try
            {
                //var SocietyMemberDocument = (SocietyMemberDocument)GenericCache.GetCacheItem(CacheName, id);
                //if (SocietyMemberDocument == null)
                //{
                //    SocietyMemberDocument = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, SocietyMemberDocument);
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

        public bool Add(SocietyMemberDocument entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberDocumentID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyMemberDocument entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberDocumentID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberDocumentID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyMemberDocument entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberDocumentID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyMemberDocument> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyMemberDocument>)GenericCache.GetCacheItem(CacheName, parentid);
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
