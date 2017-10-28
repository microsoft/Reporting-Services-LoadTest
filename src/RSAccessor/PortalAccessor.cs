// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RSAccessor.PortalAccessor
{
    public class PortalAccessor
    {
        private readonly string _reportServerApiUrl;

        public PortalAccessor(string reportServerPortalUrl)
        {
            _reportServerApiUrl = reportServerPortalUrl + "/api/v2.0";
        }

        public ICredentials ExecuteCredentials { get; set; }

        public HttpWebResponse DeleteFile(string path)
        {
            var query = string.Format("/CatalogItems(Path=%27{0}%27)", path); // TODO: path should be url encoded and single quotes should be doubled

            var webRequest = (HttpWebRequest)WebRequest.Create(_reportServerApiUrl + query);
            webRequest.Method = "DELETE";
            webRequest.Credentials = ExecuteCredentials ?? CredentialCache.DefaultNetworkCredentials;
            webRequest.Timeout = 1000 * 1000;

            return (HttpWebResponse)webRequest.GetResponse();
        }

        public HttpWebResponse UploadLargeFile(string file, string type, string path)
        {
            var query = string.Format("/{0}s(Path=%27{1}%27)/Model.Upload", type, path); // TODO: path should be url encoded and single quotes should be doubled
            var filePartName = "File";
            var contentType = "application/octet-stream";
            var parts = new Dictionary<string, string>
            {
                {"Name", type == "ExcelWorkbook" ? Path.GetFileName(file) : Path.GetFileNameWithoutExtension(file)},
                {"Path", path},
                {"@odata.type", string.Format("#Model.{0}", type)}
            };

            return UploadFileMultipartFormData(query, file, filePartName, contentType, parts);
        }

        private HttpWebResponse UploadFileMultipartFormData(string query, string file, string filePartName, string contentType, Dictionary<string, string> parts)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var webRequest = (HttpWebRequest)WebRequest.Create(_reportServerApiUrl + query);
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.Credentials = ExecuteCredentials ?? CredentialCache.DefaultNetworkCredentials;
            webRequest.Timeout = 1000 * 1000;

            Stream requestStream = webRequest.GetRequestStream();

            const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (var nameValuePair in parts)
            {
                requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                var formItem = string.Format(formdataTemplate, nameValuePair.Key, nameValuePair.Value);
                byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }
            requestStream.Write(boundarybytes, 0, boundarybytes.Length);

            const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            var header = string.Format(headerTemplate, filePartName, file, contentType);
            var headerbytes = Encoding.UTF8.GetBytes(header);
            requestStream.Write(headerbytes, 0, headerbytes.Length);

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024 * 1024];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
            }

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(trailer, 0, trailer.Length);
            requestStream.Close();

            return (HttpWebResponse)webRequest.GetResponse();
        }
    }
}