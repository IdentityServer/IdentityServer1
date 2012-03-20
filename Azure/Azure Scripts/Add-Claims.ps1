<#
.SYNOPSIS
   Adds claims to an IdentityServer table storage user
.DESCRIPTION
   Adds claims to an IdentityServer table storage user
.PARAMETER claims
   One or multiple claim type/value pairs separated by % (see examples)
.PARAMETER username
   Logon name of the user
.PARAMETER connectionString
   Connection string to table storage
.EXAMPLE
	Add-Claims -username alice -connectionString 'connectionstring to storage' -claims claimtype1%value1, claimtype2%value2
#>

[CmdletBinding()]
param(
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[string]$username,
	
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[string[]]$claims,
	
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[string]$connectionString
)

begin {
	$wif = $env:ProgramFiles + "\Reference Assemblies\Microsoft\Windows Identity Foundation\v3.5\Microsoft.IdentityModel.dll"
	
	Add-Type -Path "Thinktecture.IdentityServer.WindowsAzure.dll"
	Add-Type -Path $wif
}

process {
	
	$claimsList = New-Object System.Collections.Generic.List[Microsoft.IdentityModel.Claims.Claim]
	
	foreach ($claim in $claims) {
		$items = $claim.split("%")
		$wifClaim = New-Object Microsoft.IdentityModel.Claims.Claim -ArgumentList $items[0], $items[1]
		
		$claimsList.Add($wifClaim)
	}
	
	try {
		$context = New-Object Thinktecture.IdentityServer.Repositories.WindowsAzure.TableStorageContext -arg $connectionString
		$context.AddUserClaims($username, $claimsList)
	}
	catch {
		Write-Error $_.Exception.ToString()
	}
}