namespace Intuit.Ipp.DataAggregation.Security.Extensions
{
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    using Intuit.Ipp.DataAggregation.Exception;
    using Intuit.Ipp.DataAggregation.Security;

    public class FixedSamlValidator : IRequestValidator
    {
        private OAuthRequestValidator OauthValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSamlValidator"/> class. 
        /// Makes Saml Request and initialize a new instance of the
        ///     <see cref="T:Intuit.Ipp.DataAggregation.Security.SamlRequestValidator"/> class.
        /// </summary>
        public FixedSamlValidator(
            X509Certificate2 certificate, 
            string consumerKey, 
            string consumerSecert, 
            string issuerId, 
            string subject)
        {
            try
            {
                certificate.GetPublicKeyString();
            }
            catch (CryptographicException ex)
            {
                throw new InvalidTokenException("CertificateNullMsg");
            }

            if (string.IsNullOrWhiteSpace(consumerKey))
            {
                throw new InvalidTokenException("ConsumerKeyNullMsg");
            }

            if (string.IsNullOrWhiteSpace(consumerSecert))
            {
                throw new InvalidTokenException("ConsumerKeySecretNullMsg");
            }

            if (string.IsNullOrWhiteSpace(issuerId))
            {
                throw new InvalidTokenException("IssuerIdNullMsg");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new InvalidTokenException("SubjectNullMsg");
            }

            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecert;
            this.Certificate = certificate;
            this.IssuerId = issuerId;
            this.Subject = subject;
        }

        public X509Certificate2 Certificate { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string IssuerId { get; set; }

        public string Subject { get; set; }

        public void Authorize(WebRequest webRequest, string requestBody)
        {
            if (this.OauthValidator == null)
            { 
				// Use the FixedSamlUtils instead of the standard Intuit SamlUtils
                IDictionary<string, string> accessToken = FixedSamlUtils.getAccessToken(
                    this.IssuerId, 
                    this.Certificate, 
                    this.ConsumerKey, 
                    this.Subject);
                this.OauthValidator = new OAuthRequestValidator(
                    accessToken["oauth_token"], 
                    accessToken["oauth_token_secret"], 
                    this.ConsumerKey, 
                    this.ConsumerSecret);
            }

            this.OauthValidator.Authorize(webRequest, requestBody);
        }
    }
}