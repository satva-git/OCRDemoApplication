using System.Collections.Generic;

namespace OCRDemoApplication.Models
{
    public class ApiResult
    {
        public object Data { get; set; }
        public List<Dictionary<string, string>> DataDictionary1 { get; set; }
        public List<Dictionary<string, string>> DataDictionary2 { get; set; }
    }
}