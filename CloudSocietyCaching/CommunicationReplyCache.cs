using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class CommunicationReplyCache : IGenericChildRepository<CommunicationReply>
    {
        const string CacheName = "CommunicationReply";
        private IGenericChildRepository<CommunicationReply> _repository;
        const string _exceptioncontext = CacheName + " Cache";
        // Not caching since trans may be created/deleted from billing module
        public CommunicationReplyCache()
        {
            try
            {
                _repository = new CommunicationReplyRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public CommunicationReply GetById(Guid id)
        {
            try
            {
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

        public bool Add(CommunicationReply entity)
        {
            return _repository.Add(entity);
        }

        public bool Edit(CommunicationReply entity)
        {
            return _repository.Edit(entity);
        }

        public bool Delete(CommunicationReply entity)
        {
            return _repository.Delete(entity);
        }

        public IEnumerable<CommunicationReply> ListByParentId(Guid parentid)
        {
            try
            {
                return _repository.ListByParentId(parentid);
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
