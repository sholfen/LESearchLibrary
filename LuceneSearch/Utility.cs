using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Store;
using LuceneSearch.Modules;

namespace LuceneSearch
{
    public class Utility
    {
        public enum DirectoryType { AzureBase, FileBase, CustomizeFilePathBase }

        private Utility()
        {
        }

        public static FSDirectory CreateFileBaseDirectory()
        {
            string indexPath = AppDomain.CurrentDomain.BaseDirectory + ConfigModule.GetIndexPath();
            FSDirectory dir = FSDirectory.Open(new DirectoryInfo(indexPath));

            return dir;
        }

        public static FSDirectory CreateFileBaseDirectory(string path)
        {
            string indexPath = path;
            FSDirectory dir = FSDirectory.Open(new DirectoryInfo(indexPath));

            return dir;
        }

        public static FSDirectory CreateCustomizeFilePathBaseDirectory()
        {
            string indexPath = ConfigModule.GetCustomizeIndexPath();
            return CreateFileBaseDirectory(indexPath);
        }
    }
}
