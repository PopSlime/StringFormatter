@echo off
pushd "%~dp0"
dotnet build StringFormatter.sln --configuration Release
popd
pause
