using System;
using System.Collections.Generic;
using System.Text;
using CommonLib.Exceptions;
using CloudSocietyEntities;
using CloudSociety.Repositories;
using CommonLib.Interfaces;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Caching
{
    public class SocietyBillChargeHeadCache : ISocietyBillChargeHeadsRepository
    {
        const string CacheName = "SocietyBillChargeHead";
        private ISocietyBillChargeHeadsRepository _repository;
        const string _exceptioncontext = CacheName + " Cache";

        public SocietyBillChargeHeadCache()
        {
            try
            {
                _repository = new SocietyBillChargeHeadRepository();
            }
            catch (Exception ex)
            {
                GenericExceptionHandler.HandleException(ex, string.Concat(_exceptioncontext, " - ", "Repository Creation"));
                throw;
            }
        }

        public IEnumerable<SocietyBillChargeHeadWithHead> ListByParentId(Guid parentid)
        {
            try
            {
                // not cached
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