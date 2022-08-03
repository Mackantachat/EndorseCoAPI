using System;
using System.Collections.Generic;
using System.Text;

namespace EndorseCoAPI.Model
{
    public class EndorseCoModel
    {
        public string CusName { get; set; }
        public string IdCardNo { get; set; }
        public string BirthDate { get; set; }
        public string NameCode { get; set; }
        public string PolicyId { get; set; }

        public P_POL_EXCLUSION2_VIEW[] P_POL_EXCLUSION2 { get; set; }
        public P_POL_EXCLUSION_DETAIL_AND_CAUSE[] P_POL_EXCLUSION_DETAIL_AND_CAUSE { get; set; }
    }

    public class SaveDataModel : EndorseCoModel
    {
        public string txtPolicyNo { get; set; }
        public string UpdateProcess { get; set; }
        public string ChannelType { get; set; }
        public string PolicyHolding { get; set; }
        public string UserID { get; set; }
    }

    public class P_POL_EXCLUSION2_VIEW
    {
        public string EXCLUDE_ID { get; set; }
        public string ENDORSE_ID { get; set; }
        public string ENDORSE_DT { get; set; }
        public string CANCEL_ENDORSE_DT { get; set; }
        public string CANCEL_REFERENCE { get; set; }
        public string EXC_TMN_CAUSE { get; set; }
        public string UpdateProcess { get; set; }
        public string Seq { get; set; }
        public string EXCLUDE_TRN_DT { get; set; }
        public string EFF_DT { get; set; }
        public string REF_DT { get; set; }
        public string Reference { get; set; }
        public string Underwrite_Id { get; set; }
        public string Underwrite_Name { get; set; }
        public string TMN { get; set; }
        public string TMN_DESCRIPT { get; set; }
        public string clmtmneffdt { get; set; }
    }
}
