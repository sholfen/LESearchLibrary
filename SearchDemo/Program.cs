﻿using LuceneSearch;
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
using System.Configuration;

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
            SyncIndexFilesExample();
            //EasyToUseExample();
            //UsingConfigFileExample();
        }

        static void SyncIndexFilesExample()
        {
            Video v1 = new Video { Actors = "汁波密", VideoTitle = "Video Title1" };
            Video v2 = new Video { Actors = "汁波密", VideoTitle = "Video Title2" };
            Video v3 = new Video { Actors = "汁波密", VideoTitle = "Video Title3" };

            Indexer.ResetIndex(Utility.DirectoryType.CustomizeFilePathBase);
            Indexer indexer = new Indexer(Utility.DirectoryType.CustomizeFilePathBase);
            indexer.AddItem<Video>(v1);
            indexer.AddItem<Video>(v2);
            indexer.AddItem<Video>(v3);
            indexer.Dispose();

            Video v4 = new Video { Actors = "汁波密", VideoTitle = "Video Title4" };
            Video v5 = new Video { Actors = "汁波密", VideoTitle = "Video Title5" };
            Video v6 = new Video { Actors = "汁波密", VideoTitle = "Video Title6" };
            List<Video> list = new List<Video>();
            list.Add(v4);
            list.Add(v5);
            list.Add(v6);
            Indexer.ResetIndex(Utility.DirectoryType.FileBase);
            indexer = new Indexer(Utility.DirectoryType.FileBase);
            indexer.AddItemList<Video>(list);
            string path = ConfigurationManager.AppSettings["CustomizeIndexPath"];
            indexer.SyncIndex(new string[] { path });
            indexer.Dispose();

            Searcher searcher = new Searcher(Utility.DirectoryType.FileBase);
            List<dynamic> listResult = searcher.SearchDynamicItemsByQuery("汁波密", "Actors");
            foreach (dynamic videoItem in listResult)
            {
                Console.WriteLine(videoItem.VideoTitle);
                Console.WriteLine(videoItem.Actors);
                Console.WriteLine();
            }
        }

        static void EasyToUseExample()
        {
            Video v1 = new Video { Actors = "汁波密", VideoTitle = "Video Title1" };
            Video v2 = new Video { Actors = "汁波密", VideoTitle = "Video Title2" };
            Video v3 = new Video { Actors = "汁波密", VideoTitle = "Video Title3" };
            List<Video> list = new List<Video>();
            list.Add(v1);
            list.Add(v2);
            list.Add(v3);

            Indexer.ResetIndex(Utility.DirectoryType.FileBase);
            Indexer indexer = new Indexer(Utility.DirectoryType.FileBase);
            indexer.AddItem<Video>(v1);
            indexer.AddItem<Video>(v2);
            indexer.AddItem<Video>(v3);
            //indexer.AddItemList<Video>(list);

            indexer.Dispose();

            Searcher searcher = new Searcher(Utility.DirectoryType.FileBase);
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

            Indexer.ResetIndex(Utility.DirectoryType.FileBase);
            Indexer indexer = new Indexer(Utility.DirectoryType.FileBase);
            indexer.StartIndexFormFieldItems(setting.Values.ToList(), indexItemList);

            Searcher searcher = new Searcher(Utility.DirectoryType.FileBase);
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
