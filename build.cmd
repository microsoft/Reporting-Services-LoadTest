@powershell -NoProfile -ExecutionPolicy unrestricted -Command "((new-object net.webclient).DownloadFile('https://nuget.org/nuget.exe', 'NuGet.exe'))"
nuget restore
msbuild /verbosity:minimal /target:rebuild /p:"Platform=Any CPU"