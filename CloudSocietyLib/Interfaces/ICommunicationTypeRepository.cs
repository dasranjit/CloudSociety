using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ICommunicationTypeRepository : IGenericRepository<CommunicationType>
    {
        IEnumerable<CommunicationType> ListForMember();
    }
}
