using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSocietyEntities
{
    public partial class SocietyReceiptOnhold
    {
        public string BuildingUnit
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
    }
}
