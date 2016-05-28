# Intuit-SDK-Fix
This fixes the Intuiut C# SDK to allow for overriing of the OAuth Url <br />
Fix to allow overriding OAuth Url for use Financial Data API Facade from Finicity <br />
https://finicity.zendesk.com/hc/en-us/categories/201262086-Financial-Data-API-Facade-via-Finicity-Beta- <br />
<br />
## What Changed:
The Orginal SDK Code: <br />
    HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri("https://oauth.intuit.com/oauth/v1/get_access_token_by_saml")); <br />
<br />
This Removed the Hardcoded Address and allows for configuration via AppSettings in application config. <br />
				// Fixed Hardcoded Oauth Url Path.   <br />
				// set key in your application settings appSettings for finicity <br />
				//   <appSettings> <br />
				//    <add key="OAUTHURL" value="https://api.finicity.com/oauth/v1/get_access_token_by_saml" /> <br />
				//   </appSettings> <br />
                string str = ConfigurationManager.AppSettings["OAUTHURL"]; <br />
                if (string.IsNullOrEmpty(str)) <br />
                    str = "https://oauth.intuit.com/oauth/v1/get_access_token_by_saml"; <br />
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(str));  <br />

## Finicity Setup
Follow th instructions here: <br />
https://finicity.zendesk.com/hc/en-us/articles/208775606-Finicity-Setup <br />
Add the following AppSettings to config along with the other settings. <br />
    <add key="BaseUrl" value="https://api.finicity.com/financialdatafeed/v1" /> <br />
    <add key="OAUTHURL" value="https://api.finicity.com/oauth/v1/get_access_token_by_saml" /> <br />
    <br />
### Application Changes Required.
Modify your usage in your application. From Refernce: <br />
            //Demo purposes only.  The OAuth tokens returned by the SAML assertion are valid for 1 hour and do not need to be requested before each API call. <br />
            var validator = new SamlRequestValidator(AggCatAppSettings.Certificate, <br />
                                                                      AggCatAppSettings.ConsumerKey, <br />
                                                                      AggCatAppSettings.ConsumerSecret, <br />
                                                                      AggCatAppSettings.SamlIdentityProviderId, <br />
                                                                      AggCatAppSettings.CustomerId); <br />
            ServiceContext ctx = new ServiceContext(validator); <br />
            AggregationCategorizationService svc = new AggregationCategorizationService(ctx); <br />
To: <br />
            //Demo purposes only.  The OAuth tokens returned by the SAML assertion are valid for 1 hour and do not need to be requested before each API call. <br />
            var validator = new FixedSamlValidator(AggCatAppSettings.Certificate, <br />
                                                                      AggCatAppSettings.ConsumerKey, <br />
                                                                      AggCatAppSettings.ConsumerSecret, <br />
                                                                      AggCatAppSettings.SamlIdentityProviderId, <br />
                                                                      AggCatAppSettings.CustomerId); <br />
            ServiceContext ctx = new ServiceContext(validator); <br />
            AggregationCategorizationService svc = new AggregationCategorizationService(ctx); <br />
<br />
This will allow the SDK to work correctly. <br />
