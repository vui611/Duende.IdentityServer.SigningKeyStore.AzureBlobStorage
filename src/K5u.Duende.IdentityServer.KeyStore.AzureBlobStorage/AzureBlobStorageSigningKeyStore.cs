using System.Diagnostics;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace K5u.Duende.IdentityServer.KeyStore.AzureBlobStorage
{
    /// <summary>
    /// 
    /// </summary>
    internal class AzureBlobStorageSigningKeyStore:ISigningKeyStore
    {
        private readonly ILogger<AzureBlobStorageSigningKeyStore> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public AzureBlobStorageSigningKeyStore(ILogger<AzureBlobStorageSigningKeyStore> logger, IOptions<AzureBlobStorageSigningKeyStoreOption> azureOptions)
        {
            _logger = logger;
            try
            {
                var options = azureOptions.Value;
                if (!string.IsNullOrEmpty(options.SasUrl))
                {
                    _blobContainerClient = new BlobContainerClient(new Uri(options.SasUrl));
                }
                else
                {
                    var blobServiceClient = new BlobServiceClient(options.StorageConnectionString);
                    _blobContainerClient = blobServiceClient.GetBlobContainerClient(options.Container);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error while initializing the Signing Key Store using Azure Blob Storage");
                _logger.LogError(e.Message);
                throw;
            }
            
        }

        /// <summary>
        /// Service name for store traces
        /// </summary>
        private static string Store => "Duende.IdentityServer.Stores";

        /// <summary>
        /// Service version
        /// </summary>
        private static string ServiceVersion => "1.0.0";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SerializedKey>> LoadKeysAsync()
        {
            new ActivitySource(Store, ServiceVersion)
                .StartActivity("SigningKeyStore.LoadKeys");
            var result = new List<SerializedKey>();
            try
            {
                // List all blobs in the container
                await foreach (var blobItem in _blobContainerClient.GetBlobsAsync())
                {
                    var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
                    var downloadedData = downloadResult.Content.ToString();
                    var key = JsonConvert.DeserializeObject<SerializedKey>(downloadedData);
                    if(key!=null)
                        result.Add(key);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error while retrieving the list of signing keys from Azure Blob Storage");
                _logger.LogError(e.Message);
                throw;
            }
            return result.AsEnumerable();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task StoreKeyAsync(SerializedKey key)
        {
            new ActivitySource(Store, ServiceVersion)
                .StartActivity("SigningKeyStore.StoreKey");
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(key.Id);
                var serializedKey = JsonConvert.SerializeObject(key);
                await blobClient.UploadAsync(BinaryData.FromString(serializedKey), overwrite: true);
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error while storing the key to Azure Blob Storage");
                _logger.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DeleteKeyAsync(string id)
        {
            new ActivitySource(Store, ServiceVersion)
                .StartActivity("SigningKeyStore.DeleteKey");
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(id);
                await blobClient.DeleteAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error while deleting the key from Azure Blob Storage");
                _logger.LogError(e.Message);
                throw;
            }
        }

    }
}
