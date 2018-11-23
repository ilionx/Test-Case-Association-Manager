# Scan output folder: Get File names
$ExtensionFiles = Get-ChildItem -Path '.\output' -Filter *.vsix

# Get latest version file
$LatestExtensionFile = $ExtensionFiles[0]
For($a=0; $a -lt $ExtensionFiles.Count; $a++) {
  If ($ExtensionFiles[$a].CreationTimeUtc -gt $LatestExtensionFile.CreationTimeUtc) {
    $LatestExtensionFile = $ExtensionFiles[$a]
  }
}

Write-Host $LatestExtensionFile

# Set ExtensionName
$ExtensionPath = ".\output\$($LatestExtensionFile)"
TFX extension publish --vsix $ExtensionPath -t tiqlnh34bkjj3emcjtlsd24wfw35d2y2vftopeinvnjpeijeauha