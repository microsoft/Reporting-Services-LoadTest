// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSAccessor.SoapAccessor
{
    public class RSTimeZoneInformation
    {
        public int Bias { get; set; }
        public int StandardBias { get; set; }
        public RSSYSTEMTIME StandardDate { get; set; }
        public int DaylightBias { get; set; }
        public RSSYSTEMTIME DaylightDate { get; set; }
    }

    public class RSSYSTEMTIME
    {
        public short year { get; set; }
        public short month { get; set; }
        public short dayOfWeek { get; set; }
        public short day { get; set; }
        public short hour { get; set; }
        public short minute { get; set; }
        public short second { get; set; }
        public short milliseconds { get; set; }
    }

    public class RSCacheRefreshPlan
    {

        public string CacheRefreshPlanID { get; set; }

        /// <remarks/>
        public string ItemPath { get; set; }

        /// <remarks/>
        public string Description { get; set; }

        /// <remarks/>
        public RSCacheRefreshPlanState State { get; set; }

        /// <remarks/>
        public System.DateTime LastExecuted { get; set; }

        /// <remarks/>
        public System.DateTime ModifiedDate { get; set; }

        /// <remarks/>
        public string ModifiedBy { get; set; }

        /// <remarks/>
        public string LastRunStatus { get; set; }
    }

    public class RSCacheRefreshPlanState
    {

        public bool MissingParameterValue { get; set; }

        /// <remarks/>
        public bool InvalidParameterValue { get; set; }

        /// <remarks/>
        public bool UnknownItemParameter { get; set; }

        /// <remarks/>
        public bool CachingNotEnabledOnItem { get; set; }
    }

    public class RSJob
    {


        public string JobID { get; set; }

        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public string Path { get; set; }

        /// <remarks/>
        public string Description { get; set; }

        /// <remarks/>
        public string Machine { get; set; }

        /// <remarks/>
        public string User { get; set; }

        /// <remarks/>
        public System.DateTime StartDateTime { get; set; }

        /// <remarks/>
        public string JobActionName { get; set; }
        /// <remarks/>
        public string JobTypeName { get; set; }

        /// <remarks/>
        public string JobStatusName { get; set; }
    }


    public class RSEvent
    {
        public string Type { get; set; }
    }

    public class RSExpirationDefinition
    {
    }

    public class RSScheduleExpiration : RSExpirationDefinition
    {
        public RSScheduleDefinitionOrReference Item { get; set; }
    }

    public class RSScheduleDefinition : RSScheduleDefinitionOrReference
    {

        public System.DateTime StartDateTime { get; set; }

        /// <remarks/>
        public System.DateTime EndDate { get; set; }

        /// <remarks/>
        public bool EndDateSpecified { get; set; }

        /// <remarks/>
        public RSRecurrencePattern Item { get; set; }
    }

    public class RSDailyRecurrence : RSRecurrencePattern
    {

        public int DaysInterval { get; set; }
    }

    public class RSRecurrencePattern
    {
    }


    public class RSWeeklyRecurrence : RSRecurrencePattern
    {

        public int WeeksInterval { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WeeksIntervalSpecified { get; set; }

        /// <remarks/>
        public RSDaysOfWeekSelector DaysOfWeek { get; set; }
    }

    public class RSDaysOfWeekSelector
    {
        public bool Sunday { get; set; }

        /// <remarks/>
        public bool Monday { get; set; }

        /// <remarks/>
        public bool Tuesday { get; set; }

        /// <remarks/>
        public bool Wednesday { get; set; }

        /// <remarks/>
        public bool Thursday { get; set; }

        /// <remarks/>
        public bool Friday { get; set; }
        /// <remarks/>
        public bool Saturday { get; set; }
    }

    public class RSMonthlyRecurrence : RSRecurrencePattern
    {
        public string Days { get; set; }
        /// <remarks/>
        public RSMonthsOfYearSelector MonthsOfYear { get; set; }
    }

    public class RSMonthsOfYearSelector
    {

        public bool January { get; set; }

        /// <remarks/>
        public bool February { get; set; }

        /// <remarks/>
        public bool March { get; set; }

        /// <remarks/>
        public bool April { get; set; }

        /// <remarks/>
        public bool May { get; set; }

        /// <remarks/>
        public bool June { get; set; }

        /// <remarks/>
        public bool July { get; set; }

        /// <remarks/>
        public bool August { get; set; }

        /// <remarks/>
        public bool September { get; set; }

        /// <remarks/>
        public bool October { get; set; }

        /// <remarks/>
        public bool November { get; set; }

        /// <remarks/>
        public bool December { get; set; }
    }

    public class RSMinuteRecurrence : RSRecurrencePattern
    {

        public int MinutesInterval { get; set; }
    }

    public class RSMonthlyDOWRecurrence : RSRecurrencePattern
    {
        public RSWeekNumberEnum WhichWeek { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RSWhichWeekSpecified { get; set; }

        /// <remarks/>
        public RSDaysOfWeekSelector DaysOfWeek { get; set; }

        /// <remarks/>
        public RSMonthsOfYearSelector MonthsOfYear { get; set; }
    }

    public enum RSWeekNumberEnum
    {

        /// <remarks/>
        FirstWeek,

        /// <remarks/>
        SecondWeek,

        /// <remarks/>
        ThirdWeek,

        /// <remarks/>
        FourthWeek,

        /// <remarks/>
        LastWeek,
    }

    public class RSScheduleDefinitionOrReference
    {
    }

    public class RSScheduleReference : RSScheduleDefinitionOrReference
    {

        public string ScheduleID { get; set; }

        /// <remarks/>
        public RSScheduleDefinition Definition { get; set; }
    }

    public partial class RSNoSchedule : RSScheduleDefinitionOrReference
    {
    }

    public class RSTimeExpiration : RSExpirationDefinition
    {
        public int Minutes { get; set; }
    }

    public class RSItemParameter
    {
        public string Name { get; set; }

        /// <remarks/>
        public string ParameterTypeName { get; set; }

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
        public string ParameterStateName { get; set; }

        /// <remarks/>
        public string ErrorMessage { get; set; }
    }

    public class RSSchedule
    {
        public string ScheduleID { get; set; }

        /// <remarks/>
        public string Name { get; set; }

        /// <remarks/>
        public RSScheduleDefinition Definition { get; set; }

        /// <remarks/>
        public string Description { get; set; }

        /// <remarks/>
        public string Creator { get; set; }

        /// <remarks/>
        public System.DateTime NextRunTime { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NextRunTimeSpecified { get; set; }

        /// <remarks/>
        public System.DateTime LastRunTime { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LastRunTimeSpecified { get; set; }

        /// <remarks/>
        public bool ReferencesPresent { get; set; }

        /// <remarks/>
        public string ScheduleStateName { get; set; }
    }


    public class RSModelPerspective
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class RSModelCatalogItem
    {
        public string Model { get; set; }
        public string Description { get; set; }
        public RSModelPerspective[] Perspectives { get; set; }
    }

    public class RSModelItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ModelItemTypeName { get; set; }
        public string Description { get; set; }
        public RSModelItem[] ModelItems { get; set; }
    }

    public class RSModelDrillthroughReport
    {
        public string Path { get; set; }
        public RSDrillthroughType Type { get; set; }
    }

    public enum RSDrillthroughType
    {

        /// <remarks/>
        Detail,

        /// <remarks/>
        List,
    }

    public class RSPolicy
    {
        public string GroupUserName { get; set; }
        public RSRole[] Roles { get; set; }
    }

    public class RSRole
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class RSTask
    {
        public string TaskID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class RSDataSource
    {
        public string Name { get; set; }
        public RSDataSourceDefinitionOrReference Item { get; set; }
    }

    public class RSDataSourceDefinition : RSDataSourceDefinitionOrReference
    {
        public string Extension { get; set; }
        public string ConnectString { get; set; }
        public bool UseOriginalConnectString { get; set; }
        public bool OriginalConnectStringExpressionBased { get; set; }
        public RSCredentialRetrievalEnum CredentialRetrieval { get; set; }
        public bool WindowsCredentials { get; set; }
        public bool ImpersonateUser { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ImpersonateUserSpecified { get; set; }
        public string Prompt { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EnabledSpecified { get; set; }
    }

    public enum RSCredentialRetrievalEnum
    {

        /// <remarks/>
        Prompt,

        /// <remarks/>
        Store,

        /// <remarks/>
        Integrated,

        /// <remarks/>
        None,
    }

    public class RSDataSourceDefinitionOrReference
    {
    }

    public class RSInvalidDataSourceReference : RSDataSourceDefinitionOrReference
    {
    }

    public class RSDataSourceReference : RSDataSourceDefinitionOrReference
    {
        public string Reference { get; set; }
    }

    public class RSSubscription
    {
        public string SubscriptionID { get; set; }
        public string Owner { get; set; }
        public string Path { get; set; }
        public string VirtualPath { get; set; }
        public string Report { get; set; }
        public RSExtensionSettings DeliverySettings { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public RSActiveState Active { get; set; }
        public System.DateTime LastExecuted { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LastExecutedSpecified { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public string EventType { get; set; }
        public bool IsDataDriven { get; set; }
    }

    public class RSExtensionSettings
    {
        public string Extension { get; set; }
        public RSParameterValueOrFieldReference[] ParameterValues { get; set; }
    }

    public class RSActiveState
    {
        public bool DeliveryExtensionRemoved { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DeliveryExtensionRemovedSpecified { get; set; }
        public bool SharedDataSourceRemoved { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SharedDataSourceRemovedSpecified { get; set; }
        public bool MissingParameterValue { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MissingParameterValueSpecified { get; set; }
        public bool InvalidParameterValue { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InvalidParameterValueSpecified { get; set; }
        public bool UnknownReportParameter { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool UnknownReportParameterSpecified { get; set; }
    }

    public class RSExtensionParameter
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Required { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool RequiredSpecified { get; set; }
        public bool ReadOnly { get; set; }
        public string Value { get; set; }
        public string Error { get; set; }
        public bool Encrypted { get; set; }
        public bool IsPassword { get; set; }
        [System.Xml.Serialization.XmlArrayItemAttribute("Value")]
        public RSValidValue[] ValidValues { get; set; }
    }

    public class RSQueryDefinition
    {
        public string CommandType { get; set; }
        public string CommandText { get; set; }
        public int Timeout { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TimeoutSpecified { get; set; }
    }

    public class RSField
    {
        public string Alias { get; set; }
        public string Name { get; set; }
    }

    public class RSDataSetDefinition
    {
        public RSField[] Fields { get; set; }
        public RSQueryDefinition Query { get; set; }
        public RSSensitivityEnum CaseSensitivity { get; set; }
        public bool CaseSensitivitySpecified { get; set; }
        public string Collation { get; set; }
        public RSSensitivityEnum AccentSensitivity { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AccentSensitivitySpecified { get; set; }
        public RSSensitivityEnum KanatypeSensitivity { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool KanatypeSensitivitySpecified { get; set; }
        public RSSensitivityEnum WidthSensitivity { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool WidthSensitivitySpecified { get; set; }
        public string Name { get; set; }
    }

    public enum RSSensitivityEnum
    {

        /// <remarks/>
        True,

        /// <remarks/>
        False,

        /// <remarks/>
        Auto,
    }

    public class RSDataRetrievalPlan
    {
        public RSDataSourceDefinitionOrReference Item { get; set; }
        public RSDataSetDefinition DataSet { get; set; }
    }

    public class RSItemReferenceData
    {
        public string Name { get; set; }
        public string Reference { get; set; }
        public string ReferenceType { get; set; }
    }

    public class RSItemReference
    {
        public string Name { get; set; }
        public string Reference { get; set; }
    }

    public class RSSearchCondition
    {
        public RSConditionEnum Condition { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ConditionSpecified { get; set; }
        [System.Xml.Serialization.XmlArrayItemAttribute("Value")]
        public string[] Values { get; set; }
        public string Name { get; set; }
    }

    public enum RSConditionEnum
    {

        /// <remarks/>
        Contains,

        /// <remarks/>
        Equals,

        /// <remarks/>
        In,

        /// <remarks/>
        Between,
    }

    public class RSItemHistorySnapshot
    {
        public string HistoryID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int Size { get; set; }
    }

    public class RSCatalogItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string VirtualPath { get; set; }
        public string TypeName { get; set; }
        public int Size { get; set; }
        public bool SizeSpecified { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        public bool HiddenSpecified { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool CreationDateSpecified { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool ModifiedDateSpecified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public RSProperty[] ItemMetadata { get; set; }
    }

    public class RSProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class RSItemNamespaceHeader // : System.Web.Services.Protocols.SoapHeader
    {
        public RSItemNamespaceEnum ItemNamespace { get; set; }
        public System.Xml.XmlAttribute[] AnyAttr { get; set; }
    }

    public enum RSItemNamespaceEnum
    {

        /// <remarks/>
        PathBased,

        /// <remarks/>
        GUIDBased,
    }

    public enum RSBooleanOperatorEnum
    {

        /// <remarks/>
        And,

        /// <remarks/>
        Or,
    }

    /// 2005 only /////////////////////////////////////////////////////////////////////////////////////
    /// In 2005, we have this value as enum while in later version, we have them as string.
    public enum RSExecutionSettingEnum
    {

        /// <remarks/>
        Live,

        /// <remarks/>
        Snapshot,
    }

    public enum RSSecurityScopeEnum
    {

        /// <remarks/>
        System,

        /// <remarks/>
        Catalog,

        /// <remarks/>
        Model,

        /// <remarks/>
        All,
    }

    // same as RSItemHostorySnapshot
    public class RSReportHistorySnapshot
    {
        public string HistoryID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int Size { get; set; }
    }
}
