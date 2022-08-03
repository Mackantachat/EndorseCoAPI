using EndorseCoAPI.DataContract;
using ITUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EndorseCoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : BaseController
    {
        public ServicesController(IConfiguration Configuration) : base(Configuration)
        {
        }

        [HttpGet]
        [Route("GetChannelType")]
        public IActionResult GetChannelType()
        {
            try
            {
                NewBisSvcRef.NewBISSvcClient client = null;
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["NewBisSvcRef"]);
                client = new NewBisSvcRef.NewBISSvcClient(binding, endpoint);
                NewBisSvcRef.ProcessResult prCh = new NewBisSvcRef.ProcessResult();
                var autbChannels = client.GetAutbChannel(out prCh);
                List<DropDownListModel> data = new List<DropDownListModel>();
                if (prCh.Successed == true)
                {
                    autbChannels.ForEach(e => data.Add(new DropDownListModel { id = e.CHANNEL_TYPE, text = e.DESCRIPTION }));
                }
                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("GetPolicyHolding")]
        public IActionResult GetPolicyHolding([FromBody] RequestPolicyHolding request)
        {
            try
            {
                PolicyWithCertSvcRef.PolicyWithCertSvcClient client = null;
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["PolicyWithCertSvcRef"]);
                client = new PolicyWithCertSvcRef.PolicyWithCertSvcClient(binding, endpoint);
                PolicyWithCertSvcRef.ZTB_POLICYOWNER_PLAN_Collection policyOwnerPlanColl = new PolicyWithCertSvcRef.ZTB_POLICYOWNER_PLAN_Collection();
                ProcessResult prx = new ProcessResult();
                policyOwnerPlanColl = client.GetPolicyInChannel(request.ChannelType, out prx);
                var ObjSorting = (from cctb in policyOwnerPlanColl
                                  orderby cctb.POLICY
                                  select cctb).ToList();

                List<DropDownListModel> data = new List<DropDownListModel>();
                if (prx.Successed == true)
                {
                    ObjSorting.ForEach(e => data.Add(new DropDownListModel { id = e.POLICY, text = e.POLICY }));
                }
                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetExcludeScript")]
        public IActionResult GetExcludeScript()
        {
            try
            {
                NewBisSvcRef.NewBISSvcClient client = null;
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["NewBisSvcRef"]);
                client = new NewBisSvcRef.NewBISSvcClient(binding, endpoint);
                NewBisSvcRef.ProcessResult exPr = new NewBisSvcRef.ProcessResult();
                var ExcludeScriptColl = client.GetExcludeDescription(out exPr);
                List<DropDownListModel> data = new List<DropDownListModel>();
                if (exPr.Successed == true)
                {
                    ExcludeScriptColl.ForEach(e => data.Add(new DropDownListModel { id = e.SCRIPT_CODE.ToString(), text = e.DESCRIPTION }));
                }
                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetExcludeCause")]
        public IActionResult GetExcludeCause()
        {
            try
            {
                NewBisSvcRef.NewBISSvcClient client = null;
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["NewBisSvcRef"]);
                client = new NewBisSvcRef.NewBISSvcClient(binding, endpoint);
                NewBisSvcRef.ProcessResult exPr = new NewBisSvcRef.ProcessResult();
                var dataColl = client.GetAutbDataDicDets("EXCLUDE_CAUSE", out exPr);
                List<DropDownListModel> data = new List<DropDownListModel>();
                if (exPr.Successed == true)
                {
                    dataColl.ForEach(e => data.Add(new DropDownListModel { id = e.CODE.ToString(), text = e.DESCRIPTION }));
                }
                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }


        [HttpGet]
        [Route("GetExcludeTMNCause")]
        public IActionResult GetExcludeTMNCause()
        {
            try
            {
                PolicySvcRef.PolicySvcClient client = null;
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["PolicySvcRef"]);
                client = new PolicySvcRef.PolicySvcClient(binding, endpoint);
                string[] Col_Names = { (string)"EXCLUDE_TMN_CAUSE" };
                int[] Codes = null;
                PolicySvcRef.ZTB_CONSTANT1[] ztbArr = null;
                var dataColl = client.GetZTB_CONSTANT1(Col_Names, Codes, out ztbArr);
                List<DropDownListModel> data = new List<DropDownListModel>();
                if (ztbArr != null && ztbArr.Count() > 0)
                {
                    foreach (var x in ztbArr)
                    {
                        data.Add(new DropDownListModel
                        {
                            id = x.CODE2.ToString(),
                            text = x.DESCRIPTION
                        });
                    }
                    //ztbArr.ForEach(e => data.Add(new DropDownListModel { id = e.CODE2.ToString(), text = e.DESCRIPTION }));
                }
                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetEndorsePrinting")]
        public IActionResult GetEndorsePrinting()
        {
            try
            {
                List<DropDownListModel> data = new List<DropDownListModel>();
                data.Add(new DropDownListModel
                {
                    id = "N",
                    text = "ไม่พิมพ์"
                });
                data.Add(new DropDownListModel
                {
                    id = "Y",
                    text = "พิมพ์"
                });

                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("GenRef")]
        public IActionResult GenRef([FromBody] RequestPolicyHolding model)
        {
            try
            {
                RespondServicesModel data = new RespondServicesModel();
                EndorseSvcRef.EndorseRefSvcClient client = null;
                BasicHttpBinding binding = SetHttpBinding();
                EndpointAddress endpoint = new EndpointAddress(Configuration["EndorseSvcRef"]);
                client = new EndorseSvcRef.EndorseRefSvcClient(binding, endpoint);
                long refno = 0;
                client.GetEndorseRefNo(model.ChannelType.ToString(), "END", out refno);

                data.RefNo = refno.ToString("D5") + "/" + DateTime.Now.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("th-TH"));

                return Ok(data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

    }
}
