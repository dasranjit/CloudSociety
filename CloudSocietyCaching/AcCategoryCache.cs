using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CommonLib.Caching;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class AcCategoryCache : IGenericComboKeyRepository<AcCategory>
    {
        const string CacheName = "AcCategory";
        private IGenericComboKeyRepository<AcCategory> _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public AcCategoryCache()
        {
            try
            {
                _repository = new AcCategoryRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(AcCategory entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.CategoryID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.CategoryID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(AcCategory entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.CategoryID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.CategoryID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(AcCategory entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.CategoryID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.CategoryID.ToString());
                return true;
            }
            else
                return false;
        }

        public IEnumerable<AcCategory> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<AcCategory>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List By Society "+parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public AcCategory GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var acctg = (AcCategory)GenericCache.GetCacheItem(CacheName, parentid.ToString() + id.ToString());
                if (acctg == null)
                {
                    acctg = _repository.GetByIds(parentid, id);
                    GenericCache.AddCacheItem(CacheName, parentid.ToString() + id.ToString(), acctg);
                }
                return acctg;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Society ID: " + parentid.ToString()+" Category ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcCategory> ListById(Guid id)
        {
            try
            {
                var list = (IEnumerable<AcCategory>)GenericCache.GetCacheItem(CacheName, id);
                if (list == null)
                {
                    list = _repository.ListById(id);
                    GenericCache.AddCacheItem(CacheName, id, list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List By Category " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}