Write-Host "Retrieving build variables..."
[String]$buildNumber = $Env:BUILD_BUILDNUMBER
[String]$versionNumberPatch = $buildNumber.Substring($buildNumber.LastIndexOf('.') + 1)
Write-Host "Success! Build variables have been retrieved."

Write-Host "Initialising build variables..."
Write-Host "##vso[task.setvariable variable=VersionNumberFull;]$($buildNumber)"
Write-Host "##vso[task.setvariable variable=VersionNumberPatch;]$($versionNumberPatch)"
Write-Host "Success! Build variables have been initialised."