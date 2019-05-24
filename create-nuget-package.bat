@echo off
pushd "%~dp0"
dotnet pack StringFormatter.sln --configuration Release --output "%~dp0\output"
popd
pause
