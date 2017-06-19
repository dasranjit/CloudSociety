using System;
using System.Collections.Generic;
using CommonLib.Interfaces;
using CloudSocietyEntities;

namespace CloudSocietyLib.Interfaces
{
    public interface ISocietyMemberRepository : IGenericChildRepository<SocietyMember>
    {
        IEnumerable<SocietyMember> ListBySocietyBuildUnitID(Guid societybuildingunitid);
        IEnumerable<SocietyMember> ListBySocietyBuildUnitIDForNoOpeningBalance(Guid societybuildingunitid);
        IEnumerable<MemberLedger> ListLedgerBySocietySubscriptionIDSocietyMemberID(Guid societysubscriptionid, Guid societymemberid);
        IEnumerable<MemberRecipient> ListRecipientBySocietyIDRole(Guid societyid, Guid societymemberid, string role);
        //IEnumerable<SocietyMember> ListCommitteeMembersBySocietyID(Guid societyid);

        bool IsCommunicationEnabled(Guid societyId);
    }
}
