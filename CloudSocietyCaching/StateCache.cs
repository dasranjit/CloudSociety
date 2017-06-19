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
    public class StateCache : IReadOnlyRepository<State>
    {
        const string CacheName = "State";
        private IReadOnlyRepository<State> _repository;
//        private Cache _cache;
        //private GenericExceptionHandler GenericExceptionHandler = new GenericExceptionHandler();
        const string _exceptioncontext = CacheName + " Cache";

        public StateCache()
        {
            try 
	        {
                _repository = new StateRepository();
//                _cache = HttpContext.Current.Cache;
	        }
	        catch (Exception ex)
	        {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext," - ","Repository Creation"));
                throw;
	        }
        }

         public State GetById(Guid id)
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

        public IEnumerable<State> List()
        {
            try
            {
                var list = (IEnumerable<State>)GenericCache.GetCacheItem(CacheName);
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

        public void ClearCache()
        {
            GenericCache.RemoveCacheItem(CacheName);
        }
    }
}
