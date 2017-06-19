using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CloudSocietyEntities
{
    public partial class SocietyCommunicationSetting
    {
        public bool IsBillingSMSActive { get; set; }
        public bool ShowPaymentLink { get; set; }
        public string PaymentGatewayLink { get; set; }
        public int TransDelayHour { get; set; }
    }
}
