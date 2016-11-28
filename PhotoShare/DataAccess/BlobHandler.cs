using System;
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

        public BlobHandler()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public string Upload(HttpPostedFile file, string fileName, string username)
        {
            //Sets access to read blob only
            const BlobContainerPublicAccessType publicAccess = BlobContainerPublicAccessType.Blob;
            var container = _blobClient.GetContainerReference(username.ToLower());
            
            container.CreateIfNotExists(publicAccess);
            
            var blockBlob = container.GetBlockBlobReference(fileName);

            //The file will be corrupted if not read from the beginning
            file.InputStream.Position = 0;
            
            blockBlob.UploadFromStream(file.InputStream);

            return blockBlob.Uri.ToString();
        }
    }
}