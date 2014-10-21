#LESearch Library#

***Easy Using Library for Search Engine***

**LESeach is a light weight search engine library that is base on Lucene.**

##Example##

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

You can use configuration file to define indexing behavior.

XML file:

	<?xml version="1.0" encoding="utf-8" ?>
	<IndexTypes>
	  <Type name="Test">
	    <Field ValueType="System.String" Name="VideoTitle" IsStore="True"></Field>
	    <Field ValueType="System.String" Name="ProductNumber" IsStore="True"></Field>
	    <Field ValueType="System.String[]" Name="Actors" IsStore="True"></Field>
	    <Field ValueType="System.DateTime" Name="PublishedDate" IsStore="True"></Field>
	    <Field ValueType="System.String" Name="Publisher" IsStore="True"></Field>
	    <Field ValueType="System.String" Name="ImgLink" IsStore="True"></Field>
	    <Field ValueType="System.String" Name="ThumbImgLink" IsStore="True"></Field>
	  </Type>
	  <Type name="aaa3">
	    <Field ValueType="System.String" Name="VideoTitle" IsStore="True"></Field>
	    <Field ValueType="System.String" Name="ProductNumber" IsStore="True"></Field>
	    <Field ValueType="System.String" Name="Actors" IsStore="True"></Field>
	    <Field ValueType="System.Int32" Name="Num" IsStore="True"></Field>
	  </Type>
	</IndexTypes>

Code:

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



#New Function#

*Paging*

The parameter pageIndex starts from 0.

	List<dynamic> Searcher.SearchDynamicItemsByQuery(
	string queryString, 
	string queryField, 
	int pageIndex, 
	int pageSize)

*Custom index file path*

Add this value in app.config.

	  <appSettings>
	    <add key="CustomizeIndexPath" value="D:\MyIndex"/>
	  </appSettings>

And modify the code.

	Indexer indexer = new Indexer(Utility.DirectoryType.CustomizeFilePathBase);

*Index Files Synchronization*

	void Indexer.SyncIndex(string[] dirs)