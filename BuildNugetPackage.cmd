copy LICENSE.md lib
copy README.md lib
del /F /Q /S *.CodeAnalysisLog.xml

".nuget\NuGet.exe" pack -sym ScribeSharp.nuspec -BasePath .\

copy ScribeSharp*.nupkg c:\Nuget.LocalRepository\
pause