namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.GenerateLink.Hyperlink
{
    using System;
    using System.Web;
    using Diagnostics;
    using Sitecore.Modules.EmailCampaign.Core.Crypto;
    using Sitecore.Modules.EmailCampaign.Core.Extensions;
    using Sitecore.Modules.EmailCampaign.Core.Pipelines.GenerateLink;

    /// <summary>
    /// Processor which generates modified hyperlink.
    /// </summary>
    public class EncryptQueryString : GenerateLinkProcessor
    {
        /// <summary>
        /// An instance to perform encryption and decryption.
        /// </summary>
        private readonly QueryStringEncryption queryStringEncryption;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptQueryString"/> class.
        /// </summary>
        /// <param name="queryStringEncryption">An instance to perform encryption and decryption.</param>
        public EncryptQueryString([NotNull] QueryStringEncryption queryStringEncryption)
        {
            Assert.ArgumentNotNull(queryStringEncryption, "queryStringEncryption");

            this.queryStringEncryption = queryStringEncryption;
        }

        /// <summary>Processes the arguments and generates modified hyperlink.</summary>
        /// <param name="args">The <see cref="GenerateLinkPipelineArgs"/> arguments.</param>
        public override void Process(GenerateLinkPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.GeneratedUrl == null)
            {
                return;
            }

            // Extract the query string and encrypt it.
            var uri = new UriBuilder(args.GeneratedUrl);

            var query = HttpUtility.ParseQueryString(uri.Query);
            var encryptedQuery = this.queryStringEncryption.Encrypt(query);

            uri.Query = encryptedQuery.ToQueryString(false);

            // Remove implicit port numbers (E.g. to avoid ":80" to be added to HTTP URLs).
            if ((uri.Scheme.Equals("http") && uri.Port == 80)
                      || (uri.Scheme.Equals("https") && uri.Port == 443)
                      || (uri.Scheme.Equals("mailto") && uri.Port == 25))
            {
                uri.Port = -1;
            }

            args.GeneratedUrl = uri.ToString();
        }
    }
}