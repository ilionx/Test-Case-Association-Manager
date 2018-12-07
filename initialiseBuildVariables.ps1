Write-Host "Retrieving build variables..."
[String]$versionNumberPatch = $buildNumber.Substring($buildNumber.LastIndexOf('.') + 1)
[String]$buildNumber = $Env:BUILD_BUILDNUMBER
Write-Host "Success! Build variables have been retrieved."

Write-Host "Initialising build variables..."
Write-Host "##vso[task.setvariable variable=VersionNumberPatch;]$($versionNumberPatch)"
Write-Host "##vso[task.setvariable variable=VersionNumberFull;]$($buildNumber)"
Write-Host "Success! Build variables have been initialised."