using System;
using System.IO;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PhotoShare.DataAccess
{
    public class BlobHandler
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _container;

        public BlobHandler(string username)
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            _blobClient = _storageAccount.CreateCloudBlobClient();

            _container = _blobClient.GetContainerReference(username.ToLower());

            _container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
        }

        public string Upload(Stream fileStream, string fileName)
        {
            //The file will be corrupted if not read from the beginning
            fileStream.Position = 0;
            
            var blockBlob = _container.GetBlockBlobReference(fileName);
            
            blockBlob.UploadFromStream(fileStream);

            return blockBlob.Uri.ToString();
        }
    }
}