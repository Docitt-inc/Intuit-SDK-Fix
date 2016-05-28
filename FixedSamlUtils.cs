namespace Intuit.Ipp.DataAggregation.Security.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    using Intuit.Ipp.DataAggregation.Security.SamlTokens;

    public class FixedSamlUtils
    {
        public static IDictionary<string, string> getAccessToken(string issuerId, X509Certificate2 certificate, string consumerKey, string subject)
        {
            string[] strArray = FixedSamlUtils.getSamlResponse(issuerId, certificate, consumerKey, subject).Split(new char[2]
            {
        '&',
        '='
            });
            IDictionary<string, string> dictionary = (IDictionary<string, string>)new Dictionary<string, string>();
            dictionary[strArray[0]] = strArray[1];
            dictionary[strArray[2]] = strArray[3];
            return dictionary;
        }

        public static string getSamlResponse(string issuerId, X509Certificate2 certificate, string consumerKey, string subject)
        {
            string requestDataEncoded = SamlHelper.Encode64(SamlHelper.CreateSamlAssertion(issuerId, issuerId, subject, certificate));
            return FixedSamlUtils.PostSamlAssertion(consumerKey, requestDataEncoded);
        }

        protected static string PostSamlAssertion(string consumerKey, string requestDataEncoded)
        {
            try
            {
				// Fixed Hardcoded Oauth Url Path.  
				// set key in your application settings appSettings for finicity
				//   <appSettings>
				//    <add key="OAUTHURL" value="https://api.finicity.com/oauth/v1/get_access_token_by_saml" />
				//   </appSettings>
                string str = ConfigurationManager.AppSettings["OAUTHURL"];
                if (string.IsNullOrEmpty(str))
                    str = "https://oauth.intuit.com/oauth/v1/get_access_token_by_saml";

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(str));
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Headers.Add("Authorization", "OAuth oauth_consumer_key=\"" + consumerKey + "\"");
                byte[] bytes = Encoding.UTF8.GetBytes("saml_assertion=" + requestDataEncoded);
                httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
                return new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), Encoding.ASCII).ReadToEnd();
            }
            catch (WebException ex)
            {
                string message = "WebException: " + ex.Message + Environment.NewLine;
                if (ex.Response != null && ex.Response.Headers != null)
                {
                    foreach (string index in ex.Response.Headers.AllKeys)
                        message = message + index + ": " + ex.Response.Headers[index] + Environment.NewLine;
                }
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message + Environment.NewLine);
            }
        }
    }
}
