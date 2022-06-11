using Microsoft.Extensions.DependencyInjection;

namespace K5u.Duende.IdentityServer.KeyStore.AzureBlobStorage.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdentityServerBuilderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddAzureBlobStorageSigningKeyStore(this IIdentityServerBuilder builder, Action<AzureBlobStorageSigningKeyStoreOption> action)
        {
            builder.Services.Configure(action);
            builder.AddSigningKeyStore<AzureBlobStorageSigningKeyStore>();
            return builder;
        }
    }
}
