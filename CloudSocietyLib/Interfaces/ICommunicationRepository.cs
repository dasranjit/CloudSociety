using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ICommunicationRepository : IGenericRepository<Communication>
    {
        IEnumerable<Communication> ListDraftMessagesFromSocietyMemberID(Guid societymemberid);
        IEnumerable<Communication> ListDraftMessagesFromSocietyMemberID(Guid societymemberid,Guid societyId);
        IEnumerable<Communication> ListPublishedMessagesFromSocietyMemberID(Guid societymemberid, DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null);
        IEnumerable<Communication> ListMessagesToSocietyMemberID(Guid societymemberid, DateTime start, DateTime end, Guid? communicationtypeid = null, long? ticketNo = null, string TicketStatus = null);
    }
}
