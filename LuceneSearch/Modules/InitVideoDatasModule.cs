using LuceneSearch.DataClass;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearch.Modules
{
    public class InitVideoDatasModule
    {
        public InitVideoDatasModule()
        {
        }


        public List<VideoItem> InitVideoDatas(string json)
        {
            List<VideoItem> videoList = new List<VideoItem>();
            JObject jsonObject = JsonConvert.DeserializeObject(json) as JObject;
            JToken ItemList = jsonObject["AddItemsToIndexRequest"]["ItemList"];
            foreach (var token in ItemList.Values<JToken>())
            {
                VideoItem videoItem = JsonConvert.DeserializeObject<VideoItem>(token.ToString());
                videoList.Add(videoItem);
            }

            return videoList;
        }
    }
}
