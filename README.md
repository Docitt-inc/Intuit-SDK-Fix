# Intuit-SDK-Fix
This fixes the Intuit C# SDK to allow for overring of the OAuth Url <br />
https://www.nuget.org/packages/IPPAggCatDotNetDevKit __


Fix to allow overriding OAuth Url for use Financial Data API Façade from Finicity <br />
https://finicity.zendesk.com/hc/en-us/categories/201262086-Financial-Data-API-Facade-via-Finicity-Beta- <br />
<br />
## What Changed:
The Orginal SDK Code: <br />
>
    HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri("https://oauth.intuit.com/oauth/v1/get_access_token_by_saml")); <br />

This Removed the Hardcoded Address and allows for configuration via AppSettings in application config. <br />
>				// set key in your application settings appSettings for finicity
				//   <appSettings>
				//    <add key="OAUTHURL" value="https://api.finicity.com/oauth/v1/get_access_token_by_saml" />
				//   </appSettings>
                string str = ConfigurationManager.AppSettings["OAUTHURL"];
                if (string.IsNullOrEmpty(str))
                    str = "https://oauth.intuit.com/oauth/v1/get_access_token_by_saml";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(str));

## Finicity Setup
Follow the instructions here: <br />
https://finicity.zendesk.com/hc/en-us/articles/208775606-Finicity-Setup <br />
Add the following AppSettings to config along with the other settings. <br />
>   
    <add key="BaseUrl" value="https://api.finicity.com/financialdatafeed/v1" />
    <add key="OAUTHURL" value="https://api.finicity.com/oauth/v1/get_access_token_by_saml" />

### Application Changes Required.
Modify your usage in your application. From Reference: <br />

          >  //Demo purposes only.  The OAuth tokens returned by the SAML assertion are valid for 1 hour and do not need to be requested before each API call.
            var validator = new SamlRequestValidator(AggCatAppSettings.Certificate,
                                                                      AggCatAppSettings.ConsumerKey,
                                                                      AggCatAppSettings.ConsumerSecret,
                                                                      AggCatAppSettings.SamlIdentityProviderId,
                                                                      AggCatAppSettings.CustomerId);
            ServiceContext ctx = new ServiceContext(validator); <br />
            AggregationCategorizationService svc = new AggregationCategorizationService(ctx);

To: <br />
>            //Demo purposes only.  The OAuth tokens returned by the SAML assertion are valid for 1 hour and do not need to be requested before each API call.
            var validator = new FixedSamlValidator(AggCatAppSettings.Certificate,
                                                                      AggCatAppSettings.ConsumerKey,
                                                                      AggCatAppSettings.ConsumerSecret,
                                                                      AggCatAppSettings.SamlIdentityProviderId,
                                                                      AggCatAppSettings.CustomerId);
            ServiceContext ctx = new ServiceContext(validator);
            AggregationCategorizationService svc = new AggregationCategorizationService(ctx); 

This will allow the SDK to work correctly. <br />
