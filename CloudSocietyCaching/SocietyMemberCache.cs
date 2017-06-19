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
    public class SocietyMemberCache : ISocietyMemberRepository
    {
        const string CacheName = "SocietyMember";
        private ISocietyMemberRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public SocietyMemberCache()
        {
            try
            {
                _repository = new SocietyMemberRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public SocietyMember GetById(Guid id)
        {
            try
            {
                //var SocietyMember = (SocietyMember)GenericCache.GetCacheItem(CacheName, id);
                //if (SocietyMember == null)
                //{
                //    SocietyMember = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, SocietyMember);
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

        public bool Add(SocietyMember entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(SocietyMember entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                GenericCache.AddCacheItem(CacheName, entity.SocietyMemberID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(SocietyMember entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyID);
                GenericCache.RemoveCacheItem(CacheName, entity.SocietyMemberID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<SocietyMember> ListByParentId(Guid parentid)
        {
            try
            {
                var list = (IEnumerable<SocietyMember>)GenericCache.GetCacheItem(CacheName, parentid);
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

        public IEnumerable<SocietyMember> ListBySocietyBuildUnitID(Guid societybuildingunitid)
        {
            // cannot cache since dependent on other entities
            try
            {
                return _repository.ListBySocietyBuildUnitID(societybuildingunitid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List by Unit ID " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<SocietyMember> ListBySocietyBuildUnitIDForNoOpeningBalance(Guid societybuildingunitid)
        {
            // cannot cache since dependent on other entities
            try
            {
                return _repository.ListBySocietyBuildUnitIDForNoOpeningBalance(societybuildingunitid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List for No Opening Balance by Unit ID " + societybuildingunitid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyMemberID(Guid societysubscriptionid, Guid societymemberid)
        {
            // Not cached
            try
            {
                return _repository.ListLedgerBySocietySubscriptionIDSocietyMemberID(societysubscriptionid, societymemberid);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Ledger by Society Subscription " + societysubscriptionid.ToString() + " for Member " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<MemberRecipient> ListRecipientBySocietyIDRole(Guid societyid, Guid societymemberid, string role)
        {
            // Not cached
            try
            {
                return _repository.ListRecipientBySocietyIDRole(societyid, societymemberid, role);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Members by Society " + societyid.ToString() + " for Member " + societymemberid.ToString() + "for Role " + role);
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public bool IsCommunicationEnabled(Guid societyId)
        {
            try
            {
                return _repository.IsCommunicationEnabled(societyId);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - Check is Communication Enabled for society id" + societyId.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}