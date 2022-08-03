using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.DataContract
{
    public class P_POL_EXCLUSION_BYPLAN
    {
        public long? EXCLUDE_PLAN_ID { get; set; }
        public long? EXCLUDE_DET_ID { get; set; }
        public char? PL_BLOCK { get; set; }
        public string PL_CODE { get; set; }
        public string PL_CODE2 { get; set; }
    }
    public class P_POL_EXCLUSION_BYPLAN_Collection : List<P_POL_EXCLUSION_BYPLAN>
    {
    }
}
