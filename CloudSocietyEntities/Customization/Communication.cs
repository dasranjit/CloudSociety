using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSocietyEntities
{
    public partial class Communication
    {
        public string FlatNo
        {
            get
            {
                if (SocietyMember.SocietyBuildingUnitTransfers.Count > 0)
                {
                    return SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.SocietyBuilding.Building + " - " + SocietyMember.SocietyBuildingUnitTransfers.FirstOrDefault().SocietyBuildingUnit.Unit;
                }
                else
                {
                    return "";
                }
            }
        }
        public string CreatedByMember { get; set; }
        public string OfficeBearersList { get; set; }
        public string MemberList { get; set; }
        public bool ShowClose { get; set; }
        public string ApprovedByWithUnit { get; set; }
    }
}
