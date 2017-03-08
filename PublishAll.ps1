$current = $PWD.Path
$base = Split-Path -parent $PSCommandPath
$profiles = @("Windows 10 Server 2016", "Windows 81 Server 2012R2", "Centos 7 x64")
$projects = @("$base\src\getnet.web\getnet.web.csproj", "$base\src\getnet.service\getnet.service.csproj")
$total = $profiles.Count + $projects.Count
$count = 1

foreach ($project in $projects) {
	cd $(split-path $project)
    foreach ($profile in $profiles) {
		Write-Progress -Activity "$project" -Status "Deploying" -PercentComplete $($count/$total * 100)
        & 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe' $project /p:DeployOnBuild=true /p:PublishProfile="$profile"
		$count++
    }
}
cd $current