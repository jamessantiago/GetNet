﻿[cmdletbinding(SupportsShouldProcess=$true)]
param($publishProperties=@{}, $packOutput, $pubProfilePath)

# to learn more about this file visit https://go.microsoft.com/fwlink/?LinkId=524327

try{
    if ($publishProperties['ProjectGuid'] -eq $null){
        $publishProperties['ProjectGuid'] = '612da792-54c3-4fe1-a20a-a312746defd6'
    }

    $publishModulePath = Join-Path (Split-Path $MyInvocation.MyCommand.Path) 'publish-module.psm1'
    Import-Module $publishModulePath -DisableNameChecking -Force

	$getnetModulePath = Join-Path (Split-Path $MyInvocation.MyCommand.Path) 'GetNet-module.psm1'
    Import-Module $getnetModulePath -DisableNameChecking -Force

    # call Publish-AspNet to perform the publish operation
    Publish-AspNet -publishProperties $publishProperties -packOutput $packOutput -pubProfilePath $pubProfilePath

	#Publish-MaterialIcons $publishProperties["publishUrl"]

	#Remove-Configs "Windows 10 Server 2016"
}
catch{
    "An error occurred during publish.`n{0}" -f $_.Exception.Message | Write-Error
}