@echo off
echo Setting up NanCo Development Environment...
echo ========================================

:: Set colors
color 0A

:: Define root directory
set NANCO_ROOT=H:\Documents\Coding\NanCo

:: Create main directory structure
echo Creating directory structure...
if not exist "%NANCO_ROOT%" mkdir "%NANCO_ROOT%"
if not exist "%NANCO_ROOT%\Config" mkdir "%NANCO_ROOT%\Config"
if not exist "%NANCO_ROOT%\Media" mkdir "%NANCO_ROOT%\Media"
if not exist "%NANCO_ROOT%\Media\Sounds" mkdir "%NANCO_ROOT%\Media\Sounds"
if not exist "%NANCO_ROOT%\Media\Images" mkdir "%NANCO_ROOT%\Media\Images"
if not exist "%NANCO_ROOT%\Scripts" mkdir "%NANCO_ROOT%\Scripts"
if not exist "%NANCO_ROOT%\Projects" mkdir "%NANCO_ROOT%\Projects"

:: Create version file
echo Creating configuration files...
echo 1.0.0-alpha > "%NANCO_ROOT%\Config\version.txt"

:: Create initial users file (if it doesn't exist)
if not exist "%NANCO_ROOT%\Config\users.dat" (
    echo admin:password > "%NANCO_ROOT%\Config\users.dat"
)

:: Create run script
echo Creating run script...
(
echo @echo off
echo echo Starting NanCo Terminal...
echo dotnet run --project "%NANCO_ROOT%"
echo pause
) > "%NANCO_ROOT%\run.bat"

:: Create debug script
echo Creating debug script...
(
echo @echo off
echo echo Starting NanCo Terminal in Debug Mode...
echo dotnet run --project "%NANCO_ROOT%" --configuration Debug
echo pause
) > "%NANCO_ROOT%\debug.bat"

:: Create clean script
echo Creating clean script...
(
echo @echo off
echo echo Cleaning NanCo build files...
echo if exist "bin" rmdir /S /Q bin
echo if exist "obj" rmdir /S /Q obj
echo if exist "build" rmdir /S /Q build
echo echo Cleaned successfully!
echo pause
) > "%NANCO_ROOT%\clean.bat"

:: Copy build script if it doesn't exist
if not exist "%NANCO_ROOT%\build.bat" (
    copy "%~dp0build.bat" "%NANCO_ROOT%\build.bat"
)

:: Create README
echo Creating documentation...
(
echo # NanCo Terminal
echo.
echo ## Development Environment Setup
echo.
echo ### Scripts:
echo - build.bat - Builds the project
echo - run.bat - Runs the project
echo - debug.bat - Runs the project in debug mode
echo - clean.bat - Cleans build artifacts
echo.
echo ### Directory Structure:
echo - Config/ - Configuration files
echo - Media/ - Media resources
echo   - Sounds/ - Audio files
echo   - Images/ - Image files
echo - Scripts/ - Utility scripts
echo - Projects/ - User projects
echo.
echo ### Default Credentials:
echo Username: admin
echo Password: password
) > "%NANCO_ROOT%\README.md"

echo.
echo Setup completed successfully!
echo.
echo Directory structure created at: %NANCO_ROOT%
echo.
echo Files created:
echo - Configuration files in Config/
echo - Run scripts in root directory
echo - README.md with documentation
echo.
echo Default credentials:
echo Username: admin
echo Password: password
echo.
echo Please change the default password after first login!
echo.
pause 