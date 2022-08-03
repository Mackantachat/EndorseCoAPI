using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.DataContract
{
    public class P_POL_EXCLUSION_DETAIL
    {
        public long? EXCLUDE_DET_ID { get; set; }
        public long? EXCLUDE_ID { get; set; }
        public string EXCLUDE { get; set; }
        public DateTime? ADMIT_DT { get; set; }
        public string EXCLUDE_CAUSE { get; set; }
        public string ENDORSE_PRINTING { get; set; }
        public string TMN { get; set; }
        public string MINI_UPDATE { get; set; }
        public DateTime? MINI_DT { get; set; }
        public P_POL_EXCLUSION_DETTMN POL_EXCLUSION_DETTMN { get; set; }
        public List<P_POL_EXCLUSION_BYPLAN> POL_EXCLUSION_BYPLAN_Collection { get; set; }
    }
    public class P_POL_EXCLUSION_DETAIL_Collection : List<P_POL_EXCLUSION_DETAIL>
    {
    }
}
