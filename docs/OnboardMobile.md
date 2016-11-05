# How to onboard a new Mobile Reports Scenario

For this tutorial , we will create a new scenario called **MyNewMobile** which will contain a new set of your mobile reports and the data sources.

* Create your mobile report, shared dataset and shared data source in your server under the MyNewMobile folder, it should look like this
* In the Visual Studio Solution add a new folder under RSLoad\ContentManager\RuntimeResources\MyNewMobile
* Download the report and the shared dataset from the portal 
* Add the report and the shared dataset to the new folder, it should look like this 

![](images/image101.png)

* Add the Data sources that the report uses in the file RSLoad\ContentManager\RuntimeResources\DataSources.xml 

* Ensure the following conditions are met:
    * The data source name in the report matches the one you have in the DataSources.xml 
    * The data source is in the same folder than the report
    * The credentials of the data source are SQL Credentials and those specified in RSTest.Common.ReportServer.dll.Config for the settings DatasourceSQLUser and DatasourceSQLPassword
    * Ensure the mobile report is using the dataset in the same folder 
    * The dataset reference is set to the current folder, you can validate in the xml view of the data set
```xml
<DataSourceReference>/MyNewMobile/AdventureWorks</DataSourceReference>
```

DataSources.xml should looks like this
```xml
<?xml version="1.0" encoding="utf-8" ?>
<DataSources>
  <DataSource Name="AdventureWorks" Extension="SQL"  Database="AdventureWorks" UseWindowsCredential="false" Enabled="true" Prompt="" ImpersonateUser="false"></DataSource>
</DataSources>
```

## Validating the new mobile report, dataset and data source
Now is time to test that the pipeline works correctly, the easiest way to do it is adding a content validation unit test, there is a class in the project ready for this RSLoad\Actions\Mobile\MobileOnboard.cs just edit few lines with **MyNewMobile** and should look like this
```cs
        private static List<string> _loadTestScenariosToDeployInServer = new List<string>() { "MyNewMobile" };
        [TestCategory("DependsOnDatasource")]
        [TestMethod]
        public void ValidateMyNewMobile()
        {
            LoadAllMobileReports();
        }
```
Execute the test 

![](images/image102.png)

You can also validate that the report is in the server in the new folder

![](images/image103.png)

## Create the new  load scenario

Follow the steps to create the load scenario detailed in the tutorial for [How to onboard a new Paginated Reports Scenario](../master/docs/OnboardPaginated.md)

For mobile reports there is only one test method available LoadMobileReports , this will select randomly one of the reports and execute it during the load test

You can combine it with the Portal actions as ListDatasets, ListDataSources, ListFolders and ListMobileReports

![](images/image104.png)

![](images/image105.png)
