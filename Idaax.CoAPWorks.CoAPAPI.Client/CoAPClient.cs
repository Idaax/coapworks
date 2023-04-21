/******************************************************************************
    CoAPWorks CoAP API Client
    
    MIT License

    Copyright (c) [2023] [Idaax Inc., www.coapsharp.com]

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
using Idaax.CoAP.Channels;
using Idaax.CoAP.Helpers;
using Idaax.CoAP.Message;
using System;
using System.Collections;

namespace Idaax.CoAPWorks.CoAPAPI.Client
{
    /// <summary>
    /// Base class for all API clients
    /// </summary>
    public abstract class CoAPClient
    {
        #region Constants
        /// <summary>
        /// The host name where APIs are hosted
        /// </summary>
        protected static readonly string COAPWORKS_COAPAPI_HOSTNAME = "capi.coapworks.com";
        /// <summary>
        /// The port number
        /// </summary>
        protected static readonly int COAPWORKS_COAPAPI_PORT = 5683;
        /// <summary>
        /// The base URL of the CoAP API
        /// </summary>
        protected static readonly string COAPWORKS_CAPI_BASE_URL = "coap://" + COAPWORKS_COAPAPI_HOSTNAME + ":5683" + "/v1";
        /// <summary>
        /// API receive timeout
        /// </summary>
        protected static readonly int COAPWORKS_CAPI_RX_TIMEOUT_MILLIS = 30_000;
        #endregion

        #region Implementation
        /// <summary>
        /// Synchronous client channel
        /// </summary>
        protected CoAPSyncClientChannel _coapClient = null;
        /// <summary>
        /// Holds the machine id
        /// </summary>
        protected string _machineId = null;
        /// <summary>
        /// Holds the channel Id
        /// </summary>
        protected string _channelId = null;
        /// <summary>
        /// The secret key to use for message authentication and security
        /// </summary>
        protected string _clientSecret = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="machineId">The machine Id</param>
        /// <param name="channelId">The channel Id</param>
        /// <param name="clientSecret">The client secret</param>
        public CoAPClient(string machineId,string channelId,string clientSecret) { 
            _machineId = machineId;
            _channelId = channelId;
            _clientSecret = clientSecret;
        }
        #endregion

        #region methods
        /// <summary>
        /// Send request and receive response
        /// </summary>
        /// <param name="request">CoAPRequest</param>
        /// <returns>CoAPResponse</returns>
        public virtual CoAPResponse SendReceive(CoAPRequest request)
        {
            if(request == null) throw new ArgumentNullException(nameof(request));   
            
            if(_coapClient != null) _coapClient.Shutdown();
            _coapClient = new CoAPSyncClientChannel();
            _coapClient.Initialize(COAPWORKS_COAPAPI_HOSTNAME, COAPWORKS_COAPAPI_PORT);
            _coapClient.Send(request);
            bool timedOut = false;
            CoAPResponse resp = (CoAPResponse)_coapClient.ReceiveMessage(COAPWORKS_CAPI_RX_TIMEOUT_MILLIS, ref timedOut);
            if (timedOut)
                throw new TimeoutException();
            else
                return resp;
        }
        /// <summary>
        /// If client secret is null/empty/whitespace, we assume that open authentication
        /// mode is used and only the machine Id is used to authenticate
        /// </summary>
        /// <returns></returns>
        protected bool IsUsingOpenAuth()
        {
            return string.IsNullOrWhiteSpace(_clientSecret);
        }
        /// <summary>
        /// A secure exchange signs the message with an HMASHA256 digest and also encrypts
        /// the payload. It is here for demonstration purpose only and not supported in this
        /// library
        /// </summary>
        /// <returns>string</returns>
        protected string GetUrlComponentsForSecureExchange(bool encPayload)
        {
            if (!IsUsingOpenAuth())
            {
                string nonce = DateTime.UtcNow.ToString("mmssff");
                string url = $"&nonce={nonce}&hs256=1"; //We use HMACSHA256 for signature generation
                if (encPayload)
                    url += "&epx=a256";//We use AES256 for payload exchange
                return url;
            }
            else
                return "";
        }
        /// <summary>
        /// Get response payload as a hashtable key/value pair
        /// </summary>
        /// <param name="resp">CoAPResponse</param>
        /// <returns>Hashtable</returns>
        protected Hashtable GetResponsePayload(CoAPResponse resp)
        {
            if (resp == null) return null;
            int payloadFormat = CoAPContentFormatOption.TEXT_PLAIN; //default
            if (resp.Options.HasOption(CoAPHeaderOption.CONTENT_FORMAT))
                payloadFormat = AbstractByteUtils.ToUInt16(resp.Options.GetOption(CoAPHeaderOption.CONTENT_FORMAT).Value);
            bool payloadIsJson = (payloadFormat == CoAPContentFormatOption.APPLICATION_JSON);

            //Response body is in plain text, key1=value1;key2=value2 etc.
            //else response is in Json {"key1":"value1","key2":"value2"...}
            string payload = resp.GetPayload();
            Hashtable ht = new Hashtable();
            if (!string.IsNullOrWhiteSpace(payload))
            {
                if (payloadIsJson)
                    ht = JSONResult.FromJSON(payload);
                else
                    ht = TextResult.FromText(payload);
            }
            return ht;
        }
        /// <summary>
        /// Check if the response contains success code (error code=0)
        /// </summary>
        /// <param name="resp">CoAPResponse</param>
        /// <returns>bool</returns>
        protected bool IsResponseStatusSuccess(CoAPResponse resp)
        {
            Hashtable ht = GetResponsePayload(resp);
            if(ht != null && ht.ContainsKey("ec")) return (Int32.Parse(ht["ec"].ToString()) == 0);
            return false;
        }
        #endregion
    }
}