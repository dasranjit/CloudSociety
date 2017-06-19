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
    public class SocietyParkingTransferCache : IGenericChildRepository<SocietyParkingTransfer>
    {
        const string CacheName = "SocietyParkingTransfer";
        private IGenericChildRepository<SocietyParkingTransfer> _repository;
        //        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        public SocietyParkingTransferCache()
        {
            try
            {
                _repository = new SocietyParkingTransferRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyParkingTransfer GetById(Guid id)
        {
            try
            {
                var trf = (SocietyParkingTransfer)GenericCache.GetCacheItem(CacheName, id);
                if (trf == null)
                {
                    trf = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, trf);
                }
                return trf;
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

        public bool Add(SocietyParkingTransfer entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyParkingTransferID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyParkingTransfer entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingTransferID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyParkingTransferID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyParkingTransfer entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingTransferID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyParkingTransfer> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyParkingTransfer>)GenericCache.GetCacheItem(CacheName, parentid);
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