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
        private const int MaxResultsSize = 1000;

        private IndexSearcher indexSearcher;
        private bool isDispose;
        private Utility.DirectoryType directoryType;

        public Searcher(Utility.DirectoryType directoryType)
        {
            this.directoryType = directoryType;
            this.InitDirectory(directoryType);
        }

        private void InitDirectory(Utility.DirectoryType directoryType)
        {
            Lucene.Net.Store.Directory directory = null;
            switch (directoryType)
            {
                case Utility.DirectoryType.AzureBase:
                    directory = Utility.CreateAzureBaseDirectory();
                    break;
                case Utility.DirectoryType.FileBase:
                    directory = Utility.CreateFileBaseDirectory();
                    break;
                case Utility.DirectoryType.CustomizeFilePathBase:
                    directory = Utility.CreateCustomizeFilePathBaseDirectory();
                    break;
                default:
                    directory = Utility.CreateFileBaseDirectory();
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

        public List<dynamic> SearchDynamicItemsByQuery(List<FieldItem> fieldItemList, string queryString, string queryField)
        {
            List<dynamic> listSearchResults = new List<dynamic>();
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, queryField, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            Query query = parser.Parse(queryString);
            ScoreDoc[] hits = this.indexSearcher.Search(query, null, MaxResultsSize).ScoreDocs;

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

        /// <summary>
        /// search data
        /// </summary>
        /// <param name="queryString">query</param>
        /// <param name="queryField">data field</param>
        /// <returns></returns>
        public List<dynamic> SearchDynamicItemsByQuery(string queryString, string queryField)
        {
            List<dynamic> listSearchResults = new List<dynamic>();
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, queryField, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            Query query = parser.Parse(queryString);
            ScoreDoc[] hits = this.indexSearcher.Search(query, 1000).ScoreDocs;

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

        /// <summary>
        /// search function that provide paging.
        /// </summary>
        /// <param name="queryString">query</param>
        /// <param name="queryField">data field</param>
        /// <param name="pageIndex">start from 0</param>
        /// <param name="pageSize">result size of one page</param>
        /// <returns></returns>
        public List<dynamic> SearchDynamicItemsByQuery(string queryString, string queryField, int pageIndex, int pageSize)
        {
            //check pageIndex and pageSize first
            int skipCount = pageIndex * pageSize;
            int resultIndexTail = skipCount + pageSize;
            List<dynamic> listSearchResults = new List<dynamic>();
            if (skipCount >= MaxResultsSize)
            {
                return listSearchResults;
            }


            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, queryField, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            Query query = parser.Parse(queryString);
            ScoreDoc[] hits = this.indexSearcher.Search(query, MaxResultsSize).ScoreDocs;

            int resultLength = Math.Min(hits.Length, resultIndexTail);
            for (int i = skipCount; i < resultLength; i++)
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

        public void ReloadIndexFiles()
        {
            this.Dispose();
            this.InitDirectory(this.directoryType);
        }
    }
}
