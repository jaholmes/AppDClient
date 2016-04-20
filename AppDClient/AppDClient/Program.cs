using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Serialization;
using System.Net.Http.Headers;

namespace AppD
{
    class program
    {
        static void Main()
        {
            Task t = new Task(GetApps);
            t.Start();
            Console.ReadLine();
        }

        static async void GetApps()
        {
            var targetUrl = "http://192.168.1.40:8090/controller/rest/applications";

            Console.WriteLine("GET: + " + targetUrl);

            HttpClient client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes("admin@customer1:pw");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            HttpResponseMessage response = await client.GetAsync(targetUrl);
            HttpContent content = response.Content;

            // Check Status Code                                
            Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);
            response.EnsureSuccessStatusCode();

            // Read the XML
            var xmlStream = await content.ReadAsStreamAsync();

            // Parse the XML
            var serializer = new XmlSerializer(typeof(Applications));
            var apps = (Applications)serializer.Deserialize(xmlStream);

            // Print the XML
            Console.WriteLine("size: " + apps.Apps.Count);
            foreach (Application app in apps.Apps)
            {
                Console.WriteLine("app: " + app.Id);
            }
        }
    }

    [XmlRoot("applications")]
    public class Applications
    {
        public Applications() { Apps = new List<Application>(); }

        [XmlElement("application")]
        public List<Application> Apps { get; set; }
    }

    public class Application
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public String Name { get; set; }
    }
}