using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using LuceneSearch.Modules;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearch
{
    public class Searcher
    {
        public enum DirectoryType { AzureBase, FileBase }

        private IndexSearcher indexSearcher;
        private bool isDispose;

        public Searcher(DirectoryType directoryType)
        {
            Lucene.Net.Store.Directory directory = null;
            switch (directoryType)
            {
                case DirectoryType.AzureBase:
                    directory = CreateAzureBaseDirectory();
                    break;
                case DirectoryType.FileBase:
                    directory = CreateFileBaseDirectory();
                    break;
                default:
                    directory = CreateFileBaseDirectory();
                    break;
            }

            this.indexSearcher = new IndexSearcher(directory, true);
        }

        public void Dispose()
        {
            if (!this.isDispose)
            {
                this.indexSearcher.Dispose();
                this.isDispose = true;
            }
            else
            {
                throw new Exception("Already Dispose");
            }
        }

        public static AzureDirectory CreateAzureBaseDirectory()
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount cloudAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.DevelopmentStorageAccount;
            Microsoft.WindowsAzure.Storage.CloudStorageAccount.TryParse(Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("blobStorage"), out cloudAccount);
            var cacheDirectory = new RAMDirectory();
            string container = "LuceneNetIndex";
#if DEBUG
            container = "LuceneDevMode";
#endif
            AzureDirectory azureDirectory = new AzureDirectory(cloudAccount, container, cacheDirectory);

            return azureDirectory;
        }

        public static FSDirectory CreateFileBaseDirectory()
        {
            string indexPath = AppDomain.CurrentDomain.BaseDirectory + ConfigModule.GetIndexPath();
            FSDirectory dir = FSDirectory.Open(new DirectoryInfo(indexPath));

            return dir;
        }

        public List<dynamic> SearchDynamicItemsByQuery(List<FieldItem> fieldItemList, string queryString, string queryField)
        {
            List<dynamic> listSearchResults = new List<dynamic>();
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, queryField, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            Query query = parser.Parse(queryString);
            ScoreDoc[] hits = this.indexSearcher.Search(query, null, 1000).ScoreDocs;

            for (int i = 0; i < hits.Length; i++)
            {
                Document hitDoc = this.indexSearcher.Doc(hits[i].Doc);
                dynamic item = new ExpandoObject();
                var dicItem = (IDictionary<String, Object>)item;

                foreach (FieldItem fieldItem in fieldItemList)
                {
                    string fieldName = fieldItem.FieldName;
                    Field field = hitDoc.GetField(fieldName);
                    dicItem.Add(fieldName, hitDoc.Get(fieldName));
                }

                listSearchResults.Add(item);
            }

            return listSearchResults;
        }

        public List<dynamic> SearchDynamicItemsByQuery(string queryString, string queryField)
        {
            List<dynamic> listSearchResults = new List<dynamic>();
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, queryField, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            Query query = parser.Parse(queryString);
            ScoreDoc[] hits = this.indexSearcher.Search(query, null, 1000).ScoreDocs;

            for (int i = 0; i < hits.Length; i++)
            {
                Document hitDoc = this.indexSearcher.Doc(hits[i].Doc);
                dynamic item = new ExpandoObject();
                var dicItem = (IDictionary<String, Object>)item;

                foreach (Field docFieldItem in hitDoc.GetFields())
                {
                    dicItem.Add(docFieldItem.Name, hitDoc.Get(docFieldItem.Name));
                }

                listSearchResults.Add(item);
            }

            return listSearchResults;
        }
    }
}
