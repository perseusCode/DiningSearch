using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;
using System.Runtime.Serialization;
using HAckathon;
using Lucene.Net.Store.Azure;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // Get the xml data from blob storage:
            List<Entry> entries = GetEntryListFromBlob();

            // Generate Lucene Index.

             this.GenerateLuceneIndex(entries);
           
        }

      

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        List<Entry> GetEntryListFromBlob()
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(List<Entry>));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=diningsearchstorage;AccountKey=xeYMzXThFxrU7SsAMGbSWLdy9psLFRMk5NI8x0bx24xtEg9MPIstf/xwPdjvDm6HpHZaCPxxVFCv/7DDd5wymA==");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("diningsearch");
            container.CreateIfNotExist();

            CloudBlob blob = container.GetBlobReference("MyBlob");

            MemoryStream newStream = new MemoryStream();
            blob.DownloadToStream(newStream);

            newStream.Seek(0, SeekOrigin.Begin);

            ////string blobString = StreamToString(newStream);

            ////newStream.Seek(0, SeekOrigin.Begin);

            var deserialized = ser.ReadObject(newStream);

            return (List<Entry>)deserialized;
        }

        string StreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private void GenerateLuceneIndex(List<Entry> entries)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=diningsearchstorage;AccountKey=xeYMzXThFxrU7SsAMGbSWLdy9psLFRMk5NI8x0bx24xtEg9MPIstf/xwPdjvDm6HpHZaCPxxVFCv/7DDd5wymA==");
            AzureDirectory azureDirectory = new AzureDirectory(storageAccount, "diningsearchindex");
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            IndexWriter indexWriter = new IndexWriter(azureDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            ////
            int c = 0;
            foreach (var entry in entries)
            {
                c++;
                var item = new Document();
                item.Add(new Field("Id", c.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                item.Add(new Field("Dish", entry.DishName, Field.Store.YES, Field.Index.ANALYZED));
                item.Add(new Field("Cafe/Restaurant", string.Format("{0}/{1}", entry.CafeName, entry.RestaurantName), Field.Store.YES, Field.Index.ANALYZED));
                item.Add(new Field("URL", entry.CafeUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
                item.Add(new Field("Description", entry.Description ?? string.Empty, Field.Store.YES, Field.Index.ANALYZED));
                item.Add(new Field("Price", entry.Price, Field.Store.YES, Field.Index.NOT_ANALYZED));
                indexWriter.AddDocument(item);
            }

           

            ////
            
            indexWriter.Dispose();
            azureDirectory.Dispose();
        }
    }
}
