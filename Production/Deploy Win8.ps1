$session = New-PSSession HKMDOPS01
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe stop site GetNet}
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe stop apppool GetNet}
Invoke-Command -Session $session {stop-service GetNetservice}
cp 'F:\Deploy\GetNet\Windows 81 Server 2012R2\*' \\Devops\d$\Sites -Recurse -Force -Verbose
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe start site GetNet}
Invoke-Command -Session $session {C:\Windows\System32\inetsrv\appcmd.exe start apppool GetNet}
Invoke-Command -Session $session {start-service GetNetservice}
Remove-PSSession $session