namespace K5u.Duende.IdentityServer.KeyStore.AzureBlobStorage
{
    /// <summary>
    /// 
    /// </summary>
    public class AzureBlobStorageSigningKeyStoreOption
    {
        /// <summary>
        /// SAS Url of the Azure blob container that stores the keys
        /// </summary>
        public string SasUrl { get; set; } = null!;

        /// <summary>
        /// Name of the container that stores the key
        /// </summary>
        public string Container { get; set; } = null!;

        /// <summary>
        /// Storage connection string, skipped if SAS url is used
        /// </summary>
        public string StorageConnectionString { get; set; } = null!;
    }
}
