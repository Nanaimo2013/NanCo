@echo off
setlocal

:: Set NanCo environment variables
set NANCO_HOME=%~dp0
set NANCO_PROJECTS=%NANCO_HOME%Projects
set NANCO_SCRIPTS=%NANCO_HOME%Scripts
set PATH=%NANCO_HOME%bin;%PATH%

:: Create necessary directories if they don't exist
if not exist "%NANCO_PROJECTS%" mkdir "%NANCO_PROJECTS%"
if not exist "%NANCO_SCRIPTS%" mkdir "%NANCO_SCRIPTS%"
if not exist "%NANCO_HOME%Media\Images" mkdir "%NANCO_HOME%Media\Images"
if not exist "%NANCO_HOME%Media\Sounds" mkdir "%NANCO_HOME%Media\Sounds"

:: Start NanCo Terminal
echo Starting NanCo Terminal...
dotnet run --project "%NANCO_HOME%NanCo.csproj"

endlocal