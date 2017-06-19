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
    public class SocietyParkingCache : ISocietyParkingRepository
    {
        const string CacheName = "SocietyParking";
        private ISocietyParkingRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyParkingCache()
        {
            try
            {
                _repository = new SocietyParkingRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyParking GetById(Guid id)
        {
            try
            {
                var SocietyParking = (SocietyParking)GenericCache.GetCacheItem(CacheName, id);
                if (SocietyParking == null)
                {
                    SocietyParking = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, SocietyParking);
                }
                return SocietyParking;
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

        public bool Add(SocietyParking entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyParkingID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyParking entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyParkingID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyParking entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyParkingID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyParking> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyParking>)GenericCache.GetCacheItem(CacheName, parentid);
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

        public IEnumerable<SocietyParkingWithMember> ListWithMemberAsOnDateBySocietyID(Guid societyid, DateTime asondate)
        {
            // Not cached
            try
            {
                return _repository.ListWithMemberAsOnDateBySocietyID(societyid,asondate);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List with Member by Society " + societyid.ToString() + " As On " + asondate.ToString("dd-MMM-yyyy"));
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}