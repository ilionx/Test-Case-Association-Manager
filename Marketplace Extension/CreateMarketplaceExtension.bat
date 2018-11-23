@ECHO OFF
ECHO This script creates a Microsoft Marketplace Task extension.
ECHO.

:DependencyChecks
ECHO [Stage] Dependency checks...
ECHO | set /p=* Node - 
WHERE node >nul 2>&1 && ECHO Installed || GOTO NodeNotInstalled
ECHO | set /p=* Dotnet - 
WHERE dotnet >nul 2>&1 && ECHO Installed || GOTO DotnetNotInstalled
ECHO | set /p=* Tfx-cli - 
WHERE tfx >nul 2>&1 && ECHO Installed || GOTO TfxNotInstalled
ECHO | set /p=* Robocopy - 
WHERE robocopy >nul 2>&1 && ECHO Installed || GOTO RobocopyNotInstalled
ECHO | set /p=* Powershell - 
WHERE powershell >nul 2>&1 && ECHO Installed || GOTO PowershellNotInstalled

GOTO Installed

:NodeNotInstalled
ECHO  Not installed (URL: "https://nodejs.org/en/download/")
GOTO End

:TfxNotInstalled
ECHO  Not installed (command: "npm install -g tfx-cli")
GOTO End

:RobocopyNotInstalled
ECHO  Not installed (URL: https://www.microsoft.com/en-us/download/details.aspx?id=17657)
GOTO End

:PowershellNotInstalled
ECHO  Not installed (URL: https://www.microsoft.com/en-us/download/details.aspx?id=17657)
GOTO End

:DotnetNotInstalled
ECHO  Not installed (URL: https://www.microsoft.com/net/download)
GOTO End


:Installed
:CopyWorkFiles
ECHO.
ECHO [Stage] Copying Work files to Temporary folder...
ROBOCOPY "%cd%" "%cd%\temp" /is /e /xd "%cd%\output" /xd "%cd%\temp" /xd "%cd%\associatetooltask\tool" /xf "CreateMarketplaceExtension.bat" /xf "PatchVersionNumber.ps1" /xf "PublishExtension.ps1" /xf "BuildTool.ps1" /NJH /NJS

:RebuildSolution
ECHO.
ECHO [Stage] Rebuilding Solution...
dotnet build ".\..\AssociateTestsToTestCases\AssociateTestsToTestCases.csproj" -c Release

:CopyToolFiles
ECHO.
ECHO | set /p=[Stage] Copying files...
ROBOCOPY "%cd%\..\AssociateTestsToTestCases\bin\Release\net461" "%cd%\temp\associatetooltask\tool" /is /xf "*.pdb" /NJH /NJS

:PatchVersion
ECHO.
ECHO [Stage] Patching version number...
POWERSHELL -noprofile -executionpolicy remotesigned -File "%cd%\PatchVersionNumber.ps1"

:CreatePackage
ECHO.
ECHO [Stage] Creating Package...
CHDIR /d "%cd%\temp"
CMD /c TFX extension create --manifest-globs "vss-extension.json" --output-path "%cd%\..\output"
CHDIR /d "%cd%\.."

:PublishPrompt
ECHO.
ECHO [Stage] Publishing Package...
SET /p publish=Do you want to publish the package? (y/n) 
ECHO %publish%

IF "%publish%" == "y" GOTO publishYes
IF "%publish%" == "n" GOTO publishNo
ECHO Wrong input. Please try again.
GOTO PublishPrompt

:publishYes
POWERSHELL -noprofile -executionpolicy remotesigned -File "%cd%\PublishExtension.ps1"
GOTO End

:publishNo
%SystemRoot%\explorer "%cd%\output"
GOTO End



:End
ECHO.
ECHO [Stage] End of Program...
RMDIR /q /s "%cd%\temp"
PAUSE