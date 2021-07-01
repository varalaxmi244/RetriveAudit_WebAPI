using System;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        public class AdalServiceException : Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException
        {
            static void Main(string[] args)
            {
                // Set these values:
                    // e.g. https://yourorg.crm.dynamics.com
                    string url = String.Empty;
                // e.g. you@yourorg.onmicrosoft.com
                string userName = String.Empty;
                // e.g. y0urp455w0rd
                string password = String.Empty;

                // Azure Active Directory registered app clientid for Microsoft samples
                string clientId = String.Empty;
                var userCredential = new UserCredential();
                string apiVersion = String.Empty;
                string webApiUrl = String.Empty;
                Guid guid;

                Console.WriteLine("PLEASE PROVIDE CRM REGION CRM8/CRM4");
                 string val = Console.ReadLine();

                if (val == "CRM8")
                {
                    // Set these values:
                    // e.g. https://yourorg.crm.dynamics.com
                    url = "https://cg08900.crm8.dynamics.com/";
                    // e.g. you@yourorg.onmicrosoft.com
                    userName = "yeshwanthc@CG0890.onmicrosoft.com";
                    // e.g. y0urp455w0rd
                    password = "Year@2021";

                    clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
                    userCredential = new UserCredential(userName, password);
                    apiVersion = "9.0";
                    webApiUrl = $"https://cg08900.api.crm8.dynamics.com/api/data/v9.2/";
                }
                else {
                    // Set these values:
                    // e.g. https://yourorg.crm.dynamics.com
                    url = "https://a4crm.crm4.dynamics.com/";
                    // e.g. you@yourorg.onmicrosoft.com
                    userName = "ashfaq@a4crm.onmicrosoft.com";
                    // e.g. y0urp455w0rd
                    password = "B4biscuit";

                    clientId = "2bfd9875-1235-4fd7-ab09-617a5fa3f43f";
                    userCredential = new UserCredential(userName, password);
                    apiVersion = "9.0";
                    webApiUrl = $"https://a4crm.api.crm4.dynamics.com/api/data/v9.2/";
                }


                //Authenticate using IdentityModel.Clients.ActiveDirectory
                var authParameters = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(webApiUrl)).Result;
                var authContext = new AuthenticationContext(authParameters.Authority, false);
                var authResult = authContext.AcquireToken(url, clientId, userCredential);
                var authHeader = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(webApiUrl);
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    // Use the WhoAmI function
                    var response = client.GetAsync("WhoAmI").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.  
                        var user = response.Content.ReadAsStringAsync().Result;

                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Guid userId = (Guid)body["UserId"];
                        Console.WriteLine("Your UserId is {0}", userId);
                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of '{0}'",
                                    response.ReasonPhrase);
                    }

                    /*
                    Guid caseGuid = new Guid("b69e62a8-90df-e311-9565-a45d36fc5fe8");

                    Console.WriteLine("Your UserId is {0}", caseGuid);

                    byte[] data = File.ReadAllBytes(@"C:\Users\v-vpotuganti\Desktop\collab.png");

                    byte[] file = new byte[data.Length];

                    var jObject = new JObject
                    {
                            {"filename","collab.png"},
                            {"mimetype","text/plain" },
                            {"objectid_incident@odata.bind",  $"/incidents({caseGuid})"},
                            {"documentbody",Convert.ToBase64String(data)},
                    };


                    var createRequest = new HttpRequestMessage(HttpMethod.Post, "annotation")
                    {
                        Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json")
                    };

                    var createResponse = client.SendAsync(createRequest).Result;

                    */
                    
                    
                    HttpClient httpClient1 = null;
                    httpClient1 = new HttpClient();
                    //Default Request Headers needed to be added in the HttpClient Object
                    httpClient1.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient1.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Set the Authorization header with the Access Token received specifying the Credentials
                    httpClient1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                    httpClient1.BaseAddress = new Uri(webApiUrl);

                    //Examples of different filters here.
                    //var response1 = httpClient1.GetAsync("accounts?$select=name&amp;amp;amp;amp;amp;amp;$top=1").Result;
                    //var response1 = httpClient1.GetAsync("accounts?$select=name&amp;amp;amp;amp;amp;amp;$top=1").Result;
                    //var response1 = httpClient1.GetAsync("contacts?$select=fullname&amp;amp;amp;amp;amp;amp;$expand=parentcustomerid_account($select=accountid,name,createdon,emailaddress1,address1_telephone1)&amp;amp;amp;amp;amp;amp;$filter=emailaddress1 eq 'nowsanket@gmail.com'&amp;amp;amp;amp;amp;amp;$top=1").Result;
                    //var response1 = httpClient1.GetAsync("contacts?$select=name").Result;
                    var response1 = httpClient1.GetAsync("accounts?$select=name").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.  
                        JObject body1 = JObject.Parse(response1.Content.ReadAsStringAsync().Result);
                                               
                        // get account names
                        var accounts = response1.Content.ReadAsStringAsync().Result;
                        var jRetrieveResponse = JObject.Parse(accounts);
                        dynamic collContacts = JsonConvert.DeserializeObject(jRetrieveResponse.ToString());

                        foreach (var data in collContacts.value)
                        {
                            //You can change as per your need here
                            //guid = data.importfileid.Value;
                            Console.WriteLine("Account Name – " + data.name.Value);
                        }
                    }

                    Console.ReadLine();
                }
            }            
        }
    }
}