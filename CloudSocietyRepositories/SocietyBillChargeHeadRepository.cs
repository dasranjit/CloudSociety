using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudSocietyEntities;
using CommonLib.Exceptions;
using CloudSocietyLib.Interfaces;

namespace CloudSociety.Repositories
{
    public class SocietyBillChargeHeadRepository : ISocietyBillChargeHeadsRepository
    {
        const string _entityname = "SocietyBillChargeHead";
        const string _layername = "Repository";
        const string _exceptioncontext = _entityname + " " + _layername;

        public IEnumerable<SocietyBillChargeHeadWithHead> ListByParentId(Guid parentid)
        {
            try
            {
                var entities = new CloudSocietyModels.CloudSocietyEntities();
                return entities.ListSocietyBillChargeHeadsBySocietyBillID(parentid).ToList();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(_exceptioncontext + " - " + _entityname + " List by " + parentid.ToString());
                GenericExceptionHandler.HandleException(ex, sb.ToString());
                throw;
            }
        }
    }
}