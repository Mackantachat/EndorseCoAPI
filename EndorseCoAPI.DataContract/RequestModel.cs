using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.DataContract
{
    public class RequestModel
    {
        public string PolicyNo { get; set; }
        public string ChannelType { get; set; }
        public string PolicyHolding { get; set; }
        public string UserId { get; set; }
    }

    public class RequestPolicyHolding
    {
        public string ChannelType { get; set; }
    }

    public class AddExclusionID
    {
        public string EffDate { get; set; }
        public string RefDate { get; set; }
        public string EndorseDate { get; set; }
        public string Referance { get; set; }
        public string CancelEndorseDate { get; set; }
        public string CancelReference { get; set; }
        public string tmneffdt { get; set; }
        public string ddlExcludeTmnCause { get; set; }
    }
}
