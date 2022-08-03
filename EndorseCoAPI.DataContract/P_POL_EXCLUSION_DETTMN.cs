using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.DataContract
{
    public class P_POL_EXCLUSION_DETTMN
    {
        public long? EXCLUDE_DET_ID { get; set; }    
        public DateTime? TMN_DT { get; set; }        
        public string TMN_ID { get; set; }           
        public long? TRANSACTION_ID { get; set; }    
        public char? EXCDET_TMN_CAUSE { get; set; }  
    }
    public class P_POL_EXCLUSION_DETTMN_Collection : List<P_POL_EXCLUSION_DETTMN>
    {
    }
}
