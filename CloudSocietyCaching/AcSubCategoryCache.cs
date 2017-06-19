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
    public class AcSubCategoryCache : IGenericComboKeyRepository<AcSubCategory>
    {
        const string CacheName = "AcSubCategory";
        private IGenericComboKeyRepository<AcSubCategory> _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public AcSubCategoryCache()
        {
            try
            {
                _repository = new AcSubCategoryRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(AcSubCategory entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SubCategoryID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.SubCategoryID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(AcSubCategory entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SubCategoryID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.SubCategoryID.ToString());
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.SubCategoryID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(AcSubCategory entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SubCategoryID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.SubCategoryID.ToString());
                return true;
            }
            else
                return false;
        }

        public AcSubCategory GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var acsubctg = (AcSubCategory)GenericCache.GetCacheItem(CacheName, parentid.ToString() + id.ToString());
                if (acsubctg == null)
                {
                    acsubctg = _repository.GetByIds(parentid, id);
                    GenericCache.AddCacheItem(CacheName, parentid.ToString() + id.ToString(), acsubctg);
                }
                return acsubctg;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Society ID: " + parentid.ToString() + " Sub Category ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcSubCategory> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<AcSubCategory>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List By Category "+parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<AcSubCategory> ListById(Guid id)
        {
            try
            {
                var list = (IEnumerable<AcSubCategory>)GenericCache.GetCacheItem(CacheName, id);
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
                sb.AppendLine(_exceptioncontext + " - List By Society " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}