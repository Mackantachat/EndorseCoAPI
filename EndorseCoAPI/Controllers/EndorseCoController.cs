using EndorseCoAPI.DataContract;
using EndorseCoAPI.Model;
using ITUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PolicySvcRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EndorseCoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndorseCoController : BaseController
    {
        public EndorseCoController(IConfiguration Configuration) : base(Configuration)
        {

        }
        [HttpPost]
        [Route("SearchingPolicy")]
        public IActionResult SearchingPolicy([FromBody] RequestModel request)
        {
            try
            {
                EndorseCoModel data = new EndorseCoModel();
                request.PolicyNo = request.PolicyNo.PadLeft(8, '0');
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["NewBisSvcRef"]);
                NewBisSvcRef.NewBISSvcClient newbisClient = new NewBisSvcRef.NewBISSvcClient(binding, endpoint);
                decimal? pPolicyId = 0;
                NewBisSvcRef.ProcessResult prPol = new NewBisSvcRef.ProcessResult();
                pPolicyId = newbisClient.GetPolicyIDByChannelType(request.ChannelType, request.PolicyNo, request.PolicyHolding, out prPol);
                if (pPolicyId > 0 && pPolicyId != null)
                {
                    data.PolicyId = pPolicyId.ToString();

                    long policyId = (long)pPolicyId;
                    EndpointAddress endpointPolicy = new EndpointAddress(Configuration["PolicySvcRef"]);
                    PolicySvcRef.PolicySvcClient policyClient = new PolicySvcRef.PolicySvcClient(binding, endpointPolicy);
                    PolicySvcRef.P_POL_NAME[] pPol_NameArr = null;
                    long[] pPolicyIdArr = { (long)policyId };
                    policyClient.GetP_POL_NAME(pPolicyIdArr, null, out pPol_NameArr);
                    if (pPol_NameArr != null && pPol_NameArr.Count() > 0)
                    {
                        PolicySvcRef.P_POL_NAME pPol_NameObj = new PolicySvcRef.P_POL_NAME();
                        pPol_NameObj = pPol_NameArr[0];
                        if (pPol_NameObj.NAME_ID != null)
                        {
                            long Name_Id = pPol_NameObj.NAME_ID.Value;
                            long[] Name_ids = { (long)Name_Id };
                            PolicySvcRef.P_NAME_ID[] pName_IdArr = null;
                            policyClient.GetP_NAME_ID_ByNameId(Name_ids, out pName_IdArr);
                            if (pName_IdArr != null && pName_IdArr.Count() > 0)
                            {
                                PolicySvcRef.P_NAME_ID pName_IdObj = new PolicySvcRef.P_NAME_ID();
                                pName_IdObj = pName_IdArr[0];
                                data.CusName = pName_IdObj.PRENAME + " " + pName_IdObj.NAME + "  " + pName_IdObj.SURNAME;
                                data.IdCardNo = pName_IdObj.IDCARD_NO;
                                data.BirthDate = Utility.dateTimeToString((DateTime)pName_IdObj.BIRTH_DT, "dd/MM/yyyy", "BU");
                                //==== หา Nmaecode ====//
                                NewBisSvcRef.ProcessResult prPolName = new NewBisSvcRef.ProcessResult();
                                data.NameCode = newbisClient.GetNameCodeByNameID(Name_Id, out prPolName);
                                if (prPolName.Successed == false)
                                {
                                    throw new Exception(prPolName.ErrorMessage);
                                }
                            }
                        }
                        var exclusion = GetP_Pol_Exclusion(policyId);
                        if (exclusion.Count() > 0)
                        {
                            List<P_POL_EXCLUSION2_VIEW> pPolExView = new List<P_POL_EXCLUSION2_VIEW>();
                            List<P_POL_EXCLUSION_DETAIL_AND_CAUSE> objtest = new List<P_POL_EXCLUSION_DETAIL_AND_CAUSE>();
                            long? pExclusion_ID_tmp = 0;
                            foreach (DataContract.P_POL_EXCLUSION2 obj in exclusion)
                            {
                                P_POL_EXCLUSION2_VIEW pPolEx = new P_POL_EXCLUSION2_VIEW();
                                pPolEx.EXCLUDE_ID = obj.EXCLUDE_ID == null ? "" : obj.EXCLUDE_ID.ToString();
                                pPolEx.EXCLUDE_TRN_DT = obj.EXCLUDE_TRN_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.EXCLUDE_TRN_DT, "dd/MM/yyyy hh:mi:ss", "BU");
                                pPolEx.EFF_DT = obj.EFF_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.EFF_DT, "dd/MM/yyyy", "BU");
                                pPolEx.REF_DT = obj.REF_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.REF_DT, "dd/MM/yyyy", "BU");
                                pPolEx.Reference = obj.REFERENCE == null ? "" : obj.REFERENCE.ToString();
                                pPolEx.Underwrite_Id = obj.UNDERWRITE_ID == null ? "" : obj.UNDERWRITE_ID.ToString();
                                pPolEx.TMN = obj.TMN == null ? "" : Convert.ToChar(obj.TMN).ToString();
                                if (Convert.ToChar(obj.TMN) == 'N')
                                {
                                    pPolEx.TMN_DESCRIPT = "ไม่ยกเลิก";
                                }
                                else if (Convert.ToChar(obj.TMN) == 'Y')
                                {
                                    pPolEx.TMN_DESCRIPT = "ยกเลิก";
                                }
                                pPolEx.UpdateProcess = obj.UpdateProcess == null ? "" : obj.UpdateProcess.ToString();
                                pPolEx.ENDORSE_DT = obj.ENDORSE_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.ENDORSE_DT, "dd/MM/yyyy", "BU");
                                pPolEx.CANCEL_ENDORSE_DT = obj.CANCEL_ENDORSE_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.CANCEL_ENDORSE_DT, "dd/MM/yyyy", "BU");
                                pPolEx.CANCEL_REFERENCE = obj.CANCEL_REFERENCE == null ? "" : obj.CANCEL_REFERENCE.ToString();
                                char? excTmnCause = null;
                                if (!string.IsNullOrEmpty(obj.EXC_TMN_CAUSE))
                                {
                                    long lexcTmnCause = Convert.ToInt64(obj.EXC_TMN_CAUSE);
                                    excTmnCause = Convert.ToChar(lexcTmnCause);
                                }
                                pPolEx.EXC_TMN_CAUSE = obj.EXC_TMN_CAUSE == null ? "" : Get_ZTB_CONSTANT1("EXCLUDE_TMN_CAUSE", excTmnCause);
                                pPolEx.clmtmneffdt = obj.TMN_EFF_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.TMN_EFF_DT, "dd/MM/yyyy", "BU");
                                EndpointAddress endpointCenter = new EndpointAddress(Configuration["CenterSvcRef"]);
                                CenterSvcRef.CenterServiceClient centerClient = new CenterSvcRef.CenterServiceClient(binding, endpointCenter);
                                CenterSvcRef.User UserColl = new CenterSvcRef.User();
                                CenterSvcRef.ProcessResult pr = new CenterSvcRef.ProcessResult();
                                if (pPolEx.Underwrite_Id != null)
                                {
                                    UserColl = centerClient.getUser(pPolEx.Underwrite_Id, out pr);
                                }
                                else
                                {
                                    if (pPolEx.EXCLUDE_ID == null)
                                    {
                                        UserColl = centerClient.getUser(request.UserId , out pr);
                                    }
                                }
                                if (UserColl != null)
                                {
                                    pPolEx.Underwrite_Name = UserColl.firstName == null ? "" : UserColl.firstName.ToString() + " " + UserColl.lastName.ToString();
                                }

                                pPolExView.Add(pPolEx);
                                P_POL_EXCLUSION_DETAIL_AND_CAUSE[] pPolExclusionDetailAndCause = null;
                                
                                if (obj.EXCLUDE_ID != null)
                                {
                                    pPolExclusionDetailAndCause =  GetP_Pol_Exclusion_Detail(obj.EXCLUDE_ID, Convert.ToChar(obj.TMN));
                                    foreach (var pd in pPolExclusionDetailAndCause)
                                    {
                                        objtest.Add(pd);
                                    }
                                    if (objtest != null)
                                    {
                                        data.P_POL_EXCLUSION_DETAIL_AND_CAUSE = objtest.ToArray();
                                    }
                                   // pExclusion_ID_tmp = obj.EXCLUDE_ID;
                                }
                            }
                            data.P_POL_EXCLUSION2 = pPolExView.OrderByDescending(o => Convert.ToDateTime(o.EXCLUDE_TRN_DT)).ToArray();
                            
                        }
                    }
                }

                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        private DataContract.P_POL_EXCLUSION2[] GetP_Pol_Exclusion(long? Policy_Id)
        {
            long[] policy_ids = { (long)Policy_Id };
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpointPolicy = new EndpointAddress(Configuration["PolicySvcRef"]);
            PolicySvcRef.PolicySvcClient policyClient = new PolicySvcRef.PolicySvcClient(binding, endpointPolicy);
            long[] Exclusion_ids = null;
            PolicySvcRef.ProcessResult prEx = new PolicySvcRef.ProcessResult();
            DataContract.P_POL_EXCLUSION[] pPol_Exclusions = null;
            //prEx = policyClient.GetP_POL_EXCLUSION(Exclusion_ids, policy_ids, out pPol_Exclusions);
            serviceAction.GetP_POL_EXCLUSION(out pPol_Exclusions , Exclusion_ids, policy_ids);
            List<DataContract.P_POL_EXCLUSION2> pPol_Exclusion_Coll2 = new List<DataContract.P_POL_EXCLUSION2>();

            if (prEx.Successed == false)
            {
                throw new Exception(prEx.ErrorMessage);
            }
            else
            {

                if (pPol_Exclusions != null && pPol_Exclusions.Count() > 0)
                {

                    foreach (var p in pPol_Exclusions)
                    {
                        DataContract.P_POL_EXCLUSION2 p2 = new DataContract.P_POL_EXCLUSION2();
                        p2.EXCLUDE_ID = p.EXCLUDE_ID;
                        p2.POLICY_ID = p.POLICY_ID;
                        p2.EXCLUDE_TRN_DT = p.EXCLUDE_TRN_DT;
                        p2.EFF_DT = p.EFF_DT;
                        p2.REF_DT = p.REF_DT;
                        p2.REFERENCE = p.REFERENCE;
                        p2.UNDERWRITE_ID = p.UNDERWRITE_ID;
                        p2.TMN = p.TMN;
                        p2.UPD_DT = p.UPD_DT;
                        p2.UPD_ID = p.UPD_ID;
                        p2.MINI_UPDATE = p.MINI_UPDATE;
                        p2.MINI_DT = p.MINI_DT;
                        p2.TMN_EFF_DT = p.TMN_EFF_DT;

                        if (p.TMN == "Y")
                        {
                            using (PolicySvcRef.PolicySvcClient client1 = new PolicySvcRef.PolicySvcClient())
                            {
                                try
                                {
                                    PolicySvcRef.P_POL_EXCLUSION_TMN[] dtmnArr = null;
                                    long[] Exclude_ids = { (long)p.EXCLUDE_ID };
                                    client1.GetP_POL_EXCLUSION_TMN(Exclude_ids, out dtmnArr);
                                    if (dtmnArr != null && dtmnArr.Count() > 0)
                                    {
                                        p2.EXC_TMN_CAUSE = dtmnArr[0].EXC_TMN_CAUSE.ToString();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }

                        //=== หาข้อมูลการสลักหลัง P_ENDORSE_ID ===//
                        PolicySvcRef.P_ENDORSE_ID[] p_EndorseArr = null;
                        long[] Endorse_IDs = null;
                        policyClient.GetP_ENDORSE_ID(Endorse_IDs, policy_ids, out p_EndorseArr);
                        if (p_EndorseArr != null && p_EndorseArr.Count() > 0)
                        {
                            foreach (PolicySvcRef.P_ENDORSE_ID e in p_EndorseArr)
                            {
                                if (p2.TMN == "Y")
                                {
                                    if (e.ENDORSE_CODE == "080" && e.REFERENCE == p2.REFERENCE)
                                    {
                                        p2.ENDORSE_ID = e.ENDORSE_ID;
                                        p2.ENDORSE_DT = e.ENDORSE_DT;
                                    }
                                    if (e.ENDORSE_CODE == "081")
                                    {
                                        p2.ENDORSE_ID = e.ENDORSE_ID;
                                        p2.CANCEL_ENDORSE_DT = e.ENDORSE_DT;
                                        p2.CANCEL_REFERENCE = e.REFERENCE;
                                    }
                                }
                                else if (p2.TMN == "N")
                                {
                                    if (e.ENDORSE_CODE == "080")
                                    {
                                        p2.ENDORSE_ID = e.ENDORSE_ID;
                                        p2.ENDORSE_DT = e.ENDORSE_DT;
                                    }
                                }
                            }
                        }
                        pPol_Exclusion_Coll2.Add(p2);
                    }
                    //Bind_dtgP_Exclusion();
                }
            }

            return pPol_Exclusion_Coll2.ToArray();
        }

        private string Get_ZTB_CONSTANT1(string COL_NAME, char? CODE1)
        {
            string Description = null;
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpointPolicy = new EndpointAddress(Configuration["PolicySvcRef"]);
            PolicySvcRef.PolicySvcClient policyClient = new PolicySvcRef.PolicySvcClient(binding, endpointPolicy);
            try
            {
                PolicySvcRef.ZTB_CONSTANT1[] ztbArr = null;
                string[] Col_Names = { (string)COL_NAME };
                int[] Codes = { (int)CODE1 };
                policyClient.GetZTB_CONSTANT1(Col_Names, Codes, out ztbArr);
                if (ztbArr != null)
                {
                    Description = ztbArr[0].DESCRIPTION;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Description;
        }

        private P_POL_EXCLUSION_DETAIL_AND_CAUSE[] GetP_Pol_Exclusion_Detail(long? Exclude_Id, char? tmn)
        {
            BasicHttpBinding binding = SetHttpBinding();
            EndpointAddress endpointPolicy = new EndpointAddress(Configuration["PolicySvcRef"]);
            EndpointAddress endpointNewbis = new EndpointAddress(Configuration["NewBISSvcRef"]);
            PolicySvcRef.PolicySvcClient policyClient = new PolicySvcRef.PolicySvcClient(binding, endpointPolicy);
            NewBisSvcRef.NewBISSvcClient newbisClient = new NewBisSvcRef.NewBISSvcClient(binding, endpointNewbis);
            long[] Exclusion_ids = { (long)Exclude_Id };
            long[] Exclusion_Det_ids = null;
            PolicySvcRef.ProcessResult prExDet = new PolicySvcRef.ProcessResult();
            PolicySvcRef.P_POL_EXCLUSION_DETAIL[] pPol_Exclusion_Details = null;
            prExDet = policyClient.GetP_POL_EXCLUSION_DETAIL(Exclusion_Det_ids, Exclusion_ids, out pPol_Exclusion_Details);
            List<P_POL_EXCLUSION_DETAIL_AND_CAUSE> pPol_Exclusion_det_Coll = new List<P_POL_EXCLUSION_DETAIL_AND_CAUSE>();
            if (prExDet.Successed == false)
            {
                throw new Exception(prExDet.ErrorMessage);
            }
            else
            {
                if (pPol_Exclusion_Details != null && pPol_Exclusion_Details.Count() > 0)
                {

                    NewBisSvcRef.ProcessResult pr1 = new NewBisSvcRef.ProcessResult();
                    NewBisSvcRef.AUTB_DATADIC_DET_COLLECTION dataColl = new NewBisSvcRef.AUTB_DATADIC_DET_COLLECTION();
                    try
                    {
                        dataColl = newbisClient.GetAutbDataDicDets("EXCLUDE_CAUSE", out pr1);
                        NewBisSvcRef.AUTB_DATADIC_DET_COLLECTION dobj = new NewBisSvcRef.AUTB_DATADIC_DET_COLLECTION();
                        if (pr1.Successed == false)
                        {
                            //if (dataColl != null && dataColl.Count > 0)
                            //{
                            //    //foreach (NewBisSvcRef.AUTB_DATADIC_DET p in dataColl)
                            //    //{
                            //    //    //if (p.CODE == dr.EXCLUDE_CAUSE)
                            //    //    //{
                            //    //    //    dr.Exclude_Cause_Detail = p.DESCRIPTION;
                            //    //    //    break;
                            //    //    //}
                            //    //}

                            //}
                            throw new Exception("ไม่สามาค้นหาข้อมูลสุขภาพ newbisClient.GetAutbDataDicDets " + pr1.ErrorMessage);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                    foreach (PolicySvcRef.P_POL_EXCLUSION_DETAIL obj in pPol_Exclusion_Details)
                    {
                        string DExclude_tmn_dt = "";
                        char? DExclude_tmn_Cause = null;
                        string DExclude_tmn_Cause_Detail = "";
                        //==== หาสาเหตุการยกเลิก ====//
                        if (Convert.ToChar(obj.TMN) == 'Y')
                        {
                            using (PolicySvcRef.PolicySvcClient client1 = new PolicySvcRef.PolicySvcClient())
                            {
                                try
                                {
                                    PolicySvcRef.P_POL_EXCLUSION_DETTMN[] dtmnArr = null;
                                    long[] Exclude_det_ids = { (long)obj.EXCLUDE_DET_ID };
                                    client1.GetP_POL_EXCLUSION_DETTMN(Exclude_det_ids,out dtmnArr);
                                    if (dtmnArr != null && dtmnArr.Count() > 0)
                                    {
                                        char? ExcDet_tmn_cause = (char?)dtmnArr[0].EXCDET_TMN_CAUSE;
                                        DExclude_tmn_dt = dtmnArr[0].TMN_DT == null ? "" : Utility.dateTimeToString((DateTime)dtmnArr[0].TMN_DT, "dd/MM/yyyy", "BU");
                                        DExclude_tmn_Cause = ExcDet_tmn_cause;
                                        DExclude_tmn_Cause_Detail = ExcDet_tmn_cause == null ? "" : Get_ZTB_CONSTANT1("EXCLUDE_TMN_CAUSE", ExcDet_tmn_cause);
                                    }
                                    else
                                    {
                                        //foreach (P_POL_EXCLUSION_DETTMN d1 in pPol_Exclusion_Det_tmn_Coll)
                                        //{
                                        //    if (d1.EXCLUDE_DET_ID == obj.EXCLUDE_DET_ID)
                                        //    {
                                        //        DExclude_tmn_dt = Utility.dateTimeToString((DateTime)DateTime.Now, "dd/MM/yyyy", "BU");
                                        //        DExclude_tmn_Cause = d1.EXCDET_TMN_CAUSE;
                                        //        DExclude_tmn_Cause_Detail = d1.EXCDET_TMN_CAUSE == null ? "" : Get_ZTB_CONSTANT1("EXCLUDE_TMN_CAUSE", d1.EXCDET_TMN_CAUSE);
                                        //    }
                                        //}
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Cannot connect service PolicySvcRef.PolicySvcClient GetP_POL_EXCLUSION_DETTMN " + ex.Message.ToString());
                                }
                            }
                        }

                        pPol_Exclusion_det_Coll.Add(new P_POL_EXCLUSION_DETAIL_AND_CAUSE
                        {
                            EXCLUDE_DET_ID = obj.EXCLUDE_DET_ID == null ? null : obj.EXCLUDE_DET_ID,
                            EXCLUDE_ID = obj.EXCLUDE_ID == null ? null : obj.EXCLUDE_ID,
                            EXCLUDE = obj.EXCLUDE == null ? "" : obj.EXCLUDE.ToString(),
                            ADMIT_DT_TEXT = obj.ADMIT_DT == null ? "" : Utility.dateTimeToString((DateTime)obj.ADMIT_DT, "dd/MM/yyyy", "BU"),
                            ADMIT_DT = obj.ADMIT_DT,
                            EXCLUDE_CAUSE = obj.EXCLUDE_CAUSE == null ? "" : obj.EXCLUDE_CAUSE.ToString(),
                            ENDORSE_PRINTING = obj.ENDORSE_PRINTING == null ? null : obj.ENDORSE_PRINTING,
                            ENDORSE_PRINTING_TEXT = obj.ENDORSE_PRINTING == null ? "" : Convert.ToChar(obj.ENDORSE_PRINTING).ToString(),
                            TMN = obj.TMN == null ? null : obj.TMN,
                            TMN_TEXT = obj.TMN == null ? null : Convert.ToChar(obj.TMN).ToString(),
                            Exclude_Cause_Detail = dataColl.Where(e => e.CODE == obj.EXCLUDE_CAUSE)?.Select(s => s.DESCRIPTION).FirstOrDefault(),
                            Exclude_tmn_dt = DExclude_tmn_dt,
                            Exclude_tmn_Cause = DExclude_tmn_Cause == null ? "" : DExclude_tmn_Cause.ToString(),
                            Exclude_tmn_Cause_Detail = DExclude_tmn_Cause_Detail
                        });
                    }


                    pPol_Exclusion_det_Coll.OrderBy(c => c.TMN).ThenBy(e => e.EXCLUDE_DET_ID == null ? 1 : 0).ThenBy(o => o.EXCLUDE_DET_ID);


                    //Bind_dtgP_Exclusion_Det(tmn);
                }
            }
            return pPol_Exclusion_det_Coll.ToArray();
            #region "Backup Code"
            //if (prExDet.Successed == false)
            //{
            //    throw new Exception(prEx.ErrorMessage);
            //}
            //else
            //{
            //    dtgP_Exclusion_Detail.AutoGenerateColumns = false;
            //    dtgP_Exclusion_Detail.DataSource = null;

            //    if (pPol_Exclusion_Details != null && pPol_Exclusion_Details.Count() > 0)
            //    {
            //        PolicySvcRef.P_POL_EXCLUSION_DETAIL pPol_Exclusion_Det_Obj = new PolicySvcRef.P_POL_EXCLUSION_DETAIL();
            //        pPol_Exclusion_Det_Obj = null;

            //        int x = 0;
            //        for (int i = 0; i < pPol_Exclusion_Details.Length; i++)
            //        {
            //            pPol_Exclusion_Det_Obj = pPol_Exclusion_Details[i];
            //            dtgP_Exclusion_Detail.Rows.Add(pPol_Exclusion_Details[i]);
            //            x = x + 1;
            //            dtgP_Exclusion_Detail.Rows[i].Cells["clmSeq_Det"].Value = x.ToString();
            //            dtgP_Exclusion_Detail.Rows[i].Cells["clmExclude_Det"].Value = pPol_Exclusion_Det_Obj.EXCLUDE;
            //            dtgP_Exclusion_Detail.Rows[i].Cells["clmAdmit_Dt_Det"].Value = Utility.dateTimeToString((DateTime)pPol_Exclusion_Det_Obj.ADMIT_DT, "dd/MM/yyyy", "BU");
            //            dtgP_Exclusion_Detail.Rows[i].Cells["clmExclude_Cause_Det"].Value = pPol_Exclusion_Det_Obj.EXCLUDE_CAUSE;
            //            dtgP_Exclusion_Detail.Rows[i].Cells["clmEndose_Printing_Det"].Value = pPol_Exclusion_Det_Obj.ENDORSE_PRINTING;
            //            dtgP_Exclusion_Detail.Rows[i].Cells["clmTMN_Det"].Value = pPol_Exclusion_Det_Obj.TMN;
            //           // dtgP_Exclusion_Detail.Rows.Add(pPol_Exclusion_Details[i]);
            //        }

            //    }
            //}
            //if (client.State != System.ServiceModel.CommunicationState.Closed)
            //{
            //    client.Close();
            //}
            #endregion

        }

        [HttpPost]
        [Route("SaveData")]
        public IActionResult SaveData([FromBody] SaveDataModel model)
        {
            try
            {
                //====== หาข้อมูลกรมธรรม์ =====//
                string Status = null;
                int? Pol_yr = null;
                int? Pol_lt = null;
                DateTime? Tran_dt = null;
                // เพิ่ม 11/05/2556 Set ค่า วันที่ Endorse_Tran_dt //
                using (NewBisSvcRef.NewBISSvcClient client = new NewBisSvcRef.NewBISSvcClient())
                {
                    try
                    {
                        DateTime sysdate;
                        client.GetSysDate(out sysdate);
                        Tran_dt = sysdate;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }

                using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                {
                    try
                    {
                        long[] policy_ids = { (long)Convert.ToInt64(model.PolicyId) };
                        PolicySvcRef.P_LIFE_ID[] pLifeArr = null;
                        client.GetP_LIFE_ID(policy_ids, out pLifeArr);
                        if (pLifeArr != null && pLifeArr.Count() > 0)
                        {
                            PolicySvcRef.P_LIFE_ID pLifeObj = new PolicySvcRef.P_LIFE_ID();
                            pLifeObj = pLifeArr[0];
                            if (pLifeObj.POLICY_ID != null)
                            {
                                Status = pLifeObj.STATUS;
                                Pol_yr = pLifeObj.POL_YR;
                                Pol_lt = pLifeObj.POL_LT;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }


                #region "add & edit record"
                if (model.P_POL_EXCLUSION2 != null && model.P_POL_EXCLUSION2.Count() > 0)
                {
                    char Save_Flg = 'Y';
                    List<DataContract.P_POL_EXCLUSION2> pPol_Exclusion_Coll2 = new List<DataContract.P_POL_EXCLUSION2>();
                    if (model.P_POL_EXCLUSION2.Count() > 0)
                    {
                        foreach (var x in model.P_POL_EXCLUSION2)
                        {
                            int? tmn = null;
                            if (!string.IsNullOrEmpty(x.TMN))
                            {
                                char cTmn = Convert.ToChar(x.TMN);
                                tmn = Convert.ToInt32(cTmn);
                            }
                            pPol_Exclusion_Coll2.Add(new DataContract.P_POL_EXCLUSION2
                            {
                                CANCEL_ENDORSE_DT = string.IsNullOrEmpty(x.CANCEL_ENDORSE_DT) ? null : (DateTime?)Convert.ToDateTime(x.CANCEL_ENDORSE_DT),
                                CANCEL_REFERENCE = x.CANCEL_REFERENCE,
                                EFF_DT = string.IsNullOrEmpty(x.EFF_DT) ? null : Utility.StringToDateTime(x.EFF_DT, "BU"),
                                ENDORSE_DT = string.IsNullOrEmpty(x.ENDORSE_DT) ? null : Utility.StringToDateTime(x.ENDORSE_DT, "BU"),
                                ENDORSE_ID = string.IsNullOrEmpty(x.ENDORSE_ID) ? null : (long?)Convert.ToInt64(x.ENDORSE_ID),
                                EXCLUDE_ID = string.IsNullOrEmpty(x.EXCLUDE_ID) ? null : (long?)Convert.ToInt64(x.EXCLUDE_ID),
                                EXCLUDE_TRN_DT = string.IsNullOrEmpty(x.EXCLUDE_TRN_DT) ? null : Utility.StringToDateTime(x.EXCLUDE_TRN_DT, "BU"),
                                EXC_TMN_CAUSE = string.IsNullOrEmpty(x.EXC_TMN_CAUSE) ? null : x.EXC_TMN_CAUSE,
                                REFERENCE = string.IsNullOrEmpty(x.Reference) ? null : x.Reference,
                                REF_DT = string.IsNullOrEmpty(x.REF_DT) ? null : Utility.StringToDateTime(x.REF_DT, "BU"),
                                TMN = x.TMN,
                                UpdateProcess = string.IsNullOrEmpty(x.UpdateProcess) ? null : x.UpdateProcess,
                                // POLICY_ID = string.IsNullOrEmpty()
                            });
                        }
                    }

                    //**** ตรวจสอบค่า null ****//
                    foreach (DataContract.P_POL_EXCLUSION2 p in pPol_Exclusion_Coll2)
                    {
                        if (p.EFF_DT.ToString() == null || p.EFF_DT.ToString() == "")
                        {
                            Save_Flg = 'N';
                            throw new Exception("กรุณาระบุวันที่มีผล");
                            break;
                        }
                        else if (p.REF_DT.ToString() == null || p.REF_DT.ToString() == "")
                        {
                            Save_Flg = 'N';
                            throw new Exception("กรุณาระบุวันที่อ้างอิง");
                            break;
                        }
                        else if (p.REFERENCE == null || p.REFERENCE == "")
                        {
                            Save_Flg = 'N';
                            throw new Exception("กรุณาระบุเลขที่สลักหลัง");
                            break;
                        }
                        else if ((p.ENDORSE_DT.ToString() == null || p.ENDORSE_DT.ToString() == "") && p.TMN == "N")
                        {
                            Save_Flg = 'N';
                            throw new Exception("กรุณาระบุวันที่สลักหลัง");
                            break;
                        }
                        else if ((p.CANCEL_ENDORSE_DT.ToString() == null || p.CANCEL_ENDORSE_DT.ToString() == "") && p.TMN == "Y")
                        {
                            Save_Flg = 'N';
                            throw new Exception("กรุณาระบุวันที่สลักหลัง(ยกเลิก)");
                            break;
                        }
                        else if ((p.CANCEL_REFERENCE == null || p.CANCEL_REFERENCE == "") && p.TMN == "Y")
                        {
                            Save_Flg = 'N';
                            throw new Exception("กรุณาระบุเลขที่สลักหลัง(ยกเลิก)");
                            break;
                        }
                    }

                    if (Save_Flg == 'Y')
                    {
                        List<PolicySvcRef.P_POL_EXCLUSION_DETAIL> pPol_Exclusion_det_Coll = new List<PolicySvcRef.P_POL_EXCLUSION_DETAIL>();
                        if (model.P_POL_EXCLUSION_DETAIL_AND_CAUSE.Count() > 0)
                        {
                            foreach (var x in model.P_POL_EXCLUSION_DETAIL_AND_CAUSE)
                            {
                                int? tmnDet = null;
                                if (!string.IsNullOrEmpty(x.TMN_TEXT))
                                {
                                    char cTmnDet = Convert.ToChar(x.TMN_TEXT);
                                    tmnDet = Convert.ToInt32(cTmnDet);
                                }
                                pPol_Exclusion_det_Coll.Add(new PolicySvcRef.P_POL_EXCLUSION_DETAIL
                                {
                                    ADMIT_DT = string.IsNullOrEmpty(x.ADMIT_DT_TEXT) ? null : (DateTime?)Convert.ToDateTime(x.ADMIT_DT_TEXT),
                                    EXCLUDE = string.IsNullOrEmpty(x.EXCLUDE) ? null : x.EXCLUDE,
                                    EXCLUDE_ID = x.EXCLUDE_ID,
                                    EXCLUDE_CAUSE = string.IsNullOrEmpty(x.EXCLUDE_CAUSE) ? null : x.EXCLUDE_CAUSE,
                                    EXCLUDE_DET_ID = x.EXCLUDE_DET_ID,
                                    ENDORSE_PRINTING = x.ENDORSE_PRINTING == null ? null : x.ENDORSE_PRINTING,
                                    TMN = tmnDet
                                });
                            }
                        }

                        foreach (DataContract.P_POL_EXCLUSION2 p in pPol_Exclusion_Coll2)
                        {
                            DataContract.P_POL_EXCLUSION ExcludeRef = new DataContract.P_POL_EXCLUSION();
                            PolicySvcRef.P_POL_EXCLUSION_DETAIL ExcludeDetRef = new PolicySvcRef.P_POL_EXCLUSION_DETAIL();
                            PolicySvcRef.P_POL_EXCLUSION_TMN ExcludeRef_tmn = new PolicySvcRef.P_POL_EXCLUSION_TMN();
                            PolicySvcRef.P_POL_EXCLUSION_DETTMN ExcludeDetRef_tmn = new PolicySvcRef.P_POL_EXCLUSION_DETTMN();

                            #region "add new record"
                            int? seq = 0;
                            //======= Update data ลง P_POL_EXCLUDESION ======//
                            if (p.UpdateProcess == "INSERT")
                            {
                                using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                                {
                                    try
                                    {
                                        ExcludeRef.POLICY_ID = Convert.ToInt64(model.PolicyId);
                                        ExcludeRef.EXCLUDE_TRN_DT = Tran_dt; //p.EXCLUDE_TRN_DT;
                                        ExcludeRef.EFF_DT = p.EFF_DT;
                                        ExcludeRef.REF_DT = p.REF_DT;
                                        ExcludeRef.REFERENCE = p.REFERENCE;
                                        ExcludeRef.UNDERWRITE_ID = p.UNDERWRITE_ID == null ? model.UserID : p.UNDERWRITE_ID;
                                        ExcludeRef.TMN = p.TMN;
                                        ExcludeRef.UPD_DT = DateTime.Now;
                                        ExcludeRef.UPD_ID = model.UserID;
                                        ExcludeRef.TMN_EFF_DT = p.TMN_EFF_DT;
                                        serviceAction.AddP_POL_EXCLUSION(ref ExcludeRef);
                                        //var pr = client.AddP_POL_EXCLUSION(ref ExcludeRef);

                                        if (ExcludeRef.EXCLUDE_ID != null && p.TMN != "Y")
                                        {
                                            //======= Update data ลง P_POL_EXCLUDESION_DETAIL ======//
                                            foreach (PolicySvcRef.P_POL_EXCLUSION_DETAIL p2 in pPol_Exclusion_det_Coll)
                                            {
                                                if (p2.EXCLUDE_CAUSE != null)
                                                {
                                                    ExcludeDetRef.EXCLUDE_ID = ExcludeRef.EXCLUDE_ID;
                                                    ExcludeDetRef.EXCLUDE = p2.EXCLUDE;
                                                    ExcludeDetRef.ADMIT_DT = p2.ADMIT_DT;
                                                    ExcludeDetRef.EXCLUDE_CAUSE = p2.EXCLUDE_CAUSE;
                                                    ExcludeDetRef.ENDORSE_PRINTING = p2.ENDORSE_PRINTING;
                                                    ExcludeDetRef.TMN = p2.TMN;
                                                    client.AddP_POL_EXCLUSION_DETAIL(ref ExcludeDetRef);
                                                }
                                            }

                                            //===== เพิ่มข้อมูลใน P_EMDORSE_ID =====//

                                            PolicySvcRef.P_ENDORSE_ID pEndorseRef = new P_ENDORSE_ID();
                                            pEndorseRef.POLICY_ID = Convert.ToInt64(model.PolicyId);
                                            pEndorseRef.ENDORSE_TRN_DT = Tran_dt; //p.EXCLUDE_TRN_DT;
                                            pEndorseRef.ENDORSE_CODE = "080";
                                            pEndorseRef.REFERENCE = p.REFERENCE;
                                            pEndorseRef.ENDORSE_DT = p.ENDORSE_DT;
                                            pEndorseRef.STATUS = Status;
                                            pEndorseRef.POL_YR = Pol_yr;
                                            pEndorseRef.POL_LT = Pol_lt;
                                            pEndorseRef.FSYSTEM_DT = DateTime.Now;
                                            pEndorseRef.UPD_ID = model.UserID;
                                            pEndorseRef.TMN = 'N';
                                            client.AddP_ENDORSE_ID(ref pEndorseRef);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message.ToString());
                                    }
                                }
                            }
                            #endregion
                            #region "edit record"
                            else if (p.UpdateProcess == "EDIT")
                            {
                                using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                                {
                                    try
                                    {

                                        if (p.TMN == "Y")
                                        {
                                            //--- TMN P_POL_EXCLUSION ---//
                                            ExcludeRef.EXCLUDE_ID = p.EXCLUDE_ID;
                                            ExcludeRef.POLICY_ID = Convert.ToInt64(model.PolicyId);
                                            ExcludeRef.EXCLUDE_TRN_DT = p.EXCLUDE_TRN_DT;
                                            ExcludeRef.EFF_DT = p.EFF_DT;
                                            ExcludeRef.REF_DT = p.REF_DT;
                                            ExcludeRef.REFERENCE = p.REFERENCE;
                                            ExcludeRef.UNDERWRITE_ID = p.UNDERWRITE_ID;
                                            ExcludeRef.TMN = p.TMN;
                                            ExcludeRef.UPD_DT = DateTime.Now;
                                            ExcludeRef.TMN_EFF_DT = p.TMN_EFF_DT;
                                            ExcludeRef.UPD_ID = model.UserID;

                                            serviceAction.EditP_POL_EXCLUSION(ref ExcludeRef);
                                            //client.EditP_POL_EXCLUSION(ref ExcludeRef);

                                            ExcludeRef_tmn.EXCLUDE_ID = p.EXCLUDE_ID;
                                            ExcludeRef_tmn.TMN_DT = DateTime.Now;
                                            ExcludeRef_tmn.TMN_ID = model.UserID;

                                            ExcludeRef_tmn.EXC_TMN_CAUSE = Convert.ToInt32(p.EXC_TMN_CAUSE);
                                            //serviceAction.AddP_POL_EXCLUSION_TMN(ref ExcludeRef_tmn);
                                            client.AddP_POL_EXCLUSION_TMN(ref ExcludeRef_tmn);

                                            //===== เพิ่มข้อมูลใน P_EMDORSE_ID =====//
                                            PolicySvcRef.P_ENDORSE_ID pEndorseRef = new P_ENDORSE_ID();
                                            pEndorseRef.POLICY_ID = Convert.ToInt64(model.PolicyId);
                                            pEndorseRef.ENDORSE_TRN_DT = Tran_dt; //DateTime.Now;
                                            pEndorseRef.ENDORSE_CODE = "081";
                                            pEndorseRef.REFERENCE = p.CANCEL_REFERENCE;
                                            pEndorseRef.ENDORSE_DT = p.CANCEL_ENDORSE_DT;
                                            pEndorseRef.STATUS = Status;
                                            pEndorseRef.POL_YR = Pol_yr;
                                            pEndorseRef.POL_LT = Pol_lt;
                                            pEndorseRef.FSYSTEM_DT = DateTime.Now;
                                            pEndorseRef.UPD_ID = model.UserID;
                                            pEndorseRef.TMN = 'N';
                                            client.AddP_ENDORSE_ID(ref pEndorseRef);
                                        }
                                        else
                                        {
                                            if (p.ENDORSE_ID != null)//if (ExcludeRef.EXCLUDE_ID != null)
                                            {
                                                //======= Update data ลง P_POL_EXCLUDESION_DETAIL ======//
                                                foreach (P_POL_EXCLUSION_DETAIL_AND_CAUSE p2 in pPol_Exclusion_det_Coll)
                                                {
                                                    if (p2.EXCLUDE_ID == p.EXCLUDE_ID)
                                                    {
                                                        if (p2.EXCLUDE_DET_ID == null && p.TMN != "Y")
                                                        {
                                                            ExcludeDetRef.EXCLUDE_ID = p.EXCLUDE_ID;//ExcludeRef.EXCLUDE_ID;
                                                            ExcludeDetRef.EXCLUDE = p2.EXCLUDE;
                                                            ExcludeDetRef.ADMIT_DT = p2.ADMIT_DT;
                                                            ExcludeDetRef.EXCLUDE_CAUSE = p2.EXCLUDE_CAUSE;
                                                            ExcludeDetRef.ENDORSE_PRINTING = p2.ENDORSE_PRINTING;
                                                            ExcludeDetRef.TMN = p2.TMN;
                                                            client.AddP_POL_EXCLUSION_DETAIL(ref ExcludeDetRef);
                                                        }
                                                        else
                                                        {
                                                            if (p2.EXCLUDE_DET_ID != null)
                                                            {
                                                                //List<PolicySvcRef.P_POL_EXCLUSION_DETTMN> pPol_Exclusion_Det_tmn_Coll = new List<PolicySvcRef.P_POL_EXCLUSION_DETTMN>();
                                                                //if ()
                                                                //{

                                                                //}

                                                                ExcludeDetRef.EXCLUDE_DET_ID = p2.EXCLUDE_DET_ID;
                                                                ExcludeDetRef.EXCLUDE_ID = p2.EXCLUDE_ID;
                                                                ExcludeDetRef.EXCLUDE = p2.EXCLUDE;
                                                                ExcludeDetRef.ADMIT_DT = p2.ADMIT_DT;
                                                                ExcludeDetRef.EXCLUDE_CAUSE = p2.EXCLUDE_CAUSE;
                                                                ExcludeDetRef.ENDORSE_PRINTING = p2.ENDORSE_PRINTING;
                                                                ExcludeDetRef.TMN = p2.TMN;
                                                                client.EditP_POL_EXCLUSION_DETAIL(ref ExcludeDetRef);
                                                                if (p2.TMN == 'Y')
                                                                {
                                                                    //ExcludeDetRef_tmn.EXCLUDE_DET_ID = p2.EXCLUDE_DET_ID;
                                                                    //ExcludeDetRef_tmn.TMN_DT = DateTime.Now;
                                                                    //ExcludeDetRef_tmn.TMN_ID = UserID;
                                                                    foreach (P_POL_EXCLUSION_DETAIL_AND_CAUSE d1 in pPol_Exclusion_det_Coll)
                                                                    {
                                                                        if (d1.EXCLUDE_DET_ID == p2.EXCLUDE_DET_ID)
                                                                        {
                                                                            ExcludeDetRef_tmn.EXCLUDE_DET_ID = p2.EXCLUDE_DET_ID;
                                                                            ExcludeDetRef_tmn.TMN_DT = DateTime.Now;
                                                                            ExcludeDetRef_tmn.TMN_ID = model.UserID;
                                                                            ExcludeDetRef_tmn.EXCDET_TMN_CAUSE = d1.TMN;
                                                                            client.AddP_POL_EXCLUSION_DETTMN(ref ExcludeDetRef_tmn);
                                                                        }
                                                                    }
                                                                    //client.AddP_POL_EXCLUSION_DETTMN(ref ExcludeDetRef_tmn);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message.ToString());
                                    }
                                }
                            }
                            #endregion
                        }
                        #region "ปรับปรุงข้อมูลโครงสร้างเก่า"
                        if (pPol_Exclusion_Coll2 != null && pPol_Exclusion_Coll2.Count() > 0 && model.ChannelType.ToString() != "GM" && model.ChannelType.ToString() != "HL")
                        {
                            char P_Exclusion_Action = 'N';
                            foreach (DataContract.P_POL_EXCLUSION2 p in pPol_Exclusion_Coll2)
                            {
                                PolicySvcRef.P_POL_EXCLUSION ExcludeRef = new PolicySvcRef.P_POL_EXCLUSION();
                                PolicySvcRef.P_POL_EXCLUSION_DETAIL ExcludeDetRef = new PolicySvcRef.P_POL_EXCLUSION_DETAIL();
                                PolicySvcRef.P_POL_EXCLUSION_TMN ExcludeRef_tmn = new PolicySvcRef.P_POL_EXCLUSION_TMN();
                                PolicySvcRef.P_POL_EXCLUSION_DETTMN ExcludeDetRef_tmn = new PolicySvcRef.P_POL_EXCLUSION_DETTMN();

                                int? seq = 0;
                                using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                                {
                                    try
                                    {
                                        if (p.TMN != "Y")
                                        {
                                            P_Exclusion_Action = 'Y';

                                            //==== ลบข้อมูลโครงสร้างเก่า ====//
                                            int[] e_Seqs = null;
                                            string[] Policys = { (string)model.txtPolicyNo };
                                            PolicySvcRef.P_EXCLUSION[] pExDelArr = null;
                                            client.GetP_EXCLUSION(Policys, e_Seqs, out pExDelArr);
                                            if (pExDelArr != null && pExDelArr.Count() > 0)
                                            {
                                                foreach (PolicySvcRef.P_EXCLUSION pe1 in pExDelArr)
                                                {
                                                    PolicySvcRef.P_EXCLUSION OldExcludeRef_Del = new PolicySvcRef.P_EXCLUSION();
                                                    OldExcludeRef_Del.POLICY = pe1.POLICY;
                                                    OldExcludeRef_Del.SEQ = pe1.SEQ;
                                                    client.RemoveP_EXCLUSION(OldExcludeRef_Del);
                                                }
                                            }

                                            //==== เพิ่มข้อมูลโครงสร้างเก่า ====//
                                            foreach (PolicySvcRef.P_POL_EXCLUSION_DETAIL p2 in pPol_Exclusion_det_Coll)
                                            {
                                                if (p2.EXCLUDE_CAUSE != null)
                                                {
                                                    if (p2.TMN != 'Y')
                                                    {
                                                        seq = seq + 1;
                                                        PolicySvcRef.P_EXCLUSION OldExcludeRef = new PolicySvcRef.P_EXCLUSION();
                                                        OldExcludeRef.POLICY = model.txtPolicyNo;
                                                        OldExcludeRef.SEQ = seq;
                                                        OldExcludeRef.EXCLUDE_TRN_DT = Tran_dt; //p.EXCLUDE_TRN_DT;
                                                        OldExcludeRef.EFF_DT = p.EFF_DT;
                                                        OldExcludeRef.REF_DT = p.REF_DT;
                                                        OldExcludeRef.EFF_REF = p.REFERENCE;
                                                        OldExcludeRef.EXCLUDE = p2.EXCLUDE;
                                                        OldExcludeRef.FSYSTEM_DT = DateTime.Now;
                                                        OldExcludeRef.UPD_DT = DateTime.Now;
                                                        OldExcludeRef.UPD_ID = model.UserID;
                                                        OldExcludeRef.ENDORSE = p2.ENDORSE_PRINTING;
                                                        client.AddP_EXCLUSION(ref OldExcludeRef);
                                                    }
                                                }
                                            }

                                            if (p.UpdateProcess == "INSERT")
                                            {
                                                //===== เพิ่ม-แก้ไข ข้อมูลใน P_ENDORSE ====//
                                                string[] Policy_Nos = { (string)model.PolicyId };
                                                string[] Endorse_Codes = { (string)"080" };
                                                PolicySvcRef.P_ENDORSE pPEndorse_Ref = new PolicySvcRef.P_ENDORSE();
                                                pPEndorse_Ref.POLICY = model.txtPolicyNo;
                                                pPEndorse_Ref.ENDORSE_TRN_DT = Tran_dt; //p.EXCLUDE_TRN_DT;
                                                pPEndorse_Ref.ENDORSE_CODE = "080";
                                                pPEndorse_Ref.REFERENCE = p.REFERENCE;
                                                pPEndorse_Ref.ENDORSE_DT = p.ENDORSE_DT;
                                                pPEndorse_Ref.STATUS = Status;
                                                pPEndorse_Ref.POL_YR = Pol_yr;
                                                pPEndorse_Ref.POL_LT = Pol_lt;
                                                pPEndorse_Ref.ENDORSE_FLG = "";
                                                pPEndorse_Ref.REMARK = "";
                                                pPEndorse_Ref.FSYSTEM_DT = DateTime.Now;
                                                pPEndorse_Ref.UPD_DT = DateTime.Now;
                                                pPEndorse_Ref.UPD_ID = model.UserID;
                                                client.AddP_ENDORSE(ref pPEndorse_Ref);
                                            }

                                        }
                                        else
                                        {
                                            if (p.UpdateProcess == "EDIT")
                                            {
                                                if (P_Exclusion_Action != 'Y')
                                                {
                                                    //==== ลบข้อมูลโครงสร้างเก่า P_EXCLUSION ====// 
                                                    int[] e_Seqs = null;
                                                    string[] Policys = { (string)model.txtPolicyNo };
                                                    PolicySvcRef.P_EXCLUSION[] pExDelArr = null;
                                                    client.GetP_EXCLUSION(Policys, e_Seqs, out pExDelArr);
                                                    if (pExDelArr != null && pExDelArr.Count() > 0)
                                                    {
                                                        foreach (PolicySvcRef.P_EXCLUSION pe1 in pExDelArr)
                                                        {
                                                            PolicySvcRef.P_EXCLUSION OldExcludeRef_Del = new PolicySvcRef.P_EXCLUSION();
                                                            OldExcludeRef_Del.POLICY = pe1.POLICY;
                                                            OldExcludeRef_Del.SEQ = pe1.SEQ;
                                                            client.RemoveP_EXCLUSION(OldExcludeRef_Del);
                                                        }
                                                    }
                                                }

                                                //===== เพิ่มข้อมูลยกเลิกใน P_ENDORSE [Endorse_Codes = "081"] ====//
                                                PolicySvcRef.P_ENDORSE pPEndorse_Ref_TMN = new PolicySvcRef.P_ENDORSE();
                                                pPEndorse_Ref_TMN.POLICY = model.txtPolicyNo;
                                                pPEndorse_Ref_TMN.ENDORSE_TRN_DT = Tran_dt;  //DateTime.Now;
                                                pPEndorse_Ref_TMN.ENDORSE_CODE = "081";
                                                pPEndorse_Ref_TMN.REFERENCE = p.CANCEL_REFERENCE;
                                                pPEndorse_Ref_TMN.ENDORSE_DT = p.CANCEL_ENDORSE_DT;
                                                pPEndorse_Ref_TMN.STATUS = Status;
                                                pPEndorse_Ref_TMN.POL_YR = Pol_yr;
                                                pPEndorse_Ref_TMN.POL_LT = Pol_lt;
                                                pPEndorse_Ref_TMN.ENDORSE_FLG = "";
                                                pPEndorse_Ref_TMN.REMARK = "";
                                                pPEndorse_Ref_TMN.FSYSTEM_DT = DateTime.Now;
                                                pPEndorse_Ref_TMN.UPD_DT = DateTime.Now;
                                                pPEndorse_Ref_TMN.UPD_ID = model.UserID;
                                                client.AddP_ENDORSE(ref pPEndorse_Ref_TMN);
                                            }
                                            //===== ลบข้อมูลใน P_ENDORSE [Endorse_Codes = "080"] ====//

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message.ToString());
                                    }
                                }

                            }
                        }
                        #endregion
                    }
                }
                #endregion
                #region "remove record ตัดทิ้งไปก่อน"
                //if (deleteExclude_Coll != null && deleteExclude_Coll.Count() > 0)
                //{
                //    foreach (P_POL_EXCLUSION2 p2 in deleteExclude_Coll)
                //    {
                //        if (p2.EXCLUDE_ID != null)
                //        {
                //            using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                //            {
                //                try
                //                {
                //                    PolicySvcRef.P_POL_EXCLUSION ExcludeObj = new PolicySvcRef.P_POL_EXCLUSION();
                //                    ExcludeObj.EXCLUDE_ID = p2.EXCLUDE_ID;
                //                    client.RemoveP_POL_EXCLUSION(ExcludeObj);
                //                }
                //                catch (Exception ex)
                //                {
                //                    throw new Exception(ex.Message.ToString());
                //                }
                //            }
                //        }
                //    }
                //    foreach (P_POL_EXCLUSION_DETAIL p2 in deleteExclude_Detail_Coll)
                //    {
                //        if (p2.EXCLUDE_DET_ID != null)
                //        {
                //            using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                //            {
                //                try
                //                {
                //                    PolicySvcRef.P_POL_EXCLUSION_DETAIL ExcludeDetailObj = new PolicySvcRef.P_POL_EXCLUSION_DETAIL();
                //                    ExcludeDetailObj.EXCLUDE_DET_ID = p2.EXCLUDE_DET_ID;
                //                    client.RemoveP_POL_EXCLUSION_DETAIL(ExcludeDetailObj);
                //                }
                //                catch (Exception ex)
                //                {
                //                    throw new Exception(ex.Message.ToString());
                //                }
                //            }
                //        }
                //    }
                //}
                //// ลบเฉพาะ Detail
                //foreach (P_POL_EXCLUSION_DETAIL p2 in deleteExclude_Detail_Coll)
                //{
                //    if (p2.EXCLUDE_DET_ID != null)
                //    {
                //        using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                //        {
                //            try
                //            {
                //                PolicySvcRef.P_POL_EXCLUSION_DETAIL ExcludeDetailObj = new PolicySvcRef.P_POL_EXCLUSION_DETAIL();
                //                ExcludeDetailObj.EXCLUDE_DET_ID = p2.EXCLUDE_DET_ID;
                //                client.RemoveP_POL_EXCLUSION_DETAIL(ExcludeDetailObj);
                //            }
                //            catch (Exception ex)
                //            {
                //                throw new Exception(ex.Message.ToString());
                //            }
                //        }
                //    }
                //}
                #endregion 

                #region "ส่งข้อมูลเข้า P_ORA_TRANSACTION"
                using (PolicySvcRef.PolicySvcClient client = new PolicySvcRef.PolicySvcClient())
                {
                    try
                    {
                        PolicySvcRef.P_ORA_TRANSACTION oraRef = new P_ORA_TRANSACTION();
                        oraRef.UPD_DT = DateTime.Now;
                        oraRef.UPD_ID = model.UserID;
                        oraRef.NAMECODE = ""; //Namecode
                        oraRef.CHANNEL_TYPE = model.ChannelType.ToString();
                        if (model.ChannelType.ToString() == "GM" || model.ChannelType.ToString() == "HL")
                        {
                            oraRef.POLICY = model.txtPolicyNo;
                            oraRef.POLICY_HOLDER = model.PolicyHolding.ToString();
                        }
                        else
                        {
                            oraRef.POLICY = model.txtPolicyNo;
                        }
                        oraRef.POL_YR = Pol_yr;
                        oraRef.POL_LT = Pol_lt;
                        oraRef.STATUS = Status;
                        oraRef.PROGRAM_TYPE = "WIN";
                        oraRef.PROGRAM_NAME = "Endorse";
                        oraRef.WORKTYPE_CODE = "CTF";
                        client.AddP_ORA_TRANSACTION(ref oraRef);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                #endregion



                return StatusCode(201);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
    }
}
