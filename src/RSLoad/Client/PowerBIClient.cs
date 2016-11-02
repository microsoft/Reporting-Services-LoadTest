// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Linq;
using System.Net;
using Microsoft.OData.Client;
using RSAccessor.PortalAccessor;
using RSAccessor.PortalAccessor.OData.Model;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using RSTest.Common.ReportServer.Information;

namespace RSLoad
{
    internal class PowerBIClient 
    {
        private static string _endpoint = ReportServerInformation.DefaultInformation.PowerBIUrl;

        private const string _modelsAndExplorationEndpoint = "/{0}/modelsAndExploration";
        private const string _conceptualSchemaEndpoint = "/{0}/conceptualSchema";
        private const string _queryDataEndpoint = "/{0}/querydata";

        public static void SimulatePowerBIReportUsage(ICredentials credentials, PowerBIReport report)
        {
            var modelId = GetModelsAndExplorations(credentials, report);
            GetConceptualSchema(credentials, report, modelId);
            RunQuerySet(credentials, report, modelId);
        }

        private static string GetModelsAndExplorations(ICredentials credentials, PowerBIReport report)
        {
            WebRequest req = HttpWebRequest.Create(string.Format(_endpoint + _modelsAndExplorationEndpoint, report.Id));
            req.Method = WebRequestMethods.Http.Get;

            req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            req.Credentials = credentials;
            WebResponse response = req.GetResponse();

            string responseBody = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseBody = sr.ReadToEnd();
            }

            var json = JObject.Parse(responseBody);

            return json["models"][0]["id"].Value<string>();
        }

        private static void GetConceptualSchema(ICredentials credentials, PowerBIReport report, string modelId)
        {
            WebRequest request = HttpWebRequest.Create(string.Format(_endpoint + _conceptualSchemaEndpoint, report.Id));

            request.Method = WebRequestMethods.Http.Post;
            request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            request.Credentials = credentials;
            request.ContentType = "application/json";

            var payload = JObject.Parse(string.Format("{{ modelIds: [ {0} ] }}", modelId));
            
            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(payload.ToString());

                sw.Flush();
                sw.Close();
            }

            WebResponse resp2 = request.GetResponse();
        }

        private static void RunQuerySet(ICredentials credentials, PowerBIReport report, string modelId)
        {
            var payloads = GetQueryPayload(report, modelId);

            foreach (var payload in payloads)
            {
                WebRequest request = HttpWebRequest.Create(string.Format(_endpoint + _queryDataEndpoint, report.Id));

                request.Method = WebRequestMethods.Http.Post;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                request.Credentials = credentials;
                request.ContentType = "application/json";

                using (var sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(payload.ToString());

                    sw.Flush();
                    sw.Close();
                }

                WebResponse response = request.GetResponse();
            }
        }

        private static List<JObject> GetQueryPayload(PowerBIReport report, string modelId)
        {
            List<JObject> queries = new List<JObject>();

            foreach (var file in Directory.GetFiles(string.Format(SharedConstants.RuntimeResourcesFolder + @"\PowerBI\Queries\{0}\", report.Name)))
            {
                var query = JObject.Parse(File.ReadAllText(file));
                query["modelId"] = modelId;
                queries.Add(query);
            }

            return queries;
        }
    }
}