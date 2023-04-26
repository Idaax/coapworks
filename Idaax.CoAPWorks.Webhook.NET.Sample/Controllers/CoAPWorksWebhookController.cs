/******************************************************************************
    CoAPWorks Webhook
    
    MIT License
    Copyright (c) [2023] [Idaax Inc., www.coapworks.com]
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
 *****************************************************************************/
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Idaax.CoAPWorks.Webhook.NET.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class CoAPWorksWebhookController : ControllerBase
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<CoAPWorksWebhookController> _logger;
        /// <summary>
        /// Constructir
        /// </summary>
        /// <param name="logger">ILogger</param>
        public CoAPWorksWebhookController(ILogger<CoAPWorksWebhookController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Readiness probe
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult Index()
        {
            return Ok("Ready");
        }
        /// <summary>
        /// This is the action/path that gets called by the Webhook caller.
        /// </summary>
        /// <param name="signature">An HMACSHA256 digest of the data</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<ActionResult> Index(string signature) {
            //TODO::Your client secret will come from configuration or a secret store
            //This is the client secret that you get from CoAPWorks website when you
            //configure the webhook
            string clientSecret = "<TODO: Add your client secret here>";
            string rawRequestBody = null;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                //The request body will contain the data as Json
                rawRequestBody = await reader.ReadToEndAsync();
                if(!string.IsNullOrWhiteSpace(rawRequestBody))
                {
                    JsonNode json = JsonObject.Parse(rawRequestBody);

                    if (!string.IsNullOrWhiteSpace(signature))
                    {
                        //Double check the signature
                        byte[] key = Encoding.UTF8.GetBytes(clientSecret);
                        byte[] data = Encoding.UTF8.GetBytes(rawRequestBody);
                        System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(key);
                        string digest = Convert.ToBase64String(hmac.ComputeHash(data));
                        if (digest == signature)
                        {
                            //We got this from CoAPWorks website...do something useful
                            _logger.LogInformation($"Got Json {json}");
                            return Ok(json);//For display
                        }
                        else
                        {
                            _logger.LogWarning("Authentication failure! Signature mismatch");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No data received in body. Nothing to process.");
                }
                
            }

            return Ok("Nothing received");
        }
    }
}