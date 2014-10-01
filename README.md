##LESearch Library##

***Easy Using Library for Search Engine***

**LESeach is a light weight search engine library that is base on Lucene.**

*Example:*

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
