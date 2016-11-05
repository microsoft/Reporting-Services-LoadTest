# How to onboard a new Paginated Reports Scenario

For this tutorial, we will create a new scenario called **MyNewPaginated** which will contain a new set of your reports and the data sources

* In the Visual Studio Solution add a new folder under RSLoad\ContentManager\RuntimeResources\MyNewPaginated
* Add the reports to the new folder, it should look like this 

![](images/image1.png)

* Add the Data sources that the reports use in the file RSLoad\ContentManager\RuntimeResources\DataSources.xml 

* Ensure the following conditions are met:
    * Your reports uses a shared data source 
    * The data source name in the report matches the one you have in the DataSources.xml 
    * The data source is in the same folder than the report
    * The credentials of the data source are SQL Credentials and those specified in RSTest.Common.ReportServer.dll.Config for the settings DatasourceSQLUser and DatasourceSQLPassword

DataSources.xml should looks like this
```xml
<?xml version="1.0" encoding="utf-8" ?>
<DataSources>
  <DataSource Name="BoringDatasource" Extension="SQL"  Database="AdventureWorks2000" UseWindowsCredential="false" Enabled="true" Prompt="" ImpersonateUser="false"></DataSource>
</DataSources>
```

## Validating the new report and data source
Now is time to test that the pipeline works correctly, the easiest way to do it is adding a content validation unit test, there is a class in the project ready for this RSLoad\Actions\Professional\PaginatedOnboard.cs just edit few lines with **MyNewPaginated** and should look like this
```cs
private static List<string> _loadTestScenariosToDeployInServer = new List<string>() { "MyNewPaginated" };
[TestMethod]
public void ValidateMyNewPaginated()
{
    ValidateAllReportsRenderHTML5_UnitTest("MyNewPaginated");
}
```
Execute the test 

![](images/image2.png)

You can also validate that the report is in the server in the new folder

![](images/image3.png)

## Create the new  load scenario

Add a new load test to the solution

![](images/image4.png)

Lets use 1 hour duration

![](images/image5.png)

Ensure the scenario is called MyNewPaginated (same that the folder you just added)

![](images/image6.png)

Just for this example lets start with 10 concurrent request with an increase of 10 new users every 600 seconds

![](images/image7.png)

Select the set of actions you want to test with your reports, I have chosen Render , export and a couple of portal actions

![](images/image8.png)

Adjust the percentages

![](images/image9.png)

The loadtest should look like this

![](images/image10.png)

Now add a load test plug-in 

![](images/image11.png)

The only available is InitContentPlugin, this step is important as this Plug-in takes care of initializing the tests and publish the reports and data sources to the server

![](images/image12.png)

![](images/image13.png)

And now is time to run the test

![](images/image14.png)

![](images/image15.png)

You can see the results for each of your reports and the other actions you selected in the summary report when the load run is completed

![](images/image16.png)

Pro Tip: The load tests run in an agent in your machine so you won't see the traces and logs in Visual Studio , however you can use [dbgview](https://technet.microsoft.com/en-us/sysinternals/debugview.aspx) to see the traces from the test meanwhile is executing

