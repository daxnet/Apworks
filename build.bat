@ECHO OFF
IF /I "%1"=="Debug" GOTO BuildDebug
IF /I "%1"=="Release" GOTO BuildRelease

:ER
ECHO.
ECHO Apworks Command-Line Build Tool v1.0
ECHO.
ECHO Usage:
ECHO     build.bat Debug
ECHO         Builds the Apworks with Debug configuration.
ECHO.
ECHO     build.bat Release
ECHO         Builds the Apworks with Release configuration.
ECHO.
GOTO End

:BuildDebug
msbuild /p:Configuration=CoreDebug;TargetFrameworkVersion=v4.0 Apworks.sln
GOTO End

:BuildRelease
msbuild /p:Configuration=CoreRelease;TargetFrameworkVersion=v4.0 Apworks.sln
GOTO End

:End
@ECHO ON
