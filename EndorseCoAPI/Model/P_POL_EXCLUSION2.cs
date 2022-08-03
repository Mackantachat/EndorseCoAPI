using EndorseCoAPI.DataContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.Model
{
    public class P_POL_EXCLUSION2 : P_POL_EXCLUSION
    {
        public long? ENDORSE_ID { get; set; }
        public DateTime? ENDORSE_DT { get; set; }
        public DateTime? CANCEL_ENDORSE_DT { get; set; }
        public string CANCEL_REFERENCE { get; set; }
        public string EXC_TMN_CAUSE { get; set; }
        public string UpdateProcess { get; set; }
        public int Seq { get; set; }

    }
}
