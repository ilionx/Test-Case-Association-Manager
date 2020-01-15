Write-Host "Retrieving build variables..."
[String]$versionNumberMajor = $Env:VersionNumberMajor
[String]$versionNumberMinor = $Env:VersionNumberMinor
[String]$buildNumber = $Env:BUILD_BUILDNUMBER
[String]$versionNumberPatch = $buildNumber.Substring($buildNumber.LastIndexOf('_') + 1) -replace "[.]"
[String]$versionNumberFull =  "$($versionNumberMajor).$($versionNumberMinor).$($versionNumberPatch)"
Write-Host "Success! Build variables have been retrieved."

Write-Host "Initialising build variables..."
Write-Host "##vso[task.setvariable variable=VersionNumberPatch;]$($versionNumberPatch)"
Write-Host "##vso[task.setvariable variable=VersionNumberFull;]$($versionNumberFull)"
Write-Host "Success! Build variables have been initialised."