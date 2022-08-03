using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.DataContract
{
    public class P_POL_EXCLUSION_TMN
    {
        public long? EXCLUDE_ID { get; set; }

        public DateTime? TMN_DT { get; set; }

        public string TMN_ID { get; set; }

        public long? TRANSACTION_ID { get; set; }
        public string EXC_TMN_CAUSE { get; set; }
    }
}
