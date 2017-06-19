using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Interfaces;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;

namespace CloudSociety.Caching
{
    public class CommunicationRecipientCache : IGenericChildRepository<CommunicationRecipient>
    {
        const string CacheName = "CommunicationRecipient";
        private IGenericChildRepository<CommunicationRecipient> _repository;
        const string _exceptioncontext = CacheName + " Cache";
        // Not caching since trans may be created/deleted from billing module
        public CommunicationRecipientCache()
        {
            try
            {
                _repository = new CommunicationRecipientRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public CommunicationRecipient GetById(Guid id)
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

        public bool Add(CommunicationRecipient entity)
        {
            return _repository.Add(entity);
        }

        public bool Edit(CommunicationRecipient entity)
        {
            return _repository.Edit(entity);
        }

        public bool Delete(CommunicationRecipient entity)
        {
            return _repository.Delete(entity);
        }

        public IEnumerable<CommunicationRecipient> ListByParentId(Guid parentid)
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
