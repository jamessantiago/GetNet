$current = $PWD.Path
$base = Split-Path -parent $PSCommandPath
$profiles = @("Windows 10 Server 2016", "Windows 81 Server 2012R2", "Centos 7 x64")
$projects = @("$base\src\getnet.web\getnet.web.xproj", "$base\src\getnet.service\getnet.service.xproj")
$total = $profiles.Count + $projects.Count
$count = 1

foreach ($project in $projects) {
	cd $(split-path $project)
    foreach ($profile in $profiles) {
		Write-Progress -Activity "$project" -Status "Deploying" -PercentComplete $($count/$total * 100)
        C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe $project /p:DeployOnBuild=true /p:PublishProfile="$profile"
		$count++
    }
}
cd $current