using Idaax.CoAPWorks.CoAPAPI.Client;
using System.Collections;

//var machine = new Machine("[Machine ID]", "[Channel ID]", "[NOT USED IN THIS SAMPLE]");
var machine = new Machine("IDXINC0000", "TEMPSENSOR", "[NOT USED IN THIS SAMPLE]");

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
