@ECHO OFF
IF /I "%1"=="Debug" GOTO BuildDebug
IF /I "%1"=="Release" GOTO BuildRelease
IF /I "%1"=="CoreDebugDeployment" GOTO BuildCoreDbg
IF /I "%1"=="CoreReleaseDeployment" GOTO BuildCoreRls

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
ECHO     build.bat CoreDebugDeployment
ECHO         Builds the Apworks with CoreDebugDeployment configuration.
ECHO.
ECHO     build.bat CoreReleaseDeployment
ECHO         Builds the Apworks with CoreReleaseDeployment configuration.
ECHO.
GOTO End

:BuildDebug
msbuild /p:Configuration=Debug;TargetFrameworkVersion=v4.0 Apworks.sln
GOTO End

:BuildRelease
msbuild /p:Configuration=Release;TargetFrameworkVersion=v4.0 Apworks.sln
GOTO End

:BuildCoreDbg
msbuild /p:Configuration=CoreDebugDeployment;TargetFrameworkVersion=v4.0 Apworks.sln
GOTO End

:BuildCoreRls
msbuild /p:Configuration=CoreReleaseDeployment;TargetFrameworkVersion=v4.0 Apworks.sln
GOTO End

:End
@ECHO ON
