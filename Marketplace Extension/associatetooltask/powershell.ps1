Trace-VstsEnteringInvocation $MyInvocation
function ExecuteAssociationTool
{
  # Tool exe path
  $ToolExePath = "$PSScriptRoot\tool\AssociateTestsToTestCases.exe"
  
  # Get Vsts Input
  $SourceFolder = Get-VstsInput -Name sourceFolder
  $MinimatchPatterns = Get-VstsInput -Name minimatchPatterns
  $CollectionUri = Get-VstsInput -Name collectionUri
  $PersonalAccesstoken = Get-VstsInput -Name personalAccesstoken
  $ProjectName = Get-VstsInput -Name projectName
  $TestPlanId = Get-VstsInput -Name testPlanId
  $TestSuiteId = Get-VstsInput -Name testSuiteId
  $TestType = Get-VstsInput -Name testType
  
  $ValidateOnly = Get-VstsInput -Name validateOnly -AsBool
  $VerboseLogging = Get-VstsInput -Name verboseLogging -AsBool
  $DebugMode = Get-VstsInput -Name debugMode -AsBool
  
  # Arguments
  $arguments = New-Object System.Collections.ArrayList;
  $arguments.Add("-d ""$SourceFolder""") > $null
  $arguments.Add("-m ""$MinimatchPatterns""") > $null
  $arguments.Add("-p ""$PersonalAccesstoken""") > $null
  $arguments.Add("-u ""$CollectionUri""") > $null
  $arguments.Add("-n ""$ProjectName""") > $null
  $arguments.Add("-e ""$TestPlanId""") > $null
  $arguments.Add("-s ""$TestSuiteId""") > $null
  $arguments.Add("-t ""$TestType""") > $null
  
  if ($ValidateOnly) { $arguments.Add("-v") > $null }
  if ($VerboseLogging) { $arguments.Add("-l") > $null }
  if ($DebugMode) { $arguments.Add("-x") > $null }
   
  # Process creation & execution
  $process = Start-Process -FilePath $ToolExePath -ArgumentList $arguments -NoNewWindow -PassThru -Wait
  
  if ($process.ExitCode -ne 0)
  {
    Write-Host "##vso[task.setvariable variable=agent.jobstatus;]failed"
    Write-Host "##vso[task.complete result=Failed;]DONE"
  }
}

# Main
ExecuteAssociationTool

Trace-VstsLeavingInvocation $MyInvocation