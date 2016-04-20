using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Serialization;
using System.Net.Http.Headers;
using CommandLine;
using CommandLine.Text;

namespace AppD
{
    class Program
    {
        static CommandLineOptions Options { get; set; } = new CommandLineOptions();

        static void Main(String[] args)
        {
            if (!ParseCommandLine(args)) return;
            Task t = new Task(GetApps);
            t.Start();
            Console.ReadLine();
        }

        static bool ParseCommandLine(String[] args)
        {
            if (CommandLine.Parser.Default.ParseArguments(args, Options))
            {
                if (Options.Verbose)
                {
                    Console.WriteLine("url: " + Options.ControllerUrl);
                    Console.WriteLine("user: " + Options.User);
                    Console.WriteLine("password: " + Options.Password);
                }
                return true;
            }
            return false;
        }

        static async void GetApps()
        {
            //var targetUrl = "http://192.168.1.40:8090/controller/rest/applications";
            var targetUrl = Options.ControllerUrl + "/controller/rest/applications";

            Console.WriteLine("targetUrl: + " + targetUrl);

            HttpClient client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes(Options.User+":"+Options.Password);
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

    class CommandLineOptions
    {
        [Option('c', "controller", Required=true, HelpText = "Controller URL.")]
        public string ControllerUrl { get; set; }

        [Option('u', "user", Required=true, HelpText = "Controller User for REST Api")]
        public string User { get; set; }

        [Option('p', "password", Required=true, HelpText = "Controller Password for REST Api")]
        public string Password { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText();
            help.AddPreOptionsLine("usage: AppDClient [-<option> <optionvalue>]...");
            help.AddPreOptionsLine("example: AppDClient -c http://localhost:8090 -u abc@customer1 -p pwd");
            help.AddOptions(this);
            return help;
        }
    }

}