using Applivery.Desktop.Comics.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applivery.Desktop.Comics.Logic
{
    public static class ApiReader
    {
        public static async Task<Models.ComicJSON.Rootobject> ReadComicsFromAPI()
        {
            ObservableCollection<CreatorComicModel> creators = new ObservableCollection<CreatorComicModel>();
            Models.ComicJSON.Rootobject rootObject = null;

            try
            {
                // https://developer.marvel.com/documentation/authorization

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(30);
                    string publicKey = "8a8c5fbb2f979075bad82b7525cfac0b";
                    string privateKey = "eaebde72eef710722b3b3abd5e6913ad2eb72d33";

                    String timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");

                    string hash = Utils.MD5Hash(timeStamp + privateKey + publicKey);

                    //string url = $"http://gateway.marvel.com/v1/public/comics?&format=comic&formatType=comic&noVariants=true&ts={timeStamp}&apikey={publicKey}&hash={hash}"; // no variants
                    //string url = $"http://gateway.marvel.com/v1/public/comics?&format=comic&formatType=comic&ts={timeStamp}&apikey={publicKey}&hash={hash}"; // format comic
                    string url = $"http://gateway.marvel.com/v1/public/comics?&format=comic&ts={timeStamp}&apikey={publicKey}&hash={hash}";

                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonString = await response.Content.ReadAsStringAsync();
                            rootObject = JsonConvert.DeserializeObject<Models.ComicJSON.Rootobject>(jsonString);
                        }
                        else
                        {
                            throw new Exception("Marvel API returned a failure response code: " + response.StatusCode);
                        }
                    }
                }
                return rootObject;
            }
            catch (Exception ex)
            {
                // here log with Log4net or similar custom logger.
                var msg = ex.Message;
                return rootObject;
            }
        }
    }
}
