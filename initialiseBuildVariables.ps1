Write-Host "Retrieving build variables..."
$Env:BUILD_BUILDNUMBER
[String]$versionNumberMajor = $Env:VersionNumberMajor
[String]$versionNumberMinor = $Env:VersionNumberMinor
[String]$buildNumber = $Env:BUILD_BUILDNUMBER
[String]$versionNumberPatch = $buildNumber.Substring($buildNumber.LastIndexOf('.') + 1)
[String]$versionNumberFull =  "$($versionNumberMajor).$($versionNumberMinor).$($versionNumberPatch)"
Write-Host "Success! Build variables have been retrieved."

Write-Host "Initialising build variables..."
Write-Host "##vso[task.setvariable variable=VersionNumberPatch;]$($versionNumberPatch)"
Write-Host "##vso[task.setvariable variable=VersionNumberFull;]$($versionNumberFull)"
Write-Host "Success! Build variables have been initialised."