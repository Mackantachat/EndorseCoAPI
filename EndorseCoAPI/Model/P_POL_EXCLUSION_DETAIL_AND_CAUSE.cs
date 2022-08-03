using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndorseCoAPI.Model
{
    public class P_POL_EXCLUSION_DETAIL_AND_CAUSE : PolicySvcRef.P_POL_EXCLUSION_DETAIL
    {
        public string Exclude_Cause_Detail { get; set; }
        public string ADMIT_DT_TEXT { get; set; }
        public string TMN_TEXT { get; set; }
        public string ENDORSE_PRINTING_TEXT { get; set; }
        public string Exclude_tmn_dt { get; set; }
        public string Exclude_tmn_Cause { get; set; }
        public string Exclude_tmn_Cause_Detail { get; set; }
    }
}
