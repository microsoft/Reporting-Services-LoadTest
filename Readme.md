[![Build status](https://ci.appveyor.com/api/projects/status/github/microsoft/reporting-services-loadtest?branch=master&svg=true)](https://ci.appveyor.com/project/jtarquino/reporting-services-loadtest)

# Reporting Services LoadTest
SQL Server Reporting Services LoadTest 

## Synopsis
This project contains a [Visual Studio Load Test 2015](https://www.visualstudio.com/en-us/docs/test/performance-testing/getting-started/getting-started-with-performance-testing) solution to execute synthetic load for SQL Server Reporting Services 2016 
It uses Visual Studio 2015 Enterprise

The usage of the project requires a good understanding of Reporting Services and the different types of items (reports, datasets, data sources, mobile reports) that are available in SQL Server Reporting Services, it also make use of APIs and code that is not designed for typical user comsumption and can change in future versions of SQL Server Reporting Services

## Buid and Test
In a Visual Studio Tools Command Prompt
```
c:\repos\Reporting-Services-LoadTest>build.cmd
```
Integration tests that requires a configured SQL Server Reporting Services 2016
```
c:\repos\Reporting-Services-LoadTest>Test.cmd
```
Integration tests that requires a configured SQL Server Reporting Services 2016 and the Data sources
```
c:\repos\Reporting-Services-LoadTest>TestContent.cmd
```
Included integration tests that requires a configured SQL Server Reporting Services Technical Preview with the Data sources
```
c:\repos\Reporting-Services-LoadTest>TestContentWithPBI.cmd
```

You can also build and run the tests from Visual Studio 2015, in such case ensure is using RSLoadTest.testsettings

* Visual Studio Menu > Test > Test Settings > Select Test Settings File

# Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Load Test Execution 
The tests can be executed locally or in the cloud, instructions for both are detailed below

## 1. Update the Load Test Target Server Configuration
Update the file RSTest.Common.ReportServer.dll.Config with the Reporting Services environment
```xml
<Configuration>
  <ReportServerUrl>http://ssrs.westus.cloudapp.azure.com/ReportServer</ReportServerUrl>
  <ReportPortalUrl>http://ssrs.westus.cloudapp.azure.com/Reports</ReportPortalUrl>
  <DatasourceDatabaseServer>ssrs-datasource</DatasourceDatabaseServer>
  <ExecutionAccount>contoso\ProvideAUser</ExecutionAccount>
  <ExecutionAccountPwd>ProvideAPassword</ExecutionAccountPwd>
  <DatasourceSQLUser>ProvideASQLUser</DatasourceSQLUser>
  <DatasourceSQLPassword>ProvideASQLPassword</DatasourceSQLPassword>
</Configuration>
```
* ReportServerUrl, ReportPortalUrl should be updated to the correct location 
* ExecutionAccount and ExecutionAccountPwd should be windows users that have administrator privileges in the Reporting Services Portal
* DatasourceSQLUser and DatasourceSQLPassword should be SQL Logins with access to the databases specfied in the data sources

***In order to create a SQL Server Reporting Services Load enviroment in Azure see the section Create a SSRS Load Environment in Azure*** 

## 2. Increase the number of MaxActiveReqForOneUser
This setting is defined in the rsreportserver.config file in the Reporting Services Server you are testing, the tests use only one Windows user to access the server and the default value is 20, if is not modified an artificial throttling will affect the test results 

## 2.1 Local Run with SQL Express LocalDb
* Open the RSLoadTest.testsettings file (double click on the file) and select "Run tests using local computer or a test controller"
* Open the LoadTest to run (For example MixedLoad.loadtest)
* Run the test
    * In the left upper corner there is a test dropdown
        * Select Run Load Test or Debug Load Test
    * Make sure that you have a reasonable amount of users for a local run so you can adequately debug.

## 2.2 Local Run with Load Test Results Repository Using SQL
In case you need to store a large number of test results is recommended to use a Load Test Results Repository Using SQL 
* Create a Load Database, detailed instructions here https://msdn.microsoft.com/en-us/library/ms182600.aspx
* Visual Studio Menu > Load Test > Manage Test Controller
    * Controller: Select "Local No Controller"
    * Load test result store: Your configured load database store from the previous step
* Open the RSLoadTest.testsettings file (double click on the file) and select "Run tests using local computer or a test controller"
* Open the LoadTest to run (For example MixedLoad.loadtest)
* Run the test

## 2.3 Cloud Run with Visual Studio Team Services
* Open the RSLoadTest.testsettings file (double click on the file) and select "Run Tests Using Visual Studio Team Services"
* Detailed instructions on https://www.visualstudio.com/en-us/docs/test/performance-testing/getting-started/getting-started-with-performance-testing , check the section ***Connect to your Visual Studio Team Services account*** for example ***https://ssrsload.visualstudio.com***

### 2.3.1 Viewing Cloud Load Test Run Results
* Open the load solution
* Connect to your Visual Studio Team Services account (for example ***https://ssrsload.visualstudio.com***)
* Open the Load Test Manager
    * Load Test > Load Test Manager
    * Double click in the result you want to see


# Create a SQL Server Reporting Services Load Environment in Azure 
You can use the UI

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FReporting-Services-LoadTest%2Fmaster%2FArmTemplate%2FSSRS-MultiMachine%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2FReporting-Services-LoadTest%2Fmaster%2FArmTemplate%2FSSRS-MultiMachine%2Fazuredeploy.json" target="_blank">
  <img src="http://armviz.io/visualizebutton.png"/>
</a>

or PowerShell

* Install Azure PowerShell (https://azure.microsoft.com/en-us/documentation/articles/powershell-install-configure/)
* Edit the \Reporting-Services-LoadTest\ArmTemplate\SSRS-MultiMachine\azuredeploy.parameters.json
    * Provide a unique ssrsDNSPrefix
    * Provide the passwords (this user and passwords need to be used on RSTest.Common.ReportServer.dll.Config)
    * Edit other parameter of the template such as VM Size
* Replace 'YOUR_RESOURCE_GROUP_NAME' to a unique resource group name and then run the deployment using the following command:
```powershell
PS C:\repos\Reporting-Services-LoadTest\ArmTemplate\SSRS-MultiMachine\> .\Deploy-AzureResourceGroup.ps1 -ResourceGroupName 'YOUR_RESOURCE_GROUP_NAME' -ResourceGroupLocation 'westus2' -TemplateFile azuredeploy.json -TemplateParametersFile azuredeploy.parameters.json
```            
* Remote desktop into the Azure RS machine named SSRS-RS (using username and password specified in azuredeploy.parameters.json)
* Configure SQL Server Reporting Services and ensure is running
* In case you need to do advanced troubleshooting configure the service to take Full Dumps rsreportserver.config
            <Add Key="WatsonFlags" Value="0x0430" />      
* Increase the number of MaxActiveReqForOneUser to at least 800 in rsreportserver.config (but this really depends on your load test configuration)
            <Add Key="MaxActiveReqForOneUser" Value="800" />       

The deployment will take around 30 minutes. It sets up a Domain Controller, a RS Server, a SQL Server with Catalog DB and another SQL Server with a set o fdatabases that the tests uses.      


# Tutorials
[How to onboard a new Paginated Reports Scenario](../master/docs/OnboardPaginated.md)

[How to onboard a new Mobile Reports Scenario](../master/docs/OnboardMobile.md)

# Advanced Configuration

### Reports and LoadTest (.loadtest)
The LoadTest files contains a set of scenarios that will drive the load in the system, those are standard Visual Studio Load Test files and the details of the different settings can be found on [Editing Load Test Using the Load Test Editor](https://msdn.microsoft.com/en-us/library/ff406975(v=vs.140).aspx)

However the Load tests also deploy a set of Reports, Data sources, Mobile Reports and KPIs during the initialization, those resources are stored under Reporting-Services-LoadTest\src\RSLoad\ContentManager\RuntimeResources
The deployment is based on the name of the scenario , for example in MixedLoad.loadtest there are the following scenarios

|Scenario|Files to Deploy to the Server|
|-------|-----------|
|MobileTest|RuntimeResources\MobileTest|
|Portal|RuntimeResources\Portal|
|Paginated_Large|RuntimeResources\Paginated\Large|
|Paginated_Small|RuntimeResources\Paginated\Small|

The load test will create a folder in the Server with the scenario name and will deploy the Reporting Services items that are required by the tests.
Each scenario requires a folder with the Reporting Services items to deploy.
In every folder a set of shared datasources will be created (defined in Reporting-Services-LoadTest\src\RSLoad\ContentManager\DataSources.xml)

### Configuration Files
* DataSources.xml : Define the datasources that will be created in the server during the test initialization (Located on Reporting-Services-LoadTest\src\RSLoad\ContentManager\DataSources.xml)
* Paginated Reports Only 
  * ScaleReportsWeight.xml: Specifies how often a report will be used during the test execution (Located on Reporting-Services-LoadTest\src\RSLoad\ContentManager\Paginated\ScaleReportsWeight.xml)
  * BadCombinations.xml:  Specifies what combinatios of tests and reports shouldn't be used (Located on Reporting-Services-LoadTest\src\RSLoad\ContentManager\Paginated\BadCombinations.xml)
