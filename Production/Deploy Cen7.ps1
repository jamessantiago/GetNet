$session = New-PSSession HKMDOPS01
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe stop site GetNet}
cp 'F:\Deploy\GetNet\Centos 7 x64\' \\Devops\d$\Sites -Recurse -Force -Verbose
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe start site GetNet}
Remove-PSSession $session