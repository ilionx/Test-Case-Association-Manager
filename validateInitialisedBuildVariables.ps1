# Functions
function RetrieveBuildVariables
{
  Write-Host "Retrieving build variables..."
  $global:versionNumberMajor = [String]$Env:VersionNumberMajor
  $global:versionNumberMinor = [String]$Env:VersionNumberMinor
  $global:versionNumberPatch = [String]$Env:VersionNumberPatch
  $global:versionNumberFull = [String]$Env:VersionNumberFull
  $global:ExtensionVersionNumberFull = [String]$Env:Extension_Version
  
  $global:ExtensionVersionNumberMajor = "0"
  $global:ExtensionVersionNumberMinor = "0"
  $global:ExtensionVersionNumberPatch = "0"
  if($ExtensionVersionNumberFull  -ne "")
  {
    $ExtensionVersionArray = $ExtensionVersionNumberFull.Split(".")
    $global:ExtensionVersionNumberMajor = $ExtensionVersionArray[0]
    $global:ExtensionVersionNumberMinor = $ExtensionVersionArray[1]
    $global:ExtensionVersionNumberPatch = $ExtensionVersionArray[2]
  }
  Write-Host "Success! Build variables have been retrieved."
}

function HasNoneEmptyBuildVariablesVersionNumber
{
  param([bool] $operationSuccess)

  $emptyVariables = New-Object System.Collections.ArrayList
  if ($versionNumberMajor -eq "") { $emptyVariables.add("VersionNumberMajor") }
  if ($versionNumberMinor -eq "") { $emptyVariables.add("VersionNumberMinor") }
  if ($versionNumberPatch -eq "") { $emptyVariables.add("VersionNumberPatch") }
  if ($versionNumberFull -eq "")  { $emptyVariables.add("VersionNumberFull")  }
  
  if ($emptyVariables.Count -ne 0) 
  {
    $operationSuccess = $false
    ForEach ($emptyVariable in $emptyVariables)
    {
      Write-Host "##vso[task.logissue type=error;] Build variable '$($emptyVariable)' has not been initialised correctly by the previous task."
    }
  }
  return $operationSuccess
}

function IsGreaterThanPublishedVersion
{
  param($operationSuccess)

  $validateVersionNumberFull = "$($versionNumberMajor).$($versionNumberMinor).$($versionNumberPatch)"
  if($versionNumberFull -ne $validateVersionNumberFull)
  {
    Write-Host "##[error] Build variable 'versionNumberFull' ($($versionNumberFull)) differs from versionNumber Build variables ($($versionNumberMajor).$($versionNumberMinor).$($versionNumberPatch))."
    $operationSuccess = $false
  }
  elseif ($versionNumberFull -eq $ExtensionVersionNumberFull)
  {
    Write-Host "##[error] Build version ($($versionNumberFull)) shouldn't be equal to Published version ($($ExtensionVersionNumberFull))."
    $operationSuccess = $false
  }
  else
  {
    $isGreaterExtensionVersionNumberMajor = [Int]$ExtensionVersionNumberMajor -gt [Int]$versionNumberMajor
    $isGreaterExtensionVersionNumberMinor = [Int]$ExtensionVersionNumberMinor -gt [Int]$versionNumberMinor
    $isGreaterExtensionVersionNumberPatch = [Int]$ExtensionVersionNumberPatch -gt [Int]$versionNumberPatch

    $isEqualExtensionVersionNumberMajor = [Int]$ExtensionVersionNumberMajor -eq [Int]$versionNumberMajor
    $isEqualExtensionVersionNumberMinor = [Int]$ExtensionVersionNumberMinor -eq [Int]$versionNumberMinor

    if(($isGreaterExtensionVersionNumberMajor -eq $true)) { $operationSuccess = $false }
    elseif(($isEqualExtensionVersionNumberMajor -eq $true) -and ($isGreaterExtensionVersionNumberMinor -eq $true)) { $operationSuccess = $false }
    elseif(($isEqualExtensionVersionNumberMajor -eq $true) -and ($isEqualExtensionVersionNumberMinor -eq $true) -and ($isGreaterExtensionVersionNumberPatch -eq $true)) { $operationSuccess = $false }
  
    if ($operationSuccess -eq $false)
    {
      Write-Host "##[error] Build version ($($versionNumberFull)) should be greater than the Published version ($($ExtensionVersionNumberFull))."
    }
  }

  return $operationSuccess
}

function IsValidBuildVariables
{
  param([bool]$operationSuccess)

  Write-Host "Validating build variables..."
  $operationSuccess = HasNoneEmptyBuildVariablesVersionNumber $operationSuccess
  
  if ($operationSuccess -ne $false)
  {
    $operationSuccess = IsGreaterThanPublishedVersion $operationSuccess
  }
  return $operationSuccess
}

function OutputTaskOutcome
{
  param([bool] $isSuccessInitialisation)
  if ($isSuccessInitialisation -eq $false)
  {
    Write-Host "##vso[task.setvariable variable=agent.jobstatus;]failed"
    Write-Host "##vso[task.complete result=Failed;]DONE"
  }
  else
  {
    Write-Host "Success! Build variables have been initialised correctly by the previous task." 
  }
}

# Executions
RetrieveBuildVariables
$isSuccess = IsValidBuildVariables $true
OutputTaskOutcome $isSuccess