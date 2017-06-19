using System;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    // this is created to avoid the conflit that was happening between SocietySubscriptionService entity & model.bll class
    public interface ISocietySubscriptionServiceRepository : IGenericChildRepository<SocietySubscriptionService>
    {
        void DeletePendingBySocietySubscriptionID(Guid id);
    }
}
