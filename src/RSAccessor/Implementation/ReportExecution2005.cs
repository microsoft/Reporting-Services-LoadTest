// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System.Net;
using Microsoft.SqlServer.ReportingServices2005.Execution;

namespace RSAccessor.SoapAccessor
{
    public class ReportExecution2005 : IRSExecution
    {
        ReportExecutionService rs = new ReportExecutionService();
        SoapStructureConvert m_converter = null;
        
        /// <remarks/>
        public ReportExecution2005(string serverUrl, ICredentials executeCredentials)
        {
            rs.Url = string.Format("{0}/ReportExecution2005.asmx", serverUrl);
            rs.Credentials = executeCredentials;
            m_converter = new SoapStructureConvert(this.AssemblyName, this.Namespace);
        }

        #region Implementation Specific

        public SoapStructureConvert Converter
        {
            get
            {
                return (m_converter);
            }
        }

        public string Xmlns
        {
            get
            {
                return @"http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices";
            }
        }

        public string AssemblyName
        {
            get
            {
                return rs.GetType().Assembly.FullName;
            }
        }

        public string Namespace
        {
            get
            {
                return (rs.GetType().Namespace);
            }
        }

        #endregion

        public RSTrustedUserHeader TrustedUserHeaderValue
        {
            get
            {
                return ((RSTrustedUserHeader)Converter.Convert(rs.TrustedUserHeaderValue));
            }
            set
            {
                rs.TrustedUserHeaderValue = (TrustedUserHeader)Converter.Convert(value);
            }
        }
        public RSServerInfoHeader ServerInfoHeaderValue
        {
            get
            {
                return ((RSServerInfoHeader)Converter.Convert(rs.ServerInfoHeaderValue));
            }
            set
            {
                rs.ServerInfoHeaderValue = (ServerInfoHeader)Converter.Convert(value);
            }
        }

        public RSExecutionHeader ExecutionHeaderValue
        {
            get
            {
                return ((RSExecutionHeader)Converter.Convert(rs.ExecutionHeaderValue));
            }
            set
            {
                rs.ExecutionHeaderValue = (ExecutionHeader)Converter.Convert(value);
            }
        }

        public string[] ListSecureMethods()
        {
            return (rs.ListSecureMethods());
        }

