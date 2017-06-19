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
    public class DocumentTypeCache : IGenericRepository<DocumentType>
    {
        const string CacheName = "DocumentType";
        private IGenericRepository<DocumentType> _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public DocumentTypeCache()
        {
            try
            {
                _repository = new DocumentTypeRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public DocumentType GetById(Guid id)
        {
            try
            {
                var DocumentType = (DocumentType)GenericCache.GetCacheItem(CacheName, id);
                if (DocumentType == null)
                {
                    DocumentType = _repository.GetById(id);
                    GenericCache.AddCacheItem(CacheName, id, DocumentType);
                }
                return DocumentType;
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

        public bool Add(DocumentType entity)
        {
            if (_repository.Add(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.AddCacheItem(CacheName, entity.DocumentTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Edit(DocumentType entity)
        {
            if (_repository.Edit(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.DocumentTypeID);
                GenericCache.AddCacheItem(CacheName, entity.DocumentTypeID, entity);
                return true;
            }
            else
                return false;
        }

        public bool Delete(DocumentType entity)
        {
            if (_repository.Delete(entity))
            {
                GenericCache.RemoveCacheItem(CacheName);
                GenericCache.RemoveCacheItem(CacheName, entity.DocumentTypeID);
                return true;
            }
            else
                return false;
        }

        public IEnumerable<DocumentType> List()
        {
            try
            {
                var list = (IEnumerable<DocumentType>)GenericCache.GetCacheItem(CacheName);
                if (list == null)
                {
                    list = _repository.List();
                    GenericCache.AddCacheItem(CacheName, list);
                }
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
    }
}