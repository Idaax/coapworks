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
using Idaax.CoAP.Helpers;
using Idaax.CoAP.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Idaax.CoAPWorks.CoAPAPI.Client
{
    /// <summary>
    /// Represents a CoAP machine
    /// </summary>
    public class Machine : CoAPClient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="channelId"></param>
        /// <param name="clientSecret"></param>
        public Machine(string machineId, string channelId , string clientSecret ) :
            base(machineId, channelId, clientSecret)
        { }
        /// <summary>
        /// Provision the machine
        /// </summary>
        /// <param name="encPayload">Should we use payload encryption during provisioning</param>
        /// <param name="pToken">The provisioning token</param>
        /// <returns>The new machine Id and secret key</returns>
        public string Provision(bool encPayload,string pToken)
        {
            string newMachineId = null;
            //Step 1: Initialize provisioning
            string url = $"/machine/pme?mid={_machineId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.POST, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            req.SetContentFormat(new CoAPContentFormatOption(CoAPContentFormatOption.TEXT_PLAIN));//Body is in text-plain
            req.AddPayload($"pt={pToken}");//Add provisioning token to the body as text
            //WARN: This sample does not support secure provisioning

            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            if(success)
            {
                //Provisioning initiated successfully...
                //The body will contain the new machine Id
                //If the machine is configured for secure authentication and encrypted payload
                //Then the body will have the following [New Machine Id] : [Secret Key]
                Hashtable ht = GetResponsePayload(resp);
                newMachineId = ht["mid"].ToString().Trim();
                //Complete provisioning
                url = $"/machine/pcm?mid={newMachineId}{GetUrlComponentsForSecureExchange(encPayload)}";
                
                req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.POST, messageId);
                req.SetURL(COAPWORKS_CAPI_BASE_URL + url);

                resp = SendReceive(req);
                success = IsResponseStatusSuccess(resp);

                if(!success){
                    newMachineId = null;//reset
                }
            }
            return newMachineId;
        }
        /// <summary>
        /// Get the current UTC date and time from the CoAP API server
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <returns>DateTime if successful else a date time with year as 1970</returns>
        public DateTime GetUTCDateTime(bool encPayload)
        {
            string url = $"/time/utc?mid={_machineId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));
            
            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON,CoAPMessageCode.GET,messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            if (success)
            {
                Hashtable ht = GetResponsePayload(resp);
                string dateTimeStr = ht["utc"] as string; //Format is s DD-MM-YYYY-HH-mm-ss
                string[] dtStrParts= dateTimeStr.Split(new char[] {'-'},StringSplitOptions.RemoveEmptyEntries);
                DateTime result = new DateTime(
                    Int32.Parse(dtStrParts[2]),
                    Int32.Parse(dtStrParts[1]),
                    Int32.Parse(dtStrParts[0]),
                    Int32.Parse(dtStrParts[3]),
                    Int32.Parse(dtStrParts[4]),
                    Int32.Parse(dtStrParts[5])) ;
                return result;
            }
            else
            {
                return new DateTime(1970, 1, 1);
            }
        }
        /// <summary>
        /// Get the current time for pre-configured machine location.
        /// You must provide longitude and latitude values in the machine settings screen
        /// for this to work
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <returns>LocalTimeInfo if successful else null</returns>
        public LocalTimeInfo GetPreConfiguredLocationDateTime(bool encPayload)
        {
            string url = $"/time/pcl?mid={_machineId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.GET, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            if (success)
            {
                Hashtable ht = GetResponsePayload(resp);

                LocalTimeInfo lti = new LocalTimeInfo() {
                    IsInDST = Int32.Parse(ht["dst"].ToString())==1,
                    TimeZone= ht["tmz"].ToString()
                };

                string dateTimeStr = ht["lot"] as string; //Format is s DD-MM-YYYY-HH-mm-ss
                string[] dtStrParts = dateTimeStr.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                DateTime result = new DateTime(
                    Int32.Parse(dtStrParts[2]),
                    Int32.Parse(dtStrParts[1]),
                    Int32.Parse(dtStrParts[0]),
                    Int32.Parse(dtStrParts[3]),
                    Int32.Parse(dtStrParts[4]),
                    Int32.Parse(dtStrParts[5]));
                lti.LocaDateTime = result;
                return lti;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get the current time for given machine location based on longitude and latitude 
        /// for this to work
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        /// <returns>LocalTimeInfo if successful else null</returns>
        public LocalTimeInfo GetLocationDateTime(bool encPayload,decimal longitude,decimal latitude)
        {
            string url = $"/time/loc?mid={_machineId}&lat={latitude}&lng={longitude}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.GET, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            if (success)
            {
                Hashtable ht = GetResponsePayload(resp);

                LocalTimeInfo lti = new LocalTimeInfo()
                {
                    IsInDST = Int32.Parse(ht["dst"].ToString()) == 1,
                    TimeZone = ht["tmz"].ToString()
                };

                string dateTimeStr = ht["lot"] as string; //Format is s DD-MM-YYYY-HH-mm-ss
                string[] dtStrParts = dateTimeStr.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                DateTime result = new DateTime(
                    Int32.Parse(dtStrParts[2]),
                    Int32.Parse(dtStrParts[1]),
                    Int32.Parse(dtStrParts[0]),
                    Int32.Parse(dtStrParts[3]),
                    Int32.Parse(dtStrParts[4]),
                    Int32.Parse(dtStrParts[5]));
                lti.LocaDateTime = result;
                return lti;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get the current settings of the machine 
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <returns>Hashtable if successful else null</returns>
        public Hashtable GetSettings(bool encPayload)
        {
            string url = $"/machine/set?mid={_machineId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.GET, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            if (success)
            {
                Hashtable ht= GetResponsePayload(resp);
                if (ht.ContainsKey("ec")/*The error code key*/) ht.Remove("ec");
                return ht;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Send an email alert
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        public void SendEmailAlert(bool encPayload,string subject,string body)
        {
            if (string.IsNullOrWhiteSpace(subject) || subject.Length > 20) throw new ArgumentException("Subject null or too long.Max 20 chars");
            if (string.IsNullOrWhiteSpace(body) || body.Length > 128) throw new ArgumentException("Body null or too long.Max 128 chars");
            string url = $"/alerts/eml?mid={_machineId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.NON, CoAPMessageCode.POST, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            req.AddPayload($"s={subject};b={body}");
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            _coapClient.Send(req);
        }
        /// <summary>
        /// Send an machine heartbeat
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <param name="hbd">Heartbeat data. If this starts with { then it is assumed to be Json else plain text. Max 128 chars</param>
        /// <returns>true on success </returns>
        public bool SendHeartbeat(bool encPayload, string hbd)
        {
            string url = $"/machine/hbt?mid={_machineId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.POST, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);
            
            if (!string.IsNullOrWhiteSpace(hbd))
            {
                if (hbd.Trim().Length > 128) throw new ArgumentException("hbd value too long.Max 128 chars");
                if (hbd.Trim().StartsWith("{"))
                {
                    req.AddOption(CoAPHeaderOption.CONTENT_FORMAT, AbstractByteUtils.GetBytes(CoAPContentFormatOption.APPLICATION_JSON));
                }
                else
                {
                    req.AddOption(CoAPHeaderOption.CONTENT_FORMAT, AbstractByteUtils.GetBytes(CoAPContentFormatOption.TEXT_PLAIN));
                }
                req.AddPayload(hbd.Trim());
            }
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            return success;
        }
        /// <summary>
        /// Send an machine channel feed for storage
        /// </summary>
        /// <param name="encPayload">If true, then payload is encrypted during send/receive</param>
        /// <param name="feed">The channel feed.If this starts with { then it is assumed to be in Json. Max 128 chars</param>
        /// <returns>Hashtable </returns>
        public Hashtable StoreChannelFeed(bool encPayload, string feed)
        {
            if (string.IsNullOrWhiteSpace(_channelId)) throw new ArgumentException("Channel Id not set.");
            string url = $"/machine/scf?mid={_machineId}&cid={_channelId}{GetUrlComponentsForSecureExchange(encPayload)}";
            ushort messageId = UInt16.Parse(DateTime.UtcNow.ToString("mmssf"));

            CoAPRequest req = new CoAPRequest(CoAPMessageType.CON, CoAPMessageCode.POST, messageId);
            req.SetURL(COAPWORKS_CAPI_BASE_URL + url);

            if (!string.IsNullOrWhiteSpace(feed))
            {
                if (feed.Trim().Length > 128) throw new ArgumentException("feed value too long.Max 128 chars");
                if (feed.Trim().StartsWith("{"))
                {
                    req.AddOption(CoAPHeaderOption.CONTENT_FORMAT, AbstractByteUtils.GetBytes(CoAPContentFormatOption.APPLICATION_JSON));
                }
                else
                {
                    req.AddOption(CoAPHeaderOption.CONTENT_FORMAT, AbstractByteUtils.GetBytes(CoAPContentFormatOption.TEXT_PLAIN));
                }
                req.AddPayload(feed.Trim());
            }
            //NOTE: If authentication and encryption is used, then our propertieary security library is
            //required. This implementation does not support it.
            CoAPResponse resp = SendReceive(req);
            bool success = IsResponseStatusSuccess(resp);
            if (success)
            {
                return GetResponsePayload(resp);
            }
            else
            {
                return null;
            }
        }
    }
}
