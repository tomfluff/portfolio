﻿using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Diagnostics;

namespace SunnyDay.Backend.Controllers
{
    [Authorize]
    [MobileAppController]
    public class GetStorageTokenController : ApiController
    {
        public string ConnectionString { get; set; }
        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }

        public GetStorageTokenController()
        {
            ConnectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_MS_AzureStorageAccountConnectionString", EnvironmentVariableTarget.Process);
            Debug.WriteLine($"[GetStorageTokenController$init] Connection String = {ConnectionString}");
            StorageAccount = CloudStorageAccount.Parse(ConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        // GET api/GetStorageToken
        public async Task<StorageTokenElement> Get(string name)
        {
            // Get the container name for the user
            Debug.WriteLine($"[GetStorageTokenController] Get()");
            var claimsPrincipal = User as ClaimsPrincipal;
            var sid = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(4); // strip off the sid: from the front
            string containerName = $"container-{sid}";
            Debug.WriteLine($"[GetStorageTokenController] Container Name = {containerName}");


            // Create the container if it does not yet exist
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            Debug.WriteLine($"[GetStorageTokenController] Got Container Reference");
            // This will throw a StorageException, which results in a 500 Internal Server Error on the outside
            try
            {
                await container.CreateIfNotExistsAsync();
                Debug.WriteLine($"[GetStorageTokenController] Container is created");
            }
            catch (StorageException ex)
            {
                Debug.WriteLine($"[GetStorageTokenController] Cannot create container: {ex.Message}");
            }

            // Create a blob URI - based on a GUID
            var blobName = name;
            Debug.WriteLine($"[GetStorageTokenController] Blob Name = {blobName}");
            var blob = container.GetBlockBlobReference(blobName);
            Debug.WriteLine($"[GetStorageTokenController] Got Blob Reference");

            // Create a policy for the blob access
            var mainBlobPolicy = new SharedAccessBlobPolicy
            {
                // Set start time to five minutes before now to avoid clock skew.
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                // Allow Access for the next 60 minutes (according to Azure)
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30),
                // Allow read, write and create permissions
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
            };
            // Create a policy for the blob access
            var readBlobPolicy = new SharedAccessBlobPolicy
            {
                // Set start time to five minutes before now to avoid clock skew.
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                // Allow Access for the next 60 minutes (according to Azure)
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30),
                // Allow read, write and create permissions
                Permissions = SharedAccessBlobPermissions.Read
            };
            Debug.WriteLine($"[GetStorageTokenController] Got Blob SAS Policy");

            return new StorageTokenElement
            {
                Name = blobName,
                Uri = blob.Uri,
                MainSasToken = blob.GetSharedAccessSignature(mainBlobPolicy),
                ReadSasToken = blob.GetSharedAccessSignature(readBlobPolicy)
            };
        }
    }

    public class StorageTokenElement
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public string MainSasToken { get; set; }
        public string ReadSasToken { get; set; }
    }
}