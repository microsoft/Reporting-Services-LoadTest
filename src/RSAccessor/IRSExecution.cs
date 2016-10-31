// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System.Net;

namespace RSAccessor.SoapAccessor
{
    public interface IRSExecution
    {

        #region Implementation Specific
        string Xmlns { get; }
        string AssemblyName { get; }
        string Namespace { get; }
        SoapStructureConvert Converter { get; }
        #endregion

        RSTrustedUserHeader TrustedUserHeaderValue
        {
            get;
            set;
        }
        RSServerInfoHeader ServerInfoHeaderValue
        {
            get;
            set;
        }

        RSExecutionHeader ExecutionHeaderValue
        {
            get;
            set;
        }


        string[] ListSecureMethods();
        RSExecutionInfo LoadReport(string Report, string HistoryID);
        RSExecutionInfo2 LoadReport2(string Report, string HistoryID);
        RSExecutionInfo LoadReportDefinition([System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, out RSWarning[] warnings);
        RSExecutionInfo2 LoadReportDefinition2([System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, out RSWarning[] warnings);
        RSExecutionInfo SetExecutionCredentials(RSDataSourceCredentials[] Credentials);
        RSExecutionInfo2 SetExecutionCredentials2(RSDataSourceCredentials[] Credentials);
        RSExecutionInfo SetExecutionParameters(RSParameterValue[] Parameters, string ParameterLanguage);
        RSExecutionInfo2 SetExecutionParameters2(RSParameterValue[] Parameters, string ParameterLanguage);
        RSExecutionInfo ResetExecution();
        RSExecutionInfo2 ResetExecution2();
        byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out RSWarning[] Warnings, out string[] StreamIds);
        byte[] Render2(string Format, string DeviceInfo, RSPageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out RSWarning[] Warnings, out string[] StreamIds);
        byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType);
        RSExecutionInfo GetExecutionInfo();
        RSExecutionInfo2 GetExecutionInfo2();
        RSDocumentMapNode GetDocumentMap();
        RSExecutionInfo LoadDrillthroughTarget(string DrillthroughID);
        RSExecutionInfo2 LoadDrillthroughTarget2(string DrillthroughID);
        bool ToggleItem(string ToggleID);
        int NavigateDocumentMap(string DocMapID);
        int NavigateBookmark(string BookmarkID, out string UniqueName);
        int FindString(int StartPage, int EndPage, string FindValue);
        int Sort(string SortItem, RSSortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages);
        int Sort2(string SortItem, RSSortDirectionEnum Direction, bool Clear, RSPageCountMode PaginationMode, out string ReportItem, out RSExecutionInfo2 ExecutionInfo);
        byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType);
        RSExtension[] ListRenderingExtensions();
        void LogonUser(string userName, string password, string authority);
        void Logoff();
    }

}
