@echo off
cls
if not exist ".\.nuget" (
	mkdir ".\.nuget"
	attrib +h ".\.nuget" /s /d
)
SET "release="
SET "buildVersion="
FOR /F %%i IN (build/version.txt) DO IF NOT DEFINED release SET "release=%%i"
if defined APPVEYOR_BUILD_NUMBER (
	SET "buildVersion=build%APPVEYOR_BUILD_NUMBER%"
) else (
	FOR /F "skip=1" %%i IN (build/version.txt) DO IF NOT DEFINED buildVersion SET "buildVersion=%%i"
)

if not exist ".\.nuget\nuget.exe" powershell -Command "Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile .\.nuget\nuget.exe"
if not exist ".\packages\Fake" ".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
"packages\FAKE\tools\Fake.exe" build.fsx "%*" -ev release=%release% -ev buildVersion=%buildVersion%
exit /b %errorlevel%
