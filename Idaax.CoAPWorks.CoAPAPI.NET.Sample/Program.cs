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
using Idaax.CoAPWorks.CoAPAPI.Client;
using System.Collections;

var machine = new Machine("[Machine ID]", "[Channel ID]", null);

//Get the UTC time for the sensor node
DateTime utcDt = machine.GetUTCDateTime(false);
Console.WriteLine($"Current UTC time: {utcDt}");

//Get the pre-configured location datetime
LocalTimeInfo localTi = machine.GetPreConfiguredLocationDateTime(false);
Console.WriteLine($"Current local time at machine location: DST={localTi.IsInDST}, DateTime={localTi.LocaDateTime}, Timezone={localTi.TimeZone}");

//Get the location datetime for the given longitude and latitude
LocalTimeInfo localGPSTi = machine.GetLocationDateTime(false, (decimal)-122.00018, (decimal)37.3539342);
Console.WriteLine($"Current local time at machine location: DST={localGPSTi.IsInDST}, DateTime={localGPSTi.LocaDateTime}, Timezone={localGPSTi.TimeZone}");


//Get machine settings
Hashtable ht = machine.GetSettings(false);
foreach (var key in ht.Keys) Console.WriteLine($"{key}={ht[key]}");

//Send email alert
machine.SendEmailAlert(false, "Test subject", "This is a test email body.");
Console.WriteLine($"Email send success");

//Send heartbeat (payload as Json)
bool hbSuccess = machine.SendHeartbeat(false, "{'d':1}");
Console.WriteLine($"Send heartbeat success={hbSuccess}");

//Send channel feed
Hashtable htScf = machine.StoreChannelFeed(false, "{\"mt\":32.0,\"i\":0.34}");
foreach (var key in htScf.Keys) Console.WriteLine($"{key}={htScf[key]}");
Console.ReadLine();