        public RSExecutionInfo LoadReport(string Report, string HistoryID)
        {
            ExecutionInfo outval = rs.LoadReport(Report, HistoryID);
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 LoadReport2(string Report, string HistoryID)
        {
            ExecutionInfo2 outval = rs.LoadReport2(Report, HistoryID);
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public RSExecutionInfo LoadReportDefinition([System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, out RSWarning[] warnings)
        {
            Warning[] warns = null;
            ExecutionInfo outval = rs.LoadReportDefinition(Definition, out warns);
            warnings = (RSWarning[])Converter.Convert(warns);
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 LoadReportDefinition2([System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, out RSWarning[] warnings)
        {
            Warning[] warns = null;
            ExecutionInfo2 outval = rs.LoadReportDefinition2(Definition, out warns);
            warnings = (RSWarning[])Converter.Convert(warns);
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public RSExecutionInfo SetExecutionCredentials(RSDataSourceCredentials[] Credentials)
        {
            DataSourceCredentials[] dsCredentials = (DataSourceCredentials[])Converter.Convert(Credentials);
            ExecutionInfo outval = rs.SetExecutionCredentials(dsCredentials);
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 SetExecutionCredentials2(RSDataSourceCredentials[] Credentials)
        {
            DataSourceCredentials[] dsCredentials = (DataSourceCredentials[])Converter.Convert(Credentials);
            ExecutionInfo2 outval = rs.SetExecutionCredentials2(dsCredentials);
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public RSExecutionInfo SetExecutionParameters(RSParameterValue[] Parameters, string ParameterLanguage)
        {
            ParameterValue[] paramValues = (ParameterValue[])Converter.Convert(Parameters);
            ExecutionInfo outval = rs.SetExecutionParameters(paramValues, ParameterLanguage);
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 SetExecutionParameters2(RSParameterValue[] Parameters, string ParameterLanguage)
        {
            ParameterValue[] paramValues = (ParameterValue[])Converter.Convert(Parameters);
            ExecutionInfo2 outval = rs.SetExecutionParameters2(paramValues, ParameterLanguage);
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public RSExecutionInfo ResetExecution()
        {
            ExecutionInfo outval = rs.ResetExecution();
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 ResetExecution2()
        {
            ExecutionInfo2 outval = rs.ResetExecution2();
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public byte[] Render(string Format, string DeviceInfo, out string Extension, out string MimeType, out string Encoding, out RSWarning[] Warnings, out string[] StreamIds)
        {
            Warning[] warns = null;
            byte[] outval = rs.Render(Format, DeviceInfo, out Extension, out MimeType, out Encoding, out warns, out StreamIds);
            Warnings = (RSWarning[])Converter.Convert(warns);
            return (outval);
        }

        public byte[] Render2(string Format, string DeviceInfo, RSPageCountMode PaginationMode, out string Extension, out string MimeType, out string Encoding, out RSWarning[] Warnings, out string[] StreamIds)
        {
            Warning[] warns = null;
            PageCountMode paginatiionMode = (PageCountMode)Converter.Convert(PaginationMode);
            byte[] outval = rs.Render2(Format, DeviceInfo, paginatiionMode, out Extension, out MimeType, out Encoding, out warns, out StreamIds);
            Warnings = (RSWarning[])Converter.Convert(warns);
            return (outval);
        }

        public byte[] RenderStream(string Format, string StreamID, string DeviceInfo, out string Encoding, out string MimeType)
        {
            byte[] outval = rs.RenderStream(Format, StreamID, DeviceInfo, out Encoding, out MimeType);
            return (outval);
        }

        public RSExecutionInfo GetExecutionInfo()
        {
            ExecutionInfo outval = rs.GetExecutionInfo();
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 GetExecutionInfo2()
        {
            ExecutionInfo2 outval = rs.GetExecutionInfo2();
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public RSDocumentMapNode GetDocumentMap()
        {
            DocumentMapNode outval = rs.GetDocumentMap();
            return ((RSDocumentMapNode)Converter.Convert(outval));
        }

        public RSExecutionInfo LoadDrillthroughTarget(string DrillthroughID)
        {
            ExecutionInfo outval = rs.LoadDrillthroughTarget(DrillthroughID);
            return ((RSExecutionInfo)Converter.Convert(outval));
        }

        public RSExecutionInfo2 LoadDrillthroughTarget2(string DrillthroughID)
        {
            ExecutionInfo2 outval = rs.LoadDrillthroughTarget2(DrillthroughID);
            return ((RSExecutionInfo2)Converter.Convert(outval));
        }

        public bool ToggleItem(string ToggleID)
        {
            return (rs.ToggleItem(ToggleID));
        }

        public int NavigateDocumentMap(string DocMapID)
        {
            return (rs.NavigateDocumentMap(DocMapID));
        }

        public int NavigateBookmark(string BookmarkID, out string UniqueName)
        {
            return (rs.NavigateBookmark(BookmarkID, out UniqueName));
        }

        public int FindString(int StartPage, int EndPage, string FindValue)
        {
            return (rs.FindString(StartPage, EndPage, FindValue));
        }

        public int Sort(string SortItem, RSSortDirectionEnum Direction, bool Clear, out string ReportItem, out int NumPages)
        {
            SortDirectionEnum direction = (SortDirectionEnum)Converter.Convert(Direction);
            return (rs.Sort(SortItem, direction, Clear, out ReportItem, out NumPages));
        }

        public int Sort2(string SortItem, RSSortDirectionEnum Direction, bool Clear, RSPageCountMode PaginationMode, out string ReportItem, out RSExecutionInfo2 ExecutionInfo)
        {
            SortDirectionEnum direction = (SortDirectionEnum)Converter.Convert(Direction);
            PageCountMode paginationMode = (PageCountMode)Converter.Convert(PaginationMode);
            ExecutionInfo2 execInfo = null;
            int outval = rs.Sort2(SortItem, direction, Clear, paginationMode, out ReportItem, out execInfo);
            ExecutionInfo = (RSExecutionInfo2)Converter.Convert(execInfo);
            return (outval);
        }

        public byte[] GetRenderResource(string Format, string DeviceInfo, out string MimeType)
        {
            return (rs.GetRenderResource(Format, DeviceInfo, out MimeType));
        }

        public RSExtension[] ListRenderingExtensions()
        {
            Extension[] extensions = rs.ListRenderingExtensions();
            return (RSExtension[])Converter.Convert(extensions);
        }

        public void LogonUser(string userName, string password, string authority)
        {
            rs.LogonUser(userName, password, authority);
        }

        public void Logoff()
        {
            rs.Logoff();
        }
    }
}
