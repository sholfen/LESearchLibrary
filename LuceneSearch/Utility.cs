using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using LuceneSearch.Collections;
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

        public static AzureDirectory CreateAzureBaseDirectory()
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount cloudAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.DevelopmentStorageAccount;
            Microsoft.WindowsAzure.Storage.CloudStorageAccount.TryParse(
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting(UtilityKeyCollection.blobStorage), out cloudAccount);
            var cacheDirectory = new RAMDirectory();
            string container = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting(UtilityKeyCollection.ContainerName);
            AzureDirectory azureDirectory = new AzureDirectory(cloudAccount, container, cacheDirectory);

            return azureDirectory;
        }

        public static Lucene.Net.Store.Directory CreateLuceneDirectory(Utility.DirectoryType directoryType)
        {
            Lucene.Net.Store.Directory directory = null;
            switch (directoryType)
            {
                case Utility.DirectoryType.AzureBase:
                    directory = CreateAzureBaseDirectory();
                    break;
                case Utility.DirectoryType.FileBase:
                    directory = CreateFileBaseDirectory();
                    break;
                case Utility.DirectoryType.CustomizeFilePathBase:
                    directory = CreateCustomizeFilePathBaseDirectory();
                    break;
                default:
                    directory = CreateFileBaseDirectory();
                    break;
            }

            return directory;
        }

    }
}
