@echo off
pushd "%~dp0"
dotnet test StringFormatter.sln --configuration Release
popd
pause
