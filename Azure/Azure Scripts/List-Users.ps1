<#
.SYNOPSIS
   Lists all users from the IdentityServer table storage
.DESCRIPTION
   Lists all users from the IdentityServer table storage
.PARAMETER connectionString
   Connection string to table storage
.PARAMETER includeClaims
   Also show the claims for the corresponding users
#>

[CmdletBinding()]
param(
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[string]$connectionString,
	
	[Parameter(
		Mandatory=$False,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[switch]$includeClaims = $false
)

begin {
	Add-Type -Path "Thinktecture.IdentityServer.WindowsAzure.dll"
}

process {
	try {
		$context = New-Object Thinktecture.IdentityServer.Repositories.WindowsAzure.TableStorageContext -arg $connectionString
		$users = $context.GetUsers()
		
		foreach ($user in $users) {
			Write-Host $user
			
			if ($includeClaims) {
				$claims = $context.GetUserClaims($user)				
				Write-Host
				
				foreach ($claim in $claims) {
					Write-Host $claim.ClaimType
					Write-Host " " $claim.Value `n
				}
			}
		}
	}
	catch {
		Write-Error $_.Exception.ToString()
	}
}