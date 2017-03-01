$runtimes = @("win81-x64","win10-x64","centos.7-x64")
$current = $PWD.Path
$base = Split-Path -parent $PSCommandPath
$projectPaths = @("$base\src\getnet.web", "$base\src\getnet.service")
$publishProfiles = @("$base\src\getnet.web\getnet.web.xproj", "$base\src\getnet.service\getnet.service.xproj")
$total = $runtimes.Count * $projectPaths.Count
$count = 1;

$projectPaths |% {
	cd $_
    dotnet bump patch
	$runtimes |% {
		Write-Progress -Activity "$($pwd.path)> $_" -Status "Building" -PercentComplete $($count/$total * 100)
		dotnet build -r $_ -c Release
		$count++
	}
}
cd $current