using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISubscriptionInvoiceRepository : IReadOnlyRepository<SubscriptionInvoice>
    {
        Guid? Create(string Subscriptions, Guid SubscriberID);
        Guid? Create(string Subscriptions);
        bool Edit(SubscriptionInvoice entity);
        bool Delete(SubscriptionInvoice entity);
        IEnumerable<SubscriptionInvoice> ListPending();
    }
}
