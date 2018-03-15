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

                //We need to account for multiline responses
                if (codeOutput.Output.Length == 1)
                {
                    code.output = codeOutput.Output[0];
                }
                if(codeOutput.Output.Length > 1)
                {
                    foreach (var line in codeOutput.Output)
                    {
                        code.output += line + "<br />";
                    }
                }
            }

            return View(code);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
