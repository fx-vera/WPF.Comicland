using Applivery.Desktop.Comics.Models;
using Applivery.Desktop.Core.MVVM;
using Applivery.Desktop.Core.Utils;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace Applivery.Desktop.Comics.ViewModels
{
    /// <summary>
    /// Viewmodel to manage the main operations of the plugin.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DisplayerViewModel : ViewModel
    {
        public DisplayerViewModel()
        {
            DisplayInfoCommand = new RelayCommand(o => PerformDisplayInfoCommand(o));
            DisplayerModel = new DisplayerModel();
            ReadComicsFromAPI();
        }

        /// <summary>
        /// Executed when the user clic on a title.
        /// </summary>
        public ICommand DisplayInfoCommand { get; set; }

        /// <summary>
        /// Contains the Comics data
        /// </summary>
        public DisplayerModel DisplayerModel { get; set; }

        public string Description { get; set; }

        private void PerformDisplayInfoCommand(object o)
        {

        }

        private async void ReadComicsFromAPI()
        {
            try
            {
                // https://developer.marvel.com/documentation/authorization

                DisplayerModel.Comics.Clear();

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(30);
                    string publicKey = "8a8c5fbb2f979075bad82b7525cfac0b";
                    string privateKey = "eaebde72eef710722b3b3abd5e6913ad2eb72d33";

                    String timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");

                    string hash = MD5Hash(timeStamp + privateKey + publicKey);

                    string url = $"http://gateway.marvel.com/v1/public/comics?&format=comic&formatType=comic&noVariants=true&ts={timeStamp}&apikey={publicKey}&hash={hash}";

                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            JObject jsonContent = JObject.Parse(await response.Content.ReadAsStringAsync());
                            JToken data = jsonContent["data"];
                            JArray results = data.Value<JArray>("results");

                            var allResults = results.ToObject<List<JToken>>().ToArray();

                            var totalString = data.Value<string>("count");
                            int.TryParse(totalString, out int total);

                            for (int i = 0; i < total; i++)
                            {
                                JToken comic = allResults[i];
                                var title = comic.Value<string>("title");
                                JToken textObjects = comic.Value<JToken>("textObjects");
                                JToken description = null;

                                if (textObjects.Values().ToArray().Length > 1)
                                {
                                    description = textObjects.Values().ToArray()[2];
                                }

                                DisplayerModel.Comics.Add(
                                    new ComicModel()
                                    {
                                        Name = title,
                                        Description = description != null ? description.ToString().Remove(0, 7) : title
                                    });
                            }
                        }
                        else
                        {
                            throw new Exception("Marvel API returned a failure response code: " + response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // here log with Log4net or similar custom logger.
                var msg = ex.Message;
            }
        }

        public string MD5Hash(string text)
        {
            // https://www.thegeekyway.com/md5-hashing-using-c/

            MD5 md5H = MD5.Create();
            //convert the input string to a byte array and compute its hash
            byte[] data = md5H.ComputeHash(Encoding.UTF8.GetBytes(text));
            // create a new stringbuilder to collect the bytes and create a string
            StringBuilder sB = new StringBuilder();
            //loop through each byte of hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++)
            {
                sB.Append(data[i].ToString("x2"));
            }
            //return hexadecimal string
            return sB.ToString();
        }
    }
}
