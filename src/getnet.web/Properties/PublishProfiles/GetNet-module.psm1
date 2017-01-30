#
# GetNet_module.psm1
#
function Publish-MaterialIcons{
    [cmdletbinding()]
    param(
        [Parameter(Position=0, Mandatory=$true)]
        [string]$path
    )
    process{
		$mdPath = "D:\Code\getnet\media\material-design-icons"
		$publishPath = $(Join-Path $path "wwwroot\lib\material-design-icons")
		if (!(Test-Path $publishPath))
		{
			robocopy $mdPath $publishPath /e /xf *.* | Out-Null
		}

		"Adding $publishPath"
		$includeFonts = ls -Recurse -Include "*.cs", "*.cshtml" |% {cat $_} |? {$_ -match "([\w_]+)\</i\>"} |% {$matches[1]} | select -Unique |% {"*$_*.png","*$_*.svg"}
		$mdPathR = $mdPath -replace "\\", "\\" 
		$publishPathR = $publishPath -replace "\\", "\\"
		$(ls $mdPath -Recurse -include $includeFonts) + $(ls $mdPath -Recurse -Exclude "*.png", "*.svg")  |% {
			$newPath = join-path $($(Split-Path $_.FullName) -replace $mdPathR, $publishPathR) $_.Name
			cp $_.FullName  $newPath -Force
		}
	}
}