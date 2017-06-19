using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class CommunicationCache : ICommunicationRepository
    {
        const string CacheName = "Communication";
        private ICommunicationRepository _repository;
        //        private Cache _cache;
        const string _exceptioncontext = CacheName + " Cache";
        //const string CacheNameForCompany = CacheName + " - For Company";

        public CommunicationCache()
        {
            try
            {
                _repository = new CommunicationRepository();
                //  _cache = HttpContext.Current.Cache;
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public Communication GetById(Guid id)
        {
            try
            {
                //var ct = (Communication)GenericCache.GetCacheItem(CacheName, id);
                //if (ct == null)
                //{
                var ct = _repository.GetById(id);
                //    GenericCache.AddCacheItem(CacheName, id, ct);
                //}
                return ct;
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

        public bool Add(Communication entity)
        {
            if (_repository.Add(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.AddCacheItem(CacheName, entity.CommunicationID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(Communication entity)
        {
            if (_repository.Edit(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.RemoveCacheItem(CacheName, entity.CommunicationID);
                //GenericCache.AddCacheItem(CacheName, entity.CommunicationID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(Communication entity)
        {
            if (_repository.Delete(entity))
            {
                //GenericCache.RemoveCacheItem(CacheName);
                //GenericCache.RemoveCacheItem(CacheName, entity.CommunicationID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<Communication> List()
        {
            try
            {
                //var list = (IEnumerable<Communication>)GenericCache.GetCacheItem(CacheName);
                //if (list == null)
                //{
                var list = _repository.List();
                //    GenericCache.AddCacheItem(CacheName, list);
                //}
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List");
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        //public IEnumerable<Communication> ListMessagesFromSocietyMemberID(Guid societymemberid, bool published)
        //{
        //    try
        //    {
        //        var list = _repository.ListMessagesFromSocietyMemberID(societymemberid,published);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List From Member ID "+societymemberid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        //public IEnumerable<Communication> ListMessagesToSocietyMemberID(Guid societymemberid)
        //{
        //    try
        //    {
        //        var list = _repository.ListMessagesToSocietyMemberID(societymemberid);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(_exceptioncontext + " - List To Member ID " + societymemberid.ToString());
        //        GenericExceptionHandler.HandleException(ex, sb.ToString());
        //        throw;
        //    }
        //}

        public IEnumerable<Communication> ListDraftMessagesFromSocietyMemberID(Guid societymemberid)
        {
            try
            {
                var list = _repository.ListDraftMessagesFromSocietyMemberID(societymemberid);
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Draft Messages From Member ID " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Communication> ListDraftMessagesFromSocietyMemberID(Guid societymemberid, Guid societyId)
        {
            try
            {
                var list = _repository.ListDraftMessagesFromSocietyMemberID(societymemberid, societyId);
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Draft Messages From Member ID " + societymemberid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Communication> ListPublishedMessagesFromSocietyMemberID(Guid societymemberid, DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null)
        {
            try
            {
                var list = _repository.ListPublishedMessagesFromSocietyMemberID(societymemberid, start, end, communicationtypeid, ticketNo, TicketStatus);
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Published Messages From Member ID " + societymemberid.ToString() + " between " + start.ToString("dd-MMM-yy") + " and " + end.ToString("dd-MMM-yy"));
                if (communicationtypeid != null)
                {
                    sb.AppendLine(" for Type " + communicationtypeid.ToString());
                }
                if (ticketNo != null)
                {
                    sb.AppendLine(" for Ticket # " + String.Format("{0:000000}", ticketNo));
                }
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

        public IEnumerable<Communication> ListMessagesToSocietyMemberID(Guid societymemberid, DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null)
        {
            try
            {
                var list = _repository.ListMessagesToSocietyMemberID(societymemberid, start, end, communicationtypeid, ticketNo, TicketStatus);
                return list;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - List Messages To Member ID " + societymemberid.ToString() + " between " + start.ToString("dd-MMM-yy") + " and " + end.ToString("dd-MMM-yy"));
                if (communicationtypeid != null)
                {
                    sb.AppendLine(" for Type " + communicationtypeid.ToString());
                }
                if (ticketNo != null)
                {
                    sb.AppendLine(" for Ticket # " + String.Format("{0:000000}", ticketNo));
                }
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }

    }
}


