// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Net;
using RSAccessor.PortalAccessor.OData.Model;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using RSTest.Common.ReportServer.Information;

namespace RSLoad
{
    internal static class ExcelClient 
    {
        public static void SimulateExcelWorkbookRendering(ICredentials credentials, CatalogItem catalogItem)
        {
            HttpResponseMessage responseMessage = GetExcelWorkbook(credentials, catalogItem).Result;

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception(responseMessage.Content.ReadAsStringAsync().Result);
            }
        }

        private static async Task<HttpResponseMessage> GetExcelWorkbook(ICredentials credentials, CatalogItem catalogItem)
        {
            var portalUrl = new Uri(ReportServerInformation.DefaultInformation.ReportPortalUrl);
            var oosRootUrl = new Uri(ReportServerInformation.DefaultInformation.OosUrl);

            var rootUrl = new Uri(portalUrl.GetLeftPart(UriPartial.Authority));
            var wopiSrc = new Uri(rootUrl, $"wopi/files/{catalogItem.Id}");
            var accessToken = await GetAccessToken(credentials, catalogItem);

            var iframeUrl = $"{oosRootUrl}/x/_layouts/xlviewerinternal.aspx?WOPISrc={WebUtility.UrlEncode(wopiSrc.ToString())}&access_token={WebUtility.UrlEncode(accessToken)}";

            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetAsync(iframeUrl);
            }
        }

        private static async Task<string> GetAccessToken(ICredentials credentials, CatalogItem catalogItem)
        {
            var accessTokenUrl = $"{ReportServerInformation.DefaultInformation.ReportPortalUrl}/api/v2.0/CatalogItems({catalogItem.Id})/Model.AccessToken";

            var webRequest = WebRequest.Create(accessTokenUrl);
            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            webRequest.Credentials = credentials;

            var response = await webRequest.GetResponseAsync();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return JObject.Parse(sr.ReadToEnd())["Token"].Value<string>();
            }
        }
    }
}