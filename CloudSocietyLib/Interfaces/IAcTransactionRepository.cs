using System;
using System.Collections.Generic;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface IAcTransactionRepository   // : IGenericChildRepository<AcTransaction>
    {
        AcTransaction GetById(Guid id);
        bool Add(AcTransaction entity);
        bool Edit(AcTransaction entity);
        bool Delete(AcTransaction entity);
        IEnumerable<AcTransaction> ListBySocietySubscriptionIDDocType(Guid societysubscriptionid, String doctype);
        IEnumerable<AcTransaction> ListBySocietyIDDocTypePeriod(Guid societyid, String doctype, DateTime startdate, DateTime enddate);
    }
}
