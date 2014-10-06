using LuceneSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;

using System.Collections.Specialized;
using LuceneSearch.Modules;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using LuceneSearch.DataClass;
using Lucene.Net.Index;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SearchDemo
{
    class Video
    {
        public string VideoTitle { get; set; }
        public string Actors { get; set; }
    }
  
    class Program
    {
        static void Main(string[] args)
        {
            //EasyToUseExample();
            UsingConfigFileExample();
        }

        static void EasyToUseExample()
        {
            Video v1 = new Video { Actors = "汁波密", VideoTitle = "Video Title1" };
            Video v2 = new Video { Actors = "汁波密", VideoTitle = "Video Title2" };
            Video v3 = new Video { Actors = "汁波密", VideoTitle = "Video Title3" };

            Indexer.ResetIndex(Indexer.DirectoryType.FileBase);
            Indexer indexer = new Indexer(Indexer.DirectoryType.FileBase);
            indexer.AddItem<Video>(v1);
            indexer.AddItem<Video>(v2);
            indexer.AddItem<Video>(v3);

            indexer.Dispose();

            Searcher searcher = new Searcher(Searcher.DirectoryType.FileBase);
            List<dynamic> listResult = searcher.SearchDynamicItemsByQuery("汁波密", "Actors");
            foreach (dynamic videoItem in listResult)
            {
                Console.WriteLine(videoItem.VideoTitle);
                Console.WriteLine(videoItem.Actors);
                Console.WriteLine();
            }
        }

        static void UsingConfigFileExample()
        {
            string path = ConfigModule.GetIndexPath();
            Dictionary<string, FieldItem> setting = ConfigModule.GetJsonSetting("XMLFile1.xml", "aaa3");

            List<ExpandoObject> indexItemList = new List<ExpandoObject>();
            dynamic item = new ExpandoObject();
            item.VideoTitle = "Video Title";
            item.ProductNumber = Guid.NewGuid().ToString();
            item.Actors = "汁波密";
            item.Num = 3;
            indexItemList.Add(item);

            item = new ExpandoObject();
            item.VideoTitle = "Video Title2";
            item.ProductNumber = Guid.NewGuid().ToString();
            item.Actors = "汁波密";
            item.Num = 3;
            indexItemList.Add(item);

            Indexer.ResetIndex(Indexer.DirectoryType.FileBase);
            Indexer indexer = new Indexer(Indexer.DirectoryType.FileBase);
            indexer.StartIndexFormFieldItems(setting.Values.ToList(), indexItemList);

            Searcher searcher = new Searcher(Searcher.DirectoryType.FileBase);
            List<dynamic> listResult = searcher.SearchDynamicItemsByQuery(setting.Values.ToList(), "汁波密", "Actors");
            foreach (dynamic videoItem in listResult)
            {
                Console.WriteLine(videoItem.VideoTitle);
                Console.WriteLine(videoItem.ProductNumber);
                Console.WriteLine(videoItem.Actors);
                Console.WriteLine(videoItem.Num);
                Console.WriteLine();
            }
        }
    }
}
