using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * 8;
            ServicePointManager.Expect100Continue = false;

            var stopwatch = Stopwatch.StartNew();
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("FileStorageConnectionString"));
            var cloudFileClient = storageAccount.CreateCloudFileClient();
            var fileShare = cloudFileClient.GetShareReference("myshare");
            var rootDirectory = fileShare.GetRootDirectoryReference();
            var directory = rootDirectory.GetDirectoryReference("sourcedir");
            var cloudStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobStorageConnectionString"));
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var containerReference = cloudBlobClient.GetContainerReference("myblobcontainer");
            var cloudBlobDirectory = containerReference.GetDirectoryReference("targetdir");
            TransferManager.CopyDirectoryAsync(directory, cloudBlobDirectory, true, new CopyDirectoryOptions { Recursive = true }, new DirectoryTransferContext()).Wait();
            stopwatch.Stop();
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Copy files took {stopwatch.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }
    }
}
