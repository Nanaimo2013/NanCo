@echo off
setlocal enabledelayedexpansion

:: Set colors
color 0A

:: Check build mode
if "%1"=="test" (
    call :BuildTest
) else if "%1"=="release" (
    call :BuildRelease
) else (
    echo Usage:
    echo   build.bat test    - Build test version
    echo   build.bat release - Build release version
    exit /b 1
)
exit /b 0

:BuildTest
echo Building NanCo Test Version...
echo ============================

:: Create test build directory
if not exist "build\test" mkdir build\test

:: Clean previous test build
if exist "build\test\*" del /F /Q "build\test\*"

:: Build test version
dotnet publish -c Debug -r win-x64 --self-contained true /p:PublishSingleFile=true -o build\test

:: Copy version file with -test suffix
for /f "tokens=*" %%v in (Version.txt) do (
    echo %%v-test > "build\test\Version.txt"
)

echo Test build complete: build\test\NanCo.exe
exit /b 0

:BuildRelease
echo Building NanCo Release Version...
echo ==============================

:: Create release directories
if not exist "build\release" mkdir build\release
if not exist "build\package" mkdir build\package

:: Clean previous builds
if exist "build\release\*" del /F /Q "build\release\*"
if exist "build\package\*" del /F /Q "build\package\*"
if exist "build\NanCo.zip" del /F /Q "build\NanCo.zip"

:: Build release version
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true -o build\release

:: Create package structure
mkdir "build\package\NanCo"
mkdir "build\package\NanCo\Config"
mkdir "build\package\NanCo\Media"
mkdir "build\package\NanCo\Media\Sounds"
mkdir "build\package\NanCo\Media\Images"

:: Copy files
copy "build\release\NanCo.exe" "build\package\NanCo\"
copy "Config\*.*" "build\package\NanCo\Config\"
copy "Media\Sounds\*.*" "build\package\NanCo\Media\Sounds\"
copy "Media\Images\*.*" "build\package\NanCo\Media\Images\"
copy "Version.txt" "build\package\NanCo\"

:: Create ZIP package
powershell Compress-Archive -Path "build\package\NanCo\*" -DestinationPath "build\NanCo.zip" -Force

echo Release build complete:
echo - Executable: build\release\NanCo.exe
echo - Package: build\NanCo.zip
exit /b 0 