using System.ComponentModel.DataAnnotations;

namespace API_Playground.Models
{
    public class Code
    {
        [DataType(DataType.MultilineText)]
        public string csx { get; set; }

        [DataType(DataType.MultilineText)]
        public string output { get; set; }
    }

    public class HttpTestRequest
    {
        public string url { get; set; }
        public string body { get; set; }
    }

    //Model for the request / input
    public class CodeRequest
    {
        public string language { get; set; }
        public bool captureStats { get; set; }
        public string[] sources { get; set; }
    }

    //Model for the response / output
    public class CodeResponse
    {
        public string Phase { get; set; }
        public string Reason { get; set; }
        public bool Succeeded { get; set; }
        public string[] Output { get; set; }
    }
}
