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

function GetNet-Build {
	[cmdletbinding()]
	param(
		[Parameter(Position=0, Mandatory=$true)]
		$publishProperties
	)
	process {
		dotnet build --configuraiton $publishProperties["LastUsedBuildConfiguration"] --runtime $publishProperties["PublishRuntime"] 
	}
}

function Copy-Last {
	[cmdletbinding()]
	param(
		[Parameter(Position=0, Mandatory=$true)]
		$path
	)
	process {
		if (Test-Path $path) {
			cp $path\* "$path\..\$(split-path $path -Leaf)-last" -Recurse
		} else {
			mkdir "$path\..\$(split-path $path -Leaf)-last"
		}
	}
}

function GetNet-Diff {
	[cmdletbinding()]
	param(
		[Parameter(Position=0, Mandatory=$true)]
		$path
	)
	process {
		$last = "$path\..\$(split-path $path -Leaf)-last"
		$patch = "$path\..\$(split-path $path -Leaf)-patch"
		rm $patch -Recurse
		robocopy $path $patch /e /xf *.* | Out-Null

		ls $path |% {
			$oldFile = $_.FullName.Replace($path, $last);
			if (!(test-path $oldFile) -or $(ls $oldFile).lastwritetime -lt $_.LastWriteTime)
			{
				$patchFile = $_.Fullname.Replace($path, $patch)
				$patchFile
				cp $_.Fullname $patchFile
			}
		}
	}
}