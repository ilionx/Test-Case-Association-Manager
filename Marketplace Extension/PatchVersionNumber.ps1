# Retrieve latest version through tfx
$PublishedExtensionRAW = TFX extension show -t tiqlnh34bkjj3emcjtlsd24wfw35d2y2vftopeinvnjpeijeauha --publisher TestMcTester --extension-id AssociateToolTask
$PublishedExtensionJson = $PublishedExtensionRAW[2..$PublishedExtensionRAW.Count] | ConvertFrom-Json
$PublishedExtensionLatestVersion = $PublishedExtensionJson.Versions[0].Version

Write-Host "Latest version: $($PublishedExtensionLatestVersion)"

$PublishedExtensionLatestVersionSplit = $PublishedExtensionLatestVersion.Split(".")
$PublishedVersion = @(0,0,0)
For($a=0; $a -lt $PublishedVersion.Length; $a++) {
  $PublishedVersion[$a] = [int]$PublishedExtensionLatestVersionSplit[$a]
}

# Retrieve version input from user
$VersionTypes = @('Major','Minor','Patch')
$lowerCaseVersionTypes = $VersionTypes | % {$_.ToLower() }

while ($true) {
  $InputVersion = @(0,0,0)

  while($true) {    
     $InputVersionType = Read-Host -Prompt "Please insert the type of update ( $($VersionTypes[0]) / $($VersionTypes[1]) / $($VersionTypes[2]) )"
     if ($lowerCaseVersionTypes -contains $InputVersionType.ToString().ToLowerInvariant()) { break }

     Write-Host "--> Wrong Input. Please try again."
  }

  $ConfirmInputVersion = ""
  while ($true) {
      $positionInputVersionType = $lowerCaseVersionTypes.IndexOf($InputVersionType.ToString().ToLowerInvariant())
      $InputVersion = $PublishedVersion.Clone()
      $InputVersion[$positionInputVersionType] += 1
      
      $x = $positionInputVersionType + 1
      For($x; $x -lt $InputVersion.Length; $x++) { $InputVersion[$x] = 0 }

      $ConfirmInputVersion = Read-Host -Prompt "You have selected $($VersionTypes[$positionInputVersionType]) update ($($InputVersion[0]).$($InputVersion[1]).$($InputVersion[2])). Is this correct? (y/n)"
      if ($ConfirmInputVersion -eq 'y' -or $ConfirmInputVersion -eq 'n') { break }

      Write-Host "--> Wrong input. Please try again."
  }

  if ($ConfirmInputVersion -eq "y") { break }
}

# Open & Write Json files
$VssExtensionJsonPath = '.\temp\vss-extension.json'
$VssExtensionJson = Get-Content -Raw -Path $VssExtensionJsonPath | ConvertFrom-Json
$VssExtensionJson.Version =  [string]::Format("{0}.{1}.{2}", $InputVersion[0], $InputVersion[1], $InputVersion[2])
$VssExtensionJson | ConvertTo-Json -Depth 10 | Set-Content $VssExtensionJsonPath

$TaskJsonPath = '.\temp\associatetooltask\task.json'
$TaskJson = Get-Content -Raw -Path $TaskJsonPath | ConvertFrom-Json
$TaskJson.Version.Major = $InputVersion[0]
$TaskJson.Version.Minor = $InputVersion[1]
$TaskJson.Version.Patch = $InputVersion[2]
$TaskJson | ConvertTo-Json -Depth 10 | Set-Content $TaskJsonPath