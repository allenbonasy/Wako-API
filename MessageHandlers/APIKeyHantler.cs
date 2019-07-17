using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace DTransAPI.MessageHandlers
{
    public class APIKeyHantler: DelegatingHandler
    {
        private const string  apiKey= "XX123456789#@";
        private const string paynowGet = "/api/payGet";
        private const string paynowResult = "/api/payResult";


        protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage message,CancellationToken cancellationToken)
        {
            bool validkey = false;
            IEnumerable<String> requestHeader;

            var checkExist = message.Headers.TryGetValues("Authorization", out requestHeader);
            if (checkExist)
            {
                if (requestHeader.FirstOrDefault().Equals(apiKey))
                {
                    validkey = true;
                }
              
               
            }
            if ((message.RequestUri.LocalPath.Equals(paynowGet)) || (message.RequestUri.LocalPath.Equals(paynowResult)))
                {
                validkey = true;
            }

            if (!validkey)
            {
                return message.CreateResponse(HttpStatusCode.Forbidden, "Not Authorized");
            }

            var response = await base.SendAsync(message, cancellationToken);
            return response;

        } 
    }
}