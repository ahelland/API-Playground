using Microsoft.AspNetCore.Mvc;
using API_Playground.Models;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using static System.Environment;

namespace API_Playground.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult HTTP()
        {
            Code c = new Code
            {
                csx = "{<br />\t\"name\":\"Azure\"<br />}",
                output = "{<br />\t\"foo\":\"bar\"<br />}"
            };
            return View(c);
        }

        [HttpPost]
        public async Task<IActionResult> Http(Code code)
        {
            string output = code.csx;

            using (var client = new HttpClient())
            {
                var reqInput = code.csx;
                var req = JsonConvert.DeserializeObject<HttpTestRequest>(reqInput);
                var requestUrl = req.url;
                var request_content = req.body;

                var responseString = "";

                //For HTTP GET requests (assumption that no body == GET)
                if (request_content == "")
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl));
                    HttpResponseMessage response = await client.SendAsync(request);
                    responseString = await response.Content.ReadAsStringAsync();
                    //We feed the default body back into the view just for the sake of visibility; looks weird when it gets cleared
                    code.csx = "{<br />\t\"name\":\"Azure\"<br />}";
                }

                //For HTTP POST requests
                if (request_content != "")
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUrl))
                    {
                        Content = new StringContent(request_content, Encoding.UTF8, "application/json")
                    };
                    //No support for XML bodies at the moment
                    //Considering adding this header in the UI
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.SendAsync(request);
                    responseString = await response.Content.ReadAsStringAsync();
                    code.csx = req.body;
                }

                code.output = "{<br />\t\"result\":" + responseString + "<br />}";
            }

            return View(code);
        }

        [HttpGet]
        public IActionResult CSharp()
        {
            //Init page with "Hello World" code
            Code c = new Code
            {
                csx = $"using System;{NewLine}{NewLine}public class HelloWorld{NewLine}{{{NewLine}\t" +
                        $"public static void Main(){NewLine}\t{{{NewLine}\t\t" +
                        $"Console.WriteLine(\"Hello from Contos.io!\");{NewLine}\t}}{NewLine}}}",
                output = "{<br />\t\"foo\":\"bar\"<br />}"
            };

            return View(c);
        }

        [HttpPost]
        public async Task<IActionResult> CSharp(Code code)
        {
            string output = code.csx;

            var apiURL = "https://www.microsoft.com/net/api/code";

            using (var client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(apiURL));
                request.Headers.Add("Referer", "https://www.microsoft.com/net");
                var codeSnippet = code.csx;

                var codeInput = new CodeRequest { language = "csharp", captureStats = false, sources = new string[1] };
                codeInput.sources[0] = codeSnippet;

                var request_content = JsonConvert.SerializeObject(codeInput);
                request.Content = new StringContent(request_content, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();
                var codeOutput = JsonConvert.DeserializeObject<CodeResponse>(responseString);
                code.output = codeOutput.Output[0];
            }

            return View(code);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
