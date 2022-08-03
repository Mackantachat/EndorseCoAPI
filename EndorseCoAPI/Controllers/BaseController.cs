using EndorseCoAPI.BusinessLogic;
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
    public class BaseController : ControllerBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly ServiceAction serviceAction;
        public BaseController(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            var connecttionString = Configuration.GetConnectionString("BLA");
            this.serviceAction = new ServiceAction(connecttionString);
        }

        protected BasicHttpBinding SetHttpBinding()
        {BasicHttpBinding binding = new BasicHttpBinding();
            binding.CloseTimeout = TimeSpan.FromMinutes(10);
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(10);
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.TransferMode = TransferMode.Streamed;
            binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
            return binding;
        }
    }
}
