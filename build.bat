@echo off
echo Building NanCo Terminal...
echo ========================

:: Set colors
color 0A

:: Create build directories
if not exist "build" mkdir build
if not exist "build\release" mkdir build\release
if not exist "build\package" mkdir build\package

:: Clean previous builds
echo Cleaning previous builds...
if exist "build\release\*" del /F /Q "build\release\*"
if exist "build\package\*" del /F /Q "build\package\*"
if exist "build\NanCo.zip" del /F /Q "build\NanCo.zip"

:: Build the project
echo Building project...
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true -o build\release

:: Create directory structure for package
echo Creating package structure...
mkdir "build\package\NanCo"
mkdir "build\package\NanCo\Config"
mkdir "build\package\NanCo\Media"
mkdir "build\package\NanCo\Media\Sounds"
mkdir "build\package\NanCo\Media\Images"

:: Copy files to package
echo Copying files to package...
copy "build\release\NanCo.exe" "build\package\NanCo\"
copy "Config\*.*" "build\package\NanCo\Config\"
copy "Media\Sounds\*.*" "build\package\NanCo\Media\Sounds\"
copy "Media\Images\*.*" "build\package\NanCo\Media\Images\"

:: Create version file
echo 1.0.0-alpha > "build\package\NanCo\Config\version.txt"

:: Create README
echo Creating README...
(
echo NanCo Terminal
echo ==============
echo.
echo Installation:
echo 1. Extract all files
echo 2. Run NanCo.exe
echo.
echo Default Login:
echo Username: admin
echo Password: password
echo.
echo Please change the default password after first login!
) > "build\package\NanCo\README.txt"

:: Create ZIP file
echo Creating ZIP package...
powershell Compress-Archive -Path "build\package\NanCo\*" -DestinationPath "build\NanCo.zip" -Force

:: Check if build was successful
if exist "build\release\NanCo.exe" (
    echo.
    echo Build completed successfully!
    echo.
    echo Files created:
    echo - Executable: build\release\NanCo.exe
    echo - Package: build\NanCo.zip
    echo.
    echo You can:
    echo 1. Run the executable directly from build\release\NanCo.exe
    echo 2. Share the package at build\NanCo.zip
) else (
    echo.
    echo Build failed! Please check the error messages above.
)

:: Pause to show results
pause 