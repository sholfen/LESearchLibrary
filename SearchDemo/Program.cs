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

            //
        }
    }
}
