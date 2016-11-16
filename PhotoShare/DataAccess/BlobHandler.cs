using System;
using System.Collections.Generic;
using System.Linq;
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



            // Retrieve reference to a previously created container.
            _container = _blobClient.GetContainerReference("photos");

            
        }

        public string Upload(HttpPostedFile file)
        {
            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(file.FileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromStream(file.InputStream);

            return blockBlob.Uri.ToString();
        }

        
    }
}