using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ImageSelectionServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public static string JsonPath = AppDomain.CurrentDomain.BaseDirectory + "\\DownloadedFilesjson\\";
        public ValuesController()
        {
            Directory.CreateDirectory(JsonPath);
        }
        [Route("saveimage")]
        [HttpGet]
        public ActionResult SaveImage(string word, string dataText, bool skip)
        {
            if (!string.IsNullOrEmpty(word))
            {
                if (skip)
                {
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Skiped.txt",
                        word + "\r\n");
                    return Content(JsonConvert.SerializeObject(NextWord(word)), "application/json");
                }
                else
                {
                    System.IO.File.WriteAllText(JsonPath + word + ".json", dataText, Encoding.UTF8);
                    var image = JsonConvert.DeserializeObject<GoogleImage>(HttpUtility.UrlDecode(dataText));
                    Task.Factory.StartNew(() => DownloadImage(word, image.ImageUrl));
                }

            }
            return Content(JsonConvert.SerializeObject(NextWord(word)), "application/json");

        }
        public static void DownloadImage(string word, string url)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(url, AppDomain.CurrentDomain.BaseDirectory + "\\Images\\" + word + ".jpg");
            }
            catch (Exception)
            {
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\FailToDownload.txt",
                    word + "," + url + "\r\n");
            }

        }
        private WordImage NextWord(string oldWord)
        {
            if (!string.IsNullOrEmpty(oldWord))
                Startup.dictionary.Remove(oldWord);
            var word = Startup.dictionary.FirstOrDefault();
            return new WordImage()
            {
                word = word.Key,
                translate = word.Value,
            };
        }

        public class WordImage
        {
            public string word { get; set; }
            public string translate { get; set; }
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    public partial class GoogleImage
    {
        [JsonIgnore]
        [JsonProperty("cl")]
        public long Cl { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("isu")]
        public string Website { get; set; }

        [JsonIgnore]
        [JsonProperty("itg")]
        public long Itg { get; set; }

        [JsonProperty("ity")]
        public string Format { get; set; }

        [JsonProperty("oh")]
        public long Height { get; set; }

        [JsonProperty("ou")]
        public string ImageUrl { get; set; }

        [JsonProperty("ow")]
        public long Width { get; set; }

        [JsonProperty("pt")]
        public string Title { get; set; }

        [JsonIgnore]
        [JsonProperty("rh")]
        public string Rh { get; set; }

        [JsonIgnore]
        [JsonProperty("rid")]
        public string Rid { get; set; }

        [JsonIgnore]
        [JsonProperty("rt")]
        public long Rt { get; set; }

        [JsonProperty("ru")]
        public string PageUrl { get; set; }

        [JsonProperty("s")]
        public string Alt { get; set; }
        [JsonIgnore]
        [JsonProperty("st")]
        public string St { get; set; }
        [JsonIgnore]
        [JsonProperty("th")]
        public long Th { get; set; }
        [JsonIgnore]
        [JsonProperty("tu")]
        public Uri Tu { get; set; }
        [JsonIgnore]
        [JsonProperty("tw")]
        public long Tw { get; set; }
    }
}
