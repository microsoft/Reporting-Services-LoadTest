// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSAccessor.SoapAccessor
{
    public interface IRSManagement
    {
        #region Implementation Specific
        string Url { get; }
        string Xmlns { get; }
        string AssemblyName { get; }
        string Namespace { get; }
        SoapStructureConvert Converter { get; }
        #endregion

        RSTrustedUserHeader TrustedUserHeaderValue { get; set; }

        RSServerInfoHeader ServerInfoHeaderValue { get; set; }

        RSItemNamespaceHeader ItemNamespaceHeaderValue { get; set; }

        RSCatalogItem CreateCatalogItem(string ItemType, string Name, string Parent, bool Overwrite, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, RSProperty[] Properties, out RSWarning[] Warnings);
        RSWarning[] SetItemDefinition(string ItemPath, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, RSProperty[] Properties);
        byte[] GetItemDefinition(string ItemPath);
        string GetItemType(string ItemPath);
        void DeleteItem(string ItemPath);
        void MoveItem(string ItemPath, string Target);
        void InheritParentSecurity(string ItemPath);
        RSItemHistorySnapshot[] ListItemHistory(string ItemPath);
        RSCatalogItem[] ListChildren(string ItemPath, bool Recursive);
        RSCatalogItem[] ListDependentItems(string ItemPath);
        RSCatalogItem[] FindItems(string Folder, RSBooleanOperatorEnum BooleanOperator, RSProperty[] SearchOptions, RSSearchCondition[] SearchConditions);
        RSCatalogItem[] ListParents(string ItemPath);
        RSCatalogItem CreateFolder(string Folder, string Parent, RSProperty[] Properties);
        void SetProperties(string ItemPath, RSProperty[] Properties);
        RSProperty[] GetProperties(string ItemPath, RSProperty[] Properties);
        void SetItemReferences(string ItemPath, RSItemReference[] ItemReferences);
        RSItemReferenceData[] GetItemReferences(string ItemPath, string ReferenceItemType);
        string[] ListItemTypes();
        void SetSubscriptionProperties(string SubscriptionID, RSExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData, RSParameterValue[] Parameters);
        string GetSubscriptionProperties(string SubscriptionID, out RSExtensionSettings ExtensionSettings, out string Description, out RSActiveState Active, out string Status, out string EventType, out string MatchData, out RSParameterValue[] Parameters);
        void SetDataDrivenSubscriptionProperties(string DataDrivenSubscriptionID, RSExtensionSettings ExtensionSettings, RSDataRetrievalPlan DataRetrievalPlan, string Description, string EventType, string MatchData, RSParameterValueOrFieldReference[] Parameters);
        string GetDataDrivenSubscriptionProperties(string DataDrivenSubscriptionID, out RSExtensionSettings ExtensionSettings, out RSDataRetrievalPlan DataRetrievalPlan, out string Description, out RSActiveState Active, out string Status, out string EventType, out string MatchData, out RSParameterValueOrFieldReference[] Parameters);
        void DeleteSubscription(string SubscriptionID);
        string CreateSubscription(string ItemPath, RSExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData, RSParameterValue[] Parameters);
        string CreateDataDrivenSubscription(string ItemPath, RSExtensionSettings ExtensionSettings, RSDataRetrievalPlan DataRetrievalPlan, string Description, string EventType, string MatchData, RSParameterValueOrFieldReference[] Parameters);
        RSExtensionParameter[] GetExtensionSettings(string Extension);
        RSExtensionParameter[] ValidateExtensionSettings(string Extension, RSParameterValueOrFieldReference[] ParameterValues, string SiteUrl);
        RSSubscription[] ListSubscriptions(string ItemPathOrSiteURL);
        RSSubscription[] ListMySubscriptions(string ItemPathOrSiteURL);
        RSSubscription[] ListSubscriptionsUsingDataSource(string DataSource);
        void ChangeSubscriptionOwner(string SubscriptionID, string NewOwner);
        RSCatalogItem CreateDataSource(string DataSource, string Parent, bool Overwrite, RSDataSourceDefinition Definition, RSProperty[] Properties);
        RSDataSetDefinition PrepareQuery(RSDataSource DataSource, RSDataSetDefinition DataSet, out bool Changed, out string[] ParameterNames);
        void EnableDataSource(string DataSource);
        void DisableDataSource(string DataSource);
        void SetDataSourceContents(string DataSource, RSDataSourceDefinition Definition);
        RSDataSourceDefinition GetDataSourceContents(string DataSource);
        string[] ListDatabaseCredentialRetrievalOptions();
        void SetItemDataSources(string ItemPath, RSDataSource[] DataSources);
        RSDataSource[] GetItemDataSources(string ItemPath);
        bool TestConnectForDataSourceDefinition(RSDataSourceDefinition DataSourceDefinition, string UserName, string Password, out string ConnectError);
        bool TestConnectForItemDataSource(string ItemPath, string DataSourceName, string UserName, string Password, out string ConnectError);
        void CreateRole(string Name, string Description, string[] TaskIDs);
        void SetRoleProperties(string Name, string Description, string[] TaskIDs);
        string[] GetRoleProperties(string Name, string SiteUrl, out string Description);
        void DeleteRole(string Name);
        RSRole[] ListRoles(string SecurityScope, string SiteUrl);
        RSTask[] ListTasks(string SecurityScope);
        void SetPolicies(string ItemPath, RSPolicy[] Policies);
        RSPolicy[] GetPolicies(string ItemPath, out bool InheritParent);
        RSDataSourcePrompt[] GetItemDataSourcePrompts(string ItemPath);
        RSCatalogItem GenerateModel(string DataSource, string Model, string Parent, RSProperty[] Properties, out RSWarning[] Warnings);
        string[] GetModelItemPermissions(string Model, string ModelItemID);
        void SetModelItemPolicies(string Model, string ModelItemID, RSPolicy[] Policies);
        RSPolicy[] GetModelItemPolicies(string Model, string ModelItemID, out bool InheritParent);
        byte[] GetUserModel(string Model, string Perspective);
        void InheritModelItemParentSecurity(string Model, string ModelItemID);
        void SetModelDrillthroughReports(string Model, string ModelItemID, RSModelDrillthroughReport[] Reports);
        RSModelDrillthroughReport[] ListModelDrillthroughReports(string Model, string ModelItemID);
        RSModelItem[] ListModelItemChildren(string Model, string ModelItemID, bool Recursive);
        string[] ListModelItemTypes();
        RSModelCatalogItem[] ListModelPerspectives(string Model);
        RSWarning[] RegenerateModel(string Model);
        void RemoveAllModelItemPolicies(string Model);
        string CreateSchedule(string Name, RSScheduleDefinition ScheduleDefinition, string SiteUrl);
        void DeleteSchedule(string ScheduleID);
        RSSchedule[] ListSchedules(string SiteUrl);
        RSSchedule GetScheduleProperties(string ScheduleID);
        string[] ListScheduleStates();
        void PauseSchedule(string ScheduleID);
        void ResumeSchedule(string ScheduleID);
        void SetScheduleProperties(string Name, string ScheduleID, RSScheduleDefinition ScheduleDefinition);
        RSCatalogItem[] ListScheduledItems(string ScheduleID);
        void SetItemParameters(string ItemPath, RSItemParameter[] Parameters);
        RSItemParameter[] GetItemParameters(string ItemPath, string HistoryID, bool ForRendering, RSParameterValue[] Values, RSDataSourceCredentials[] Credentials);
        string[] ListParameterTypes();
        string[] ListParameterStates();
        string CreateReportEditSession(string Report, string Parent, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, out RSWarning[] Warnings);
        void CreateLinkedItem(string ItemPath, string Parent, string Link, RSProperty[] Properties);
        void SetItemLink(string ItemPath, string Link);
        string GetItemLink(string ItemPath);
        string[] ListExecutionSettings();
        void SetExecutionOptions(string ItemPath, string ExecutionSetting, RSScheduleDefinitionOrReference Item);
        string GetExecutionOptions(string ItemPath, out RSScheduleDefinitionOrReference Item);
        void UpdateItemExecutionSnapshot(string ItemPath);
        void SetCacheOptions(string ItemPath, bool CacheItem, RSExpirationDefinition Item);
        bool GetCacheOptions(string ItemPath, out RSExpirationDefinition Item);
        void FlushCache(string ItemPath);
        string CreateItemHistorySnapshot(string ItemPath, out RSWarning[] Warnings);
        void DeleteItemHistorySnapshot(string ItemPath, string HistoryID);
        void SetItemHistoryLimit(string ItemPath, bool UseSystem, int HistoryLimit);
        int GetItemHistoryLimit(string ItemPath, out bool IsSystem, out int SystemLimit);
        void SetItemHistoryOptions(string ItemPath, bool EnableManualSnapshotCreation, bool KeepExecutionSnapshots, RSScheduleDefinitionOrReference Item);
        bool GetItemHistoryOptions(string ItemPath, out bool KeepExecutionSnapshots, out RSScheduleDefinitionOrReference Item);
        string GetReportServerConfigInfo(bool ScaleOut);
        bool IsSSLRequired();
        void SetSystemProperties(RSProperty[] Properties);
        RSProperty[] GetSystemProperties(RSProperty[] Properties);
        void SetSystemPolicies(RSPolicy[] Policies);
        RSPolicy[] GetSystemPolicies();
        RSExtension[] ListExtensions(string ExtensionType);
        string[] ListExtensionTypes();
        RSEvent[] ListEvents();
        void FireEvent(string EventType, string EventData, string SiteUrl);
        RSJob[] ListJobs();
        string[] ListJobTypes();
        string[] ListJobActions();
        string[] ListJobStates();
        bool CancelJob(string JobID);
        string CreateCacheRefreshPlan(string ItemPath, string Description, string EventType, string MatchData, RSParameterValue[] Parameters);
        void SetCacheRefreshPlanProperties(string CacheRefreshPlanID, string Description, string EventType, string MatchData, RSParameterValue[] Parameters);
        string GetCacheRefreshPlanProperties(string CacheRefreshPlanID, out string LastRunStatus, out RSCacheRefreshPlanState State, out string EventType, out string MatchData, out RSParameterValue[] Parameters);
        void DeleteCacheRefreshPlan(string CacheRefreshPlanID);
        RSCacheRefreshPlan[] ListCacheRefreshPlans(string ItemPath);
        void LogonUser(string userName, string password, string authority);
        void Logoff();
        string[] GetPermissions(string ItemPath);
        string[] GetSystemPermissions();
        string[] ListSecurityScopes();
        #region 2005 methods
        // they were skiped, but will create new class just for 2005 specific ones that can't be mapped from here.
        #endregion
    }
}
