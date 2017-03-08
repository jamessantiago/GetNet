$session = New-PSSession HKMDOPS01
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe stop site GetNet}
cp 'F:\Deploy\GetNet\Windows 10 Server 2016\' \\Devops\d$\Sites -Recurse -Force -Verbose
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe start site GetNet}
Remove-PSSession $session