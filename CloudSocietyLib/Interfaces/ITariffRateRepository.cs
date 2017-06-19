using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ITariffRateRepository : IGenericChildRepository<TariffRate>
    {
        bool CopyTariffRatesFromPreviousTariff(Guid TariffID);
        bool InsertTariffRatesFromServiceTypes(Guid TariffID);
        IEnumerable<TariffRate> CurrentList();
        IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusForSubscription(Guid SocietySubscriptionID);
        IEnumerable<TariffRateWithActiveStatus> ListWithActiveStatusMonthlyForSubscription(Guid SocietySubscriptionID);
    }
}
