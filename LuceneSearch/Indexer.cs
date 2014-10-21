using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

using Lucene;
using Lucene.Net;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Analysis.Standard;
using LuceneSearch.Modules;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Lucene.Net.Store.Azure;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using LuceneSearch.DataClass;
using System.Reflection;

namespace LuceneSearch
{
    public class Indexer
    {    
        private IndexWriter indexWriter;
		private bool isDispose;
        private Utility.DirectoryType directoryType;

        public Indexer(Utility.DirectoryType directoryType)
        {
            this.directoryType = directoryType;
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

            this.indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), false, IndexWriter.MaxFieldLength.LIMITED);
            this.indexWriter.SetRAMBufferSizeMB(100.0);
        }

		public void Dispose()
		{
			if (!this.isDispose) {
                this.indexWriter.Dispose();
				this.isDispose = true;
			} else {
				throw new Exception ("Already Dispose");
			}
		}

        public static void ResetIndex(Utility.DirectoryType directoryType)
        {
            IndexWriter indexWriter = null;
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

            try
            {
                indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);
                indexWriter.SetRAMBufferSizeMB(100.0);
                indexWriter.Optimize();
                indexWriter.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                indexWriter.Dispose();
            }
        }

        public void StartIndexFormFieldItems(List<FieldItem> fieldItemList, List<ExpandoObject> indexItemList)
        {
            try
            {
                if (this.isDispose)
                {
                    throw new Exception("Already Dispose");
                }

                foreach (ExpandoObject item in indexItemList)
                {
                    Document doc = new Document();
                    var dicItem = (IDictionary<String, Object>)item;
                    foreach (FieldItem fieldItem in fieldItemList)
                    {
                        Type type = fieldItem.DataType;
                        Field.Store isSotre = Field.Store.NO;
                        if (fieldItem.IsStore) isSotre = Field.Store.YES;
                        AbstractField field = null;

                        if (type == typeof(string))
                        {
                            field = new Field(
                                fieldItem.FieldName,
                                dicItem[fieldItem.FieldName].ToString(),
                                isSotre, Field.Index.ANALYZED,
                                Field.TermVector.NO);
                        }
                        else if (type.IsArray)
                        {
                            
                        }
                        else if (type.IsValueType)
                        {
                            field = new NumericField(
                                fieldItem.FieldName,
                                isSotre,
                                true);

                            if (type == typeof(int))
                            {
                                ((NumericField)field).SetIntValue((int)dicItem[fieldItem.FieldName]);
                            }
                            else if (type == typeof(double))
                            {
                                ((NumericField)field).SetDoubleValue((double)dicItem[fieldItem.FieldName]);
                            }
                            else if (type == typeof(float))
                            {
                                ((NumericField)field).SetFloatValue((float)dicItem[fieldItem.FieldName]);
                            }
                        }
                        if (field == null)
                        {
                            throw new Exception("Fied Type Not Supported");
                        }

                        doc.Add(field);
                    }

                    this.indexWriter.AddDocument(doc);
                }

                //save data
                this.indexWriter.Optimize();
                this.indexWriter.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
         
        }

        private Dictionary<string, List<FieldItem>> typeCache;
        /// <summary>
        /// add data and index it.
        /// </summary>
        /// <typeparam name="T">input data's data type</typeparam>
        /// <param name="item">data that you wnat to index</param>
        public void AddItem<T>(T item)
        {
            if (this.typeCache == null) this.typeCache = new Dictionary<string, List<FieldItem>>();
            List<FieldItem> listTypeSetting = null;
            Type type = typeof(T);
            if (this.typeCache.ContainsKey(type.FullName))
            {
                listTypeSetting = this.typeCache[type.FullName];
            }
            else
            {
                listTypeSetting = this.CreateTypeSetting(type);
            }

            //create fields
            Document doc = this.AddDataToIndex<T>(listTypeSetting, type, item); 

            this.indexWriter.AddDocument(doc);
            this.indexWriter.Optimize();
            this.indexWriter.Commit();
        }

        public void AddItemList<T>(List<T> list)
        {
            if (this.typeCache == null) this.typeCache = new Dictionary<string, List<FieldItem>>();
            List<FieldItem> listTypeSetting = null;
            Type type = typeof(T);
            if (this.typeCache.ContainsKey(type.FullName))
            {
                listTypeSetting = this.typeCache[type.FullName];
            }
            else
            {
                listTypeSetting = this.CreateTypeSetting(type);
            }

            foreach (var item in list)
            {
                //create fields
                Document doc = this.AddDataToIndex<T>(listTypeSetting, type, item);
                this.indexWriter.AddDocument(doc);
            }

            this.indexWriter.Optimize();
            this.indexWriter.Commit();
        }

        private Document AddDataToIndex<T>(List<FieldItem> listTypeSetting, Type type, T item)
        {
            Document doc = new Document();
            foreach (FieldItem fieldItem in listTypeSetting)
            {
                Type fieldDataType = fieldItem.DataType;
                PropertyInfo propertyInfo = type.GetProperty(fieldItem.FieldName);
                AbstractField field = null;
                object propertyValue = propertyInfo.GetValue(item);

                if (fieldDataType == typeof(string))
                {
                    field = new Field(
                        fieldItem.FieldName,
                        propertyValue.ToString(),
                        Field.Store.YES, Field.Index.ANALYZED,
                        Field.TermVector.NO);
                }
                else if (type.IsValueType)
                {
                    field = new NumericField(
                        fieldItem.FieldName,
                        Field.Store.YES,
                        true);

                    if (type == typeof(int))
                    {
                        ((NumericField)field).SetIntValue((int)propertyValue);
                    }
                    else if (type == typeof(double))
                    {
                        ((NumericField)field).SetDoubleValue((double)propertyValue);
                    }
                    else if (type == typeof(float))
                    {
                        ((NumericField)field).SetFloatValue((float)propertyValue);
                    }
                }
                if (field == null)
                {
                    throw new Exception("Fied Type Not Supported");
                }

                doc.Add(field);
            }
            return doc;
        }

        private List<FieldItem> CreateTypeSetting(Type type)
        {
            List<FieldItem> listTypeSetting = new List<FieldItem>();
            this.typeCache.Add(type.FullName, listTypeSetting);

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    FieldItem fieldItem = new FieldItem();
                    fieldItem.DataType = property.PropertyType;
                    fieldItem.FieldName = property.Name;
                    fieldItem.IsStore = true;
                    listTypeSetting.Add(fieldItem);
                }
            }
            return listTypeSetting;
        }

        public void SyncIndex(string[] dirs)
        {
            if (this.directoryType == Utility.DirectoryType.FileBase ||
                this.directoryType == Utility.DirectoryType.CustomizeFilePathBase)
            {
                if (dirs.Length > 5)
                {
                    throw new Exception("directories are too many");
                }
                else
                {
                    FSDirectory[] fsdirs = new FSDirectory[dirs.Length];
                    for (int i = 0; i < dirs.Length; ++i)
                    {
                        fsdirs[i] = FSDirectory.Open(dirs[i]);
                    }
                    this.indexWriter.AddIndexesNoOptimize(fsdirs);
                    this.indexWriter.Optimize();
                }
            }
            else
            {
                throw new Exception("Not Support");
            }
        }
    }
}
