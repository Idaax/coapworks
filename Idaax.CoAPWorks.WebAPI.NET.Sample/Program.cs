/******************************************************************************
    CoAPWorks Web API Client
    
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
using System.Text;
using System.Security.Cryptography;
using System.Json; 
public class Program{
    private static readonly string WEB_CLIENT_NAME="<Your Web Client Name>";
    private static readonly string WEB_CLIENT_SECRET="<Your Web Client Secret>";
    private static readonly int WEB_CLIENT_ID = -99999;//Enter the web client ID as provided on the CoAPWorks website
    public static async Task Main(String[] args){
        Console.WriteLine("Starting...."); 
        
        string machineId="<Your machine name>";
        string channelId="<Your machine's channel name>";

        await CreateMachine(machineId);
        await UpdateMachine(machineId);
        await MachineExists(machineId);

        await MachineHeartbeats(machineId);
        
        await CreateChannel(machineId,channelId);
        await UpdateChannel(machineId,channelId);
        await UpdateCustomScript(machineId,channelId);
        await GetCustomScript(machineId,channelId);
        await GetChannelFeeds(machineId,channelId);
        
        await UpdateMachineSettings(machineId);
        await ReadMachineSettings(machineId);
        await DeleteMachineSettings(machineId);

        await DeleteChannel(machineId,channelId);
        await DeleteMachine(machineId);
    }

    static async Task CreateMachine(string machineId){
        Console.WriteLine("Into CreateMachine....");
        var postParams = new JsonObject();
        postParams["ID"]=machineId;
        postParams["Description"]="Machine created using Http web client";
        postParams["IsActive"]=true;
        postParams["IsPublic"]=false;
        postParams["UsesOpenConnection"]=true;
        postParams["Firmware"]="VGhpcyBpcyBhIHRlc3Q="; //A base64 string
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post("/machine/create",content);
    }
    static async Task UpdateMachine(string machineId){
        Console.WriteLine("Into UpdateMachine....");
        var postParams = new JsonObject();
        postParams["ID"]=machineId;
        postParams["Description"]="Machine updated using Http web client";
        postParams["IsActive"]=true;
        postParams["IsPublic"]=false;
        postParams["UsesOpenConnection"]=true;
        postParams["DeleteFirmware"]=true;
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post("/machine/update",content);
    }
    static async Task DeleteMachine(string machineId){
        Console.WriteLine("Into DeleteMachine....");
        await Post($"/machine/delete?id={machineId}",null);
    }
    
    static async Task MachineExists(string machineId)
    {
        Console.WriteLine("Into MachineExists....");
        await Get($"/machine/exists?id={machineId}");
    }

    static async Task MachineHeartbeats(string machineId)
    {
        long since = DateTime.UtcNow.AddDays(-1).Ticks;

        Console.WriteLine("Into MachineHeartbeat....");
        await Get($"/machine/heartbeats?id={machineId}&since={since}&count=10");
    }

    static async Task CreateChannel(string machineId,string channelId){
        Console.WriteLine("Into CreateChannel....");
        var postParams = new JsonObject();
        postParams["MachineID"]=machineId;
        postParams["ChannelID"]=channelId;
        postParams["IsActive"]=true;
        postParams["IsPublic"]=false;
        postParams["Description"]="Test machine channel without RefVars and custom script";
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post("/machinechannel/create",content);
    }

    static async Task UpdateChannel(string machineId,string channelId){
        Console.WriteLine("Into UpdateChannel....");
        var postParams = new JsonObject();
        postParams["MachineID"]=machineId;
        postParams["ChannelID"]=channelId;
        postParams["IsActive"]=true;
        postParams["IsPublic"]=false;
        postParams["Description"]="Test machine channel without RefVars and custom script";
        postParams["RefVars"]="TTH=30.00";
        postParams["CustomScript"]="if(t < TTH) return \"{'alarm':1}\"";
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post("/machinechannel/update",content);
    }

    static async Task DeleteChannel(string machineId,string channelId){
        Console.WriteLine("Into DeleteChannel....");
        await Post($"/machinechannel/delete?mid={machineId}&cid={channelId}",null);
    }

    static async Task GetChannelFeeds(string machineId,string channelId){
        Console.WriteLine("Into GetChannelFeed....");
        long since = DateTime.UtcNow.AddDays(-1).Ticks;
        await Get($"/machinechannel/feeds?mid={machineId}&cid={channelId}&since={since}&count=10");
    }
    static async Task UpdateCustomScript(string machineId,string channelId){
        Console.WriteLine("Into UpdateCustomScript....");
        var postParams = new JsonObject();
        postParams["MachineID"]=machineId;
        postParams["ChannelID"]=channelId;
        postParams["RefVars"]="TTH=20.00";
        postParams["CustomScript"]="if(t > TTH) return \"{'alarm':1}\"";
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post("/machinechannel/updatecustomscript",content);
    }

    static async Task GetCustomScript(string machineId,string channelId){
        Console.WriteLine("Into GetCustomScript....");
        await Get($"/machinechannel/customscript?mid={machineId}&cid={channelId}");
    }

    static async Task ReadMachineSettings(string machineId){
        Console.WriteLine("Into ReadMachineSettings....");
        await Get($"/machinesettings/read?mid={machineId}");
    }

    static async Task UpdateMachineSettings(string machineId){
        Console.WriteLine("Into UpdateMachineSettings....");
        var postParams = new JsonObject();
        postParams["MachineID"]=machineId;
        postParams["SettingName"]="lng";
        postParams["SettingValue"]="-122.34";
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post($"/machinesettings/update",content);
    }
    static async Task DeleteMachineSettings(string machineId){
        Console.WriteLine("Into DeleteMachineSettings....");
        var postParams = new JsonObject();
        postParams["MachineID"]=machineId;
        postParams["SettingName"]="lng";
        var content = new StringContent(postParams.ToString(), Encoding.UTF8, "application/json");
        await Post($"/machinesettings/delete",content);
    }

    static async Task Post(string path,HttpContent postData){
        var http = PrepareHttpClient();
        var response = await http.PostAsync(http.BaseAddress+path,postData);
        Console.WriteLine(response);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    static async Task Get(string path){
        var http = PrepareHttpClient();
        var response = await http.GetAsync(http.BaseAddress+path);
        Console.WriteLine(response);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }

    static HttpClient PrepareHttpClient(){
        int nonce=Int32.Parse(DateTime.UtcNow.ToString("HHmmss"));
        
        HttpClient http = new HttpClient();
        http.BaseAddress= new Uri("https://wapi.coapworks.com/v1/api");
        
        http.DefaultRequestHeaders.Add("X-COAPWK-WCID", WEB_CLIENT_ID.ToString());
        http.DefaultRequestHeaders.Add("X-COAPWK-NONCE",nonce.ToString());
        http.DefaultRequestHeaders.Add("X-COAPWK-WCNAME",WEB_CLIENT_NAME);

        string dataToHash = WEB_CLIENT_NAME.Trim() + ";" + WEB_CLIENT_ID.ToString() + ";" + nonce.ToString();
        byte[] dataToHashBytes = ASCIIEncoding.ASCII.GetBytes(dataToHash);
        byte[] key = Encoding.ASCII.GetBytes(WEB_CLIENT_SECRET);
        HMACSHA256 hasher = new HMACSHA256(key);
        byte[] hashed = hasher.ComputeHash(dataToHashBytes);
        string hashedAsB64 = Convert.ToBase64String(hashed);

        http.DefaultRequestHeaders.Add("X-COAPWK-WCHASH",hashedAsB64);

        return http;
    }
}
