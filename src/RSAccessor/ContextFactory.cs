// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.OData.Client;

namespace RSAccessor.PortalAccessor
{
    public static class ContextFactory
    {
        private const string MeRequest = "{0}/me";
        public const string XsrfToken = "X-XSRF-Token";
        private const string CookieHeader = "Cookie";

        private readonly static CookieContainer _cookieContainer = new CookieContainer();
        private static string _reportServerPortalUrl;
        private static bool _isInitialized = false;

        public static CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
        }

        public static void InitializeContainer(string reportServerPortalUrl, ICredentials credentials, DataServiceContext container)
        {
            _reportServerPortalUrl = reportServerPortalUrl;
            var executeCredentials = credentials ?? CredentialCache.DefaultNetworkCredentials;
            InitSession(executeCredentials);

            container.SendingRequest2 += (sender, e) =>
            {
                var request = (HttpWebRequestMessage)e.RequestMessage;
                request.Credentials = executeCredentials;
                request.HttpWebRequest.CookieContainer = _cookieContainer;
            };
            container.BuildingRequest += (sender, e) =>
            {
                var uri = e.RequestUri;
                e.Headers.Add(XsrfToken, GetXsrfToken(uri));

                var path = uri.ToString().Substring(container.BaseUri.ToString().Length);

                // Fix CatalogItemByPath path param
                if (path.Contains("/CatalogItemByPath"))
                {
                    var uriBuilder = new UriBuilder(container.BaseUri.ToString());
                    var match = Regex.Match(path, @"/CatalogItemByPath\(path=('.*?')\)");
                    if (match.Success)
                    {
                        uriBuilder.Path += "/CatalogItemByPath(path=@path)";
                        uriBuilder.Query = string.Format("@path={0}", match.Groups[1].Value)
                                           + (string.IsNullOrEmpty(uri.Query) ? string.Empty : "&" + uri.Query.Substring(1));
                    }
                    e.RequestUri = uriBuilder.Uri;
                }

                // Fix Resource content GET
                else if (path.EndsWith("/Model.Resource/Content"))
                {
                    e.RequestUri = new Uri(string.Format("{0}/$value", e.RequestUri.ToString()));
                }
            };
        }

        public static NameValueCollection GetHeaders()
        {
            var uri = new Uri(_reportServerPortalUrl);
            var result = new NameValueCollection(2);
            result.Add(XsrfToken, GetXsrfToken(uri));
            result.Add(CookieHeader, _cookieContainer.GetCookieHeader(uri));

            return result;
        }

        private static void InitSession(ICredentials credentials)
        {
            if (_isInitialized)
                return;

            var request = (HttpWebRequest)WebRequest.Create(string.Format(CultureInfo.InvariantCulture, MeRequest, _reportServerPortalUrl));
            request.CookieContainer = _cookieContainer;
            request.Credentials = credentials;

            using (var response = request.GetResponse())
            {
                // Starting a Me-request to get CSRF token for the subsequent requests
            }
            _isInitialized = true;
        }

        public static string GetXsrfToken(Uri uri)
        {
            var cookies = _cookieContainer.GetCookies(uri);
            var cookie = cookies["XSRF-TOKEN"];
            if (cookie == null)
                return null;

            return WebUtility.UrlDecode(cookie.Value);
        }
    }
}
