using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;


namespace HTTP_Test
{
    class program
    {
        static void Main()
        {
            Task t = new Task(HTTP_GET);
            t.Start();
            Console.ReadLine();
        }

        static async void HTTP_GET()
        {
            var TARGETURL = "http://192.168.1.40:8090/controller/rest/applications";


            Console.WriteLine("GET: + " + TARGETURL);

            // ... Use HttpClient.            
            HttpClient client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes("admin@customer1:password");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            HttpResponseMessage response = await client.GetAsync(TARGETURL);
            HttpContent content = response.Content;

            // ... Check Status Code                                
            Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);

            // ... Read the string.
            string result = await content.ReadAsStringAsync();

            // ... Display the result.
            if (result != null &&
            result.Length >= 50)
            {
                Console.WriteLine(result.Substring(0, 500) + "...");
            }
        }
    }
}