// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSAccessor.SoapAccessor
{
    using System.Net;
    using Microsoft.SqlServer.ReportingServices2010;

    public class ReportingServices2010 : IRSManagement
        {
            ReportingService2010 rs = new ReportingService2010();
            SoapStructureConvert m_converter = null;

            public ReportingServices2010(string serverUrl, ICredentials executeCredentials)
            {
                rs.Url = string.Format("{0}/ReportService2010.asmx", serverUrl);
                rs.Credentials = executeCredentials;
                m_converter = new SoapStructureConvert(this.AssemblyName, this.Namespace);
            }

            public string Url
            {
                get
                {
                    return rs.Url;
                }
            }

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
                    return @"http://schemas.microsoft.com/sqlserver/reporting/2010/03/01/ReportServer";
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

            public RSItemNamespaceHeader ItemNamespaceHeaderValue
            {
                get
                {
                    return ((RSItemNamespaceHeader)Converter.Convert(rs.ItemNamespaceHeaderValue));
                }

                set
                {
                    rs.ItemNamespaceHeaderValue = (ItemNamespaceHeader)Converter.Convert(value);
                }
            }

            public RSCatalogItem CreateCatalogItem(string ItemType, string Name, string Parent, bool Overwrite, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, RSProperty[] Properties, out RSWarning[] Warnings)
            {
                Property[] properties = (Property[])Converter.Convert((object)Properties);
                Warning[] warns = null;
                CatalogItem catalog = rs.CreateCatalogItem(ItemType, Name, Parent, Overwrite, Definition, properties, out warns);

                RSCatalogItem rsCatalog = (RSCatalogItem)Converter.Convert(catalog, typeof(RSCatalogItem));
                Warnings = (RSWarning[])Converter.Convert((object)warns);

                return (rsCatalog);
            }

            public RSWarning[] SetItemDefinition(string ItemPath, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, RSProperty[] Properties)
            {
                Property[] properties = (Property[])Converter.Convert((object)Properties);
                Warning[] warns = null;
                warns = rs.SetItemDefinition(ItemPath, Definition, properties);
                RSWarning[] warnings = (RSWarning[])Converter.Convert((object)warns);
                return (warnings);
            }

            public byte[] GetItemDefinition(string ItemPath)
            {
                byte[] definition = rs.GetItemDefinition(ItemPath);
                return (definition);
            }

            public string GetItemType(string ItemPath)
            {
                return (rs.GetItemType(ItemPath));
            }

            public void DeleteItem(string ItemPath)
            {
                rs.DeleteItem(ItemPath);
            }

            public void MoveItem(string ItemPath, string Target)
            {
                rs.MoveItem(ItemPath, Target);
            }

            public void InheritParentSecurity(string ItemPath)
            {
                rs.InheritParentSecurity(ItemPath);
            }

            public RSItemHistorySnapshot[] ListItemHistory(string ItemPath)
            {
                ItemHistorySnapshot[] snapshots = rs.ListItemHistory(ItemPath);
                return ((RSItemHistorySnapshot[])Converter.Convert(snapshots));
            }

            public RSCatalogItem[] ListChildren(string ItemPath, bool Recursive)
            {
                CatalogItem[] catalogs = rs.ListChildren(ItemPath, Recursive);
                return ((RSCatalogItem[])Converter.Convert(catalogs));
            }

            public RSCatalogItem[] ListDependentItems(string ItemPath)
            {
                CatalogItem[] catalogs = rs.ListDependentItems(ItemPath);
                return ((RSCatalogItem[])Converter.Convert(catalogs));
            }

            public RSCatalogItem[] FindItems(string Folder, RSBooleanOperatorEnum BooleanOperator, RSProperty[] SearchOptions, RSSearchCondition[] SearchConditions)
            {
                BooleanOperatorEnum boolOperator = (BooleanOperatorEnum)Converter.Convert(BooleanOperator);
                Property[] searchOptions = ((Property[])Converter.Convert(SearchOptions));
                SearchCondition[] searchCond = ((SearchCondition[])Converter.Convert(SearchConditions));
                CatalogItem[] catalogs = rs.FindItems(Folder, boolOperator, searchOptions, searchCond);
                return ((RSCatalogItem[])Converter.Convert(catalogs));
            }

            public RSCatalogItem[] ListParents(string ItemPath)
            {
                CatalogItem[] catalogs = rs.ListParents(ItemPath);
                return ((RSCatalogItem[])Converter.Convert(catalogs));
            }

            public RSCatalogItem CreateFolder(string Folder, string Parent, RSProperty[] Properties)
            {
                Property[] props = ((Property[])Converter.Convert(Properties));
                CatalogItem catalog = rs.CreateFolder(Folder, Parent, props);
                return ((RSCatalogItem)Converter.Convert(catalog));
            }

            public void SetProperties(string ItemPath, RSProperty[] Properties)
            {
                Property[] props = ((Property[])Converter.Convert(Properties));
                rs.SetProperties(ItemPath, props);
            }

            public RSProperty[] GetProperties(string ItemPath, RSProperty[] Properties)
            {
                Property[] props = ((Property[])Converter.Convert(Properties));
                Property[] outProps = rs.GetProperties(ItemPath, props);
                return ((RSProperty[])Converter.Convert(outProps));
            }

            public void SetItemReferences(string ItemPath, RSItemReference[] ItemReferences)
            {
                ItemReference[] references = ((ItemReference[])Converter.Convert(ItemReferences));
                rs.SetItemReferences(ItemPath, references);
            }

            public RSItemReferenceData[] GetItemReferences(string ItemPath, string ReferenceItemType)
            {
                ItemReferenceData[] refData = rs.GetItemReferences(ItemPath, ReferenceItemType);
                return ((RSItemReferenceData[])Converter.Convert(refData));
            }

            public string[] ListItemTypes()
            {
                return (rs.ListItemTypes());
            }

            public void SetSubscriptionProperties(string SubscriptionID, RSExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData, RSParameterValue[] Parameters)
            {
                ExtensionSettings extSetting = (ExtensionSettings)Converter.Convert(ExtensionSettings);
                ParameterValue[] paramValues = (ParameterValue[])Converter.Convert(Parameters);
                rs.SetSubscriptionProperties(SubscriptionID, extSetting, Description, EventType, MatchData, paramValues);
            }

            public string GetSubscriptionProperties(string SubscriptionID, out RSExtensionSettings ExtensionSettings, out string Description, out RSActiveState Active, out string Status, out string EventType, out string MatchData, out RSParameterValue[] Parameters)
            {
                ExtensionSettings extSetting;
                ActiveState active;
                ParameterValue[] paramValues;
                string outVal = rs.GetSubscriptionProperties(SubscriptionID, out extSetting, out Description, out active, out Status, out EventType, out MatchData, out paramValues);
                Active = (RSActiveState)Converter.Convert(active);
                Parameters = (RSParameterValue[])Converter.Convert(paramValues);
                ExtensionSettings = (RSExtensionSettings)Converter.Convert(extSetting);

                return (outVal);
            }

            public void SetDataDrivenSubscriptionProperties(string DataDrivenSubscriptionID, RSExtensionSettings ExtensionSettings, RSDataRetrievalPlan DataRetrievalPlan, string Description, string EventType, string MatchData, RSParameterValueOrFieldReference[] Parameters)
            {
                ExtensionSettings extSettings = (ExtensionSettings)Converter.Convert(ExtensionSettings);
                DataRetrievalPlan dataRetrievalPaln = (DataRetrievalPlan)Converter.Convert(DataRetrievalPlan);
                ParameterValueOrFieldReference[] paramValues = (ParameterValueOrFieldReference[])Converter.Convert(Parameters);
                rs.SetDataDrivenSubscriptionProperties(DataDrivenSubscriptionID, extSettings, dataRetrievalPaln, Description, EventType, MatchData, paramValues);
            }

            public string GetDataDrivenSubscriptionProperties(string DataDrivenSubscriptionID, out RSExtensionSettings ExtensionSettings, out RSDataRetrievalPlan DataRetrievalPlan, out string Description, out RSActiveState Active, out string Status, out string EventType, out string MatchData, out RSParameterValueOrFieldReference[] Parameters)
            {
                ExtensionSettings extSettings;
                DataRetrievalPlan dataRetrievalPaln;
                ParameterValueOrFieldReference[] paramValues;
                ActiveState active;
                string outval = rs.GetDataDrivenSubscriptionProperties(DataDrivenSubscriptionID, out extSettings, out dataRetrievalPaln, out Description, out active, out Status, out EventType, out MatchData, out paramValues);
                ExtensionSettings = (RSExtensionSettings)Converter.Convert(extSettings);
                DataRetrievalPlan = (RSDataRetrievalPlan)Converter.Convert(dataRetrievalPaln);
                Active = (RSActiveState)Converter.Convert(active);
                Parameters = (RSParameterValueOrFieldReference[])Converter.Convert(paramValues);
                return (outval);
            }

            public void DeleteSubscription(string SubscriptionID)
            {
                rs.DeleteSubscription(SubscriptionID);
            }

            public string CreateSubscription(string ItemPath, RSExtensionSettings ExtensionSettings, string Description, string EventType, string MatchData, RSParameterValue[] Parameters)
            {
                ExtensionSettings extSetting = (ExtensionSettings)Converter.Convert(ExtensionSettings);
                ParameterValue[] param = (ParameterValue[])Converter.Convert(Parameters);
                string outval = rs.CreateSubscription(ItemPath, extSetting, Description, EventType, MatchData, param);
                return (outval);
            }

            public string CreateDataDrivenSubscription(string ItemPath, RSExtensionSettings ExtensionSettings, RSDataRetrievalPlan DataRetrievalPlan, string Description, string EventType, string MatchData, RSParameterValueOrFieldReference[] Parameters)
            {
                ExtensionSettings extSetting = (ExtensionSettings)Converter.Convert(ExtensionSettings);
                DataRetrievalPlan dataRetrievalPlan = (DataRetrievalPlan)Converter.Convert(DataRetrievalPlan);
                ParameterValueOrFieldReference[] paramValues = (ParameterValueOrFieldReference[])Converter.Convert(Parameters);
                string outval = rs.CreateDataDrivenSubscription(ItemPath, extSetting, dataRetrievalPlan, Description, EventType, MatchData, paramValues);
                return (outval);
            }

            public RSExtensionParameter[] GetExtensionSettings(string Extension)
            {
                ExtensionParameter[] parameters = rs.GetExtensionSettings(Extension);
                return ((RSExtensionParameter[])Converter.Convert(parameters));
            }

            public RSExtensionParameter[] ValidateExtensionSettings(string Extension, RSParameterValueOrFieldReference[] ParameterValues, string SiteUrl)
            {
                ParameterValueOrFieldReference[] paramValues = (ParameterValueOrFieldReference[])Converter.Convert(ParameterValues);
                ExtensionParameter[] outval = rs.ValidateExtensionSettings(Extension, paramValues, SiteUrl);
                return ((RSExtensionParameter[])Converter.Convert(outval));
            }

            public RSSubscription[] ListSubscriptions(string ItemPathOrSiteURL)
            {
                Subscription[] subs = rs.ListSubscriptions(ItemPathOrSiteURL);
                return ((RSSubscription[])Converter.Convert(subs));
            }

            public RSSubscription[] ListMySubscriptions(string ItemPathOrSiteURL)
            {
                Subscription[] subs = rs.ListMySubscriptions(ItemPathOrSiteURL);
                return ((RSSubscription[])Converter.Convert(subs));
            }

            public RSSubscription[] ListSubscriptionsUsingDataSource(string DataSource)
            {
                Subscription[] subs = rs.ListSubscriptionsUsingDataSource(DataSource);
                return ((RSSubscription[])Converter.Convert(subs));
            }

            public void ChangeSubscriptionOwner(string SubscriptionID, string NewOwner)
            {
                rs.ChangeSubscriptionOwner(SubscriptionID, NewOwner);
            }

            public RSCatalogItem CreateDataSource(string DataSource, string Parent, bool Overwrite, RSDataSourceDefinition Definition, RSProperty[] Properties)
            {
                DataSourceDefinition dsDef = (DataSourceDefinition)Converter.Convert(Definition);
                Property[] props = (Property[])Converter.Convert(Properties);
                CatalogItem outval = rs.CreateDataSource(DataSource, Parent, Overwrite, dsDef, props);
                return ((RSCatalogItem)Converter.Convert(outval));
            }

            public RSDataSetDefinition PrepareQuery(RSDataSource DataSource, RSDataSetDefinition DataSet, out bool Changed, out string[] ParameterNames)
            {
                DataSource ds = (DataSource)Converter.Convert(DataSource);
                DataSetDefinition dsDef = (DataSetDefinition)Converter.Convert(DataSet);
                DataSetDefinition outval = rs.PrepareQuery(ds, dsDef, out Changed, out ParameterNames);
                return ((RSDataSetDefinition)Converter.Convert(outval));
            }

            public void EnableDataSource(string DataSource)
            {
                rs.EnableDataSource(DataSource);
            }

            public void DisableDataSource(string DataSource)
            {
                rs.DisableDataSource(DataSource);
            }

            public void SetDataSourceContents(string DataSource, RSDataSourceDefinition Definition)
            {
                DataSourceDefinition dsDef = (DataSourceDefinition)Converter.Convert(Definition);
                rs.SetDataSourceContents(DataSource, dsDef);
            }

            public RSDataSourceDefinition GetDataSourceContents(string DataSource)
            {
                DataSourceDefinition dsDef = rs.GetDataSourceContents(DataSource);
                return ((RSDataSourceDefinition)Converter.Convert(dsDef));
            }

            public string[] ListDatabaseCredentialRetrievalOptions()
            {
                return (rs.ListDatabaseCredentialRetrievalOptions());
            }

            public void SetItemDataSources(string ItemPath, RSDataSource[] DataSources)
            {
                DataSource[] ds = (DataSource[])Converter.Convert(DataSources);
                rs.SetItemDataSources(ItemPath, ds);
            }

            public RSDataSource[] GetItemDataSources(string ItemPath)
            {
                DataSource[] datasources = rs.GetItemDataSources(ItemPath);
                return ((RSDataSource[])Converter.Convert(datasources));
            }

            public bool TestConnectForDataSourceDefinition(RSDataSourceDefinition DataSourceDefinition, string UserName, string Password, out string ConnectError)
            {
                DataSourceDefinition dsDef = (DataSourceDefinition)Converter.Convert(DataSourceDefinition);
                bool outval = rs.TestConnectForDataSourceDefinition(dsDef, UserName, Password, out ConnectError);
                return (outval);
            }

            public bool TestConnectForItemDataSource(string ItemPath, string DataSourceName, string UserName, string Password, out string ConnectError)
            {
                return (rs.TestConnectForItemDataSource(ItemPath, DataSourceName, UserName, Password, out ConnectError));
            }

            public void CreateRole(string Name, string Description, string[] TaskIDs)
            {
                rs.CreateRole(Name, Description, TaskIDs);
            }

            public void SetRoleProperties(string Name, string Description, string[] TaskIDs)
            {
                rs.SetRoleProperties(Name, Description, TaskIDs);
            }

            public string[] GetRoleProperties(string Name, string SiteUrl, out string Description)
            {
                return (rs.GetRoleProperties(Name, SiteUrl, out Description));
            }

            public void DeleteRole(string Name)
            {
                rs.DeleteRole(Name);
            }

            public RSRole[] ListRoles(string SecurityScope, string SiteUrl)
            {
                Role[] roles = rs.ListRoles(SecurityScope, SiteUrl);
                return ((RSRole[])Converter.Convert(roles));
            }

            public RSTask[] ListTasks(string SecurityScope)
            {
                Task[] tasks = rs.ListTasks(SecurityScope);
                return ((RSTask[])Converter.Convert(tasks));
            }

            public void SetPolicies(string ItemPath, RSPolicy[] Policies)
            {
                Policy[] policies = (Policy[])Converter.Convert(Policies);
                rs.SetPolicies(ItemPath, policies);
            }

            public RSPolicy[] GetPolicies(string ItemPath, out bool InheritParent)
            {
                Policy[] policies = rs.GetPolicies(ItemPath, out InheritParent);
                return ((RSPolicy[])Converter.Convert(policies));
            }

            public RSDataSourcePrompt[] GetItemDataSourcePrompts(string ItemPath)
            {
                DataSourcePrompt[] prompts = rs.GetItemDataSourcePrompts(ItemPath);
                return ((RSDataSourcePrompt[])Converter.Convert(prompts));
            }

            public RSCatalogItem GenerateModel(string DataSource, string Model, string Parent, RSProperty[] Properties, out RSWarning[] Warnings)
            {
                Warning[] warns = null;
                Property[] props = (Property[])Converter.Convert(Properties);
                CatalogItem outval = rs.GenerateModel(DataSource, Model, Parent, props, out warns);
                Warnings = (RSWarning[])Converter.Convert(warns);
                return ((RSCatalogItem)Converter.Convert(outval));

            }

            public string[] GetModelItemPermissions(string Model, string ModelItemID)
            {
                return (rs.GetModelItemPermissions(Model, ModelItemID));
            }

            public void SetModelItemPolicies(string Model, string ModelItemID, RSPolicy[] Policies)
            {
                Policy[] policies = (Policy[])Converter.Convert(Policies);
                rs.SetModelItemPolicies(Model, ModelItemID, policies);
            }

            public RSPolicy[] GetModelItemPolicies(string Model, string ModelItemID, out bool InheritParent)
            {
                Policy[] policies = rs.GetModelItemPolicies(Model, ModelItemID, out InheritParent);
                return ((RSPolicy[])Converter.Convert(policies));
            }

            public byte[] GetUserModel(string Model, string Perspective)
            {
                return (rs.GetUserModel(Model, Perspective));
            }

            public void InheritModelItemParentSecurity(string Model, string ModelItemID)
            {
                rs.InheritModelItemParentSecurity(Model, ModelItemID);
            }

            public void SetModelDrillthroughReports(string Model, string ModelItemID, RSModelDrillthroughReport[] Reports)
            {
                ModelDrillthroughReport[] reports = (ModelDrillthroughReport[])Converter.Convert(Reports);
                rs.SetModelDrillthroughReports(Model, ModelItemID, reports);
            }

            public RSModelDrillthroughReport[] ListModelDrillthroughReports(string Model, string ModelItemID)
            {
                ModelDrillthroughReport[] drillthroughReports = rs.ListModelDrillthroughReports(Model, ModelItemID);
                return ((RSModelDrillthroughReport[])Converter.Convert(drillthroughReports));
            }

            public RSModelItem[] ListModelItemChildren(string Model, string ModelItemID, bool Recursive)
            {
                ModelItem[] modelItems = rs.ListModelItemChildren(Model, ModelItemID, Recursive);
                return ((RSModelItem[])Converter.Convert(modelItems));
            }

            public string[] ListModelItemTypes()
            {
                return (rs.ListModelItemTypes());
            }

            public RSModelCatalogItem[] ListModelPerspectives(string Model)
            {
                ModelCatalogItem[] modelCatalogs = rs.ListModelPerspectives(Model);
                return ((RSModelCatalogItem[])Converter.Convert(modelCatalogs));
            }

            public RSWarning[] RegenerateModel(string Model)
            {
                Warning[] warns = rs.RegenerateModel(Model);
                return ((RSWarning[])Converter.Convert(warns));
            }

            public void RemoveAllModelItemPolicies(string Model)
            {
                rs.RemoveAllModelItemPolicies(Model);
            }

            public string CreateSchedule(string Name, RSScheduleDefinition ScheduleDefinition, string SiteUrl)
            {
                ScheduleDefinition schedDef = (ScheduleDefinition)Converter.Convert(ScheduleDefinition);
                string outval = rs.CreateSchedule(Name, schedDef, SiteUrl);
                return (outval);
            }

            public void DeleteSchedule(string ScheduleID)
            {
                rs.DeleteSchedule(ScheduleID);
            }

            public RSSchedule[] ListSchedules(string SiteUrl)
            {
                Schedule[] data = rs.ListSchedules(SiteUrl);
                return ((RSSchedule[])Converter.Convert(data));
            }

            public RSSchedule GetScheduleProperties(string ScheduleID)
            {
                Schedule data = rs.GetScheduleProperties(ScheduleID);
                return ((RSSchedule)Converter.Convert(data));
            }

            public string[] ListScheduleStates()
            {
                return (rs.ListScheduleStates());
            }

            public void PauseSchedule(string ScheduleID)
            {
                rs.PauseSchedule(ScheduleID);
            }

            public void ResumeSchedule(string ScheduleID)
            {
                rs.ResumeSchedule(ScheduleID);
            }

            public void SetScheduleProperties(string Name, string ScheduleID, RSScheduleDefinition ScheduleDefinition)
            {
                ScheduleDefinition schedDef = (ScheduleDefinition)Converter.Convert(ScheduleDefinition);
                rs.SetScheduleProperties(Name, ScheduleID, schedDef);
            }

            public RSCatalogItem[] ListScheduledItems(string ScheduleID)
            {
                CatalogItem[] data = rs.ListScheduledItems(ScheduleID);
                return ((RSCatalogItem[])Converter.Convert(data));
            }

            public void SetItemParameters(string ItemPath, RSItemParameter[] Parameters)
            {
                ItemParameter[] itemParams = (ItemParameter[])Converter.Convert(Parameters);
                rs.SetItemParameters(ItemPath, itemParams);
            }

            public RSItemParameter[] GetItemParameters(string ItemPath, string HistoryID, bool ForRendering, RSParameterValue[] Values, RSDataSourceCredentials[] Credentials)
            {
                ParameterValue[] paramValues = (ParameterValue[])Converter.Convert(Values);
                DataSourceCredentials[] credentials = (DataSourceCredentials[])Converter.Convert(Credentials);
                ItemParameter[] outval = rs.GetItemParameters(ItemPath, HistoryID, ForRendering, paramValues, credentials);
                return ((RSItemParameter[])Converter.Convert(outval));
            }

            public string[] ListParameterTypes()
            {
                return (rs.ListParameterTypes());
            }

            public string[] ListParameterStates()
            {
                return (rs.ListParameterStates());
            }

            public string CreateReportEditSession(string Report, string Parent, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] Definition, out RSWarning[] Warnings)
            {
                Warning[] warns = null;
                string outval = rs.CreateReportEditSession(Report, Parent, Definition, out warns);
                Warnings = (RSWarning[])Converter.Convert(warns);
                return (outval);
            }

            public void CreateLinkedItem(string ItemPath, string Parent, string Link, RSProperty[] Properties)
            {
                Property[] props = (Property[])Converter.Convert(Properties);
                rs.CreateLinkedItem(ItemPath, Parent, Link, props);
            }

            public void SetItemLink(string ItemPath, string Link)
            {
                rs.SetItemLink(ItemPath, Link);
            }

            public string GetItemLink(string ItemPath)
            {
                return (rs.GetItemLink(ItemPath));
            }

            public string[] ListExecutionSettings()
            {
                return (rs.ListExecutionSettings());
            }

            public void SetExecutionOptions(string ItemPath, string ExecutionSetting, RSScheduleDefinitionOrReference Item)
            {
                ScheduleDefinitionOrReference item = (ScheduleDefinitionOrReference)Converter.Convert(Item);
                rs.SetExecutionOptions(ItemPath, ExecutionSetting, item);
            }

            public string GetExecutionOptions(string ItemPath, out RSScheduleDefinitionOrReference Item)
            {
                ScheduleDefinitionOrReference item = null;
                string outval = rs.GetExecutionOptions(ItemPath, out item);
                Item = (RSScheduleDefinitionOrReference)Converter.Convert(item);
                return (outval);
            }

            public void UpdateItemExecutionSnapshot(string ItemPath)
            {
                rs.UpdateItemExecutionSnapshot(ItemPath);
            }

            public void SetCacheOptions(string ItemPath, bool CacheItem, RSExpirationDefinition Item)
            {
                ExpirationDefinition item = (ExpirationDefinition)Converter.Convert(Item);
                rs.SetCacheOptions(ItemPath, CacheItem, item);
            }

            public bool GetCacheOptions(string ItemPath, out RSExpirationDefinition Item)
            {
                ExpirationDefinition item = null;
                bool outval = rs.GetCacheOptions(ItemPath, out item);
                Item = (RSExpirationDefinition)Converter.Convert(item);
                return (outval);
            }

            public void FlushCache(string ItemPath)
            {
                rs.FlushCache(ItemPath);
            }

            public string CreateItemHistorySnapshot(string ItemPath, out RSWarning[] Warnings)
            {
                Warning[] warns = null;
                string outval = rs.CreateItemHistorySnapshot(ItemPath, out warns);
                Warnings = (RSWarning[])Converter.Convert(warns);
                return (outval);
            }

            public void DeleteItemHistorySnapshot(string ItemPath, string HistoryID)
            {
                rs.DeleteItemHistorySnapshot(ItemPath, HistoryID);
            }

            public void SetItemHistoryLimit(string ItemPath, bool UseSystem, int HistoryLimit)
            {
                rs.SetItemHistoryLimit(ItemPath, UseSystem, HistoryLimit);
            }

            public int GetItemHistoryLimit(string ItemPath, out bool IsSystem, out int SystemLimit)
            {
                return (rs.GetItemHistoryLimit(ItemPath, out IsSystem, out SystemLimit));
            }

            public void SetItemHistoryOptions(string ItemPath, bool EnableManualSnapshotCreation, bool KeepExecutionSnapshots, RSScheduleDefinitionOrReference Item)
            {
                ScheduleDefinitionOrReference item = (ScheduleDefinitionOrReference)Converter.Convert(Item); ;
                rs.SetItemHistoryOptions(ItemPath, EnableManualSnapshotCreation, KeepExecutionSnapshots, item);
            }

            public bool GetItemHistoryOptions(string ItemPath, out bool KeepExecutionSnapshots, out RSScheduleDefinitionOrReference Item)
            {
                ScheduleDefinitionOrReference item = null;
                bool outval = rs.GetItemHistoryOptions(ItemPath, out KeepExecutionSnapshots, out item);
                Item = (RSScheduleDefinitionOrReference)Converter.Convert(item);
                return (outval);
            }

            public string GetReportServerConfigInfo(bool ScaleOut)
            {
                return (rs.GetReportServerConfigInfo(ScaleOut));
            }

            public bool IsSSLRequired()
            {
                return (rs.IsSSLRequired());
            }

            public void SetSystemProperties(RSProperty[] Properties)
            {
                Property[] props = (Property[])Converter.Convert(Properties);
                rs.SetSystemProperties(props);
            }

            public RSProperty[] GetSystemProperties(RSProperty[] Properties)
            {
                Property[] props = (Property[])Converter.Convert(Properties);
                Property[] propsOut = rs.GetSystemProperties(props);
                return ((RSProperty[])Converter.Convert(propsOut));
            }

            public void SetSystemPolicies(RSPolicy[] Policies)
            {
                Policy[] policies = (Policy[])Converter.Convert(Policies);
                rs.SetSystemPolicies(policies);
            }

            public RSPolicy[] GetSystemPolicies()
            {
                Policy[] data = rs.GetSystemPolicies();
                return ((RSPolicy[])Converter.Convert(data));
            }

            public RSExtension[] ListExtensions(string ExtensionType)
            {
                Extension[] data = rs.ListExtensions(ExtensionType);
                return ((RSExtension[])Converter.Convert(data));
            }

            public string[] ListExtensionTypes()
            {
                return (rs.ListExtensionTypes());
            }

            public RSEvent[] ListEvents()
            {
                Event[] data = rs.ListEvents();
                return ((RSEvent[])Converter.Convert(data));
            }

            public void FireEvent(string EventType, string EventData, string SiteUrl)
            {
                rs.FireEvent(EventType, EventData, SiteUrl);
            }

            public RSJob[] ListJobs()
            {
                Job[] data = rs.ListJobs();
                return ((RSJob[])Converter.Convert(data));
            }

            public string[] ListJobTypes()
            {
                return (rs.ListJobTypes());
            }

            public string[] ListJobActions()
            {
                return (rs.ListJobActions());
            }

            public string[] ListJobStates()
            {
                return (rs.ListJobStates());
            }

            public bool CancelJob(string JobID)
            {
                return (rs.CancelJob(JobID));
            }

            public string CreateCacheRefreshPlan(string ItemPath, string Description, string EventType, string MatchData, RSParameterValue[] Parameters)
            {
                ParameterValue[] paramValues = (ParameterValue[])Converter.Convert(Parameters);
                string outval = rs.CreateCacheRefreshPlan(ItemPath, Description, EventType, MatchData, paramValues);
                return (outval);
            }

            public void SetCacheRefreshPlanProperties(string CacheRefreshPlanID, string Description, string EventType, string MatchData, RSParameterValue[] Parameters)
            {
                ParameterValue[] paramValues = (ParameterValue[])Converter.Convert(Parameters);
                rs.SetCacheRefreshPlanProperties(CacheRefreshPlanID, Description, EventType, MatchData, paramValues);
            }

            public string GetCacheRefreshPlanProperties(string CacheRefreshPlanID, out string LastRunStatus, out RSCacheRefreshPlanState State, out string EventType, out string MatchData, out RSParameterValue[] Parameters)
            {
                CacheRefreshPlanState state;
                ParameterValue[] paramValues;
                string outval = rs.GetCacheRefreshPlanProperties(CacheRefreshPlanID, out LastRunStatus, out state, out EventType, out MatchData, out paramValues);
                State = (RSCacheRefreshPlanState)Converter.Convert(state);
                Parameters = (RSParameterValue[])Converter.Convert(paramValues);
                return (outval);
            }

            public void DeleteCacheRefreshPlan(string CacheRefreshPlanID)
            {
                rs.DeleteCacheRefreshPlan(CacheRefreshPlanID);
            }

            public RSCacheRefreshPlan[] ListCacheRefreshPlans(string ItemPath)
            {
                CacheRefreshPlan[] data = rs.ListCacheRefreshPlans(ItemPath);
                return ((RSCacheRefreshPlan[])Converter.Convert(data));
            }

            public void LogonUser(string userName, string password, string authority)
            {
                rs.LogonUser(userName, password, authority);
            }

            public void Logoff()
            {
                rs.Logoff();
            }

            public string[] GetPermissions(string ItemPath)
            {
                return (rs.GetPermissions(ItemPath));
            }

            public string[] GetSystemPermissions()
            {
                return (rs.GetSystemPermissions());
            }

            public string[] ListSecurityScopes()
            {
                return (rs.ListSecurityScopes());
            }
        }
}
