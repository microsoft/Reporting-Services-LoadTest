// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Linq;
using Microsoft.OData.Client;
using RSAccessor.PortalAccessor;
using RSLoad.Utilites;
using RSAccessor.PortalAccessor.OData.Model;


namespace RSLoad
{
    internal class MobileClient 
    {
        private static object _lock = new object();

        public static void LoadMobileReport(RSPortalAccessorV1 portalAccessor, string report)
        {
            lock (_lock)
            {
                var ctx = portalAccessor.CreateContext();

                // runtime
                ProceedIfOk(() => ctx.SafeGetSystemResourceContent("mobilereportruntime", "web").GetValue());

                // report
                MobileReport mobileReport = null;
                ProceedIfOk(() => mobileReport = ctx.CatalogItemByPath(report).GetValue() as MobileReport);

                // definition
                ProceedIfOk(() => ctx.CatalogItems.ByKey(mobileReport.Manifest.Definition.Id).GetValue());

                // resources
                foreach (var resource in mobileReport.Manifest.Resources)
                {
                    foreach (var item in resource.Items)
                    {
                        if (resource.Type == MobileReportResourceGroupType.Map && item.Key == "json")
                        {
                            LoadResource(portalAccessor, item.Id);
                        }
                        else if (resource.Type == MobileReportResourceGroupType.Style && new[] { "colors", "Windows8-Style.xaml" }.Contains(item.Key))
                        {
                            LoadResource(portalAccessor, item.Id);
                        }
                    }
                }

                // datasets
                foreach (var dataset in mobileReport.Manifest.DataSets.RandomWhere(x => x.Type == MobileReportDataSetType.Shared))
                {
                    ProceedIfOk(() => ctx.CatalogItems.ByKey(dataset.Id).CastToDataSet().GetData(new DataSetParameter[] { }, null).GetValue());
                }
            }
        }

        private static void LoadResource(RSPortalAccessorV1 portalAccessor, Guid id)
        {
            var ctx = portalAccessor.CreateContext();
            ProceedIfOk(() => ctx.CatalogItems.ByKey(id).CastToResource().Select(x => x.Content).GetValue());
        }

        private static void ProceedIfOk(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (DataServiceQueryException ex)
            {
                if (ex.Response.StatusCode != 200)
                    throw;
            }
        }
    }
}