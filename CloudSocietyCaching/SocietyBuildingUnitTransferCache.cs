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
    public class SocietyBuildingUnitTransferCache : ISocietyBuildingUnitTransferRepository
    {
        const string CacheName = "SocietyBuildingUnitTransfer";
        private ISocietyBuildingUnitTransferRepository _repository;
        //        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        public SocietyBuildingUnitTransferCache()
        {
            try
            {
                _repository = new SocietyBuildingUnitTransferRepository();
                //                _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyBuildingUnitTransfer GetById(Guid id)
        {
            try
            {
                //var trf = (SocietyBuildingUnitTransfer)GenericCache.GetCacheItem(CacheName, id);
                //if (trf == null)
                //{
                //    trf = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, trf);
                //}
                //return trf;
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

        public bool Add(SocietyBuildingUnitTransfer entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingUnitTransferID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyBuildingUnitTransfer entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitTransferID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyBuildingUnitTransferID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyBuildingUnitTransfer entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyBuildingUnitTransferID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyBuildingUnitTransfer> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyBuildingUnitTransfer>)GenericCache.GetCacheItem(CacheName, parentid);
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

        public IEnumerable<SocietyBuildingUnitTransfer> ListBySocietyBuildingUnitIDSocietyMemberID(Guid societybuildingunitid, Guid societymemberid)
        {
            try
            {
                var list = (IEnumerable<SocietyBuildingUnitTransfer>)GenericCache.GetCacheItem(CacheName, societybuildingunitid.ToString()+societymemberid.ToString());
                if (list == null)
                {
                    list = _repository.ListBySocietyBuildingUnitIDSocietyMemberID(societybuildingunitid, societymemberid);
                    GenericCache.AddCacheItem(CacheName, societybuildingunitid.ToString() + societymemberid.ToString(), list);
                }
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Unit " + societybuildingunitid.ToString() + ", Member " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}