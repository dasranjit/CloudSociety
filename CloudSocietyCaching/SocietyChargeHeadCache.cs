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
    public class SocietyChargeHeadCache : IGenericComboKeyRepository<SocietyChargeHead>
    {
        const string CacheName = "SocietyChargeHead";
        private IGenericComboKeyRepository<SocietyChargeHead> _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public SocietyChargeHeadCache()
        {
            try
            {
                _repository = new SocietyChargeHeadRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public bool Add(SocietyChargeHead entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.ChargeHeadID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.ChargeHeadID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyChargeHead entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.ChargeHeadID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.ChargeHeadID.ToString());
                GenericCache.AddCacheItem(CacheName, entity.SocietyID.ToString() + entity.ChargeHeadID.ToString(), entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyChargeHead entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.ChargeHeadID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID.ToString() + entity.ChargeHeadID.ToString());
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyChargeHead> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyChargeHead>)GenericCache.GetCacheItem(CacheName, parentid);
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
                sb.AppendLine(_exceptioncontext + " - List by Society " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public SocietyChargeHead GetByIds(Guid parentid, Guid id)
        {
            try
            {
                var societyChargeHead = (SocietyChargeHead)GenericCache.GetCacheItem(CacheName, parentid.ToString()+id.ToString());
                if (societyChargeHead == null)
                {
                    societyChargeHead = _repository.GetByIds(parentid, id);
                    GenericCache.AddCacheItem(CacheName, parentid.ToString() + id.ToString(), societyChargeHead);
                }
                return societyChargeHead;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Get");
                sb.AppendLine("Society ID: " + parentid.ToString() + ", ID: " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyChargeHead> ListById(Guid id)
        {
            try
            {
                var list = (IEnumerable<SocietyChargeHead>)GenericCache.GetCacheItem(CacheName, id);
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
                sb.AppendLine(_exceptioncontext + " - List by Head " + id.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}