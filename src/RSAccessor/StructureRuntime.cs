// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

namespace RSAccessor.SoapAccessor
{
    public class RSServerInfoHeader
    {
        public string ReportServerVersionNumber { get; set; }
        public string ReportServerEdition { get; set; }
        public string ReportServerVersion { get; set; }
        public string ReportServerDateTime { get; set; }
        public System.Xml.XmlAttribute[] AnyAttr { get; set; }
    }

    public class RSPrintControlClsidHeader
    {
        public string Clsid32
        {
            get;
            set;
        }

        /// <remarks/>
        public string Clsid64
        {
            get;
            set;
        }

        public System.Xml.XmlAttribute[] AnyAttr
        {
            get;
            set;
        }
    }


    public class RSExtension
    {
        public RSExtensionTypeEnum ExtensionType { get; set; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public bool Visible { get; set; }
        public bool IsModelGenerationSupported { get; set; }
    }

    public enum RSExtensionTypeEnum
    {

        /// <remarks/>
        Delivery,

        /// <remarks/>
        Render,

        /// <remarks/>
        Data,

        /// <remarks/>
        All,
    }


    public class RSDocumentMapNode
    {

        /// <remarks/>
        public string Label { get; set; }

        /// <remarks/>
        public string UniqueName { get; set; }

        /// <remarks/>
        public RSDocumentMapNode[] Children { get; set; }
    }


    public class RSParameterValueOrFieldReference
    {
    }

    public class RSParameterValue : RSParameterValueOrFieldReference
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public class RSDataSourceCredentials
    {
        public string DataSourceName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RSWarning
    {
        public string Code { get; set; }
        public string Severity { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string Message { get; set; }
    }


    public class RSReportMargins
    {
        public double Top { get; set; }

        /// <remarks/>
        public double Bottom { get; set; }

        /// <remarks/>
        public double Left { get; set; }

        /// <remarks/>
        public double Right { get; set; }
    }

    public class RSReportPaperSize
    {

        public double Height { get; set; }

        /// <remarks/>
        public double Width { get; set; }
    }

    public class RSPageSettings
    {

        public RSReportPaperSize PaperSize { get; set; }

        public RSReportMargins Margins { get; set; }
    }


    public class RSDataSourcePrompt
    {
        public string Name { get; set; }
        public string DataSourceID { get; set; }
        public string Prompt { get; set; }
    }

    public class RSValidValue
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class RSReportParameter
    {
        public string Name { get; set; }

        /// <remarks/>
        public RSParameterTypeEnum Type { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TypeSpecified { get; set; }

        /// <remarks/>
        public bool Nullable { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NullableSpecified { get; set; }

        /// <remarks/>
        public bool AllowBlank { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AllowBlankSpecified { get; set; }

        /// <remarks/>
        public bool MultiValue { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MultiValueSpecified { get; set; }

        /// <remarks/>
        public bool QueryParameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QueryParameterSpecified { get; set; }
        /// <remarks/>
        public string Prompt { get; set; }
        /// <remarks/>
        public bool PromptUser { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PromptUserSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Dependency")]
        public string[] Dependencies { get; set; }

        /// <remarks/>
        public bool ValidValuesQueryBased { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ValidValuesQueryBasedSpecified { get; set; }

        /// <remarks/>
        public RSValidValue[] ValidValues { get; set; }

        /// <remarks/>
        public bool DefaultValuesQueryBased { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DefaultValuesQueryBasedSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value")]
        public string[] DefaultValues { get; set; }

        /// <remarks/>
        public RSParameterStateEnum State { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StateSpecified { get; set; }

        /// <remarks/>
        public string ErrorMessage { get; set; }
    }

    public enum RSParameterTypeEnum
    {

        /// <remarks/>
        Boolean,

        /// <remarks/>
        DateTime,

        /// <remarks/>
        Integer,

        /// <remarks/>
        Float,

        /// <remarks/>
        String,
    }

    public enum RSParameterStateEnum
    {

        /// <remarks/>
        HasValidValue,

        /// <remarks/>
        MissingValidValue,

        /// <remarks/>
        HasOutstandingDependencies,

        /// <remarks/>
        DynamicValuesUnavailable,
    }

    public partial class RSExecutionInfo
    {

        public bool HasSnapshot { get; set; }

        /// <remarks/>
        public bool NeedsProcessing { get; set; }

        /// <remarks/>
        public bool AllowQueryExecution { get; set; }

        public bool CredentialsRequired { get; set; }

        /// <remarks/>
        public bool ParametersRequired { get; set; }

        /// <remarks/>
        public System.DateTime ExpirationDateTime { get; set; }

        /// <remarks/>
        public System.DateTime ExecutionDateTime { get; set; }

        /// <remarks/>
        public int NumPages { get; set; }

        /// <remarks/>
        public RSReportParameter[] Parameters { get; set; }

        /// <remarks/>
        public RSDataSourcePrompt[] DataSourcePrompts { get; set; }

        /// <remarks/>
        public bool HasDocumentMap { get; set; }

        /// <remarks/>
        public string ExecutionID { get; set; }

        /// <remarks/>
        public string ReportPath { get; set; }

        /// <remarks/>
        public string HistoryID { get; set; }

        /// <remarks/>
        public RSPageSettings ReportPageSettings { get; set; }

        /// <remarks/>
        public int AutoRefreshInterval { get; set; }
    }


    public class RSExecutionInfo2 : RSExecutionInfo
    {
        public RSPageCountMode PageCountMode { get; set; }
    }

    public enum RSPageCountMode
    {

        /// <remarks/>
        Actual,

        /// <remarks/>
        Estimate,
    }

    public class RSExecutionHeader
    {

        public string ExecutionID { get; set; }

        public System.Xml.XmlAttribute[] AnyAttr { get; set; }
    }

    public class RSTrustedUserHeader
    {

        public string UserName { get; set; }

        public byte[] UserToken { get; set; }

        public System.Xml.XmlAttribute[] AnyAttr { get; set; }
    }

    public enum RSSortDirectionEnum
    {

        /// <remarks/>
        None,

        /// <remarks/>
        Ascending,

        /// <remarks/>
        Descending,
    }
}
